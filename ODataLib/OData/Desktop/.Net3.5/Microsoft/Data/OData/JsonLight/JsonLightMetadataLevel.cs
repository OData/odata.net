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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for logic that varies based on the JSON Light metadata level. 
    /// </summary>
    internal abstract class JsonLightMetadataLevel
    {
        /// <summary>
        /// Creates the appropriate metadata level based on the media type being written.
        /// </summary>
        /// <param name="mediaType">The full media type being written. This media type must have a type/subtype of "application/json" 
        /// and should not imply verbose json (by including "odata=verbose" as a parameter).</param>
        /// <param name="metadataDocumentUri">The metadata document uri from the writer settings.</param>
        /// <param name="model">The edm model.</param>
        /// <param name="writingResponse">true if we are writing a response, false otherwise.</param>
        /// <returns>The JSON Light metadata level being written.</returns>
        internal static JsonLightMetadataLevel Create(MediaType mediaType, Uri metadataDocumentUri, IEdmModel model, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(mediaType != null, "mediaType != null");

            Debug.Assert(
                string.Compare(mediaType.FullTypeName, MimeConstants.MimeApplicationJson, StringComparison.OrdinalIgnoreCase) == 0,
                "media type should be application/json at this point");

            if (writingResponse && mediaType.Parameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in mediaType.Parameters)
                {
                    if (!HttpUtils.CompareMediaTypeParameterNames(parameter.Key, MimeConstants.MimeODataParameterName))
                    {
                        // Only look at the "odata" parameter.
                        continue;
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeODataParameterValueMinimalMetadata, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonMinimalMetadataLevel();
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeODataParameterValueFullMetadata, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonFullMetadataLevel(metadataDocumentUri, model);
                    }

                    if (string.Compare(parameter.Value, MimeConstants.MimeODataParameterValueNoMetadata, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return new JsonNoMetadataLevel();
                    }

                    Debug.Assert(
                        string.Compare(parameter.Value, MimeConstants.MimeODataParameterValueVerbose, StringComparison.OrdinalIgnoreCase) != 0,
                        "media type should not indicate verbose json at this point.");
                }
            }

            // No "odata" media type parameter implies minimal metadata.
            return new JsonMinimalMetadataLevel();
        }

        /// <summary>
        /// Returns the oracle to use when determing the type name to write for entries and values.
        /// </summary>
        /// <param name="autoComputePayloadMetadataInJson">
        /// If true, the type name to write will vary based on the metadata level. 
        /// If false, the type name writing rules will always match minimal metadata, 
        /// regardless of the actual metadata level being written. 
        /// This is for backwards compatibility.
        /// </param>
        /// <returns>An oracle that can be queried to determine the type name to write.</returns>
        internal abstract JsonLightTypeNameOracle GetTypeNameOracle(bool autoComputePayloadMetadataInJson);

        /// <summary>
        /// Indicates whether the "odata.metadata" URI should be written based on the current metadata level.
        /// </summary>
        /// <returns>true if the metadata URI should be written, false otherwise.</returns>
        internal abstract bool ShouldWriteODataMetadataUri();

        /// <summary>
        /// Creates the metadata builder for the given entry. If such a builder is set, asking for payload
        /// metadata properties (like EditLink) of the entry may return a value computed by convention, 
        /// depending on the metadata level and whether the user manually set an edit link or not.
        /// </summary>
        /// <param name="entry">The entry to create the metadata builder for.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
        /// <param name="serializationInfo">The serialization info for the entry.</param>
        /// <param name="actualEntityType">The entity type of the entry.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <param name="isResponse">true if the entity metadata builder to create should be for a response payload; false for a request.</param>
        /// <param name="keyAsSegment">true if keys should go in seperate segments in auto-generated URIs, false if they should go in parentheses.
        /// A null value means the user hasn't specified a preference and we should look for an annotation in the entity container, if available.</param>
        /// <returns>The created metadata builder.</returns>
        internal abstract ODataEntityMetadataBuilder CreateEntityMetadataBuilder(
            ODataEntry entry, 
            IODataFeedAndEntryTypeContext typeContext,
            ODataFeedAndEntrySerializationInfo serializationInfo, 
            IEdmEntityType actualEntityType, 
            SelectedPropertiesNode selectedProperties, 
            bool isResponse, 
            bool? keyAsSegment);

        /// <summary>
        /// Injects the appropriate metadata builder based on the metadata level.
        /// </summary>
        /// <param name="entry">The entry to inject the builder.</param>
        /// <param name="builder">The metadata builder to inject.</param>
        internal abstract void InjectMetadataBuilder(ODataEntry entry, ODataEntityMetadataBuilder builder);
    }
}
