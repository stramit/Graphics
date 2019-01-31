using System;
using System.Linq;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace UnityEditor.ShaderGraph.Drawing
{
    public class BlackboardPortFieldView : VisualElement
    {
        readonly BlackboardField m_BlackboardField;
        readonly AbstractMaterialGraph m_Graph;

        ShaderPort m_Input;

        static Type s_ContextualMenuManipulator = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesOrNothing()).FirstOrDefault(t => t.FullName == "UnityEngine.UIElements.ContextualMenuManipulator");

        internal BlackboardPortFieldView()
        {

        }

        internal BlackboardPortFieldView(BlackboardField blackboardField, AbstractMaterialGraph graph, ShaderPort input)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderGraphBlackboard"));
            m_BlackboardField = blackboardField;
            m_Graph = graph;
            m_Input = input;

            Add(GetControlForShaderPort(input));
            AddToClassList("sgblackboardPortFieldView");
        }

        private VisualElement GetControlForShaderPort(ShaderPort parameter)
        {
            var container = new VisualElement { name = "Container" };
            container.style.marginLeft = 6;
            container.style.marginRight = 6;
            container.style.flexDirection = FlexDirection.Row;

            var labelContainer = new VisualElement { name = "LabelContainer" };
            labelContainer.style.width = 60;
            var label = new Label("Default");
            label.style.paddingTop = 4;
            labelContainer.Add(label);
            container.Add(labelContainer);

            var valueContainer = new VisualElement { name = "ValueContainer" };
            valueContainer.Add(parameter.control.GetControl(parameter));
            valueContainer.style.flexGrow = 1;
            valueContainer.style.flexDirection = FlexDirection.Row;
            container.Add(valueContainer);
            return container;
        }
    }
}
