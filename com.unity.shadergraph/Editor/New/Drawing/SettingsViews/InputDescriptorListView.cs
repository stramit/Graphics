using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditorInternal;

namespace UnityEditor.ShaderGraph.Drawing
{
    class InputDescriptorListView : VisualElement
    {
        // Node data
        ShaderNode m_Node;
        List<InputDescriptor> m_InputDescriptors;
        ShaderValueDescriptorType m_DescriptorType;

        // GUI data
		IMGUIContainer m_Container;
        ReorderableList m_ReorderableList;
        int m_SelectedIndex = -1;

        public InputDescriptorListView(ShaderNode node, List<InputDescriptor> descriptors, ShaderValueDescriptorType descriptorType)
        {
            // Styling
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/SettingsViews/InputDescriptorListView"));

            // Set Node data
            m_Node = node;
            m_InputDescriptors = descriptors;
            m_DescriptorType = descriptorType;

            // Generate GUI
            m_Container = new IMGUIContainer(() => OnGUIHandler ()) { name = "ListContainer" };
            Add(m_Container);
        }

        void OnGUIHandler()
        {
            // If changed
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                // Create Reorderable List
                if(m_ReorderableList == null)
                {
                    var label = string.Format("{0}s", Regex.Replace(m_DescriptorType.ToString(), "(\\B[A-Z])", " $1"));
                    CreateReorderableList(m_InputDescriptors, label, true, true, true, true);
                }
                
                m_ReorderableList.index = m_SelectedIndex;
                m_ReorderableList.DoLayoutList();

                // If changed repaint
                if (changeCheckScope.changed)
                    m_Node.Dirty(ModificationScope.Node);
            }
        }

        private void CreateReorderableList(List<InputDescriptor> list, string label, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) 
        {
            m_ReorderableList = new ReorderableList(list, typeof(InputDescriptor), draggable, displayHeader, displayAddButton, displayRemoveButton);
        
            m_ReorderableList.drawHeaderCallback = (Rect rect) => 
            {  
                var labelRect = new Rect(rect.x, rect.y, rect.width-10, rect.height);
                EditorGUI.LabelField(labelRect, label);
            };

            // Draw Element
            m_ReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                rect.y += 2;
                DrawEntry(m_ReorderableList, index, new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight));
            };

            // Element height
            m_ReorderableList.elementHeightCallback = (int indexer) => 
            {
                return m_ReorderableList.elementHeight;
            };
            
            // Add callback delegates
            m_ReorderableList.onSelectCallback += SelectEntry;
            m_ReorderableList.onAddCallback += AddEntry;
            m_ReorderableList.onRemoveCallback += RemoveEntry;
            m_ReorderableList.onReorderCallback += ReorderEntries;
        }

        private void DrawEntry(ReorderableList list, int index, Rect rect)
        {
            // Label style
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            // Get Descriptor data
            InputDescriptor descriptor = list.list[index] as InputDescriptor;
            bool hasControl = m_DescriptorType != ShaderValueDescriptorType.Output;

            // Draw element GUI
            int elementCount = 2;
            EditorGUI.BeginChangeCheck();
            var name = EditorGUI.DelayedTextField( new Rect(rect.x, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), descriptor.name, labelStyle);       
            var valueType = (ConcreteSlotValueType)EditorGUI.EnumPopup( new Rect(rect.x + rect.width / elementCount, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), descriptor.valueType);

            // InputDescriptor inDescriptor = descriptor as InputDescriptor;
            // Type[] validControlTypes = GetAllShaderControlTypeThatSupportValueType(valueType);
            // string[] validControlTypeStrings = new string[validControlTypes.Length];
            // for(int i = 0; i < validControlTypes.Length; i++)
            //     validControlTypeStrings[i] = validControlTypes[i].Name;

            // var controlValue = EditorGUI.Popup( new Rect(rect.x + (rect.width / elementCount) * 2, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), 
            //      GetIndexOfControl(inDescriptor.control.GetType(), valueType),
            //      validControlTypeStrings);

            list.list[index] = new InputDescriptor(descriptor.id, name, valueType, valueType.ToDefaultControl()/*, GetControlFromIndex(controlValue, valueType)*/);

            // Update Descriptors if changed
            if(EditorGUI.EndChangeCheck())
                m_Node.ValidateNode();
        }

        private void SelectEntry(ReorderableList list)
        {
            m_SelectedIndex = list.index;
        }

        private void AddEntry(ReorderableList list)
        {
            var defaultType = ConcreteSlotValueType.Vector1;
            switch(m_DescriptorType)
            {
                case ShaderValueDescriptorType.Input:
                    list.list.Add(new InputDescriptor(-1, "Invalid", defaultType, defaultType.ToDefaultControl()));
                    m_SelectedIndex = m_InputDescriptors.Count - 1;
                    break;
                case ShaderValueDescriptorType.Output:
                    // Exception
                    break;
                case ShaderValueDescriptorType.Parameter:
                    list.list.Add(new InputDescriptor(-1, "Invalid", defaultType, defaultType.ToDefaultControl()));
                    m_SelectedIndex = m_InputDescriptors.Count - 1;
                    break;
            }

            m_Node.ValidateNode();
        }

        private void RemoveEntry(ReorderableList list)
        {
            list.index = m_SelectedIndex;
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            m_SelectedIndex = list.index;
            m_Node.ValidateNode();
        }

        private void ReorderEntries(ReorderableList list)
        {
            m_Node.ValidateNode();
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
