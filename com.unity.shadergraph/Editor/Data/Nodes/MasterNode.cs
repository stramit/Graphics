using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    abstract class MasterNode : AbstractMaterialNode, IMasterNode, IHasSettings
    {
        public override bool allowedInSubGraph
        {
            get { return false; }
        }

        public virtual bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            foreach (var subShader in owner.subShaders)
            {
                if (subShader.IsPipelineCompatible(GraphicsSettings.renderPipelineAsset))
                    return true;
            }
            return false;
        }

        [SerializeField]
        bool m_DOTSInstancing = false;

        public ToggleData dotsInstancing
        {
            get { return new ToggleData(m_DOTSInstancing); }
            set
            {
                if (m_DOTSInstancing == value.isOn)
                    return;

                m_DOTSInstancing = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        public abstract string GetShader(GenerationMode mode, string outputName, out List<PropertyCollector.TextureInfo> configuredTextures, List<string> sourceAssetDependencyPaths = null);
        public abstract bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset);
        public abstract int GetPreviewPassIndex();

        public VisualElement CreateSettingsElement()
        {
            var container = new VisualElement();
            var commonSettingsElement = CreateCommonSettingsElement();
            if (commonSettingsElement != null)
                container.Add(commonSettingsElement);

            return container;
        }

        protected virtual VisualElement CreateCommonSettingsElement()
        {
            return null;
        }

        public virtual object saveContext => null;

        public virtual void ProcessPreviewMaterial(Material Material) {}
    }
}
