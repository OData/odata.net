//---------------------------------------------------------------------
// <copyright file="ClientEdmCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Implementation of <see cref="IEdmCollectionValue"/> which wraps client-side objects.
    /// </summary>
    internal sealed class ClientEdmCollectionValue : IEdmCollectionValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEdmCollectionValue"/> class.
        /// </summary>
        /// <param name="type">The type of the collection.</param>
        /// <param name="elements">The elements of the collection.</param>
        public ClientEdmCollectionValue(IEdmTypeReference type, IEnumerable<IEdmValue> elements)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(elements != null, "values != null");

            this.Type = type;
            this.Elements = elements.Select(v => (IEdmDelayedValue)new NullEdmDelayedValue(v));
        }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.Collection; }
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements { get; private set; }

        /// <summary>
        /// Non-delayed implementation of <see cref="IEdmDelayedValue"/>
        /// </summary>
        private class NullEdmDelayedValue : IEdmDelayedValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NullEdmDelayedValue"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public NullEdmDelayedValue(IEdmValue value)
            {
                this.Value = value;
            }

            /// <summary>
            /// Gets the data stored in this value.
            /// </summary>
            public IEdmValue Value { get; private set; }
        }
    }
}