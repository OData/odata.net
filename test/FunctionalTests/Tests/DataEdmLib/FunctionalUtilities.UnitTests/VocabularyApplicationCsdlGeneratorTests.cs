//---------------------------------------------------------------------
// <copyright file="VocabularyApplicationCsdlGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Xml.Linq;
    using EdmLibTests.StubEdm;
    using EdmLibTests.VocabularyStubs;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyApplicationCsdlGeneratorTests
    {
        private VocabularyApplicationCsdlGenerator generator = new VocabularyApplicationCsdlGenerator();

        [TestMethod]
        public void Simple_VocabularyAnnotation_Should_Be_Generated()
        {
            var model = new StubEdmModel();

            var entity = new StubEdmEntityType("NS1", "Person");

            var vt = new StubTerm("NS1", "MyValueTerm") { Type = EdmCoreModel.Instance.GetString(true) };
            var va1 = new StubVocabularyAnnotation() { Term = vt, Value = new StubStringConstantExpression("Great!!!") };
            var va2 = new StubVocabularyAnnotation() { Term = vt, Qualifier = "phone", Value = new StubStringConstantExpression("Fabulous!!!") };
            entity.AddVocabularyAnnotation(va1);
            entity.AddVocabularyAnnotation(va2);

            model.Add(entity);

            XElement result = this.generator.GenerateApplicationCsdl(EdmVersion.V40, model);

            string expected = @"
<Schema Namespace='Application.NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='NS1.Person'>
    <Annotation Term='NS1.MyValueTerm' String='Great!!!' />
    <Annotation Term='NS1.MyValueTerm' Qualifier='phone' String='Fabulous!!!' />
  </Annotations>
</Schema>";
            AssertHelper.AssertXElementEquals(expected, result);
        }

        [TestMethod]
        public void Annotations_On_NonEntityType_Should_Be_Generated()
        {
            var model = new StubEdmModel();

            var entity = new StubEdmEntityType("NS1", "Person");
            var vt = new StubTerm("NS1", "MyValueTerm") { Type = EdmCoreModel.Instance.GetString(true) };
            var va1 = new StubVocabularyAnnotation() { Term = vt, Value = new StubStringConstantExpression("Great!!!") };
            entity.AddVocabularyAnnotation(va1);
            model.Add(entity);

            var entitySet = new StubEdmEntitySet("personSet", null);
            var va2 = new StubVocabularyAnnotation() { Term = vt, Value = new StubStringConstantExpression("Aha!!!") };
            entitySet.AddVocabularyAnnotation(va2);

            var container = new StubEdmEntityContainer("NS1", "myContainer") { entitySet };
            var va3 = new StubVocabularyAnnotation() { Term = vt, Value = new StubStringConstantExpression("Huh??") };
            container.AddVocabularyAnnotation(va3);
            model.Add(container);

            XElement result = this.generator.GenerateApplicationCsdl(EdmVersion.V40, model);

            string expected = @"
<Schema Namespace='Application.NS1' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
  <Annotations Target='NS1.Person'>
    <Annotation Term='NS1.MyValueTerm' String='Great!!!' />
  </Annotations>
  <Annotations Target='NS1.myContainer'>
    <Annotation Term='NS1.MyValueTerm' String='Huh??' />
  </Annotations>
  <Annotations Target='NS1.myContainer/personSet'>
    <Annotation Term='NS1.MyValueTerm' String='Aha!!!' />
  </Annotations>
</Schema>";
            AssertHelper.AssertXElementEquals(expected, result);
        }
    }
}
