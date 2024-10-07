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
    using Microsoft.OData.Edm;
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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal static void WriteError(
            IJsonWriter jsonWriter,
            Action<ICollection<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
            ODataError error,
            bool includeDebugInformation,
            ODataMessageWriterSettings messageWriterSettings)
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
                messageWriterSettings);
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
                return;
            }

            if (value is ODataCollectionValue collectionValue)
            {
                ODataCollectionValueToString(sb, collectionValue);
                return;
            }

            if (value is ODataResourceValue resourceValue)
            {
                ODataResourceValueToString(sb, resourceValue);
                return;
            }

            if (value is ODataPrimitiveValue primitiveValue)
            {
                object valueAsObject = primitiveValue.FromODataValue();
                string valueAsString;
                if (ODataRawValueUtils.TryConvertPrimitiveToString(valueAsObject, out valueAsString))
                {
                    if (valueAsObject is string)
                    {
                        valueAsString = JsonValueUtils.GetEscapedJsonString(valueAsString);
                        sb.Append('"').Append(valueAsString).Append('"');
                    }
                    else if (valueAsObject is byte[]
                        || valueAsObject is DateTimeOffset
                        || valueAsObject is Guid
                        || valueAsObject is TimeSpan
                        || valueAsObject is Date
                        || valueAsObject is TimeOfDay
                        || valueAsObject is DateOnly
                        || valueAsObject is TimeOnly)
                    {
                        sb.Append('"').Append(valueAsString).Append('"');
                    }
                    else
                    {
                        sb.Append(valueAsString);
                    }
                }
                else
                {
                    // For unsupported primitive values (e.g. spatial values)
                    sb.Append('"').Append(JsonValueUtils.GetEscapedJsonString(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(valueAsObject.GetType().FullName))).Append('"');
                }

                return;
            }

            // Subclasses of ODataValue that are not supported in ODataInnerError.Properties dictionary
            sb.Append('"').Append(JsonValueUtils.GetEscapedJsonString(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName))).Append('"');
        }

        /// <summary>
        /// Asynchronously writes an error message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Delegate to write the instance annotations.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static Task WriteErrorAsync(
            IJsonWriter jsonWriter,
            Func<ICollection<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate,
            ODataError error,
            bool includeDebugInformation,
            ODataMessageWriterSettings messageWriterSettings)
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
                messageWriterSettings);
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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        private static void WriteError(
            IJsonWriter jsonWriter,
            string code,
            string message,
            string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            Action<ICollection<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");

            // "error": {
            jsonWriter.StartObjectScope();

            jsonWriter.WriteName(ODataJsonConstants.ODataErrorPropertyName);

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
                WriteInnerError(
                    jsonWriter,
                    innerError,
                    JsonConstants.ODataErrorInnerErrorName,
                    recursionDepth: 0,
                    messageWriterSettings);
            }

            Debug.Assert(writeInstanceAnnotationsDelegate != null, "writeInstanceAnnotationsDelegate != null");
            writeInstanceAnnotationsDelegate(instanceAnnotations);

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
                jsonWriter.WriteValue(detail.Code ?? string.Empty);

                // "message": "$search query option not supported",
                jsonWriter.WriteName(JsonConstants.ODataErrorMessageName);
                jsonWriter.WriteValue(detail.Message ?? string.Empty);

                if (detail.Target != null)
                {
                    // "target": "$search"
                    jsonWriter.WriteName(JsonConstants.ODataErrorTargetName);
                    jsonWriter.WriteValue(detail.Target);
                }

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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        private static void WriteInnerError(
            IJsonWriter jsonWriter,
            ODataInnerError innerError,
            string innerErrorPropertyName,
            int recursionDepth,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, messageWriterSettings.MessageQuotas.MaxNestingDepth);

            // "innererror":
            jsonWriter.WriteName(innerErrorPropertyName);
            jsonWriter.StartObjectScope();

            foreach (KeyValuePair<string, ODataValue> pair in innerError.Properties)
            {
                jsonWriter.WriteName(pair.Key);

                jsonWriter.WriteODataValue(pair.Value);
            }

            if (innerError.InnerError != null)
            {
                string nestedInnerErrorPropertyName;
                if (messageWriterSettings.LibraryCompatibility.HasFlag(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization))
                {
                    nestedInnerErrorPropertyName = JsonConstants.ODataErrorInnerErrorInnerErrorName;
                }
                else
                {
                    nestedInnerErrorPropertyName = JsonConstants.ODataErrorInnerErrorName;
                }

                // "internalexception": { <nested inner error> } or "innererror": { <nested inner error> }
                WriteInnerError(jsonWriter, innerError.InnerError, nestedInnerErrorPropertyName, recursionDepth, messageWriterSettings);
            }

            // }
            jsonWriter.EndObjectScope();
        }

        private static void ODataCollectionValueToString(StringBuilder sb, ODataCollectionValue value)
        {
            bool isFirst = true;
            sb.Append('[');
            foreach (object item in value.Items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(',');
                }

                if (item is ODataValue odataValue)
                {
                    ODataValueToString(sb, odataValue);
                }
                else
                {
                    sb.Append('"').Append(JsonValueUtils.GetEscapedJsonString(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(item.GetType().FullName))).Append('"');
                }
            }

            sb.Append(']');
        }

        private static void ODataResourceValueToString(StringBuilder sb, ODataResourceValue value)
        {
            bool firstProperty = true;
            sb.Append('{');
            foreach (ODataProperty property in value.Properties)
            {
                if (firstProperty)
                {
                    firstProperty = false;
                }
                else
                {
                    sb.Append(',');
                }

                sb.Append("\"").Append(property.Name).Append("\"").Append(':');
                ODataValueToString(sb, property.ODataValue);
            }

            sb.Append('}');
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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteErrorAsync(
            IJsonWriter jsonWriter,
            string code,
            string message,
            string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            ICollection<ODataInstanceAnnotation> instanceAnnotations,
            Func<ICollection<ODataInstanceAnnotation>, Task> writeInstanceAnnotationsDelegate,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            ExceptionUtils.CheckArgumentNotNull(writeInstanceAnnotationsDelegate, "writeInstanceAnnotationsDelegate");

            // "error": {
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteNameAsync(ODataJsonConstants.ODataErrorPropertyName).ConfigureAwait(false);
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
                await WriteInnerErrorAsync(
                    jsonWriter,
                    innerError,
                    JsonConstants.ODataErrorInnerErrorName,
                    recursionDepth: 0,
                    messageWriterSettings).ConfigureAwait(false);
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
        private static async Task WriteErrorDetailsAsync(IJsonWriter jsonWriter, IEnumerable<ODataErrorDetail> details,
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
                await jsonWriter.WriteValueAsync(detail.Code ?? string.Empty).ConfigureAwait(false);

                // "message": "$search query option not supported",
                await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorMessageName).ConfigureAwait(false);
                await jsonWriter.WriteValueAsync(detail.Message ?? string.Empty).ConfigureAwait(false);

                if (detail.Target != null)
                {
                    // "target": "$search"
                    await jsonWriter.WriteNameAsync(JsonConstants.ODataErrorTargetName).ConfigureAwait(false);
                    await jsonWriter.WriteValueAsync(detail.Target).ConfigureAwait(false);
                }

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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteInnerErrorAsync(
            IJsonWriter jsonWriter,
            ODataInnerError innerError,
            string innerErrorPropertyName,
            int recursionDepth,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(innerErrorPropertyName != null, "innerErrorPropertyName != null");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, messageWriterSettings.MessageQuotas.MaxNestingDepth);

            // "innererror":
            await jsonWriter.WriteNameAsync(innerErrorPropertyName).ConfigureAwait(false);
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

            foreach (KeyValuePair<string, ODataValue> pair in innerError.Properties)
            {
                await jsonWriter.WriteNameAsync(pair.Key).ConfigureAwait(false);

                await jsonWriter.WriteODataValueAsync(pair.Value).ConfigureAwait(false);
            }

            if (innerError.InnerError != null)
            {
                string nestedInnerErrorPropertyName;
                if (messageWriterSettings.LibraryCompatibility.HasFlag(ODataLibraryCompatibility.UseLegacyODataInnerErrorSerialization))
                {
                    nestedInnerErrorPropertyName = JsonConstants.ODataErrorInnerErrorInnerErrorName;
                }
                else
                {
                    nestedInnerErrorPropertyName = JsonConstants.ODataErrorInnerErrorName;
                }

                // "internalexception": { <nested inner error> } or "innererror": { <nested inner error> }
                await WriteInnerErrorAsync(
                    jsonWriter,
                    innerError.InnerError,
                    nestedInnerErrorPropertyName,
                    recursionDepth,
                    messageWriterSettings).ConfigureAwait(false);
            }

            // }
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the 'value' property name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        internal static void WriteValuePropertyName(this IJsonWriter jsonWriter)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            jsonWriter.WriteName(ODataJsonConstants.ODataValuePropertyName);
        }

        /// <summary>
        /// Write a JSON property name which represents a property annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        internal static void WritePropertyAnnotationName(this IJsonWriter jsonWriter, string propertyName, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            jsonWriter.WriteName(propertyName + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        internal static void WriteInstanceAnnotationName(this IJsonWriter jsonWriter, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            jsonWriter.WriteName(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Writes the 'value' property name asynchronously.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        internal static Task WriteValuePropertyNameAsync(this IJsonWriter jsonWriter)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            return jsonWriter.WriteNameAsync(ODataJsonConstants.ODataValuePropertyName);
        }

        /// <summary>
        /// Write a JSON property name which represents a property annotation asynchronously.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation to write.</param>
        internal static Task WritePropertyAnnotationNameAsync(this IJsonWriter jsonWriter, string propertyName, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            return jsonWriter.WriteNameAsync(propertyName + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }

        /// <summary>
        /// Write a JSON instance annotation name which represents a instance annotation.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="annotationName">The name of the instance annotation to write.</param>
        internal static Task WriteInstanceAnnotationNameAsync(this IJsonWriter jsonWriter, string annotationName)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            return jsonWriter.WriteNameAsync(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + annotationName);
        }
    }
}
