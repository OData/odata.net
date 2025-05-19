//---------------------------------------------------------------------
// <copyright file="VocabularyInterfaceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.E2E.Tests.VocabularyStubs;
using Microsoft.OData.Edm.Vocabularies;

public class VocabularyInterfaceTests : EdmLibTestCaseBase
{
    [Fact]
    public void AttachTermAnnotation()
    {
        var entityType = new StubEdmEntityType("NS1", "Person");

        var valueTerm = new StubTerm("", "FullName") { Type = EdmCoreModel.Instance.GetString(false) };

        var valueAnnotation = new StubVocabularyAnnotation()
        {
            Term = valueTerm,
            Value = new StubStringConstantExpression("Forever Young"),
        };

        entityType.AddVocabularyAnnotation(valueAnnotation);

        Assert.Single(entityType.InlineVocabularyAnnotations);

        var actual = entityType.InlineVocabularyAnnotations.Single();

        Assert.Equal("", actual.Term.Namespace);
        Assert.Equal("FullName", actual.Term.Name);
        Assert.True(actual.Term.Type.IsString() , "Term type is string");
        Assert.Equal("Forever Young", ((IEdmStringConstantExpression)actual.Value).Value);
    }
}
