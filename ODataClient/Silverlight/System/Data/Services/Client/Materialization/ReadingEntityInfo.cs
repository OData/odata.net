//---------------------------------------------------------------------
// <copyright file="ReadingEntityInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Client.Materialization
{
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Data.OData;

    /// <summary>
    /// To cache the entity instance as annotation for firing ReadingEntity event
    /// </summary>
    internal sealed class ReadingEntityInfo
    {
        /// <summary>XDocument instance to cache the payload.</summary>
        internal readonly XElement EntryPayload;

        /// <summary>BaseUri for the entry payload.</summary>
        internal readonly Uri BaseUri;

        /// <summary>
        /// Creates a new instance of ReadingEntityInfo
        /// </summary>
        /// <param name="payload">XElement containing the entry payload.</param>
        /// <param name="uri">base uri for the entry payload.</param>
        internal ReadingEntityInfo(XElement payload, Uri uri)
        {
            Debug.Assert(payload != null, "payload != null");
            this.EntryPayload = payload;
            this.BaseUri = uri;
        }

        /// <summary>
        /// Returns the new XmlReader to cache the payload for firing ReadingEntity event.
        /// </summary>
        /// <param name="entry">ODataEntry instance that is currently getting deserialized.</param>
        /// <param name="entryReader">XmlReader that is used to read the payload.</param>
        /// <param name="baseUri">BaseUri for the entry payload.</param>
        /// <returns>XmlReader instance that needs to be used to read the payload for the given odataentry.</returns>
        internal static XmlReader BufferAndCacheEntryPayload(ODataEntry entry, XmlReader entryReader, Uri baseUri)
        {
            XElement entryPayload = XElement.Load(entryReader.ReadSubtree(), LoadOptions.None);

            // Move the parent entry reader after the end element of the entry.
            entryReader.Read();

            entry.SetAnnotation(new ReadingEntityInfo(entryPayload, baseUri));
            XmlReader xmlReader = entryPayload.CreateReader();
            xmlReader.Read();
            return xmlReader;
        }
    }
}
