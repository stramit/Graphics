using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering
{
	[Serializable, DebuggerDisplay(k_DebuggerDisplay)]
    public sealed class GradientParameter : VolumeParameter<Gradient>
    {
        public GradientParameter(Gradient value, bool overrideState = false)
            : base(value, overrideState) { }

        public override void Interp(Gradient from, Gradient to, float t)
        {
            if (m_Value == null)
                return;

            List<GradientColorKey> colorKeys = new List<GradientColorKey>();

            foreach(GradientColorKey key in from.colorKeys)
                colorKeys.Add(new GradientColorKey(Color.Lerp(key.color, to.Evaluate(key.time), t), key.time));
            foreach(GradientColorKey key in to.colorKeys)
                colorKeys.Add(new GradientColorKey(Color.Lerp(key.color, from.Evaluate(key.time), 1-t), key.time));

            for(int i = 0; i < colorKeys.Count; i++)
            {
                for(int j = i+1; j < colorKeys.Count; j++)
                {
                    if(colorKeys[i].time == colorKeys[j].time)
                        colorKeys.Remove(colorKeys[j]);
                }
            }

            List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();

            foreach(GradientAlphaKey key in from.alphaKeys)
                alphaKeys.Add(new GradientAlphaKey(Mathf.Lerp(key.alpha, to.Evaluate(key.time).a, t), key.time));
            foreach(GradientAlphaKey key in to.alphaKeys)
                alphaKeys.Add(new GradientAlphaKey(Mathf.Lerp(key.alpha, from.Evaluate(key.time).a, 1-t), key.time));

            for(int i = 0; i < alphaKeys.Count; i++)
            {
                for(int j = i+1; j < alphaKeys.Count; j++)
                {
                    if(alphaKeys[i].time == alphaKeys[j].time)
                        alphaKeys.Remove(alphaKeys[j]);
                }
            }

            m_Value = new Gradient();
            m_Value.mode = (GradientMode)(Mathf.Lerp((int)from.mode, (int)to.mode, t));
            m_Value.colorKeys = colorKeys.ToArray();
            m_Value.alphaKeys = alphaKeys.ToArray();
        }
    }
}
