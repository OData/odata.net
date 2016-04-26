//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartSerializationInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Class to provide additional serialization information to the <see cref="ODataCollectionWriter"/> for an <see cref="ODataCollectionStart"/>.
    /// </summary>
    public sealed class ODataCollectionStartSerializationInfo
    {
        /// <summary>
        /// The fully qualified type name of the collection to be written.
        /// </summary>
        private string collectionTypeName;

        /// <summary>
        /// The fully qualified type name of the collection to be written.
        /// </summary>
        public string CollectionTypeName
        {
            get
            {
                return this.collectionTypeName;
            }

            set
            {
                ExceptionUtils.CheckArgumentStringNotNullOrEmpty(value, "CollectionTypeName");
                ValidationUtils.ValidateCollectionTypeName(value);
                this.collectionTypeName = value;
            }
        }

        /// <summary>
        /// Validates the <paramref name="serializationInfo"/> instance.
        /// </summary>
        /// <param name="serializationInfo">The serialization info instance to validate.</param>
        /// <returns>The <paramref name="serializationInfo"/> instance.</returns>
        internal static ODataCollectionStartSerializationInfo Validate(ODataCollectionStartSerializationInfo serializationInfo)
        {
            if (serializationInfo != null)
            {
                ExceptionUtils.CheckArgumentNotNull(serializationInfo.CollectionTypeName, "serializationInfo.CollectionTypeName");
            }

            return serializationInfo;
        }
    }
}