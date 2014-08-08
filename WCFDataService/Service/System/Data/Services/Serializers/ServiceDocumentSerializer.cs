//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Serializers
{
    #region Namespaces
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Atom;
    #endregion Namespaces

    /// <summary>
    /// Serializer for writing service document format.
    /// </summary>
    internal sealed class ServiceDocumentSerializer
    {
        /// <summary>
        /// ODataMessageWriter instance which needs to be used for writing out the response payload.
        /// </summary>
        private readonly ODataMessageWriter writer;

        /// <summary>
        /// Creates a new instance of ServiceDocumentSerializer.
        /// </summary>
        /// <param name="writer">ODataMessageWriter instance to be used for writing out the response payload.</param>
        internal ServiceDocumentSerializer(ODataMessageWriter writer)
        {
            Debug.Assert(writer != null, "writer != null");
            this.writer = writer;
        }

        /// <summary>Writes the Service Document to the output stream.</summary>
        /// <param name="provider">DataServiceProviderWrapper instance.</param>
        internal void WriteServiceDocument(DataServiceProviderWrapper provider)
        {
            ODataWorkspace odataWorkspace = new ODataWorkspace();
            odataWorkspace.Collections = provider.GetResourceSets().Select(rs =>
            {
                ODataResourceCollectionInfo resourceCollectionInfo = new ODataResourceCollectionInfo()
                {
                    Url = new Uri(rs.Name, UriKind.RelativeOrAbsolute),
                    Name = rs.Name
                };

                resourceCollectionInfo.SetAnnotation<AtomResourceCollectionMetadata>(new AtomResourceCollectionMetadata()
                {
                    Title = new AtomTextConstruct { Text = rs.Name }
                });

                return resourceCollectionInfo;
            });

            this.writer.WriteServiceDocument(odataWorkspace);
        }
    }
}
