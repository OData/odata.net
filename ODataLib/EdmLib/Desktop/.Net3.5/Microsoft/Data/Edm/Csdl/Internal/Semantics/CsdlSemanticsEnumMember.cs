//   Copyright 2011 Microsoft Corporation
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

using System;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlEnumMember.
    /// </summary>
    internal class CsdlSemanticsEnumMember : CsdlSemanticsElement, IEdmEnumMember
    {
        private readonly CsdlEnumMember member;
        private readonly CsdlSemanticsEnumTypeDefinition declaringType;

        private readonly Cache<CsdlSemanticsEnumMember, IEdmPrimitiveValue> valueCache = new Cache<CsdlSemanticsEnumMember, IEdmPrimitiveValue>();
        private static readonly Func<CsdlSemanticsEnumMember, IEdmPrimitiveValue> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsEnumMember(CsdlSemanticsEnumTypeDefinition declaringType, CsdlEnumMember member)
            : base(member)
        {
            this.member = member;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.member.Name; }
        }

        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        public IEdmPrimitiveValue Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringType.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.member; }
        }

        private IEdmPrimitiveValue ComputeValue()
        {
            return new EdmIntegerConstant(new EdmPrimitiveTypeReference(this.DeclaringType.UnderlyingType, false), this.member.Value.Value);
        }
    }
}
