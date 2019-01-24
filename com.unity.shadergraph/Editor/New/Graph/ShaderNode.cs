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

            List<int> validParameters = new List<int>();
            for(int i = 0; i <context.descriptor.parameters.Length; i++)
            {
                AddParameter(new ShaderParameter(context.descriptor.parameters[i]));
                validParameters.Add(context.descriptor.parameters[i].id);
            }
            RemoveParametersNameNotMatching(validParameters);
        }

        [SerializeField]
        private List<ShaderParameter> m_Parameters = new List<ShaderParameter>();

        private bool m_Preview;

        public List<ShaderParameter> parameters
        {
            get { return m_Parameters; }
        }

        public override bool hasPreview
        {
            get { return m_Preview; }
        }

        public abstract void Setup(ref NodeSetupContext context);
        public abstract void OnModified(ref NodeChangeContext context);

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            NodeChangeContext context = new NodeChangeContext();
            OnModified(ref context);
            if(context.descriptor == null)
                return;

            foreach (var argument in context.descriptor.outArguments)
                visitor.AddShaderChunk(argument.valueType.ToString(precision) + " " + FindShaderValue(argument.id).ToShaderVariableName() + ";", true);

            string call = GetFunctionName(context.descriptor.name) + "(";
            bool first = true;
            foreach (var argument in context.descriptor.inArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetShaderValueString(argument.id, generationMode);
            }
            foreach (var argument in context.descriptor.outArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += FindShaderValue(argument.id).ToShaderVariableName();
            }
            call += ");";
            visitor.AddShaderChunk(call, true);
        }

        public virtual void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            NodeChangeContext context = new NodeChangeContext();
            OnModified(ref context);
            if(context.descriptor == null)
                return;

            registry.ProvideFunction(GetFunctionName(context.descriptor.name), s =>
                {
                    s.AppendLine(GetFunctionHeader(context.descriptor));
                    using(s.BlockScope())
                    {
                        s.AppendLines(context.descriptor.body);
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

        public void AddParameter(ShaderParameter parameter)
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

        public void RemoveParameter(int parameterId)
        {
            m_Parameters.RemoveAll(x => x.id == parameterId);
            Dirty(ModificationScope.Topological);
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

        public void RemoveParametersNameNotMatching(IEnumerable<int> parameterIds, bool supressWarnings = false)
        {
            var invalidParameters = m_Parameters.Select(x => x.id).Except(parameterIds);

            foreach (var invalidParameter in invalidParameters.ToArray())
            {
                if (!supressWarnings)
                    Debug.LogWarningFormat("Removing Invalid Parameter: {0}", invalidParameter);
                RemoveSlot(invalidParameter);
            }
        }

        public IShaderValue FindShaderValue(int id)
        {
            var parameter = FindParameter(id);
            if(parameter != null)
                return parameter;

            var port = FindSlot<ShaderPort>(id);
            if(port != null)
                return port;

            return null;
        }

        public string GetShaderValueString(int id, GenerationMode generationMode)
        {
            var parameter = FindParameter(id);
            if (parameter != null)
            {
                if (generationMode.IsPreview())
                    return parameter.ToShaderVariableName();

                return parameter.ToShaderVariableValue(precision);
            }

            var port = FindSlot<ShaderPort>(id) as ShaderPort;
            if (port != null)
                return port.InputValue(owner, generationMode);

            return string.Empty;
        }

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            s_TempSlots.Clear();
            GetInputSlots(s_TempSlots);
            foreach (var slot in s_TempSlots)
            {
                ShaderPort port = slot as ShaderPort;
                if(port.HasEdges())
                    return;
                
                properties.Add(port.ToPreviewProperty(port.ToShaderVariableName()));
            }

            foreach(ShaderParameter parameter in m_Parameters)
                properties.Add(parameter.ToPreviewProperty(parameter.ToShaderVariableName()));
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            foreach (var port in this.GetInputSlots<ShaderPort>())
            {
                if(!port.HasEdges())
                {
                    string overrideReferenceName = port.ToShaderVariableName();
                    IShaderProperty[] defaultProperties = port.ToDefaultPropertyArray(overrideReferenceName);
                    foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
                }
            }

            foreach (var parameter in m_Parameters)
            {
                string overrideReferenceName = parameter.ToShaderVariableName();
                IShaderProperty[] defaultProperties = parameter.ToDefaultPropertyArray(overrideReferenceName);
                foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
            }
        }
    }
}
