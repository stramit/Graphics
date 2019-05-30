using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    abstract class ShaderInput : ShaderValue
    {
#region Data
        [SerializeField]
        private ShaderInputData m_InputData;

        public ShaderInputData inputData
        {
            get => m_InputData;
            set
            {
                if(m_InputData == value)
                    return;
                
                OnDataChanged();
                m_InputData = value;
            }
        }

        public virtual void OnDataChanged() {}
#endregion

#region Utility
        public abstract AbstractMaterialNode ToConcreteNode();
#endregion
    }

#region Data
    [Serializable]
    struct ShaderInputData
    {
        public bool booleanValue;
        public Vector4 vectorValue;
        public Matrix4x4 matrixValue;
        public SerializableTexture textureValue;
        public SerializableTextureArray textureArrayValue;
        public SerializableCubemap cubemapValue;
        public TextureSamplerState samplerStateValue;
        public Gradient gradientValue;

        public ShaderInputData(ConcreteSlotValueType valueType)
        {
            booleanValue = false;
            vectorValue = Vector4.zero;
            matrixValue = Matrix4x4.identity;
            textureValue = null;
            textureArrayValue = null;
            cubemapValue = null;
            samplerStateValue = null;
            gradientValue = null;

            switch(valueType)
            {
                case ConcreteSlotValueType.SamplerState:
                    samplerStateValue = new TextureSamplerState();
                    break;
                case ConcreteSlotValueType.Texture2D:
                case ConcreteSlotValueType.Texture3D:
                    textureValue = new SerializableTexture();
                    break;
                case ConcreteSlotValueType.Texture2DArray:
                    textureArrayValue = new SerializableTextureArray();
                    break;
                case ConcreteSlotValueType.Cubemap:
                    cubemapValue = new SerializableCubemap();
                    break;
                case ConcreteSlotValueType.Gradient:
                    gradientValue = new Gradient();
                    break;
            }
        }
    }
#endregion
}
