//---------------------------------------------------------------------
// <copyright file="FilteredDimensionIndexCombinationGenerator.cs" company="Microsoft">
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
	/// Applies a predicate to the output of a <see cref="DimensionIndexCombinationGenerator"/> and returns only those
	/// combinations that pass the predicate.
	/// </summary>
	internal class FilteredDimensionIndexCombinationGenerator : DimensionIndexCombinationGenerator
	{
		private readonly DimensionIndexCombinationGenerator _underlyingGenerator;
		private readonly Func<ReadOnlyCollection<int>, bool> _filter;

		public FilteredDimensionIndexCombinationGenerator(DimensionIndexCombinationGenerator underlyingGenerator,
			Func<ReadOnlyCollection<int>, bool> filter)
		{
			if (underlyingGenerator == null)
			{
				throw new ArgumentNullException("underlyingGenerator");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			_underlyingGenerator = underlyingGenerator;
			_filter = filter;
		}

		public override IEnumerable<ReadOnlyCollection<int>> Generate()
		{
			return _underlyingGenerator.Generate().Where(_filter);
		}
	}
}
