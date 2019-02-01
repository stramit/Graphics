using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph.NodeLibrary
{
    sealed class CustomFunctionNode : ShaderNode, IHasSettings
    {
        [SerializeField]
        private List<InputDescriptor> m_InDescriptors = new List<InputDescriptor>();

        [SerializeField]
        private List<InputDescriptor> m_ParameterDescriptors = new List<InputDescriptor>();

        [SerializeField]
        private List<OutputDescriptor> m_OutDescriptors = new List<OutputDescriptor>();

        [SerializeField]
        private HlslFunctionDescriptor m_FunctionDescriptor;

        public HlslFunctionDescriptor functionDescriptor
        {
            get
            {
                if(m_FunctionDescriptor == null)
                {
                    m_FunctionDescriptor = new HlslFunctionDescriptor()
                    {
                        name = s_FunctionName,
                        source = HlslSource.File(s_FunctionSource, true)
                    };
                }
                m_FunctionDescriptor.inArguments = m_InDescriptors.Union(m_ParameterDescriptors).ToArray();
                m_FunctionDescriptor.outArguments = m_OutDescriptors.ToArray();
                return m_FunctionDescriptor;
            }
            set => m_FunctionDescriptor = value;
        }

        private static string s_FunctionName =>  "Function name here...";
        private static string s_FunctionSource => "Hlsl include file path here...";

#region Initialization
        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Custom Function",
                preview = true
            });
        }
#endregion

#region Validation
        public override void ValidateNode()
        {
            List<int> validShaderValues = new List<int>();
            for(int i = 0; i < m_InDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = m_InDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Input);
                validShaderValues.Add(descriptor.id);
                m_InDescriptors[i] = (InputDescriptor)descriptor;
            }
            for(int i = 0; i < m_OutDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = m_OutDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Output);
                validShaderValues.Add(descriptor.id);
                m_OutDescriptors[i] = (OutputDescriptor)descriptor;
            }
            for(int i = 0; i < m_ParameterDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = m_ParameterDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Parameter);
                validShaderValues.Add(descriptor.id);
                m_ParameterDescriptors[i] = (InputDescriptor)descriptor;
            }
            RemoveShaderValuesNotMatching(validShaderValues);

            base.ValidateNode();
        }

        private void ValidateDescriptor(ref IShaderValueDescriptor descriptor)
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

        internal override bool IsValidFunctionDescriptor(HlslFunctionDescriptor descriptor)
        {
            return (!string.IsNullOrEmpty(descriptor.name) &&
                !string.IsNullOrEmpty(descriptor.source.value) &&
                descriptor.name != s_FunctionName &&
                descriptor.source.value != s_FunctionSource);
        }
#endregion

#region Function
        internal override void OnGenerateFunction(ref FunctionDefinitionContext context)
        {
            context.SetHlslFunction(functionDescriptor);
        }
#endregion

#region Views
        public VisualElement CreateSettingsElement()
        {
            PropertySheet ps = new PropertySheet();
            ps.style.width = 400;
            ps.Add(new InputDescriptorListView(this, m_InDescriptors, ShaderValueDescriptorType.Input));
            ps.Add(new OutputDescriptorListView(this, m_OutDescriptors));
            ps.Add(new InputDescriptorListView(this, m_ParameterDescriptors, ShaderValueDescriptorType.Parameter));
            ps.Add(new HlslSourceView(this, functionDescriptor));
            return ps;
        }
#endregion

    }
}
