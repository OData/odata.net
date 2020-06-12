//---------------------------------------------------------------------
// <copyright file="ODataPathParserTypeCastTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.UriParser;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class ODataPathParserTypeCastTests
    {
        #region Type cast with Namespace alias
        [Fact]
        public void ParseTypeCastOnSingletonWithoutAliasSettingThrows()
        {
            IEdmModel edmModel = GetSingletonEdmModel(""); // without Derived type constraint

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));
            Action parsePath = () => pathParser.ParsePath(new[] { "Me", "MyAlias.VipCustomer" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_ResourceNotFound("MyAlias.VipCustomer"));
        }

        [Theory]
        [InlineData("Customer", "NS.")]
        [InlineData("Customer", "MyAlias.")]
        [InlineData("VipCustomer", "NS.")]
        [InlineData("VipCustomer", "MyAlias.")]
        [InlineData("NormalCustomer", "NS.")]
        [InlineData("NormalCustomer", "MyAlias.")]
        public void ParseTypeCastOnSingletonWithNamespaceAndAliasWorks(string typeCastName, string namespaceOrAlias)
        {
            IEdmModel edmModel = GetSingletonEdmModel(""); // without Derived type constraint
            edmModel.SetNamespaceAlias("NS", "MyAlias");

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType targetType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == typeCastName);

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // ~/Me/NS.Customer
            var path = pathParser.ParsePath(new[] { "Me", namespaceOrAlias + typeCastName });
            path[1].ShouldBeTypeSegment(targetType, customer);
        }
        #endregion

        #region Singleton Type cast
        [Theory]
        [InlineData("Customer")]
        [InlineData("VipCustomer")]
        [InlineData("NormalCustomer")]
        public void ParseTypeCastOnSingletonWithoutDerivedTypeConstraintAnnotationWorks(string typeCastName)
        {
            IEdmModel edmModel = GetSingletonEdmModel(""); // without Derived type constraint

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType targetType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == typeCastName);

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // ~/Me/NS.Customer
            var path = pathParser.ParsePath(new[] { "Me", "NS." + typeCastName });
            path[1].ShouldBeTypeSegment(targetType, customer);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnSingletonWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                      "<String>NS.VipCustomer</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetSingletonEdmModel(annotation, isInLine);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself:  ~/Me/NS.Customer
            var path = pathParser.ParsePath(new[] { "Me", "NS.Customer" });
            path[1].ShouldBeTypeSegment(customer, customer);

            // verify the positive type cast:  ~/Me/NS.VipCustomer
            path = pathParser.ParsePath(new[] { "Me", "NS.VipCustomer" });
            path[1].ShouldBeTypeSegment(vipCustomer, customer);

            // verify the negative type cast: ~/Me/NS.NormalCustomer
            Action parsePath = () => pathParser.ParsePath(new[] { "Me", "NS.NormalCustomer" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "singleton", "Me"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnSingletonWithDerivedTypeConstraintButEmptyCollectionAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection />" +
                "</Annotation>";

            IEdmModel edmModel = GetSingletonEdmModel(annotation, isInLine);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself:  ~/Me/NS.Customer
            var path = pathParser.ParsePath(new[] { "Me", "NS.Customer" });
            path[1].ShouldBeTypeSegment(customer, customer);

            // verify the negative type cast: ~/Me/NS.VipCustomer
            Action parsePath = () => pathParser.ParsePath(new[] { "Me", "NS.VipCustomer" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.VipCustomer", "singleton", "Me"));

            // verify the negative type cast: ~/Me/NS.NormalCustomer
            pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));
            parsePath = () => pathParser.ParsePath(new[] { "Me", "NS.NormalCustomer" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "singleton", "Me"));
        }

        private static IEdmModel GetSingletonEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"" />
      <EntityType Name=""VipCustomer"" BaseType=""NS.Customer"" />
      <EntityType Name=""NormalCustomer"" BaseType=""NS.Customer"" />
      <EntityContainer Name =""Default"">
         <Singleton Name=""Me"" Type=""NS.Customer"" >
           {0}
         </Singleton>
      </EntityContainer>
      <Annotations Target=""NS.Default/Me"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion Singleton Type cast

        #region EntitySet or entity Type cast
        [Theory]
        [InlineData("Customer")]
        [InlineData("VipCustomer")]
        [InlineData("NormalCustomer")]
        public void ParseTypeCastOnEntitySetWithoutDerivedTypeConstraintAnnotationWorks(string typeCastName)
        {
            IEdmModel edmModel = GetEntitySetEdmModel(""); /*without Derived type constraint*/

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType castType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == typeCastName);
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));
            IEdmType collectionCastType = new EdmCollectionType(new EdmEntityTypeReference(castType, true));
            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // ~/Customers/NS.Customer
            var path = pathParser.ParsePath(new[] { "Customers", "NS." + typeCastName });
            path[1].ShouldBeTypeSegment(collectionCastType, collectionCustomerType);

            // ~/Customers(1)/NS.Customer
            path = pathParser.ParsePath(new[] { "Customers(1)", "NS." + typeCastName });
            path[2].ShouldBeTypeSegment(castType, customer);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnEntitySetWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                    "<String>NS.VipCustomer</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetEntitySetEdmModel(annotation, isInLine);

            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");
            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));
            IEdmType collectionVipCustomerType = new EdmCollectionType(new EdmEntityTypeReference(vipCustomer, true));

            int index = 1;
            foreach (string segment in new[] { "Customers", "Customers(1)" })
            {
                IEdmType actualType = index == 1 ? collectionCustomerType : customer;
                IEdmType expectVipCustomerType = index == 1 ? collectionVipCustomerType : vipCustomer;

                ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

                // verify the positive type cast on itself:  ~/Customers/NS.Customer
                var path = pathParser.ParsePath(new[] { segment, "NS.Customer" });
                path[index].ShouldBeTypeSegment(actualType, actualType);

                // verify the positive type cast:  ~/Customers/NS.VipCustomer
                path = pathParser.ParsePath(new[] { segment, "NS.VipCustomer" });
                path[index].ShouldBeTypeSegment(expectVipCustomerType, actualType);

                // verify the negative type cast: ~/Customers/NS.NormalCustomer
                Action parsePath = () => pathParser.ParsePath(new[] { segment, "NS.NormalCustomer" });
                parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "entity set", "Customers"));

                index++;
            }
        }

        private static IEdmModel GetEntitySetEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""VipCustomer"" BaseType=""NS.Customer"" />
      <EntityType Name=""NormalCustomer"" BaseType=""NS.Customer"" />
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" >
           {0}
         </EntitySet>
      </EntityContainer>
      <Annotations Target=""NS.Default/Customers"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion EntitySet or entity Type cast

        #region Property type cast
        [Theory]
        [InlineData("Address", "NS.Address")] // cast to itself
        [InlineData("Address", "NS.CnAddress")]
        [InlineData("Address", "NS.UsAddress")]
        [InlineData("Locations", "NS.Address")] // cast to itself
        [InlineData("Locations", "NS.CnAddress")]
        [InlineData("Locations", "NS.UsAddress")]
        public void ParseTypeCastOnPropertyWithoutDerivedTypeConstraintAnnotationWorks(string propertyName, string typeCast)
        {
            IEdmModel edmModel = GetPropertyEdmModel("");

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmProperty property = customer.DeclaredProperties.First(c => c.Name == propertyName);
            IEdmType expectType = edmModel.FindDeclaredType(typeCast);
            if (property.Type.IsCollection())
            {
                expectType = new EdmCollectionType(new EdmComplexTypeReference(expectType as IEdmComplexType, true));
            }
            Assert.NotNull(expectType);

            // ~/Customers(1)/{propertyName}/{typeCast}
            var path = pathParser.ParsePath(new[] { "Customers(1)", propertyName, typeCast });
            path[3].ShouldBeTypeSegment(expectType, property.Type.Definition);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnPropertyWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                      "<String>NS.UsAddress</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetPropertyEdmModel(annotation, isInLine);

            IEdmComplexType address = edmModel.SchemaElements.OfType<IEdmComplexType>().First(c => c.Name == "Address");
            IEdmComplexType usAddress = edmModel.SchemaElements.OfType<IEdmComplexType>().First(c => c.Name == "UsAddress");
            IEdmType collectionAddressType = new EdmCollectionType(new EdmComplexTypeReference(address, true));
            IEdmType collectionUsAddressType = new EdmCollectionType(new EdmComplexTypeReference(usAddress, true));

            int index = 1;
            foreach (string segment in new[] { "Address", "Locations" })
            {
                IEdmType actualType = index == 1 ? address : collectionAddressType;
                IEdmType expectUsAddressType = index == 1 ? usAddress : collectionUsAddressType;

                // verify the positive type cast on itself
                ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));
                var path = pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.Address" });
                path[3].ShouldBeTypeSegment(actualType, actualType);

                // verify the positive type cast
                path = pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.UsAddress" });
                path[3].ShouldBeTypeSegment(expectUsAddressType, actualType);

                // verify the negative type cast
                Action parsePath = () => pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.CnAddress" });
                parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.CnAddress", "property", segment));
                index++;
            }
        }

        private static IEdmModel GetPropertyEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""NS.Address"" >
           {0}
        </Property>
        <Property Name=""Locations"" Type=""Collection(NS.Address)"" >
           {0}
        </Property>
      </EntityType>
      <ComplexType Name=""Address"" />
      <ComplexType Name=""UsAddress"" BaseType=""NS.Address"" />
      <ComplexType Name=""CnAddress"" BaseType=""NS.Address"" />
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
      <Annotations Target=""NS.Customer/Address"">
        {1}
      </Annotations>
      <Annotations Target=""NS.Customer/Locations"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion Property type cast

        #region Navigation Property type cast
        [Theory]
        [InlineData("DirectReport", null, "NS.Customer")] // cast to itself
        [InlineData("DirectReport", null, "NS.VipCustomer")]
        [InlineData("DirectReport", null, "NS.NormalCustomer")]
        [InlineData("SubCustomers", null, "NS.Customer")] // cast to itself
        [InlineData("SubCustomers", null, "NS.VipCustomer")]
        [InlineData("SubCustomers", null, "NS.NormalCustomer")]
        [InlineData("SubCustomers", "(1)", "NS.Customer")] // cast to itself
        [InlineData("SubCustomers", "(1)", "NS.VipCustomer")]
        [InlineData("SubCustomers", "(1)", "NS.NormalCustomer")]
        public void ParseTypeCastOnNavigationPropertyWithoutDerivedTypeConstraintAnnotationWorks(string propertyName, string key, string typeCast)
        {
            IEdmModel edmModel = GetNavigationPropertyEdmModel("");

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmNavigationProperty property = customer.NavigationProperties().First(c => c.Name == propertyName);
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));

            IEdmType castType = edmModel.FindDeclaredType(typeCast);
            Assert.NotNull(castType);

            int index = key != null ? 4 : 3;
            IEdmType actualType = propertyName == "SubCustomers" && key == null ? collectionCustomerType : customer;
            IEdmType expectedType = propertyName == "SubCustomers" && key == null ?
                new EdmCollectionType(new EdmEntityTypeReference(castType as IEdmEntityType, true)) : castType;

            // ~/Customers(1)/{propertyName}/{typeCast}
            var path = pathParser.ParsePath(new[] { "Customers(1)", propertyName + key, typeCast });
            path[index].ShouldBeTypeSegment(expectedType, actualType);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnNavigationPropertyWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                      "<String>NS.VipCustomer</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetNavigationPropertyEdmModel(annotation, isInLine);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));
            IEdmType collectionVipCustomerType = new EdmCollectionType(new EdmEntityTypeReference(vipCustomer, true));

            int index = 1;
            foreach (string segment in new[] { "DirectReport", "SubCustomers" })
            {
                IEdmType actualType = index == 1 ? customer : collectionCustomerType;
                IEdmType expectedVipCustomerType = index == 1 ? vipCustomer : collectionVipCustomerType;

                // verify the positive type cast on itself
                ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));
                var path = pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.Customer" });
                path[3].ShouldBeTypeSegment(actualType, actualType);

                // verify the positive type cast
                path = pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.VipCustomer" });
                path[3].ShouldBeTypeSegment(expectedVipCustomerType, actualType);

                // verify the negative type cast
                Action parsePath = () => pathParser.ParsePath(new[] { "Customers(1)", segment, "NS.NormalCustomer" });
                parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "navigation property", segment));
                index++;
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnNavigationPropertyWithKeySegmentWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                      "<String>NS.VipCustomer</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetNavigationPropertyEdmModel(annotation, isInLine);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");
            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself
            var path = pathParser.ParsePath(new[] { "Customers(1)", "SubCustomers(1)", "NS.Customer" });
            path[4].ShouldBeTypeSegment(customer, customer);

            // verify the positive type cast
            path = pathParser.ParsePath(new[] { "Customers(1)", "SubCustomers(1)", "NS.VipCustomer" });
            path[4].ShouldBeTypeSegment(vipCustomer, customer);

            // verify the negative type cast
            Action parsePath = () => pathParser.ParsePath(new[] { "Customers(1)", "SubCustomers(1)", "NS.NormalCustomer" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "navigation property", "SubCustomers"));
        }

        private static IEdmModel GetNavigationPropertyEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""DirectReport"" Type=""NS.Customer"" >
           {0}
        </NavigationProperty>
        <NavigationProperty Name=""SubCustomers"" Type=""Collection(NS.Customer)"" >
           {0}
        </NavigationProperty>
      </EntityType>
      <EntityType Name=""VipCustomer"" BaseType=""NS.Customer"" />
      <EntityType Name=""NormalCustomer"" BaseType=""NS.Customer"" />
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
      <Annotations Target=""NS.Customer/DirectReport"">
        {1}
      </Annotations>
      <Annotations Target=""NS.Customer/SubCustomers"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion Navigation Property type cast

        #region Type definition type cast
        [Theory(Skip = "Should pass if fix issue: see: https://github.com/OData/odata.net/issues/1326")]
        [InlineData("Edm.PrimitiveType")] // cast to itself
        [InlineData("Edm.Int32")]
        [InlineData("Edm.Boolean")]
        [InlineData("Edm.Double")]
        public void ParseTypeCastOnTypeDefinitionPropertyWithoutDerivedTypeConstraintAnnotationWorks(string typeCast)
        {
            IEdmModel edmModel = GetTypeDefinitionEdmModel(""); // without Derived type constraint

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmProperty property = customer.DeclaredProperties.First(c => c.Name == "Data");
            Assert.Equal(EdmTypeKind.TypeDefinition, property.Type.TypeKind());

            IEdmType expectType = edmModel.FindType(typeCast);
            Assert.NotNull(expectType);

            // ~/Customers(1)/Data/{typeCast}
            var path = pathParser.ParsePath(new[] { "Customers(1)", "Data", typeCast });
            path[3].ShouldBeTypeSegment(property.Type.Definition, expectType);
        }

        [Theory(Skip = "Should pass if fix issue: see: https://github.com/OData/odata.net/issues/1326")]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseTypeCastOnTypeDefinitionPropertyWithDerivedTypeConstraintAnnotationWorks(bool isInLine)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                    "<Collection>" +
                      "<String>Edm.Int32</String>" +
                      "<String>Edm.Boolean</String>" +
                    "</Collection>" +
                "</Annotation>";

            IEdmModel edmModel = GetTypeDefinitionEdmModel(annotation, isInLine);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmProperty property = customer.DeclaredProperties.First(c => c.Name == "Data");
            Assert.Equal(EdmTypeKind.TypeDefinition, property.Type.TypeKind());

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself
            var path = pathParser.ParsePath(new[] { "Customers(1)", "Data", "Edm.Int32" });
            path[3].ShouldBeTypeSegment(property.Type.Definition, edmModel.FindType("Edm.Int32"));

            // verify the positive type cast
            path = pathParser.ParsePath(new[] { "Customers(1)", "Data", "Edm.Boolean" });
            path[3].ShouldBeTypeSegment(property.Type.Definition, edmModel.FindType("Edm.Boolean"));

            // verify the negative type cast
            Action parsePath = () => pathParser.ParsePath(new[] { "Customers(1)", "Data", "Edm.Double" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("Edm.Double", "type definition", "Data"));
        }

        private static IEdmModel GetTypeDefinitionEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <TypeDefinition Name=""DataType"" UnderlyingType=""Edm.PrimitiveType"" >
         {0}
      </TypeDefinition>
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Data"" Type=""NS.DataType"" />
      </EntityType>
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
      <Annotations Target=""NS.DataType"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion Type definition type cast

        #region Operation type cast
        [Theory]
        [InlineData("Customer")]
        [InlineData("VipCustomer")]
        [InlineData("NormalCustomer")]
        public void ParseTypeCastOnOperationWithoutDerivedTypeConstraintAnnotationWorks(string typeCastName)
        {
            IEdmModel edmModel = GetOperationEdmModel(""); // without derived type constraint

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType castType = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == typeCastName);
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));
            IEdmType collectionExpectedType = new EdmCollectionType(new EdmEntityTypeReference(castType, true));
            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself: ~/Customers/NS.Customer/NS.Image()
            var path = pathParser.ParsePath(new[] { "Customers", "NS." + typeCastName, "NS.Image()" });
            path[1].ShouldBeTypeSegment(collectionExpectedType, collectionCustomerType);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParseBoundOperationWithDerivedTypeConstraintAnnotationWorks(bool inLineAnnotation)
        {
            string annotation =
                @"<Annotation Term=""Org.OData.Validation.V1.DerivedTypeConstraint"" >" +
                     "<Collection>" +
                       "<String>NS.VipCustomer</String>" +
                     "</Collection>" +
                   "</Annotation>";

            IEdmModel edmModel = GetOperationEdmModel(annotation, inLineAnnotation);

            IEdmEntityType customer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "Customer");
            IEdmEntityType vipCustomer = edmModel.SchemaElements.OfType<IEdmEntityType>().First(c => c.Name == "VipCustomer");
            IEdmType collectionCustomerType = new EdmCollectionType(new EdmEntityTypeReference(customer, true));
            IEdmType collectionVipCustomerType = new EdmCollectionType(new EdmEntityTypeReference(vipCustomer, true));

            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(edmModel));

            // verify the positive type cast on itself: ~/Customers/NS.Customer/NS.Image()
            var path = pathParser.ParsePath(new[] { "Customers", "NS.Customer", "NS.Image()" });
            path[1].ShouldBeTypeSegment(collectionCustomerType, collectionCustomerType);

            // verify the positive type cast: ~/Customers(1)/NS.VipCustomer/NS.Image()
            path = pathParser.ParsePath(new[] { "Customers", "NS.VipCustomer", "NS.Image()" });
            path[1].ShouldBeTypeSegment(collectionVipCustomerType, collectionCustomerType);

            // verify the negative type cast: ~/Customers(1)/NS.NormalCustomer/NS.Image()
            Action parsePath = () => pathParser.ParsePath(new[] { "Customers", "NS.NormalCustomer", "NS.Image()" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint("NS.NormalCustomer", "operation", "Image"));
        }

        private static IEdmModel GetOperationEdmModel(string annotation, bool inline = true)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"" />
      <EntityType Name=""VipCustomer"" BaseType=""NS.Customer"" />
      <EntityType Name=""NormalCustomer"" BaseType=""NS.Customer"" />
      <Function Name=""Image"" IsBound=""true"">
        <Parameter Name=""entity"" Type=""Collection(NS.Customer)"" >
          {0}
        </Parameter>
        <ReturnType Type=""Edm.String"" />
      </Function>
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
      <Annotations Target=""NS.Image(Collection(NS.Customer))/entity"">
        {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string inlineAnnotation = inline ? annotation : "";
            string outLineAnnotation = inline ? "" : annotation;
            string modelText = string.Format(template, inlineAnnotation, outLineAnnotation);

            return GetEdmModel(modelText);
        }
        #endregion Operation type cast

        private static IEdmModel GetEdmModel(string csdl)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }
    }
}
