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

            AddSlot(new ShaderInputPort(new InputDescriptor("Vector1", SlotValueType.Vector1)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Vector2", SlotValueType.Vector2)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Vector3", SlotValueType.Vector3)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Color3", SlotValueType.Vector3, new ColorControl(Color.white))));
            AddSlot(new ShaderInputPort(new InputDescriptor("Vector4", SlotValueType.Vector4)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Color4", SlotValueType.Vector4, new ColorControl(Color.white))));
            AddSlot(new ShaderInputPort(new InputDescriptor("Boolean", SlotValueType.Boolean)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Texture2D", SlotValueType.Texture2D)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Texture2DArray", SlotValueType.Texture2DArray)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Texture3D", SlotValueType.Texture3D)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Cubemap", SlotValueType.Cubemap)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Sampler", SlotValueType.SamplerState)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Matrix2x2", SlotValueType.Matrix2)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Matrix3x3", SlotValueType.Matrix3)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Matrix4x4", SlotValueType.Matrix4)));
            AddSlot(new ShaderInputPort(new InputDescriptor("Gradient", SlotValueType.Gradient)));
        }
    }
}
