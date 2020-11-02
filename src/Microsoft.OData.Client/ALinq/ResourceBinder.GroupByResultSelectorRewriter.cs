//---------------------------------------------------------------------
// <copyright file="ResourceBinder.GroupByResultSelectorRewriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    #endregion Namespaces    

    internal partial class ResourceBinder
    {
        /// <summary>
        /// Rewrites the GroupBy result selector to a form usable by the project plan compiler during materialization.
        /// </summary>
        private class GroupByResultSelectorRewriter : ALinqExpressionVisitor
        {
            // The result of an aggregation especially when grouping 
            // by navigation property will be heavily nested 
            // and the property names for the aggregated values will be
            // the user's prerogative. E.g.,
            //
            // Request: http://service/Sales?$apply=groupby(
            // 	(Product/Color,Product/TaxRate),
            // 	aggregate(Amount with sum as SumAmount,Amount with average as AverageAmount))
            // 
            // Response:
            // {
            // 	"@odata.context": "http://service/$metadata#Sales
            // 	(Product(Color,TaxRate),SumAmount,AverageAmount)",
            // 	"value": [
            // 		{
            // 			"@odata.id": null,
            // 			"AverageAmount": 2.00,
            // 			"SumAmount": 4.00,
            // 			"Product": { "@odata.id": null, "TaxRate": 0.06, "Color": "White" }
            // 		},
            // 		{
            // 			"@odata.id": null,
            // 			"AverageAmount": 2.00,
            // 			"SumAmount": 8.00,
            // 			"Product": { "@odata.id": null, "TaxRate": 0.14, "Color": "White" }
            // 		}
            // 	]
            // }
            //
            // We typically won't have a defined type that fits squarely 
            // as a backing type during materialization. For this reason, 
            // we build one dynamically and then rewrite the result selector

            #region Fields

            /// <summary>The name used to reference the grouping key.</summary>
            private const string IGroupingKey = "Key";
            /// <summary>The input resource expression.</summary>
            private QueryableResourceExpression input;
            /// <summary>The lambda parameter type.</summary>
            private readonly Type lambdaParameterType;
            /// <summary>The lambda parameter expression.</summary>
            private readonly ParameterExpression lambdaParameter;
            /// <summary>Mapping of assigment expressions to their respective member.</summary>
            private IDictionary<Expression, MemberInfo> expressionsMap;

            #endregion Fields

            /// <summary>
            /// Creates an <see cref="GroupByResultSelectorRewriter"/> instance.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="lambdaParameterType">The lambda parameter type.</param>
            private GroupByResultSelectorRewriter(QueryableResourceExpression input, Type lambdaParameterType)
            {
                this.input = input;
                this.lambdaParameterType = lambdaParameterType;
                this.lambdaParameter = Expression.Parameter(this.lambdaParameterType, "p1");
            }

            /// <summary>
            /// Rewrites the GroupBy result selector to a form usable by the project plan compiler during materialization.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="resultSelector">The result selector expression to rewrite.</param>
            /// <returns>Rewritten GroupBy result selector.</returns>
            internal static LambdaExpression Rewrite(QueryableResourceExpression input, LambdaExpression resultSelector)
            {
                Debug.Assert(input != null, "input != null");
                Debug.Assert(resultSelector != null, "resultSelector != null");
                Debug.Assert(resultSelector.Body.NodeType == ExpressionType.MemberInit || resultSelector.Body.NodeType == ExpressionType.New,
                    "resultSelector.Body.NodeType == ExpressionType.MemberInit || resultSelector.Body.NodeType == ExpressionType.New");

                IDictionary<Expression, MemberInfo> expressionMap;

                // Build a backing type for the aggregated result dynamically
                Type materializationType;
                MaterializationTypeBuilder.BuildMaterializationType(input, resultSelector, out materializationType, out expressionMap);

                GroupByResultSelectorRewriter rewriterInstance = new GroupByResultSelectorRewriter(input, materializationType);
                rewriterInstance.expressionsMap = expressionMap;

                MemberInitExpression memberInitExpr = resultSelector.Body as MemberInitExpression;

                if (memberInitExpr != null)
                {
                    MemberInitExpression miExpr = rewriterInstance.Visit(memberInitExpr) as MemberInitExpression;

                    return Expression.Lambda(
                        Expression.MemberInit(Expression.New(resultSelector.Body.Type), miExpr.Bindings),
                        rewriterInstance.lambdaParameter);
                }
                else
                {
                    NewExpression newExpr = rewriterInstance.Visit(resultSelector.Body) as NewExpression;

                    ConstructorInfo ctor = resultSelector.Body.Type.GetConstructors().LastOrDefault();
                    return Expression.Lambda(
                        Expression.New(ctor, newExpr.Arguments, newExpr.Members),
                        rewriterInstance.lambdaParameter);
                }
            }

            /// <inheritdoc/>
            internal override Expression VisitParameter(ParameterExpression p)
            {
                MemberInfo memberInfo;
                // Parameter may be assigned to a member in some scenarios
                if (this.expressionsMap.TryGetValue(p, out memberInfo))
                {
                    Debug.Assert(this.input.Apply != null, "this.input.Apply != null");

                    // Scenarios:
                    // 1) Grouping by constant, e.g., GroupBy(d1 => [Constant], (d1, d2) => new { Constant = d1, ... }
                    // 2) Grouping by single property, e.g. GroupBy(d1 => d1.Property, (d1, d2) => new { Property = d1, ... }
                    KeyValuePair<string, Expression> pair = this.input.Apply.GroupingExpressionsMap.Single();

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
                MemberInfo memberInfo;
                if (this.expressionsMap.TryGetValue(m, out memberInfo))
                {
                    // In scenarios like GroupBy(d1 => new { d1.Prop }, (d1, d2) => new { d1.Prop.[KnownPrimitiveTypeProperty], ... })
                    // where Prop is an Edm property, the [KnownPrimitiveTypeProperty] (e.g. Length) will be evaluated on the client side
                    // We strip the non-translated member expressions.
                    // We later take care of the stripped expressions when rewriting the result selector
                    Stack<MemberExpression> strippedExpressions;
                    Expression translatedExpr = StripToTranslatedExpression(m, out strippedExpressions);

                    // In the current context, any member access should map to a grouping expression
                    Expression expr;

                    // Member access expressions involving grouping by constant or single property
                    // - GroupBy(d1 => [Constant])
                    // - GroupBy(d1 => d1.Prop)
                    if (MatchSimpleGroupingExpression(this.input, translatedExpr, out expr))
                    {
                        // Grouping by a constant
                        if (expr.NodeType == ExpressionType.Constant)
                        {
                            // Append the stripped member expressions (if any) into the constant expression
                            return AppendStrippedExpressions(Expression.Constant(((ConstantExpression)expr).Value), strippedExpressions);
                        }
                    }
                    else
                    {
                        Debug.Assert(translatedExpr.NodeType == ExpressionType.MemberAccess,
                            "translatedExpr.NodeType == ExpressionType.MemberAccess");

                        this.input.Apply.GroupingExpressionsMap.TryGetValue(((MemberExpression)translatedExpr).Member.Name, out expr);
                    }

                    MemberExpression memberExpr = expr as MemberExpression;
                    Debug.Assert(memberExpr != null);
                    // Append the stripped member expressions (if any) into the new member access expression
                    memberExpr = (MemberExpression)AppendStrippedExpressions(memberExpr, strippedExpressions);

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
                while (expressionStack.Count != 0)
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
                    case SequenceMethod.CountDistinctSelector:
                        MemberInfo memberInfo;
                        if (this.expressionsMap.TryGetValue(m, out memberInfo))
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
            /// Strips a member expression to the expression that require server-side translation.
            /// </summary>
            /// <param name="memberExpr">The member expression.</param>
            /// <param name="strippedExpressions">The stripped expressions.</param>
            /// <returns>Expression that requires server-side translation.</returns>
            private static Expression StripToTranslatedExpression(MemberExpression memberExpr, out Stack<MemberExpression> strippedExpressions)
            {
                Debug.Assert(memberExpr != null, "memberExpr != null");

                strippedExpressions = new Stack<MemberExpression>();

                if (!PrimitiveType.IsKnownNullableType(memberExpr.Expression.Type))
                {
                    // No reason to proceed further
                    return memberExpr;
                }

                MemberExpression targetExpr = memberExpr;
                // Expression representing the containing object is a member expression (e.g. d1.Prop.Length)
                MemberExpression tempExpr = StripTo<MemberExpression>(memberExpr.Expression);
                while (tempExpr != null && PrimitiveType.IsKnownNullableType(tempExpr.Type))
                {
                    strippedExpressions.Push(targetExpr);
                    targetExpr = tempExpr;
                    tempExpr = StripTo<MemberExpression>(tempExpr.Expression);
                }

                // Expression representing the containing object is a parameter expression (e.g. d1.Length)
                if (PrimitiveType.IsKnownNullableType(targetExpr.Expression.Type)
                    && StripTo<ParameterExpression>(targetExpr.Expression) != null)
                {
                    strippedExpressions.Push(targetExpr);
                    return targetExpr.Expression;
                }

                return targetExpr;
            }

            /// <summary>
            /// Appends stripped member expressions to the expression
            /// </summary>
            /// <param name="expr">Target expression.</param>
            /// <param name="strippedExpressions">The stripped expressions.</param>
            /// <returns></returns>
            private static Expression AppendStrippedExpressions(Expression expr, Stack<MemberExpression> strippedExpressions)
            {
                Expression finalExpr = expr;

                // Append the stripped expressions into the new member access expression
                while (strippedExpressions.Count != 0)
                {
                    MemberExpression tempExpr = strippedExpressions.Pop();
                    finalExpr = Expression.MakeMemberAccess(finalExpr, tempExpr.Member);
                }

                return finalExpr;
            }

            /// <summary>
            /// Matches simple expressions representing grouping by constant or single property 
            /// to expressions in the grouping expressions map.
            /// </summary>
            /// <param name="input">The input resource expression.</param>
            /// <param name="targetExpr">Expression to match.</param>
            /// <param name="expr">The matched expression.</param>
            /// <returns>true if a match is found in grouping expressions map; false otherwise.</returns>
            private static bool MatchSimpleGroupingExpression(QueryableResourceExpression input, Expression targetExpr, out Expression expr)
            {
                // Scenarios:
                // - GroupBy(d1 => d1.Prop, (d1, d2) => new { d1.[KnownPrimitiveTypeProperty], ...})
                // - GroupBy(d1 => [Constant]).Select(d2 => new { d2.Key, ... })
                // - GroupBy(d1 => d1.Prop).Select(d2 => new { d2.Key, ... })
                // - GroupBy(d1 => [Constant]).Select(d2 => new { d2.Key.[KnownPrimitiveTypeProperty], ... })
                // - GroupBy(d1 => d1.Prop).Select(d2 => new { d2.Key.[KnownPrimitiveTypeProperty], ... })

                expr = null;

                if ((
                    targetExpr.NodeType == ExpressionType.Parameter
                    && PrimitiveType.IsKnownNullableType(targetExpr.Type))
                    || (targetExpr.NodeType == ExpressionType.MemberAccess
                    && ((MemberExpression)targetExpr).Member.Name.Equals(IGroupingKey, StringComparison.Ordinal)))
                {
                    // We expect a single item in the grouping expressions mapping
                    Debug.Assert(input.Apply.GroupingExpressionsMap.Count == 1,
                        "this.input.Apply.GroupingExpressionsMap.Count == 1");

                    // Using .Single() here is deliberate.
                    // If there are multiple items in the grouping expressions mapping then there's a problem
                    KeyValuePair<string, Expression> pair = input.Apply.GroupingExpressionsMap.Single();

                    expr = pair.Value;
                    Debug.Assert(expr != null, "expr != null");

                    return true;
                }

                return false;
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
                private readonly IDictionary<string, Type> propertiesMap;
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
                    propertiesMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
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

                    MaterializationTypeBuilder builderInstance = new MaterializationTypeBuilder(input);

                    builderInstance.Visit(resultSelector.Body);

                    TypeInfo typeInfo = TypeBuilderUtil.CreateTypeInfo("MaterializationType", builderInstance.propertiesMap);
                    materializationType = typeInfo.AsType();
                    expressionMap = builderInstance.expressionMap;
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
                    // We need to handle the following possible scenarios:
                    // - GroupBy(d1 => d1.Prop, (d1, d2) => new { d1.[KnownPrimitiveTypeProperty], ... }
                    // - GroupBy(d1 => d1.Prop).Select(d2 => new { d2.Key.[KnownPrimitiveTypeProperty], ... }
                    // - GroupBy(d1 => new { d1.Prop1, d1.Prop2 }, (d1, d2) => { d1.Prop1.[KnownPrimitiveTypeProperty], ... }
                    // - GroupBy(d1 => new { d1.Prop1, d1.Prop2 }).Select(d2 => new { d2.Key.Prop1.[KnownPrimitiveTypeProperty], ... }
                    // where Prop, Prop1, and Prop2 are Edm properties of known primitive types (e.g. string), 
                    // and [KnownPrimitiveTypeProperty] represents a property of the known primitive type (e.g. Length).
                    // In such scenarios, the [KnownPrimitiveTypeProperty] will be evaluated on the client side
                    // Practical example: GroupBy(d1 => d1.Prop, (d1, d2) => new { d1.Length, ... }
                    // We strip the non-translated expressions since they are a client-side concern 
                    // and are irrelevant during materialization.
                    Expression translatedExpr = StripToTranslatedExpression(m, out _);

                    // In the current context, any member access should map to a grouping expression
                    Expression expr;

                    // Member access involving grouping by constant or single property
                    // - GroupBy(d1 => [Constant])
                    // - GroupBy(d1 => d1.Prop)
                    if (MatchSimpleGroupingExpression(this.input, translatedExpr, out expr))
                    {
                        if (expr.NodeType == ExpressionType.Constant)
                        {
                            // Scenarios - GroupBy(d => [Constant])
                            // Do nothing
                            return m;
                        }
                    }
                    else
                    {
                        Debug.Assert(translatedExpr.NodeType == ExpressionType.MemberAccess,
                            "translatedExpr.NodeType == ExpressionType.MemberAccess");

                        this.input.Apply.GroupingExpressionsMap.TryGetValue(((MemberExpression)translatedExpr).Member.Name, out expr);
                    }

                    MemberExpression memberExpr = expr as MemberExpression;
                    Debug.Assert(memberExpr != null);

                    // For nested member access, e.g. d1.Product.Category.Name, 
                    // we are only concerned with the property at the first level
                    // e.g. Product in this example
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

                    // Add relevant property with the expected return type to the properties map
                    if (!this.propertiesMap.ContainsKey(memberExpr.Member.Name))
                    {
                        propertiesMap.Add(memberExpr.Member.Name, memberExpr.Type);
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
                        case SequenceMethod.CountDistinctSelector:
                            MemberInfo memberInfo;
                            if (expressionMap.TryGetValue(m, out memberInfo))
                            {
                                // The member name will be the alias assigned to the aggregated value
                                // We add a relevant property with the expected return type
                                this.propertiesMap.Add(memberInfo.Name, m.Type);

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
                    Debug.Assert(expression != null, "expression != null");
                    Debug.Assert(expression.NodeType == ExpressionType.Parameter,
                        "expression.NodeType == ExpressionType.Parameter");
                    Debug.Assert(this.input.Apply.GroupingExpressionsMap.Count != 0,
                        "this.input.Apply.GroupingExpressionsMap.Count != 0");

                    foreach (KeyValuePair<string, Expression> pair in this.input.Apply.GroupingExpressionsMap)
                    {
                        // It'll be either a ConstantExpression or MemberExpression
                        // Nothing needs to be done for ConstantExpression
                        MemberExpression memberExpr = pair.Value as MemberExpression;

                        if (memberExpr != null)
                        {
                            this.VisitMemberAccess(memberExpr);
                        }
                    }
                }
            }
        }
    }
}
