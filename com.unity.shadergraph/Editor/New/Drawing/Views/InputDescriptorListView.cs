using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing.Util;
using UnityEditorInternal;

namespace UnityEditor.ShaderGraph.Drawing
{
    class InputDescriptorListView : ShaderValueDescriptorListView
    {
        private Dictionary<SlotValueType, List<Type>> m_ControlTypeMap;
        private List<Type> m_TempTypes = new List<Type>();

        public Dictionary<SlotValueType, List<Type>> controlTypeMap
        {
            get
            {
                if(m_ControlTypeMap == null)
                    m_ControlTypeMap = GenerateControlTypeMap();
                return m_ControlTypeMap;
            }
        }

        private Dictionary<SlotValueType, List<Type>> GenerateControlTypeMap()
        {
            List<IShaderControl> controlInstances = new List<IShaderControl>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    if (type.IsClass && !type.IsAbstract && typeof(IShaderControl).IsAssignableFrom(type) && type != typeof(TextureControl<>))
                        controlInstances.Add(Activator.CreateInstance(type) as IShaderControl);
                }
            }

            var tempTypeList = new List<Type>();
            var typeMap = new Dictionary<SlotValueType, List<Type>>();
            for(int t = 0; t < Enum.GetNames(typeof(SlotValueType)).Length; t++)
            {
                tempTypeList.Clear();
                for(int c = 0; c < controlInstances.Count; c++)
                {
                    if(controlInstances[c].validPortTypes.ToList().Contains((SlotValueType)t))
                        tempTypeList.Add(controlInstances[c].GetType());
                }
                typeMap.Add((SlotValueType)t, tempTypeList);
            }
            return typeMap;
        }

        List<InputDescriptor> m_InputDescriptors;

        public InputDescriptorListView(ShaderNode node, List<InputDescriptor> descriptors, ShaderValueDescriptorType descriptorType)
            : base (node, descriptorType)
        {
            m_InputDescriptors = descriptors;
        }

        internal override ReorderableList CreateList()
        {
            return new ReorderableList(m_InputDescriptors, typeof(IShaderValueDescriptor), true, true, true, true);
        }

        internal override void DrawDescriptorRow(ReorderableList list, int index, Rect rect)
        {
            InputDescriptor descriptor = (InputDescriptor)list.list[index]; 

            m_TempTypes = new List<Type>();
            var valueType = (SlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), descriptor.valueType);
            controlTypeMap.TryGetValue(valueType, out m_TempTypes);  
            var controlType = EditorGUI.Popup(new Rect(rect.x + (rect.width - labelWidth) * 0.5f, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), 0, m_TempTypes.Select(s => s.Name).ToArray());

            list.list[index] = new InputDescriptor(descriptor.id, descriptor.name, valueType, valueType.ToDefaultControl()/*, GetControlFromIndex(controlValue, valueType)*/);
        }
    }
}
