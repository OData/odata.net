//---------------------------------------------------------------------
// <copyright file="ReadingWritingEntityEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
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

        /// <summary>Gets the object representation of data returned from the <see cref="Microsoft.OData.Client.ReadingWritingEntityEventArgs.Data" /> property. </summary>
        /// <returns><see cref="System.Object" /> representation of the <see cref="Microsoft.OData.Client.ReadingWritingEntityEventArgs.Data" /> property.</returns>
        public object Entity
        {
            get { return this.entity; }
        }

        /// <summary>Gets an entry or feed data represented as an <see cref="System.Xml.Linq.XElement" />.</summary>
        /// <returns>
        ///   <see cref="System.Xml.Linq.XElement" />
        /// </returns>
        public XElement Data
        {
            [DebuggerStepThrough]
            get { return this.data; }
        }

        /// <summary>Gets the base URI base of the entry or feed.</summary>
        /// <returns>Returns <see cref="System.Uri" />.</returns>
        public Uri BaseUri
        {
            [DebuggerStepThrough]
            get { return this.baseUri; }
        }
    }
}