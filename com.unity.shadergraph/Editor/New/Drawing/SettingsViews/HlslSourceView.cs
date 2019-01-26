using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.NodeLibrary;

namespace UnityEditor.ShaderGraph.Drawing
{
    internal class HlslSourceView : VisualElement
    {
        private HlslFunctionDescriptor GetNewDescriptor(string functionName, string source, HlslSourceType type)
        {
            HlslSource hlslSource = new HlslSource();
            switch(type)
            {
                case HlslSourceType.File:
                    hlslSource = HlslSource.File(source, true);
                    break;
                case HlslSourceType.String:
                    hlslSource = HlslSource.String(source, true);
                    break;
            }
            return new HlslFunctionDescriptor()
            {
                name = functionName,
                source = hlslSource
            };
        }

        private EnumField m_Type;
        private TextField m_FunctionName;
        private TextField m_FunctionSource;
        private TextField m_FunctionBody;

        internal HlslSourceView(ListNode node, HlslFunctionDescriptor function)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/SettingsViews/HlslSourceView"));
            Draw(node, function);            
        }

        private void Draw(ListNode node, HlslFunctionDescriptor function)
        {
            var currentControls = this.Children().ToArray();
            for(int i = 0; i < currentControls.Length; i++)
                currentControls[i].RemoveFromHierarchy();

            m_Type = new EnumField(function.source.type);
            m_Type.RegisterValueChangedCallback(s =>
            {
                node.owner.owner.RegisterCompleteObjectUndo("Function Change");
                string value = (HlslSourceType)s.newValue == HlslSourceType.File ? m_FunctionSource.value : m_FunctionBody.value;
                node.functionDescriptor = GetNewDescriptor(m_FunctionName.value, value, (HlslSourceType)s.newValue);
                Draw(node, node.functionDescriptor);
                node.Dirty(ModificationScope.Graph);
            });

            m_FunctionName = new TextField { value = function.name, multiline = false };
            m_FunctionName.RegisterValueChangedCallback(s =>
            {
                node.owner.owner.RegisterCompleteObjectUndo("Function Change");
                string value = (HlslSourceType)m_Type.value == HlslSourceType.File ? m_FunctionSource.value : m_FunctionBody.value;
                node.functionDescriptor = GetNewDescriptor(s.newValue, value, (HlslSourceType)m_Type.value);
            });
            m_FunctionName.RegisterCallback<FocusOutEvent>(s =>
            {
                node.Dirty(ModificationScope.Graph);
            });

            m_FunctionSource = new TextField { value = function.source.value, multiline = false };
            m_FunctionSource.RegisterValueChangedCallback(s =>
            {
                node.owner.owner.RegisterCompleteObjectUndo("Function Change");
                node.functionDescriptor = GetNewDescriptor(m_FunctionName.value, s.newValue, (HlslSourceType)m_Type.value);
            });
            m_FunctionSource.RegisterCallback<FocusOutEvent>(s =>
            {
                node.Dirty(ModificationScope.Graph);
            });

            m_FunctionBody = new TextField { value = function.source.value, multiline = true };
            m_FunctionBody.RegisterValueChangedCallback(s =>
            {
                node.owner.owner.RegisterCompleteObjectUndo("Function Change");
                node.functionDescriptor = GetNewDescriptor(m_FunctionName.value, s.newValue, (HlslSourceType)m_Type.value);
            });
            m_FunctionBody.RegisterCallback<FocusOutEvent>(s =>
            {
                node.Dirty(ModificationScope.Graph);
            });

            VisualElement typeRow = new VisualElement() { name = "Row" };
            {
                typeRow.Add(new Label("Type"));
                typeRow.Add(m_Type);
            }
            Add(typeRow);
            VisualElement nameRow = new VisualElement() { name = "Row" };
            {
                nameRow.Add(new Label("Name"));
                nameRow.Add(m_FunctionName);
            }
            Add(nameRow);
            switch(function.source.type)
            {
                case HlslSourceType.File:
                    VisualElement sourceRow = new VisualElement() { name = "Row" };
                    {
                        sourceRow.Add(new Label("Source"));
                        sourceRow.Add(m_FunctionSource);
                    }
                    Add(sourceRow);
                    break;
                case HlslSourceType.String:
                    VisualElement bodyRow = new VisualElement() { name = "Row" };
                    {
                        bodyRow.Add(new Label("Body"));
                        bodyRow.style.height = 200;
                        bodyRow.Add(m_FunctionBody);
                    }
                    Add(bodyRow);
                    break;
            }
        }
    }
}
