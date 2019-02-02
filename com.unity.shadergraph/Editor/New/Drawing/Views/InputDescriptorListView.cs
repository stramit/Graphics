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
                    if (type.IsClass && !type.IsAbstract && typeof(IShaderControl).IsAssignableFrom(type) 
                        && type != typeof(TextureControl<>) && type != typeof(DefaultControl))
                        controlInstances.Add(Activator.CreateInstance(type) as IShaderControl);
                }
            }

            var typeMap = new Dictionary<SlotValueType, List<Type>>();
            for(int t = 0; t < Enum.GetNames(typeof(SlotValueType)).Length; t++)
            {
                var types = new List<Type>();
                for(int c = 0; c < controlInstances.Count; c++)
                {
                    if(controlInstances[c].validPortTypes.ToList().Contains((SlotValueType)t))
                        types.Add(controlInstances[c].GetType());
                }
                typeMap.Add((SlotValueType)t, types);
            }
            return typeMap;
        }

        private int GetIndexOfControlType(SlotValueType valueType, IShaderControl controlType)
        {
            List<Type> types = new List<Type>();
            controlTypeMap.TryGetValue(valueType, out types);
            return types.IndexOf(controlType.GetType());
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

            List<Type> tempTypes = new List<Type>();
            SlotValueType previousValueType = descriptor.valueType;
            var valueType = (SlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), descriptor.valueType);
            controlTypeMap.TryGetValue(valueType, out tempTypes);  
            var controlIndex =  GetIndexOfControlType(valueType, previousValueType != valueType ? valueType.ToDefaultControl() : descriptor.control);
            var controlType = EditorGUI.Popup(new Rect(rect.x + (rect.width - labelWidth) * 0.5f, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), controlIndex, tempTypes.Select(s => s.Name).ToArray());
            list.list[index] = new InputDescriptor(descriptor.id, descriptor.name, valueType, (IShaderControl)Activator.CreateInstance(tempTypes[controlType]));
        }
    }
}
