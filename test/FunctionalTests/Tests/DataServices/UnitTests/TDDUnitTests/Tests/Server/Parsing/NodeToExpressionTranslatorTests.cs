//---------------------------------------------------------------------
// <copyright file="NodeToExpressionTranslatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Parsing;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DataServiceProviderMethods = Microsoft.OData.Service.Providers.DataServiceProviderMethods;
    using ErrorStrings = Microsoft.OData.Service.Strings;
    using OpenTypeMethods = Microsoft.OData.Service.Providers.OpenTypeMethods;

    [TestClass]
    public class NodeToExpressionTranslatorTests
    {
        private readonly FunctionExpressionBinder functionExpressionBinder;
        private readonly EdmEntityType customerEdmType;
        private readonly EdmEntityType derivedCustomerEdmType;
        private readonly EdmEntityType weaklyBackedCustomerEdmType;
        private readonly IEdmStructuralProperty nameProperty;
        private readonly IEdmStructuralProperty addressProperty;
        private readonly IEdmStructuralProperty namesProperty;
        private readonly IEdmStructuralProperty addressesProperty;
        private readonly IEdmStructuralProperty weaklyBackedProperty;
        private readonly IEdmNavigationProperty bestFriendNavigation;
        private readonly IEdmNavigationProperty otherFriendsNavigation;
        private readonly IEdmEntitySet entitySet;
        private readonly ResourceProperty weaklyBackedResourceProperty;
        private readonly ResourceType weaklyBackedDerivedType;
        private readonly EdmModel model;
        private readonly ParameterExpression implicitParameterExpression = Expression.Parameter(typeof(object), "it");
        private readonly ResourceType customerResourceType;
        private NodeToExpressionTranslator testSubject;

        public NodeToExpressionTranslatorTests()
        {
            this.functionExpressionBinder = new FunctionExpressionBinder(t => { throw new Exception(); });

            this.customerResourceType = new ResourceType(typeof(Customer), ResourceTypeKind.EntityType, null, "Fake", "Customer", false) { IsOpenType = true };
            var derivedCustomerResourceType = new ResourceType(typeof(DerivedCustomer), ResourceTypeKind.EntityType, this.customerResourceType, "Fake", "DerivedCustomer", false);
            this.weaklyBackedDerivedType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, derivedCustomerResourceType, "Fake", "WeaklyBackedCustomer", false) { CanReflectOnInstanceType = false };
            var nameResourceProperty = new ResourceProperty("Name", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            this.customerResourceType.AddProperty(nameResourceProperty);
            var addressResourceType = new ResourceType(typeof(Address), ResourceTypeKind.ComplexType, null, "Fake", "Address", false);
            var addressResourceProperty = new ResourceProperty("Address", ResourcePropertyKind.ComplexType, addressResourceType);
            this.customerResourceType.AddProperty(addressResourceProperty);

            var namesResourceProperty = new ResourceProperty("Names", ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(string))));
            this.customerResourceType.AddProperty(namesResourceProperty);
            var addressesResourceProperty = new ResourceProperty("Addresses", ResourcePropertyKind.Collection, ResourceType.GetCollectionResourceType(addressResourceType));
            this.customerResourceType.AddProperty(addressesResourceProperty);

            var bestFriendResourceProperty = new ResourceProperty("BestFriend", ResourcePropertyKind.ResourceReference, this.customerResourceType);
            this.customerResourceType.AddProperty(bestFriendResourceProperty);
            var otherFriendsResourceProperty = new ResourceProperty("OtherFriends", ResourcePropertyKind.ResourceSetReference, this.customerResourceType);
            this.customerResourceType.AddProperty(otherFriendsResourceProperty);

            this.weaklyBackedResourceProperty = new ResourceProperty("WeaklyBacked", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) { CanReflectOnInstanceTypeProperty = false };

            var guid1ResourceProperty = new ResourceProperty("Guid1", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Guid)));
            var guid2ResourceProperty = new ResourceProperty("Guid2", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Guid)));
            var nullableGuid1ResourceProperty = new ResourceProperty("NullableGuid1", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Guid?)));
            var nullableGuid2ResourceProperty = new ResourceProperty("NullableGuid2", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Guid?)));

            this.customerResourceType.AddProperty(guid1ResourceProperty);
            this.customerResourceType.AddProperty(guid2ResourceProperty);
            this.customerResourceType.AddProperty(nullableGuid1ResourceProperty);
            this.customerResourceType.AddProperty(nullableGuid2ResourceProperty);

            var resourceSet = new ResourceSet("Customers", this.customerResourceType);
            resourceSet.SetReadOnly();
            var resourceSetWrapper = ResourceSetWrapper.CreateForTests(resourceSet, EntitySetRights.All);
            
            this.model = new EdmModel();

            this.customerEdmType = new MetadataProviderEdmEntityType("Fake", this.customerResourceType, null, false, true, false, t => {});
            this.model.AddElement(this.customerEdmType);

            this.derivedCustomerEdmType = new MetadataProviderEdmEntityType("Fake", derivedCustomerResourceType, this.customerEdmType, false, false, false, t => { });
            this.model.AddElement(this.derivedCustomerEdmType);

            this.weaklyBackedCustomerEdmType = new MetadataProviderEdmEntityType("Fake", weaklyBackedDerivedType, this.derivedCustomerEdmType, false, false, false, t => { });
            this.model.AddElement(this.weaklyBackedCustomerEdmType);

            this.nameProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, nameResourceProperty, EdmCoreModel.Instance.GetString(true), null);
            this.customerEdmType.AddProperty(this.nameProperty);

            var addressEdmType = new MetadataProviderEdmComplexType("Fake", addressResourceType, null, false, false, t => {});
            this.model.AddElement(addressEdmType);

            this.addressProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, addressResourceProperty, new EdmComplexTypeReference(addressEdmType, true), null);
            this.customerEdmType.AddProperty(this.addressProperty);

            this.namesProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, namesResourceProperty, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))), null);
            this.customerEdmType.AddProperty(this.namesProperty);

            this.addressesProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, addressesResourceProperty, new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressEdmType, false))), null);
            this.customerEdmType.AddProperty(this.addressesProperty);

            this.bestFriendNavigation = new MetadataProviderEdmNavigationProperty(this.customerEdmType, bestFriendResourceProperty, new EdmEntityTypeReference(this.customerEdmType, true));
            this.customerEdmType.AddProperty(this.bestFriendNavigation);

            this.otherFriendsNavigation = new MetadataProviderEdmNavigationProperty(this.customerEdmType, otherFriendsResourceProperty, new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.customerEdmType, true))));
            this.customerEdmType.AddProperty(this.otherFriendsNavigation);

            this.weaklyBackedProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, this.weaklyBackedResourceProperty, EdmCoreModel.Instance.GetString(true), null);
            this.customerEdmType.AddProperty(this.weaklyBackedProperty);

            var guid1EdmProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, guid1ResourceProperty, EdmCoreModel.Instance.GetGuid(false), null);
            this.customerEdmType.AddProperty(guid1EdmProperty);
            var guid2EdmProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, guid2ResourceProperty, EdmCoreModel.Instance.GetGuid(false), null);
            this.customerEdmType.AddProperty(guid2EdmProperty);
            var nullableGuid1EdmProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, nullableGuid1ResourceProperty, EdmCoreModel.Instance.GetGuid(true), null);
            this.customerEdmType.AddProperty(nullableGuid1EdmProperty);
            var nullableGuid2EdmProperty = new MetadataProviderEdmStructuralProperty(this.customerEdmType, nullableGuid2ResourceProperty, EdmCoreModel.Instance.GetGuid(true), null);
            this.customerEdmType.AddProperty(nullableGuid2EdmProperty);

            this.entitySet = new EdmEntitySetWithResourceSet(new EdmEntityContainer("Fake", "Container"), resourceSetWrapper, this.customerEdmType);
            ((EdmEntitySet)this.entitySet).AddNavigationTarget(this.bestFriendNavigation, this.entitySet);
            ((EdmEntitySet)this.entitySet).AddNavigationTarget(this.otherFriendsNavigation, this.entitySet);

            this.model.SetAnnotationValue(this.customerEdmType, this.customerResourceType);
            this.model.SetAnnotationValue(this.derivedCustomerEdmType, derivedCustomerResourceType);
            this.model.SetAnnotationValue(this.weaklyBackedCustomerEdmType, this.weaklyBackedDerivedType);
            this.model.SetAnnotationValue(this.nameProperty, nameResourceProperty);
            this.model.SetAnnotationValue(addressEdmType, addressResourceType);
            this.model.SetAnnotationValue(this.addressProperty, addressResourceProperty);
            this.model.SetAnnotationValue(this.namesProperty, namesResourceProperty);
            this.model.SetAnnotationValue(this.addressesProperty, addressesResourceProperty);
            this.model.SetAnnotationValue(this.bestFriendNavigation, bestFriendResourceProperty);
            this.model.SetAnnotationValue(this.otherFriendsNavigation, otherFriendsResourceProperty);
            this.model.SetAnnotationValue(this.weaklyBackedProperty, this.weaklyBackedResourceProperty);
            this.model.SetAnnotationValue(this.entitySet, resourceSetWrapper);
            this.model.SetAnnotationValue(guid1EdmProperty, guid1ResourceProperty);
            this.model.SetAnnotationValue(guid2EdmProperty, guid2ResourceProperty);
            this.model.SetAnnotationValue(nullableGuid1EdmProperty, nullableGuid1ResourceProperty);
            this.model.SetAnnotationValue(nullableGuid2EdmProperty, nullableGuid2ResourceProperty);

            this.testSubject = this.CreateTestSubject();
        }

        [TestMethod]
        public void TranslatorPlusTest()
        {
            this.TestBinary<int, int>(BinaryOperatorKind.Add, Constant(1), Parameter<int>("o"), o => 1 + o);
        }

        [TestMethod]
        public void TranslatorSubtractTest()
        {
            this.TestBinary<int, int>(BinaryOperatorKind.Subtract, Parameter<int>("o"), Constant(1), o => o - 1);
        }

        [TestMethod]
        public void TranslatorMultiplyTest()
        {
            this.TestBinary<int, double>(BinaryOperatorKind.Multiply, Constant(1.0), Parameter<int>("o"), o => 1.0 * o);
        }

        [TestMethod]
        public void TranslatorDivideTest()
        {
            this.TestBinary<int, double>(BinaryOperatorKind.Divide, Constant(2.0), Parameter<int>("o"), o => 2.0 / ((double)o));
        }

        [TestMethod]
        public void TranslatorModuloTest()
        {
            this.TestBinary<int, int>(BinaryOperatorKind.Modulo, Parameter<int>("o"), Constant(3), o => o % 3);
        }

        [TestMethod]
        public void TranslatorNegateTest()
        {
            this.TestUnary<int>(UnaryOperatorKind.Negate, Parameter<int>("o"), o => -o);
        }

        [TestMethod]
        public void TranslatorNotTest()
        {
            this.TestUnary<bool>(UnaryOperatorKind.Not, Parameter<bool>("o"), o => !o);
        }

        [TestMethod]
        public void TranslatorOrTest()
        {
            this.TestBinary<bool, bool>(BinaryOperatorKind.Or, Constant(false), Parameter<bool>("o"), o => false || o);
        }

        [TestMethod]
        public void TranslatorAndTest()
        {
            this.TestBinary<bool, bool>(BinaryOperatorKind.And, Parameter<bool>("o"), Constant(false), o =>  o && false);
        }

        [TestMethod]
        public void TranslatorSimpleLiteralTest()
        {
            this.TestConstant(5, c => c.Value.Should().Be(5));
        }

        [TestMethod]
        public void TranslatorNullLiteralTest()
        {
            this.TestConstant(null, c => c.Value.Should().BeNull());
        }

        [TestMethod]
        public void TranslatorSpatialLiteralTest()
        {
            var point = GeometryPoint.Create(1, 2);
            this.TestConstant(point, c =>
                                     {
                                         c.Value.Should().BeSameAs(point);
                                         c.Type.Should().Be(typeof(GeometryPoint));
                                     });
        }

        [TestMethod]
        public void TranslatorEqualToTest()
        {
            this.TestBinary<int, bool>(BinaryOperatorKind.Equal, Parameter<int>("o"), Constant(3), o => o == 3);
        }

        [TestMethod]
        public void TranslatorNotEqualToTest()
        {
            this.TestBinary<long, bool>(BinaryOperatorKind.NotEqual, Constant(3L), Parameter<long>("o"), o => 3L != o);
        }

        [TestMethod]
        public void TranslatorGreaterThanTest()
        {
            this.TestBinary<double, bool>(BinaryOperatorKind.GreaterThan, Parameter<double>("d"), Constant(3.0), d => d > 3.0);
        }

        [TestMethod]
        public void TranslatorGreaterThanOrEqualTest()
        {
            this.TestBinary<float, bool>(BinaryOperatorKind.GreaterThanOrEqual, Constant(3.0f), Parameter<float>("f"), f => 3.0f >= f);
        }

        [TestMethod]
        public void TranslatorLessThanTest()
        {
            this.TestBinary<decimal, bool>(BinaryOperatorKind.LessThan, Constant(3.0M), Parameter<decimal>("d"), d => 3.0M < d);
        }

        [TestMethod]
        public void TranslatorLessThanOrEqualTest()
        {
            this.TestBinary<decimal, bool>(BinaryOperatorKind.LessThanOrEqual, Parameter<decimal>("d"), Constant(3.0M), d => d <= 3.0M);
        }

        [TestMethod]
        public void TranslatorShouldUseSpecialComparisonForStrings()
        {
            this.TestBinary<string, bool>(BinaryOperatorKind.GreaterThan, Parameter<string>("s"), Constant("foo"), s => DataServiceProviderMethods.Compare(s, "foo") > 0);
        }

        [TestMethod]
        public void TranslatorShouldUseSpecialComparisonForGuids()
        {
            this.TestBinary<Customer, bool>(BinaryOperatorKind.GreaterThanOrEqual, this.PropertyFromParameter("e", "Guid1"), this.PropertyFromParameter("e", "Guid2"), e => DataServiceProviderMethods.Compare(e.Guid1, e.Guid2) >= 0);
        }

        [TestMethod]
        public void TranslatorShouldUseSpecialComparisonForNullableGuids()
        {
            this.TestBinary<Customer, bool>(BinaryOperatorKind.GreaterThanOrEqual, this.PropertyFromParameter("e", "NullableGuid1"), this.PropertyFromParameter("e", "NullableGuid2"), e => DataServiceProviderMethods.Compare(e.NullableGuid1, e.NullableGuid2) >= 0);
        }

        [TestMethod]
        public void TranslatorShouldUseSpecialComparisonForBooleans()
        {
            this.TestBinary<bool, bool>(BinaryOperatorKind.GreaterThan, Constant(false), Parameter<bool>("b"), b => DataServiceProviderMethods.Compare(false, b) > 0);
            this.TestBinary<bool?, bool>(BinaryOperatorKind.LessThan, Parameter<bool?>("b"), Constant(true), b => DataServiceProviderMethods.Compare(b, true) < 0);
        }

        [TestMethod]
        public void TranslatorShouldConvertNullsForEquality()
        {
            this.TestBinary<string, bool>(BinaryOperatorKind.Equal, Constant(null), Parameter<string>("s"), s => ((string)null) == s);
            this.TestBinary<string, bool>(BinaryOperatorKind.Equal, Parameter<string>("s"), Constant(null), s => s == ((string)null));
        }

        [TestMethod]
        public void TranslatorShouldConvertNulls()
        {
            this.TestBinary<int, bool>(BinaryOperatorKind.NotEqual, Constant(null), Parameter<int>("i"), i => null != ((int?)i));
            this.TestBinary<int, bool>(BinaryOperatorKind.NotEqual, Parameter<int>("i"), Constant(null), i => ((int?)i) != null);
        }

        [TestMethod]
        public void TranslatorShouldConvertPrimitivePropertyAccess()
        {
            this.TestProperty<Customer, string>(EntityParameter<Customer>("c"), this.nameProperty, c => c.Name);
        }

        [TestMethod]
        public void TranslatorShouldConvertComplexPropertyAccess()
        {
            this.TestProperty<Customer, Address>(EntityParameter<Customer>("c"), this.addressProperty, c => c.Address);
        }

        [TestMethod]
        public void TranslatorShouldConvertPrimitiveCollectionPropertyAccess()
        {
            this.TestProperty<Customer, List<string>>(EntityParameter<Customer>("c"), this.namesProperty, c => c.Names);
        }

        [TestMethod]
        public void TranslatorShouldConvertComplexCollectionPropertyAccess()
        {
            this.TestProperty<Customer, List<Address>>(EntityParameter<Customer>("c"), this.addressesProperty, c => c.Addresses);
        }

        [TestMethod]
        public void TranslatorShouldConvertReferenceNavigation()
        {
            this.TestNavigation<Customer, Customer>(EntityParameter<Customer>("c"), this.bestFriendNavigation, c => c.BestFriend);
        }

        [TestMethod]
        public void TranslatorShouldConvertCollectionNavigation()
        {
            this.TestNavigation<Customer, IEnumerable<Customer>>(EntityParameter<Customer>("c"), this.otherFriendsNavigation, c => c.OtherFriends);
        }

        [TestMethod]
        public void TranslatorShouldConvertOpenProperty()
        {
            this.TestOpenProperty<Customer, object>(EntityParameter<Customer>("c"), "Foo", c => OpenTypeMethods.GetValue(c, "Foo"));
        }

        [TestMethod]
        public void TranslatorShouldConvertOpenCollectionProperty()
        {
            this.TestOpenCollectionProperty<Customer, object>(EntityParameter<Customer>("c"), "Foo", c => OpenTypeMethods.GetCollectionValue(c, "Foo"));
        }

        [TestMethod]
        public void TranslatorShouldConvertWeaklyBackedProperty()
        {
            SingleValueNode source = EntityParameter<Customer>("c");
            QueryNode node = new SingleValuePropertyAccessNode(source, this.weaklyBackedProperty);
            var result = this.testSubject.TranslateNode(node);

            var parameterExpression = Expression.Parameter(typeof(Customer), "c");
            var methodCallExpression = Expression.Call(typeof(DataServiceProviderMethods), "GetValue", new Type[0], parameterExpression, Expression.Constant(this.weaklyBackedResourceProperty));
            var expected = Expression.Convert(methodCallExpression, typeof(string));
            CompareExpressions(expected, result);
        }

        [TestMethod]
        public void TranslatorShouldConvertSingleEntityCast()
        {
            this.TestCast<Customer, DerivedCustomer>(this.EntityParameter<Customer>("c"), this.derivedCustomerEdmType, c => c as DerivedCustomer);
        }

        [TestMethod]
        public void TranslatorShouldConvertEntityCollectionCast()
        {
            this.TestCast<Customer, IEnumerable<DerivedCustomer>>(this.CollectionNavigationFromParameter("c"), this.derivedCustomerEdmType, c => c.OtherFriends.OfType<DerivedCustomer>());
        }

        [TestMethod]
        public void TranslatorShouldConvertWeaklyBackedSingleEntityCast()
        {
            SingleResourceNode source = EntityParameter<Customer>("c");
            QueryNode node = new SingleResourceCastNode(source, this.weaklyBackedCustomerEdmType);
            var result = this.testSubject.TranslateNode(node);

            var parameterExpression = Expression.Parameter(typeof(Customer), "c");
            var expected = Expression.Call(typeof(DataServiceProviderMethods), "TypeAs", new[] { typeof(object) }, parameterExpression, Expression.Constant(this.weaklyBackedDerivedType));
            CompareExpressions(expected, result);
        }

        [TestMethod]
        public void TranslatorShouldConvertWeaklyBackedEntityCollectionCast()
        {
            CollectionResourceNode source = this.CollectionNavigationFromParameter("c");
            QueryNode node = new CollectionResourceCastNode(source, this.weaklyBackedCustomerEdmType);
            var result = this.testSubject.TranslateNode(node);

            var parameterExpression = Expression.Parameter(typeof(Customer), "c");
            var propertyExpression = Expression.Property(parameterExpression, "OtherFriends");
            var expected = Expression.Call(typeof(DataServiceProviderMethods), "OfType", new[] { typeof(Customer), typeof(object) }, propertyExpression, Expression.Constant(this.weaklyBackedDerivedType));
            expected = Expression.Call(typeof(Enumerable), "Cast", new[] { typeof(object) }, expected);
            CompareExpressions(expected, result);
        }

        [TestMethod]
        public void TranslatorShouldConvertFunctionCall()
        {
            this.TestFunctionCall<string, bool>("contains", new[] {Constant("foo"), Parameter<string>("s")}, s => "foo".Contains(s));
        }

        [TestMethod]
        public void TranslatorShouldFailOnUnrecognizedFunctionCall()
        {
            var node = new SingleValueFunctionCallNode("fake", new[] { Parameter<string>("s"), Constant("foo") }, null);
            Action translate = () => this.testSubject.TranslateNode(node);
            translate.ShouldThrow<DataServiceException>(ErrorStrings.RequestQueryParser_UnknownFunction("fake"));
        }

        [TestMethod]
        public void TranslatorShouldFailOnReplaceIfDisabled()
        {
            var withReplaceDisabled = this.CreateTestSubject(new DataServiceBehavior { AcceptReplaceFunctionInQuery = false });
            var node = new SingleValueFunctionCallNode("replace", new[] { Parameter<string>("s"), Constant("foo"), Constant("bar") }, null);
            Action translate = () => withReplaceDisabled.TranslateNode(node);
            translate.ShouldThrow<DataServiceException>(ErrorStrings.RequestQueryParser_UnknownFunction("fake"));
        }

        [TestMethod]
        public void TranslatorShouldTranslateReplaceIfEnabled()
        {
            this.testSubject = this.CreateTestSubject(new DataServiceBehavior { AcceptReplaceFunctionInQuery = true });
            this.TestFunctionCall<string, string>("replace", new[] { Parameter<string>("s"), Constant("foo"), Constant("bar") }, s => s.Replace("foo", "bar"), this.testSubject);
        }

        [TestMethod]
        public void TranslatorShouldTranslateIsOfFunctionCallWithTwoParameters()
        {
            ConstantNode constantNode;
            this.testSubject = this.CreateTestSubjectWithEdmStringLiteral(out constantNode);
            this.TestFunctionCall<object, bool>("isof", new[] { Parameter<object>("o"), constantNode }, o => o is string, this.testSubject);
        }

        [TestMethod]
        public void TranslatorShouldTranslateCastFunctionCallWithTwoParameters()
        {
            ConstantNode constantNode;
            this.testSubject = this.CreateTestSubjectWithEdmStringLiteral(out constantNode);
            this.TestFunctionCall<object, string>("cast", new[] { Parameter<object>("o"), constantNode }, o => (string)o, this.testSubject);
        }

        [TestMethod]
        public void TranslatorShouldTranslateIsOfFunctionCallWithImplicitParameter()
        {
            ConstantNode constantNode;
            var testSubjectWithResourceTypeLiteral = CreateTestSubjectWithResourceTypeStringLiteral(out constantNode);

            QueryNode node = new SingleValueFunctionCallNode("isof", new[] { constantNode }, null);

            var result = testSubjectWithResourceTypeLiteral.TranslateNode(node);

            var expected = Expression.TypeIs(this.implicitParameterExpression, typeof(Customer));
            CompareExpressions(expected, result);  
        }

        [TestMethod]
        public void TranslatorShouldConvertEmptyAny()
        {
            this.TestLambda<AnyNode, Customer, bool>(this.CollectionNavigationFromParameter("o"), null, Constant(true), o => o.OtherFriends.Any());
        }

        [TestMethod]
        public void TranslatorShouldConvertAnyWithBody()
        {
            this.TestLambda<AnyNode, Customer, bool>(this.CollectionNavigationFromParameter("o"), "i", Constant(true), o => o.OtherFriends.Any(i => true));
        }

        [TestMethod]
        public void TranslatorShouldConvertAllWithBody()
        {
            this.TestLambda<AllNode, Customer, bool>(this.CollectionNavigationFromParameter("o"), "i", Constant(false), o => o.OtherFriends.All(i => false));
        }

        [TestMethod]
        public void OpenCollectionTranslatorShouldConvertAnyWithBody()
        {
            this.TestLambda<AnyNode, Customer, bool>(this.OpenCollectionNavigationFromParameter("o", "OpenOtherCustomers"), "i", Constant(true), o => OpenTypeMethods.GetCollectionValue(o, "OpenOtherCustomers").Any(i => true));
        }

        [TestMethod]
        public void OpenCollectionTranslatorShouldConvertAllWithBody()
        {
            this.TestLambda<AllNode, Customer, bool>(this.OpenCollectionNavigationFromParameter("o", "OpenOtherCustomers"), "i", Constant(false), o => OpenTypeMethods.GetCollectionValue(o, "OpenOtherCustomers").All(i => false));
        }

        [TestMethod]
        public void TranslatorShouldRequireProtocolAndRequestVersionThreeForAnyAndAll()
        {
            ODataProtocolVersion validatedProtocolVersion = ODataProtocolVersion.V4;
            ODataProtocolVersion validatedRequestVersion = ODataProtocolVersion.V4;
            this.testSubject = this.CreateTestSubject(verifyProtocolVersion: v => { validatedProtocolVersion = v; }, verifyRequestVersion:v => { validatedRequestVersion = v; });
            
            LambdaNode node = new AnyNode(new Collection<RangeVariable>(), null);
            node.Body = Constant(true);
            node.Source = this.CollectionNavigationFromParameter("o");

            this.testSubject.TranslateNode(node);
            validatedProtocolVersion.Should().Be(ODataProtocolVersion.V4);
            validatedRequestVersion.Should().Be(ODataProtocolVersion.V4);
        }

        [TestMethod]
        public void TranslatorShouldRequireProtocolVersionThreeForCollectionTypeSegment()
        {
            ODataProtocolVersion validatedProtocolVersion = ODataProtocolVersion.V4;
            this.testSubject = this.CreateTestSubject(verifyProtocolVersion: v => { validatedProtocolVersion = v; }, verifyRequestVersion: v => { throw new Exception("Should not be called."); });

            QueryNode node = new CollectionResourceCastNode(this.CollectionNavigationFromParameter("o"), this.customerEdmType);

            this.testSubject.TranslateNode(node);
            validatedProtocolVersion.Should().Be(ODataProtocolVersion.V4);
        }

        [TestMethod]
        public void TranslatorShouldRequireProtocolVersionThreeForSingletonTypeSegment()
        {
            ODataProtocolVersion validatedProtocolVersion = ODataProtocolVersion.V4;
            this.testSubject = this.CreateTestSubject(verifyProtocolVersion: v => { validatedProtocolVersion = v; }, verifyRequestVersion: v => { throw new Exception("Should not be called."); });

            QueryNode node = new SingleResourceCastNode(this.EntityParameter<Customer>("o"), this.customerEdmType);

            this.testSubject.TranslateNode(node);
            validatedProtocolVersion.Should().Be(ODataProtocolVersion.V4);
        }

        [TestMethod]
        public void TranslatorShouldFailIfAnyAndAllAreDisabledInConfiguration()
        {
            var withAnyAndAllDisabled = this.CreateTestSubject(new DataServiceBehavior { AcceptAnyAllRequests = false });
            Action translate = () => withAnyAndAllDisabled.TranslateNode(new AnyNode(new Collection<RangeVariable>(), null));
            translate.ShouldThrow<DataServiceException>();
        }

        private void TestConstant(object value, Action<ConstantExpression> verify)
        {
            var node = new ConstantNode(value);

            var result = this.testSubject.TranslateNode(node);
            result.Should().BeAssignableTo<ConstantExpression>();
            verify(result.As<ConstantExpression>());
        }

        private void TestNavigation<TParam, TReturn>(SingleResourceNode source, IEdmNavigationProperty navigation, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            QueryNode node;
            if (navigation.Type.IsCollection())
            {
                node = new CollectionNavigationNode(source, navigation, new EdmPathExpression(navigation.Name));
            }
            else
            {
                node = new SingleNavigationNode(source, navigation, new EdmPathExpression(navigation.Name));
            }

            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestProperty<TParam, TReturn>(SingleValueNode source, IEdmProperty property, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            QueryNode node;
            if (property.Type.IsCollection())
            {
                node = new CollectionPropertyAccessNode(source, property);
            }
            else
            {
                node = new SingleValuePropertyAccessNode(source, property);
            }

            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestOpenProperty<TParam, TReturn>(SingleValueNode source, string propertyName, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            QueryNode node = new SingleValueOpenPropertyAccessNode(source, propertyName);
            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestOpenCollectionProperty<TParam, TReturn>(SingleValueNode source, string propertyName, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            QueryNode node = new CollectionOpenPropertyAccessNode(source, propertyName);
            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestBinary<TParam, TReturn>(BinaryOperatorKind kind, SingleValueNode left, SingleValueNode right, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            var node = new BinaryOperatorNode(kind, left, right);

            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestUnary<T>(UnaryOperatorKind kind, SingleValueNode operand, Expression<Func<T, T>> expectedExpression)
        {
            var node = new UnaryOperatorNode(kind, operand);

            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestCast<TParam, TReturn>(CollectionResourceNode source, IEdmEntityType cast, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            var node = new CollectionResourceCastNode(source, cast);
            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestCast<TParam, TReturn>(SingleResourceNode source, IEdmEntityType cast, Expression<Func<TParam, TReturn>> expectedExpression)
        {
            var node = new SingleResourceCastNode(source, cast);
            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestFunctionCall<TParam, TReturn>(string functionName, IEnumerable<SingleValueNode> parameters, Expression<Func<TParam, TReturn>> expectedExpression, NodeToExpressionTranslator translator = null)
        {
            if (translator == null)
            {
                translator = this.testSubject;
            }

            var node = new SingleValueFunctionCallNode(functionName, parameters, null);
            var result = translator.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private void TestLambda<TLambda, TParam, TReturn>(CollectionNode source, string parameterName, SingleValueNode body, Expression<Func<TParam, TReturn>> expectedExpression)
            where TLambda : LambdaNode
        {
            ResourceRangeVariable currentRangeVariable = null;
            if (parameterName != null)
            {
                currentRangeVariable = new ResourceRangeVariable(parameterName, new EdmEntityTypeReference(this.customerEdmType, false), this.entitySet);
                this.testSubject.ParameterExpressions[currentRangeVariable] = Expression.Parameter(typeof(TParam), parameterName);
            }

            LambdaNode node;
            if(typeof(TLambda) == typeof(AnyNode))
            {
                node = new AnyNode(new Collection<RangeVariable>(), currentRangeVariable);
            }
            else
            {
                node = new AllNode(new Collection<RangeVariable>(), currentRangeVariable);
            }

            node.Body = body;
            node.Source = source;

            var result = this.testSubject.TranslateNode(node);
            CompareExpressions(expectedExpression.Body, result);
        }

        private static void CompareExpressions(Expression expected, Expression actual)
        {
            actual.ToString().Should().Be(expected.ToString());
        }

        private static SingleValueNode Constant(object value)
        {
            var constantNode = new ConstantNode(value);
            return constantNode;
        }

        private SingleValueNode Parameter<T>(string name)
        {
            var nonentityRangeVariable = new NonResourceRangeVariable(name, null, null);
            this.testSubject.ParameterExpressions[nonentityRangeVariable] = Expression.Parameter(typeof(T), name);
            return new NonResourceRangeVariableReferenceNode(name, nonentityRangeVariable);
        }

        private SingleResourceNode EntityParameter<T>(string name)
        {
            var entityRangeVariable = new ResourceRangeVariable(name, new EdmEntityTypeReference(this.entitySet.EntityType(), false), this.entitySet);
            this.testSubject.ParameterExpressions[entityRangeVariable] = Expression.Parameter(typeof(T), name);
            return new ResourceRangeVariableReferenceNode(name, entityRangeVariable);
        }

        private CollectionResourceNode CollectionNavigationFromParameter(string name)
        {
            return new CollectionNavigationNode(this.EntityParameter<Customer>(name), this.otherFriendsNavigation, new EdmPathExpression(this.otherFriendsNavigation.Name));
        }

        private CollectionOpenPropertyAccessNode OpenCollectionNavigationFromParameter(string parameterName, string openPropertyName)
        {
            return new CollectionOpenPropertyAccessNode(this.EntityParameter<Customer>(parameterName), openPropertyName);
        }

        private SingleValueNode PropertyFromParameter(string parameterName, string propertyName)
        {
            return new SingleValuePropertyAccessNode(this.EntityParameter<Customer>(parameterName), this.customerEdmType.FindProperty(propertyName));
        }

        private NodeToExpressionTranslator CreateTestSubject(
            DataServiceBehavior dataServiceBehavior = null, 
            FunctionExpressionBinder expressionBinder = null,
            Action<ODataProtocolVersion> verifyProtocolVersion = null,
            Action<ODataProtocolVersion> verifyRequestVersion = null)
        {
            if (dataServiceBehavior == null)
            {
                dataServiceBehavior = new DataServiceBehavior();
            }

            if (expressionBinder == null)
            {
                expressionBinder = this.functionExpressionBinder;
            }

            if (verifyProtocolVersion == null)
            {
                verifyProtocolVersion = v => { };
            }

            if (verifyRequestVersion == null)
            {
                verifyRequestVersion = v => { };
            }

            return NodeToExpressionTranslator.CreateForTests(
                expressionBinder,
                dataServiceBehavior, 
                new object(), 
                false, 
                this.implicitParameterExpression, 
                verifyProtocolVersion, 
                verifyRequestVersion);
        }

        private NodeToExpressionTranslator CreateTestSubjectWithEdmStringLiteral(out ConstantNode constantNode)
        {
            var binder = new FunctionExpressionBinder(t =>
            {
                t.Should().Be("Edm.String");
                return ResourceType.GetPrimitiveResourceType(typeof(string));
            });
            var withSpecialExpressionBinder = this.CreateTestSubject(expressionBinder: binder);

            constantNode = new ConstantNode("Edm.String", "'Edm.String'");

            return withSpecialExpressionBinder;
        }

        private NodeToExpressionTranslator CreateTestSubjectWithResourceTypeStringLiteral(out ConstantNode constantNode)
        {
            string fullName = this.customerEdmType.FullName();
            var binder = new FunctionExpressionBinder(t =>
            {
                t.Should().Be(fullName);
                return this.customerResourceType;
            });
            var withSpecialExpressionBinder = this.CreateTestSubject(expressionBinder: binder);

            constantNode = new ConstantNode(fullName, "'" + fullName + "'");

            return withSpecialExpressionBinder;
        }

        private class Customer
        {
            public string Name { get; set; }
            public List<string> Names { get; set; }
            public Address Address { get; set; }
            public List<Address> Addresses { get; set; }
            public Customer BestFriend { get; set; }
            public IEnumerable<Customer> OtherFriends { get; set; }

            public Guid Guid1 { get; set; }
            public Guid Guid2 { get; set; }
            public Guid? NullableGuid1 { get; set; }
            public Guid? NullableGuid2 { get; set; }
        }

        private class DerivedCustomer : Customer
        {
        }

        private class Address
        {
        }

        private class EdmEntitySetWithResourceSet : EdmEntitySet, IResourceSetBasedEdmEntitySet
        {
            public EdmEntitySetWithResourceSet(IEdmEntityContainer container, ResourceSetWrapper resourceSet, IEdmEntityType elementType)
                : base(container, resourceSet.Name, elementType)
            {
                this.ResourceSet = resourceSet;
            }

            public ResourceSetWrapper ResourceSet { get; private set; }
        }
    }
}
