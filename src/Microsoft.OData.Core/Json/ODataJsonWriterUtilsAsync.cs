//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JSON format.
    /// </summary>
    internal static partial class ODataJsonWriterUtils
    {
        /// <summary>
        /// Asynchronously writes an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Delegate to write the instance annotations.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static async Task WriteErrorAsync(IJsonWriterAsync jsonWriter,
            Func<IEnumerable<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate, ODataError error,
            bool includeDebugInformation, int maxInnerErrorDepth, bool writingJsonLight)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            string code, message;
            ErrorUtils.GetErrorDetails(error, out code, out message);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;

            await WriteErrorAsync(
                jsonWriter,
                code,
                message,
                error.Target,
                error.Details,
                innerError,
                error.GetInstanceAnnotations(),
                writeInstanceAnnotationsDelegate,
                maxInnerErrorDepth,
                writingJsonLight).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the function's name and start the JSONP scope if we are writing a response and the
        /// JSONP function name is not null or empty.
        /// </summary>
        /// <param name="jsonWriter">JsonWriter to write to.</param>
        /// <param name="settings">Writer settings.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal static async Task StartJsonPaddingIfRequiredAsync(IJsonWriterAsync jsonWriter, ODataMessageWriterSettings settings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                await jsonWriter.WritePaddingFunctionNameAsync(settings.JsonPCallback).ConfigureAwait(false);
                await jsonWriter.StartPaddingFunctionScopeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// If we are writing a response and the given Json Padding function name is not null or empty
        /// this function will close the JSONP scope asynchronously.
        /// </summary>
        /// <param name="jsonWriter">JsonWriter to write to.</param>
        /// <param name="settings">Writer settings.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal static async Task EndJsonPaddingIfRequiredAsync(IJsonWriterAsync jsonWriter, ODataMessageWriterSettings settings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                await jsonWriter.EndPaddingFunctionScopeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously writes an error message.
        /// </summary>
        /// <param name="jsonWriter">JSON writer.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="target">The target of the error.</param>
        /// <param name="details">The details of the error.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="instanceAnnotations">Instance annotations for this error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Delegate to write the instance annotations.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteErrorAsync(IJsonWriterAsync jsonWriter, string code, string message, string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
            Func<IEnumerable<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate, int maxInnerErrorDepth,
            bool writingJsonLight)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");

            // "error": {
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            if (writingJsonLight)
            {
                await jsonWriter.WriteNameAsync(JsonLightConstants.ODataErrorPropertyName).ConfigureAwait(false);
            }
            else
            {
                await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorName).ConfigureAwait(false);
            }

            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

            // "code": "<code>"
            await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorCodeName).ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(code).ConfigureAwait(false);

            // "message": "<message string>"
            await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorMessageName).ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(message).ConfigureAwait(false);

            // For example, "target": "query",
            if (target != null)
            {
                await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorTargetName).ConfigureAwait(false);
                await jsonWriter.WriteValueAsync(target).ConfigureAwait(false);
            }

            // Such as, "details": [
            //  {
            //   "code": "301",
            //   "target": "$search",
            //   "message": "$search query option not supported"
            //  }]
            if (details != null)
            {
                await WriteErrorDetailsAsync(jsonWriter, details, JsonConstants.ODataErrorDetailsName).ConfigureAwait(false);
            }

            if (innerError != null)
            {
                await WriteInnerErrorAsync(jsonWriter, innerError, JsonConstants.ODataErrorInnerErrorName, 
                    /* recursionDepth */ 0, maxInnerErrorDepth).ConfigureAwait(false);
            }

            if (writingJsonLight)
            {
                Debug.Assert(writeInstanceAnnotationsDelegate != null, "writeInstanceAnnotations != null");
                await writeInstanceAnnotationsDelegate(instanceAnnotations).ConfigureAwait(false);
            }

            // } }
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the error details.
        /// </summary>
        /// <param name="jsonWriter">JSON writer.</param>
        /// <param name="details">The details of the error.</param>
        /// <param name="odataErrorDetailsName">The property name for the error details property.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteErrorDetailsAsync(IJsonWriterAsync jsonWriter, IEnumerable<ODataErrorDetail> details,
            string odataErrorDetailsName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(details != null, "details != null");
            Debug.Assert(odataErrorDetailsName != null, "odataErrorDetailsName != null");

            // "details": [
            await jsonWriter.WriteNameAsync(odataErrorDetailsName).ConfigureAwait(false);
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

            foreach (var detail in details.Where(d => d != null))
            {
                // {
                await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

                // "code": "301",
                await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorCodeName).ConfigureAwait(false);
                await jsonWriter.WriteValueAsync(detail.ErrorCode ?? string.Empty).ConfigureAwait(false);

                if (detail.Target != null)
                {
                    // "target": "$search"
                    await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorTargetName).ConfigureAwait(false);
                    await jsonWriter.WriteValueAsync(detail.Target).ConfigureAwait(false);
                }

                // "message": "$search query option not supported",
                await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorMessageName).ConfigureAwait(false);
                await jsonWriter.WriteValueAsync(detail.Message ?? string.Empty).ConfigureAwait(false);

                // }
                await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
            }

            // ]
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Write an inner error property and message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="innerError">Inner error details.</param>
        /// <param name="innerErrorPropertyName">The property name for the inner error property.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteInnerErrorAsync(IJsonWriterAsync jsonWriter, ODataInnerError innerError, 
            string innerErrorPropertyName, int recursionDepth, int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);

            // "innererror":
            await jsonWriter.WriteNameAsync(innerErrorPropertyName).ConfigureAwait(false);
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

            if (innerError.Properties != null)
            {
                foreach (KeyValuePair<string, ODataValue> pair in innerError.Properties)
                {
                    await jsonWriter.WriteNameAsync(pair.Key).ConfigureAwait(false);

                    if (pair.Value is ODataNullValue &&
                        (pair.Key == JsonConstants.ODataErrorInnerErrorMessageName ||
                        pair.Key == JsonConstants.ODataErrorInnerErrorStackTraceName ||
                        pair.Key == JsonConstants.ODataErrorInnerErrorTypeNameName))
                    {
                        // Write empty string for null values in stacktrace, type and message properties of inner error.
                        await jsonWriter.WriteODataValueAsync(new ODataPrimitiveValue(string.Empty)).ConfigureAwait(false);
                    }
                    else
                    {
                        await jsonWriter.WriteODataValueAsync(pair.Value).ConfigureAwait(false);
                    }
                }
            }

            if (innerError.InnerError != null)
            {
                // "internalexception": { <nested inner error> }
                await WriteInnerErrorAsync(jsonWriter, innerError.InnerError, JsonConstants.ODataErrorInnerErrorInnerErrorName, 
                    recursionDepth, maxInnerErrorDepth).ConfigureAwait(false);
            }

            // }
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }
    }
}
