//---------------------------------------------------------------------
// <copyright file="OneDimensionalConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Returns true iff the given value satisfies the constraint.
	/// </summary>
	/// <param name="value">The value to validate.</param>
	/// <typeparam name="T">The type of values from the dimension.</typeparam>
	public delegate bool OneDimensionalValidator<T>(T value);

	/// <summary>
	/// A constraint on the input space that checks the value of a single dimension.
	/// </summary>
	/// <typeparam name="T">The type of values from the dimension.</typeparam>
	/// <remarks>
	/// Typically, you wouldn't want to use a one dimensional constraint.
	/// Instead, you should adjust the ExplorationStrategy for your dimension to just return the valid values.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// OneDimensionalConstraint<string> shortColumnNameConstraint = new OneDimensionalConstraint<string>(columnNameDimension,
	///		delegate(string v) { return v.Length < 128; });
	/// ]]></code>
	/// </example>
	public class OneDimensionalConstraint<T> : IConstraint
	{
		private readonly QualifiedDimension<T> _targetDimension;
		private readonly OneDimensionalValidator<T> _validator;
		private readonly ReadOnlyCollection<QualifiedDimension> _requiredDimensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="OneDimensionalConstraint&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="targetDimension">The target dimension.</param>
		/// <param name="validator">The validator.</param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		public OneDimensionalConstraint(QualifiedDimension<T> targetDimension, OneDimensionalValidator<T> validator)
		{
			if (targetDimension == null)
				throw new ArgumentNullException("targetDimension");
			if (validator == null)
				throw new ArgumentNullException("validator");
			_targetDimension = targetDimension;
			_validator = validator;
			_requiredDimensions = new List<QualifiedDimension>() { _targetDimension }.AsReadOnly();
		}

		/// <summary>
		/// Special constructors for sub-classes that just want to override <see cref="IsValidValue"/> instead of providing a delegate.
		/// </summary>
		/// <param name="targetDimension"></param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		protected OneDimensionalConstraint(QualifiedDimension<T> targetDimension)
		{
			if (targetDimension == null)
				throw new ArgumentNullException("targetDimension");
			_targetDimension = targetDimension;
			_validator = null;
			_requiredDimensions = new List<QualifiedDimension>() { _targetDimension }.AsReadOnly();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", GetType().Name, _targetDimension.Dimension.Name);
		}

		/// <summary>
		/// Gets the target dimension.
		/// </summary>
		/// <value>The target dimension.</value>
		public QualifiedDimension<T> TargetDimension
		{
			get { return _targetDimension; }
		}

		/// <summary>
		/// Checks the input vector and returns true iff it doesn't violate this constraint.
		/// </summary>
		/// <param name="target">The target vector to validate.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <parmref name="target"/> is valid; otherwise, <c>false</c>.
		/// </returns>
		public bool IsValid(Vector target)
		{
			return IsValidValue(target.GetValue(_targetDimension));
		}

		/// <summary>
		/// Checks the given value if it satisfies the constraint.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <parmref name="value"/> is valid; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// The default implementation uses the validator delegate, but derived classes can override this behavior if they want.
		/// </remarks>
		protected virtual bool IsValidValue(T value)
		{
			return _validator(value);
		}

		/// <summary>
		/// The set of dimensions checked by this constraint (that should be present in the Vector given to IsValid).
		/// </summary>
		public ReadOnlyCollection<QualifiedDimension> RequiredDimensions
		{
			get
			{
				return _requiredDimensions;
			}
		}
	}
}
