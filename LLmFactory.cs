/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System;
using FALLA.Implementation;

namespace FALLA
{
    public static class LLmFactory
    {
        public static BaseLlm CreateLlm(LlmType type, string apiKey, LlmConfig config)
        {
            return type switch
            {
                LlmType.Gemini => new GeminiLlm(apiKey, config),
                LlmType.Mistral => new MistralLlm(apiKey, config),
                LlmType.DeepSeek => new DeepSeekLlm(apiKey, config),
                LlmType.Claude => new ClaudeLlm(apiKey, config),
                LlmType.GPT => new GptLlm(apiKey, config),
                LlmType.OpenRouter => new OpenRouterLlm(apiKey, config),
                LlmType.Gemma => new GemmaLlm(apiKey, config),
                LlmType.Custom => null, //TODO change
                LlmType.Phi => new PhiLlm(apiKey, config), //TODO change
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
