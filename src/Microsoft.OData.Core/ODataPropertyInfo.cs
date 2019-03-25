//---------------------------------------------------------------------
// <copyright file="ODataPropertyInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents information about a single property of a resource.
    /// </summary>
    public class ODataPropertyInfo : ODataItem
    {
        /// <summary>Gets or sets the property name.</summary>
        /// <returns>The property name.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the kind of primitive type of the property.</summary>
        /// <returns>The <see cref="EdmPrimitiveTypeKind"/> of the property.</returns>
        public virtual EdmPrimitiveTypeKind PrimitiveTypeKind
        {
            get;
            set;
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
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataProperty"/>.
        /// </summary>
        internal ODataPropertySerializationInfo SerializationInfo { get; set; }
    }
}