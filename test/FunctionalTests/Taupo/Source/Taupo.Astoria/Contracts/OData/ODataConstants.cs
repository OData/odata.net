//---------------------------------------------------------------------
// <copyright file="ODataConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    /// <summary>
    /// Set of constants specific to OData or Atom
    /// </summary>
    public static class ODataConstants
    {
        /// <summary>
        /// Constant for "http://www.w3.org/2005/Atom"
        /// </summary>
        public const string AtomNamespaceName = "http://www.w3.org/2005/Atom";

        /// <summary>
        /// Constant for "http://www.w3.org/2007/app"
        /// </summary>
        public const string AtomPubNamespaceName = "http://www.w3.org/2007/app";

        /// <summary>
        /// Constant prefix for "http://www.w3.org/2007/app".
        /// </summary>
        public const string AtomPubNamespacePrefix = "app";

        /// <summary>
        /// Constant for "http://www.w3.org/XML/1998/namespace"
        /// </summary>
        public const string XmlNamespaceName = "http://www.w3.org/XML/1998/namespace";

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/data"
        /// </summary>
        public const string DataServicesNamespaceName = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>
        /// Constant for the default prefix for "http://docs.oasis-open.org/odata/ns/data" which is "d"
        /// </summary>
        public const string DataServicesNamespaceDefaultPrefix = "d";

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/metadata"
        /// </summary>
        public const string DataServicesMetadataNamespaceName = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>
        /// Constant for the default prefix for "http://docs.oasis-open.org/odata/ns/metadata" which is "m"
        /// </summary>
        public const string DataServicesMetadataNamespaceDefaultPrefix = "m";

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/related/"
        /// </summary>
        public const string DataServicesRelatedNamespaceName = "http://docs.oasis-open.org/odata/ns/related/"; // MUST END IN SLASH

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/relatedlinks/"
        /// </summary>
        public const string DataServicesRelatedLinksNamespaceName = "http://docs.oasis-open.org/odata/ns/relatedlinks/"; // MUST END IN SLASH

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/scheme"
        /// </summary>
        public const string DataServicesSchemeNamespaceName = "http://docs.oasis-open.org/odata/ns/scheme";

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/mediaresource/"
        /// </summary>
        public const string DataServicesMediaResourceNamespaceName = "http://docs.oasis-open.org/odata/ns/mediaresource/"; // MUST END IN SLASH

        /// <summary>
        /// Constant for "http://docs.oasis-open.org/odata/ns/edit-media/"
        /// </summary>
        public const string DataServicesMediaResourceEditNamespaceName = "http://docs.oasis-open.org/odata/ns/edit-media/"; // MUST END IN SLASH

        /// <summary>
        /// Constant for "MultiValue(" which marks the start of a multi-value type name
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public const string BeginMultiValueTypeIdentifier = "Collection(";

        /// <summary>
        /// Constant for ")" which marks the end of a multi-value type name
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public const string EndMultiValueTypeNameIdentifier = ")";

        /// <summary>
        /// Constant for the 'null' attribute name in xml payloads
        /// </summary>
        public const string NullAttributeName = "null";

        /// <summary>
        /// Constant for the 'type' attribute name in xml payloads
        /// </summary>
        public const string TypeAttributeName = "type";

        /// <summary>
        /// Constant for the 'author' element name in atom payloads
        /// </summary>
        public const string AuthorElementName = "author";

        /// <summary>
        /// Constant for the 'title' element name in atom payloads
        /// </summary>
        public const string TitleElementName = "title";

        /// <summary>
        /// Constant for the 'updated' element name in atom payloads
        /// </summary>
        public const string UpdatedElementName = "updated";

        /// <summary>
        /// Constant for the 'summary' element name in atom payloads
        /// </summary>
        public const string SummaryElementName = "summary";

        /// <summary>
        /// Constant for the 'link' element name in atom payloads
        /// </summary>
        public const string LinkElementName = "link";

        /// <summary>
        /// Constant for the 'rel' attribute name in atom payloads
        /// </summary>
        public const string RelAttributeName = "rel";

        /// <summary>
        /// Constant for the 'scheme' attribute name in atom payloads
        /// </summary>
        public const string SchemeAttributeName = "scheme";

        /// <summary>
        /// Constant for the 'term' attribute name in atom payloads
        /// </summary>
        public const string TermAttributeName = "term";

        /// <summary>
        /// Constant for the 'category' element name in atom payloads
        /// </summary>
        public const string CategoryElementName = "category";

        /// <summary>
        /// Constant for the 'name' element name in atom payloads
        /// </summary>
        public const string NameElementName = "name";

        /// <summary>
        /// Constant for the 'contributor' element name in atom payloads
        /// </summary>
        public const string ContributorElementName = "contributor";           

        /// <summary>
        /// Constant for the 'email' element name in atom payloads
        /// </summary>
        public const string EmailElementName = "email";

        /// <summary> 
        /// Constant for the 'FC_SourcePath' attribute name in CSDL
        /// </summary>
        public const string SourcePathAttribute = "FC_SourcePath";

        /// <summary>
        /// Constant for the 'FC_TargetPath' attribute name in CSDL
        /// </summary>
        public const string TargetPathAttribute = "FC_TargetPath";

        /// <summary> 
        /// Constant for the 'FC_ContentKind' attribute name in CSDL
        /// </summary>
        public const string ContentKindAttribute = "FC_ContentKind";

        /// <summary> 
        /// Constant for the 'FC_KeepInContent' attribute name in CSDL
        /// </summary>
        public const string KeepInContentAttribute = "FC_KeepInContent";

        /// <summary>
        /// Constant for the 'FC_NsUri' attribute name in CSDL
        /// </summary>
        public const string NSUriAttribute = "FC_NsUri";

        /// <summary> 
        /// Constant for the 'FC_NsPrefix' attribute name in CSDL
        /// </summary>
        public const string NSPrefixAttribute = "FC_NsPrefix";

        /// <summary> 
        /// Constant for the 'Syndication' attribute value prefix in CSDL
        /// </summary>
        public const string EpmAttributePrefix = "Syndication";

        /// <summary>
        /// Constant for the 'HasStream' attribute name
        /// </summary>
        public const string HasStreamAttributeName = "HasStream";

        /// <summary>
        /// Constant for the 'href' attribute name in atom payloads
        /// </summary>
        public const string HrefAttributeName = "href";

        /// <summary>
        /// Constant for the 'inline' element name in atom payloads
        /// </summary>
        public const string InlineElementName = "inline";

        /// <summary>
        /// Constant for the 'service' element name in atom service documents
        /// </summary>
        public const string ServiceElementName = "service";

        /// <summary>
        /// Constant for the 'workspace' element name in atom service documents
        /// </summary>
        public const string WorkspaceElementName = "workspace";

        /// <summary>
        /// Constant for the 'collection' element name in atom service documents
        /// </summary>
        public const string CollectionElementName = "collection";

        /// <summary>
        /// Constant for the 'base' attribute name in xml documents
        /// </summary>
        public const string XmlBaseAttributeName = "base";

        /// <summary>
        /// Constant for the 'error' element name in error payloads
        /// </summary>
        public const string ErrorElementName = "error";

        /// <summary>
        /// Constant for the 'innererror' element name in error payloads
        /// </summary>
        public const string InnerErrorElementName = "innererror";

        /// <summary>
        /// Constant for the 'code' element name in error payloads
        /// </summary>
        public const string CodeElementName = "code";

        /// <summary>
        /// Constant for the 'message' element name in in-stream errors
        /// </summary>
        public const string MessageElementName = "message";

        /// <summary>
        /// Constant for the 'value' element name in json error payloads
        /// </summary>
        public const string ValueElementName = "value";

        /// <summary>
        /// Constant for the 'typename' element name in error payloads
        /// </summary>
        public const string TypeNameElementName = "type";

        /// <summary>
        /// Constant for the 'stacktrace' element name in error payloads
        /// </summary>
        public const string StackTraceElementName = "stacktrace";

        /// <summary>
        /// Constant for the 'internalexception' element name in error payloads
        /// </summary>
        public const string InternalExceptionElementName = "internalexception";

        /// <summary>
        /// Constant for the 'element' element name in collections and multi-values
        /// </summary>
        public const string CollectionItemElementName = "element";

        /// <summary>
        /// Constant for the 'all' operator in OData uris
        /// </summary>
        public const string AllUriOperator = "all";  

        /// <summary>
        /// Constant for the 'any' operator in OData uris
        /// </summary>
        public const string AnyUriOperator = "any";

        /// <summary>
        /// Constant for the '$it', the implicit outer variable name in any/all filters
        /// </summary>
        public const string ImplicitOuterVariableName = "$it";

        /// <summary>
        /// Constant for the '__metadata' property name in JSON
        /// </summary>
        public const string JsonMetadataPropertyName = "__metadata";

        /// <summary>The default maximum allowed recursion depth for recursive payload definitions, such as complex values inside complex values.</summary>
        public const int DefaultMaxRecursionDepth = 100;

        /// <summary>
        /// Initializes static members of the ODataConstants class.
        /// </summary>
        static ODataConstants()
        {
            CollectionElement = XName.Get(CollectionElementName, AtomPubNamespaceName);
            HrefAttribute = XName.Get(HrefAttributeName);
            ServiceElement = XName.Get(ServiceElementName, AtomPubNamespaceName);
            TitleElement = XName.Get(TitleElementName, AtomNamespaceName);
            InlineElement = XName.Get(InlineElementName, DataServicesMetadataNamespaceName);
            WorkspaceElement = XName.Get(WorkspaceElementName, AtomPubNamespaceName);
            XmlBaseAttribute = XName.Get(XmlBaseAttributeName, XmlNamespaceName);
            AtomNamespace = XNamespace.Get(AtomNamespaceName);
            DataServicesNamespace = XNamespace.Get(DataServicesNamespaceName);
            DataServicesMetadataNamespace = XNamespace.Get(DataServicesMetadataNamespaceName);
            ErrorElement = XName.Get(ErrorElementName, DataServicesMetadataNamespaceName);

            DateTimeTicksForEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            TicksPerMillisecond = new TimeSpan(0, 0, 0, 0, 1).Ticks;

            JsonDateTimeTickFormatRegex = new Regex(@"^/Date\((?<ticks>-?[0-9]+)\)/");
            JsonDateTimeOffsetTickFormatRegex = new Regex(@"^/Date\((?<ticks>-?[0-9]+)(?<offset>[+-][0-9]{1,4})\)/");
        }

        /// <summary>
        /// Gets an XName for the 'm:error' element
        /// </summary>
        public static XName ErrorElement { get; private set; }

        /// <summary>
        /// Gets an XName for the 'atom:title' element
        /// </summary>
        public static XName TitleElement { get; private set; }

        /// <summary>
        /// Gets an XName for the 'href' attribute
        /// </summary>
        public static XName HrefAttribute { get; private set; }

        /// <summary>
        /// Gets an XName for the 'xml:base' attribute
        /// </summary>
        public static XName XmlBaseAttribute { get; private set; }

        /// <summary>
        /// Gets an XName for the 'm:inline' element
        /// </summary>
        public static XName InlineElement { get; private set; }

        /// <summary>
        /// Gets an XName for the 'service' element in ATOM Service document
        /// </summary>
        public static XName ServiceElement { get; private set; }

        /// <summary>
        /// Gets an XName for the 'workspace' element in ATOM Service document
        /// </summary>
        public static XName WorkspaceElement { get; private set; }

        /// <summary>
        /// Gets an XName for the 'collection' element in ATOM Service document
        /// </summary>
        public static XName CollectionElement { get; private set; }

        /// <summary>
        /// Gets an XNamespace for the atom namespace
        /// </summary>
        public static XNamespace AtomNamespace { get; private set; }

        /// <summary>
        /// Gets an XNamespace for the data services namespace
        /// </summary>
        public static XNamespace DataServicesNamespace { get; private set; }

        /// <summary>
        /// Gets an XNamespace for the data services metadata namespace
        /// </summary>
        public static XNamespace DataServicesMetadataNamespace { get; private set; }

        /// <summary>
        /// Gets the the datetime tick-count for midnight on January 1st, 1970 UTC
        /// </summary>
        public static long DateTimeTicksForEpoch { get; private set; }

        /// <summary>
        /// Gets the number of ticks in 1 millisecond
        /// </summary>
        public static long TicksPerMillisecond { get; private set; }

        /// <summary>
        /// Gets the regular-expression for the json tick-based datetime format
        /// </summary>
        public static Regex JsonDateTimeTickFormatRegex { get; private set; }

        /// <summary>
        /// Gets the regular-expression for the json tick-based datetime-offset format
        /// </summary>
        public static Regex JsonDateTimeOffsetTickFormatRegex { get; private set; }
    }
}
