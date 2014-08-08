//   OData .NET Libraries ver. 5.6.2
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

using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
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
