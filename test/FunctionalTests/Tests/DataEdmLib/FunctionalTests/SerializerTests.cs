//---------------------------------------------------------------------
// <copyright file="SerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializerTests : EdmLibTestCaseBase
    {
        public SerializerTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void SerializerTestComplexTypeWithBaseTypes()
        {
            var actualCsdl = ModelBuilder.ComplexTypeWithBaseType(this.EdmVersion);
            var roundTripCsdl = this.GetSerializerResult(actualCsdl).Select(n => XElement.Parse(n));
            new ConstructiveApiCsdlXElementComparer().Compare(roundTripCsdl.ToList(), actualCsdl.ToList());
        }

        [TestMethod]
        public void SerializerTestStringWithFacets()
        {
            var serializedResult = XElement.Parse(this.GetSerializerResult(ModelBuilder.StringWithFacets()).Single());
            var namespaceName = serializedResult.GetDefaultNamespace().NamespaceName;

            var NullableStringUnboundedUnicode = serializedResult.Descendants(XName.Get("Property", namespaceName)).Single(n => n.Attribute("Name").Value == "NullableStringUnboundedUnicode");
            Assert.AreEqual("max", NullableStringUnboundedUnicode.Attribute("MaxLength").Value, "The MaxLength of NullableStringUnboundedUnicode should be max");

            var SimpleString = serializedResult.Descendants(XName.Get("Property", namespaceName)).Single(n => n.Attribute("Name").Value == "SimpleString");
            Assert.IsNull(SimpleString.Attribute("MaxLength"), "The MaxLength of SimpleString should not exist.");

            var NullableStringMaxLength10 = serializedResult.Descendants(XName.Get("Property", namespaceName)).Single(n => n.Attribute("Name").Value == "NullableStringMaxLength10");
            Assert.IsTrue((new System.Text.RegularExpressions.Regex("\\d+")).IsMatch(NullableStringMaxLength10.Attribute("MaxLength").Value), "The MaxLength of NullableStringMaxLength10 should be of integer.");
        }

        [TestMethod]
        public void SerializerTestNoContentModel()
        {
            IEnumerable<EdmError> actualSerializationErrors;
            IEnumerable<EdmError> expectedSerializationErrors = new EdmLibTestErrors() 
            {
                {0, 0, EdmErrorCode.NoSchemasProduced},
            };
            IEnumerable<string> csdlStrings = this.GetSerializerResult(new EdmModel(), out actualSerializationErrors);
            Assert.AreEqual(0, csdlStrings.Count(), "CsdlWriter.WriteCsdl should not generate no ");
            CompareErrors(actualSerializationErrors, expectedSerializationErrors);
        }

        [TestMethod]
        public void SerializerTestValidNameCheckModel()
        {
            var csdls = this.GetSerializerResult(ModelBuilder.ValidNameCheckModelEdm()).Select(n => XElement.Parse(n));

            var tempCsdl = csdls.Where(n => n.Attribute("Namespace").Value.Equals("a")).Single();

            var roundTripModel = this.GetParserResult(csdls);
            var roundTripCsdls = this.GetSerializerResult(roundTripModel);

            new ConstructiveApiCsdlXElementComparer().Compare(csdls.ToList(), roundTripCsdls.Select(n => XElement.Parse(n)).ToList());
        }

        [TestMethod]
        public void SerializerTestSimpleAllPrimtiveTypesNullableAttribute()
        {
            var testPairs = from explicitNullable in new bool[] { true, false }
                            from isNullable in new bool[] { true, false }
                            select new { isNullable, explicitNullable };

            foreach (var testPair in testPairs)
            {
                var expectedEdmModel = this.GetParserResult(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, testPair.explicitNullable, testPair.isNullable));
                var actualCsdl = this.GetSerializerResult(expectedEdmModel).Select(n => XElement.Parse(n)).Single();
                foreach (var type in expectedEdmModel.SchemaElements.OfType<IEdmStructuredType>())
                {
                    if (type.TypeKind == EdmTypeKind.Entity || type.TypeKind == EdmTypeKind.Complex)
                    {
                        var typeKindName = type.TypeKind == EdmTypeKind.Entity ? "EntityType" : "ComplexType";
                        foreach (var property in type.DeclaredProperties)
                        {
                            var attribute = actualCsdl.Element(XName.Get(typeKindName, actualCsdl.Name.NamespaceName))
                                                    .Elements(XName.Get("Property", actualCsdl.Name.NamespaceName))
                                                         .Single(n => n.Attribute("Name").Value == property.Name).Attribute("Nullable");
                            Assert.AreEqual(property.Type.IsNullable, Boolean.Parse(attribute == null ? "true" : attribute.Value), "The value of Nullable of {0} should be {1}", property.Name, property.Type.IsNullable);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SerializerTestInlineCollectionAtomic()
        {
            var edmModel = this.GetParserResult(ModelBuilder.InlineCollectionAtomic());
            var actualEdmModel
                = this.GetParserResult(this.GetSerializerResult(edmModel).Select(n => XElement.Parse(n)));

            Func<string, string, IEdmStructuralProperty> GetProperty =
                (typeName, propertyName) =>
                    (IEdmStructuralProperty)((IEdmStructuredType)actualEdmModel.FindType(typeName)).FindProperty(propertyName);

            Func<string, string, IEdmOperationParameter> GetParameter =
                (functionName, parameterName) =>
                    (IEdmOperationParameter)(actualEdmModel.FindOperations(functionName)).Single().FindParameter(parameterName);

            Func<string, IEdmTypeReference> GetFunctionReturnType =
                (functionName) => (actualEdmModel.FindOperations(functionName)).Single().ReturnType;

            Func<string, string, IEdmTypeReference> GetFunctionImportReturnType =
                (entityContainerName, functionImportName) => (((IEdmEntityContainer)actualEdmModel.FindEntityContainer(entityContainerName)).FindOperationImports(functionImportName)).Single().Operation.ReturnType;
        }

        [TestMethod]
        public void SerializerTestElementCollectionAtomic()
        {
            var edmModel = this.GetParserResult(ModelBuilder.ElementCollectionAtomic());
            var actualEdmModel
                = this.GetParserResult(this.GetSerializerResult(edmModel).Select(n => XElement.Parse(n)));

            Func<string, string, IEdmOperationParameter> GetParameter =
                (functionName, parameterName) =>
                    (IEdmOperationParameter)(actualEdmModel.FindOperations(functionName)).Single().FindParameter(parameterName);

            Func<string, IEdmTypeReference> GetFunctionReturnType =
                (functionName) => (actualEdmModel.FindOperations(functionName)).Single().ReturnType;
        }

        [TestMethod]
        public void SerializerTestEnumMemberReferenceExpression()
        {
            var expectedCsdlElements = VocabularyTestModelBuilder.OutOfLineAnnotationNavigationProperty();
            IEnumerable<EdmError> edmError = null;
            var actualCsdlElements = this.GetSerializerResult(this.GetParserResult(expectedCsdlElements), EdmLibCsdlContentGenerator.GetEdmVersion(expectedCsdlElements.First().Name.Namespace), out edmError).Select(n => XElement.Parse(n));
            Assert.IsTrue(!edmError.Any(), "Unexpected serializer errors.");
            new SerializerResultVerifierUsingXml().Verify(expectedCsdlElements, actualCsdlElements);
        }

        private IEnumerable<string> GetSerializerResult(IEnumerable<XElement> csdlElements)
        {
            IEdmModel edmModel = this.GetParserResult(csdlElements);

            return this.GetSerializerResult(edmModel);
        }

        [TestMethod]
        public void SerializerTestEntityTypeWithoutKey()
        {
            BasicCsdlToEdmModelSerializerTest(ODataTestModelBuilder.InvalidCsdl.EntityTypeWithoutKey);
        }

        [TestMethod]
        public void SerializerTestDuplicateEntityTypes()
        {
            BasicCsdlToEdmModelSerializerTest(ODataTestModelBuilder.InvalidCsdl.DuplicateEntityTypes);
        }

        [TestMethod]
        public void SerializerTestDuplicateComplexTypes()
        {
            BasicCsdlToEdmModelSerializerTest(ODataTestModelBuilder.InvalidCsdl.DuplicateComplexTypes);
        }

        [TestMethod]
        public void SerializerTestComplexTypeWithDuplicateProperties()
        {
            BasicCsdlToEdmModelSerializerTest(ODataTestModelBuilder.InvalidCsdl.ComplexTypeWithDuplicateProperties);
        }

        [TestMethod]
        public void SerializerTestEntityTypeWithDuplicateProperties()
        {
            BasicCsdlToEdmModelSerializerTest(ODataTestModelBuilder.InvalidCsdl.EntityTypeWithDuplicateProperties);
        }

        [TestMethod]
        public void SerializerTestModelOperationImportReturnTypeWithFacets()
        {
            var model = this.GetParserResult(ModelBuilder.FunctionReturnTypeWithFacets());
            var resultCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

            Assert.AreEqual(1, resultCsdls.Count(), "Invalid count of csdl.");
            var actualCsdl = resultCsdls.First();

            var function = actualCsdl.Elements(XName.Get("Function", "http://docs.oasis-open.org/odata/ns/edm")).First();
            var returnType = function.Elements(XName.Get("ReturnType", "http://docs.oasis-open.org/odata/ns/edm")).Single();
            Assert.AreEqual("false", returnType.Attribute("Nullable").Value, "Invalid Nullable attribute value.");
            Assert.AreEqual("Edm.String", returnType.Attribute("Type").Value, "Invalid Nullable attribute value.");
            Assert.AreEqual("1023", returnType.Attribute("MaxLength").Value, "Invalid Nullable attribute value.");
        }

        [TestMethod]
        public void SerializerTestModelOperationReturnTypeWithDuplicateFacets()
        {
            var csdls = ModelBuilder.OperationReturnTypeWithDuplicateFacets();
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out edmModel, out errors);

            Assert.AreEqual(2, errors.Count(), "Expecting errors.");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlAttribute, errors.ElementAt(0).ErrorCode, "Invalid error code.");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlAttribute, errors.ElementAt(1).ErrorCode, "Invalid error code.");
        }

        [TestMethod]
        public void SerializerTestModelOperationDuplicateReturnType()
        {
            var csdls = ModelBuilder.FunctionDuplicateReturnType();
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdls.Select(e => e.CreateReader()), out edmModel, out errors);

            Assert.AreEqual(1, errors.Count(), "Expecting errors.");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.ElementAt(0).ErrorCode, "Invalid error code.");
        }

        //[TestMethod, Variation(Id = 172, SkipReason = @"[EdmLib] Extra immediate annotation namespace gets generated for ComplexType, EntityType and EntityContainer if namespace name is not expected - postponed")] 
        public void ImmediateAnnotationRoundTrip()
        {
            var model = this.GetParserResult(ODataTestModelBuilder.ImmediateAnnotationRoundTrip);

            Assert.AreEqual(2, model.DirectValueAnnotations(model.FindEntityContainer("DefaultContainer")).Count(), "Invalid immediate annotation count.");
            Assert.AreEqual(2, model.DirectValueAnnotations(model.FindEntityType("DefaultNamespace.PersonType")).Count(), "Invalid immediate annotation count.");
            Assert.AreEqual(2, model.DirectValueAnnotations(model.FindType("DefaultNamespace.Address")).Count(), "Invalid immediate annotation count.");

            var result = this.GetSerializerResult(model).Select(n => XElement.Parse(n)).First();

            var entityContainerElement = result.Elements(XName.Get("EntityContainer", "http://docs.oasis-open.org/odata/ns/edm")).First();
            Assert.AreEqual(3, entityContainerElement.Attributes().Count(), "Invalid attribute count.");

            var entityTypeElement = result.Elements(XName.Get("EntityType", "http://docs.oasis-open.org/odata/ns/edm")).First();
            Assert.AreEqual(3, entityTypeElement.Attributes().Count(), "Invalid attribute count.");

            var complexTypeElement = result.Elements(XName.Get("ComplexType", "http://docs.oasis-open.org/odata/ns/edm")).First();
            Assert.AreEqual(3, complexTypeElement.Attributes().Count(), "Invalid attribute count.");
        }

        [TestMethod]
        public void SerializerTestModelWithCircularPartner()
        {
            var model = ValidationTestModelBuilder.ModelWithCircularNavigationPartner();
            var expectedErrors = new EdmLibTestErrors()
            {
                { "(EdmLibTests.StubEdm.StubEdmNavigationProperty)", EdmErrorCode.InterfaceCriticalNavigationPartnerInvalid }
            };

            IEnumerable<EdmError> edmErrors;
            this.GetSerializerResult(model, out edmErrors);
            this.CompareErrors(expectedErrors, edmErrors);
        }

        [TestMethod]
        public void SerializerTestModelWithInconsistentNavigationPropertyPartner()
        {
            var model = ValidationTestModelBuilder.ModelWithInconsistentNavigationPropertyPartner();
            var resultCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

            Assert.AreEqual(1, resultCsdls.Count(), "Invalid csdl count created.");
        }

        [TestMethod]
        public void SerializerTestEdmCoreModelInstance()
        {
            this.VerifyThrowsException(typeof(InvalidOperationException), () => this.GetSerializerResult(EdmCoreModel.Instance));
        }

        private void BasicCsdlToEdmModelSerializerTest(IEnumerable<XElement> csdls)
        {
            IEdmModel model = this.GetParserResult(csdls);
            List<XElement> expectResults = csdls.ToList<XElement>();
            List<XElement> actualResults = this.GetSerializerResult(model).Select(e => XElement.Parse(e.ToString())).ToList<XElement>();
            (new SerializerResultVerifierUsingXml()).Verify(expectResults, actualResults);
        }
    }
}
