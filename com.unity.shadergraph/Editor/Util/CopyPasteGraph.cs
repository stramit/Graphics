using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.ShaderGraph;

namespace UnityEditor.Graphing.Util
{
    [Serializable]
    sealed class CopyPasteGraph : ISerializationCallbackReceiver
    {
        [NonSerialized]
        HashSet<IEdge> m_Edges = new HashSet<IEdge>();

        [NonSerialized]
        HashSet<INode> m_Nodes = new HashSet<INode>();

        [SerializeField]
        List<GroupData> m_Groups = new List<GroupData>();

        [NonSerialized]
        HashSet<IShaderValue> m_GraphInputs = new HashSet<IShaderValue>();

        // The meta properties are properties that are not copied into the tatget graph
        // but sent along to allow property nodes to still hvae the data from the original
        // property present.
        [NonSerialized]
        HashSet<IShaderValue> m_MetaGraphInputs = new HashSet<IShaderValue>();

        [NonSerialized]
        SerializableGuid m_SourceGraphGuid;

        [SerializeField]
        List<SerializationHelper.JSONSerializedElement> m_SerializableNodes = new List<SerializationHelper.JSONSerializedElement>();

        [SerializeField]
        List<SerializationHelper.JSONSerializedElement> m_SerializableEdges = new List<SerializationHelper.JSONSerializedElement>();

        [SerializeField]
        List<SerializationHelper.JSONSerializedElement> m_SerilaizeableGraphInputs = new List<SerializationHelper.JSONSerializedElement>();

        [SerializeField]
        List<SerializationHelper.JSONSerializedElement> m_SerializableMetaGraphInputs = new List<SerializationHelper.JSONSerializedElement>();

        [SerializeField]
        SerializationHelper.JSONSerializedElement m_SerializeableSourceGraphGuid = new SerializationHelper.JSONSerializedElement();

        public CopyPasteGraph() {}

        public CopyPasteGraph(Guid sourceGraphGuid, IEnumerable<GroupData> groups, IEnumerable<INode> nodes, IEnumerable<IEdge> edges, IEnumerable<IShaderValue> graphInputs, IEnumerable<IShaderValue> metaGraphInputs)
        {
            m_SourceGraphGuid = new SerializableGuid(sourceGraphGuid);

            foreach (var groupData in groups)
            {
                AddGroup(groupData);
            }

            foreach (var node in nodes)
            {
                AddNode(node);
                foreach (var edge in NodeUtils.GetAllEdges(node))
                    AddEdge(edge);
            }

            foreach (var edge in edges)
                AddEdge(edge);

            foreach (var property in graphInputs)
                AddGraphInput(property);

            foreach (var metaProperty in metaGraphInputs)
                AddMetaGraphInput(metaProperty);
        }

        public void AddGroup(GroupData group)
        {
            m_Groups.Add(group);
        }

        public void AddNode(INode node)
        {
            m_Nodes.Add(node);
        }

        public void AddEdge(IEdge edge)
        {
            m_Edges.Add(edge);
        }

        public void AddGraphInput(IShaderValue property)
        {
            m_GraphInputs.Add(property);
        }

        public void AddMetaGraphInput(IShaderValue metaProperty)
        {
            m_MetaGraphInputs.Add(metaProperty);
        }

        public IEnumerable<T> GetNodes<T>() where T : INode
        {
            return m_Nodes.OfType<T>();
        }

        public IEnumerable<GroupData> groups
        {
            get { return m_Groups; }
        }

        public IEnumerable<IEdge> edges
        {
            get { return m_Edges; }
        }

        public IEnumerable<IShaderValue> graphInputs
        {
            get { return m_GraphInputs; }
        }

        public IEnumerable<IShaderValue> metaGraphInputs
        {
            get { return m_MetaGraphInputs; }
        }

        public Guid sourceGraphGuid
        {
            get { return m_SourceGraphGuid.guid; }
        }

        public void OnBeforeSerialize()
        {
            m_SerializeableSourceGraphGuid = SerializationHelper.Serialize(m_SourceGraphGuid);
            m_SerializableNodes = SerializationHelper.Serialize<INode>(m_Nodes);
            m_SerializableEdges = SerializationHelper.Serialize<IEdge>(m_Edges);
            m_SerilaizeableGraphInputs = SerializationHelper.Serialize<IShaderValue>(m_GraphInputs);
            m_SerializableMetaGraphInputs = SerializationHelper.Serialize<IShaderValue>(m_MetaGraphInputs);
        }

        public void OnAfterDeserialize()
        {
            m_SourceGraphGuid = SerializationHelper.Deserialize<SerializableGuid>(m_SerializeableSourceGraphGuid, GraphUtil.GetLegacyTypeRemapping());

            var nodes = SerializationHelper.Deserialize<INode>(m_SerializableNodes, GraphUtil.GetLegacyTypeRemapping());
            m_Nodes.Clear();
            foreach (var node in nodes)
                m_Nodes.Add(node);
            m_SerializableNodes = null;

            var edges = SerializationHelper.Deserialize<IEdge>(m_SerializableEdges, GraphUtil.GetLegacyTypeRemapping());
            m_Edges.Clear();
            foreach (var edge in edges)
                m_Edges.Add(edge);
            m_SerializableEdges = null;

            var graphInputs = SerializationHelper.Deserialize<IShaderValue>(m_SerilaizeableGraphInputs, GraphUtil.GetLegacyTypeRemapping());
            m_GraphInputs.Clear();
            foreach (var property in graphInputs)
                m_GraphInputs.Add(property);
            m_SerilaizeableGraphInputs = null;

            var metaGraphInputs = SerializationHelper.Deserialize<IShaderValue>(m_SerializableMetaGraphInputs, GraphUtil.GetLegacyTypeRemapping());
            m_MetaGraphInputs.Clear();
            foreach (var metaProperty in metaGraphInputs)
            {
                m_MetaGraphInputs.Add(metaProperty);
            }
            m_SerializableMetaGraphInputs = null;
        }

        internal static CopyPasteGraph FromJson(string copyBuffer)
        {
            try
            {
                return JsonUtility.FromJson<CopyPasteGraph>(copyBuffer);
            }
            catch
            {
                // ignored. just means copy buffer was not a graph :(
                return null;
            }
        }
    }
}
