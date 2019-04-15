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


        private void UpdateNode()
        {
            var graph = owner as GraphData;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            if (property is Vector1ShaderProperty)
            {
                AddSlot(new Vector1MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, 0));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector2ShaderProperty)
            {
                AddSlot(new Vector2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector3ShaderProperty)
            {
                AddSlot(new Vector3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Vector4ShaderProperty)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is ColorShaderProperty)
            {
                AddSlot(new Vector4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, Vector4.zero));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is TextureShaderProperty)
            {
                AddSlot(new Texture2DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Texture2DArrayShaderProperty)
            {
                AddSlot(new Texture2DArrayMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is Texture3DShaderProperty)
            {
                AddSlot(new Texture3DMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] {OutputSlotId});
            }
            else if (property is CubemapShaderProperty)
            {
                AddSlot(new CubemapMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is BooleanShaderProperty)
            {
                AddSlot(new BooleanMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output, false));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is Matrix2ShaderProperty)
            {
                AddSlot(new Matrix2MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is Matrix3ShaderProperty)
            {
                AddSlot(new Matrix3MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is Matrix4ShaderProperty)
            {
                AddSlot(new Matrix4MaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is SamplerStateShaderProperty)
            {
                AddSlot(new SamplerStateMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
            else if (property is GradientShaderProperty)
            {
                AddSlot(new GradientMaterialSlot(OutputSlotId, property.displayName, "Out", SlotType.Output));
                RemoveSlotsNameNotMatching(new[] { OutputSlotId });
            }
        }

        public void GenerateNodeCode(ShaderSnippetRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            var graph = owner as GraphData;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);
            if (property == null)
                return;

            using(registry.ProvideSnippet(GetVariableNameForNode(), guid, out var s))
            {
                if (property is Vector1ShaderProperty)
                {
                    s.AppendLine("$precision {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Vector2ShaderProperty)
                {
                    s.AppendLine("$precision2 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Vector3ShaderProperty)
                {
                    s.AppendLine("$precision3 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Vector4ShaderProperty)
                {
                    s.AppendLine("$precision4 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is ColorShaderProperty)
                {
                    s.AppendLine("$precision4 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is BooleanShaderProperty)
                {
                    s.AppendLine("$precision {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Matrix2ShaderProperty)
                {
                    s.AppendLine("$precision2x2 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Matrix3ShaderProperty)
                {
                    s.AppendLine("$precision3x3 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is Matrix4ShaderProperty)
                {
                    s.AppendLine("$precision4x4 {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                }
                else if (property is SamplerStateShaderProperty)
                {
                    SamplerStateShaderProperty samplerStateProperty = property as SamplerStateShaderProperty;
                    s.AppendLine("SamplerState {0} = {1}_{2}_{3};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , samplerStateProperty.referenceName
                            , samplerStateProperty.value.filter
                            , samplerStateProperty.value.wrap);
                }
                else if (property is GradientShaderProperty)
                {
                    if(generationMode == GenerationMode.Preview)
                    {
                        s.AppendLine("Gradient {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId) 
                            , GradientUtils.GetGradientForPreview(property.referenceName));
                    }
                    else
                    {
                        s.AppendLine("Gradient {0} = {1};"
                            , GetVariableNameForSlot(OutputSlotId)
                            , property.referenceName);
                    }
                }
            }
        }

        public Guid propertyGuid
        {
            get { return m_PropertyGuid; }
            set
            {
                if (m_PropertyGuid == value)
                    return;

                var graph = owner as GraphData;
                var property = graph.properties.FirstOrDefault(x => x.guid == value);
                if (property == null)
                    return;
                m_PropertyGuid = value;

                UpdateNode();

                Dirty(ModificationScope.Topological);
            }
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            var graph = owner as GraphData;
            var property = graph.properties.FirstOrDefault(x => x.guid == propertyGuid);

            if (!(property is TextureShaderProperty) &&
                !(property is Texture2DArrayShaderProperty) &&
                !(property is Texture3DShaderProperty) &&
                !(property is CubemapShaderProperty))
                return base.GetVariableNameForSlot(slotId);

            return property.referenceName;
        }

        protected override bool CalculateNodeHasError(ref string errorMessage)
        {
            var graph = owner as GraphData;

            if (!propertyGuid.Equals(Guid.Empty) && !graph.properties.Any(x => x.guid == propertyGuid))
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
