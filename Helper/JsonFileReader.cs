/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FALLA.Helper
{
    public static class JsonFileReader
    {
        /// <summary>
        /// Expect JSON files in the format: [{ "gemini" : "GEMINI_API" }, { "mistral" : "MISTRAL_API }]
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadValuePairJson(string path)
        {
            var jsonText = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonText);
            return data;
        }

        /// <summary>
        /// Expect JSON files in the format: [{ "gemini" : "GEMINI_API" }, { "mistral" : "MISTRAL_API" }]
        /// The key must match the name of the JSON key, such as "gemini" and "mistral" in the example.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromValuePairJson(string path, string key)
        {
            var data = ReadValuePairJson((path));
            var keyValue = data.Find(entry => entry.ContainsKey(key))[key];
            return keyValue;
        }
    }
}