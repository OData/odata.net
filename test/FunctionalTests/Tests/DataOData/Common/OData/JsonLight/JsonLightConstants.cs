//---------------------------------------------------------------------
// <copyright file="JsonLightConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.JsonLight
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Constants for the JSON Lite format.
    /// </summary>
    public static class JsonLightConstants
    {
        /// <summary>The prefix for OData annotation names.</summary>
        public const string ODataAnnotationNamespacePrefix = "odata.";

        /// <summary>The separator of property annotations.</summary>
        public const string ODataPropertyAnnotationSeparator = "@";

        /// <summary>The OData Context annotation name.</summary>
        public const string ODataContextAnnotationName = "odata.context";

        /// <summary>The OData Type annotation name.</summary>
        public const string ODataTypeAnnotationName = "odata.type";

        /// <summary>The OData ID annotation name.</summary>
        public const string ODataIdAnnotationName = "odata.id";

        /// <summary>The OData etag annotation name.</summary>
        public const string ODataETagAnnotationName = "odata.etag";

        /// <summary>The OData edit link annotation name.</summary>
        public const string ODataEditLinkAnnotationName = "odata.editLink";

        /// <summary>The OData read link annotation name.</summary>
        public const string ODataReadLinkAnnotationName = "odata.readLink";

        /// <summary>The OData media edit link annotation name.</summary>
        public const string ODataMediaEditLinkAnnotationName = "odata.mediaEditLink";

        /// <summary>The OData media read link annotation name.</summary>
        public const string ODataMediaReadLinkAnnotationName = "odata.mediaReadLink";

        /// <summary>The OData media content type annotation name.</summary>
        public const string ODataMediaContentTypeAnnotationName = "odata.mediaContentType";

        /// <summary>The OData media etag annotation name.</summary>
        public const string ODataMediaETagAnnotationName = "odata.mediaEtag";

        /// <summary>The OData actions annotation name.</summary>
        public const string ODataActionsAnnotationName = "odata.actions";

        /// <summary>The OData functions annotation name.</summary>
        public const string ODataFunctionsAnnotationName = "odata.functions";

        /// <summary>The 'odata.count' annotation name.</summary>
        public const string ODataCountAnnotationName = "odata.count";

        /// <summary>The 'odata.nextLink' annotation name.</summary>
        public const string ODataNextLinkAnnotationName = "odata.nextLink";

        /// <summary>The 'value' property name for the Json Light value property.</summary>
        public const string ODataValuePropertyName = "value";

        /// <summary>The 'error' property name for the Json Light value property.</summary>
        public const string ODataErrorPropertyName = "error";

        /// <summary>The name of the property returned for a URL of a workspace collection.</summary>
        public const string ODataWorkspaceCollectionUrlName = "url";

        /// <summary>The name of the property returned for a name of a workspace collection.</summary>
        public const string ODataWorkspaceCollectionNameName = "name";

        /// <summary>The 'odata.navigationLink' annotation name.</summary>
        public const string ODataNavigationLinkUrlAnnotationName = "odata.navigationLink";

        /// <summary>The 'odata.bind' annotation name.</summary>
        public const string ODataBindAnnotationName = "odata.bind";
        
        /// <summary>The 'odata.associationLink' annotation name.</summary>
        public const string ODataAssociationLinkUrlAnnotationName = "odata.associationLink";

        /// <summary>The default metadata document URI.</summary>
        public static readonly Uri DefaultMetadataDocumentUri = new Uri("http://odata.org/test/$metadata");

        /// <summary>The question mark character used as start of the query string in a URI.</summary>
        public const char ContextUriQueryOptionStartIndicator = '?';

        /// <summary>The name of the $select query option.</summary>
        public const string ContextUriSelectQueryOptionName = "$select";

        /// <summary>The '=' character used to separate a query option name from its value.</summary>
        public const char ContextUriQueryOptionValueSeparator = '=';

        /// <summary>The '&amp;' separator character between query options.</summary>
        public const char ContextUriQueryOptionSeparator = '&';

        /// <summary>The hash sign acting as fragment indicator in a context URI.</summary>
        public const char ContextUriFragmentIndicator = '#';

        /// <summary>The slash sign used as separator in the fragment of a context URI.</summary>
        public const char ContextUriFragmentPartSeparator = '/';

        /// <summary>The $entity token that indicates that the payload is a single item from a set.</summary>
        public const string ContextUriFragmentItemSelector = "$entity";

        /// <summary>The query option used to control the amount of metadata reported in JSON Light.</summary>
        public const string ControlInfoQueryOptionName = "$controlinfo";

        /// <summary>The 'none' value for the controlinfo query option.</summary>
        public const string ControlInfoNoneValue = "none";

        /// <summary>The 'default' value for the controlinfo query option.</summary>
        public const string ControlInfoDefaultValue = "default";

        /// <summary>The 'all' value for the controlinfo query option.</summary>
        public const string ControlInfoAllValue = "all";

        /// <summary>The '(' used to mark the start of Select and Expand clauses in the fragment of a context URI.</summary>
        public const char ContextUriProjectionStart = '(';

        /// <summary>The ')' used to mark the end of Select and Expand clauses in the fragment of a context URI.</summary>
        public const char ContextUriProjectionEnd = ')';
    }
}
