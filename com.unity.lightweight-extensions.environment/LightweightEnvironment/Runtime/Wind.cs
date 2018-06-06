using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.Rendering.LightweightPipeline.Extensions
{
	public enum WindMode
    {
        Directional,
        Omni
    }

	[Serializable]
    public sealed class WindModeParameter : VolumeParameter<WindMode> {}

    [Serializable]
    public class Wind : VolumeComponent, IRendererEffect
    {
		[DisplayName("Enable"), Tooltip("")]
      	public BoolParameter                	enable = new BoolParameter(true);

		[DisplayName("Mode"), Tooltip("")]
		public WindModeParameter				mode = new WindModeParameter();

		[DisplayName("Direction"), Tooltip("")]
		public Vector3Parameter					direction = new Vector3Parameter(Vector3.zero);

		[DisplayName("Strength"), Range(0f, 1f), Tooltip("")]
		public FloatParameter					strength = new FloatParameter(1);

		[DisplayName("Turbluence"), Range(0f, 1f), Tooltip("")]
		public FloatParameter					turbulence = new FloatParameter(1);
      	
		public string GetKeywordName () { return "WIND"; }

		public EffectData[] GetValue(RendererListener listener)
		{
			EffectData directionData = new EffectData("Direction", Vector3.zero, typeof(Vector3));
			switch(mode.value)
			{
				case WindMode.Directional:
					directionData.data = direction.GetValue<Vector3>();
					break;
				case WindMode.Omni:
					directionData.data = Vector3.Normalize(listener.transform.position - direction.GetValue<Vector3>());
					break;
			}

			EffectData strengthData = new EffectData("Strength", enable ? strength.GetValue<float>() : 0, typeof(float));
			EffectData turbulenceData = new EffectData("Turbulence", turbulence.GetValue<float>(), typeof(float));
			return new EffectData[] { directionData, strengthData, turbulenceData };
		}

		public override void ProcessVolumeData(Volume volume)
		{
			switch(mode.value)
			{
				case WindMode.Directional:
					direction.value = volume.transform.forward;
					break;
				case WindMode.Omni:
					direction.value = volume.transform.position;
					break;
			}
		}
    }
}