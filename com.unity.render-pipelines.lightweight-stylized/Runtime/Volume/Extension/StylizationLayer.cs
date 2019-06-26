using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.ToonPipeline;

namespace UnityEngine.Experimental.Rendering
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    [AddComponentMenu("Rendering/Stylization Layer", 1000)]
    [RequireComponent(typeof(Renderer))]
    public sealed class StylizationLayer : Volume
    {
        [SerializeField]
        private LayerMask m_VolumeLayer;
        private Transform m_Trigger;  
        private Renderer m_Renderer;
        private Material[] m_Materials;
        private VolumeStack m_Stack;
        private LayerMask m_PrevLayer;

        public LayerMask volumeLayer
        {
            get { return m_VolumeLayer; }
            set { m_VolumeLayer = value; }
        }

        public Transform trigger
        {
            get { return m_Trigger; }
        }

        public Renderer rend
        {
            get 
            { 
                if(m_Renderer == null)
                    m_Renderer = GetComponent<Renderer>();
                return m_Renderer; 
            }
        }

        private ToonPipelineAsset m_PipelineAsset;
        public ToonPipelineAsset pipelineAsset
        {
            get
            {
                if(m_PipelineAsset == null)
                    m_PipelineAsset = GraphicsSettings.renderPipelineAsset as ToonPipelineAsset;
                return m_PipelineAsset;
            }
        }

        private void OnEnable()
        {
            m_Trigger = transform;
            m_PrevLayer = gameObject.layer;
            m_Stack = VolumeManager.instance.CreateStack();

            UpdateBaseProfile();
            VolumeManager.instance.Register(this, gameObject.layer);
        }

        private void OnDisable()
        {
            // Disable all local material keywords
            foreach(VolumeComponent component in m_Stack.components.Values)
            {
                IMaterialStyle style = component as IMaterialStyle;
                if(style == null)
                    continue;

                foreach(Material mat in rend.sharedMaterials)
                    MaterialStyleUtils.SetKeyword(StyleScope.Local, mat, style, false);
            }

            VolumeManager.instance.Unregister(this, gameObject.layer);
        }

        private void Update()
        {
            if(m_PrevLayer != gameObject.layer)
            {
                VolumeManager.instance.Unregister(this, gameObject.layer);
                m_PrevLayer = gameObject.layer;
                VolumeManager.instance.Register(this, gameObject.layer);
            }

            LayerMask objectLayer = LayerMask.GetMask(new string[] { LayerMask.LayerToName(gameObject.layer) });

            // Get volume stack data
            VolumeManager.instance.Update(m_Stack, trigger, (int)volumeLayer | (int)objectLayer);
            EvaluateMaterialStyle();
        }

        private void UpdateBaseProfile()
        {
            if(pipelineAsset)
            {
                profile = pipelineAsset.profile;
                priority = -1;
                isGlobal = true;
            }
        }

        private void EvaluateMaterialStyle()
        {
            // Iterate all material style components on the stack
            // Set shader keyword and value arrays on matching components
            foreach(VolumeComponent component in m_Stack.components.Values)
            {
                IMaterialStyle style = component as IMaterialStyle;
                if(style == null)
                    continue;

                MaterialStyleData[] styleData = style.GetValue();
                foreach(Material mat in rend.sharedMaterials)
                {
                    foreach(MaterialStyleData data in styleData)
                        MaterialStyleUtils.SetVariable(StyleScope.Local, mat, data);
                    MaterialStyleUtils.SetKeyword(StyleScope.Local, mat, style, component.active);
                }
            }
        }
    }
}