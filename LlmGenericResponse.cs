/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 *
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

namespace FALLA
{
    public struct LlmGenericResponse
    {
        public readonly string response;
        public readonly bool success;

        public LlmGenericResponse(string response, bool success)
        {
            this.response = response;
            this.success = success;
        }

        public static LlmGenericResponse EmptyResponse() => new LlmGenericResponse("", false);
    }
}