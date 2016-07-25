//---------------------------------------------------------------------
// <copyright file="FilterBinderFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.MetadataBinder
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
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
    /// Tests for the MetadataBinder on a filter expression.
    /// </summary>
    [TestClass, TestCase]
    public class FilterBinderFunctionalTests : ODataTestCase
    {
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public UntypedDataServiceProviderFactory UntypedDataServiceProviderFactory { get; set; }

        public class FilterTestCase
        {
            public FilterTestCase() { this.EntitySetName = "Customers"; }
            public string EntitySetName { get; set; }
            public string Filter { get; set; }
            public SingleValueNode ExpectedFilterCondition { get; set; }
            public override string ToString() { return this.Filter; }
        }

        static IEnumerable<FilterTestCase> LiteralTestCases()
        {
            foreach (string booleanLiteral in new[] { "true", "false" })
            {
                yield return new FilterTestCase()
                {
                    Filter = booleanLiteral,
                    ExpectedFilterCondition = new ConstantNode(bool.Parse(booleanLiteral))
                };
            }
        }

        static IEnumerable<FilterTestCase> BinaryOperatorTestCases()
        {
            #region Single operator
            foreach (var operatorKind in QueryTestUtils.BinaryOperatorGroups.Where(og => og.IsRelational).SelectMany(og => og.OperatorKinds))
            {
                yield return new FilterTestCase()
                {
                    Filter = "false " + operatorKind.ToOperatorName() + " true",
                    ExpectedFilterCondition = new BinaryOperatorNode(operatorKind, new ConstantNode(false), new ConstantNode(true))
                };
            }

            foreach (var operatorKind in QueryTestUtils.BinaryOperatorGroups.Where(og => !og.IsRelational).SelectMany(og => og.OperatorKinds))
            {
                BinaryOperatorNode left = new BinaryOperatorNode(operatorKind, new ConstantNode(1), new ConstantNode(2));
                yield return new FilterTestCase()
                {
                    Filter = "1 " + operatorKind.ToOperatorName() + " 2 le 5",
                    ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, left, new ConstantNode(5))
                };
            }
            #endregion Single operator

            #region Two operators from the same group
            foreach (var operatorGroup in QueryTestUtils.BinaryOperatorGroups.Where(od => od.IsRelational))
            {
                IEnumerable<BinaryOperatorKind[]> operatorPairs = new[] {
                    new BinaryOperatorKind[] { operatorGroup.OperatorKinds[0], operatorGroup.OperatorKinds[0] }
                };
                if (operatorGroup.OperatorKinds.Length > 1)
                {
                    operatorPairs = operatorPairs.Concat(operatorGroup.OperatorKinds.Variations(2));
                }

                foreach (var operatorPair in operatorPairs)
                {
                    BinaryOperatorNode left = new BinaryOperatorNode(operatorPair[0], new ConstantNode(true), new ConstantNode(false));
                    yield return new FilterTestCase()
                    {
                        Filter = "true " + operatorPair[0].ToOperatorName() + " false " + operatorPair[1].ToOperatorName() + " true",
                        ExpectedFilterCondition = new BinaryOperatorNode(operatorPair[1], left, new ConstantNode(true))
                    };

                    BinaryOperatorNode right = new BinaryOperatorNode(operatorPair[1], new ConstantNode(false), new ConstantNode(true));
                    yield return new FilterTestCase()
                    {
                        Filter = "true " + operatorPair[0].ToOperatorName() + " (false " + operatorPair[1].ToOperatorName() + " true)",
                        ExpectedFilterCondition = new BinaryOperatorNode(operatorPair[0], new ConstantNode(true), right)
                    };
                }
            }

            foreach (var operatorGroup in QueryTestUtils.BinaryOperatorGroups.Where(od => !od.IsRelational))
            {
                IEnumerable<BinaryOperatorKind[]> operatorPairs = new[] {
                    new BinaryOperatorKind[] { operatorGroup.OperatorKinds[0], operatorGroup.OperatorKinds[0] }
                };
                if (operatorGroup.OperatorKinds.Length > 1)
                {
                    operatorPairs = operatorPairs.Concat(operatorGroup.OperatorKinds.Variations(2));
                }

                foreach (var operatorPair in operatorPairs)
                {
                    BinaryOperatorNode innerLeft1 = new BinaryOperatorNode(operatorPair[0], new ConstantNode(1), new ConstantNode(2));
                    BinaryOperatorNode outerLeft1 = new BinaryOperatorNode(operatorPair[1], innerLeft1, new ConstantNode(3));
                    yield return new FilterTestCase()
                    {
                        Filter = "1 " + operatorPair[0].ToOperatorName() + " 2 " + operatorPair[1].ToOperatorName() + " 3 le 5",
                        ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outerLeft1, new ConstantNode(5))
                    };


                    BinaryOperatorNode innerRight2 = new BinaryOperatorNode(operatorPair[1], new ConstantNode(2), new ConstantNode(3));
                    BinaryOperatorNode outerLeft2 = new BinaryOperatorNode(operatorPair[0], new ConstantNode(1), innerRight2);
                    yield return new FilterTestCase()
                    {
                        Filter = "1 " + operatorPair[0].ToOperatorName() + " (2 " + operatorPair[1].ToOperatorName() + " 3) le 5",
                        ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outerLeft2, new ConstantNode(5))
                    };
                }
            }
            #endregion Two operators from the same group

            #region Two operators from different groups
            foreach (var operatorGroupHigher in QueryTestUtils.BinaryOperatorGroups.Where(og => og.IsRelational))
            {
                foreach (var operatorGroupLower in QueryTestUtils.BinaryOperatorGroups.Where(og => og.IsRelational && og.Priority > operatorGroupHigher.Priority))
                {
                    foreach (var operatorKindHigher in operatorGroupHigher.OperatorKinds)
                    {
                        foreach (var operatorKindLower in operatorGroupLower.OperatorKinds)
                        {
                            BinaryOperatorNode left = new BinaryOperatorNode(operatorKindLower, new ConstantNode(true), new ConstantNode(false));
                            // Lower and higher
                            yield return new FilterTestCase()
                            {
                                Filter = "true " + operatorKindLower.ToOperatorName() + " false " + operatorKindHigher.ToOperatorName() + " true",
                                ExpectedFilterCondition = new BinaryOperatorNode(operatorKindHigher, left, new ConstantNode(true))
                            };

                            // Lower and higher with parentheses
                            BinaryOperatorNode right = new BinaryOperatorNode(operatorKindHigher, new ConstantNode(false), new ConstantNode(true));
                            yield return new FilterTestCase()
                            {
                                Filter = "true " + operatorKindLower.ToOperatorName() + " (false " + operatorKindHigher.ToOperatorName() + " true)",
                                ExpectedFilterCondition = new BinaryOperatorNode(operatorKindLower, new ConstantNode(true), right)

                            };

                            BinaryOperatorNode right2 = new BinaryOperatorNode(operatorKindLower, new ConstantNode(false), new ConstantNode(true));

                            // Higher and lower
                            yield return new FilterTestCase()
                            {
                                Filter = "true " + operatorKindHigher.ToOperatorName() + " false " + operatorKindLower.ToOperatorName() + " true",
                                ExpectedFilterCondition = new BinaryOperatorNode(operatorKindHigher, new ConstantNode(true), right2)
                            };
                        }
                    }
                }
            }

            foreach (var operatorGroupHigher in QueryTestUtils.BinaryOperatorGroups.Where(og => !og.IsRelational))
            {
                foreach (var operatorGroupLower in QueryTestUtils.BinaryOperatorGroups.Where(og => !og.IsRelational && og.Priority > operatorGroupHigher.Priority))
                {
                    foreach (var operatorKindHigher in operatorGroupHigher.OperatorKinds)
                    {
                        foreach (var operatorKindLower in operatorGroupLower.OperatorKinds)
                        {
                            // Lower and higher
                            BinaryOperatorNode innerLeft = new BinaryOperatorNode(operatorKindLower, new ConstantNode(1), new ConstantNode(2));
                            BinaryOperatorNode outerLeft = new BinaryOperatorNode(operatorKindHigher, innerLeft, new ConstantNode(3));
                            yield return new FilterTestCase()
                            {
                                Filter = "1 " + operatorKindLower.ToOperatorName() + " 2 " + operatorKindHigher.ToOperatorName() + " 3 le 5",
                                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outerLeft, new ConstantNode(5))
                            };

                            // Lower and higher with parentheses
                            BinaryOperatorNode innerRight = new BinaryOperatorNode(operatorKindHigher, new ConstantNode(2), new ConstantNode(3));
                            BinaryOperatorNode outerLeft2 = new BinaryOperatorNode(operatorKindLower, new ConstantNode(1), innerRight);
                            yield return new FilterTestCase()
                            {
                                Filter = "1 " + operatorKindLower.ToOperatorName() + " (2 " + operatorKindHigher.ToOperatorName() + " 3) le 5",
                                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outerLeft2, new ConstantNode(5))
                            };

                            // Higher and lower
                            BinaryOperatorNode innerRight2 = new BinaryOperatorNode(operatorKindLower, new ConstantNode(2), new ConstantNode(3));
                            BinaryOperatorNode outerLeft3 = new BinaryOperatorNode(operatorKindHigher, new ConstantNode(1), innerRight2);

                            yield return new FilterTestCase()
                            {
                                Filter = "1 " + operatorKindHigher.ToOperatorName() + " 2 " + operatorKindLower.ToOperatorName() + " 3 le 5",
                                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outerLeft3, new ConstantNode(5))
                            };
                        }
                    }
                }
            }
            #endregion Two operators from different groups
        }

        public static IEnumerable<FilterTestCase> UnaryOperatorTestCases()
        {
            // Single unary operator
            UnaryOperatorNode left = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Negate.ToOperatorName() + "(1) le 5",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, left, new ConstantNode(5))
            };

            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Not.ToOperatorName() + " true",
                ExpectedFilterCondition = new UnaryOperatorNode(UnaryOperatorKind.Not, new ConstantNode(true))
            };

            // Two unary operators
            UnaryOperatorNode inner = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(1));
            UnaryOperatorNode outer = new UnaryOperatorNode(UnaryOperatorKind.Negate, inner);
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Negate.ToOperatorName() + "(" + UnaryOperatorKind.Negate.ToOperatorName() + "(1)) le 5",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outer, new ConstantNode(5))
            };

            UnaryOperatorNode inner2 = new UnaryOperatorNode(UnaryOperatorKind.Not, new ConstantNode(true));
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Not.ToOperatorName() + " " + UnaryOperatorKind.Not.ToOperatorName() + " true",
                ExpectedFilterCondition = new UnaryOperatorNode(UnaryOperatorKind.Not, inner2)
            };

            // Unary and binary operator.
            UnaryOperatorNode inner3 = new UnaryOperatorNode(UnaryOperatorKind.Not, new ConstantNode(true));
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Not.ToOperatorName() + " true " + BinaryOperatorKind.Equal.ToOperatorName() + " false",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, inner3, new ConstantNode(false))
            };

            // With parenthesis
            BinaryOperatorNode innerBinary = new BinaryOperatorNode(BinaryOperatorKind.Add, new ConstantNode(2), new ConstantNode(3));
            UnaryOperatorNode outer2 = new UnaryOperatorNode(UnaryOperatorKind.Negate, innerBinary);
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Negate.ToOperatorName() + " (2 " + BinaryOperatorKind.Add.ToOperatorName() + " 3) le 5",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, outer2, new ConstantNode(5))
            };

            BinaryOperatorNode innerBinary2 = new BinaryOperatorNode(BinaryOperatorKind.Equal, new ConstantNode(true), new ConstantNode(false));
            yield return new FilterTestCase()
            {
                Filter = UnaryOperatorKind.Not.ToOperatorName() + " (true " + BinaryOperatorKind.Equal.ToOperatorName() + " false)",
                ExpectedFilterCondition = new UnaryOperatorNode(UnaryOperatorKind.Not, innerBinary2)
            };
        }

        public static IEnumerable<FilterTestCase> PropertyAccessTestCases(IEdmModel model)
        {
            // Accessing a primitive property on the entity type
            ResourceRangeVariable customersEntityRangeVariable = new ResourceRangeVariable("dummy", model.ResolveTypeReference("TestNS.Customer", false).AsEntity(), model.FindEntityContainer("BinderTestMetadata").FindEntitySet("Customers"));
            SingleValuePropertyAccessNode propertyAccessNode = new SingleValuePropertyAccessNode(new ResourceRangeVariableReferenceNode(customersEntityRangeVariable.Name, customersEntityRangeVariable),
                        model.ResolveProperty("TestNS.Customer.Name"));
            yield return new FilterTestCase()
            {
                Filter = "Name eq 'Vitek'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, propertyAccessNode, new ConstantNode("Vitek"))
            };

            // Accessing a complex on entity and primitive on complex
            SingleValuePropertyAccessNode propertyAccessNode2 = new SingleValuePropertyAccessNode(
                new SingleComplexNode(
                    new ResourceRangeVariableReferenceNode(customersEntityRangeVariable.Name, customersEntityRangeVariable),
                    model.ResolveProperty("TestNS.Customer.Address")
                    ),
                model.ResolveProperty("TestNS.Address.City")
                );
            yield return new FilterTestCase()
            {
                Filter = "Address/City ne 'Prague'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.NotEqual, propertyAccessNode2, new ConstantNode("Prague"))
            };
        }

        public static IEnumerable<FilterTestCase> TopFilterExpressionTestCases(IEdmModel model)
        {
            ResourceRangeVariable entityRangeVariable = new ResourceRangeVariable("dummy", model.ResolveTypeReference("TestNS.TypeWithPrimitiveProperties", false).AsEntity(), model.FindEntityContainer("BinderTestMetadata").FindEntitySet("TypesWithPrimitiveProperties"));
            yield return new FilterTestCase()
            {
                EntitySetName = "TypesWithPrimitiveProperties",
                Filter = "BoolProperty",
                ExpectedFilterCondition = new SingleValuePropertyAccessNode(
                 new ResourceRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable),
                 model.ResolveProperty("TestNS.TypeWithPrimitiveProperties.BoolProperty")
             )
            };

            yield return new FilterTestCase()
            {
                EntitySetName = "TypesWithPrimitiveProperties",
                Filter = "NullableBoolProperty",
                ExpectedFilterCondition = new SingleValuePropertyAccessNode(
              new ResourceRangeVariableReferenceNode(entityRangeVariable.Name, entityRangeVariable),
              model.ResolveProperty("TestNS.TypeWithPrimitiveProperties.NullableBoolProperty")
          )
            };
        }

        public static IEnumerable<FilterTestCase> BuiltInStringFunctionCallTestCases(IEdmModel model)
        {
            QueryNode[] args = new QueryNode[]
            {
                new ConstantNode("Johny"),
                new ConstantNode ("John" )
            };
            yield return new FilterTestCase
            {
                Filter = "endswith('Johny','John')",
                ExpectedFilterCondition = new SingleValueFunctionCallNode("endswith", args, EdmCoreModel.Instance.GetBoolean(false))
            };

            QueryNode[] args2 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode("John")
                };
            SingleValueFunctionCallNode outer = new SingleValueFunctionCallNode("indexof", args2, EdmCoreModel.Instance.GetInt32(false));
            yield return new FilterTestCase
            {
                Filter = "indexof('Johny','John') eq 0",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer, new ConstantNode(0))
            };

            QueryNode[] args3 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode("John"),
                    new ConstantNode("Vitek")
                };
            SingleValueFunctionCallNode outer2 = new SingleValueFunctionCallNode("replace", args3, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "replace('Johny','John','Vitek') eq ''",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer2, new ConstantNode(""))
            };

            QueryNode[] args4 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode("John") 
                };
            yield return new FilterTestCase
            {
                Filter = "startswith('Johny','John')",
                ExpectedFilterCondition = new SingleValueFunctionCallNode("startswith", args4, EdmCoreModel.Instance.GetBoolean(false))
            };

            QueryNode[] args5 = new QueryNode[]
                {
                    new ConstantNode("Johny")
                };
            SingleValueFunctionCallNode outer3 = new SingleValueFunctionCallNode("tolower", args5, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "tolower('Johny') eq 'johny'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer3, new ConstantNode("johny"))
            };

            QueryNode[] args6 = new QueryNode[]
                {
                    new ConstantNode("Johny")
                };
            SingleValueFunctionCallNode outer4 = new SingleValueFunctionCallNode("toupper", args6, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "toupper('Johny') eq 'JOHNY'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer4, new ConstantNode("JOHNY"))
            };

            QueryNode[] args7 = new QueryNode[]
                {
                    new ConstantNode("Johny")
                };
            SingleValueFunctionCallNode outer5 = new SingleValueFunctionCallNode("trim", args7, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "trim('Johny') eq ''",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer5, new ConstantNode(""))
            };

            QueryNode[] args8 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode (3)
                };
            SingleValueFunctionCallNode outer6 = new SingleValueFunctionCallNode("substring", args8, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "substring('Johny',3) eq 'ny'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer6, new ConstantNode("ny"))
            };

            QueryNode[] args9 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode(3),
                    new ConstantNode(1)
                };
            SingleValueFunctionCallNode outer7 = new SingleValueFunctionCallNode("substring", args9, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "substring('Johny',3,1) eq 'n'",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer7, new ConstantNode("n"))
            };

            QueryNode[] args10 = new QueryNode[]
                {
                    new ConstantNode("oh"),
                    new ConstantNode("Johny")
                };
            yield return new FilterTestCase
            {
                Filter = "contains('oh','Johny')",
                ExpectedFilterCondition = new SingleValueFunctionCallNode("contains", args10, EdmCoreModel.Instance.GetBoolean(false))
            };

            QueryNode[] args11 = new QueryNode[]
                {
                    new ConstantNode("Johny"),
                    new ConstantNode(" Smith")
                };
            SingleValueFunctionCallNode outer8 = new SingleValueFunctionCallNode("concat", args11, EdmCoreModel.Instance.GetString(true));
            yield return new FilterTestCase
            {
                Filter = "concat('Johny',' Smith') eq ''",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.Equal, outer8, new ConstantNode(""))
            };

            QueryNode[] args12 = new QueryNode[]
                {
                    new ConstantNode("Johny")
                };
            SingleValueFunctionCallNode outer9 = new SingleValueFunctionCallNode("length", args12, EdmCoreModel.Instance.GetInt32(false));
            yield return new FilterTestCase
            {
                Filter = "length('Johny') gt 0",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.GreaterThan, outer9, new ConstantNode(0))
            };
        }

        public static IEnumerable<FilterTestCase> BuiltInDateTimeOffsetFunctionCallTestCases(IEdmModel model)
        {
            string[] functionNames = new string[] { "year", "month", "day", "hour", "minute", "second" };
            QueryNode[] args = new QueryNode[]
                {
                    new ConstantNode(new DateTimeOffset(2010, 12, 9, 14, 10, 20, TimeSpan.Zero))
                };
            return functionNames.Select(functionName => new FilterTestCase
            {
                Filter = functionName + "(2010-12-09T14:10:20Z) gt 0",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.GreaterThan, new SingleValueFunctionCallNode(functionName, args, EdmCoreModel.Instance.GetInt32(false)), new ConstantNode(0))
            });
        }

        public static IEnumerable<FilterTestCase> BuiltInMathFunctionCallTestCases(IEdmModel model)
        {
            string[] functionNames = new string[] { "round", "floor", "ceiling" };
            var mathFunctionTypes = new[]
                {
                    new { Type = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDouble(false), Literal = "42.42", Value = (object)(double)42.42 },
                    new { Type = (IEdmPrimitiveTypeReference)EdmCoreModel.Instance.GetDecimal(false), Literal = "42m", Value = (object)(decimal)42 }
                };

            return functionNames.SelectMany(functionName => mathFunctionTypes.Select(mathFunctionType => new FilterTestCase
            {
                Filter = functionName + "(" + mathFunctionType.Literal + ")" + " gt 0",
                ExpectedFilterCondition = new BinaryOperatorNode(BinaryOperatorKind.GreaterThan,
                    new SingleValueFunctionCallNode(functionName, new QueryNode[] { new ConstantNode(mathFunctionType.Value) }, mathFunctionType.Type),
                    new ConstantNode(mathFunctionType.Type.AsPrimitive().PrimitiveKind() == EdmPrimitiveTypeKind.Decimal ? (object)0m : (object)0d))
            }));
        }

        //// TODO: test more complex filters
        //// TODO: test error cases
        [TestMethod, Variation(Description = "Verifies correct binding of filters.")]
        public void FilterBinderTest()
        {
            IEdmModel model = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            this.CombinatorialEngineProvider.RunCombinations(
                LiteralTestCases()
                .Concat(UnaryOperatorTestCases())
                .Concat(BinaryOperatorTestCases())
                .Concat(PropertyAccessTestCases(model))
                .Concat(TopFilterExpressionTestCases(model))
                .Concat(BuiltInStringFunctionCallTestCases(model))
                .Concat(BuiltInDateTimeOffsetFunctionCallTestCases(model))
                .Concat(BuiltInMathFunctionCallTestCases(model)),
                (testCase) =>
                {
                    string query = "/" + testCase.EntitySetName + "?$filter=" + testCase.Filter;
                    ODataUri actual = QueryNodeUtils.BindQuery(query, model);

                    // construct the expected filter node
                    var entitySet = model.FindDeclaredEntitySet(testCase.EntitySetName);
                    CollectionResourceNode entityCollectionNode = new EntitySetNode(entitySet);
                    var expectedFilter = new FilterClause(
                        testCase.ExpectedFilterCondition,
                        new ResourceRangeVariable(ExpressionConstants.It, entitySet.EntityType().ToTypeReference(false).AsEntity(), entityCollectionNode)
                        );

                    QueryNodeUtils.VerifyFilterClausesAreEqual(
                        expectedFilter,
                        actual.Filter,
                        this.Assert);
                });
        }

        public class InvalidFilterTestCase
        {
            public string Filter { get; set; }
            public string ExpectedExceptionMessage { get; set; }
            public override string ToString() { return this.Filter; }
        }

        public IEnumerable<InvalidFilterTestCase> InvalidBinaryOperatorTestCases()
        {
            return new InvalidFilterTestCase[] 
            {
                // TODO: Add invalid binary operator tests
            };
        }

        public IEnumerable<InvalidFilterTestCase> InvalidUnaryOperatorTestCases()
        {
            return new InvalidFilterTestCase[] 
            {
                // TODO: Add invalid unary operator tests
            };
        }

        public IEnumerable<InvalidFilterTestCase> InvalidPropertyAccessTestCases()
        {
            return new InvalidFilterTestCase[] 
            {
                // TODO: Add tests for invalid source, currently we don't have support for tokens which would bind o a non-single value in a filter expression

                new InvalidFilterTestCase {
                    Filter = "Emails",
                    ExpectedExceptionMessage = "The $filter expression must evaluate to a single boolean value."
                },
                new InvalidFilterTestCase {
                    Filter = "NonExistant",
                    ExpectedExceptionMessage = "Could not find a property named 'NonExistant' on type 'TestNS.Customer'."
                },
            };
        }

        [TestMethod, Variation(Description = "Verifies the binder correctly fails on invalid filter expressions.")]
        public void InvalidFilterBinderTest()
        {
            var metadata = QueryTestMetadata.BuildTestMetadata(this.PrimitiveTypeResolver, this.UntypedDataServiceProviderFactory);

            this.CombinatorialEngineProvider.RunCombinations(
                InvalidBinaryOperatorTestCases()
                .Concat(InvalidUnaryOperatorTestCases())
                .Concat(InvalidPropertyAccessTestCases()),
                (testCase) =>
                {
                    string query = "/Customers?$filter=" + testCase.Filter;
                    this.Assert.ExpectedException<ODataException>(
                        () => QueryNodeUtils.BindQuery(query, metadata),
                        testCase.ExpectedExceptionMessage,
                        "Invalid filter binder test.");
                });
        }
    }
}
