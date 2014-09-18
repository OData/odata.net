//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for collections.
    /// </summary>
    internal sealed class ODataAtomCollectionSerializer : ODataAtomPropertyAndValueSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomCollectionSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
        }

        /// <summary>
        /// Writes the start of a collection.
        /// </summary>
        internal void WriteCollectionStart()
        {
            // <m:value>
            this.XmlWriter.WriteStartElement(AtomConstants.ODataMetadataNamespacePrefix, AtomConstants.ODataValueElementName, AtomConstants.ODataMetadataNamespace);

            this.WriteDefaultNamespaceAttributes(
                ODataAtomSerializer.DefaultNamespaceFlags.OData |
                ODataAtomSerializer.DefaultNamespaceFlags.Gml |
                ODataAtomSerializer.DefaultNamespaceFlags.GeoRss);
        }
    }
}
