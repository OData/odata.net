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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for service documents.
    /// </summary>
    internal sealed class ODataJsonLightServiceDocumentSerializer : ODataJsonLightSerializer
    {
        /// <summary>The metadata uri builder to use.</summary>
        private readonly ODataJsonLightMetadataUriBuilder metadataUriBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightServiceDocumentSerializer(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            // DEVNOTE: grab this early so that any validation errors are thrown at creation time rather than when Write___ is called.
            this.metadataUriBuilder = jsonLightOutputContext.CreateMetadataUriBuilder();
        }

        /// <summary>
        /// Writes a service document in JsonLight format.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        internal void WriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(defaultWorkspace != null, "defaultWorkspace != null");

            IEnumerable<ODataResourceCollectionInfo> collections = defaultWorkspace.Collections;

            this.WriteTopLevelPayload(
                () =>
                {
                    // "{"
                    this.JsonWriter.StartObjectScope();

                    // "odata.metadata":...
                    Uri metadataUri;
                    if (this.metadataUriBuilder.TryBuildServiceDocumentMetadataUri(out metadataUri))
                    {
                        this.WriteMetadataUriProperty(metadataUri);
                    }

                    // "value":
                    this.JsonWriter.WriteValuePropertyName();

                    // "["
                    this.JsonWriter.StartArrayScope();

                    if (collections != null)
                    {
                        foreach (ODataResourceCollectionInfo collectionInfo in collections)
                        {
                            // validate that the collection has a non-null url.
                            ValidationUtils.ValidateResourceCollectionInfo(collectionInfo);

                            if (string.IsNullOrEmpty(collectionInfo.Name))
                            {
                                throw new ODataException(Strings.ODataJsonLightServiceDocumentSerializer_ResourceCollectionMustSpecifyName);
                            }

                            // "{"
                            this.JsonWriter.StartObjectScope();

                            // "name": ...
                            this.JsonWriter.WriteName(JsonLightConstants.ODataWorkspaceCollectionNameName);
                            this.JsonWriter.WriteValue(collectionInfo.Name);

                            // "url": ...
                            this.JsonWriter.WriteName(JsonLightConstants.ODataWorkspaceCollectionUrlName);
                            this.JsonWriter.WriteValue(this.UriToString(collectionInfo.Url));

                            // "}"
                            this.JsonWriter.EndObjectScope();
                        }
                    }

                    // "]"
                    this.JsonWriter.EndArrayScope();

                    // "}"
                    this.JsonWriter.EndObjectScope();
                });
        }
    }
}
