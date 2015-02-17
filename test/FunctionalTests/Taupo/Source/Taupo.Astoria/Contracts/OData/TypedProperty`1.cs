//---------------------------------------------------------------------
// <copyright file="TypedProperty`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Abstract base class for primitive and complex properties
    /// </summary>
    /// <typeparam name="TValue">A type deriving from TypedValue with an empty constructor</typeparam>
    public abstract class TypedProperty<TValue> : PropertyInstance where TValue : ODataPayloadElement, ITypedValue, new()
    {
        /// <summary>
        ///  Private storage for the value
        /// </summary>
        private TValue currentValue = null;
        
        /// <summary>
        /// Initializes a new instance of the TypedProperty class
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The property's value</param>
        protected TypedProperty(string name, TValue value)
            : base(name)
        {
            this.currentValue = value;
        }

        /// <summary>
        /// Gets or sets the value of the property. Will never be null.
        /// </summary>
        public TValue Value
        {
            get
            {
                // if it is null, create a new value
                if (this.currentValue == null)
                {
                    // we intentionally go through the setter for this property
                    this.Value = new TValue();
                }

                return this.currentValue;
            }

            set
            {
                // do not allow null to be assigned
                ExceptionUtilities.CheckArgumentNotNull(value, "value");
                this.currentValue = value;
            }
        }

        /// <summary>
        /// Gets a string representation of the element to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Name, this.Value.StringRepresentation);
            }
        }
    }
}
