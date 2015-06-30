//---------------------------------------------------------------------
// <copyright file="CapabilitiesVocabularyTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies.Community.V1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommunityVocabularyTest
    {
        private readonly IEdmModel model = CommunityVocabularyModel.Instance;

        [TestMethod]
        public void TestAlternateKeysVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""OData.Community.AlternateKeys.V1"" Alias=""Keys"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""AlternateKey"">
    <Property Name=""Key"" Type=""Collection(Keys.PropertyRef)"">
      <Annotation Term=""Core.Description"" String=""The set of properties that make up this key"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""PropertyRef"">
    <Property Name=""Name"" Type=""Edm.PropertyPath"">
      <Annotation Term=""Core.Description"" String=""A path expression resolving to a primitive property of the entity type itself or to a primitive property of a complex property (recursively) of the entity type. The names of the properties in the path are joined together by forward slashes."" />
    </Property>
    <Property Name=""Alias"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""A SimpleIdentifier that MUST be unique within the set of aliases, structural and navigation properties of the containing entity type that MUST be used in the key predicate of URLs"" />
    </Property>
  </ComplexType>
  <Term Name=""AlternateKeys"" Type=""Collection(Keys.AlternateKey)"" AppliesTo=""EntityType"">
    <Annotation Term=""Core.Description"" String=""Communicates available alternate keys"" />
  </Term>
</Schema>";

            var alternateKeysTerm = model.FindDeclaredValueTerm("OData.Community.AlternateKeys.V1.AlternateKeys");
            Assert.IsNotNull(alternateKeysTerm);
            Assert.AreEqual(CommunityVocabularyModel.AlternateKeysTerm, alternateKeysTerm);
            Assert.AreEqual("OData.Community.AlternateKeys.V1", alternateKeysTerm.Namespace);
            Assert.AreEqual("AlternateKeys", alternateKeysTerm.Name);
            Assert.AreEqual(EdmTermKind.Value, alternateKeysTerm.TermKind);

            StringWriter sw = new StringWriter();
            IEnumerable<EdmError> errors;
            using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                Assert.IsTrue(model.TryWriteCsdl(xw, out errors));
            }

            string output = sw.ToString();

            Assert.IsTrue(!errors.Any(), "No Errors");
            Assert.AreEqual(expectedText, output, "expectedText = output");
        }
    }
}

