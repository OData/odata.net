//---------------------------------------------------------------------
// <copyright file="ExhaustiveEnumStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// An exploration strategy for Enum types that simply returns all the possible values in this enum.
    /// For nullable enum types null value is considered as one of the possible values.
	/// </summary>
	/// <typeparam name="T">The Enum type of the dimension.</typeparam>
	public class ExhaustiveEnumStrategy<T> : ExplorationStrategy<T>
	{
        private bool isNullable;
        private Type enumType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExhaustiveEnumStrategy&lt;T&gt;"/> class.
		/// </summary>
		/// <exception cref="ArgumentException"><typeparamref name="T"/> is not an enum or nullable enum.</exception>
		public ExhaustiveEnumStrategy()
		{
            Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (!underlyingType.IsEnum())
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not an enum", typeof(T).FullName));
            }

            this.enumType = underlyingType;
            this.isNullable = underlyingType != typeof(T);
		}

		/// <summary>
		/// Explores the input (sub-)space.
		/// </summary>
		/// <returns>
		/// Enumeration over all the values in the Enum.
		/// </returns>
		public override IEnumerable<T> Explore()
		{
			// Note: Silverlight does not support Enum.GetValues(type)
            foreach (object v in Utilities.GetEnumValues(this.enumType))
				yield return (T)v;

            if (this.isNullable)
            {
                yield return default(T);
            }
		}
	}
}
