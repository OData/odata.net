//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if !INTERNAL_DROP || ODATALIB

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Core.Atom;
    #endregion Namespaces

    /// <summary>
    /// Utility methods serializing the xml error payload
    /// </summary>
    internal static class ErrorUtils
    {
        /// <summary>Default language for error messages if not specified.</summary>
        /// <remarks>
        /// This constant is included here since this file is compiled into WCF DS Server as well
        /// so we can't compile in the ODataConstants.
        /// </remarks>
        internal const string ODataErrorMessageDefaultLanguage = "en-US";

        /// <summary>
        /// Extracts error details from an <see cref="ODataError"/>.
        /// </summary>
        /// <param name="error">The ODataError instance to extract the error details from.</param>
        /// <param name="code">A data service-defined string which serves as a substatus to the HTTP response code.</param>
        /// <param name="message">A human readable message describing the error.</param>
        internal static void GetErrorDetails(ODataError error, out string code, out string message)
        {
            Debug.Assert(error != null, "error != null");

            code = error.ErrorCode ?? string.Empty;
            message = error.Message ?? string.Empty;
        }
        
        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        internal static void WriteXmlError(XmlWriter writer, ODataError error, bool includeDebugInformation, int maxInnerErrorDepth)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(error != null, "error != null");

            string code, message;
            ErrorUtils.GetErrorDetails(error, out code, out message);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;
            WriteXmlError(writer, code, message, innerError, maxInnerErrorDepth);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        private static void WriteXmlError(XmlWriter writer, string code, string message, ODataInnerError innerError, int maxInnerErrorDepth)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");

            // <m:error>
            writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorElementName, AtomConstants.ODataMetadataNamespace);

            // <m:code>code</m:code>
            writer.WriteElementString(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorCodeElementName, AtomConstants.ODataMetadataNamespace, code);

            // <m:message>error message</m:message>
            writer.WriteElementString(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataErrorMessageElementName, AtomConstants.ODataMetadataNamespace, message);

            if (innerError != null)
            {
                WriteXmlInnerError(writer, innerError, AtomConstants.ODataInnerErrorElementName, /* recursionDepth */ 0, maxInnerErrorDepth);
            }

            // </m:error>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the inner exception information in debug mode.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="innerError">The inner error to write.</param>
        /// <param name="innerErrorElementName">The local name of the element representing the inner error.</param>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of nested inner errors to allow.</param>
        private static void WriteXmlInnerError(XmlWriter writer, ODataInnerError innerError, string innerErrorElementName, int recursionDepth, int maxInnerErrorDepth)
        {
            Debug.Assert(writer != null, "writer != null");

            recursionDepth++;
            if (recursionDepth > maxInnerErrorDepth)
            {
#if ODATALIB
                throw new ODataException(Strings.ValidationUtils_RecursionDepthLimitReached(maxInnerErrorDepth));
#else
                throw new ODataException(Microsoft.OData.Service.Strings.BadRequest_DeepRecursion(maxInnerErrorDepth));
#endif
            }

            // <m:innererror> or <m:internalexception>
            writer.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, innerErrorElementName, AtomConstants.ODataMetadataNamespace);

            //// NOTE: we add empty elements if no information is provided for the message, error type and stack trace
            ////       to stay compatible with Astoria.

            // <m:message>...</m:message>
            string errorMessage = innerError.Message ?? String.Empty;
            writer.WriteStartElement(AtomConstants.ODataInnerErrorMessageElementName, AtomConstants.ODataMetadataNamespace);
            writer.WriteString(errorMessage);
            writer.WriteEndElement();

            // <m:type>...</m:type>
            string errorType = innerError.TypeName ?? string.Empty;
            writer.WriteStartElement(AtomConstants.ODataInnerErrorTypeElementName, AtomConstants.ODataMetadataNamespace);
            writer.WriteString(errorType);
            writer.WriteEndElement();

            // <m:stacktrace>...</m:stacktrace>
            string errorStackTrace = innerError.StackTrace ?? String.Empty;
            writer.WriteStartElement(AtomConstants.ODataInnerErrorStackTraceElementName, AtomConstants.ODataMetadataNamespace);
            writer.WriteString(errorStackTrace);
            writer.WriteEndElement();

            if (innerError.InnerError != null)
            {
                WriteXmlInnerError(writer, innerError.InnerError, AtomConstants.ODataInnerErrorInnerErrorElementName, recursionDepth, maxInnerErrorDepth);
            }

            // </m:innererror> or </m:internalexception>
            writer.WriteEndElement();
        }
    }
}

#endif
