using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [FormerName("UnityEditor.ShaderGraph.AbstractShaderProperty")]
    class ShaderProperty : ShaderInput
    {
#region Constructors
        public ShaderProperty(PropertyType type)
        {
            displayName = type.ToString();
            propertyType = type;
            inputData = new ShaderInputData(type.ToConcreteShaderValueType());
        }
#endregion

#region Type
        public PropertyType propertyType { get; }
        public override ConcreteSlotValueType concreteShaderValueType => propertyType.ToConcreteShaderValueType();
#endregion

#region Data
        public override void OnDataChanged()
        {
            if(propertyType == PropertyType.SamplerState)
                overrideReferenceName = $"{concreteShaderValueType.ToShaderString()}_{GuidEncoder.Encode(guid)}_{inputData.samplerStateValue.filter}_{inputData.samplerStateValue.wrap}";
        }
#endregion

#region Precision
        [SerializeField]
        private Precision m_Precision = Precision.Inherit;
        
        private ConcretePrecision m_ConcretePrecision = ConcretePrecision.Float;

        public Precision precision
        {
            get => m_Precision;
            set => m_Precision = value;
        }

        public ConcretePrecision concretePrecision => m_ConcretePrecision;

        public void SetConcretePrecision(ConcretePrecision inheritedPrecision)
        {
            m_ConcretePrecision = (precision == Precision.Inherit) ? inheritedPrecision : precision.ToConcrete();
        }
#endregion

#region Capabilities
        public bool isBatchable
        {
            get
            {
                switch(propertyType)
                {
                    case PropertyType.SamplerState:
                    case PropertyType.Texture2D:
                    case PropertyType.Texture2DArray:
                    case PropertyType.Texture3D:
                    case PropertyType.Cubemap:
                    case PropertyType.Gradient:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public bool isExposable
        {
            get
            {
                switch(propertyType)
                {
                    case PropertyType.SamplerState:
                    case PropertyType.Matrix4:
                    case PropertyType.Matrix3:
                    case PropertyType.Matrix2:
                    case PropertyType.Gradient:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public bool isRenamable
        {
            get
            {
                switch(propertyType)
                {
                    case PropertyType.SamplerState:
                        return false;
                    default:
                        return true;
                }
            }
        }
#endregion

#region PropertyBlock
        [SerializeField]
        private bool m_GeneratePropertyBlock = true;

        public bool generatePropertyBlock
        {
            get => m_GeneratePropertyBlock;
            set => m_GeneratePropertyBlock = value;
        }

        [SerializeField]
        bool m_Hidden = false;

        public bool hidden
        {
            get => m_Hidden;
            set => m_Hidden = value;
        }

        public string hideTagString => hidden ? "[HideInInspector]" : "";
        public string modifiableTagString => modifiable ? "" : "[NonModifiableTextureData]";
        public string hdrTagString => colorMode == ColorMode.HDR ? "[HDR]" : "";
        private string enumTagString
        {
            get
            {
                switch(enumType)
                {
                    case EnumType.CSharpEnum:
                        return $"[Enum({m_CSharpEnumType.ToString()})]";
                    case EnumType.KeywordEnum:
                        return $"[KeywordEnum({string.Join(", ", enumNames)})]";
                    default:
                        string enumValuesString = "";
                        for (int i = 0; i < enumNames.Count; i++)
                        {
                            int value = (i < enumValues.Count) ? enumValues[i] : i;
                            enumValuesString += (enumNames[i] + ", " + value + ((i != enumNames.Count - 1) ? ", " : ""));
                        }
                        return $"[Enum({enumValuesString})]";
                }
            }
        }

        public string GetPropertyBlockString()
        {
            switch(propertyType)
            {
                case PropertyType.Texture2D:
                    return $"{hideTagString}{modifiableTagString}[NoScaleOffset] {referenceName}(\"{displayName}\", 2D) = \"{defaultTextureType.ToString().ToLower()}\" {{}}";
                case PropertyType.Texture2DArray:
                    return $"{hideTagString}{modifiableTagString}[NoScaleOffset] {referenceName}(\"{displayName}\", 2DArray) = \"white\" {{}}";
                case PropertyType.Texture3D:
                    return $"{hideTagString}{modifiableTagString}[NoScaleOffset] {referenceName}(\"{displayName}\", 3D) = \"white\" {{}}";
                case PropertyType.Cubemap:
                    return $"{hideTagString}{modifiableTagString}[NoScaleOffset] {referenceName}(\"{displayName}\", CUBE) = \"\" {{}}";
                case PropertyType.Vector4:
                case PropertyType.Vector3:
                case PropertyType.Vector2:
                    return $"{hideTagString}{referenceName}(\"{displayName}\", Vector) = ({NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.y)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.z)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.w)})";
                case PropertyType.Vector1:
                    switch(floatType)
                    {
                        case FloatType.Slider:
                            return $"{hideTagString} {referenceName}(\"{displayName}\", Range({NodeUtils.FloatToShaderValue(m_RangeValues.x)}, {NodeUtils.FloatToShaderValue(m_RangeValues.y)})) = {NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}";
                        case FloatType.Integer:
                            return $"{hideTagString} {referenceName}(\"{displayName}\", Int) = {NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}";
                        case FloatType.Enum:
                            return $"{hideTagString}{enumTagString} {referenceName}(\"{displayName}\", Float) = {NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}";
                        default:
                            return $"{hideTagString} {referenceName}(\"{displayName}\", Float) = {NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}";
                    }
                case PropertyType.Boolean:
                    return $"{hideTagString}[ToggleUI]{referenceName}(\"{displayName}\", Float) = {(inputData.booleanValue == true ? 1 : 0)}";
                case PropertyType.Color:
                    return $"{hideTagString}{hdrTagString} {referenceName}(\"{displayName}\", Color) = ({NodeUtils.FloatToShaderValue(inputData.vectorValue.x)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.y)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.z)}, {NodeUtils.FloatToShaderValue(inputData.vectorValue.w)})";
                default:
                    return string.Empty;
            }
        }
#endregion

#region ShaderValue
        public string GetPropertyDeclarationString(string delimiter = ";")
        {
            switch(propertyType)
            {
                case PropertyType.SamplerState:
                    return $"SAMPLER({referenceName}){delimiter}";
                case PropertyType.Texture2D:
                    return $"TEXTURE2D({referenceName}){delimiter} SAMPLER(sampler{referenceName}); {concretePrecision.ToShaderString()}4 {referenceName}_TexelSize{delimiter}";
                case PropertyType.Texture2DArray:
                    return $"TEXTURE2D_ARRAY({referenceName}){delimiter} SAMPLER(sampler{referenceName}){delimiter}";
                case PropertyType.Texture3D:
                    return $"TEXTURE3D({referenceName}){delimiter} SAMPLER(sampler{referenceName}){delimiter}";
                case PropertyType.Cubemap:
                    return $"TEXTURECUBE({referenceName}){delimiter} SAMPLER(sampler{referenceName}){delimiter}";
                case PropertyType.Gradient:
                    return GradientUtil.GetGradientPropertyDeclaration(referenceName, inputData.gradientValue, concretePrecision);
                case PropertyType.Matrix4:
                case PropertyType.Matrix3:
                case PropertyType.Matrix2:
                    return $"{concretePrecision.ToShaderString()}4x4 {referenceName}{delimiter}";
                default:
                    return $"{concreteShaderValueType.ToShaderString(concretePrecision.ToShaderString())} {referenceName}{delimiter}";
            }
        }

        public string GetPropertyAsArgumentString()
        {
            switch(propertyType)
            {
                case PropertyType.SamplerState:
                    return $"SamplerState {referenceName}";
                case PropertyType.Texture2D:
                    return $"TEXTURE2D_PARAM({referenceName}, sampler{referenceName})";
                case PropertyType.Texture2DArray:
                    return $"TEXTURE2D_ARRAY_PARAM({referenceName}, sampler{referenceName})";
                case PropertyType.Texture3D:
                    return $"TEXTURE3D_PARAM({referenceName}, sampler{referenceName})";
                case PropertyType.Cubemap:
                    return $"TEXTURECUBE_PARAM({referenceName}, sampler{referenceName})";
                case PropertyType.Gradient:
                    return "Gradient " + referenceName;
                default:
                    return GetPropertyDeclarationString(string.Empty);
            }
        }
#endregion

#region Options
        [SerializeField]
        private bool m_Modifiable = true;

        public bool modifiable
        {
            get => m_Modifiable;
            set => m_Modifiable = value;
        }
        
        [SerializeField]
        private ColorMode m_ColorMode;

        public ColorMode colorMode
        {
            get => m_ColorMode;
            set => m_ColorMode = value;
        }

        [SerializeField]
        private DefaultTextureType m_DefaultTextureType = DefaultTextureType.White;

        public DefaultTextureType defaultTextureType
        {
            get { return m_DefaultTextureType; }
            set { m_DefaultTextureType = value; }
        }

        [SerializeField]
        private FloatType m_FloatType = FloatType.Default;

        public FloatType floatType
        {
            get => m_FloatType;
            set => m_FloatType = value;
        }

        [SerializeField]
        private Vector2 m_RangeValues = new Vector2(0, 1);

        public Vector2 rangeValues
        {
            get => m_RangeValues;
            set => m_RangeValues = value;
        }

        private EnumType m_EnumType = EnumType.Enum;

        public EnumType enumType
        {
            get => m_EnumType;
            set => m_EnumType = value;
        }
    
        private Type m_CSharpEnumType;

        public Type cSharpEnumType
        {
            get => m_CSharpEnumType;
            set => m_CSharpEnumType = value;
        }

        private List<string> m_EnumNames = new List<string>();
        private List<int> m_EnumValues = new List<int>();

        public List<string> enumNames
        {
            get => m_EnumNames;
            set => m_EnumNames = value;
        }

        public List<int> enumValues
        {
            get => m_EnumValues;
            set => m_EnumValues = value;
        }
#endregion

#region Utility
        public override AbstractMaterialNode ToConcreteNode()
        {
            switch(propertyType)
            {
                case PropertyType.SamplerState:
                    return new SamplerStateNode() 
                    {
                        filter = inputData.samplerStateValue.filter,
                        wrap = inputData.samplerStateValue.wrap
                    };
                case PropertyType.Matrix4:
                    return new Matrix4Node
                    {
                        row0 = new Vector4(inputData.matrixValue.m00, inputData.matrixValue.m01, inputData.matrixValue.m02, inputData.matrixValue.m03),
                        row1 = new Vector4(inputData.matrixValue.m10, inputData.matrixValue.m11, inputData.matrixValue.m12, inputData.matrixValue.m13),
                        row2 = new Vector4(inputData.matrixValue.m20, inputData.matrixValue.m21, inputData.matrixValue.m22, inputData.matrixValue.m23),
                        row3 = new Vector4(inputData.matrixValue.m30, inputData.matrixValue.m31, inputData.matrixValue.m32, inputData.matrixValue.m33)
                    };
                case PropertyType.Matrix3:
                    return new Matrix3Node
                    {
                        row0 = new Vector3(inputData.matrixValue.m00, inputData.matrixValue.m01, inputData.matrixValue.m02),
                        row1 = new Vector3(inputData.matrixValue.m10, inputData.matrixValue.m11, inputData.matrixValue.m12),
                        row2 = new Vector3(inputData.matrixValue.m20, inputData.matrixValue.m21, inputData.matrixValue.m22)
                    };
                case PropertyType.Matrix2:
                    return new Matrix2Node
                    {
                        row0 = new Vector2(inputData.matrixValue.m00, inputData.matrixValue.m01),
                        row1 = new Vector2(inputData.matrixValue.m10, inputData.matrixValue.m11)
                    };
                case PropertyType.Texture2D:
                    return new Texture2DAssetNode { texture = inputData.textureValue.texture };
                case PropertyType.Texture2DArray:
                    return new Texture2DArrayAssetNode { texture = inputData.textureArrayValue.textureArray };
                case PropertyType.Texture3D:
                    return new Texture3DAssetNode { texture = (Texture3D)value.texture };
                case PropertyType.Cubemap:
                    return new CubemapAssetNode { cubemap = inputData.cubemapValue.cubemap };
                case PropertyType.Gradient:
                    return new GradientNode { gradient = inputData.gradientValue };
                case PropertyType.Vector4:
                    var v4node = new Vector4Node();
                    v4node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotXId).value = inputData.vectorValue.x;
                    v4node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotYId).value = inputData.vectorValue.y;
                    v4node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotZId).value = inputData.vectorValue.z;
                    v4node.FindInputSlot<Vector1MaterialSlot>(Vector4Node.InputSlotWId).value = inputData.vectorValue.w;
                    return node;
                case PropertyType.Vector3:
                    var v3node = new Vector3Node();
                    v3node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotXId).value = inputData.vectorValue.x;
                    v3node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotYId).value = inputData.vectorValue.y;
                    v3node.FindInputSlot<Vector1MaterialSlot>(Vector3Node.InputSlotZId).value = inputData.vectorValue.z;
                    return node;
                case PropertyType.Vector2:
                    var v2node = new Vector2Node();
                    v2node.FindInputSlot<Vector1MaterialSlot>(Vector2Node.InputSlotXId).value = inputData.vectorValue.x;
                    v2node.FindInputSlot<Vector1MaterialSlot>(Vector2Node.InputSlotYId).value = inputData.vectorValue.y;
                    return node;
                case PropertyType.Vector1:
                    switch (m_FloatType)
                    {
                        case FloatType.Slider:
                            return new SliderNode { value = new Vector3(value, m_RangeValues.x, m_RangeValues.y) };
                        case FloatType.Integer:
                            return new IntegerNode { value = (int)value };
                        default:
                            var node = new Vector1Node();
                            node.FindInputSlot<Vector1MaterialSlot>(Vector1Node.InputSlotXId).value = value;
                            return node;
                    }
                case PropertyType.Boolean:
                    return new BooleanNode { value = new ToggleData(inputData.booleanValue) };
                case PropertyType.Color:
                    return new ColorNode { color = new ColorNode.Color(inputData.vectorValue, colorMode) };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty(propertyType)
            {
                name = referenceName,
                value = inputData
            };
        }

        public ShaderProperty Copy()
        {
            var copied = new ShaderProperty(propertyType);
            copied.displayName = displayName;
            copied.overrideReferenceName = overrideReferenceName;
            copied.inputData = inputData;
            copied.hidden = hidden;
            copied.modifiable = modifiable;
            copied.colorMode = colorMode;
            copied.floatType = floatType;
            copied.rangeValues = rangeValues;
            copied.enumType = enumType;
            copied.enumNames = enumNames;
            copied.enumValues = enumValues;
            return copied;
        }
#endregion
    }
}
