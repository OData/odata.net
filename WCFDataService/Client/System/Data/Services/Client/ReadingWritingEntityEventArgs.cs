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

namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Xml.Linq;

    /// <summary>
    /// Event args for the event fired during reading or writing of
    /// an entity serialization/deserialization
    /// </summary>
    public sealed class ReadingWritingEntityEventArgs : EventArgs
    {
        /// <summary>The entity being (de)serialized</summary>
        private object entity;

        /// <summary>The ATOM entry data to/from the network</summary>
        private XElement data;

        /// <summary>The xml base of the feed or entry containing the current ATOM entry</summary>
        private Uri baseUri;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entity">The entity being (de)serialized</param>
        /// <param name="data">The ATOM entry data to/from the network</param>
        /// <param name="baseUri">The xml base of the feed or entry containing the current ATOM entry</param>
        internal ReadingWritingEntityEventArgs(object entity, XElement data, Uri baseUri)
        {
            Debug.Assert(entity != null, "entity != null");
            Debug.Assert(data != null, "data != null");
            Debug.Assert(baseUri == null || baseUri.IsAbsoluteUri, "baseUri == null || baseUri.IsAbsoluteUri");

            this.entity = entity;
            this.data = data;
            this.baseUri = baseUri;
        }

        /// <summary>Gets the object representation of data returned from the <see cref="P:System.Data.Services.Client.ReadingWritingEntityEventArgs.Data" /> property. </summary>
        /// <returns><see cref="T:System.Object" /> representation of the <see cref="P:System.Data.Services.Client.ReadingWritingEntityEventArgs.Data" /> property.</returns>
        public object Entity
        {
            get { return this.entity; }
        }

        /// <summary>Gets an entry or feed data represented as an <see cref="T:System.Xml.Linq.XElement" />.</summary>
        /// <returns>
        ///   <see cref="T:System.Xml.Linq.XElement" />
        /// </returns>
        public XElement Data
        {
            [DebuggerStepThrough]
            get { return this.data; }
        }

        /// <summary>Gets the base URI base of the entry or feed.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.</returns>
        public Uri BaseUri
        {
            [DebuggerStepThrough]
            get { return this.baseUri; }
        }
    }
}
