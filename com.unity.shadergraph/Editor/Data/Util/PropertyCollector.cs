using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditor.ShaderGraph
{
    class PropertyCollector
    {
        public struct TextureInfo
        {
            public string name;
            public int textureId;
            public bool modifiable;
        }

        public readonly List<AbstractShaderProperty> properties = new List<AbstractShaderProperty>();

        public void AddShaderProperty(AbstractShaderProperty chunk)
        {
            if (properties.Any(x => x.referenceName == chunk.referenceName))
                return;
            properties.Add(chunk);
        }

        public string GetPropertiesBlock(int baseIndentLevel)
        {
            var sb = new StringBuilder();
            foreach (var prop in properties.Where(x => x.generatePropertyBlock))
            {
                for (var i = 0; i < baseIndentLevel; i++)
                {
                    //sb.Append("\t");
                    sb.Append("    "); // unity convention use space instead of tab...
                }
                sb.AppendLine(prop.GetPropertyBlockString());
            }
            return sb.ToString();
        }

        public void GetPropertiesDeclaration(ShaderSnippetRegistry registry, GenerationMode mode)
        {
            var batchAll = mode == GenerationMode.Preview;
            
            using(registry.ProvideSnippet("PerMaterialCBuffer_Start", Guid.Empty, out var s))
            {
                s.AppendLine("CBUFFER_START(UnityPerMaterial)");
                s.IncreaseIndent();
            }
            
            foreach (var prop in properties.Where(n => batchAll || (n.generatePropertyBlock && n.isBatchable)))
            {
                using(registry.ProvideSnippet(string.Format("PerMaterialCBuffer_{0}", prop.referenceName), prop.guid, out var s))
                {
                    s.AppendLine(prop.GetPropertyDeclarationString());
                }
            }
            
            using(registry.ProvideSnippet("PerMaterialCBuffer_End", Guid.Empty, out var s))
            {
                s.DecreaseIndent();
                s.AppendLine("CBUFFER_END");
                s.AppendNewLine();
            }

            if (batchAll)
                return;
            
            foreach (var prop in properties.Where(n => !n.isBatchable || !n.generatePropertyBlock))
            {
                using(registry.ProvideSnippet(string.Format("UnbatchedUniforms_{0}", prop.referenceName), prop.guid, out var s))
                {
                    s.AppendLine(prop.GetPropertyDeclarationString());
                }
            }
            
            using(registry.ProvideSnippet("UnbatchedUniforms_End", Guid.Empty, out var s))
            {
                s.AppendNewLine();
            }
        }

        public List<TextureInfo> GetConfiguredTexutres()
        {
            var result = new List<TextureInfo>();

            foreach (var prop in properties.OfType<TextureShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.texture != null ? prop.value.texture.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }

            foreach (var prop in properties.OfType<Texture2DArrayShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.textureArray != null ? prop.value.textureArray.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }

            foreach (var prop in properties.OfType<Texture3DShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.texture != null ? prop.value.texture.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }

            foreach (var prop in properties.OfType<CubemapShaderProperty>())
            {
                if (prop.referenceName != null)
                {
                    var textureInfo = new TextureInfo
                    {
                        name = prop.referenceName,
                        textureId = prop.value.cubemap != null ? prop.value.cubemap.GetInstanceID() : 0,
                        modifiable = prop.modifiable
                    };
                    result.Add(textureInfo);
                }
            }
            return result;
        }
    }
}
