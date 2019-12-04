using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Universal", "Universal Sprite Options")]
    class UniversalSpriteOptionsBlock : BlockData
    {
        public enum MaterialType
        {
            Unlit,
            Lit,
        }     

        [SerializeField]
        MaterialType m_MaterialType;

        public UniversalSpriteOptionsBlock()
        {
            name = "Universal Sprite Options";
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
                owner.AddRequiredBlocks(requireBlocks);
                owner.UpdateSupportedBlocks();
                Dirty(Graphing.ModificationScope.Graph);
            }
        }

        Type[] GetRequiredBlockTypes()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(BaseColorBlock));

            if(materialType == MaterialType.Lit)
                types.Add(typeof(SpriteMaskBlock));

            return types.ToArray();
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
