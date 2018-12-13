//---------------------------------------------------------------------
// <copyright file="UnresolvedReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedReturnType : BadElement, IEdmOperationReturnType, IUnresolvedElement
    {
        // Type cache.
        private readonly Cache<UnresolvedReturnType, IEdmTypeReference> type = new Cache<UnresolvedReturnType, IEdmTypeReference>();
        private static readonly Func<UnresolvedReturnType, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public UnresolvedReturnType(IEdmOperation declaringOperation, EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedReturnType, Edm.Strings.Bad_UnresolvedReturnType("")) })
        {
            this.DeclaringOperation = declaringOperation;
        }

        public IEdmTypeReference Type
        {
            get { return this.type.GetValue(this, ComputeTypeFunc, null); }
        }

        public IEdmOperation DeclaringOperation { get; private set; }

        public bool IsNullable
        {
            get { return Type.IsNullable; }
        }

        public IEdmType Definition
        {
            get { return Type.Definition; }
        }

        private IEdmTypeReference ComputeType()
        {
            return new BadTypeReference(new BadType(Errors), true);
        }
    }
}
