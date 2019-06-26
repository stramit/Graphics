using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Rendering.ToonPipeline
{
	public static class TextureUtils 
	{
		public static Texture2D BakeGradientsToTexture2d(Gradient[] gradients, int width, 
					TextureFormat textureFormat = TextureFormat.RGBA32, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
		{
			Texture2D texture = new Texture2D(width, gradients.Length, textureFormat, false, false);
			texture.wrapMode = wrapMode;
			Color[] cols = new Color[width * gradients.Length];
			for(int h = 0; h < gradients.Length; h++)
			{
				for (int w = 0; w < width; w++)
				{
					cols[w + (width * h)] = gradients[h].Evaluate((float)w / width);
				}
			}
			texture.SetPixels(cols);
			texture.Apply();
			return texture;
		}
	}
}