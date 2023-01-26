//---------------------------------------------------------------------
// <copyright file="CaseInsensitiveSchemaElementsCachetESTS.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    public class CaseInsensitiveSchemaElementsCacheTests
    {

        [Theory]
        [InlineData("NS.Models.Person")]
        [InlineData("ns.models.person")]
        [InlineData("ns.MODELS.perSon")]
        public void FindElements_FindsElementBasedOnCaseInsensitiveMatch(string qualifiedName)
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddComplexType("NS.Models", "Address");

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindElements(qualifiedName);

            Assert.Equal(1, matches.Count);
            Assert.Equal(type1, matches[0]);
        }

        [Fact]
        public void FindElements_ReturnsEmptyList_IfNoMatchesFound()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindElements("ns.models.dog");

            Assert.Equal(0, matches.Count);
        }

        [Fact]
        public void FindElements_ReturnsAllMatches_IfMultipleMatchesFound()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "PERSON", type2.ToTypeReference());
            model.AddElement(func);
            model.AddComplexType("NS.Models", "Address");

            var cache = new CaseInsensitiveSchemaElementsCache(model);
            var matches = cache.FindElements("ns.models.person");

            Assert.Equal(4, matches.Count);
            Assert.Contains(matches, match => match == type1);
            Assert.Contains(matches, match => match == type2);
            Assert.Contains(matches, match => match == type3);
            Assert.Contains(matches, match => match == func);
        }

        [Fact]
        public void FindElementsOfType_ReturnsAllMatches_ThatMatchTheElementType()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "PERSON", type2.ToTypeReference());
            model.AddElement(func);
            model.AddComplexType("NS.Models", "Address");

            var cache = new CaseInsensitiveSchemaElementsCache(model);
            var matches = cache.FindElementsOfType<IEdmSchemaType>("ns.models.person");

            Assert.Equal(3, matches.Count);
            Assert.Contains(matches, match => match == type1);
            Assert.Contains(matches, match => match == type2);
            Assert.Contains(matches, match => match == type3);
        }

        [Fact]
        public void FindElementsOfType_ReturnsEmptyList_IfNoMatchFound()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "MostRecent", type2.ToTypeReference());
            model.AddElement(func);
            model.AddComplexType("NS.Models", "Address");

            var cache = new CaseInsensitiveSchemaElementsCache(model);

            var matches = cache.FindElementsOfType<IEdmSchemaType>("ns.models.dog");
            Assert.Equal(0, matches.Count);

            var functionMatches = cache.FindElementsOfType<IEdmOperation>("ns.models.person");
            Assert.Equal(0, functionMatches.Count);
        }

        [Fact]
        public void FindSingleOfType_ReturnsUniqueElement_ThatMatches()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "PERSON", type2.ToTypeReference());
            model.AddElement(func);
            var type4 = model.AddComplexType("NS.Models", "Address");


            var cache = new CaseInsensitiveSchemaElementsCache(model);
            var match = cache.FindSingleOfType<IEdmSchemaType>("ns.models.address", _ => string.Empty);

            Assert.Equal(type4, match);
        }

        [Fact]
        public void FindSingleOfType_ReturnsNull_IfNoMatchWasFound()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "MostRecent", type2.ToTypeReference());
            model.AddElement(func);
            var type4 = model.AddComplexType("NS.Models", "Address");


            var cache = new CaseInsensitiveSchemaElementsCache(model);
            var match = cache.FindSingleOfType<IEdmSchemaType>("ns.models.dog", _ => string.Empty);
            Assert.Null(match);

            var functionMatch = cache.FindSingleOfType<IEdmOperation>("ns.models.person", _ => string.Empty);
            Assert.Null(functionMatch);
        }

        [Fact]
        public void FindSingleOfType_ThrowsException_IfDuplicateFound()
        {
            var model = new EdmModel();
            var type1 = model.AddEntityType("NS.Models", "Person");
            type1.AddKeys(type1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type2 = model.AddEntityType("NS.Models", "person");
            type2.AddKeys(type2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            var type3 = model.AddComplexType("NS.Models", "perSon");
            var func = new EdmFunction("NS.Models", "MostRecent", type2.ToTypeReference());
            model.AddElement(func);
            var type4 = model.AddComplexType("NS.Models", "Address");


            var cache = new CaseInsensitiveSchemaElementsCache(model);
            var exception = Assert.Throws<ODataException>(() =>
                cache.FindSingleOfType<IEdmSchemaType>("ns.models.person", name => $"Duplicate types matching '{name}'."));
            Assert.Equal("Duplicate types matching 'ns.models.person'.", exception.Message);
        }
    }
}
