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
    public sealed class RendererListener : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_VolumeLayer;
        private Transform m_Trigger;  
        private VolumeStack m_Stack;

        private Renderer m_Renderer;
        private Material[] m_Materials;
        private MaterialPropertyBlock m_Block;

        public LayerMask volumeLayer
        {
            get { return m_VolumeLayer; }
            set { m_VolumeLayer = value; }
        }

        public Transform trigger
        {
            get { return m_Trigger; }
        }

        public Renderer renderer
        {
            get 
            { 
                if(m_Renderer == null)
                    m_Renderer = GetComponent<Renderer>();
                return m_Renderer; 
            }
        }

        private void OnEnable()
        {
            m_Trigger = transform;
            m_Block = new MaterialPropertyBlock();
            m_Stack = VolumeManager.instance.CreateStack();
        }

        private void OnDisable()
        {
            // Disable all local material keywords
            if(m_Stack != null)
            {
                foreach(VolumeComponent component in m_Stack.components.Values)
                {
                    IRendererEffect effect = component as IRendererEffect;
                    if(effect == null)
                        continue;

                    foreach(Material mat in renderer.sharedMaterials)
                        RendererEffectUtils.SetKeyword(EffectScope.Local, mat, effect, false);
                }
            }

            m_Block = null;
        }

        private void Update()
        {
            // Get volume stack data
            if(m_Stack != null)
            {
                VolumeManager.instance.Update(m_Stack, trigger, volumeLayer);
                EvaluateComponents();
            }
        }

        private void EvaluateComponents()
        {
            m_Block.Clear();

            // Iterate all material style components on the stack
            // Set shader keyword and value arrays on matching components
            foreach(VolumeComponent component in m_Stack.components.Values)
            {
                IRendererEffect effect = component as IRendererEffect;
                if(effect == null)
                    continue;

                EffectData[] effectData = effect.GetValue(this);
                foreach(Material mat in renderer.sharedMaterials)
                {
                    foreach(EffectData data in effectData)
                        RendererEffectUtils.SetVariable(EffectScope.Local, m_Block, data);
                    RendererEffectUtils.SetKeyword(EffectScope.Local, mat, effect, true);
                }
            }

            renderer.SetPropertyBlock(m_Block);
        }
    }
}