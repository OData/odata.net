//---------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Microsoft">
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
    using EdmLibTests.FunctionalUtilities;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExtensionMethodsTests : EdmLibTestCaseBase
    {
        public ExtensionMethodsTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void Container_FindEntitySet()
        {
            IEdmModel edmModel = this.GetEdmModel();
            IEdmEntityContainer container = edmModel.EntityContainer;

            var result = container.FindEntitySet("_Not_Exist_");
            Assert.IsNull(result, "Should not find EntitySet {0}", "_Not_Exist_");

            var entitySet = container.Elements.OfType<IEdmEntitySet>().First();
            result = container.FindEntitySet(entitySet.Name);
            Assert.AreEqual(entitySet, result, "Did not find EntitySet {0}", entitySet.Name);
        }

        [TestMethod]
        public void Container_FindEntitySet_with_null()
        {
            IEdmModel edmModel = this.GetEdmModel();
            IEdmEntityContainer container = edmModel.EntityContainer;

            Assert.IsNull(container.FindEntitySet(null));
        }

        [TestMethod]
        public void ComplexType_reference_extensions()
        {
            IEdmModel edmModel = this.GetEdmModel();
            IEdmComplexType derivedComplexType = edmModel.SchemaElements.OfType<IEdmComplexType>().First(c => c.BaseType != null);
            IEdmComplexType baseComplexType = derivedComplexType.BaseComplexType();

            Assert.IsNotNull(baseComplexType, "Base complex type should not be null!");

            IEdmComplexTypeReference derivedComplexTypeRef = (IEdmComplexTypeReference)derivedComplexType.ToTypeReference();

            Assert.AreEqual(baseComplexType, derivedComplexTypeRef.BaseComplexType(), "ComplexTypeReference.BaseComplexType()");
            Assert.AreEqual(baseComplexType, derivedComplexTypeRef.BaseType(), "ComplexTypeReference.BaseType()");

            Assert.AreEqual(derivedComplexType.IsAbstract, derivedComplexTypeRef.IsAbstract(), "StructuralTypeReference.IsAbstract()");
            Assert.AreEqual(derivedComplexType.IsOpen, derivedComplexTypeRef.IsOpen(), "StructuralTypeReference.IsOpen()");

            Assert.AreEqual(derivedComplexType.DeclaredStructuralProperties().Count(), derivedComplexTypeRef.DeclaredStructuralProperties().Count(), "StructuralTypeReference.DeclaredStructuralProperties()");
            Assert.AreEqual(derivedComplexType.StructuralProperties().Count(), derivedComplexTypeRef.StructuralProperties().Count(), "StructuralTypeReference.StructuralProperties()");
        }

        [TestMethod]
        public void EntityType_reference_extensions()
        {
            IEdmModel edmModel = this.GetEdmModel();
            IEdmEntityType derivedEntityType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.BaseType != null);
            IEdmEntityType baseEntityType = derivedEntityType.BaseEntityType();

            Assert.IsNotNull(baseEntityType, "Base entity type should not be null!");

            IEdmEntityTypeReference derivedEntityTypeRef = (IEdmEntityTypeReference)derivedEntityType.ToTypeReference();

            Assert.AreEqual(baseEntityType, derivedEntityTypeRef.BaseEntityType(), "EntityTypeReference.BaseEntityType()");
            Assert.AreEqual(baseEntityType, derivedEntityTypeRef.BaseType(), "EntityTypeReference.BaseType()");

            Assert.AreEqual(derivedEntityType.IsAbstract, derivedEntityTypeRef.IsAbstract(), "StructuralTypeReference.IsAbstract()");
            Assert.AreEqual(derivedEntityType.IsOpen, derivedEntityTypeRef.IsOpen(), "StructuralTypeReference.IsOpen()");

            Assert.AreEqual(derivedEntityType.DeclaredStructuralProperties().Count(), derivedEntityTypeRef.DeclaredStructuralProperties().Count(), "StructuralTypeReference.DeclaredStructuralProperties()");
            Assert.AreEqual(derivedEntityType.StructuralProperties().Count(), derivedEntityTypeRef.StructuralProperties().Count(), "StructuralTypeReference.StructuralProperties()");

            Assert.AreEqual(derivedEntityType.DeclaredNavigationProperties().Count(), derivedEntityTypeRef.DeclaredNavigationProperties().Count(), "EntityTypeReference.DeclaredNavigationProperties()");
            Assert.AreEqual(derivedEntityType.NavigationProperties().Count(), derivedEntityTypeRef.NavigationProperties().Count(), "EntityTypeReference.NavigationProperties()");

            IEdmNavigationProperty result = derivedEntityTypeRef.FindNavigationProperty("_Not_Exist_");
            Assert.IsNull(result, "Should not find Navigation Property {0}", "_Not_Exist_");

            var navigation = derivedEntityType.NavigationProperties().First();
            result = derivedEntityTypeRef.FindNavigationProperty(navigation.Name);
            Assert.AreEqual(navigation, result, "FindNavigationProperty({0})", navigation.Name);
        }

        [TestMethod]
        public void EntityType_reference_findNavigation_with_null()
        {
            IEdmModel edmModel = this.GetEdmModel();
            IEdmEntityType derivedEntityType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.BaseType != null);
            IEdmEntityTypeReference derivedEntityTypeRef = (IEdmEntityTypeReference)derivedEntityType.ToTypeReference();

            this.VerifyThrowsException(typeof(System.ArgumentNullException), () => derivedEntityTypeRef.FindNavigationProperty(null));
        }

        [TestMethod]
        public void Primitive_type_reference_fullName_returns_fullName()
        {
            var primitives = new Dictionary<IEdmPrimitiveTypeReference, string> 
            {
                { EdmCoreModel.Instance.GetBinary(false), "Edm.Binary" },
                { EdmCoreModel.Instance.GetBoolean(false), "Edm.Boolean" },
                { EdmCoreModel.Instance.GetByte(false), "Edm.Byte" },
                { EdmCoreModel.Instance.GetDateTimeOffset(false), "Edm.DateTimeOffset" },
                { EdmCoreModel.Instance.GetDecimal(false), "Edm.Decimal" },
                { EdmCoreModel.Instance.GetDouble(false), "Edm.Double" },
                { EdmCoreModel.Instance.GetGuid(false), "Edm.Guid" },
                { EdmCoreModel.Instance.GetInt16(false), "Edm.Int16" },
                { EdmCoreModel.Instance.GetInt32(false), "Edm.Int32" },
                { EdmCoreModel.Instance.GetInt64(false), "Edm.Int64" },
                { EdmCoreModel.Instance.GetSByte(false), "Edm.SByte" },
                { EdmCoreModel.Instance.GetSingle(false), "Edm.Single" },
                { EdmCoreModel.Instance.GetStream(false), "Edm.Stream" },
                { EdmCoreModel.Instance.GetString(false), "Edm.String" },
                { EdmCoreModel.Instance.GetDuration(false), "Edm.Duration" },
                { EdmCoreModel.Instance.GetDate(false), "Edm.Date" },
                { EdmCoreModel.Instance.GetTimeOfDay(false), "Edm.TimeOfDay" },
            };

            foreach (var p in primitives)
            {
                string expectedFullName = p.Value;
                string actualFullName = p.Key.FullName();
                Assert.AreEqual(expectedFullName, actualFullName, "Wrong FullName.");
            }
        }

        [TestMethod]
        public void Named_type_reference_fullName_returns_fullName()
        {
            IEdmModel edmModel = this.GetEdmModel();

            foreach (var entity in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                var entityTypeRef = new EdmEntityTypeReference(entity, false);

                Assert.IsNotNull(entityTypeRef.FullName(), "EntityTypeReference.FullName()");
                Assert.AreEqual(entity.FullName(), entityTypeRef.FullName(), "EntityTypeReference.FullName()");
            }

            foreach (var complex in edmModel.SchemaElements.OfType<IEdmComplexType>())
            {
                var complexTypeRef = new EdmComplexTypeReference(complex, false);

                Assert.IsNotNull(complexTypeRef.FullName(), "ComplexTypeReference.FullName()");
                Assert.AreEqual(complex.FullName(), complexTypeRef.FullName(), "ComplexTypeReference.FullName()");
            }
        }

        [TestMethod]
        public void FindAllDerivedTypesWordsAcrossModels()
        {
            EdmModel model = new EdmModel();
            EdmModel referencedModel = new EdmModel();

            var A = new EdmEntityType("Referenced", "A");
            var AProp = A.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            A.AddKeys(new[] { AProp });

            var B = new EdmEntityType("Referenced", "B", A);
            var C = new EdmEntityType("Referenced", "C", B);

            var D = new EdmEntityType("Referenced", "D", C);

            referencedModel.AddElements(new[] { A, B, C });
            model.AddReferencedModel(referencedModel);
            model.AddElement(D);

            IEnumerable<IEdmStructuredType> derivedTypes = model.FindAllDerivedTypes(A);
            Assert.AreEqual(3, derivedTypes.Count(), "Correct number of derived types");
            Assert.IsTrue(derivedTypes.Contains(B), "Contains B");
            Assert.IsTrue(derivedTypes.Contains(C), "Contains C");
            Assert.IsTrue(derivedTypes.Contains(D), "Contains D");
        }

        [TestMethod]
        public void Primitive_type_reference_ShortQualifiedName_returns_ShortQualifiedName()
        {
            var primitives = new Dictionary<IEdmPrimitiveTypeReference, string> 
            {
                { EdmCoreModel.Instance.GetBinary(false), "Edm.Binary" },
                { EdmCoreModel.Instance.GetBoolean(false), "Edm.Boolean" },
                { EdmCoreModel.Instance.GetByte(false), "Edm.Byte" },
                { EdmCoreModel.Instance.GetDateTimeOffset(false), "Edm.DateTimeOffset" },
                { EdmCoreModel.Instance.GetDecimal(false), "Edm.Decimal" },
                { EdmCoreModel.Instance.GetDouble(false), "Edm.Double" },
                { EdmCoreModel.Instance.GetGuid(false), "Edm.Guid" },
                { EdmCoreModel.Instance.GetInt16(false), "Edm.Int16" },
                { EdmCoreModel.Instance.GetInt32(false), "Edm.Int32" },
                { EdmCoreModel.Instance.GetInt64(false), "Edm.Int64" },
                { EdmCoreModel.Instance.GetSByte(false), "Edm.SByte" },
                { EdmCoreModel.Instance.GetSingle(false), "Edm.Single" },
                { EdmCoreModel.Instance.GetStream(false), "Edm.Stream" },
                { EdmCoreModel.Instance.GetString(false), "Edm.String" },
                { EdmCoreModel.Instance.GetDuration(false), "Edm.Duration" },
                { EdmCoreModel.Instance.GetDate(false), "Edm.Date" },
                { EdmCoreModel.Instance.GetTimeOfDay(false), "Edm.TimeOfDay" },
            };

            foreach (var p in primitives)
            {
                string expectedFullName = p.Value.Replace("Edm.", string.Empty);
                string actualFullName = p.Key.ShortQualifiedName();
                Assert.AreEqual(expectedFullName, actualFullName, "Wrong FullName.");
            }
        }

        [TestMethod]
        public void Named_type_reference_ShortQualifiedName_returns_ShortQualifiedName()
        {
            IEdmModel edmModel = this.GetEdmModel();

            foreach (var entity in edmModel.SchemaElements.OfType<IEdmEntityType>())
            {
                var entityTypeRef = new EdmEntityTypeReference(entity, true);

                Assert.IsNotNull(entityTypeRef.ShortQualifiedName(), "EntityTypeReference.ShortQualifiedName()");
                Assert.AreEqual(entity.ShortQualifiedName(), entityTypeRef.ShortQualifiedName(), "EntityTypeReference.ShortQualifiedName()");
            }

            foreach (var complex in edmModel.SchemaElements.OfType<IEdmComplexType>())
            {
                var complexTypeRef = new EdmComplexTypeReference(complex, true);

                Assert.IsNotNull(complexTypeRef.ShortQualifiedName(), "ComplexTypeReference.ShortQualifiedName()");
                Assert.AreEqual(complex.ShortQualifiedName(), complexTypeRef.ShortQualifiedName(), "ComplexTypeReference.ShortQualifiedName()");
            }
        }

        [TestMethod]
        public void BuildModelWithGetUInt16()
        {
            var model = new EdmModel();

            var personType = new EdmEntityType("MyNS", "Person");
            personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt16("MyNS", true)));
            model.AddElement(personType);

            string outputText = this.SerializeModel(model);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt16\" UnderlyingType=\"Edm.Int32\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt16\" />\r\n  </EntityType>\r\n</Schema>", outputText);
        }

        [TestMethod]
        public void BuildModelWithGetUInt32()
        {
            var model = new EdmModel();

            var personType = new EdmEntityType("MyNS", "Person");
            personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt32("MyNS", true)));
            model.AddElement(personType);

            string outputText = this.SerializeModel(model);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt32\" UnderlyingType=\"Edm.Int64\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt32\" />\r\n  </EntityType>\r\n</Schema>", outputText);
        }

        [TestMethod]
        public void BuildModelWithGetUInt64()
        {
            var model = new EdmModel();

            var personType = new EdmEntityType("MyNS", "Person");
            personType.AddKeys(personType.AddStructuralProperty("ID", model.GetUInt64("MyNS", true)));
            model.AddElement(personType);

            string outputText = this.SerializeModel(model);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Schema Namespace=\"MyNS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">\r\n  <TypeDefinition Name=\"UInt64\" UnderlyingType=\"Edm.Decimal\" />\r\n  <EntityType Name=\"Person\">\r\n    <Key>\r\n      <PropertyRef Name=\"ID\" />\r\n    </Key>\r\n    <Property Name=\"ID\" Type=\"MyNS.UInt64\" />\r\n  </EntityType>\r\n</Schema>", outputText);
        }

        [TestMethod]
        public void SetAndGetPrimitiveValueConverterShouldWork()
        {
            var model = new EdmModel();
            var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
            var converter = new TestPrimitiveValueConverter();
            model.SetPrimitiveValueConverter(typeReference, converter);
            Assert.AreEqual(model.GetPrimitiveValueConverter(typeReference), converter);
        }

        [TestMethod]
        public void PassNullConverterToSetPrimitiveValueConverterShouldFail()
        {
            var model = new EdmModel();
            var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
            Action setConverter = () => model.SetPrimitiveValueConverter(typeReference, null);
            setConverter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void PassNullModelToSetPrimitiveValueConverterShouldFail()
        {
            EdmModel model = null;
            var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
            var converter = new TestPrimitiveValueConverter();
            Action setConverter = () => model.SetPrimitiveValueConverter(typeReference, converter);
            setConverter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void PassNullTypeReferenceToSetPrimitiveValueConverterShouldFail()
        {
            var model = new EdmModel();
            var converter = new TestPrimitiveValueConverter();
            Action setConverter = () => model.SetPrimitiveValueConverter((IEdmTypeDefinitionReference)null, converter);
            setConverter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void PassNullModelToGetPrimitiveValueConverterShouldFail()
        {
            EdmModel model = null;
            var typeReference = new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), true);
            Action getConverter = () => model.GetPrimitiveValueConverter(typeReference);
            getConverter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void PassNullTypeReferenceToGetPrimitiveValueConverterShouldReturnPassThroughConverter()
        {
            EdmModel model = new EdmModel();
            Assert.AreEqual(model.GetPrimitiveValueConverter((IEdmTypeReference)null), PassThroughPrimitiveValueConverter.Instance);
        }

        [TestMethod]
        public void PassNonTypeDefinitionToGetPrimitiveValueConverterShouldReturnPassThroughConverter()
        {
            EdmModel model = new EdmModel();
            Assert.AreEqual(model.GetPrimitiveValueConverter(EdmCoreModel.Instance.GetInt32(true)), PassThroughPrimitiveValueConverter.Instance);
        }

        [TestMethod]
        public void PassNullModelToGetUIntShouldFail()
        {
            EdmModel model = null;
            Action getConverter = () => model.GetUInt16("MyNS", false);
            getConverter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void PassNullNamespaceToGetUIntShouldFail()
        {
            EdmModel model = new EdmModel();
            Action getConverter = () => model.GetUInt16(null, true);
            getConverter.ShouldThrow<ArgumentNullException>();
        }

        #region referenced model
        [TestMethod]
        public void FindEntitySetInExtendedContainer_1stTest()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            EdmModel model1 = new EdmModel();
            EdmEntityType entityType1 = new EdmEntityType("Namespace1", "EntityType");
            EdmEntityContainer edmEntityContainer1 = new EdmEntityContainer("Namespace1", "Container_sub");
            edmEntityContainer1.AddEntitySet("EntitySet1", entityType1);
            model1.AddElement(edmEntityContainer1);

            IEdmModel mainModel;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            IEdmEntitySet entitySet1 = mainModel.FindDeclaredEntitySet("EntitySet1");
            IEdmEntitySet entitySet1FromContainer = mainModel.EntityContainer.FindEntitySetExtended("EntitySet1");
            Assert.AreEqual(entitySet1, entitySet1FromContainer, "Should find the same entityset.");
            Assert.IsNotNull(entitySet1, "Should find across model and get not-null entity set.");
            Assert.AreEqual("Namespace1.Container_sub", entitySet1.Container.FullName(), "name should be correct.");
        }

        [TestMethod]
        public void FindEntitySetInExtendedContainer_2ndTest()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"">
            <EntitySet Name=""EntitySet1"" EntityType=""Namespace1.VipCustomer"" />
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEnumerable<EdmError> errors;
            IEdmModel model1;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out model1, out errors);
            Assert.IsTrue(parsed);

            IEdmModel mainModel;
            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            IEdmEntitySet entitySet1 = mainModel.FindDeclaredEntitySet("EntitySet1");
            IEdmEntitySet entitySet1FromContainer = mainModel.EntityContainer.FindEntitySetExtended("EntitySet1");
            Assert.AreEqual(entitySet1, entitySet1FromContainer, "Should find the same entityset.");
            Assert.IsNotNull(entitySet1, "Should find across model and get not-null entity set.");
            Assert.AreEqual("Namespace1.Container_sub", entitySet1.Container.FullName(), "name should be correct.");
        }

        [TestMethod]
        public void FindSingletonInExtendedContainer()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"">
            <Singleton Name=""Singleton1"" Type=""Namespace1.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEnumerable<EdmError> errors;
            IEdmModel model1;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out model1, out errors);
            Assert.IsTrue(parsed);

            IEdmModel mainModel;
            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            IEdmSingleton singleton1 = mainModel.FindDeclaredSingleton("Singleton1");
            IEdmSingleton singleton1FromContainer = mainModel.EntityContainer.FindSingletonExtended("Singleton1");
            Assert.AreEqual(singleton1, singleton1FromContainer, "Should find the same singleton.");
            Assert.IsNotNull(singleton1, "Should find across model and get not-null singleton.");
            Assert.AreEqual("Namespace1.Container_sub", singleton1.Container.FullName(), "name should be correct.");
        }

        [TestMethod]
        public void FindOperationImportInExtendedContainer()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <Function Name=""Function1"">
            <ReturnType Type=""Edm.Int16"" />
        </Function>
        <EntityContainer Name=""Container_sub"">
            <FunctionImport Name=""FunctionImport1"" Function=""Namespace1.Function1""/>
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEnumerable<EdmError> errors;
            IEdmModel model1;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out model1, out errors);
            Assert.IsTrue(parsed);

            IEdmModel mainModel;
            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            IEnumerable<IEdmOperationImport> operationImports1 = mainModel.FindDeclaredOperationImports("FunctionImport1");
            IEdmOperationImport operationImport1 = operationImports1.Single();
            IEdmOperationImport operationImport1FromContainer = mainModel.EntityContainer.FindOperationImportsExtended("FunctionImport1").Single();
            Assert.AreEqual("FunctionImport1", operationImport1.Name);
        }

        [TestMethod]
        public void FindEntitySetInExtendedContainer_CircleExceptionTest()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
            <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""Container_sub"" Extends=""Container_sub"">
            <EntitySet Name=""EntitySet1_hdskfhdslfhssdfhdshdssfsa"" EntityType=""Namespace1.VipCustomer"" />
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEnumerable<EdmError> errors;
            IEdmModel model1;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out model1, out errors);
            Assert.IsTrue(parsed);

            IEdmModel mainModel;
            parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            Action action = (() => mainModel.FindDeclaredEntitySet("EntitySet1"));
            action.ShouldThrow<InvalidOperationException>().WithMessage(Microsoft.OData.Edm.Strings.Bad_CyclicEntityContainer("Namespace1.Container_sub"));

            action = (() => mainModel.EntityContainer.FindEntitySetExtended("EntitySet1"));
            action.ShouldThrow<InvalidOperationException>().WithMessage(Microsoft.OData.Edm.Strings.Bad_CyclicEntityContainer("Namespace1.Container_sub"));
        }

        [TestMethod]
        public void AllElementsInExtendedContainerTest()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
        </EntityContainer>  
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            EdmModel model1 = new EdmModel();
            EdmEntityType entityType1 = new EdmEntityType("Namespace1", "EntityType");
            EdmEntityContainer edmEntityContainer1 = new EdmEntityContainer("Namespace1", "Container_sub");
            edmEntityContainer1.AddEntitySet("EntitySet1", entityType1);
            model1.AddElement(edmEntityContainer1);

            IEdmModel mainModel;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out mainModel, out errors);
            Assert.IsTrue(parsed);

            var container = mainModel.EntityContainer;
            Assert.AreEqual(1, container.AllElements().Count());
        }

        [TestMethod]
        public void AllElementsWithCircleRefTest()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx""> 
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityContainer Name=""Container"" Extends=""Namespace0.Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel mainModel;
            IEnumerable<EdmError> errors;

            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), out mainModel, out errors);
            Assert.IsTrue(parsed);

            var container = mainModel.EntityContainer;
            Action action = () => container.AllElements();
            action.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Bad_CyclicEntityContainer("Namespace0.Container"));
        }
        #endregion

        private IEdmModel GetEdmModel()
        {
            return ModelBuilder.ModelWithAllConceptsEdm();
        }

        private string SerializeModel(IEdmModel model)
        {
            IEnumerable<EdmError> errors;

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();

            return sw.ToString();
        }

        private class TestPrimitiveValueConverter : IPrimitiveValueConverter
        {
            public object ConvertToUnderlyingType(object value)
            {
                return value;
            }

            public object ConvertFromUnderlyingType(object value)
            {
                return value;
            }
        }
    }
}
