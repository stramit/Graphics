using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    [Serializable]
    public class Rimlight : VolumeComponent, IMaterialStyle
    {
      	public BoolParameter                	enable = new BoolParameter(true);
      	public ColorParameter                	color = new ColorParameter(Color.white) { hdr = true, showAlpha = false };
		public FloatParameter					power = new FloatParameter(2.0f);

		public string GetKeywordName () { return "RIMLIGHT"; }

		public MaterialStyleData[] GetValue()
		{
			Color c = enable.value ? color.value : Color.black;
			float a = enable.value ? power.value : 0.0f;
			
			MaterialStyleData styleData = new MaterialStyleData("Rimlight", new Vector4(c.r, c.g, c.b, a), typeof(Vector4));
			return new MaterialStyleData[] {styleData};
		}
    }
}