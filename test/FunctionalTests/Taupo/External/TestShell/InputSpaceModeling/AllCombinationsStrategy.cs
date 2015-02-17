//---------------------------------------------------------------------
// <copyright file="AllCombinationsStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy that returns all possible combinations for a given set of values.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <seealso cref="ParseableCollection&lt;T&gt;"/>
	public class AllCombinationsStrategy<TCollection, TValue> : ExplorationStrategy<TCollection>
		where TCollection : ICollection<TValue>, new()
	{
		private readonly bool _allowEmptySet;
		private ReadOnlyCollection<TValue> _possibleValues;

		/// <summary>
		/// Gets a value indicating whether the empty combination will be included in the strategy.
		/// </summary>
		public bool AllowEmptySet
		{
			get { return _allowEmptySet; }
		}

		/// <summary>
		/// Gets the possible values.
		/// </summary>
		/// <value>The possible values.</value>
		public ReadOnlyCollection<TValue> PossibleValues
		{
			get { return _possibleValues; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AllCombinationsStrategy&lt;TCollection, TValue&gt;"/> class.
		/// </summary>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values to combine.</param>
		/// <exception cref="ArgumentNullException"><paramref name="possibleValues"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="possibleValues"/> has more than 63 elements.</exception>
		public AllCombinationsStrategy(bool allowEmptySet, params TValue[] possibleValues)
			: this(allowEmptySet, (IEnumerable<TValue>)possibleValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AllCombinationsStrategy&lt;TCollection, TValue&gt;"/> class.
		/// </summary>
		/// <param name="allowEmptySet">Parameter indicating whether the empty combination will be included in the strategy.
		/// The default value is <c>false</c>.</param>
		/// <param name="possibleValues">The possible values to combine.</param>
		/// <exception cref="ArgumentNullException"><paramref name="possibleValues"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="possibleValues"/> has more than 63 elements.</exception>
		public AllCombinationsStrategy(bool allowEmptySet, IEnumerable<TValue> possibleValues)
		{
			_allowEmptySet = allowEmptySet;
			if (possibleValues == null)
			{
				throw new ArgumentNullException("possibleValues");
			}
			_possibleValues = new List<TValue>(possibleValues).AsReadOnly();
			if (_possibleValues.Count >= sizeof(ulong) * 8)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture,
					"The given set has {0} values, which is more than the supported {1} values.", _possibleValues.Count, sizeof(ulong) * 8));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AllCombinationsStrategy&lt;TCollection, TValue&gt;"/> class.
		/// </summary>
		/// <param name="possibleValues">The possible values to combine.</param>
		/// <exception cref="ArgumentNullException"><paramref name="possibleValues"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="possibleValues"/> has more than 63 elements.</exception>
		public AllCombinationsStrategy(params TValue[] possibleValues)
			: this(false, (IEnumerable<TValue>)possibleValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AllCombinationsStrategy&lt;TCollection, TValue&gt;"/> class.
		/// </summary>
		/// <param name="possibleValues">The possible values to combine.</param>
		/// <exception cref="ArgumentNullException"><paramref name="possibleValues"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="possibleValues"/> has more than 63 elements.</exception>
		public AllCombinationsStrategy(IEnumerable<TValue> possibleValues)
			: this(false, possibleValues)
		{
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vectors/values from the (sub-)space.
		/// </returns>
		/// <remarks>
		/// Either <see cref="ExplorationStrategy&lt;T&gt;.Explore"/> or <see cref="ExplorationStrategy&lt;T&gt;.DynamicExplore"/> should be overriden.
		/// </remarks>
		public override IEnumerable<TCollection> Explore()
		{
			// Using a standard binary counting technique for getting all combinations: just imagine that you want all combinations
			// from a set of three elements, then you can do that by encoding that as a bit-string of three bits, and considering
			// that an element is included in the set iff the corresponding bit position has a 1 in it. So the bit string: 110
			// specifies that the first two elements are included, but the last one is not. going through all binary numbers that take
			// three bits goes through all combinations in this manner.
			// (We start with 0000 or 0001 depending on whether we allow the empty combination.)

			for (ulong i = AllowEmptySet ? 0uL : 1uL; i < ((ulong)1 << _possibleValues.Count); i++)
			{
				TCollection ret = new TCollection();
				for (int j = 0; j < _possibleValues.Count; j++)
				{
					if ((((ulong)1 << j) & i) != 0)
					{
						ret.Add(_possibleValues[j]);
					}
				}
				yield return ret;
			}
		}
	}
}
