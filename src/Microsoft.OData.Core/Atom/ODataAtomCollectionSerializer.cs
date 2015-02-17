//---------------------------------------------------------------------
// <copyright file="ODataAtomCollectionSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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