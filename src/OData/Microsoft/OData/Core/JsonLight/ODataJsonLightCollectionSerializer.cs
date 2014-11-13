//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// OData JsonLight serializer for collections.
    /// </summary>
    internal sealed class ODataJsonLightCollectionSerializer : ODataJsonLightValueSerializer
    {
        /// <summary>true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</summary>
        private readonly bool writingTopLevelCollection;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="writingTopLevelCollection">true when writing a top-level collection that requires the 'value' wrapper object; otherwise false.</param>
        internal ODataJsonLightCollectionSerializer(ODataJsonLightOutputContext jsonLightOutputContext, bool writingTopLevelCollection)
            : base(jsonLightOutputContext, /*initContextUriBuilder*/true)
        {
            this.writingTopLevelCollection = writingTopLevelCollection;
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        /// <param name="collectionStart">The collection start to write.</param>
        /// <param name="itemTypeReference">The item type of the collection or null if no metadata is available.</param>
        internal void WriteCollectionStart(ODataCollectionStart collectionStart, IEdmTypeReference itemTypeReference)
        {
            Debug.Assert(collectionStart != null, "collectionStart != null");

            if (this.writingTopLevelCollection)
            {
                // "{"
                this.JsonWriter.StartObjectScope();

                // "@odata.context":...
                this.WriteContextUriProperty(ODataPayloadKind.Collection, () => ODataContextUrlInfo.Create(collectionStart.SerializationInfo, itemTypeReference));

                // "value":
                this.JsonWriter.WriteValuePropertyName();
            }

            // Write the start of the array for the collection items
            // "["
            this.JsonWriter.StartArrayScope();
        }

        /// <summary>
        /// Writes the end of a collection.
        /// </summary>
        internal void WriteCollectionEnd()
        {
            // Write the end of the array for the collection items
            // "]"
            this.JsonWriter.EndArrayScope();

            if (this.writingTopLevelCollection)
            {
                // "}"
                this.JsonWriter.EndObjectScope();
            }
        }
    }
}
