//---------------------------------------------------------------------
// <copyright file="DSPMethodTranslatingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Spatial;

    /// <summary>Expression visitor which translates calls to methods on the <see cref="DataServiceProviderMethods"/> class
    /// into expressions which can be evaluated by LINQ to Objects.</summary>
    internal class DSPMethodTranslatingVisitor : ExpressionVisitor
    {
        /// <summary>MethodInfo for object DataServiceProviderMethods.GetValue(this object value, ResourceProperty property).</summary>
        internal static readonly MethodInfo GetValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for IEnumerable&lt;T&gt; DataServiceProviderMethods.GetSequenceValue(this object value, ResourceProperty property).</summary>
        internal static readonly MethodInfo GetSequenceValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetSequenceValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for Convert.</summary>
        internal static readonly MethodInfo ConvertMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "Convert",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo TypeIsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "TypeIs",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for OfType.</summary>
        internal static readonly MethodInfo OfTypeIQueryableMethodInfo = ((MethodInfo[])typeof(DataServiceProviderMethods).GetMember(
            "OfType",
            MemberTypes.Method,
            BindingFlags.Static | BindingFlags.Public)).Where(m => m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)).Single();

        /// <summary>MethodInfo for OfType.</summary>
        internal static readonly MethodInfo OfTypeIEnumerableMethodInfo = ((MethodInfo[])typeof(DataServiceProviderMethods).GetMember(
            "OfType",
            MemberTypes.Method,
            BindingFlags.Static | BindingFlags.Public)).Where(m => m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Single();

        /// <summary>MethodInfo for TypeAs.</summary>
        internal static readonly MethodInfo TypeAsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "TypeAs",
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for object OpenTypeMethods.GetValue(object value, string propertyName).</summary>
        internal static readonly MethodInfo OpenTypeGetValueMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "GetValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(string) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.Equal(object left, object right).</summary>
        internal static readonly MethodInfo OpenTypeEqualMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "Equal",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(object) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.LessThan(object left, object right).</summary>
        internal static readonly MethodInfo OpenTypeLessThanMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "LessThan",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(object) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.GreaterThan(object left, object right).</summary>
        internal static readonly MethodInfo OpenTypeGreaterThanMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "GreaterThan",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(object) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.Distance(object left, object right).</summary>
        internal static readonly MethodInfo OpenTypeDistanceMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "Distance",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(object) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.Length(object geometryLineString or geographyLineString).</summary>
        internal static readonly MethodInfo OpenTypeLengthMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "Length",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object) },
            null);

        /// <summary>MethodInfo for object OpenTypeMethods.Intersects(object left, object right).</summary>
        internal static readonly MethodInfo OpenTypeIntersectsMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "Intersects",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(object) },
            null);

        private readonly IRuntimeEvaluator evaluator;

        private DSPMethodTranslatingVisitor()
        {
            this.evaluator = new RuntimeEvaluator();
        }

        /// <summary>Method which translates an expression using the <see cref="DataServiceProviderMethods"/> methods
        /// into a new expression which can be evaluated by LINQ to Objects.</summary>
        /// <param name="expression">The expression to translate.</param>
        /// <returns>The translated expression.</returns>
        public static Expression TranslateExpression(Expression expression)
        {
            DSPMethodTranslatingVisitor visitor = new DSPMethodTranslatingVisitor();
            return visitor.Visit(expression);
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method == GetValueMethodInfo)
            {
                // Arguments[0] - the resource to get property value of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceProperty to get value of

                // Just call the targetResource.GetValue(resourceProperty.Name)
                return Expression.Call(
                    this.Visit(m.Arguments[0]),
                    typeof(DSPResource).GetMethod("GetValue"),
                    Expression.Property(m.Arguments[1], "Name"));
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == GetSequenceValueMethodInfo)
            {
                // Arguments[0] - the resource to get property value of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceProperty to get value of

                // Just call the targetResource.GetValue(resourceProperty.Name) and cast it to the right IEnumerable<T> (which is the return type of the GetSequenceMethod
                return Expression.Convert(
                    Expression.Call(
                        this.Visit(m.Arguments[0]),
                        typeof(DSPResource).GetMethod("GetValue"),
                        Expression.Property(m.Arguments[1], "Name")),
                    m.Method.ReturnType);
            }
            else if (m.Method == ConvertMethodInfo)
            {
                // All our resources are of the same underlying CLR type, so no need for conversion of the CLR type
                //   and we don't have any specific action to take to convert the Resource Types either (as we access properties from a property bag)
                // So get rid of the conversions as we don't need them
                return this.Visit(m.Arguments[0]);
            }
            else if (m.Method == TypeIsMethodInfo)
            {
                // Arguments[0] - the resource to determine the type of - we assume it's a DSPEntity
                // Arguments[1] - the ResourceType to test for

                // Using typeAs and checking if that returns a non-null value.
                // If TypeAs returns non-null, then this should return true, otherwise false.
                return
                    Expression.NotEqual(
                        Expression.Call(
                            this.Visit(m.Arguments[0]),
                            typeof(DSPResource).GetMethod("TypeAs"),
                            m.Arguments[1]),
                        Expression.Constant(null, typeof(DSPResource)));
            }
            else if (m.Method == OpenTypeGetValueMethodInfo)
            {
                // Arguments[0] - the resource to get the open property for.
                // Arguments[1] - the name of the property to get.

                // Just call the targetResource.GetOpenPropertyValue(propertyName)
                return Expression.Call(
                    this.Visit(m.Arguments[0]),
                    typeof(DSPResource).GetMethod("GetOpenPropertyValue"),
                    m.Arguments[1]);
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == TypeAsMethodInfo)
            {
                return Expression.Call(
                        this.Visit(m.Arguments[0]),
                        typeof(DSPResource).GetMethod("TypeAs"),
                        m.Arguments[1]);
            }
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == OfTypeIQueryableMethodInfo)
            {
                ResourceType targetResourceType = ((ConstantExpression)m.Arguments[1]).Value as ResourceType;

                Expression source = this.Visit(m.Arguments[0]);

                ParameterExpression parameter = Expression.Parameter(TypeSystem.GetIEnumerableElementType(source.Type), "element");
                Expression body = Expression.NotEqual(
                    Expression.Call(parameter, typeof(DSPResource).GetMethod("TypeAs", BindingFlags.Public | BindingFlags.Instance), m.Arguments[1]),
                    Expression.Constant(null, typeof(DSPResource)));
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
            else if (m.Method.IsGenericMethod && m.Method.GetGenericMethodDefinition() == OfTypeIEnumerableMethodInfo)
            {
                ResourceType targetResourceType = ((ConstantExpression)m.Arguments[1]).Value as ResourceType;

                Expression source = this.Visit(m.Arguments[0]);

                ParameterExpression parameter = Expression.Parameter(TypeSystem.GetIEnumerableElementType(source.Type), "element");
                Expression body = Expression.NotEqual(
                    Expression.Call(parameter, typeof(DSPResource).GetMethod("TypeAs", BindingFlags.Public | BindingFlags.Instance), m.Arguments[1]),
                    Expression.Constant(null, typeof(DSPResource)));
                Expression lambda = Expression.Lambda(body, parameter);

                source = Expression.Call(
                    typeof(Enumerable),
                    "Where",
                    new Type[] { parameter.Type },
                    source,
                    lambda);

                MethodInfo methodInfo = typeof(Enumerable).GetMethod("Cast");
                methodInfo = methodInfo.MakeGenericMethod(targetResourceType.InstanceType);
                return Expression.Call(methodInfo, source);
            }
            else if (m.Method == OpenTypeDistanceMethodInfo)
            {
                // Distance(left, right)
                // Depending on what left and right is, we need to resolve to the right method
                MethodInfo methodInfo = this.GetType().GetMethod("DistanceMethodResolver", BindingFlags.Public | BindingFlags.Static);
                return Expression.Call(methodInfo, Visit(m.Arguments[0]), Visit(m.Arguments[1]));
            }
            else if (m.Method == OpenTypeLengthMethodInfo)
            {
                // Length(geoOperand)
                MethodInfo methodInfo = this.GetType().GetMethod("LengthMethodResolver", BindingFlags.Public | BindingFlags.Static);
                return Expression.Call(methodInfo, Visit(m.Arguments[0]));
            }
            else if (m.Method == OpenTypeIntersectsMethodInfo)
            {
                // Intersects(left, right)
                // Depending on what left and right is, we need to resolve to the right method
                MethodInfo methodInfo = this.GetType().GetMethod("IntersectsMethodResolver", BindingFlags.Public | BindingFlags.Static);
                return Expression.Call(methodInfo, Visit(m.Arguments[0]), Visit(m.Arguments[1]));
            }

            return base.VisitMethodCall(m);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Method == OpenTypeEqualMethodInfo || node.Method == OpenTypeLessThanMethodInfo || node.Method == OpenTypeGreaterThanMethodInfo)
            {
                MethodInfo mi = typeof(IRuntimeEvaluator).GetMethod("Compare", new Type[] { typeof(object), typeof(object) });

                Expression call = Expression.Call(
                                        Expression.Constant(this.evaluator, typeof(IRuntimeEvaluator)),
                                        mi,
                                        Expression.Convert(this.Visit(node.Left), typeof(object)),
                                        Expression.Convert(this.Visit(node.Right), typeof(object)));

                return Expression.MakeBinary(node.NodeType, call, Expression.Constant(0, typeof(int)));
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
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

        /// <summary>
        /// Call the right distance method depending on the arguments
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>The distance between the left and right values</returns>
        public static Object DistanceMethodResolver(object left, object right)
        {
            if (SpatialImplementation.CurrentImplementation != null && SpatialImplementation.CurrentImplementation.Operations != null)
            {
                if (left is Geography && right is Geography)
                {
                    return ((Geography)left).Distance((Geography)right);
                }
                else if (left is Geometry && right is Geometry)
                {
                    return ((Geometry)left).Distance((Geometry)right);
                }
            }

            throw new NotImplementedException(String.Format("Distance method is not implemented between {0} and {1}", left, right));
        }

        /// <summary>
        /// Call the right distance method depending on the arguments
        /// </summary>
        /// <param name="geoOperand">The geometryLineString or geographyLineString value</param>
        /// <returns>The length of the geoOperand line.</returns>
        public static Object LengthMethodResolver(object geoOperand)
        {
            if (SpatialImplementation.CurrentImplementation != null && SpatialImplementation.CurrentImplementation.Operations != null)
            {
                if (geoOperand is Geography)
                {
                    return ((Geography)geoOperand).Length();
                }
                else if (geoOperand is Geometry)
                {
                    return ((Geometry)geoOperand).Length();
                }
            }

            throw new NotImplementedException(String.Format("Length method is not implemented for {0} ", geoOperand));
        }

        /// <summary>
        /// Call the right Intersects method depending on the arguments
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>The bool about if point and polygon will intersect.</returns>
        public static Object IntersectsMethodResolver(object left, object right)
        {
            if (SpatialImplementation.CurrentImplementation != null && SpatialImplementation.CurrentImplementation.Operations != null)
            {
                if (left is Geography && right is Geography)
                {
                    return ((Geography)left).Intersects((Geography)right);
                }
                else if (left is Geometry && right is Geometry)
                {
                    return ((Geometry)left).Intersects((Geometry)right);
                }
            }

            throw new NotImplementedException(String.Format("Intersects method is not implemented for {0} and {1}", left, right));
        }
    }
}
