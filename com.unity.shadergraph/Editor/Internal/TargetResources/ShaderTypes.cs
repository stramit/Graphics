namespace UnityEditor.ShaderGraph.Internal
{
    public static class ShaderTypes
    {
        [Types]
        public static class Primitives
        {
            public static HlslPrimitiveDescriptor @float = new HlslPrimitiveDescriptor("float", 1);
            public static HlslPrimitiveDescriptor float2 = new HlslPrimitiveDescriptor("float2", 2);
            public static HlslPrimitiveDescriptor float3 = new HlslPrimitiveDescriptor("float3", 3);
            public static HlslPrimitiveDescriptor float4 = new HlslPrimitiveDescriptor("float4", 4);

            public static HlslPrimitiveDescriptor @uint = new HlslPrimitiveDescriptor("uint", 1);
            public static HlslPrimitiveDescriptor uint2 = new HlslPrimitiveDescriptor("uint2", 2);
            public static HlslPrimitiveDescriptor uint3 = new HlslPrimitiveDescriptor("uint3", 3);
            public static HlslPrimitiveDescriptor uint4 = new HlslPrimitiveDescriptor("uint4", 4);
        }

        public static class Structs
        {
            [Types]
            public static class Attributes
            {
                public static HlslStructDescriptor descriptor = new HlslStructDescriptor(
                    "Attributes",
                    new FieldDescriptor[]
                    {
                        StructFields.Attributes.positionOS,
                        StructFields.Attributes.normalOS,
                        StructFields.Attributes.tangentOS,
                        StructFields.Attributes.uv0,
                        StructFields.Attributes.uv1,
                        StructFields.Attributes.uv2,
                        StructFields.Attributes.uv3,
                        StructFields.Attributes.weights,
                        StructFields.Attributes.indicies,
                        StructFields.Attributes.color,
                        StructFields.Attributes.instanceID,
                    });
            }

            [Types]
            public static class Varyings
            {
                public static HlslStructDescriptor descriptor = new HlslStructDescriptor(
                    "Varyings",
                    new FieldDescriptor[]
                    {
                        // ...
                    });
            }

            [Types]
            public static class Output
            {
                public static HlslStructDescriptor descriptor = new HlslStructDescriptor(
                    "Output",
                    new FieldDescriptor[]
                    {
                        // ...
                    });
            }
        }
    }
}
