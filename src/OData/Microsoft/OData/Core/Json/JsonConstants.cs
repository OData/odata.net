//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces

    #endregion Namespaces

    /// <summary>
    /// Constants for the JSON format.
    /// </summary>
    internal static class JsonConstants
    {
        /// <summary>
        /// "results" header for Json data array.
        /// </summary>
        internal const string ODataResultsName = "results";

        /// <summary>
        /// Text used to start a data object wrapper in JSON.
        /// </summary>
        internal const string ODataDataWrapper = "\"d\":";

        /// <summary>
        /// Data property name of the data object wrapper in JSON.
        /// </summary>
        internal const string ODataDataWrapperPropertyName = "d";

        /// <summary>
        /// "id" header for the id of an Entry. 
        /// </summary>
        internal const string ODataEntryIdName = "id";

        /// <summary>
        /// "__metadata" header for Json metadata object
        /// </summary>
        internal const string ODataMetadataName = "__metadata";

        /// <summary>
        /// "uri" header for the URI identifying the entry being represented.
        /// </summary>
        internal const string ODataMetadataUriName = "uri";

        /// <summary>
        /// "type" header for the type name of the entity
        /// </summary>
        internal const string ODataMetadataTypeName = "type";

        /// <summary>
        /// "etag" header for the ETag of an entity
        /// </summary>
        internal const string ODataMetadataETagName = "etag";

        /// <summary>
        /// "__mediaresource" property name for the metadata of a stream reference value.
        /// </summary>
        internal const string ODataMetadataMediaResourceName = "__mediaresource";

        /// <summary>
        /// "media_src" header for the MLE read link
        /// </summary>
        internal const string ODataMetadataMediaUriName = "media_src";

        /// <summary>
        /// "content_type" header for the MLE
        /// </summary>
        internal const string ODataMetadataContentTypeName = "content_type";

        /// <summary>
        /// "media_etag" header for the MLE
        /// </summary>
        internal const string ODataMetadataMediaETagName = "media_etag";

        /// <summary>
        /// "edit_media" header for the MLE
        /// </summary>
        internal const string ODataMetadataEditMediaName = "edit_media";

        /// <summary>
        /// "properties" header for the property metadata
        /// </summary>
        internal const string ODataMetadataPropertiesName = "properties";

        /// <summary>
        /// "associationuri" header for the association link url of a navigation property
        /// </summary>
        internal const string ODataMetadataPropertiesAssociationUriName = "associationuri";

        /// <summary>
        /// "__count" header for the inline count in a feed
        /// </summary>
        internal const string ODataCountName = "__count";

        /// <summary>
        /// "__next" header for the next link in a feed
        /// </summary>
        internal const string ODataNextLinkName = "__next";

        /// <summary>
        /// "__deferred" header for the non-expanded link in an entry
        /// </summary>
        internal const string ODataDeferredName = "__deferred";

        /// <summary>"actions" header for entry metadata.</summary>
        internal const string ODataActionsMetadataName = "actions";

        /// <summary>"functions" header for entry metadata.</summary>
        internal const string ODataFunctionsMetadataName = "functions";

        /// <summary>"title" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationTitleName = "title";

        /// <summary>"metadata" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationMetadataName = "metadata";

        /// <summary>"target" header for "actions" and "functions" metadata.</summary>
        internal const string ODataOperationTargetName = "target";

        /// <summary>
        /// "error" header for the error payload
        /// </summary>
        internal const string ODataErrorName = "error";

        /// <summary>
        /// "code" header for the error code property
        /// </summary>
        internal const string ODataErrorCodeName = "code";

        /// <summary>
        /// "message" header for the error message property
        /// </summary>
        internal const string ODataErrorMessageName = "message";

        /// <summary>
        /// "innererror" header for the inner error property
        /// </summary>
        internal const string ODataErrorInnerErrorName = "innererror";

        /// <summary>
        /// "message" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorMessageName = "message";

        /// <summary>
        /// "typename" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorTypeNameName = "type";

        /// <summary>
        /// "stacktrace" header for an inner error (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorStackTraceName = "stacktrace";

        /// <summary>
        /// "internalexception" header for an inner, inner error property (for Astoria compatibility)
        /// </summary>
        internal const string ODataErrorInnerErrorInnerErrorName = "internalexception";

        /// <summary>
        /// JSON datetime format.
        /// </summary>
        internal const string ODataDateTimeFormat = @"\/Date({0})\/";

        /// <summary>
        /// JSON datetime offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetFormat = @"\/Date({0}{1}{2:D4})\/";

        /// <summary>
        /// A plus sign for the date time offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetPlusSign = "+";

        /// <summary>
        /// The fixed property name for the entity sets array in a service document payload.
        /// </summary>
        internal const string ODataServiceDocumentEntitySetsName = "EntitySets";

        /// <summary>
        /// The true value literal.
        /// </summary>
        internal const string JsonTrueLiteral = "true";

        /// <summary>
        /// The false value literal.
        /// </summary>
        internal const string JsonFalseLiteral = "false";

        /// <summary>
        /// The null value literal.
        /// </summary>
        internal const string JsonNullLiteral = "null";

        /// <summary>
        /// Character which starts the object scope.
        /// </summary>
        internal const string StartObjectScope = "{";

        /// <summary>
        /// Character which ends the object scope.
        /// </summary>
        internal const string EndObjectScope = "}";

        /// <summary>
        /// Character which starts the array scope.
        /// </summary>
        internal const string StartArrayScope = "[";

        /// <summary>
        /// Character which ends the array scope.
        /// </summary>
        internal const string EndArrayScope = "]";

        /// <summary>
        /// "(" Json Padding Function scope open parens.
        /// </summary>
        internal const string StartPaddingFunctionScope = "(";

        /// <summary>
        /// ")" Json Padding Function scope close parens.
        /// </summary>
        internal const string EndPaddingFunctionScope = ")";

        /// <summary>
        /// The separator between object members.
        /// </summary>
        internal const string ObjectMemberSeparator = ",";

        /// <summary>
        /// The separator between array elements.
        /// </summary>
        internal const string ArrayElementSeparator = ",";

        /// <summary>
        /// The separator between the name and the value.
        /// </summary>
        internal const string NameValueSeparator = ":";

        /// <summary>
        /// The quote character.
        /// </summary>
        internal const char QuoteCharacter = '"';
    }
}
