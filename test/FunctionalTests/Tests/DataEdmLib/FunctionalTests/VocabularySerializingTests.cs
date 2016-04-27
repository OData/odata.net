//---------------------------------------------------------------------
// <copyright file="VocabularySerializingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VocabularySerializingTests : EdmLibTestCaseBase
    {
        public VocabularySerializingTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void VocabularySerializingVocabularyTestModels()
        {
            var edmModels = new EdmLibTestModelExtractor().GetModels<IEdmModel>(typeof(VocabularyTestModelBuilder), this.EdmVersion, new CustomVocabularySerializerTestAttribute(), false);
            foreach (var edmModel in edmModels)
            {
                var csdls = new List<XElement>();
                csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, edmModel.Value));
                csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, edmModel.Value));

                IEdmModel parsedModel = this.GetParserResult(csdls);

                var expectedResults = CombineSchemaElements(csdls);
                var actualResults = this.GetSerializerResult(parsedModel).Select(n => XElement.Parse(n));

                Assert.AreEqual(expectedResults.Count(), actualResults.Count(), "The number of the expected CSDLs is different from that of the actual CSDLs.");

                foreach (var expected in expectedResults)
                {
                    var actual = actualResults.Single(n => this.GetSchemaId(expected) == this.GetSchemaId(n));
                    new CsdlXElementComparer().Compare(expected, actual);
                }
            }
        }

        [TestMethod]
        public void VocabularySerializingMultipleValueAnnotations()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.MultipleValueAnnotations());
        }

        [TestMethod]
        public void VocabularySerializingSimpleValueAnnotation()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.SimpleValueAnnotation());
        }

        [TestMethod]
        public void VocabularySerializingSimpleValueAnnotationWithQualifiers()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.SimpleValueAnnotationWithQualifiers());
        }

        [TestMethod]
        public void VocabularySerializingSimpleValueAnnotationConfict()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.SimpleValueAnnotationConfict());
        }

        [TestMethod]
        public void VocabularySerializingStructuredValueAnnotation()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.StructuredValueAnnotation());
        }

        [TestMethod]
        public void VocabularySerializingValueAnnotationWithRecord()
        {
            PerformCustomVocabularySerializerTest(VocabularyTestModelBuilder.ValueAnnotationWithRecord());
        }

        [TestMethod]
        public void VocabularySerializingAnnotationTermsWithNoNamespace()
        {
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = CsdlReader.TryParse(VocabularyTestModelBuilder.AnnotationTermsWithNoNamespace().Select(e => e.CreateReader()), out edmModel, out errors);
            Assert.IsFalse(isParsed, "CsdlReader.TryParse failed");
            Assert.IsFalse(errors.Count() == 0, "CsdlReader.TryParse returned errors");
        }

        [TestMethod]
        public void VocabularySerializingAnnotationsWithWrongTarget()
        {
            IEdmModel expectedModel = this.GetParserResult(VocabularyTestModelBuilder.AnnotationsWithWrongTarget());

            StringWriter actualResult = new StringWriter();
            XmlWriter xw = XmlWriter.Create(actualResult);
            IEnumerable<EdmError> errors;
            expectedModel.TryWriteCsdl(xw, out errors);
            xw.Flush();
            xw.Close();

            Assert.AreNotEqual(string.Empty, actualResult.ToString().Trim(), "The serializer should not generate empty CSDLs");
            var actualModel = this.GetParserResult(new XElement[] { XElement.Parse(actualResult.ToString()) });

            Assert.AreEqual((actualModel.VocabularyAnnotations.Single().Target as IEdmCheckable).Errors.Single().ErrorCode, EdmErrorCode.BadUnresolvedType, "The Target property should have an error of unresolved type.");
        }

        [TestMethod]
        public void VocabularySerializingAnnotationsInlineOnEntityContainer()
        {
            IEdmModel expectedModel = this.GetParserResult(VocabularyTestModelBuilder.AnnotationsInlineOnEntityContainer());

            StringWriter actualResult = new StringWriter();
            XmlWriter xw = XmlWriter.Create(actualResult);
            IEnumerable<EdmError> errors;
            expectedModel.TryWriteCsdl(xw, out errors);
            xw.Flush();
            xw.Close();

            Assert.AreNotEqual(string.Empty, actualResult.ToString().Trim(), "The serializer should not generate empty CSDLs");
            var actualModel = this.GetParserResult(new XElement[] { XElement.Parse(actualResult.ToString()) });

            var actualVocab = actualModel.EntityContainer.VocabularyAnnotations(actualModel);
            Assert.AreEqual(1, actualVocab.Count(), "Invalid vocabulary annotation count.");
        }

        private void PerformCustomVocabularySerializerTest(IEdmModel testEdmModel)
        {
            var csdls = new List<XElement>();
            csdls.AddRange(new VocabularyDefinitionCsdlGenerator().GenerateDefinitionCsdl(this.EdmVersion, testEdmModel));
            csdls.Add(new VocabularyApplicationCsdlGenerator().GenerateApplicationCsdl(this.EdmVersion, testEdmModel));

            var expectedModel = this.GetParserResult(csdls);
            var actualModel = this.GetParserResult(this.GetSerializerResult(expectedModel).Select(XElement.Parse));

            var comparer = new VocabularyModelComparer();
            var compareErrorMessages = comparer.CompareModels(expectedModel, actualModel);

            compareErrorMessages.ToList().ForEach(Console.WriteLine);
            Assert.AreEqual(0, compareErrorMessages.Count, "Comparison errors");
        }

        private string GetSchemaId(XElement schemaElement)
        {
            Func<XElement, string, string> GetSchemaAttribute =
                (element, attributeName)
                => element.Attribute(attributeName) != null ? element.Attribute(attributeName).Value : string.Empty;
            return GetSchemaAttribute(schemaElement, "Namespace") + GetSchemaAttribute(schemaElement, "NamespaceUri");
        }

        private IEnumerable<XElement> CombineSchemaElements(IEnumerable<XElement> sourceElements)
        {
            var sortedElements = sourceElements.Where(m => m.Elements().Any()).OrderBy(n => GetSchemaId(n));
            var resultElements = new List<XElement>();
            resultElements.Add(sortedElements.First());
            var lastResultElementId = GetSchemaId(resultElements.Last());

            for (int i = 1; i < sortedElements.Count(); ++i)
            {
                var temp = sortedElements.ElementAt(i);
                if (GetSchemaId(temp) == lastResultElementId)
                {
                    var cursor = resultElements.Last();
                    foreach (var element in temp.Elements())
                    {
                        if (!cursor.Elements().Any(n => n == element))
                        {
                            cursor.Add(element);
                        }
                    }
                }
                else
                {
                    resultElements.Add(temp);
                    lastResultElementId = GetSchemaId(temp);
                }
            }
            return resultElements;
        }

        [TestMethod]
        public void SerializeSimpleValueAnnotationCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.SimpleValueAnnotationCsdl());
        }

        [TestMethod]
        public void SerializeValueTermNameConflictCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueTermNameConflictCsdl());
        }

        [TestMethod]
        public void SerializeValueTermNameConflictWithOthersCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueTermNameConflictWithOthersCsdl());
        }

        [TestMethod]
        public void SerializeValueTermTypeNotResolvableCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueTermTypeNotResolvableCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationTargetNotResolvableCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationTargetNotResolvableCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationTermNotResolvableCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationTermNotResolvableCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationAmbiguousSameTermNoQualiferCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationAmbiguousSameTermNoQualiferCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationAmbiguousSameTermSameQualiferCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationAmbiguousSameTermSameQualiferCsdl());
        }

        [TestMethod]
        public void SerializeInvalidPropertyValueAnnotationCsdl()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"" />
    <Annotations Target=""DefaultNamespace.Container"">
        <Annotation Term=""foo.HasPhoneNumber"" Int=""0"" />
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""HasPhoneNumber"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ListofFriendsAge"" Type=""Collection(Edm.Int32)"" />
    </EntityType>
</Schema>");
            SerializingValidator(VocabularyTestModelBuilder.InvalidPropertyValueAnnotationCsdl(), actualCsdl);
        }

        [TestMethod]
        public void SerializeValueTermOnlyCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueTermOnlyCsdl());
        }

        [TestMethod]
        public void SerializeValueTermWithAnnotationTargetCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueTermWithAnnotationTargetCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationPropertyTypeExactMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPropertyTypeExactMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationTypeNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationTypeNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationPropertyNameNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPropertyNameNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationNullablePropertyUndeclaredCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationNullablePropertyUndeclaredCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationPropertyTypeNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPropertyTypeNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationNestedCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationNestedCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationNestedPropertyNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationNestedPropertyNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationTypeConvertibleCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationTypeConvertibleCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationBadValueCsdl()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""SByteValue"" Type=""SByte"" />
    <Term Name=""ByteValue"" Type=""Byte"" />
    <Term Name=""Int64Value"" Type=""Int64"" />
    <Term Name=""DoubleValue"" Type=""Double"" />
    <Term Name=""DateTimeValue"" Type=""DateTimeOffset"" />
    <Term Name=""GuidValue"" Type=""Guid"" />
    <ComplexType Name=""Address"">
        <Property Name=""Id"" Type=""Int64"" Nullable=""false""/>
    </ComplexType>  
    <Annotations Target=""Self.Address"">
        <Annotation Term=""My.NS1.SByteValue"" Int=""-129"" />
        <Annotation Term=""My.NS1.ByteValue"" Int=""256"" />

        <Annotation Term=""My.NS1.Int64Value"" Int=""0"" />
        <Annotation Term=""My.NS1.DoubleValue"" Float=""0"" />

        <Annotation Term=""My.NS1.DateTimeValue"" DateTimeOffset=""0001-01-01T00:00:00Z"" />
        <Annotation Term=""My.NS1.GuidValue"" Guid=""00000000-0000-0000-0000-000000000000"" />
    </Annotations>
</Schema>");

            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationBadValueCsdl(), actualCsdl);
        }

        [TestMethod]
        public void SerializeValueAnnotationPathCsdl()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Int32Value"" Type=""Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    </EntityType>  
    <Annotations Target=""Self.Person"">
        <Annotation Term=""My.NS1.Int32Value"" Path=""Id"" />
    </Annotations>
</Schema>");

            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPathCsdl(), actualCsdl);
        }

        [TestMethod]
        public void SerializeValueAnnotationPathNotValidCsdl()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"
<Schema Namespace=""My.NS1"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Int32Value"" Type=""Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    </EntityType>  
    <Annotations Target=""Self.Person"">
        <Annotation Term=""My.NS1.Int32Value"" Path=""Undefined"" />
    </Annotations>
</Schema>");

            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPathNotValidCsdl(), actualCsdl);
        }

        [TestMethod]
        public void SerializeValueAnnotationIfCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationIfCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationIfTypeNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationIfTypeNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationIfTypeNotResolvedCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationIfTypeNotResolvedCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationFunctionCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationFunctionCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationFunctionTypeNotMatchCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationFunctionTypeNotMatchCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationNullablePropertyWithNullExpressionCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationNullablePropertyWithNullExpressionCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationPropertyWithNullExpressionCsdl()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationPropertyWithNullExpressionCsdl());
        }

        [TestMethod]
        public void SerializeSimpleValueAnnotationModel()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"
<Schema Namespace=""Foo"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"">
        <Property Name=""StringValue"" Type=""Edm.String"" Nullable=""true""/>
    </ComplexType>
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Int32Value"" />
        </Key>
        <Property Name=""Int32Value"" Type=""Edm.Int32"" Nullable=""false""/>
    </EntityType>  
    <Term Name=""SimpleTerm"" Type=""Edm.Int32"">
        <Annotation Int=""1"" Term=""Foo.SimpleTerm"" />
    </Term>
    <Term Name=""ComplexTerm"" Type=""Foo.SimpleType"" />
    <Annotations Target=""Foo.ComplexTerm"">
        <Annotation Int=""2"" Term=""Foo.SimpleTerm"" />
    </Annotations>
    <Term Name=""EntityTerm"" Type=""Foo.SimpleEntity"" />
</Schema>");

            SerializingValidator(VocabularyTestModelBuilder.SimpleValueAnnotationModel(), actualCsdl);
        }

        [TestMethod]
        public void SerializeInlineAnnotationWithInvalidTargetModel()
        {
            var actualCsdl = ConvertCsdlsToXElements(@"<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""SimpleType"">
    <Property Name=""StringValue"" Nullable=""true"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""SimpleEntity"">
    <Key>
      <PropertyRef Name=""Int32Value"" />
    </Key>
    <Property Name=""Int32Value"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""InvalidFunction"">
    <Parameter Name=""InvalidParameter"" Nullable=""true"" Type=""Edm.Int32"">
      <Annotation Int=""1"" Term=""Foo.SimpleTerm"" />
    </Parameter>
    <ReturnType Type=""Edm.Int32"" />
    <Annotation Int=""1"" Term=""Foo.SimpleTerm"" />
  </Function>
  <Term Name=""ComplexTerm"" Type=""Foo.SimpleType"" />
  <Term Name=""SimpleTerm"" Type=""Edm.Int32"" />
  <Term Name=""EntityTerm"" Type=""Foo.SimpleEntity"" />
</Schema>");

            SerializingValidator(VocabularyTestModelBuilder.AnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation.Inline), actualCsdl);
        }

        [TestMethod]
        public void SerializeOutOfLineAnnotationWithInvalidTargetModel()
        {
            IEnumerable<EdmError> actualErrors;
            var expectedErrors = new EdmLibTestErrors()
            {
                { "(Microsoft.OData.Edm.Vocabularies.EdmAnnotation)", EdmErrorCode.InvalidName },
                { "(Microsoft.OData.Edm.Vocabularies.EdmAnnotation)", EdmErrorCode.InvalidName },
                { "(Microsoft.OData.Edm.Vocabularies.EdmAnnotation)", EdmErrorCode.InvalidName },
                { "(Microsoft.OData.Edm.Vocabularies.EdmAnnotation)", EdmErrorCode.InvalidName }
            };

            GetSerializerResult(VocabularyTestModelBuilder.AnnotationWithInvalidTargetModel(EdmVocabularyAnnotationSerializationLocation.OutOfLine), out actualErrors);
            CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void SerializeAnnotationWithInvalidTermModel()
        {
            IEnumerable<EdmError> actualErrors;
            var expectedErrors = new EdmLibTestErrors();

            GetSerializerResult(VocabularyTestModelBuilder.AnnotationWithInvalidTermModel(), out actualErrors);
            CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void SerializeSimpleValueAnnotationWithComplexTypeModel()
        {
            SerializingValidator(VocabularyTestModelBuilder.SimpleValueAnnotationWithComplexTypeModel(), VocabularyTestModelBuilder.SimpleValueAnnotationWithComplexTypeCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationComplexTypeWithNullValuesModel()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationComplexTypeWithNullValuesModel(), VocabularyTestModelBuilder.ValueAnnotationComplexTypeWithNullValuesCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationComplexTypeWithFewerPropertiesModel()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationComplexTypeWithFewerPropertiesModel(), VocabularyTestModelBuilder.ValueAnnotationComplexTypeWithFewerPropertiesCsdl());
        }

        [TestMethod]
        public void SerializeValueAnnotationWithCollectionComplexTypeModel()
        {
            SerializingValidator(VocabularyTestModelBuilder.ValueAnnotationWithCollectionComplexTypeModel(), VocabularyTestModelBuilder.ValueAnnotationWithCollectionComplexTypeCsdl());
        }

        [TestMethod]
        public void SerializeAnnotationsOnlyWithNoNamespace()
        {
            SerializingValidator(VocabularyTestModelBuilder.AnnotationsOnlyWithNoNamespace(), VocabularyTestModelBuilder.AnnotationsOnlyWithNoNamespaceCsdl());
        }

        private void SerializingValidator(IEnumerable<XElement> actualCsdls)
        {
            IEdmModel expectedModel = this.GetParserResult(actualCsdls);
            this.SerializingValidator(expectedModel, actualCsdls);
        }

        private void SerializingValidator(IEnumerable<XElement> inputCsdls, IEnumerable<XElement> actualCsdls)
        {
            IEdmModel expectedModel = this.GetParserResult(inputCsdls);
            this.SerializingValidator(expectedModel, actualCsdls);
        }

        private void SerializingValidator(IEdmModel expectedModel, IEnumerable<XElement> actualCsdls)
        {
            var expectedCsdls = this.GetSerializerResult(expectedModel).Select(n => XElement.Parse(n));
            
            var updatedExpectedCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(expectedCsdls.ToArray(), EdmVersion.V40);
            var updatedActualCsdls = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(actualCsdls.ToArray(), EdmVersion.V40);

            new ConstructiveApiCsdlXElementComparer().Compare(updatedExpectedCsdls.ToList(), updatedActualCsdls.ToList());
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }
    }
}
