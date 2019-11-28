using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    static class CreateShaderGraph
    {
        [MenuItem("Assets/Create/Shader/Mesh Shader Graph", false, 208)]
        public static void CreateLitGraph()
        {
            GraphUtil.CreateNewGraph(typeof(MeshTarget));
        }

        [MenuItem("Assets/Create/Shader/Sprite Shader Graph", false, 208)]
        public static void CreateSpriteLitGraph()
        {
            GraphUtil.CreateNewGraph(typeof(SpriteTarget));
        }

        [MenuItem("Assets/Create/Shader/Visual Effect Shader Graph", false, 208)]
        public static void CreateVisualEffectGraph()
        {
            GraphUtil.CreateNewGraph(typeof(VFXTarget));
        }
    }
}
