//---------------------------------------------------------------------
// <copyright file="EdmCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM collection value.
    /// </summary>
    public class EdmCollectionValue : EdmValue, IEdmCollectionValue
    {
        private readonly IEnumerable<IEdmDelayedValue> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionValue"/> class.
        /// </summary>
        /// <param name="type">A reference to a collection type that describes this collection value</param>
        /// <param name="elements">The collection of values stored in this collection value</param>
        public EdmCollectionValue(IEdmCollectionTypeReference type, IEnumerable<IEdmDelayedValue> elements)
            : base(type)
        {
            this.elements = elements;
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Collection; }
        }
    }
}
