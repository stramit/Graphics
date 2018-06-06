using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline.Extensions
{
	public interface IRendererEffect 
	{
		string GetKeywordName();

		EffectData[] GetValue(RendererListener listener);
	}
}