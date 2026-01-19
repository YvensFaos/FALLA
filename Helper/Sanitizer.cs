/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using Newtonsoft.Json.Linq;

namespace FALLA.Helper
{
    public static class Sanitizer
    {
        public static string ExtractJson(string llmResponse)
        {
            if (string.IsNullOrEmpty(llmResponse))
                return "{}";
        
            llmResponse = llmResponse.Replace("```json", "").Replace("```", "");
        
            var start = llmResponse.IndexOf('{');
            var end = llmResponse.LastIndexOf('}');

            if (start < 0 || end <= start) return "{}";
            var json = llmResponse.Substring(start, end - start + 1);
            json = json.Replace("\n", "").Replace("\r", "");
            try
            {
                JObject.Parse(json);
                return json;
            }
            catch
            {
                json = json.Replace("'", "\"");
                return json;
            }
        }
    }
}