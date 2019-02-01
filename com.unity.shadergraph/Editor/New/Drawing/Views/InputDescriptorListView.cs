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
            var valueType = (SlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight), descriptor.valueType);

            list.list[index] = new InputDescriptor(descriptor.id, descriptor.name, valueType, valueType.ToDefaultControl()/*, GetControlFromIndex(controlValue, valueType)*/);
        }
    }
}
