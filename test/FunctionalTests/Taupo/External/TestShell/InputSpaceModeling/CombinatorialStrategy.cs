//---------------------------------------------------------------------
// <copyright file="CombinatorialStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Attempted to set exploration strategy for a dimension that is not in the target matrix.
	/// </summary>
	[Serializable]
	public class DimensionNotInMatrixException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionNotInMatrixException"/> class.
		/// </summary>
		public DimensionNotInMatrixException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionNotInMatrixException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DimensionNotInMatrixException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionNotInMatrixException"/> class.
		/// </summary>
		/// <param name="missingDimension">The missing dimension.</param>
		public DimensionNotInMatrixException(Dimension missingDimension)
			: base(string.Format(CultureInfo.InvariantCulture, "Dimension: {0} is not in the test matrix", missingDimension))
		{ }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionNotInMatrixException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public DimensionNotInMatrixException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionNotInMatrixException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected DimensionNotInMatrixException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Attempted to explore a combinatorial strategy without setting exploration strategies for all the dimensions.
	/// </summary>
	[Serializable]
	public class DimensionStrategyNotSetException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionStrategyNotSetException"/> class.
		/// </summary>
		public DimensionStrategyNotSetException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionStrategyNotSetException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DimensionStrategyNotSetException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionStrategyNotSetException"/> class.
		/// </summary>
		/// <param name="missingDimension">The missing dimension.</param>
		public DimensionStrategyNotSetException(Dimension missingDimension)
			: base(string.Format(CultureInfo.InvariantCulture, "Dimension: {0} doesn't have a corresponding exploration strategy", missingDimension))
		{ }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionStrategyNotSetException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public DimensionStrategyNotSetException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionStrategyNotSetException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected DimensionStrategyNotSetException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// An exploration strategy over a test matrix that independently chooses values from each dimension,
	/// then proceeds to choose a set of combinations of these values.
	/// </summary>
	public abstract class CombinatorialStrategy : ExplorationStrategy<Vector>
	{
		private Matrix _targetMatrix;
		private List<IConstraint> _constraints;
		private Dictionary<QualifiedDimension, IExplorationStrategy> _explorationStrategies = new Dictionary<QualifiedDimension, IExplorationStrategy>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CombinatorialStrategy"/> class.
		/// </summary>
		/// <param name="targetMatrix">The target matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		protected CombinatorialStrategy(Matrix targetMatrix, IEnumerable<IConstraint> constraints)
		{
			if (targetMatrix == null)
				throw new ArgumentNullException("targetMatrix");
			if (constraints == null)
				throw new ArgumentNullException("constraints");
			_targetMatrix = targetMatrix;
			_constraints = new List<IConstraint>(constraints);
			foreach (IConstraint c in _constraints)
			{
				if (c == null)
					throw new ArgumentNullException("constraints", "One of the constraints is null");
			}
			SetDefaultStrategies(Enumerable.Empty<Matrix>(), _targetMatrix);
		}

		private void SetDefaultStrategies(IEnumerable<Matrix> path, Matrix targetMatrix)
		{
			foreach (Dimension dim in targetMatrix.Dimensions)
			{
				Matrix asMatrix;
                Type underlyingType = Nullable.GetUnderlyingType(dim.Domain) ?? dim.Domain;
                if (underlyingType.IsEnum())
				{
					_explorationStrategies.Add(QualifiedDimension.Create(dim, path),
						(IExplorationStrategy)typeof(ExhaustiveEnumStrategy<>).MakeGenericType(dim.Domain).GetInstanceConstructor(true, new Type[0]).Invoke(null));
				}
				else if (typeof(bool).Equals(dim.Domain))
				{
					_explorationStrategies.Add(QualifiedDimension.Create(dim, path), new ExhaustiveIEnumerableStrategy<bool>(true, false));
				}
				else if (typeof(bool?).Equals(dim.Domain))
				{
					_explorationStrategies.Add(QualifiedDimension.Create(dim, path), new ExhaustiveIEnumerableStrategy<bool?>(true, false, null));
				}
				else if ((asMatrix = dim as Matrix) != null)
				{
					SetDefaultStrategies(path.Concat(Enumerable.Repeat(asMatrix, 1)), asMatrix);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CombinatorialStrategy"/> class as a copy of the given strategy.
		/// </summary>
		/// <param name="sourceStrategy">The source strategy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sourceStrategy"/> is null.</exception>
		protected CombinatorialStrategy(CombinatorialStrategy sourceStrategy)
		{
			if (sourceStrategy == null)
				throw new ArgumentNullException("sourceStrategy");
			_targetMatrix = sourceStrategy._targetMatrix;
			_constraints = new List<IConstraint>(sourceStrategy._constraints);
			ImportDimensionStrategies(sourceStrategy);
		}

		/// <summary>
		/// Gets the target matrix.
		/// </summary>
		/// <value>The target matrix.</value>
		public Matrix TargetMatrix
		{
			get { return _targetMatrix; }
		}

		/// <summary>
		/// The constraints that should be respected when exploring the matrix.
		/// </summary>
		/// <value>The constraints.</value>
		public Collection<IConstraint> Constraints
		{
			get { return new Collection<IConstraint>(_constraints); }
		}

		/// <summary>
		/// Sets the exploration strategy for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="strategy">The strategy. Can be null which will clear out the strategy for <paramref name="dimension"/>.</param>
		/// <remarks>
		/// Strategies should be set for all the dimensions in the test matrix;
		/// except for enum and bool dimensions, which are exhaustive by default.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		/// <exception cref="DimensionNotInMatrixException"><paramref name="dimension"/> is not in the target matrix.</exception>
		public void SetDimensionStrategy<T>(Dimension<T> dimension, ExplorationStrategy<T> strategy)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			if (!_targetMatrix.Dimensions.Contains(dimension))
				throw new DimensionNotInMatrixException(dimension);
			SetDimensionStrategy((QualifiedDimension<T>)dimension, strategy);
		}

		/// <summary>
		/// Sets the exploration strategy for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <param name="strategy">The strategy. Can be null which will clear out the strategy for <paramref name="dimension"/>.</param>
		/// <remarks>
		/// Strategies should be set for all the dimensions in the test matrix;
		/// except for enum and bool dimensions, which are exhaustive by default.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		/// <exception cref="DimensionNotInMatrixException"><paramref name="dimension"/> is not in the target matrix.</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the generic version for type safety")]
		public void SetDimensionStrategy<T>(QualifiedDimension<T> dimension, ExplorationStrategy<T> strategy)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			SetDimensionStrategyBase(dimension, strategy);
		}

        /// <summary>
        /// Sets the exploration strategy for the given dimension.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        /// <param name="strategy">The strategy. Can be null which will clear out the strategy for <paramref name="dimension"/>.</param>
        /// <remarks>
        /// Strategies should be set for all the dimensions in the test matrix;
        /// except for enum and bool dimensions, which are exhaustive by default.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
        /// <exception cref="DimensionNotInMatrixException"><paramref name="dimension"/> is not in the target matrix.</exception>
        public void SetDimensionStrategy(Dimension dimension, IExplorationStrategy strategy)
        {
            if (dimension == null)
                throw new ArgumentNullException("dimension");

            if (!_targetMatrix.Dimensions.Contains(dimension))
                throw new DimensionNotInMatrixException(dimension);

            SetDimensionStrategyBase((QualifiedDimension)dimension, strategy);
        }

		private void SetDimensionStrategyBase(QualifiedDimension dimension, IExplorationStrategy strategy)
		{
			Matrix currentMatrix = _targetMatrix;
			List<Matrix> innerPath = new List<Matrix>();
			foreach (Matrix innerMatrix in dimension.Path)
			{
				if (!currentMatrix.Dimensions.Contains(innerMatrix))
				{
					throw new DimensionNotInMatrixException(innerMatrix);
				}
				if (strategy != null)
				{
					_explorationStrategies.Remove(QualifiedDimension.Create(innerMatrix, innerPath));
				}
				currentMatrix = innerMatrix;
				innerPath.Add(innerMatrix);
			}
			if (strategy == null)
				_explorationStrategies.Remove(dimension);
			else if (_explorationStrategies.ContainsKey(dimension))
				_explorationStrategies[dimension] = strategy;
			else
				_explorationStrategies.Add(dimension, strategy);
		}

		/// <summary>
		/// Sets the exploration strategy for the given dimension to a categorical strategy with the given categories defined.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension"></param>
		/// <param name="allCategories"></param>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		/// <exception cref="DimensionNotInMatrixException"><paramref name="dimension"/> is not in the target matrix.</exception>
		/// <example><code><![CDATA[
		/// _fullStrategy.PartitionDimension(_dataTypeDimension,
		///	new PointCategory<PimodType>("String", PimodType.VarChar(20), PimodType.NVarChar(10), PimodType.Char(5), PimodType.NChar(50)),
		///	new PointCategory<PimodType>("Integer", PimodType.TinyInt(), PimodType.SmallInt(), PimodType.Int(), PimodType.BigInt()));
		///_fullStrategy.PartitionDimension(_tableSizeDimension,
		///	new PointCategory<int>("OneRow", 1),
		///	new IntegerRangeCategory("Small", 10, 20),
		///	new IntegerRangeCategory("Large", 10000, 15000));
		/// ]]></code></example>
		public void PartitionDimension<T>(Dimension<T> dimension, params Category<T>[] allCategories)
		{
			SetDimensionStrategy(dimension, new CategoricalStrategy<T>(allCategories));
		}

		/// <summary>
		/// Sets the exploration strategy for the given dimension to a categorical strategy with the given categories defined.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension"></param>
		/// <param name="allCategories"></param>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		/// <exception cref="DimensionNotInMatrixException"><paramref name="dimension"/> is not in the target matrix.</exception>
		/// <example><code><![CDATA[
		/// _fullStrategy.PartitionDimension(_dataTypeDimension,
		///	new PointCategory<PimodType>("String", PimodType.VarChar(20), PimodType.NVarChar(10), PimodType.Char(5), PimodType.NChar(50)),
		///	new PointCategory<PimodType>("Integer", PimodType.TinyInt(), PimodType.SmallInt(), PimodType.Int(), PimodType.BigInt()));
		///_fullStrategy.PartitionDimension(_tableSizeDimension,
		///	new PointCategory<int>("OneRow", 1),
		///	new IntegerRangeCategory("Small", 10, 20),
		///	new IntegerRangeCategory("Large", 10000, 15000));
		/// ]]></code></example>
		public void PartitionDimension<T>(QualifiedDimension<T> dimension, params Category<T>[] allCategories)
		{
			SetDimensionStrategy(dimension, new CategoricalStrategy<T>(allCategories));
		}

		/// <summary>
		/// Gets the current exploration strategy for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// The strategy set for the dimension, or null if none found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the generic version for type safety")]
		public ExplorationStrategy<T> GetDimensionStrategy<T>(QualifiedDimension<T> dimension)
		{
			return (ExplorationStrategy<T>)(GetBaseDimensionStrategy(dimension));
		}

		/// <summary>
		/// Gets the current exploration strategy for the given dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// The strategy set for the dimension, or null if none found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "I use the generic version for type safety")]
		public ExplorationStrategy<T> GetDimensionStrategy<T>(Dimension<T> dimension)
		{
			return (ExplorationStrategy<T>)(GetBaseDimensionStrategy(dimension));
		}

		/// <summary>
		/// (Internal usage) Gets the current exploration strategy for the given dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// The strategy set for the dimension, or null if none found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		public IExplorationStrategy GetBaseDimensionStrategy(Dimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			return GetBaseDimensionStrategy(QualifiedDimension.Create(dimension));
		}

		/// <summary>
		/// (Internal usage) Gets the current exploration strategy for the given dimension.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <returns>
		/// The strategy set for the dimension, or null if none found.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="dimension"/> is null.</exception>
		public IExplorationStrategy GetBaseDimensionStrategy(QualifiedDimension dimension)
		{
			if (dimension == null)
				throw new ArgumentNullException("dimension");
			IExplorationStrategy ret;
			if (_explorationStrategies.TryGetValue(dimension, out ret))
				return ret;
			else
				return null;
		}

		/// <summary>
		/// Imports the dimension exploration strategies from any common dimensions found with the given source combinatorial strategy.
		/// </summary>
		/// <remarks>
		/// If sourceStrategy has a test matrix with dimensions {A,B,C,D}, and it has known strategies for dimensions {A,B,C},
		/// and this strategy has dimensions {B,C,D,E}, then this method will import the strategies for dimensions {B,C}.
		/// </remarks>
		/// <param name="sourceStrategy"></param>
		/// <returns>The number of strategies imported.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="sourceStrategy"/> is null.</exception>
		public int ImportDimensionStrategies(CombinatorialStrategy sourceStrategy)
		{
			if (sourceStrategy == null)
				throw new ArgumentNullException("sourceStrategy");
			int ret = 0;
			foreach (KeyValuePair<QualifiedDimension, IExplorationStrategy> dimStrategy in sourceStrategy._explorationStrategies)
			{
				if (_explorationStrategies.ContainsKey(dimStrategy.Key))
				{
					_explorationStrategies[dimStrategy.Key] = dimStrategy.Value;
					ret++;
				}
				else if (_targetMatrix.Dimensions.Contains(dimStrategy.Key.FullPath.First()))
				{
					_explorationStrategies.Add(dimStrategy.Key, dimStrategy.Value);
					ret++;
				}
			}
			return ret;
		}

		/// <summary>
		/// Imports the dimension exploration strategies for the given innner matrix found with the given source combinatorial strategy.
		/// </summary>
		/// <param name="innerMatrix">The inner matrix.</param>
		/// <param name="sourceStrategy">The source strategy.</param>
		/// <remarks>
		/// This is useful for exploring dimensions from a sub-matrix in the same strategy, which is useful e.g. for doing pairwise coverage
		/// across sub-matrix boundaries.
		/// </remarks>
		/// <returns>The number of strategies imported.</returns>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		public int ImportSubMatrixStrategies(Matrix innerMatrix, CombinatorialStrategy sourceStrategy)
		{
			if (innerMatrix == null)
			{
				throw new ArgumentNullException("innerMatrix");
			}
			return ImportSubMatrixStrategies(new Matrix[] { innerMatrix }, sourceStrategy);
		}

		/// <summary>
		/// Imports the dimension exploration strategies for the given innner matrix found with the given source combinatorial strategy.
		/// </summary>
		/// <param name="innerMatrixPath">The inner matrix path.</param>
		/// <param name="sourceStrategy">The source strategy.</param>
		/// <remarks>
		/// This is useful for exploring dimensions from a sub-matrix in the same strategy, which is useful e.g. for doing pairwise coverage
		/// across sub-matrix boundaries.
		/// </remarks>
		/// <returns>The number of strategies imported.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="sourceStrategy"/> is null.</exception>
		public int ImportSubMatrixStrategies(IEnumerable<Matrix> innerMatrixPath, CombinatorialStrategy sourceStrategy)
		{
			if (sourceStrategy == null)
			{
				throw new ArgumentNullException("sourceStrategy");
			}
			if (innerMatrixPath == null || innerMatrixPath.Any(m => m == null))
			{
				throw new ArgumentNullException("innerMatrixPath");
			}
			int ret = 0;
			foreach (KeyValuePair<QualifiedDimension, IExplorationStrategy> dimStrategy in sourceStrategy._explorationStrategies)
			{
				SetDimensionStrategyBase(QualifiedDimension.Create(dimStrategy.Key.BaseDimension,
					innerMatrixPath.Concat(dimStrategy.Key.Path)), dimStrategy.Value);
				ret++;
			}
			_constraints.AddRange(sourceStrategy._constraints.Select<IConstraint, IConstraint>(c => new InnerSubMatrixWrapperConstraint(c, innerMatrixPath)));
			return ret;
		}

		/// <summary>
		/// Checks that all dimensions have exploration strategies.
		/// </summary>
		/// <exception cref="DimensionStrategyNotSetException">Any dimension found with no corresponding strategy.</exception>
		protected void CheckAllDimensionsHaveExplorationStrategies()
		{
			CheckAllDimensionsHaveExplorationStrategies(Enumerable.Empty<Matrix>(), _targetMatrix);
		}

		/// <summary>
		/// Checks that all dimensions have exploration strategies.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="targetMatrix">The target matrix.</param>
		/// <exception cref="DimensionStrategyNotSetException">Any dimension found with no corresponding strategy.</exception>
		private void CheckAllDimensionsHaveExplorationStrategies(IEnumerable<Matrix> path, Matrix targetMatrix)
		{
			foreach (Dimension dimension in targetMatrix.Dimensions)
			{
				if (!_explorationStrategies.ContainsKey(QualifiedDimension.Create(dimension, path)))
				{
					Matrix asMatrix;
					if ((asMatrix = dimension as Matrix) != null)
					{
						CheckAllDimensionsHaveExplorationStrategies(path.Concat(Enumerable.Repeat(asMatrix, 1)), asMatrix);
					}
					else
					{
						throw new DimensionStrategyNotSetException(dimension);
					}
				}
			}
		}

		private IEnumerable<ValueFactoryWithOptionalConcreteValue> GetValues(IExplorationStrategy strategy)
		{
			foreach (IValueFactory value in strategy.BaseExplore())
				yield return new ValueFactoryWithOptionalConcreteValue(value);
		}
		/// <summary>
		/// Gets all individual dimension values.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="DimensionStrategyNotSetException">Any dimension found with no corresponding strategy.</exception>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
			Justification = "Every invocation could yield different results.")]
		protected ReadOnlyCollection<DimensionWithValues> GetAllDimensionValues()
		{
			return GetAllDimensionValues(Enumerable.Empty<Matrix>(), _targetMatrix);
		}

		/// <summary>
		/// Gets all individual dimension values.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="targetMatrix">The target matrix.</param>
		/// <returns></returns>
		/// <exception cref="DimensionStrategyNotSetException">Any dimension found with no corresponding strategy.</exception>
		private ReadOnlyCollection<DimensionWithValues> GetAllDimensionValues(IEnumerable<Matrix> path, Matrix targetMatrix)
		{
			CheckAllDimensionsHaveExplorationStrategies();
			List<DimensionWithValues> ret = new List<DimensionWithValues>(targetMatrix.Dimensions.Count);
			foreach (Dimension dimension in targetMatrix.Dimensions)
			{
				QualifiedDimension QualifiedDimension = QualifiedDimension.Create(dimension, path);
				if (_explorationStrategies.ContainsKey(QualifiedDimension))
				{
					ret.Add(new DimensionWithValues(QualifiedDimension, GetValues(GetBaseDimensionStrategy(QualifiedDimension))));
				}
				else
				{
					Matrix asMatrix = (Matrix)dimension;
					ret.AddRange(GetAllDimensionValues(path.Concat(Enumerable.Repeat(asMatrix, 1)), asMatrix));
				}
			}
			return ret.AsReadOnly();
		}

		private class InnerSubMatrixWrapperConstraint : IConstraint
		{
			private readonly IConstraint _innerConstraint;
			private readonly ReadOnlyCollection<Matrix> _path;

			public InnerSubMatrixWrapperConstraint(IConstraint innerConstraint, IEnumerable<Matrix> path)
			{
				_path = new List<Matrix>(path).AsReadOnly();
				_innerConstraint = innerConstraint;
			}

			#region IConstraint Members

			public bool IsValid(Vector target)
			{
				Vector innerVector = target.GetValue(QualifiedDimension.Create(_path.Last(), _path.Take(_path.Count - 1)));
				return _innerConstraint.IsValid(innerVector);
			}

			public ReadOnlyCollection<QualifiedDimension> RequiredDimensions
			{
				get
				{
					return new List<QualifiedDimension>(_innerConstraint.RequiredDimensions
						.Select(d => QualifiedDimension.Create(d.BaseDimension, _path.Concat(d.Path)))
					).AsReadOnly();
				}
			}

			#endregion
		}

		/// <summary>
		/// Determines whether the specified target is valid according to the constraints.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns>
		/// 	<c>true</c> if the specified target is valid; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
		protected bool IsValidVector(Vector target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			return _constraints.All(c => c.IsValid(target));
		}
	}
}
