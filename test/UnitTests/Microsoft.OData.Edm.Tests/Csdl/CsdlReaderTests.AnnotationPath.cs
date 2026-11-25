//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.AnnotationPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlReaderTests
    {
        [Fact]
        public void ParsingAnnotationPathElementInCollectionWorks()
        {
            const string csdl = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key><PropertyRef Name=""Id""/></Key>
        <Property Name=""Id"" Type=""Edm.Int32""/>
      </EntityType>
      <Annotations Target=""NS.Customer"">
        <Annotation Term=""NS.Visualizations"">
          <Collection>
            <AnnotationPath>@UI.LineItem</AnnotationPath>
          </Collection>
        </Annotation>
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            using (XmlReader reader = XmlReader.Create(new StringReader(csdl)))
            {
                bool parsed = CsdlReader.TryParse(reader, out IEdmModel model, out var errors);

                Assert.True(parsed, string.Join("; ", errors.Select(e => e.ErrorMessage)));
                IEdmVocabularyAnnotation annotation = Assert.Single(model.VocabularyAnnotations);
                var collection = Assert.IsAssignableFrom<IEdmCollectionExpression>(annotation.Value);
                var path = Assert.IsAssignableFrom<IEdmPathExpression>(Assert.Single(collection.Elements));
                Assert.Equal(EdmExpressionKind.AnnotationPath, path.ExpressionKind);
                Assert.Equal("@UI.LineItem", path.Path);
            }
        }
    }
}