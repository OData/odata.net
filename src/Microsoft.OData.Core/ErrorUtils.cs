//---------------------------------------------------------------------
// <copyright file="ErrorUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Xml;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;

namespace Microsoft.OData
{
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

            code = error.Code ?? string.Empty;
            message = error.Message ?? string.Empty;
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="error">The error instance to write.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal static void WriteXmlError(
            XmlWriter writer,
            ODataError error,
            bool includeDebugInformation,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(error != null, "error != null");

            string code, message;
            ErrorUtils.GetErrorDetails(error, out code, out message);

            ODataInnerError innerError = includeDebugInformation ? error.InnerError : null;
            WriteXmlError(writer, code, message, innerError, messageWriterSettings);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name="writer">The Xml writer to write to.</param>
        /// <param name="code">The code of the error.</param>
        /// <param name="message">The message of the error.</param>
        /// <param name="innerError">Inner error details that will be included in debug mode (if present).</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        private static void WriteXmlError(
            XmlWriter writer,
            string code,
            string message,
            ODataInnerError innerError,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(code != null, "code != null");
            Debug.Assert(message != null, "message != null");

            // <m:error>
            writer.WriteStartElement(ODataMetadataConstants.ODataMetadataNamespacePrefix, ODataMetadataConstants.ODataErrorElementName, ODataMetadataConstants.ODataMetadataNamespace);

            // <m:code>code</m:code>
            writer.WriteElementString(ODataMetadataConstants.ODataMetadataNamespacePrefix, ODataMetadataConstants.ODataErrorCodeElementName, ODataMetadataConstants.ODataMetadataNamespace, code);

            // <m:message>error message</m:message>
            writer.WriteElementString(ODataMetadataConstants.ODataMetadataNamespacePrefix, ODataMetadataConstants.ODataErrorMessageElementName, ODataMetadataConstants.ODataMetadataNamespace, message);

            if (innerError != null)
            {
                WriteXmlInnerError(writer, innerError, ODataMetadataConstants.ODataInnerErrorElementName, recursionDepth: 0, messageWriterSettings);
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
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        private static void WriteXmlInnerError(
            XmlWriter writer,
            ODataInnerError innerError,
            string innerErrorElementName,
            int recursionDepth,
            ODataMessageWriterSettings messageWriterSettings)
        {
            Debug.Assert(writer != null, "writer != null");

            int maxInnerErrorDepth = messageWriterSettings.MessageQuotas.MaxNestingDepth;
            recursionDepth++;
            if (recursionDepth > maxInnerErrorDepth)
            {
#if ODATA_CORE
                throw new ODataException(Strings.ValidationUtils_RecursionDepthLimitReached(maxInnerErrorDepth));
#else
                throw new ODataException(Microsoft.OData.Service.Strings.BadRequest_DeepRecursion(maxInnerErrorDepth));
#endif
            }

            // <m:innererror> or <m:internalexception>
            writer.WriteStartElement(ODataMetadataConstants.ODataMetadataNamespacePrefix, innerErrorElementName, ODataMetadataConstants.ODataMetadataNamespace);

            ODataValue propertyValue = null;

            // <m:message>...</m:message>
            if (innerError.Properties.TryGetValue(ODataMetadataConstants.ODataInnerErrorMessageElementName, out propertyValue)
                && propertyValue is ODataPrimitiveValue innerErrorMessage)
            {
                writer.WriteStartElement(ODataMetadataConstants.ODataInnerErrorMessageElementName, ODataMetadataConstants.ODataMetadataNamespace);
                writer.WriteString((string)innerErrorMessage.Value);
                writer.WriteEndElement();
            }

            // <m:type>...</m:type>
            if (innerError.Properties.TryGetValue(ODataMetadataConstants.ODataInnerErrorTypeElementName, out propertyValue)
                && propertyValue is ODataPrimitiveValue innerErrorTypeName)
            {
                writer.WriteStartElement(ODataMetadataConstants.ODataInnerErrorTypeElementName, ODataMetadataConstants.ODataMetadataNamespace);
                writer.WriteString((string)innerErrorTypeName.Value);
                writer.WriteEndElement();
            }

            // <m:stacktrace>...</m:stacktrace>
            if (innerError.Properties.TryGetValue(ODataMetadataConstants.ODataInnerErrorTypeElementName, out propertyValue)
                && propertyValue is ODataPrimitiveValue innerErrorStackTrace)
            {
                writer.WriteStartElement(ODataMetadataConstants.ODataInnerErrorStackTraceElementName, ODataMetadataConstants.ODataMetadataNamespace);
                writer.WriteString((string)innerErrorStackTrace.Value);
                writer.WriteEndElement();
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

                WriteXmlInnerError(writer, innerError.InnerError, nestedInnerErrorPropertyName, recursionDepth, messageWriterSettings);
            }

            // </m:innererror> or </m:internalexception>
            writer.WriteEndElement();
        }
    }
}