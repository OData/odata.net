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
