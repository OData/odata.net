//---------------------------------------------------------------------
// <copyright file="Constraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A constraint on input test vectors that restricts the input space to exclude some invalid combinations.
	/// </summary>
	public interface IConstraint
	{
		/// <summary>
		/// Checks the input vector and returns true iff it doesn't violate this constraint.
		/// </summary>
		/// <param name="target">The target vector to validate.</param>
		/// <returns>
        /// 	<c>true</c> if the specified <paramref name="target"/> is valid; otherwise, <c>false</c>.
		/// </returns>
		bool IsValid(Vector target);

		/// <summary>
		/// The set of dimensions checked by this constraint (that should be present in the Vector given to IsValid).
		/// </summary>
		ReadOnlyCollection<QualifiedDimension> RequiredDimensions
		{
			get;
		}
	}
}
