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
    public class NoCandidateException : System.Exception
    {
        private UnityWebRequest _request;
        private string _responseText;

        public NoCandidateException(UnityWebRequest request, string responseText)
        {
            _request = request;
            _responseText = responseText;
        }
    }
}