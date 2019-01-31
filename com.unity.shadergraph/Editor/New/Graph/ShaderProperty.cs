using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls; // TODO - Remove legacy controls

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderProperty : IShaderValue
    {
        public ShaderProperty()
        {
        }

        public ShaderProperty(PropertyType type, string name)
        {
            m_PropertyType = type;
            m_DisplayName = type.ToString();
            m_ShaderOutputName = string.IsNullOrEmpty(name) ? string.Format("{0}_{1}", type.ToString(), GuidEncoder.Encode(guid)) : name;
            m_ShaderValueData = new ShaderValueData();
        }

        [SerializeField]
        int m_Id;

        [SerializeField]
        private SerializableGuid m_Guid = new SerializableGuid();

        [SerializeField]
        string m_DisplayName = "Not Initilaized";

        [SerializeField]
        string m_ShaderOutputName;

        [SerializeField]
        private PropertyType m_PropertyType;

        [SerializeField]
        private ConcreteSlotValueType m_ConcreteSlotValueType = ConcreteSlotValueType.Vector1;

        [SerializeField]
        private ShaderValueData m_ShaderValueData;

        [SerializeField]
        private bool m_GeneratePropertyBlock = true;

        public INode owner { get; set; } // TODO - Owner cant be node for all values, in this case its a graph!
        public int id => m_Id;
        public Guid guid => m_Guid.guid;
        public string shaderOutputName => m_ShaderOutputName;
        public ConcreteSlotValueType concreteValueType => m_ConcreteSlotValueType;
        public ShaderValueData value => m_ShaderValueData;
        public bool generatePropertyBlock
        {
            get => m_GeneratePropertyBlock;
            set => m_GeneratePropertyBlock = value;
        }

        [SerializeField]
        string m_DefaultReferenceName;

        private bool m_OverrideMembers = false;
        private string m_OverrideSlotName;

        public string referenceName
        {
            get
            {
                if (string.IsNullOrEmpty(overrideReferenceName))
                {
                    if (string.IsNullOrEmpty(m_DefaultReferenceName))
                        m_DefaultReferenceName = string.Format("{0}_{1}", propertyType, GuidEncoder.Encode(guid));
                    return m_DefaultReferenceName;
                }
                return overrideReferenceName;
            }
        }

        [SerializeField]
        string m_OverrideReferenceName;

        public string overrideReferenceName
        {
            get { return m_OverrideReferenceName; }
            set { m_OverrideReferenceName = value; }
        }

        public string displayName
        {
            get => m_DisplayName;
            set => m_DisplayName = value;
        }

        public void OverrideMembers(string slotName)
        {
            m_OverrideMembers = true;
            m_OverrideSlotName = slotName;
        }

        public PropertyType propertyType => m_PropertyType;

        public void UpdateValue(ShaderValueData value)
        {
            // TODO what to call when value changes from UI?
            if(!m_ShaderValueData.Equals(value))
            {
                m_ShaderValueData = value;
                //owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                //owner.Dirty(ModificationScope.Node);
            }
        }

        public virtual string GetPropertyAsArgumentString()
        {
            switch(propertyType)
            {
                case PropertyType.Cubemap:
                    return string.Format("TEXTURECUBE_ARGS({0}, sampler{0})", referenceName);
                case PropertyType.Texture2DArray:
                    return string.Format("TEXTURE2D_ARRAY_ARGS({0}, sampler{0})", referenceName);
                case PropertyType.Texture2D:
                    return string.Format("TEXTURE2D_ARGS({0}, sampler{0})", referenceName);
                default:
                    return GetPropertyDeclarationString(string.Empty);
            }
        }

        public bool isBatchable 
        { 
            get
            {
                switch(propertyType)
                {
                    case PropertyType.Boolean:
                    case PropertyType.Color:
                    case PropertyType.Matrix2:
                    case PropertyType.Matrix3:
                    case PropertyType.Matrix4:
                    case PropertyType.Vector1:
                    case PropertyType.Vector2:
                    case PropertyType.Vector3:
                    case PropertyType.Vector4:
                        return true;
                    case PropertyType.Cubemap:
                    case PropertyType.Gradient:
                    case PropertyType.SamplerState:
                    case PropertyType.Texture2DArray:
                    case PropertyType.Texture2D:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } 
        }

        public string GetPropertyBlockString()
        {
             if (!generatePropertyBlock)
                return string.Empty;

            switch(propertyType)
            {
                case PropertyType.Boolean:
                    return string.Format("[ToggleUI] {0}(\"{1}\", Float) = {2}", referenceName, displayName, value.booleanValue == true ? 1 : 0);
                case PropertyType.Color:
                    return string.Format("{0}(\"{1}\", Color) = ({2},{3},{4},{5})", referenceName, displayName, value.vectorValue.x, value.vectorValue.y, value.vectorValue.z, value.vectorValue.w);
                case PropertyType.Cubemap:
                    return string.Format("[NoScaleOffset] {0}(\"{1}\", CUBE) = \"\" {}", referenceName, displayName);
                case PropertyType.Gradient:
                case PropertyType.Matrix2:
                case PropertyType.Matrix3:
                case PropertyType.Matrix4:
                case PropertyType.SamplerState:
                    return string.Empty;
                case PropertyType.Texture2DArray:
                    return string.Format("[NoScaleOffset] {0}(\"{1}\", 2DArray) = \"white\" {}", referenceName, displayName);
                case PropertyType.Texture2D:
                    return string.Format("[NoScaleOffset] {0}(\"{1}\", 2D) = \"white\" {}", referenceName, displayName); // TODO - Default value white/grey/black/bump
                case PropertyType.Vector1:
                    return string.Format("{0}(\"{1}\", Float) = {2}", referenceName, displayName, NodeUtils.FloatToShaderValue(value.vectorValue.x)); // TODO - Sliders/Ints/Enum/etc
                case PropertyType.Vector2:
                case PropertyType.Vector3:
                case PropertyType.Vector4:
                    return string.Format("{0}(\"{1}\", Vector) = ({2},{3},{4},{5})", referenceName, displayName, 
                        NodeUtils.FloatToShaderValue(value.vectorValue.x), NodeUtils.FloatToShaderValue(value.vectorValue.y), NodeUtils.FloatToShaderValue(value.vectorValue.z), NodeUtils.FloatToShaderValue(value.vectorValue.w));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetPropertyDeclarationString(string delimiter = ";")
        {
            switch(propertyType)
            {
                case PropertyType.Boolean:
                case PropertyType.Color:
                    return string.Format("float {0}{1}", referenceName, delimiter);
                case PropertyType.Cubemap:
                    return string.Format("TEXTURECUBE({0}){1} SAMPLER(sampler{0}){1}", referenceName, delimiter);
                case PropertyType.Gradient:
                    return GetGradientDeclarationString();
                case PropertyType.Matrix2:
                case PropertyType.Matrix3:
                case PropertyType.Matrix4:
                    return string.Format("float4x4 {0} = float4x4(1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0){1}", referenceName, delimiter);
                case PropertyType.SamplerState:
                    return string.Format(@"SAMPLER({0}){1}", referenceName, delimiter);
                case PropertyType.Texture2DArray:
                    return string.Format("TEXTURE2D_ARRAY({0}){1} SAMPLER(sampler{0}){1}", referenceName, delimiter);
                case PropertyType.Texture2D:
                    return string.Format("TEXTURE2D({0}){1} SAMPLER(sampler{0}); float4 {0}_TexelSize{1}", referenceName, delimiter);
                case PropertyType.Vector1:
                    return string.Format("float {0}{1}", referenceName, delimiter);
                case PropertyType.Vector2:
                    return string.Format("float2 {0}{1}", referenceName, delimiter);
                case PropertyType.Vector3:
                    return string.Format("float3 {0}{1}", referenceName, delimiter);
                case PropertyType.Vector4:
                    return string.Format("float4 {0}{1}", referenceName, delimiter);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetGradientDeclarationString()
        {
            if (m_OverrideMembers)
            {
                ShaderStringBuilder s = new ShaderStringBuilder();
                s.AppendLine("Gradient Unity{0} ()",
                    referenceName);
                using (s.BlockScope())
                {
                    s.AppendLine("Gradient g;");
                    s.AppendLine("g.type = {0}_Type;", m_OverrideSlotName);
                    s.AppendLine("g.colorsLength = {0}_ColorsLength;", m_OverrideSlotName);
                    s.AppendLine("g.alphasLength = {0}_AlphasLength;", m_OverrideSlotName);
                    for (int i = 0; i < 8; i++)
                    {
                        s.AppendLine("g.colors[{0}] = {1}_ColorKey{0};", i, m_OverrideSlotName);
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        s.AppendLine("g.alphas[{0}] = {1}_AlphaKey{0};", i, m_OverrideSlotName);
                    }
                    s.AppendLine("return g;", true);
                }
                return s.ToString();
            }
            else
            {
                ShaderStringBuilder s = new ShaderStringBuilder();
                s.AppendLine("Gradient Unity{0} ()", referenceName);
                using (s.BlockScope())
                {
                    GradientUtils.GetGradientDeclaration(value.gradientValue, ref s);
                    s.AppendLine("return g;", true);
                }
                return s.ToString();
            }
        }

        public PreviewProperty GetPreviewMaterialProperty()
        {
            switch(propertyType)
            {
                case PropertyType.Boolean:
                    return new PreviewProperty(PropertyType.Boolean)
                    {
                        name = referenceName,
                        booleanValue = value.booleanValue
                    };
                case PropertyType.Color:
                    return new PreviewProperty(PropertyType.Color)
                    {
                        name = referenceName,
                        colorValue = value.vectorValue
                    };
                case PropertyType.Cubemap:
                    return new PreviewProperty(PropertyType.Cubemap)
                    {
                        name = referenceName,
                        cubemapValue = (Cubemap)value.textureValue
                    };
                case PropertyType.Gradient:
                    return new PreviewProperty(PropertyType.Gradient)
                    {
                        name = referenceName,
                        gradientValue = value.gradientValue
                    };
                case PropertyType.Texture2DArray:
                    return new PreviewProperty(PropertyType.Texture2D)
                    {
                        name = referenceName,
                        textureValue = (Texture2DArray)value.textureValue
                    };
                case PropertyType.Texture2D:
                    return new PreviewProperty(PropertyType.Texture2D)
                    {
                        name = referenceName,
                        textureValue = (Texture2D)value.textureValue
                    };
                case PropertyType.Vector1:
                    return new PreviewProperty(PropertyType.Vector1)
                    {
                        name = referenceName,
                        floatValue = value.vectorValue.x
                    };
                case PropertyType.Vector2:
                    return new PreviewProperty(PropertyType.Vector2)
                    {
                        name = referenceName,
                        vector4Value = value.vectorValue
                    };
                case PropertyType.Vector3:
                    return new PreviewProperty(PropertyType.Vector3)
                    {
                        name = referenceName,
                        vector4Value = value.vectorValue
                    };
                case PropertyType.Vector4:
                    return new PreviewProperty(PropertyType.Vector4)
                    {
                        name = referenceName,
                        vector4Value = value.vectorValue
                    };
                default:
                    return default(PreviewProperty);
            }
        }

        public INode ToConcreteNode()
        {
            switch(propertyType)
            {
                case PropertyType.Boolean:
                    return new BooleanNode { value = new ToggleData(value.booleanValue) };
                case PropertyType.Color:
                    return new ColorNode { color = new ColorNode.Color(value.vectorValue, 0) }; // TODO - Handle ColorMode
                case PropertyType.Cubemap:
                    return new CubemapAssetNode { cubemap = (Cubemap)value.textureValue };
                case PropertyType.Gradient:
                    return new GradientNode { gradient = value.gradientValue };
                case PropertyType.Matrix2:
                    return new Matrix2Node
                    {
                        row0 = new Vector2(value.matrixValue.m00, value.matrixValue.m01),
                        row1 = new Vector2(value.matrixValue.m10, value.matrixValue.m11)
                    };
                case PropertyType.Matrix3:
                    return new Matrix3Node
                    {
                        row0 = new Vector3(value.matrixValue.m00, value.matrixValue.m01, value.matrixValue.m02),
                        row1 = new Vector3(value.matrixValue.m10, value.matrixValue.m11, value.matrixValue.m12),
                        row2 = new Vector3(value.matrixValue.m20, value.matrixValue.m21, value.matrixValue.m22)
                    };
                case PropertyType.Matrix4:
                    return new Matrix4Node
                    {
                        row0 = new Vector4(value.matrixValue.m00, value.matrixValue.m01, value.matrixValue.m02, value.matrixValue.m03),
                        row1 = new Vector4(value.matrixValue.m10, value.matrixValue.m11, value.matrixValue.m12, value.matrixValue.m13),
                        row2 = new Vector4(value.matrixValue.m20, value.matrixValue.m21, value.matrixValue.m22, value.matrixValue.m23),
                        row3 = new Vector4(value.matrixValue.m30, value.matrixValue.m31, value.matrixValue.m32, value.matrixValue.m33)
                    };
                case PropertyType.SamplerState:
                    return new SamplerStateNode();
                case PropertyType.Texture2DArray:
                    return new Texture2DAssetNode { texture = (Texture2DArray)value.textureValue };
                case PropertyType.Texture2D:
                    return new Texture2DAssetNode { texture = value.textureValue };
                case PropertyType.Vector1: // TODO - Add Slider/Int/Enum/etc here also
                    var vector1Node = new Vector1Node();
                    vector1Node.FindInputSlot<Vector1MaterialSlot>(Vector1Node.InputSlotXId).value = value.vectorValue.x;
                    return vector1Node;
                case PropertyType.Vector2:
                    var vector2Node = new Vector2Node();
                    vector2Node.FindInputSlot<Vector1MaterialSlot>(Vector2Node.InputSlotXId).value = value.vectorValue.x;
                    vector2Node.FindInputSlot<Vector1MaterialSlot>(Vector2Node.InputSlotYId).value = value.vectorValue.y;
                    return vector2Node;
                case PropertyType.Vector3:
                    var vector3Node = new Vector3Node();
                    vector3Node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotXId).value = value.vectorValue.x;
                    vector3Node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotYId).value = value.vectorValue.y;
                    vector3Node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotZId).value = value.vectorValue.z;
                    return vector3Node;
                case PropertyType.Vector4:
                    var vector4Node = new Vector4Node();
                    vector4Node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotXId).value = value.vectorValue.x;
                    vector4Node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotYId).value = value.vectorValue.y;
                    vector4Node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotZId).value = value.vectorValue.z;
                    vector4Node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotWId).value = value.vectorValue.w;
                    return vector4Node;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ShaderProperty Copy()
        {
            var copied = new ShaderProperty(propertyType);
            copied.displayName = displayName;
            copied.m_ShaderValueData = value;
            return copied;
        }

        // ------------------------------------------------------------------------------------------------------------


    }
}
