using UnityEngine.Rendering;
using UnityEngine;
using UnityEditor;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
	public static class VolumeUtils 
	{
		public static VolumeProfile CreateVolumeProfile(string path, string name)
		{
			var fullPath = string.Format("{0}/{1}.asset", path, name);
			fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

			var profile = ScriptableObject.CreateInstance<VolumeProfile>();
			AssetDatabase.CreateAsset(profile, fullPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return profile;
		}
	}
}
