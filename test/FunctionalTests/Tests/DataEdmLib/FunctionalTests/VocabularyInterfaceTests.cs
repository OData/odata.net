//---------------------------------------------------------------------
// <copyright file="VocabularyInterfaceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Linq;
    using EdmLibTests.StubEdm;
    using EdmLibTests.VocabularyStubs;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyInterfaceTests : EdmLibTestCaseBase
    {
        [TestMethod]
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

            Assert.AreEqual(1, entityType.InlineVocabularyAnnotations.Count(), "annotation count");

            var actual = entityType.InlineVocabularyAnnotations.Single();

            Assert.AreEqual("", actual.Term.Namespace, "namespace");
            Assert.AreEqual("FullName", actual.Term.Name, "name");
            Assert.IsTrue(actual.Term.Type.IsString() , "Term type is string");
            Assert.AreEqual("Forever Young", ((IEdmStringConstantExpression)actual.Value).Value, "annotation value");
        }
    }
}
