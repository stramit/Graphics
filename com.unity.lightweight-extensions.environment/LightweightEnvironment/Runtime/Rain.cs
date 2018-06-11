using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline.Extensions
{
    [Serializable]
    public class Rain : VolumeComponent, IRendererEffect
    {
		[DisplayName("Enable"), Tooltip("")]
      	public BoolParameter                	enable = new BoolParameter(true);

		[DisplayName("Amount"), Range(0f, 1f), Tooltip("")]
		public FloatParameter					amount = new FloatParameter(1);

		[DisplayName("Strength"), Range(0f, 1f), Tooltip("")]
		public FloatParameter					strength = new FloatParameter(1);

		[DisplayName("Speed"), Range(0f, 1f), Tooltip("")]
		public FloatParameter					speed = new FloatParameter(1);
      	
		public string GetKeywordName () { return "WIND"; }

		public EffectData[] GetValue(Listener listener)
		{
			EffectData amountData = new EffectData("RainAmount", enable ? amount.GetValue<float>() : 0, typeof(float));
			EffectData strengthData = new EffectData("RainStrength",  strength.GetValue<float>(), typeof(float));
			EffectData speedData = new EffectData("RainSpeed", speed.GetValue<float>(), typeof(float));
			return new EffectData[] { amountData, strengthData, speedData };
		}
    }
}