#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    using CoreUtils = UnityEngine.Rendering.CoreUtils;

    internal enum DefaultMaterialType
    {
        Standard,
        Particle,
        Terrain,
        Sprite,
        UnityBuiltinDefault
    }

    public class ToonPipelineAsset : LightweightRenderPipelineAsset, ISerializationCallbackReceiver
    {      
        public static readonly string s_LWSearchPathPackage = "Packages/com.unity.render-pipelines.lightweight";
        public static readonly string s_ToonSearchPathPackage = "Packages/com.kink3d.toon-pipeline";
        Shader m_DefaultToonShader;
        internal ScriptableRenderer m_Renderer;
        [SerializeField] internal ScriptableRendererData m_RendererData = null;

        [SerializeField] private VolumeProfile m_Profile;
        public VolumeProfile profile
        {
            get { return m_Profile; }
            private set { m_Profile = value; }
        }

#if UNITY_EDITOR
        [NonSerialized] ToonPipelineEditorResources m_EditorResourcesAsset;

        [MenuItem("Assets/Create/Rendering/Toon Pipeline Asset", priority = CoreUtils.assetCreateMenuPriority1)]
        static void CreateToonPipeline()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateToonPipelineAsset>(),
                "ToonPipelineAsset.asset", null, null);
        }

        //[MenuItem("Assets/Create/Rendering/Toon Pipeline Editor Resources", priority = CoreUtils.assetCreateMenuPriority1)]
        static void CreateToonPipelineEditorResources()
        {
            var instance = CreateInstance<ToonPipelineEditorResources>();
            AssetDatabase.CreateAsset(instance, string.Format("Assets/{0}.asset", typeof(ToonPipelineEditorResources).Name));
        }

        public static ToonPipelineAsset Create()
        {
            var instance = CreateInstance<ToonPipelineAsset>();

            instance.m_RendererData = instance.LoadBuiltinRendererData();
            instance.m_EditorResourcesAsset = LoadResourceFile<ToonPipelineEditorResources>();
            instance.m_Renderer = instance.m_RendererData.InternalCreateRenderer();
            return instance;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
        internal class CreateToonPipelineAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                AssetDatabase.CreateAsset(Create(), pathName);
            }
        }

        static T LoadResourceFile<T>(string projectPath, string packagePath) where T : ScriptableObject
        {
            T resourceAsset = null;
            var guids = AssetDatabase.FindAssets(typeof(T).Name + " t:scriptableobject", new[] {projectPath});
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                resourceAsset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (resourceAsset != null)
                    break;
            }

            // There's currently an issue that prevents FindAssets from find resources withing the package folder.
            if (resourceAsset == null)
            {
                string path = packagePath + typeof(T).Name + ".asset";
                resourceAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return resourceAsset;
        }

        ToonPipelineEditorResources editorResources
        {
            get
            {
                if (m_EditorResourcesAsset == null)
                    m_EditorResourcesAsset = LoadResourceFile<ToonPipelineEditorResources>();

                return m_EditorResourcesAsset;
            }
        }
#endif

        Material GetMaterial(DefaultMaterialType materialType)
        {
#if UNITY_EDITOR
            if (editorResources == null)
                return null;

            switch (materialType)
            {
                case DefaultMaterialType.Standard:
                    return editorResources.DefaultMaterial;

                case DefaultMaterialType.Particle:
                    return editorResources.DefaultParticleMaterial;

                case DefaultMaterialType.Terrain:
                    return editorResources.DefaultTerrainMaterial;

                // Unity Builtin Default
                default:
                    return null;
            }
#else
            return null;
#endif
        }

        // public override Material GetDefaultMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.Standard);
        // }

        // public override Material GetDefaultParticleMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.Particle);
        // }

        // public override Material GetDefaultLineMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.UnityBuiltinDefault);
        // }

        // public override Material GetDefaultTerrainMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.Terrain);
        // }

        // public override Material GetDefaultUIMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.UnityBuiltinDefault);
        // }

        // public override Material GetDefaultUIOverdrawMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.UnityBuiltinDefault);
        // }

        // public override Material GetDefaultUIETC1SupportedMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.UnityBuiltinDefault);
        // }

        // public override Material GetDefault2DMaterial()
        // {
        //     return GetMaterial(DefaultMaterialType.UnityBuiltinDefault);
        // }

        public override Shader defaultShader
        {
            get
            {
                if (m_DefaultToonShader == null)
                    m_DefaultToonShader = Shader.Find(ShaderUtils.GetShaderPath(ShaderPathID.Lit));
                return m_DefaultToonShader;
            }
        }
    }
}