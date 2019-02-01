using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class SerializableGradient
    {
        [SerializeField]
        Vector4[] m_ColorKeys;

        [SerializeField]
        Vector2[] m_AlphaKeys;

        [SerializeField]
        int m_Mode;

        public Gradient gradient
        {
            get
            {
                var colorKeys = m_ColorKeys != null ? m_ColorKeys.Select(k => new GradientColorKey(new Color(k.x, k.y, k.z, 1f), k.w)).ToArray() : new GradientColorKey[0];
                var alphaKeys = m_AlphaKeys != null ?  m_AlphaKeys.Select(k => new GradientAlphaKey(k.x, k.y)).ToArray() : new GradientAlphaKey[0];

                return new Gradient()
                {
                    colorKeys = colorKeys,
                    alphaKeys = alphaKeys,
                    mode = (GradientMode)m_Mode
                };
            }
            set
            {
                if (!GradientUtils.CheckEquivalency(gradient, value))
                {
                    m_ColorKeys = value.colorKeys.Select(k => new Vector4(k.color.r, k.color.g, k.color.b, k.time)).ToArray();
                    m_AlphaKeys = value.alphaKeys.Select(k => new Vector2(k.alpha, k.time)).ToArray();
                }
            }
        }
    }
}