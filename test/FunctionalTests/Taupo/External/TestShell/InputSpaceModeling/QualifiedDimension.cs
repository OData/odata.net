//---------------------------------------------------------------------
// <copyright file="QualifiedDimension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// A dimension that is potentially nested in an inner matrix (or multiple ones).
	/// </summary>
	/// <remarks>
	/// This is used to represent dimensions that are nested deep into a matrix. For example,
	/// let's say your top-level matrix has an inner matrix M as a dimension, which in turn has an
	/// inner matrix N, which has a dimension D. Then to represent dimension D as it relates to
	/// the top level matrix, we use
	/// <code>QualifiedDimension.Create(D, M, N);</code>.
	/// </remarks>
	public class QualifiedDimension
	{
		private readonly Dimension _actualDimension;
		private readonly ReadOnlyCollection<Matrix> _path;

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
		public ReadOnlyCollection<Matrix> Path
		{
			get { return _path; }
		}

		/// <summary>
		/// Gets the base actual dimension.
		/// </summary>
		/// <value>The base actual dimension.</value>
		internal Dimension BaseDimension
		{
			get { return _actualDimension; }
		}

		/// <summary>
		/// Gets the full path, which includes the actual dimension at the end.
		/// </summary>
		/// <value>The full path.</value>
		internal IEnumerable<Dimension> FullPath
		{
			get
			{
				return _path.Cast<Dimension>().Concat(Enumerable.Repeat(_actualDimension, 1));
			}
		}

		/// <summary>
		/// Gets the fully qualified name of dimension.
		/// </summary>
		/// <value>The the fully qualified name.</value>
		public string FullyQualifiedName
		{
			get
			{
				return string.Join(".", FullPath.Select(d => d.Name).ToArray());
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QualifiedDimension"/> class.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		protected QualifiedDimension(Dimension actualDimension, params Matrix[] path)
			: this(actualDimension, (IEnumerable<Matrix>)path)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QualifiedDimension"/> class.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		protected QualifiedDimension(Dimension actualDimension, IEnumerable<Matrix> path)
		{
			if (actualDimension == null)
			{
				throw new ArgumentNullException("actualDimension");
			}
			_actualDimension = actualDimension;
			if (path == null)
			{
				_path = new List<Matrix>().AsReadOnly();
			}
			else
			{
				List<Matrix> verifiedPath = new List<Matrix>();
				Matrix lastMatrix = null;
				foreach (Matrix currentMatrix in path)
				{
					if (currentMatrix == null)
					{
						throw new ArgumentNullException("path", "One of the matrixes in the path parameter is null.");
					}
					if (lastMatrix != null && !lastMatrix.Dimensions.Contains(currentMatrix))
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
							"The matrix {0} in the passed path is not a actually a dimension in matrix {1} preceding it",
							currentMatrix.Name, lastMatrix.Name), "path");
					}
					verifiedPath.Add(currentMatrix);
					lastMatrix = currentMatrix;
				}
				if (lastMatrix != null && !lastMatrix.Dimensions.Contains(actualDimension))
				{
					throw new DimensionNotInMatrixException(string.Format(CultureInfo.InvariantCulture,
						"The given dimension {0} is not actually a dimension in the last matrix in the path {1}",
						actualDimension.Name, lastMatrix.Name));
				}
				_path = verifiedPath.AsReadOnly();
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override bool Equals(object obj)
		{
			QualifiedDimension asQualifiedDimension;
			if ((asQualifiedDimension = obj as QualifiedDimension) == null)
			{
				return false;
			}
			return BaseDimension.Equals(asQualifiedDimension.BaseDimension) &&
				Path.SequenceEqual(asQualifiedDimension.Path);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return unchecked(BaseDimension.GetHashCode() + Path.Select(m => m.GetHashCode()).Sum());
		}

		/// <summary>
		/// Creates a qualified dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static QualifiedDimension<T> Create<T>(Dimension<T> actualDimension, params Matrix[] path)
		{
			return new QualifiedDimension<T>(actualDimension, path);
		}

		/// <summary>
		/// Creates a qualified dimension.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static QualifiedDimension Create(Dimension actualDimension, params Matrix[] path)
		{
			return new QualifiedDimension(actualDimension, path);
		}

		/// <summary>
		/// Creates a qualified dimension.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static QualifiedDimension<T> Create<T>(Dimension<T> actualDimension, IEnumerable<Matrix> path)
		{
			return new QualifiedDimension<T>(actualDimension, path);
		}

		/// <summary>
		/// Creates a qualified dimension.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static QualifiedDimension Create(Dimension actualDimension, IEnumerable<Matrix> path)
		{
			return new QualifiedDimension(actualDimension, path);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Dimension"/> to <see cref="QualifiedDimension"/>.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <returns>The result of the conversion.</returns>
		[SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
			Justification = "The Create method is an effective alternative.")]
		public static implicit operator QualifiedDimension(Dimension actualDimension)
		{
			return new QualifiedDimension(actualDimension);
		}
	}

	/// <summary>
	/// A dimension that is potentially nested in an inner matrix (or multiple ones).
	/// </summary>
	/// <remarks>
	/// This is used to represent dimensions that are nested deep into a matrix. For example,
	/// let's say your top-level matrix has an inner matrix M as a dimension, which in turn has an
	/// inner matrix N, which has a dimension D. Then to represent dimension D as it relates to
	/// the top level matrix, we use
	/// <code>QualifiedDimension.Create(D, M, N);</code>.
	/// </remarks>
	public sealed class QualifiedDimension<T> : QualifiedDimension
	{
		/// <summary>
		/// Gets the actual dimension.
		/// </summary>
		/// <value>The actual dimension.</value>
		public Dimension<T> Dimension
		{
			get { return (Dimension<T>)BaseDimension; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QualifiedDimension&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		internal QualifiedDimension(Dimension<T> actualDimension, IEnumerable<Matrix> path)
			: base(actualDimension, path)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QualifiedDimension&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <param name="path">The path.</param>
		internal QualifiedDimension(Dimension<T> actualDimension, params Matrix[] path)
			: this(actualDimension, (IEnumerable<Matrix>)path)
		{
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling.Dimension&lt;T&gt;"/>
		/// to <see cref="Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling.QualifiedDimension&lt;T&gt;"/>.
		/// </summary>
		/// <param name="actualDimension">The actual dimension.</param>
		/// <returns>The result of the conversion.</returns>
		[SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates",
			Justification = @"I get CA1000 if I try to implement the recommended FromDimension method (because this is a generic so it shouldn't have statics).
Anyway the constructor is a good named alternate.")]
		public static implicit operator QualifiedDimension<T>(Dimension<T> actualDimension)
		{
			return new QualifiedDimension<T>(actualDimension);
		}
	}
}
