//---------------------------------------------------------------------
// <copyright file="NodeToExpressionTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Spatial;
    using DataServiceProviderMethods = Microsoft.OData.Service.Providers.DataServiceProviderMethods;
    using OpenTypeMethods = Microsoft.OData.Service.Providers.OpenTypeMethods;
    using Strings = Microsoft.OData.Service.Strings;

    /// <summary>
    /// Component for translating a tree of nodes representing an OData query into a LINQ expression.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class coupling is due to being a visitor and handling many different node types.")]
    internal class NodeToExpressionTranslator : QueryNodeVisitor<Expression>
    {
        /// <summary>Dictionary of system functions.</summary>
        private static readonly Dictionary<string, FunctionDescription[]> functions = FunctionDescription.CreateFunctions();

        /// <summary>Method info for string comparison</summary>
        private static readonly MethodInfo StringCompareMethodInfo = typeof(DataServiceProviderMethods)
                                                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                .Single(m => m.Name == "Compare" && m.GetParameters()[0].ParameterType == typeof(string));

        /// <summary>Method info for Bool comparison</summary>
        private static readonly MethodInfo BoolCompareMethodInfo = typeof(DataServiceProviderMethods)
                                                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                .Single(m => m.Name == "Compare" && m.GetParameters()[0].ParameterType == typeof(bool));

        /// <summary>Method info for Bool? comparison</summary>
        private static readonly MethodInfo BoolCompareMethodInfoNullable = typeof(DataServiceProviderMethods)
                                                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                .Single(m => m.Name == "Compare" && m.GetParameters()[0].ParameterType == typeof(bool?));

        /// <summary>Method info for Guid comparison</summary>
        private static readonly MethodInfo GuidCompareMethodInfo = typeof(DataServiceProviderMethods)
                                                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                .Single(m => m.Name == "Compare" && m.GetParameters()[0].ParameterType == typeof(Guid));

        /// <summary>Method info for Guid? comparison</summary>
        private static readonly MethodInfo GuidCompareMethodInfoNullable = typeof(DataServiceProviderMethods)
                                                                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                .Single(m => m.Name == "Compare" && m.GetParameters()[0].ParameterType == typeof(Guid?));

        /// <summary>The function expression binder for checking/promoting operands.</summary>
        private readonly FunctionExpressionBinder functionExpressionBinder;

        /// <summary>The current data service behavior.</summary>
        private readonly DataServiceBehavior dataServiceBehavior;

        /// <summary>The service instance.</summary>
        private readonly object serviceInstance;

        /// <summary>Whether null propagation is required.</summary>
        private readonly bool nullPropagationRequired;

        /// <summary>Expression to use for the implicit '$it' parameter in function calls.</summary>
        private readonly ParameterExpression implicitParameterExpression;

        /// <summary>Callback to verify that the service's max protocol version is greather than or equal to the version required for a specific feature.</summary>
        private readonly Action<ODataProtocolVersion> verifyProtocolVersion;

        /// <summary>Callback to verify that the request's version is greather than or equal to the version required for a specific feature.</summary>
        private readonly Action<ODataProtocolVersion> verifyRequestVersion;

        /// <summary>Dictionary that associates range variables with their parameter expressions.</summary>
        private readonly IDictionary<RangeVariable, ParameterExpression> parameterExpressions = new Dictionary<RangeVariable, ParameterExpression>(); 

        /// <summary>
        /// Initializes a new instance of <see cref="NodeToExpressionTranslator"/>.
        /// </summary>
        /// <param name="functionExpressionBinder">The function expression binder for checking/promoting operands.</param>
        /// <param name="dataServiceBehavior">The data service behavior from the service's configuration.</param>
        /// <param name="serviceInstance">The data service instance.</param>
        /// <param name="nullPropagationRequired">Whether null propagation is required.</param>
        /// <param name="implicitParameterExpression">Expression to use for the implicit '$it' parameter in function calls.</param>
        /// <param name="verifyProtocolVersion">Callback to verify that the service's max protocol version is greather than or equal to the version required for a specific feature.</param>
        /// <param name="verifyRequestVersion">Callback to verify that the request's version is greather than or equal to the version required for a specific feature.</param>
        private NodeToExpressionTranslator(
            FunctionExpressionBinder functionExpressionBinder, 
            DataServiceBehavior dataServiceBehavior, 
            object serviceInstance,
            bool nullPropagationRequired, 
            ParameterExpression implicitParameterExpression,
            Action<ODataProtocolVersion> verifyProtocolVersion,
            Action<ODataProtocolVersion> verifyRequestVersion)
        {
            Debug.Assert(functionExpressionBinder != null, "functionExpressionBinder != null");
            Debug.Assert(dataServiceBehavior != null, "dataServiceBehavior != null");
            Debug.Assert(serviceInstance != null, "serviceInstance != null");
            Debug.Assert(verifyProtocolVersion != null, "verifyProtocolVersion != null");
            Debug.Assert(verifyRequestVersion != null, "verifyRequestVersion != null");

            this.functionExpressionBinder = functionExpressionBinder;
            this.dataServiceBehavior = dataServiceBehavior;
            this.serviceInstance = serviceInstance;
            this.nullPropagationRequired = nullPropagationRequired;
            this.implicitParameterExpression = implicitParameterExpression;
            this.verifyProtocolVersion = verifyProtocolVersion;
            this.verifyRequestVersion = verifyRequestVersion;
        }

        /// <summary>
        /// Dictionary that associates range variables with their parameter expressions.
        /// </summary>
        public IDictionary<RangeVariable, ParameterExpression> ParameterExpressions
        {
            get { return this.parameterExpressions; }
        }

        /// <summary>
        /// Translates a <see cref="AllNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(AllNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslateLambda(node);
        }

        /// <summary>
        /// Translates a <see cref="AnyNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(AnyNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslateLambda(node);
        }

        /// <summary>
        /// Translates a <see cref="BinaryOperatorNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(BinaryOperatorNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");

            var left = this.TranslateNode(node.Left);
            var right = this.TranslateNode(node.Right);

            switch (node.OperatorKind)
            {
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.LessThanOrEqual:
                case BinaryOperatorKind.Has:
                    return this.TranslateComparison(node.OperatorKind, left, right);
            }

            if (IsLogical(node.OperatorKind))
            {
                this.functionExpressionBinder.CheckAndPromoteOperands(typeof(OperationSignatures.ILogicalSignatures), node.OperatorKind, ref left, ref right);
            }
            else
            {
                this.functionExpressionBinder.CheckAndPromoteOperands(typeof(OperationSignatures.IArithmeticSignatures), node.OperatorKind, ref left, ref right);    
            }
            
            switch (node.OperatorKind)
            {
                case BinaryOperatorKind.And:
                    return ExpressionGenerator.GenerateLogicalAnd(left, right);

                case BinaryOperatorKind.Or:
                    return ExpressionGenerator.GenerateLogicalOr(left, right);
                    
                case BinaryOperatorKind.Add:
                    return ExpressionGenerator.GenerateAdd(left, right);

                case BinaryOperatorKind.Subtract:
                    return ExpressionGenerator.GenerateSubtract(left, right);

                case BinaryOperatorKind.Multiply:
                    return ExpressionGenerator.GenerateMultiply(left, right);

                case BinaryOperatorKind.Divide:
                    return ExpressionGenerator.GenerateDivide(left, right);

                case BinaryOperatorKind.Modulo:
                    return ExpressionGenerator.GenerateModulo(left, right);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(CollectionNavigationNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(CollectionPropertyAccessNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Property);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(ConstantNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");

            if (node.Value == null)
            {
                return ExpressionUtils.NullLiteral;
            }

            Type targetType = node.Value.GetType();
            if (node.TypeReference.IsSpatial())
            {
                if (!this.dataServiceBehavior.AcceptSpatialLiteralsInQuery)
                {
                    throw DataServiceException.CreateSyntaxError(Strings.RequestQueryParser_SpatialNotSupported);
                }

                // [Spatial] TypeAccessException when querying service deployed to IIS
                // With OData V3, we allow users to construct spatial types in the URI
                // such as GeographyPoint,GeomentryPoint etc.
                // These types are specified as public abstract base types and all implementations of these
                // are internal. If we use the Expression.Constant overload that only takes a value,
                // CLR will use reflection to find out the type through which it will create accessors
                // for running geometry functions such as Distance, e.t.c.
                // When running in partial trust in IIS, we cannot use accessors based on internal types
                // without causing a SecurityException.
                // So, we use the Expression.Constant overload which takes a type parameter and all
                // access to values of this constant expression is done via public types.
                targetType = GetPublicSpatialBaseType(node.Value);
                Debug.Assert(targetType != null, "TryCreateLiteral value should have a public spatial type in its hierarchy", "Value type : {0}", node.Value.ToString());
            }

            // DEVNOTE(pqian):
            // The following code rely on Expression.Constant returning UNIQUE instances for every call to it
            // i.e., Expression.Constant(1) does not reference equals to Expression.Constant(1)
            ConstantExpression expr = Expression.Constant(node.Value, targetType);
            this.functionExpressionBinder.TrackOriginalTextOfLiteral(expr, node.LiteralText);
            return expr;
        }

        /// <summary>
        /// Translates a <see cref="ConvertNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(ConvertNode node)
        {
            // For now, ignore converts and handle them in LINQ-space
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslateNode(node.Source);
        }

        /// <summary>
        /// Translates a <see cref="CollectionResourceCastNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(CollectionResourceCastNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            Debug.Assert(node.Source != null, "sourceNode != null");

            // Whenever we encounter the type segment, we need to only verify that the MPV is set to 4.0 or higher.
            // There is no need to check for request DSV, request MaxDSV since there are no protocol changes in
            // the payload for uri's with type identifier.
            this.verifyProtocolVersion(ODataProtocolVersion.V4);
            
            Expression source = this.TranslateNode(node.Source);
            return TranslateTypeCast(node, source);
        }

        /// <summary>
        /// Translates a <see cref="ResourceRangeVariableReferenceNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(ResourceRangeVariableReferenceNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.ParameterExpressions[node.RangeVariable];
        }

        /// <summary>
        /// Translates a <see cref="NonResourceRangeVariableReferenceNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(NonResourceRangeVariableReferenceNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.ParameterExpressions[node.RangeVariable];
        }

        /// <summary>
        /// Translates a <see cref="SingleResourceCastNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleResourceCastNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");

            // Whenever we encounter the type segment, we need to only verify that the MPV is set to 4.0 or higher.
            // There is no need to check for request DSV, request MaxDSV since there are no protocol changes in
            // the payload for uri's with type identifier.
            this.verifyProtocolVersion(ODataProtocolVersion.V4);
            
            Expression source = this.TranslateNode(node.Source);

            ResourceType resourceType = MetadataProviderUtils.GetResourceType(node.StructuredTypeReference.Definition);
            Debug.Assert(resourceType != null, "resourceType != null");

            return ExpressionGenerator.GenerateTypeAs(source, resourceType);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueCastNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleValueCastNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");

            // Whenever we encounter the type segment, we need to only verify that the MPV is set to 4.0 or higher.
            // There is no need to check for request DSV, request MaxDSV since there are no protocol changes in
            // the payload for uri's with type identifier.
            this.verifyProtocolVersion(ODataProtocolVersion.V4);

            Expression source = this.TranslateNode(node.Source);
            ResourceType resourceType = MetadataProviderUtils.GetResourceType(node.TypeReference.Definition);
            Debug.Assert(resourceType != null, "resourceType != null");

            return ExpressionGenerator.GenerateTypeAs(source, resourceType);
        }

        /// <summary>
        /// Translates a <see cref="SingleNavigationNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleNavigationNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="SingleResourceFunctionCallNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleResourceFunctionCallNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslateFunctionCall(node.Name, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueFunctionCallNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleValueFunctionCallNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslateFunctionCall(node.Name, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueOpenPropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleValueOpenPropertyAccessNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");

            Expression source = this.TranslateNode(node.Source);

            // object OpenTypeMethods.GetValue(object, string)
            Expression call = Expression.Call(null /* instance */, OpenTypeMethods.GetValueOpenPropertyMethodInfo, source, Expression.Constant(node.Name));
            if (this.nullPropagationRequired)
            {
                call = ExpressionUtils.AddNullPropagationIfNeeded(source, call);
            }

            return call;
        }

        /// <summary>
        /// Translates an <see cref="CollectionOpenPropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(CollectionOpenPropertyAccessNode node)
        {
            Expression source = this.TranslateNode(node.Source);

            // object OpenTypeMethods.GetValue(object, string)
            Expression call = Expression.Call(null /* instance */, OpenTypeMethods.GetCollectionValueOpenPropertyMethodInfo, source, Expression.Constant(node.Name));
            if (this.nullPropagationRequired)
            {
                call = ExpressionUtils.AddNullPropagationIfNeeded(source, call);
            }

            return call;
        }

        /// <summary>
        /// Translates a <see cref="SingleValuePropertyAccessNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleValuePropertyAccessNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Property);
        }

        /// <summary>
        /// Translates a <see cref="SingleComplexNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(SingleComplexNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            return this.TranslatePropertyAccess(node.Source, node.Property);
        }

        /// <summary>
        /// Translates a <see cref="UnaryOperatorNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        public override Expression Visit(UnaryOperatorNode node)
        {
            WebUtil.CheckArgumentNull(node, "node");
            Expression expr = this.TranslateNode(node.Operand);

            if (node.OperatorKind == UnaryOperatorKind.Negate)
            {
                this.functionExpressionBinder.CheckAndPromoteOperand(typeof(OperationSignatures.INegationSignatures), node.OperatorKind, ref expr);
                expr = ExpressionGenerator.GenerateNegate(expr);
            }
            else
            {
                Debug.Assert(node.OperatorKind == UnaryOperatorKind.Not, "Unexpected unary operator kind.");
                this.functionExpressionBinder.CheckAndPromoteOperand(typeof(OperationSignatures.INotSignatures), node.OperatorKind, ref expr);
                expr = ExpressionGenerator.GenerateNot(expr);
            }

            return expr;
        }

        /// <summary>
        /// Creates a new instance of <see cref="NodeToExpressionTranslator"/>.
        /// </summary>
        /// <param name="functionExpressionBinder">The function expression binder for checking/promoting operands.</param>
        /// <param name="dataServiceBehavior">The data service behavior from the service's configuration.</param>
        /// <param name="serviceInstance">The data service instance.</param>
        /// <param name="nullPropagationRequired">Whether null propagation is required.</param>
        /// <param name="implicitParameterExpression">Expression to use for the implicit '$it' parameter in function calls.</param>
        /// <param name="verifyProtocolVersion">Callback to verify that the service's max protocol version is greather than or equal to the version required for a specific feature.</param>
        /// <param name="verifyRequestVersion">Callback to verify that the request's version is greather than or equal to the version required for a specific feature.</param>
        /// <returns>The new translator.</returns>
        internal static NodeToExpressionTranslator CreateForTests(
            FunctionExpressionBinder functionExpressionBinder, 
            DataServiceBehavior dataServiceBehavior, 
            object serviceInstance, 
            bool nullPropagationRequired,
            ParameterExpression implicitParameterExpression,
            Action<ODataProtocolVersion> verifyProtocolVersion,
            Action<ODataProtocolVersion> verifyRequestVersion)
        {
            return new NodeToExpressionTranslator(
                functionExpressionBinder, 
                dataServiceBehavior,
                serviceInstance,
                nullPropagationRequired, 
                implicitParameterExpression, 
                verifyProtocolVersion, 
                verifyRequestVersion);
        }

        /// <summary>
        /// Creates a new instance of <see cref="NodeToExpressionTranslator"/>.
        /// </summary>
        /// <param name="service">The data service.</param>
        /// <param name="requestDescription">The current request description.</param>
        /// <param name="implicitParameterExpression">Expression to use for the implicit '$it' parameter in function calls.</param>
        /// <returns>The new translator.</returns>
        internal static NodeToExpressionTranslator Create(IDataService service, RequestDescription requestDescription, ParameterExpression implicitParameterExpression)
        {
            Debug.Assert(service != null, "service != null");
            var functionExpressionBinder = new FunctionExpressionBinder(t => WebUtil.TryResolveResourceType(service.Provider, t));
            return new NodeToExpressionTranslator(
                functionExpressionBinder,
                service.Configuration.DataServiceBehavior,
                service.Instance,
                service.Provider.NullPropagationRequired,
                implicitParameterExpression,
                v => requestDescription.VerifyProtocolVersion(v.ToVersion(), service),
                v => requestDescription.VerifyRequestVersion(v.ToVersion(), service));
        }

        /// <summary>Translates a <see cref="FilterClause"/> into a <see cref="LambdaExpression"/>.</summary>
        /// <param name="filterClause">The filter clause to translate.</param>
        /// <returns>The translated expression.</returns>
        internal LambdaExpression TranslateFilterClause(FilterClause filterClause)
        {
            Debug.Assert(filterClause != null, "filterClause != null");
            this.ParameterExpressions[filterClause.RangeVariable] = this.implicitParameterExpression;
            Expression expr = this.TranslateNode(filterClause.Expression);
            expr = ExpressionUtils.EnsurePredicateExpressionIsBoolean(expr);
            return Expression.Lambda(expr, this.implicitParameterExpression);
        }

        /// <summary>Translates a <see cref="OrderByClause"/> into a set of <see cref="OrderingExpression"/>.</summary>
        /// <param name="orderByClause">The order-by clause to translate.</param>
        /// <returns>The translated expressions.</returns>
        internal IEnumerable<OrderingExpression> TranslateOrderBy(OrderByClause orderByClause)
        {
            Debug.Assert(orderByClause != null, "orderByClause != null");

            List<OrderingExpression> orderings = new List<OrderingExpression>();
            while (orderByClause != null)
            {
                this.ParameterExpressions[orderByClause.RangeVariable] = this.implicitParameterExpression;

                Expression expr = this.TranslateNode(orderByClause.Expression);
                expr = Expression.Lambda(expr, this.implicitParameterExpression);
                orderings.Add(new OrderingExpression(expr, orderByClause.Direction == OrderByDirection.Ascending));
                orderByClause = orderByClause.ThenBy;
            }

            return orderings;
        }

        /// <summary>
        /// Main dispatching visit method for translating query-nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <returns>The LINQ expression resulting from visiting the node.</returns>
        internal Expression TranslateNode(QueryNode node)
        {
            Debug.Assert(node != null, "node != null");
            return node.Accept(this);
        }

        /// <summary>
        /// Given left and right hand side expressions, generates a comparison expression based
        /// on the given comparison token
        /// </summary>
        /// <param name="operatorKind">The operator kind of the comparison.</param>
        /// <param name="left">The left side of the comparison.</param>
        /// <param name="right">The right side of the comparison.</param>
        /// <returns>Resulting comparison expression</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal Expression TranslateComparison(BinaryOperatorKind operatorKind, Expression left, Expression right)
        {
            bool equality = operatorKind == BinaryOperatorKind.Equal || operatorKind == BinaryOperatorKind.NotEqual;

            // Comparsion operations is not allowed between spatial types
            if (left.Type.IsSpatial() || right.Type.IsSpatial())
            {
                throw IncompatibleOperandsError(operatorKind, left, right);
            }

            if (equality && !left.Type.IsValueType && !right.Type.IsValueType)
            {
                if (left.Type != right.Type)
                {
                    if (ExpressionUtils.IsNullConstant(left))
                    {
                        left = Expression.Constant(null, right.Type);
                    }
                    else if (ExpressionUtils.IsNullConstant(right))
                    {
                        right = Expression.Constant(null, left.Type);
                    }
                    else if (left.Type.IsAssignableFrom(right.Type))
                    {
                        right = Expression.Convert(right, left.Type);
                    }
                    else if (right.Type.IsAssignableFrom(left.Type))
                    {
                        left = Expression.Convert(left, right.Type);
                    }
                    else
                    {
                        throw IncompatibleOperandsError(operatorKind, left, right);
                    }
                }
            }
            else if (ExpressionUtils.IsNullConstant(left) || ExpressionUtils.IsNullConstant(right))
            {
                if (!equality)
                {
                    string message = Strings.RequestQueryParser_NullOperatorUnsupported(operatorKind);
                    throw DataServiceException.CreateSyntaxError(message);
                }

                // Because we don't have an explicit "is null" check, literal comparisons
                // to null are special.
                if (!WebUtil.TypeAllowsNull(left.Type))
                {
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
                }
                else if (!WebUtil.TypeAllowsNull(right.Type))
                {
                    right = Expression.Convert(right, typeof(Nullable<>).MakeGenericType(right.Type));
                }
            }
            else
            {
                // Enums should be checked here for promotion when supported, but they aren't in this version.
                Debug.Assert(!WebUtil.GetNonNullableType(left.Type).IsEnum, "!IsEnumType(left.Type)");
                Debug.Assert(!WebUtil.GetNonNullableType(right.Type).IsEnum, "!IsEnumType(right.Type)");

                this.functionExpressionBinder.CheckAndPromoteOperands(typeof(OperationSignatures.IRelationalSignatures), operatorKind, ref left, ref right);
            }

            MethodInfo comparisonMethodInfo = null;
            if (!equality)
            {
                if (left.Type == typeof(string))
                {
                    comparisonMethodInfo = StringCompareMethodInfo;
                }
                else if (left.Type == typeof(bool))
                {
                    comparisonMethodInfo = BoolCompareMethodInfo;
                }
                else if (left.Type == typeof(bool?))
                {
                    comparisonMethodInfo = BoolCompareMethodInfoNullable;
                }
                else if (left.Type == typeof(Guid))
                {
                    comparisonMethodInfo = GuidCompareMethodInfo;
                }
                else if (left.Type == typeof(Guid?))
                {
                    comparisonMethodInfo = GuidCompareMethodInfoNullable;
                }
            }

            switch (operatorKind)
            {
                case BinaryOperatorKind.Equal:
                    left = ExpressionGenerator.GenerateEqual(left, right);
                    break;
                case BinaryOperatorKind.NotEqual:
                    left = ExpressionGenerator.GenerateNotEqual(left, right);
                    break;
                case BinaryOperatorKind.GreaterThan:
                    left = ExpressionGenerator.GenerateGreaterThan(left, right, comparisonMethodInfo);
                    break;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    left = ExpressionGenerator.GenerateGreaterThanEqual(left, right, comparisonMethodInfo);
                    break;
                case BinaryOperatorKind.LessThan:
                    left = ExpressionGenerator.GenerateLessThan(left, right, comparisonMethodInfo);
                    break;
                case BinaryOperatorKind.LessThanOrEqual:
                    left = ExpressionGenerator.GenerateLessThanEqual(left, right, comparisonMethodInfo);
                    break;
            }

            return left;
        }

        /// <summary>
        /// Gets the public base type for a given <paramref name="spatialValue"/> spatial value
        /// </summary>
        /// <param name="spatialValue">The spatial instance value for which we want to find the public type.</param>
        /// <returns>The public spatial base type for the instance value.</returns>
        private static Type GetPublicSpatialBaseType(object spatialValue)
        {
            Debug.Assert(spatialValue != null, "spatialValue should not be null");
            Debug.Assert(spatialValue is ISpatial, "spatialValue should implement ISpatial");

            Type publicSpatialBaseType = spatialValue.GetType();
            while (publicSpatialBaseType != null && !publicSpatialBaseType.IsPublic)
            {
                publicSpatialBaseType = publicSpatialBaseType.BaseType;
            }

            Debug.Assert(publicSpatialBaseType != null, "TryCreateLiteral value should have a public spatial type in its hierarchy", "Value type : {0}", spatialValue.ToString());
            return publicSpatialBaseType;
        }
        
        /// <summary>
        /// Returns whether the operator kind represents a logical operator.
        /// </summary>
        /// <param name="binaryOperatorKind">The operator kind.</param>
        /// <returns>True for logical operators (And/Or); false for everything else.</returns>
        private static bool IsLogical(BinaryOperatorKind binaryOperatorKind)
        {
            return binaryOperatorKind == BinaryOperatorKind.And || binaryOperatorKind == BinaryOperatorKind.Or;
        }

        /// <summary>Creates an exception indicated that two operands are incompatible.</summary>
        /// <param name="operatorKind">Kind of operation for operands.</param>
        /// <param name="left">Expression for left-hand operand.</param>
        /// <param name="right">Expression for right-hand operand.</param>
        /// <returns>A new <see cref="Exception"/>.</returns>
        private static Exception IncompatibleOperandsError(BinaryOperatorKind operatorKind, Expression left, Expression right)
        {
            string message = Strings.RequestQueryParser_IncompatibleOperands(
                operatorKind,
                WebUtil.GetTypeName(left.Type),
                WebUtil.GetTypeName(right.Type));
            return DataServiceException.CreateSyntaxError(message);
        }

        /// <summary>
        /// Translates a <see cref="CollectionResourceCastNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <param name="source">The already-translated source of the type cast.</param>
        /// <returns>The translated expression.</returns>
        private static Expression TranslateTypeCast(CollectionResourceCastNode node, Expression source)
        {
            Debug.Assert(node != null, "node != null");
            Debug.Assert(node.Source != null, "sourceNode != null");
            Debug.Assert(node.ItemType != null, "typeReference != null");
            Debug.Assert(source != null, "source != null");

            ResourceType resourceType = MetadataProviderUtils.GetResourceType(node.ItemType.Definition);
            Debug.Assert(resourceType != null, "resourceType != null");

            return ExpressionGenerator.GenerateOfType(source, resourceType);
        }

        /// <summary>
        /// Helper for translating an access to a metadata-defined property or navigation.
        /// </summary>
        /// <param name="sourceNode">The source of the property access.</param>
        /// <param name="edmProperty">The structural or navigation property being accessed.</param>
        /// <param name="navigationSource">The navigation source of the result, required for navigations.</param>
        /// <returns>The translated expression.</returns>
        private Expression TranslatePropertyAccess(QueryNode sourceNode, IEdmProperty edmProperty, IEdmNavigationSource navigationSource = null)
        {
            Debug.Assert(sourceNode != null, "sourceNode != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");

            Expression source = this.TranslateNode(sourceNode);

            ResourceType currentResourceType = MetadataProviderUtils.GetResourceType(edmProperty.DeclaringType);
            ResourceProperty property = ((IResourcePropertyBasedEdmProperty)edmProperty).ResourceProperty;
            ResourceSetWrapper container = (navigationSource != null && !(navigationSource is IEdmUnknownEntitySet)) ? ((IResourceSetBasedEdmEntitySet)navigationSource).ResourceSet : null;

            Debug.Assert(currentResourceType != null, "currentResourceType != null");
            Debug.Assert(property != null, "property != null");

            if (property.TypeKind == ResourceTypeKind.EntityType && container == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidPropertyNameSpecified(property.Name, currentResourceType.FullName));
            }

            // We have a strongly-type property.
            Expression propertyAccess;
            if (property.CanReflectOnInstanceTypeProperty)
            {
                propertyAccess = Expression.Property(source, currentResourceType.GetPropertyInfo(property));
            }
            else
            {
                // object DataServiceProviderMethods.GetValue(object, ResourceProperty)
                propertyAccess = Expression.Call(null /*instance*/, DataServiceProviderMethods.GetValueMethodInfo, source, Expression.Constant(property));
                propertyAccess = Expression.Convert(propertyAccess, property.Type);
            }

            Expression result = propertyAccess;
            if (this.nullPropagationRequired)
            {
                result = ExpressionUtils.AddNullPropagationIfNeeded(source, result);
            }

            if (container != null)
            {
                bool singleResult = property.Kind == ResourcePropertyKind.ResourceReference;
                DataServiceConfiguration.CheckResourceRightsForRead(container, singleResult);
                Expression filter = DataServiceConfiguration.ComposeQueryInterceptors(this.serviceInstance, container);
                if (filter != null)
                {
                    // We did null propagation for accessing the property, but we may also need
                    // to do null propagation on the property value itself (otherwise the interception
                    // lambda needs to check the argument for null, which is probably unexpected).
                    result = ExpressionUtils.ComposePropertyNavigation(
                        result, (LambdaExpression)filter, this.nullPropagationRequired, singleResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Translates a function call into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="argumentNodes">The argument nodes.</param>
        /// <returns>
        /// The translated expression.
        /// </returns>
        private Expression TranslateFunctionCall(string functionName, IEnumerable<QueryNode> argumentNodes)
        {
            FunctionDescription[] functionDescriptions;
            if (!functions.TryGetValue(functionName, out functionDescriptions))
            {
                string message = Strings.RequestQueryParser_UnknownFunction(functionName);
                throw DataServiceException.CreateSyntaxError(message);
            }

            if (!this.dataServiceBehavior.AcceptReplaceFunctionInQuery && functionDescriptions[0].IsReplace)
            {
                string message = Strings.RequestQueryParser_UnknownFunction(functionName);
                throw DataServiceException.CreateSyntaxError(message);
            }

            Expression[] originalArguments = argumentNodes.Select(this.TranslateNode).ToArray();

            Expression[] arguments = this.nullPropagationRequired ? originalArguments : (Expression[])originalArguments.Clone();

            // check if the one of the parameters is of open type. If yes, then we need to invoke
            // LateBoundMethods for this function otherwise not.
            bool openParameters = !functionDescriptions[0].IsTypeCheckOrCast && arguments.Any(OpenTypeMethods.IsOpenPropertyExpression);

            Expression result;
            FunctionDescription function = null;
            if (openParameters)
            {
                function = functionDescriptions.FirstOrDefault(f => f.ParameterTypes.Length == arguments.Length);
            }
            else
            {
                function = this.functionExpressionBinder.FindBestFunction(functionDescriptions, ref arguments);
            }

            if (function == null)
            {
                string message = Strings.RequestQueryParser_NoApplicableFunction(functionName, FunctionDescription.BuildSignatureList(functionName, functionDescriptions));
                throw DataServiceException.CreateSyntaxError(message);
            }

            if (openParameters)
            {
                Expression[] openArguments = new Expression[arguments.Length];
                for (int i = 0; i < openArguments.Length; i++)
                {
                    if (OpenTypeMethods.IsOpenPropertyExpression(arguments[i]))
                    {
                        openArguments[i] = arguments[i];
                    }
                    else
                    {
                        openArguments[i] = Expression.Convert(arguments[i], typeof(object));
                    }
                }

                result = function.InvokeOpenTypeMethod(openArguments);
            }
            else
            {
                // Special case for null propagation - we never strip nullability from expressions.
                if (this.nullPropagationRequired && function.IsTypeCast)
                {
                    Expression typeExpression = arguments[arguments.Length - 1];
                    Debug.Assert(typeExpression != null, "typeExpression != null -- otherwise function finding failed.");
                    if (typeExpression.Type == typeof(Type))
                    {
                        Type castTargetType = (Type)((ConstantExpression)typeExpression).Value;
                        if (!WebUtil.TypeAllowsNull(castTargetType))
                        {
                            arguments[arguments.Length - 1] = Expression.Constant(typeof(Nullable<>).MakeGenericType(castTargetType));
                        }
                    }
                }

                Expression[] finalArguments;
                if (function.ConversionFunction == FunctionDescription.BinaryIsOfResourceType || function.ConversionFunction == FunctionDescription.BinaryCastResourceType)
                {
                    finalArguments = new Expression[arguments.Length + 1];
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        finalArguments[i] = arguments[i];
                    }

                    finalArguments[arguments.Length] = Expression.Constant(OpenTypeMethods.IsOpenPropertyExpression(arguments[0]), typeof(bool));
                }
                else
                {
                    finalArguments = arguments;
                }

                result = function.ConversionFunction(this.implicitParameterExpression, finalArguments);
            }

            if (this.nullPropagationRequired && function.RequiresNullPropagation)
            {
                Debug.Assert(originalArguments.Length == arguments.Length, "originalArguments.Length == arguments.Length -- arguments should not be added/removed");
                for (int i = 0; i < originalArguments.Length; i++)
                {
                    Expression element = originalArguments[i];
                    result = ExpressionUtils.AddNullPropagationIfNeeded(element, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Translates a <see cref="LambdaNode"/> into a corresponding <see cref="Expression"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated expression.</returns>
        private Expression TranslateLambda(LambdaNode node)
        {
            Debug.Assert(node != null, "node != null");

            // Previously, the parser would not even recognize any/all if this setting was false and so would throw a generic error
            // about accessing properties on collections.
            if (!this.dataServiceBehavior.AcceptAnyAllRequests)
            {
                string identifier = node.Kind == QueryNodeKind.Any ? XmlConstants.AnyMethodName : XmlConstants.AllMethodName;
                string message = Strings.RequestQueryParser_DisallowMemberAccessForResourceSetReference(identifier);
                throw DataServiceException.CreateSyntaxError(message);
            }

            // Although there are no payload changes required for any/all
            // we still need to verify that the MPV and request version are set to 4.0 or higher
            this.verifyProtocolVersion(ODataProtocolVersion.V4);
            this.verifyRequestVersion(ODataProtocolVersion.V4);

            // to emulate the old null-propagation behavior of Any/All handling, we need to peek at the source of the lambda and see if it is a type cast.
            // If it is, translate it's source and then add the null propagation before the type cast.
            // If it isn't, then just add the type cast before the lambda.
            QueryNode sourceNode = node.Source;
            CollectionResourceCastNode castNode = null;
            if (sourceNode.Kind == QueryNodeKind.CollectionResourceCast)
            {
                castNode = ((CollectionResourceCastNode)sourceNode);
                sourceNode = castNode.Source;

                if (sourceNode.Kind == QueryNodeKind.CollectionResourceCast)
                {
                    // If 2 identifiers are specified back to back, then we need to check and throw in that scenario
                    string message = Strings.RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier(((CollectionResourceCastNode)sourceNode).ItemType.FullName(), castNode.ItemType.FullName());
                    throw DataServiceException.CreateBadRequestError(message);
                }
            }

            // translate the source and add null-propagation.
            Expression source = this.TranslateNode(sourceNode);
            source = this.ConvertNullCollectionToEmpty(source);

            // if there was a cast, translate it now that null-propagation has been added to the expression.
            if (castNode != null)
            {
                source = TranslateTypeCast(castNode, source);
            }

            Debug.Assert(node.Body != null, "node.Body != null");
            if (node.CurrentRangeVariable == null)
            {
                Debug.Assert(node.Body.Kind == QueryNodeKind.Constant && (bool)((ConstantNode)node.Body).Value, "Not having a range variable indicates this must be an empty any");
                if (node.Kind == QueryNodeKind.All)
                {
                    throw DataServiceException.CreateSyntaxError(Strings.RequestQueryParser_AllWithoutAPredicateIsNotSupported);
                }

                Debug.Assert(node.Kind == QueryNodeKind.Any, "Unexpected lambda node kind: " + node.Kind);
                return source.EnumerableAny();
            }

            ParameterExpression predicateParameter = Expression.Parameter(BaseServiceProvider.GetIEnumerableElement(source.Type), node.CurrentRangeVariable.Name);
            this.ParameterExpressions[node.CurrentRangeVariable] = predicateParameter;

            Expression predicateBody = this.TranslateNode(node.Body);
            predicateBody = ExpressionUtils.EnsurePredicateExpressionIsBoolean(predicateBody);

            LambdaExpression predicateLambda = Expression.Lambda(predicateBody, predicateParameter);

            if (node.Kind == QueryNodeKind.Any)
            {
                return source.EnumerableAny(predicateLambda);
            }

            Debug.Assert(node.Kind == QueryNodeKind.All, "this method must be called for only 'any' or 'all'.");
            return source.EnumerableAll(predicateLambda);
        }

        /// <summary>Convert null collection to empty collection for Any/All/OfType methods.</summary>
        /// <param name="expressionToCheck">An IEnumerable expression.</param>
        /// <returns>An expression that checks <paramref name="expressionToCheck"/> for null.</returns>
        private Expression ConvertNullCollectionToEmpty(Expression expressionToCheck)
        {
            if (this.nullPropagationRequired)
            {
                Type elementType = BaseServiceProvider.GetIEnumerableElement(expressionToCheck.Type);
                Debug.Assert(elementType != null, "only IEnumerable expressions can come here");

                Expression test = Expression.Equal(expressionToCheck, Expression.Constant(null, expressionToCheck.Type));
                Expression falseIf = expressionToCheck;
                Expression trueIf = ExpressionUtils.EnumerableEmpty(elementType);
                return Expression.Condition(test, trueIf, falseIf, trueIf.Type);
            }

            return expressionToCheck;
        }
    }
}
