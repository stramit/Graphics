using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    class Generator
    {
        const string kShaderTemplatePath = "Packages/com.unity.shadergraph/Editor/Templates/DefaultShader.template";

        ShaderStringBuilder m_Builder;
        GeneratorContext m_Context;
        IEnumerable<IGraphTarget> m_Targets;
        List<string> m_AssetDependencyPaths = new List<string>();

        internal Generator(GraphData graph, IEnumerable<IGraphTarget> targets = null)
        {
            // Setup
            m_Context = new GeneratorContext(graph.GetGraphInfo());
            m_Builder = new ShaderStringBuilder();

            // Get Targets
            if(targets == null)
            {
                targets = graph.targets;
            }
            m_Targets = targets;

            // Iterate Targets
            foreach(IGraphTarget target in targets)
            {
                // Generate
                target.Generate(m_Context);

                // Dependencies
                if(target.assetDependencyPaths != null)
                {
                    m_AssetDependencyPaths.AddRange(target.assetDependencyPaths);
                }
                foreach(ShaderPass pass in m_Context.passes)
                {
                    m_AssetDependencyPaths.Add(pass.templatePath);
                    m_AssetDependencyPaths.AddRange(pass.includes);
                }
            }
        }

        public string GetShaderCode()
        {
            BuildShaderCode();
            return m_Builder.ToString();
        }

        void BuildShaderCode()
        {
            // Read Template
            string template = File.ReadAllText(kShaderTemplatePath);
            m_Builder.AppendLines(template);

            // Properties
            // MATT: Splice Properties
            
            // Targets
            using(var targetSplice = new Splice("Targets", m_Builder))
            {
                foreach(IGraphTarget target in m_Targets)
                {
                    BuildTargetCode(target, targetSplice.builder);
                }
            }

            // Editor
            using(var editorSplice = new Splice("Editor", m_Builder))
            {
                string editorPath = string.Empty;
                if(!string.IsNullOrEmpty(m_Context.defaultEditorPath))
                {
                    editorPath = $"CustomEditor \"{m_Context.defaultEditorPath}\"";
                }

                editorSplice.builder.AppendLine(editorPath);
            }

            // Fallback
            using(var fallbackSplice = new Splice("Fallback", m_Builder))
            {
                fallbackSplice.builder.AppendLine("FallBack \"Hidden/InternalErrorShader\"");
            }
        }

        void BuildTargetCode(IGraphTarget target, ShaderStringBuilder builder)
        {
            // Read Template
            string template = File.ReadAllText(target.targetTemplatePath);
            builder.AppendLines(template);

            // Tags
            // using(var tagSplice = new Splice("Tags", builder))
            // {
            //     tags.GetTags(tagSplice.builder, m_Context.pipelineTag);
            // }

            // Passes
            using(var passSplice = new Splice("Passes", builder))
            {
                if(m_Context.passes != null)
                {
                    foreach(ShaderPass pass in m_Context.passes)
                    {
                        BuildPassCode(pass, passSplice.builder);
                    }
                }
            }
        }

        void BuildPassCode(ShaderPass pass, ShaderStringBuilder builder)
        {
            // Read Template
            string template = File.ReadAllText(pass.templatePath);
            builder.AppendLines(template);

            // Pragmas
            using(var pragmaSplice = new Splice("Pragmas", builder))
            {
                if(pass.pragmas != null)
                {
                    foreach(string pragma in pass.pragmas)
                    {
                        pragmaSplice.builder.AppendLine($"#pragma {pragma}");
                    }
                }
            }

            // Keywords
            using(var keywordSplice = new Splice("Keywords", builder))
            {
                if(pass.keywords != null)
                {
                    foreach(KeywordDescriptor keyword in pass.keywords)
                    {
                        keywordSplice.builder.AppendLine(keyword.ToDeclarationString());
                    }
                }
            }

            // Includes
            using(var includeSplice = new Splice("Includes", builder))
            {
                if(pass.includes != null)
                {
                    foreach(string include in pass.includes)
                    {
                        includeSplice.builder.AppendLine($"#include \"{include}\"");
                    }
                }
            }
        }
    }
}
