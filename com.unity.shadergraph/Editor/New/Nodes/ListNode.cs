using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph.NodeLibrary
{
    sealed class ListNode : ShaderNode, IHasSettings
    {
        [SerializeField]
        public List<InputDescriptor> m_InDescriptors = new List<InputDescriptor>();

        [SerializeField]
        public List<InputDescriptor> m_ParameterDescriptors = new List<InputDescriptor>();

        [SerializeField]
        public List<OutputDescriptor> m_OutDescriptors = new List<OutputDescriptor>();

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "List",
                preview = true
            });
        }

        public override void ValidateNode()
        {
            List<int> validShaderValues = new List<int>();
            foreach(InputDescriptor descriptor in m_InDescriptors)
            {
                ValidateDescriptor(descriptor);
                AddSlot(new ShaderPort(descriptor));
                validShaderValues.Add(descriptor.id);
            }
            foreach(OutputDescriptor descriptor in m_OutDescriptors)
            {
                ValidateDescriptor(descriptor);
                AddSlot(new ShaderPort(descriptor));
                validShaderValues.Add(descriptor.id);
            }
            foreach(InputDescriptor descriptor in m_ParameterDescriptors)
            {
                ValidateDescriptor(descriptor);
                AddParameter(new ShaderParameter(descriptor));
                validShaderValues.Add(descriptor.id);
            }
            RemoveShaderValuesNotMatching(validShaderValues);
            
            base.ValidateNode();
        }

        private void ValidateDescriptor(IShaderValueDescriptor descriptor)
        {
            if(descriptor.id != -1)
                return;

            int idCeiling = -1;
            int duplicateNameCeiling = 0;
            var shaderValues = GetShaderValues();
            foreach(IShaderValue value in shaderValues)
            {
                idCeiling = value.id > idCeiling ? value.id : idCeiling;
                if(value.displayName.StartsWith("New"))
                    duplicateNameCeiling++;
            }
            descriptor.id = idCeiling + 1;
            descriptor.name = "New";
            if(duplicateNameCeiling > 0)
                descriptor.name += string.Format(" ({0})", duplicateNameCeiling);
        }

        public VisualElement CreateSettingsElement()
        {
            PropertySheet ps = new PropertySheet();
            ps.style.width = 400;
            ps.Add(new InputDescriptorListView(this, m_InDescriptors, ShaderValueDescriptorType.Input));
            ps.Add(new OutputDescriptorListView(this, m_OutDescriptors));
            ps.Add(new InputDescriptorListView(this, m_ParameterDescriptors, ShaderValueDescriptorType.Parameter));
            return ps;
        }
    }
}