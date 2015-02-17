//---------------------------------------------------------------------
// <copyright file="TypedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Abstract base class for PrimitiveValue and ComplexInstance, represents a value with a type and a flag for null
    /// </summary>
    public abstract class TypedValue : ODataPayloadElement, ITypedValue
    {
        private bool isNull;
        private string fullTypeName;

        /// <summary>
        /// Initializes a new instance of the TypedValue class with the given values
        /// </summary>
        /// <param name="fullTypeName">The full type name of the value</param>
        /// <param name="isNull">Whether or not the value is null</param>
        protected TypedValue(string fullTypeName, bool isNull)
        {
            this.FullTypeName = fullTypeName;
            this.IsNull = isNull;
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name for the value
        /// </summary>
        public string FullTypeName 
        {
            get
            {
                return this.fullTypeName;
            }

            set
            {
                this.fullTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the value is null
        /// </summary>
        public bool IsNull
        {
            get
            {
                return this.isNull;
            }

            set
            {
                this.isNull = value;
            }
        }
    }
}
