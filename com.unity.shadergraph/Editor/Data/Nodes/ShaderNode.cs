using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    abstract class ShaderNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction
    {      
        public ShaderNode()
        {
            NodeSetupContext context = new NodeSetupContext();
            Setup(ref context);

            if(context.descriptor == null)
                return;

            name = context.descriptor.name;
            m_Preview = context.descriptor.preview;

            List<int> validPorts = new List<int>();
            for(int i = 0; i < context.descriptor.inPorts.Length; i++)
            {
                AddSlot(new ShaderPort(context.descriptor.inPorts[i]));
                validPorts.Add(context.descriptor.inPorts[i].id);
            }
            for(int i = 0; i < context.descriptor.outPorts.Length; i++)
            {
                AddSlot(new ShaderPort(context.descriptor.outPorts[i]));
                validPorts.Add(context.descriptor.outPorts[i].id);
            }
            RemoveSlotsNameNotMatching(validPorts);

            m_Parameters = new ShaderParameter[context.descriptor.parameters.Length];
            for(int i = 0; i < m_Parameters.Length; i++)
            {
                m_Parameters[i] = new ShaderParameter(context.descriptor.parameters[i]);
                m_Parameters[i].owner = this;
            }
        }

        private bool m_Preview;

        public override bool hasPreview
        {
            get { return m_Preview; }
        }

        [SerializeField]
        private ShaderParameter[] m_Parameters = new ShaderParameter[0];

        public ShaderParameter[] parameters
        {
            get { return m_Parameters; }
        }

        private string GetFunctionName(string name)
        {
            return string.Format("{0}_{1}", name, precision);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            NodeChangeContext context = new NodeChangeContext();
            OnModified(ref context);

            foreach (var outArgument in context.descriptor.outArguments)
                visitor.AddShaderChunk(NodeUtils.ConvertConcreteSlotValueTypeToString(precision, outArgument.valueType) + " " + GetVariableNameForSlot(outArgument.id) + ";", true);

            string call = GetFunctionName(context.descriptor.name) + "(";
            bool first = true;
            foreach (var argument in context.descriptor.inArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetShaderValue(argument.id, generationMode);
            }
            foreach (var argument in context.descriptor.outArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetVariableNameForSlot(argument.id);
            }
            call += ");";
            visitor.AddShaderChunk(call, true);
        }

        public virtual void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            NodeChangeContext context = new NodeChangeContext();
            OnModified(ref context);

            registry.ProvideFunction(GetFunctionName(context.descriptor.name), s =>
                {
                    s.AppendLine(GetFunctionHeader(context.descriptor));
                    using(s.BlockScope())
                    {
                        s.AppendLines(context.descriptor.body);
                    }
                });
        }

        public ShaderParameter FindParameter(int id)
        {
            foreach (var parameter in m_Parameters)
            {
                if (parameter.id == id)
                    return parameter;
            }
            return null;
        }

        public string GetShaderValue(int id, GenerationMode generationMode)
        {
            var parameter = FindParameter(id);
            if (parameter != null)
            {
                if (generationMode.IsPreview())
                    return string.Format("_{0}_{1}", GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(parameter.shaderOutputName));

                return ShaderValueAsVariable(parameter, precision);
            }

            var inputPort = FindSlot<ShaderPort>(id);
            if (inputPort == null)
                return string.Empty;

            var edges = owner.GetEdges(inputPort.slotReference).ToArray();

            if (edges.Any())
            {
                var fromSocketRef = edges[0].outputSlot;
                var fromNode = owner.GetNodeFromGuid<AbstractMaterialNode>(fromSocketRef.nodeGuid);
                if (fromNode == null)
                    return string.Empty;

                var slot = fromNode.FindOutputSlot<MaterialSlot>(fromSocketRef.slotId);
                if (slot == null)
                    return string.Empty;

                return ShaderGenerator.AdaptNodeOutput(fromNode, slot.id, inputPort.concreteValueType);
            }

            return inputPort.GetDefaultValue(generationMode);
        }

        // TODO: IShaderValue version of AbstractMaterialSlot.ConcreteSlotValueAsVariable
        protected string ShaderValueAsVariable(IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            var channelCount = SlotValueHelper.GetChannelCount(shaderValue.concreteValueType);
            Matrix4x4 matrix = shaderValue.value.matrixValue;
            switch (shaderValue.concreteValueType)
            {
                case ConcreteSlotValueType.Vector1:
                    return NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue.x);
                case ConcreteSlotValueType.Vector4:
                case ConcreteSlotValueType.Vector3:
                case ConcreteSlotValueType.Vector2:
                    {
                        string values = NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue.x);
                        for (var i = 1; i < channelCount; i++)
                            values += ", " + NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue[i]);
                        return string.Format("{0}{1}({2})", precision, channelCount, values);
                    }
                case ConcreteSlotValueType.Boolean:
                    return (shaderValue.value.booleanValue ? 1 : 0).ToString();
                case ConcreteSlotValueType.Texture2D:
                case ConcreteSlotValueType.Texture3D:
                case ConcreteSlotValueType.Texture2DArray:
                case ConcreteSlotValueType.Cubemap:
                case ConcreteSlotValueType.SamplerState:
                    return ShaderValueToVariableName(shaderValue);
                case ConcreteSlotValueType.Matrix2:
                    return string.Format("{0}2x2 ({1},{2},{3},{4})", precision, 
                        matrix.m00, matrix.m01, 
                        matrix.m10, matrix.m11);
                case ConcreteSlotValueType.Matrix3:
                    return string.Format("{0}3x3 ({1},{2},{3},{4},{5},{6},{7},{8},{9})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, 
                        matrix.m10, matrix.m11, matrix.m12,
                        matrix.m20, matrix.m21, matrix.m22);
                case ConcreteSlotValueType.Matrix4:
                    return string.Format("{0}4x4 ({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, matrix.m03, 
                        matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                        matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                        matrix.m30, matrix.m31, matrix.m32, matrix.m33);
                case ConcreteSlotValueType.Gradient:
                    return string.Format("Unity{0}()", ShaderValueToVariableName(shaderValue));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO: Override for collecting parameters
        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            base.CollectPreviewMaterialProperties(properties);

            foreach(ShaderParameter parameter in m_Parameters)
            {
                s_TempPreviewProperties.Clear();
                parameter.GetPreviewProperties(s_TempPreviewProperties, ShaderValueToVariableName(parameter));
                for (int i = 0; i < s_TempPreviewProperties.Count; i++)
                {
                    if (s_TempPreviewProperties[i].name == null)
                        continue;

                    properties.Add(s_TempPreviewProperties[i]);
                }
            }
        }

        // TODO: Override for collecting parameters
        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            base.CollectShaderProperties(properties, generationMode);

            foreach (var parameter in m_Parameters)
                parameter.AddDefaultProperty(properties, generationMode);
        }

        // TODO: Replaces AbstractMaterialNode.GetVariableNameForSlot
        private string ShaderValueToVariableName(IShaderValue shaderValue)
        {
            return string.Format("_{0}_{1}", GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(shaderValue.shaderOutputName));
        }

        private string GetFunctionHeader(HlslFunctionDescriptor descriptor)
        {
            string header = "void " + GetFunctionName(descriptor.name) + "(";

            var first = true;
            foreach (var argument in descriptor.inArguments)
            {
                if (!first)
                    header += ", ";
                first = false;
                header += string.Format("{0} {1}", NodeUtils.ConvertConcreteSlotValueTypeToString(precision, argument.valueType), argument.name);
            }
            foreach (var argument in descriptor.outArguments)
            {
                if (!first)
                    header += ", ";
                first = false;
                header += string.Format("out {0} {1}", NodeUtils.ConvertConcreteSlotValueTypeToString(precision, argument.valueType), argument.name);
            }

            header += ")";
            return header;
        }

        public abstract void Setup(ref NodeSetupContext context);
        public abstract void OnModified(ref NodeChangeContext context);
    }
}
