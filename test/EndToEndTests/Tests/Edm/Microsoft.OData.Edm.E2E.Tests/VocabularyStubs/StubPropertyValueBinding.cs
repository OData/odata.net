//---------------------------------------------------------------------
// <copyright file="StubPropertyValueBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Vocabularies;

public class StubPropertyValueBinding : StubEdmElement, IEdmPropertyValueBinding
{
    public IEdmProperty BoundProperty { get; set; }

    public IEdmExpression Value { get; set; }
}
