using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    abstract class ShaderNode : AbstractMaterialNode
    {
        [SerializeField]
        List<ShaderParameter> m_Parameters = new List<ShaderParameter>();

        internal List<ShaderParameter> parameters => m_Parameters;

        internal ShaderNode()
        {
            NodeDefinitionContext context = new NodeDefinitionContext();
            Setup(ref context);
            if(string.IsNullOrEmpty(context.type.name))
                return;

            name = context.type.name;
            AddShaderValuesFromTypeDescriptor(context.type);
        }

#region Initialization
        internal abstract void Setup(ref NodeDefinitionContext context);

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

            var validParameters = new List<Guid>();
            if(descriptor.parameters != null)
            {
                foreach (InputDescriptor parameter in descriptor.parameters)
                {
                    AddParameter(new ShaderParameter(parameter));
                    validParameters.Add(parameter.guid.guid);
                }
            }
            RemoveParametersNameNotMatching(validParameters);
        }
#endregion

#region ShaderValue
        internal IShaderValue GetShaderValue(IShaderValueDescriptor descriptor)
        {
            var parameter = FindParameter(descriptor.guid.guid);
            if(parameter != null)
                return parameter;

            var port = FindSlot<ShaderPort>(descriptor.id);
            if(port != null)
                return port;

            return null;
        }
#endregion

#region Parameters
        private void AddParameter(ShaderParameter parameter)
        {
            var addingParameter = parameter;
            var foundParameter = FindParameter(parameter.guid.guid);

            m_Parameters.RemoveAll(x => x.guid.guid == parameter.guid.guid);
            m_Parameters.Add(parameter);
            parameter.owner = this;

            Dirty(ModificationScope.Topological);

            if (foundParameter == null)
                return;

            addingParameter.CopyValuesFrom(foundParameter);
        }

        private void RemoveParameter(Guid parameterGuid)
        {
            m_Parameters.RemoveAll(x => x.guid.guid == parameterGuid);
            Dirty(ModificationScope.Topological);
        }

        private ShaderParameter FindParameter(Guid guid)
        {
            foreach (var parameter in m_Parameters)
            {
                if (parameter.guid.guid == guid)
                    return parameter;
            }
            return null;
        }

        private void RemoveParametersNameNotMatching(IEnumerable<Guid> parameterGuids, bool supressWarnings = false)
        {
            var invalidParameters = m_Parameters.Select(x => x.guid.guid).Except(parameterGuids);

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

                properties.Add(port.ToPreviewProperty(port.ToVariableSnippet()));
            }

            foreach(ShaderParameter parameter in m_Parameters)
                properties.Add(parameter.ToPreviewProperty(parameter.ToVariableSnippet()));
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            foreach (var port in this.GetInputSlots<ShaderInputPort>())
            {
                if(!port.HasEdges())
                {
                    string overrideReferenceName = port.ToVariableSnippet();
                    IShaderProperty[] defaultProperties = port.ToDefaultPropertyArray(overrideReferenceName);
                    foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
                }
            }

            foreach (var parameter in m_Parameters)
            {
                string overrideReferenceName = parameter.ToVariableSnippet();
                IShaderProperty[] defaultProperties = parameter.ToDefaultPropertyArray(overrideReferenceName);
                foreach(IShaderProperty property in defaultProperties)
                        properties.AddShaderProperty(property);
            }
        }
#endregion

    }
}
