using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    abstract class ShaderNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction
    {
        [SerializeField]
        List<ShaderParameter> m_Parameters = new List<ShaderParameter>();

        [SerializeField]
        private bool m_Preview;

        internal List<ShaderParameter> parameters => m_Parameters;

        public override bool hasPreview => m_Preview;

        internal ShaderNode()
        {
            NodeDefinitionContext context = new NodeDefinitionContext();
            Setup(ref context);
            if(string.IsNullOrEmpty(context.type.name))
                return;

            name = context.type.name;
            m_Preview = context.type.preview;
            AddShaderValuesFromTypeDescriptor(context.type);
        }

        internal abstract void Setup(ref NodeDefinitionContext context);

        internal virtual void OnGenerateFunction(ref FunctionDefinitionContext context)
        {
        }

#region Initialization
        internal void AddShaderValuesFromTypeDescriptor(NodeTypeDescriptor descriptor)
        {
            var validSlotIds = new List<int>();
            if(descriptor.inPorts != null)
            {
                foreach (InputDescriptor input in descriptor.inPorts)
                {
                    AddSlot(new ShaderInputPort(input));
                    validSlotIds.Add(input.id);
                }
            }
            if(descriptor.outPorts != null)
            {
                foreach (OutputDescriptor output in descriptor.outPorts)
                {
                    AddSlot(new ShaderPort(output));
                    validSlotIds.Add(output.id);
                }
            }
            RemoveSlotsNameNotMatching(validSlotIds);

            var validParameters = new List<int>();
            if(descriptor.parameters != null)
            {
                foreach (InputDescriptor parameter in descriptor.parameters)
                {
                    AddParameter(new ShaderParameter(parameter));
                    validParameters.Add(parameter.id);
                }
            }
            RemoveParametersNameNotMatching(validParameters);
        }
#endregion

#region Validation
        internal virtual bool IsValidFunctionDescriptor(HlslFunctionDescriptor descriptor)
        {
            return (descriptor != null &&
                !string.IsNullOrEmpty(descriptor.name) &&
                !string.IsNullOrEmpty(descriptor.source.value));
        }
#endregion

#region NodeCode
        public virtual void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            FunctionDefinitionContext context = new FunctionDefinitionContext();
            OnGenerateFunction(ref context);
            if(!IsValidFunctionDescriptor(context.function))
                return;

            foreach (var argument in context.function.outArguments)
            {
                var shaderValue = GetShaderValue(argument);
                visitor.AddShaderChunk(string.Format("{0};", shaderValue.ToVariableDefinitionSnippet(precision)));
            }

            string call = GetFunctionName(context.function.name) + "(";
            bool first = true;
            foreach (var argument in context.function.inArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                IShaderValue shaderValue = GetShaderValue(argument);
                call += shaderValue.ToValueReferenceSnippet(precision, generationMode);
            }
            foreach (var argument in context.function.outArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetShaderValue(argument).ToVariableNameSnippet();
            }
            call += ");";
            visitor.AddShaderChunk(call, true);
        }

        private string GetFunctionName(string name)
        {
            return string.Format("{0}_{1}", name, precision);
        }
#endregion

#region Function
        public virtual void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            FunctionDefinitionContext context = new FunctionDefinitionContext();
            OnGenerateFunction(ref context);
            if(!IsValidFunctionDescriptor(context.function))
                return;

            registry.ProvideFunction(context.function.name, builder =>
            {
                switch (context.function.source.type)
                {
                    case HlslSourceType.File:
                        builder.AppendLine($"#include \"{context.function.source.value}\"");
                        break;
                    case HlslSourceType.String:
                        builder.AppendLine(GetFunctionHeader(context.function));
                        using(builder.BlockScope())
                        {
                            builder.AppendLines(context.function.source.value);
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
                header += string.Format("{0} {1}", argument.valueType.ToConcreteValueType().ToString(precision), argument.name);
            }
            foreach (var argument in descriptor.outArguments)
            {
                if (!first)
                    header += ", ";
                first = false;
                header += string.Format("out {0} {1}", argument.valueType.ToConcreteValueType().ToString(precision), argument.name);
            }
            header += ")";
            return header;
        }
#endregion

#region ShaderValue
        internal void AddShaderValue(IShaderValueDescriptor descriptor, ShaderValueDescriptorType type)
        {
            switch(type)
            {
                case ShaderValueDescriptorType.Input:
                    AddSlot(new ShaderInputPort((InputDescriptor)descriptor));
                    break;
                case ShaderValueDescriptorType.Output:
                    AddSlot(new ShaderPort((OutputDescriptor)descriptor));
                    break;
                case ShaderValueDescriptorType.Parameter:
                    AddParameter(new ShaderParameter((InputDescriptor)descriptor));
                    break;
            }
        }

        internal IShaderValue GetShaderValue(IShaderValueDescriptor descriptor)
        {
            var parameter = FindParameter(descriptor.id);
            if(parameter != null)
                return parameter;

            var port = FindSlot<ShaderPort>(descriptor.id);
            if(port != null)
                return port;

            return null;
        }

        internal List<IShaderValue> GetShaderValues()
        {
            List<ShaderPort> ports = new List<ShaderPort>();
            GetSlots(ports);
            List<ShaderParameter> parameters = new List<ShaderParameter>();
            GetParameters(parameters);
            return ports.ToList<IShaderValue>().Union(parameters.ToList<IShaderValue>()).ToList();
        }

        internal void RemoveShaderValuesNotMatching(IEnumerable<int> shaderValueIds, bool supressWarnings = false)
        {
            RemoveParametersNameNotMatching(shaderValueIds, supressWarnings);
            RemoveSlotsNameNotMatching(shaderValueIds, supressWarnings);
        }
#endregion

#region Parameters
        private void AddParameter(ShaderParameter parameter)
        {
            var addingParameter = parameter;
            var foundParameter = FindParameter(parameter.id);

            m_Parameters.RemoveAll(x => x.id == parameter.id);
            m_Parameters.Add(parameter);
            parameter.owner = this;

            Dirty(ModificationScope.Topological);

            if (foundParameter == null)
                return;

            addingParameter.CopyValuesFrom(foundParameter);
        }

        private void RemoveParameter(int parameterId)
        {
            m_Parameters.RemoveAll(x => x.id == parameterId);
            Dirty(ModificationScope.Topological);
        }

        internal void GetParameters(List<ShaderParameter> foundSlots)
        {
            foreach (var slot in m_Parameters)
            {
                foundSlots.Add(slot);
            }
        }

        private ShaderParameter FindParameter(int id)
        {
            foreach (var parameter in m_Parameters)
            {
                if (parameter.id == id)
                    return parameter;
            }
            return null;
        }

        private void RemoveParametersNameNotMatching(IEnumerable<int> parameterIds, bool supressWarnings = false)
        {
            var invalidParameters = m_Parameters.Select(x => x.id).Except(parameterIds);

            foreach (var invalidParameter in invalidParameters.ToArray())
            {
                if (!supressWarnings)
                    Debug.LogWarningFormat("Removing Invalid Parameter: {0}", invalidParameter);
                RemoveParameter(invalidParameter);
            }
        }
#endregion       

#region Properties
        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            s_TempSlots.Clear();
            GetInputSlots(s_TempSlots);
            foreach (var slot in s_TempSlots)
            {
                ShaderInputPort port = slot as ShaderInputPort;
                if(port.HasEdges())
                    continue;

                properties.Add(port.ToPreviewProperty(port.ToVariableNameSnippet()));
            }

            foreach(ShaderParameter parameter in m_Parameters)
                properties.Add(parameter.ToPreviewProperty(parameter.ToVariableNameSnippet()));
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            foreach (var port in this.GetInputSlots<ShaderInputPort>())
            {
                if(!port.HasEdges())
                {
                    string overrideReferenceName = port.ToVariableNameSnippet();
                    IShaderProperty[] defaultProperties = port.ToDefaultPropertyArray(overrideReferenceName);
                    foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
                }
            }

            foreach (var parameter in m_Parameters)
            {
                string overrideReferenceName = parameter.ToVariableNameSnippet();
                IShaderProperty[] defaultProperties = parameter.ToDefaultPropertyArray(overrideReferenceName);
                foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
            }
        }
#endregion

    }
}
