//---------------------------------------------------------------------
// <copyright file="ParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.Linq;
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
    public class ParserTests : EdmLibTestCaseBase
    {
        public ParserTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        /* 
         * TODO: Add test cases for the default vaules of facets such as 
         *  3. String::fixedLength - The fixedLength facet is a Boolean value. The value indicates whether the store requires a string to be fixed length or not (that is, in SqlServer setting this facet to true would require a fixed-length field (char or nchar) instead of variable-length (varchar or nvarchar)).
         *  4. <Property> can define a Nullable facet. The default value is Nullable=true. (Any Property that has a Type of ComplexType, MUST also define a Nullable attribute which MUST be set to false.)
         */
        public void ParseTestStringWithFacets()
        {
            IEdmModel edmModel = GetParserResult(ModelBuilder.StringWithFacets());

            Assert.IsNotNull(edmModel, "The Parser failed to read the CSDL content");
            var contentEntityType = edmModel.SchemaElements.Single() as IEdmEntityType;
            Assert.IsNotNull(edmModel, "The Parser failed to parse an entity type");

            var NullableStringUnboundedUnicode = contentEntityType.FindProperty("NullableStringUnboundedUnicode").Type.AsPrimitive().AsString();
            Assert.IsTrue(NullableStringUnboundedUnicode.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNull(NullableStringUnboundedUnicode.MaxLength, "The parser failed to process the value for the maxLength facet");
            Assert.IsTrue(NullableStringUnboundedUnicode.IsUnicode == true, "The parser failed to process the value of the unicode facet");

            var SimpleString = contentEntityType.FindProperty("SimpleString").Type.AsPrimitive().AsString();
            Assert.IsFalse(SimpleString.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNull(SimpleString.MaxLength, "The parser failed to process the value for the maxLength facet");
            Assert.AreEqual(true, SimpleString.IsUnicode, "The parser failed to process the value of the unicode facet");

            var NullableStringMaxLength10 = contentEntityType.FindProperty("NullableStringMaxLength10").Type.AsPrimitive().AsString();
            Assert.IsFalse(NullableStringMaxLength10.IsUnbounded, "The parser failed to process the value, Max, for the maxLength facet");
            Assert.IsNotNull(NullableStringMaxLength10.MaxLength, "The parser failed to process the value for the maxLength facet");
            Assert.AreEqual(true, NullableStringMaxLength10.IsUnicode, "The parser failed to process the value of the unicode facet");

            var NullableStringUnbounded = contentEntityType.FindProperty("NullableStringUnbounded").Type.AsPrimitive().AsString();
            Assert.IsTrue(NullableStringUnbounded.IsUnicode == false, "The parser failed to process the value of the unicode facet");
        }

        [TestMethod]
        public void ParserTestSimpleAllPrimitiveTypesDefaultValueCheck()
        {
            var csdlElements = ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, false, true);
            IEdmModel resultEdmModel = this.GetParserResult(csdlElements);
            foreach (var schemaElement in resultEdmModel.SchemaElements)
            {
                var structuredType = schemaElement as IEdmStructuredType;
                foreach (var property in structuredType.StructuralProperties())
                {
                    if (property.Type as IEdmTypeReference != null)
                    {
                        var entityType = structuredType as IEdmEntityType;
                        if (entityType == null || !entityType.DeclaredKey.Any(n => n.Name == property.Name))
                        {
                            Assert.AreEqual(true, ((IEdmTypeReference)property.Type).IsNullable, "{0}'s IsNullable's default value should be true.", property.Name);
                        }
                    }
                    if (property.Type as IEdmBinaryTypeReference != null)
                    {
                        var type = (IEdmBinaryTypeReference)property.Type;
                        Assert.IsFalse(type.MaxLength.HasValue, "The default value of non boolean facets is Null.");
                    }
                    else if (property.Type as IEdmStringTypeReference != null)
                    {
                        var type = (IEdmStringTypeReference)property.Type;
                        Assert.IsFalse(type.MaxLength.HasValue, "The default value of non boolean facets is Null.");
                        Assert.AreEqual(true, type.IsUnicode, "IsUnicode's default value should be true.");
                    }
                    else if (property.Type as IEdmTemporalTypeReference != null)
                    {
                        var type = (IEdmTemporalTypeReference)property.Type;
                        Assert.AreEqual(0, type.Precision, "Default value of Precision is 0.");
                    }
                    else if (property.Type as IEdmDecimalTypeReference != null)
                    {
                        var type = (IEdmDecimalTypeReference)property.Type;
                        Assert.IsFalse(type.Precision.HasValue, "Default value of Precision is null.");
                        Assert.IsTrue(type.Scale.HasValue);
                        Assert.AreEqual(type.Scale.Value, 0, "Scale's default value should be 0.");
                    }
                }
            }
        }

        [TestMethod]
        public void ParserTestSimpleAllPrimtiveTypesNullableAttribute()
        {
            var testPairs = from isNullable in new bool[] { true, false }
                            from explicitNullable in new bool[] { true, false }
                            select new { isNullable, explicitNullable };

            foreach (var testPair in testPairs)
            {
                var resultEdmModel = this.GetParserResult(ModelBuilder.SimpleAllPrimitiveTypes(this.EdmVersion, testPair.explicitNullable, testPair.isNullable));
                foreach (var type in resultEdmModel.SchemaElements.OfType<IEdmStructuredType>())
                {
                    foreach (var property in type.DeclaredProperties)
                    {
                        if (property.Name != "Id")
                        {
                            Assert.AreEqual(property.Type.IsNullable, testPair.isNullable, "IsNullable of {0} should be {1}", property.Name, testPair.isNullable);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ParserTestTypeRefFacets()
        {
            var csdlElements = ModelBuilder.TypeRefFacets();
            this.VerifyParserResult(csdlElements, this.GetParserResult(csdlElements));

            var operationGroup = this.GetParserResult(csdlElements).FindOperations("MyNamespace.MyFunction");
            var operation = operationGroup.Single();
            Assert.AreEqual(true, operation.FindParameter("P1").Type.AsCollection().ElementType().IsNullable, "The value of the IsNullable of P1 should be false.");
            Assert.AreEqual(true, operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().IsUnbounded, "The value of the isUnbounded of P2 should be true.");
            Assert.AreEqual(true, operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().IsUnicode, "The value of the IsUnicode of P2 should be true.");
            Assert.IsNull(operation.FindParameter("P2").Type.AsCollection().ElementType().AsString().MaxLength, "The value of the MaxLength of P2 should be null.");
            Assert.AreEqual(false, operation.FindParameter("P2").Type.AsCollection().ElementType().IsNullable, "The value of the IsNullable of P2 should be false.");
            Assert.AreEqual(true, operation.FindParameter("P3").Type.AsCollection().ElementType().IsNullable, "The default value of IsNullable is true.");
            Assert.AreEqual(true, operation.FindParameter("P4").Type.AsCollection().ElementType().IsNullable, "The default value of IsNullable is true.");
            Assert.AreEqual(false, operation.FindParameter("P4").Type.AsCollection().ElementType().IsCollection(), "The value of IsCollection of P4 is false.");
            Assert.IsFalse(operation.FindParameter("P4").Type.AsCollection().ElementType().AsBinary().IsUnbounded, "The value of isUnbounded of P4 is false.");
            Assert.IsNull(operation.FindParameter("P4").Type.AsCollection().ElementType().AsBinary().MaxLength, "The value of the MaxLength of P4 should be null.");
            // TODO: After fixing the related bug, add more verification steps for the TypeRef elements
        }

        [TestMethod]
        public void ParserTestForDifferentUnderlyingXmlReaders()
        {
            var csdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""FunctionImportsWithReturnTypePrimitiveDataType"">
        <Parameter Name=""PrimitiveParameter1"" Type=""Edm.Binary"" />
        <Parameter Name=""PrimitiveParameter2""><Foo /></Parameter>
      </Action>
      <EntityContainer Name=""MyContainer"">
        <ActionImport Name=""FunctionImportsWithReturnTypePrimitiveDataType"" Action=""DefaultNamespace.FunctionImportsWithReturnTypePrimitiveDataType"" />
      </EntityContainer>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            var csdlElements = new XElement[] { XElement.Parse(csdl) };
            bool parsed = SchemaReader.TryParse(new System.Xml.XmlReader[] { System.Xml.XmlReader.Create(new System.IO.StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(2, errors.Count(), "2 errors");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode, "10: Unexpected Xml Element");

            parsed = SchemaReader.TryParse(new System.Xml.XmlReader[] { csdlElements.First().CreateReader() }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(2, errors.Count(), "2 errors");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode, "10: Unexpected Xml Element");
        }

        [TestMethod]
        public void ParserTestForIncorrectCsdl20Namespace()
        {
            var csdls = ModelBuilder.SimpleConstructiveApiTestModel();
            IEdmModel model;
            IEnumerable<EdmError> errors;

            foreach (var csdl in csdls)
            {
                bool parsed = SchemaReader.TryParse(new System.Xml.XmlReader[] { csdl.CreateReader() }, out model, out errors);
                Assert.IsTrue(parsed, "parsed");

                // http://docs.oasis-open.org/odata/ns/edm is a namespace that OData uses. 
                var csdlStringNewNamespace = csdl.ToString();
                parsed = SchemaReader.TryParse(new System.Xml.XmlReader[] { System.Xml.XmlReader.Create(new System.IO.StringReader(csdlStringNewNamespace)) }, out model, out errors);
                Assert.IsTrue(parsed, "parsed");

                csdlStringNewNamespace = csdl.ToString().Replace(csdl.GetDefaultNamespace().NamespaceName, "http://schemas.microsoft.com/ado/2007/09/edm");
                parsed = SchemaReader.TryParse(new System.Xml.XmlReader[] { System.Xml.XmlReader.Create(new System.IO.StringReader(csdlStringNewNamespace)) }, out model, out errors);
                Assert.IsFalse(parsed, "The parser should not support other invalid namespaces than http://docs.oasis-open.org/odata/ns/edm.");
            }
        }

        [TestMethod]
        public void ParserTestEntityTypeWithoutKey()
        {
            var csdls = ODataTestModelBuilder.InvalidCsdl.EntityTypeWithoutKey;
            var model = this.GetParserResult(csdls);

            Assert.IsTrue(model.SchemaElements.Count() == 2, "Invalid schema element count");

            IEdmEntityType entityType = (IEdmEntityType)model.SchemaElements.First();
            Assert.AreEqual("TestModel.EntityTypeWithoutKey", entityType.FullName(), "Invalid entity type full name");
            Assert.AreEqual("EntityTypeWithoutKey", entityType.Name, "Invalid entity type name");

            IEdmEntityContainer entityContainer = model.EntityContainer;
            Assert.AreEqual("DefaultContainer", entityContainer.Name, "Invalid entity container name");
            Assert.IsTrue(entityContainer.Elements.Count() == 2, "Entity container has invalid amount of elements");
            
            IEdmEntitySet entitySet = entityContainer.FindEntitySet("EntityTypeWithoutKey");
            Assert.AreEqual("EntityTypeWithoutKey", entitySet.Name, "Invalid entity set name");
            Assert.AreEqual("TestModel.EntityTypeWithoutKey", entitySet.EntityType().FullName(), "Invalid entity set element type");

            IEdmSingleton singleton = entityContainer.FindSingleton("SingletonWhoseEntityTypeWithoutKey");
            Assert.AreEqual("SingletonWhoseEntityTypeWithoutKey", singleton.Name, "Invalid singleton name");
            Assert.AreEqual("TestModel.EntityTypeWithoutKey", singleton.EntityType().FullName(), "Invalid singleton element type");
            
            var expectedErrors = new EdmLibTestErrors()
            {
                { 4, 10, EdmErrorCode.NavigationSourceTypeHasNoKeys },
                { 7, 6, EdmErrorCode.KeyMissingOnEntityType },
                { 5, 10, EdmErrorCode.NavigationSourceTypeHasNoKeys },
            };

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ParserTestDuplicateEntityTypes()
        {
            var csdls = ODataTestModelBuilder.InvalidCsdl.DuplicateEntityTypes;
            var model = this.GetParserResult(csdls);

            IEdmEntityContainer entityContainer = model.EntityContainer;
            Assert.AreEqual("DefaultContainer", entityContainer.Name, "Invalid entity container name");
            Assert.IsTrue(entityContainer.Elements.Count() == 2, "Entity container has invalid amount of elements");
            Assert.AreEqual(EdmContainerElementKind.EntitySet, entityContainer.Elements.ElementAt(0).ContainerElementKind, "Invalid container element kind");
            Assert.AreEqual(EdmContainerElementKind.EntitySet, entityContainer.Elements.ElementAt(1).ContainerElementKind, "Invalid container element kind");

            IEdmEntitySet entitySetElement1 = (IEdmEntitySet)entityContainer.Elements.ElementAt(0);
            Assert.AreEqual("DuplicateEntityType", entitySetElement1.Name, "Invalid entity set name");
            Assert.AreEqual("TestModel.DuplicateEntityType", entitySetElement1.EntityType().FullName(), "Invalid entity set element type");

            IEdmEntitySet entitySetElement2 = (IEdmEntitySet)entityContainer.Elements.ElementAt(1);
            Assert.AreEqual("DuplicateEntityType", entitySetElement2.Name, "Invalid entity set name");
            Assert.AreEqual("TestModel.DuplicateEntityType", entitySetElement2.EntityType().FullName(), "Invalid entity set element type");

            Assert.IsTrue(model.SchemaElements.Count() == 3, "Invalid schema element count");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind, "Invalid schema element kind");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(1).SchemaElementKind, "Invalid schema element kind");

            IEdmEntityType entityTypeElement1 = (IEdmEntityType)model.SchemaElements.ElementAt(0);
            Assert.AreEqual("TestModel.DuplicateEntityType", entityTypeElement1.FullName(), "Invalid entity type full name");
            Assert.AreEqual("DuplicateEntityType", entityTypeElement1.Name, "Invalid entity type name");
            Assert.IsTrue(entityTypeElement1.Properties().Count() == 1, "Invalid property count");
            Assert.AreEqual("Id", entityTypeElement1.Properties().Single().Name, "Invalid property name");
            Assert.IsTrue(entityTypeElement1.DeclaredKey.Count() == 1, "Invalid declare key count for entity type");

            IEdmEntityType entityTypeElement2 = (IEdmEntityType)model.SchemaElements.ElementAt(1);
            Assert.AreEqual("TestModel.DuplicateEntityType", entityTypeElement2.FullName(), "Invalid entity type full name");
            Assert.AreEqual("DuplicateEntityType", entityTypeElement2.Name, "Invalid entity type name");
            Assert.IsTrue(entityTypeElement2.Properties().Count() == 1, "Invalid property count");
            Assert.AreEqual("Id", entityTypeElement2.Properties().Single().Name, "Invalid property name");
            Assert.IsTrue(entityTypeElement2.DeclaredKey.Count() == 1, "Invalid declare key count for entity type");

            var expectedErrors = new EdmLibTestErrors()
            {
                { 5, 10, EdmErrorCode.DuplicateEntityContainerMemberName },
                { 13, 6, EdmErrorCode.AlreadyDefined },
                { 4, 10, EdmErrorCode.BadUnresolvedEntityType },
                { 5, 10, EdmErrorCode.BadUnresolvedEntityType }
            };

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ParserTestDuplicateComplexTypes()
        {
            var csdls = ODataTestModelBuilder.InvalidCsdl.DuplicateComplexTypes;
            var model = this.GetParserResult(csdls);

            IEdmEntityContainer entityContainer = model.EntityContainer;
            Assert.AreEqual("DefaultContainer", entityContainer.Name, "Invalid entity container name");
            Assert.IsTrue(entityContainer.Elements.Count() == 0, "Entity container has invalid amount of elements");

            Assert.IsTrue(model.SchemaElements.Count() == 3, "Invalid schema element count");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind, "Invalid schema element kind");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(1).SchemaElementKind, "Invalid schema element kind");

            IEdmComplexType complexTypeElement1 = (IEdmComplexType)model.SchemaElements.ElementAt(0);
            Assert.AreEqual("TestModel.DuplicateComplexTypes", complexTypeElement1.FullName(), "Invalid complex type full name");
            Assert.AreEqual("DuplicateComplexTypes", complexTypeElement1.Name, "Invalid complex type name");

            IEdmComplexType complexTypeElement2 = (IEdmComplexType)model.SchemaElements.ElementAt(1);
            Assert.AreEqual("TestModel.DuplicateComplexTypes", complexTypeElement2.FullName(), "Invalid complex type full name");
            Assert.AreEqual("DuplicateComplexTypes", complexTypeElement2.Name, "Invalid complex type name");

            var expectedErrors = new EdmLibTestErrors()
            {
                { 7, 6, EdmErrorCode.AlreadyDefined }
            };

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ParserTestComplexTypeWithDuplicateProperties()
        {
            var csdls = ODataTestModelBuilder.InvalidCsdl.ComplexTypeWithDuplicateProperties;
            var model = this.GetParserResult(csdls);

            IEdmEntityContainer entityContainer = model.EntityContainer;
            Assert.AreEqual("DefaultContainer", entityContainer.Name, "Invalid entity container name");
            Assert.IsTrue(entityContainer.Elements.Count() == 0, "Entity container has invalid amount of elements");

            Assert.IsTrue(model.SchemaElements.Count() == 2, "Invalid schema element count");
            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, model.SchemaElements.ElementAt(0).SchemaElementKind, "Invalid schema element kind");

            IEdmComplexType complexTypeElement = (IEdmComplexType)model.SchemaElements.ElementAt(0);
            Assert.AreEqual("TestModel.DuplicatePropertiesComplexType", complexTypeElement.FullName(), "Invalid complex type full name");
            Assert.AreEqual("DuplicatePropertiesComplexType", complexTypeElement.Name, "Invalid complex type name");

            Assert.IsTrue(complexTypeElement.DeclaredProperties.Count() == 2, "Invalid complex type property count");
            Assert.AreEqual(EdmPropertyKind.Structural, complexTypeElement.DeclaredProperties.ElementAt(0).PropertyKind, "Invalid property kind");
            Assert.AreEqual(EdmPropertyKind.Structural, complexTypeElement.DeclaredProperties.ElementAt(1).PropertyKind, "Invalid property kind");

            IEdmProperty complexProperty1 = (IEdmProperty)complexTypeElement.DeclaredProperties.ElementAt(0);
            Assert.AreEqual("Duplicate", complexProperty1.Name, "Invalid property name");

            IEdmProperty complexProperty2 = (IEdmProperty)complexTypeElement.DeclaredProperties.ElementAt(1);
            Assert.AreEqual("Duplicate", complexProperty2.Name, "Invalid property name");

            var expectedErrors = new EdmLibTestErrors()
            {
                { 6, 10, EdmErrorCode.AlreadyDefined }
            };

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        [TestMethod]
        public void ParserTestEntityTypeWithDuplicateProperties()
        {
            var csdls = ODataTestModelBuilder.InvalidCsdl.EntityTypeWithDuplicateProperties;
            var model = this.GetParserResult(csdls);

            IEdmEntityContainer entityContainer = model.EntityContainer;
            Assert.AreEqual("DefaultContainer", entityContainer.Name, "Invalid entity container name");
            Assert.IsTrue(entityContainer.Elements.Count() == 1, "Entity container has invalid amount of elements");
            Assert.AreEqual(EdmContainerElementKind.EntitySet, entityContainer.Elements.Single().ContainerElementKind, "Invalid container element kind");

            IEdmEntitySet entitySet = (IEdmEntitySet)entityContainer.Elements.Single();
            Assert.AreEqual("DuplicatePropertiesEntityType", entitySet.Name, "Invalid entity set name");
            Assert.AreEqual("TestModel.DuplicatePropertiesEntityType", entitySet.EntityType().FullName(), "Invalid entity set element type");

            Assert.IsTrue(model.SchemaElements.Count() == 2, "Invalid schema element count");

            IEdmEntityType entityType = (IEdmEntityType)model.SchemaElements.First();
            Assert.AreEqual("TestModel.DuplicatePropertiesEntityType", entityType.FullName(), "Invalid entity type full name");
            Assert.AreEqual("DuplicatePropertiesEntityType", entityType.Name, "Invalid entity type name");

            Assert.IsTrue(entityType.Properties().Count() == 3, "Invalid entity type property count");
            Assert.AreEqual(EdmPropertyKind.Structural, entityType.Properties().ElementAt(0).PropertyKind, "Invalid property kind");
            Assert.AreEqual(EdmPropertyKind.Structural, entityType.Properties().ElementAt(1).PropertyKind, "Invalid property kind");
            Assert.AreEqual(EdmPropertyKind.Structural, entityType.Properties().ElementAt(2).PropertyKind, "Invalid property kind");

            Assert.AreEqual("Id", entityType.Properties().ElementAt(0).Name, "Invalid property name");
            Assert.AreEqual("Duplicate", entityType.Properties().ElementAt(1).Name, "Invalid property name");
            Assert.AreEqual("Duplicate", entityType.Properties().ElementAt(2).Name, "Invalid property name");

            var expectedErrors = new EdmLibTestErrors()
            {
                { 12, 10, EdmErrorCode.AlreadyDefined }
            };

            IEnumerable<EdmError> actualErrors = null;
            model.Validate(out actualErrors);
            this.CompareErrors(actualErrors, expectedErrors);
        }

        //[TestMethod, Variation(Id = 171, SkipReason = @"An FK association based on PK-PK selfjoin should not be allowed -- postponed")]
        public void ParsingSelfReferencingNavigationPropertyPrimaryKey()
        {
            var csdl = @"
<Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""PersonToFriend"" Type=""Self.Person"" Partner=""FriendToPerson"">
            <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
        </NavigationProperty>
        <NavigationProperty Name=""FriendToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""PersonToFriend"" />
    </EntityType>
</Schema>";

            var model = this.GetParserResult(new List<string>() { csdl });
            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreNotEqual(0, errors.Count(), "Invalid count of validation errors.");
        }

        //[TestMethod, Variation(Id = 172, SkipReason = @"An FK association based on PK-PK selfjoin should not be allowed -- postponed")]
        public void ParsingSelfReferencingNavigationPropertyPrimaryKeyWithBaseType()
        {
            var csdl = @"
<Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""PersonToEmployee"" Type=""Self.Employee"" Partner=""EmployeeToPerson"" />
    </EntityType>
    <EntityType Name=""Employee"" BaseType=""DefaultNamespace.Person"">
        <Property Name=""Name"" Type=""Edm.String"" />
        <NavigationProperty Name=""EmployeeToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""PersonToEmployee"">
             <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
</Schema>";

            var model = this.GetParserResult(new List<string>() { csdl });
            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreNotEqual(0, errors.Count(), "Invalid count of validation errors.");
        }

        private void VerifyParserResult(IEnumerable<XElement> csdl, IEdmModel model)
        {
            CsdlToEdmModelComparer.Compare(csdl, model);
        }
    }
}
