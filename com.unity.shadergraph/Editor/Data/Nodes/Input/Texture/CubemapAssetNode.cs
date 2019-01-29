using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Texture", "Cubemap Asset")]
    class CubemapAssetNode : AbstractMaterialNode, IPropertyFromNode
    {
        public const int OutputSlotId = 0;

        const string kOutputSlotName = "Out";

        public CubemapAssetNode()
        {
            name = "Cubemap Asset";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Cubemap-Asset-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new CubemapMaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output));
            RemoveSlotsNameNotMatching(new[] { OutputSlotId });
        }

        [SerializeField]
        private SerializableCubemap m_Cubemap = new SerializableCubemap();

        [CubemapControl("")]
        public Cubemap cubemap
        {
            get { return m_Cubemap.cubemap; }
            set
            {
                if (m_Cubemap.cubemap == value)
                    return;
                m_Cubemap.cubemap = value;
                Dirty(ModificationScope.Node);
            }
        }

        public override void CollectGraphInputs(PropertyCollector properties, GenerationMode generationMode)
        {
            properties.AddGraphInput(new ShaderProperty(PropertyType.Cubemap));
            // {
            //     overrideReferenceName = GetVariableNameForSlot(OutputSlotId),
            //     generatePropertyBlock = true,
            //     value = m_Cubemap,
            //     modifiable = false
            // });
        }

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            properties.Add(new PreviewProperty(PropertyType.Cubemap)
            {
                name = GetVariableNameForSlot(OutputSlotId),
                cubemapValue = cubemap
            });
        }

        public ShaderProperty AsShaderProperty()
        {
            var prop = new ShaderProperty(PropertyType.Cubemap);// { value = m_Cubemap };
            if (cubemap != null)
                prop.displayName = cubemap.name;
            return prop;
        }

        public int outputSlotId { get { return OutputSlotId; } }
    }
}
