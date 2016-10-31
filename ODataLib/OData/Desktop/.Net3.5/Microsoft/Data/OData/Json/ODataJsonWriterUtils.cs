//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.JsonLight;
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
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        internal static void WriteError(IJsonWriter jsonWriter, Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate, ODataError error, bool includeDebugInformation, int maxInnerErrorDepth, bool writingJsonLight)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(error != null, "error != null");

            string code, message, messageLanguage;
            ErrorUtils.GetErrorDetails(error, out code, out message, out messageLanguage);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;

            var instanceAnnotations = error.GetInstanceAnnotationsForWriting();
            WriteError(jsonWriter, code, message, messageLanguage, innerError, instanceAnnotations, writeInstanceAnnotationsDelegate, maxInnerErrorDepth, writingJsonLight);
        }

        /// <summary>
        /// Writes the __metadata property with the specified type name.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="typeName">The type name to write.</param>
        internal static void WriteMetadataWithTypeName(IJsonWriter jsonWriter, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(typeName != null, "typeName != null");

            // Write the __metadata object
            jsonWriter.WriteName(JsonConstants.ODataMetadataName);
            jsonWriter.StartObjectScope();

            // "type": "typename"
            jsonWriter.WriteName(JsonConstants.ODataMetadataTypeName);
            jsonWriter.WriteValue(typeName);

            // End the __metadata
            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Will write the function's name and start the JSONP scope if we are writing a response and the 
        /// JSONP function name is not null or empty.
        /// </summary>
        /// <param name="jsonWriter">JsonWriter to write to.</param>
        /// <param name="settings">Writer settings.</param>
        internal static void StartJsonPaddingIfRequired(IJsonWriter jsonWriter, ODataMessageWriterSettings settings)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(jsonWriter != null, "jsonWriter should not be null");

            if (settings.HasJsonPaddingFunction())
            {
                jsonWriter.EndPaddingFunctionScope();
            }
        }

        /// <summary>
        /// Returns the string representation of the URI; Converts the URI into an absolute URI if the <paramref name="makeAbsolute"/> parameter is set to true.
        /// </summary>
        /// <param name="outputContext">The output context for which to convert the URI.</param>
        /// <param name="uri">The uri to process.</param>
        /// <param name="makeAbsolute">true, if the URI needs to be translated into an absolute URI; false otherwise.</param>
        /// <returns>If the <paramref name="makeAbsolute"/> parameter is set to true, then a string representation of an absolute URI which is either the 
        /// specified <paramref name="uri"/> if it was absolute, or it's a combination of the BaseUri and the relative <paramref name="uri"/>; 
        /// otherwise a string representation of the specified <paramref name="uri"/>.
        /// </returns>
        /// <remarks>This method will fail if <paramref name="makeAbsolute"/> is set to true and the specified <paramref name="uri"/> is relative and there's no base URI available.</remarks>
        internal static string UriToUriString(ODataOutputContext outputContext, Uri uri, bool makeAbsolute)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (outputContext.UrlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = outputContext.UrlResolver.ResolveUrl(outputContext.MessageWriterSettings.BaseUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            resultUri = uri;

            if (!resultUri.IsAbsoluteUri)
            {
                if (makeAbsolute)
                {
                    if (outputContext.MessageWriterSettings.BaseUri == null)
                    {
                        throw new ODataException(Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
                    }

                    resultUri = UriUtils.UriToAbsoluteUri(outputContext.MessageWriterSettings.BaseUri, uri);
                }
                else
                {
                    // NOTE: the only URIs that are allowed to be relative are metadata URIs 
                    //       in operations; for such metadata URIs there is no base URI.
                    resultUri = UriUtils.EnsureEscapedRelativeUri(resultUri);
                }
            }

            return UriUtilsCommon.UriToString(resultUri);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="jsonWriter">JSON writer.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="messageLanguage">The language of the message.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="instanceAnnotations">Instance annotations for this error.</param>
        /// <param name="writeInstanceAnnotationsDelegate">Action to write the instance annotations.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        /// <param name="writingJsonLight">true if we're writing JSON lite, false if we're writing verbose JSON.</param>
        private static void WriteError(IJsonWriter jsonWriter, string code, string message, string messageLanguage, ODataInnerError innerError, IEnumerable<ODataInstanceAnnotation> instanceAnnotations, Action<IEnumerable<ODataInstanceAnnotation>> writeInstanceAnnotationsDelegate, int maxInnerErrorDepth, bool writingJsonLight)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageLanguage != null, "messageLanguage != null");
            Debug.Assert(instanceAnnotations != null, "instanceAnnotations != null");

            // if verbose JSON: { "error": {
            // if JSON lite: { "odata.error": {
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName(writingJsonLight ? ODataAnnotationNames.ODataError : JsonConstants.ODataErrorName);
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

        /// <summary>
        /// Write an inner error property and message.
        /// </summary>
        /// <param name="jsonWriter">The JSON writer to write the error to.</param>
        /// <param name="innerError">Inner error details.</param>
        /// <param name="innerErrorPropertyName">The property name for the inner error property.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
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
