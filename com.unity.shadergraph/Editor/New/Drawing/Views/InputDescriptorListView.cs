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
        ControlDictionary m_ControlDictionary;

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

            SlotValueType previousValueType = descriptor.valueType;
            var valueType = (SlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), descriptor.valueType); 
            
            var previousControlIndex =  m_ControlDictionary.GetIndexOfControl(valueType, previousValueType != valueType ? valueType.ToDefaultControl() : descriptor.control);
            var controlIndex = EditorGUI.Popup(new Rect(rect.x + (rect.width - labelWidth) * 0.5f, rect.y, (rect.width - labelWidth) * 0.5f, EditorGUIUtility.singleLineHeight), previousControlIndex, m_ControlDictionary.GetControlEntries(valueType));
            
            list.list[index] = new InputDescriptor(descriptor.id, descriptor.name, valueType, m_ControlDictionary.GetControlFromIndex(valueType, controlIndex));
        }
    }
}
