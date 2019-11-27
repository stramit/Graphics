using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.Drawing.Controls
{
    [AttributeUsage(AttributeTargets.Property)]
    class TargetControlAttribute : Attribute, IControlAttribute
    {
        public TargetControlAttribute()
        {

        }

        public VisualElement InstantiateControl(AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            return new TargetControlView(node, propertyInfo);
        }
    }

    class TargetControlView : VisualElement, AbstractMaterialNodeModificationListener
    {
        AbstractMaterialNode m_Node;
        PropertyInfo m_PropertyInfo;
        VisualElement m_Container;

        public TargetControlView(AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/TargetControlView"));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            m_Container = new VisualElement() { name = "container" };
            m_Container.Add(new IMGUIContainer(TargetOnGUIHandler) { name = "control"} );
            m_Container.Add(new IMGUIContainer(VariantOnGUIHandler) { name = "control"} );
            Add(m_Container);
        }

        void TargetOnGUIHandler()
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Target");
            m_Node.owner.activeTargetIndex = EditorGUILayout.Popup(m_Node.owner.activeTargetIndex,
                m_Node.owner.validTargets.Select(x => x.displayName).ToArray(), GUILayout.Width(100f));
            if (EditorGUI.EndChangeCheck())
                UpdateTargets();
            GUILayout.EndHorizontal();
        }

        void VariantOnGUIHandler()
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Variants");
            m_Node.owner.activeTargetImplementationBitmask = EditorGUILayout.MaskField(m_Node.owner.activeTargetImplementationBitmask,
                m_Node.owner.validImplementations.Select(x => x.displayName).ToArray(), GUILayout.Width(100f));
            if (EditorGUI.EndChangeCheck())
                UpdateTargets();
            GUILayout.EndHorizontal();
        }

        void UpdateTargets()
        {
            m_Node.owner.UpdateTargets();
            m_Node.owner.targetBlock.Dirty(ModificationScope.Graph);
        }

        public void OnNodeModified(ModificationScope scope)
        {
            if (scope == ModificationScope.Graph)
                m_Container.MarkDirtyRepaint();
        }
    }
}
