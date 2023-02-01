//---------------------------------------------------------------------
// <copyright file="CaseInsensitiveSchemaElementsCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    public class CaseInsensitiveSchemaElementsCacheTests
    {

        [Theory]
        [InlineData("NS.Models.Person")]
        [InlineData("ns.models.person")]
        [InlineData("ns.MODELS.perSon")]
        public void FindSchemaTypes_FindsTypesBasedOnCaseInsensitiveMatch(string qualifiedName)
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddComplexType("NS.Models", "Address");

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindSchemaTypes(qualifiedName);

            Assert.Single(matches);
            Assert.Equal(type1, matches[0]);
        }

        [Fact]
        public void FindSchemaTypes_FindsTypesThatMatchName()
        {
            var model = new EdmModel();
            var p1 = model.AddEntityType("NS.Models", "Person");
            p1.AddKeys(p1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddComplexType("NS.Models", "Address");
            var p2 = model.AddComplexType("NS.Models", "person");

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindSchemaTypes("ns.models.person");
            Assert.Equal(2, matches.Count);
            Assert.Equal(p1, matches[0]);
            Assert.Equal(p2, matches[1]);
        }

        [Fact]
        public void FindSchemaTypes_ReturnsNullIfThereAreNoMatches()
        {
            var model = new EdmModel();
            var p1 = model.AddEntityType("NS.Models", "Person");
            p1.AddKeys(p1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddComplexType("NS.Models", "Address");
            model.AddComplexType("NS.Models", "person");

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindSchemaTypes("ns.models.pers");
            Assert.Null(matches);
        }

        [Theory]
        [InlineData("NS.Models.DoStuff")]
        [InlineData("ns.models.dostuff")]
        [InlineData("ns.MODELS.doStuff")]
        public void FindOperations_FindsOperationsBasedOnCaseInsensitiveMatch(string qualifiedName)
        {
            var model = new EdmModel();
            var operation = new EdmAction("NS.Models", "DoStuff", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(operation);
            model.AddElement(new EdmFunction("Ns.Models", "ComputeStuff", EdmCoreModel.Instance.GetBoolean(false)));

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindOperations(qualifiedName);

            Assert.Single(matches);
            Assert.Equal(operation, matches[0]);
        }

        [Fact]
        public void FindOperations_FindsOperationsThatMatchName()
        {
            var model = new EdmModel();
            var operation = new EdmAction("NS.Models", "DoStuff", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(new EdmAction("NS.Models", "DoStuff", EdmCoreModel.Instance.GetBoolean(false)));
            var func1 = new EdmFunction("Ns.Models", "ComputeStuff", EdmCoreModel.Instance.GetBoolean(false));
            var func2 = new EdmFunction("Ns.Models", "ComputeStuff", EdmCoreModel.Instance.GetBoolean(false));
            func2.AddParameter("foo", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(func1);
            model.AddElement(func2);

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindOperations("ns.models.computeStuff");
            Assert.Equal(2, matches.Count);
            Assert.Contains(matches, match => match == func1);
            Assert.Contains(matches, match => match == func2);
        }

        [Fact]
        public void FindOperations_ReturnsNullIfThereAreNoMatches()
        {
            var model = new EdmModel();
            var operation = new EdmAction("NS.Models", "DoStuff", EdmCoreModel.Instance.GetBoolean(false));
            model.AddElement(operation);
            model.AddElement(new EdmFunction("Ns.Models", "ComputeStuff", EdmCoreModel.Instance.GetBoolean(false)));

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindOperations("ns.models.dost");
            Assert.Null(matches);
        }

        [Theory]
        [InlineData("NS.Models.SomeTerm")]
        [InlineData("ns.models.someterm")]
        [InlineData("ns.MODELS.someTerm")]
        public void FindTerm_FindsTermsBasedOnCaseInsensitiveMatch(string qualifiedName)
        {
            var model = new EdmModel();
            var term = model.AddTerm("NS.Models", "SomeTerm", EdmPrimitiveTypeKind.String);
            model.AddTerm("NS.Models", "OtherTerm", EdmPrimitiveTypeKind.String);

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindTerms(qualifiedName);

            Assert.Single(matches);
            Assert.Equal(term, matches[0]);
        }

        [Fact]
        public void FindTerms_FindsAllTermshatMatchName()
        {
            var model = new EdmModel();
            var term1 = model.AddTerm("NS.Models", "SomeTerm", EdmPrimitiveTypeKind.String);
            model.AddTerm("NS.Models", "OtherTerm", EdmPrimitiveTypeKind.String);
            var term2 = model.AddTerm("NS.Models", "someTerm", EdmPrimitiveTypeKind.String);

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindTerms("ns.models.someTerm");
            Assert.Equal(2, matches.Count);
            Assert.Contains(matches, match => match == term1);
            Assert.Contains(matches, match => match == term2);
        }

        [Fact]
        public void FindTerms_ReturnsNullIfThereAreNoMatches()
        {
            var model = new EdmModel();
            model.AddTerm("NS.Models", "SomeTerm", EdmPrimitiveTypeKind.String);
            model.AddTerm("NS.Models", "OtherTerm", EdmPrimitiveTypeKind.String);

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindTerms("ns.models.some");
            Assert.Null(matches);
        }
    }
}
