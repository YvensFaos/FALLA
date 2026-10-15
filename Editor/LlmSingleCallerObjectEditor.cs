using System;
using FALLA.Implementation;
using UnityEditor;
using UnityEngine;

namespace FALLA.Editor
{
    [CustomEditor(typeof(LlmSingleCallerObject))]
    public class LlmSingleCallerObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var llmSingleCallerObject = (LlmSingleCallerObject)target;

            EditorGUILayout.Space(10);
            // ReSharper disable once InvertIf
            if (GUILayout.Button("Preload Config"))
            {
                var type = llmSingleCallerObject.GetLlmType();
                var config = LLmFactory.LoadDefaultConfig(type);
                llmSingleCallerObject.SetConfig(config);
            }
        }
    }
}