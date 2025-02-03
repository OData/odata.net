//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlReaderTests
    {
        [InlineData("4.0")]
        [InlineData("4.01")]
        [Theory]
        public async Task ValidateNavigationPropertyBindingPathEndingInTypeCast_Async(string odataVersion)
        {
            var entitySetWithNavProperties =
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"Nav/NS.EntityX1\" Target=\"SetX1\" />" +
                              "<NavigationPropertyBinding Path=\"Nav/NS.EntityX2\" Target=\"SetX2\" />" +
                            "</EntitySet>";
            var entitySetWithoutNavProperties =
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\" />";
            var csdl =
                  "<edmx:Edmx Version=\"{0}\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityA\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                              "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                              "<NavigationProperty Name=\"Nav\" Type=\"NS.EntityX\" Nullable=\"false\" />" +
                            "</EntityType>" +
                          "<EntityType Name=\"EntityX\" Abstract=\"true\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityX1\" BaseType=\"NS.EntityX\" />" +
                          "<EntityType Name=\"EntityX2\" BaseType=\"NS.EntityX\" />" +
                          "<EntityContainer Name=\"Container\">" +
                          "{1}" +
                            "<EntitySet Name=\"SetX1\" EntityType=\"NS.EntityX1\" />" +
                            "<EntitySet Name=\"SetX2\" EntityType=\"NS.EntityX2\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(String.Format(csdl, odataVersion, entitySetWithNavProperties)).CreateReader());
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));
            Assert.Empty(errors);

            var setA = model.FindDeclaredNavigationSource("SetA");
            Assert.NotNull(setA);

            var navProp = setA.EntityType.FindProperty("Nav") as IEdmNavigationProperty;
            Assert.NotNull(navProp);
            Assert.Equal(2, setA.NavigationPropertyBindings.Count());

            // NavPropBinding for for EntityX1 should be SetX1
            var X1NavPropBinding = setA.NavigationPropertyBindings.FirstOrDefault(b => b.Path.PathSegments.Last() == "NS.EntityX1");
            Assert.NotNull(X1NavPropBinding);
            var setX1 = model.FindDeclaredNavigationSource("SetX1");
            Assert.Same(setX1, X1NavPropBinding.Target);
            Assert.Same(setX1, setA.FindNavigationTarget(navProp, new EdmPathExpression("Nav/NS.EntityX1")));

            // NavPropBinding for EntityX2 should be SetX2
            var X2NavPropBinding = setA.NavigationPropertyBindings.FirstOrDefault(b => b.Path.PathSegments.Last() == "NS.EntityX2");
            Assert.NotNull(X2NavPropBinding);
            var setX2 = model.FindDeclaredNavigationSource("SetX2");
            Assert.Same(setX2, X2NavPropBinding.Target);
            Assert.Same(setX2, setA.FindNavigationTarget(navProp, new EdmPathExpression("Nav/NS.EntityX2")));

            // Navigation Property without path should return UnknownEntitySet
            Assert.True(setA.FindNavigationTarget(navProp) is EdmUnknownEntitySet);

            // Verify writing model
            model.SetEdmVersion(Version.Parse(odataVersion));
            await VerifyXmlModelAsync(model, odataVersion == "4.0" ? entitySetWithoutNavProperties : entitySetWithNavProperties).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReadAnnotationWithoutSpecifiedValueAndUseDefault_Async()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                         "<Annotation Term=\"NS.MyDefaultTerm\" />" + // does not specify value
                       "</ComplexType>" +
                       "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                     "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>";
            // Parse into CSDL

            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmTerm defaultTerm = model.FindTerm("NS.MyDefaultTerm");
            Assert.Equal("This is a test", defaultTerm.DefaultValue);
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(2, annotations.Length);

            IEdmVocabularyAnnotation annotationWithSpecifiedValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[0]);
            Assert.NotNull(annotationWithSpecifiedValue.Value);
            Assert.IsAssignableFrom<IEdmPathExpression>(annotationWithSpecifiedValue.Value);

            IEdmVocabularyAnnotation annotationWithDefaultValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[1]);
            Assert.NotNull(annotationWithDefaultValue.Value);
            IEdmStringConstantExpression stringConstantExp = Assert.IsAssignableFrom<IEdmStringConstantExpression>(annotationWithDefaultValue.Value);
            Assert.Equal("This is a test", stringConstantExp.Value);

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            await CsdlWriterTests.WriteAndVerifyXmlAsync(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.MyAnnotationPathTerm\" AnnotationPath=\"abc/efg\" />" +
                     "<Annotation Term=\"NS.MyDefaultTerm\" />" + // no longer has value
                   "</ComplexType>" +
                   "<Term Name=\"MyAnnotationPathTerm\" Type=\"Edm.AnnotationPath\" Nullable=\"false\" />" +
                   "<Term Name=\"MyDefaultTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>").ConfigureAwait(false);
        }

        [Fact]
        public async Task ReadWriteAnnotationUsingDefaultValueWithContinuity_Async()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyDefaultBoolTerm1\" />" + // does not specify value
                         "<Annotation Term=\"NS.MyDefaultBoolTerm2\" Bool=\"true\" />" + // specifies default
                         "<Annotation Term=\"NS.MyDefaultBoolTerm3\" Bool=\"false\" />" + // specifies non-default
                       "</ComplexType>" +
                       "<Term Name=\"MyDefaultBoolTerm1\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm2\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm3\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                      "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>";
            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmTerm defaultTerm = model.FindTerm("NS.MyDefaultBoolTerm1");
            Assert.True(bool.Parse(defaultTerm.DefaultValue));
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(3, annotations.Length);
            IEdmVocabularyAnnotation annotation1 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[0]);
            IEdmBooleanConstantExpression annotationValue1 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation1.Value);
            Assert.True(annotationValue1.Value);

            IEdmVocabularyAnnotation annotation2 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[1]);
            IEdmBooleanConstantExpression annotationValue2 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation2.Value);
            Assert.True(annotationValue2.Value);

            IEdmVocabularyAnnotation annotation3 = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[2]);
            IEdmBooleanConstantExpression annotationValue3 = Assert.IsAssignableFrom<IEdmBooleanConstantExpression>(annotation3.Value);
            Assert.False(annotationValue3.Value);

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            await CsdlWriterTests.WriteAndVerifyXmlAsync(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                 "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                   "<edmx:DataServices>" +
                     "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                       "<ComplexType Name=\"Complex\">" +
                         "<Annotation Term=\"NS.MyDefaultBoolTerm1\" />" + // does not specify value
                         "<Annotation Term=\"NS.MyDefaultBoolTerm2\" Bool=\"true\" />" + // for back compatiable, write the default value again.
                         "<Annotation Term=\"NS.MyDefaultBoolTerm3\" Bool=\"false\" />" + // specifies non-default
                       "</ComplexType>" +
                       "<Term Name=\"MyDefaultBoolTerm1\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm2\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                       "<Term Name=\"MyDefaultBoolTerm3\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                     "</Schema>" +
                   "</edmx:DataServices>" +
                 "</edmx:Edmx>").ConfigureAwait(false);
        }

        [Fact]
        public async Task ReadAnnotationWithoutSpecifiedValueAndUsePrimitiveDefaultValues_Async()
        {
            CsdlXmlWriterSettings writerSettings = new CsdlXmlWriterSettings
            {
                LibraryCompatibility = EdmLibraryCompatibility.UseLegacyVariableCasing
            };

            var csdl = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.DefaultBinaryTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDecimalTerm\" />" +
                     "<Annotation Term=\"NS.DefaultStringTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDurationTerm\" />" +
                     "<Annotation Term=\"NS.DefaultTODTerm\" />" +
                     "<Annotation Term=\"NS.DefaultBoolTerm\" />" +
                     "<Annotation Term=\"NS.DefaultSingleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDoubleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultGuidTerm\" />" +
                     "<Annotation Term=\"NS.DefaultInt16Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt32Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt64Term\" />" +
                     "<Annotation Term=\"NS.DefaultDateTerm\" />" +
                   "</ComplexType>" +
                 "<Term Name=\"DefaultBinaryTerm\" Type=\"Edm.Binary\" DefaultValue=\"01\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDecimalTerm\" Type=\"Edm.Decimal\" DefaultValue=\"0.34\" AppliesTo=\"Property Term\" Nullable=\"false\" Scale=\"Variable\" />" +
                 "<Term Name=\"DefaultStringTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDurationTerm\" Type=\"Edm.Duration\" DefaultValue=\"P11DT23H59M59.999999999999S\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultTODTerm\" Type=\"Edm.TimeOfDay\" DefaultValue=\"21:45:00\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultBoolTerm\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultSingleTerm\" Type=\"Edm.Single\" DefaultValue=\"0.2\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDoubleTerm\" Type=\"Edm.Double\" DefaultValue=\"3.94\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultGuidTerm\" Type=\"Edm.Guid\" DefaultValue=\"21EC2020-3AEA-1069-A2DD-08002B30309D\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt16Term\" Type=\"Edm.Int16\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt32Term\" Type=\"Edm.Int32\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt64Term\" Type=\"Edm.Int64\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDateTerm\" Type=\"Edm.Date\" DefaultValue=\"2000-12-10\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>";
            // Parse into CSDL
            IEdmModel model;
            using (XmlReader xr = XElement.Parse(csdl).CreateReader())
            {
                model = CsdlReader.Parse(xr);
            }
            IEdmVocabularyAnnotation[] annotations = model.VocabularyAnnotations.ToArray();
            Assert.Equal(13, annotations.Length);
            for (int i = 0; i < annotations.Length; i++)
            {
                IEdmVocabularyAnnotation annotationWithDefaultValue = Assert.IsAssignableFrom<IEdmVocabularyAnnotation>(annotations[i]);
                Assert.NotNull(annotationWithDefaultValue.Value);
            }

            // Validate model
            IEnumerable<EdmError> errors;
            bool validated = model.Validate(out errors);
            Assert.True(validated);

            // Act & Assert for Reserialized XML
            await CsdlWriterTests.WriteAndVerifyXmlAsync(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
             "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
               "<edmx:DataServices>" +
                 "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                   "<ComplexType Name=\"Complex\">" +
                     "<Annotation Term=\"NS.DefaultBinaryTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDecimalTerm\" />" +
                     "<Annotation Term=\"NS.DefaultStringTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDurationTerm\" />" +
                     "<Annotation Term=\"NS.DefaultTODTerm\" />" +
                     "<Annotation Term=\"NS.DefaultBoolTerm\" />" +
                     "<Annotation Term=\"NS.DefaultSingleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultDoubleTerm\" />" +
                     "<Annotation Term=\"NS.DefaultGuidTerm\" />" +
                     "<Annotation Term=\"NS.DefaultInt16Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt32Term\" />" +
                     "<Annotation Term=\"NS.DefaultInt64Term\" />" +
                     "<Annotation Term=\"NS.DefaultDateTerm\" />" +
                   "</ComplexType>" +
                 "<Term Name=\"DefaultBinaryTerm\" Type=\"Edm.Binary\" DefaultValue=\"01\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDecimalTerm\" Type=\"Edm.Decimal\" DefaultValue=\"0.34\" AppliesTo=\"Property Term\" Nullable=\"false\" Scale=\"Variable\" />" +
                 "<Term Name=\"DefaultStringTerm\" Type=\"Edm.String\" DefaultValue=\"This is a test\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDurationTerm\" Type=\"Edm.Duration\" DefaultValue=\"P11DT23H59M59.999999999999S\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultTODTerm\" Type=\"Edm.TimeOfDay\" DefaultValue=\"21:45:00\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultBoolTerm\" Type=\"Edm.Boolean\" DefaultValue=\"true\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultSingleTerm\" Type=\"Edm.Single\" DefaultValue=\"0.2\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDoubleTerm\" Type=\"Edm.Double\" DefaultValue=\"3.94\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultGuidTerm\" Type=\"Edm.Guid\" DefaultValue=\"21EC2020-3AEA-1069-A2DD-08002B30309D\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt16Term\" Type=\"Edm.Int16\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt32Term\" Type=\"Edm.Int32\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultInt64Term\" Type=\"Edm.Int64\" DefaultValue=\"4\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "<Term Name=\"DefaultDateTerm\" Type=\"Edm.Date\" DefaultValue=\"2000-12-10\" AppliesTo=\"Property Term\" Nullable=\"false\" />" +
                 "</Schema>" +
               "</edmx:DataServices>" +
             "</edmx:Edmx>", writerSettings).ConfigureAwait(false);
        }

        static async Task VerifyXmlModelAsync(IEdmModel model, string csdl)
        {
            var stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings() { Async = true });
            var (success, errors) = await CsdlWriter.TryWriteCsdlAsync(model, xmlWriter, CsdlTarget.OData).ConfigureAwait(false);
            Assert.True(success);
            Assert.Empty(errors);
            stream.Position = 0;
            Assert.Contains(csdl, new StreamReader(stream).ReadToEnd());
        }
    }
}
