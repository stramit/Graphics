using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEngine.Experimental.Rendering
{
	[CustomEditor(typeof(StylizationLayer))]
	public class StylizationLayerEditor : Editor 
	{
		internal class Styles
        {
            public static GUIContent layerLabel = new GUIContent("Layer");
        }

		SerializedProperty m_VolumeLayer;

		void OnEnable()
        {
            m_VolumeLayer = serializedObject.FindProperty("m_VolumeLayer");
		}

		public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_VolumeLayer, Styles.layerLabel);

            serializedObject.ApplyModifiedProperties();
        }
	}
}
