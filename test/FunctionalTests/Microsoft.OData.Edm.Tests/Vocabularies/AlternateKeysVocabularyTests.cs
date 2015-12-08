//---------------------------------------------------------------------
// <copyright file="AlternateKeysVocabularyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public class AlternateKeysVocabularyTests
    {
        private readonly IEdmModel model = AlternateKeysVocabularyModel.Instance;

        [Fact]
        public void TestAlternateKeysVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""OData.Community.Keys.V1"" Alias=""Keys"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
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

            var alternateKeysTerm = model.FindDeclaredValueTerm("OData.Community.Keys.V1.AlternateKeys");
            Assert.NotNull(alternateKeysTerm);
            Assert.Equal(AlternateKeysVocabularyModel.AlternateKeysTerm, alternateKeysTerm);
            Assert.Equal("OData.Community.Keys.V1", alternateKeysTerm.Namespace);
            Assert.Equal("AlternateKeys", alternateKeysTerm.Name);
            Assert.Equal(EdmTermKind.Value, alternateKeysTerm.TermKind);

            StringWriter sw = new StringWriter();
            IEnumerable<EdmError> errors;
            using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                Assert.True(model.TryWriteCsdl(xw, out errors));
            }

            string output = sw.ToString();

            Assert.False(errors.Any(), "No Errors");
            Assert.True(expectedText == output, "expectedText = output");
        }
    }
}

