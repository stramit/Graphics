using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderControl
    {
        SlotValueType[] GetValidPortTypes { get; }

        void UpdateDefaultValue(ShaderPort port);
        VisualElement GetControl(ShaderPort port);
    }

    [Serializable]
    internal class SerializedControl
    {
        [SerializeField]
        private string m_AssemblyName;

        [SerializeField]
        private string m_TypeName;

        public SerializedControl(IShaderControl control)
        {
            m_AssemblyName = control.GetType().Assembly.GetName().ToString();
            m_TypeName = control.GetType().Name;
        }

        public IShaderControl Deserialize()
        {
            return (IShaderControl)Activator.CreateInstance(m_AssemblyName, m_TypeName);
        }
    }
}
