//---------------------------------------------------------------------
// <copyright file="TypePromotionUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using System;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Unit and short-span integration tests for the methods on TypePromotionUtils.
    /// </summary>
    public class TypePromotionUtilsTests
    {
        #region PromoteOperandTypes Tests (For Binary Operators)
        [Fact]
        public void EqualsOnComplexAndNullIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = null;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname"); // open property's TypeReference is null
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void NotEqualsOnNullAndComplexIsSupported()
        {
            IEdmTypeReference left = null;
            IEdmTypeReference right = HardCodedTestModel.GetPersonAddressProp().Type;
            SingleValueNode leftNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname"); // open property's TypeReference is null
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.NotEqual, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void EqualsOnComplexAndSameComplexIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = HardCodedTestModel.GetPersonAddressProp().Type;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void EqualsOnComplexAndOtherComplexIsNotSupported()
        {
            var otherComplexType = new EdmComplexTypeReference(new EdmComplexType("NS", "OtherComplex"), true);
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = otherComplexType;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(otherComplexType);
        }

        [Fact]
        public void OtherOperandsWithComplexAreNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = HardCodedTestModel.GetPersonAddressProp().Type;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.GreaterThan, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void EqualsOnEntityAndNullIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = null;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname"); // open property's TypeReference is null
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void NotEqualsOnNullAndEntityIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = null;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null)/*parent*/, "myOpenPropertyname"); // open property's TypeReference is null
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.NotEqual, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnEntityAndSameEntityIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = HardCodedTestModel.GetPersonTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnParentEntityAndChildEntityIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = HardCodedTestModel.GetEmployeeTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnChildEntityAndParentEntityIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetEmployeeTypeReference();
            IEdmTypeReference right = HardCodedTestModel.GetPersonTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnTwoDifferentEntitiesIsNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetDogTypeReference();
            IEdmTypeReference right = HardCodedTestModel.GetPersonTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetDogTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnEntityAndComplexIsNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = HardCodedTestModel.GetPersonAddressProp().Type;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void EqualsOnComplexAndEntityIsNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = HardCodedTestModel.GetPersonTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnEntitiesWithDifferentNullabilityIsSupported()
        {
            var notNullableType = new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false);
            var nullableType = new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), true);

            IEdmTypeReference left = notNullableType;
            IEdmTypeReference right = nullableType;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(nullableType);
            right.ShouldBeEquivalentTo(nullableType);

            // Reverse order
            left = nullableType;
            right = notNullableType;
            leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(nullableType);
            right.ShouldBeEquivalentTo(nullableType);
        }

        [Fact]
        public void EqualsOnComplexTypesWithDifferentNullabilityIsSupported()
        {
            var notNullableType = new EdmComplexTypeReference(HardCodedTestModel.GetAddressType(), false);
            var nullableType = new EdmComplexTypeReference(HardCodedTestModel.GetAddressType(), true);

            IEdmTypeReference left = notNullableType;
            IEdmTypeReference right = nullableType;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(nullableType);
            right.ShouldBeEquivalentTo(nullableType);

            // Reverse order
            left = nullableType;
            right = notNullableType;
            leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(nullableType);
            right.ShouldBeEquivalentTo(nullableType);
        }

        [Fact]
        public void EqualsOnPrimitiveAndEntityIsNotSupported()
        {
            IEdmTypeReference left = EdmCoreModel.Instance.GetInt32(true);
            IEdmTypeReference right = HardCodedTestModel.GetPersonTypeReference();
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
        }

        [Fact]
        public void EqualsOnPrimitiveAndComplexIsNotSupported()
        {
            IEdmTypeReference left = EdmCoreModel.Instance.GetInt32(true);
            IEdmTypeReference right = HardCodedTestModel.GetPersonAddressProp().Type;
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
        }

        [Fact]
        public void EqualsOnEntityAndPrimitiveIsNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonTypeReference();
            IEdmTypeReference right = EdmCoreModel.Instance.GetInt32(true);
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonTypeReference());
            right.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
        }

        [Fact]
        public void EqualsOnComplexAndPrimitiveIsNotSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPersonAddressProp().Type;
            IEdmTypeReference right = EdmCoreModel.Instance.GetInt32(true);
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeFalse();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPersonAddressProp().Type);
            right.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(true));
        }

        [Fact]
        public void EqualsOnEnumAndStringIsSupported()
        {
            IEdmTypeReference left = HardCodedTestModel.GetPet2PetColorPatternProperty().Type;
            IEdmTypeReference right = EdmCoreModel.Instance.GetString(true);
            SingleValueNode leftNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", left));
            SingleValueNode rightNode = new SingleValuePropertyAccessNode(new ConstantNode(null)/*parent*/, new EdmStructuralProperty(new EdmEntityType("MyNamespace", "MyEntityType"), "myPropertyName", right));
            var result = TypePromotionUtils.PromoteOperandTypes(BinaryOperatorKind.Equal, leftNode, rightNode, out left, out right, new TypeFacetsPromotionRules());

            result.Should().BeTrue();
            left.ShouldBeEquivalentTo(HardCodedTestModel.GetPet2PetColorPatternProperty().Type);
            right.ShouldBeEquivalentTo(HardCodedTestModel.GetPet2PetColorPatternProperty().Type);
        }
        #endregion

        #region PromoteOperandType Tests (For Unary Operators)

        [Fact]
        public void NegateOnEntityTypeIsNotSupported()
        {
            IEdmTypeReference type = HardCodedTestModel.GetPersonAddressProp().Type;
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeFalse();
        }

        [Fact]
        public void NegateOnComplexTypeIsNotSupported()
        {
            IEdmTypeReference type = HardCodedTestModel.GetPersonAddressProp().Type;
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeFalse();
        }

        [Fact]
        public void NegateOnNumericTypeIsSupported()
        {
            IEdmTypeReference type = EdmCoreModel.Instance.GetInt32(false);
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeTrue();
            type.ShouldBeEquivalentTo(EdmCoreModel.Instance.GetInt32(false));
        }

        [Fact]
        public void NegateOnDateTimeOffsetIsNotSupported()
        {
            IEdmTypeReference type = EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false);
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeFalse();
        }

        [Fact]
        public void NegateOnTimeOfDayIsNotSupported()
        {
            IEdmTypeReference type = EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false);
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeFalse();
        }

        [Fact]
        public void NegateOnNullIsSupported()
        {
            IEdmTypeReference type = null;
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Negate, ref type);

            result.Should().BeTrue();
            type.Should().BeNull();
        }

        [Fact]
        public void NotOnComplexTypeIsNotSupported()
        {
            IEdmTypeReference type = HardCodedTestModel.GetPersonAddressProp().Type;
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Not, ref type);

            result.Should().BeFalse();
        }

        [Fact]
        public void NotOnNumericTypeIsNotSupported()
        {
            IEdmTypeReference type = EdmCoreModel.Instance.GetInt32(false);
            var result = TypePromotionUtils.PromoteOperandType(UnaryOperatorKind.Not, ref type);

            result.Should().BeFalse();
        }
        #endregion

        [Fact]
        public void TryFindFunctionSignatureWithNumberOfArgumentsReturnsAFunctionSignatureIfMatchFound()
        {
            // regression test for: FunctionCallBinder should validate that the number of parameters matches for canonical function calls on open properties
            FunctionSignatureWithReturnType[] functions = new FunctionSignatureWithReturnType[]
            {
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDateTimeOffset(true), new IEdmTypeReference[] {EdmCoreModel.Instance.GetString(true)}),
            };

            // we specifically want to find just the first function that matches the number of arguments, we don't care about
            // ambiguity here because we're already in an ambiguous case where we don't know what kind of types
            // those arguments are.
            functions.FirstOrDefault(candidateFunction => candidateFunction.ArgumentTypes.Count() == 1).Should().NotBeNull();
        }

        [Fact]
        public void TryFundFunctionSignatureAllowsAmbiguousFunctionCall()
        {
            FunctionSignatureWithReturnType first = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDateTimeOffset(true), new IEdmTypeReference[] { EdmCoreModel.Instance.GetString(true) });
            FunctionSignatureWithReturnType second = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDateTimeOffset(true), new IEdmTypeReference[] { EdmCoreModel.Instance.GetInt32(true) });
            FunctionSignatureWithReturnType[] functions = new FunctionSignatureWithReturnType[]
            {
                first,
                second
            };

            // we specifically want to find just the first function that matches the number of arguments, we don't care about
            // ambiguity here because we're already in an ambiguous case where we don't know what kind of types
            // those arguments are.
            functions.FirstOrDefault(candidateFunction => candidateFunction.ArgumentTypes.Count() == 1).Should().Be(first);
        }

        [Fact]
        public void TryFindFunctionSignatureWithNumberOfArgumentsReturnsNothingfNoMatchFound()
        {
            // regression test for: FunctionCallBinder should validate that the number of parameters matches for canonical function calls on open properties
            FunctionSignatureWithReturnType[] functions = new FunctionSignatureWithReturnType[]
            {
                new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDateTimeOffset(true), new IEdmTypeReference[] {EdmCoreModel.Instance.GetString(true)}),
            };

            // we specifically want to find just the first function that matches the number of arguments, we don't care about
            // ambiguity here because we're already in an ambiguous case where we don't know what kind of types
            // those arguments are.
            functions.FirstOrDefault(candidateFunction => candidateFunction.ArgumentTypes.Count() == 2).Should().BeNull();
        }

        [Fact]
        public void PrimitiveTypesOfSameKindCanConvertToEachOther()
        {
            var stringType = new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            var primitiveType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);
            Assert.True(TypePromotionUtils.CanConvertTo(null, stringType, primitiveType));
            Assert.True(TypePromotionUtils.CanConvertTo(null, primitiveType, stringType));
        }

        [Fact]
        public void DefaultTypeFacetsPromotionRulesTest()
        {
            DefaultTypeFacetsPromotionRulesTestFilter(false);
            DefaultTypeFacetsPromotionRulesTestFilter(true);
            DefaultTypeFacetsPromotionRulesTestOrderBy(false);
            DefaultTypeFacetsPromotionRulesTestOrderBy(true);
        }

        internal void DefaultTypeFacetsPromotionRulesTestFilter(bool nullable)
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "Entity");
            var container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("Set", entityType);
            model.AddElements(new IEdmSchemaElement[] { entityType, container });
            entityType.AddStructuralProperty("Decimal_6_3_A", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_6_3_B", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_5_4", EdmCoreModel.Instance.GetDecimal(5, 4, nullable));
            entityType.AddStructuralProperty("Duration_6", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 6, nullable));
            entityType.AddStructuralProperty("Date", EdmCoreModel.Instance.GetDate(nullable));
            var svcRoot = new Uri("http://host", UriKind.Absolute);

            FilterClause tree;
            BinaryOperatorNode binaryNode;
            ConvertNode convertNode;
            UnaryOperatorNode unaryNode;

            // arithmeticSignatures: decimal, decimal
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul Decimal_6_3_B ne 0.0", UriKind.Absolute)).ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Scale);

            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul 0.0 ne 0.0", UriKind.Absolute)).ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(null, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(0, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Scale);

            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul Decimal_5_4 ne 0.0", UriKind.Absolute)).ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(5, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);

            // negationSignatures: decimal
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=-Decimal_6_3_A ne 0.0", UriKind.Absolute)).ParseUri().Filter;
            unaryNode = (UnaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Scale);

            // GetAdditionTermporalSignatures: date, duration
            var dateTypeDefinition = EdmCoreModel.Instance.GetDate(nullable).Definition;
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Date add Duration_6 ne 2016-08-18", UriKind.Absolute)).ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.True(binaryNode.Left is SingleValuePropertyAccessNode);
            Assert.True(binaryNode.Left.TypeReference.Definition.IsEquivalentTo(dateTypeDefinition));
            Assert.Equal(6, ((IEdmTemporalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
        }

        internal void DefaultTypeFacetsPromotionRulesTestOrderBy(bool nullable)
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "Entity");
            var container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("Set", entityType);
            model.AddElements(new IEdmSchemaElement[] { entityType, container });
            entityType.AddStructuralProperty("Decimal_6_3_A", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_6_3_B", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_5_4", EdmCoreModel.Instance.GetDecimal(5, 4, nullable));
            entityType.AddStructuralProperty("Duration_6", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 6, nullable));
            entityType.AddStructuralProperty("Date", EdmCoreModel.Instance.GetDate(nullable));
            var svcRoot = new Uri("http://host", UriKind.Absolute);

            OrderByClause tree;
            BinaryOperatorNode binaryNode;
            ConvertNode convertNode;
            UnaryOperatorNode unaryNode;

            // arithmeticSignatures: decimal, decimal
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul Decimal_6_3_B", UriKind.Absolute)).ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Scale);

            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul 0.0", UriKind.Absolute)).ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(null, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(0, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Scale);

            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul Decimal_5_4", UriKind.Absolute)).ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(5, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);

            // negationSignatures: decimal
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=-Decimal_6_3_A", UriKind.Absolute)).ParseUri().OrderBy;
            unaryNode = (UnaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Scale);

            // GetAdditionTermporalSignatures: date, duration
            var dateTypeDefinition = EdmCoreModel.Instance.GetDate(nullable).Definition;
            tree = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Date add Duration_6", UriKind.Absolute)).ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.True(binaryNode.Left is SingleValuePropertyAccessNode);
            Assert.True(binaryNode.Left.TypeReference.Definition.IsEquivalentTo(dateTypeDefinition));
            Assert.Equal(6, ((IEdmTemporalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
        }

        [Fact]
        public void CustomTypeFacetsPromotionRulesTest()
        {
            CustomTypeFacetsPromotionRulesTestFilter(false);
            CustomTypeFacetsPromotionRulesTestFilter(true);
            CustomTypeFacetsPromotionRulesTestOrderBy(false);
            CustomTypeFacetsPromotionRulesTestOrderBy(true);
        }

        internal class CustomTypeFacetsPromotionRules : TypeFacetsPromotionRules
        {
            public override int? GetPromotedPrecision(int? left, int? right)
            {
                return left  == null ? right :
                       right == null ? left  :
                       Math.Min((int)left, (int)right);
            }

            public override int? GetPromotedScale(int? left, int? right)
            {
                return left  == null ? right :
                       right == null ? left  :
                       Math.Min((int)left, (int)right);
            }
        }

        internal void CustomTypeFacetsPromotionRulesTestFilter(bool nullable)
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "Entity");
            var container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("Set", entityType);
            model.AddElements(new IEdmSchemaElement[] { entityType, container });
            entityType.AddStructuralProperty("Decimal_6_3_A", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_6_3_B", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_5_4", EdmCoreModel.Instance.GetDecimal(5, 4, nullable));
            entityType.AddStructuralProperty("Duration_6", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 6, nullable));
            entityType.AddStructuralProperty("Date", EdmCoreModel.Instance.GetDate(nullable));
            var svcRoot = new Uri("http://host", UriKind.Absolute);

            ODataUriParser parser;
            FilterClause tree;
            BinaryOperatorNode binaryNode;
            ConvertNode convertNode;
            UnaryOperatorNode unaryNode;

            // arithmeticSignatures: decimal, decimal
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul Decimal_6_3_B ne 0.0", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Scale);

            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul 0.0 ne 0.0", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(null, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(0, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Scale);

            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Decimal_6_3_A mul Decimal_5_4 ne 0.0", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(5, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Left;
            Assert.Equal(5, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(5, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(5, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);

            // negationSignatures: decimal
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=-Decimal_6_3_A ne 0.0", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().Filter;
            unaryNode = (UnaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.Equal(6, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Scale);

            // GetAdditionTermporalSignatures: date, duration
            var dateTypeDefinition = EdmCoreModel.Instance.GetDate(nullable).Definition;
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$filter=Date add Duration_6 ne 2016-08-18", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().Filter;
            binaryNode = (BinaryOperatorNode)((BinaryOperatorNode)tree.Expression).Left;
            Assert.True(binaryNode.Left is SingleValuePropertyAccessNode);
            Assert.True(binaryNode.Left.TypeReference.Definition.IsEquivalentTo(dateTypeDefinition));
            Assert.Equal(6, ((IEdmTemporalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
        }

        internal void CustomTypeFacetsPromotionRulesTestOrderBy(bool nullable)
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "Entity");
            var container = new EdmEntityContainer("NS", "Container");
            container.AddEntitySet("Set", entityType);
            model.AddElements(new IEdmSchemaElement[] { entityType, container });
            entityType.AddStructuralProperty("Decimal_6_3_A", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_6_3_B", EdmCoreModel.Instance.GetDecimal(6, 3, nullable));
            entityType.AddStructuralProperty("Decimal_5_4", EdmCoreModel.Instance.GetDecimal(5, 4, nullable));
            entityType.AddStructuralProperty("Duration_6", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 6, nullable));
            entityType.AddStructuralProperty("Date", EdmCoreModel.Instance.GetDate(nullable));
            var svcRoot = new Uri("http://host", UriKind.Absolute);

            ODataUriParser parser;
            OrderByClause tree;
            BinaryOperatorNode binaryNode;
            ConvertNode convertNode;
            UnaryOperatorNode unaryNode;

            // arithmeticSignatures: decimal, decimal
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul Decimal_6_3_B", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Scale);

            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul 0.0", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Left).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(6, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(null, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(0, ((IEdmDecimalTypeReference)((ConstantNode)convertNode.Source).TypeReference).Scale);

            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Decimal_6_3_A mul Decimal_5_4", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.Equal(5, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)binaryNode.TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Left;
            Assert.Equal(5, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);
            convertNode = (ConvertNode)binaryNode.Right;
            Assert.Equal(5, ((IEdmDecimalTypeReference)convertNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)convertNode.TypeReference).Scale);
            Assert.Equal(5, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Precision);
            Assert.Equal(4, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)convertNode.Source).TypeReference).Scale);

            // negationSignatures: decimal
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=-Decimal_6_3_A", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().OrderBy;
            unaryNode = (UnaryOperatorNode)tree.Expression;
            Assert.Equal(6, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)unaryNode.TypeReference).Scale);
            Assert.Equal(6, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Precision);
            Assert.Equal(3, ((IEdmDecimalTypeReference)((SingleValuePropertyAccessNode)unaryNode.Operand).TypeReference).Scale);

            // GetAdditionTermporalSignatures: date, duration
            var dateTypeDefinition = EdmCoreModel.Instance.GetDate(nullable).Definition;
            parser = new ODataUriParser(model, svcRoot, new Uri("http://host/Set?$orderby=Date add Duration_6", UriKind.Absolute));
            parser.Resolver.TypeFacetsPromotionRules = new CustomTypeFacetsPromotionRules();
            tree = parser.ParseUri().OrderBy;
            binaryNode = (BinaryOperatorNode)tree.Expression;
            Assert.True(binaryNode.Left is SingleValuePropertyAccessNode);
            Assert.True(binaryNode.Left.TypeReference.Definition.IsEquivalentTo(dateTypeDefinition));
            Assert.Equal(6, ((IEdmTemporalTypeReference)((SingleValuePropertyAccessNode)binaryNode.Right).TypeReference).Precision);
        }
    }
}
