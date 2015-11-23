﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.JsonLight;
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
        internal static void WriteError(IJsonWriter jsonWriter,
            Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate, ODataError error,
            bool includeDebugInformation, int maxInnerErrorDepth, bool writingJsonLight)
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
        private static void WriteError(IJsonWriter jsonWriter, string code, string message, string target,
            IEnumerable<ODataErrorDetail> details,
            ODataInnerError innerError,
            IEnumerable<ODataInstanceAnnotation> instanceAnnotations,
            Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate, int maxInnerErrorDepth,
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
                Debug.Assert(writeInstanceAnnotationsDelegate != null, "writeInstanceAnnotations != null");
                writeInstanceAnnotationsDelegate(instanceAnnotations);
            }

            // } }
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        private static void WriteErrorDetails(IJsonWriter jsonWriter, IEnumerable<ODataErrorDetail> details,
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
        private static void WriteInnerError(IJsonWriter jsonWriter, ODataInnerError innerError, string innerErrorPropertyName, int recursionDepth, int maxInnerErrorDepth)
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
                WriteInnerError(jsonWriter, innerError.InnerError, JsonConstants.ODataErrorInnerErrorInnerErrorName, recursionDepth, maxInnerErrorDepth);
            }

            // }
            jsonWriter.EndObjectScope();
        }
    }
}
