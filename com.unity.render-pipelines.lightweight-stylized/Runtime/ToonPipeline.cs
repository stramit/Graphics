using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    using LightweightPipeline = UnityEngine.Rendering.LWRP.LightweightRenderPipeline;

    public partial class ToonPipeline : LightweightPipeline
    {
        public ToonPipelineAsset toonPipelineAsset { get; private set; }

        public ToonPipeline(ToonPipelineAsset asset) : base (asset)
        {
            toonPipelineAsset = asset;
            Shader.globalRenderPipeline = "LightweightPipeline";
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            EvaluateMaterialStyle(context);

            base.Render(context, cameras);
        }

        private void EvaluateMaterialStyle(ScriptableRenderContext context)
        {
            CommandBuffer cmd = CommandBufferPool.Get("SetupShaderConstants");

            // Iterate all material style components on the profile
            // Set shader keyword and value arrays on matching components
            foreach(VolumeComponent component in toonPipelineAsset.profile.components)
            {
                IMaterialStyle style = component as IMaterialStyle;
                if(style == null)
                    continue;

                MaterialStyleData[] styleData = style.GetValue();
                foreach(MaterialStyleData data in styleData)
                    MaterialStyleUtils.SetVariable(StyleScope.Global, cmd, data);
                MaterialStyleUtils.SetKeyword(StyleScope.Global, cmd, style, component.active);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
