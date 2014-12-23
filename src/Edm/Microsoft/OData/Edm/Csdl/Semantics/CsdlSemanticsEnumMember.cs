//   OData .NET Libraries ver. 6.9
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

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
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
