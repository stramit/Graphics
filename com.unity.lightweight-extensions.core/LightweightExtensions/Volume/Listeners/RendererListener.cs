using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    [AddComponentMenu("Volume Listeners/Renderer Listener", 1000)]
    [RequireComponent(typeof(Renderer))]
    public sealed class RendererListener : Listener
    {
        private Renderer m_Renderer;
        private Material[] m_Materials;
        private MaterialPropertyBlock m_Block;

        public Renderer renderer
        {
            get 
            { 
                if(m_Renderer == null)
                    m_Renderer = GetComponent<Renderer>();
                return m_Renderer; 
            }
        }

        public override void OnEnable()
        {
            m_Block = new MaterialPropertyBlock();
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            m_Block = null;
        }

        public override void Update()
        {
            m_Block.Clear();
            base.Update();
            renderer.SetPropertyBlock(m_Block);
        }

        public override void DisableComponent(VolumeComponent component)
        {
            IRendererEffect effect = component as IRendererEffect;
            if(effect == null)
                return;

            foreach(Material mat in renderer.sharedMaterials)
                RendererEffectUtils.SetKeyword(EffectScope.Local, mat, effect, false);
        }

        public override void EvaluateComponent(VolumeComponent component)
        {
            IRendererEffect effect = component as IRendererEffect;
            if(effect == null)
                return;

            EffectData[] effectData = effect.GetValue(this);
            foreach(Material mat in renderer.sharedMaterials)
            {
                foreach(EffectData data in effectData)
                    RendererEffectUtils.SetVariable(EffectScope.Local, m_Block, data);
                RendererEffectUtils.SetKeyword(EffectScope.Local, mat, effect, true);
            }
        }
    }
}