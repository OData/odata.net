//---------------------------------------------------------------------
// <copyright file="LimitCoverageStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Limits the coverage of vectors from a wrapped strategy.
	/// </summary>
	/// <example><code><![CDATA[
	/// CombinatorialStrategy baseStrategy = new ExhaustiveCombinatorialStrategy(TestMatrix, Constraints);
	/// IntegerRangeCategory hugeCategory = new IntegerRangeCategory("Huge", 100000, 120000);
	/// baseStrategy.PartitionDimension(_tableSizeDimension,
	/// 	new PointCategory<int>("Zero", 0),
	/// 	new IntegerRangeCategory("Medium", 100, 200),
	/// 	hugeCategory);
	/// // Set other dimension strategies...
	/// // Shuffle the base strategy to get different vectors each time,
	/// // then limit it to only execute one test vector with a huge table (since it'll take a big amount of time)
	/// LimitCoverageStrategy _fullStrategy = new LimitCoverageStrategy(new ShuffleStrategy<Vector>(baseStrategy));
	/// _fullStrategy.RestrictCategoryCoverage(_tableSizeDimension, hugeCategory, 1);
	/// ]]></code></example>
	public class LimitCoverageStrategy : ExplorationStrategy<Vector>
	{
		#region LimitingInformation classes
		/// <summary>
		/// An accumulator that monitors the current coverage level of the stream of coming vectors,
		/// and signals if it's already satisfied at any point.
		/// </summary>
		private interface ICoverageAccumulator
		{
			/// <summary>
			/// Take the given vector and update myself to mark it as covered.
			/// </summary>
			/// <param name="vector">The vector.</param>
			void Accumulate(Vector vector);

			/// <summary>
			/// Checks if we've already covered enough vectors similar to the given vector.
			/// </summary>
			/// <param name="vector">The vector.</param>
			/// <returns><c>true</c> iff we have covered enough similar vectors and shouldn't yield <paramref name="vector"/>.</returns>
			bool AlreadyCovered(Vector vector);
		}

		/// <summary>
		/// An implementation of <see cref="ICoverageAccumulator"/> that just increments a count every
		/// time it sees a qualifying vector, and makes sure to stay below a given maximum count.
		/// </summary>
		private class CountAccumulator : ICoverageAccumulator
		{
			private readonly Predicate<Vector> _doesQualify;
			private readonly int _maxCount;
			private int _currentCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="CountAccumulator"/> class.
			/// </summary>
			/// <param name="maxCount">The maximum count.</param>
			/// <param name="doesQualify">A predicate to check if any vector qualifies for this accumulator.</param>
			public CountAccumulator(int maxCount, Predicate<Vector> doesQualify)
			{
				if (doesQualify == null)
				{
					throw new ArgumentNullException("doesQualify");
				}
				_maxCount = maxCount;
				_doesQualify = doesQualify;
			}

			#region ICoverageAccumulator Members

			/// <summary>
			/// Take the given vector and update myself to mark it as covered.
			/// </summary>
			/// <param name="vector">The vector.</param>
			public void Accumulate(Vector vector)
			{
				if (_doesQualify(vector))
				{
					_currentCount++;
				}
			}

			/// <summary>
			/// Checks if we've already covered enough vectors similar to the given vector.
			/// </summary>
			/// <param name="vector">The vector.</param>
			/// <returns><c>true</c> iff we have covered enough similar vectors and shouldn't yield <paramref name="vector"/>.</returns>
			public bool AlreadyCovered(Vector vector)
			{
				return _currentCount >= _maxCount && _doesQualify(vector);
			}
			#endregion
		}

		/// <summary>
		/// A data structure to hold information on how to limit the coverage of my stream of vectors.
		/// </summary>
		private interface ILimitingInformation
		{
			/// <summary>
			/// Creates the accumulator that will monitor the current stream of vectors.
			/// </summary>
			/// <returns>A proper accumulator (should never be <c>null</c>).</returns>
			ICoverageAccumulator CreateAccumulator();
		}

		/// <summary>
		/// An implementation of <see cref="ILimitingInformation"/> to limit vectors based on
		/// the coverage of a specific dimension.
		/// </summary>
		/// <typeparam name="T">The type of the domain of the dimension.</typeparam>
		private class DimensionLimitingInformation<T> : ILimitingInformation
		{
			private readonly int _maxCount;
			private readonly Predicate<Vector> _doesQualify;

			private DimensionLimitingInformation(int maxCount, Predicate<Vector> doesQualify)
			{
				if (doesQualify == null)
				{
					throw new ArgumentNullException("doesQualify");
				}
				_maxCount = maxCount;
				_doesQualify = doesQualify;
			}

			/// <summary>
			/// Creates a <see cref="DimensionLimitingInformation&lt;T&gt;"/> object that limits the stream
			/// of vectors based on how many time a specific value is covered.
			/// </summary>
			/// <param name="dimension">The dimension.</param>
			/// <param name="limitedValue">The limited value.</param>
			/// <param name="maxCount">The max count.</param>
			/// <returns></returns>
			public static DimensionLimitingInformation<T> LimitValue(Dimension<T> dimension, T limitedValue, int maxCount)
			{
				if (dimension == null)
				{
					throw new ArgumentNullException("dimension");
				}
				return new DimensionLimitingInformation<T>(maxCount,
					delegate(Vector v) { return Object.Equals(v.GetValue(dimension), limitedValue); });
			}

			/// <summary>
			/// Creates a <see cref="DimensionLimitingInformation&lt;T&gt;"/> object that limits the stream
			/// of vectors based on how many time a specific category is covered.
			/// </summary>
			/// <param name="dimension">The dimension.</param>
			/// <param name="limitedCategory">The limited category.</param>
			/// <param name="maxCount">The max count.</param>
			/// <returns></returns>
			public static DimensionLimitingInformation<T> LimitCategory(Dimension<T> dimension, Category<T> limitedCategory, int maxCount)
			{
				if (dimension == null)
				{
					throw new ArgumentNullException("dimension");
				}
				return new DimensionLimitingInformation<T>(maxCount,
					delegate(Vector v) { return Object.Equals(v.GetCategory(dimension), limitedCategory); });
			}

			/// <summary>
			/// Creates the accumulator that will monitor the current stream of vectors.
			/// </summary>
			/// <returns>
			/// A proper accumulator (should never be <c>null</c>).
			/// </returns>
			public ICoverageAccumulator CreateAccumulator()
			{
				return new CountAccumulator(_maxCount, _doesQualify);
			}
		}
		#endregion

		#region Private data members
		private List<ILimitingInformation> _limitingInformation;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="LimitCoverageStrategy"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		public LimitCoverageStrategy(ExplorationStrategy<Vector> wrappedStrategy)
			: base(wrappedStrategy)
		{
			_limitingInformation = new List<ILimitingInformation>();
		}

		/// <summary>
		/// Restricts the coverage of a given value in a given dimension to a limited number of times.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetDimension">The target dimension.</param>
		/// <param name="limitedValue">The value to restrict the coverage of.</param>
		/// <param name="maxCount">The maximum number of times to include this value in a vector.</param>
		public void RestrictValueCoverage<T>(Dimension<T> targetDimension,
			T limitedValue, int maxCount)
		{
			_limitingInformation.Add(DimensionLimitingInformation<T>.LimitValue(targetDimension, limitedValue, maxCount));
		}

		/// <summary>
		/// Restricts the coverage of a given category in a given dimension to a limited number of times.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetDimension">The target dimension.</param>
		/// <param name="limitedCategory">The category to restrict the coverage of.</param>
		/// <param name="maxCount">The maximum number of times to include this category in a vector.</param>
		public void RestrictCategoryCoverage<T>(Dimension<T> targetDimension,
			Category<T> limitedCategory, int maxCount)
		{
			_limitingInformation.Add(DimensionLimitingInformation<T>.LimitCategory(targetDimension, limitedCategory, maxCount));
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<Vector> Explore()
		{
			List<ICoverageAccumulator> accumulators = _limitingInformation.Select(x => x.CreateAccumulator()).ToList();

			foreach (Vector current in WrappedStrategies[0].Explore())
			{
				if (accumulators.Exists(delegate(ICoverageAccumulator accumulator) { return accumulator.AlreadyCovered(current); }))
				{
					// It's already covered, skip
					continue;
				}
				
                yield return current;

                // Mark it as covered
                foreach (var accumulator in accumulators)
                {
                    accumulator.Accumulate(current);
                }
			}
		}
	}
}
