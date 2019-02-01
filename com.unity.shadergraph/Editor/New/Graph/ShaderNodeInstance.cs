using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal class ShaderNodeInstance : ShaderNode
    {   
        [NonSerialized]
        object m_Data;

        [SerializeField]
        SerializationHelper.JSONSerializedElement m_SerializedData;

        [SerializeField]
        string m_ShaderNodeTypeName;

        NodeTypeState m_TypeState;

        public HlslFunctionDescriptor function { get; set; }

        public NodeTypeState typeState
        {
            get => m_TypeState;
            set
            {
                m_TypeState = value;
                m_ShaderNodeTypeName = value?.baseNodeType.GetType().FullName;
            }
        }

        public bool isNew { get; set; }

        public object data
        {
            get => m_Data;
            set => m_Data = value;
        }

        public string shaderNodeTypeName => m_ShaderNodeTypeName;

        public override bool hasPreview => true;

        public ShaderNodeInstance()
        {
        }

        internal override void Setup(ref NodeDefinitionContext context)
        {
        }

        internal override void OnGenerateFunction(ref FunctionDefinitionContext context)
        {
            context.SetHlslFunction(function);
        }

        // TODO: This one is only really used in SearchWindowProvider, as we need a dummy node with slots for the code there.
        // Eventually we can make the code in SWP nicer, and remove this constructor.
        public ShaderNodeInstance(NodeTypeState typeState)
        {
            this.typeState = typeState;
            name = typeState.type.name;
            isNew = true;

            AddShaderValuesFromTypeDescriptor(typeState.type);
        }

        public override void ValidateNode()
        {
            base.ValidateNode();

            var errorDetected = true;
            if (owner == null)
                Debug.LogError($"{name} ({guid}) has a null owner.");
            else if (typeState == null)
                Debug.LogError($"{name} ({guid}) has a null state.");
            else if (typeState.owner != owner)
                Debug.LogError($"{name} ({guid}) has an invalid state.");
            else
                errorDetected = false;

            hasError |= errorDetected;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            if (data != null)
                m_SerializedData = SerializationHelper.Serialize(data);

            if (typeState != null)
                m_ShaderNodeTypeName = typeState.baseNodeType.GetType().FullName;
        }

        public override void UpdateNodeAfterDeserialization()
        {
            if (m_SerializedData.typeInfo.IsValid())
            {
                m_Data = SerializationHelper.Deserialize<object>(m_SerializedData, GraphUtil.GetLegacyTypeRemapping());
                m_SerializedData = default;
            }

            UpdateStateReference();
        }

        public override void UpdatePortDimension(int portId, int dimension)
        {
            // TODO - Revisit dynamics?
            // for (int i=0; i< typeState.inputPorts.Count; ++i)
            // {
            //     if (typeState.inputPorts[i].id == portId)
            //     {
            //         var port = typeState.inputPorts[i];
            //         var portValue = port.value;
            //         // Not sure what to do here. Constructing a new object seems better than adding a function to set the dimension.
            //         if (portValue.type == PortValueType.DynamicVector)
            //         {
            //             port.value = PortValue.DynamicVector(portValue.vector4Value, dimension);
            //             typeState.inputPorts[i] = port;
            //         }
            //         return;
            //     }
            // }
        }

        public override void UpdatePortConnection()
        {
            typeState.modifiedNodes.Add(tempId.index);
        }

        public void UpdateStateReference()
        {
            var materialOwner = (AbstractMaterialGraph)owner;
            typeState = materialOwner.nodeTypeStates.FirstOrDefault(x => x.baseNodeType.GetType().FullName == shaderNodeTypeName);
            if (typeState == null)
            {
                throw new InvalidOperationException($"Cannot find an {nameof(ShaderNodeType)} with type name {shaderNodeTypeName}");
            }
            AddShaderValuesFromTypeDescriptor(typeState.type);
        }

        public override void GetSourceAssetDependencies(List<string> paths)
        {
            foreach (var source in typeState.hlslSources)
            {
                if (source.type == HlslSourceType.File)
                {
                    paths.Add(source.value);
                }
            }
        }
    }
}
