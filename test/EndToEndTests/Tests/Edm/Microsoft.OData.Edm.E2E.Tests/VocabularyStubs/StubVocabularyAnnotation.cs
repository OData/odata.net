//---------------------------------------------------------------------
// <copyright file="StubVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;

public class StubVocabularyAnnotation : StubEdmElement, IEdmVocabularyAnnotation
{
    public string Qualifier { get; set; }

    public IEdmTerm Term { get; set; }

    public IEdmVocabularyAnnotatable Target { get; set; }

    public IEdmExpression Value { get; set; }

    public bool UsesDefault => throw new NotImplementedException();
}
