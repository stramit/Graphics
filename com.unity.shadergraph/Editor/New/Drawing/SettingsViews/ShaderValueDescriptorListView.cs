using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditorInternal;

namespace UnityEditor.ShaderGraph.Drawing
{
    abstract class ShaderValueDescriptorListView : VisualElement
    {
        private ShaderNode m_Node;
        private ShaderValueDescriptorType m_DescriptorType;
        private ReorderableList m_ReorderableList;
        private IMGUIContainer m_Container;
        private GUIStyle m_LabelStyle;
        private int m_SelectedIndex = -1;
        private string label => string.Format("{0}s", Regex.Replace(m_DescriptorType.ToString(), "(\\B[A-Z])", " $1"));
        public int labelWidth => 80;

        public GUIStyle labelStyle
        {
            get
            {
                if(m_LabelStyle == null)
                {
                    m_LabelStyle = new GUIStyle();
                    m_LabelStyle.normal.textColor = Color.white;
                }
                return m_LabelStyle;
            }
        }

        protected ShaderValueDescriptorListView(ShaderNode node, ShaderValueDescriptorType descriptorType)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/SettingsViews/ShaderValueDescriptorListView"));
            m_Node = node;
            m_DescriptorType = descriptorType;
            m_Container = new IMGUIContainer(() => OnGUIHandler ()) { name = "ListContainer" };
            Add(m_Container);
        }

        internal abstract ReorderableList CreateList();
        internal abstract void DrawDescriptorRow(ReorderableList reorderableList, int index, Rect rect);

        private void OnGUIHandler()
        {
            if(m_ReorderableList == null)
            {
                m_ReorderableList = CreateList();
                AddCallbacks();
            }
                
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                m_ReorderableList.index = m_SelectedIndex;
                m_ReorderableList.DoLayoutList();

                if (changeCheckScope.changed)
                    m_Node.Dirty(ModificationScope.Node);
            }
        }

        private void AddCallbacks() 
        {      
            m_ReorderableList.drawHeaderCallback = (Rect rect) => 
            {  
                var labelRect = new Rect(rect.x, rect.y, rect.width-10, rect.height);
                EditorGUI.LabelField(labelRect, label);
            };

            // Draw Element
            m_ReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                rect.y += 2;
                EditorGUI.BeginChangeCheck();
                IShaderValueDescriptor descriptor = m_ReorderableList.list[index] as IShaderValueDescriptor;
                descriptor.name = EditorGUI.DelayedTextField( new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), descriptor.name, labelStyle); 
                DrawDescriptorRow(m_ReorderableList, index, new Rect(rect.x + labelWidth, rect.y, rect.width, EditorGUIUtility.singleLineHeight));
                if(EditorGUI.EndChangeCheck())
                    m_Node.ValidateNode();
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
                    m_SelectedIndex = list.list.Count - 1;
                    break;
                case ShaderValueDescriptorType.Output:
                    list.list.Add(new OutputDescriptor(-1, "Invalid", defaultType));
                    break;
                case ShaderValueDescriptorType.Parameter:
                    list.list.Add(new InputDescriptor(-1, "Invalid", defaultType, defaultType.ToDefaultControl()));
                    m_SelectedIndex = list.list.Count - 1;
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
    }
}
