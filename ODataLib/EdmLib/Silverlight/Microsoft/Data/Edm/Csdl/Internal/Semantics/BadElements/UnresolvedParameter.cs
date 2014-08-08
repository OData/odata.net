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
