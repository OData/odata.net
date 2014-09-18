//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedParameter : BadElement, IEdmOperationParameter, IUnresolvedElement
    {
        // Type cache.
        private readonly Cache<UnresolvedParameter, IEdmTypeReference> type = new Cache<UnresolvedParameter, IEdmTypeReference>();
        private static readonly Func<UnresolvedParameter, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public UnresolvedParameter(IEdmOperation declaringOperation, string name, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedParameter, Edm.Strings.Bad_UnresolvedParameter(name)) })
        {
            this.Name = name ?? string.Empty;
            this.DeclaringOperation = declaringOperation;
        }

        public string Name { get; private set; }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public IEdmOperation DeclaringOperation { get; private set; }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
