using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace MacacaGames.EffectSystem.Editor
{
    [CustomEditor(typeof(EffectGroup))]
    public class EffectGroupEditor : UnityEditor.Editor
    {
        SerializedProperty effectsProperty;

        void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty("effects");
        }

        string json;
        public override void OnInspectorGUI()
        {
            var effectGroup = serializedObject.targetObject as EffectGroup;
            DrawDefaultInspector();

            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("ExportEffectToJson"))
                {
                    effectGroup.ExportEffectToJson();
                }
            }
            using (var horizon = new EditorGUILayout.HorizontalScope())
            {
                json = EditorGUILayout.TextField("FromJson", json);
                if (GUILayout.Button("FromJson"))
                {
                    effectGroup.FromJson(json);
                    EditorUtility.SetDirty(effectGroup);
                }
            }
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}