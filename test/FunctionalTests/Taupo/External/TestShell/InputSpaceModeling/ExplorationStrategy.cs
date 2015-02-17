//---------------------------------------------------------------------
// <copyright file="ExplorationStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// (Internal usage) Base interface for non-generic exploration strategies.
	/// </summary>
	public interface IExplorationStrategy
	{
		/// <summary>
		/// (Internal usage) Retrieves a set of values to use for the dimension in a non-generic fashion.
		/// </summary>
		/// <returns>The stream of values to use.</returns>
		IEnumerable<IValueFactory> BaseExplore();
	}

	/// <summary>
	/// A way to explore the input (sub-)space to yield a reasonable set of vectors/values to run.
	/// </summary>
	/// <typeparam name="T">The type of values returned.</typeparam>
	/// <remarks>
	/// In an input-space model, each <see cref="Dimension&lt;T&gt;"/> object is usually associated with one
	/// (or more) <see cref="ExplorationStrategy&lt;T&gt;"/> object that specify for each test run what values to
	/// choose from the dimension as test cases. Scalar dimensions (e.g. TableSize) would usually have a
	/// <see cref="CategoricalStrategy&lt;T&gt;"/> object to split them into categories (equivalence classes)
	/// or an <see cref="ExhaustiveEnumStrategy&lt;T&gt;"/> if the dimension simply enumerates over a small
	/// <see cref="Enum"/> type. More complex spaces (<see cref="Matrix"/> objects) are usually associated
	/// with a <see cref="CombinatorialStrategy"/> to yield various combinations as test cases.
	/// </remarks>
	public abstract class ExplorationStrategy<T> : IExplorationStrategy
	{
		private ReadOnlyCollection<ExplorationStrategy<T>> _wrappedStrategies;
		private bool _exploreOverriden = true;
		private bool _dynamicExploreOverriden = true;
        
		/// <summary>
		/// (Internal usage) Retrieves a set of values to use for the dimension in a non-generic fashion.
		/// </summary>
		/// <remarks>
		/// This just iterates over the generic Explore method and returns the values from there.
		/// </remarks>
		/// <returns></returns>
		public IEnumerable<IValueFactory> BaseExplore()
		{
			foreach (IValueFactory current in DynamicExplore())
				yield return current;
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>A (reasonably-sized) stream of vectors/values from the (sub-)space.</returns>
		/// <remarks>
		/// Either <see cref="ExplorationStrategy&lt;T&gt;.Explore"/> or <see cref="ExplorationStrategy&lt;T&gt;.DynamicExplore"/> should be overriden.
		/// </remarks>
		public virtual IEnumerable<T> Explore()
		{
			_exploreOverriden = false;
			if (!_dynamicExploreOverriden)
			{
                throw new InvalidOperationException("You must override either Explore or Dynamic Explore");
			}
			foreach (IValueFactory<T> current in DynamicExplore())
			{
				yield return current.GetValue();
			}
		}

		/// <summary>
		/// Explores the input (sub-)space to give (potentially dynamic) value factories. Each invocation may return a different set.
		/// </summary>
		/// <returns>A (reasonably-sized) stream of vector/value-factories from the (sub-)space.</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public virtual IEnumerable<IValueFactory<T>> DynamicExplore()
		{
			_dynamicExploreOverriden = false;
			if (!_exploreOverriden)
			{
                throw new InvalidOperationException("You must override either Explore or Dynamic Explore");
			}
			foreach (T current in Explore())
			{
				yield return new SingleValueFactory<T>(current);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExplorationStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="wrappedStrategies">(Optional) The wrapped strategies.</param>
		protected ExplorationStrategy(params ExplorationStrategy<T>[] wrappedStrategies)
		{
			if (wrappedStrategies == null)
			{
				throw new ArgumentNullException("wrappedStrategies");
			}
			_wrappedStrategies = new List<ExplorationStrategy<T>>(wrappedStrategies).AsReadOnly();
		}

		/// <summary>
		/// Gets the wrapped strategies, if any.
		/// </summary>
		/// <value>The wrapped strategies.</value>
		/// <remarks>
		/// Strategies can wrap other strategies, e.g. to filter them or do other operations on their values.
		/// If that is the case, then the wrapped strategies are given in this collection.
		/// For an example of a wrapper strategy, see <see cref="ShuffleStrategy&lt;T&gt;"/>
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
			Justification = "This is an advanced property that normally wouldn't be used by people outside Shell.")]
		public ReadOnlyCollection<ExplorationStrategy<T>> WrappedStrategies
		{
			get { return _wrappedStrategies; }
		}

	}
}
