//---------------------------------------------------------------------
// <copyright file="CsdlWriterTests.VocabularyAnnotation.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlWriterTests
    {
        [Fact]
        public async Task CanWriteAnnotationWithoutSpecifiedValue_UsingCustomIEdmVocabularyImplementation_Async()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmComplexType complex = new EdmComplexType("NS", "Complex");
            EdmTerm term1 = new EdmTerm("NS", "MyAnnotationPathTerm", EdmCoreModel.Instance.GetAnnotationPath(false));
            EdmTerm term2 = new EdmTerm("NS", "MyDefaultStringTerm", EdmCoreModel.Instance.GetString(false), "Property Term", "This is a test");
            EdmTerm term3 = new EdmTerm("NS", "MyDefaultBoolTerm", EdmCoreModel.Instance.GetBoolean(false), "Property Term", "true");
            model.AddElements(new IEdmSchemaElement[] { complex, term1, term2, term3 });

            // annotation with value
            IEdmVocabularyAnnotation annotation = new CustomEdmVocabularyAnnotation(complex, term1, new EdmAnnotationPathExpression("abc/efg"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            // annotation without value
            annotation = new CustomEdmVocabularyAnnotation(complex, term2, null);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            annotation = new CustomEdmVocabularyAnnotation(complex, term3, null);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            // Validate model
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            // Act & Assert for XML
            await WriteAndVerifyXmlAsync(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                     "<Annotation Term=\"NS.MyDefaultStringTerm\" />" + // no longer has value
                     "<Annotation Term=\"NS.MyDefaultBoolTerm\" />" + // no longer has value
                   "</ComplexType>" +
                   "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                   "<Term Name=\"MyDefaultStringTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                   "<Term Name=\"MyDefaultBoolTerm\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>").ConfigureAwait(false);

            // Act & Assert for JSON
            await WriteAndVerifyJsonAsync(model, @"{
  ""$Version"": ""4.0"",
  ""NS"": {
    ""Complex"": {
      ""$Kind"": ""ComplexType"",
      ""@NS.MyAnnotationPathTerm"": ""abc/efg"",
      ""@NS.MyDefaultStringTerm"": ""This is a test"",
      ""@NS.MyDefaultBoolTerm"": true
    },
    ""MyAnnotationPathTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""Edm.AnnotationPath""
    },
    ""MyDefaultStringTerm"": {
      ""$Kind"": ""Term"",
      ""$AppliesTo"": [
        ""Property Term""
      ],
      ""$DefaultValue"": ""This is a test""
    },
    ""MyDefaultBoolTerm"": {
      ""$Kind"": ""Term"",
      ""$Type"": ""Edm.Boolean"",
      ""$AppliesTo"": [
        ""Property Term""
      ],
      ""$DefaultValue"": ""true""
    }
  }
}").ConfigureAwait(false);
        }
    }
}
