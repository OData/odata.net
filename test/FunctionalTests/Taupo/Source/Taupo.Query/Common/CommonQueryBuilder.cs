//---------------------------------------------------------------------
// <copyright file="CommonQueryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Factory methods used to construct common QueryExpression tree nodes.
    /// </summary>
    public static class CommonQueryBuilder
    {
        private static QueryConstantExpression constantUnspecifiedTypeNull = new QueryConstantExpression(new QueryScalarValue(QueryUnspecifiedType.Instance, null, null, DummyQueryEvaluationStrategy.Instance)); 

        /// <summary>
        /// Gets the <see cref="QueryConstantExpression"/> which represents an untyped null.
        /// </summary>
        /// <returns>The untyped null <see cref="QueryConstantExpression"/>.</returns>
        public static QueryConstantExpression ConstantUnspecifiedTypeNull
        {
            get { return constantUnspecifiedTypeNull; }
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryAddExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryAddExpression"/> with the provided arguments.</returns>
        public static QueryExpression Add(this QueryExpression left, QueryExpression right)
        {
            return Add(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryAndExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryAndExpression"/> with the provided arguments.</returns>
        public static QueryExpression And(this QueryExpression left, QueryExpression right)
        {
            return And(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing an As operation
        /// </summary>
        /// <param name="source">The Argument for the type operation</param>
        /// <param name="typeToCompareAgainst">The Type to test against</param>
        /// <returns>The <see cref="QueryExpression"/> representing an IsOf operation</returns>
        public static QueryExpression As(this QueryExpression source, QueryType typeToCompareAgainst)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(typeToCompareAgainst, "typeToCompareAgainst");
            AssertNotUnresolvedType(typeToCompareAgainst);
            
            return new QueryAsExpression(source, typeToCompareAgainst);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing a Cast operation
        /// </summary>
        /// <param name="argument">The Argument for the type operation</param>
        /// <param name="typeToCastTo">The Type to test against</param>
        /// <returns>The <see cref="QueryExpression"/> representing a Cast operation</returns>
        public static QueryExpression Cast(this QueryExpression argument, QueryType typeToCastTo)
        {
            ExceptionUtilities.CheckArgumentNotNull(argument, "argument");
            ExceptionUtilities.CheckArgumentNotNull(typeToCastTo, "typeToCastTo");
            AssertNotUnresolvedType(typeToCastTo);

            return new QueryCastExpression(argument, typeToCastTo);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryCustomFunctionCallExpression"/>.
        /// </summary>
        /// <param name="function">The function to call.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>The <see cref="QueryCustomFunctionCallExpression"/> with the provided arguments.</returns>
        public static QueryExpression Call(this Function function, params QueryExpression[] arguments)
        {
            return new QueryCustomFunctionCallExpression(QueryType.Unresolved, function, null, false, false, arguments);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryFunctionImportCallExpression"/>.
        /// </summary>
        /// <param name="functionImport">The function import to call.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>The <see cref="QueryFunctionImportCallExpression"/> with the provided arguments.</returns>
        public static QueryExpression Call(this FunctionImport functionImport, params QueryExpression[] arguments)
        {
            return new QueryFunctionImportCallExpression(QueryType.Unresolved, functionImport, false, arguments);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryConstantExpression"/>.
        /// </summary>
        /// <param name="scalarValue">Scalar value.</param>
        /// <returns>The <see cref="QueryConstantExpression"/> with the provided arguments.</returns>
        public static QueryConstantExpression Constant(QueryScalarValue scalarValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(scalarValue, "scalarValue");

            return new QueryConstantExpression(scalarValue);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryConstantExpression"/>.
        /// </summary>
        /// <param name="value">Value of the expression.</param>
        /// <returns>The <see cref="QueryConstantExpression"/> with the provided value.</returns>
        public static QueryConstantExpression Constant(object value)
        {
            return Constant(value, QueryType.UnresolvedPrimitive);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryConstantExpression"/>.
        /// </summary>
        /// <param name="value">Value of the expression.</param>
        /// <param name="valueType">Type of the expression.</param>
        /// <returns>The <see cref="QueryConstantExpression"/> with the provided arguments.</returns>
        public static QueryConstantExpression Constant(object value, QueryScalarType valueType)
        {
            ExceptionUtilities.CheckArgumentNotNull(valueType, "valueType");

            if (value == null)
            {
                ExceptionUtilities.Assert(!valueType.IsUnresolved, "When value is null type cannot be unresolved.");
            }

            return Constant(valueType.CreateValue(value));
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryDivideExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryDivideExpression"/> with the provided arguments.</returns>
        public static QueryExpression Divide(this QueryExpression left, QueryExpression right)
        {
            return Divide(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryEqualToExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryEqualToExpression"/> with the provided arguments.</returns>
        public static QueryExpression EqualTo(this QueryExpression left, QueryExpression right)
        {
            return EqualTo(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing a function parameter reference
        /// </summary>
        /// <param name="parameterName">The function parameter name</param>
        /// <returns>The <see cref="QueryExpression"/> representing a function parameter reference</returns>
        public static QueryExpression FunctionParameterReference(string parameterName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(parameterName, "parameterName");

            return new QueryFunctionParameterReferenceExpression(parameterName, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryGreaterThanExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryGreaterThanExpression"/> with the provided arguments.</returns>
        public static QueryExpression GreaterThan(this QueryExpression left, QueryExpression right)
        {
            return GreaterThan(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryGreaterThanOrEqualToExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryGreaterThanOrEqualToExpression"/> with the provided arguments.</returns>
        public static QueryExpression GreaterThanOrEqualTo(this QueryExpression left, QueryExpression right)
        {
            return GreaterThanOrEqualTo(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create <see cref="QueryIsNotNullExpression"/>.
        /// </summary>
        /// <param name="argument">Argument for the expression.</param>
        /// <returns>The <see cref="QueryIsNotNullExpression"/> with the provided argument.</returns>
        public static QueryExpression IsNotNull(this QueryExpression argument)
        {
            ExceptionUtilities.CheckArgumentNotNull(argument, "argument");

            return new QueryIsNotNullExpression(argument, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create <see cref="QueryIsNullExpression"/>.
        /// </summary>
        /// <param name="argument">Argument for the expression.</param>
        /// <returns>The <see cref="QueryIsNullExpression"/> with the provided argument.</returns>
        public static QueryExpression IsNull(this QueryExpression argument)
        {
            ExceptionUtilities.CheckArgumentNotNull(argument, "argument");

            return new QueryIsNullExpression(argument, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing an IsOf operation
        /// </summary>
        /// <param name="argument">The Argument for the type operation</param>
        /// <param name="typeToCompareAgainst">The Type to test against</param>
        /// <returns>The <see cref="QueryExpression"/> representing an IsOf operation</returns>
        public static QueryExpression IsOf(this QueryExpression argument, QueryType typeToCompareAgainst)
        {
            ExceptionUtilities.CheckArgumentNotNull(argument, "argument");
            ExceptionUtilities.CheckArgumentNotNull(typeToCompareAgainst, "typeToCompareAgainst");
            AssertNotUnresolvedType(typeToCompareAgainst);

            var boolType = typeToCompareAgainst.EvaluationStrategy.BooleanType;
            return new QueryIsOfExpression(argument, typeToCompareAgainst, boolType);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryLessThanExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryLessThanExpression"/> with the provided arguments.</returns>
        public static QueryExpression LessThan(this QueryExpression left, QueryExpression right)
        {
            return LessThan(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryLessThanOrEqualToExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryLessThanOrEqualToExpression"/> with the provided arguments.</returns>
        public static QueryExpression LessThanOrEqualTo(this QueryExpression left, QueryExpression right)
        {
            return LessThanOrEqualTo(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryModuloExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryModuloExpression"/> with the provided arguments.</returns>
        public static QueryExpression Modulo(this QueryExpression left, QueryExpression right)
        {
            return Modulo(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryMultiplyExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryMultiplyExpression"/> with the provided arguments.</returns>
        public static QueryExpression Multiply(this QueryExpression left, QueryExpression right)
        {
            return Multiply(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryNotEqualToExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryNotEqualToExpression"/> with the provided arguments.</returns>
        public static QueryExpression NotEqualTo(this QueryExpression left, QueryExpression right)
        {
            return NotEqualTo(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryNotExpression"/>.
        /// </summary>
        /// <param name="expression">The operand.</param>
        /// <returns>The <see cref="QueryNotExpression"/> with the provided arguments.</returns>
        public static QueryExpression Not(this QueryExpression expression)
        {
            return Not(expression, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing a Null keyword
        /// </summary>
        /// <param name="type">Type of the Null keyword.</param>
        /// <returns>The <see cref="QueryExpression"/> representing the Null keyword</returns>
        public static QueryExpression Null(QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            AssertNotUnresolvedType(type);

            return new QueryNullExpression(type);
        }

        /// <summary>
        /// Factory method to create a <see cref="QueryExpression"/> representing an OfType operation
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="typeToFilterAgainst">The Type to test against</param>
        /// <returns>The <see cref="QueryExpression"/> representing an OfType operation</returns>
        public static QueryExpression OfType(this QueryExpression source, QueryType typeToFilterAgainst)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(typeToFilterAgainst, "typeToFilterAgainst");
            AssertNotUnresolvedType(typeToFilterAgainst);

            return new QueryOfTypeExpression(source, typeToFilterAgainst, typeToFilterAgainst.CreateCollectionType());
        }

        /// <summary>
        /// Factory method to create the <see cref="QuerySubtractExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QuerySubtractExpression"/> with the provided arguments.</returns>
        public static QueryExpression Subtract(this QueryExpression left, QueryExpression right)
        {
            return Subtract(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryOrExpression"/>.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The <see cref="QueryOrExpression"/> with the provided arguments.</returns>
        public static QueryExpression Or(this QueryExpression left, QueryExpression right)
        {
            return Or(left, right, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryPropertyExpression"/>.
        /// </summary>
        /// <param name="instance">Instance expression.</param>
        /// <param name="queryProperty">The property.</param>
        /// <returns>The <see cref="QueryPropertyExpression"/> with the provided instance, name and type.</returns>
        public static QueryExpression Property(this QueryExpression instance, QueryProperty queryProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(queryProperty, "queryProperty");

            return instance.Property(queryProperty.Name);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryPropertyExpression"/>.
        /// </summary>
        /// <param name="instance">Instance expression.</param>
        /// <param name="name">Name of the property.</param>
        /// <returns>The <see cref="QueryPropertyExpression"/> with the provided instance, name and type.</returns>
        public static QueryExpression Property(this QueryExpression instance, string name)
        {
            return instance.Property(name, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryPropertyExpression"/>.
        /// </summary>
        /// <param name="instance">Instance expression.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <returns>The <see cref="QueryPropertyExpression"/> with the provided instance, name and type.</returns>
        public static QueryExpression Property(this QueryExpression instance, string name, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryPropertyExpression(instance, name, type);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryRootExpression"/>.
        /// </summary>
        /// <param name="name">Unqualified name of the source query.</param>
        /// <param name="expressionType">Type of the expression.</param>
        /// <returns>The <see cref="QueryRootExpression"/> with the provided instance, name and type.</returns>
        public static QueryExpression Root(string name, QueryType expressionType)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.CheckArgumentNotNull(expressionType, "expressionType");
            AssertNotUnresolvedType(expressionType);

            return new QueryRootExpression(name, expressionType);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryRootExpression"/> for the given entity set.
        /// </summary>
        /// <param name="entitySet">The underlying entity set for the root query.</param>
        /// <returns>The <see cref="QueryRootExpression"/> with the specified underlying entity set.</returns>
        public static QueryExpression Root(EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            return new QueryRootExpression(entitySet, QueryType.Unresolved);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryCustomFunctionCallExpression"/> which is a root expression.
        /// </summary>
        /// <param name="function">The function to call.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>The <see cref="QueryCustomFunctionCallExpression"/> with the provided arguments.</returns>
        public static QueryExpression AsRootQuery(this Function function, params QueryExpression[] arguments)
        {
            return new QueryCustomFunctionCallExpression(QueryType.Unresolved, function, null, true, false, arguments);
        }

        /// <summary>
        /// Factory method to create the <see cref="QueryCustomFunctionCallExpression"/> which is a root expression.
        /// </summary>
        /// <param name="functionImport">The function import to call.</param>
        /// <param name="arguments">The arguments for the function import call.</param>
        /// <returns>The <see cref="QueryFunctionImportCallExpression"/> with the provided arguments.</returns>
        public static QueryExpression AsRootQuery(this FunctionImport functionImport, params QueryExpression[] arguments)
        {
            return new QueryFunctionImportCallExpression(QueryType.Unresolved, functionImport, true, arguments);
        }

        internal static QueryExpression Add(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryAddExpression(left, right, type);
        }

        internal static QueryExpression And(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryAndExpression(left, right, type);
        }

        internal static QueryExpression Divide(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryDivideExpression(left, right, type);
        }

        internal static QueryExpression EqualTo(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryEqualToExpression(left, right, type);
        }

        internal static QueryExpression GreaterThan(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryGreaterThanExpression(left, right, type);
        }

        internal static QueryExpression GreaterThanOrEqualTo(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryGreaterThanOrEqualToExpression(left, right, type);
        }

        internal static QueryExpression LessThan(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryLessThanExpression(left, right, type);
        }

        internal static QueryExpression LessThanOrEqualTo(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryLessThanOrEqualToExpression(left, right, type);
        }

        internal static QueryExpression Modulo(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryModuloExpression(left, right, type);
        }

        internal static QueryExpression Multiply(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryMultiplyExpression(left, right, type);
        }

        internal static QueryExpression NotEqualTo(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryNotEqualToExpression(left, right, type);
        }

        internal static QueryExpression Not(this QueryExpression expression, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryNotExpression(expression, type);
        }

        internal static QueryExpression Subtract(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QuerySubtractExpression(left, right, type);
        }

        internal static QueryExpression Or(this QueryExpression left, QueryExpression right, QueryType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(right, "right");
            ExceptionUtilities.CheckArgumentNotNull(left, "left");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return new QueryOrExpression(left, right, type);
        }

        private static void AssertNotUnresolvedType(QueryType type)
        {
            ExceptionUtilities.Assert(!type.IsUnresolved, "Type must not be unresolved.");
        }
    }
}
