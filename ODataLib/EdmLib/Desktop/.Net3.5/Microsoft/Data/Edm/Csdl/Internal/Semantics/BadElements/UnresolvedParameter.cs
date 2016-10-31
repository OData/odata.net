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
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class UnresolvedParameter : BadElement, IEdmFunctionParameter, IUnresolvedElement
    {
        private readonly string name;
        private readonly IEdmFunctionBase declaringFunction;

        // Type cache.
        private readonly Cache<UnresolvedParameter, IEdmTypeReference> type = new Cache<UnresolvedParameter, IEdmTypeReference>();
        private static readonly Func<UnresolvedParameter, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public UnresolvedParameter(IEdmFunctionBase declaringFunction, string name, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedParameter, Edm.Strings.Bad_UnresolvedParameter(name)) })
        {
            this.name = name ?? string.Empty;
            this.declaringFunction = declaringFunction;
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public EdmFunctionParameterMode Mode
        {
            get { return EdmFunctionParameterMode.In; }
        }

        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
