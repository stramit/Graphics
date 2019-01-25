using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal class ShaderNodeInstance : ShaderNode, IGeneratesBodyCode, IGeneratesFunction
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
            /*for (int i=0; i< typeState.inputPorts.Count; ++i)
            {
                if (typeState.inputPorts[i].id == portId)
                {
                    var port = typeState.inputPorts[i];
                    var portValue = port.value;
                    // Not sure what to do here. Constructing a new object seems better than adding a function to set the dimension.
                    if (portValue.type == PortValueType.DynamicVector)
                    {
                        port.value = PortValue.DynamicVector(portValue.vector4Value, dimension);
                        typeState.inputPorts[i] = port;
                    }
                    return;
                }
            }*/
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

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            foreach (var argument in function.outArguments)
                visitor.AddShaderChunk(argument.valueType.ToString(precision) + " " + GetShaderValue(argument).ToShaderVariableName() + ";", true);

            string call = GetFunctionName(function.name) + "(";
            bool first = true;
            foreach (var argument in function.inArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                IShaderValue shaderValue = GetShaderValue(argument);
                call += shaderValue.ToVariableReference(precision, generationMode);
            }
            foreach (var argument in function.outArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetShaderValue(argument).ToShaderVariableName();
            }
            call += ");";
            visitor.AddShaderChunk(call, true);
        }

        public virtual void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            registry.ProvideFunction(function.name, builder =>
            {
                switch (function.source.type)
                {
                    case HlslSourceType.File:
                        builder.AppendLine($"#include \"{function.source.value}\"");
                        break;
                    case HlslSourceType.String:
                        builder.AppendLine(GetFunctionHeader(function));
                        using(builder.BlockScope())
                        {
                            builder.AppendLines(function.source.value);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private string GetFunctionHeader(HlslFunctionDescriptor descriptor)
        {
            string header = string.Format("void {0}_{1}(", descriptor.name, precision);
            var first = true;
            foreach (var argument in descriptor.inArguments)
            {
                if (!first)
                    header += ", ";
                first = false;
                header += string.Format("{0} {1}", argument.valueType.ToString(precision), argument.name);
            }
            foreach (var argument in descriptor.outArguments)
            {
                if (!first)
                    header += ", ";
                first = false;
                header += string.Format("out {0} {1}", argument.valueType.ToString(precision), argument.name);
            }
            header += ")";
            return header;
        }

        private string GetFunctionName(string name)
        {
            return string.Format("{0}_{1}", name, precision);
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
