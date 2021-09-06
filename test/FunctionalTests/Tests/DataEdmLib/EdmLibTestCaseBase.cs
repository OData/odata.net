//---------------------------------------------------------------------
// <copyright file="EdmLibTestCaseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using ApprovalTests;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Framework.Common;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EdmVersion = Microsoft.Test.OData.Utils.Metadata.EdmVersion;
    using ExceptionUtilities = Microsoft.Test.OData.Utils.Common.ExceptionUtilities;

    /// <summary>
    /// Base class for all test cases.
    ///   It derives from Taupo TestCase, so that it can run in Taupo Runner - which already handles dependency injection
    ///   It also defines the Initialization action for MsTest - which will handle dependency injection
    /// </summary>
    [TestClass]
    public class EdmLibTestCaseBase
    {
        protected Dictionary<EdmVersion, Version> toProductVersionlookup = new Dictionary<EdmVersion, Version>()
        {
            { EdmVersion.V40, Microsoft.OData.Edm.EdmConstants.EdmVersion4 }
        };

        private EdmVersion v;
        public EdmVersion EdmVersion {
            get { return this.v; }
            set { this.v = value; }
        }

        public TestContext TestContext { get; set; }

        public Version GetProductVersion(EdmVersion edmVersion)
        {
            return this.toProductVersionlookup[EdmVersion];
        }

        protected IEdmModel GetParserResult(IEnumerable<string> csdlStrings, params IEdmModel[] referencedModels)
        {
            return this.GetParserResult(csdlStrings.Select(XElement.Parse), referencedModels);
        }

        protected IEdmModel GetParserResult(IEnumerable<XElement> csdlElements, params IEdmModel[] referencedModels)
        {
            IEdmModel edmModel;
            IEnumerable<EdmError> errors;
            var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out edmModel, out errors);
            Assert.IsTrue(isParsed, "SchemaReader.TryParse failed");
            Assert.IsTrue(!errors.Any(), "SchemaReader.TryParse returned errors");
            return edmModel;
        }

        protected IEdmModel GetEdmxParserResult(string edmx)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);

            Assert.IsTrue(parsed, "Invalid edmx parsing.");
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            return model;
        }

        protected void BasicRoundtripTest(IEdmModel edmModel, bool skipBaseline = false)
        {
            IEnumerable<EdmError> errors;
            var csdlElements = this.GetSerializerResult(edmModel, out errors).ToArray();
            ExceptionUtilities.Assert(!errors.Any(), "Did not expect serializer errors: " + string.Join(",", errors.Select(e => e.ErrorMessage)));

            if (!skipBaseline)
            {
                var csdl = PrettyPrintCsdl(csdlElements) + Environment.NewLine;
                Approvals.Verify(new ApprovalTextWriter(csdl), new CustomSourcePathNamer(this.TestContext.DeploymentDirectory), Approvals.GetReporter());
            }

            IEdmModel resultEdmModel = this.GetParserResult(csdlElements);
            CsdlToEdmModelComparer.Compare(csdlElements.Select(XElement.Parse), resultEdmModel);
        }

        protected void BasicRoundtripTest(IEnumerable<XElement> csdlElements)
        {
            IEdmModel resultEdmModel = this.GetParserResult(csdlElements);
            CsdlToEdmModelComparer.Compare(csdlElements, resultEdmModel);

            IEnumerable<EdmError> errors;
            var resultCsdlElements = this.GetSerializerResult(resultEdmModel, out errors).ToArray();
            ExceptionUtilities.Assert(!errors.Any(), "Did not expect serializer errors: " + string.Join(",", errors.Select(e => e.ErrorMessage)));

            var csdl = PrettyPrintCsdl(resultCsdlElements) + Environment.NewLine;
            Approvals.Verify(new ApprovalTextWriter(csdl), new CustomSourcePathNamer(this.TestContext.DeploymentDirectory), Approvals.GetReporter());
        }

        protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, EdmVersion edmVersion, out IEnumerable<EdmError> errors)
        {
            // TODO: figure out the best way for multiple schemas
            List<StringBuilder> stringBuilders = new List<StringBuilder>();
            List<XmlWriter> xmlWriters = new List<XmlWriter>();
            edmModel.SetEdmVersion(this.toProductVersionlookup[edmVersion]);
            edmModel.TryWriteSchema(
                s =>
                {
                    stringBuilders.Add(new StringBuilder());
                    xmlWriters.Add(XmlWriter.Create(stringBuilders.Last()));

                    return xmlWriters.Last();
                }, out errors);

            for (int i = 0; i < stringBuilders.Count; i++)
            {
                xmlWriters[i].Close();
            }

            List<string> strings = new List<string>();
            foreach (var sb in stringBuilders)
            {
                strings.Add(sb.ToString());
            }
            return strings;
        }

        protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel)
        {
            IEnumerable<EdmError> errors;
            IEnumerable<string> csdls = this.GetSerializerResult(edmModel, out errors);
            Assert.AreEqual(0, errors.Count(), "Serialization errors were not expected: " + Environment.NewLine + String.Join(Environment.NewLine, errors));
            return csdls;
        }

        protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, out IEnumerable<EdmError> errors)
        {
            return this.GetSerializerResult(edmModel, this.EdmVersion, out errors);
        }

        protected void VerifyThrowsException(Type exceptionType, Action action, string expectedErrorMessageResourceKey)
        {
            bool exceptionThrown = false;
            StringResourceVerifier stringResourceVerifier = null;
            if (!string.IsNullOrEmpty(expectedErrorMessageResourceKey))
            {
                stringResourceVerifier = new StringResourceVerifier(new AssemblyResourceLookup(typeof(IEdmModel).Assembly));
            }
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Assert.AreEqual(exceptionType, ex.GetType(), "Unexpected Exception type!");
                if (stringResourceVerifier != null)
                {
                    Assert.IsTrue(stringResourceVerifier.IsMatch(expectedErrorMessageResourceKey, ex.Message, false), "Unexpected Exception string!");
                }
            }
            Assert.IsTrue(exceptionThrown, "Didn't throw exception!");
        }

        protected void VerifyThrowsException(Type exceptionType, Action action)
        {
            VerifyThrowsException(exceptionType, action, string.Empty);
        }

        protected void BasicFindMethodsTest(IEdmModel model)
        {
            var csdlElements = this.GetSerializerResult(model).Select(XElement.Parse).ToArray();
            BasicFindMethodsTest(csdlElements);
        }

        protected void BasicFindMethodsTest(IEnumerable<XElement> testData)
        {
            testData = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(testData.ToArray(), this.EdmVersion);

            var testModelImmutable = this.GetParserResult(testData);
            this.VerifyFindMethods(testData, testModelImmutable);

            var testModelConstructible = (new EdmToStockModelConverter()).ConvertToStockModel(testModelImmutable);
            IEnumerable<EdmError> serializationErrors;
            var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
            Assert.AreEqual(0, serializationErrors.Count(), "Find method test should not have serialization errors: " + Environment.NewLine + String.Join(Environment.NewLine, serializationErrors));
            this.VerifyFindMethods(testCsdls, testModelConstructible);
        }

        private static string PrettyPrintCsdl(IEnumerable<string> csdls)
        {
            var stringBuilder = new StringBuilder();
            foreach (var csdl in csdls)
            {
                var xml = XElement.Parse(csdl);
                stringBuilder.Append(xml.ToString());
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private void VerifyFindMethods(IEnumerable<XElement> csdls, IEdmModel edmModel)
        {
            foreach (var csdl in csdls)
            {
                this.VerifyFindSchemaElementMethod(csdl, edmModel);
                this.VerifyFindEntityContainer(csdl, edmModel);
                this.VerifyFindEntityContainerElement(csdl, edmModel);
                this.VerifyFindPropertyMethod(csdl, edmModel);
                this.VerifyFindParameterMethod(csdl, edmModel);
                this.VerifyFindNavigationPropertyMethod(csdl, edmModel);
                this.VerifyFindTypeReferencePropertyMethod(csdl, edmModel);
            }
            this.VerifyFindDerivedType(csdls, edmModel);
        }

        private void VerifyFindDerivedType(IEnumerable<XElement> sourceCsdls, IEdmModel testModel)
        {
            Func<IEdmStructuredType, string> getFullName = (type) =>
                {
                    return type as IEdmComplexType != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
                };

            Func<IEdmStructuredType, string> getNamespace = (type) =>
                {
                    return type as IEdmComplexType != null ? ((IEdmComplexType)type).Namespace : type as IEdmEntityType != null ? ((IEdmEntityType)type).Namespace : null;
                };

            Action<IEdmStructuredType, string> checkFindAllDerivedFunction = (structuredType, structuralTypeElementName) =>
                {
                    var expectedResults = EdmLibCsdlContentGenerator.GetDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
                    var actualResults = testModel.FindAllDerivedTypes(structuredType).Select(n => getFullName(n));
                    Assert.IsTrue(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any(),
                                  "FindAllDerivedTypes returns unexpected results.");
                };

            Action<IEdmStructuredType, string> checkFindDirectlyDerivedFunction = (structuredType, structuralTypeElementName) =>
                {
                    var expectedResults = EdmLibCsdlContentGenerator.GetDirectlyDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
                    var actualResults = testModel.FindDirectlyDerivedTypes(structuredType).Select(n => getFullName(n));

                    Assert.IsTrue(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any(),
                                  "FindDirectlyDerivedTypes returns unexpected results.");
                };

            IEnumerable<EdmError> edmErrors;
            testModel.Validate(out edmErrors);

            var structuredTypes = testModel.SchemaElements.OfType<IEdmStructuredType>();
            foreach (var structuredType in structuredTypes)
            {
                if (edmErrors.Any(n => n.ErrorCode == EdmErrorCode.BadCyclicEntity && n.ErrorMessage.Contains(getFullName(structuredType))))
                {
                    Assert.IsTrue(0 == testModel.FindDirectlyDerivedTypes(structuredType).Count(), "FindDirectlyDerivedTypes returns unexpected results.");
                    Assert.IsTrue(0 == testModel.FindAllDerivedTypes(structuredType).Count(), "FindAllDerivedTypes returns unexpected results.");
                }
                else
                {
                    if (structuredType is IEdmEntityType)
                    {
                        checkFindDirectlyDerivedFunction(structuredType, "EntityType");
                        checkFindAllDerivedFunction(structuredType, "EntityType");
                    }
                    else if (structuredType is IEdmComplexType)
                    {
                        checkFindDirectlyDerivedFunction(structuredType, "ComplexType");
                        checkFindAllDerivedFunction(structuredType, "ComplexType");
                    }
                }
            }

        }

        private void VerifyFindNavigationPropertyMethod(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            var namespaceValue = sourceCsdl.Attribute("Namespace").Value;
            var elementTypes = new string[] { "NavigationProperty" };

            foreach (var elementType in elementTypes)
            {
                var elementTypeFoundList = sourceCsdl.Descendants().Elements(XName.Get(elementType, csdlNamespace.NamespaceName));

                foreach (var elementTypeFound in elementTypeFoundList)
                {
                    Assert.IsTrue(new string[] { "EntityType" }.Any(n => n == elementTypeFound.Parent.Name.LocalName), "<NavigationProperty> is used in {0}", elementTypeFound.Parent.Name.LocalName);

                    var entityTypeName = elementTypeFound.Parent.Attribute("Name").Value;
                    var entityType = testModel.FindType(namespaceValue + "." + entityTypeName) as IEdmEntityType;
                    var entityTypeReference = new EdmEntityTypeReference(entityType, true);

                    var navigationFound = entityTypeReference.FindNavigationProperty(elementTypeFound.Attribute("Name").Value);

                    Assert.IsNotNull(navigationFound, "Navigation property cannot found.");
                }
            }
        }

        private void VerifyFindPropertyMethod(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            var namespaceValue = sourceCsdl.Attribute("Namespace").Value;
            var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

            foreach (var schemaElementType in schemaElementTypes)
            {
                Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
                foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
                {
                    IEdmProperty foundProperty = null;
                    Assert.IsTrue(new string[] { "EntityType", "ComplexType", "RowType" }.Any(n => n == propertyElement.Parent.Name.LocalName), "<Property> is used in {0}", propertyElement.Parent.Name.LocalName);

                    if (propertyElement.Parent.Name.LocalName != "RowType")
                    {
                        var schemaElementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent.Attribute("Name").Value);
                        var elementFound = testModel.FindType(schemaElementName) as IEdmStructuredType;
                        foundProperty = elementFound.FindProperty(propertyElement.Attribute("Name").Value);
                    }
                    else if (propertyElement.Parent.Parent.Name.LocalName == "CollectionType")
                    {
                        // TODO: Make VerifyFindPropertyMethod support properties defined in RowType of CollectionType.
                        throw new NotImplementedException("VerifyFindPropertyMethod does not support properties defined in RowType of CollectionType.");
                    }

                    Assert.IsNotNull(foundProperty, "Failed to FindProperty for the property : {0}", propertyElement.Attribute("Name").Value);
                    Assert.AreEqual(foundProperty.Name, propertyElement.Attribute("Name").Value, "FindProperty returns a wrong property for {0}", propertyElement.Attribute("Name").Value);
                }
            }
        }

        private void VerifyFindTypeReferencePropertyMethod(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            var namespaceValue = sourceCsdl.Attribute("Namespace").Value;
            var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

            foreach (var schemaElementType in schemaElementTypes)
            {
                Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
                foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
                {
                    IEdmProperty foundProperty = null;
                    Assert.IsTrue(new string[] { "EntityType", "ComplexType", "RowType" }.Any(n => n == propertyElement.Parent.Name.LocalName), "<Property> is used in {0}", propertyElement.Parent.Name.LocalName);
                    if (propertyElement.Parent.Name.LocalName != "RowType")
                    {
                        var elementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent.Attribute("Name").Value);
                        var elementTypeFound = testModel.FindType(elementName) as IEdmStructuredType;

                        IEdmStructuredTypeReference elementTypeReferenceFound = null;
                        if (propertyElement.Parent.Name.LocalName.Equals("ComplexType"))
                        {
                            elementTypeReferenceFound = new EdmComplexTypeReference((IEdmComplexType)elementTypeFound, true);
                        }
                        else if (propertyElement.Parent.Name.LocalName.Equals("EntityType"))
                        {
                            elementTypeReferenceFound = new EdmEntityTypeReference((IEdmEntityType)elementTypeFound, true);
                        }

                        foundProperty = elementTypeReferenceFound.FindProperty(propertyElement.Attribute("Name").Value);
                    }
                    else if (propertyElement.Parent.Parent.Name.LocalName == "CollectionType")
                    {
                        // TODO: Make VerifyFindPropertyMethod support properties defined in RowType of CollectionType.
                        throw new NotImplementedException("VerifyFindPropertyMethod does not support properties defined in RowType of CollectionType.");
                    }

                    Assert.IsNotNull(foundProperty, "Failed to FindProperty for the property : {0}", propertyElement.Attribute("Name").Value);
                    Assert.AreEqual(foundProperty.Name, propertyElement.Attribute("Name").Value, "FindProperty returns a wrong property for {0}", propertyElement.Attribute("Name").Value);
                }
            }
        }

        private void VerifyFindParameterMethod(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            var namespaceValue = sourceCsdl.Attribute("Namespace").Value;
            var schemaElementTypes = new string[] { "Parameter" };

            foreach (var schemaElementType in schemaElementTypes)
            {
                Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

                foreach (var parameterElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
                {
                    IEdmOperationParameter parameterFound = null;
                    var parameterName = parameterElement.Attribute("Name").Value;
                    Assert.IsTrue(new string[] {"Function", "Action"}.Any(n => n == parameterElement.Parent.Name.LocalName), "<Parameter> is used in {0}", parameterElement.Parent.Name.LocalName);

                    IEdmOperation elementFound;

                    var schemaElementName = string.Format("{0}.{1}", namespaceValue, parameterElement.Parent.Attribute("Name").Value);
                    elementFound = testModel.FindOperations(schemaElementName).Where(n => n.FindParameter(parameterName) != null).FirstOrDefault();

                    parameterFound = elementFound.FindParameter(parameterName);

                    Assert.IsNotNull(parameterFound, "Faild to FindParameter for the parameter : {0}", parameterName);
                    Assert.AreEqual(parameterFound.Name, parameterName, "FindParameter returns a wrong parameter for {0}", parameterElement.Attribute("Name").Value);
                }
            }
        }

        protected void VerifyFindSchemaElementMethod(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            IEnumerable<string> schemaElementTypes = new string[] { "EntityType", "ComplexType", "Function", "Term" };

            // TODO: Function should be filtered based on the CSDL version; It is supported from CSDL 2.0.
            // TODO: What is the expected behavior of the parser when a CSDL of 1.0 has Functions.
            var namespaceValue = sourceCsdl.Attribute("Namespace").Value;
            foreach (var schemaElementType in schemaElementTypes)
            {
                var schemaElements = from element in sourceCsdl.Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName))
                                     select element;
                Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

                foreach (var schemaElement in schemaElements)
                {
                    var elementNameExpected = string.Format("{0}.{1}", namespaceValue, schemaElement.Attribute("Name").Value);
                    Console.WriteLine("FindSchemaType for {0}", elementNameExpected);

                    var typeFound = testModel.FindType(elementNameExpected);
                    Assert.AreEqual(typeFound, testModel.FindDeclaredType(elementNameExpected), "The results between FindMethod and its declared version should be same.");

                    var operationGroup = testModel.FindOperations(elementNameExpected);
                    Assert.IsTrue(operationGroup.Count() == testModel.FindDeclaredOperations(elementNameExpected).Count() && !operationGroup.Except(testModel.FindDeclaredOperations(elementNameExpected)).Any(), "The results between FindMethod and its declared version should be same.");

                    var valueTermFound = testModel.FindTerm(elementNameExpected);
                    Assert.AreEqual(valueTermFound, testModel.FindDeclaredTerm(elementNameExpected), "The results between FindMethod and its declared version should be same.");

                    Assert.IsFalse((typeFound == null) && (operationGroup == null) && (valueTermFound == null), "Failed to FindSchemaType for {0}", elementNameExpected);

                    IEdmSchemaElement schemaElementFound = null;
                    if (operationGroup.Count() > 0)
                    {
                        schemaElementFound = (IEdmSchemaElement)operationGroup.First();
                    }
                    else if (typeFound != null)
                    {
                        schemaElementFound = typeFound;
                    }
                    else if (valueTermFound != null)
                    {
                        schemaElementFound = valueTermFound;
                    }

                    Assert.IsTrue(elementNameExpected.Equals(schemaElementFound.FullName(), System.StringComparison.Ordinal), "The found result {0} is different from {1}", schemaElementFound.FullName(), elementNameExpected);
                }
            }
        }

        protected void VerifyFindEntityContainer(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            var entityContainerNames = from element in sourceCsdl.Elements(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                                       select element.Attribute("Name").Value;
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            foreach (var entityContainerName in entityContainerNames)
            {
                Console.WriteLine("FindEntityContainer for {0}", entityContainerName);

                var elementFound = testModel.FindEntityContainer(entityContainerName);

                Assert.AreEqual(elementFound, testModel.EntityContainer, "The results between FindMethod and its declared version should be same.");
                Assert.IsNotNull(elementFound, "Failed to FindEntityContainer for {0}", entityContainerName);
                Assert.IsTrue(entityContainerName.Equals(elementFound.Name, System.StringComparison.Ordinal),
                             "The found result {0} is different from {1}", elementFound.Name, entityContainerName);
            }
        }

        protected void VerifyFindEntityContainerElement(XElement sourceCsdl, IEdmModel testModel)
        {
            var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(this.EdmVersion);
            Assert.AreEqual(csdlNamespace, sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            var entityContainers = from element in sourceCsdl.DescendantsAndSelf(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                                   select element;

            IEnumerable<string> entityContainerElementTypes = new string[] { "EntitySet", "FunctionImport" };

            foreach (var entityContainer in entityContainers)
            {
                var entityContainerElements = from element in entityContainer.Descendants()
                                              where entityContainerElementTypes.Select(n => XName.Get(n, csdlNamespace.NamespaceName)).Any(m => m == element.Name)
                                              select element;
                var entityContainerName = entityContainer.Attribute("Name").Value;
                var entityContainerObj = testModel.FindEntityContainer(entityContainerName) as IEdmEntityContainer;

                foreach (var entityContainerElement in entityContainerElements)
                {
                    var entityContainerElementName = entityContainerElement.Attribute("Name").Value;
                    var entitySetFound = entityContainerObj.FindEntitySet(entityContainerElementName);
                    var functionImportsFound = entityContainerObj.FindOperationImports(entityContainerElementName);

                    IEdmEntityContainerElement entityContainerElementFound = null;
                    var elementsWithSameName = entityContainerElements.Where(n => n.Attribute("Name").Value.Equals(entityContainerElementName)).Count();

                    if (functionImportsFound != null && functionImportsFound.Count() == elementsWithSameName)
                    {
                        entityContainerElementFound = functionImportsFound.First();
                    }
                    else if (entitySetFound != null)
                    {
                        entityContainerElementFound = entitySetFound;
                    }

                    Assert.IsNotNull(entityContainerElementFound, "FindElement failed for {0}.{1}", entityContainerName, entityContainerElementName);
                    Assert.AreEqual
                        (
                            entityContainerElementName,
                            entityContainerElementFound.Name,
                            "FindElement returned a wrong result, {0}, for {1}", entityContainerElementFound.Name, entityContainerElementName
                        );
                    Assert.IsTrue
                        (
                            entityContainerElement.Name.LocalName == "EntitySet" || entityContainerElement.Name.LocalName == "FunctionImport",
                            "FoundElement for {0} returns a wrong element kind", entityContainerName
                        );
                }
            }
        }

        protected void VerifyValidTestModels(Type modelBuilder)
        {
            var csdlsElements = new EdmLibTestModelExtractor().GetModels<IEnumerable<XElement>>(modelBuilder, this.EdmVersion, new ValidationTestInvalidModelAttribute(), false);
            foreach (var csdls in csdlsElements)
            {
                IEnumerable<EdmError> edmErrors;
                var edmModel = this.GetParserResult(csdls.Value);
                edmModel.Validate(out edmErrors);
                Assert.IsFalse(edmErrors.Any(), "The test model of {0} has unexpected validation errors", csdls.Key);
            }
        }

        protected void VerifySemanticValidation(IEdmModel testModel, IEnumerable<EdmError> expectedErrors)
        {
            this.VerifySemanticValidation(testModel, this.EdmVersion, expectedErrors);
        }

        protected void VerifySemanticValidation(IEdmModel testModel, ValidationRuleSet ruleset, IEnumerable<EdmError> expectedErrors)
        {
            // Compare the actual errors of the test models to the expected errors.
            IEnumerable<EdmError> actualErrors = null;
            var validationResult = testModel.Validate(ruleset, out actualErrors);
            Assert.IsTrue(actualErrors.Any() ? !validationResult : validationResult, "The return value of the Validate method does not match the reported validation errors.");
            this.CompareErrors(actualErrors, expectedErrors);

            // Compare the round-tripped immutable model through the CSDL serialized from the original test model against the expected errors.
            Func<EdmVersion> GetEdmVersionFromRuleSet = () =>
            {
                EdmVersion result = EdmVersion.Latest;
                foreach (var edmVersion in new EdmVersion[] {EdmVersion.V40 })
                {
                    var versionRuleSet = ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]);
                    if (versionRuleSet.Count() == ruleset.Count() && !ruleset.Except(versionRuleSet).Any())
                    {
                        result = edmVersion;
                        break;
                    }
                }
                return result;
            };

            IEnumerable<EdmError> serializationErrors;
            var serializedCsdls = this.GetSerializerResult(testModel, GetEdmVersionFromRuleSet(), out serializationErrors).Select(n => XElement.Parse(n));
            if (!serializedCsdls.Any())
            {
                Assert.AreNotEqual(0, serializationErrors.Count(), "Empty models should have associated errors");
                return;
            }

            if (!actualErrors.Any())
            {
                // if the original test model is valid, the round-tripped model should be well-formed and valid.
                IEnumerable<EdmError> parserErrors = null;
                IEdmModel roundtrippedModel = null;
                var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out parserErrors);
                Assert.IsTrue(isWellformed && !parserErrors.Any(), "The model from valid CSDLs should be generated back to well-formed CSDLs.");

                IEnumerable<EdmError> validationErrors = null;
                var isValid = roundtrippedModel.Validate(out validationErrors);
                Assert.IsTrue(!validationErrors.Any() && isValid, "The model from valid CSDLs should be generated back to valid CSDLs.");
            }
            else
            {
                // if the originl test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guarantted.
                IEnumerable<EdmError> parserErrors = null;
                IEdmModel roundtrippedModel = null;
                var isWellformed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out parserErrors);
                Assert.IsTrue(isWellformed, "The parser cannot handle the CSDL that the serializer generated:" + Environment.NewLine + String.Join(Environment.NewLine, parserErrors));
            }
        }

        protected void VerifySemanticValidation(IEdmModel testModel, EdmVersion edmVersion, IEnumerable<EdmError> expectedErrors)
        {
            VerifySemanticValidation(testModel, ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]), expectedErrors);
        }

        protected void VerifySemanticValidation(IEnumerable<XElement> testCsdls, EdmVersion edmVersion, IEnumerable<EdmError> expectedErrors)
        {
            this.VerifySemanticValidation(this.GetParserResult(testCsdls), edmVersion, expectedErrors);
        }

        protected void VerifySemanticValidation(IEnumerable<XElement> testCsdls, IEnumerable<EdmError> expectedErrors)
        {
            this.VerifySemanticValidation(this.GetParserResult(testCsdls), expectedErrors);
        }

        protected void CompareErrors(IEnumerable<EdmError> actualErrors, IEnumerable<EdmError> expectedErrors)
        {
            if (expectedErrors == null)
            {
                expectedErrors = new EdmLibTestErrors();
            }

            actualErrors.ToList().ForEach(e => Console.WriteLine(e.ToString()));

            var copyOfExpectedErrors = new List<EdmError>(expectedErrors);

            Assert.AreEqual(expectedErrors.Count(), actualErrors.Count(), "The number of actual errors does not match the expected errors.");
            foreach (var actualError in actualErrors)
            {
                if (actualError.ErrorCode != EdmErrorCode.BadAmbiguousElementBinding)
                {
                    Assert.IsNotNull(actualError.ErrorLocation, "actualError.ErrorLocation");
                }

                // First try to find an expected error with error location
                var expectedErrorWithLocation = copyOfExpectedErrors.FirstOrDefault(e => e.ErrorCode == actualError.ErrorCode && e.ErrorLocation != null && e.ErrorLocation.ToString() == actualError.ErrorLocation.ToString());
                if (expectedErrorWithLocation != null)
                {
                    copyOfExpectedErrors.Remove(expectedErrorWithLocation);
                    continue;
                }

                // Now try to find an expected error with no expected location
                var expectedErrorWithNoLocation = copyOfExpectedErrors.FirstOrDefault(e => e.ErrorCode == actualError.ErrorCode && e.ErrorLocation == null);
                if (expectedErrorWithNoLocation != null)
                {
                    copyOfExpectedErrors.Remove(expectedErrorWithNoLocation);
                    continue;
                }

                Assert.Fail("Unexpected error : " + actualError);
            }

            Assert.AreEqual(0, copyOfExpectedErrors.Count, "Expected errors not found");
        }

        protected class EdmLibTestErrors : List<EdmError>
        {
            public void Add(int? lineNumber, int? linePostion, EdmErrorCode edmErrorCode)
            {
                EdmLibTestCsdlLocation errorLocation = null;
                if (lineNumber != null && linePostion != null)
                {
                    errorLocation = new EdmLibTestCsdlLocation(lineNumber, linePostion);
                }
                base.Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
            }

            public void Add(string typeName, EdmErrorCode edmErrorCode)
            {
                var errorLocation = new EdmLibTestObjectLocation(typeName);
                base.Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
            }
        }

        private class EdmLibTestObjectLocation : EdmLocation
        {
            /// <summary>
            /// Gets the object type name where the error happens.
            /// </summary>
            public string ObjectTypeName { get; set; }
            public EdmLibTestObjectLocation(string typeName)
            {
                ObjectTypeName = typeName;
            }
            /// <summary>
            /// Gets a string representation of the location.
            /// </summary>
            /// <returns>A string representation of the location.</returns>
            public override string ToString()
            {
                return ObjectTypeName;
            }
        }

        private class EdmLibTestCsdlLocation : EdmLocation
        {
            /// <summary>
            /// Gets the line number in the file.
            /// </summary>
            public int? LineNumber { get; set; }

            /// <summary>
            /// Gets the position in the line.
            /// </summary>
            public int? LinePosition { get; set; }

            public EdmLibTestCsdlLocation(int? lineNumber, int? linePostion)
            {
                this.LineNumber = lineNumber;
                this.LinePosition = linePostion;
            }

            /// <summary>
            /// Gets a string representation of the location.
            /// </summary>
            /// <returns>A string representation of the location.</returns>
            public override string ToString()
            {
                return "(" + Convert.ToString(LineNumber, CultureInfo.InvariantCulture) + ", " + Convert.ToString(LinePosition, CultureInfo.InvariantCulture) + ")";
            }
        }
    }
}
