//---------------------------------------------------------------------
// <copyright file="ExhaustiveCombinatorialStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A combinatorial exploration strategy that returns all possible combinations from the dimensions.
	/// </summary>
	public class ExhaustiveCombinatorialStrategy : CombinatorialStrategy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveCombinatorialStrategy"/> class.
		/// </summary>
		/// <param name="entireMatrix">The entire matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		public ExhaustiveCombinatorialStrategy(Matrix entireMatrix, IEnumerable<IConstraint> constraints)
			: base(entireMatrix, constraints)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveCombinatorialStrategy"/> class.
		/// </summary>
		/// <param name="entireMatrix">The entire matrix.</param>
		/// <param name="constraints">The constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the parameters is null.</exception>
		public ExhaustiveCombinatorialStrategy(Matrix entireMatrix, params IConstraint[] constraints)
			: base(entireMatrix, constraints)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveCombinatorialStrategy"/> class.
		/// </summary>
		/// <param name="sourceStrategy">The source strategy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sourceStrategy"/> is null.</exception>
		public ExhaustiveCombinatorialStrategy(CombinatorialStrategy sourceStrategy)
			: base(sourceStrategy)
		{
		}

		/// <summary>
		/// Explores the input (sub-)space. Each invocation may return a different set.
		/// </summary>
		/// <returns>
		/// All valid combinations of values from the dimensions.
		/// </returns>
		/// <exception cref="DimensionStrategyNotSetException">Any dimension found with no corresponding strategy.</exception>
		public override IEnumerable<Vector> Explore()
		{
			if (TargetMatrix.Dimensions.Count == 0)
				return Enumerable.Empty<Vector>(); // No dimensions to explore

			ReadOnlyCollection<DimensionWithValues> dimensionValues = GetAllDimensionValues();

			return CombinatorialUtilities.EnumerateAllVectors(dimensionValues).Where(IsValidVector);
		}
	}
}
