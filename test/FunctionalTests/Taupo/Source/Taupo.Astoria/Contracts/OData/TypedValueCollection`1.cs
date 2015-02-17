//---------------------------------------------------------------------
// <copyright file="TypedValueCollection`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Abstract class for representing a collection of typed values which itself has a type
    /// </summary>
    /// <typeparam name="TElement">The type of the elements</typeparam>
    public abstract class TypedValueCollection<TElement> : ODataPayloadElementCollection<TElement>, ITypedValue
        where TElement : ODataPayloadElement, ITypedValue
    {
        /// <summary>
        /// Initializes a new instance of the TypedValueCollection class
        /// </summary>
        protected TypedValueCollection()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TypedValueCollection class
        /// </summary>
        /// <param name="fullTypeName">The full type name</param>
        /// <param name="isNull">Whether or not the value is null</param>
        /// <param name="list">the initial contents of the collection</param>
        protected TypedValueCollection(string fullTypeName, bool isNull, params TElement[] list)
            : base(list)
        {
            this.FullTypeName = fullTypeName;
            this.IsNull = isNull;
        }

        /// <summary>
        /// Gets or sets the Type Name of the collection
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the value is null
        /// </summary>
        public bool IsNull { get; set; }
    }
}
