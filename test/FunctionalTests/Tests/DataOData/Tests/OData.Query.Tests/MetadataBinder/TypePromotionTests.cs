//---------------------------------------------------------------------
// <copyright file="TypePromotionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for type promotion in the semantic tree.
    /// </summary>
    [TestClass, TestCase]
    public class TypePromotionTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public StronglyTypedDataServiceProviderFactory StronglyTypedDataServiceProviderFactory { get; set; }

        #region Property descriptors
        private static readonly PropertyDescriptor stringPropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "StringProperty",
            NullablePropertyName = "StringProperty",
            PromotedType = EdmCoreModel.Instance.GetString(true),
            PropertyType = EdmCoreModel.Instance.GetString(true),
        };

        private static readonly PropertyDescriptor binaryPropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "BinaryProperty",
            NullablePropertyName = "BinaryProperty",
            PromotedType = EdmCoreModel.Instance.GetBinary(true),
            PropertyType = EdmCoreModel.Instance.GetBinary(true),
        };

        private static readonly PropertyDescriptor boolPropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "BoolProperty",
            NullablePropertyName = "NullableBoolProperty",
            PromotedType = EdmCoreModel.Instance.GetBoolean(false),
            PropertyType = EdmCoreModel.Instance.GetBoolean(false),
        };

        private static readonly PropertyDescriptor bytePropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "ByteProperty",
            NullablePropertyName = "NullableByteProperty",
            PromotedType = EdmCoreModel.Instance.GetInt32(false),
            PropertyType = EdmCoreModel.Instance.GetByte(false),
        };

        private static readonly PropertyDescriptor decimalPropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "DecimalProperty",
            NullablePropertyName = "NullableDecimalProperty",
            PromotedType = EdmCoreModel.Instance.GetDecimal(false),
            PropertyType = EdmCoreModel.Instance.GetDecimal(false),
        };

        private static readonly PropertyDescriptor doublePropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "DoubleProperty",
            NullablePropertyName = "NullableDoubleProperty",
            PromotedType = EdmCoreModel.Instance.GetDouble(false),
            PropertyType = EdmCoreModel.Instance.GetDouble(false),
        };

        private static readonly PropertyDescriptor guidPropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "GuidProperty",
            NullablePropertyName = "NullableGuidProperty",
            PromotedType = EdmCoreModel.Instance.GetGuid(false),
            PropertyType = EdmCoreModel.Instance.GetGuid(false),
        };

        private static readonly PropertyDescriptor int16PropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "Int16Property",
            NullablePropertyName = "NullableInt16Property",
            PromotedType = EdmCoreModel.Instance.GetInt32(false),
            PropertyType = EdmCoreModel.Instance.GetInt16(false),
        };

        private static readonly PropertyDescriptor int32PropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "Int32Property",
            NullablePropertyName = "NullableInt32Property",
            PromotedType = EdmCoreModel.Instance.GetInt32(false),
            PropertyType = EdmCoreModel.Instance.GetInt32(false),
        };

        private static readonly PropertyDescriptor int64PropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "Int64Property",
            NullablePropertyName = "NullableInt64Property",
            PromotedType = EdmCoreModel.Instance.GetInt64(false),
            PropertyType = EdmCoreModel.Instance.GetInt64(false),
        };

        private static readonly PropertyDescriptor sbytePropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "SByteProperty",
            NullablePropertyName = "NullableSByteProperty",
            PromotedType = EdmCoreModel.Instance.GetInt32(false),
            PropertyType = EdmCoreModel.Instance.GetSByte(false),
        };

        private static readonly PropertyDescriptor singlePropertyDescriptor = new PropertyDescriptor
        {
            PropertyName = "SingleProperty",
            NullablePropertyName = "NullableSingleProperty",
            PromotedType = EdmCoreModel.Instance.GetSingle(false),
            PropertyType = EdmCoreModel.Instance.GetSingle(false),
        };
        #endregion Property descriptors

        [TestMethod, Variation(Description = "Verifies correct handling of type promotions during binding.")]
        public void RelationalOperatorTypePromotion()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.StronglyTypedDataServiceProviderFactory);

            BinaryOperatorKind[] relationalOperatorKinds = new BinaryOperatorKind[] 
            { 
                BinaryOperatorKind.Equal,
                BinaryOperatorKind.NotEqual,
                BinaryOperatorKind.GreaterThan,
                BinaryOperatorKind.GreaterThanOrEqual,
                BinaryOperatorKind.LessThan,
                BinaryOperatorKind.LessThanOrEqual
            };

            var testCases = ComputeRelationalTestCases().Concat(ComputeRelationalErrorTestCases());

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                relationalOperatorKinds,
                (testCase, relationalOperatorKind) =>
                {
                    string filter = testCase.Arguments[0] + " " + QueryTestUtils.ToOperatorName(relationalOperatorKind) + " " + testCase.Arguments[1];

                    string errorMessage = null;
                    if (testCase.ExpectedErrorMessage != null)
                    {
                        errorMessage = string.Format(CultureInfo.InvariantCulture, testCase.ExpectedErrorMessage, relationalOperatorKind);
                    }

                    var actualFilter = this.BindFilter(model, filter, errorMessage);

                    if (errorMessage == null)
                    {
                        this.Assert.IsNotNull(actualFilter, "Filter must not be null.");

                        BinaryOperatorNode binaryOperatorNode = null;
                        if (actualFilter.Expression.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            binaryOperatorNode = ((ConvertNode)actualFilter.Expression).Source as BinaryOperatorNode;
                        }
                        else
                        {
                            binaryOperatorNode = actualFilter.Expression as BinaryOperatorNode;
                        }
                        this.Assert.IsNotNull(binaryOperatorNode, "Expected a binary operator at the top of the filter.");

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            binaryOperatorNode.Left.TypeReference,
                            this.Assert);

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            binaryOperatorNode.Right.TypeReference,
                            this.Assert);
                    }

                });
        }

        [TestMethod, Variation(Description = "Verifies correct handling of type promotions during binding.")]
        public void ArithmeticOperatorTypePromotion()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.StronglyTypedDataServiceProviderFactory);

            BinaryOperatorKind[] arithmeticOperatorKinds = new BinaryOperatorKind[] 
            { 
                BinaryOperatorKind.Add,
                BinaryOperatorKind.Subtract,
                BinaryOperatorKind.Multiply,
                BinaryOperatorKind.Divide,
                BinaryOperatorKind.Modulo,
            };

            var testCases = ComputeArithmeticTestCases().Concat(ComputeArithmeticErrorTestCases());

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                arithmeticOperatorKinds,
                (testCase, arithmeticOperatorKind) =>
                {
                    string filter = testCase.Arguments[0] + " " + QueryTestUtils.ToOperatorName(arithmeticOperatorKind) + " " + testCase.Arguments[1] + " le 0";

                    string errorMessage = null;
                    if (testCase.ExpectedErrorMessage != null)
                    {
                        errorMessage = string.Format(CultureInfo.InvariantCulture, testCase.ExpectedErrorMessage, arithmeticOperatorKind);
                    }

                    var actualFilter = this.BindFilter(metadata, filter, errorMessage);

                    if (errorMessage == null)
                    {
                        this.Assert.IsNotNull(actualFilter, "Filter must not be null.");

                        BinaryOperatorNode binaryOperatorNode = null;
                        if (actualFilter.Expression.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            binaryOperatorNode = ((ConvertNode)actualFilter.Expression).Source as BinaryOperatorNode;
                        }
                        else
                        {
                            binaryOperatorNode = actualFilter.Expression as BinaryOperatorNode;
                        }
                        this.Assert.IsNotNull(binaryOperatorNode, "Expected a binary operator at the top of the filter.");

                        if (binaryOperatorNode.Left.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            binaryOperatorNode = ((ConvertNode)binaryOperatorNode.Left).Source as BinaryOperatorNode;
                        }
                        else
                        {
                            binaryOperatorNode = binaryOperatorNode.Left as BinaryOperatorNode;
                        }
                        this.Assert.IsNotNull(binaryOperatorNode, "Expected a binary operator as the left argument of the top-level binary operator.");

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            binaryOperatorNode.Left.TypeReference,
                            this.Assert);

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            binaryOperatorNode.Right.TypeReference,
                            this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies correct handling of type promotions for the 'negate' operator during binding.")]
        public void NegateTypePromotion()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.StronglyTypedDataServiceProviderFactory);

            this.CombinatorialEngineProvider.RunCombinations(
                ComputeUnaryTestCases(UnaryOperatorKind.Negate).Concat(ComputeUnaryErrorTestCases(UnaryOperatorKind.Negate)),
                (testCase) =>
                {
                    string filter = "-" + testCase.Arguments[0] + " le 0";

                    string errorMessage = null;
                    if (testCase.ExpectedErrorMessage != null)
                    {
                        errorMessage = string.Format(CultureInfo.InvariantCulture, testCase.ExpectedErrorMessage, "Negate");
                    }

                    var actualFilter = this.BindFilter(metadata, filter, errorMessage);

                    if (errorMessage == null)
                    {
                        this.Assert.IsNotNull(actualFilter, "Filter must not be null.");

                        BinaryOperatorNode binaryOperatorNode = null;
                        if (actualFilter.Expression.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            binaryOperatorNode = ((ConvertNode)actualFilter.Expression).Source as BinaryOperatorNode;
                        }
                        else
                        {
                            binaryOperatorNode = actualFilter.Expression as BinaryOperatorNode;
                        }
                        this.Assert.IsNotNull(binaryOperatorNode, "Expected a binary operator at the top of the filter.");

                        UnaryOperatorNode unaryOperatorNode = null;
                        if (binaryOperatorNode.Left.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            unaryOperatorNode = ((ConvertNode)binaryOperatorNode.Left).Source as UnaryOperatorNode;
                        }
                        else
                        {
                            unaryOperatorNode = binaryOperatorNode.Left as UnaryOperatorNode;
                        }
                        this.Assert.IsNotNull(unaryOperatorNode, "Expected a unary operator as the left argument of the binary operator.");

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            unaryOperatorNode.Operand.TypeReference,
                            this.Assert);
                    }
                });
        }

        [TestMethod, Variation(Description = "Verifies correct handling of type promotions for the 'not' operator during binding.")]
        public void NotTypePromotion()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.StronglyTypedDataServiceProviderFactory);

            // run over all operator kinds (not, negate)
            // use all combinations with the same argument types (plain and nullable)

            this.CombinatorialEngineProvider.RunCombinations(
                ComputeUnaryTestCases(UnaryOperatorKind.Not).Concat(ComputeUnaryErrorTestCases(UnaryOperatorKind.Not)),
                (testCase) =>
                {
                    string filter = "not " + testCase.Arguments[0];

                    string errorMessage = null;
                    if (testCase.ExpectedErrorMessage != null)
                    {
                        errorMessage = string.Format(CultureInfo.InvariantCulture, testCase.ExpectedErrorMessage, "Not");
                    }

                    var actualFilter = this.BindFilter(metadata, filter, errorMessage);

                    if (errorMessage == null)
                    {
                        this.Assert.IsNotNull(actualFilter, "Filter must not be null.");

                        UnaryOperatorNode unaryOperatorNode = null;
                        if (actualFilter.Expression.InternalKind == InternalQueryNodeKind.Convert)
                        {
                            unaryOperatorNode = ((ConvertNode)actualFilter.Expression).Source as UnaryOperatorNode;
                        }
                        else
                        {
                            unaryOperatorNode = actualFilter.Expression as UnaryOperatorNode;
                        }
                        this.Assert.IsNotNull(unaryOperatorNode, "Expected a unary operator at the top of the filter.");

                        QueryTestUtils.VerifyTypesAreEqual(
                            testCase.ExpectedResultType,
                            unaryOperatorNode.Operand.TypeReference,
                            this.Assert);
                    }
                });
        }

        private static IEdmTypeReference MakeNullableType(IEdmTypeReference type)
        {
            if (type.IsNullable)
            {
                return type;
            }

            return Microsoft.Test.Taupo.OData.Common.MetadataUtils.ToTypeReference(type.Definition, true);
        }

        private static IEdmTypeReference ComputeResultType(bool firstNullable, bool secondNullable, IEdmTypeReference promotedType)
        {
            // we rely on the promoted type
            if (firstNullable || secondNullable)
                return MakeNullableType(promotedType);
            return promotedType;
        }

        private static IEnumerable<TypePromotionTestCase> ComputeBinaryTestCases(PropertyDescriptor first, PropertyDescriptor second, IEdmTypeReference promotedType)
        {
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.PropertyName, second.PropertyName },
                ExpectedResultType = ComputeResultType(false, false, promotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.NullablePropertyName, second.PropertyName },
                ExpectedResultType = ComputeResultType(true, false, promotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.PropertyName, second.NullablePropertyName },
                ExpectedResultType = ComputeResultType(false, true, promotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.NullablePropertyName, second.NullablePropertyName },
                ExpectedResultType = ComputeResultType(true, true, promotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.PropertyName, "null" },
                ExpectedResultType = MakeNullableType(first.PromotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.NullablePropertyName, "null" },
                ExpectedResultType = MakeNullableType(first.PromotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { "null", second.PropertyName },
                ExpectedResultType = MakeNullableType(second.PromotedType)
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { "null", second.NullablePropertyName },
                ExpectedResultType = MakeNullableType(second.PromotedType)
            };
        }

        private static IEnumerable<TypePromotionTestCase> ComputeBinaryErrorTestCases(PropertyDescriptor first, PropertyDescriptor second)
        {
            string errorMessage = string.Format(
                CultureInfo.InvariantCulture,
                "A binary operator with incompatible types was detected. Found operand types '{0}' and '{1}' for operator kind '{{0}}'.",
                first.PropertyType.TestFullName(),
                second.PropertyType.TestFullName());

            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.PropertyName, second.PropertyName },
                ExpectedErrorMessage = errorMessage
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.NullablePropertyName, second.PropertyName },
                ExpectedErrorMessage = errorMessage
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.PropertyName, second.NullablePropertyName },
                ExpectedErrorMessage = errorMessage
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { first.NullablePropertyName, second.NullablePropertyName },
                ExpectedErrorMessage = errorMessage
            };
        }

        private static IEnumerable<TypePromotionTestCase> ComputeUnaryTestCases(PropertyDescriptor descriptor)
        {
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { descriptor.PropertyName },
                ExpectedResultType = descriptor.PromotedType
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { descriptor.NullablePropertyName },
                ExpectedResultType = MakeNullableType(descriptor.PromotedType)
            };
        }

        private static IEnumerable<TypePromotionTestCase> ComputeUnaryErrorTestCases(PropertyDescriptor descriptor)
        {
            string errorMessage = string.Format(
                CultureInfo.InvariantCulture,
                "A unary operator with an incompatible type was detected. Found operand type '{0}' for operator kind '{{0}}'.",
                descriptor.PropertyType.TestFullName());

            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { descriptor.PropertyName },
                ExpectedErrorMessage = errorMessage
            };
            yield return new TypePromotionTestCase
            {
                Arguments = new string[] { descriptor.NullablePropertyName },
                ExpectedErrorMessage = errorMessage
            };
        }

        private static IEnumerable<TypePromotionTestCase> ComputeArithmeticTestCases()
        {
            var cases = new PropertyComparison[]
                {
                    // same primitive type for both operands
                    new PropertyComparison { First = bytePropertyDescriptor, Second = bytePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = decimalPropertyDescriptor, Second = decimalPropertyDescriptor,  PromotedType = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false) },
                    new PropertyComparison { First = doublePropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = int16PropertyDescriptor, Second = int16PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = int32PropertyDescriptor, Second = int32PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = int64PropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt64(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = sbytePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = singlePropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },

                    // different primitive types for the two operands
                    new PropertyComparison { First = decimalPropertyDescriptor, Second = bytePropertyDescriptor,  PromotedType = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false) },
                    new PropertyComparison { First = decimalPropertyDescriptor, Second = int16PropertyDescriptor,  PromotedType = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false) },
                    new PropertyComparison { First = decimalPropertyDescriptor, Second = int32PropertyDescriptor,  PromotedType = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false) },
                    new PropertyComparison { First = decimalPropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false) },
                    new PropertyComparison { First = bytePropertyDescriptor, Second = int16PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = int16PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = bytePropertyDescriptor, Second = int32PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = int32PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = int16PropertyDescriptor, Second = int32PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt32(false) },
                    new PropertyComparison { First = bytePropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt64(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt64(false) },
                    new PropertyComparison { First = int16PropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt64(false) },
                    new PropertyComparison { First = int32PropertyDescriptor, Second = int64PropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetInt64(false) },
                    new PropertyComparison { First = bytePropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },
                    new PropertyComparison { First = int16PropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },
                    new PropertyComparison { First = int32PropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },
                    new PropertyComparison { First = int64PropertyDescriptor, Second = singlePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetSingle(false) },
                    new PropertyComparison { First = bytePropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = sbytePropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = int16PropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = int32PropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = int64PropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                    new PropertyComparison { First = singlePropertyDescriptor, Second = doublePropertyDescriptor,  PromotedType = EdmCoreModel.Instance.GetDouble(false) },
                };

            return cases.SelectMany(c => ComputeBinaryTestCases(c.First, c.Second, c.PromotedType));
        }

        private static IEnumerable<TypePromotionTestCase> ComputeArithmeticErrorTestCases()
        {
            var cases = new[]
                {
                    new { First = decimalPropertyDescriptor, Second = singlePropertyDescriptor },
                    new { First = decimalPropertyDescriptor, Second = doublePropertyDescriptor },
                };

            // decimal to other type promotion
            return cases.SelectMany(c => ComputeBinaryErrorTestCases(c.First, c.Second));
        }

        private static IEnumerable<TypePromotionTestCase> ComputeRelationalTestCases()
        {
            var cases = new PropertyComparison[]
                {
                    new PropertyComparison { First = stringPropertyDescriptor, Second = stringPropertyDescriptor, PromotedType = EdmCoreModel.Instance.GetString(true) },
                    new PropertyComparison { First = binaryPropertyDescriptor, Second = binaryPropertyDescriptor, PromotedType = EdmCoreModel.Instance.GetBinary(true) },
                    new PropertyComparison { First = boolPropertyDescriptor, Second = boolPropertyDescriptor, PromotedType = EdmCoreModel.Instance.GetBoolean(false) },
                    new PropertyComparison { First = guidPropertyDescriptor, Second = guidPropertyDescriptor, PromotedType = EdmCoreModel.Instance.GetGuid(false) },
                };

            // same primitive type for both operands
            return ComputeArithmeticTestCases()
                .Concat(cases.SelectMany(c => ComputeBinaryTestCases(c.First, c.Second, c.PromotedType)));
        }

        private static IEnumerable<TypePromotionTestCase> ComputeRelationalErrorTestCases()
        {
            var cases = new[]
                {
                    // string to other type promotion
                    new { First = stringPropertyDescriptor, Second = boolPropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = sbytePropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = bytePropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = int16PropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = int32PropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = int64PropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = singlePropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = doublePropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = decimalPropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = guidPropertyDescriptor },
                    new { First = stringPropertyDescriptor, Second = binaryPropertyDescriptor },

                    // Guid to other type promotion
                    new { First = guidPropertyDescriptor, Second = boolPropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = sbytePropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = bytePropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = int16PropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = int32PropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = int64PropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = singlePropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = doublePropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = decimalPropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = stringPropertyDescriptor },
                    new { First = guidPropertyDescriptor, Second = binaryPropertyDescriptor },

                    // Binary to other type promotion
                    new { First = binaryPropertyDescriptor, Second = boolPropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = sbytePropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = bytePropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = int16PropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = int32PropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = int64PropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = singlePropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = doublePropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = decimalPropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = stringPropertyDescriptor },
                    new { First = binaryPropertyDescriptor, Second = guidPropertyDescriptor },

                    // bool to other type promotion
                    new { First = boolPropertyDescriptor, Second = sbytePropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = bytePropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = int16PropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = int32PropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = int64PropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = singlePropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = doublePropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = decimalPropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = stringPropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = binaryPropertyDescriptor },
                    new { First = boolPropertyDescriptor, Second = guidPropertyDescriptor },

                    // decimal to other type promotion
                    new { First = decimalPropertyDescriptor, Second = boolPropertyDescriptor },
                    new { First = decimalPropertyDescriptor, Second = binaryPropertyDescriptor },
                    new { First = decimalPropertyDescriptor, Second = stringPropertyDescriptor },
                    new { First = decimalPropertyDescriptor, Second = guidPropertyDescriptor },
                };


            return ComputeArithmeticErrorTestCases()
                .Concat(cases.SelectMany(c => ComputeBinaryErrorTestCases(c.First, c.Second)));
        }

        private static IEnumerable<TypePromotionTestCase> ComputeUnaryTestCases(UnaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case UnaryOperatorKind.Negate:
                    var negateCases = new[]
                        {
                            new { Operand = bytePropertyDescriptor },
                            new { Operand = decimalPropertyDescriptor },
                            new { Operand = doublePropertyDescriptor },
                            new { Operand = int16PropertyDescriptor },
                            new { Operand = int32PropertyDescriptor },
                            new { Operand = int64PropertyDescriptor },
                            new { Operand = sbytePropertyDescriptor },
                            new { Operand = singlePropertyDescriptor },
                        };

                    return negateCases.SelectMany(c => ComputeUnaryTestCases(c.Operand));

                case UnaryOperatorKind.Not:
                    var notCases = new[]
                        {
                            new { Operand = boolPropertyDescriptor },
                        };

                    return notCases.SelectMany(c => ComputeUnaryTestCases(c.Operand));

                default:
                    throw new NotSupportedException();
            }
        }

        private static IEnumerable<TypePromotionTestCase> ComputeUnaryErrorTestCases(UnaryOperatorKind operatorKind)
        {

            switch (operatorKind)
            {
                case UnaryOperatorKind.Negate:
                    var negateCases = new[]
                        {
                            new { Operand = stringPropertyDescriptor },
                            new { Operand = boolPropertyDescriptor },
                            new { Operand = guidPropertyDescriptor },
                            new { Operand = binaryPropertyDescriptor },
                        };

                    return negateCases.SelectMany(c => ComputeUnaryErrorTestCases(c.Operand));

                case UnaryOperatorKind.Not:
                    var notCases = new[]
                        {
                            new { Operand = stringPropertyDescriptor },
                            new { Operand = guidPropertyDescriptor },
                            new { Operand = sbytePropertyDescriptor },
                            new { Operand = bytePropertyDescriptor },
                            new { Operand = int16PropertyDescriptor },
                            new { Operand = int32PropertyDescriptor },
                            new { Operand = int64PropertyDescriptor },
                            new { Operand = singlePropertyDescriptor },
                            new { Operand = doublePropertyDescriptor },
                            new { Operand = decimalPropertyDescriptor },
                            new { Operand = binaryPropertyDescriptor },
                        };

                    return notCases.SelectMany(c => ComputeUnaryErrorTestCases(c.Operand));

                default:
                    throw new NotSupportedException();
            }
        }

        private FilterClause BindFilter(IEdmModel model, string filter, string errorMessage)
        {
            FilterClause actualFilter = null;
            TestExceptionUtils.ExpectedException<ODataException>(
                this.Assert,
                () =>
                {
                    string query = "/TypesWithPrimitiveProperties?$filter=" + filter;
                    ODataUri actual = QueryNodeUtils.BindQuery(query, model);

                    // the filter node should be the top-level node in the query tree
                    actualFilter = (FilterClause)actual.Filter;
                },
                errorMessage,
                null);
            return actualFilter;
        }

        public class TypePromotionTestCase
        {
            public string[] Arguments { get; set; }
            public IEdmTypeReference ExpectedResultType { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }

        private sealed class PropertyDescriptor
        {
            public string PropertyName { get; set; }
            public string NullablePropertyName { get; set; }
            public IEdmTypeReference PromotedType { get; set; }
            public IEdmTypeReference PropertyType { get; set; }
        }

        private sealed class PropertyComparison
        {
            public PropertyDescriptor First { get; set; }
            public PropertyDescriptor Second { get; set; }
            public IEdmPrimitiveTypeReference PromotedType { get; set; }
        }
    }
}
