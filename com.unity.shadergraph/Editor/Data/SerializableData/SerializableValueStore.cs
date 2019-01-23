using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{   
    [Serializable]
    public class SerializableValueStore
    {
        [SerializeField]
        private Vector4 m_VectorValue = Vector4.zero;

        [SerializeField]
        private bool m_BooleanValue = false;

        [SerializeField]
        private SerializableTexture m_Texture = new SerializableTexture();

        [SerializeField]
        private SerializableGradient m_GradientValue = new SerializableGradient();

        public Vector4 vectorValue
        {
            get { return m_VectorValue; }
            set { m_VectorValue = value; }
        }

        public bool booleanValue
        {
            get { return m_BooleanValue; }
            set { m_BooleanValue = value; }
        }

        public Texture textureValue
        {
            get { return m_Texture.texture; }
            set { m_Texture.texture = value; }
        }

        public Gradient gradientValue
        {
            get { return m_GradientValue.gradient; }
            set { m_GradientValue.gradient = value; }
        }
    }
}
