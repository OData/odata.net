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
