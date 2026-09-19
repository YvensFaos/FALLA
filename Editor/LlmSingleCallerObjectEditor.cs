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
            if (GUILayout.Button("Preload Config"))
            {
                var type = llmSingleCallerObject.GetLlmType();
                var config = type switch
                {
                    LlmType.Gemini => GeminiLlm.GetDefaultConfig(),
                    LlmType.Mistral => MistralLlm.GetDefaultConfig(),
                    LlmType.DeepSeek => DeepSeekLlm.GetDefaultConfig(),
                    LlmType.Claude => ClaudeLlm.GetDefaultConfig(),
                    LlmType.GPT => GptLlm.GetDefaultConfig(),
                    LlmType.Gemma => GemmaLlm.GetDefaultConfig(),
                    LlmType.OpenRouter => OpenRouterLlm.GetDefaultConfig(),
                    LlmType.Phi => PhiLlm.GetDefaultConfig(),
                    LlmType.Custom => new LlmConfig("","","",""),
                    _ => throw new ArgumentOutOfRangeException()
                };
                llmSingleCallerObject.SetConfig(config);
            }
        }
    }
}