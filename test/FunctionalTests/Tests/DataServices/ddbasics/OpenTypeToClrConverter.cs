//---------------------------------------------------------------------
// <copyright file="OpenTypeToClrConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class OpenTypeToClrConverter : ExpressionVisitor
    {
        protected OpenTypeToClrConverter(IDataServiceQueryProvider dsp, string typeNamePropertyName)
        {
            this.Evaluator = new RuntimeEvaluator();
            this.Provider = dsp;
            this.TypeNamePropertyName = typeNamePropertyName;
        }

        private IDataServiceQueryProvider Provider
        {
            get;
            set;
        }

        private RuntimeEvaluator Evaluator
        {
            get;
            set;
        }

        private string TypeNamePropertyName
        {
            get;
            set;
        }

        public static Expression ToClrExpression(Expression input, IDataServiceQueryProvider dsp, string typeNamePropertyName)
        {
            return new OpenTypeToClrConverter(dsp, typeNamePropertyName).Visit(input);
        }

        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            MethodInfo method = m.Method;
            if (method.ReflectedType == typeof(DataServiceProviderMethods))
            {
                switch (method.Name)
                {
                    case "GetValue":
                        return this.ConvertGetValue(m);

                    case "GetSequenceValue":
                        return this.ConvertGetSequenceValue(m);

                    case "Convert":
                        return this.ConvertCast(m);
                    case "TypeIs":
                        return this.ConvertTypeIs(m);

                    case "OfType":
                        return this.ConvertOfType(m);
                }
            }
            else
                if (method.ReflectedType == typeof(OpenTypeMethods))
                {
                    switch (method.Name)
                    {
                        case "GetValue":
                            return this.ConvertGetValue(m);

                        case "AndAlso":
                        case "OrElse":
                            return this.ConvertLogicalOperator(m);

                        case "Concat":
                            return this.ConvertConcat(m);
                        case "EndsWith":
                            return this.ConvertEndsWith(m);
                        case "IndexOf":
                            return this.ConvertIndexOf(m);
                        case "Length":
                            return this.ConvertLength(m);
                        case "Replace":
                            return this.ConvertReplace(m);
                        case "StartsWith":
                            return this.ConvertStartsWith(m);
                        case "Substring":
                            return this.ConvertSubstring(m);
                        case "Contains":
                            return this.ConvertContains(m);
                        case "ToLower":
                            return this.ConvertToLower(m);
                        case "ToUpper":
                            return this.ConvertToUpper(m);
                        case "Trim":
                            return this.ConvertTrim(m);

                        case "Convert":
                            return this.ConvertCast(m);
                        case "TypeIs":
                            return this.ConvertTypeIs(m);

                        case "Round":
                        case "Floor":
                        case "Ceiling":
                            return this.ConvertNumericMethod(m);

                        case "Year":
                        case "Month":
                        case "Day":
                        case "Hour":
                        case "Minute":
                        case "Second":
                            return this.ConvertDateTimeMethod(m);
                    }
                }

            return base.VisitMethodCall(m);
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            MethodInfo method = b.Method;

            if (method != null && method.ReflectedType == typeof(OpenTypeMethods))
            {
                switch (method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Modulo":
                        return this.ConvertArithmeticOperator(method.Name, b);

                    case "LessThan":
                    case "LessThanOrEqual":
                    case "GreaterThan":
                    case "GreaterThanOrEqual":
                    case "Equal":
                    case "NotEqual":
                        return this.ConvertComparisonOperator(ExpressionTypeFromOperatorName(method.Name), b);
                }
            }

            return base.VisitBinary(b);
        }

        internal override Expression VisitUnary(UnaryExpression u)
        {
            MethodInfo method = u.Method;
            if (method != null && method.ReflectedType == typeof(OpenTypeMethods))
            {
                switch (method.Name)
                {
                    case "Not":
                    case "Negate":
                        return this.ConvertUnaryOperator(method.Name, u);
                }
            }
            else
            if (u.Type == typeof(bool))
            {
                Expression operand = this.Visit(u.Operand);
                return Expression.MakeUnary(
                            u.NodeType, 
                                Expression.Call(
                                    typeof(OpenTypeToClrConverter).GetMethod("ConvertToBoolean", BindingFlags.Static | BindingFlags.NonPublic),
                                    Expression.Convert(operand, typeof(object))), 
                            u.Type, 
                            u.Method);
            }
            
            return base.VisitUnary(u);
        }

        /// <summary>Transalates a call to GetValue into IDSP.GetPropertyValue/GetOpenPropertyValue.</summary>
        /// <param name="m">Method invoked.</param>
        /// <returns>Translated call to IDSP method.</returns>
        protected virtual Expression ConvertGetValue(MethodCallExpression m)
        {
            ParameterInfo[] p = m.Method.GetParameters();

            String methodName = p[1].ParameterType == typeof(string) ? "GetOpenPropertyValue" : "GetPropertyValue";

            if (p[1].ParameterType == typeof(ResourceProperty))
            {
                MethodInfo mi = typeof(IDataServiceQueryProvider).GetMethod(
                    "GetPropertyValue",
                    new Type[] { p[0].ParameterType, p[1].ParameterType });

                return Expression.Call(
                    Expression.Constant(this.Provider, typeof(IDataServiceQueryProvider)),
                    mi,
                    this.VisitExpressionList(m.Arguments));
            }
            else
            {
                MethodInfo mi = typeof(OpenTypeToClrConverter).GetMethod(
                                    "GetAnyPropertyValue", 
                                    BindingFlags.NonPublic | BindingFlags.Instance);

                return Expression.Call(
                    Expression.Constant(this, typeof(OpenTypeToClrConverter)),
                    mi,
                    this.VisitExpressionList(m.Arguments));
            }
        }

        protected virtual Expression ConvertOfType(MethodCallExpression m)
        {
            ResourceType targetResourceType = ((ConstantExpression)m.Arguments[1]).Value as ResourceType;
            Expression source = this.Visit(m.Arguments[0]);

            ParameterExpression parameter = Expression.Parameter(m.Method.GetGenericArguments()[0], "element");
            Expression body = Expression.Equal(Expression.Property(parameter, this.TypeNamePropertyName), Expression.Constant(targetResourceType.FullName));
            Expression lambda = Expression.Lambda(body, parameter);

            source = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { parameter.Type },
                source,
                Expression.Quote(lambda));

            MethodInfo methodInfo = typeof(Queryable).GetMethod("Cast");
            methodInfo = methodInfo.MakeGenericMethod(targetResourceType.InstanceType);
            return Expression.Call(methodInfo, source);
        }

        /// <summary>Transalates a call to GetSequenceValue into IDSP.GetPropertyValue.</summary>
        /// <param name="m">Method invoked.</param>
        /// <returns>Translated call to IDSP.GetPropertyValue</returns>
        protected virtual Expression ConvertGetSequenceValue(MethodCallExpression m)
        {
            ParameterInfo[] p = m.Method.GetParameters();

            MethodInfo mi = typeof(IDataServiceQueryProvider).GetMethod(
                "GetPropertyValue",
                new Type[] { p[0].ParameterType, p[1].ParameterType });

            Expression getPropertyValueExpression = Expression.Call(
                Expression.Constant(this.Provider, typeof(IDataServiceQueryProvider)),
                mi,
                this.VisitExpressionList(m.Arguments));

            MethodInfo sequencerMi = null;
            if (m.Method.IsGenericMethod)
            {
                sequencerMi = typeof(IRuntimeEvaluator).GetMethod(
                    "SequenceGetValueGeneric",
                    new Type[] { typeof(object) });
                sequencerMi = sequencerMi.MakeGenericMethod(m.Method.GetGenericArguments()[0]);
            }
            else
            {
                sequencerMi = typeof(IRuntimeEvaluator).GetMethod(
                    "SequenceGetValue",
                    new Type[] { typeof(object) });
            }

            return Expression.Call(Expression.Constant(this.Evaluator, typeof(IRuntimeEvaluator)),
                sequencerMi,
                getPropertyValueExpression);
        }

        /// <summary>
        /// Convert an arithmetic operator to ILateBoundRuntimeEvaluator's equivalence.
        /// </summary>
        /// <param name="methodName">The arithmetic method name</param>
        /// <param name="b">The original method call expression</param>
        /// <returns>A new instance of method call expression without the LateBound methods</returns>
        protected virtual Expression ConvertArithmeticOperator(String methodName, BinaryExpression b)
        {
            MethodInfo mi = typeof(IRuntimeEvaluator).GetMethod(methodName, new Type[] { typeof(object), typeof(object) });

            return Expression.Call(Expression.Constant(this.Evaluator, typeof(IRuntimeEvaluator)), mi, this.Visit(b.Left), this.Visit(b.Right));
        }

        /// <summary>Converts a comparsion operator to Comparer.Compare</summary>
        /// <param name="comparsionType">The type of comparsion</param>
        /// <param name="b">The original binary expression</param>
        /// <returns>The converted binary expression</returns>
        protected virtual Expression ConvertComparisonOperator(ExpressionType comparsionType, BinaryExpression b)
        {
            MethodInfo mi = typeof(IRuntimeEvaluator).GetMethod("Compare", new Type[] { typeof(object), typeof(object) });

            Expression call = Expression.Call(
                                    Expression.Constant(this.Evaluator, typeof(IRuntimeEvaluator)),
                                    mi,
                                    Expression.Convert(this.Visit(b.Left), typeof(object)),
                                    Expression.Convert(this.Visit(b.Right), typeof(object)));

            return Expression.MakeBinary(comparsionType, call, Expression.Constant(0, typeof(int)));
        }

        /// <summary>Converts logical expressions.</summary>
        /// <param name="m">AndAlso or OrElse method.</param>
        /// <returns>Linq Expression corresponding to And or Or operators.</returns>
        protected virtual Expression ConvertLogicalOperator(MethodCallExpression m)
        {
            bool isOrElse = m.Method.Name == "OrElse";

            Expression left = Expression.Convert(this.Visit(m.Arguments[0]), typeof(bool));
            Expression right = Expression.Convert(this.Visit(m.Arguments[1]), typeof(bool));

            if (isOrElse)
            {
                return Expression.OrElse(left, right);
            }
            else
            {
                return Expression.AndAlso(left, right);
            }
        }

        /// <summary>Performs a cast of the first argument to second argument type.</summary>
        /// <param name="m">Cast method call.</param>
        /// <returns>Result of cast operation.</returns>
        protected virtual Expression ConvertCast(MethodCallExpression m)
        {
            ConstantExpression ce = m.Arguments[1] as ConstantExpression;
            ResourceType rt = ce.Value as ResourceType;

            PropertyInfo pi = this.TypeNamePropertyName != null ? m.Arguments[0].Type.GetProperty(this.TypeNamePropertyName) : null;

            if (pi != null)
            {
                return Expression.Condition(
                        Expression.Equal(
                            Expression.MakeMemberAccess(this.Visit(m.Arguments[0]), pi),
                            Expression.Constant(rt.FullName)),
                        this.Visit(m.Arguments[0]),
                        Expression.Constant(null, m.Arguments[0].Type));
            }
            else
            {
                if (rt.CanReflectOnInstanceType)
                {
                    return Expression.Convert(this.Visit(m.Arguments[0]), rt.InstanceType);
                }
                else
                {
                    return Expression.Constant(null, m.Arguments[0].Type);
                }
            }
        }

        /// <summary>Checks if type of first argument is the second argument.</summary>
        /// <param name="m">TypeIs method call.</param>
        /// <returns>true if the type is correct, false otherwise.</returns>
        protected virtual Expression ConvertTypeIs(MethodCallExpression m)
        {
            ConstantExpression ce = m.Arguments[1] as ConstantExpression;
            ResourceType rt = ce.Value as ResourceType;

            PropertyInfo pi = this.TypeNamePropertyName != null ? m.Arguments[0].Type.GetProperty(this.TypeNamePropertyName) : null;
            
            if (pi != null)
            {
                return Expression.Equal(
                            Expression.MakeMemberAccess(this.Visit(m.Arguments[0]), pi),
                            Expression.Constant(rt.FullName));
            }
            else
            {
                if (rt.CanReflectOnInstanceType)
                {
                    return Expression.TypeIs(this.Visit(m.Arguments[0]), rt.InstanceType);
                }
                else
                {
                    return Expression.Constant(false);
                }
            }
        }

        /// <summary>Unary operator processor.</summary>
        /// <param name="methodName">Operator name.</param>
        /// <param name="u">Expression for operator.</param>
        /// <returns>Result of operator.</returns>
        protected virtual Expression ConvertUnaryOperator(String methodName, UnaryExpression u)
        {
            MethodInfo mi = typeof(IRuntimeEvaluator).GetMethod(methodName, new Type[] { typeof(object) });
            return Expression.Call(Expression.Constant(this.Evaluator, typeof(IRuntimeEvaluator)), mi, Expression.Convert(this.Visit(u.Operand), typeof(object)));
        }

        /// <summary>Numeric methods such as floor, ceiling, round.</summary>
        /// <param name="m">Method name.</param>
        /// <returns>Result of numeric method.</returns>
        protected virtual Expression ConvertNumericMethod(MethodCallExpression m)
        {
            MethodInfo mi = typeof(IRuntimeEvaluator).GetMethod(m.Method.Name, new Type[] { typeof(object) });
            return Expression.Call(Expression.Constant(this.Evaluator, typeof(IRuntimeEvaluator)), mi, this.Visit(m.Arguments[0]));
        }

        /// <summary>Datetime methods such as year, month, day, hour, minute, second.</summary>
        /// <param name="m">Method name.</param>
        /// <returns>Result of datetime method.</returns>
        protected virtual Expression ConvertDateTimeMethod(MethodCallExpression m)
        {
            System.Reflection.PropertyInfo pi = Type.GetType("System.DateTime").GetProperty(m.Method.Name);
            return Expression.Convert(Expression.Property(Expression.Convert(this.Visit(m.Arguments[0]), typeof(DateTime)), pi), typeof(object));
        }

        #region String methods

        /// <summary>String.Concat</summary>
        /// <param name="m">Concat method.</param>
        /// <returns>Concatenated expression.</returns>
        protected virtual Expression ConvertConcat(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("Concat", new Type[] { typeof(String), typeof(String) });

            return Expression.Convert(
                        Expression.Call(
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.EndsWith</summary>
        /// <param name="m">EndsWith method.</param>
        /// <returns>Result of EndsWith.</returns>
        protected virtual Expression ConvertEndsWith(MethodCallExpression m)
        {
            MethodInfo mi = typeof(string).GetMethod("EndsWith", new Type[] { typeof(String) });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.IndexOf</summary>
        /// <param name="m">IndexOf method.</param>
        /// <returns>Result of IndexOf.</returns>
        protected virtual Expression ConvertIndexOf(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("IndexOf", new Type[] { typeof(String) });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.Length</summary>
        /// <param name="m">Length method.</param>
        /// <returns>Result of Length.</returns>
        protected virtual Expression ConvertLength(MethodCallExpression m)
        {
            PropertyInfo pi = typeof(String).GetProperty("Length", BindingFlags.Instance | BindingFlags.Public);

            return Expression.Convert(
                        Expression.Property(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(string)),
                            pi), 
                        typeof(object));
        }

        /// <summary>String.Replace</summary>
        /// <param name="m">Replace method.</param>
        /// <returns>Result of Replace.</returns>
        protected virtual Expression ConvertReplace(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("Replace", new Type[] { typeof(string), typeof(string) });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String)),
                            Expression.Convert(this.Visit(m.Arguments[2]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.StartsWith</summary>
        /// <param name="m">StartsWith method.</param>
        /// <returns>Result of StartsWith.</returns>
        protected virtual Expression ConvertStartsWith(MethodCallExpression m)
        {
            MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.Substring</summary>
        /// <param name="m">Substring method.</param>
        /// <returns>Result of Substring.</returns>
        protected virtual Expression ConvertSubstring(MethodCallExpression m)
        {
            if (m.Arguments.Count == 2)
            {
                MethodInfo mi = typeof(String).GetMethod("Substring", new Type[] { typeof(int) });

                return Expression.Convert(
                            Expression.Call(
                                Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                                mi,
                                Expression.Convert(this.Visit(m.Arguments[1]), typeof(int))),
                            typeof(object));
            }
            else
            {
                MethodInfo mi = typeof(String).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });

                return Expression.Convert(
                            Expression.Call(
                                Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                                mi,
                                Expression.Convert(this.Visit(m.Arguments[1]), typeof(int)),
                                Expression.Convert(this.Visit(m.Arguments[2]), typeof(int))),
                            typeof(object));
            }
        }

        /// <summary>String.Contains</summary>
        /// <param name="m">Contains method.</param>
        /// <returns>Result of Contains.</returns>
        protected virtual Expression ConvertContains(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("Contains");

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi,
                            Expression.Convert(this.Visit(m.Arguments[1]), typeof(String))), 
                        typeof(object));
        }

        /// <summary>String.ToLower</summary>
        /// <param name="m">ToLower method.</param>
        /// <returns>Result of ToLower.</returns>
        protected virtual Expression ConvertToLower(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("ToLower", new Type[] { });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi), 
                        typeof(object));
        }

        /// <summary>String.ToUpper</summary>
        /// <param name="m">ToUpper method.</param>
        /// <returns>Result of ToUpper.</returns>
        protected virtual Expression ConvertToUpper(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("ToUpper", new Type[] { });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi), 
                        typeof(object));
        }

        /// <summary>String.Trim</summary>
        /// <param name="m">Trim method.</param>
        /// <returns>Result of Trim.</returns>
        protected virtual Expression ConvertTrim(MethodCallExpression m)
        {
            MethodInfo mi = typeof(String).GetMethod("Trim", new Type[] { });

            return Expression.Convert(
                        Expression.Call(
                            Expression.Convert(this.Visit(m.Arguments[0]), typeof(String)),
                            mi), 
                        typeof(object));
        }

        #endregion

        /// <summary>Given name gives the comparison operator expression type.</summary>
        /// <param name="operatorName">Comparison operator name.</param>
        /// <returns>ExpressionType corresponding to operator name.</returns>
        private static ExpressionType ExpressionTypeFromOperatorName(string operatorName)
        {
            switch (operatorName)
            {
                case "LessThan":
                    return ExpressionType.LessThan;
                case "LessThanOrEqual":
                    return ExpressionType.LessThanOrEqual;
                case "GreaterThan":
                    return ExpressionType.GreaterThan;
                case "GreaterThanOrEqual":
                    return ExpressionType.GreaterThanOrEqual;
                case "Equal":
                    return ExpressionType.Equal;
                default:
                    return ExpressionType.NotEqual;
            }
        }

        /// <summary>
        /// Gets the property value of a resource type property where it's unknown whether it is an open property or not.
        /// </summary>
        /// <param name="value">Instance.</param>
        /// <param name="propertyName">Name of property to get.</param>
        /// <returns>Value of the resource property</returns>
        private object GetAnyPropertyValue(object value, string propertyName)
        {
            ResourceType resourceType = this.Provider.GetResourceType(value);
            if (resourceType == null)
            {
                throw new InvalidOperationException("No valid resource type found.");
            }

            // first check non-open properties
            ResourceProperty property = resourceType.Properties.FirstOrDefault(p => p.Name == propertyName);

            if (property != null)
            {
                return this.Provider.GetPropertyValue(value, property);
            }
            else
            {
                return this.Provider.GetOpenPropertyValue(value, propertyName);
            }
        }

        /// <summary>Converts the input object to a boolean value.</summary>
        /// <param name="input">Input object.</param>
        /// <returns>boolean value corresponding to input.</returns>
        private static bool ConvertToBoolean(object input)
        {
            switch (Type.GetTypeCode(input.GetType()))
            {
                case TypeCode.Boolean:
                    return (bool)input;
                case TypeCode.SByte:
                    return (SByte)input != 0;
                case TypeCode.Int16:
                    return (Int16)input != 0;
                case TypeCode.Int32:
                    return (Int32)input != 0;
                case TypeCode.Int64:
                    return (Int64)input != 0;
                case TypeCode.Byte:
                    return (Byte)input != 0;
                case TypeCode.UInt16:
                    return (UInt16)input != 0;
                case TypeCode.UInt32:
                    return (UInt32)input != 0;
                case TypeCode.UInt64:
                    return (UInt64)input != 0;
                default:
                    return false;
            }
        }
    }
}