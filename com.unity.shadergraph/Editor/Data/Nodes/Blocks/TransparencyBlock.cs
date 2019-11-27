using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    [Title("Basic", "Transparency")]
    class TransparencyBlock : BlockData, IHasSettings
    {
        public TransparencyBlock()
        {
            name = "Transparency";
            UpdateNodeAfterDeserialization();
        }

        const int kAlphaId = 0;
        const string kAlphaName = "Alpha";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(kAlphaId, kAlphaName, kAlphaName, SlotType.Input, 1));
            RemoveSlotsNameNotMatching(new[] { kAlphaId });
        }

        [SerializeField]
        AlphaMode m_AlphaMode;

        public AlphaMode alphaMode
        {
            get { return m_AlphaMode; }
            set
            {
                if (m_AlphaMode == value)
                    return;

                m_AlphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.SurfaceTransparent, true),

                // Blend Mode
                new ConditionalField(Fields.BlendAdd, alphaMode == AlphaMode.Additive),
                new ConditionalField(Fields.BlendAlpha, alphaMode == AlphaMode.Alpha),
                new ConditionalField(Fields.BlendMultiply, alphaMode == AlphaMode.Multiply),
                new ConditionalField(Fields.BlendPremultiply, alphaMode == AlphaMode.Premultiply),
            };
        }

        public VisualElement CreateSettingsElement()
        {
            PropertySheet ps = new PropertySheet();
            ps.Add(new PropertyRow(new Label("Blend")), (row) =>
                {
                    row.Add(new EnumField(AlphaMode.Additive), (field) =>
                    {
                        field.value = alphaMode;
                        field.RegisterValueChangedCallback(evt => 
                        {
                            if (Equals(alphaMode, evt.newValue))
                                return;

                            owner.owner.RegisterCompleteObjectUndo("Alpha Mode Change");
                            alphaMode = (AlphaMode)evt.newValue;
                        });
                    });
                });

            return ps;
        }
    }
}
