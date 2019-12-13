using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Visual Effect", "Visual Effect Options")]
    class VisualEffectOptions : BlockData
    {
        public enum MaterialType
        {
            Unlit,
            Lit,
        }

        [SerializeField]
        MaterialType m_MaterialType;

        public VisualEffectOptions()
        {
            name = "Visual Effect Options";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => GetRequiredBlockTypes();
        
        [EnumControl("Material")]
        public MaterialType materialType
        {
            get => m_MaterialType;
            set 
            {
                m_MaterialType = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        Type[] GetRequiredBlockTypes()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(BaseColorBlock));

            if(materialType == MaterialType.Lit)
            {
                types.Add(typeof(MetallicBlock));
                types.Add(typeof(SmoothnessBlock));
            }

            return types.ToArray();
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
