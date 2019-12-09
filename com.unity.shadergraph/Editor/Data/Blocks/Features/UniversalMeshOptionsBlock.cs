using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Universal", "Universal Mesh Options")]
    class UniversalMeshOptionsBlock : BlockData
    {
        public enum MaterialType
        {
            Unlit,
            Lit,
        }

        public enum WorkflowMode
        {
            Metallic,
            Specular,
        }        

        [SerializeField]
        MaterialType m_MaterialType;

        [SerializeField]
        WorkflowMode m_WorkflowMode;

        public UniversalMeshOptionsBlock()
        {
            name = "Universal Mesh Options";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => GetRequiredBlockTypes();
        
        [EnumControl("Material")]
        public MaterialType materialType
        {
            get => m_MaterialType;
            set 
            {
                m_MaterialType = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [EnumControl("Workflow")]
        public WorkflowMode workflowMode
        {
            get => m_WorkflowMode;
            set 
            {
                m_WorkflowMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        Type[] GetRequiredBlockTypes()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(BaseColorBlock));

            if(materialType == MaterialType.Lit)
            {
                if(workflowMode == WorkflowMode.Specular)
                    types.Add(typeof(SpecularColorBlock));
                else
                    types.Add(typeof(MetallicBlock));
                types.Add(typeof(SmoothnessBlock));
            }

            return types.ToArray();
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.SpecularSetup, workflowMode == WorkflowMode.Specular),
            };
        }
    }
}
