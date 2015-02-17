//---------------------------------------------------------------------
// <copyright file="ObjectContextType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System.Collections.Generic;
#if EF6Provider
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Diagnostics;
    using System.Linq;

    #endregion

    /// <summary>
    /// Provides an encapsulation of the Entity Framework StructuralType class.
    /// </summary>
    internal class ObjectContextType : IProviderType
    {
        /// <summary>StructuralType being encapsulated.</summary>
        private readonly StructuralType structuralType;

        /// <summary>
        /// Creates a new encapsulations of the specified type.
        /// </summary>
        /// <param name="structuralType">StructuralType to encapsulate.</param>
        internal ObjectContextType(StructuralType structuralType)
        {
            Debug.Assert(structuralType != null, "Can't create ObjectContextType from a null structuralType.");
            this.structuralType = structuralType;
        }

        /// <summary>
        /// Returns the members declared on this type only, not including any inherited members.
        /// </summary>
        public IEnumerable<IProviderMember> Members
        {
            get
            {
                foreach (EdmMember member in this.structuralType.Members.Where(m => m.DeclaringType == this.structuralType))
                {
                    yield return new ObjectContextMember(member);
                }
            }
        }

        /// <summary>
        /// Name of the type without its namespace
        /// </summary>
        public string Name
        {
            get
            {
                return this.structuralType.Name;
            }
        }
    }
}
