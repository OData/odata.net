//---------------------------------------------------------------------
// <copyright file="JsonConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    /// <summary>
    /// Constants for use in JSON format
    /// </summary>
    public static class JsonConstants
    {
        /// <summary>The name of the metadata property.</summary>
        public const string ODataMetadataPropertyName = "__metadata";

        /// <summary>The name of the media source property.</summary>
        public const string ODataMetadataMediaSourcePropertyName = "media_src";

        /// <summary>The name of the content type property.</summary>
        public const string ODataMetadataContentTypePropertyName = "content_type";

        /// <summary>The name of the edit media property.</summary>
        public const string ODataMetadataEditMediaPropertyName = "edit_media";

        /// <summary>The name of the media etag property.</summary>
        public const string ODataMetadataMediaETagPropertyName = "media_etag";

        /// <summary>"properties" header for the property metadata</summary>
        public const string ODataMetadataPropertiesName = "properties";

        /// <summary>"associationuri" header for the association link url of a navigation property</summary>
        public const string ODataMetadataPropertiesAssociationUriName = "associationuri";

        /// <summary>"actions" header for entry metadata.</summary>
        public const string ODataActionsMetadataName = "actions";

        /// <summary>"functions" header for entry metadata.</summary>
        public const string ODataFunctionsMetadataName = "functions";

        /// <summary>"title" header for "actions" and "functions" metadata.</summary>
        public const string ODataOperationTitleName = "title";

        /// <summary>"metadata" header for "actions" and "functions" metadata.</summary>
        public const string ODataOperationMetadataName = "metadata";

        /// <summary>"target" header for "actions" and "functions" metadata.</summary>
        public const string ODataOperationTargetName = "target";

        /// <summary>"error" header for the error payload</summary>
        public const string ODataErrorName = "error";

        /// <summary>"code" header for the error code property</summary>
        public const string ODataErrorCodeName = "code";

        /// <summary>"message" header for the error message property</summary>
        public const string ODataErrorMessageName = "message";

        /// <summary>"innererror" header for the inner error property</summary>
        public const string ODataErrorInnerErrorName = "innererror";

        /// <summary>"message" header for an inner error (for Astoria compatibility)</summary>
        public const string ODataErrorInnerErrorMessageName = "message";

        /// <summary>"typename" header for an inner error (for Astoria compatibility)</summary>
        public const string ODataErrorInnerErrorTypeNameName = "type";

        /// <summary>"stacktrace" header for an inner error (for Astoria compatibility)</summary>
        public const string ODataErrorInnerErrorStackTraceName = "stacktrace";

        /// <summary>"internalexception" header for an inner, inner error property (for Astoria compatibility)</summary>
        public const string ODataErrorInnerErrorInnerErrorName = "internalexception";
    }
}
