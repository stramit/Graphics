using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{   
    [Serializable]
    public class ShaderValueData
    {
        [SerializeField]
        private Vector4 m_VectorValue = Vector4.zero;

        [SerializeField]
        private bool m_BooleanValue = false;

        [SerializeField]
        private Matrix4x4 m_MatrixValue = Matrix4x4.identity;

        [SerializeField]
        private SerializableTexture m_Texture = new SerializableTexture();

        [SerializeField]
        private SerializableGradient m_GradientValue = new SerializableGradient();

        public Vector4 vectorValue
        {
            get => m_VectorValue;
            set => m_VectorValue = value;
        }

        public bool booleanValue
        {
            get => m_BooleanValue;
            set => m_BooleanValue = value;
        }

        public Matrix4x4 matrixValue
        {
            get => m_MatrixValue;
            set => m_MatrixValue = value;
        }

        public Texture textureValue
        {
            get => m_Texture.texture;
            set => m_Texture.texture = value;
        }

        public Gradient gradientValue
        {
            get => m_GradientValue.gradient;
            set => m_GradientValue.gradient = value;
        }
    }
}
