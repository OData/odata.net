//---------------------------------------------------------------------
// <copyright file="ODataComplexValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    #endregion Namespaces

    /// <summary>
    /// OData representation of a complex value.
    /// </summary>
    internal sealed class ODataComplexValue : ODataValue
    {
        /// <summary>Gets or sets the properties and values of the complex value.</summary>
        /// <returns>The properties and values of the complex value.</returns>
        public IEnumerable<ODataProperty> Properties
        {
            get;
            set;
        }

        /// <summary>Gets or sets the type of the complex value.</summary>
        /// <returns>The type of the complex value.</returns>
        public string TypeName
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
    }
}
