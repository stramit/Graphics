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
            InputDescriptor descriptor = list.list[index] as InputDescriptor;    
            var valueType = (ConcreteSlotValueType)EditorGUI.EnumPopup( new Rect(rect.x, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight), descriptor.valueType);

            // InputDescriptor inDescriptor = descriptor as InputDescriptor;
            // Type[] validControlTypes = GetAllShaderControlTypeThatSupportValueType(valueType);
            // string[] validControlTypeStrings = new string[validControlTypes.Length];
            // for(int i = 0; i < validControlTypes.Length; i++)
            //     validControlTypeStrings[i] = validControlTypes[i].Name;

            // var controlValue = EditorGUI.Popup( new Rect(rect.x + (rect.width / elementCount) * 2, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), 
            //      GetIndexOfControl(inDescriptor.control.GetType(), valueType),
            //      validControlTypeStrings);

            list.list[index] = new InputDescriptor(descriptor.id, descriptor.name, valueType, valueType.ToDefaultControl()/*, GetControlFromIndex(controlValue, valueType)*/);
        }

        // --------------------------------------------------
        // MOVE

        private int GetIndexOfControl(Type controlType, ConcreteSlotValueType valueType)
        {
            var allControls = GetAllShaderControlTypeThatSupportValueType(valueType);
            for(int i = 0; i < allControls.Length; i++)
            {
                if(allControls[i] == controlType)
                    return i;
            }
            return 0;
        }

        private IShaderControl GetControlFromIndex(int index, ConcreteSlotValueType valueType)
        {
            var allControls = GetAllShaderControlTypeThatSupportValueType(valueType);
            return (IShaderControl)Activator.CreateInstance(allControls[index]);
        }

        private Type[] GetAllShaderControlTypeThatSupportValueType(ConcreteSlotValueType valueType)
        {
            List<Type> controlTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    // TODO - Need to handle generics for TextureControl
                    if (type.IsClass && !type.IsAbstract 
                        && typeof(IShaderControl).IsAssignableFrom(type) 
                        && !type.IsGenericType
                        && type != typeof(DefaultControl))
                    {
                        IShaderControl control = (IShaderControl)System.Activator.CreateInstance(type);
                        if(control.validPortTypes.Contains(valueType))
                            controlTypes.Add(type);
                    }
                }
            }
            return controlTypes.ToArray();
        }
    }
}
