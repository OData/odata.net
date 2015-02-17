//---------------------------------------------------------------------
// <copyright file="GroupByStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Groups vectors by a set of dimensions so that all vectors with the
	/// same values/categories are returned together.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This strategy is needed when you need to reuse resources across multiple vectors, and
	/// you want to maximize that resource usage. For example, say you have a couple of tables in your
	/// suite that you want to populate and use, and you have a dimension per table specifying its size.
	/// You would (probably) want to reuse the same tables as much as possible instead of repopulating
	/// them while exercising the other dimensions. In order to do that, you would the stream of vectors
	/// to come like this:</para>
	/// <para>
	/// (FirstTableSize:10000, SecondTableSize:10000, JoinType:Inner, ...)
	/// (FirstTableSize:10000, SecondTableSize:10000, JoinType:LeftOuter, ...)
	/// (FirstTableSize:10000, SecondTableSize:10000, JoinType:RightOuter, ...)
	/// ...
	/// (FirstTableSize:10000, SecondTableSize:50000, JoinType:Inner, ...)
	/// (FirstTableSize:10000, SecondTableSize:50000, JoinType:LeftOuter, ...)
	/// ...
	/// </para>
	/// This way, if your test suite is written craftily enough, it can reuse the same tables across the first
	/// block of vectors, and only switch when necessary.
	/// </remarks>
	/// <example><code><![CDATA[
	/// TestMatrix = new Matrix(_firstTableSizeDimension, _secondTableSizeDimension, _joinHintDimension, _joinTypeDimension);
	/// _fullStrategy = new ExhaustiveCombinatorialStrategy(TestMatrix, Constraints);
	/// _fullStrategy.SetDimensionStrategy(_joinHintDimension, new ExhaustiveIEnumerableStrategy<JoinHint>(
	/// 	JoinHint.None, JoinHint.Loop, JoinHint.Hash, JoinHint.Merge));
	/// _fullStrategy.PartitionDimension(_firstTableSizeDimension,
	/// 	new IntegerRangeCategory("S", 100, 200),
	/// 	new IntegerRangeCategory("L", 100000, 200000));
	/// _fullStrategy.PartitionDimension(_secondTableSizeDimension,
	/// 	new IntegerRangeCategory("S", 100, 200),
	/// 	new IntegerRangeCategory("M", 10000, 20000),
	/// 	new IntegerRangeCategory("L", 100000, 200000));
	/// _fullStrategy = new GroupByStrategy(_fullStrategy, _firstTableSizeDimension, _secondTableSizeDimension);
	/// ]]></code>
	/// </example>
	public class GroupByStrategy : ExplorationStrategy<Vector>
	{
		#region Private data
		private List<Dimension> _groupingDimensions;
		private bool _ensureSameValuesForCategorizedValues = true;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupByStrategy"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="groupingDimensions">The grouping dimensions.</param>
		public GroupByStrategy(ExplorationStrategy<Vector> wrappedStrategy, params Dimension[] groupingDimensions)
			: this(wrappedStrategy, (IEnumerable<Dimension>)groupingDimensions)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupByStrategy"/> class.
		/// </summary>
		/// <param name="wrappedStrategy">The wrapped strategy.</param>
		/// <param name="groupingDimensions">The grouping dimensions.</param>
		public GroupByStrategy(ExplorationStrategy<Vector> wrappedStrategy, IEnumerable<Dimension> groupingDimensions)
			: base(wrappedStrategy)
		{
			_groupingDimensions = new List<Dimension>(groupingDimensions);
		}
		#endregion

		#region Public properties
		/// <summary>
		/// Gets or sets a value indicating whether to ensure same values for categorized values within a group (default is <c>true</c>).
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this property is <c>true</c> (default), then this strategy will ensure that vectors in the same group will have the same
		/// value for each of the grouping dimensions if they have the same category.
		/// </para>
		/// <para>
		/// For example, if the underlying strategy gave out the vectors
		/// </para>
		/// <para>
		/// ((Large,1234), (Medium,223), A, ...)
		/// ((Large,1333), (Medium,111), B, ...)
		/// </para>
		/// <para>
		/// And we're grouping by the first two dimensions, this strategy will transform the vectors to be:
		/// </para>
		/// <para>
		/// ((Large,1234), (Medium,223), A, ...)
		/// ((Large,1234), (Medium,223), B, ...)
		/// </para>
		/// </remarks>
		/// <value>
		/// 	<c>true</c> if the strategy will ensure same values for categorized values within a group; otherwise, <c>false</c>.
		/// </value>
		public bool EnsureSameValuesForCategorizedValues
		{
			get { return _ensureSameValuesForCategorizedValues; }
			set { _ensureSameValuesForCategorizedValues = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		public override IEnumerable<Vector> Explore()
		{
			Dictionary<GroupingId, List<Vector>> groups = new Dictionary<GroupingId, List<Vector>>();
			foreach (Vector v in WrappedStrategies[0].Explore())
			{
				GroupingId group = new GroupingId(v, _groupingDimensions);
				List<Vector> list;
				if (!groups.TryGetValue(group, out list))
				{
					list = new List<Vector>();
					groups.Add(group, list);
				}
				list.Add(v);
			}
			foreach (KeyValuePair<GroupingId, List<Vector>> group in groups)
			{
				foreach (Vector v in group.Value)
				{
					if (EnsureSameValuesForCategorizedValues)
					{
						group.Key.EnsureSameValuesForCategorizedValues(v);
					}
					yield return v;
				}
			}
		}
		#endregion

		/// <summary>
		/// This holds the grouping information for a set of vectors.
		/// </summary>
		/// <remarks>
		/// The strategy will hold that for any two vectors v1 and v2,
		/// iff <c>new GroupingId(v1).Equals(new GroupingId(v2))</c>, then
		/// v1 and v2 will be in the same group.
		/// </remarks>
		private sealed class GroupingId
		{
			private readonly Dictionary<Dimension, ValueFactoryWithOptionalConcreteValue> _values;

			/// <summary>
			/// Initializes a new instance of the <see cref="GroupingId"/> class.
			/// </summary>
			/// <param name="v">The prototype vector for the group.</param>
			/// <param name="dimensions">The dimensions.</param>
			public GroupingId(Vector v, IEnumerable<Dimension> dimensions)
			{
				_values = new Dictionary<Dimension, ValueFactoryWithOptionalConcreteValue>();
				foreach (Dimension d in dimensions)
				{
					_values.Add(d, v.GetValueFactoryWithOptionalConcreteValue(d));
				}
			}

			/// <summary>
			/// Ensures the same values for categorized values.
			/// </summary>
			/// <param name="v">The vector.</param>
			public void EnsureSameValuesForCategorizedValues(Vector v)
			{
				List<Dimension> temp = new List<Dimension>(_values.Keys);
				foreach (Dimension d in temp)
				{
					if (_values[d].ValueFactory.Equals(v.GetValueFactoryWithOptionalConcreteValue(d).ValueFactory))
					{
						if (!_values[d].IsConcrete)
						{
							_values[d] = _values[d].MakeConcrete();
						}
						v.SetBaseValue(d, _values[d]);
					}
				}
			}

			/// <summary>
			/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
			/// </summary>
			/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
			/// <returns>
			/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
			/// </returns>
			/// <exception cref="T:System.NullReferenceException">
			/// The <paramref name="obj"/> parameter is null.
			/// </exception>
			public override bool Equals(object obj)
			{
				GroupingId other = obj as GroupingId;
				if (other == null)
				{
					return false;
				}
				if (_values.Count != other._values.Count)
				{
					return false;
				}
				foreach (Dimension d in _values.Keys)
				{
					ValueFactoryWithOptionalConcreteValue thisValue = _values[d], otherValue;
					Debug.Assert(thisValue != null);
					if (!other._values.TryGetValue(d, out otherValue))
					{
						return false;
					}
					if (!thisValue.ValueFactory.Equals(otherValue.ValueFactory))
					{
						return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <returns>
			/// A hash code for the current <see cref="T:System.Object"/>.
			/// </returns>
			public override int GetHashCode()
			{
				unchecked
				{
					int ret = 0;
					foreach (KeyValuePair<Dimension, ValueFactoryWithOptionalConcreteValue> v in _values)
					{
						ret += v.Key.GetHashCode();
						ret += v.Value.GetHashCode();
					}
					return ret;
				}
			}
		}
	}

}
