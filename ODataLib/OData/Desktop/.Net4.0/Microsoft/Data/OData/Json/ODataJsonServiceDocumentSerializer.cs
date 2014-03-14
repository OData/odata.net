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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// OData JSON serializer for service documents.
    /// </summary>
    internal sealed class ODataJsonServiceDocumentSerializer : ODataJsonSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonOutputContext">The output context to write to.</param>
        internal ODataJsonServiceDocumentSerializer(ODataJsonOutputContext jsonOutputContext)
            : base(jsonOutputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes a service document in JSON format.
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

                    // "EntitySets":
                    this.JsonWriter.WriteName(JsonConstants.ODataServiceDocumentEntitySetsName);

                    // "["
                    this.JsonWriter.StartArrayScope();

                    if (collections != null)
                    {
                        foreach (ODataResourceCollectionInfo collectionInfo in collections)
                        {
                            // validate that the collection has a non-null url.
                            ValidationUtils.ValidateResourceCollectionInfo(collectionInfo);

                            // Note that this is an exception case; if the Base URI is missing we will still write the relative URI.
                            // We allow this because collections are specified to be the entity set names in JSON and 
                            // there is no base Uri in JSON.
                            this.JsonWriter.WriteValue(UriUtilsCommon.UriToString(collectionInfo.Url));
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
