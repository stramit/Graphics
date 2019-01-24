using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal abstract class ShaderNodeInstance : ShaderNode, IGeneratesBodyCode, IGeneratesFunction
    {      
        internal ShaderNodeInstance()
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

        private bool m_Preview;
        public override bool hasPreview
        {
            get { return m_Preview; }
        }

        internal abstract void Setup(ref NodeSetupContext context);
        internal abstract void OnModified(ref NodeChangeContext context);

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
    }
}
