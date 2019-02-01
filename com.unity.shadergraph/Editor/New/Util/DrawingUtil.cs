using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.Drawing
{
    internal static class DrawingUtil
    {

#region ShaderInput
        internal static VisualElement GetInspectorRowForInput(IShaderInput input)
        {
            var container = new VisualElement { name = "Field" };
            container.style.maxWidth = 200;
            container.style.marginLeft = 6;
            container.style.marginRight = 6;
            container.style.flexDirection = FlexDirection.Row;

            var labelContainer = new VisualElement { name = "Label" };
            labelContainer.style.width = 60;
            var label = new Label(input.displayName);
            label.style.paddingTop = 4;
            labelContainer.Add(label);
            container.Add(labelContainer);

            var valueContainer = new VisualElement { name = "Value" };
            valueContainer.Add(input.control.GetControl(input));
            valueContainer.style.flexGrow = 1;
            container.Add(valueContainer);
            return container;
        }
#endregion

    }
}
