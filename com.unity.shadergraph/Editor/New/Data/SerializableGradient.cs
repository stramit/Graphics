using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class SerializableGradient
    {
        [SerializeField]
        Vector4[] m_SerializableColorKeys = new Vector4[0];

        [SerializeField]
        Vector2[] m_SerializableAlphaKeys = new Vector2[0];

        [SerializeField]
        int m_SerializableMode = 0;
        
        public Gradient gradient
        {
            get
            {
                var colorKeys = m_SerializableColorKeys.Select(k => new GradientColorKey(new Color(k.x, k.y, k.z, 1f), k.w)).ToArray();
                var alphaKeys = m_SerializableAlphaKeys.Select(k => new GradientAlphaKey(k.x, k.y)).ToArray();

                return new Gradient()
                {
                    colorKeys = colorKeys,
                    alphaKeys = alphaKeys,
                    mode = (GradientMode)m_SerializableMode
                };
            }
            set
            {
                if (!GradientUtils.CheckEquivalency(gradient, value))
                {
                    m_SerializableColorKeys = value.colorKeys.Select(k => new Vector4(k.color.r, k.color.g, k.color.b, k.time)).ToArray();
                    m_SerializableAlphaKeys = value.alphaKeys.Select(k => new Vector2(k.alpha, k.time)).ToArray();
                }
            }
        }
    }
}
