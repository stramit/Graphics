using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

namespace UnityEditor.ShaderGraph.Drawing
{
    class OutputDescriptorListView : ShaderValueDescriptorListView
    {
        List<OutputDescriptor> m_OutputDescriptors;

        public OutputDescriptorListView(ShaderNode node, List<OutputDescriptor> descriptors)
            : base (node, ShaderValueDescriptorType.Output)
        {
            m_OutputDescriptors = descriptors;
        }

        internal override ReorderableList CreateList()
        {
            return new ReorderableList(m_OutputDescriptors, typeof(IShaderValueDescriptor), true, true, true, true);
        }

        internal override void DrawDescriptorRow(ReorderableList list, int index, Rect rect)
        {
            OutputDescriptor descriptor = list.list[index] as OutputDescriptor;
            var valueType = (ConcreteSlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight), descriptor.valueType);
            list.list[index] = new OutputDescriptor(descriptor.id, descriptor.name, valueType);
        }
    }
}