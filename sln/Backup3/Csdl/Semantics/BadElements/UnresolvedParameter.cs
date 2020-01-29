//---------------------------------------------------------------------
// <copyright file="UnresolvedParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
