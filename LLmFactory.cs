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
        public static BaseLlm CreateLlm(LlmType type, string apiKey)
        {
            return type switch
            {
                LlmType.Gemini => new GeminiLlm(apiKey),
                LlmType.Mistral => new MistralLlm(apiKey),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}