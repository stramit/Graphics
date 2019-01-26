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
    class OutputDescriptorListView : VisualElement
    {
        // Node data
        ShaderNode m_Node;
        List<OutputDescriptor> m_OutputDescriptors;

        // GUI data
		IMGUIContainer m_Container;
        ReorderableList m_ReorderableList;
        int m_SelectedIndex = -1;

        public OutputDescriptorListView(ShaderNode node, List<OutputDescriptor> descriptors)
        {
            // Styling
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/SettingsViews/OutputDescriptorListView"));

            // Set Node data
            m_Node = node;
            m_OutputDescriptors = descriptors;

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
                    CreateReorderableList(m_OutputDescriptors, "Outputs", true, true, true, true);
                
                m_ReorderableList.index = m_SelectedIndex;
                m_ReorderableList.DoLayoutList();

                // If changed repaint
                if (changeCheckScope.changed)
                    m_Node.Dirty(ModificationScope.Node);
            }
        }

        private void CreateReorderableList(List<OutputDescriptor> list, string label, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) 
        {
            m_ReorderableList = new ReorderableList(list, typeof(OutputDescriptor), draggable, displayHeader, displayAddButton, displayRemoveButton);
        
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
            OutputDescriptor descriptor = list.list[index] as OutputDescriptor;

            // Draw element GUI
            int elementCount = 2;
            EditorGUI.BeginChangeCheck();
            var name = EditorGUI.DelayedTextField( new Rect(rect.x, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), descriptor.name, labelStyle);       
            var valueType = (ConcreteSlotValueType)EditorGUI.EnumPopup( new Rect(rect.x + rect.width / elementCount, rect.y, rect.width / elementCount, EditorGUIUtility.singleLineHeight), descriptor.valueType);

            list.list[index] = new OutputDescriptor(descriptor.id, name, valueType);

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
            list.list.Add(new OutputDescriptor(-1, "Invalid", defaultType));
            m_SelectedIndex = m_OutputDescriptors.Count - 1;
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
    }
}