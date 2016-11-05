//---------------------------------------------------------------------
// <copyright file="NodeToExpressionTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Services.ODataWCFService.BuiltInFunctionHelper;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class NodeToExpressionTranslator : QueryNodeVisitor<Expression>
    {
        /// <summary>
        /// Expression to represent the implicit variable parameter in filter 
        /// </summary>
        public Expression ImplicitVariableParameterExpression;

        /// <summary>
        /// The URI parser.
        /// </summary>
        public ODataUriParser UriParser { get; set; }

        public IODataDataSource DataSource { get; set; }

        /// <summary>
        /// MethodInfo for Queryable.AsQueryable
        /// </summary>
        private static MethodInfo _queryableAsQueryableMethod = GenericMethodOf(_ => Queryable.AsQueryable<int>(default(IEnumerable<int>)));

        /// <summary>
        /// MethodInfo for Queryable.LongCount
        /// </summary>
        private static MethodInfo _countMethod = GenericMethodOf(_ => Queryable.LongCount<int>(default(IQueryable<int>)));

        /// <summary>
        /// Helper method to get the MethodInfo from the body of the given lambda expression.
        /// </summary>
        /// <typeparam name="TReturn">The function type paramenter.</typeparam>
        /// <param name="lambda">Lambda expression.</param>
        /// <returns>Returns the MethodInfo from the body of the given lambda expression.</returns>
        private static MethodInfo GenericMethodOf<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            Debug.Assert(expression.NodeType == ExpressionType.Lambda, "cannot call GenericMethodOf with non LambdaExpression");
            Debug.Assert(expression != null, "LambdaExpression can not be null");
            Debug.Assert(expression.Body.NodeType == ExpressionType.Call, "LambdaExpression body node type should be call");

            return (expression.Body as MethodCallExpression).Method.GetGenericMethodDefinition();
        }
        
        /// <summary>
        /// Visit an AllNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        ///// <returns>The translated expression</returns>
        public override Expression Visit(AllNode nodeIn)
        {
            var instanceType = EdmClrTypeUtils.GetInstanceType(nodeIn.RangeVariables[0].TypeReference);
            ParameterExpression parameter = Expression.Parameter(instanceType, nodeIn.RangeVariables[0].Name);
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = this.UriParser,
                DataSource = this.DataSource,
            };
            Expression conditionExpression = nodeToExpressionTranslator.TranslateNode(nodeIn.Body);
            Expression rootExpression = this.TranslateNode(nodeIn.Source);

            return Expression.Call(
                typeof (Enumerable),
                "All",
                new Type[] {instanceType},
                rootExpression,
                Expression.Lambda(conditionExpression, parameter));
        }

        /// <summary>
        /// Visit an AnyNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(AnyNode nodeIn)
        {
            var instanceType = EdmClrTypeUtils.GetInstanceType(nodeIn.RangeVariables[0].TypeReference);
            ParameterExpression parameter = Expression.Parameter(instanceType, nodeIn.RangeVariables[0].Name);
            NodeToExpressionTranslator nodeToExpressionTranslator = new NodeToExpressionTranslator()
            {
                ImplicitVariableParameterExpression = parameter,
                UriParser = this.UriParser,
                DataSource = this.DataSource,
            };
            Expression conditionExpression = nodeToExpressionTranslator.TranslateNode(nodeIn.Body);
            Expression rootExpression = this.TranslateNode(nodeIn.Source);

            return Expression.Call(
                typeof (Enumerable),
                "Any",
                new Type[] {instanceType},
                rootExpression,
                Expression.Lambda(conditionExpression, parameter));
        }

        /// <summary>
        /// Visit a BinaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(BinaryOperatorNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "BinaryOpeatorNode");

            var left = this.TranslateNode(nodeIn.Left);
            var right = this.TranslateNode(nodeIn.Right);

            switch (nodeIn.OperatorKind)
            {
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.LessThanOrEqual:
                case BinaryOperatorKind.Has:
                    return this.TranslateComparison(nodeIn.OperatorKind, left, right);
            }

            if (nodeIn.OperatorKind == BinaryOperatorKind.And || nodeIn.OperatorKind == BinaryOperatorKind.Or)
            {
                if (Nullable.GetUnderlyingType(left.Type) != null)
                {
                    left = Expression.Call(left, "GetValueOrDefault", Type.EmptyTypes);
                }
                if (Nullable.GetUnderlyingType(right.Type) != null)
                {
                    right = Expression.Call(right, "GetValueOrDefault", Type.EmptyTypes);
                }

                return nodeIn.OperatorKind == BinaryOperatorKind.And
                    ? Expression.And(left, right)
                    : Expression.Or(left, right);
            }

            switch (nodeIn.OperatorKind)
            {
                case BinaryOperatorKind.Add:
                    return Expression.Add(left, right);

                case BinaryOperatorKind.Subtract:
                    return Expression.Subtract(left, right);

                case BinaryOperatorKind.Multiply:
                    return Expression.Multiply(left, right);

                case BinaryOperatorKind.Divide:
                    return Expression.Divide(left, right);

                case BinaryOperatorKind.Modulo:
                    return Expression.Modulo(left, right);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Visit a SearchTermNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SearchTermNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "SearchTermNode");

            return new SearchHelper(nodeIn.Text, this.ImplicitVariableParameterExpression).Build();
        }

        /// <summary>
        /// Visit a CountNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(CountNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "CountNode");

            Type propType = null;
            Expression propExpr = null;
            
            // Element of collection could be primitive type or enum or complex or entity type 
            if (nodeIn.Source.ItemType.IsPrimitive() || nodeIn.Source.ItemType.IsEnum()
                || (nodeIn.Source.ItemType.IsComplex() && nodeIn.Source.Kind.Equals(QueryNodeKind.CollectionPropertyAccess)))
            {
                // This does not handle complex collection cast case, if it is a complex collection cast, the Kind will be CollectionPropertyCast and node.Source is CollectionPropertyCastNode
                var collection = (CollectionPropertyAccessNode)nodeIn.Source;
                propExpr = Visit(collection);
                var def = collection.Property.Type.AsCollection();
                propType = EdmClrTypeUtils.GetInstanceType(def.ElementType());
            }
            else if (nodeIn.Source.ItemType.IsEntity() && nodeIn.Source.Kind.Equals(QueryNodeKind.CollectionNavigationNode))
            {
                // This does not handle entity collection cast case, if it is a entity collection cast, the Kind will be EntityCollectionCast and node.Source is EntityCollectionCastNode
                var collection = (CollectionNavigationNode)nodeIn.Source;
                propExpr = Visit(collection);
                var def = collection.NavigationProperty.Type.AsCollection();
                propType = EdmClrTypeUtils.GetInstanceType(def.ElementType());
            }
            else
            {
                // Should no such case as collection item is either primitive or enum or complex or entity.
                throw new NotSupportedException(string.Format("Filter based on count of collection with item of type {0} is not supported yet.", nodeIn.Source.ItemType));
            }

            // Per standard, collection can not be null, but could be an empty collection, so null is not considered here.
            var asQuerableMethod = _queryableAsQueryableMethod.MakeGenericMethod(propType);
            Expression asQuerableExpression = Expression.Call(null, asQuerableMethod, propExpr);

            var countMethod = _countMethod.MakeGenericMethod(propType);
            var countExpression = Expression.Call(null, countMethod, asQuerableExpression);
            return countExpression;
        }

        /// <summary>
        /// Visit a CollectionNavigationNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(CollectionNavigationNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "CollectionNavigationNode");

            return Expression.Property(this.ImplicitVariableParameterExpression, nodeIn.NavigationProperty.Name);
        }

        /// <summary>
        /// Visit a CollectionPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(CollectionPropertyAccessNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "CollectionPropertyAccessNode");
            return this.TranslatePropertyAccess(nodeIn.Source, nodeIn.Property);
        }

        /// <summary>
        /// Visit a ConstantNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(ConstantNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "ConstantNode");
            if (null == nodeIn.Value)
            {
                return Expression.Constant(null);
            }

            // collection of entity
            if (nodeIn.TypeReference != null && nodeIn.TypeReference.IsCollection() &&
                (nodeIn.TypeReference.Definition as IEdmCollectionType).ElementType.IsEntity())
            {
                return Expression.Constant(ParseEntityCollection(nodeIn));
            }

            // the value is entity or entity reference.
            if (nodeIn.TypeReference != null && nodeIn.TypeReference.IsEntity())
            {
                return Expression.Constant(ParseEntity(nodeIn));
            }
            // the value is complex or collection.
            if (nodeIn.Value is ODataComplexValue || nodeIn.Value is ODataCollectionValue || nodeIn.Value is ODataEntry)
            {

                object value = ODataObjectModelConverter.ConvertPropertyValue(nodeIn.Value);
                return Expression.Constant(value);
            }

            // the value is enum
            if (nodeIn.TypeReference.IsEnum())
            {
                ODataEnumValue enumValue = (ODataEnumValue) nodeIn.Value;
                object enumClrVal = Enum.Parse(EdmClrTypeUtils.GetInstanceType(enumValue.TypeName), enumValue.Value);
                return Expression.Constant(enumClrVal, EdmClrTypeUtils.GetInstanceType(nodeIn.TypeReference));
            }

            // the value is primitive
            Type targetType = nodeIn.Value.GetType();
            if (nodeIn.TypeReference.IsSpatial())
            {
                targetType = GetPublicSpatialBaseType(nodeIn.Value);
            }

            return Expression.Constant(nodeIn.Value, targetType);
        }

        /// <summary>
        /// Visit a ParameterAliasNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(ParameterAliasNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "ParameterAliasNode");
            var aliasName = nodeIn.Alias;
            return TranslateParameterAlias(aliasName);
        }

        /// <summary>
        /// Visit a ConvertNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(ConvertNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "ConvertNode");
            var sourceExpression = this.TranslateNode(nodeIn.Source);

            var targetEdmType = nodeIn.TypeReference;
            if (null == targetEdmType)
            {
                //Open property's target type is null, so return the source expression directly, supposely the caller should be ready to handle data of Object type.
                return sourceExpression;
            }
            var targetClrType = EdmClrTypeUtils.GetInstanceType(targetEdmType);
            return Expression.Convert(sourceExpression, targetClrType);
        }

        /// <summary>
        /// Visit an EntityCollectionCastNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(EntityCollectionCastNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an EntityRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(EntityRangeVariableReferenceNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "EntityRangeVariableReferenceNode");

            // when this is called for a filter like svc/Customers?$filter=PersonID eq 1, nodeIn.Name has value "$it".
            // when this is called by any/all option, nodeIn.Name is specified by client, it can be any value.
            return this.ImplicitVariableParameterExpression;
        }

        /// <summary>
        /// Visit a NonentityRangeVariableReferenceNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(NonentityRangeVariableReferenceNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "NonentityRangeVariableReferenceNode");
            return this.ImplicitVariableParameterExpression;
        }

        /// <summary>
        /// Visit a SingleEntityCastNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleEntityCastNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "node");
            return this.TranslateSingleValueCastAccess(nodeIn.Source, nodeIn.TypeReference);
        }

        /// <summary>
        /// Visit a SingleNavigationNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleNavigationNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "node");
            return this.TranslateSingleNavigationAccess(nodeIn.Source, nodeIn.NavigationProperty);
        }

        /// <summary>
        /// Visit a SingleEntityFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleEntityFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleValueFunctionCallNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "node");
            return this.TranslateFunctionCall(nodeIn.Name, nodeIn.Parameters);
        }

        /// <summary>
        /// Visit a EntityCollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(EntityCollectionFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a CollectionFunctionCallNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(CollectionFunctionCallNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a SingleValueOpenPropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleValueOpenPropertyAccessNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "SingleValueOpenPropertyAccessNode");
            return this.TranslateOpenPropertyAccess(nodeIn.Source, nodeIn.Name);
        }

        /// <summary>
        /// Visit a SingleValuePropertyAccessNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleValuePropertyAccessNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "SingleValuePropertyAccessNode");
            return this.TranslatePropertyAccess(nodeIn.Source, nodeIn.Property);
        }

        /// <summary>
        /// Visit a UnaryOperatorNode
        /// </summary>
        /// <param name="nodeIn">The node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(UnaryOperatorNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "UnaryOperatorNode");

            switch (nodeIn.OperatorKind)
            {
                case UnaryOperatorKind.Not:
                    return Expression.Not(this.TranslateNode(nodeIn.Operand));
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Visit a NamedFunctionParameterNode.
        /// </summary>
        /// <param name="nodeIn">The node to visit.</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(NamedFunctionParameterNode nodeIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit an SingleValueCastNode
        /// </summary>
        /// <param name="nodeIn">the node to visit</param>
        /// <returns>The translated expression</returns>
        public override Expression Visit(SingleValueCastNode nodeIn)
        {
            this.CheckArgumentNull(nodeIn, "node");
            return this.TranslateSingleValueCastAccess(nodeIn.Source, nodeIn.TypeReference);
        }

        /// <summary>
        /// Main dispatching visit method for translating query nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <returns>The LINQ expression resulting from visiting the node.</returns>
        internal Expression TranslateNode(QueryNode node)
        {
            this.CheckArgumentNull(node, "QueryNode");
            return node.Accept(this);
        }

        /// <summary>
        /// get the parsed alias node
        /// </summary>
        /// <param name="aliasName">the alias name with '@' prefix</param>
        /// <returns></returns>
        private Expression TranslateParameterAlias(String aliasName)
        {
            if (aliasName == null)
            {
                throw new InvalidOperationException("Unexpected null alias name.");
            }

            var singleValueNode = this.UriParser.ParameterAliasNodes[aliasName];

            if (null == singleValueNode)
            {
                return Expression.Constant(null);
            }

            return this.TranslateNode(singleValueNode);
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
            switch (functionName)
            {
                #region string functions
                case "contains":
                    var methodInfoOfContains = typeof(string).GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance);
                    var instanceOfContains = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfContains = argumentNodes.ElementAt(1).Accept(this);
                    return Expression.Call(instanceOfContains, methodInfoOfContains, argumentOfContains);

                case "endswith":
                    var methodInfoOfEndsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                    var instanceOfEndsWith = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfEndsWith = argumentNodes.ElementAt(1).Accept(this);
                    return Expression.Call(instanceOfEndsWith, methodInfoOfEndsWith, argumentOfEndsWith);

                case "startswith":
                    var methodInfoOfStartsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    var instanceOfStartsWith = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfStartsWith = argumentNodes.ElementAt(1).Accept(this);
                    return Expression.Call(instanceOfStartsWith, methodInfoOfStartsWith, argumentOfStartsWith);

                case "length":
                    var propertyInfoOfLength = typeof(string).GetProperty("Length", typeof(int));
                    var instanceOfLength = argumentNodes.ElementAt(0).Accept(this);
                    return Expression.Property(instanceOfLength, propertyInfoOfLength);

                case "indexof":
                    var methodInfoOfIndexOf = typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) });
                    var instanceOfIndexOf = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfIndexOf = argumentNodes.ElementAt(1).Accept(this);
                    return Expression.Call(instanceOfIndexOf, methodInfoOfIndexOf, argumentOfIndexOf);

                case "substring":
                    var argumentCount = argumentNodes.Count();
                    if (argumentNodes.Count() == 2)
                    {
                        var methodInfoOfSubString = typeof(string).GetMethod("Substring", new Type[] { typeof(int) });
                        var instanceOfSubString = argumentNodes.ElementAt(0).Accept(this);
                        var argumentOfSubString = argumentNodes.ElementAt(1).Accept(this);
                        return Expression.Call(instanceOfSubString, methodInfoOfSubString, argumentOfSubString);
                    }
                    else if (argumentNodes.Count() == 3)
                    {
                        var methodInfoOfSubString = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
                        var instanceOfSubString = argumentNodes.ElementAt(0).Accept(this);
                        var argumentOfSubString = argumentNodes.ElementAt(1).Accept(this);
                        var argumentOfSubString2 = argumentNodes.ElementAt(2).Accept(this);
                        return Expression.Call(instanceOfSubString, methodInfoOfSubString, argumentOfSubString, argumentOfSubString2);
                    }
                    else
                    {
                        throw new ArgumentException("argumentNodes");
                    }

                case "tolower":
                    var methodInfoOfToLower = typeof(string).GetMethod("ToLower", new Type[] { });
                    var instanceOfToLower = argumentNodes.ElementAt(0).Accept(this);
                    return Expression.Call(instanceOfToLower, methodInfoOfToLower);

                case "toupper":
                    var methodInfoOfToUpper = typeof(string).GetMethod("ToUpper", new Type[] { });
                    var instanceOfToUpper = argumentNodes.ElementAt(0).Accept(this);
                    return Expression.Call(instanceOfToUpper, methodInfoOfToUpper);

                case "trim":
                    var methodInfoOfTrim = typeof(string).GetMethod("Trim", new Type[] { });
                    var instanceOfTrim = argumentNodes.ElementAt(0).Accept(this);
                    return Expression.Call(instanceOfTrim, methodInfoOfTrim);

                case "concat":
                    var methodInfoOfConcat = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
                    var argumentOfConcat1 = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfConcat2 = argumentNodes.ElementAt(1).Accept(this);
                    return Expression.Call(methodInfoOfConcat, argumentOfConcat1, argumentOfConcat2);
                #endregion

                #region DateTime Method
                case "year":
                    return TranslateDateTimeInstanceProperty("Year", argumentNodes);
                case "month":
                    return TranslateDateTimeInstanceProperty("Month", argumentNodes);
                case "day":
                    return TranslateDateTimeInstanceProperty("Day", argumentNodes);
                case "hour":
                    return TranslateDateTimeInstanceProperty("Hour", argumentNodes);
                case "minute":
                    return TranslateDateTimeInstanceProperty("Minute", argumentNodes);
                case "second":
                    return TranslateDateTimeInstanceProperty("Second", argumentNodes);

                // Don't support those type by now.
                //case "fractionalseconds":
                //    return TranslateDateTimeInstanceProperty("Millisecond", argumentNodes);
                //case "date":
                //    return TranslateDateTimeInstanceProperty("Date", argumentNodes);
                //case "time":
                //    return TranslateDateTimeProperty("Year");
                //case "totaloffsetminutes":
                //    return TranslateDateTimeProperty("Date", argumentNodes);
                //case "now":
                //    return TranslateDateTimeProperty("Now", argumentNodes);
                //case "mindatetime":
                //    return TranslateDateTimeProperty("MinValue", argumentNodes);
                //case "maxdatetime":
                //    return TranslateDateTimeProperty("MaxValue", argumentNodes);
                #endregion

                #region Math Methods
                case "round":
                    return TranslateMathFunction("Round", argumentNodes);
                case "floor":
                    return TranslateMathFunction("Floor", argumentNodes);
                case "ceiling":
                    return TranslateMathFunction("Ceiling", argumentNodes);
                #endregion

                #region Type Functions
                case "cast":
                    var instanceOfCast = argumentNodes.ElementAt(0).Accept(this);
                    var typeInfoOfCast = (ConstantNode)argumentNodes.ElementAt(1);
                    var targetTypeOfCast = EdmClrTypeUtils.GetInstanceType(typeInfoOfCast.Value.ToString());
                    var methodInfoOfCast = typeof(TypeFunctionHelper).GetMethod("TypeCastFunction", BindingFlags.Public | BindingFlags.Static);
                    methodInfoOfCast = methodInfoOfCast.MakeGenericMethod(new Type[] { targetTypeOfCast, instanceOfCast.Type });
                    return Expression.Call(methodInfoOfCast, instanceOfCast);

                case "isof":
                    var instanceOfIsOf = argumentNodes.ElementAt(0).Accept(this);
                    var typeInfoOfIsOf = (ConstantNode)argumentNodes.ElementAt(1);
                    return Expression.TypeIs(instanceOfIsOf, EdmClrTypeUtils.GetInstanceType(typeInfoOfIsOf.Value.ToString()));

                #endregion

                #region Geo Functions
                case "geo.distance":
                    var argumentOfGeoDistance1 = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfGeoDistance2 = argumentNodes.ElementAt(1).Accept(this);
                    var methodInfoOfGeoDistance = typeof(GeoFunctionHelper)
                        .GetMethod("GetDistance", new Type[] { argumentOfGeoDistance1.Type, argumentOfGeoDistance2.Type });
                    return Expression.Call(methodInfoOfGeoDistance, argumentOfGeoDistance1, argumentOfGeoDistance2);

                case "geo.length":
                    var argumentOfGeoLength = argumentNodes.ElementAt(0).Accept(this);
                    var methodInfoOfGeoLength = typeof(GeoFunctionHelper)
                        .GetMethod("GetLength", new Type[] { argumentOfGeoLength.Type });
                    return Expression.Call(methodInfoOfGeoLength, argumentOfGeoLength);

                case "geo.intersects":
                    var argumentOfGeoIntersects1 = argumentNodes.ElementAt(0).Accept(this);
                    var argumentOfGeoIntersects2 = argumentNodes.ElementAt(1).Accept(this);
                    var methodInfoOfGeoIntersectse = typeof(GeoFunctionHelper)
                        .GetMethod("GetIsIntersects", new Type[] { argumentOfGeoIntersects1.Type, argumentOfGeoIntersects2.Type });
                    return Expression.Call(methodInfoOfGeoIntersectse, argumentOfGeoIntersects1, argumentOfGeoIntersects2);
                #endregion
                default:
                    throw new ArgumentException(functionName);
            }
        }

        #region private methods

        private static T TypeCastFunction<T>(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("Cast input");
            }
            return (T)instance;
        }

        /// <summary>
        /// Translate math functions
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="argumentNodes">Argument nodes</param>
        /// <returns>Expression</returns>
        private Expression TranslateMathFunction(string functionName, IEnumerable<QueryNode> argumentNodes)
        {
            var argumentInfo = argumentNodes.ElementAt(0).Accept(this);
            var methodInfo = typeof(Math).GetMethod(functionName, new Type[] { argumentInfo.Type });
            return Expression.Call(methodInfo, argumentInfo);
        }

        /// <summary>
        /// Translate datetime class property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="argumentNodes">Argument nodes</param>
        /// <returns>Expression</returns>
        private Expression TranslateDateTimeProperty(string propertyName)
        {
            var propertyInfo = typeof(DateTimeOffset).GetProperty(propertyName);
            return Expression.Property(null, propertyInfo);
        }

        /// <summary>
        /// Translate datetime instance property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="argumentNodes">Argument nodes</param>
        /// <returns>Expression</returns>
        private Expression TranslateDateTimeInstanceProperty(string propertyName, IEnumerable<QueryNode> argumentNodes)
        {
            var instance = argumentNodes.ElementAt(0).Accept(this);
            var propertyInfo = instance.Type.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return Expression.Property(instance, propertyInfo);
            }
            else
            {
                throw new NotSupportedException(string.Format("Can't support type {0}", instance.Type.ToString()));
            }
        }

        /// <summary>
        /// Given left and right hand side expressions, generates a comparison expression based on the given comparison token
        /// </summary>
        /// <param name="operatorKind">The operator kind of the comparison.</param>
        /// <param name="left">The left side of the comparison.</param>
        /// <param name="right">The right side of the comparison.</param>
        /// <returns>Resulting comparison expression</returns>
        private Expression TranslateComparison(BinaryOperatorKind operatorKind, Expression left, Expression right)
        {
            if (left.Type != right.Type)
            {
                if (IsNullConstant(left))
                {
                    left = Expression.Constant(null, right.Type);
                }
                else if (IsNullConstant(right))
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
                    throw new InvalidOperationException("Incompatible bianry operators");
                }
            }

            //TODO: Need to handle null value
            if (left.Type.IsEnum || left.Type.IsGenericType && left.Type.GetGenericArguments()[0].IsEnum)
            {
                Type enumType = left.Type.IsNullable() ? left.Type.GetGenericArguments()[0] : left.Type;

                left = Expression.Convert(left, enumType.GetEnumUnderlyingType());
                right = Expression.Convert(right, enumType.GetEnumUnderlyingType());
            }

            switch (operatorKind)
            {
                case BinaryOperatorKind.Equal:
                    left = Expression.Equal(left, right);
                    break;
                case BinaryOperatorKind.NotEqual:
                    left = Expression.NotEqual(left, right);
                    break;
                case BinaryOperatorKind.GreaterThan:
                    left = Expression.GreaterThan(left, right);
                    break;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    left = Expression.GreaterThanOrEqual(left, right);
                    break;
                case BinaryOperatorKind.LessThan:
                    left = Expression.LessThan(left, right);
                    break;
                case BinaryOperatorKind.LessThanOrEqual:
                    left = Expression.LessThanOrEqual(left, right);
                    break;
                case BinaryOperatorKind.Has:
                    left = Expression.Equal(Expression.And(left, right), right);
                    break;
            }

            return left;
        }

        /// <summary>
        /// Translate a single entity cast access to expression
        /// </summary>
        /// <param name="sourceNode">The source of single entity cast access.</param>
        /// <param name="typeReference">The target type reference of casting.</param>
        /// <returns></returns>
        private Expression TranslateSingleValueCastAccess(QueryNode sourceNode, IEdmTypeReference typeReference)
        {
            Expression source = this.TranslateNode(sourceNode);
            Type targetType = EdmClrTypeUtils.GetInstanceType(typeReference);
            return Expression.TypeAs(source, targetType);
        }

        /// <summary>
        /// Translate a single navigation property access to expression
        /// </summary>
        /// <param name="sourceNode">The source of single navigation property access.</param>
        /// <param name="edmProperty">The single navigation property being accessed.</param>
        /// <returns></returns>
        private Expression TranslateSingleNavigationAccess(QueryNode sourceNode, IEdmProperty edmProperty)
        {
            Expression source = this.TranslateNode(sourceNode);
            return Expression.Property(source, edmProperty.Name);
        }

        /// <summary>
        /// Translate an open property access to expression
        /// </summary>
        /// <param name="sourceNode">The source of the open property access.</param>
        /// <param name="propertyName">The name of the open propert.y</param>
        /// <returns></returns>
        private Expression TranslateOpenPropertyAccess(QueryNode sourceNode, string propertyName)
        {
            // get OpenProperties
            Expression source = this.TranslateNode(sourceNode);
            var propertyAccessExpression = Expression.Property(source, "OpenProperties");
            // key
            var key = Expression.Constant(propertyName, typeof(string));
            // OpenProperties.ContainsKey(propertyName)
            MethodInfo containsKeyMethod = typeof(Dictionary<string, object>).GetMethod("ContainsKey", new[] { typeof(string) });
            var containsExpression = Expression.Call(propertyAccessExpression, containsKeyMethod, key);
            //OpenProperties[propertyName]
            var queryOpenPropertyExpression = Expression.Property(propertyAccessExpression, "Item", key);
            return Expression.Condition(containsExpression, queryOpenPropertyExpression, Expression.Constant(null));
        }

        /// <summary>
        /// Translate a property aceess to expression
        /// </summary>
        /// <param name="sourceNode">The source of the property access.</param>
        /// <param name="edmProperty">The structural or navigation property being accessed.</param>
        /// <returns>The translated expression</returns>
        private Expression TranslatePropertyAccess(QueryNode sourceNode, IEdmProperty edmProperty)
        {
            Expression source = this.TranslateNode(sourceNode);
            Expression constantNullExpression = Expression.Constant(null);
            Expression propertyAccess = Expression.Property(source, edmProperty.Name);

            return Expression.Condition(
                Expression.Equal(source, constantNullExpression),
                Expression.Convert(constantNullExpression, propertyAccess.Type),
                propertyAccess);
        }

        /// <summary>Checks whether expression is a null constant.</summary>
        /// <param name="expression">Expression to check.</param>
        /// <returns>true if expression is a null constant; false otherwise.</returns>
        private bool IsNullConstant(Expression expression)
        {
            return (expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null);
        }

        /// <summary>
        /// Gets the public base type for a given spatial value
        /// </summary>
        /// <param name="spatialValue">The spatial instance value</param>
        /// <returns>The public spatial base type for the instance value.</returns>
        private static Type GetPublicSpatialBaseType(object spatialValue)
        {
            Type publicSpatialBaseType = spatialValue.GetType();
            while (publicSpatialBaseType != null && !publicSpatialBaseType.IsPublic)
            {
                publicSpatialBaseType = publicSpatialBaseType.BaseType;
            }

            return publicSpatialBaseType;
        }

        /// <summary>
        /// Check whether the given query node is null
        /// </summary>
        /// <param name="node">The query node</param>
        /// <param name="name">The query node type name</param>
        private void CheckArgumentNull(QueryNode node, string name)
        {
            if (node == null)
            {
                throw new InvalidOperationException("Unexpected null " + name + " node.");
            }
        }

        /// <summary>
        /// Parse the constant entity node.
        /// </summary>
        /// <param name="nodeIn">The input constant node.</param>
        /// <returns>The parsed object.</returns>
        private object ParseEntity(ConstantNode nodeIn)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nodeIn.LiteralText)),
            };

            var entityType = (nodeIn.TypeReference as IEdmEntityTypeReference).Definition as IEdmEntityType;

            using (
                ODataMessageReader reader = new ODataMessageReader(message as IODataRequestMessage, settings,
                    this.UriParser.Model))
            {
                if (nodeIn.LiteralText.Contains("@odata.id"))
                {
                    ODataEntityReferenceLink referenceLink = reader.ReadEntityReferenceLink();
                    var queryContext = new QueryContext(this.UriParser.ServiceRoot, referenceLink.Url,
                        this.DataSource.Model);
                    var target = queryContext.ResolveQuery(this.DataSource);
                    return target;
                }

                var entryReader = reader.CreateODataEntryReader(
                    new EdmEntitySet(new EdmEntityContainer("NS", "Test"), "TestType", entityType),
                    entityType);
                ODataEntry entry = null;
                while (entryReader.Read())
                {
                    if (entryReader.State == ODataReaderState.EntryEnd)
                    {
                        entry = entryReader.Item as ODataEntry;

                    }
                }
                return ODataObjectModelConverter.ConvertPropertyValue(entry);
            }
        }

        /// <summary>
        /// Parse the constant entity collection node.
        /// </summary>
        /// <param name="nodeIn">The input constant node.</param>
        /// <returns>The parsed object.</returns>
        private object ParseEntityCollection(ConstantNode nodeIn)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            InMemoryMessage message = new InMemoryMessage()
            {
                Stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(nodeIn.LiteralText)),
            };

            var entityType =
                ((nodeIn.TypeReference.Definition as IEdmCollectionType).ElementType as IEdmEntityTypeReference)
                    .Definition as IEdmEntityType;
            object list = null;
            MethodInfo addMethod = null;
            using (
                ODataMessageReader reader = new ODataMessageReader(message as IODataRequestMessage, settings,
                    this.UriParser.Model))
            {
                if (nodeIn.LiteralText.Contains("@odata.id"))
                {
                    ODataEntityReferenceLinks referenceLinks = reader.ReadEntityReferenceLinks();
                    foreach (var referenceLink in referenceLinks.Links)
                    {
                        var queryContext = new QueryContext(this.UriParser.ServiceRoot, referenceLink.Url,
                            this.DataSource.Model);
                        var target = queryContext.ResolveQuery(this.DataSource);

                        if (list == null)
                        {
                            // create the list. This would require the first type is not derived type.
                            Type listType = typeof(List<>).MakeGenericType(target.GetType());
                            addMethod = listType.GetMethod("Add");
                            list = Activator.CreateInstance(listType);
                        }

                        addMethod.Invoke(list, new[] { target });
                    }

                    return list;
                }

                var feedReader = reader.CreateODataFeedReader(
                    new EdmEntitySet(new EdmEntityContainer("NS", "Test"), "TestType", entityType),
                    entityType);
                ODataEntry entry = null;
                while (feedReader.Read())
                {
                    if (feedReader.State == ODataReaderState.EntryEnd)
                    {
                        entry = feedReader.Item as ODataEntry;
                        object item = ODataObjectModelConverter.ConvertPropertyValue(entry);


                        if (list == null)
                        {
                            // create the list. This would require the first type is not derived type.
                            var type = EdmClrTypeUtils.GetInstanceType(entry.TypeName);
                            Type listType = typeof(List<>).MakeGenericType(type);
                            addMethod = listType.GetMethod("Add");
                            list = Activator.CreateInstance(listType);
                        }

                        addMethod.Invoke(list, new[] { item });
                    }
                }
                return list;
            }
        }
        #endregion
    }
}

