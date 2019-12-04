using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph.Internal
{
    class DefaultVFXTarget : ITargetImplementation
    {
        public Type targetType => typeof(VFXTarget);
        public string displayName => "Default";
        public string passTemplatePath => null;
        public string sharedTemplateDirectory => null;

        public Type[] requireBlocks => null;

        public void SetupTarget(ref TargetSetupContext context)
        {
        }

        public List<Type> GetSupportedBlocks(List<BlockData> currentBlocks)
        {
            var supportedBlocks = ListPool<Type>.Get();

            // Add always supported features
            supportedBlocks.Add(typeof(VisualEffectOptions));
            supportedBlocks.Add(typeof(AlphaClipBlock));
            
            // Add always supported data
            supportedBlocks.Add(typeof(BaseColorBlock));
            supportedBlocks.Add(typeof(AlphaBlock));

            // Evaluate remaining supported blocks
            if(currentBlocks.Any(x => x is VisualEffectOptions optionsBlock && optionsBlock.materialType == VisualEffectOptions.MaterialType.Lit))
            {
                supportedBlocks.Add(typeof(MetallicBlock));
                supportedBlocks.Add(typeof(SmoothnessBlock));
                supportedBlocks.Add(typeof(NormalTSBlock));
                supportedBlocks.Add(typeof(EmissionBlock));
            }
            if(currentBlocks.Any(x => x is AlphaClipBlock alphaClipBlock))
            {
                supportedBlocks.Add(typeof(ClipThresholdBlock));
            }

            return supportedBlocks;
        }
    }
}
