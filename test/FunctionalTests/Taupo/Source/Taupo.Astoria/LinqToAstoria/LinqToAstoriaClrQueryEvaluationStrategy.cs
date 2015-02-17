//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaClrQueryEvaluationStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Astoria-specific clr evaluation strategy.
    /// </summary>
    public class LinqToAstoriaClrQueryEvaluationStrategy : ClrQueryEvaluationStrategy, ILinqToAstoriaQueryEvaluationStrategy
    {
        /// <summary>
        /// Gets or sets the spatial operations manager.
        /// </summary>
        [InjectDependency]
        public ISpatialRegistrationManager SpatialOperationsManager { get; set; }

        /// <summary>
        /// Gets or sets the canonical function evaluator.
        /// </summary>
        [InjectDependency]
        public IClrCanonicalFunctionEvaluator CanonicalFunctionEvaluator { get; set; }

        /// <summary>
        /// Gets a value indicating whether the order of collection value is predictable.
        /// </summary>
        /// <returns>true if the order of collection value is predictable.</returns>
        public virtual bool IsCollectionOrderPredictable
        {
            get { return true; }
        }

        /// <summary>
        /// Evaluates built in function with the given namespace and name.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="functionNamespace">The function namespace.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public override QueryValue EvaluateBuiltInFunction(QueryType resultType, string functionNamespace, string functionName, params QueryValue[] arguments)
        {
            ExceptionUtilities.CheckObjectNotNull(this.CanonicalFunctionEvaluator, "Cannot evaluate built in function without an evaluator");
            var value = this.CanonicalFunctionEvaluator.Evaluate(resultType, functionName, arguments);

            if (arguments.Any(a => a.IsDynamicPropertyValue()))
            {
                value = value.AsDynamicPropertyValue();
            }

            return value;
        }

        /// <summary>
        /// Do the specified BinaryOperation for the two values and returns the result.
        /// </summary>
        /// <param name="operation">The binary operation to perform.</param>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <returns>
        /// Result of the operation.
        /// </returns>
        public override QueryScalarValue Evaluate(QueryBinaryOperation operation, QueryScalarValue firstValue, QueryScalarValue secondValue)
        {
            if (operation == QueryBinaryOperation.LessThan || operation == QueryBinaryOperation.LessThanOrEqualTo)
            {
                return this.PerformNullSafeLessThan(firstValue, secondValue, operation);
            }
            
            return base.Evaluate(operation, firstValue, secondValue);
        }
        
        /// <summary>
        /// Determines whether a typed data can do certain operation with another typed data
        /// </summary>
        /// <param name="operation">the operation</param>
        /// <param name="sourceType">the type of data to operation on</param>
        /// <param name="otherType">the other type</param>
        /// <returns>Value <c>true</c> if operation can be performed; otherwise, <c>false</c>.</returns>
        public override bool Supports(QueryBinaryOperation operation, QueryScalarType sourceType, QueryScalarType otherType)
        {
            if (this.IsSpatialType((IQueryClrType)sourceType) || this.IsSpatialType((IQueryClrType)otherType))
            {
                return false;
            }

            return base.Supports(operation, sourceType, otherType);
        }

        /// <summary>
        /// Evaluates member property of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The property result type.</param>
        /// <param name="memberPropertyName">The member property name to evaluate.</param>
        /// <returns>
        /// Query value which is the result of method evaluation.
        /// </returns>
        public override QueryValue EvaluateMemberProperty(QueryValue instance, QueryType resultType, string memberPropertyName)
        {
            if (this.IsSpatialType((IQueryClrType)instance.Type))
            {
                var clrInstance = ((QueryScalarValue)instance).Value;
                if (clrInstance == null)
                {
                    var nullValue = resultType.NullValue;
                    nullValue.EvaluationError = new QueryError("Cannot access member property on a null instance");
                    return nullValue;
                }

                var clrType = clrInstance.GetType();
                while (!clrType.IsPublic())
                {
                    clrType = clrType.GetBaseType();
                }

                var property = clrType.GetProperty(memberPropertyName, true, false);
                ExceptionUtilities.CheckObjectNotNull(property, "Could not find property");

                var propertyValue = property.GetValue(clrInstance, null);
                if (propertyValue == null)
                {
                    return resultType.NullValue;
                }

                var collectionType = resultType as QueryCollectionType;
                if (collectionType != null)
                {
                    var enumerable = propertyValue as IEnumerable;
                    ExceptionUtilities.CheckObjectNotNull(enumerable, "Collection value must be ienumerable. Value was '{0}'", propertyValue);

                    var elementType = (QueryScalarType)collectionType.ElementType;
                    return collectionType.CreateCollectionWithValues(enumerable.Cast<object>().Select(o => (QueryValue)elementType.CreateValue(o)));
                }
                else
                {
                    return ((QueryScalarType)resultType).CreateValue(propertyValue);
                }
            }

            return base.EvaluateMemberProperty(instance, resultType, memberPropertyName);
        }

        /// <summary>
        /// Evaluates member method of a spatial instance.
        /// </summary>
        /// <param name="instance">The instance of query value object</param>
        /// <param name="resultType">The function result type.</param>
        /// <param name="methodName">The member method to evaluate.</param>
        /// <param name="arguments">Arguments for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public override QueryValue EvaluateMemberMethod(QueryValue instance, QueryType resultType, string methodName, params QueryValue[] arguments)
        {
            var value = base.EvaluateMemberMethod(instance, resultType, methodName, arguments);

            if (instance.IsDynamicPropertyValue() || arguments.Any(a => a.IsDynamicPropertyValue()))
            {
                value = value.AsDynamicPropertyValue();
            }

            return value;
        }

        /// <summary>
        /// Returns whether or not to expect lazy evaluation of less than for a null first value.
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <returns>True if lazy null evaluation should be expected</returns>
        internal static bool ExpectLazyNullEvaluatiorForLessThan(QueryScalarValue firstValue)
        {
            var clrType = ((IQueryClrType)firstValue.Type).ClrType;
            bool lazyNullEvaluation = firstValue.IsNull && IsSpecialCaseTypeForLessThan(clrType) && !firstValue.IsDynamicPropertyValue();
            return lazyNullEvaluation;
        }

        /// <summary>
        /// Performs a null safe less-than or less-than-or-equal comparison
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        /// <param name="queryBinaryOperation">The query binary operation. Must be LessThan or LessThanOrEqualTo.</param>
        /// <returns>The result of the comparison.</returns>
        internal QueryScalarValue PerformNullSafeLessThan(QueryScalarValue firstValue, QueryScalarValue secondValue, QueryBinaryOperation queryBinaryOperation)
        {
            ExceptionUtilities.Assert(queryBinaryOperation == QueryBinaryOperation.LessThan || queryBinaryOperation == QueryBinaryOperation.LessThanOrEqualTo, "Unsupported  binary operation type {0}", queryBinaryOperation.ToString());

            // LinqToObjects will lazy treat any LessThan or LessThanOrEqual expression
            // where the first value is null as false UNLESS there is a specific comparison
            // method supplied. It so happens that for certain types the Astoria server
            // does supply the method, so the behavior is different based on the type
            bool result;
            if (ExpectLazyNullEvaluatiorForLessThan(firstValue))
            {
                result = false;
            }
            else
            {
                var comparisonResult = this.Compare(firstValue, secondValue);
                if (queryBinaryOperation == QueryBinaryOperation.LessThan)
                {
                    result = comparisonResult < 0;
                }
                else
                {
                    result = comparisonResult <= 0;
                }
            }

            return this.BooleanType.CreateValue(result);
        }

        /// <summary>
        /// Finds the member method, including extensions defined in the type or its base types' assemblies.
        /// </summary>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>
        /// The member method info.
        /// </returns>
        protected override MethodInfo FindMemberMethod(Type instanceType, string methodName, IEnumerable<Type> argTypes)
        {
            var methodToInvoke = base.FindMemberMethod(instanceType, methodName, argTypes);

            // look for an extension method if there was no instance method
            if (methodToInvoke == null)
            {
                methodToInvoke = instanceType.GetExtensionMethod(methodName, argTypes.ToArray());
            }

            return methodToInvoke;
        }

        /// <summary>
        /// Invokes the member method.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="method">The method.</param>
        /// <param name="argValues">The argument values.</param>
        /// <returns>
        /// The result of invoking the method
        /// </returns>
        protected override object InvokeMemberMethod(object instance, MethodInfo method, object[] argValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(method, "method");
            ExceptionUtilities.CheckArgumentNotNull(argValues, "argValues");

            if (method.IsDefined(typeof(ExtensionAttribute), true))
            {
                argValues = new object[] { instance }
                    .Concat(argValues)
                    .ToArray();
            }

            this.SpatialOperationsManager.IfValid(m => m.RegisterOperations());
            return base.InvokeMemberMethod(instance, method, argValues);
        }

        /// <summary>
        /// Returns whether the type is one of the special case types for less-than. For other types Compare will be used instead by the Astoria server, resulting in slightly different behavior.
        /// </summary>
        /// <param name="clrType">The clr type.</param>
        /// <returns>Whether the type is special.</returns>
        private static bool IsSpecialCaseTypeForLessThan(Type clrType)
        {
            return clrType != typeof(string)
                && clrType != typeof(bool)
                && clrType != typeof(bool?)
                && clrType != typeof(Guid)
                && clrType != typeof(Guid?);
        }
    }
}
