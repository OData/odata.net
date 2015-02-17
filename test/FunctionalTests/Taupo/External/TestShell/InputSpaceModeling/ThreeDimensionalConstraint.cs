//---------------------------------------------------------------------
// <copyright file="ThreeDimensionalConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
	/// <summary>
	/// Returns true iff the given values satisfy the constraint.
	/// </summary>
	/// <param name="firstValue">First of the tuple to validate.</param>
	/// <param name="secondValue">Second of the tuple to validate.</param>
	/// <param name="thirdValue">Third of the tuple to validate.</param>
	/// <typeparam name="TFirstDomainType">The type of values from the first dimension.</typeparam>
	/// <typeparam name="TSecondDomainType">The type of values from the second dimension.</typeparam>
	/// <typeparam name="TThirdDomainType">The type of values from the third dimension.</typeparam>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	public delegate bool ThreeDimensionalValidator<TFirstDomainType, TSecondDomainType, TThirdDomainType>(TFirstDomainType firstValue,
		TSecondDomainType secondValue, TThirdDomainType thirdValue);

	/// <summary>
	/// A constraint on the input space that checks combinations of three of the dimensions.
	/// </summary>
	/// <typeparam name="TFirstDomainType">The type of values from the first dimension.</typeparam>
	/// <typeparam name="TSecondDomainType">The type of values from the second dimension.</typeparam>
	/// <typeparam name="TThirdDomainType">The type of values from the third dimension.</typeparam>
	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
	public class ThreeDimensionalConstraint<TFirstDomainType, TSecondDomainType, TThirdDomainType> : IConstraint
	{
		private readonly QualifiedDimension<TFirstDomainType> _firstDimension;
		private readonly QualifiedDimension<TSecondDomainType> _secondDimension;
		private readonly QualifiedDimension<TThirdDomainType> _thirdDimension;
		private readonly ThreeDimensionalValidator<TFirstDomainType, TSecondDomainType, TThirdDomainType> _validator;
		private readonly ReadOnlyCollection<QualifiedDimension> _requiredDimensions;

		/// <summary>
		/// Initializes a new instance of the <see cref="TwoDimensionalConstraint&lt;FirstDomainType, SecondDomainType&gt;"/> class.
		/// </summary>
		/// <param name="firstDimension">The first dimension.</param>
		/// <param name="secondDimension">The second dimension.</param>
		/// <param name="thirdDimension">The third dimension.</param>
		/// <param name="validator">The validator method to use.</param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		public ThreeDimensionalConstraint(QualifiedDimension<TFirstDomainType> firstDimension,
			QualifiedDimension<TSecondDomainType> secondDimension, QualifiedDimension<TThirdDomainType> thirdDimension,
			ThreeDimensionalValidator<TFirstDomainType, TSecondDomainType, TThirdDomainType> validator)
		{
			if (firstDimension == null)
				throw new ArgumentNullException("firstDimension");
			if (secondDimension == null)
				throw new ArgumentNullException("secondDimension");
			if (thirdDimension == null)
				throw new ArgumentNullException("thirdDimension");
			if (validator == null)
				throw new ArgumentNullException("validator");
			_firstDimension = firstDimension;
			_secondDimension = secondDimension;
			_thirdDimension = thirdDimension;
			_validator = validator;
			_requiredDimensions = new List<QualifiedDimension>() { _firstDimension, _secondDimension, _thirdDimension }.AsReadOnly();
		}

		/// <summary>
		/// Special constructors for sub-classes that just want to override <see cref="IsValidValue"/> instead of providing a delegate.
		/// </summary>
		/// <param name="firstDimension">The first dimension.</param>
		/// <param name="secondDimension">The second dimension.</param>
		/// <param name="thirdDimension">The third dimension.</param>
		/// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
		protected ThreeDimensionalConstraint(QualifiedDimension<TFirstDomainType> firstDimension,
			QualifiedDimension<TSecondDomainType> secondDimension, QualifiedDimension<TThirdDomainType> thirdDimension)
		{
			if (firstDimension == null)
				throw new ArgumentNullException("firstDimension");
			if (secondDimension == null)
				throw new ArgumentNullException("secondDimension");
			if (thirdDimension == null)
				throw new ArgumentNullException("thirdDimension");
			_firstDimension = firstDimension;
			_secondDimension = secondDimension;
			_thirdDimension = thirdDimension;
			_validator = null;
			_requiredDimensions = new List<QualifiedDimension>() { _firstDimension, _secondDimension, _thirdDimension }.AsReadOnly();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2},{3})", GetType().Name,
				_firstDimension.Dimension.Name, _secondDimension.Dimension.Name, _thirdDimension.Dimension.Name);
		}

		/// <summary>
		/// Gets the first dimension.
		/// </summary>
		/// <value>The first dimension.</value>
		public QualifiedDimension<TFirstDomainType> FirstDimension
		{
			get { return _firstDimension; }
		}

		/// <summary>
		/// Gets the second dimension.
		/// </summary>
		/// <value>The second dimension.</value>
		public QualifiedDimension<TSecondDomainType> SecondDimension
		{
			get { return _secondDimension; }
		}

		/// <summary>
		/// Gets the third dimension.
		/// </summary>
		/// <value>The third dimension.</value>
		public QualifiedDimension<TThirdDomainType> ThirdDimension
		{
			get { return _thirdDimension; }
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
			return IsValidValue(target.GetValue(_firstDimension), target.GetValue(_secondDimension), target.GetValue(_thirdDimension));
		}

		/// <summary>
		/// Checks the given values if they satisfy the constraint.
		/// </summary>
		/// <param name="firstDimensionValue">The first dimension value.</param>
		/// <param name="secondDimensionValue">The second dimension value.</param>
		/// <param name="thirdDimensionValue">The third dimension value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <parmref name="value"/> is valid; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// The default implementation uses the validator delegate, but derived classes can override this behavior if they want.
		/// </remarks>
		protected virtual bool IsValidValue(TFirstDomainType firstDimensionValue,
			TSecondDomainType secondDimensionValue, TThirdDomainType thirdDimensionValue)
		{
			return _validator(firstDimensionValue, secondDimensionValue, thirdDimensionValue);
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
