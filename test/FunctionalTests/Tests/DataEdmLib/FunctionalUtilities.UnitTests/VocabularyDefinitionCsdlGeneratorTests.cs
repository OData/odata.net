//---------------------------------------------------------------------
// <copyright file="VocabularyDefinitionCsdlGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Linq;
    using System.Xml.Linq;
    using EdmLibTests.StubEdm;
    using EdmLibTests.VocabularyStubs;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyDefinitionCsdlGeneratorTests
    {
        private VocabularyDefinitionCsdlGenerator generator = new VocabularyDefinitionCsdlGenerator();

        [TestMethod]
        public void Simple_TermDefinitions_Should_Be_Generated()
        {
            var model = new StubEdmModel()
            {
                new StubTerm("NS1", "fooValueTerm") { Type = EdmCoreModel.Instance.GetInt32(true) },
                new StubTerm("NS1", "barValueTerm") { Type = EdmCoreModel.Instance.GetString(false)},
            };

            XElement result = this.generator.GenerateDefinitionCsdl(EdmVersion.V40, model).Single();

            string expected = @"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Term Name='fooValueTerm' Type='Edm.Int32' />
  <Term Name='barValueTerm' Type='Edm.String' Nullable='false' />
</Schema>";
            AssertHelper.AssertXElementEquals(expected, result);
        }

        [TestMethod]
        public void Simple_TermDefinitions_WithEntityType_InSameNamespace_Should_Be_Generated_Together()
        {
            var model = new StubEdmModel()
            {
                new StubEdmEntityType("NS1", "Person") 
                { 
                    new StubEdmStructuralProperty("Name") { Type = EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:null, isUnicode:true, isNullable:false)},
                },
                new StubTerm("NS1", "fooValueTerm") { Type = EdmCoreModel.Instance.GetInt32(true)},
                new StubTerm("NS1", "barValueTerm") { Type = EdmCoreModel.Instance.GetString(false)},
            };

            XElement result = this.generator.GenerateDefinitionCsdl(EdmVersion.V40, model).Single();

            string expected = @"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Name' Type='Edm.String' Nullable='false' />
  </EntityType>
  <Term Name='fooValueTerm' Type='Edm.Int32' />
  <Term Name='barValueTerm' Type='Edm.String' Nullable='false' />
</Schema>";
            AssertHelper.AssertXElementEquals(expected, result);
        }

        [TestMethod]
        public void TermDefinitions_InMultipleNamespaces_Should_Be_Generated()
        {
            var model = new StubEdmModel()
            {
                new StubTerm("NS1", "fooValueTerm") { Type = EdmCoreModel.Instance.GetInt32(true)},
                new StubTerm("NS2", "barValueTerm") { Type = EdmCoreModel.Instance.GetString(false)},
            };

            var fileContents = this.generator.GenerateDefinitionCsdl(EdmVersion.V40, model);
            Assert.AreEqual(2, fileContents.Count());

            var result1 = fileContents.ElementAt(0);
            string expected1 = @"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Term Name='fooValueTerm' Type='Edm.Int32' />
</Schema>";
            AssertHelper.AssertXElementEquals(expected1, result1);

            var result2 = fileContents.ElementAt(1);
            string expected2 = @"
<Schema Namespace='NS2' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Term Name='barValueTerm' Type='Edm.String' Nullable='false' />
</Schema>";
            AssertHelper.AssertXElementEquals(expected2, result2);
        }

        [TestMethod]
        public void Simple_TermDefinitions_WithEntityTypes_InDifferentNamespaces_Should_Be_Generated_Separately()
        {
            var model = new StubEdmModel()
            {
                new StubEdmEntityType("NS0", "Person") 
                { 
                    new StubEdmStructuralProperty("Name") { Type = EdmCoreModel.Instance.GetString(isUnbounded:false, maxLength:null, isUnicode:true, isNullable:false) },
                },
                new StubTerm("NS1", "fooValueTerm") { Type = EdmCoreModel.Instance.GetInt32(true)},
                new StubTerm("NS1", "barValueTerm") { Type = EdmCoreModel.Instance.GetString(false)},
            };

            var fileContents = this.generator.GenerateDefinitionCsdl(EdmVersion.V40, model);
            Assert.AreEqual(2, fileContents.Count());

            var result1 = fileContents.ElementAt(0);
            string expected1 = @"
<Schema Namespace='NS0' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <EntityType Name='Person'>
    <Property Name='Name' Type='Edm.String' Nullable='false' />
  </EntityType>
</Schema>";
            AssertHelper.AssertXElementEquals(expected1, result1);

            var result2 = fileContents.ElementAt(1);
            string expected2 = @"
<Schema Namespace='NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Term Name='fooValueTerm' Type='Edm.Int32' />
  <Term Name='barValueTerm' Type='Edm.String' Nullable='false' />
</Schema>";
            AssertHelper.AssertXElementEquals(expected2, result2);
        }
    }
}
