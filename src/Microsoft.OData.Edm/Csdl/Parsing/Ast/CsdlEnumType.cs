//---------------------------------------------------------------------
// <copyright file="CsdlEnumType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL enumeration type.
    /// </summary>
    internal class CsdlEnumType : CsdlNamedElement
    {
        private readonly string underlyingTypeName;
        private readonly bool isFlags;
        private readonly List<CsdlEnumMember> members;

        public CsdlEnumType(string name, string underlyingTypeName, bool isFlags, IEnumerable<CsdlEnumMember> members, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.underlyingTypeName = underlyingTypeName;
            this.isFlags = isFlags;
            this.members = new List<CsdlEnumMember>(members);
        }

        public string UnderlyingTypeName
        {
            get { return this.underlyingTypeName; }
        }

        public bool IsFlags
        {
            get { return this.isFlags; }
        }

        public IEnumerable<CsdlEnumMember> Members
        {
            get { return this.members; }
        }
    }
}
