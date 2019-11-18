using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph.Drawing
{
    class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        EditorWindow m_EditorWindow;
        GraphData m_Graph;
        GraphView m_GraphView;
        Texture2D m_Icon;
        VisualElement m_Target;

        public ShaderPort connectedPort { get; set; }
        public bool nodeNeedsRepositioning { get; set; }
        public MaterialSlot targetSlot { get; private set; }
        public Vector2 targetPosition { get; private set; }
        private const string k_HiddenFolderName = "Hidden";
        
        public VisualElement target
        {
            get => m_Target;
            set => m_Target = value;
        }

        public void Initialize(EditorWindow editorWindow, GraphData graph, GraphView graphView)
        {
            m_EditorWindow = editorWindow;
            m_Graph = graph;
            m_GraphView = graphView;

            // Transparent icon to trick search window into indenting items
            m_Icon = new Texture2D(1, 1);
            m_Icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            m_Icon.Apply();
        }

        void OnDestroy()
        {
            if (m_Icon != null)
            {
                DestroyImmediate(m_Icon);
                m_Icon = null;
            }
        }

        struct CreateEntry
        {
            public string[] title;
            public object obj;
            public int compatibleSlotId;
        }

        List<int> m_Ids;
        List<MaterialSlot> m_Slots = new List<MaterialSlot>();

        List<CreateEntry> GetEntryList()
        {
            var entries = new List<CreateEntry>();

            if(target is ContextView contextView)
            {
                foreach (var item in FieldRegistry.instance.descriptors)
                {
                    entries.Add(new CreateEntry()
                    {
                        title = new string[] { "Field Block", item.tag, item.name },
                        obj = item,
                    });
                }
                return entries;
            }
            
            TypeCache.TypeCollection contextCollection = TypeCache.GetTypesDerivedFrom<IContext>();
            foreach(var type in contextCollection)
            {
                var typeRef = new TypeRef<IContext>(type);
                entries.Add(new CreateEntry()
                {
                    title = new string[] { "Context", typeRef.instance.name },
                    obj = typeRef,
                });
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(AbstractMaterialNode)))
                        && type != typeof(PropertyNode)
                        && type != typeof(KeywordNode)
                        && type != typeof(SubGraphNode))
                    {
                        var attrs = type.GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            var node = (AbstractMaterialNode)Activator.CreateInstance(type);
                            var path = new string[]{"Node"}.Concat(attrs[0].title).ToArray();
                            AddEntries(node, path, entries);
                        }
                    }
                }
            }

            foreach (var guid in AssetDatabase.FindAssets(string.Format("t:{0}", typeof(SubGraphAsset))))
            {
                var asset = AssetDatabase.LoadAssetAtPath<SubGraphAsset>(AssetDatabase.GUIDToAssetPath(guid));
                var node = new SubGraphNode { asset = asset };
                var title = asset.path.Split('/').ToList();

                if (asset.descendents.Contains(m_Graph.assetGuid) || asset.assetGuid == m_Graph.assetGuid)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(asset.path))
                {
                    AddEntries(node, new string[] { "Node", asset.name }, entries);
                }

                else if (title[0] != k_HiddenFolderName)
                {
                    title.Add(asset.name);
                    AddEntries(node, new string[] {"Node"}.Concat(title).ToArray(), entries);
                }
            }

            foreach (var property in m_Graph.properties)
            {
                var node = new PropertyNode();
                node.owner = m_Graph;
                node.property = property;
                node.owner = null;
                AddEntries(node, new[] {"Node", "Properties", "Property: " + property.displayName }, entries);
            }
            foreach (var keyword in m_Graph.keywords)
            {
                var node = new KeywordNode();
                node.owner = m_Graph;
                node.keyword = keyword;
                node.owner = null;
                AddEntries(node, new[] {"Node", "Keywords", "Keyword: " + keyword.displayName }, entries);
            }

            return entries;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var createEntries = GetEntryList();

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            createEntries.Sort((entry1, entry2) =>
                {
                    for (var i = 0; i < entry1.title.Length; i++)
                    {
                        if (i >= entry2.title.Length)
                            return 1;
                        var value = entry1.title[i].CompareTo(entry2.title[i]);
                        if (value != 0)
                        {
                            // Make sure that leaves go before nodes
                            if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                                return entry1.title.Length < entry2.title.Length ? -1 : 1;
                            return value;
                        }
                    }
                    return 0;
                });

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create"), 0),
            };

            foreach (var nodeEntry in createEntries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    if (i >= groups.Count)
                    {
                        // The current group path matches a prefix of the current entry's group path, so we add the
                        // rest of the group path from the currrent entry.
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        // A prefix of the current group path matches a prefix of the current entry's group path,
                        // so we remove everyfrom from the point where it doesn't match anymore, and then add the rest
                        // of the group path from the current entry.
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                // Create new group entries as needed.
                // If we don't need to modify the group path, `createIndex` will be `int.MaxValue` and thus the loop won't run.
                for (var i = createIndex; i < nodeEntry.title.Length - 1; i++)
                {
                    var group = nodeEntry.title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(nodeEntry.title.Last(), m_Icon)) { level = nodeEntry.title.Length, userData = nodeEntry });
            }

            return tree;
        }

        void AddEntries(AbstractMaterialNode node, string[] title, List<CreateEntry> nodeEntries)
        {
            if (m_Graph.isSubGraph && !node.allowedInSubGraph)
                return;
            if (!m_Graph.isSubGraph && !node.allowedInMainGraph)
                return;
            if (connectedPort == null)
            {
                nodeEntries.Add(new CreateEntry
                {
                    obj = node,
                    title = title,
                    compatibleSlotId = -1
                });
                return;
            }

            var connectedSlot = connectedPort.slot;
            m_Slots.Clear();
            node.GetSlots(m_Slots);
            var hasSingleSlot = m_Slots.Count(s => s.isOutputSlot != connectedSlot.isOutputSlot) == 1;
            m_Slots.RemoveAll(slot =>
                {
                    var materialSlot = (MaterialSlot)slot;
                    return !materialSlot.IsCompatibleWith(connectedSlot);
                });

            m_Slots.RemoveAll(slot =>
                {
                    var materialSlot = (MaterialSlot)slot;
                    return !materialSlot.IsCompatibleStageWith(connectedSlot);
                });

            if (hasSingleSlot && m_Slots.Count == 1)
            {
                nodeEntries.Add(new CreateEntry
                {
                    obj = node,
                    title = title,
                    compatibleSlotId = m_Slots.First().id
                });
                return;
            }

            foreach (var slot in m_Slots)
            {
                var entryTitle = new string[title.Length];
                title.CopyTo(entryTitle, 0);
                entryTitle[entryTitle.Length - 1] += ": " + slot.displayName;
                nodeEntries.Add(new CreateEntry
                {
                    title = entryTitle,
                    obj = node,
                    compatibleSlotId = slot.id
                });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var createEntry = (CreateEntry)entry.userData;

            // Get Mouse Position
            var windowRoot = m_EditorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - m_EditorWindow.position.position);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            // BlockFieldData
            if(createEntry.obj is FieldDescriptor field)
            {
                if(!(target is ContextView contextView))
                    return false;

                m_Graph.owner.RegisterCompleteObjectUndo("Add " + field.name + " Block");
                contextView.data.blocks.Add(new FieldBlockData(field));
                return true;
            }

            // Context
            else if(createEntry.obj is TypeRef<IContext> contextType)
            {
                m_Graph.owner.RegisterCompleteObjectUndo($"Add {contextType.instance.name} Context");
                m_Graph.contexts.Add(new ContextData
                {
                    displayName = contextType.instance.name,
                    contextType = contextType,
                    inputPorts = contextType.instance.inputPorts,
                    outputPorts = contextType.instance.outputPorts,
                    position = graphMousePosition,
                });
                return true;
            }

            // AbstractMaterialNode
            else if(createEntry.obj is AbstractMaterialNode node)
            {
                var drawState = node.drawState;
                drawState.position = new Rect(graphMousePosition, Vector2.zero);
                node.drawState = drawState;

                m_Graph.owner.RegisterCompleteObjectUndo("Add " + node.name);
                m_Graph.AddNode(node);

                if (connectedPort != null)
                {
                    var connectedSlot = connectedPort.slot;
                    var compatibleSlot = node.FindSlot(createEntry.compatibleSlotId);

                    var from = connectedSlot.isOutputSlot ? connectedSlot : compatibleSlot;
                    var to = connectedSlot.isOutputSlot ? compatibleSlot : connectedSlot;
                    m_Graph.Connect(from, to);

                    nodeNeedsRepositioning = true;
                    targetSlot = compatibleSlot;
                    targetPosition = graphMousePosition;
                }
            }

            return true;
        }
    }
}
