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
