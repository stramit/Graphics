using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public abstract class Listener : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_VolumeLayer;
        private Transform m_Trigger;  
        private VolumeStack m_Stack;

        public LayerMask volumeLayer
        {
            get { return m_VolumeLayer; }
            set { m_VolumeLayer = value; }
        }

        public Transform trigger
        {
            get { return m_Trigger; }
        }

        public virtual void OnEnable()
        {
            m_Trigger = transform;
            m_Stack = VolumeManager.instance.CreateStack();
        }

        public virtual void OnDisable()
        {
            if(m_Stack != null)
            {
                foreach(VolumeComponent component in m_Stack.components.Values)
                    DisableComponent(component);
            }
        }

        public abstract void DisableComponent(VolumeComponent component);

        public virtual void Update()
        {
            if(m_Stack != null)
            {
                VolumeManager.instance.Update(m_Stack, trigger, volumeLayer);
                
                foreach(VolumeComponent component in m_Stack.components.Values)
                    EvaluateComponent(component);
            }
        }

        public abstract void EvaluateComponent(VolumeComponent component);
    }
}