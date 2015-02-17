//---------------------------------------------------------------------
// <copyright file="ClrCanonicalFunctionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Implementation of <see cref="IClrCanonicalFunctionEvaluator"/> 
    /// </summary>
    [ImplementationName(typeof(IClrCanonicalFunctionEvaluator), "Default")]
    public class ClrCanonicalFunctionEvaluator : IClrCanonicalFunctionEvaluator
    {
        private readonly Dictionary<string, Func<string, QueryType, QueryValue[], QueryValue>> functionNameToEvaluationFunc;
        
        /// <summary>
        /// Initializes a new instance of the ClrCanonicalFunctionEvaluator class.
        /// </summary>
        public ClrCanonicalFunctionEvaluator()
        {
            this.functionNameToEvaluationFunc = new Dictionary<string, Func<string, QueryType, QueryValue[], QueryValue>>()
            {
                // Math
                { "Ceiling", this.EvaluateMathMethod },
                { "Floor", this.EvaluateMathMethod },
                { "Round", this.EvaluateMathMethod },

                // String
                { "Concat", this.EvaluateStringMethod },
                { "EndsWith", this.EvaluateStringMethod }, 
                { "IndexOf", this.EvaluateStringMethod }, 
                { "Length", this.EvaluateProperty },
                { "Replace", this.EvaluateStringMethod },
                { "Substring", this.EvaluateStringMethod }, 
                { "StartsWith", this.EvaluateStringMethod },
                { "ToLower", this.EvaluateStringMethod },
                { "ToUpper", this.EvaluateStringMethod }, 
                { "Trim", this.EvaluateStringMethod }, 
   
                // Date and Time
                { "Year", this.EvaluateProperty },
                { "Month", this.EvaluateProperty },
                { "Day", this.EvaluateProperty },
                { "Hour", this.EvaluateProperty },
                { "Minute", this.EvaluateProperty },
                { "Second", this.EvaluateProperty },    
                { "Hours", this.EvaluateProperty },
                { "Minutes", this.EvaluateProperty },
                { "Seconds", this.EvaluateProperty },                
            };
        }

        /// <summary>
        /// Evaluates canonical function
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="arguments">The arguments values for the function call.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        public QueryValue Evaluate(QueryType resultType, string functionName, params QueryValue[] arguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(resultType, "resultType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(functionName, "functionName");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            Func<string, QueryType, QueryValue[], QueryValue> evaluationFunc;
            ExceptionUtilities.Assert(this.functionNameToEvaluationFunc.TryGetValue(functionName, out evaluationFunc), "Cannot evaluate cannonical function {0}.", functionName);

            return evaluationFunc(functionName, resultType, arguments);
        }

        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="type">The query type.</param>
        /// <param name="expectedCount">The expected argument count.</param>
        /// <param name="arguments">The arguments.</param>
        private static void ValidateArguments(string functionName, QueryType type, int? expectedCount, QueryValue[] arguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(functionName, "functionName");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            ExceptionUtilities.Assert(type is QueryScalarType, "Type for function '{0}' was not a scalar type", functionName);
            ExceptionUtilities.Assert(arguments.All(a => a is QueryScalarValue), "Arguments for function '{0}' were not all scalar", functionName);

            if (expectedCount.HasValue)
            {
                ExceptionUtilities.Assert(arguments.Length == expectedCount.Value, "Number of arguments for function '{0}' was wrong. Expected {1} but got {2}", functionName, expectedCount.Value, arguments.Length);
            }
        }

        /// <summary>
        /// Evaluates canonical function.
        /// </summary>
        /// <param name="resultType">The function result type.</param>
        /// <param name="classType">Type of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments for the function call.</param>
        /// <returns>
        /// Query value which is the result of the function evaluation.
        /// </returns>
        private QueryValue EvaluateMethod(QueryScalarType resultType, Type classType, string methodName, params QueryScalarValue[] arguments)
        {
            var argsTypes = arguments.Select(a => ((IQueryClrType)a.Type).ClrType).ToArray();
            var method = classType.GetMethod(methodName, argsTypes);
            if (method == null)
            {
                argsTypes = argsTypes.Skip(1).ToArray();
                method = classType.GetMethod(methodName, argsTypes, true, false);
            }

            ExceptionUtilities.CheckObjectNotNull(method, "Could not find instance or static method '{0}' on type '{1}'", methodName, classType.FullName);

            object[] argsValues = arguments.Select(a => a.Value).ToArray();

            object value = null;
            if (method.IsStatic)
            {
                value = method.Invoke(null, argsValues);
            }
            else if (argsValues[0] != null)
            {
                // null needs to be propagated for instance methods, otherwise this will fail
                value = method.Invoke(argsValues[0], argsValues.Skip(1).ToArray());
            }

            return resultType.CreateValue(value);
        }

        /// <summary>
        /// Evaluates the property.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// Query value which is the result of the function evaluation.
        /// </returns>
        private QueryValue EvaluateProperty(QueryScalarType resultType, string propertyName, QueryScalarValue instance)
        {
            if (instance.Value == null)
            {
                // TODO: evaluation error?
                return resultType.NullValue;
            }

            var type = instance.Value.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            ExceptionUtilities.CheckObjectNotNull(propertyInfo, "Could not find property named '{0}' on '{1}'", propertyName, instance.Value);

            var value = propertyInfo.GetValue(instance.Value, null);
            return resultType.CreateValue(value);
        }

        // Math
        private QueryValue EvaluateMathMethod(string functionName, QueryType type, QueryValue[] arguments)
        {
            ValidateArguments(functionName, type, 1, arguments);
            return this.EvaluateMethod((QueryScalarType)type, typeof(Math), functionName, (QueryScalarValue)arguments[0]);
        }

        // String
        private QueryValue EvaluateStringMethod(string functionName, QueryType type, QueryValue[] arguments)
        {
            ValidateArguments(functionName, type, null, arguments);
            return this.EvaluateMethod((QueryScalarType)type, typeof(string), functionName, arguments.Cast<QueryScalarValue>().ToArray());
        }
        
        private QueryValue EvaluateProperty(string functionName, QueryType type, QueryValue[] arguments)
        {
            ValidateArguments(functionName, type, 1, arguments);
            return this.EvaluateProperty((QueryScalarType)type, functionName, (QueryScalarValue)arguments[0]);
        }
    }
}