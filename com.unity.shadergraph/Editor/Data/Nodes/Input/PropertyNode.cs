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
            var property = graph.graphInputs.OfType<ShaderProperty>().FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property.propertyType == PropertyType.Vector1)
            {
                AddSlot(new Vector1MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, 0));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Vector2)
            {
                AddSlot(new Vector2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Vector3)
            {
                AddSlot(new Vector3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Vector4)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Color)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Texture2D)
            {
                AddSlot(new Texture2DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Texture2DArray)
            {
                AddSlot(new Texture2DArrayMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Texture3D)
            {
                AddSlot(new Texture3DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            if (property.propertyType == PropertyType.Cubemap)
            {
                AddSlot(new CubemapMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            if (property.propertyType == PropertyType.Boolean)
            {
                AddSlot(new BooleanMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, false));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var graph = owner as AbstractMaterialGraph;
            var property = graph.graphInputs.OfType<ShaderProperty>().FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property.propertyType == PropertyType.Vector1)
            {
                var result = string.Format("{0} {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.propertyType == PropertyType.Vector2)
            {
                var result = string.Format("{0}2 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.propertyType == PropertyType.Vector3)
            {
                var result = string.Format("{0}3 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.propertyType == PropertyType.Vector4)
            {
                var result = string.Format("{0}4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.propertyType == PropertyType.Color)
            {
                var result = string.Format("{0}4 {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
                visitor.AddShaderChunk(result, true);
            }
            if (property.propertyType == PropertyType.Boolean)
            {
                var result = string.Format("{0} {1} = {2};"
                        , precision
                        , GetVariableNameForSlot(OutputSlotId)
                        , property.referenceName);
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
            var property = graph.graphInputs.OfType<ShaderProperty>().FirstOrDefault(x => x.guid == propertyGuid);

            if (!(property.propertyType == PropertyType.Texture2D) &&
                !(property.propertyType == PropertyType.Texture2DArray) &&
                !(property.propertyType == PropertyType.Texture3D) &&
                !(property.propertyType == PropertyType.Cubemap))
                return base.GetVariableNameForSlot(slotId);

            return property.referenceName;
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
