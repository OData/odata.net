//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
