using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.UIElements.GraphView;

namespace UnityEditor.ShaderGraph
{
    public struct HlslSource
    {
        internal HlslSourceType type { get; private set; }
        internal string value { get; private set; }

        public static HlslSource File(string source)
        {
            if (!System.IO.File.Exists(Path.GetFullPath(source)))
            {
                throw new ArgumentException($"Cannot open file at \"{source}\"");
            }
            
            return new HlslSource
            {
                type = HlslSourceType.File,
                value = source
            };
        }

        public static HlslSource String(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException($"String \"{source}\" is null or empty");
            }
            
            return new HlslSource
            {
                type = HlslSourceType.String,
                value = source
            };
        }
    }

    public enum HlslSourceType
    {
        File,
        String
    }
}
