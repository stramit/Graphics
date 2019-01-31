using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct ShaderValueData
    {
        [SerializeField]
        Vector4 m_Vector;

        [SerializeField]
        bool m_Boolean;

        [SerializeField]
        Matrix4x4 m_Matrix;

        [SerializeField]
        SerializableTexture m_Texture;

        [SerializeField]
        SerializableGradient m_Gradient;

        public Vector4 vector
        {
            get => m_Vector;
            set => m_Vector = value;
        }

        public bool boolean
        {
            get => m_Boolean;
            set => m_Boolean = value;
        }

        public Matrix4x4 matrix
        {
            get => m_Matrix;
            set => m_Matrix = value;
        }

        public Texture texture
        {
            get => m_Texture.texture;
            set => m_Texture.texture = value;
        }

        public Gradient gradient
        {
            get => m_Gradient.gradient;
            set => m_Gradient.gradient = value;
        }
    }
}