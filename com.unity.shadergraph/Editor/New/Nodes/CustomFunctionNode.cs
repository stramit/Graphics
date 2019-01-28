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
    sealed class CustomFunctionNode : ShaderNode, IHasSettings, IGeneratesBodyCode, IGeneratesFunction
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

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Custom Function",
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

        private bool IsValidFunctionDescriptor()
        {
            return (!string.IsNullOrEmpty(functionDescriptor.name) &&
                !string.IsNullOrEmpty(functionDescriptor.source.value) &&
                functionDescriptor.name != s_FunctionName &&
                functionDescriptor.source.value != s_FunctionSource);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            if(!IsValidFunctionDescriptor())
                return;

            foreach (var argument in functionDescriptor.outArguments)
                visitor.AddShaderChunk(argument.valueType.ToString(precision) + " " + GetShaderValue(argument).ToVariableName() + ";", true);

            string call = GetFunctionName(functionDescriptor.name) + "(";
            bool first = true;
            foreach (var argument in functionDescriptor.inArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                IShaderValue shaderValue = GetShaderValue(argument);
                call += shaderValue.ToVariableReference(precision, generationMode);
            }
            foreach (var argument in functionDescriptor.outArguments)
            {
                if (!first)
                    call += ", ";
                first = false;
                call += GetShaderValue(argument).ToVariableName();
            }
            call += ");";
            visitor.AddShaderChunk(call, true);
        }

        public void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            if(!IsValidFunctionDescriptor())
                return;

            registry.ProvideFunction(functionDescriptor.name, builder =>
            {
                switch (functionDescriptor.source.type)
                {
                    case HlslSourceType.File:
                        builder.AppendLine($"#include \"{functionDescriptor.source.value}\"");
                        break;
                    case HlslSourceType.String:
                        builder.AppendLine(GetFunctionHeader(functionDescriptor));
                        using(builder.BlockScope())
                        {
                            builder.AppendLines(functionDescriptor.source.value);
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
    }
}