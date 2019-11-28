using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Features", "Transparency")]
    class TransparencyBlock : BlockData
    {
        public TransparencyBlock()
        {
            name = "Transparency";
        }

        [SerializeField]
        TransparencyMode m_Mode;

        [EnumControl("Mode")]
        public TransparencyMode mode
        {
            get { return m_Mode; }
            set
            {
                if (m_Mode == value)
                    return;

                m_Mode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => new Type[] { typeof(AlphaBlock) };

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.SurfaceTransparent, true),

                // Blend Mode
                new ConditionalField(Fields.BlendAdd, mode == TransparencyMode.Additive),
                new ConditionalField(Fields.BlendAlpha, mode == TransparencyMode.Alpha),
                new ConditionalField(Fields.BlendMultiply, mode == TransparencyMode.Multiply),
                new ConditionalField(Fields.BlendPremultiply, mode == TransparencyMode.Premultiply),
            };
        }
    }
}
