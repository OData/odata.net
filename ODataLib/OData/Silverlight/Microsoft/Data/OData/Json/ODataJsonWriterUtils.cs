//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JSON format.
    /// </summary>
    internal static class ODataJsonWriterUtils
    {
        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        internal static void WriteError(JsonWriter jsonWriter, ODataError error, bool includeDebugInformation, int maxInnerErrorDepth)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            string code, message, messageLanguage;
            ErrorUtils.GetErrorDetails(error, out code, out message, out messageLanguage);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;
            ODataJsonWriterUtils.WriteError(jsonWriter, code, message, messageLanguage, innerError, maxInnerErrorDepth);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="messageLanguage">The language of the message.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        private static void WriteError(JsonWriter jsonWriter, string code, string message, string messageLanguage, ODataInnerError innerError, int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageLanguage != null, "messageLanguage != null");

            // { "error": {
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName(JsonConstants.ODataErrorName);
            jsonWriter.StartObjectScope();

            // "code": "<code>"
            jsonWriter.WriteName(JsonConstants.ODataErrorCodeName);
            jsonWriter.WriteValue(code);

            // "message": {
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageName);
            jsonWriter.StartObjectScope();

            // "lang": "<messageLanguage>"
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageLanguageName);
            jsonWriter.WriteValue(messageLanguage);

            // "value": "<message>"
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageValueName);
            jsonWriter.WriteValue(message);

            // }
            jsonWriter.EndObjectScope();

            if (innerError != null)
            {
                ODataJsonWriterUtils.WriteInnerError(jsonWriter, innerError, JsonConstants.ODataErrorInnerErrorName, /* recursionDepth */ 0, maxInnerErrorDepth);
            }

            // } }
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Write an inner error property and message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="innerError">Inner error details.</param>
        /// <param name="innerErrorPropertyName">The property name for the inner error property.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        private static void WriteInnerError(JsonWriter jsonWriter, ODataInnerError innerError, string innerErrorPropertyName, int recursionDepth, int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);

            // "innererror": {
            jsonWriter.WriteName(innerErrorPropertyName);
            jsonWriter.StartObjectScope();

            //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
            ////       to stay compatible with Astoria.

            // "message": "<message>"
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorMessageName);
            jsonWriter.WriteValue(innerError.Message ?? string.Empty);

            // "type": "<typename">
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorTypeNameName);
            jsonWriter.WriteValue(innerError.TypeName ?? string.Empty);

            // "stacktrace": "<stacktrace>"
            jsonWriter.WriteName(JsonConstants.ODataErrorInnerErrorStackTraceName);
            jsonWriter.WriteValue(innerError.StackTrace ?? string.Empty);

            if (innerError.InnerError != null)
            {
                // "internalexception": { <nested inner error> }
                ODataJsonWriterUtils.WriteInnerError(jsonWriter, innerError.InnerError, JsonConstants.ODataErrorInnerErrorInnerErrorName, recursionDepth, maxInnerErrorDepth);
            }

            // }
            jsonWriter.EndObjectScope();
        }
    }
}
