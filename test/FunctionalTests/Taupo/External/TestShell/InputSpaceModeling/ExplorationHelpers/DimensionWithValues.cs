//---------------------------------------------------------------------
// <copyright file="DimensionWithValues.cs" company="Microsoft">
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
	/// An individual dimension with its values explored.
	/// </summary>
	public class DimensionWithValues
	{
		private QualifiedDimension _dimension;
		private List<ValueFactoryWithOptionalConcreteValue> _values;

		/// <summary>
		/// Initializes a new instance of the <see cref="DimensionWithValues"/> struct.
		/// </summary>
		/// <param name="dimension">The dimension.</param>
		/// <param name="values">The values.</param>
		public DimensionWithValues(QualifiedDimension dimension, IEnumerable<ValueFactoryWithOptionalConcreteValue> values)
		{
			_dimension = dimension;
			_values = new List<ValueFactoryWithOptionalConcreteValue>(values);
		}

		/// <summary>
		/// Gets the dimension.
		/// </summary>
		/// <value>The dimension.</value>
		public QualifiedDimension Dimension
		{
			get { return _dimension; }
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public ReadOnlyCollection<ValueFactoryWithOptionalConcreteValue> Values
		{
			get { return _values.AsReadOnly(); }
		}
	}
}
