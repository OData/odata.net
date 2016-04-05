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
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Library;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularyInterfaceTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void AttachValueTermAnnotation()
        {
            var entityType = new StubEdmEntityType("NS1", "Person");

            var valueTerm = new StubValueTerm("", "FullName") { Type = EdmCoreModel.Instance.GetString(false) };

            var valueAnnotation = new StubValueAnnotation()
            {
                Term = valueTerm,
                Value = new StubStringConstantExpression("Forever Young"),
            };

            entityType.AddVocabularyAnnotation(valueAnnotation);

            Assert.AreEqual(1, entityType.InlineVocabularyAnnotations.Count(), "annotation count");

            var actual = entityType.InlineVocabularyAnnotations.Single() as IEdmValueAnnotation;

            Assert.AreEqual("", actual.Term.Namespace, "namespace");
            Assert.AreEqual("FullName", actual.Term.Name, "name");
            Assert.IsTrue(((IEdmValueTerm)actual.Term).Type.IsString() , "value term type is string");
            Assert.AreEqual("Forever Young", ((IEdmStringConstantExpression)actual.Value).Value, "annotation value");
        }
    }
}
