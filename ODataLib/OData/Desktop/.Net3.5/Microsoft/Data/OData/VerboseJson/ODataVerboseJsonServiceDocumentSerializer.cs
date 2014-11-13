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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// OData Verbose JSON serializer for service documents.
    /// </summary>
    internal sealed class ODataVerboseJsonServiceDocumentSerializer : ODataVerboseJsonSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="verboseJsonOutputContext">The output context to write to.</param>
        internal ODataVerboseJsonServiceDocumentSerializer(ODataVerboseJsonOutputContext verboseJsonOutputContext)
            : base(verboseJsonOutputContext)
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
