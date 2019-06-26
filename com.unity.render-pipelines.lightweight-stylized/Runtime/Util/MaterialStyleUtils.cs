using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	public enum StyleScope { Local, Global }

	[Serializable]
	public class MaterialStyleData
	{
		public string variableName;
		public object data;
		public Type dataType;

		public MaterialStyleData(string name, object obj, Type type)
		{
			variableName = name;
			data = obj;
			dataType = type;
		}
	}

	public static class MaterialStyleUtils 
	{
		public static string GetKeyword(StyleScope scope, IMaterialStyle style)
		{
			// Get a full keyword name from an IMaterialStyle
			string prefix = scope == StyleScope.Global ? "_GLOBAL_" : "_LOCAL_";
			return prefix + style.GetKeywordName();
		}

		public static string GetVariable(StyleScope scope, MaterialStyleData styleData)
		{
			// Get a full variable name from a MaterialStyleData
			string prefix = scope == StyleScope.Global ? "_Global" : "_Local";
			return prefix + styleData.variableName;
		}

		public static void SetKeyword(StyleScope scope, object obj, IMaterialStyle style, bool state)
        {
			switch(scope)
			{
				case StyleScope.Global:
					CommandBuffer cmd;
					if(!CastToCommandBuffer(obj, out cmd))
						return;

					string globalKeyword = GetKeyword(StyleScope.Global, style);
					SetKeyword(cmd, globalKeyword, state);
					break;
				case StyleScope.Local:
					Material mat;
					if(!CastToMaterial(obj, out mat))
						return;
					
					string localKeyword = GetKeyword(StyleScope.Local, style);
					SetKeyword(mat, localKeyword, state);
					break;
			}           
        }

		public static void SetVariable(StyleScope scope, object obj, MaterialStyleData styleData)
		{
			var variableName = GetVariable(scope, styleData);
			switch(scope)
			{
				case StyleScope.Global:
					CommandBuffer cmd;
					if(!CastToCommandBuffer(obj, out cmd))
						return;

					if(styleData.dataType == typeof(Color))
						cmd.SetGlobalColor(variableName, (Color)styleData.data);
					else if(styleData.dataType == typeof(Texture2D))
						cmd.SetGlobalTexture(variableName, (Texture2D)styleData.data);
					else if(styleData.dataType == typeof(int))
						cmd.SetGlobalInt(variableName, (int)styleData.data);
					else if(styleData.dataType == typeof(float))
						cmd.SetGlobalFloat(variableName, (float)styleData.data);
					else if(styleData.dataType == typeof(Vector4))
						cmd.SetGlobalVector(variableName, (Vector4)styleData.data);
					break;
				case StyleScope.Local:
					Material mat;
					if(!CastToMaterial(obj, out mat))
						return;
					
					if(styleData.dataType == typeof(Color))
						mat.SetColor(variableName, (Color)styleData.data);
					else if(styleData.dataType == typeof(Texture2D))
						mat.SetTexture(variableName, (Texture2D)styleData.data);
					else if(styleData.dataType == typeof(int))
						mat.SetInt(variableName, (int)styleData.data);
					else if(styleData.dataType == typeof(float))
						mat.SetFloat(variableName, (float)styleData.data);
					else if(styleData.dataType == typeof(Vector4))
						mat.SetVector(variableName, (Vector4)styleData.data);
					break;
			}
		}

		public static void SetKeyword(CommandBuffer cmd, string keyword, bool state)
        {
			// Set keyword on a Command Buffer
            if(state == true)
                cmd.EnableShaderKeyword(keyword);
            else
                cmd.DisableShaderKeyword(keyword);
        }

		private static void SetKeyword(Material mat, string keyword, bool state)
		{
			// Set keyword on a Material
			if(state == true)
				mat.EnableKeyword(keyword);
			else
				mat.DisableKeyword(keyword);
		}

		private static bool CastToCommandBuffer(object obj, out CommandBuffer cmd)
		{
			// Test cast object to Command Buffer returning error in failure case
			cmd = obj as CommandBuffer;
			if(cmd == null)
				Debug.LogError("Global material style object must be of type \"Command Buffer\"");
			return cmd != null;
		}

		private static bool CastToMaterial(object obj, out Material mat)
		{
			// Test cast object to Material returning error in failure case
			mat = obj as Material;
			if(mat == null)
				Debug.LogError("Local material style object must be of type \"Material\"");
			return mat != null;
		}
	}
}