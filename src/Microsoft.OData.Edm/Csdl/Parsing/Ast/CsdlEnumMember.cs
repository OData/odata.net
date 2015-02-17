//---------------------------------------------------------------------
// <copyright file="CsdlEnumMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL enumeration type member.
    /// </summary>
    internal class CsdlEnumMember : CsdlNamedElement
    {
        public CsdlEnumMember(string name, long? value, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the underlying type value of the member.
        /// Value can be null only during deserialization of the declaring enumeration type.
        /// When the type's deserialization is complete, all its members get their values assigned.
        /// </summary>
        public long? Value
        {
            get;
            set;
        }
    }
}
