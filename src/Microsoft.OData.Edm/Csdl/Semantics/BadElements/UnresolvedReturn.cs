//---------------------------------------------------------------------
// <copyright file="UnresolvedReturn.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedReturn : BadElement, IEdmOperationReturn, IUnresolvedElement
    {
        // Type cache.
        private readonly Cache<UnresolvedReturn, IEdmTypeReference> type = new Cache<UnresolvedReturn, IEdmTypeReference>();
        private static readonly Func<UnresolvedReturn, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public UnresolvedReturn(IEdmOperation declaringOperation, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedReturn, Edm.Strings.Bad_UnresolvedReturn(declaringOperation.Name)) })
        {
            this.DeclaringOperation = declaringOperation;
        }

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
