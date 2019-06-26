using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    [Serializable]
    public class Fog : VolumeComponent, IMaterialStyle
    {
		public BoolParameter                	enable = new BoolParameter(true);
        public GradientParameter                color = new GradientParameter(new Gradient());

		public string GetKeywordName () { return "FOG"; }

		public MaterialStyleData[] GetValue()
        {
			Gradient defaultColor = new Gradient() 
											{colorKeys = new GradientColorKey[1] 
											{
												new GradientColorKey(UnityEngine.RenderSettings.fogColor, 0)
											},
											alphaKeys = new GradientAlphaKey[2] 
											{
												new GradientAlphaKey(0, 0), 
												new GradientAlphaKey(1, 1)
											}};

			Gradient colorGradient = color.overrideState ? color.value : defaultColor;
			Gradient alphaGradient = new Gradient();
			List<GradientColorKey> keys = new List<GradientColorKey>();

			foreach(GradientAlphaKey key in color.value.alphaKeys)
				keys.Add(new GradientColorKey(new Color(key.alpha, key.alpha, key.alpha, key.alpha), key.time));

			alphaGradient.colorKeys = keys.ToArray();

			Texture2D texture = TextureUtils.BakeGradientsToTexture2d(new Gradient[2]{colorGradient, alphaGradient}, 128);
			MaterialStyleData styleData = new MaterialStyleData("Fog", texture, typeof(Texture2D));
			return new MaterialStyleData[] {styleData};
        }
    }
}