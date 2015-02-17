//---------------------------------------------------------------------
// <copyright file="ODataCollectionStart.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
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
