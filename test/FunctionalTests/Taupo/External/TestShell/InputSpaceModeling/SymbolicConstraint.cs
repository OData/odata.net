//---------------------------------------------------------------------
// <copyright file="SymbolicConstraint.cs" company="Microsoft">
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
	/// A constraint on the input space expressed as a symbolic expression.
	/// </summary>
	public class SymbolicConstraint : IConstraint
	{
		/// <summary>
		/// Initializes a symbolic constraint and its associated context.
		/// </summary>
		/// <param name="constraint">symbolic constraint.</param>
		/// <param name="matrix">context.</param>
		public SymbolicConstraint(Expression constraint, Matrix matrix)
		{
			if ((object)constraint == null)
			{
				throw new ArgumentNullException("constraint");
			}

			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}

			_constraint = constraint;
			_matrix = matrix;

			_requiredDimensions = new List<QualifiedDimension>();
			_dimensions = new Dictionary<string, Dimension>();

			foreach (ExpressionIdentifier identifier in _constraint.Identifiers())
			{
				bool foundDimension = false;

				foreach (Dimension dimension in _matrix.Dimensions)
				{
					if (identifier.Name == dimension.Name)
					{
						_dimensions.Add(identifier.Name, dimension);
						_requiredDimensions.Add(dimension);
						foundDimension = true;
						break;
					}
				}

				if (!foundDimension)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0} not a dimension name in the matrix", identifier.Name));
				}
			}
		}

		/// <summary>
		/// Evaluate the constraint
		/// </summary>
		/// <param name="target">The target vector to evaluate.</param>
		/// <returns></returns>
		public bool IsValid(Vector target)
		{
			if (target == null)
			{
				return false;
			}

			return _constraint.LogicalEvaluate(
				delegate(ExpressionIdentifier identifier)
				{
					Dimension dimension = _dimensions[identifier.Name];

					if (dimension.Domain == typeof(int))
					{
						return ExpressionConstant.Constant(target.GetValue<int>((Dimension<int>)dimension));
					}
					else if (dimension.Domain == typeof(bool))
					{
						return ExpressionConstant.Constant(target.GetValue<bool>((Dimension<bool>)dimension));
					}
					else if (dimension.Domain == typeof(string))
					{
						return ExpressionConstant.Constant(target.GetValue<string>((Dimension<string>)dimension));
					}
					else
					{
						throw new InvalidOperationException("Invalid type");
					}
				});
		}

		/// <summary>
		/// The set of dimensions checked by this constraint (that should be present in the Vector given to IsValid).
		/// </summary>
		public ReadOnlyCollection<QualifiedDimension> RequiredDimensions
		{
			get { return _requiredDimensions.AsReadOnly(); }
		}

		/// <summary>
		/// The actual symbolic constraint expression.
		/// </summary>
		public Expression Expression
		{
			get { return _constraint; }
		}

		private Expression _constraint;
		private Matrix _matrix;
		private List<QualifiedDimension> _requiredDimensions;
		private Dictionary<string, Dimension> _dimensions;
	}

	/// <summary>
	/// Untyped expression component.
	/// </summary>
	abstract public class Expression
	{
		/// <summary>
		/// Initializes an expression identifer.
		/// </summary>
		/// <param name="name">the name of the identifier.</param>
		/// <returns>expression identifier.</returns>
		public static ExpressionIdentifier Identifier(string name)
		{
			return new ExpressionIdentifier(name);
		}

		/// <summary>
		/// Initializes an integer expression value.
		/// </summary>
		/// <param name="value">the integer value.</param>
		/// <returns>expression value/</returns>
		public static ExpressionConstant Constant(int value)
		{
			return new ExpressionConstant(value);
		}

		/// <summary>
		/// Initializes a string expression value.
		/// </summary>
		/// <param name="value">the string value.</param>
		/// <returns>expression value</returns>
		public static ExpressionConstant Constant(string value)
		{
			return new ExpressionConstant(value);
		}

		/// <summary>
		/// Initializes a boolean expression value.
		/// </summary>
		/// <param name="value">the string value.</param>
		/// <returns>expression value</returns>
		public static ExpressionConstant Constant(bool value)
		{
			return new ExpressionConstant(value);
		}

		/// <summary>
		/// Initializes an "IF predicate THEN trueValue" expression.
		/// </summary>
		/// <param name="predicate">boolean expression.</param>
		/// <param name="trueValue">expression when predicate is true.</param>
		/// <returns>expression value</returns>
		public static Expression Conditional(Expression predicate, Expression trueValue)
		{
			return new ExpressionOperation(OperationCode.Conditional, predicate, trueValue);
		}

		/// <summary>
		/// Initializes an "IF predicate THEN trueValue ELSE falseValue" expression.
		/// </summary>
		/// <param name="predicate">boolean expression.</param>
		/// <param name="trueValue">expression when predicate is true.</param>
		/// <param name="falseValue">expression when predicate is false.</param>
		/// <returns>expression value</returns>
		public static Expression Conditional(Expression predicate, Expression trueValue, Expression falseValue)
		{
			return new ExpressionOperation(OperationCode.Conditional, predicate, trueValue, falseValue);
		}

		// Comparison operators: ==, !=, >, >=, <, <=

		/// <summary>
		/// Initializes an "expression1 == expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		public static Expression operator ==(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Equals, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 == value2" expression.
		/// </summary>
		/// <param name="expression1">expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		public static Expression operator ==(Expression expression1, int value2)
		{
			return expression1 == new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 == value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">string value.</param>
		/// <returns>expression value.</returns>
		public static Expression operator ==(Expression expression1, string value2)
		{
			return expression1 == new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 != expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		public static Expression operator !=(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.NotEquals, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 != value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		public static Expression operator !=(Expression expression1, int value2)
		{
			return expression1 != new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 != value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">string value.</param>
		/// <returns>expression value.</returns>
		public static Expression operator !=(Expression expression1, string value2)
		{
			return expression1 != new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 > expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator >(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.GreaterThan, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 > value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator >(Expression expression1, int value2)
		{
			return expression1 > new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 >= expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator >=(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.GreaterThanEqual, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 >= value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator >=(Expression expression1, int value2)
		{
			return expression1 >= new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 &lt; expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator <(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.LessThan, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 &lt; value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator <(Expression expression1, int value2)
		{
			return expression1 < new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "expression1 &lt;= expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator <=(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.LessThanEqual, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 &lt;= value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator <=(Expression expression1, int value2)
		{
			return expression1 <= new ExpressionConstant(value2);
		}

		// Multiplicative operators: *, /, %

		/// <summary>
		/// Initializes an "expression1 * expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator *(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Multiply, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 * value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator *(Expression expression1, int value2)
		{
			return expression1 * new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "value1 * expression2" expression.
		/// </summary>
		/// <param name="value1">integer value.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator *(int value1, Expression expression2)
		{
			return new ExpressionConstant(value1) * expression2;
		}

		/// <summary>
		/// Initializes an "expression1 / expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator /(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Divide, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 / value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator /(Expression expression1, int value2)
		{
			return expression1 / new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "value1 / expression2" expression.
		/// </summary>
		/// <param name="value1">integer value.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator /(int value1, Expression expression2)
		{
			return new ExpressionConstant(value1) / expression2;
		}

		/// <summary>
		/// Initializes an "expression1 % expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator %(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Modulo, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 % value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator %(Expression expression1, int value2)
		{
			return expression1 % new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "value1 % expression2" expression.
		/// </summary>
		/// <param name="value1">integer value.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator %(int value1, Expression expression2)
		{
			return new ExpressionConstant(value1) % expression2;
		}


		// Additive operators: +, -

		/// <summary>
		/// Initializes an "expression1 + expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator +(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Add, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 + value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator +(Expression expression1, int value2)
		{
			return expression1 + new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "value1 + expression2" expression.
		/// </summary>
		/// <param name="value1">integer value.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator +(int value1, Expression expression2)
		{
			return new ExpressionConstant(value1) + expression2;
		}

		/// <summary>
		/// Initializes an "expression1 - expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator -(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Subtract, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 - value2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="value2">integer value.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator -(Expression expression1, int value2)
		{
			return expression1 - new ExpressionConstant(value2);
		}

		/// <summary>
		/// Initializes an "value1 - expression2" expression.
		/// </summary>
		/// <param name="value1">integer value.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator -(int value1, Expression expression2)
		{
			return new ExpressionConstant(value1) - expression2;
		}


		/// <summary>
		/// Initializes an "expression1 &amp; expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator &(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.And, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 | expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator |(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Or, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "expression1 ^ expression2" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <param name="expression2">second expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator ^(Expression expression1, Expression expression2)
		{
			return new ExpressionOperation(OperationCode.Xor, expression1, expression2);
		}

		/// <summary>
		/// Initializes an "!expression1" expression.
		/// </summary>
		/// <param name="expression1">first expression.</param>
		/// <returns>expression value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
		public static Expression operator !(Expression expression1)
		{
			return new ExpressionOperation(OperationCode.Not, expression1);
		}

		/// <summary>
		/// Evaluate the expression as a boolean using the given function to lookup identifier values.
		/// </summary>
		/// <param name="convert">look up an identifer and return the appropriate constant value</param>
		/// <returns>true if the expression evaluates to true; false otherwise</returns>
		public bool LogicalEvaluate(Converter<ExpressionIdentifier, ExpressionConstant> convert)
		{
			ExpressionConstant e = Evaluate(convert);

			return e.Bool;
		}

		/// <summary>
		/// Find all of the identifiers referenced in the expression.
		/// </summary>
		/// <returns>the set of referenced identifiers.</returns>
		public ReadOnlyCollection<ExpressionIdentifier> Identifiers()
		{
			HashSet<ExpressionIdentifier> identifiers = new HashSet<ExpressionIdentifier>();

			Traverse(
				delegate(Expression expression)
				{
					ExpressionIdentifier identifier = expression as ExpressionIdentifier;

					if ((object)identifier != null)
					{
						if (!identifiers.Contains(identifier))
						{
							identifiers.Add(identifier);
						}
					}
				});


			return (new List<ExpressionIdentifier>(identifiers)).AsReadOnly();
		}

		/// <summary>
		/// Traverse expression tree, calling a function on each node.
		/// </summary>
		/// <param name="action">the function to execute on each part of the expression.</param>
		private void Traverse(System.Action<Expression> action)
		{
			action(this);

			ExpressionOperation operation = this as ExpressionOperation;
			if ((object)operation != null)
			{
				foreach (Expression subExpression in operation.Operands)
				{
					subExpression.Traverse(action);
				}
			}
		}

		/// <summary>
		/// Evaluate the expression using the given function to lookup identifier values.
		/// </summary>
		/// <param name="convert">look up an identifer and return the appropriate constant value</param>
		/// <returns>the expression value.</returns>
		public abstract ExpressionConstant Evaluate(Converter<ExpressionIdentifier, ExpressionConstant> convert);

		/// <summary>
		/// Equals implementation to satisfy overloading requirements.
		/// </summary>
		/// <param name="obj">the comparand.</param>
		/// <returns>equality result.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// GetHashCode implementation to satisfy overloading requirements.
		/// </summary>
		/// <returns>hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// The name of an identifier (variable) in an expression.
	/// </summary>
	public class ExpressionIdentifier : Expression
	{
		/// <summary>
		/// Initializes an expression identifier.
		/// </summary>
		/// <param name="name">name of the identifier.</param>
		public ExpressionIdentifier(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Name of the expression identifier.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		private string _name;

		/// <summary>
		/// Evaluate the identifier
		/// </summary>
		/// <param name="convert">lookup an identifier and return the appropriate constant value</param>
		/// <returns>the identifier value</returns>
		public override ExpressionConstant Evaluate(Converter<ExpressionIdentifier, ExpressionConstant> convert)
		{
			return convert(this);
		}

		/// <summary>
		/// Provide structural equality comparison
		/// </summary>
		/// <param name="obj">the other value</param>
		/// <returns>true if equal, false otherwise</returns>
		public override bool Equals(object obj)
		{
			ExpressionIdentifier other = obj as ExpressionIdentifier;

			if ((object)other != null)
			{
				return Name.Equals(other.Name, StringComparison.Ordinal);
			}

			return false;
		}

		/// <summary>
		/// Provide structural hashing
		/// </summary>
		/// <returns>hash value</returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}

	/// <summary>
	/// Types of expression constants.
	/// </summary>
	public enum ConstantType
	{
		/// <summary>
		/// String constant.
		/// </summary>
		String,
		/// <summary>
		/// Integer constant.
		/// </summary>
		Int,
		/// <summary>
		/// Boolean constant.
		/// </summary>
		Bool
	}

	/// <summary>
	/// The value of an expression constant.
	/// </summary>
	public class ExpressionConstant : Expression
	{
		/// <summary>
		/// Initializes a constant string expression.
		/// </summary>
		/// <param name="value">value of the expression.</param>
		public ExpressionConstant(string value)
		{
			m_value = value;
			type = ConstantType.String;
		}

		/// <summary>
		/// Initializes a constant integer expression.
		/// </summary>
		/// <param name="value">value of the expression.</param>
		public ExpressionConstant(int value)
		{
			m_value = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			type = ConstantType.Int;
		}

		/// <summary>
		/// Initializes a constant boolean expression.
		/// </summary>
		/// <param name="value">value of the expression.</param>
		public ExpressionConstant(bool value)
		{
			m_value = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			type = ConstantType.Bool;
		}

		/// <summary>
		/// The string value of the constant expression.
		/// </summary>
		public string String
		{
			get { return m_value; }
		}

		/// <summary>
		/// Evaluate the constant
		/// </summary>
		/// <param name="convert">lookup an identifier and return the appropriate constant value</param>
		/// <returns>the constant value</returns>
		public override ExpressionConstant Evaluate(Converter<ExpressionIdentifier, ExpressionConstant> convert)
		{
			return this;
		}

		/// <summary>
		/// The integer value of the constant expression.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
		public int Int
		{
			get { return Convert.ToInt32(m_value, System.Globalization.CultureInfo.InvariantCulture); }
		}

		/// <summary>
		/// The boolean value of the constant expression.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
		public bool Bool
		{
			get { return Convert.ToBoolean(m_value, System.Globalization.CultureInfo.InvariantCulture); }
		}

		/// <summary>
		/// The internal string value of the constant expression.
		/// </summary>
		public string Value
		{
			get { return m_value; }
		}
		private string m_value;

		/// <summary>
		/// The type of the constant expression.
		/// </summary>
		public ConstantType ConstantType
		{
			get { return type; }
		}
		private ConstantType type;
	}

	/// <summary>
	/// Operations supported within <see cref="ExpressionOperation"/> objects.
	/// </summary>
	public enum OperationCode
	{
		/// <summary>
		/// IfThen or IfThenElse operations
		/// </summary>
		Conditional,
		/// <summary>
		/// Are two operands equal?
		/// </summary>
		Equals,
		/// <summary>
		/// Are two operands not equal?
		/// </summary>
		NotEquals,
		/// <summary>
		/// Is an operand greater than the other?
		/// </summary>
		GreaterThan,
		/// <summary>
		/// Is an operand greater than or equal to the other?
		/// </summary>
		GreaterThanEqual,
		/// <summary>
		/// Is an operand less than the other?
		/// </summary>
		LessThan,
		/// <summary>
		/// Is an operand less than or equal to the other?
		/// </summary>
		LessThanEqual,
		/// <summary>
		/// The product of two operands.
		/// </summary>
		Multiply,
		/// <summary>
		/// The ratio of two operands.
		/// </summary>
		Divide,
		/// <summary>
		/// The remainder of two operands.
		/// </summary>
		Modulo,
		/// <summary>
		/// The sum of two operands.
		/// </summary>
		Add,
		/// <summary>
		/// The difference between to operands.
		/// </summary>
		Subtract,
		/// <summary>
		/// The logical "and" of two operands.
		/// </summary>
		And,
		/// <summary>
		/// The logical "or" of two operands.
		/// </summary>
		Or,
		/// <summary>
		/// The logical exclusive "or" of two operands.
		/// </summary>
		Xor,
		/// <summary>
		/// The logical negations of an operand.
		/// </summary>
		Not
	}

	/// <summary>
	/// An operation on a collection of expressions.
	/// </summary>
	public class ExpressionOperation : Expression
	{
		/// <summary>
		/// Initializes an operation on a collection of expressions.
		/// </summary>
		/// <param name="op">the name of the operation.</param>
		/// <param name="operands">the expressions for the operation.</param>
		public ExpressionOperation(OperationCode op, params Expression[] operands)
		{
			_op = op;
			_operands = new List<Expression>(operands);
		}

		private OperationCode _op;
		private List<Expression> _operands;

		/// <summary>
		/// Evaluate the operation
		/// </summary>
		/// <param name="convert">lookup an identifier and return the appropriate constant value</param>
		/// <returns>the operation result</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public override ExpressionConstant Evaluate(Converter<ExpressionIdentifier, ExpressionConstant> convert)
		{
			if (_operands.Count >= 3)
			{
				throw new InvalidOperationException("Too many operands");
			}
			ExpressionConstant c1
				= _operands.Count > 0 ? _operands[0].Evaluate(convert) : null;
			ExpressionConstant c2
				= _operands.Count > 1 ? _operands[1].Evaluate(convert) : null;
			ExpressionConstant c3
				= _operands.Count > 2 ? _operands[2].Evaluate(convert) : null;

			switch (_op)
			{
				// Comparison operators: ==, !=, >, >=, <, <=
				case OperationCode.Equals:
					return new ExpressionConstant(c1.Value == c2.Value);

				case OperationCode.NotEquals:
					return new ExpressionConstant(c1.Value != c2.Value);

				case OperationCode.GreaterThan:
					if (c1.ConstantType == ConstantType.Int && c2.ConstantType == ConstantType.Int)
					{
						return new ExpressionConstant(c1.Int > c2.Int);
					}
					else
					{
						return new ExpressionConstant(string.Compare(c1.Value, c2.Value, StringComparison.Ordinal) > 0);
					}

				case OperationCode.GreaterThanEqual:
					if (c1.ConstantType == ConstantType.Int && c2.ConstantType == ConstantType.Int)
					{
						return new ExpressionConstant(c1.Int >= c2.Int);
					}
					else
					{
						return new ExpressionConstant(string.Compare(c1.Value, c2.Value, StringComparison.Ordinal) >= 0);
					}

				case OperationCode.LessThan:
					if (c1.ConstantType == ConstantType.Int && c2.ConstantType == ConstantType.Int)
					{
						return new ExpressionConstant(c1.Int < c2.Int);
					}
					else
					{
						return new ExpressionConstant(string.Compare(c1.Value, c2.Value, StringComparison.Ordinal) < 0);
					}

				case OperationCode.LessThanEqual:
					if (c1.ConstantType == ConstantType.Int && c2.ConstantType == ConstantType.Int)
					{
						return new ExpressionConstant(c1.Int <= c2.Int);
					}
					else
					{
						return new ExpressionConstant(string.Compare(c1.Value, c2.Value, StringComparison.Ordinal) <= 0);
					}

				// Multiplicative operators: *, /, %
				case OperationCode.Multiply:
					return new ExpressionConstant(c1.Int * c2.Int);

				case OperationCode.Divide:
					return new ExpressionConstant(c1.Int / c2.Int);

				case OperationCode.Modulo:
					return new ExpressionConstant(c1.Int % c2.Int);

				// Additive operators: +, -
				case OperationCode.Add:
					if (c1.ConstantType == ConstantType.Int && c2.ConstantType == ConstantType.Int)
					{
						return new ExpressionConstant(c1.Int + c2.Int);
					}
					else
					{
						return new ExpressionConstant(c1.Value + c2.Value);
					}

				case OperationCode.Subtract:
					return new ExpressionConstant(c1.Int + c2.Int);

				case OperationCode.And:
					return new ExpressionConstant(c1.Bool & c2.Bool);

				case OperationCode.Or:
					return new ExpressionConstant(c1.Bool | c2.Bool);

				case OperationCode.Xor:
					return new ExpressionConstant(c1.Bool | c2.Bool);

				case OperationCode.Not:
					return new ExpressionConstant(!c1.Bool);

				case OperationCode.Conditional:
					if (c1.Bool)
					{
						return c2;
					}
					else
					{
						return c3;
					}

				default:
					throw new InvalidOperationException("Unexpected operator");
			}
		}

		/// <summary>
		/// The name of the operator.
		/// </summary>
		public OperationCode Operator
		{
			get { return _op; }
		}

		/// <summary>
		/// The expressions for the operation.
		/// </summary>
		public ReadOnlyCollection<Expression> Operands
		{
			get { return _operands.AsReadOnly(); }
		}
	}
}
