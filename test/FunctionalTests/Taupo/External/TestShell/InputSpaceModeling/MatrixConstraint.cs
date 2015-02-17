//---------------------------------------------------------------------
// <copyright file="MatrixConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A wrapper constraint to wrap a list of matrix constraints when a matrix is embedded as a dimension into a parent matrix.
	/// </summary>
	/// <remarks>
	/// Since ISM models can be hierarchical (i.e. a Matrix can be embedded as a dimension into a higher level Matrix),
	/// users need a way propagate constraints up the hierarchy. <see cref="MatrixConstraint"/> provides a simple way to accomplish
	/// this by encapsulating all the constraints that apply to the inner matrix as a one-dimensional constraint in the outer matrix
	/// (since as far as the outer matrix is concerned, the inner matrix is just a single dimension).
	/// </remarks>
	/// <example><code><![CDATA[
	/// // Define the dimensions for one column
	/// Dimension<SqlDataType> dataTypeDimension = new Dimension<SqlDataType>("DataType");
	/// Dimension<bool> isNullDimension = new Dimension<bool>("IsNull");
	/// Dimension<bool> isUniqueDimension = new Dimension<bool>("IsUnique");
	/// // Define the constraints for the column dimensions
	/// IConstraint timestampNotNullable = new TwoDimensionalConstraint<SqlDataType, bool>(dataTypeDimension, isNullDimension,
	/// 	delegate(SqlDataType type, bool isNull)
	/// 	{
	/// 		if (type == SqlDataType.Timestamp)
	/// 			return !isNull;
	/// 		else
	/// 			return true;
	/// 	});
	/// IConstraint xmlNotUnique = new TwoDimensionalConstraint<SqlDataType, bool>(dataTypeDimension, isUniqueDimension,
	/// 	delegate(SqlDataType type, bool isUnique)
	/// 	{
	/// 		if (type == SqlDataType.Xml)
	/// 			return !isUnique;
	/// 		else
	/// 			return true;
	/// 	});
	/// 
	/// // Contain the dimensions for a column in a sub-matrix
	/// Matrix firstColumnMatrix = new Matrix("FirstColumn", dataTypeDimension, isNullDimension, isUniqueDimension);
	/// // Clone the matrix for a second column
	/// Matrix secondColumnMatrix = (Matrix)firstColumnMatrix.Clone("SecondColumn");
	/// // Define other dimensions we need in the overall matrix
	/// Dimension<int> tableSizeDimension = new Dimension<int>("TableSize");
	/// // Create my model
	/// Matrix overallModel = new Matrix(firstColumnMatrix, secondColumnMatrix, tableSizeDimension);
	/// // Create my constraints
	/// List<IConstraint> modelConstraints = new List<IConstraint>();
	/// // Encapsulate the constraints for my inner matrixes
	/// modelConstraints.Add(new MatrixConstraint(firstColumnMatrix, timestampNotNullable, xmlNotUnique));
	/// modelConstraints.Add(new MatrixConstraint(secondColumnMatrix, timestampNotNullable, xmlNotUnique));
	/// ]]></code></example>
	public class MatrixConstraint : OneDimensionalConstraint<Vector>
	{
		private readonly ReadOnlyCollection<IConstraint> _matrixConstraints;

		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixConstraint"/> class.
		/// </summary>
		/// <param name="targetMatrix">The target matrix.</param>
		/// <param name="matrixConstraints">The matrix constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		public MatrixConstraint(Matrix targetMatrix, IEnumerable<IConstraint> matrixConstraints)
			: base(targetMatrix)
		{
			if (matrixConstraints == null)
				throw new ArgumentNullException("matrixConstraints");
			_matrixConstraints = new List<IConstraint>(matrixConstraints).AsReadOnly();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixConstraint"/> class.
		/// </summary>
		/// <param name="targetMatrix">The target matrix.</param>
		/// <param name="matrixConstraints">The matrix constraints.</param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		public MatrixConstraint(Matrix targetMatrix, params IConstraint[] matrixConstraints)
			: this(targetMatrix, (IEnumerable<IConstraint>)matrixConstraints)
		{
		}

		/// <summary>
		/// Gets the matrix constraints encapsulated here.
		/// </summary>
		/// <value>The matrix constraints.</value>
		public ReadOnlyCollection<IConstraint> MatrixConstraints
		{
			get { return _matrixConstraints; }
		} 

		/// <summary>
		/// Checks the given value if it satisfies the constraint.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <parmref name="value"/> is valid; otherwise, <c>false</c>.
		/// </returns>
		protected override bool IsValidValue(Vector value)
		{
			foreach (IConstraint constraint in _matrixConstraints)
				if (!constraint.IsValid(value))
					return false;
			return true;
		}
	}
}
