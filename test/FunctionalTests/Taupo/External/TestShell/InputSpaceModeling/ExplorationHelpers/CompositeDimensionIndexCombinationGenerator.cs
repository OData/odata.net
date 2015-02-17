//---------------------------------------------------------------------
// <copyright file="CompositeDimensionIndexCombinationGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Combines multiple <see cref="DimensionIndexCombinationGenerator"/> by concatenating their outputs.
	/// </summary>
	internal class CompositeDimensionIndexCombinationGenerator : DimensionIndexCombinationGenerator
	{
		private readonly ReadOnlyCollection<DimensionIndexCombinationGenerator> _underlyingGenerators;

		public CompositeDimensionIndexCombinationGenerator(IEnumerable<DimensionIndexCombinationGenerator> underlyingGenerators)
		{
			if (underlyingGenerators == null)
			{
				throw new ArgumentNullException("underlyingGenerators");
			}
			_underlyingGenerators = underlyingGenerators.ToList().AsReadOnly();
		}

		/// <summary>
		/// Generates the combinations.
		/// </summary>
		/// <returns>All the combinations generated.</returns>
		public override IEnumerable<ReadOnlyCollection<int>> Generate()
		{
			return _underlyingGenerators.SelectMany(g => g.Generate());
		}
	}
}
