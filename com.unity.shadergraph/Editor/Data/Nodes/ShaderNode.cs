using System.Collections;
using System.Collections.Generic;
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
        }

        private bool m_Preview;

        public override bool hasPreview
        {
            get { return m_Preview; }
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
                call += GetSlotValue(argument.id, generationMode);
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
