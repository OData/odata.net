//---------------------------------------------------------------------
// <copyright file="EdmValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM value.
    /// </summary>
    public abstract class EdmValue : IEdmValue, IEdmDelayedValue
    {
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the EdmValue class.
        /// </summary>
        /// <param name="type">Type of the value.</param>
        protected EdmValue(IEdmTypeReference type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public abstract EdmValueKind ValueKind
        {
            get;
        }

        IEdmValue IEdmDelayedValue.Value
        {
            get { return this; }
        }
    }
}
