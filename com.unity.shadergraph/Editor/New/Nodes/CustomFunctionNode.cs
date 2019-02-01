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
        private List<MaterialSlot> m_TempSlots = new List<MaterialSlot>();
        private List<ShaderParameter> m_TempParameters = new List<ShaderParameter>();

        private List<InputDescriptor> m_InDescriptors;

        public List<InputDescriptor> inDescriptors
        {
            get
            {
                if(m_InDescriptors == null)
                {
                    m_InDescriptors = new List<InputDescriptor>();
                    m_TempSlots.Clear();
                    GetInputSlots(m_TempSlots);
                    for(int i = 0; i < m_TempSlots.Count; i++)
                    {
                        if(m_TempSlots[i] is ShaderInputPort port)
                            m_InDescriptors.Add(new InputDescriptor(port.id, port.RawDisplayName(), port.valueType, port.control));
                    }
                }
                return m_InDescriptors;
            }
            set => m_InDescriptors = value;
        }

        private List<OutputDescriptor> m_OutDescriptors;

        public List<OutputDescriptor> outDescriptors
        {
            get
            {
                if(m_OutDescriptors == null)
                {
                    m_OutDescriptors = new List<OutputDescriptor>();
                    m_TempSlots.Clear();
                    GetOutputSlots(m_TempSlots);
                    for(int i = 0; i < m_TempSlots.Count; i++)
                    {
                        if(m_TempSlots[i] is ShaderPort port)
                            m_OutDescriptors.Add(new OutputDescriptor(port.id, port.RawDisplayName(), port.valueType));
                    }
                }
                return m_OutDescriptors;
            }
            set => m_OutDescriptors = value;
        }

        private List<InputDescriptor> m_ParameterDescriptors;

        public List<InputDescriptor> parameterDescriptors
        {
            get
            {
                if(m_ParameterDescriptors == null)
                {
                    m_ParameterDescriptors = new List<InputDescriptor>();
                    m_TempParameters.Clear();
                    GetParameters(m_TempParameters);
                    for(int i = 0; i < m_TempParameters.Count; i++)
                    {
                        if(m_TempParameters[i] is ShaderParameter parameter)
                            m_ParameterDescriptors.Add(new InputDescriptor(parameter.id, parameter.displayName, parameter.valueType, parameter.control));
                    }
                }
                return m_ParameterDescriptors;
            }
            set => m_ParameterDescriptors = value;
        }

        [SerializeField]
        private HlslSourceType m_SourceType;

        [SerializeField]
        private string m_FunctionName;

        [SerializeField]
        private string m_FunctionSource;

        private HlslFunctionDescriptor m_FunctionDescriptor;

        public HlslFunctionDescriptor functionDescriptor
        {
            get
            {
                if(string.IsNullOrEmpty(m_FunctionDescriptor.name))
                {
                    if(string.IsNullOrEmpty(m_FunctionName) && string.IsNullOrEmpty(m_FunctionSource))
                    {
                        m_FunctionDescriptor = new HlslFunctionDescriptor()
                        {
                            name = s_FunctionName,
                            source = HlslSource.File(s_FunctionSource, true)
                        };
                    }
                    else
                    {
                        switch(m_SourceType)
                        {
                            case HlslSourceType.File:
                                m_FunctionDescriptor = new HlslFunctionDescriptor()
                                {
                                    name = m_FunctionName,
                                    source = HlslSource.File(m_FunctionSource, true)
                                };
                                break;
                            case HlslSourceType.String:
                                m_FunctionDescriptor = new HlslFunctionDescriptor()
                                {
                                    name = m_FunctionName,
                                    source = HlslSource.String(m_FunctionSource, true)
                                };
                                break;
                        }
                    }
                }
                m_FunctionDescriptor.inArguments = inDescriptors.Union(parameterDescriptors).ToArray();
                m_FunctionDescriptor.outArguments = outDescriptors.ToArray();
                return m_FunctionDescriptor;
            }
            set
            {
                m_FunctionDescriptor = value;
                m_FunctionName = value.name;
                m_SourceType = value.source.type;
                m_FunctionSource = value.source.value;
            }
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
            for(int i = 0; i < inDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = inDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Input);
                validShaderValues.Add(descriptor.id);
                inDescriptors[i] = (InputDescriptor)descriptor;
            }
            for(int i = 0; i < outDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = outDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Output);
                validShaderValues.Add(descriptor.id);
                outDescriptors[i] = (OutputDescriptor)descriptor;
            }
            for(int i = 0; i < parameterDescriptors.Count; i++)
            {
                IShaderValueDescriptor descriptor = parameterDescriptors[i];
                ValidateDescriptor(ref descriptor);
                AddShaderValue(descriptor, ShaderValueDescriptorType.Parameter);
                validShaderValues.Add(descriptor.id);
                parameterDescriptors[i] = (InputDescriptor)descriptor;
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
            ps.Add(new InputDescriptorListView(this, inDescriptors, ShaderValueDescriptorType.Input));
            ps.Add(new OutputDescriptorListView(this, outDescriptors));
            ps.Add(new InputDescriptorListView(this, parameterDescriptors, ShaderValueDescriptorType.Parameter));
            HlslFunctionDescriptor descriptor = functionDescriptor;
            ps.Add(new HlslSourceView(this, ref descriptor));
            functionDescriptor = descriptor;
            return ps;
        }
#endregion

    }
}
