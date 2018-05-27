//---------------------------------------------------------------------
// <copyright file="QueryTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests
{
    #region Namespaces
    using System.Collections;
    using Microsoft.OData.Edm;
    using System.Linq;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for writing query tests.
    /// </summary>
    internal static class QueryTestUtils
    {
        public static BinaryOperatorKind[] BinaryOperatorKinds = new BinaryOperatorKind[]
        {
            BinaryOperatorKind.Add,
            BinaryOperatorKind.And,
            BinaryOperatorKind.Divide,
            BinaryOperatorKind.Equal,
            BinaryOperatorKind.GreaterThan,
            BinaryOperatorKind.GreaterThanOrEqual,
            BinaryOperatorKind.LessThan,
            BinaryOperatorKind.LessThanOrEqual,
            BinaryOperatorKind.Modulo,
            BinaryOperatorKind.Multiply,
            BinaryOperatorKind.NotEqual,
            BinaryOperatorKind.Or,
            BinaryOperatorKind.Subtract
        };

        public static UnaryOperatorKind[] UnaryOperatorKinds = new UnaryOperatorKind[]
        {
            UnaryOperatorKind.Negate,
            UnaryOperatorKind.Not
        };

        internal class BinaryOperatorGroup
        {
            public int Priority { get; set; }
            public bool IsRelational { get; set; }
            public BinaryOperatorKind[] OperatorKinds { get; set; }
        }

        internal static BinaryOperatorGroup[] BinaryOperatorGroups = new BinaryOperatorGroup[]
        {
            new BinaryOperatorGroup
            {
                Priority = 1,
                IsRelational = true,
                OperatorKinds = new BinaryOperatorKind[]
                {
                    BinaryOperatorKind.Or,
                }
            },
            new BinaryOperatorGroup
            {
                Priority = 2,
                IsRelational = true,
                OperatorKinds = new BinaryOperatorKind[]
                {
                    BinaryOperatorKind.And,
                }
            },
            new BinaryOperatorGroup
            {
                Priority = 3,
                IsRelational = true,
                OperatorKinds = new BinaryOperatorKind[]
                {
                    BinaryOperatorKind.Equal,
                    BinaryOperatorKind.NotEqual,
                    BinaryOperatorKind.GreaterThan,
                    BinaryOperatorKind.GreaterThanOrEqual,
                    BinaryOperatorKind.LessThan,
                    BinaryOperatorKind.LessThanOrEqual,
                }
            },
            new BinaryOperatorGroup
            {
                Priority = 4,
                IsRelational = false,
                OperatorKinds = new BinaryOperatorKind[]
                {
                    BinaryOperatorKind.Add,
                    BinaryOperatorKind.Subtract,
                }
            },
            new BinaryOperatorGroup
            {
                Priority = 5,
                IsRelational = false,
                OperatorKinds = new BinaryOperatorKind[]
                {
                    BinaryOperatorKind.Multiply,
                    BinaryOperatorKind.Divide,
                    BinaryOperatorKind.Modulo
                }
            },
        };

        public static string ToOperatorName(this BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Add: return "add";
                case BinaryOperatorKind.And: return "and";
                case BinaryOperatorKind.Divide: return "div";
                case BinaryOperatorKind.Equal: return "eq";
                case BinaryOperatorKind.GreaterThan: return "gt";
                case BinaryOperatorKind.GreaterThanOrEqual: return "ge";
                case BinaryOperatorKind.LessThan: return "lt";
                case BinaryOperatorKind.LessThanOrEqual: return "le";
                case BinaryOperatorKind.Modulo: return "mod";
                case BinaryOperatorKind.Multiply: return "mul";
                case BinaryOperatorKind.NotEqual: return "ne";
                case BinaryOperatorKind.Or: return "or";
                case BinaryOperatorKind.Subtract: return "sub";
                default: return null;
            }
        }

        public static string ToOperatorName(this UnaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case UnaryOperatorKind.Negate: return "-";
                case UnaryOperatorKind.Not: return "not";
                default: return null;
            }
        }

        public static void VerifyTypesAreEqual(IEdmTypeReference expected, IEdmTypeReference actual, AssertionHandler assert)
        {
            assert.IsTrue(expected.IsEquivalentTo(actual), "The expected and the actual types don't match.");
        }

        public static void VerifyPropertiesAreEqual(IEdmProperty expected, IEdmProperty actual, AssertionHandler assert)
        {
            if (expected == null)
            {
                assert.IsNull(actual, "The property should be null.");
            }
            else
            {
                assert.IsNotNull(actual, "The property should not be null.");
                assert.AreEqual(expected.Name, actual.Name, "The property names are different.");
                // No need to verify the type of the property, we just need to verify the declaring type, which can't be done here
                //  if the declaring type is the same and the names are the same, it's the same property.
            }
        }

        public static void VerifyEntitySetsAreEqual(IEdmEntitySet expected, IEdmEntitySet actual, AssertionHandler assert)
        {
            if (expected == null)
            {
                assert.IsNull(actual, "The entity set should be null.");
            }
            else
            {
                assert.IsNotNull(actual, "The entity set should not be null.");
                assert.AreEqual(expected.Name, actual.Name, "The entity set names are different.");
            }
        }

        public static void VerifyServiceOperationsAreEqual(IEdmOperationImport expected, IEdmOperationImport actual, AssertionHandler assert)
        {
            if (expected == null)
            {
                assert.IsNull(actual, "The service operation should be null.");
            }
            else
            {
                assert.IsNotNull(actual, "The service operation should not be null.");
                assert.AreEqual(expected.Name, actual.Name, "The service operation names are different.");
            }
        }

        public static void VerifyQueryResultsAreEqual(object expected, object actual, AssertionHandler assert)
        {
            IEnumerable expectedEnumerable = expected as IEnumerable;
            IEnumerable actualEnumerable = actual as IEnumerable;

            if (expectedEnumerable == null && actualEnumerable != null)
            {
                assert.IsTrue(false, "Expected a single value result but the actual result is an enumerable.");
                return;
            }
            else if (expectedEnumerable != null && actualEnumerable == null)
            {
                assert.IsTrue(false, "Expected an enumerable result but the actual result is a single value.");
                return;
            }

            if (expectedEnumerable != null)
            {
                VerificationUtils.VerifyEnumerationsAreEqual(expectedEnumerable.Cast<object>(), actualEnumerable.Cast<object>(), assert);
            }
            else
            {
                assert.AreEqual(expected, actual, "Expected and actual single value result are different.");
            }
        }
    }
}
