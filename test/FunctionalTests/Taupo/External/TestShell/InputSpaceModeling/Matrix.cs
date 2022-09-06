//---------------------------------------------------------------------
// <copyright file="Matrix.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Dimensions in the test matrix have the same name.
	/// </summary>
    [Serializable]
	public class DuplicateDimensionNameException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateDimensionNameException"/> class.
		/// </summary>
		public DuplicateDimensionNameException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateDimensionNameException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public DuplicateDimensionNameException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicateDimensionNameException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="inner">The inner.</param>
		public DuplicateDimensionNameException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
		/// Initializes a new instance of the <see cref="DuplicateDimensionNameException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected DuplicateDimensionNameException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// A multi-dimensional input space.
	/// </summary>
	/// <remarks>
	/// The Matrix class extends Dimension so that it can be incorporated as a dimension into a bigger input space.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Dimension<int> tableSizeDimension = new Dimension<int>("TableSize");
	/// Dimension<SqlDataType> dataTypeDimension = new Dimension<SqlDataType>("DataType");
	/// Matrix firstTableDimension = new Matrix("FirstTable", tableSizeDimension, dataTypeDimension);
	/// Matrix secondTableDimension = (Matrix)firstTableDimension.Clone("SecondTable");
	/// Matrix entireTestMatrix = new Matrix(firstTableDimension, secondTableDimension);
	/// ]]></code>
	/// </example>
	public class Matrix : Dimension<Vector>
	{
		// DEVNOTE: I'm OK with the default behavior of KeyedCollection in this case (extracting the name at
		// construction time) since the Name in Dimension is a read-only property.
		private class KeyedByNameDimensionCollection : KeyedCollection<string, Dimension>
		{
			protected override string GetKeyForItem(Dimension item)
			{
				return item.Name;
			}
		}

		// All the dimensions keyed on their name
		private KeyedByNameDimensionCollection _dimensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix"/> class.
		/// </summary>
		/// <param name="matrixDimensions">The matrix dimensions.</param>
		/// <exception cref="DuplicateDimensionNameException">Any of the dimensions have the same name.</exception>
		/// <exception cref="ArgumentNullException">Any of the dimensions is null.</exception>
		public Matrix(params Dimension[] matrixDimensions)
			: this(GenerateDimensionName(matrixDimensions), matrixDimensions)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Matrix"/> class.
		/// </summary>
		/// <param name="name">The name of the matrix.</param>
		/// <param name="matrixDimensions">The matrix dimensions.</param>
		public Matrix(string name, params Dimension[] matrixDimensions)
			: base(name)
		{
			_dimensions = new KeyedByNameDimensionCollection();
			foreach (Dimension currentDimension in matrixDimensions)
			{
				if (currentDimension == null)
					throw new ArgumentNullException("matrixDimensions");
				if (_dimensions.Contains(currentDimension.Name))
				{
					throw new DuplicateDimensionNameException(string.Format(CultureInfo.InvariantCulture,
						"Dimensions have the same name: {0}", currentDimension.Name));
				}
				_dimensions.Add(currentDimension);
			}
		}

		/// <summary>
		/// Creates a deep copy of the matrix with a new name.
		/// </summary>
		/// <param name="newName">The name of the clone.</param>
		/// <returns>The new copied matrix.</returns>
		public override Dimension Clone(string newName)
		{
			Dimension[] dimCopies = new Dimension[_dimensions.Count];
			for (int i = 0; i < _dimensions.Count; i++)
				dimCopies[i] = _dimensions[i].Clone(_dimensions[i].Name);
			return new Matrix(newName, dimCopies);
		}

		/// <summary>
		/// The readonly collection of dimensions in the space.
		/// </summary>
		public ReadOnlyCollection<Dimension> Dimensions
		{
			get { return new ReadOnlyCollection<Dimension>(_dimensions); }
		}

		private static string GenerateDimensionName(Dimension[] matrixDimensions)
		{
			StringBuilder ret = new StringBuilder();
			ret.Append("{");
			bool putSemicolon = false;
			foreach (Dimension dim in matrixDimensions)
			{
				if (putSemicolon)
					ret.Append(";");
				putSemicolon = true;
				ret.Append(dim.Name);
			}
			ret.Append("}");
			return ret.ToString();
		}
	}
}
