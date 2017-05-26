//---------------------------------------------------------------------
// <copyright file="ODataMetadataConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
{
    /// <summary>
    /// Constant values related to the Metadata format.
    /// </summary>
    internal static class ODataMetadataConstants
    {
        #region Xml constants -----------------------------------------------------------------------------------------
        /// <summary>Attribute use to add xml: namespaces specific attributes.</summary>
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        /// <summary>XML attribute value to indicate the base URI for a document or element.</summary>
        public const string XmlBaseAttributeName = "base";
        #endregion Xml constants

        #region OData constants ---------------------------------------------------------------------------------------

        /// <summary>XML namespace for data service annotations.</summary>
        public const string ODataMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>XML namespace prefix for data service annotations.</summary>
        public const string ODataMetadataNamespacePrefix = "m";

        /// <summary>XML namespace for data services.</summary>
        public const string ODataNamespace = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>OData element name for the 'value' element</summary>
        public const string ODataValueElementName = "value";

        /// <summary>Name of the error element for Xml error responses.</summary>
        public const string ODataErrorElementName = "error";

        /// <summary>Name of the error code element for Xml error responses.</summary>
        public const string ODataErrorCodeElementName = "code";

        /// <summary>Name of the error message element for Xml error responses.</summary>
        public const string ODataErrorMessageElementName = "message";

        /// <summary>Name of the inner error message element for Xml error responses.</summary>
        public const string ODataInnerErrorElementName = "innererror";

        /// <summary>Name of the message element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorMessageElementName = "message";

        /// <summary>Name of the type element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorTypeElementName = "type";

        /// <summary>Name of the stack trace element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorStackTraceElementName = "stacktrace";

        /// <summary>Name of the inner error element nested in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorInnerErrorElementName = "internalexception";
        #endregion OData constants
    }
}
