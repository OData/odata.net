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

using System;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class UnresolvedEnumMember : BadElement, IEdmEnumMember
    {
        private readonly string name;
        private readonly IEdmEnumType declaringType;

        // Value cache.
        private readonly Cache<UnresolvedEnumMember, IEdmPrimitiveValue> value = new Cache<UnresolvedEnumMember, IEdmPrimitiveValue>();
        private static readonly Func<UnresolvedEnumMember, IEdmPrimitiveValue> ComputeValueFunc = (me) => me.ComputeValue();

        public UnresolvedEnumMember(string name, IEdmEnumType declaringType, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedEnumMember, Edm.Strings.Bad_UnresolvedEnumMember(name)) })
        {
            this.name = name ?? string.Empty;
            this.declaringType = declaringType;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmPrimitiveValue Value
        {
            get { return this.value.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmEnumType DeclaringType
        {
            get { return this.declaringType; }
        }

        private IEdmPrimitiveValue ComputeValue()
        {
            return new EdmIntegerConstant(0);
        }
    }
}
