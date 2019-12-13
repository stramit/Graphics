using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Legacy;
using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [FormerName("UnityEditor.ShaderGraph.MaterialGraph")]
    [FormerName("UnityEditor.ShaderGraph.SubGraph")]
    [FormerName("UnityEditor.ShaderGraph.AbstractMaterialGraph")]
    sealed class GraphData : JsonObject
    {
        JsonStore m_Owner;

        public JsonStore owner
        {
            get => m_Owner;
            set => m_Owner = value;
        }

        [SerializeField]
        int m_Version = 1;

        #region ContextData

        [SerializeField]
        JsonList<ContextData> m_Contexts = new JsonList<ContextData>();

        public List<ContextData> contexts => m_Contexts;

        ContextManager m_ContextManager;

        public ContextManager contextManager => m_ContextManager;

        #endregion

        #region Input data

        [SerializeField]
        JsonList<AbstractShaderProperty> m_Properties = new JsonList<AbstractShaderProperty>();

        public IEnumerable<AbstractShaderProperty> properties => m_Properties;

        [SerializeField]
        JsonList<ShaderKeyword> m_Keywords = new JsonList<ShaderKeyword>();

        public IEnumerable<ShaderKeyword> keywords => m_Keywords;

        public string assetGuid { get; set; }

        #endregion

        #region Node data

        [SerializeField]
        JsonList<AbstractMaterialNode> m_Nodes = new JsonList<AbstractMaterialNode>();

        public IEnumerable<AbstractMaterialNode> nodes => m_Nodes;

        public IEnumerable<T> GetNodes<T>()
        {
            return nodes.OfType<T>();
        }

        [NonSerialized]
        List<AbstractMaterialNode> m_PastedNodes = new List<AbstractMaterialNode>();

        public IEnumerable<AbstractMaterialNode> pastedNodes
        {
            get { return m_PastedNodes; }
        }

        #endregion

        #region Group Data

        GroupData m_NullGroup = new GroupData();

        [SerializeField]
        JsonList<GroupData> m_Groups = new JsonList<GroupData>();

        public IEnumerable<GroupData> groups => m_Groups;

        [NonSerialized]
        List<GroupData> m_PastedGroups = new List<GroupData>();

        public IEnumerable<GroupData> pastedGroups
        {
            get { return m_PastedGroups; }
        }

        [NonSerialized]
        GroupData m_MostRecentlyCreatedGroup;

        public GroupData mostRecentlyCreatedGroup => m_MostRecentlyCreatedGroup;

        [NonSerialized]
        Dictionary<GroupData, List<IGroupItem>> m_GroupItems = new Dictionary<GroupData, List<IGroupItem>>();

        public IEnumerable<IGroupItem> GetItemsInGroup(GroupData groupData)
        {
            if (m_GroupItems.TryGetValue(groupData ?? m_NullGroup, out var nodes))
            {
                return nodes;
            }

            return Enumerable.Empty<IGroupItem>();
        }

        #endregion

        #region StickyNote Data

        [SerializeField]
        JsonList<StickyNoteData> m_StickyNotes = new JsonList<StickyNoteData>();

        public IEnumerable<StickyNoteData> stickyNotes => m_StickyNotes;

        [NonSerialized]
        List<StickyNoteData> m_PastedStickyNotes = new List<StickyNoteData>();

        public IEnumerable<StickyNoteData> pastedStickyNotes => m_PastedStickyNotes;

        #endregion

        #region Edge data

        [SerializeField]
        List<Edge> m_Edges = new List<Edge>();

        public IEnumerable<Edge> edges
        {
            get { return m_Edges; }
        }

        [NonSerialized]
        Dictionary<AbstractMaterialNode, List<Edge>> m_NodeEdges = new Dictionary<AbstractMaterialNode, List<Edge>>();

        #endregion

        [SerializeField]
        InspectorPreviewData m_PreviewData = new InspectorPreviewData();

        public InspectorPreviewData previewData
        {
            get { return m_PreviewData; }
            set { m_PreviewData = value; }
        }

        [SerializeField]
        string m_Path;

        public string path
        {
            get { return m_Path; }
            set
            {
                if (m_Path == value)
                    return;
                m_Path = value;
                if (owner != null)
                    owner.RegisterCompleteObjectUndo("Change Path");
            }
        }

        public MessageManager messageManager { get; set; }
        public bool isSubGraph { get; set; }

        [SerializeField]
        private ConcretePrecision m_ConcretePrecision = ConcretePrecision.Float;

        public ConcretePrecision concretePrecision
        {
            get => m_ConcretePrecision;
            set => m_ConcretePrecision = value;
        }

        #region Targets
        [NonSerialized]
        List<ITarget> m_ValidTargets = new List<ITarget>();

        public List<ITarget> validTargets => m_ValidTargets;

        [SerializeField]
        int m_ActiveTargetIndex;

        public int activeTargetIndex
        {
            get => m_ActiveTargetIndex;
            set => m_ActiveTargetIndex = value;
        }

        public ITarget activeTarget => m_ValidTargets[m_ActiveTargetIndex];

        [NonSerialized]
        List<ITargetImplementation> m_ValidImplementations = new List<ITargetImplementation>();

        public List<ITargetImplementation> validImplementations => m_ValidImplementations;

        [SerializeField]
        int m_ActiveTargetImplementationBitmask = -1;

        public int activeTargetImplementationBitmask
        {
            get => m_ActiveTargetImplementationBitmask;
            set => m_ActiveTargetImplementationBitmask = value;
        }

        public List<ITargetImplementation> activeTargetImplementations
        {
            get
            {
                // Return a list of all valid TargetImplementations enabled in the bitmask
                return m_ValidImplementations.Where(s => ((1 << m_ValidImplementations.IndexOf(s)) &
                    m_ActiveTargetImplementationBitmask) == (1 << m_ValidImplementations.IndexOf(s))).ToList();
            }
        }

        public void SetTarget(Type type)
        {
            if(!typeof(ITarget).IsAssignableFrom(type))
                return;

            // Set active target index to a Type    
            for(int i = 0; i < validTargets.Count; i++)
            {
                if(validTargets[i].GetType() == type)
                    activeTargetIndex = i;
            }
        }
        #endregion

        public SubGraphOutputNode subGraphOutput => GetNodes<SubGraphOutputNode>().FirstOrDefault();

        internal delegate void SaveGraphDelegate(Shader shader);

        internal static SaveGraphDelegate onSaveGraph;

        public GraphData()
        {
            m_GroupItems[m_NullGroup] = new List<IGroupItem>();
            m_ContextManager = new ContextManager(this);
        }

        public void ClearChanges()
        {
            // TODO: Handle these differently.
            m_PastedNodes.Clear();
            m_PastedGroups.Clear();
            m_PastedStickyNotes.Clear();
            m_MostRecentlyCreatedGroup = null;
        }

        public void AddNode(AbstractMaterialNode node)
        {
            if (node is AbstractMaterialNode materialNode)
            {
                if (isSubGraph && !materialNode.allowedInSubGraph)
                {
                    Debug.LogWarningFormat("Attempting to add {0} to Sub Graph. This is not allowed.", materialNode.GetType());
                    return;
                }

                AddNodeNoValidate(materialNode);

                // If adding a Sub Graph node whose asset contains Keywords
                // Need to restest Keywords against the variant limit
                if (node is SubGraphNode subGraphNode &&
                    subGraphNode.asset != null &&
                    subGraphNode.asset.keywords.Count > 0)
                {
                    OnKeywordChangedNoValidate();
                }

                ValidateGraph();
            }
            else
            {
                Debug.LogWarningFormat("Trying to add node {0} to Material graph, but it is not a {1}", node, typeof(AbstractMaterialNode));
            }
        }

        public void CreateGroup(GroupData groupData)
        {
            if (AddGroup(groupData))
            {
                m_MostRecentlyCreatedGroup = groupData;
            }
        }

        bool AddGroup(GroupData groupData)
        {
            if (m_Groups.Contains(groupData))
                return false;

            m_Groups.Add(groupData);
            m_GroupItems.Add(groupData, new List<IGroupItem>());

            return true;
        }

        public void RemoveGroup(GroupData groupData)
        {
            RemoveGroupNoValidate(groupData);
            ValidateGraph();
        }

        void RemoveGroupNoValidate(GroupData group)
        {
            if (!m_Groups.Contains(group))
                throw new InvalidOperationException("Cannot remove a group that doesn't exist.");
            m_Groups.Remove(group);

            if (m_GroupItems.TryGetValue(group, out var items))
            {
                foreach (IGroupItem groupItem in items.ToList())
                {
                    SetGroup(groupItem, null);
                }

                m_GroupItems.Remove(group);
            }
        }

        public void AddStickyNote(StickyNoteData stickyNote)
        {
            if (m_StickyNotes.Contains(stickyNote))
            {
                throw new InvalidOperationException("Sticky note has already been added to the graph.");
            }

            if (!m_GroupItems.ContainsKey(stickyNote.group ?? m_NullGroup))
            {
                throw new InvalidOperationException("Trying to add sticky note with group that doesn't exist.");
            }

            m_StickyNotes.Add(stickyNote);
            m_GroupItems[stickyNote.group ?? m_NullGroup].Add(stickyNote);
        }

        void RemoveNoteNoValidate(StickyNoteData stickyNote)
        {
            if (!m_StickyNotes.Contains(stickyNote))
            {
                throw new InvalidOperationException("Cannot remove a note that doesn't exist.");
            }

            m_StickyNotes.Remove(stickyNote);

            if (m_GroupItems.TryGetValue(stickyNote.group ?? m_NullGroup, out var groupItems))
            {
                groupItems.Remove(stickyNote);
            }
        }

        public void RemoveStickyNote(StickyNoteData stickyNote)
        {
            RemoveNoteNoValidate(stickyNote);
            ValidateGraph();
        }

        public void SetGroup(IGroupItem node, GroupData group)
        {
            var groupChange = new ParentGroupChange()
            {
                groupItem = node,
                oldGroup = node.group,

                // Checking if the groupdata is null. If it is, then it means node has been removed out of a group.
                // If the group data is null, then maybe the old group id should be removed
                newGroup = group
            };
            node.group = group;

            var oldGroupNodes = m_GroupItems[groupChange.oldGroup ?? m_NullGroup];
            oldGroupNodes.Remove(node);

            m_GroupItems[groupChange.newGroup].Add(node);
        }

        void AddNodeNoValidate(AbstractMaterialNode node)
        {
            if (!m_GroupItems.ContainsKey(node.group ?? m_NullGroup))
            {
                throw new InvalidOperationException("Cannot add a node whose group doesn't exist.");
            }

            node.owner = this;
            m_Nodes.Add(node);
            m_GroupItems[node.group ?? m_NullGroup].Add(node);
        }

        public void RemoveNode(AbstractMaterialNode node)
        {
            if (!node.canDeleteNode)
            {
                throw new InvalidOperationException($"Node {node.name} cannot be deleted.");
            }

            RemoveNodeNoValidate(node);
            ValidateGraph();
        }

        void RemoveNodeNoValidate(AbstractMaterialNode node)
        {
            if (!ContainsNode(node))
            {
                throw new InvalidOperationException("Cannot remove a node that doesn't exist.");
            }

            node.owner = null;
            
            if(node is BlockData blockData)
            {
                foreach(var context in contexts)
                    context.blocks.Remove(blockData);
            }
            
            m_Nodes.Remove(node);
            messageManager?.RemoveNode(node);

            if (m_GroupItems.TryGetValue(node.group ?? m_NullGroup, out var groupItems))
            {
                groupItems.Remove(node);
            }
        }

        void AddEdgeToNodeEdges(Edge edge)
        {
            List<Edge> inputEdges;
            if (!m_NodeEdges.TryGetValue(edge.inputSlot.owner, out inputEdges))
                m_NodeEdges[edge.inputSlot.owner] = inputEdges = new List<Edge>();
            inputEdges.Add(edge);

            List<Edge> outputEdges;
            if (!m_NodeEdges.TryGetValue(edge.outputSlot.owner, out outputEdges))
                m_NodeEdges[edge.outputSlot.owner] = outputEdges = new List<Edge>();
            outputEdges.Add(edge);
        }

        Edge ConnectNoValidate(MaterialSlot fromSlot, MaterialSlot toSlot)
        {
            var fromNode = fromSlot.owner;
            var toNode = toSlot.owner;

            if (fromNode == null || toNode == null)
                return null;

            // if fromNode is already connected to toNode
            // do now allow a connection as toNode will then
            // have an edge to fromNode creating a cycle.
            // if this is parsed it will lead to an infinite loop.
            var dependentNodes = new List<AbstractMaterialNode>();
            NodeUtils.CollectNodesNodeFeedsInto(dependentNodes, toNode);
            if (dependentNodes.Contains(fromNode))
                return null;

            if (fromSlot.isOutputSlot == toSlot.isOutputSlot)
                throw new InvalidOperationException($"Cannot connect two slots of opposite directions ({fromNode.name}/{fromSlot.displayName} and {toNode.name}/{toSlot.displayName})");

            var outputSlot = fromSlot.isOutputSlot ? fromSlot : toSlot;
            var inputSlot = fromSlot.isInputSlot ? fromSlot : toSlot;

            s_TempEdges.Clear();
            GetEdges(inputSlot, s_TempEdges);

            // remove any inputs that exits before adding
            foreach (var edge in s_TempEdges)
            {
                RemoveEdgeNoValidate((Edge)edge);
            }

            var newEdge = new Edge(outputSlot, inputSlot);
            m_Edges.Add(newEdge);
            AddEdgeToNodeEdges(newEdge);

            //Debug.LogFormat("Connected edge: {0} -> {1} ({2} -> {3})\n{4}", newEdge.outputSlot.nodeGuid, newEdge.inputSlot.nodeGuid, fromNode.name, toNode.name, Environment.StackTrace);
            return newEdge;
        }

        public Edge Connect(MaterialSlot fromSlot, MaterialSlot toSlot)
        {
            var newEdge = ConnectNoValidate(fromSlot, toSlot);
            ValidateGraph();
            return newEdge;
        }

        public void RemoveEdge(Edge e)
        {
            RemoveEdgeNoValidate((Edge)e);
            ValidateGraph();
        }

#region EdgeData
        // This section is mostly a duplication of all the edge handling logic.
        // ContextData uses a new PortData definition instead of MaterialSlot.
        // This requires a new Edge implemenetation (EdgeData).
        // To segregate these changes and minimise refactoring we have duplicated
        // all the connection logic here.

        [SerializeField]
        List<EdgeData> m_EdgeDatas = new List<EdgeData>();

        public IEnumerable<EdgeData> edgeDatas
        {
            get { return m_EdgeDatas; }
        }

        [NonSerialized]
        Dictionary<ContextData, List<EdgeData>> m_ContextEdgeDatas = new Dictionary<ContextData, List<EdgeData>>();

        static List<EdgeData> s_TempEdgeDatas = new List<EdgeData>();

        public EdgeData Connect(PortData output, PortData input)
        {
            var newEdge = ConnectNoValidate(output, input);
            ValidateGraph();
            return newEdge;
        }

        EdgeData ConnectNoValidate(PortData from, PortData to)
        {
            if (from.direction == to.direction)
                throw new InvalidOperationException($"Cannot connect two PortDatas of opposite directions");

            if (from.orientation != to.orientation)
                throw new InvalidOperationException($"Cannot connect two PortDatas of different orientations");

            var outputPort = from.direction == PortData.Direction.Output ? from : to;
            var inputPort = from.direction == PortData.Direction.Input ? from : to;

            s_TempEdgeDatas.Clear();
            GetEdgeDatas(inputPort, s_TempEdgeDatas);

            // remove any inputs that exits before adding
            foreach (var edge in s_TempEdgeDatas)
            {
                RemoveEdgeDataNoValidate((EdgeData)edge);
            }

            var newEdgeData = new EdgeData(inputPort, outputPort);
            m_EdgeDatas.Add(newEdgeData);

            if(from.orientation == PortData.Orientation.Vertical)
            {
                AddEdgeDataToContextEdgeDatas(newEdgeData);
            }
            else
            {
                // Implement AddEdgeDataToNodeEdgeDatas here
                throw new NotImplementedException("EdgeData type not supported for horizontal edges");
            }

            return newEdgeData;
        }

        public void RemoveEdge(EdgeData edgeData)
        {
            RemoveEdgeDataNoValidate((EdgeData)edgeData);
            ValidateGraph();
        }

        void RemoveEdgeDataNoValidate(EdgeData edgeData)
        {
            edgeData = m_EdgeDatas.FirstOrDefault(x => x.Equals(edgeData));
            if (edgeData == null)
                throw new ArgumentException("Trying to remove an edge that does not exist.", "e");
            m_EdgeDatas.Remove(edgeData as EdgeData);

            List<EdgeData> inputNodeEdgeDatas;
            if (edgeData.input.owner != null && m_ContextEdgeDatas.TryGetValue(edgeData.input.owner, out inputNodeEdgeDatas))
                inputNodeEdgeDatas.Remove(edgeData);

            List<EdgeData> outputNodeEdgeDatas;
            if (edgeData.output.owner != null && m_ContextEdgeDatas.TryGetValue(edgeData.output.owner, out outputNodeEdgeDatas))
                outputNodeEdgeDatas.Remove(edgeData);
        }

        public IEnumerable<EdgeData> GetEdgeDatas(PortData portData)
        {
            var edgeDatas = new List<EdgeData>();
            GetEdgeDatas(portData, edgeDatas);
            return edgeDatas;
        }

        public void GetEdgeDatas(PortData portData, List<EdgeData> edgeDatas)
        {
            var contextData = portData.owner;
            if (contextData == null)
                return;

            List<EdgeData> candidateEdges;
            if (!m_ContextEdgeDatas.TryGetValue(portData.owner, out candidateEdges))
                return;

            foreach (var edgeData in candidateEdges)
            {
                if ((portData.direction == PortData.Direction.Input ? edgeData.input : edgeData.output) == portData)
                {
                    edgeDatas.Add(edgeData);
                }
            }
        }

        void AddEdgeDataToContextEdgeDatas(EdgeData edgeData)
        {
            List<EdgeData> inputEdgeDatas;
            if (!m_ContextEdgeDatas.TryGetValue(edgeData.input.owner, out inputEdgeDatas))
                m_ContextEdgeDatas[edgeData.input.owner] = inputEdgeDatas = new List<EdgeData>();
            inputEdgeDatas.Add(edgeData);

            List<EdgeData> outputEdgeDatas;
            if (!m_ContextEdgeDatas.TryGetValue(edgeData.output.owner, out outputEdgeDatas))
                m_ContextEdgeDatas[edgeData.output.owner] = outputEdgeDatas = new List<EdgeData>();
            outputEdgeDatas.Add(edgeData);
        }
#endregion

        public void RemoveElements(AbstractMaterialNode[] nodes, Edge[] edges, GroupData[] groups, StickyNoteData[] notes, EdgeData[] edgeDatas = null)
        {
            foreach (var node in nodes)
            {
                if (!node.canDeleteNode)
                {
                    throw new InvalidOperationException($"Node {node.name} cannot be deleted.");
                }
            }

            foreach (var edge in edges.ToArray())
            {
                RemoveEdgeNoValidate((Edge)edge);
            }   

            if(edgeDatas != null)
            {
                foreach (var edgeData in edgeDatas)
                    RemoveEdgeDataNoValidate(edgeData);
            }

            foreach (var serializableNode in nodes)
            {
                RemoveNodeNoValidate(serializableNode);
            }

            foreach (var noteData in notes)
            {
                RemoveNoteNoValidate(noteData);
            }

            foreach (var groupData in groups)
            {
                RemoveGroupNoValidate(groupData);
            }

            ValidateGraph();
        }

        void RemoveEdgeNoValidate(Edge e)
        {
            e = m_Edges.FirstOrDefault(x => x.Equals(e));
            if (e == null)
                throw new ArgumentException("Trying to remove an edge that does not exist.", "e");
            m_Edges.Remove(e as Edge);

            List<Edge> inputNodeEdges;
            if (e.inputSlot.owner != null && m_NodeEdges.TryGetValue(e.inputSlot.owner, out inputNodeEdges))
                inputNodeEdges.Remove(e);

            List<Edge> outputNodeEdges;
            if (e.outputSlot.owner != null && m_NodeEdges.TryGetValue(e.outputSlot.owner, out outputNodeEdges))
                outputNodeEdges.Remove(e);
        }

        public bool ContainsNode(AbstractMaterialNode node)
        {
            if (node == null)
            {
                return false;
            }

            if(m_Nodes.Contains(node))
                return true;
            
            // Now also needs to check Blocks in Contexts as all 
            // our lookup code for Edges depends on this function.
            foreach(ContextData context in contexts)
            {
                if(context.blocks.Contains(node))
                    return true;
            }

            return false;
        }

        public void GetEdges(MaterialSlot slot, List<Edge> foundEdges)
        {
            var node = slot.owner;
            if (node == null)
            {
                return;
            }

            List<Edge> candidateEdges;
            if (!m_NodeEdges.TryGetValue(slot.owner, out candidateEdges))
                return;

            foreach (var edge in candidateEdges)
            {
                if ((slot.isInputSlot ? edge.inputSlot : edge.outputSlot) == slot)
                {
                    foundEdges.Add(edge);
                }
            }
        }

        public IEnumerable<Edge> GetEdges(MaterialSlot s)
        {
            var edges = new List<Edge>();
            GetEdges(s, edges);
            return edges;
        }

        public void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            foreach (var prop in properties)
            {
                if (prop is GradientShaderProperty gradientProp && generationMode == GenerationMode.Preview)
                {
                    GradientUtil.GetGradientPropertiesForPreview(collector, gradientProp.referenceName, gradientProp.value);
                    continue;
                }

                collector.AddShaderProperty(prop);
            }
        }

        public void CollectShaderKeywords(KeywordCollector collector, GenerationMode generationMode)
        {
            foreach (var keyword in keywords)
            {
                collector.AddShaderKeyword(keyword);
            }

            // Alwways calculate permutations when collecting
            collector.CalculateKeywordPermutations();
        }

        public void AddGraphInput(ShaderInput input)
        {
            if (input == null)
                return;

            switch (input)
            {
                case AbstractShaderProperty property:
                    if (m_Properties.Contains(property))
                        return;
                    m_Properties.Add(property);
                    break;
                case ShaderKeyword keyword:
                    if (m_Keywords.Contains(keyword))
                        return;
                    m_Keywords.Add(keyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SanitizeGraphInputName(ShaderInput input)
        {
            input.displayName = input.displayName.Trim();
            switch (input)
            {
                case AbstractShaderProperty property:
                    input.displayName = GraphUtil.SanitizeName(properties.Where(p => !ReferenceEquals(p, property)).Select(p => p.displayName), "{0} ({1})", input.displayName);
                    break;
                case ShaderKeyword keyword:
                    input.displayName = GraphUtil.SanitizeName(keywords.Where(p => !ReferenceEquals(p, keyword)).Select(p => p.displayName), "{0} ({1})", input.displayName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SanitizeGraphInputReferenceName(ShaderInput input, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return;

            string name = newName.Trim();
            if (string.IsNullOrEmpty(name))
                return;

            name = Regex.Replace(name, @"(?:[^A-Za-z_0-9])|(?:\s)", "_");
            switch (input)
            {
                case AbstractShaderProperty property:
                    property.overrideReferenceName = GraphUtil.SanitizeName(properties.Where(p => p != property).Select(p => p.referenceName), "{0}_{1}", name);
                    break;
                case ShaderKeyword keyword:
                    keyword.overrideReferenceName = GraphUtil.SanitizeName(keywords.Where(p => p.guid != input.guid).Select(p => p.referenceName), "{0}_{1}", name).ToUpper();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveGraphInput(ShaderInput input)
        {
            switch (input)
            {
                case AbstractShaderProperty property:
                    var propertyNodes = GetNodes<PropertyNode>().Where(x => x.property == property).ToList();
                    foreach (var propNode in propertyNodes)
                        ReplacePropertyNodeWithConcreteNodeNoValidate(propNode);
                    break;
            }

            RemoveGraphInputNoValidate(input);
            ValidateGraph();
        }

        public void MoveProperty(AbstractShaderProperty property, int newIndex)
        {
            if (newIndex > m_Properties.Count || newIndex < 0)
                throw new ArgumentException("New index is not within properties list.");
            var currentIndex = m_Properties.IndexOf(property);
            if (currentIndex == -1)
                throw new ArgumentException("Property is not in graph.");
            if (newIndex == currentIndex)
                return;
            m_Properties.RemoveAt(currentIndex);
            if (newIndex > currentIndex)
                newIndex--;
            var isLast = newIndex == m_Properties.Count;
            if (isLast)
                m_Properties.Add(property);
            else
                m_Properties.Insert(newIndex, property);
        }

        public void MoveKeyword(ShaderKeyword keyword, int newIndex)
        {
            if (newIndex > m_Keywords.Count || newIndex < 0)
                throw new ArgumentException("New index is not within keywords list.");
            var currentIndex = m_Keywords.IndexOf(keyword);
            if (currentIndex == -1)
                throw new ArgumentException("Keyword is not in graph.");
            if (newIndex == currentIndex)
                return;
            m_Keywords.RemoveAt(currentIndex);
            if (newIndex > currentIndex)
                newIndex--;
            var isLast = newIndex == m_Keywords.Count;
            if (isLast)
                m_Keywords.Add(keyword);
            else
                m_Keywords.Insert(newIndex, keyword);
        }

        public int GetGraphInputIndex(ShaderInput input)
        {
            switch (input)
            {
                case AbstractShaderProperty property:
                    return m_Properties.IndexOf(property);
                case ShaderKeyword keyword:
                    return m_Keywords.IndexOf(keyword);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void RemoveGraphInputNoValidate(ShaderInput shaderInput)
        {
            if (shaderInput is AbstractShaderProperty property)
            {
                m_Properties.Remove(property.ToJsonRef());
            }
            else if (shaderInput is ShaderKeyword keyword)
            {
                m_Keywords.Remove(keyword.ToJsonRef());
            }
        }

        static List<Edge> s_TempEdges = new List<Edge>();

        public void ReplacePropertyNodeWithConcreteNode(PropertyNode propertyNode)
        {
            ReplacePropertyNodeWithConcreteNodeNoValidate(propertyNode);
            ValidateGraph();
        }

        void ReplacePropertyNodeWithConcreteNodeNoValidate(PropertyNode propertyNode)
        {
            var property = propertyNode.property;
            if (property == null)
                return;

            var node = property.ToConcreteNode();
            if (node == null)
                return;

            var slot = propertyNode.FindOutputSlot<MaterialSlot>(PropertyNode.OutputSlotId);
            var newSlot = node.GetOutputSlots<MaterialSlot>().FirstOrDefault(s => s.valueType == slot.valueType);
            if (newSlot == null)
                return;

            node.drawState = propertyNode.drawState;
            node.group = propertyNode.group;
            AddNodeNoValidate(node);

            foreach (var edge in GetEdges(slot))
                ConnectNoValidate(newSlot, edge.inputSlot);

            RemoveNodeNoValidate(propertyNode);
        }

        public void OnKeywordChanged()

        {
            OnKeywordChangedNoValidate();
            ValidateGraph();
        }

        public void OnKeywordChangedNoValidate()
        {
            var allNodes = GetNodes<AbstractMaterialNode>();
            foreach (AbstractMaterialNode node in allNodes)
            {
                node.Dirty(ModificationScope.Topological);
                node.ValidateNode();
            }
        }

        public void ValidateGraph()
        {
            var propertyNodes = GetNodes<PropertyNode>().Where(n => !m_Properties.Any(p => p == n.property)).ToArray();
            foreach (var pNode in propertyNodes)
                ReplacePropertyNodeWithConcreteNodeNoValidate(pNode);

            messageManager?.ClearAllFromProvider(this);

            //First validate edges, remove any
            //orphans. This can happen if a user
            //manually modifies serialized data
            //of if they delete a node in the inspector
            //debug view.
            foreach (var edge in m_Edges.ToArray())
            {
                var outputNode = edge.outputSlot.owner;
                var inputNode = edge.inputSlot.owner;

                MaterialSlot outputSlot = null;
                MaterialSlot inputSlot = null;
                if (outputNode != null && inputNode != null)
                {
                    outputSlot = edge.outputSlot;
                    inputSlot = edge.inputSlot;
                }

                if (outputNode?.owner != this
                    || inputNode?.owner != this
                    || outputSlot.owner == null
                    || inputSlot.owner == null)
                {
                    //orphaned edge
                    RemoveEdgeNoValidate(edge);
                }
            }

            var temporaryMarks = new HashSet<AbstractMaterialNode>();
            var permanentMarks = new HashSet<AbstractMaterialNode>();
            var slots = ListPool<MaterialSlot>.Get();

            // Make sure we process a node's children before the node itself.
            var stack = StackPool<AbstractMaterialNode>.Get();
            foreach (var node in GetNodes<AbstractMaterialNode>())
            {
                stack.Push(node);
            }

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (permanentMarks.Contains(node))
                {
                    continue;
                }

                if (temporaryMarks.Contains(node))
                {
                    node.ValidateNode();
                    permanentMarks.Add(node);
                }
                else
                {
                    temporaryMarks.Add(node);
                    stack.Push(node);
                    node.GetInputSlots(slots);
                    foreach (var inputSlot in slots)
                    {
                        var nodeEdges = GetEdges(inputSlot);
                        foreach (var edge in nodeEdges)
                        {
                            var fromSlot = edge.outputSlot;
                            var childNode = fromSlot.owner;
                            if (childNode != null)
                            {
                                stack.Push(childNode);
                            }
                        }
                    }

                    slots.Clear();
                }
            }

            StackPool<AbstractMaterialNode>.Release(stack);
            ListPool<MaterialSlot>.Release(slots);
        }

        public void AddValidationError(AbstractMaterialNode node, string errorMessage,
            ShaderCompilerMessageSeverity severity = ShaderCompilerMessageSeverity.Error)
        {
            messageManager?.AddOrAppendError(this, node, new ShaderMessage(errorMessage, severity));
        }

        public void ClearErrorsForNode(AbstractMaterialNode node)
        {
            messageManager?.ClearNodesFromProvider(this, node.ToEnumerable());
        }

        // TODO: FIX COPY PASTE
        internal void PasteGraph(CopyPasteGraph graphToPaste, List<AbstractMaterialNode> remappedNodes, List<Edge> remappedEdges)
        {
            /*
            var groupGuidMap = new Dictionary<Guid, Guid>();
            foreach (var group in graphToPaste.groups)
            {
                var position = group.position;
                position.x += 30;
                position.y += 30;

                GroupData newGroup = new GroupData(group.title, position);

                var oldGuid = group.guid;
                var newGuid = newGroup.guid;
                groupGuidMap[oldGuid] = newGuid;

                AddGroup(newGroup);
                m_PastedGroups.Add(newGroup);
            }

            foreach (var stickyNote in graphToPaste.stickyNotes)
            {
                var position = stickyNote.position;
                position.x += 30;
                position.y += 30;

                StickyNoteData pastedStickyNote = new StickyNoteData(stickyNote.title, stickyNote.content, position);
                if (groupGuidMap.ContainsKey(stickyNote.groupGuid))
                {
                    pastedStickyNote.groupGuid = groupGuidMap[stickyNote.groupGuid];
                }

                AddStickyNote(pastedStickyNote);
                m_PastedStickyNotes.Add(pastedStickyNote);
            }

            var nodeGuidMap = new Dictionary<Guid, Guid>();
            foreach (var node in graphToPaste.GetNodes<AbstractMaterialNode>())
            {
                AbstractMaterialNode pastedNode = node;

                var oldGuid = node.guid;
                var newGuid = node.RewriteGuid();
                nodeGuidMap[oldGuid] = newGuid;

                // Check if the property nodes need to be made into a concrete node.
                if (node is PropertyNode propertyNode)
                {
                    // If the property is not in the current graph, do check if the
                    // property can be made into a concrete node.
                    if (!m_Properties.Select(x => x.guid).Contains(propertyNode.propertyGuid))
                    {
                        // If the property is in the serialized paste graph, make the property node into a property node.
                        var pastedGraphMetaProperties = graphToPaste.metaProperties.Where(x => x.guid == propertyNode.propertyGuid);
                        if (pastedGraphMetaProperties.Any())
                        {
                            pastedNode = pastedGraphMetaProperties.FirstOrDefault().ToConcreteNode();
                            pastedNode.drawState = node.drawState;
                            nodeGuidMap[oldGuid] = pastedNode.guid;
                        }
                    }
                }

                AbstractMaterialNode abstractMaterialNode = (AbstractMaterialNode)node;

                // If the node has a group guid and no group has been copied, reset the group guid.
                // Check if the node is inside a group
                if (abstractMaterialNode.groupGuid != Guid.Empty)
                {
                    if (groupGuidMap.ContainsKey(abstractMaterialNode.groupGuid))
                    {
                        var absNode = pastedNode as AbstractMaterialNode;
                        absNode.groupGuid = groupGuidMap[abstractMaterialNode.groupGuid];
                        pastedNode = absNode;
                    }
                    else
                    {
                        pastedNode.groupGuid = Guid.Empty;
                    }
                }

                var drawState = node.drawState;
                var position = drawState.position;
                position.x += 30;
                position.y += 30;
                drawState.position = position;
                node.drawState = drawState;
                remappedNodes.Add(pastedNode);
                AddNode(pastedNode);

                // add the node to the pasted node list
                m_PastedNodes.Add(pastedNode);

                // Check if the keyword nodes need to have their keywords copied.
                if (node is KeywordNode keywordNode)
                {
                    // If the keyword is not in the current graph and is in the serialized paste graph copy it.
                    if (!keywords.Select(x => x.guid).Contains(keywordNode.keywordGuid))
                    {
                        var pastedGraphMetaKeywords = graphToPaste.metaKeywords.Where(x => x.guid == keywordNode.keywordGuid);
                        if (pastedGraphMetaKeywords.Any())
                        {
                            var keyword = pastedGraphMetaKeywords.FirstOrDefault(x => x.guid == keywordNode.keywordGuid);
                            SanitizeGraphInputName(keyword);
                            SanitizeGraphInputReferenceName(keyword, keyword.overrideReferenceName);
                            AddGraphInput(keyword);
                        }
                    }

                    // Always update Keyword nodes to handle any collisions resolved on the Keyword
                    keywordNode.UpdateNode();
                }
            }

            // only connect edges within pasted elements, discard
            // external edges.
            foreach (var edge in graphToPaste.edges)
            {
                var outputSlot = edge.outputSlot;
                var inputSlot = edge.inputSlot;

                Guid remappedOutputNodeGuid;
                Guid remappedInputNodeGuid;
                if (nodeGuidMap.TryGetValue(outputSlot.nodeGuid, out remappedOutputNodeGuid)
                    && nodeGuidMap.TryGetValue(inputSlot.nodeGuid, out remappedInputNodeGuid))
                {
                    var outputSlotRef = new SlotReference(remappedOutputNodeGuid, outputSlot.slotId);
                    var inputSlotRef = new SlotReference(remappedInputNodeGuid, inputSlot.slotId);
                    remappedEdges.Add(Connect(outputSlotRef, inputSlotRef));
                }
            }

            ValidateGraph();
            */
        }

        internal override void OnDeserializing()
        {
            m_Version = 0;
        }

        internal override void OnDeserialized(string json)
        {
            if (m_Version == 0)
            {
                m_Version = 1;
                UpgradeV0ToV1(json);
            }
        }

        void UpgradeV0ToV1(string json)
        {
            var graphDataV0 = JsonUtility.FromJson<GraphDataV0>(json);

            var propertyJsonList = new List<string>();
            var legacyPropertyMap = new Dictionary<string, AbstractShaderProperty>();
            SerializationHelper.Upgrade(graphDataV0.properties, m_Properties, propertyJsonList);
            for (var i = 0; i < m_Properties.Count; i++)
            {
                var legacyProperty = JsonUtility.FromJson<LegacyShaderInput>(propertyJsonList[i]);
                legacyPropertyMap[legacyProperty.guid.guidSerialized] = m_Properties[i];
            }

            var keywordJsonList = new List<string>();
            var legacyKeywordMap = new Dictionary<string, ShaderKeyword>();
            SerializationHelper.Upgrade(graphDataV0.keywords, m_Keywords, keywordJsonList);
            for (var i = 0; i < m_Keywords.Count; i++)
            {
                var legacyKeyword = JsonUtility.FromJson<LegacyShaderInput>(keywordJsonList[i]);
                legacyKeywordMap[legacyKeyword.guid.guidSerialized] = m_Keywords[i];
            }

            var legacyGroupMap = new Dictionary<string, GroupData>();
            for (var i = 0; i < m_Groups.Count; i++)
            {
                var dataV0 = graphDataV0.groups[i];
                var data = new GroupData(dataV0.title, dataV0.position);
                legacyGroupMap[dataV0.guidSerialized] = data;
                m_Groups.Add(data.ToJsonRef());
                DeserializationContext.Enqueue(data, null);
            }

            var nodeJsonList = new List<string>();
            var slotJsonList = new List<string>();
            var legacySlotMap = new Dictionary<(string, int), MaterialSlot>();
            SerializationHelper.Upgrade(graphDataV0.nodes, m_Nodes, nodeJsonList);
            for (var i = 0; i < m_Nodes.Count; i++)
            {
                // TODO: Property guid etc.
                var nodeV0 = JsonUtility.FromJson<LegacyNode>(nodeJsonList[i]);
                var node = m_Nodes[i];
                if (!string.IsNullOrEmpty(nodeV0.groupGuid) && legacyGroupMap.TryGetValue(nodeV0.groupGuid, out var groupData))
                {
                    node.group = groupData;
                }

                var slots = node.InternalGetSlots();
                slots.Clear();
                SerializationHelper.Upgrade(nodeV0.slots, slots, slotJsonList);
                for (var j = 0; j < slots.Count; j++)
                {
                    var slot = slots[j];
                    slot.owner = node;
                    var slotJson = slotJsonList[j];
                    var slotId = JsonUtility.FromJson<SlotId>(slotJson).value;
                    legacySlotMap[(nodeV0.guid, slotId)] = slot;
                }

                slotJsonList.Clear();

                if (node is PropertyNode propertyNode && !string.IsNullOrEmpty(nodeV0.propertyGuid) &&
                    legacyPropertyMap.TryGetValue(nodeV0.propertyGuid, out var property))
                {
                    propertyNode.InternalSetProperty(property);
                }

                if (node is KeywordNode keywordNode && !string.IsNullOrEmpty(nodeV0.keywordGuid) &&
                    legacyKeywordMap.TryGetValue(nodeV0.keywordGuid, out var keyword))
                {
                    keywordNode.InternalSetKeyword(keyword);
                }
            }

            if (graphDataV0.edges != null)
            {
                foreach (var serializableEdge in graphDataV0.edges)
                {
                    var legacyEdge = JsonUtility.FromJson<LegacyEdge>(serializableEdge.JSONnodeData);
                    var outputSlot = legacySlotMap[(legacyEdge.outputSlot.nodeGUID, legacyEdge.outputSlot.slotId)];
                    var inputSlot = legacySlotMap[(legacyEdge.inputSlot.nodeGUID, legacyEdge.inputSlot.slotId)];
                    if (inputSlot != null && outputSlot != null)
                    {
                        m_Edges.Add(new Edge(outputSlot, inputSlot));
                    }
                }
            }

            for (var i = 0; i < m_StickyNotes.Count; i++)
            {
                var dataV0 = graphDataV0.stickyNotes[i];
                var data = new StickyNoteData(dataV0.title, dataV0.content, dataV0.position)
                {
                    theme = dataV0.theme,
                    textSize = dataV0.textSize
                };

                if (!string.IsNullOrEmpty(dataV0.groupGuidSerialized) && legacyGroupMap.TryGetValue(dataV0.groupGuidSerialized, out var groupData))
                {
                    data.group = groupData;
                }

                m_StickyNotes.Add(data);
                DeserializationContext.Enqueue(data, null);
            }
        }

        internal override void OnStoreDeserialized(string json)
        {
            foreach (var group in groups)
            {
                m_GroupItems.Add(group, new List<IGroupItem>());
            }

            foreach (var node in nodes)
            {
                node.owner = this;
                m_NodeEdges[node] = new List<Edge>();
                m_GroupItems[node.group ?? m_NullGroup].Add(node);
                foreach (var slot in node.InternalGetSlots())
                {
                    slot.owner = node;
                }
            }

            foreach (var stickyNote in stickyNotes)
            {
                m_GroupItems[stickyNote.group ?? m_NullGroup].Add(stickyNote);
            }
            
            // Now set owner through all Blocks and their Slots
            foreach (var context in contexts)
            {
                m_ContextEdgeDatas[context] = new List<EdgeData>();
                foreach(var block in context.blocks)
                {
                    block.owner = this;
                    foreach(var slot in block.InternalGetSlots())
                    {
                        slot.owner = block;
                    }
                }
            }

            foreach (var edge in m_Edges)
            {
                AddEdgeToNodeEdges(edge);
            }

            foreach (var edgeData in m_EdgeDatas)
            {
                AddEdgeDataToContextEdgeDatas(edgeData);
            }

            ValidateGraph();
            UpdateTargets();
        }

        public void OnEnable()
        {
            foreach (var node in GetNodes<AbstractMaterialNode>().OfType<IOnAssetEnabled>())
            {
                node.OnEnable();
            }

            UpdateTargets();

            ShaderGraphPreferences.onVariantLimitChanged += OnKeywordChanged;
        }

        public void OnDisable()
        {
            ShaderGraphPreferences.onVariantLimitChanged -= OnKeywordChanged;
        }

        public void UpdateTargets()
        {
            // First get all valid TargetImplementations that are valid with the current graph
            List<ITargetImplementation> foundImplementations = new List<ITargetImplementation>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    var isImplementation = !type.IsAbstract && !type.IsGenericType && type.IsClass && typeof(ITargetImplementation).IsAssignableFrom(type);
                    //for subgraph output nodes, preview target is the only valid target
                    if (isSubGraph && isImplementation && typeof(DefaultPreviewTarget).IsAssignableFrom(type))
                    {
                        var implementation = (DefaultPreviewTarget)Activator.CreateInstance(type);
                        foundImplementations.Add(implementation);
                    }
                    else if (isImplementation && !foundImplementations.Any(s => s.GetType() == type) && !typeof(DefaultPreviewTarget).IsAssignableFrom(type))
                    {
                        var implementation = (ITargetImplementation)Activator.CreateInstance(type);
                        foundImplementations.Add(implementation);
                    }
                }
            }

            // Next we get all Targets that have valid TargetImplementations
            List<ITarget> foundTargets = new List<ITarget>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    var isTarget = !type.IsAbstract && !type.IsGenericType && type.IsClass && typeof(ITarget).IsAssignableFrom(type);
                    if (isTarget && !foundTargets.Any(s => s.GetType() == type))
                    {
                        var target = (ITarget)Activator.CreateInstance(type);
                        if(foundImplementations.Where(s => s.targetType == type).Any())
                            foundTargets.Add(target);
                    }
                }
            }

            // Assembly reload, just rebuild the non-serialized lists
            if(m_ValidTargets.Count == 0)
            {
                m_ValidTargets = foundTargets;
                m_ValidImplementations = foundImplementations.Where(s => s.targetType == foundTargets[0].GetType()).ToList();
            }

            // Active Target is no longer valid
            // Reset all Target selections and return
            if(!foundTargets.Select(s => s.GetType()).Contains(m_ValidTargets[m_ActiveTargetIndex].GetType()))
            {
                m_ActiveTargetIndex = 0; // Default
                m_ActiveTargetImplementationBitmask = -1; // Everything
                m_ValidTargets = foundTargets;
                m_ValidImplementations = foundImplementations.Where(s => s.targetType == foundTargets[0].GetType()).ToList();
                return;
            }

            // Active Target index has changed
            // Still need to validate TargetImplementation bitmask
            if(foundTargets[m_ActiveTargetIndex].GetType() != activeTarget.GetType())
            {
                var activeTargetInFoundList = foundTargets.Where(s => s.GetType() == activeTarget.GetType()).FirstOrDefault();
                m_ActiveTargetIndex = foundTargets.IndexOf(activeTargetInFoundList);
            }

            // Update valid Targets and TargetImplementations
            m_ValidTargets = foundTargets;
            m_ValidImplementations = foundImplementations.Where(s => s.targetType == activeTarget.GetType()).ToList();

            // Nothing or Everything. No need to update bitmask.
            if(m_ActiveTargetImplementationBitmask == 0 || m_ActiveTargetImplementationBitmask == -1)
                return;

            // Current ITargetImplementation bitmask is set to Mixed...
            // We need to build a new bitmask from the indicies in the new Implementation list
            int newBitmask = m_ActiveTargetImplementationBitmask;//0;
            // foreach(ITargetImplementation implementation in activeTargetImplementations)
            // {
            //     var implementationInFound = foundImplementations.Where(s => s.GetType() == implementation.GetType()).FirstOrDefault();
            //     if(implementationInFound != null)
            //     {
            //         // If the new Implementation list contains this Implementation
            //         // add its new index to the bitmask
            //         newBitmask = newBitmask | (1 << foundImplementations.IndexOf(implementationInFound));
            //     }
            // }
            m_ActiveTargetImplementationBitmask = newBitmask;
        }
    }

    [Serializable]
    class InspectorPreviewData
    {
        public SerializableMesh serializedMesh = new SerializableMesh();

        [NonSerialized]
        public Quaternion rotation = Quaternion.identity;

        [NonSerialized]
        public float scale = 1f;
    }
}
