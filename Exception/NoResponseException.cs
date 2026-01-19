/*
 * Copyright (c) 2026 Yvens R Serpa [https://github.com/YvensFaos/]
 * 
 * This work is licensed under the Creative Commons Attribution 4.0 International License.
 * To view a copy of this license, visit http://creativecommons.org/licenses/by/4.0/
 * or see the LICENSE file in the root directory of this repository.
 */

using UnityEngine.Networking;

namespace FALLA.Exception
{
    public class NoResponseException : System.Exception
    {
        private UnityWebRequest _request;
        private readonly string _error;
        private readonly string _downloadHandlerText;

        public NoResponseException(UnityWebRequest request, string error, string downloadHandlerText)
        {
            _request = request;
            _error = error;
            _downloadHandlerText = downloadHandlerText;
        }

        public override string Message => $"Error: {_error}. Download Handler Text: {_downloadHandlerText}.";
    }
}