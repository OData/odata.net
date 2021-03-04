//---------------------------------------------------------------------
// <copyright file="RefOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Asynchronous methods do not support ref and out parameters.
    /// This class provides a workaround to enable passing around
    /// of the value in an object.
    /// </summary>
    /// <typeparam name="T">The type of object wrapped in the object.</typeparam>
    internal sealed class Ref<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class.
        /// </summary>
        public Ref() : this(default(T)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class with value to be wrapped.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        public Ref(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value wrapped in the object.
        /// </summary>
        public T Value { get; set; }
    }
}
