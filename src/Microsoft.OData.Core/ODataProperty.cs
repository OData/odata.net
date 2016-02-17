//---------------------------------------------------------------------
// <copyright file="ODataProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a single property of an entry.
    /// </summary>
    public sealed class ODataProperty : ODataAnnotatable
    {
        /// <summary>
        /// The value of this property, accessed and set by both <seealso cref="Value"/> and <seealso cref="ODataValue"/>.
        /// </summary>
        private ODataAnnotatable odataOrUndeclaredValue;

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataProperty"/>.
        /// </summary>
        private ODataPropertySerializationInfo serializationInfo;

        /// <summary>Gets or sets the property name.</summary>
        /// <returns>The property name.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the property value.</summary>
        /// <returns>The property value.</returns>
        public object Value
        {
            get
            {
                if (this.odataOrUndeclaredValue == null)
                {
                    return null;
                }

                ODataUndeclaredPropertyValue tmpValue = this.odataOrUndeclaredValue as ODataUndeclaredPropertyValue;
                if (tmpValue != null)
                {
                    return tmpValue;
                }

                return ((ODataValue)this.odataOrUndeclaredValue).FromODataValue();
            }

            set
            {
                ODataUndeclaredPropertyValue tmpValue = value as ODataUndeclaredPropertyValue;
                if (tmpValue != null)
                {
                    this.odataOrUndeclaredValue = tmpValue;
                }
                else
                {
                    this.odataOrUndeclaredValue = value.ToODataValue();
                }
            }
        }


        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }

        /// <summary>
        /// Property value, represented as an ODataValue.
        /// </summary>
        /// <remarks>
        /// This value is the same as <see cref="Value"/>, except that primitive types are wrapped 
        /// in an instance of ODataPrimitiveValue, and null values are represented by an instance of ODataNullValue.
        /// </remarks>
        internal ODataValue ODataValue
        {
            get
            {
                return (ODataValue)this.odataOrUndeclaredValue;
            }
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataProperty"/>.
        /// </summary>
        internal ODataPropertySerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = value;
            }
        }
    }
}
