//---------------------------------------------------------------------
// <copyright file="TranslatedDimensionIndexCombinationGenerator.cs" company="Microsoft">
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
	/// Takes a combination generator on indexes 0..n and translates them using a translation vector [i0..in].
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// new TranslatedDimensionIndexCombinationGenerator(new PrimitiveDimensionIndexCombinationGenerator(3, 2), new List<int> { 2, 6, 3 }.AsReadOnly())
	/// ]]>
	/// </code>
	/// would yield {2, 6}, {2, 3}, {6, 3}
	/// </example>
	internal class TranslatedDimensionIndexCombinationGenerator : DimensionIndexCombinationGenerator
	{
		private readonly DimensionIndexCombinationGenerator _underlyingGenerator;
		private readonly ReadOnlyCollection<int> _translatedIndexes;

		public TranslatedDimensionIndexCombinationGenerator(DimensionIndexCombinationGenerator underlyingGenerator,
			ReadOnlyCollection<int> translatedIndexes)
		{
			if (underlyingGenerator == null)
			{
				throw new ArgumentNullException("underlyingGenerator");
			}
			if (translatedIndexes == null)
			{
				throw new ArgumentNullException("translatedIndexes");
			}
			_underlyingGenerator = underlyingGenerator;
			_translatedIndexes = translatedIndexes;
		}

		public override IEnumerable<ReadOnlyCollection<int>> Generate()
		{
			return _underlyingGenerator
				.Generate()
				.Select(seq => seq
					.Select(i => _translatedIndexes[i])
					.ToList()
					.AsReadOnly()
				);
		}
	}
}
