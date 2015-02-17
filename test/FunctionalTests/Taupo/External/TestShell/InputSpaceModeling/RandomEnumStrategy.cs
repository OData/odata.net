//---------------------------------------------------------------------
// <copyright file="RandomEnumStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy for Enum types that returns one (or a fixed number of) random value(s) from this enum each time.
	/// </summary>
	/// <typeparam name="T">The Enum type of the dimension.</typeparam>
	public class RandomEnumStrategy<T> : ExplorationStrategy<T>
	{
		private readonly int _numberOfValuesToReturn;
        private readonly Func<int, int> _nextInt;

		private class ValueFactory : IValueFactory<T>
		{
            Func<int, int> _nextInt;
            public ValueFactory(Func<int, int> nextInt)
            {
                _nextInt = nextInt;
            }
            
            public T GetValue()
			{
                return (T)(RandomUtilities.ChooseRandomEnumValue(typeof(T), _nextInt));
			}

			public override string ToString()
			{
				return "Random enum value(" + typeof(T).Name + ")";
			}

			object IValueFactory.GetBaseValue()
			{
				return GetValue();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomEnumStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an enum.</exception>
		public RandomEnumStrategy(Func<int, int> nextInt)
            : this(1, nextInt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomEnumStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="numberOfValuesToReturn">The fixed number of random values to return each time (deafult is 1).</param>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an enum.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfValuesToReturn"/> is less than 1.</exception>
		public RandomEnumStrategy(int numberOfValuesToReturn, Func<int, int> nextInt)
		{
			if (!typeof(Enum).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not an enum", typeof(T).FullName));
			}
			if (numberOfValuesToReturn < 1)
			{
				throw new ArgumentOutOfRangeException("numberOfValuesToReturn", "numberOfValuesToReturn should be a positive number.");
			}
            if (nextInt == null)
            {
                throw new ArgumentNullException("nextInt");
            }

			_numberOfValuesToReturn = numberOfValuesToReturn;
            _nextInt = nextInt;
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// A (reasonably-sized) stream of vector/value-factories from the (sub-)space.
		/// </returns>
		public override IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			for (int i = 0; i < _numberOfValuesToReturn; i++)
			{
				yield return new ValueFactory(_nextInt);
			}
		}
	}
}
