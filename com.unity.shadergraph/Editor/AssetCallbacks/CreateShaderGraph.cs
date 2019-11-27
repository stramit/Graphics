using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    static class CreateShaderGraph
    {
        [MenuItem("Assets/Create/Shader/Lit Graph", false, 208)]
        public static void CreateLitGraph()
        {
            GraphUtil.CreateNewGraph(typeof(MeshTarget), new Type[] { typeof(ColorBlock), typeof(LitMetallicBlock) });
        }

        [MenuItem("Assets/Create/Shader/Sprite Lit Graph", false, 208)]
        public static void CreateSpriteLitGraph()
        {
            GraphUtil.CreateNewGraph(typeof(SpriteTarget), new Type[] { typeof(ColorBlock), typeof(LitSpriteBlock) });
        }

        [MenuItem("Assets/Create/Shader/Unlit Graph", false, 208)]
        public static void CreateUnlitGraph()
        {
            GraphUtil.CreateNewGraph(typeof(MeshTarget), new Type[] { typeof(ColorBlock) });
        }
    }
}
