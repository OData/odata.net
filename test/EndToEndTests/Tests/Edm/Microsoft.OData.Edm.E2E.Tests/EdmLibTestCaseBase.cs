//---------------------------------------------------------------------
// <copyright file="EdmLibTestCaseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests;

public class EdmLibTestCaseBase
{
    protected Dictionary<EdmVersion, Version> toProductVersionlookup = new Dictionary<EdmVersion, Version>()
        {
            { EdmVersion.V40, EdmConstants.EdmVersion4 }
        };

    private EdmVersion v;
    public EdmVersion EdmVersion
    {
        get { return v; }
        set { v = value; }
    }

    public Version GetProductVersion(EdmVersion edmVersion)
    {
        return toProductVersionlookup[EdmVersion];
    }

    protected IEdmModel GetParserResult(IEnumerable<string> csdlStrings, params IEdmModel[] referencedModels)
    {
        return GetParserResult(csdlStrings.Select(XElement.Parse), referencedModels);
    }

    protected IEdmModel GetParserResult(IEnumerable<XElement> csdlElements, params IEdmModel[] referencedModels)
    {
        IEdmModel edmModel;
        IEnumerable<EdmError> errors;
        var isParsed = SchemaReader.TryParse(csdlElements.Select(e => e.CreateReader()), referencedModels, out edmModel, out errors);
        Assert.True(isParsed, "SchemaReader.TryParse failed");
        Assert.True(!errors.Any(), "SchemaReader.TryParse returned errors");
        return edmModel;
    }

    protected IEdmModel GetParserResult(XElement csdlElement, params IEdmModel[] referencedModels)
    {
        IEdmModel edmModel;
        IEnumerable<EdmError> errors;
        var isParsed = SchemaReader.TryParse(new[] { csdlElement.CreateReader() }, referencedModels, out edmModel, out errors);
        Assert.True(isParsed, "SchemaReader.TryParse failed");
        Assert.True(!errors.Any(), "SchemaReader.TryParse returned errors");
        return edmModel;
    }

    protected IEdmModel GetEdmxParserResult(string edmx)
    {
        IEdmModel model;
        IEnumerable<EdmError> errors;
        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);

        Assert.True(parsed, "Invalid edmx parsing.");
        Assert.True(!errors.Any(), "Invalid error count.");
        return model;
    }

    protected void BasicRoundtripTest(IEdmModel edmModel, bool skipBaseline = false)
    {
        IEnumerable<EdmError> errors;
        var csdlElements = GetSerializerResult(edmModel, out errors).ToArray();
        Assert.True(errors.Any(), "Did not expect serializer errors: " + string.Join(",", errors.Select(e => e.ErrorMessage)));

        if (!skipBaseline)
        {
            var csdl = PrettyPrintCsdl(csdlElements) + Environment.NewLine;
            //Approvals.Verify(new ApprovalTextWriter(csdl), new CustomSourcePathNamer(TestContext.DeploymentDirectory), Approvals.GetReporter());
        }

        IEdmModel resultEdmModel = GetParserResult(csdlElements);
        CsdlToEdmModelComparer.Compare(csdlElements.Select(XElement.Parse), resultEdmModel);
    }

    protected void BasicRoundtripTest(IEnumerable<XElement> csdlElements)
    {
        IEdmModel resultEdmModel = GetParserResult(csdlElements);
        CsdlToEdmModelComparer.Compare(csdlElements, resultEdmModel);

        IEnumerable<EdmError> errors;
        var resultCsdlElements = GetSerializerResult(resultEdmModel, out errors).ToArray();
        Assert.True(!errors.Any(), "Did not expect serializer errors: " + string.Join(",", errors.Select(e => e.ErrorMessage)));

        var csdl = PrettyPrintCsdl(resultCsdlElements) + Environment.NewLine;
        //Approvals.Verify(new ApprovalTextWriter(csdl), new CustomSourcePathNamer(TestContext.DeploymentDirectory), Approvals.GetReporter());
    }

    protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, EdmVersion edmVersion, out IEnumerable<EdmError> errors)
    {
        List<StringBuilder> stringBuilders = new List<StringBuilder>();
        List<XmlWriter> xmlWriters = new List<XmlWriter>();
        edmModel.SetEdmVersion(toProductVersionlookup[edmVersion]);
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
        IEnumerable<string> csdls = GetSerializerResult(edmModel, out errors);
        Assert.True(!errors.Any(), "Serialization errors were not expected: " + Environment.NewLine + string.Join(Environment.NewLine, errors));
        return csdls;
    }

    protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, out IEnumerable<EdmError> errors)
    {
        return GetSerializerResult(edmModel, EdmVersion, out errors);
    }

    protected void VerifyThrowsException(Type exceptionType, Action action, string expectedErrorMessageResourceKey)
    {
        bool exceptionThrown = false;
        StringResourceVerifier? stringResourceVerifier = null;
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
            Assert.Equal(exceptionType, ex.GetType());
            if (stringResourceVerifier != null)
            {
                Assert.True(stringResourceVerifier.IsMatch(expectedErrorMessageResourceKey, ex.Message, false), "Unexpected Exception string!");
            }
        }

        Assert.True(exceptionThrown, "Didn't throw exception!");
    }

    protected void VerifyThrowsException(Type exceptionType, Action action)
    {
        VerifyThrowsException(exceptionType, action, string.Empty);
    }

    protected void BasicFindMethodsTest(IEdmModel model)
    {
        var csdlElements = GetSerializerResult(model).Select(XElement.Parse).ToArray();
        BasicFindMethodsTest(csdlElements);
    }

    protected void BasicFindMethodsTest(IEnumerable<XElement> testData)
    {
        testData = ModelBuilderHelpers.ReplaceCsdlNamespacesForEdmVersion(testData.ToArray(), EdmVersion);

        var testModelImmutable = GetParserResult(testData);
        VerifyFindMethods(testData, testModelImmutable);

        var testModelConstructible = new EdmToStockModelConverter().ConvertToStockModel(testModelImmutable);
        IEnumerable<EdmError> serializationErrors;
        var testCsdls = this.GetSerializerResult(testModelConstructible, out serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializationErrors.Count() == 0, "Find method test should not have serialization errors: " + Environment.NewLine + string.Join(Environment.NewLine, serializationErrors));
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
            VerifyFindSchemaElementMethod(csdl, edmModel);
            VerifyFindEntityContainer(csdl, edmModel);
            VerifyFindEntityContainerElement(csdl, edmModel);
            VerifyFindPropertyMethod(csdl, edmModel);
            VerifyFindParameterMethod(csdl, edmModel);
            VerifyFindNavigationPropertyMethod(csdl, edmModel);
            VerifyFindTypeReferencePropertyMethod(csdl, edmModel);
        }
        VerifyFindDerivedType(csdls, edmModel);
    }

    private void VerifyFindDerivedType(IEnumerable<XElement> sourceCsdls, IEdmModel testModel)
    {
        Func<IEdmStructuredType, string> getFullName = (type) =>
        {
            return (type as IEdmComplexType) != null ? ((IEdmComplexType)type).FullName() : type as IEdmEntityType != null ? ((IEdmEntityType)type).FullName() : null;
        };

        Func<IEdmStructuredType, string> getNamespace = (type) =>
        {
            return (type as IEdmComplexType) != null ? ((IEdmComplexType)type).Namespace : type as IEdmEntityType != null ? ((IEdmEntityType)type).Namespace : null;
        };

        Action<IEdmStructuredType, string> checkFindAllDerivedFunction = (structuredType, structuralTypeElementName) =>
        {
            var expectedResults = EdmLibCsdlContentGenerator.GetDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
            var actualResults = testModel.FindAllDerivedTypes(structuredType).Select(n => getFullName(n));
            Assert.True(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any(),
                          "FindAllDerivedTypes returns unexpected results.");
        };

        Action<IEdmStructuredType, string> checkFindDirectlyDerivedFunction = (structuredType, structuralTypeElementName) =>
        {
            var expectedResults = EdmLibCsdlContentGenerator.GetDirectlyDerivedTypes(sourceCsdls, structuralTypeElementName, getFullName(structuredType));
            var actualResults = testModel.FindDirectlyDerivedTypes(structuredType).Select(n => getFullName(n));

            Assert.True(expectedResults.Count() == actualResults.Count() && !expectedResults.Except(actualResults).Any(),
                          "FindDirectlyDerivedTypes returns unexpected results.");
        };

        IEnumerable<EdmError> edmErrors;
        testModel.Validate(out edmErrors);

        var structuredTypes = testModel.SchemaElements.OfType<IEdmStructuredType>();
        foreach (var structuredType in structuredTypes)
        {
            if (edmErrors.Any(n => n.ErrorCode == EdmErrorCode.BadCyclicEntity && n.ErrorMessage.Contains(getFullName(structuredType))))
            {
                Assert.True(!testModel.FindDirectlyDerivedTypes(structuredType).Any(), "FindDirectlyDerivedTypes returns unexpected results.");
                Assert.True(0 == testModel.FindAllDerivedTypes(structuredType).Count(), "FindAllDerivedTypes returns unexpected results.");
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
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var elementTypes = new string[] { "NavigationProperty" };

        foreach (var elementType in elementTypes)
        {
            var elementTypeFoundList = sourceCsdl.Descendants().Elements(XName.Get(elementType, csdlNamespace.NamespaceName));

            foreach (var elementTypeFound in elementTypeFoundList)
            {
                Assert.True(new string[] { "EntityType" }.Any(n => n == elementTypeFound.Parent?.Name.LocalName), $"<NavigationProperty> is used in {elementTypeFound.Parent?.Name.LocalName}");

                var entityTypeName = elementTypeFound.Parent?.Attribute("Name")?.Value;
                var entityType = testModel.FindType(namespaceValue + "." + entityTypeName) as IEdmEntityType;
                var entityTypeReference = new EdmEntityTypeReference(entityType, true);

                var navigationFound = entityTypeReference.FindNavigationProperty(elementTypeFound.Attribute("Name")?.Value);

                Assert.True(navigationFound != null, "Navigation property cannot found.");
            }
        }
    }

    private void VerifyFindPropertyMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
            foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmProperty foundProperty = null;
                Assert.True(new string[] { "EntityType", "ComplexType", "RowType" }.Any(n => n == propertyElement.Parent?.Name.LocalName), $"<Property> is used in {propertyElement.Parent?.Name.LocalName}");

                if (propertyElement.Parent?.Name.LocalName != "RowType")
                {
                    var schemaElementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent?.Attribute("Name")?.Value);
                    var elementFound = testModel.FindType(schemaElementName) as IEdmStructuredType;
                    foundProperty = elementFound.FindProperty(propertyElement.Attribute("Name")?.Value);
                }
                else if (propertyElement.Parent.Parent?.Name.LocalName == "CollectionType")
                {
                    // TODO: Make VerifyFindPropertyMethod support properties defined in RowType of CollectionType.
                    throw new NotImplementedException("VerifyFindPropertyMethod does not support properties defined in RowType of CollectionType.");
                }

                Assert.True(foundProperty != null, $"Failed to FindProperty for the property : {propertyElement.Attribute("Name")?.Value}");
                Assert.True(foundProperty.Name == propertyElement.Attribute("Name")?.Value, $"FindProperty returns a wrong property for {propertyElement.Attribute("Name")?.Value}");
            }
        }
    }

    private void VerifyFindTypeReferencePropertyMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Property", "NavigationProperty" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());
            foreach (var propertyElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmProperty? foundProperty = null;
                Assert.True(new string[] { "EntityType", "ComplexType", "RowType" }.Any(n => n == propertyElement.Parent?.Name.LocalName), $"<Property> is used in {propertyElement.Parent?.Name.LocalName}");
                if (propertyElement.Parent?.Name.LocalName != "RowType")
                {
                    var elementName = string.Format("{0}.{1}", namespaceValue, propertyElement.Parent?.Attribute("Name").Value);
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
                else if (propertyElement.Parent.Parent?.Name.LocalName == "CollectionType")
                {
                    throw new NotImplementedException("VerifyFindPropertyMethod does not support properties defined in RowType of CollectionType.");
                }

                Assert.True(foundProperty != null, $"Failed to FindProperty for the property : {propertyElement.Attribute("Name")?.Value}");
                Assert.True(foundProperty.Name == propertyElement.Attribute("Name")?.Value, $"FindProperty returns a wrong property for {propertyElement.Attribute("Name")?.Value}");
            }
        }
    }

    private void VerifyFindParameterMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        var schemaElementTypes = new string[] { "Parameter" };

        foreach (var schemaElementType in schemaElementTypes)
        {
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            foreach (var parameterElement in sourceCsdl.Descendants().Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName)))
            {
                IEdmOperationParameter? parameterFound = null;
                var parameterName = parameterElement.Attribute("Name")?.Value;
                Assert.True(new string[] { "Function", "Action" }.Any(n => n == parameterElement.Parent?.Name.LocalName), $"<Parameter> is used in {parameterElement.Parent?.Name.LocalName}");

                IEdmOperation? elementFound;

                var schemaElementName = string.Format("{0}.{1}", namespaceValue, parameterElement.Parent?.Attribute("Name")?.Value);
                elementFound = testModel.FindOperations(schemaElementName).Where(n => n.FindParameter(parameterName) != null).FirstOrDefault();

                parameterFound = elementFound?.FindParameter(parameterName);

                Assert.True(parameterFound != null, $"Failed to FindParameter for the parameter : {parameterName}");
                Assert.True(parameterFound.Name == parameterName, $"FindParameter returns a wrong parameter for {parameterElement.Attribute("Name")?.Value}");
            }
        }
    }

    protected void VerifyFindSchemaElementMethod(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        IEnumerable<string> schemaElementTypes = new string[] { "EntityType", "ComplexType", "Function", "Term" };

        // TODO: Function should be filtered based on the CSDL version; It is supported from CSDL 2.0.
        // TODO: What is the expected behavior of the parser when a CSDL of 1.0 has Functions.
        var namespaceValue = sourceCsdl.Attribute("Namespace")?.Value;
        foreach (var schemaElementType in schemaElementTypes)
        {
            var schemaElements = from element in sourceCsdl.Elements(XName.Get(schemaElementType, csdlNamespace.NamespaceName))
                                 select element;
            Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

            foreach (var schemaElement in schemaElements)
            {
                var elementNameExpected = string.Format("{0}.{1}", namespaceValue, schemaElement.Attribute("Name")?.Value);
                Console.WriteLine("FindSchemaType for {0}", elementNameExpected);

                var typeFound = testModel.FindType(elementNameExpected);
                Assert.True(typeFound == testModel.FindDeclaredType(elementNameExpected), "The results between FindMethod and its declared version should be same.");

                var operationGroup = testModel.FindOperations(elementNameExpected);
                Assert.True(operationGroup.Count() == testModel.FindDeclaredOperations(elementNameExpected).Count() && !operationGroup.Except(testModel.FindDeclaredOperations(elementNameExpected)).Any(), "The results between FindMethod and its declared version should be same.");

                var valueTermFound = testModel.FindTerm(elementNameExpected);
                Assert.True(valueTermFound == testModel.FindDeclaredTerm(elementNameExpected), "The results between FindMethod and its declared version should be same.");

                Assert.False(typeFound == null && operationGroup == null && valueTermFound == null, $"Failed to FindSchemaType for {elementNameExpected}");

                IEdmSchemaElement? schemaElementFound = null;
                if (operationGroup?.Count() > 0)
                {
                    schemaElementFound = operationGroup.First();
                }
                else if (typeFound != null)
                {
                    schemaElementFound = typeFound;
                }
                else if (valueTermFound != null)
                {
                    schemaElementFound = valueTermFound;
                }

                Assert.True(elementNameExpected.Equals(schemaElementFound.FullName(), StringComparison.Ordinal), $"The found result {schemaElementFound.FullName()} is different from {elementNameExpected}");
            }
        }
    }

    protected void VerifyFindEntityContainer(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        var entityContainerNames = from element in sourceCsdl.Elements(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                                   select element.Attribute("Name").Value;
        Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

        foreach (var entityContainerName in entityContainerNames)
        {
            Console.WriteLine("FindEntityContainer for {0}", entityContainerName);

            var elementFound = testModel.FindEntityContainer(entityContainerName);

            Assert.True(elementFound == testModel.EntityContainer, "The results between FindMethod and its declared version should be same.");
            Assert.True(elementFound != null, $"Failed to FindEntityContainer for {entityContainerName}");
            Assert.True(entityContainerName.Equals(elementFound.Name, StringComparison.Ordinal),
                         $"The found result {elementFound.Name} is different from {entityContainerName}");
        }
    }

    protected void VerifyFindEntityContainerElement(XElement sourceCsdl, IEdmModel testModel)
    {
        var csdlNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(EdmVersion);
        Assert.True(csdlNamespace == sourceCsdl.Name.Namespace, "The source CSDL's namespace should match the target EDM version of the test cases.");

        Console.WriteLine("Test CSDL:\n\r{0}", sourceCsdl.ToString());

        var entityContainers = from element in sourceCsdl.DescendantsAndSelf(XName.Get("EntityContainer", csdlNamespace.NamespaceName))
                               select element;

        IEnumerable<string> entityContainerElementTypes = new string[] { "EntitySet", "FunctionImport" };

        foreach (var entityContainer in entityContainers)
        {
            var entityContainerElements = from element in entityContainer.Descendants()
                                          where entityContainerElementTypes.Select(n => XName.Get(n, csdlNamespace.NamespaceName)).Any(m => m == element.Name)
                                          select element;
            var entityContainerName = entityContainer.Attribute("Name")?.Value;
            var entityContainerObj = testModel.FindEntityContainer(entityContainerName) as IEdmEntityContainer;

            foreach (var entityContainerElement in entityContainerElements)
            {
                var entityContainerElementName = entityContainerElement.Attribute("Name")?.Value;
                var entitySetFound = entityContainerObj.FindEntitySet(entityContainerElementName);
                var functionImportsFound = entityContainerObj.FindOperationImports(entityContainerElementName);

                IEdmEntityContainerElement? entityContainerElementFound = null;
                var elementsWithSameName = entityContainerElements.Where(n => n.Attribute("Name").Value.Equals(entityContainerElementName)).Count();

                if (functionImportsFound != null && functionImportsFound.Count() == elementsWithSameName)
                {
                    entityContainerElementFound = functionImportsFound.First();
                }
                else if (entitySetFound != null)
                {
                    entityContainerElementFound = entitySetFound;
                }

                Assert.True(entityContainerElementFound != null, $"FindElement failed for {entityContainerName}.{entityContainerElementName}");
                Assert.True
                    (
                        entityContainerElementName == entityContainerElementFound.Name,
                        $"FindElement returned a wrong result, {entityContainerElementFound.Name}, for {entityContainerElementName}"
                    );
                Assert.True
                    (
                        entityContainerElement.Name.LocalName == "EntitySet" || entityContainerElement.Name.LocalName == "FunctionImport",
                        $"FoundElement for {entityContainerName} returns a wrong element kind"
                    );
            }
        }
    }

    protected void VerifyValidTestModels(Type modelBuilder)
    {
        var csdlsElements = new EdmLibTestModelExtractor().GetModels<IEnumerable<XElement>>(modelBuilder, EdmVersion, new ValidationTestInvalidModelAttribute(), false);
        foreach (var csdls in csdlsElements)
        {
            IEnumerable<EdmError> edmErrors;
            var edmModel = this.GetParserResult(csdls.Value);
            edmModel.Validate(out edmErrors);
            Assert.True(!edmErrors.Any(), $"The test model of {csdls.Key} has unexpected validation errors");
        }
    }

    protected void VerifySemanticValidation(IEdmModel testModel, IEnumerable<EdmError> expectedErrors)
    {
        VerifySemanticValidation(testModel, EdmVersion, expectedErrors);
    }

    protected void VerifySemanticValidation(IEdmModel testModel, ValidationRuleSet ruleset, IEnumerable<EdmError> expectedErrors)
    {
        // Compare the actual errors of the test models to the expected errors.
        IEnumerable<EdmError>? actualErrors = null;
        var validationResult = testModel.Validate(ruleset, out actualErrors);
        Assert.True(actualErrors.Any() ? !validationResult : validationResult, "The return value of the Validate method does not match the reported validation errors.");
        CompareErrors(actualErrors, expectedErrors);

        // Compare the round-tripped immutable model through the CSDL serialized from the original test model against the expected errors.
        Func<EdmVersion> GetEdmVersionFromRuleSet = () =>
        {
            EdmVersion result = EdmVersion.Latest;
            foreach (var edmVersion in new EdmVersion[] { EdmVersion.V40 })
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
        var serializedCsdls = GetSerializerResult(testModel, GetEdmVersionFromRuleSet(), out serializationErrors).Select(n => XElement.Parse(n));
        if (!serializedCsdls.Any())
        {
            Assert.True(serializationErrors.Any(), "Empty models should have associated errors");
            return;
        }

        if (!actualErrors.Any())
        {
            // if the original test model is valid, the round-tripped model should be well-formed and valid.
            IEnumerable<EdmError>? parserErrors = null;
            IEdmModel? roundtrippedModel = null;
            var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out parserErrors);
            Assert.True(isWellFormed && !parserErrors.Any(), "The model from valid CSDLs should be generated back to well-formed CSDLs.");

            IEnumerable<EdmError>? validationErrors = null;
            var isValid = roundtrippedModel.Validate(out validationErrors);
            Assert.True(!validationErrors.Any() && isValid, "The model from valid CSDLs should be generated back to valid CSDLs.");
        }
        else
        {
            // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
            IEnumerable<EdmError>? parserErrors = null;
            IEdmModel? roundtrippedModel = null;
            var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out roundtrippedModel, out parserErrors);
            Assert.True(isWellFormed, "The parser cannot handle the CSDL that the serializer generated:" + Environment.NewLine + string.Join(Environment.NewLine, parserErrors));
        }
    }

    protected void VerifySemanticValidation(IEdmModel testModel, EdmVersion edmVersion, IEnumerable<EdmError> expectedErrors)
    {
        VerifySemanticValidation(testModel, ValidationRuleSet.GetEdmModelRuleSet(toProductVersionlookup[edmVersion]), expectedErrors);
    }

    protected void VerifySemanticValidation(IEnumerable<XElement> testCsdls, EdmVersion edmVersion, IEnumerable<EdmError> expectedErrors)
    {
        VerifySemanticValidation(GetParserResult(testCsdls), edmVersion, expectedErrors);
    }

    protected void VerifySemanticValidation(IEnumerable<XElement> testCsdls, IEnumerable<EdmError> expectedErrors)
    {
        VerifySemanticValidation(GetParserResult(testCsdls), expectedErrors);
    }

    protected void CompareErrors(IEnumerable<EdmError> actualErrors, IEnumerable<EdmError> expectedErrors)
    {
        if (expectedErrors == null)
        {
            expectedErrors = new EdmLibTestErrors();
        }

        actualErrors.ToList().ForEach(e => Console.WriteLine(e.ToString()));

        var copyOfExpectedErrors = new List<EdmError>(expectedErrors);

        Assert.True(expectedErrors.Count() == actualErrors.Count(), "The number of actual errors does not match the expected errors.");
        foreach (var actualError in actualErrors)
        {
            if (actualError.ErrorCode != EdmErrorCode.BadAmbiguousElementBinding)
            {
                Assert.NotNull(actualError.ErrorLocation);
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

        Assert.True(!copyOfExpectedErrors.Any(), "Expected errors not found");
    }

    public class EdmLibTestErrors : List<EdmError>
    {
        public void Add(int? lineNumber, int? linePostion, EdmErrorCode edmErrorCode)
        {
            EdmLibTestCsdlLocation? errorLocation = null;
            if (lineNumber != null && linePostion != null)
            {
                errorLocation = new EdmLibTestCsdlLocation(lineNumber, linePostion);
            }
            Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
        }

        public void Add(string typeName, EdmErrorCode edmErrorCode)
        {
            var errorLocation = new EdmLibTestObjectLocation(typeName);
            Add(new EdmError(errorLocation, edmErrorCode, string.Empty));
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
            LineNumber = lineNumber;
            LinePosition = linePostion;
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
