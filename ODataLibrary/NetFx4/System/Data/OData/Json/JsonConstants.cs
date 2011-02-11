//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Json
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
        internal const string ODataDataWrapper = "\"d\" : ";

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
        /// "__associationuri" header for the association link url of a navigation property
        /// </summary>
        internal const string ODataMetadataPropertiesAssociationUriName = "__associationuri";

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

        /// <summary>
        /// "uri" header for the link URL
        /// </summary>
        internal const string ODataLinkUriName = "uri";

        /// <summary>
        /// The name of the property returned for a singleton $links query
        /// </summary>
        internal const string ODataUriName = "uri";

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
        /// "lang" header for the error message language property
        /// </summary>
        internal const string ODataErrorMessageLanguageName = "lang";

        /// <summary>
        /// "value" header for the error message value property
        /// </summary>
        internal const string ODataErrorMessageValueName = "value";

        /// <summary>
        /// "innererror" header for the inner error property
        /// </summary>
        internal const string ODataErrorInnerErrorName = "innererror";

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
        /// The separator between object members.
        /// </summary>
        internal const string ObjectMemberSeparator = ", ";

        /// <summary>
        /// The separator between array elements.
        /// </summary>
        internal const string ArrayElementSeparator = ", ";

        /// <summary>
        /// The separator between the name and the value.
        /// </summary>
        internal const string NameValueSeparator = ": ";

        /// <summary>
        /// The quote character.
        /// </summary>
        internal const char QuoteCharacter = '"';
    }
}
