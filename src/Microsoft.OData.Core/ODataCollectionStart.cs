//---------------------------------------------------------------------
// <copyright file="ODataCollectionStart.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// OData representation of a top-level collection.
    /// </summary>
    public sealed class ODataCollectionStart : ODataAnnotatable
    {
        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataCollectionWriter"/> for this <see cref="ODataCollectionStart"/>.
        /// </summary>
        private ODataCollectionStartSerializationInfo serializationInfo;

        /// <summary>Gets or sets the name of the collection (ATOM only).</summary>
        /// <returns>The name of the collection.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets the number of items in the collection.</summary>
        /// <returns>The number of items in the collection.</returns>
        public long? Count
        {
            get;
            set;
        }

        /// <summary>Gets the URI representing the next page link.</summary>
        /// <returns>The URI representing the next page link.</returns>
        public Uri NextPageLink
        {
            get;
            set;
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataCollectionWriter"/> for this <see cref="ODataCollectionStart"/>.
        /// </summary>
        internal ODataCollectionStartSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataCollectionStartSerializationInfo.Validate(value);
            }
        }
    }
}
