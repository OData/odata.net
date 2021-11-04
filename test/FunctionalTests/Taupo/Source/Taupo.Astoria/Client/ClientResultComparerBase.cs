//---------------------------------------------------------------------
// <copyright file="ClientResultComparerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using CoreLinq = System.Linq;

    /// <summary>
    /// Base class for comparing results of Linq to Astoria query evaluation to a baseline. Also compares exceptions thrown during query evaluation (if any).
    /// </summary>
    public abstract class ClientResultComparerBase
    {
        private Dictionary<QueryStructuralValue, List<object>> visitedEntities;
        private List<Func<string>> logBuffer;

        /// <summary>
        /// Initializes a new instance of the ClientResultComparerBase class.
        /// </summary>
        protected ClientResultComparerBase()
        {
            this.Logger = Logger.Null;
            this.ShouldThrow = true;
        }

        /// <summary>
        /// Result of the comparison
        /// </summary>
        protected enum ComparisonResult
        {
            /// <summary>
            /// Comparison successful.
            /// </summary>
            Success,

            /// <summary>
            /// Comparison failed.
            /// </summary>
            Failure,

            /// <summary>
            /// Comparison skipped.
            /// </summary>
            Skipped,
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
                this.visitedEntities = new Dictionary<QueryStructuralValue, List<object>>();

                if (expectedValue.EvaluationError != null)
                {
                    this.CompareError(expectedValue.EvaluationError, queryEvaluationFunc);
                }
                else
                {
                    object actualValue = queryEvaluationFunc();

                    var queryable = actualValue as IQueryable;
                    if (queryable != null)
                    {
                        this.Logger.WriteLine(LogLevel.Verbose, "Executing query: {0}", queryable.Expression);
                    }

                    this.LinqResultComparerContextAdapter.AfterQueryExecuted(context);
                    
                    this.Compare(expectedValue, actualValue, "result", this.ShouldThrow);
                }
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
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
        protected virtual ComparisonResult Compare(QueryValue expected, object actual, string path, bool shouldThrow)
        {
            var comparisonVisitor = new TypeBasedValueComparisonVisitor(this, expected, actual, path, shouldThrow);

            return comparisonVisitor.GetComparisonResult();
        }

        /// <summary>
        /// Compares primitive object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
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
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
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
        /// Compares structural object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
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
                // if expected is null but actual is non-null this can be a result of the fact that entity was present in the ObjectStateManager and got attached even though
                // this particular query did not bring it back. 
                if (this.LinqResultComparerContextAdapter.IsObjectTrackedByContext(actual))
                {
                    this.AddToLogBuffer("Verifying structural value. Path: {0} SKIPPED (Entity in Object State Manager)", path);

                    return ComparisonResult.Skipped;
                }
                else
                {
                    this.ThrowOrLogError(shouldThrow, "Expecting null value in '{0}'. Actual: {1}[{2}].", path, actual, actual.GetType());

                    return ComparisonResult.Failure;
                }
            }

            List<object> visited;
            if (!this.visitedEntities.TryGetValue(expected, out visited))
            {
                visited = new List<object>();
                this.visitedEntities.Add(expected, visited);
            }

            if (visited.Contains(actual))
            {
                this.AddToLogBuffer("Verifying structural value. Path: {0} SKIPPED (Already visited)", path);

                return ComparisonResult.Skipped;
            }

            visited.Add(actual);

            var expectedType = expected.Type as IQueryClrType;

            if (expectedType != null)
            {
                ExceptionUtilities.Assert(expectedType.ClrType != null, "Expecting non-null CLR type.");

                if (!expectedType.ClrType.IsAssignableFrom(actual.GetType()))
                {
                    this.ThrowOrLogError(shouldThrow, "Types not match in {0}. Expecting: {1}. Actual: {2}.", path, expectedType.ClrType, actual.GetType());

                    return ComparisonResult.Failure;
                }
            }

            bool comparisonSkippedForAnyProperty = false;
            var properties = actual.GetType().GetProperties();

            IEnumerable<string> propertyNames = expected.MemberNames;

            foreach (string expectedName in propertyNames)
            {
                var actualProperty = properties.Where(p => p.Name == expectedName).SingleOrDefault();

                if (actualProperty == null)
                {
                    this.ThrowOrLogError(shouldThrow, "Expected property not found: {0} in '{1}'.", expectedName, path);

                    return ComparisonResult.Failure;
                }

                object actualValue = actualProperty.GetValue(actual, null);
                QueryType expectedPropertyType = expected.Type.Properties.Where(p => p.Name == expectedName).Single().PropertyType;

                string newPath = path + "." + expectedName;
                var comparisonResult = this.CompareProperty(expected, expectedName, expectedPropertyType, actualValue, newPath, shouldThrow);
                if (comparisonResult == ComparisonResult.Failure)
                {
                    visited.Remove(actual);
                    return ComparisonResult.Failure;
                }

                if (comparisonResult == ComparisonResult.Skipped)
                {
                    comparisonSkippedForAnyProperty = true;
                }
            }

            if (comparisonSkippedForAnyProperty)
            {
                this.AddToLogBuffer("Verifying structural value. Path: {0} SKIPPED", path);

                return ComparisonResult.Skipped;
            }
            else
            {
                this.AddToLogBuffer("Verifying structural value. Path: {0} SUCCESSFUL", path);

                return ComparisonResult.Success;
            }
        }

        /// <summary>
        /// Compares collection object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
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

            if (expected.Type.ElementType is QueryEntityType)
            {
                // for collection of entities, if we expect more elements than there is, collections are surely not equivalent
                // if there are more elements than expected however, it may mean that some of them are in the ObjectStateManager, 
                // so we need to accomodate for that
                if (expected.Elements.Count() > actualElements.Count)
                {
                    this.ThrowOrLogError(shouldThrow, "Collection element counts do not match in '{0}'. Expected: {1}. Actual: {2}.", path, expected.Elements.Count(), actualElements.Count);

                    return ComparisonResult.Failure;
                }
            }
            else
            {
                // for collection of non-entities there is no need to look up in the ObjectStateManager, if the counts don't match we can return 'Failed'
                if (expected.Elements.Count() != actualElements.Count)
                {
                    this.ThrowOrLogError(shouldThrow, "Collection element counts do not match in '{0}'. Expected: {1}. Actual: {2}.", path, expected.Elements.Count(), actualElements.Count);

                    return ComparisonResult.Failure;
                }
            }

            bool comparisonSkippedForAnyElement = false;
            if (expected.IsSorted)
            {
                if (expected.Elements.Count() != actualElements.Count)
                {
                    this.ThrowOrLogError(shouldThrow, "Collection element counts do not match in '{0}'. Expected: {1}. Actual: {2}.", path, expected.Elements.Count(), actualElements.Count);

                    return ComparisonResult.Failure;
                }

                // for sorted collection, compare until first error is found
                // we don't need to take OSM into account, as reference span is only applicable to collections which are not explicitly projected
                // in order to sort the collection, one must explicitly project it as well
                for (int i = 0; i < expected.Elements.Count(); i++)
                {
                    this.Compare(expected.Elements[i], actualElements[i], path + "[" + i + "]", true);
                }
            }
            else
            {
                ComparisonResult result = this.CompareCollections(expected, actualElements, path, shouldThrow, out comparisonSkippedForAnyElement);
                if (result == ComparisonResult.Failure)
                {
                    return ComparisonResult.Failure;
                }
            }

            if (comparisonSkippedForAnyElement)
            {
                this.AddToLogBuffer("Verifying collection value. Path: {0} SKIPPED", path);

                return ComparisonResult.Skipped;
            }
            else
            {
                this.AddToLogBuffer("Verifying collection value. Path: {0} SUCCESSFUL", path);

                return ComparisonResult.Success;
            }
        }

        /// <summary>
        /// Compares two collections
        /// </summary>
        /// <param name="expected">Expected CollectionValue</param>
        /// <param name="actualElements">actual collection</param>
        /// <param name="path">path being compared</param>
        /// <param name="shouldThrow">True or false to throw on an error or not</param>
        /// <param name="comparisonSkippedForAnyElement">determines if we should skip for a particular element or not</param>
        /// <returns>Returns a comparison result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Required due to maintain backward compat with previous implementations")]
        protected virtual ComparisonResult CompareCollections(QueryCollectionValue expected, IEnumerable<object> actualElements, string path, bool shouldThrow, out bool comparisonSkippedForAnyElement)
        {
            comparisonSkippedForAnyElement = false;

            List<object> actualElementsList = new List<object>(actualElements);

            for (int i = 0; i < expected.Elements.Count; i++)
            {
                var expectedElement = expected.Elements[i];

                bool matchFound = false;
                object foundElement = null;
                List<object> filteredActualElements = new List<object>(actualElements);
                // This was added for debugging purposes, on Silverlight it fails due to reflection calls so commenting it out
                filteredActualElements = this.FilterOutNonEqualActualElements(expectedElement, actualElements);
      
                // foreaching instead of LINQ for easier debugging
                foreach (var actualElement in filteredActualElements)
                {
                    var comparisonResult = this.Compare(expectedElement, actualElement, path + "[" + i + "]", false);
                    if (comparisonResult != ComparisonResult.Failure)
                    {
                        matchFound = true;
                        foundElement = actualElement;

                        // remove the element from the actual elements only if the result is 'Success', and leave it for 'Skipped'
                        if (comparisonResult == ComparisonResult.Success)
                        {
                            actualElementsList.Remove(foundElement);
                        }
                        else
                        {
                            comparisonSkippedForAnyElement = true;
                        }

                        break;
                    }
                }

                // if after comparison we still have actual elements left, and all these elements are in ObjectStateManager it means that 
                // we may have skipped verification for some elements and therefore should return 'Skipped'
                if (actualElementsList.Count > 0 && actualElementsList.All(ae => this.LinqResultComparerContextAdapter.IsObjectTrackedByContext(ae)))
                {
                    comparisonSkippedForAnyElement = true;
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
        /// <returns>Result of the comparison</returns>
        protected virtual ComparisonResult CompareProperty(QueryStructuralValue structuralValue, string expectedName, QueryType expectedPropertyType, object actualValue, string path, bool shouldThrow)
        {
            var expectedPropertyValue = structuralValue.GetValue(expectedName);
            return this.Compare(expectedPropertyValue, actualValue, path, shouldThrow);
        }

        /// <summary>
        /// Compares exception thrown by the evaluation with the expected error.
        /// </summary>
        /// <param name="expected">Expected error.</param>
        /// <param name="queryEvaluationFunc">Function to evaluate which should throw the exception.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "obj", Justification = "Need to have 'obj' for better debugging experience.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch everything here.")]
        protected void CompareError(QueryError expected, Func<object> queryEvaluationFunc)
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

            this.CompareException(expected, exception);
        }

        /// <summary>
        /// Compares the actual exception to expected error.
        /// </summary>
        /// <param name="expected">The expected error.</param>
        /// <param name="exception">The actual exception.</param>
        protected void CompareException(QueryError expected, Exception exception)
        {
            if (exception == null)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Expecting error: '{0}'. No exception was thrown.", expected.ToString()));
            }

            if (expected == null)
            {
                throw new TaupoInvalidOperationException("Got unexpected exception.", exception);
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Got expected error: " + expected.ToString());
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

        private List<object> FilterOutNonEqualActualElements(QueryValue expectedValue, IEnumerable<object> actualObjects)
        {
            var expectedStructuralValue = expectedValue as QueryStructuralValue;

            var filteredActualObjects = new List<object>(actualObjects);

            if (expectedStructuralValue != null)
            {
                var collectionProperties = expectedStructuralValue.Type.Properties.Where(p => p.PropertyType is QueryCollectionType);

                // Essentially we are filtering out actual items where the counts of inner collections are not equal.
                // We could extend this later for say string length and other properties, essentially a hash code of the value in 
                // types of properties. This makes the overall comparison a bit faster as it doesn't need to search over the whole collection
                // Also this really really helps with debugging, previously was a nightmare. Extend this as needed to make debugging
                // easier
                foreach (var collectionProperty in collectionProperties)
                {
                    var subSubFilteredElements = new List<object>();

                    var expectedCollectionValue = expectedStructuralValue.GetCollectionValue(collectionProperty.Name) as QueryCollectionValue;
                    foreach (var filteredActualObject in filteredActualObjects)
                    {
                        var filteredCollectionValue = filteredActualObject.GetType().GetProperty(collectionProperty.Name).GetValue(filteredActualObject, null);

                        var list = filteredCollectionValue as IList;

                        bool possibleMatch = false;
                        if (expectedCollectionValue.IsNull && filteredCollectionValue == null)
                        {
                            possibleMatch = true;
                        }

                        if (list != null && !expectedCollectionValue.IsNull && expectedCollectionValue.Elements.Count == list.Count)
                        {
                            possibleMatch = true;
                        }

                        if (possibleMatch)
                        {
                            subSubFilteredElements.Add(filteredActualObject);
                        }
                    }
                    
                    filteredActualObjects = subSubFilteredElements;

                    // If there is only one element found with a count in the collection then we can break out
                    if (filteredActualObjects.Count == 1)
                    {
                        break;
                    }
                }
            }
            
            return filteredActualObjects;
        }

        private ComparisonResult ProcessExpectedNullCollection(object actual, IEnumerable actualCollection, string path, bool shouldThrow)
        {
            ExceptionUtilities.Assert(actualCollection != null, "Expecting a collection.");

            // do not enumerate the collection twice, this may not be supported by the product
            List<object> actualElements = actualCollection.Cast<object>().ToList();
            if (actualElements.Any())
            {
                // if expected is null but actual is non-null this can be a result of the fact that entities in actual collection were present in the ObjectStateManager 
                // and got attached even though this particular query did not bring them back. 
                bool allEntitiesInObjectStateManager = true;
                foreach (var entity in actualElements)
                {
                    if (!this.LinqResultComparerContextAdapter.IsObjectTrackedByContext(entity))
                    {
                        allEntitiesInObjectStateManager = false;
                        break;
                    }
                }

                if (allEntitiesInObjectStateManager)
                {
                    this.AddToLogBuffer("Verifying collection value. Path: {0} SKIPPED (All elements in Object State Manager)", path);

                    return ComparisonResult.Skipped;
                }

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
            private ClientResultComparerBase parent;
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
            public TypeBasedValueComparisonVisitor(ClientResultComparerBase parent, QueryValue expectedValue, object actualValue, string path, bool shouldThrow)
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
                throw new TaupoNotSupportedException("Comparison of enums is not supported.");
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
                throw new TaupoNotSupportedException("Comparison of grouping value is not supported.");
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
                throw new TaupoNotSupportedException("Comparison of spatial types is not supported.");
            }

            /// <summary>
            /// Visits a <see cref="QueryRecordType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryRecordType type)
            {
                throw new TaupoNotSupportedException("Comparison of query record value is not supported.");
            }

            /// <summary>
            /// Visits a <see cref="QueryReferenceType"/>.
            /// </summary>
            /// <param name="type">Query type being visited.</param>
            /// <returns>The result of visiting this query type.</returns>
            public ComparisonResult Visit(QueryReferenceType type)
            {
                throw new TaupoNotSupportedException("Comparison of query reference value is not supported.");
            }
        }
    }
}