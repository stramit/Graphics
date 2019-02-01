using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Title("INTERNAL", "Port Test")]
    class PortTestNode : AbstractMaterialNode
    {
        public PortTestNode()
        {
            name = "Port Test";
            UpdateNodeAfterDeserialization();

            AddSlot(new ShaderInputPort(new InputDescriptor(0, "Vector1", SlotValueType.Vector1)));
            AddSlot(new ShaderInputPort(new InputDescriptor(1, "Vector2", SlotValueType.Vector2)));
            AddSlot(new ShaderInputPort(new InputDescriptor(2, "Vector3", SlotValueType.Vector3)));
            AddSlot(new ShaderInputPort(new InputDescriptor(3, "Color3", SlotValueType.Vector3, new ColorControl(Color.white))));
            AddSlot(new ShaderInputPort(new InputDescriptor(4, "Vector4", SlotValueType.Vector4)));
            AddSlot(new ShaderInputPort(new InputDescriptor(5, "Color4", SlotValueType.Vector4, new ColorControl(Color.white))));
            AddSlot(new ShaderInputPort(new InputDescriptor(6, "Boolean", SlotValueType.Boolean)));
            AddSlot(new ShaderInputPort(new InputDescriptor(7, "Texture2D", SlotValueType.Texture2D)));
            AddSlot(new ShaderInputPort(new InputDescriptor(8, "Texture2DArray", SlotValueType.Texture2DArray)));
            AddSlot(new ShaderInputPort(new InputDescriptor(9, "Texture3D", SlotValueType.Texture3D)));
            AddSlot(new ShaderInputPort(new InputDescriptor(10, "Cubemap", SlotValueType.Cubemap)));
            AddSlot(new ShaderInputPort(new InputDescriptor(11, "Sampler", SlotValueType.SamplerState)));
            AddSlot(new ShaderInputPort(new InputDescriptor(12, "Matrix2x2", SlotValueType.Matrix2)));
            AddSlot(new ShaderInputPort(new InputDescriptor(13, "Matrix3x3", SlotValueType.Matrix3)));
            AddSlot(new ShaderInputPort(new InputDescriptor(14, "Matrix4x4", SlotValueType.Matrix4)));
            AddSlot(new ShaderInputPort(new InputDescriptor(15, "Gradient", SlotValueType.Gradient)));
        }
    }
}
