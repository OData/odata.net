//---------------------------------------------------------------------
// <copyright file="EdmPrimitiveValueSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    internal class EdmPrimitiveValueSimulator : IEdmPrimitiveValue
    {
        public EdmPrimitiveValueSimulator(IEdmTypeReference type, EdmValueKind valueKind)
        {
            this.Type = type;
            this.ValueKind = valueKind;
        }

        public IEdmTypeReference Type { get; private set; }

        public EdmValueKind ValueKind { get; private set; }
    }
}