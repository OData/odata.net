//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtils.cs" company="Microsoft">
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
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData.JsonLight;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer for the JSON format.
    /// </summary>
    internal static class ODataJsonWriterUtils
    {
        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Action to write the instance annotations.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        internal static void WriteError(
            IJsonWriter jsonWriter,
            Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
            ODataError error,
            bool includeDebugInformation,
            int maxInnerErrorDepth,
            bool writingJsonLight)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            string code, message;
            ErrorUtils.GetErrorDetails(error, out code, out message);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;

            WriteError(
                jsonWriter,
                code,
                message,
                error.Target,
                error.Details,
                innerError,
                error.GetInstanceAnnotations(),
                writeInstanceAnnotationsDelegate,
                maxInnerErrorDepth,
                writingJsonLight);
        }

        /// <summary>
        /// Will write the function's name and start the JSONP scope if we are writing a response and the
        /// JSONP function name is not null or empty.
        /// </summary>
        /// <param name="jsonWriter">JsonWriter to write to.</param>
        /// <param name="settings">Writer settings.</param>
        internal static void StartJsonPaddingIfRequired(IJsonWriter jsonWriter, ODataMessageWriterSettings settings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                jsonWriter.WritePaddingFunctionName(settings.JsonPCallback);
                jsonWriter.StartPaddingFunctionScope();
            }
        }

        /// <summary>
        /// If we are writing a response and the given Json Padding function name is not null or empty
        /// this function will close the JSONP scope.
        /// </summary>
        /// <param name="jsonWriter">JsonWriter to write to.</param>
        /// <param name="settings">Writer settings.</param>
        internal static void EndJsonPaddingIfRequired(IJsonWriter jsonWriter, ODataMessageWriterSettings settings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                jsonWriter.EndPaddingFunctionScope();
            }
        }

        internal static void ODataValueToString(StringBuilder sb, ODataValue value)
        {
            if (value == null || value is ODataNullValue)
            {
                sb.Append("null");
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                ODataCollectionValueToString(sb, collectionValue);
            }

            ODataResourceValue resourceValue = value as ODataResourceValue;
            if (resourceValue != null)
            {
                ODataResourceValueToString(sb, resourceValue);
            }

            ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                if (primitiveValue.FromODataValue() is string)
                {
                    sb.Append(string.Concat("\"", JsonValueUtils.GetEscapedJsonString(value.FromODataValue()?.ToString()), "\""));
                }
                else
                {
                    sb.Append(JsonValueUtils.GetEscapedJsonString(value.FromODataValue()?.ToString()));
                }
            }
        }

        /// <summary>
        /// Asynchronously writes an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Delegate to write the instance annotations.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static Task WriteErrorAsync(
            IJsonWriterAsync jsonWriter,
            Func<IEnumerable<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate,
            ODataError error,
            bool includeDebugInformation,
            int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            ExceptionUtils.CheckArgumentNotNull(writeInstanceAnnotationsDelegate, "writeInstanceAnnotationsDelegate");

            string code, message;
            ErrorUtils.GetErrorDetails(error, out code, out message);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;

            return WriteErrorAsync(
                jsonWriter,
                code,
                message,
                error.Target,
                error.Details,
                innerError,
                error.GetInstanceAnnotations(),
                writeInstanceAnnotationsDelegate,
                maxInnerErrorDepth);
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
        internal static Task EndJsonPaddingIfRequiredAsync(IJsonWriterAsync jsonWriter, ODataMessageWriterSettings settings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                return jsonWriter.EndPaddingFunctionScopeAsync();
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">JSON writer.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="target">The target of the error.</param>
        /// <param name="details">The details of the error.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="instanceAnnotations">Instance annotations for this error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Action to write the instance annotations.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        private static void WriteError(
            IJsonWriter jsonWriter,
            string code,
            string message,
            string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
            Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
            int maxInnerErrorDepth,
            bool writingJsonLight)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");

            // "error": {
            jsonWriter.StartObjectScope();
            if (writingJsonLight)
            {
                jsonWriter.WriteName(JsonLightConstants.ODataErrorPropertyName);
            }
            else
            {
                jsonWriter.WriteName(JsonConstants.ODataErrorName);
            }

            jsonWriter.StartObjectScope();

            // "code": "<code>"
            jsonWriter.WriteName(JsonConstants.ODataErrorCodeName);
            jsonWriter.WriteValue(code);

            // "message": "<message string>"
            jsonWriter.WriteName(JsonConstants.ODataErrorMessageName);
            jsonWriter.WriteValue(message);

            // For example, "target": "query",
            if (target != null)
            {
                jsonWriter.WriteName(JsonConstants.ODataErrorTargetName);
                jsonWriter.WriteValue(target);
            }

            // Such as, "details": [
            //  {
            //   "code": "301",
            //   "target": "$search",
            //   "message": "$search query option not supported"
            //  }]
            if (details != null)
            {
                WriteErrorDetails(jsonWriter, details, JsonConstants.ODataErrorDetailsName);
            }

            if (innerError != null)
            {
                WriteInnerError(jsonWriter, innerError, JsonConstants.ODataErrorInnerErrorName, /* recursionDepth */ 0, maxInnerErrorDepth);
            }

            if (writingJsonLight)
            {
                Debug.Assert(writeInstanceAnnotationsDelegate != null, "writeInstanceAnnotationsDelegate != null");
                writeInstanceAnnotationsDelegate(instanceAnnotations);
            }

            // } }
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        private static void WriteErrorDetails(
            IJsonWriter jsonWriter,
            IEnumerable<ODataErrorDetail> details,
            string odataErrorDetailsName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(details != null, "details != null");
            Debug.Assert(odataErrorDetailsName != null, "odataErrorDetailsName != null");

            // "details": [
            jsonWriter.WriteName(odataErrorDetailsName);
            jsonWriter.StartArrayScope();

            foreach (var detail in details.Where(d => d != null))
            {
                // {
                jsonWriter.StartObjectScope();

                // "code": "301",
                jsonWriter.WriteName(JsonConstants.ODataErrorCodeName);
                jsonWriter.WriteValue(detail.ErrorCode ?? string.Empty);

                if (detail.Target != null)
                {
                    // "target": "$search"
                    jsonWriter.WriteName(JsonConstants.ODataErrorTargetName);
                    jsonWriter.WriteValue(detail.Target);
                }

                // "message": "$search query option not supported",
                jsonWriter.WriteName(JsonConstants.ODataErrorMessageName);
                jsonWriter.WriteValue(detail.Message ?? string.Empty);

                // }
                jsonWriter.EndObjectScope();
            }

            // ]
            jsonWriter.EndArrayScope();
        }

        /// <summary>
        /// Write an inner error property and message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="innerError">Inner error details.</param>
        /// <param name="innerErrorPropertyName">The property name for the inner error property.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximum number of nested inner errors to allow.</param>
        private static void WriteInnerError(
            IJsonWriter jsonWriter,
            ODataInnerError innerError,
            string innerErrorPropertyName,
            int recursionDepth,
            int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);

            // "innererror":
            jsonWriter.WriteName(innerErrorPropertyName);
            jsonWriter.StartObjectScope();

            if (innerError.Properties != null)
            {
                foreach (KeyValuePair<string, ODataValue> pair in innerError.Properties)
                {
                    jsonWriter.WriteName(pair.Key);

                    if (pair.Value is ODataNullValue &&
                        (pair.Key == JsonConstants.ODataErrorInnerErrorMessageName ||
                        pair.Key == JsonConstants.ODataErrorInnerErrorStackTraceName ||
                        pair.Key == JsonConstants.ODataErrorInnerErrorTypeNameName))
                    {
                        // Write empty string for null values in stacktrace, type and message properties of inner error.
                        jsonWriter.WriteODataValue(new ODataPrimitiveValue(string.Empty));
                    }
                    else
                    {
                        jsonWriter.WriteODataValue(pair.Value);
                    }
                }
            }

            if (innerError.InnerError != null)
            {
                // "internalexception": { <nested inner error> }
                WriteInnerError(jsonWriter, innerError.InnerError, JsonConstants.ODataErrorInnerErrorInnerErrorName, recursionDepth, maxInnerErrorDepth);
            }

            // }
            jsonWriter.EndObjectScope();
        }

        private static void ODataCollectionValueToString(StringBuilder sb, ODataCollectionValue value)
        {
            bool isFirst = true;
            sb.Append("[");
            foreach (object item in value.Items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }

                ODataValue odataValue = item as ODataValue;
                if (odataValue != null)
                {
                    ODataValueToString(sb, odataValue);
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueInCollection);
                }
            }

            sb.Append("]");
        }

        private static void ODataResourceValueToString(StringBuilder sb, ODataResourceValue value)
        {
            bool isFirst = true;
            sb.Append("{");
            foreach (ODataProperty property in value.Properties)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }

                sb.Append("\"").Append(property.Name).Append("\"").Append(":");
                ODataValueToString(sb, property.ODataValue);
            }

            sb.Append("}");
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
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteErrorAsync(
            IJsonWriterAsync jsonWriter,
            string code,
            string message,
            string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
            Func<IEnumerable<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate,
            int maxInnerErrorDepth)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");

            ExceptionUtils.CheckArgumentNotNull(writeInstanceAnnotationsDelegate, "writeInstanceAnnotationsDelegate");

            // "error": {
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteNameAsync(JsonLightConstants.ODataErrorPropertyName).ConfigureAwait(false);
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

            await writeInstanceAnnotationsDelegate(instanceAnnotations).ConfigureAwait(false);

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
