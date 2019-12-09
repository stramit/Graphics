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

        List<CreateEntry> GetEntries()
        {
            // First build up temporary data structure containing group & title as an array of strings (the last one is the actual title) and associated node type.
            var entries = new List<CreateEntry>();

            if(target is ContextView contextView)
            {
                TypeCache.TypeCollection blockCollection = TypeCache.GetTypesDerivedFrom<BlockData>();
                foreach(var type in blockCollection)
                {
                    var attrs = type.GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
                    if (attrs != null && attrs.Length > 0)
                    {
                        if (attrs[0].title[0] != k_HiddenFolderName)
                        {
                            var node = (BlockData)Activator.CreateInstance(type);
                            if(node.contextType == contextView.data.contextType.type)
                                AddEntries(node, attrs[0].title, entries);
                        }
                    }
                }

                return entries;
            }

            if(connectedPort?.userData is PortData portData)
            {
                if(portData.orientation == PortData.Orientation.Horizontal)
                    throw new NotImplementedException("PortData is not valid for horizontal edges");

                Type portType = portData.valueType.type;
                TypeCache.TypeCollection contextCollection = TypeCache.GetTypesDerivedFrom<IContext>();
                foreach(var type in contextCollection)
                {
                    // Never allow duplicate Output contexts
                    if(type == typeof(OutputContext))
                        continue;

                    var typeRef = new TypeRef<IContext>(type);
                    if(portData.direction == PortData.Direction.Input)
                    {
                        if(!typeRef.instance.outputPorts.Any(x => x.valueType.type == portType))
                            continue;
                    }
                    else if(portData.direction == PortData.Direction.Output)
                    {
                        if(!typeRef.instance.inputPorts.Any(x => x.valueType.type == portType))
                            continue;
                    }

                    var contextData = new ContextData
                    {
                        displayName = typeRef.instance.name,
                        contextType = typeRef,
                        inputPorts = typeRef.instance.inputPorts,
                        outputPorts = typeRef.instance.outputPorts,
                    };
                    foreach(var input in contextData.inputPorts)
                        input.owner = contextData;
                    foreach(var output in contextData.outputPorts)
                        output.owner = contextData;
                    AddEntries(contextData, new string[] {typeRef.instance.name}, entries);
                }

                return entries;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    if (type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(AbstractMaterialNode)))
                        && type != typeof(PropertyNode)
                        && type != typeof(KeywordNode)
                        && type != typeof(SubGraphNode)
                        && !type.IsSubclassOf(typeof(BlockData)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(TitleAttribute), false) as TitleAttribute[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            var node = (AbstractMaterialNode)Activator.CreateInstance(type);
                            AddEntries(node, attrs[0].title, entries);
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
                    AddEntries(node, new string[1] { asset.name }, entries);
                }

                else if (title[0] != k_HiddenFolderName)
                {
                    title.Add(asset.name);
                    AddEntries(node, title.ToArray(), entries);
                }
            }

            foreach (var property in m_Graph.properties)
            {
                var node = new PropertyNode();
                node.owner = m_Graph;
                node.property = property;
                node.owner = null;
                AddEntries(node, new[] { "Properties", "Property: " + property.displayName }, entries);
            }
            foreach (var keyword in m_Graph.keywords)
            {
                var node = new KeywordNode();
                node.owner = m_Graph;
                node.keyword = keyword;
                node.owner = null;
                AddEntries(node, new[] { "Keywords", "Keyword: " + keyword.displayName }, entries);
            }

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group.
            // Example result:
            // - Art/BlendMode
            // - Art/Adjustments/ColorBalance
            // - Art/Adjustments/Contrast
            entries.Sort((entry1, entry2) =>
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

            return entries;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = GetEntries();

            //* Build up the data structure needed by SearchWindow.

            // `groups` contains the current group path we're in.
            var groups = new List<string>();

            // First item in the tree is the title of the window.
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            foreach (var entry in entries)
            {
                // `createIndex` represents from where we should add new group entries from the current entry's group path.
                var createIndex = int.MaxValue;

                // Compare the group path of the current entry to the current group path.
                for (var i = 0; i < entry.title.Length - 1; i++)
                {
                    var group = entry.title[i];
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
                for (var i = createIndex; i < entry.title.Length - 1; i++)
                {
                    var group = entry.title[i];
                    groups.Add(group);
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                // Finally, add the actual entry.
                tree.Add(new SearchTreeEntry(new GUIContent(entry.title.Last(), m_Icon)) { level = entry.title.Length, userData = entry });
            }

            return tree;
        }

        void AddEntries(object obj, string[] title, List<CreateEntry> entries)
        {  
            AbstractMaterialNode node = obj as AbstractMaterialNode;
            
            if(node != null)
            {
                if (m_Graph.isSubGraph && !node.allowedInSubGraph)
                    return;
                if (!m_Graph.isSubGraph && !node.allowedInMainGraph)
                    return;
            }
            
            if (connectedPort == null || obj is ContextData contextData)
            {
                entries.Add(new CreateEntry
                {
                    obj = obj,
                    title = title,
                    compatibleSlotId = -1
                });
                return;
            }

            if(node == null)
                return;

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
                entries.Add(new CreateEntry
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
                entries.Add(new CreateEntry
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
            
            var windowRoot = m_EditorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - m_EditorWindow.position.position);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            if(createEntry.obj is ContextData contextData)
            {
                m_Graph.owner.RegisterCompleteObjectUndo("Add " + contextData.displayName + " Context");
                contextData.position = graphMousePosition;
                m_Graph.contexts.Add(contextData);

                if (connectedPort != null)
                {
                    var connectedPortData = connectedPort.userData as PortData;
                    var compatiblePortData = connectedPortData.direction == PortData.Direction.Input ? contextData.outputPorts[0] : contextData.inputPorts[0];

                    var from = connectedPortData.direction == PortData.Direction.Output ? connectedPortData : compatiblePortData;
                    var to = connectedPortData.direction == PortData.Direction.Output ? compatiblePortData : connectedPortData;
                    m_Graph.Connect(from, to);
                }

                return true;
            }

            if(createEntry.obj is BlockData blockData)
            {
                if(!(target is ContextView contextView))
                    return false;

                if(contextView.data.blocks.Any(x => x.GetType() == blockData.GetType()))
                    return false;
                
                m_Graph.owner.RegisterCompleteObjectUndo("Add " + blockData.name + " Block");
                int index = contextView.GetInsertionIndex(context.screenMousePosition);
                blockData.owner = m_Graph;
                contextView.data.blocks.Insert(index, blockData);
                
                bool blocksNeedUpdating = false;
                m_Graph.contextManager.AddBlocksOfType(blockData.requireBlocks, ref blocksNeedUpdating);
                m_Graph.contextManager.UpdateBlocks();

                return true;
            }

            if(createEntry.obj is AbstractMaterialNode node)
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

                return true;
            }
            
            return false;
        }
    }
}
