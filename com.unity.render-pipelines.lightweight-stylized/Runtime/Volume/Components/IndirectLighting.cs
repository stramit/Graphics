using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
    [Serializable]
    public class IndirectLighting : VolumeComponent, IMaterialStyle
    {
      	public BoolParameter                	enable = new BoolParameter(true);
      	public ColorParameter                	color = new ColorParameter(new Color(0.2f, 0.2f, 0.2f, 1f)) { hdr = true, showAlpha = false };

		public string GetKeywordName () { return "INDIRECTLIGHTING"; }

		public MaterialStyleData[] GetValue()
		{
			Color c = enable.value ? color.value : Color.black;
			
			MaterialStyleData styleData = new MaterialStyleData("IndirectLighting", c, typeof(Color));
			return new MaterialStyleData[] {styleData};
		}
    }
}