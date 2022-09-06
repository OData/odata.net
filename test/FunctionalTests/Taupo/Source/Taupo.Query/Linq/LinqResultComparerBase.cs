//---------------------------------------------------------------------
// <copyright file="LinqResultComparerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using CoreLinq = System.Linq;
    
    /// <summary>
    /// Base class for comparing results of Linq query evaluation to a baseline. Also compares exceptions thrown during query evaluation (if any).
    /// </summary>
    public abstract class LinqResultComparerBase
    {
        private Dictionary<QueryStructuralValue, object> visitedEntities;
        private List<Func<string>> logBuffer;

        /// <summary>
        /// Initializes a new instance of the LinqResultComparerBase class.
        /// </summary>
        protected LinqResultComparerBase()
        {
            this.Logger = Logger.Null;
            this.ShouldThrow = true;
        }

        /// <summary>
        /// Gets or sets LinqComparerContextAdapter
        /// </summary>
        /// <value>An Adapter to the context.</value>
        [InjectDependency(IsRequired = true)]
        public ILinqResultComparerContextAdapter LinqResultComparerContextAdapter { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency(IsRequired = true)]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an error should throw or not
        /// Variable only used for the initial value that is used with in the initial public this.Compare call, mostly a testability hook
        /// </summary>
        protected bool ShouldThrow { get; set; }

        /// <summary>
        /// Evaluates the specified function and compares the result against provided value.
        /// If the evaluation function throws, the exception is compared against
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="queryEvaluationFunc">Function which is evaluated to get the results of a query
        /// (or throw an exception which is then caught and verified by the comparer).</param>
        /// <param name="context">Context to be used when executing and comparing results of the query.</param>
        public void Compare(QueryValue expectedValue, Func<object> queryEvaluationFunc, object context)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedValue, "expectedValue");
            ExceptionUtilities.CheckArgumentNotNull(queryEvaluationFunc, "queryEvaluationFunc");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            this.LinqResultComparerContextAdapter.PrepareContext(context);

            try
            {
                this.logBuffer = new List<Func<string>>();
                this.visitedEntities = new Dictionary<QueryStructuralValue, object>();

                var expectedExceptions = (ExpectedExceptions)expectedValue.EvaluationError;
                var exceptionValid = this.CompareError(expectedExceptions, queryEvaluationFunc);
                if (exceptionValid == ComparisonResult.Success)
                {
                    return;
                }
                
                object actualValue = queryEvaluationFunc();

                var queryable = actualValue as IQueryable;
                if (queryable != null)
                {
                    this.Logger.WriteLine(LogLevel.Verbose, "Executing query: {0}", queryable.Expression);
                }

                this.LinqResultComparerContextAdapter.AfterQueryExecuted(context);

                this.Compare(expectedValue, actualValue, "result", this.ShouldThrow);
            }
            finally
            {
                this.LinqResultComparerContextAdapter.RestoreContext(context);
            }
        }

        /// <summary>
        /// Compares object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected virtual ComparisonResult Compare(QueryValue expected, object actual, string path, bool shouldThrow)
        {
            var comparisonVisitor = new TypeBasedValueComparisonVisitor(this, expected, actual, path, shouldThrow);

            return comparisonVisitor.GetComparisonResult();
        }

        /// <summary>
        /// Compares the object with the expected record value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected virtual ComparisonResult CompareRecord(QueryRecordValue expected, object actual, string path, bool shouldThrow)
        {
            throw new TaupoNotSupportedException("Comparison of query record value is not supported here.");
        }

        /// <summary>
        /// Compares the object with the expected reference value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected virtual ComparisonResult CompareReference(QueryReferenceValue expected, object actual, string path, bool shouldThrow)
        {
            throw new TaupoNotSupportedException("Comparison of query reference value is not supported here.");
        }

        /// <summary>
        /// Compares primitive object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected ComparisonResult ComparePrimitive(QueryScalarValue expected, object actual, string path, bool shouldThrow)
        {
            this.AddToLogBuffer("Verifying scalar value. Path: {0}", path);

            if (actual == null)
            {
                if (expected.Value == null)
                {
                    return ComparisonResult.Success;
                }

                this.ThrowOrLogError(shouldThrow, "Expecting non-null value: {0} in '{1}'. Got null value instead.", expected, path);

                return ComparisonResult.Failure;
            }

            if (expected.Value == null)
            {
                this.ThrowOrLogError(shouldThrow, "Expecting null value in '{0}'. Actual: {1}[{2}].", path, actual, actual.GetType());

                return ComparisonResult.Failure;
            }

            if (expected.Value.GetType().IsEnum() || actual.GetType().IsEnum())
            {
                if (expected.Value.GetType().IsEnum() && actual.GetType().IsEnum())
                {
                    return expected.Value.Equals(actual) ? ComparisonResult.Success : ComparisonResult.Failure;
                }
                else
                {
                    // If only one value is a clr enum then it is a failure because it either means there is an error in the evaluator or 
                    // the result materialization in the product. (Valid enum values should always be materialized as enum objects rather than 
                    // integer values)
                    return ComparisonResult.Failure;
                }
            }

            if (expected.Type.EvaluationStrategy.AreEqual(expected, expected.Type.CreateValue(actual)))
            {
                this.AddToLogBuffer("Verifying scalar value. Path: {0} SUCCESSFUL", path);

                return ComparisonResult.Success;
            }
            else
            {
                this.ThrowOrLogError(shouldThrow, "Scalar value mismatch in '{0}'. Expected: {1}. Actual: {2}[{3}].", path, expected, actual, actual.GetType());

                return ComparisonResult.Failure;
            }
        }

        /// <summary>
        /// Compares anonymous object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected ComparisonResult CompareAnonymous(QueryStructuralValue expected, object actual, string path, bool shouldThrow)
        {
            if (actual != null && !actual.GetType().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any())
            {
                this.ThrowOrLogError(shouldThrow, "Expecting anonymous type in '{0}'. Got: {1}.", path, actual.GetType());

                return ComparisonResult.Failure;
            }

            return this.CompareStructural(expected, actual, path, shouldThrow);
        }

        /// <summary>
        /// Compares grouping structure object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected ComparisonResult CompareGrouping(QueryStructuralValue expected, object actual, string path, bool shouldThrow)
        {
            var groupingInterface = actual.GetType().GetInterfaces().Where(i => i.IsGenericType() && i.GetGenericTypeDefinition() == typeof(IGrouping<,>)).SingleOrDefault();

            if (groupingInterface == null)
            {
                this.ThrowOrLogError(shouldThrow, "Expecting grouping in '{0}'. Actual: {1}[{2}].", path, actual, actual.GetType());

                return ComparisonResult.Failure;
            }

            Type keyType = groupingInterface.GetGenericArguments()[0];
            Type elementType = groupingInterface.GetGenericArguments()[1];

            var compareGroupingMethod = typeof(LinqResultComparerBase).GetMethods(false, false).Where(m => m.Name == "CompareGroupingItem" && m.IsGenericMethod).Single();
            compareGroupingMethod = compareGroupingMethod.MakeGenericMethod(keyType, elementType);

            Func<QueryStructuralValue, object, string, bool, ComparisonResult> func = (Func<QueryStructuralValue, object, string, bool, ComparisonResult>)compareGroupingMethod
                .CreateDelegate(typeof(Func<QueryStructuralValue, object, string, bool, ComparisonResult>), this);

            return func(expected, actual, path, shouldThrow);
        }

        /// <summary>
        /// Compares structural object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not really high.")]
        protected virtual ComparisonResult CompareStructural(QueryStructuralValue expected, object actual, string path, bool shouldThrow)
        {
            this.AddToLogBuffer("Verifying structural value. Path: {0}", path);

            if (actual == null)
            {
                if (expected.IsNull)
                {
                    return ComparisonResult.Success;
                }

                this.ThrowOrLogError(shouldThrow, "Expecting non-null value: {0} in '{1}. Got null instead.", expected, path);

                return ComparisonResult.Failure;
            }

            if (expected.IsNull)
            {
                this.ThrowOrLogError(shouldThrow, "Expecting null value in '{0}'. Actual: {1}[{2}].", path, actual, actual.GetType());

                return ComparisonResult.Failure;
            }

            if (expected.Type is QueryEntityType)
            {
                if (this.visitedEntities.ContainsKey(expected))
                {
                    if (object.ReferenceEquals(this.visitedEntities[expected], actual))
                    {
                        this.AddToLogBuffer("Comparing structural value. Path: {0}. Result - EQUAL (ALREADY VISITED)", path);

                        return ComparisonResult.Success;
                    }
                    else
                    {
                        this.ThrowOrLogError(shouldThrow, "Expecting different object in '{0}'. Expected: {1}. Actual: {2}.", path, this.visitedEntities[expected], actual);

                        return ComparisonResult.Failure;
                    }
                }
                else
                {
                    this.visitedEntities.Add(expected, actual);
                }
            }

            var expectedType = expected.Type as IQueryClrType;
            if (expectedType != null)
            {
                ExceptionUtilities.Assert(expectedType.ClrType != null, "Expecting non-null CLR type.");

                if (!expectedType.ClrType.IsAssignableFrom(actual.GetType()))
                {
                    this.visitedEntities.Remove(expected);
                    this.ThrowOrLogError(shouldThrow, "Types not match in {0}. Expecting: {1}. Actual: {2}.", path, expectedType.ClrType, actual.GetType());

                    return ComparisonResult.Failure;
                }
            }

            var properties = actual.GetType().GetProperties();
            IEnumerable<string> propertyNames = expected.MemberNames;

            foreach (string expectedName in propertyNames)
            {
                var actualProperty = properties.Where(p => p.Name == expectedName).SingleOrDefault();

                if (actualProperty == null)
                {
                    this.visitedEntities.Remove(expected);
                    this.ThrowOrLogError(shouldThrow, "Expected property not found: {0} in '{1}'.", expectedName, path);

                    return ComparisonResult.Failure;
                }

                object actualValue = actualProperty.GetValue(actual, null);
                QueryType expectedPropertyType = expected.Type.Properties.Where(p => p.Name == expectedName).Single().PropertyType;

                string newPath = path + "." + expectedName;
                var propertyComparisonResult = this.CompareProperty(expected, expectedName, expectedPropertyType, actualValue, newPath, shouldThrow);
                if (propertyComparisonResult == ComparisonResult.Failure)
                {
                    this.visitedEntities.Remove(expected);

                    return ComparisonResult.Failure;
                }
            }

            this.AddToLogBuffer("Verifying structural value. Path: {0} SUCCESSFUL", path);

            return ComparisonResult.Success;
        }

        /// <summary>
        /// Compares collection object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected ComparisonResult CompareCollection(QueryCollectionValue expected, object actual, string path, bool shouldThrow)
        {
            this.AddToLogBuffer("Verifying collection value. Path: {0}", path);

            if (actual == null)
            {
                if (expected.IsNull)
                {
                    return ComparisonResult.Success;
                }

                this.ThrowOrLogError(shouldThrow, "Expecting non-null value: {0} in '{1}'. Got null instead.", expected, path);

                return ComparisonResult.Failure;
            }

            IEnumerable actualCollection = actual as IEnumerable;

            if (expected.IsNull)
            {
                return this.ProcessExpectedNullCollection(actual, actualCollection, path, shouldThrow);
            }

            if (actualCollection == null)
            {
                this.ThrowOrLogError(shouldThrow, "Expecting a collection in '{0}'.", path);

                return ComparisonResult.Failure;
            }

            List<object> actualElements = actualCollection.Cast<object>().ToList();
            
            if (expected.Elements.Count() != actualElements.Count)
            {
                this.ThrowOrLogError(shouldThrow, "Collection element counts do not match in '{0}'. Expected: {1}. Actual: {2}.", path, expected.Elements.Count(), actualElements.Count);

                return ComparisonResult.Failure;
            }
            
            if (expected.IsSorted)
            {
                // for sorted collection, compare until first error is found
                for (int i = 0; i < expected.Elements.Count(); i++)
                {
                    this.Compare(expected.Elements[i], actualElements[i], path + "[" + i + "]", true);
                }
            }
            else
            {
                var result = this.CompareCollections(expected, actualElements, path, shouldThrow);
                if (result == ComparisonResult.Failure)
                {
                    return ComparisonResult.Failure;
                }
            }

            this.AddToLogBuffer("Verifying collection value. Path: {0} SUCCESSFUL", path);

            return ComparisonResult.Success;
        }

        /// <summary>
        /// Compares two collections
        /// </summary>
        /// <param name="expected">Expected CollectionValue</param>
        /// <param name="actualElements">actual collection</param>
        /// <param name="path">path being compared</param>
        /// <param name="shouldThrow">True or false to throw on an error or not</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Required due to maintain backward compat with previous implementations")]
        protected virtual ComparisonResult CompareCollections(QueryCollectionValue expected, IEnumerable<object> actualElements, string path, bool shouldThrow)
        {
            var actualElementsList = new List<object>(actualElements);

            for (int i = 0; i < expected.Elements.Count; i++)
            {
                var expectedElement = expected.Elements[i];

                bool matchFound = false;
                object foundElement = null;

                // foreaching instead of LINQ for easier debugging
                foreach (var actualElement in actualElementsList)
                {
                    var comparisonResult = this.Compare(expectedElement, actualElement, path + "[" + i + "]", false);
                    if (comparisonResult == ComparisonResult.Success)
                    {
                        matchFound = true;
                        foundElement = actualElement;
                        actualElementsList.Remove(foundElement);

                        break;
                    }
                }

                if (!matchFound)
                {
                    this.ThrowOrLogError(shouldThrow, "Match not found: {0} in {1}.", expectedElement.ToString(), path);

                    return ComparisonResult.Failure;
                }
            }

            return ComparisonResult.Success;
        }

        /// <summary>
        /// Compares a property of a structural object with expected value
        /// </summary>
        /// <param name="structuralValue">The structural object containing the property to compare</param>
        /// <param name="expectedName">The name of the property to compare</param>
        /// <param name="expectedPropertyType">The expected type of the property</param>
        /// <param name="actualValue">The actual value of the property</param>
        /// <param name="path">The path to the compared object (for debugging purposes)</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered</param>
        /// <returns>Result of the comparison, Success or Failure.</returns>
        protected virtual ComparisonResult CompareProperty(QueryStructuralValue structuralValue, string expectedName, QueryType expectedPropertyType, object actualValue, string path, bool shouldThrow)
        {
            var expectedPropertyValue = structuralValue.GetValue(expectedName);
            return this.Compare(expectedPropertyValue, actualValue, path, shouldThrow);
        }

        /// <summary>
        /// Compares exception thrown by the evaluation with the expected error.
        /// </summary>
        /// <param name="expectedExceptions">Expected exceptions.</param>
        /// <param name="queryEvaluationFunc">Function to evaluate which should throw the exception.</param>
        /// <returns>Whether the expected exception matches the actual exception, or if the comparison was skipped because no exceptions were expected or thrown.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "obj", Justification = "Need to have 'obj' for better debugging experience.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch everything here.")]
        protected ComparisonResult CompareError(ExpectedExceptions expectedExceptions, Func<object> queryEvaluationFunc)
        {
            Exception exception = null;
            try
            {
                object result = queryEvaluationFunc();
                var enumerable = result as IEnumerable;
                if (enumerable != null)
                {
                    foreach (var obj in enumerable)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return ExpectedExceptionsComparer.CompareException(expectedExceptions, exception);
        }

        /// <summary>
        /// Throw an exception or log an error message
        /// </summary>
        /// <param name="shouldThrow">Whether an exception should be thrown</param>
        /// <param name="errorMessage">The error message format string</param>
        /// <param name="parameters">The parameters for the error message format string</param>
        protected void ThrowOrLogError(bool shouldThrow, string errorMessage, params object[] parameters)
        {
            if (shouldThrow)
            {
                this.DisplayLog();
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, errorMessage, parameters));
            }
            else
            {
                this.AddToLogBuffer("ERROR: " + errorMessage, parameters);
            }
        }

        /// <summary>
        /// Add message to log buffer
        /// </summary>
        /// <param name="message">The string message.</param>
        /// <param name="arguments">The arguments in the message.</param>
        protected void AddToLogBuffer(string message, params object[] arguments)
        {
            this.logBuffer.Add(() => string.Format(CultureInfo.InvariantCulture, message, arguments));
        }

        /// <summary>
        /// Writes entries from the buffer to the log
        /// </summary>
        protected void DisplayLog()
        {
            foreach (var logEntry in this.logBuffer)
            {
                this.Logger.WriteLine(LogLevel.Verbose, logEntry());
            }
        }

        private ComparisonResult CompareGroupingItem<TKey, TElement>(QueryStructuralValue expected, object actual, string path, bool shouldThrow)
        {
            var groupingInterface = (IGrouping<TKey, TElement>)actual;

            QueryValue expectedKey = expected.GetValue("Key");
            var keysMatch = this.Compare(expectedKey, groupingInterface.Key, path, false);

            if (keysMatch == ComparisonResult.Success)
            {
                var expectedElements = (QueryCollectionValue)expected.GetValue("Elements");

                return this.Compare(expectedElements, groupingInterface.Select(e => e), path, shouldThrow);
            }
            else
            {
                this.ThrowOrLogError(shouldThrow, "Grouping keys don't match in '{0}'. Expected: {1}. Actual: {2}.", path, expectedKey, groupingInterface.Key);

                return ComparisonResult.Failure;
            }
        }

        private ComparisonResult ProcessExpectedNullCollection(object actual, IEnumerable actualCollection, string path, bool shouldThrow)
        {
            ExceptionUtilities.Assert(actualCollection != null, "Expecting a collection.");

            // do not enumerate the collection twice, this may not be supported by the product
            List<object> actualElements = actualCollection.Cast<object>().ToList();
            if (actualElements.Any())
            {
                this.ThrowOrLogError(shouldThrow, "Expecting null value in '{0}'. Actual: {1}[{2}].", path, actual, actual.GetType());

                return ComparisonResult.Failure;
            }
            else
            {
                return ComparisonResult.Success;
            }
        }

        /// <summary>
        /// A helper visitor to compare value based on the type
        /// </summary>
        private class TypeBasedValueComparisonVisitor : IQueryTypeVisitor<ComparisonResult>
        {
            private LinqResultComparerBase parent;
            private QueryValue expectedValue;
            private object actualValue;
            private string path;
            private bool shouldThrow;

            /// <summary>
            /// Initializes a new instance of the TypeBasedValueComparisonVisitor class.
            /// </summary>
            /// <param name="parent">The parent result comparer, who defines the actual comparison logics.</param>
            /// <param name="expectedValue">The expected query value.</param>
            /// <param name="actualValue">The actual value.</param>
            /// <param name="path">The path to the value (for debugging purposes)</param>
            /// <param name="shouldThrow">Should exception be thrown if error is encountered</param>
            public TypeBasedValueComparisonVisitor(LinqResultComparerBase parent, QueryValue expectedValue, object actualValue, string path, bool shouldThrow)
            {
                this.parent = parent;
                this.expectedValue = expectedValue;
                this.actualValue = actualValue;
                this.path = path;
                this.shouldThrow = shouldThrow;
            }

            /// <summary>
            /// Compares the expected value and actual value, based on the type of the expected value.
            /// </summary>
            /// <returns>Result of the comparison</returns>
            /// <remarks>
            /// This should be the only entry point for this visitor class. You should never call the Visit() methods from the outside.
            /// </remarks>
            public ComparisonResult GetComparisonResult()
            {
                QueryType resultType = this.expectedValue.Type;
                return resultType.Accept(this);
            }

            /// <summary>
            /// Visits a <see cref="QueryAnonymousStructuralType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryAnonymousStructuralType type)
            {
                return this.parent.CompareAnonymous((QueryStructuralValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryClrEnumType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryClrEnumType type)
            {
                return this.parent.ComparePrimitive((QueryScalarValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryClrPrimitiveType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryClrPrimitiveType type)
            {
                return this.parent.ComparePrimitive((QueryScalarValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryCollectionType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryCollectionType type)
            {
                return this.parent.CompareCollection((QueryCollectionValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryComplexType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryComplexType type)
            {
                return this.parent.CompareStructural((QueryStructuralValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryEntityType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryEntityType type)
            {
                return this.parent.CompareStructural((QueryStructuralValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryGroupingType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryGroupingType type)
            {
                return this.parent.CompareGrouping((QueryStructuralValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryMappedScalarType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryMappedScalarType type)
            {
                return this.parent.ComparePrimitive((QueryScalarValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryMappedScalarTypeWithStructure"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryMappedScalarTypeWithStructure type)
            {
                return this.parent.ComparePrimitive((QueryScalarValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryRecordType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryRecordType type)
            {
                return this.parent.CompareRecord((QueryRecordValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }

            /// <summary>
            /// Visits a <see cref="QueryReferenceType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryReferenceType type)
            {
                return this.parent.CompareReference((QueryReferenceValue)this.expectedValue, this.actualValue, this.path, this.shouldThrow);
            }
        }
    }
}
