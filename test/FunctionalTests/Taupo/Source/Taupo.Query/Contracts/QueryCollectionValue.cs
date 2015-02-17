//---------------------------------------------------------------------
// <copyright file="QueryCollectionValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;

    /// <summary>
    /// Represents <see cref="QueryValue"/> which is a collection.
    /// </summary>
    [DebuggerDisplay("Collection. IsNull={IsNull} Elements={Elements} Type={Type}")]
    public class QueryCollectionValue : QueryValue
    {
        /// <summary>
        /// Initializes a new instance of the QueryCollectionValue class.
        /// </summary>
        /// <param name="type">The collection type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="elements">The elements.</param>
        public QueryCollectionValue(QueryCollectionType type, IQueryEvaluationStrategy evaluationStrategy, QueryError evaluationError, IEnumerable<QueryValue> elements)
            : this(type, evaluationStrategy, evaluationError, elements, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the QueryCollectionValue class.
        /// </summary>
        /// <param name="type">The collection type.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="isSorted">Determines whether the collection is sorted.</param>
        public QueryCollectionValue(QueryCollectionType type, IQueryEvaluationStrategy evaluationStrategy, QueryError evaluationError, IEnumerable<QueryValue> elements, bool isSorted)
            : base(evaluationError, evaluationStrategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            this.IsSorted = isSorted;
            this.Type = type;
            if (elements == null)
            {
                this.Elements = null;
            }
            else
            {
                this.Elements = elements.ToList();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is sorted.
        /// </summary>
        public bool IsSorted { get; private set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Must be the same as the base class.")]
        public new QueryCollectionType Type { get; private set; }

        /// <summary>
        /// Gets the elements of the collection.
        /// </summary>
        public IList<QueryValue> Elements { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        public override bool IsNull
        {
            get { return this.Elements == null; }
        }

        /// <summary>
        /// Creates the collection value with the specified elements.
        /// </summary>
        /// <param name="elementType">The type of each collection element.</param>
        /// <param name="elements">The elements.</param>
        /// <returns>
        /// Strongly-typed collection with the specified values.
        /// </returns>
        public static QueryCollectionValue Create(QueryType elementType, params QueryValue[] elements)
        {
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");

            return new QueryCollectionValue(
                elementType.CreateCollectionType(),
                elementType.EvaluationStrategy,
                QueryError.GetErrorFromValues(elements),
                elements);
        }

        /// <summary>
        /// Creates the collection value with the specified elements.
        /// </summary>
        /// <param name="elementType">The type of each collection element.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="isSorted">Indicating if the collection is sorted.</param>
        /// <returns>Strongly-typed collection with the specified values.</returns>
        public static QueryCollectionValue Create(QueryType elementType, IEnumerable<QueryValue> elements, bool isSorted)
        {
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");

            return new QueryCollectionValue(
                elementType.CreateCollectionType(),
                elementType.EvaluationStrategy,
                QueryError.GetErrorFromValues(elements),
                elements,
                isSorted);
        }

        /// <summary>
        /// Verify that the results are ordered accoring to the specified ordering.
        /// </summary>
        /// <param name="ordering">The ordering to verify by.</param>
        /// <param name="elements">The elements on which to verify the ordering.</param>
        /// <returns>A value indicating whether the collection is properly sorted.</returns>
        public static bool IsOrdered(QueryOrdering ordering, IEnumerable<object> elements)
        {
            ExceptionUtilities.CheckArgumentNotNull(ordering, "ordering");

            QueryScalarValue[][] sortedSelectorsTable = BuildSortedSelectorsTable(ordering, elements);
            int totalRows = sortedSelectorsTable.GetLength(0);

            for (int row = 0; row < totalRows - 1; row++)
            {
                if (!RowsAreOrdered(sortedSelectorsTable[row], sortedSelectorsTable[row + 1], ordering))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether all elements in a source collection
        /// match a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether all elements in a source collection match
        /// a given predicate.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue All(Func<QueryValue, QueryScalarValue> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            return this.EvaluationStrategy.All(this.Elements, predicate);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether there is any element in a source collection.
        /// </summary>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether there is any element in a source collection.
        /// </returns>
        public QueryScalarValue Any()
        {
            return this.EvaluationStrategy.Any(this.Elements);
        }

        /// <summary>
        /// Gets a <see cref="QueryScalarValue"/> indicating whether any element in the collection
        /// matches a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Instance of <see cref="QueryScalarValue"/> indicating whether any element in the collection matches
        /// a given predicate.
        /// </returns>
        /// <remarks>The value can be store-dependent w.r.t. handling of NULL values of the input collection
        /// and/or result of the predicate.</remarks>
        public QueryScalarValue Any(Func<QueryValue, QueryScalarValue> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            return this.EvaluationStrategy.Any(this.Elements, predicate);
        }

        /// <summary>
        /// Gets the count of the collection items.
        /// </summary>
        /// <returns>Count of the collection.</returns>
        public QueryScalarValue Count()
        {
            return this.Count(null);
        }

        /// <summary>
        /// Gets the count of the collection items which match a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Count of the collection.</returns>
        public QueryScalarValue Count(Func<QueryValue, QueryScalarValue> predicate)
        {
            if (this.IsNull)
            {
                return this.EvaluationStrategy.IntegerType.NullValue;
            }

            IEnumerable<QueryValue> elementsToCount = this.Elements;
            if (predicate != null)
            {
                elementsToCount = this.Where(predicate).Elements;
            }

            if (this.Type.ElementType is QueryScalarType)
            {
                return this.EvaluationStrategy.Count(elementsToCount.Cast<QueryScalarValue>());
            }

            return this.EvaluationStrategy.IntegerType.CreateValue(elementsToCount.Count());
        }

        /// <summary>
        /// Gets distinct elements from the collection.
        /// </summary>
        /// <returns>Distinct elements of the collection.</returns>
        public QueryCollectionValue Distinct()
        {
            List<QueryValue> distinctValues = new List<QueryValue>();

            foreach (var element in this.Elements)
            {
                if (!distinctValues.Any(v => this.ValuesAreEqual(v, element)))
                {
                    distinctValues.Add(element);
                }
            }

            return new QueryCollectionValue(this.Type, this.EvaluationStrategy, QueryError.GetErrorFromValues(distinctValues), distinctValues);
        }

        /// <summary>
        /// Combines elements from both collections into one without eliminating duplicates.
        /// </summary>
        /// <param name="collection">The input collection for the union all operation</param>
        /// <returns>Combined elements from both collections.</returns>
        public QueryCollectionValue UnionAll(QueryCollectionValue collection)
        {
            List<QueryValue> resultValues = new List<QueryValue>();
            resultValues.AddRange(this.Elements);
            resultValues.AddRange(collection.Elements);
            return new QueryCollectionValue(this.Type, this.EvaluationStrategy, QueryError.GetErrorFromValues(resultValues), resultValues);
        }

        /// <summary>
        /// Combines elements from both collections into one and eliminates duplicates.
        /// </summary>
        /// <param name="collection">The input collection for the union operation</param>
        /// <returns>Combined elements from both collections.</returns>
        public QueryCollectionValue Union(QueryCollectionValue collection)
        {
            return this.UnionAll(collection).Distinct();
        }

        /// <summary>
        /// Returns common elements from both input collections.
        /// </summary>
        /// <param name="collection">The input collection for the intersect operation</param>
        /// <returns>Common elements from both collections.</returns>
        public QueryCollectionValue Intersect(QueryCollectionValue collection)
        {
            List<QueryValue> resultValues = new List<QueryValue>();
            foreach (QueryValue element in collection.Elements)
            {
                if (this.Elements.Any(v => this.ValuesAreEqual(v, element)))
                {
                    resultValues.Add(element);
                }
            }

            // intersect removes duplicates from the result, so applying distinct at the end
            var intersectedCollection = new QueryCollectionValue(this.Type, this.EvaluationStrategy, QueryError.GetErrorFromValues(resultValues), resultValues);
            var result = intersectedCollection.Distinct();

            return result;
        }

        /// <summary>
        /// Checks if both input collections have common elements
        /// </summary>
        /// <param name="collection">The input collection for the overlaps operation</param>
        /// <returns>A true if both collections have common elements</returns>
        public QueryScalarValue Overlaps(QueryCollectionValue collection)
        {
            IEnumerable<QueryValue> intersectingElements = this.Intersect(collection).Elements;
            if (intersectingElements.Count() > 0)
            {
                return this.EvaluationStrategy.BooleanType.CreateValue(true);
            }

            return this.EvaluationStrategy.BooleanType.CreateValue(false);
        }

        /// <summary>
        /// Checks if the collection is empty
        /// </summary>
        /// <returns>A true if the collection has elements</returns>
        public QueryScalarValue Exists()
        {
            if ((int)this.Count().Value > 0)
            {
                return this.EvaluationStrategy.BooleanType.CreateValue(true);
            }

            return this.EvaluationStrategy.BooleanType.CreateValue(false);
        }

        /// <summary>
        /// Checks if the element exists in the collection
        /// </summary>
        /// <param name="element">The input collection for the contains operation</param>
        /// <returns>A true if the collection contains the input element</returns>
        public QueryScalarValue HasElement(QueryValue element)
        {
            if (this.Elements.Any(v => this.ValuesAreEqual(v, element)))
            {
                return this.EvaluationStrategy.BooleanType.CreateValue(true);
            }

            return this.EvaluationStrategy.BooleanType.CreateValue(false);
        }

        /// <summary>
        /// Returns the first collection without the common elements from both input collections.
        /// </summary>
        /// <param name="collection">The input collection for the except operation</param>
        /// <returns>The firt collection with commmon elements removed.</returns>
        public QueryCollectionValue Except(QueryCollectionValue collection)
        {
            var resultValues = new List<QueryValue>();
            foreach (QueryValue element in this.Elements)
            {
                if (!collection.Elements.Any(v => this.ValuesAreEqual(v, element)))
                {
                    resultValues.Add(element);
                }
            }

            // except removes duplicates from the result, so applying distinct at the end
            var exceptResult = new QueryCollectionValue(this.Type, this.EvaluationStrategy, QueryError.GetErrorFromValues(resultValues), resultValues);
            var result = exceptResult.Distinct();

            return result;
        }

        /// <summary>
        /// Returns the first collection concatenanted with the second collection.
        /// </summary>
        /// <param name="collection">The collection to concatenate with.</param>
        /// <returns>The concatenated result.</returns>
        public QueryCollectionValue Concat(QueryCollectionValue collection)
        {
            var resultValues = this.Elements.Concat(collection.Elements);

            return new QueryCollectionValue(this.Type, this.EvaluationStrategy, QueryError.GetErrorFromValues(resultValues), resultValues);
        }

        /// <summary>
        /// Gets the first element from the collection or returns an error value if the collection is null
        /// or empty.
        /// </summary>
        /// <returns>First element of the collection.</returns>
        public QueryValue First()
        {
            if (this.IsNull || this.Elements.Count == 0)
            {
                return this.Type.ElementType.CreateErrorValue(new QueryError("Empty sequence."));
            }

            return this.Elements.First();
        }

        /// <summary>
        /// Gets the only element from the collection or null if it is empty.
        /// If multiple values are found, then an error value is returned.
        /// </summary>
        /// <returns>Single element of the collection or null.</returns>
        public QueryValue SingleOrDefault()
        {
            if (this.IsNull || this.Elements.Count == 0)
            {
                return this.Type.ElementType.NullValue;
            }

            if (this.Elements.Count > 1)
            {
                return this.Type.ElementType.CreateErrorValue(new QueryError("Multiple elements in sequence."));
            }

            return this.Elements.Single();
        }

        /// <summary>
        /// Groups elements using key computed from the key selector.
        /// </summary>
        /// <param name="keySelector">Key selector lambda.</param>
        /// <returns>Elements grouped based on the provided key.</returns>
        public QueryCollectionValue GroupBy(Func<QueryValue, QueryValue> keySelector)
        {
            if (this.IsNull)
            {
                return new QueryCollectionValue(this.Type.ElementType.CreateCollectionType(), this.EvaluationStrategy, this.EvaluationError, null);
            }

            var keys = this.Elements.Select(keySelector).ToList();
            var keyCollection = new QueryCollectionValue(this.Type.ElementType.CreateCollectionType(), this.EvaluationStrategy, this.EvaluationError, keys);
            var distinctKeys = keyCollection.Distinct().Elements;
            var elementType = this.Type.ElementType;

            var groupings = distinctKeys.Select(key =>
            {
                if (key.IsNull)
                {
                    var matchingNullElements = this.Elements.Where(element => keySelector(element).IsNull);
                    return this.CreateGroupingValue(key, QueryCollectionValue.Create(elementType, matchingNullElements.ToArray()));
                }
                else
                {
                    return this.CreateGroupingValue(key, QueryCollectionValue.Create(elementType, this.Elements.Where(element => this.ValuesAreEqual(keySelector(element), key)).ToArray()));
                }
            });

            var keyType = keySelector(elementType.NullValue).Type;
            var groupingType = new QueryGroupingType(keyType, elementType, this.EvaluationStrategy);

            return new QueryCollectionValue(groupingType.CreateCollectionType(), this.EvaluationStrategy, QueryError.GetErrorFromValues(groupings), groupings.Cast<QueryValue>());
        }

        /// <summary>
        /// Gets the count of the collection items.
        /// </summary>
        /// <returns>Count of the collection.</returns>
        public QueryScalarValue LongCount()
        {
            return this.LongCount(null);
        }

        /// <summary>
        /// Gets the count of the collection items which match a given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Count of the collection.</returns>
        public QueryScalarValue LongCount(Func<QueryValue, QueryScalarValue> predicate)
        {
            if (this.IsNull)
            {
                return this.EvaluationStrategy.IntegerType.NullValue;
            }

            IEnumerable<QueryValue> elementsToCount = this.Elements;
            if (predicate != null)
            {
                elementsToCount = this.Where(predicate).Elements;
            }

            if (this.Type.ElementType is QueryScalarType)
            {
                return this.EvaluationStrategy.LongCount(elementsToCount.Cast<QueryScalarValue>());
            }

            return this.EvaluationStrategy.LongIntegerType.CreateValue(elementsToCount.LongCount());
        }

        /// <summary>
        /// Get the maximum value of the scalar value collection.
        /// </summary>
        /// <returns>The maximum value</returns>
        public QueryScalarValue Max()
        {
            return this.Max(e => (QueryScalarValue)e);
        }

        /// <summary>
        /// Gets the item with the largest selected property value in the collection.
        /// </summary>
        /// <param name="selector">The predicate which selects the property value on which to perform comparisons</param>
        /// <returns>The maximum item</returns>
        public QueryScalarValue Max(Func<QueryValue, QueryScalarValue> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            if (this.IsNull)
            {
                return selector(this.Type.ElementType.NullValue);
            }

            if (this.Elements.Select(selector).Any(v => !v.IsNull))
            {
                var scalarType = selector(this.Type.ElementType.DefaultValue).Type;
                var projectedCollection = scalarType.CreateCollectionType().CreateCollectionWithValues(this.Elements.Select(selector).Cast<QueryValue>());
                return this.EvaluationStrategy.Max(projectedCollection).MaterializeValueIfEnum(scalarType);
            }
            else
            {
                return selector(this.Type.ElementType.DefaultValue).Type.NullValue;
            }
        }

        /// <summary>
        /// Get the minimum value of the scalar value collection.
        /// </summary>
        /// <returns>The minimum value</returns>
        public QueryScalarValue Min()
        {
            return this.Min(e => (QueryScalarValue)e);
        }

        /// <summary>
        /// Gets the item with the smallest selected property value in the collection.
        /// </summary>
        /// <param name="selector">The predicate which selects the property value on which to perform comparisons</param>
        /// <returns>The minimum item</returns>
        public QueryScalarValue Min(Func<QueryValue, QueryScalarValue> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            if (this.IsNull)
            {
                return selector(this.Type.ElementType.NullValue);
            }

            if (this.Elements.Select(selector).Any(v => !v.IsNull))
            {
                var scalarType = selector(this.Type.ElementType.DefaultValue).Type;
                var projectedCollection = scalarType.CreateCollectionType().CreateCollectionWithValues(this.Elements.Select(selector).Cast<QueryValue>());
                return this.EvaluationStrategy.Min(projectedCollection).MaterializeValueIfEnum(scalarType);
            }
            else
            {
                return selector(this.Type.ElementType.DefaultValue).Type.NullValue;
            }
        }

        /// <summary>
        /// Orders the collection by applying given ordering.
        /// </summary>
        /// <param name="ordering">The ordering to use.</param>
        /// <returns>Ordered collection.</returns>
        /// <remarks>This overload allows for more intellisense-friendly way of specifying ordering.</remarks>
        public QueryCollectionValue OrderBy(QueryOrdering ordering)
        {
            ExceptionUtilities.CheckArgumentNotNull(ordering, "ordering");
            ExceptionUtilities.CheckCollectionNotEmpty(ordering.Selectors, "ordering.Selectors");
            if (this.IsNull)
            {
                return this;
            }

            IEnumerable<QueryValue> orderedElements = ordering.Apply(this.Elements.Cast<object>()).Cast<QueryValue>();
            QueryError currentError = QueryError.Combine(this.EvaluationError, QueryError.GetErrorFromValues(orderedElements));
            if (currentError != null)
            {
                return (QueryCollectionValue)this.Type.CreateErrorValue(currentError);
            }

            // if this collection contains duplicate order by key selectors
            if (HasNondeterministicOrdering(ordering, orderedElements))
            {
                return new QueryCollectionValue(this.Type, this.EvaluationStrategy, null, orderedElements, false);
            }
            else
            {
                return new QueryCollectionValue(this.Type, this.EvaluationStrategy, null, orderedElements, true);
            }
        }

        /// <summary>
        /// Orders the collection by applying given ordering.
        /// </summary>
        /// <param name="orderingFunc">The function which builds an ordering to use for sorting elements of the collection.</param>
        /// <returns>Ordered collection.</returns>
        /// <remarks>This overload allows for more intellisense-friendly way of specifying ordering.</remarks>
        public QueryCollectionValue OrderBy(Func<QueryOrdering, QueryOrdering> orderingFunc)
        {
            ExceptionUtilities.CheckArgumentNotNull(orderingFunc, "orderingFunc");

            var ordering = orderingFunc(new QueryOrdering());

            return this.OrderBy(ordering);
        }

        /// <summary>
        /// Applies a projection operator which selects a value from each collection element.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <returns>
        /// Collection of values containing results of the selector applied to all elements of the collection.
        /// </returns>
        public QueryCollectionValue Select(Func<QueryValue, QueryValue> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            IEnumerable<QueryValue> result;
            QueryCollectionType resultType;

            this.ComputeSelectResult(selector, out result, out resultType);

            return new QueryCollectionValue(resultType, this.EvaluationStrategy, QueryError.GetErrorFromValues(result), result, this.IsSorted);
        }

        /// <summary>
        /// Applies a projection operator which selects a value from each collection element.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="resultType">Type of the result</param>
        /// <returns>
        /// Collection of values containing results of the selector applied to all elements of the collection.
        /// </returns>
        public QueryCollectionValue Select(Func<QueryValue, QueryValue> selector, QueryCollectionType resultType)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");
            IEnumerable<QueryValue> result = null;
            if (!this.IsNull)
            {
                result = this.Elements.Cast<QueryValue>().Select(e => selector(e)).ToList();
            }

            return new QueryCollectionValue(resultType, this.EvaluationStrategy, QueryError.GetErrorFromValues(result), result, this.IsSorted);
        }

        /// <summary>
        /// Applies the Skip operation on the given collection.
        /// </summary>
        /// <param name="skipCount">How many elements to skip.</param>
        /// <returns>Collection with the applied Skip operation.</returns>
        public QueryCollectionValue Skip(QueryScalarValue skipCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(skipCount, "skipCount");

            QueryError currentError = QueryError.Combine(this.EvaluationError, skipCount.EvaluationError);
            if (currentError != null)
            {
                return (QueryCollectionValue)this.Type.CreateErrorValue(currentError);
            }

            IEnumerable<QueryValue> result = null;
            if (!this.IsNull)
            {
                // perhaps this must be changed - moved to EvaluationStrategy
                result = this.Elements.Skip((int)skipCount.Value);
            }

            return new QueryCollectionValue(this.Type, this.Type.EvaluationStrategy, QueryError.GetErrorFromValues(result), result, this.IsSorted);
        }

        /// <summary>
        /// Applies the Take operation on the given collection.
        /// </summary>
        /// <param name="takeCount">How many elements to take.</param>
        /// <returns>Collection with the applied Take operation.</returns>
        public QueryCollectionValue Take(QueryScalarValue takeCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(takeCount, "takeCount");

            QueryError currentError = QueryError.Combine(this.EvaluationError, takeCount.EvaluationError);
            if (currentError != null)
            {
                return (QueryCollectionValue)this.Type.CreateErrorValue(currentError);
            }

            IEnumerable<QueryValue> result = null;
            if (!this.IsNull)
            {
                result = this.Elements.Take((int)takeCount.Value).ToList();
            }

            return new QueryCollectionValue(this.Type, this.Type.EvaluationStrategy, QueryError.GetErrorFromValues(result), result, this.IsSorted);
        }

        /// <summary>
        /// Filters the collection using given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// Filtered collection (including elements where the predicate matches).
        /// </returns>
        public QueryCollectionValue Where(Func<QueryValue, QueryScalarValue> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");
            return this.EvaluationStrategy.Filter(this, predicate);
        }

        /// <summary>
        /// Determines if the current value is of a given type or an equivalent type in the type hierarchy
        /// </summary>
        /// <param name="resultType">The type to compare against</param>
        /// <param name="performExactMatch">A flag that determines if an exact match needs to be performed</param>
        /// <returns>The elements from the collection, that match the input type or an equivalent type</returns>
        public QueryValue OfType(QueryStructuralType resultType, bool performExactMatch)
        {
            QueryCollectionValue result = resultType.CreateCollectionType().CreateCollectionWithValues(Enumerable.Empty<QueryValue>());

            if (!this.IsNull)
            {
                foreach (QueryStructuralValue element in this.Elements)
                {
                    bool isOfEquivalentType = (bool)((QueryScalarValue)element.IsOf(resultType, performExactMatch)).Value;
                    if (isOfEquivalentType)
                    {
                        result.Elements.Add(element.TreatAs(resultType));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Casts a <see cref="QueryScalarValue"/> to a <see cref="QueryScalarType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryScalarValue"/> which is cast to the appropriate type</returns>
        public override QueryValue Cast(QueryType type)
        {
            // For QueryCollectionValues, the cast operation casts all ELEMENTS to the type provided
            // rather than cast the collection type itself. This is consistent with the behaviour of
            // the cast operation in linq and esql.
            int counter = 0;
            
            while (counter < this.Elements.Count)
            {
                this.Elements[counter] = this.Elements[counter].Cast(type);
                counter++;
            }

            this.Type = type.CreateCollectionType();

            return this;
        }

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">A flag that determines if an exact match needs to be performed</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public override QueryValue IsOf(QueryType type, bool performExactMatch)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform IsOf on a collection of values"));
        }

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public override QueryValue TreatAs(QueryType type)
        {
            return type.CreateErrorValue(new QueryError("Cannot perform As on a collection of values"));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.EvaluationError != null)
            {
                return "Collection Error=" + this.EvaluationError + ", Type=" + this.Type.StringRepresentation;
            }
            else if (this.IsNull)
            {
                return "Null Collection, Type=" + this.Type.StringRepresentation;
            }
            else
            {
                return "Collection with " + this.Elements.Count + " items, Type=" + this.Type.StringRepresentation;
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query value.</param>
        /// <returns>The result of visiting this query value.</returns>
        public override TResult Accept<TResult>(IQueryValueVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected override QueryType GetTypeInternal()
        {
            return this.Type;
        }

        private static bool RowsAreOrdered(QueryScalarValue[] selectorsRow1, QueryScalarValue[] selectorsRow2, QueryOrdering ordering)
        {
            var verifyNextColumn = true;
            var col = 0;
            while (verifyNextColumn && col < selectorsRow1.Length)
            {
                verifyNextColumn = false;
                var isDecending = ordering.AreDescending.ElementAt(col);
                var compareResult = QueryScalarValue.Comparer.Compare(selectorsRow1[col], selectorsRow2[col]);
                if (compareResult == 0)
                {
                    verifyNextColumn = true;
                }

                if (isDecending)
                {
                    if (compareResult < 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (compareResult > 0)
                    {
                        return false;
                    }
                }

                col++;
            }

            return true;
        }

        /// <summary>
        /// Returns a boolean indicating whether the result set of orderby has non-deterministic ordering, ie, two results have duplicate orderby key rows.
        /// </summary>
        /// <param name="ordering">query ordering of orderby keys, note keys could be multiple.</param>
        /// <param name="elements">Elements to check against.</param>
        /// <returns>true if the orderby keys have duplicate rows, false otherwise.</returns>
        private static bool HasNondeterministicOrdering(QueryOrdering ordering, IEnumerable<QueryValue> elements)
        {
            ExceptionUtilities.CheckArgumentNotNull(elements, "elements");

            QueryScalarValue[][] sortedSelectorsTable = BuildSortedSelectorsTable(ordering, elements.Cast<object>());
            return ContainsAdjacentDuplicateRows(sortedSelectorsTable);
        }

        /// <summary>
        /// Checks if the sorted query primitive value table has two adjacent rows containing duplicate data. False otherwise.
        /// </summary>
        /// <param name="sortedSelectorsTable">the 2-dimensional array representation of sorted orderby primitive values</param>
        /// <returns>Returns true if the sorted query scalar value table has two adjacent rows containing duplicate data. False otherwise.</returns>
        private static bool ContainsAdjacentDuplicateRows(QueryScalarValue[][] sortedSelectorsTable)
        {
            // TODO: change to sortedSelectorsCollection.Distinct().Count != sortedSelectorsCollection.Count when Distinct() is supported on 
            // arbitrary queryvalues, to be specific, on row data
            int totalRows = sortedSelectorsTable.GetLength(0);

            for (int row = 0; row < totalRows - 1; row++)
            {
                if (AreDuplicateRows(sortedSelectorsTable[row], sortedSelectorsTable[row + 1]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the row1 and row2 contain duplicate data. 
        /// </summary>
        /// <param name="selectorsRow1">the current row to examine</param>
        /// <param name="selectorsRow2">the next row to examine</param>
        /// <returns>true if row1 and row2 contain duplicate data, false otherwise</returns>
        private static bool AreDuplicateRows(QueryScalarValue[] selectorsRow1, QueryScalarValue[] selectorsRow2)
        {
            for (int col = 0; col < selectorsRow1.Length; col++)
            {
                if (QueryScalarValue.Comparer.Compare(selectorsRow1[col], selectorsRow2[col]) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Builds a table of sorted orderby primitive values, where the rows by the size of result set, the columns by the number of orderby key selectors.
        /// </summary>
        /// <param name="ordering">query ordering of order by key selectors</param>
        /// <param name="elements">Elements used to get the table values.</param>
        /// <returns>the table of sorted orderby/thenby scalar values. </returns>
        private static QueryScalarValue[][] BuildSortedSelectorsTable(QueryOrdering ordering, IEnumerable<object> elements)
        {
            int orderingSelectorsCount = ordering.Selectors.Count();
            int selectorRowCount = elements.Count();
            var orderingSelectorsTable = new QueryScalarValue[selectorRowCount][];

            for (int i = 0; i < selectorRowCount; i++)
            {
                orderingSelectorsTable[i] = new QueryScalarValue[orderingSelectorsCount];
            }

            for (int row = 0; row < selectorRowCount; row++)
            {
                var currentRow = elements.ElementAt(row);
                for (int col = 0; col < orderingSelectorsCount; col++)
                {
                    var selector = ordering.Selectors.ElementAt(col);
                    orderingSelectorsTable[row][col] = selector(currentRow);
                }
            }

            return orderingSelectorsTable;
        }

        private QueryStructuralValue CreateGroupingValue(QueryValue key, QueryCollectionValue elements)
        {
            QueryGroupingType groupingType = new QueryGroupingType(key.Type, elements.Type.ElementType, key.Type.EvaluationStrategy);

            var error = QueryError.GetErrorFromValues(elements.Elements.Concat(new[] { key }));
            var result = new QueryStructuralValue(groupingType, false, error, groupingType.EvaluationStrategy);
            result.SetValue("Key", key);
            result.SetValue("Elements", elements);

            return result;
        }

        private void ComputeSelectResult(Func<QueryValue, QueryValue> selector, out IEnumerable<QueryValue> result, out QueryCollectionType resultType)
        {
            // compute the result type by applying predicate to a NULL value
            var nullElement = this.Type.ElementType.NullValue;
            var selectedElementType = selector(nullElement).Type;
            resultType = selectedElementType.CreateCollectionType();
            result = null;

            if (!this.IsNull)
            {
                result = this.Elements.Select(e => selector(e));
            }
        }

        private bool ValuesAreEqual(QueryValue firstValue, QueryValue secondValue)
        {
            var firstValueScalar = firstValue as QueryScalarValue;
            if (firstValueScalar != null)
            {
                var secondValueScalar = secondValue as QueryScalarValue;
                return secondValueScalar != null && this.ScalarValuesAreEqual(firstValueScalar, secondValueScalar);
            }

            var firstValueStructural = firstValue as QueryStructuralValue;
            if (firstValueStructural != null)
            {
                var secondValueStructural = secondValue as QueryStructuralValue;
                return secondValueStructural != null && this.StructuralValuesAreEqual(firstValueStructural, secondValueStructural);
            }

            var firstValueReference = firstValue as QueryReferenceValue;
            if (firstValueReference != null)
            {
                var secondValueReference = secondValue as QueryReferenceValue;
                return secondValueReference != null && this.ReferenceValuesAreEqual(firstValueReference, secondValueReference);
            }

            var firstValueRecord = firstValue as QueryRecordValue;
            if (firstValueRecord != null)
            {
                var secondValueRecord = secondValue as QueryRecordValue;
                return secondValueRecord != null && this.RecordValuesAreEqual(firstValueRecord, secondValueRecord);
            }

            var firstValueCollection = firstValue as QueryCollectionValue;
            ExceptionUtilities.Assert(firstValueCollection != null, "Now only support query value equality comparison on primitive / structural / collection, not {0}.", firstValue);

            var secondValueCollection = secondValue as QueryCollectionValue;
            return secondValueCollection != null && this.CollectionValuesAreEqual(firstValueCollection, secondValueCollection);
        }

        private bool ScalarValuesAreEqual(QueryScalarValue firstValueScalar, QueryScalarValue secondValueScalar)
        {
            if (!this.ScalarTypesAreEqualComparable(firstValueScalar.Type, secondValueScalar.Type))
            {
                return false;
            }

            if (firstValueScalar.IsNull)
            {
                return secondValueScalar.IsNull;
            }

            if (secondValueScalar.IsNull)
            {
                return false;
            }

            return (bool)firstValueScalar.EqualTo(secondValueScalar).Value;
        }

        private bool ScalarTypesAreEqualComparable(QueryScalarType firstScalarType, QueryScalarType secondScalarType)
        {
            return firstScalarType.Supports(QueryBinaryOperation.EqualTo, secondScalarType);
        }

        private bool StructuralValuesAreEqual(QueryStructuralValue firstValueStructural, QueryStructuralValue secondValueStructural)
        {
            if (!this.StructuralTypesAreEqual(firstValueStructural.Type, secondValueStructural.Type))
            {
                return false;
            }

            if (firstValueStructural.IsNull)
            {
                return secondValueStructural.IsNull;
            }
            else if (secondValueStructural.IsNull)
            {
                return false;
            }

            IEnumerable<QueryProperty> membersToCompare = firstValueStructural.Type.Properties;
            if (firstValueStructural.Type is QueryEntityType)
            {
                membersToCompare = membersToCompare.Where(m => m.IsPrimaryKey);
            }

            foreach (QueryProperty member in membersToCompare)
            {
                QueryValue innerFirstvalue = firstValueStructural.GetValue(member.Name);
                QueryValue innerSecondvalue = secondValueStructural.GetValue(member.Name);

                if (!this.ValuesAreEqual(innerFirstvalue, innerSecondvalue))
                {
                    return false;
                }
            }

            return true;
        }

        private bool StructuralTypesAreEqual(QueryStructuralType firstStructuralType, QueryStructuralType secondStructuralType)
        {
            var firstComplexType = firstStructuralType as QueryComplexType;
            if (firstComplexType != null)
            {
                var secondComplexType = secondStructuralType as QueryComplexType;
                return secondComplexType != null && firstComplexType.ComplexType.Equals(secondComplexType.ComplexType);
            }

            var firstEntityType = firstStructuralType as QueryEntityType;
            if (firstEntityType != null)
            {
                var secondEntityType = secondStructuralType as QueryEntityType;
                return secondEntityType != null && firstEntityType.EntityType.Equals(secondEntityType.EntityType);
            }

            var firstAnonymousType = firstStructuralType as QueryAnonymousStructuralType;
            if (firstAnonymousType != null)
            {
                var secondAnonymousType = secondStructuralType as QueryAnonymousStructuralType;
                return secondAnonymousType != null && firstAnonymousType.Equals(secondAnonymousType);
            }

            var firstGroupingType = firstStructuralType as QueryGroupingType;
            ExceptionUtilities.Assert(firstGroupingType != null, "Unknown QueryStructuralType: {0}", firstStructuralType);

            var secondGroupingType = secondStructuralType as QueryGroupingType;
            return secondGroupingType != null;
        }

        private bool RecordValuesAreEqual(QueryRecordValue firstValueRecord, QueryRecordValue secondValueRecord)
        {
            if (firstValueRecord.IsNull)
            {
                return secondValueRecord.IsNull;
            }
            else if (secondValueRecord.IsNull)
            {
                return false;
            }

            for (int index = 0; index < firstValueRecord.Type.Properties.Count; index++)
            {
                QueryValue innerFirstvalue = firstValueRecord.GetMemberValue(index);
                QueryValue innerSecondvalue = secondValueRecord.GetMemberValue(index);

                if (!this.ValuesAreEqual(innerFirstvalue, innerSecondvalue))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ReferenceValuesAreEqual(QueryReferenceValue firstValueReference, QueryReferenceValue secondValueReference)
        {
            if (firstValueReference.EntitySetFullName != secondValueReference.EntitySetFullName)
            {
                return false;
            }

            return this.ValuesAreEqual(firstValueReference.KeyValue, secondValueReference.KeyValue);
        }

        private bool CollectionValuesAreEqual(QueryCollectionValue firstValueCollection, QueryCollectionValue secondValueCollection)
        {
            if (firstValueCollection.IsNull)
            {
                return secondValueCollection.IsNull;
            }
            else if (secondValueCollection.IsNull)
            {
                return false;
            }

            if (firstValueCollection.Elements.Count != secondValueCollection.Elements.Count)
            {
                return false;
            }

            for (int i = 0; i < firstValueCollection.Elements.Count; i++)
            {
                if (!firstValueCollection.Elements.Any(e => this.ValuesAreEqual(e, secondValueCollection.Elements[i])) ||
                    !secondValueCollection.Elements.Any(e => this.ValuesAreEqual(e, firstValueCollection.Elements[i])))
                {
                    return false;
                }
            }

            return true;
        }
    }
}