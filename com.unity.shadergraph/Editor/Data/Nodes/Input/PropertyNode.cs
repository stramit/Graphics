using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Property")]
    class PropertyNode : AbstractMaterialNode, IGeneratesBodyCode, IOnAssetEnabled
    {
        private Guid m_PropertyGuid;

        [SerializeField]
        private string m_PropertyGuidSerialized;

        public const int OutputSlotId = 0;

        public PropertyNode()
        {
            name = "Property";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Property-Node"; }
        }

        private void UpdateNode()
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.graphInputs.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property.concreteValueType == ConcreteSlotValueType.Vector1)
            {
                AddSlot(new Vector1MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, 0));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector2)
            {
                AddSlot(new Vector2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector3)
            {
                AddSlot(new Vector3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector4)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Boolean)
            {
                AddSlot(new BooleanMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, false));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.Texture2D)
            {
                AddSlot(new Texture2DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Texture2DArray)
            {
                AddSlot(new Texture2DArrayMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Texture3D)
            {
                AddSlot(new Texture3DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.concreteValueType == ConcreteSlotValueType.Cubemap)
            {
                AddSlot(new CubemapMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.Gradient)
            {
                AddSlot(new GradientMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix2)
            {
                AddSlot(new Matrix2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix3)
            {
                AddSlot(new Matrix3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix4)
            {
                AddSlot(new Matrix4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.concreteValueType == ConcreteSlotValueType.SamplerState)
            {
                AddSlot(new SamplerStateMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.graphInputs.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property.concreteValueType == ConcreteSlotValueType.Vector1)
            {
                var result = string.Format("{0} {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector2)
            {
                var result = string.Format("{0}2 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector3)
            {
                var result = string.Format("{0}3 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Vector4)
            {
                var result = string.Format("{0}4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Boolean)
            {
                var result = string.Format("{0} {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix2)
            {
                var result = string.Format("{0}2x2 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix3)
            {
                var result = string.Format("{0}3x3 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Matrix4)
            {
                var result = string.Format("{0}4x4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.concreteValueType == ConcreteSlotValueType.Gradient)
            {
                var result = string.Format("Gradient {0} = {1};"
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.shaderOutputName);
                visitor.AddShaderChunk(result, true);
            }
        }

        public Guid propertyGuid
        {
            get { return m_PropertyGuid; }
            set
            {
                if (m_PropertyGuid == value)
                    return;

                var graph = owner as AbstractMaterialGraph;
                var property = graph.graphInputs.FirstOrDefault(x => x.guid == value);
                if (property == null)
                    return;
                m_PropertyGuid = value;

                UpdateNode();

                Dirty(ModificationScope.Topological);
            }
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.graphInputs.FirstOrDefault(x => x.guid == propertyGuid);

            if (!(property.concreteValueType == ConcreteSlotValueType.Texture2D) &&
                !(property.concreteValueType == ConcreteSlotValueType.Texture2DArray) &&
                !(property.concreteValueType == ConcreteSlotValueType.Texture3D) &&
                !(property.concreteValueType == ConcreteSlotValueType.Cubemap))
                return base.GetVariableNameForSlot(slotId);

            return property.shaderOutputName;
        }

        protected override bool CalculateNodeHasError(ref string errorMessage)
        {
            var graph = owner as AbstractMaterialGraph;

            if (!propertyGuid.Equals(Guid.Empty) && !graph.graphInputs.Any(x => x.guid == propertyGuid))
                return true;

            return false;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            m_PropertyGuidSerialized = m_PropertyGuid.ToString();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (!string.IsNullOrEmpty(m_PropertyGuidSerialized))
                m_PropertyGuid = new Guid(m_PropertyGuidSerialized);
        }

        public void OnEnable()
        {
            UpdateNode();
        }
    }
}
