//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
