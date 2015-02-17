//---------------------------------------------------------------------
// <copyright file="Strategies.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A collection of helper factory and extension methods to make creating exploration strategies simple.
	/// </summary>
	public static class Strategies
	{
		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			this Dimension<TCollection> dimension, bool allowEmptySet, params TValue[] possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(allowEmptySet, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			this Dimension<TCollection> dimension, bool allowEmptySet, IEnumerable<TValue> possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(allowEmptySet, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			this Dimension<TCollection> dimension, params TValue[] possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			this Dimension<TCollection> dimension, IEnumerable<TValue> possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The easy usage comes from the overload that takes a dimension parameter.")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			bool allowEmptySet, params TValue[] possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(allowEmptySet, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The easy usage comes from the overload that takes a dimension parameter.")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			bool allowEmptySet, IEnumerable<TValue> possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(allowEmptySet, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The easy usage comes from the overload that takes a dimension parameter.")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			params TValue[] possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all possible non-empty combinations for a given set of values.
		/// </summary>
		/// <typeparam name="TCollection">The type of the collection.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		/// <seealso cref="ParseableCollection&lt;T&gt;"/>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The easy usage comes from the overload that takes a dimension parameter.")]
		public static AllCombinationsStrategy<TCollection, TValue> AllCombinations<TCollection, TValue>(
			IEnumerable<TValue> possibleValues)
			where TCollection : ICollection<TValue>, new()
		{
			return new AllCombinationsStrategy<TCollection, TValue>(possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that that partitions the dimension into a number of categories (or equivalence classes),
		/// then picks one value from each category.
		/// </summary>
		/// <typeparam name="T">Type of values in the dimension.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="categories">The categories.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static CategoricalStrategy<T> Categorized<T>(this Dimension<T> dimension, params Category<T>[] categories)
		{
			return new CategoricalStrategy<T>(categories);
		}

		/// <summary>
		/// Returns an exploration strategy that that partitions the dimension into a number of categories (or equivalence classes),
		/// then picks one value from each category.
		/// </summary>
		/// <typeparam name="T">Type of values in the dimension.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="categories">The categories.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static CategoricalStrategy<T> Categorized<T>(this Dimension<T> dimension, IEnumerable<Category<T>> categories)
		{
			return new CategoricalStrategy<T>(categories);
		}

		/// <summary>
		/// Returns an exploration strategy that that partitions the dimension into a number of categories (or equivalence classes),
		/// then picks one value from each category.
		/// </summary>
		/// <typeparam name="T">Type of values in the dimension.</typeparam>
		/// <param name="categories">The categories.</param>
		/// <returns></returns>
		public static CategoricalStrategy<T> Categorized<T>(params Category<T>[] categories)
		{
			return new CategoricalStrategy<T>(categories);
		}

		/// <summary>
		/// Returns an exploration strategy that that partitions the dimension into a number of categories (or equivalence classes),
		/// then picks one value from each category.
		/// </summary>
		/// <typeparam name="T">Type of values in the dimension.</typeparam>
		/// <param name="categories">The categories.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static CategoricalStrategy<T> Categorized<T>(IEnumerable<Category<T>> categories)
		{
			return new CategoricalStrategy<T>(categories);
		}

		/// <summary>
		/// Returns a combinatorial exploration strategy that returns all possible combinations from the dimensions.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <returns></returns>
		public static ExhaustiveCombinatorialStrategy Exhaustive(this Matrix matrix, params IConstraint[] constraints)
		{
			return new ExhaustiveCombinatorialStrategy(matrix, constraints);
		}

		/// <summary>
		/// Returns a combinatorial exploration strategy that returns all possible combinations from the dimensions.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <returns></returns>
		public static ExhaustiveCombinatorialStrategy Exhaustive(this Matrix matrix, IEnumerable<IConstraint> constraints)
		{
			return new ExhaustiveCombinatorialStrategy(matrix, constraints);
		}

		/// <summary>
		/// Returns a combinatorial exploration strategy that ensures that all pair-wise combinations for each two dimensions are picked.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <returns></returns>
		public static PairwiseStrategy Pairwise(this Matrix matrix, Func<int, int> nextInt, params IConstraint[] constraints)
		{
            return new PairwiseStrategy(matrix, nextInt, constraints);
		}

		/// <summary>
		/// Returns a combinatorial exploration strategy that ensures that all pair-wise combinations for each two dimensions are picked.
		/// </summary>
		/// <param name="matrix">The matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <returns></returns>
        public static PairwiseStrategy Pairwise(this Matrix matrix, Func<int, int> nextInt, IEnumerable<IConstraint> constraints)
		{
            return new PairwiseStrategy(matrix, nextInt, constraints);
		}

		/// <summary>
		///  Returns an exploration strategy which simply returns all values in the given list.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="wantedValues">The values.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static ExhaustiveIEnumerableStrategy<T> Values<T>(this Dimension<T> dimension, params T[] wantedValues)
		{
			return new ExhaustiveIEnumerableStrategy<T>(wantedValues);
		}

		/// <summary>
		///  Returns an exploration strategy which simply returns all values in the given list.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="wantedValues">The values.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static ExhaustiveIEnumerableStrategy<T> Values<T>(this Dimension<T> dimension, IEnumerable<T> wantedValues)
		{
			return new ExhaustiveIEnumerableStrategy<T>(wantedValues);
		}

		/// <summary>
		///  Returns an exploration strategy which simply returns all values in the given list.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wantedValues">The values.</param>
		/// <returns></returns>
		public static ExhaustiveIEnumerableStrategy<T> Values<T>(params T[] wantedValues)
		{
			return new ExhaustiveIEnumerableStrategy<T>(wantedValues);
		}

		/// <summary>
		///  Returns an exploration strategy which simply returns all values in the given list.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wantedValues">The values.</param>
		/// <returns></returns>
		public static ExhaustiveIEnumerableStrategy<T> Values<T>(IEnumerable<T> wantedValues)
		{
			return new ExhaustiveIEnumerableStrategy<T>(wantedValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all the values from within a given range.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="inclusiveLowerLimit">The inclusive lower limit.</param>
		/// <param name="exclusiveUpperLimit">The exclusive upper limit.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static IntegerRangeStrategy Range(this Dimension<int> dimension, int inclusiveLowerLimit, int exclusiveUpperLimit)
		{
			return new IntegerRangeStrategy(inclusiveLowerLimit, exclusiveUpperLimit);
		}

		/// <summary>
		/// Returns an exploration strategy that returns all the values from within a given range.
		/// </summary>
		/// <param name="inclusiveLowerLimit">The inclusive lower limit.</param>
		/// <param name="exclusiveUpperLimit">The exclusive upper limit.</param>
		/// <returns></returns>
		public static IntegerRangeStrategy Range(int inclusiveLowerLimit, int exclusiveUpperLimit)
		{
			return new IntegerRangeStrategy(inclusiveLowerLimit, exclusiveUpperLimit);
		}

		/// <summary>
		/// Returns an exploration strategy which groups vectors by a set of dimensions so that
		/// all vectors with the same values/categories are returned together.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="groupingDimensions">The grouping dimensions.</param>
		/// <returns></returns>
		public static ExplorationStrategy<Vector> GroupBy(this ExplorationStrategy<Vector> wrappedStrategy,
			params Dimension[] groupingDimensions)
		{
			return new GroupByStrategy(wrappedStrategy, groupingDimensions);
		}

		/// <summary>
		/// Returns an exploration strategy which groups vectors by a set of dimensions so that
		/// all vectors with the same values/categories are returned together.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="groupingDimensions">The grouping dimensions.</param>
		/// <returns></returns>
		public static ExplorationStrategy<Vector> GroupBy(this ExplorationStrategy<Vector> wrappedStrategy,
			IEnumerable<Dimension> groupingDimensions)
		{
			return new GroupByStrategy(wrappedStrategy, groupingDimensions);
		}

		/// <summary>
		/// Returns an exploration strategy which restricts the coverage of vectors from a wrapped strategy.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="targetDimension">The target dimension.</param>
		/// <param name="limitedValue">The value to restrict the coverage of.</param>
		/// <param name="maxCount">The maximum number of times to include this value in a vector.</param>
		/// <returns></returns>
		/// <example><code><![CDATA[
		/// CombinatorialStrategy baseStrategy = TestMatrix.ExhaustiveCombinatorial(Constraints);
		/// baseStrategy.SetDimensionStrategy(_typeDimension, _typeDimension.AllValues(PimodType.Int(), PimodType.VarChar(-1));
		/// // Set other dimension strategies...
		/// // Shuffle the base strategy to get different vectors each time,
		/// // then limit it to only execute one test vector with a LOB(since it'll take a big amount of time)
		/// _fullStrategy = baseStrategy.Shuffle().RestrictCoverage(_typeDimension, PimodType.VarChar(-1), 1);
		/// ]]></code></example>
		public static LimitCoverageStrategy RestrictCoverage<T>(this ExplorationStrategy<Vector> wrappedStrategy,
			Dimension<T> targetDimension, T limitedValue, int maxCount)
		{
			LimitCoverageStrategy ret = new LimitCoverageStrategy(wrappedStrategy);
			ret.RestrictValueCoverage(targetDimension, limitedValue, maxCount);
			return ret;
		}

		/// <summary>
		/// Returns an exploration strategy which restricts the coverage of vectors from a wrapped strategy.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="targetDimension">The target dimension.</param>
		/// <param name="limitedCategory">The category to restrict the coverage of.</param>
		/// <param name="maxCount">The maximum number of times to include this category in a vector.</param>
		/// <returns></returns>
		/// <example><code><![CDATA[
		/// CombinatorialStrategy baseStrategy = TestMatrix.ExhaustiveCombinatorial(Constraints);
		/// IntegerRangeCategory hugeCategory = new IntegerRangeCategory("Huge", 100000, 120000);
		/// baseStrategy.PartitionDimension(_tableSizeDimension,
		/// 	new PointCategory<int>("Zero", 0),
		/// 	new IntegerRangeCategory("Medium", 100, 200),
		/// 	hugeCategory);
		/// // Set other dimension strategies...
		/// // Shuffle the base strategy to get different vectors each time,
		/// // then limit it to only execute one test vector with a huge table (since it'll take a big amount of time)
		/// _fullStrategy = baseStrategy.Shuffle().RestrictCoverage(_typeDimension, hugeCategory, 1);
		/// ]]></code></example>
		public static LimitCoverageStrategy RestrictCoverage<T>(this ExplorationStrategy<Vector> wrappedStrategy,
			Dimension<T> targetDimension, Category<T> limitedCategory, int maxCount)
		{
			LimitCoverageStrategy ret = new LimitCoverageStrategy(wrappedStrategy);
			ret.RestrictCategoryCoverage(targetDimension, limitedCategory, maxCount);
			return ret;
		}

		/// <summary>
		/// Returns an exploration strategy for Enum types that returns one (or a fixed number of) random value(s) from this enum each time.
		/// </summary>
		/// <typeparam name="T">The type of values (must be an enum type)</typeparam>
		/// <param name="dimension">The target dimension.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static RandomEnumStrategy<T> RandomEnum<T>(this Dimension<T> dimension, Func<int, int> nextInt)
		{
            return new RandomEnumStrategy<T>(nextInt);
		}

		/// <summary>
		/// Returns an exploration strategy for Enum types that returns one (or a fixed number of) random value(s) from this enum each time.
		/// </summary>
		/// <typeparam name="T">The type of values (must be an enum type)</typeparam>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The easy usage comes from the overload that takes a dimension parameter.")]
		public static RandomEnumStrategy<T> RandomEnum<T>(Func<int, int> nextInt)
		{
            return new RandomEnumStrategy<T>(nextInt);
		}

		/// <summary>
		/// Returns an exploration strategy that returns one random value from the given set of possible values.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="dimension">The target dimension.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
		public static RandomIEnumerableStrategy<T> Random<T>(this Dimension<T> dimension, Func<int, int> nextInt, params T[] possibleValues)
		{
			return new RandomIEnumerableStrategy<T>(nextInt, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns one random value from the given set of possible values.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="dimension">The target dimension.</param>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
        public static RandomIEnumerableStrategy<T> Random<T>(this Dimension<T> dimension, Func<int, int> nextInt, IEnumerable<T> possibleValues)
		{
			return new RandomIEnumerableStrategy<T>(nextInt, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns one random value from the given set of possible values.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
        public static RandomIEnumerableStrategy<T> Random<T>(Func<int, int> nextInt, params T[] possibleValues)
		{
			return new RandomIEnumerableStrategy<T>(nextInt, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy that returns one random value from the given set of possible values.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="possibleValues">The possible values.</param>
		/// <returns></returns>
        public static RandomIEnumerableStrategy<T> Random<T>(Func<int, int> nextInt, IEnumerable<T> possibleValues)
		{
			return new RandomIEnumerableStrategy<T>(nextInt, possibleValues);
		}

		/// <summary>
		/// Returns an exploration strategy for integers that just randomly select an integer from within a range every time.
		/// </summary>
		/// <param name="dimension">The target dimension.</param>
		/// <param name="inclusiveLowerLimit">The inclusive lower limit.</param>
		/// <param name="exclusiveUpperLimit">The exclusive upper limit.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dimension", Justification = "Discoverability")]
        public static RandomIntegerRangeStrategy RandomRange(this Dimension<int> dimension, int inclusiveLowerLimit, int exclusiveUpperLimit, Func<int, int, int> nextIntInRange)
		{
            return new RandomIntegerRangeStrategy(inclusiveLowerLimit, exclusiveUpperLimit, nextIntInRange);
		}

		/// <summary>
		/// Returns an exploration strategy for integers that just randomly select an integer from within a range every time.
		/// </summary>
		/// <param name="inclusiveLowerLimit">The inclusive lower limit.</param>
		/// <param name="exclusiveUpperLimit">The exclusive upper limit.</param>
		/// <returns></returns>
        public static RandomIntegerRangeStrategy RandomRange(int inclusiveLowerLimit, int exclusiveUpperLimit, Func<int, int, int> nextIntInRange)
		{
            return new RandomIntegerRangeStrategy(inclusiveLowerLimit, exclusiveUpperLimit, nextIntInRange);
		}

		/// <summary>
		/// Returns an exploration strategy that just repeats the values returned by the underlying strategy a set number of times.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="numberOfTimesToRepeat">The number of times to repeat.</param>
		/// <returns></returns>
		public static RepeatStrategy<T> Repeat<T>(this ExplorationStrategy<T> wrappedStrategy, int numberOfTimesToRepeat)
		{
			return new RepeatStrategy<T>(wrappedStrategy, numberOfTimesToRepeat);
		}

		/// <summary>
		/// Returns an exploration strategy that shuffles the values returned by the underlying strategy.
		/// </summary>
		/// <typeparam name="T">The type of values in the dimension.</typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <returns></returns>
        public static ShuffleStrategy<T> Shuffle<T>(this ExplorationStrategy<T> wrappedStrategy, Func<int, int> nextInt)
		{
            return new ShuffleStrategy<T>(wrappedStrategy, nextInt);
		}

		/// <summary>
		/// Returns an exploration strategy that gives the union (with duplicate removal) of the two given strategies.
		/// </summary>
		/// <param name="firstStrategy">The first strategy.</param>
		/// <param name="secondStrategy">The second strategy.</param>
		/// <returns></returns>
		public static UnionStrategy Union(this ExplorationStrategy<Vector> firstStrategy,
			ExplorationStrategy<Vector> secondStrategy)
		{
			return new UnionStrategy(firstStrategy, secondStrategy);
		}

		/// <summary>
		/// Returns an exploration strategy that gives the union (with duplicate removal) of the two given strategies.
		/// </summary>
		/// <param name="firstStrategy">The first strategy.</param>
		/// <param name="secondStrategy">The second strategy.</param>
		/// <returns></returns>
		public static UnionStrategy<T> Union<T>(this ExplorationStrategy<T> firstStrategy,
			ExplorationStrategy<T> secondStrategy)
		{
			return new UnionStrategy<T>(firstStrategy, secondStrategy);
		}

		/// <summary>
		/// Returns an exploration strategy that gives the concatenation (no duplicate removal) of the two given strategies.
		/// </summary>
		/// <param name="firstStrategy">The first strategy.</param>
		/// <param name="secondStrategy">The second strategy.</param>
		/// <returns></returns>
		public static ConcatStrategy<T> Concat<T>(this ExplorationStrategy<T> firstStrategy,
			ExplorationStrategy<T> secondStrategy)
		{
			return new ConcatStrategy<T>(firstStrategy, secondStrategy);
		}

		/// <summary>
		/// Returns an exploration strategy that takes the first fixed number of elements from a wrapped strategy (or all the
		/// elements if there aren't enough to take) and just returns those.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="numberToTake">The number to take.</param>
		/// <returns></returns>
		public static FirstNStrategy<T> Take<T>(this ExplorationStrategy<T> wrappedStrategy, int numberToTake)
		{
			return new FirstNStrategy<T>(wrappedStrategy, numberToTake);
		}

		/// <summary>
		/// Returns an exploration strategy that filters values from a wrapped strategy using a given predicate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		public static FilterStrategy<T> Where<T>(this ExplorationStrategy<T> wrappedStrategy, Func<T, bool> predicate)
		{
			return new FilterStrategy<T>(wrappedStrategy, predicate);
		}
	}
}
