//---------------------------------------------------------------------
// <copyright file="StructuralType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Structural type
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public abstract class StructuralType : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the StructuralType class.
        /// </summary>
        protected StructuralType()
        {
            this.Properties = new List<MemberProperty>();
        }

        /// <summary>
        /// Gets list of properties of the type. (Self-defined, not including the ones defined in BaseType)
        /// </summary>
        public IList<MemberProperty> Properties { get; private set; }

        /// <summary>
        /// Adds a new property to the type.
        /// </summary>
        /// <param name="property">Property to add</param>
        public void Add(MemberProperty property)
        {
            this.Properties.Add(property);
        }
    }
}
