//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
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
