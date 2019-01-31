using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.Drawing
{
    class BlackboardProvider
    {
        readonly AbstractMaterialGraph m_Graph;
        public static readonly Texture2D exposedIcon = Resources.Load<Texture2D>("GraphView/Nodes/BlackboardFieldExposed");
        readonly Dictionary<Guid, BlackboardRow> m_PropertyRows;
        readonly BlackboardSection m_Section;
        //WindowDraggable m_WindowDraggable;
        //ResizeBorderFrame m_ResizeBorderFrame;
        public Blackboard blackboard { get; private set; }
        Label m_PathLabel;
        TextField m_PathLabelTextField;
        bool m_EditPathCancelled = false;
        List<MaterialNodeView> m_SelectedNodes = new List<MaterialNodeView>();

        //public Action onDragFinished
        //{
        //    get { return m_WindowDraggable.OnDragFinished; }
        //    set { m_WindowDraggable.OnDragFinished = value; }
        //}

        //public Action onResizeFinished
        //{
        //    get { return m_ResizeBorderFrame.OnResizeFinished; }
        //    set { m_ResizeBorderFrame.OnResizeFinished = value; }
        //}

        Dictionary<IShaderValue, bool> m_ExpandedProperties = new Dictionary<IShaderValue, bool>();

        public Dictionary<IShaderValue, bool> expandedProperties
        {
            get { return m_ExpandedProperties; }
        }

        public string assetName
        {
            get { return blackboard.title; }
            set
            {
                blackboard.title = value;
            }
        }

        public BlackboardProvider(AbstractMaterialGraph graph)
        {
            m_Graph = graph;
            m_PropertyRows = new Dictionary<Guid, BlackboardRow>();

            blackboard = new Blackboard()
            {
                scrollable = true,
                subTitle = FormatPath(graph.path),
                editTextRequested = EditTextRequested,
                addItemRequested = AddItemRequested,
                moveItemRequested = MoveItemRequested
            };

            m_PathLabel = blackboard.hierarchy.ElementAt(0).Q<Label>("subTitleLabel");
            m_PathLabel.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);

            m_PathLabelTextField = new TextField { visible = false };
            m_PathLabelTextField.Q("unity-text-input").RegisterCallback<FocusOutEvent>(e => { OnEditPathTextFinished(); });
            m_PathLabelTextField.Q("unity-text-input").RegisterCallback<KeyDownEvent>(OnPathTextFieldKeyPressed);
            blackboard.hierarchy.Add(m_PathLabelTextField);

            // m_WindowDraggable = new WindowDraggable(blackboard.shadow.Children().First().Q("header"));
            // blackboard.AddManipulator(m_WindowDraggable);

            // m_ResizeBorderFrame = new ResizeBorderFrame(blackboard) { name = "resizeBorderFrame" };
            // blackboard.shadow.Add(m_ResizeBorderFrame);

            m_Section = new BlackboardSection { headerVisible = false };
            foreach (var property in graph.graphInputs.OfType<IShaderValue>())
                AddItem(property);
            blackboard.Add(m_Section);
        }

        void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            if (m_SelectedNodes.Any())
            {
                foreach (var node in m_SelectedNodes)
                {
                    node.RemoveFromClassList("hovered");
                }
                m_SelectedNodes.Clear();
            }
        }

        void OnMouseDownEvent(MouseDownEvent evt)
        {
            if (evt.clickCount == 2 && evt.button == (int)MouseButton.LeftMouse)
            {
                StartEditingPath();
                evt.PreventDefault();
            }
        }

        void StartEditingPath()
        {
            m_PathLabelTextField.visible = true;

            m_PathLabelTextField.value = m_PathLabel.text;
            m_PathLabelTextField.style.position = Position.Absolute;
            var rect = m_PathLabel.ChangeCoordinatesTo(blackboard, new Rect(Vector2.zero, m_PathLabel.layout.size));
            m_PathLabelTextField.style.left = rect.xMin;
            m_PathLabelTextField.style.top = rect.yMin;
            m_PathLabelTextField.style.width = rect.width;
            m_PathLabelTextField.style.fontSize = 11;
            m_PathLabelTextField.style.marginLeft = 0;
            m_PathLabelTextField.style.marginRight = 0;
            m_PathLabelTextField.style.marginTop = 0;
            m_PathLabelTextField.style.marginBottom = 0;

            m_PathLabel.visible = false;

            m_PathLabelTextField.Q("unity-text-input").Focus();
            m_PathLabelTextField.SelectAll();
        }

        void OnPathTextFieldKeyPressed(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    m_EditPathCancelled = true;
                    m_PathLabelTextField.Q("unity-text-input").Blur();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    m_PathLabelTextField.Q("unity-text-input").Blur();
                    break;
                default:
                    break;
            }
        }

        void OnEditPathTextFinished()
        {
            m_PathLabel.visible = true;
            m_PathLabelTextField.visible = false;

            var newPath = m_PathLabelTextField.text;
            if (!m_EditPathCancelled && (newPath != m_PathLabel.text))
            {
                newPath = SanitizePath(newPath);
            }

            m_Graph.path = newPath;
            m_PathLabel.text = FormatPath(newPath);
            m_EditPathCancelled = false;
        }

        static string FormatPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "—";
            return path;
        }

        static string SanitizePath(string path)
        {
            var splitString = path.Split('/');
            List<string> newStrings = new List<string>();
            foreach (string s in splitString)
            {
                var str = s.Trim();
                if (!string.IsNullOrEmpty(str))
                {
                    newStrings.Add(str);
                }
            }

            return string.Join("/", newStrings.ToArray());
        }

        void MoveItemRequested(Blackboard blackboard, int newIndex, VisualElement visualElement)
        {
            var shaderValue = visualElement.userData as IShaderValue;
            if (shaderValue == null)
                return;

            m_Graph.owner.RegisterCompleteObjectUndo("Move Property");
            m_Graph.MoveShaderProperty(shaderValue, newIndex);
        }

        void AddItemRequested(Blackboard blackboard)
        {
            var gm = new GenericMenu();
            if(m_Graph is SubGraph subgraph) // TODO: Generalise
            {
                gm.AddItem(new GUIContent("Vector1"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Vector1", ConcreteSlotValueType.Vector1)), true));
                gm.AddItem(new GUIContent("Vector2"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Vector2", ConcreteSlotValueType.Vector2)), true));
                gm.AddItem(new GUIContent("Vector3"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Vector3", ConcreteSlotValueType.Vector3)), true));
                gm.AddItem(new GUIContent("Vector4"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Vector4", ConcreteSlotValueType.Vector4)), true));
                gm.AddItem(new GUIContent("Boolean"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Boolean", ConcreteSlotValueType.Boolean)), true));
                gm.AddItem(new GUIContent("Texture2D"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Texture2D", ConcreteSlotValueType.Texture2D)), true));
                gm.AddItem(new GUIContent("Texture2DArray"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Texture2DArray", ConcreteSlotValueType.Texture2DArray)), true));
                gm.AddItem(new GUIContent("Texture3D"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Texture3D", ConcreteSlotValueType.Texture3D)), true));
                gm.AddItem(new GUIContent("Cubemap"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Cubemap", ConcreteSlotValueType.Cubemap)), true));
                gm.AddItem(new GUIContent("SamplerState"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "SamplerState", ConcreteSlotValueType.SamplerState)), true));
                gm.AddItem(new GUIContent("Matrix2x2"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Matrix2x2", ConcreteSlotValueType.Matrix2)), true));
                gm.AddItem(new GUIContent("Matrix3x3"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Matrix3x3", ConcreteSlotValueType.Matrix3)), true));
                gm.AddItem(new GUIContent("Matrix4x4"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Matrix4x4", ConcreteSlotValueType.Matrix4)), true));
                gm.AddItem(new GUIContent("Gradient"), false, () => AddItem(new ShaderPort(new InputDescriptor(-1, "Gradient", ConcreteSlotValueType.Gradient)), true));
            }
            else
            {
                gm.AddItem(new GUIContent("Vector1"), false, () => AddItem(new ShaderProperty(PropertyType.Vector1), true));
                gm.AddItem(new GUIContent("Vector2"), false, () => AddItem(new ShaderProperty(PropertyType.Vector2), true));
                gm.AddItem(new GUIContent("Vector3"), false, () => AddItem(new ShaderProperty(PropertyType.Vector3), true));
                gm.AddItem(new GUIContent("Vector4"), false, () => AddItem(new ShaderProperty(PropertyType.Vector4), true));
                gm.AddItem(new GUIContent("Color"), false, () => AddItem(new ShaderProperty(PropertyType.Color), true));
                gm.AddItem(new GUIContent("Texture2D"), false, () => AddItem(new ShaderProperty(PropertyType.Texture2D), true));
                gm.AddItem(new GUIContent("Texture2D Array"), false, () => AddItem(new ShaderProperty(PropertyType.Texture2DArray), true));
                gm.AddItem(new GUIContent("Texture3D"), false, () => AddItem(new ShaderProperty(PropertyType.Texture3D), true));
                gm.AddItem(new GUIContent("Cubemap"), false, () => AddItem(new ShaderProperty(PropertyType.Cubemap), true));
                gm.AddItem(new GUIContent("Boolean"), false, () => AddItem(new ShaderProperty(PropertyType.Boolean), true));
            }
            gm.ShowAsContext();
        }

        void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            var field = (BlackboardField)visualElement;
            var shaderValue = (IShaderValue)field.userData;
            if (!string.IsNullOrEmpty(newText) && newText != shaderValue.displayName)
            {
                m_Graph.owner.RegisterCompleteObjectUndo("Edit Property Name");
                newText = m_Graph.SanitizePropertyName(newText, shaderValue.guid);
                shaderValue.displayName = newText;
                field.text = newText;
                DirtyNodes();
            }
        }

        public void HandleGraphChanges()
        {
            foreach (var propertyGuid in m_Graph.removedGraphInputs)
            {
                BlackboardRow row;
                if (m_PropertyRows.TryGetValue(propertyGuid, out row))
                {
                    row.RemoveFromHierarchy();
                    m_PropertyRows.Remove(propertyGuid);
                }
            }

            foreach (var shaderValue in m_Graph.addedGraphInputs.OfType<IShaderValue>())
                AddItem(shaderValue, index: m_Graph.GetShaderPropertyIndex(shaderValue));

            foreach (var propertyDict in expandedProperties)
            {
                SessionState.SetBool(propertyDict.Key.guid.ToString(), propertyDict.Value);
            }

            if (m_Graph.movedGraphInputs.Any())
            {
                foreach (var row in m_PropertyRows.Values)
                    row.RemoveFromHierarchy();

                foreach (var property in m_Graph.graphInputs)
                    m_Section.Add(m_PropertyRows[property.guid]);
            }
            m_ExpandedProperties.Clear();
        }

        void AddItem(IShaderValue shaderValue, bool create = false, int index = -1)
        {
            if (m_PropertyRows.ContainsKey(shaderValue.guid))
                return;

            BlackboardField field;
            VisualElement propertyView;
            if(shaderValue is ShaderPort port)
            {
                string label = shaderValue.shaderOutputName;
                if (create)
                    label = m_Graph.SanitizePropertyName(shaderValue.shaderOutputName);
                field = new BlackboardField(exposedIcon, label, port.concreteValueType.ToString()) { userData = shaderValue };
                propertyView = new BlackboardPortFieldView(field, m_Graph, port);
            }
            else
            {
                if (create)
                    shaderValue.displayName = m_Graph.SanitizePropertyName(shaderValue.displayName);
                ShaderProperty property = shaderValue as ShaderProperty;
                var icon = property.generatePropertyBlock ? exposedIcon : null;
                field = new BlackboardField(icon, shaderValue.displayName, property.propertyType.ToString()) { userData = shaderValue };
                propertyView = new BlackboardFieldPropertyView(field, m_Graph, property);
            }
           
            var row = new BlackboardRow(field, propertyView);
            var pill = row.Q<Pill>();
            pill.RegisterCallback<MouseEnterEvent>(evt => OnMouseHover(evt, shaderValue));
            pill.RegisterCallback<MouseLeaveEvent>(evt => OnMouseHover(evt, shaderValue));
            pill.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);

            var expandButton = row.Q<Button>("expandButton");
            expandButton.RegisterCallback<MouseDownEvent>(evt => OnExpanded(evt, shaderValue), TrickleDown.TrickleDown);

            row.userData = shaderValue;
            if (index < 0)
                index = m_PropertyRows.Count;
            if (index == m_PropertyRows.Count)
                m_Section.Add(row);
            else
                m_Section.Insert(index, row);
            m_PropertyRows[shaderValue.guid] = row;

            m_PropertyRows[shaderValue.guid].expanded = SessionState.GetBool(shaderValue.guid.ToString(), true);

            if (create)
            {
                row.expanded = true;
                m_Graph.owner.RegisterCompleteObjectUndo("Create Property");
                m_Graph.AddShaderProperty(shaderValue);
                field.OpenTextEditor();
            }
        }

        void OnExpanded(MouseDownEvent evt, IShaderValue shaderValue)
        {
            m_ExpandedProperties[shaderValue] = !m_PropertyRows[shaderValue.guid].expanded;
        }

        void DirtyNodes()
        {
            foreach (var node in m_Graph.GetNodes<PropertyNode>())
            {
                node.OnEnable();
                node.Dirty(ModificationScope.Node);
            }
        }

        public BlackboardRow GetBlackboardRow(Guid guid)
        {
            return m_PropertyRows[guid];
        }

        void OnMouseHover(EventBase evt, IShaderValue shaderValue)
        {
            var graphView = blackboard.GetFirstAncestorOfType<MaterialGraphView>();
            if (evt.eventTypeId == MouseEnterEvent.TypeId())
            {
                foreach (var node in graphView.nodes.ToList().OfType<MaterialNodeView>())
                {
                    if (node.node is PropertyNode propertyNode)
                    {
                        if (propertyNode.propertyGuid == shaderValue.guid)
                        {
                            m_SelectedNodes.Add(node);
                            node.AddToClassList("hovered");
                        }
                    }
                }
            }
            else if (evt.eventTypeId == MouseLeaveEvent.TypeId() && m_SelectedNodes.Any())
            {
                foreach (var node in m_SelectedNodes)
                {
                    node.RemoveFromClassList("hovered");
                }
                m_SelectedNodes.Clear();
            }
        }
    }
}
