//---------------------------------------------------------------------
// <copyright file="GroupByProjectionRewriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Rewrites the GroupBy result selector to a form usable by the project plan compiler during materialization.
    /// </summary>
    internal class GroupByProjectionRewriter : ALinqExpressionVisitor
    {
        #region Fields

        private const string IGroupingKey = "Key";
        /// <summary>The input resource expression.</summary>
        private QueryableResourceExpression input;
        /// <summary>The lambda parameter type.</summary>
        private readonly Type lambdaParameterType;
        /// <summary>The lambda parameter expression.</summary>
        private readonly ParameterExpression lambdaParameter;
        /// <summary>Mapping of assigment expressions to their respective member.</summary>
        private IDictionary<Expression, MemberInfo> expressionMap;

        #endregion Fields

        /// <summary>
        /// Creates an <see cref="GroupByProjectionRewriter"/> instance.
        /// </summary>
        /// <param name="input">The input resource expression.</param>
        /// <param name="lambdaParameterType">The lambda parameter type.</param>
        private GroupByProjectionRewriter(QueryableResourceExpression input, Type lambdaParameterType)
        {
            this.input = input;
            this.lambdaParameterType = lambdaParameterType;
            this.lambdaParameter = Expression.Parameter(this.lambdaParameterType, "p1");
        }

        /// <summary>
        /// Rewrites the GroupBy result selector to a form usable by the project plan compiler during materialization.
        /// </summary>
        /// <param name="input">The input resource expression.</param>
        /// <param name="resultSelector">The result selector lambda expression to rewrite.</param>
        /// <returns>Rewritten GroupBy result selector.</returns>
        internal static LambdaExpression Rewrite(QueryableResourceExpression input, LambdaExpression resultSelector)
        {
            Debug.Assert(input != null, "input != null");
            Debug.Assert(resultSelector != null, "resultSelector != null");

            /*
            The result of an aggregation especially when grouping with by navigation property
            will be heavily nested and the property names for the aggregated values will be
            the user's prerogative. E.g.,

            Request: http://service/Sales?$apply=groupby((Product/Color,Product/TaxRate),aggregate(Amount with sum as SumAmount,Amount with average as AverageAmount))
            
            Response:
            {
                "@odata.context": "http://service/$metadata#Sales(Product(Color,TaxRate),SumAmount,AverageAmount)",
                "value": [
                    {
                        "@odata.id": null,
                        "AverageAmount": 2.00,
                        "SumAmount": 4.00,
                        "Product": { "@odata.id": null, "TaxRate": 0.06, "Color": "White" }
                    },
                    {
                        "@odata.id": null,
                        "AverageAmount": 2.00,
                        "SumAmount": 8.00,
                        "Product": { "@odata.id": null, "TaxRate": 0.14, "Color": "White" }
                    }
                ]
            }
            We typically won't have a defined type that fits squarely as a backing type during materialization
            For this reason, we build one dynamically and then rewrite the result selector
            */

            IDictionary<Expression, MemberInfo> expressionMap;

            // Build a type dynamically
            Type materializationType;
            MaterializationTypeBuilder.BuildMaterializationType(input, resultSelector, out materializationType, out expressionMap);

            GroupByProjectionRewriter instance = new GroupByProjectionRewriter(input, materializationType);
            instance.expressionMap = expressionMap;
            
            MemberInitExpression memberInitExpr = resultSelector.Body as MemberInitExpression;

            if (memberInitExpr != null)
            {
                MemberInitExpression miExpr = instance.Visit(memberInitExpr) as MemberInitExpression;

                return Expression.Lambda(Expression.MemberInit(Expression.New(resultSelector.Body.Type), miExpr.Bindings), instance.lambdaParameter);
            }
            else
            {
                NewExpression newExpr = instance.Visit(resultSelector.Body) as NewExpression;

                ConstructorInfo ctor = resultSelector.Body.Type.GetConstructors().LastOrDefault();
                return Expression.Lambda(Expression.New(ctor, newExpr.Arguments, newExpr.Members), instance.lambdaParameter);
            }
        }

        /// <inheritdoc/>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            MemberInfo memberInfo;
            // Parameter may be assigned to a member in some scenarios
            // E.g. .GroupBy(d1 => d1.Prop, (d1, d2) => new { Prop = d1, ... }
            if (this.expressionMap.TryGetValue(p, out memberInfo))
            {
                Debug.Assert(this.input.Apply != null, "this.input.Apply != null");

                // In this context, parameter might point to either
                // 1) a constant, e.g., .GroupBy(d1 => [Constant], (d1, d2) => new { Constant = d1, ... }
                // 2) a grouping expression, e.g. .GroupBy(d1 => d1.Property, (d1, d2) => new { Property = d1, ... }
                KeyValuePair<string, Expression> pair = this.input.Apply.GroupingExpressionsMap.FirstOrDefault();
                
                Expression expr = pair.Value;
                Debug.Assert(expr != null, "expr != null");

                if (expr.NodeType == ExpressionType.Constant)
                {
                    return Expression.Constant(((ConstantExpression)expr).Value);
                }
                
                return BindParameter((MemberExpression)expr, memberInfo);
            }
            else
            {
                return base.VisitParameter(p);
            }
        }

        /// <inheritdoc/>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            // TODO: Validate member body node type?
            // TODO: Confirm behaviour if a property of the property type is applied
            // e.g., .GroupBy(d1 => d1.Prop).Select(d2 => new { d1.Key.Length, ... })
            // Where Prop is of type string and Length is a property of string
            MemberInfo memberInfo;
            if (this.expressionMap.TryGetValue(m, out memberInfo))
            {
                // Member access in current context should map to a grouping expression
                Expression expr;

                // If member name is Key, then we're either:
                // 1) Grouping by a single property, e.g., .GroupBy(d1 => d1.Prop).Select(d2 => new { d2.Key, ... })
                // 2) Grouping by a constant, e.g., .GroupBy(d1 => [Constant]).Select(d2 => new { d2.Key, ... })
                // In such a scenario we should only have one item in the grouping expressions mapping
                if (m.Member.Name.Equals(IGroupingKey, StringComparison.Ordinal)
                    && this.input.Apply.GroupingExpressionsMap.Count == 1)
                {
                    KeyValuePair<string, Expression> pair = this.input.Apply.GroupingExpressionsMap.First();

                    expr = pair.Value;
                    Debug.Assert(expr != null, "expr != null");

                    // Grouping by a constant
                    if (expr.NodeType == ExpressionType.Constant)
                    {
                        return Expression.Constant(((ConstantExpression)expr).Value);
                    }
                }
                else
                {
                    this.input.Apply.GroupingExpressionsMap.TryGetValue(m.Member.Name, out expr);
                }

                MemberExpression memberExpr = expr as MemberExpression;
                Debug.Assert(memberExpr != null);

                return BindParameter(memberExpr, memberInfo);
            }
            
            // Something would have to be wrong for us to find ourselves here
            throw Error.NotSupported();
        }

        /// <summary>
        /// Binds member expression to the lambda parameter.
        /// </summary>
        /// <param name="memberExpr">The member expression.</param>
        /// <param name="memberInfo">The corresponding field or property.</param>
        /// <returns>A bound expression.</returns>
        private Expression BindParameter(MemberExpression memberExpr, MemberInfo memberInfo)
        {
            Debug.Assert(memberExpr != null, "memberExpr != null");
            Debug.Assert(memberInfo != null, "memberInfo != null");

            Stack<MemberExpression> expressionStack = new Stack<MemberExpression>();

            do
            {
                // Push member access expressions into the stack
                expressionStack.Push(memberExpr);

                if (memberExpr.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = (MemberExpression)memberExpr.Expression;
                }
                else
                {
                    break;
                }
            } while (true);

            MemberExpression mExpr = expressionStack.Pop();
            MemberExpression propertyExpr = Expression.PropertyOrField(this.lambdaParameter, mExpr.Member.Name);

            // Construct the member access expression dynamically, e.g. p1.Product.Category.Name
            while(expressionStack.Count != 0)
            {
                mExpr = expressionStack.Pop();

                propertyExpr = Expression.Property(propertyExpr, mExpr.Member.Name);
            }

            return propertyExpr;
        }

        /// <inheritdoc/>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            SequenceMethod sequenceMethod;
            ReflectionUtil.TryIdentifySequenceMethod(m.Method, out sequenceMethod);

            switch (sequenceMethod)
            {
                case SequenceMethod.SumIntSelector:
                case SequenceMethod.SumDoubleSelector:
                case SequenceMethod.SumDecimalSelector:
                case SequenceMethod.SumLongSelector:
                case SequenceMethod.SumSingleSelector:
                case SequenceMethod.SumNullableIntSelector:
                case SequenceMethod.SumNullableDoubleSelector:
                case SequenceMethod.SumNullableDecimalSelector:
                case SequenceMethod.SumNullableLongSelector:
                case SequenceMethod.SumNullableSingleSelector:
                case SequenceMethod.AverageIntSelector:
                case SequenceMethod.AverageDoubleSelector:
                case SequenceMethod.AverageDecimalSelector:
                case SequenceMethod.AverageLongSelector:
                case SequenceMethod.AverageSingleSelector:
                case SequenceMethod.AverageNullableIntSelector:
                case SequenceMethod.AverageNullableDoubleSelector:
                case SequenceMethod.AverageNullableDecimalSelector:
                case SequenceMethod.AverageNullableLongSelector:
                case SequenceMethod.AverageNullableSingleSelector:
                case SequenceMethod.MinIntSelector:
                case SequenceMethod.MinDoubleSelector:
                case SequenceMethod.MinDecimalSelector:
                case SequenceMethod.MinLongSelector:
                case SequenceMethod.MinSingleSelector:
                case SequenceMethod.MinNullableIntSelector:
                case SequenceMethod.MinNullableDoubleSelector:
                case SequenceMethod.MinNullableDecimalSelector:
                case SequenceMethod.MinNullableLongSelector:
                case SequenceMethod.MinNullableSingleSelector:
                case SequenceMethod.MaxIntSelector:
                case SequenceMethod.MaxDoubleSelector:
                case SequenceMethod.MaxDecimalSelector:
                case SequenceMethod.MaxLongSelector:
                case SequenceMethod.MaxSingleSelector:
                case SequenceMethod.MaxNullableIntSelector:
                case SequenceMethod.MaxNullableDoubleSelector:
                case SequenceMethod.MaxNullableDecimalSelector:
                case SequenceMethod.MaxNullableLongSelector:
                case SequenceMethod.MaxNullableSingleSelector:
                case SequenceMethod.Count:
                case SequenceMethod.LongCount:
                    MemberInfo memberInfo;
                    if (this.expressionMap.TryGetValue(m, out memberInfo))
                    {
                        // Project plan compiler wouldn't know how to handle an aggregation method
                        // Replace method call with a property access expression
                        return Expression.PropertyOrField(this.lambdaParameter, memberInfo.Name);
                    }
                    // Something would have to be wrong for us to find ourselves here
                    throw Error.NotSupported();
                default:
                    throw Error.MethodNotSupported(m);
            };
        }

        /// <summary>
        /// Dynamically builds a type to be used as a backing type during materialization.
        /// </summary>
        private class MaterializationTypeBuilder : ALinqExpressionVisitor
        {
            #region Fields

            /// <summary>The input resource expression.</summary>
            private readonly QueryableResourceExpression input;
            /// <summary>Mapping of properties to their respective types.</summary>
            private readonly IDictionary<string, Type> properties;
            /// <summary>Mapping of assigment expressions to their respective member.</summary>
            private readonly IDictionary<Expression, MemberInfo> expressionMap;

            #endregion Fields

            /// <summary>
            /// Creates a <see cref="MaterializationTypeBuilder"/> instance.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            private MaterializationTypeBuilder(QueryableResourceExpression input)
            {
                this.input = input;
                properties = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                this.expressionMap = new Dictionary<Expression, MemberInfo>(ReferenceEqualityComparer<Expression>.Instance);
            }

            /// <summary>
            /// Dynamically builds a type to be used as a backing type during materialization.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="resultSelector">The result selector lambda expression.</param>
            /// <param name="materializationType">The materialization type.</param>
            /// <param name="expressionMap">Mapping of expressions on the result selector and the assigned members.</param>
            internal static void BuildMaterializationType(
                QueryableResourceExpression input,
                LambdaExpression resultSelector,
                out Type materializationType,
                out IDictionary<Expression, MemberInfo> expressionMap)
            {
                Debug.Assert(input != null, "input != null");
                Debug.Assert(input.Apply != null, "input.Apply != null");
                Debug.Assert(resultSelector != null, "resultSelector != null");

                MaterializationTypeBuilder instance = new MaterializationTypeBuilder(input);

                instance.Visit(resultSelector.Body);

                TypeInfo typeInfo =  TypeBuilderUtil.CreateTypeInfo("MaterializationType", instance.properties);
                materializationType = typeInfo.AsType();
                expressionMap = instance.expressionMap;
            }

            /// <inheritdoc/>
            internal override NewExpression VisitNew(NewExpression nex)
            {
                // Map assignment expressions to their respective member
                for (int i = 0; i < nex.Arguments.Count; i++)
                {
                    Expression argument = nex.Arguments[i];
                    this.expressionMap.Add(argument, nex.Members[i]);

                    // Mapping of parameter assignment needs to be handled differently
                    if (argument.NodeType == ExpressionType.Parameter)
                    {
                        AnalyzeParameterAssignment(argument);
                    }
                }

                return base.VisitNew(nex);
            }

            /// <inheritdoc/>
            internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                // Map assignment expression to the respective member
                this.expressionMap.Add(assignment.Expression, assignment.Member);

                // Mapping of parameter assignment needs to be handled differently
                if (assignment.Expression.NodeType == ExpressionType.Parameter)
                {
                    AnalyzeParameterAssignment(assignment.Expression);
                }

                return base.VisitMemberAssignment(assignment);
            }

            /// <inheritdoc/>
            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                // TODO: Validate member body node type?
                // Member access in current context should map to a grouping expression
                Expression expr;

                // If member name is Key, then we're either:
                // 1) Grouping by a single property, e.g., .GroupBy(d1 => d1.Prop).Select(d2 => new { d2.Key, ... })
                // 2) Grouping by a constant, e.g., .GroupBy(d1 => [Constant]).Select(d2 => new { d2.Key, ... })
                // In such a scenario we should only have one item in the grouping expressions mapping
                if (m.Member.Name.Equals(IGroupingKey, StringComparison.Ordinal)
                    && this.input.Apply.GroupingExpressionsMap.Count == 1)
                {
                    KeyValuePair<string, Expression> pair = this.input.Apply.GroupingExpressionsMap.First();

                    expr = pair.Value;
                    Debug.Assert(expr != null, "expr != null");

                    if (expr.NodeType == ExpressionType.Constant) // .GroupBy(d => [Constant])
                    {
                        // Do nothing
                        return m;
                    }
                }
                else
                {
                    this.input.Apply.GroupingExpressionsMap.TryGetValue(m.Member.Name, out expr);
                }

                MemberExpression memberExpr = expr as MemberExpression;
                Debug.Assert(memberExpr != null);

                // For nested member access, e.g. d.Product.Category.Name, 
                // we only target the Product property type
                do
                {
                    if (memberExpr.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        memberExpr = (MemberExpression)memberExpr.Expression;
                    }
                    else
                    {
                        break;
                    }
                } while (true);
                
                // Add relevent property with the expected return type
                if (!this.properties.ContainsKey(memberExpr.Member.Name))
                {
                    properties.Add(memberExpr.Member.Name, memberExpr.Type);
                }

                return m;
            }

            /// <inheritdoc/>
            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                SequenceMethod sequenceMethod;
                ReflectionUtil.TryIdentifySequenceMethod(m.Method, out sequenceMethod);

                switch (sequenceMethod)
                {
                    case SequenceMethod.SumIntSelector:
                    case SequenceMethod.SumDoubleSelector:
                    case SequenceMethod.SumDecimalSelector:
                    case SequenceMethod.SumLongSelector:
                    case SequenceMethod.SumSingleSelector:
                    case SequenceMethod.SumNullableIntSelector:
                    case SequenceMethod.SumNullableDoubleSelector:
                    case SequenceMethod.SumNullableDecimalSelector:
                    case SequenceMethod.SumNullableLongSelector:
                    case SequenceMethod.SumNullableSingleSelector:
                    case SequenceMethod.AverageIntSelector:
                    case SequenceMethod.AverageDoubleSelector:
                    case SequenceMethod.AverageDecimalSelector:
                    case SequenceMethod.AverageLongSelector:
                    case SequenceMethod.AverageSingleSelector:
                    case SequenceMethod.AverageNullableIntSelector:
                    case SequenceMethod.AverageNullableDoubleSelector:
                    case SequenceMethod.AverageNullableDecimalSelector:
                    case SequenceMethod.AverageNullableLongSelector:
                    case SequenceMethod.AverageNullableSingleSelector:
                    case SequenceMethod.MinIntSelector:
                    case SequenceMethod.MinDoubleSelector:
                    case SequenceMethod.MinDecimalSelector:
                    case SequenceMethod.MinLongSelector:
                    case SequenceMethod.MinSingleSelector:
                    case SequenceMethod.MinNullableIntSelector:
                    case SequenceMethod.MinNullableDoubleSelector:
                    case SequenceMethod.MinNullableDecimalSelector:
                    case SequenceMethod.MinNullableLongSelector:
                    case SequenceMethod.MinNullableSingleSelector:
                    case SequenceMethod.MaxIntSelector:
                    case SequenceMethod.MaxDoubleSelector:
                    case SequenceMethod.MaxDecimalSelector:
                    case SequenceMethod.MaxLongSelector:
                    case SequenceMethod.MaxSingleSelector:
                    case SequenceMethod.MaxNullableIntSelector:
                    case SequenceMethod.MaxNullableDoubleSelector:
                    case SequenceMethod.MaxNullableDecimalSelector:
                    case SequenceMethod.MaxNullableLongSelector:
                    case SequenceMethod.MaxNullableSingleSelector:
                    case SequenceMethod.Count:
                    case SequenceMethod.LongCount:
                        MemberInfo memberInfo;
                        if (expressionMap.TryGetValue(m, out memberInfo))
                        {
                            // The member name will be the alias assigned to the aggregated value
                            // We add a relevant property with the expected return type
                            this.properties.Add(memberInfo.Name, m.Type);

                            return m;
                        }
                        // Something would have to be wrong for us to find ourselves here
                        throw Error.NotSupported();
                    default:
                        throw Error.MethodNotSupported(m);
                };
            }

            /// <summary>
            /// Analyzes parameter assignment expression to determine the property it should be mapped to.
            /// </summary>
            /// <param name="expression">The parameter expression</param>
            private void AnalyzeParameterAssignment(Expression expression)
            {
                Debug.Assert(expression != null, "assignment != null");
                Debug.Assert(expression.NodeType == ExpressionType.Parameter, "assignment.NodeType == ExpressionType.Parameter");
                Debug.Assert(this.input.Apply.GroupingExpressionsMap.Count != 0, "this.input.Apply.GroupingExpressionsMap.Count != 0");

                foreach(KeyValuePair<string, Expression> pair in this.input.Apply.GroupingExpressionsMap)
                {
                    // It'll be either a ConstantExpression or MemberExpression
                    MemberExpression memberExpr = pair.Value as MemberExpression;

                    // Nothing needs to be done for ConstantExpression
                    if (memberExpr != null)
                    {
                        this.VisitMemberAccess(memberExpr);
                    }
                }
            }
        }
    }
}
