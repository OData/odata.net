//---------------------------------------------------------------------
// <copyright file="Code.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Convenient CodeDom methods to construct code expressions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Helper methods to convert many different types.")]
    public static class Code
    {
        /// <summary>
        /// Builds a code statement that throws the given expression
        /// </summary>
        /// <param name="expression">Expression to throw</param>
        /// <returns>Code statement that will throw</returns>
        public static CodeStatement Throw(this CodeExpression expression)
        {
            return new CodeThrowExceptionStatement(expression);
        }

        /// <summary>
        /// Extends a property from a code expression
        /// </summary>
        /// <param name="expression">The code expression to extend</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>the reference to the extended property</returns>
        public static CodeExpression Property(this CodeExpression expression, string propertyName)
        {
            return new CodePropertyReferenceExpression(expression, propertyName);
        }

        /// <summary>
        /// Extends a field from a code expression
        /// </summary>
        /// <param name="expression">The code expression to extend</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>The reference to the extended field</returns>
        public static CodeExpression Field(this CodeExpression expression, string fieldName)
        {
            return new CodeFieldReferenceExpression(expression, fieldName);
        }

        /// <summary>
        /// Comments code
        /// </summary>
        /// <param name="commentLine">The code comment</param>
        /// <returns>The CodeCommentStatement</returns>
        public static CodeCommentStatement Comment(string commentLine)
        {
            return new CodeCommentStatement(commentLine);
        }

        /// <summary>
        /// Extends an indexer from a code expression
        /// </summary>
        /// <param name="target">The code expression to extend</param>
        /// <param name="arrayIndex">The index to reference</param>
        /// <returns>The reference to the extended indexer</returns>
        public static CodeExpression Index(this CodeExpression target, CodeExpression arrayIndex)
        {
            return new CodeArrayIndexerExpression(target, arrayIndex);
        }

        /// <summary>
        /// Extends a method call from a code expression
        /// </summary>
        /// <param name="expression">the code expression to extend</param>
        /// <param name="methodName">name of the method</param>
        /// <param name="arguments">the list of argument expressions</param>
        /// <returns>the method invocation expression</returns>
        public static CodeExpression Call(this CodeExpression expression, string methodName, params CodeExpression[] arguments)
        {
            return new CodeMethodInvokeExpression(expression, methodName, arguments);
        }

        /// <summary>
        /// Extends a method call with generic arguments from a code expression
        /// </summary>
        /// <param name="expression">The code expression to extend</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="genericTypeReferences">The collection of CodeTypeReferences representing generic arguments</param>
        /// <param name="arguments">The list of argument expressions</param>
        /// <returns>The method invocation expression</returns>
        public static CodeExpression Call(this CodeExpression expression, string methodName, CodeTypeReference[] genericTypeReferences, params CodeExpression[] arguments)
        {
            var methodReferenceExpression = new CodeMethodReferenceExpression(expression, methodName, genericTypeReferences);
            var methodInvokeExpression = new CodeMethodInvokeExpression(methodReferenceExpression, arguments);
            return methodInvokeExpression;
        }

        /// <summary>
        /// Adds parameter to a lambda expression
        /// </summary>
        /// <param name="lambda">the lambda expression to add parameter</param>
        /// <param name="name">the name of the parameter with implicit type reference</param>
        /// <returns>the newly constructed lambda expression</returns>
        public static CodeLambdaExpression WithParameter(this CodeLambdaExpression lambda, string name)
        {
            lambda.Parameters.Add(new CodeParameterDeclarationExpression(new CodeImplicitTypeReference(), name));
            return lambda;
        }

        /// <summary>
        /// Adds parameters to a lambda expression
        /// </summary>
        /// <param name="lambda">the lambda expression to which parameters are added</param>
        /// <param name="names">the names of the parameters with implicit type references</param>
        /// <returns>the newly constructed lambda expression</returns>
        public static CodeLambdaExpression WithParameters(this CodeLambdaExpression lambda, params string[] names)
        {
            foreach (string name in names)
            {
                lambda = lambda.WithParameter(name);
            }

            return lambda;
        }

        /// <summary>
        /// Adds parameter to a lambda expression
        /// </summary>
        /// <param name="lambda">the lambda expression to add parameter</param>
        /// <param name="type">the type of the paramter</param>
        /// <param name="name">the name of the parameter with strong type reference</param>
        /// <returns>the newly constructed lambda expression</returns>
        public static CodeLambdaExpression WithParameter(this CodeLambdaExpression lambda, Type type, string name)
        {
            lambda.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(type), name));
            return lambda;
        }

        /// <summary>
        /// Adds a body that is composed of a single expression with a return value to a <see cref="CodeLambdaExpression"/>.
        /// </summary>
        /// <param name="lambda">The <see cref="CodeLambdaExpression"/> to which to add a body.</param>
        /// <param name="body">The <see cref="CodeExpression"/> that serves as the body of the lambda.</param>
        /// <returns>The same <see cref="CodeLambdaExpression"/> with the <paramref name="body"/> applied.</returns>
        public static CodeLambdaExpression WithBody(this CodeLambdaExpression lambda, CodeExpression body)
        {
            lambda.Body = body;
            return lambda;
        }

        /// <summary>
        /// Adds a body that is composed of multiple statements with a return value to a <see cref="CodeLambdaExpression"/>.
        /// </summary>
        /// <param name="lambda">The <see cref="CodeLambdaExpression"/> to which to add a body.</param>
        /// <param name="body">A set of <see cref="CodeStatement"/>s that make up the body of the lambda.</param>
        /// <returns>The same <see cref="CodeLambdaExpression"/> with the <paramref name="body"/> applied.</returns>
        public static CodeLambdaExpression WithBody(this CodeLambdaExpression lambda, params CodeStatement[] body)
        {
            lambda.BodyStatements.AddRange(body);
            return lambda;
        }

        /// <summary>
        /// Adds a body that is composed of a single expression with no return value to a <see cref="CodeLambdaExpression"/>.
        /// </summary>
        /// <param name="lambda">The <see cref="CodeLambdaExpression"/> to which to add a body.</param>
        /// <param name="body">The <see cref="CodeExpression"/> that serves as the body of the lambda.</param>
        /// <returns>The same <see cref="CodeLambdaExpression"/> with the <paramref name="body"/> applied.</returns>
        public static CodeLambdaExpression WithBodyNoReturn(this CodeLambdaExpression lambda, CodeExpression body)
        {
            lambda.HasReturnValue = false;
            return lambda.WithBody(body);
        }

        /// <summary>
        /// Adds a body that is composed of multiple statements with no return value to a <see cref="CodeLambdaExpression"/>.
        /// </summary>
        /// <param name="lambda">The <see cref="CodeLambdaExpression"/> to which to add a body.</param>
        /// <param name="body">A set of <see cref="CodeStatement"/>s that make up the body of the lambda.</param>
        /// <returns>the newly constructed lambda expression</returns>
        public static CodeLambdaExpression WithBodyNoReturn(this CodeLambdaExpression lambda, params CodeStatement[] body)
        {
            lambda.HasReturnValue = false;
            return lambda.WithBody(body);
        }

        /// <summary>
        /// Constructs an Argument reference expression
        /// </summary>
        /// <param name="argumentName">the name of the argument</param>
        /// <returns>the newly constructed argument reference expression</returns>
        public static CodeExpression Argument(string argumentName)
        {
            return new CodeArgumentReferenceExpression(argumentName);
        }

        /// <summary>
        /// Constructs a value equality comparison expression
        /// </summary>
        /// <param name="left">the left side of the equality comparison</param>
        /// <param name="right">the right side of the equality comparison</param>
        /// <returns>the newly constructed binary operator expression</returns>
        public static CodeExpression ValueEquals(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.ValueEquality, right);
        }

        /// <summary>
        /// Constructs a reference (identity) equality comparison expression
        /// </summary>
        /// <param name="left">the left side of the equality comparison</param>
        /// <param name="right">the right side of the equality comparison</param>
        /// <returns>the newly constructed binary operator expression</returns>
        public static CodeExpression IdentityEquals(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityEquality, right);
        }

        /// <summary>
        /// Creates a Set of statements
        /// </summary>
        /// <param name="individualStatements">Param of CodeStatements</param>
        /// <returns>A CodeStatement Array</returns>
        public static CodeStatement[] Statements(params CodeStatement[] individualStatements)
        {
            var statementList = new List<CodeStatement>();
            statementList.AddRange(individualStatements);
            return statementList.ToArray();
        }

        /// <summary>
        /// Constructs a CodeConditionStatement 
        /// </summary>
        /// <param name="condition">Condition to be evaluated within the If statement</param>
        /// <param name="trueStatements">True statements</param>
        /// <returns>If Then Condition statement</returns>
        public static CodeConditionStatement IfThen(this CodeExpression condition, params CodeStatement[] trueStatements)
        {
            var conditionStatement = new CodeConditionStatement(condition, trueStatements);
            return conditionStatement;
        }

        /// <summary>
        /// Constructs a CodeConditionStatement 
        /// </summary>
        /// <param name="condition">Condition to be evaluated within the If statement</param>
        /// <param name="trueStatements">True statements</param>
        /// <param name="falseStatements">False statements</param>
        /// <returns>If Then Condition statement</returns>
        public static CodeConditionStatement IfThenElse(this CodeExpression condition, CodeStatement[] trueStatements, CodeStatement[] falseStatements)
        {
            var conditionStatement = new CodeConditionStatement(condition, trueStatements, falseStatements);
            return conditionStatement;
        }

        /// <summary>
        /// Constructs a value inequality comparison expression
        /// </summary>
        /// <param name="left">the left side of the equality comparison</param>
        /// <param name="right">the right side of the equality comparison</param>
        /// <returns>the newly constructed binary operator expression</returns>
        public static CodeExpression ValueNotEquals(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(
                new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.ValueEquality, right),
                CodeBinaryOperatorType.ValueEquality,
                Primitive(false));
        }

        /// <summary>
        /// Constructs a reference (identity) inequality comparison expression
        /// </summary>
        /// <param name="left">the left side of the equality comparison</param>
        /// <param name="right">the right side of the equality comparison</param>
        /// <returns>the newly constructed binary operator expression</returns>
        public static CodeExpression IdentityNotEquals(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(
                new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityEquality, right),
                CodeBinaryOperatorType.ValueEquality,
                Primitive(false));
        }

        /// <summary>
        /// Constructs a type reference expression
        /// </summary>
        /// <typeparam name="TType">Type to use for TypeReferenceExpression</typeparam>
        /// <returns>the newly constructed type reference expression</returns>
        public static CodeTypeReferenceExpression Type<TType>()
        {
            return new CodeTypeReferenceExpression(typeof(TType));
        }

        /// <summary>
        /// Constructs a type reference expression
        /// </summary>
        /// <param name="typeName">the name of the type</param>
        /// <returns>the newly constructed type reference expression</returns>
        public static CodeTypeReferenceExpression Type(string typeName)
        {
            return new CodeTypeReferenceExpression(typeName);
        }

        /// <summary>
        /// Constructs a type reference expression
        /// </summary>
        /// <param name="typeRef">the code type reference</param>
        /// <returns>the newly constructed type reference expression</returns>
        public static CodeTypeReferenceExpression Type(CodeTypeReference typeRef)
        {
            return new CodeTypeReferenceExpression(typeRef);
        }

        /// <summary>
        /// Converts the expression to the specified type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="targetType">Target type reference expression.</param>
        /// <returns>Expression with applied as operator.</returns>
        public static CodeExpression TypeAs(this CodeExpression expression, CodeTypeReference targetType)
        {
            return new CodeAsExpression(expression, Code.Type(targetType));
        }

        /// <summary>
        /// Checks if the specified expression is of a particular type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="targetType">Target type reference expression.</param>
        /// <returns>Expression with applied is operator.</returns>
        public static CodeExpression TypeIs(this CodeExpression expression, CodeTypeReference targetType)
        {
            return new CodeIsExpression(expression, Code.Type(targetType));
        }

        /// <summary>
        /// Constructs a typeOf expression
        /// </summary>
        /// <param name="typeReference">the type to to do the typeof on</param>
        /// <returns>the newly constructed typeof expression</returns>
        public static CodeTypeOfExpression TypeOf(this CodeTypeReference typeReference)
        {
            return new CodeTypeOfExpression(typeReference);
        }

        /// <summary>
        /// Constructs a typeOf expression
        /// </summary>
        /// <param name="typeName">Name of type to construct TypeOfExpression</param>
        /// <returns>the newly constructed typeof expression</returns>
        public static CodeTypeOfExpression TypeOf(string typeName)
        {
            return new CodeTypeOfExpression(typeName);
        }

        /// <summary>
        /// Constructs a variable reference expression
        /// </summary>
        /// <param name="variableName">the name of the variable</param>
        /// <returns>the newly constructed variable reference expression</returns>
        public static CodeExpression Variable(string variableName)
        {
            return new CodeVariableReferenceExpression(variableName);
        }

        /// <summary>
        /// Constructs a primitive type expression
        /// </summary>
        /// <param name="value">the value of the primitive type</param>
        /// <returns>the newly constructed primitive type expression</returns>
        public static CodeExpression Primitive(object value)
        {
            return new CodePrimitiveExpression(value);
        }

        /// <summary>
        /// Constructs a null value expression
        /// </summary>
        /// <returns>the newly constructed primitive type expression</returns>
        public static CodeExpression Null()
        {
            return new CodePrimitiveExpression(null);
        }

        /// <summary>
        /// Constructs a reference expression to current type object
        /// </summary>
        /// <returns>the newly constructed this reference expression</returns>
        public static CodeExpression This()
        {
            return new CodeThisReferenceExpression();
        }

        /// <summary>
        /// Constructs an empty lambda expression
        /// </summary>
        /// <returns>the newly constructed lambda expression</returns>
        public static CodeLambdaExpression Lambda()
        {
            return new CodeLambdaExpression();
        }

        /// <summary>
        /// Generates the 'return' statement which returns the specified return value.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        /// <returns>Generated statement.</returns>
        public static CodeStatement Return(CodeExpression returnValue)
        {
            return new CodeMethodReturnStatement(returnValue);
        }

        /// <summary>
        /// Generates variable declaration statement.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="initExpression">The init expression.</param>
        /// <returns>Variable declaration statement</returns>
        /// <remarks>Variable type will be inferred from init expression.</remarks>
        public static CodeStatement DeclareVariable(string variableName, CodeExpression initExpression)
        {
            return DeclareVariable(new CodeImplicitTypeReference(), variableName, initExpression);
        }

        /// <summary>
        /// Generates variable declaration statement.
        /// </summary>
        /// <param name="variableTypeName">Name of the variable type.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="initExpression">The init expression.</param>
        /// <returns>Variable declaration statement</returns>
        public static CodeStatement DeclareVariable(string variableTypeName, string variableName, CodeExpression initExpression)
        {
            return DeclareVariable(new CodeTypeReference(variableTypeName), variableName, initExpression);
        }

        /// <summary>
        /// Generates variable declaration statement.
        /// </summary>
        /// <param name="variableType">Type of the variable.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="initExpression">The init expression.</param>
        /// <returns>Variable declaration statement</returns>
        public static CodeStatement DeclareVariable(CodeTypeReference variableType, string variableName, CodeExpression initExpression)
        {
            return new CodeVariableDeclarationStatement(variableType, variableName, initExpression);
        }

        /// <summary>
        /// Generates variable declaration statement.
        /// </summary>
        /// <param name="variableType">Type of the variable.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="initExpression">The init expression.</param>
        /// <returns>Variable declaration statement</returns>
        public static CodeStatement DeclareVariable(Type variableType, string variableName, CodeExpression initExpression)
        {
            return DeclareVariable(new CodeTypeReference(variableType), variableName, initExpression);
        }

        /// <summary>
        /// Creates an integer based for loop incrementing by one on each iteration
        /// </summary>
        /// <param name="from">Value to iterate from</param>
        /// <param name="lessThan">Value to stop execution at</param>
        /// <param name="variable">Name to be used for the integer variable</param>
        /// <param name="contents">Statements to execute in the loop</param>
        /// <returns>For loop statement containing the supplied statements</returns>
        public static CodeStatement CreateForLoop(int from, int lessThan, string variable, params CodeStatement[] contents)
        {
            return new CodeIterationStatement(
                        DeclareVariable(typeof(int), variable, Primitive(from)),
                        new CodeBinaryOperatorExpression(Variable(variable), CodeBinaryOperatorType.LessThan, Primitive(lessThan)),
                        Variable(variable).Assign(Variable(variable).Add(Primitive(1))),
                        contents);
        }

        /// <summary>
        /// Generates an object value expression.
        /// </summary>
        /// <param name="value">The object to generate an expression for.</param>
        /// <returns>Object value expression.</returns>
        public static CodeExpression ObjectValue(object value)
        {
            if (value == null)
            {
                return Code.Null();
            }
            
            var type = value.GetType();
            if (type.IsPrimitive() || value is string)
            {
                if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) || type == typeof(long))
                {
                    // byte, sbyte, short, ushort do not have special markers, and for some reason CodeDom leaves out the marker for long
                    return Code.Primitive(value).Cast(Code.TypeRef(type));
                }
                else
                {
                    return Code.Primitive(value);
                }
            }
            
            if (type.IsArray)
            {
                return Code.Array((Array)value);
            }
            
            if (type == typeof(DateTime))
            {
                var dateTimeValue = (DateTime)value;

                var ticks = Code.Primitive(dateTimeValue.Ticks);
                var kind = Code.EnumValue(dateTimeValue.Kind);
                return Code.New(typeof(DateTime), ticks, kind);
            }

            if (type == typeof(TimeSpan))
            {
                var timeSpanValue = (TimeSpan)value;
                var ticks = Code.Primitive(timeSpanValue.Ticks);

                return Code.New(typeof(TimeSpan), ticks);
            }

            if (type == typeof(DateTimeOffset))
            {
                DateTimeOffset dateTimeOffsetValue = (DateTimeOffset)value;
                var datetimePart = Code.ObjectValue(dateTimeOffsetValue.DateTime);
                var offsetPart = Code.ObjectValue(dateTimeOffsetValue.Offset);

                return Code.New(typeof(DateTimeOffset), datetimePart, offsetPart);
            }

            if (type == typeof(Guid))
            {
                return Code.New(typeof(Guid), Code.Primitive(value.ToString()));
            }

            if (type == typeof(decimal))
            {
                return Code.Primitive(Convert.ToDecimal((decimal)value));
            }
            
            var enumValue = value as Enum;
            if (enumValue != null)
            {
                var memberName = Enum.GetName(type, value);

                if (memberName == null)
                {
                    // Enum values outside the range of named enum members can only be validly used with enum properties with an explicit cast.
                    return Code.Primitive(Convert.ToInt64(value, CultureInfo.InvariantCulture)).Cast(type);
                }
                else
                {
                    return Code.EnumValue(enumValue);
                }
            }

            throw new TaupoNotSupportedException("Type not supported: " + type);
        }

        /// <summary>
        /// Generates object creation expression.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="creationArguments">The creation arguments.</param>
        /// <returns>Object creation expression.</returns>
        public static CodeExpression New(CodeTypeReference type, params CodeExpression[] creationArguments)
        {
            return new CodeObjectCreateExpression(type, creationArguments);
        }

        /// <summary>
        /// Generates object creation expression.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="creationArguments">The creation arguments.</param>
        /// <returns>Object creation expression.</returns>
        public static CodeExpression New(Type type, params CodeExpression[] creationArguments)
        {
            return new CodeObjectCreateExpression(type, creationArguments);
        }

        /// <summary>
        /// Generates object creation expression.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="creationArguments">The creation arguments.</param>
        /// <returns>Object creation expression.</returns>
        public static CodeExpression New(string type, params CodeExpression[] creationArguments)
        {
            return new CodeObjectCreateExpression(type, creationArguments);
        }

        /// <summary>
        /// Generates a new array of objects expression.
        /// </summary>
        /// <param name="value">The value of the array.</param>
        /// <returns>Array expression.</returns>
        public static CodeExpression Array(Array value)
        {
            CodeExpression[] codeExpressions = new CodeExpression[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                codeExpressions[i] = Code.ObjectValue(value.GetValue(i));
            }

            return new CodeArrayCreateExpression(new CodeTypeReference(value.GetType()), codeExpressions);
        }

        /// <summary>
        /// Creates a code expression for an anonymous array with the given elements.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>Code expression for an anonymous array</returns>
        public static CodeExpression Array(params CodeExpression[] elements)
        {
            return new CodeAnonymousArrayExpression(elements);
        }

        /// <summary>
        /// Creates the generic type reference with given base name and argument names.
        /// </summary>
        /// <param name="baseTypeName">Name of the base type.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Generic type reference.</returns>
        public static CodeTypeReference GenericType(string baseTypeName, params CodeTypeReference[] typeArguments)
        {
            return new CodeTypeReference(baseTypeName, typeArguments);
        }

        /// <summary>
        /// Creates the generic type reference with given base name and argument names.
        /// </summary>
        /// <param name="baseTypeName">Name of the base type.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Generic type reference.</returns>
        public static CodeTypeReference GenericType(string baseTypeName, params string[] typeArguments)
        {
            return new CodeTypeReference(baseTypeName, GenericArguments(typeArguments));
        }

        /// <summary>
        /// Constructs CodeTypeReference for the specific generic argument name.
        /// </summary>
        /// <param name="argumentName">The argument name.</param>
        /// <returns>CodeTypeReference representing generic argument.</returns>
        public static CodeTypeReference GenericArgument(string argumentName)
        {
            return new CodeTypeReference(new CodeTypeParameter(argumentName));
        }

        /// <summary>
        /// Constructs an array of CodeTypeRefernce[] given array of type names..
        /// </summary>
        /// <param name="argumentNames">The argument names.</param>
        /// <returns>Array of CodeTypeReference.</returns>
        public static CodeTypeReference[] GenericArguments(params string[] argumentNames)
        {
            return argumentNames.Select(GenericArgument).ToArray();
        }

        /// <summary>
        /// Assigns one expression to another.
        /// </summary>
        /// <param name="left">Expression to be assigned to.</param>
        /// <param name="right">Expression to assign.</param>
        /// <returns>Assignment expression.</returns>
        public static CodeStatement Assign(this CodeExpression left, CodeExpression right)
        {
            return new CodeAssignStatement(left, right);
        }

        /// <summary>
        /// Generates an expression to compare two expressions using value equality or identity equality
        /// </summary>
        /// <param name="left">Left expression to be compared.</param>
        /// <param name="right">Right expression to be compared.</param>
        /// <param name="operandType">The type being compared</param>
        /// <returns>Expression to return true if the two expressions are equal.</returns>
        public static CodeExpression Equal(this CodeExpression left, CodeExpression right, DataType operandType)
        {
            return UseValueEquality(operandType) ? left.ValueEquals(right) : left.IdentityEquals(right);
        }

        /// <summary>
        /// Generates an expression to compare two expressions using Object.Equals()
        /// </summary>
        /// <param name="left">Left expression to be compared.</param>
        /// <param name="right">Right expression to be compared.</param>
        /// <returns>Expression to return true if the two expressions are equal.</returns>
        public static CodeExpression ObjectEquals(this CodeExpression left, CodeExpression right)
        {
            return new CodeTypeReferenceExpression(typeof(object)).Call("Equals", left, right);
        }

        /// <summary>
        /// Generates an expression to compare two expressions using value inequality or identity inequality
        /// </summary>
        /// <param name="left">Left expression to be compared.</param>
        /// <param name="right">Right expression to be compared.</param>
        /// <param name="operandType">The type being compared</param>
        /// <returns>Code Expression to return true if the two expressions are not equal</returns>
        public static CodeExpression NotEqual(this CodeExpression left, CodeExpression right, DataType operandType)
        {
            return UseValueEquality(operandType) ? left.ValueNotEquals(right) : left.IdentityNotEquals(right);
        }

        /// <summary>
        /// Generates an expression to determine if an expression is not null.
        /// </summary>
        /// <param name="expression">Expression to be checked.</param>
        /// <returns>Expression to return true if the expressions is not null.</returns>
        public static CodeExpression NotNull(this CodeExpression expression)
        {
            return expression.IdentityNotEquals(Null());
        }

        /// <summary>
        /// Generates an expression to perform logical AND on 2 expressions.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The logical AND Expression of the two input expressions.</returns>
        public static CodeExpression BooleanAnd(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanAnd, right);
        }

        /// <summary>
        /// Generates an expression to create a string of boolean and expressions from a list of input expressions
        /// </summary>
        /// <param name="arguments">The list of expressions that need to be AND'ed together</param>
        /// <returns>The list of expressions AND'ed together</returns>
        public static CodeExpression BooleanAnd(this IEnumerable<CodeExpression> arguments)
        {
            CodeExpression result = null;

            foreach (var arg in arguments)
            {
                if (result != null)
                {
                    result = result.BooleanAnd(arg);
                }
                else
                {
                    result = arg;
                }
            }

            return result ?? Primitive(true);
        }

        /// <summary>
        /// Generates an expression to perform logical OR on 2 expressions.
        /// </summary>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression.</param>
        /// <returns>The logical OR Expression of the two input expressions.</returns>
        public static CodeExpression BooleanOr(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BooleanOr, right);
        }

        /// <summary>
        /// Generates an expression to add one expression to another
        /// </summary>
        /// <param name="left">Left expression to be added.</param>
        /// <param name="right">Right expression to be added.</param>
        /// <returns>Expression to perform the addition.</returns>
        public static CodeExpression Add(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.Add, right);
        }

        /// <summary>
        /// Generates an expression to subtract one expression from another
        /// </summary>
        /// <param name="left">Expression to be subtracted from.</param>
        /// <param name="right">Expression to subtract.</param>
        /// <returns>Expression to perform the subtraction.</returns>
        public static CodeExpression Subtract(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.Subtract, right);
        }

        /// <summary>
        /// Bitwise or enums together
        /// </summary>
        /// <param name="values">List of Enums to be Bitwise Ored together</param>
        /// <returns>Bitwise ored enums</returns>
        public static CodeExpression BitwiseOrEnums(params Enum[] values)
        {
            CodeExpression expr = null;
            foreach (Enum value in values)
            {
                if (expr == null)
                {
                    expr = EnumValue(value);
                }
                else
                {
                    expr = new CodeBinaryOperatorExpression(expr, CodeBinaryOperatorType.BitwiseOr, EnumValue(value));
                }
            }

            return expr;
        }

        /// <summary>
        /// Returns the CodeExpression representing the value of the enumeration.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <returns>Expression representing value of the enumeration</returns>
        public static CodeExpression EnumValue(Enum value)
        {
            return new CodeTypeReferenceExpression(value.GetType()).Field(Enum.GetName(value.GetType(), value));
        }

        /// <summary>
        /// Adds the namespace to the compile unit.
        /// </summary>
        /// <param name="codeCompileUnit">The code compile unit.</param>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <returns>Added namespace.</returns>
        public static CodeNamespace AddNamespace(this CodeCompileUnit codeCompileUnit, string namespaceName)
        {
            var codeNamespace = new CodeNamespace(namespaceName);
            codeCompileUnit.Namespaces.Add(codeNamespace);
            return codeNamespace;
        }

        /// <summary>
        /// Adds namespace import (using) into the specified namespace.
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="usedNamespace">The used namespace.</param>
        public static void ImportNamespace(this CodeNamespace codeNamespace, string usedNamespace)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport(usedNamespace));
        }

        /// <summary>
        /// Declares the type in the specified namespace
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="typeAttributes">The type attributes.</param>
        /// <returns>Declared type.</returns>
        public static CodeTypeDeclaration DeclareType(this CodeNamespace codeNamespace, string typeName, TypeAttributes typeAttributes)
        {
            var type = new CodeTypeDeclaration(typeName);
            type.TypeAttributes = typeAttributes;
            codeNamespace.Types.Add(type);
            return type;
        }

        /// <summary>
        /// Declares the public type in the specified namespace
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Declared type.</returns>
        public static CodeTypeDeclaration DeclareType(this CodeNamespace codeNamespace, string typeName)
        {
            return codeNamespace.DeclareType(typeName, TypeAttributes.Public);
        }

        /// <summary>
        /// Declares the public type to house the extension methods in the specified namespace
        /// </summary>
        /// <param name="codeNamespace">The code namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Declared type.</returns>
        public static CodeTypeDeclaration DeclareExtensionMethodContainerType(this CodeNamespace codeNamespace, string typeName)
        {
            codeNamespace.ImportNamespace("System.Runtime.CompilerServices");

            // CodeDOM does not generate static classes, add a token in the the class name so we can add "static" to the class declaration later
            var type = codeNamespace.DeclareType(ExtendedCodeGenerator.StaticClassNameToken + typeName);
            type.IsClass = true;

            // ensure VB code uses Module instead of class for extension methods
            type.UserData.Add("Module", true);

            return type;
        }

        /// <summary>
        /// Insert an extenstion method in the specified class.
        /// </summary>
        /// <param name="containerClass">The class to insert the extension method.</param>
        /// <param name="methodName">The name of the function to insert.</param>
        /// <param name="bindingTypeParameter">The binding type function parameter.</param>
        /// <returns>Declared method.</returns>
        public static CodeMemberMethod AddExtensionMethod(this CodeTypeDeclaration containerClass, string methodName, CodeParameterDeclarationExpression bindingTypeParameter)
        {
            // CodeDOM does not generate extension method, add a token in the method name so we can add "this" for the binding parameter later
            CodeMemberMethod method = containerClass.AddMethod(methodName + ExtendedCodeGenerator.ExtensionMethodNameToken, MemberAttributes.Public | MemberAttributes.Static);

            // add Extension attribute
            method.AddCustomAttribute(Code.TypeRef("Extension"));

            // add the binding type as the first parameter
            method.Parameters.Add(bindingTypeParameter);
            return method;
        }

        /// <summary>
        /// Declares a constructor in the specified type.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <returns>Declared method.</returns>
        public static CodeConstructor AddConstructor(this CodeTypeDeclaration type)
        {
            var ctor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
            };

            type.Members.Add(ctor);
            return ctor;
        }

        /// <summary>
        /// Declares a public method in the specified type.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>Declared method.</returns>
        public static CodeMemberMethod AddMethod(this CodeTypeDeclaration type, string methodName)
        {
            return type.AddMethod(methodName, MemberAttributes.Public | MemberAttributes.Final);
        }

        /// <summary>
        /// Declares a method in the specified type.
        /// </summary>
        /// <param name="type">The containing type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="memberAttributes">The member attributes.</param>
        /// <returns>Declared method.</returns>
        public static CodeMemberMethod AddMethod(this CodeTypeDeclaration type, string methodName, MemberAttributes memberAttributes)
        {
            var method = new CodeMemberMethod
            {
                Name = methodName,
                Attributes = memberAttributes,
            };

            type.Members.Add(method);
            return method;
        }

        /// <summary>
        /// Adds the field to the given type.
        /// </summary>
        /// <param name="type">The type to add field to.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Added fieldName.</returns>
        public static CodeMemberField AddField(this CodeTypeDeclaration type, CodeTypeReference fieldType, string fieldName)
        {
            var field = new CodeMemberField
            {
                Name = fieldName,
                Attributes = MemberAttributes.Private,
                Type = fieldType,
            };

            type.Members.Add(field);
            return field;
        }

        /// <summary>
        /// Adds the Event to the given type.
        /// </summary>
        /// <param name="type">The type to add event to.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns>Added eventName.</returns>
        public static CodeMemberEvent AddEvent(this CodeTypeDeclaration type, CodeTypeReference eventType, string eventName)
        {
            var eventField = new CodeMemberEvent
            {
                Name = eventName,
                Attributes = MemberAttributes.Public,
                Type = eventType,
            };

            type.Members.Add(eventField);
            return eventField;
        }

        /// <summary>
        /// Adds the property to the given type.
        /// </summary>
        /// <param name="type">The type to add property to.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Added property.</returns>
        public static CodeMemberProperty AddProperty(this CodeTypeDeclaration type, CodeTypeReference propertyType, string propertyName)
        {
            var property = new CodeMemberProperty()
            {
                Name = propertyName,
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Type = propertyType,
            };

            type.Members.Add(property);
            return property;
        }

        /// <summary>
        /// Adds automatically implemented property to the given type.
        /// </summary>
        /// <param name="type">The type to add property to.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Added property.</returns>
        public static CodeMemberAutoImplementedProperty AddAutoImplementedProperty(this CodeTypeDeclaration type, CodeTypeReference propertyType, string propertyName)
        {
            var property = new CodeMemberAutoImplementedProperty
            {
                Name = propertyName,
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Type = propertyType,
            };

            type.Members.Add(property);
            return property;
        }

        /// <summary>
        /// Sets the base type for a type.
        /// </summary>
        /// <param name="type">The type to modify.</param>
        /// <param name="baseType">Type base type.</param>
        /// <returns>The input type</returns>
        public static CodeTypeDeclaration InheritsFrom(this CodeTypeDeclaration type, string baseType)
        {
            return type.InheritsFrom(new CodeTypeReference(baseType));
        }

        /// <summary>
        /// Sets the base type for a type.
        /// </summary>
        /// <param name="type">The type to modify.</param>
        /// <param name="baseType">Type base type.</param>
        /// <returns>The input type</returns>
        public static CodeTypeDeclaration InheritsFrom(this CodeTypeDeclaration type, CodeTypeReference baseType)
        {
            type.BaseTypes.Add(baseType);
            return type;
        }

        /// <summary>
        /// Sets the specified member to be virtual.
        /// </summary>
        /// <typeparam name="T">Type of the member</typeparam>
        /// <param name="member">The member.</param>
        /// <returns>The member (suitable for chaining calls together)</returns>
        public static T SetVirtual<T>(this T member)
            where T : CodeTypeMember
        {
            member.Attributes &= ~MemberAttributes.Final;
            return member;
        }

        /// <summary>
        /// Sets the specified type to be abstract.
        /// </summary>
        /// <param name="type">The type to mark as abstract.</param>
        /// <returns>Input type (suitable for chaining calls together).</returns>
        public static CodeTypeDeclaration SetAbstract(this CodeTypeDeclaration type)
        {
            type.TypeAttributes |= TypeAttributes.Abstract;
            return type;
        }

        /// <summary>
        /// Adds the custom attribute to the given object.
        /// </summary>
        /// <typeparam name="T">Object type (can be class or member)</typeparam>
        /// <param name="codeObject">The code object.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>Input object (suitable for chaining calls together).</returns>
        public static CodeAttributeDeclaration AddCustomAttribute<T>(this T codeObject, Type attributeType)
            where T : CodeTypeMember
        {
            return codeObject.AddCustomAttribute(new CodeTypeReference(attributeType));
        }

        /// <summary>
        /// Adds the custom attribute to the given object.
        /// </summary>
        /// <typeparam name="T">Object type (can be class or member)</typeparam>
        /// <param name="codeObject">The code object.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>Input object (suitable for chaining calls together).</returns>
        public static CodeAttributeDeclaration AddCustomAttribute<T>(this T codeObject, CodeTypeReference attributeType)
            where T : CodeTypeMember
        {
            var attribute = new CodeAttributeDeclaration(attributeType);
            codeObject.CustomAttributes.Add(attribute);
            return attribute;
        }

        /// <summary>
        /// Adds the custom attribute to the given object
        /// </summary>
        /// <typeparam name="T">Object type (can be class or member)</typeparam>
        /// <param name="codeObject">The code object.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="args">CodeAttribute Argument list</param>
        /// <returns>Input object (suitable for chaining calls together).</returns>
        public static CodeAttributeDeclaration AddCustomAttribute<T>(this T codeObject, Type attributeType, CodeAttributeArgument[] args)
            where T : CodeTypeMember
        {
            return codeObject.AddCustomAttribute(new CodeTypeReference(attributeType), args);
        }

        /// <summary>
        /// Adds the custom attribute to the given object.
        /// </summary>
        /// <typeparam name="T">Object type (can be class or member)</typeparam>
        /// <param name="codeObject">The code object.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="args">CodeAttribute Argument list</param>
        /// <returns>Input object (suitable for chaining calls together).</returns>
        public static CodeAttributeDeclaration AddCustomAttribute<T>(this T codeObject, CodeTypeReference attributeType, CodeAttributeArgument[] args)
            where T : CodeTypeMember
        {
            var attribute = new CodeAttributeDeclaration(attributeType, args);
            codeObject.CustomAttributes.Add(attribute);
            return attribute;
        }

        /// <summary>
        /// Creates code-dom representations of the given attribute arguments
        /// </summary>
        /// <param name="arguments">The arguments</param>
        /// <returns>The code-dom representations of the arguments</returns>
        public static CodeAttributeArgument[] CustomAttributeArguments(IEnumerable<KeyValuePair<string, object>> arguments)
        {
            return arguments.Select(pair => Code.CustomAttributeArgument(pair.Key, pair.Value)).ToArray();
        }

        /// <summary>
        /// Creates a code-dom attribute-argument with the given name and value.
        /// </summary>
        /// <param name="name">The name of the argument or null</param>
        /// <param name="value">The value of the argument</param>
        /// <returns>The attribute argument</returns>
        public static CodeAttributeArgument CustomAttributeArgument(string name, object value)
        {
            var valueExpression = value as CodeExpression;
            if (valueExpression == null)
            {
                // Enums are not the same as primitives, so call the enum-specific API
                // NOTE: if this has to be replicated elsewhere, consider changing Code.Primitive to handle enums
                var enumValue = value as Enum;
                if (enumValue != null)
                {
                    valueExpression = Code.EnumValue(enumValue);
                }
                else
                {
                    valueExpression = Code.Primitive(value);
                }
            }

            return new CodeAttributeArgument(name, valueExpression);
        }

        /// <summary>
        /// Adds the generic type argument to the method argument list.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeParameterName">Name of the type parameter.</param>
        /// <returns>The input method.</returns>
        public static CodeMemberMethod WithGenericParameter(this CodeMemberMethod method, string typeParameterName)
        {
            method.TypeParameters.Add(new CodeTypeParameter(typeParameterName));
            return method;
        }

        /// <summary>
        /// Sets the return type for a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="returnType">Return type for the method.</param>
        /// <returns>The input method.</returns>
        public static CodeMemberMethod WithReturnType(this CodeMemberMethod method, CodeTypeReference returnType)
        {
            method.ReturnType = returnType;
            return method;
        }

        /// <summary>
        /// Sets the return type for a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="returnTypeName">Name of the return type.</param>
        /// <returns>The input method.</returns>
        public static CodeMemberMethod WithReturnType(this CodeMemberMethod method, string returnTypeName)
        {
            method.ReturnType = new CodeTypeReference(returnTypeName);
            return method;
        }

        /// <summary>
        /// Adds the argument to the method argument list.
        /// </summary>
        /// <typeparam name="TMethod">The type of the method.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns>The input method.</returns>
        public static TMethod WithArgument<TMethod>(this TMethod method, CodeTypeReference argumentType, string argumentName)
            where TMethod : CodeMemberMethod
        {
            method.Parameters.Add(new CodeParameterDeclarationExpression(argumentType, argumentName));
            return method;
        }

        /// <summary>
        /// Add a ParameterDeclaration to a method
        /// </summary>
        /// <param name="codeMember">Method to add parameter declaration to</param>
        /// <param name="codeTypeReference">TypeRef to specify</param>
        /// <param name="name">name of the parameter</param>
        /// <param name="fieldDirection">Direction of the parameter</param>
        /// <typeparam name="TMethod">CodeMemberMethod Type</typeparam>
        /// <returns>A CodeParameterDeclarationExpression</returns>
        public static CodeParameterDeclarationExpression WithArgument<TMethod>(this TMethod codeMember, CodeTypeReference codeTypeReference, string name, FieldDirection fieldDirection)
            where TMethod : CodeMemberMethod
        {
            var parameter = new CodeParameterDeclarationExpression(codeTypeReference, name)
            {
                Direction = fieldDirection
            };

            codeMember.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds the base constructor argument to the constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="baseConstructorArgument">The base constructor argument.</param>
        /// <returns>The input constructor.</returns>
        public static CodeConstructor WithBaseConstructorArgument(this CodeConstructor constructor, CodeExpression baseConstructorArgument)
        {
            constructor.BaseConstructorArgs.Add(baseConstructorArgument);
            return constructor;
        }

        /// <summary>
        /// Decides if we need to use Value or Identity equality based on the input data type
        /// </summary>
        /// <param name="dataType">The input data type</param>
        /// <returns>A true or false indicating if Value inequality needs to be used or not</returns>
        public static bool UseValueEquality(DataType dataType)
        {
            if (dataType is BinaryDataType)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Casts the specified expression to the specified type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="targetType">Target expression type.</param>
        /// <returns>Expression with applied Cast() operator.</returns>
        public static CodeExpression Cast(this CodeExpression expression, CodeTypeReference targetType)
        {
            return new CodeCastExpression(targetType, expression);
        }

        /// <summary>
        /// Casts the specified expression to the specified type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="targetType">Target expression type.</param>
        /// <returns>Expression with applied Cast() operator.</returns>
        public static CodeExpression Cast(this CodeExpression expression, Type targetType)
        {
            return new CodeCastExpression(targetType, expression);
        }

        /// <summary>
        /// Creates a type reference to the specified type.
        /// </summary>
        /// <param name="clrType">Clr type to refer to.</param>
        /// <returns>Type reference.</returns>
        public static CodeTypeReference TypeRef(Type clrType)
        {
            return new CodeTypeReference(clrType);
        }

        /// <summary>
        /// Creates a type reference to the specified type.
        /// </summary>
        /// <param name="clrTypeName">Name of the CLR type.</param>
        /// <returns>Type reference.</returns>
        public static CodeTypeReference TypeRef(string clrTypeName)
        {
            return new CodeTypeReference(clrTypeName);
        }

        /// <summary>
        /// Creates a type reference to the specified type.
        /// </summary>
        /// <typeparam name="TType">The type to refer to.</typeparam>
        /// <returns>Type reference.</returns>
        public static CodeTypeReference TypeRef<TType>()
        {
            return TypeRef(typeof(TType));
        }

        /// <summary>
        /// Applies exclusive or (XOR) operator to two expressions
        /// </summary>
        /// <param name="left">Lhe left side of the expression</param>
        /// <param name="right">The right side of the expression</param>
        /// <returns>The newly constructed expression</returns>
        public static CodeExpression Xor(this CodeExpression left, CodeExpression right)
        {
            return new CodeExclusiveOrExpression(left, right);
        }

        /// <summary>
        /// Constructs a greater-than expression.
        /// </summary>
        /// <param name="left">The left side of the expression</param>
        /// <param name="right">The right side of the expression</param>
        /// <returns>The newly constructed binary operator expression</returns>
        public static CodeExpression GreaterThan(this CodeExpression left, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.GreaterThan, right);
        }

        /// <summary>
        /// Constructs a code which invokes a delegate with the given parameters.
        /// </summary>
        /// <param name="delegateInstance">The delegate instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Code which invokes a delegate.</returns>
        public static CodeExpression InvokeDelegate(this CodeExpression delegateInstance, params CodeExpression[] parameters)
        {
            return new CodeDelegateInvokeExpression(delegateInstance, parameters);
        }

        /// <summary>
        /// Constructs a code statement collection which locks the given argument while executing the given statements
        /// </summary>
        /// <param name="lockArgument">The value to lock</param>
        /// <param name="statements">The statements to execute while the value is locked</param>
        /// <returns>The code statements for the locked execution of the given statements</returns>
        public static CodeStatementCollection Lock(CodeExpression lockArgument, params CodeStatement[] statements)
        {
            var lockStatments = new CodeStatementCollection();
            lockStatments.Add(Code.Type("System.Threading.Monitor").Call("Enter", lockArgument));
            lockStatments.Add(new CodeTryCatchFinallyStatement(
                statements,
                new CodeCatchClause[0],
                Code.Statements(new CodeExpressionStatement(Code.Type("System.Threading.Monitor").Call("Exit", lockArgument)))));
            return lockStatments;
        }

        /// <summary>
        /// Constructs a ternary expression with the given arguments
        /// </summary>
        /// <param name="condition">An expression for the condition to test for.</param>
        /// <param name="ifTrue">An expression for the value if condition is true.</param>
        /// <param name="ifFalse">An expression for the value if condition is false.</param>
        /// <returns>A ternary code expression</returns>
        public static CodeExpression Ternary(CodeExpression condition, CodeExpression ifTrue, CodeExpression ifFalse)
        {
            return new CodeTernaryExpression(condition, ifTrue, ifFalse);
        }
    }
}
