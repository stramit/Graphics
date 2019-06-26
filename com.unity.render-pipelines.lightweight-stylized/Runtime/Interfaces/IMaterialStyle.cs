using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Experimental.Rendering
{
	public interface IMaterialStyle 
	{
		string GetKeywordName();

		MaterialStyleData[] GetValue();
	}
}
