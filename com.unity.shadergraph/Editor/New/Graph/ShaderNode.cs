using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal abstract class ShaderNode : AbstractMaterialNode
    {      
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

        [SerializeField]
        private List<ShaderParameter> m_Parameters = new List<ShaderParameter>();

        internal List<ShaderParameter> parameters => m_Parameters;

        private bool m_Preview;
        public override bool hasPreview => m_Preview;

        internal void AddShaderValuesFromTypeDescriptor(NodeTypeDescriptor descriptor)
        {
            var validSlotIds = new List<int>();
            if(descriptor.inPorts != null)
            {
                foreach (InputDescriptor input in descriptor.inPorts)
                {
                    AddSlot(new ShaderPort(input));
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

        private void RemoveParameter(int parameterId)
        {
            m_Parameters.RemoveAll(x => x.id == parameterId);
            Dirty(ModificationScope.Topological);
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

        internal void GetParameters(List<ShaderParameter> foundSlots)
        {
            foreach (var slot in m_Parameters)
            {
                foundSlots.Add(slot);
            }
        }

        public void RemoveShaderValuesNotMatching(IEnumerable<int> shaderValueIds, bool supressWarnings = false)
        {
            RemoveParametersNameNotMatching(shaderValueIds, supressWarnings);
            RemoveSlotsNameNotMatching(shaderValueIds, supressWarnings);
        }

        public void RemoveParametersNameNotMatching(IEnumerable<int> parameterIds, bool supressWarnings = false)
        {
            var invalidParameters = m_Parameters.Select(x => x.id).Except(parameterIds);

            foreach (var invalidParameter in invalidParameters.ToArray())
            {
                if (!supressWarnings)
                    Debug.LogWarningFormat("Removing Invalid Parameter: {0}", invalidParameter);
                RemoveParameter(invalidParameter);
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

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            s_TempSlots.Clear();
            GetInputSlots(s_TempSlots);
            foreach (var slot in s_TempSlots)
            {
                ShaderPort port = slot as ShaderPort;
                if(port.HasEdges())
                    continue;
                
                properties.Add(port.ToPreviewProperty(port.ToVariableName()));
            }

            foreach(ShaderParameter parameter in m_Parameters)
                properties.Add(parameter.ToPreviewProperty(parameter.ToVariableName()));
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            foreach (var port in this.GetInputSlots<ShaderPort>())
            {
                if(!port.HasEdges())
                {
                    string overrideReferenceName = port.ToVariableName();
                    IShaderProperty[] defaultProperties = port.ToDefaultPropertyArray(overrideReferenceName);
                    foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
                }
            }

            foreach (var parameter in m_Parameters)
            {
                string overrideReferenceName = parameter.ToVariableName();
                IShaderProperty[] defaultProperties = parameter.ToDefaultPropertyArray(overrideReferenceName);
                foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
            }
        }
    }
}
