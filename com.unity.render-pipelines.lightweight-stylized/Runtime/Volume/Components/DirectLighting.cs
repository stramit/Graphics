using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    [Serializable]
    public class DirectLighting : VolumeComponent, IMaterialStyle
    {
		public BoolParameter                	enable = new BoolParameter(true);
        public GradientParameter                diffuse = new GradientParameter(new Gradient());
        public GradientParameter        		specular = new GradientParameter(new Gradient());

		public string GetKeywordName () { return "DIRECTLIGHTING"; }

		public MaterialStyleData[] GetValue()
        {
			Gradient defaultGradient = new Gradient() 
											{colorKeys = new GradientColorKey[2] 
											{
												new GradientColorKey(Color.black, 0), 
												new GradientColorKey(Color.white, 1)
											}};
			Gradient diffuseGradient = enable.value ? diffuse.value : defaultGradient;
			Gradient specularGradient = enable.value ? specular.value : defaultGradient;
			
			Texture2D texture = TextureUtils.BakeGradientsToTexture2d(new Gradient[2]{diffuseGradient, specularGradient}, 128);
			MaterialStyleData styleData = new MaterialStyleData("DirectLighting", texture, typeof(Texture2D));
			return new MaterialStyleData[] {styleData};
        }
    }
}