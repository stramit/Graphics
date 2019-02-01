using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public struct HlslSource
    {
        [SerializeField]
        private HlslSourceType m_Type;

        [SerializeField]
        private string m_Value;

        internal HlslSourceType type 
        { 
            get => m_Type; 
            private set => m_Type = value; 
        }

        internal string value 
        { 
            get => m_Value; 
            private set => m_Value = value; 
        }

        public static HlslSource File(string source, bool suppressWarnings = false)
        {
            if (!System.IO.File.Exists(Path.GetFullPath(source)) && !suppressWarnings)
            {
                throw new ArgumentException($"Cannot open file at \"{source}\"");
            }

            return new HlslSource
            {
                type = HlslSourceType.File,
                value = source
            };
        }

        public static HlslSource String(string source, bool suppressWarnings = false)
        {
            if (string.IsNullOrEmpty(source) && !suppressWarnings)
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
