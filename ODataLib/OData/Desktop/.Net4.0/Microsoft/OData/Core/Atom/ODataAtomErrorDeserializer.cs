//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for error payloads.
    /// </summary>
    internal sealed class ODataAtomErrorDeserializer : ODataAtomDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomErrorDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
        }

        /// <summary>
        /// An enumeration of the various kinds of elements in an m:error element.
        /// </summary>
        [Flags]
        private enum DuplicateErrorElementPropertyBitMask
        {
            /// <summary>No duplicates.</summary>
            None = 0,

            /// <summary>The 'code' element of the error element.</summary>
            Code = 1,

            /// <summary>The 'message' element of the error element.</summary>
            Message = 2,

            /// <summary>The 'innererror' element of the error element.</summary>
            InnerError = 4,
        }

        /// <summary>
        /// An enumeration of the various kinds of elements in an internal error element.
        /// </summary>
        [Flags]
        private enum DuplicateInnerErrorElementPropertyBitMask
        {
            /// <summary>No duplicates.</summary>
            None = 0,

            /// <summary>The 'message' element of the inner error element.</summary>
            Message = 1,

            /// <summary>The 'type' element of the inner error element.</summary>
            TypeName = 2,

            /// <summary>The 'stacktrace' element of the inner error element.</summary>
            StackTrace = 4,

            /// <summary>The 'internalexception' element of the inner error element.</summary>
            InternalException = 8,
        }

        /// <summary>
        /// Reads the content of an error element.
        /// </summary>
        /// <param name="xmlReader">The Xml reader to read the error payload from.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception elements to allow.</param>
        /// <returns>The <see cref="ODataError"/> representing the error.</returns>
        /// <remarks>
        /// This method is used to read top-level errors as well as in-stream errors (from inside the buffering Xml reader).
        /// Pre-Condition:  XmlNodeType.Element   - The m:error start element.
        /// Post-Condition: XmlNodeType.EndElement - The m:error end-element.
        ///                 XmlNodeType.Element    - The empty m:error start element.
        /// </remarks>
        internal static ODataError ReadErrorElement(BufferingXmlReader xmlReader, int maxInnerErrorDepth)
        {
            Debug.Assert(xmlReader != null, "this.XmlReader != null");
            Debug.Assert(xmlReader.NodeType == XmlNodeType.Element, "xmlReader.NodeType == XmlNodeType.Element");
            Debug.Assert(xmlReader.LocalName == AtomConstants.ODataErrorElementName, "Expected reader to be positioned on <m:error> element.");
            Debug.Assert(xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace), "this.XmlReader.NamespaceEquals(atomizedMetadataNamespace)");

            ODataError error = new ODataError();
            DuplicateErrorElementPropertyBitMask elementsReadBitmask = DuplicateErrorElementPropertyBitMask.None;

            if (!xmlReader.IsEmptyElement)
            {
                // Move to the first child node of the element.
                xmlReader.Read();

                do
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            // end of the <m:error> element
                            continue;

                        case XmlNodeType.Element:
                            if (xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace))
                            {
                                switch (xmlReader.LocalName)
                                {
                                    // <m:code>
                                    case AtomConstants.ODataErrorCodeElementName:
                                        VerifyErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateErrorElementPropertyBitMask.Code,
                                            AtomConstants.ODataErrorCodeElementName);
                                        error.ErrorCode = xmlReader.ReadElementValue();
                                        continue;

                                    // <m:message >
                                    case AtomConstants.ODataErrorMessageElementName:
                                        VerifyErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateErrorElementPropertyBitMask.Message,
                                            AtomConstants.ODataErrorMessageElementName);
                                        error.Message = xmlReader.ReadElementValue();
                                        continue;

                                    // <m:innererror>
                                    case AtomConstants.ODataInnerErrorElementName:
                                        VerifyErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateErrorElementPropertyBitMask.InnerError,
                                            AtomConstants.ODataInnerErrorElementName);
                                        error.InnerError = ReadInnerErrorElement(xmlReader, 0 /* recursionDepth */, maxInnerErrorDepth);
                                        continue;

                                    default:
                                        break;
                                }
                            }

                            break;
                        default:
                            break;
                    }

                    xmlReader.Skip();
                }
                while (xmlReader.NodeType != XmlNodeType.EndElement);
            }

            return error;
        }

        /// <summary>
        /// Reads a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.None    - assumes that the Xml reader has not been used yet.
        /// Post-Condition: Any                 - the next node after the m:error end element or the empty m:error element node.
        /// </remarks>
        internal ODataError ReadTopLevelError()
        {
            Debug.Assert(this.ReadingResponse, "Top-level errors are only supported in responses.");
            Debug.Assert(this.XmlReader != null, "this.XmlReader != null");
            Debug.Assert(!this.XmlReader.DisableInStreamErrorDetection, "!this.XmlReader.DetectInStreamErrors");

            try
            {
                this.XmlReader.DisableInStreamErrorDetection = true;

                this.ReadPayloadStart();
                this.AssertXmlCondition(XmlNodeType.Element);

                // check for the <m:error> element
                if (!this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace) ||
                    !this.XmlReader.LocalNameEquals(this.XmlReader.ODataErrorElementName))
                {
                    throw new ODataErrorException(
                        Strings.ODataAtomErrorDeserializer_InvalidRootElement(this.XmlReader.Name, this.XmlReader.NamespaceURI));
                }

                ODataError error = ReadErrorElement(this.XmlReader, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

                // The ReadErrorElement leaves the reader on the m:error end element or empty start element.
                // So read over that to consume the entire m:error element.
                this.XmlReader.Read();

                this.ReadPayloadEnd();
                return error;
            }
            finally
            {
                this.XmlReader.DisableInStreamErrorDetection = false;
            }
        }

        /// <summary>
        /// Verifies that the specified element was not yet found in a top-level error element.
        /// </summary>
        /// <param name="elementsFoundBitField">
        /// The bit field which stores which elements of an error were found so far.
        /// </param>
        /// <param name="elementFoundBitMask">The bit mask for the element to check.</param>
        /// <param name="elementName">The name of the element to check (used for error reporting).</param>
        private static void VerifyErrorElementNotFound(
            ref DuplicateErrorElementPropertyBitMask elementsFoundBitField,
            DuplicateErrorElementPropertyBitMask elementFoundBitMask,
            string elementName)
        {
            Debug.Assert(((int)elementFoundBitMask & (((int)elementFoundBitMask) - 1)) == 0, "elementFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(elementName), "!string.IsNullOrEmpty(elementName)");

            if ((elementsFoundBitField & elementFoundBitMask) == elementFoundBitMask)
            {
                throw new ODataException(Strings.ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName(elementName));
            }

            elementsFoundBitField |= elementFoundBitMask;
        }

        /// <summary>
        /// Verifies that the specified element was not yet found in an inner error element.
        /// </summary>
        /// <param name="elementsFoundBitField">
        /// The bit field which stores which elements of an inner error were found so far.
        /// </param>
        /// <param name="elementFoundBitMask">The bit mask for the element to check.</param>
        /// <param name="elementName">The name of the element to check (used for error reporting).</param>
        private static void VerifyInnerErrorElementNotFound(
            ref DuplicateInnerErrorElementPropertyBitMask elementsFoundBitField,
            DuplicateInnerErrorElementPropertyBitMask elementFoundBitMask,
            string elementName)
        {
            Debug.Assert(((int)elementFoundBitMask & (((int)elementFoundBitMask) - 1)) == 0, "elementFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(elementName), "!string.IsNullOrEmpty(elementName)");

            if ((elementsFoundBitField & elementFoundBitMask) == elementFoundBitMask)
            {
                throw new ODataException(Strings.ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName(elementName));
            }

            elementsFoundBitField |= elementFoundBitMask;
        }

        /// <summary>
        /// Reads the content of an inner error element.
        /// </summary>
        /// <param name="xmlReader">The (buffering) Xml reader to read the error payload from.</param>
        /// <param name="recursionDepth">The number of times this function has been called recursively.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception elements to allow.</param>
        /// <returns>The <see cref="ODataInnerError"/> representing the inner error.</returns>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element - the m:innererror or m:internalexception element
        /// Post-Condition: Any                 - the node after the m:innererror/m:internalexception end element or the node after the empty m:innererror/m:internalexception element node.
        /// </remarks>
        private static ODataInnerError ReadInnerErrorElement(BufferingXmlReader xmlReader, int recursionDepth, int maxInnerErrorDepth)
        {
            Debug.Assert(xmlReader != null, "this.XmlReader != null");
            Debug.Assert(xmlReader.NodeType == XmlNodeType.Element, "xmlReader.NodeType == XmlNodeType.Element");
            Debug.Assert(
                xmlReader.LocalName == AtomConstants.ODataInnerErrorElementName || 
                xmlReader.LocalName == AtomConstants.ODataInnerErrorInnerErrorElementName,
                "Expected reader to be positioned on 'm:innererror' or 'm:internalexception' element.");
            Debug.Assert(xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace), "this.XmlReader.NamespaceEquals(this.ODataMetadataNamespace)");

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);

            ODataInnerError innerError = new ODataInnerError();
            DuplicateInnerErrorElementPropertyBitMask elementsReadBitmask = DuplicateInnerErrorElementPropertyBitMask.None;

            if (!xmlReader.IsEmptyElement)
            {
                // Move to the first child node of the element.
                xmlReader.Read();

                do
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            // end of the <m:innererror> or <m:internalexception> element
                            continue;

                        case XmlNodeType.Element:
                            if (xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace))
                            {
                                switch (xmlReader.LocalName)
                                {
                                    // <m:message>
                                    case AtomConstants.ODataInnerErrorMessageElementName:
                                        VerifyInnerErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateInnerErrorElementPropertyBitMask.Message,
                                            AtomConstants.ODataInnerErrorMessageElementName);
                                        innerError.Message = xmlReader.ReadElementValue();
                                        continue;

                                    // <m:type>
                                    case AtomConstants.ODataInnerErrorTypeElementName:
                                        VerifyInnerErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateInnerErrorElementPropertyBitMask.TypeName,
                                            AtomConstants.ODataInnerErrorTypeElementName);
                                        innerError.TypeName = xmlReader.ReadElementValue();
                                        continue;

                                    // <m:stacktrace>
                                    case AtomConstants.ODataInnerErrorStackTraceElementName:
                                        VerifyInnerErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateInnerErrorElementPropertyBitMask.StackTrace,
                                            AtomConstants.ODataInnerErrorStackTraceElementName);
                                        innerError.StackTrace = xmlReader.ReadElementValue();
                                        continue;

                                    // <m:internalexception>
                                    case AtomConstants.ODataInnerErrorInnerErrorElementName:
                                        VerifyInnerErrorElementNotFound(
                                            ref elementsReadBitmask,
                                            DuplicateInnerErrorElementPropertyBitMask.InternalException,
                                            AtomConstants.ODataInnerErrorInnerErrorElementName);
                                        innerError.InnerError = ReadInnerErrorElement(xmlReader, recursionDepth, maxInnerErrorDepth);
                                        continue;

                                    default:
                                        break;
                                }
                            }

                            break;
                        default:
                            break;
                    }

                    xmlReader.Skip();
                }
                while (xmlReader.NodeType != XmlNodeType.EndElement);
            }

            // Read over the end element, or empty start element.
            xmlReader.Read();

            return innerError;
        }
    }
}
