using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEditor.Rendering.HighDefinition.ShaderGraph;

namespace UnityEditor.Rendering.HighDefinition
{
    public class HDShaderUtils
    {
        //enum representing all shader and shadergraph that we expose to user
        public enum ShaderID
        {
            Lit,
            LitTesselation,
            LayeredLit,
            LayeredLitTesselation,
            Unlit,
            Decal,
            TerrainLit,
            AxF,
            Count_Standard,
            SG_Unlit = Count_Standard,
            SG_Lit,
            SG_Hair,
            SG_Fabric,
            SG_StackLit,
            SG_Decal,
            SG_Eye,
            Count_All,
            Count_ShaderGraph = Count_All - Count_Standard
        }

        // exposed shader, for reference while searching the ShaderID
        static readonly string[] s_ShaderPaths =
        {
            "HDRP/Lit",
            "HDRP/LitTessellation",
            "HDRP/LayeredLit",
            "HDRP/LayeredLitTessellation",
            "HDRP/Unlit",
            "HDRP/Decal",
            "HDRP/TerrainLit",
            "HDRP/AxF",
        };

        // exposed shadergraph, for reference while searching the ShaderID
        // static readonly Type[] s_MasterNodes =
        // {
        //     typeof(HDUnlitMasterNode),
        //     typeof(HDLitMasterNode),
        //     typeof(HairMasterNode),
        //     typeof(FabricMasterNode),
        //     typeof(StackLitMasterNode),
        //     typeof(DecalMasterNode),
        //     typeof(EyeMasterNode),
        // };
        
        // list of methods for resetting keywords
        delegate void MaterialResetter(Material material);
        static Dictionary<ShaderID, MaterialResetter> k_MaterialResetters = new Dictionary<ShaderID, MaterialResetter>()
        {
            { ShaderID.Lit, LitGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.LitTesselation, LitGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.LayeredLit,  LayeredLitGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.LayeredLitTesselation, LayeredLitGUI.SetupMaterialKeywordsAndPass },
            // no entry for ShaderID.StackLit
            { ShaderID.Unlit, UnlitGUI.SetupUnlitMaterialKeywordsAndPass },
            // no entry for ShaderID.Fabric
            { ShaderID.Decal, DecalUI.SetupMaterialKeywordsAndPass },
            { ShaderID.TerrainLit, TerrainLitGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.AxF, AxFGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.SG_Unlit, UnlitGUI.SetupUnlitMaterialKeywordsAndPass },
            { ShaderID.SG_Lit, HDLitGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.SG_Hair, HairGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.SG_Fabric, FabricGUI.SetupMaterialKeywordsAndPass },
            { ShaderID.SG_StackLit, StackLitGUI.SetupMaterialKeywordsAndPass },
            // no entry for ShaderID.SG_Decal
            // no entry for ShaderID.SG_Eye
        };

        /// <summary>
        /// Reset the dedicated Keyword and Pass regarding the shader kind.
        /// Also re-init the drawers and set the material dirty for the engine.
        /// </summary>
        /// <param name="material">The material that needs to be setup</param>
        /// <returns>
        /// True: managed to do the operation.
        /// False: unknown shader used in material
        /// </returns>
        public static bool ResetMaterialKeywords(Material material)
        {
            MaterialResetter resetter;

            // If we send a non HDRP material we don't throw an exception, the return type already handles errors.
            try {
                k_MaterialResetters.TryGetValue(GetShaderEnumFromShader(material.shader), out resetter);
            } catch {
                return false;
            }

            if (resetter != null)
            {
                CoreEditorUtils.RemoveMaterialKeywords(material);
                // We need to reapply ToggleOff/Toggle keyword after reset via ApplyMaterialPropertyDrawers
                MaterialEditor.ApplyMaterialPropertyDrawers(material);
                resetter(material);
                EditorUtility.SetDirty(material);
                return true;
            }

            return false;
        }

        /// <summary>Gather all the shader preprocessors</summary>
        /// <returns>The list of shader preprocessor</returns>
        internal static List<BaseShaderPreprocessor> GetBaseShaderPreprocessorList()
            => UnityEngine.Rendering.CoreUtils
                .GetAllTypesDerivedFrom<BaseShaderPreprocessor>()
                .Select(Activator.CreateInstance)
                .Cast<BaseShaderPreprocessor>()
                .OrderByDescending(spp => spp.Priority)
                .ToList();
        
        internal static bool IsHDRPShader(Shader shader, bool upgradable = false)
        {
            if (shader == null)
                return false;

            if (shader.IsShaderGraph())
            {
                return true;
            }
            else if (upgradable)
                return s_ShaderPaths.Contains(shader.name);
            else
                return shader.name.Contains("HDRP");
        }

        internal static bool IsUnlitHDRPShader(Shader shader)
        {
            if (shader == null)
                return false;

            if (shader.IsShaderGraph())
            {
                // TODO
                // We currently need to deserialize the entire graph
                // to determine what blocks are active.
                // We should store the stack state as objects in metadata
                // So we dont need to deserialize the graph and external
                // packages can read the metadata without access to internals.
                var path = AssetDatabase.GetAssetPath(shader);
                var textGraph = System.IO.File.ReadAllText(path);
                var set = JsonStore.Deserialize(textGraph);
                var graphData = set.First<GraphData>();

                if(graphData.contextManager.allBlocks.Any(x => x is HDRPMeshOptionsBlock optionsBlock &&
                    optionsBlock.lightingType == HDRPMeshOptionsBlock.LightingType.Lit))
                {
                    return false;
                }

                return true;
            }
            else
                return shader.name == "HDRP/Unlit";
        }

        internal static string GetShaderPath(ShaderID id)
        {
            int index = (int)id;
            if (index < 0 && index >= (int)ShaderID.Count_Standard)
            {
                Debug.LogError("Trying to access HDRP shader path out of bounds");
                return "";
            }

            return s_ShaderPaths[index];
        }

        internal static ShaderID GetShaderEnumFromShader(Shader shader)
        {
            if (shader.IsShaderGraph())
            {
                // How do we resolve the GUI now?
                // There should only be one...

                // TODO
                // We currently need to deserialize the entire graph
                // to determine what lighting type this is...
                var path = AssetDatabase.GetAssetPath(shader);
                var textGraph = System.IO.File.ReadAllText(path);
                var set = JsonStore.Deserialize(textGraph);
                var graphData = set.First<GraphData>();
                graphData.contextManager.UpdateBlocks(false);

                var optionsBlock = graphData.contextManager.allBlocks.FirstOrDefault(x => x is HDRPMeshOptionsBlock)
                    as HDRPMeshOptionsBlock;
                var index = (int)optionsBlock.lightingType;
                if (index == -1)
                    throw new ArgumentException("Unknown shader");
                return (ShaderID)(index + ShaderID.Count_Standard);
            }
            else
            {
                var index = Array.FindIndex(s_ShaderPaths, m => m == shader.name);
                if (index == -1)
                    throw new ArgumentException("Unknown shader");
                return (ShaderID)index;
            }
        }
    }
}
