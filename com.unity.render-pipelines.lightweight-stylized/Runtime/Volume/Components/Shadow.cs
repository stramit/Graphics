using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    [Serializable]
    public class Shadow : VolumeComponent, IMaterialStyle
    {
		public BoolParameter                	enable = new BoolParameter(true);
        public GradientParameter                color = new GradientParameter(new Gradient());

		public string GetKeywordName () { return "SHADOW"; }

		public MaterialStyleData[] GetValue()
        {
			Gradient defaultGradient = new Gradient() 
											{colorKeys = new GradientColorKey[2] 
											{
												new GradientColorKey(Color.black, 0), 
												new GradientColorKey(Color.white, 1)
											}};

			Gradient colorGradient = enable.value ? color.value : defaultGradient;
			
			Texture2D texture = TextureUtils.BakeGradientsToTexture2d(new Gradient[1]{colorGradient}, 128);
			MaterialStyleData styleData = new MaterialStyleData("Shadow", texture, typeof(Texture2D));
			return new MaterialStyleData[] {styleData};
        }
    }
}