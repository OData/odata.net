//---------------------------------------------------------------------
// <copyright file="Dimension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An orthogonal parameter in the input space.
	/// </summary>
	public abstract class Dimension
	{
		/// <summary>
		/// Name of the dimension. Should be unique across the input space.
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// The type of values allowed in this dimension.
		/// </summary>
		public abstract Type Domain
		{
			get;
		}

		/// <summary>
		/// Creates a deep copy of this dimension with a new name.
		/// </summary>
		/// <param name="newName"></param>
		/// <returns></returns>
		public abstract Dimension Clone(string newName);
	}

	/// <summary>
	/// An orthogonal parameter in the input space.
	/// </summary>
	/// <typeparam name="T">The type of values allowed in this dimension.</typeparam>
	/// <remarks>
	/// If <typeparamref name="T"/> is not a standard or primitive type, please make sure to provide a
	/// <see cref="System.ComponentModel.TypeConverter"/> for it so that the runtime can convert it
	/// to/from strings. Example of this:
	/// <code><![CDATA[
	///[TypeConverter(typeof(MyConverter))]
	///class SomeClassWithConverter
	///{
	///  private int _x;
	///
	///  public int X { get { return _x; } }
	///
	///  public SomeClassWithConverter(int x) { _x = x; }
	///}
	///
	///class MyConverter : TypeConverter
	///{
	///  public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	///  {
	///    string valueAsString = value as string;
	///    if (valueAsString != null)
	///    {
	///      return new SomeClassWithConverter(Int32.Parse(valueAsString, culture));
	///    }
	///    return base.ConvertFrom(context, culture, value);
	///  }
	///
	///  public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	///  {
	///    if (destinationType.Equals(typeof(string)))
	///    {
	///      return ((SomeClassWithConverter)value).X.ToString(culture);
	///    }
	///    return base.ConvertTo(context, culture, value, destinationType);
	///  }
	///}
	/// ]]></code>
	/// </remarks>
	public class Dimension<T> : Dimension
	{
		private readonly string _name;

		/// <summary>
		/// Name of the dimension. Should be unique across the input space.
		/// </summary>
		/// <value></value>
		public override string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// The type of values allowed in this dimension.
		/// </summary>
		/// <value></value>
		public override Type Domain
		{
			get { return typeof(T); }
		}

		/// <summary>
		/// Creates a deep copy of this dimension with a new name.
		/// </summary>
		/// <param name="newName"></param>
		/// <returns></returns>
		public override Dimension Clone(string newName)
		{
			return new Dimension<T>(newName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Dimension&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The name of the dimension. Ideally should be unique across the test matrix.</param>
		public Dimension(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Constructs a dimension-value pair for this dimension.
		/// </summary>
		/// <param name="dimensionValue">The value.</param>
		/// <returns>The dimension-value pair.</returns>
		public DimensionValuePair<T> Value(T dimensionValue)
		{
			return new DimensionValuePair<T>(this, dimensionValue);
		}
	}
}
