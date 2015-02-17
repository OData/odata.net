//---------------------------------------------------------------------
// <copyright file="TaupoToEdmModelConverterUsingStubTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Values;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Edmlib.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TaupoToEdmModelConverterUsingStubTests
    {
        private ITaupoToEdmModelConverter converter = new TaupoToEdmModelConverterUsingStub();
        private XNamespace annotationNamespace = "bogus";

        [TestMethod]
        public void ConvertComplexType()
        {
            var taupoModel = new EntityModelSchema()
            {
                new ComplexType("NS1", "Complex1")
                {
                    new MemberProperty("p1", EdmDataTypes.Int32),
                    new MemberProperty("p2", EdmDataTypes.Int32.Nullable())
                    {
                        DefaultValue = 100,
                        Annotations = 
                        { 
                            new ConcurrencyTokenAnnotation(),
                            new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo1", "bar1") },
                        },
                    },
                    new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo2", "bar2") },
                },
            };

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(1, result.SchemaElements.Count());
            Assert.AreEqual(1, result.SchemaElements.OfType<IEdmComplexType>().Count());

            IEdmComplexType complex = result.SchemaElements.OfType<IEdmComplexType>().Single();

            Assert.AreEqual("NS1.Complex1", complex.FullName());
            Assert.IsNull(complex.BaseComplexType());
            Assert.AreEqual(2, complex.DeclaredStructuralProperties().Count());
            Assert.AreEqual(2, complex.StructuralProperties().Count());

            IEdmStructuralProperty property = complex.DeclaredStructuralProperties().ElementAt(0);
            IEdmTypeReference propertyType = property.Type;

            Assert.AreEqual("p1", property.Name);
            Assert.AreEqual("Edm.Int32", propertyType.FullName());
            Assert.AreEqual(EdmTypeKind.Primitive, propertyType.TypeKind());
            Assert.IsFalse(propertyType.IsNullable);

            Assert.AreEqual(complex, property.DeclaringType);
            Assert.AreEqual(EdmConcurrencyMode.None, property.ConcurrencyMode);
            Assert.IsNull(property.DefaultValueString);

            Assert.AreEqual(0, result.DirectValueAnnotations(property).Count());

            property = complex.DeclaredStructuralProperties().ElementAt(1);
            propertyType = property.Type;

            Assert.AreEqual("p2", property.Name);
            Assert.AreEqual("Edm.Int32", propertyType.FullName());
            Assert.AreEqual(EdmTypeKind.Primitive, propertyType.TypeKind());
            Assert.IsTrue(propertyType.IsNullable);

            Assert.AreEqual(complex, property.DeclaringType);
            Assert.AreEqual(EdmConcurrencyMode.Fixed, property.ConcurrencyMode);
            Assert.AreEqual("100", property.DefaultValueString);

            Assert.AreEqual(1, result.DirectValueAnnotations(complex).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(complex).First().NamespaceUri);
            Assert.AreEqual("foo2", result.DirectValueAnnotations(complex).First().Name);
            Assert.AreEqual("bar2", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(complex).First()).Value as IEdmStringValue).Value);

            Assert.AreEqual(1, result.DirectValueAnnotations(property).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(property).First().NamespaceUri);
            Assert.AreEqual("foo1", result.DirectValueAnnotations(property).First().Name);
            Assert.AreEqual("bar1", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(property).First()).Value as IEdmStringValue).Value);
        }

        [TestMethod]
        public void ConvertComplexType_Nested()
        {
            var taupoModel = new EntityModelSchema()
            {
                new ComplexType("NS1", "Complex1")
                {
                    new MemberProperty("p1", DataTypes.ComplexType.WithName("NS2", "NestedComplex")),
                },
                new ComplexType("NS2", "NestedComplex")
                {
                    new MemberProperty("p1", EdmDataTypes.Int16),
                },
            }
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(2, result.SchemaElements.Count());
            Assert.AreEqual(2, result.SchemaElements.OfType<IEdmComplexType>().Count());

            IEdmComplexType complex = result.SchemaElements.OfType<IEdmComplexType>().ElementAt(0);

            Assert.AreEqual("NS1.Complex1", complex.FullName());
            Assert.AreEqual(1, complex.DeclaredStructuralProperties().Count());
            Assert.AreEqual("p1", complex.DeclaredStructuralProperties().First().Name);

            IEdmTypeReference propertyType = complex.DeclaredStructuralProperties().First().Type;
            Assert.AreEqual("NS2.NestedComplex", propertyType.FullName());
            Assert.AreEqual(EdmTypeKind.Complex, propertyType.TypeKind());

            complex = result.SchemaElements.OfType<IEdmComplexType>().ElementAt(1);

            Assert.AreEqual("NS2.NestedComplex", complex.FullName());
            Assert.AreEqual(1, complex.DeclaredStructuralProperties().Count());
            Assert.AreEqual("p1", complex.DeclaredStructuralProperties().First().Name);
        }

        [TestMethod]
        public void ConvertComplexType_Inheritance()
        {
            var taupoModel = new EntityModelSchema()
            {
                new ComplexType("NS1", "BaseComplex")
                {
                    new MemberProperty("p1", EdmDataTypes.Int16),
                },
                new ComplexType("NS2", "DerivedComplex")
                {
                    BaseType = "BaseComplex",
                    Properties = { new MemberProperty("p2", EdmDataTypes.Int16) },
                },
            }
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(2, result.SchemaElements.Count());
            Assert.AreEqual(2, result.SchemaElements.OfType<IEdmComplexType>().Count());

            IEdmComplexType baseComplex = result.SchemaElements.OfType<IEdmComplexType>().ElementAt(0);

            Assert.AreEqual("NS1.BaseComplex", baseComplex.FullName());
            Assert.AreEqual(1, baseComplex.DeclaredStructuralProperties().Count());
            Assert.AreEqual("p1", baseComplex.DeclaredStructuralProperties().First().Name);

            IEdmComplexType derivedComplex = result.SchemaElements.OfType<IEdmComplexType>().ElementAt(1);

            Assert.AreEqual("NS2.DerivedComplex", derivedComplex.FullName());
            Assert.AreEqual(1, derivedComplex.DeclaredStructuralProperties().Count());
            Assert.AreEqual("p2", derivedComplex.DeclaredStructuralProperties().First().Name);

            Assert.AreEqual(2, derivedComplex.StructuralProperties().Count());
            Assert.AreEqual(2, derivedComplex.Properties().Count());
            Assert.AreEqual(baseComplex, derivedComplex.BaseComplexType());
        }

        [TestMethod]
        public void ConvertEntityType()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("NS1", "Entity1")
                {
                    new MemberProperty("p1", EdmDataTypes.Int16) 
                    {                     
                        new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo1", "bar1") },
                    },
                    new MemberProperty("p2", EdmDataTypes.Int64) { IsPrimaryKey = true },
                    new MemberProperty("p3", EdmDataTypes.Boolean) { IsPrimaryKey = true },
                    new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo2", "bar2") },
                },
            };

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(1, result.SchemaElements.Count());
            Assert.AreEqual(1, result.SchemaElements.OfType<IEdmEntityType>().Count());

            IEdmEntityType entity = result.SchemaElements.OfType<IEdmEntityType>().First();

            Assert.AreEqual("NS1.Entity1", entity.FullName());
            Assert.AreEqual(3, entity.DeclaredStructuralProperties().Count());
            Assert.AreEqual("p1", entity.DeclaredStructuralProperties().ElementAt(0).Name);
            Assert.AreEqual("p2", entity.DeclaredStructuralProperties().ElementAt(1).Name);
            Assert.AreEqual("p3", entity.DeclaredStructuralProperties().ElementAt(2).Name);

            Assert.AreEqual(2, entity.DeclaredKey.Count());
            Assert.AreEqual("p2", entity.DeclaredKey.ElementAt(0).Name);
            Assert.AreEqual("p3", entity.DeclaredKey.ElementAt(1).Name);

            Assert.AreEqual(1, result.DirectValueAnnotations(entity).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(entity).First().NamespaceUri);
            Assert.AreEqual("foo2", result.DirectValueAnnotations(entity).First().Name);
            Assert.AreEqual("bar2", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(entity).First()).Value as IEdmStringValue).Value);

            IEdmStructuralProperty p1 = entity.DeclaredStructuralProperties().ElementAt(0);
            Assert.AreEqual(1, result.DirectValueAnnotations(p1).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(p1).First().NamespaceUri);
            Assert.AreEqual("foo1", result.DirectValueAnnotations(p1).First().Name);
            Assert.AreEqual("bar1", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(p1).First()).Value as IEdmStringValue).Value);

            IEdmStructuralProperty p2 = entity.DeclaredStructuralProperties().ElementAt(1);
            Assert.AreEqual(0, result.DirectValueAnnotations(p2).Count());
        }

        [TestMethod]
        public void ConvertEntityType_Inheritance()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("Derived")
                {
                    BaseType = "Base",
                },
                new EntityType("Base")
                {
                    new MemberProperty("p1", EdmDataTypes.Int16) { IsPrimaryKey = true },
                    new MemberProperty("p2", EdmDataTypes.Guid), 
                    new MemberProperty("p3", EdmDataTypes.Double),
                },
            }
            .ApplyDefaultNamespace("NS1")
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(2, result.SchemaElements.Count());
            Assert.AreEqual(2, result.SchemaElements.OfType<IEdmEntityType>().Count());

            IEdmEntityType convertedDerived = result.SchemaElements.OfType<IEdmEntityType>().ElementAt(0);
            IEdmEntityType convertedBase = result.SchemaElements.OfType<IEdmEntityType>().ElementAt(1);

            Assert.AreEqual("NS1.Derived", convertedDerived.FullName());
            Assert.AreEqual(0, convertedDerived.DeclaredStructuralProperties().Count());
            Assert.AreEqual(convertedBase, convertedDerived.BaseEntityType());

            Assert.AreEqual("NS1.Base", convertedBase.FullName());
            Assert.AreEqual(3, convertedBase.DeclaredStructuralProperties().Count());

            Assert.AreEqual(1, convertedDerived.Key().Count());
            Assert.AreEqual(1, convertedBase.DeclaredKey.Count());
            Assert.AreEqual(convertedBase.DeclaredKey.First(), convertedDerived.Key().First());
        }

        [TestMethod]
        public void ConvertAssociationType()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("Person")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                },
                new AssociationType("PersonFriends")
                {
                    new AssociationEnd("Friend1", "Person", EndMultiplicity.Many),
                    new AssociationEnd("Friend2", "Person", EndMultiplicity.Many)
                },
            }
            .ApplyDefaultNamespace("NS1")
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(1, result.SchemaElements.Count());
            Assert.AreEqual(1, result.SchemaElements.OfType<IEdmEntityType>().Count());

            IEdmEntityType convertedPerson = result.SchemaElements.OfType<IEdmEntityType>().First();

            Assert.AreEqual(2, convertedPerson.NavigationProperties().Count());

            IEdmNavigationProperty toFriend2 = convertedPerson.NavigationProperties().ElementAt(0);
            IEdmNavigationProperty toFriend1 = convertedPerson.NavigationProperties().ElementAt(1);

            Assert.AreEqual("PersonFriends_ToFriend2", toFriend2.Name);

            Assert.IsInstanceOfType(toFriend2.Type, typeof(IEdmCollectionTypeReference));
            var elementTypeRef = ((IEdmCollectionTypeReference)toFriend2.Type).CollectionDefinition().ElementType;
            Assert.IsInstanceOfType(elementTypeRef, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(convertedPerson, ((IEdmEntityTypeReference)elementTypeRef).EntityDefinition());

            Assert.AreSame(convertedPerson, toFriend2.DeclaringType);
            Assert.AreSame(toFriend1, toFriend2.Partner);
            Assert.AreEqual(EdmOnDeleteAction.None, toFriend2.OnDelete);
            Assert.AreEqual(false, toFriend2.IsPrincipal);
            Assert.IsNull(toFriend2.DependentProperties);

            Assert.AreEqual("PersonFriends_ToFriend1", toFriend1.Name);

            Assert.IsInstanceOfType(toFriend1.Type, typeof(IEdmCollectionTypeReference));
            elementTypeRef = ((IEdmCollectionTypeReference)toFriend1.Type).CollectionDefinition().ElementType;
            Assert.IsInstanceOfType(elementTypeRef, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(convertedPerson, ((IEdmEntityTypeReference)elementTypeRef).EntityDefinition());

            Assert.AreSame(convertedPerson, toFriend1.DeclaringType);
            Assert.AreSame(toFriend2, toFriend1.Partner);
            Assert.AreEqual(EdmOnDeleteAction.None, toFriend1.OnDelete);
            Assert.AreEqual(false, toFriend1.IsPrincipal);
            Assert.IsNull(toFriend1.DependentProperties);

            // check custom naming information
            Assert.AreEqual("NS1.PersonFriends", result.GetAssociationFullName(toFriend2));
            Assert.AreEqual("Friend1", result.GetAssociationEndName(toFriend2));
            Assert.AreEqual("NS1.PersonFriends", result.GetAssociationFullName(toFriend1));
            Assert.AreEqual("Friend2", result.GetAssociationEndName(toFriend1));
        }

        [TestMethod]
        public void ConvertAssociationType_WithRefrentialConstraint_NoNavigation()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("Customer")
                {
                    new MemberProperty("Id1", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("Id2", EdmDataTypes.Int32) { IsPrimaryKey = true },
                },
                new EntityType("Order")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("cId1", EdmDataTypes.Int32),
                    new MemberProperty("cId2", EdmDataTypes.Int32),
                },
                new AssociationType("CustomerOrder")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One, OperationAction.Cascade),
                    new AssociationEnd("Order", "Order", EndMultiplicity.ZeroOne),
                    new ReferentialConstraint()
                            .WithDependentProperties("Order", "cId2", "cId1")
                            .ReferencesPrincipalProperties("Customer", "Id2", "Id1"),
                },
            }
            .ApplyDefaultNamespace("NS1")
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.IsNull(result.EntityContainer);
            Assert.AreEqual(2, result.SchemaElements.Count());
            Assert.AreEqual(2, result.SchemaElements.OfType<IEdmEntityType>().Count());

            IEdmEntityType customer = (IEdmEntityType)result.FindType("NS1.Customer");
            IEdmEntityType order = (IEdmEntityType)result.FindType("NS1.Order");

            Assert.AreEqual(1, customer.NavigationProperties().Count());
            Assert.AreEqual(1, order.NavigationProperties().Count());

            IEdmNavigationProperty toOrder = customer.NavigationProperties().First();
            IEdmNavigationProperty toCustomer = order.NavigationProperties().First();

            Assert.AreEqual("CustomerOrder_ToOrder", toOrder.Name);

            Assert.IsInstanceOfType(toOrder.Type, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(order, ((IEdmEntityTypeReference)toOrder.Type).EntityDefinition());
            Assert.IsTrue(((IEdmEntityTypeReference)toOrder.Type).IsNullable);

            Assert.AreSame(customer, toOrder.DeclaringType);
            Assert.AreSame(toCustomer, toOrder.Partner);
            Assert.AreEqual(EdmOnDeleteAction.Cascade, toOrder.OnDelete);
            Assert.AreEqual(true, toOrder.IsPrincipal);
            Assert.IsNull(toOrder.DependentProperties);

            Assert.AreEqual("CustomerOrder_ToCustomer", toCustomer.Name);

            Assert.IsInstanceOfType(toCustomer.Type, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(customer, ((IEdmEntityTypeReference)toCustomer.Type).EntityDefinition());
            Assert.IsFalse(((IEdmEntityTypeReference)toCustomer.Type).IsNullable);

            Assert.AreSame(order, toCustomer.DeclaringType);
            Assert.AreSame(toOrder, toCustomer.Partner);
            Assert.AreEqual(EdmOnDeleteAction.None, toCustomer.OnDelete);
            Assert.AreEqual(false, toCustomer.IsPrincipal);
            Assert.AreEqual(2, toCustomer.DependentProperties.Count());
            Assert.AreSame(order.FindProperty("cId1"), toCustomer.DependentProperties.ElementAt(0));
            Assert.AreSame(order.FindProperty("cId2"), toCustomer.DependentProperties.ElementAt(1));

            // check custom naming information
            Assert.AreEqual("NS1.CustomerOrder", result.GetAssociationFullName(toOrder));
            Assert.AreEqual("Customer", result.GetAssociationEndName(toOrder));
            Assert.AreEqual("NS1.CustomerOrder", result.GetAssociationFullName(toCustomer));
            Assert.AreEqual("Order", result.GetAssociationEndName(toCustomer));
        }

        [TestMethod]
        public void ConvertAssociationType_WithRefrentialConstraint_WithNavigation()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("Customer")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new NavigationProperty("ToOrders", "CustomerOrder", "Customer", "Order")
                    {
                        new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo2", "bar2") },
                    },
                },
                new EntityType("Order")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                    new MemberProperty("cId", EdmDataTypes.Int32),
                    new NavigationProperty("ToCustomer", "CustomerOrder", "Order", "Customer")
                },
                new AssociationType("CustomerOrder")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One, OperationAction.Cascade),
                    new AssociationEnd("Order", "Order", EndMultiplicity.Many),
                    new ReferentialConstraint()
                            .WithDependentProperties("Order", "cId")
                            .ReferencesPrincipalProperties("Customer", "Id"),
                },
            }
            .ApplyDefaultNamespace("NS1")
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            IEdmEntityType customer = (IEdmEntityType)result.FindType("NS1.Customer");
            IEdmEntityType order = (IEdmEntityType)result.FindType("NS1.Order");

            Assert.AreEqual(1, customer.NavigationProperties().Count());
            Assert.AreEqual(1, order.NavigationProperties().Count());
            
            IEdmNavigationProperty toOrders = customer.NavigationProperties().First();
            IEdmNavigationProperty toCustomer = order.NavigationProperties().First();

            Assert.AreEqual("ToOrders", toOrders.Name);
            Assert.IsFalse(toOrders.ContainsTarget);

            Assert.IsInstanceOfType(toOrders.Type, typeof(IEdmCollectionTypeReference));
            var elementTypeRef = ((IEdmCollectionTypeReference)toOrders.Type).CollectionDefinition().ElementType;
            Assert.IsInstanceOfType(elementTypeRef, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(order, ((IEdmEntityTypeReference)elementTypeRef).EntityDefinition());

            Assert.AreSame(customer, toOrders.DeclaringType);
            Assert.AreSame(toCustomer, toOrders.Partner);
            Assert.AreEqual(EdmOnDeleteAction.Cascade, toOrders.OnDelete);
            Assert.AreEqual(true, toOrders.IsPrincipal);
            Assert.IsNull(toOrders.DependentProperties);

            Assert.AreEqual("ToCustomer", toCustomer.Name);
            Assert.IsFalse(toCustomer.ContainsTarget);

            Assert.IsInstanceOfType(toCustomer.Type, typeof(IEdmEntityTypeReference));
            Assert.AreEqual(customer, ((IEdmEntityTypeReference)toCustomer.Type).EntityDefinition());
            Assert.IsFalse(((IEdmEntityTypeReference)toCustomer.Type).IsNullable);

            Assert.AreSame(order, toCustomer.DeclaringType);
            Assert.AreSame(toOrders, toCustomer.Partner);
            Assert.AreEqual(EdmOnDeleteAction.None, toCustomer.OnDelete);
            Assert.AreEqual(false, toCustomer.IsPrincipal);
            Assert.AreEqual(1, toCustomer.DependentProperties.Count());
            Assert.AreSame(order.FindProperty("cId"), toCustomer.DependentProperties.ElementAt(0));

            // check immediate annotation
            var annotation = result.DirectValueAnnotations(toOrders).Single(a => a.NamespaceUri == "bogus");
            Assert.AreEqual("foo2", annotation.Name);
            Assert.AreEqual("bar2", (((IEdmDirectValueAnnotation)annotation).Value as IEdmStringValue).Value);

            // check custom naming information
            Assert.AreEqual("NS1.CustomerOrder", result.GetAssociationFullName(toOrders));
            Assert.AreEqual("Customer", result.GetAssociationEndName(toOrders));
            Assert.AreEqual("NS1.CustomerOrder", result.GetAssociationFullName(toCustomer));
            Assert.AreEqual("Order", result.GetAssociationEndName(toCustomer));
        }

        [TestMethod]
        public void ConvertEntityContainer()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityContainer("MyContainer")
                {
                    new EntitySet("Customers", "Customer"),
                    new EntitySet("Orders", "Order")
                    {
                        new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo1", "bar1") },
                    },
                    new AssociationSet("CustomerOrders", "CustomerOrder")
                    {
                        new AssociationSetEnd("Order", "Orders"),
                        new AssociationSetEnd("Customer", "Customers")
                    },
                    new FunctionImport("FunctionImport1")
                    {
                        ReturnTypes = {new FunctionImportReturnType( DataTypes.CollectionOfEntities("Customer"),"Customers")},
                        Parameters = { new FunctionParameter("ExcludingId", EdmDataTypes.Int32) },
                        Annotations = { new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo5", "bar5") } },
                    },
                    new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo4", "bar4") },
                },
                new EntityType("Customer")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                },
                new EntityType("Order")
                {
                    new MemberProperty("Id", EdmDataTypes.Int32) { IsPrimaryKey = true },
                },
                new AssociationType("CustomerOrder")
                {
                    new AssociationEnd("Customer", "Customer", EndMultiplicity.One, OperationAction.Cascade),
                    new AssociationEnd("Order", "Order", EndMultiplicity.Many),
                },
            }
            .ApplyDefaultNamespace("NS1")
            .Resolve();

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            IEdmEntityType customer = (IEdmEntityType)result.FindType("NS1.Customer");
            IEdmEntityType order = (IEdmEntityType)result.FindType("NS1.Order");

            IEdmEntityContainer convertedContainer = result.EntityContainer;
            Assert.AreEqual("MyContainer", convertedContainer.Name);
            Assert.AreEqual(3, convertedContainer.Elements.Count());
            Assert.AreEqual(2, convertedContainer.Elements.OfType<IEdmEntitySet>().Count());
            Assert.AreEqual(1, convertedContainer.Elements.OfType<IEdmOperationImport>().Count());

            Assert.AreEqual(1, result.DirectValueAnnotations(convertedContainer).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(convertedContainer).First().NamespaceUri);
            Assert.AreEqual("foo4", result.DirectValueAnnotations(convertedContainer).First().Name);
            Assert.AreEqual("bar4", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(convertedContainer).First()).Value as IEdmStringValue).Value);

            IEdmEntitySet convertedCustomerSet = convertedContainer.Elements.OfType<IEdmEntitySet>().ElementAt(0);
            Assert.AreEqual("Customers", convertedCustomerSet.Name);
            Assert.AreEqual(result.FindType("NS1.Customer"), convertedCustomerSet.ElementType);

            Assert.AreEqual(0, result.DirectValueAnnotations(convertedCustomerSet).Count(a => a.NamespaceUri == "bogus"));

            IEdmEntitySet convertedOrderSet = convertedContainer.Elements.OfType<IEdmEntitySet>().ElementAt(1);
            Assert.AreEqual("Orders", convertedOrderSet.Name);
            Assert.AreEqual(result.FindType("NS1.Order"), convertedOrderSet.ElementType);

            var annotations = result.DirectValueAnnotations(convertedOrderSet).Where(a => a.NamespaceUri == "bogus");
            Assert.AreEqual(1, annotations.Count());
            Assert.AreEqual("foo1", annotations.First().Name);
            Assert.AreEqual("bar1", (((IEdmDirectValueAnnotation)annotations.First()).Value as IEdmStringValue).Value);

            var toOrder = customer.NavigationProperties().First();
            Assert.AreSame(convertedOrderSet, convertedCustomerSet.FindNavigationTarget(toOrder));
            Assert.AreEqual("CustomerOrders", result.GetAssociationSetName(convertedCustomerSet, toOrder));

            var toCustomer = order.NavigationProperties().First();
            Assert.AreSame(convertedCustomerSet, convertedOrderSet.FindNavigationTarget(toCustomer));
            Assert.AreEqual("CustomerOrders", result.GetAssociationSetName(convertedOrderSet, toCustomer));

            IEdmOperationImport convertedFunctionImport = convertedContainer.Elements.OfType<IEdmOperationImport>().First();
            Assert.AreEqual("FunctionImport1", convertedFunctionImport.Name);
            IEdmEntitySet eset;
            Assert.IsTrue(convertedFunctionImport.TryGetStaticEntitySet(out eset));
            Assert.AreEqual(convertedCustomerSet, eset);

            Assert.AreEqual(EdmTypeKind.Collection, convertedFunctionImport.ReturnType.TypeKind());
            Assert.AreEqual(null, convertedFunctionImport.ReturnType.FullName());

            Assert.AreEqual(1, convertedFunctionImport.Parameters.Count());
            Assert.AreEqual("ExcludingId", convertedFunctionImport.Parameters.First().Name);
            Assert.AreEqual(EdmTypeKind.Primitive, convertedFunctionImport.Parameters.First().Type.TypeKind());

            Assert.AreEqual(1, result.DirectValueAnnotations(convertedFunctionImport).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(convertedFunctionImport).First().NamespaceUri);
            Assert.AreEqual("foo5", result.DirectValueAnnotations(convertedFunctionImport).First().Name);
            Assert.AreEqual("bar5", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(convertedFunctionImport).First()).Value as IEdmStringValue).Value);
        }

        [TestMethod]
        public void ConvertFunction()
        {
            var taupoModel = new EntityModelSchema()
            {
                new Function("NS1", "Function1")
                {
                    ReturnType = EdmDataTypes.Int32,
                    Parameters = 
                    {
                        new FunctionParameter("Param1", EdmDataTypes.Int32, FunctionParameterMode.InOut),
                        new FunctionParameter("Param2", EdmDataTypes.Int32, FunctionParameterMode.Out)
                        {
                            new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo1", "bar1") },
                        },
                    },
                    Annotations = 
                    {
                        new AttributeAnnotation() { Content = new XAttribute(this.annotationNamespace + "foo2", "bar2") },
                    },
                },
            };

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.AreEqual(1, result.SchemaElements.Count());
            Assert.AreEqual(1, result.SchemaElements.OfType<IEdmOperation>().Count());

            IEdmOperation operation = result.SchemaElements.OfType<IEdmOperation>().First();
            Assert.AreEqual("NS1.Function1", operation.FullName());

            Assert.AreEqual("Edm.Int32", operation.ReturnType.FullName());

            Assert.AreEqual(2, operation.Parameters.Count());
            IEdmOperationParameter p1 = operation.Parameters.ElementAt(0);
            Assert.AreEqual("Param1", p1.Name);

            IEdmOperationParameter p2 = operation.Parameters.ElementAt(1);
            Assert.AreEqual("Param2", p2.Name);

            Assert.AreEqual(1, result.DirectValueAnnotations(p2).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(p2).First().NamespaceUri);
            Assert.AreEqual("foo1", result.DirectValueAnnotations(p2).First().Name);
            Assert.AreEqual("bar1", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(p2).First()).Value as IEdmStringValue).Value);

            Assert.AreEqual(1, result.DirectValueAnnotations(operation).Count());
            Assert.AreEqual("bogus", result.DirectValueAnnotations(operation).First().NamespaceUri);
            Assert.AreEqual("foo2", result.DirectValueAnnotations(operation).First().Name);
            Assert.AreEqual("bar2", (((IEdmDirectValueAnnotation)result.DirectValueAnnotations(operation).First()).Value as IEdmStringValue).Value);
        }

        [TestMethod]
        public void ConvertFunction_with_overloads()
        {
            var taupoModel = new EntityModelSchema()
            {
                new Function("NS1", "MyFunction")
                {
                    new FunctionParameter("Param1", EdmDataTypes.Int32, FunctionParameterMode.InOut),
                    new FunctionParameter("Param2", EdmDataTypes.Int32, FunctionParameterMode.Out)
                },
                new Function("NS1", "MyFunction")
                {
                    new FunctionParameter("Param", EdmDataTypes.Int16),
                },
            };

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            Assert.AreEqual(2, result.SchemaElements.Count());
            Assert.AreEqual(2, result.SchemaElements.OfType<IEdmOperation>().Count());

            IEdmOperation operation = result.SchemaElements.OfType<IEdmOperation>().ElementAt(0);
            Assert.AreEqual("NS1.MyFunction", operation.FullName());
            Assert.AreEqual(2, operation.Parameters.Count());
            Assert.AreEqual("Param1", operation.Parameters.ElementAt(0).Name);
            Assert.AreEqual("Param2", operation.Parameters.ElementAt(1).Name);

            operation = result.SchemaElements.OfType<IEdmOperation>().ElementAt(1);
            Assert.AreEqual("NS1.MyFunction", operation.FullName());
            Assert.AreEqual(1, operation.Parameters.Count());
            Assert.AreEqual("Param", operation.Parameters.ElementAt(0).Name);
        }

        [TestMethod]
        public void ConvertPrimitiveTypeProperties()
        {
            var taupoModel = new EntityModelSchema()
            {
                new EntityType("NS1", "Entity1")
                {
                    new MemberProperty("p0", EdmDataTypes.Binary(100, true)),
                    new MemberProperty("p1", EdmDataTypes.Boolean),
                    new MemberProperty("p2", EdmDataTypes.Byte),
                    new MemberProperty("p4", EdmDataTypes.DateTimeOffset(7)),
                    new MemberProperty("p5", EdmDataTypes.Decimal(10, 2)),
                    new MemberProperty("p6", EdmDataTypes.Double),
                    new MemberProperty("p7", EdmDataTypes.Guid),
                    new MemberProperty("p8", EdmDataTypes.Int16),
                    new MemberProperty("p9", EdmDataTypes.Int32),
                    new MemberProperty("p10", EdmDataTypes.Int64),
                    new MemberProperty("p11", EdmDataTypes.SByte),
                    new MemberProperty("p12", EdmDataTypes.Single),
                    new MemberProperty("p13", EdmDataTypes.String(50, true, false)),
                    new MemberProperty("p14", EdmDataTypes.Time(11)),
                },
            };

            IEdmModel result = this.converter.ConvertToEdmModel(taupoModel);

            IEdmEntityType entity = result.SchemaElements.OfType<IEdmEntityType>().First();

            Assert.AreEqual(14, entity.DeclaredStructuralProperties().Count());
            Assert.AreEqual("Edm.Binary", entity.DeclaredStructuralProperties().ElementAt(0).Type.FullName());
            Assert.AreEqual("Edm.Boolean", entity.DeclaredStructuralProperties().ElementAt(1).Type.FullName());
            Assert.AreEqual("Edm.Byte", entity.DeclaredStructuralProperties().ElementAt(2).Type.FullName());
            Assert.AreEqual("Edm.DateTimeOffset", entity.DeclaredStructuralProperties().ElementAt(3).Type.FullName());
            Assert.AreEqual("Edm.Decimal", entity.DeclaredStructuralProperties().ElementAt(4).Type.FullName());
            Assert.AreEqual("Edm.Double", entity.DeclaredStructuralProperties().ElementAt(5).Type.FullName());
            Assert.AreEqual("Edm.Guid", entity.DeclaredStructuralProperties().ElementAt(6).Type.FullName());
            Assert.AreEqual("Edm.Int16", entity.DeclaredStructuralProperties().ElementAt(7).Type.FullName());
            Assert.AreEqual("Edm.Int32", entity.DeclaredStructuralProperties().ElementAt(8).Type.FullName());
            Assert.AreEqual("Edm.Int64", entity.DeclaredStructuralProperties().ElementAt(9).Type.FullName());
            Assert.AreEqual("Edm.SByte", entity.DeclaredStructuralProperties().ElementAt(10).Type.FullName());
            Assert.AreEqual("Edm.Single", entity.DeclaredStructuralProperties().ElementAt(11).Type.FullName());
            Assert.AreEqual("Edm.String", entity.DeclaredStructuralProperties().ElementAt(12).Type.FullName());
            Assert.AreEqual("Edm.Duration", entity.DeclaredStructuralProperties().ElementAt(13).Type.FullName());

            foreach (var p in entity.DeclaredStructuralProperties())
            {
                Assert.AreEqual(EdmTypeKind.Primitive, p.Type.TypeKind());
                Assert.IsFalse(p.Type.IsNullable);
            }
        }
    }
}