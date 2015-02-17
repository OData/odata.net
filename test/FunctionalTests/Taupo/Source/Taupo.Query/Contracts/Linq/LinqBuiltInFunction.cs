//---------------------------------------------------------------------
// <copyright file="LinqBuiltInFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts.Linq
{
    using Microsoft.Test.Taupo.Common;
    
    /// <summary>
    /// Linq built-in function (i.e. canonical or provider-specific built-in function).
    /// </summary>
    public class LinqBuiltInFunction : AnnotatedQueryItem
    {
        /// <summary>
        /// Initializes a new instance of the LinqBuiltInFunction class
        /// </summary>
        /// <param name="className">The class name that contains the method for the function call.</param>
        /// <param name="methodName">The method name corresponding to the function.</param>
        /// <param name="queryFunction">The query function metadata.</param>
        /// <param name="functionType">Type of the function.</param>
        public LinqBuiltInFunction(string className, string methodName, QueryBuiltInFunction queryFunction, LinqBuiltInFunctionKind functionType)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(className, "className");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(methodName, "methodName");
            ExceptionUtilities.CheckArgumentNotNull(queryFunction, "queryFunction");
            ExceptionUtilities.CheckArgumentNotNull(functionType, "functionType");

            this.ClassName = className;
            this.MethodName = methodName;
            this.BuiltInFunction = queryFunction;
            this.BuiltInFunctionKind = functionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqBuiltInFunction"/> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="queryFunction">The query function.</param>
        public LinqBuiltInFunction(string className, string methodName, QueryBuiltInFunction queryFunction) :
            this(className, methodName, queryFunction, LinqBuiltInFunctionKind.StaticMethod)
        {
        }

        /// <summary>
        /// Gets the class name to use for the function call in a linq query.
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        /// Gets the method name to use for the function call in a linq query.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the corresponding query function.
        /// </summary>
        public QueryBuiltInFunction BuiltInFunction { get; private set; }

        /// <summary>
        /// Gets the type of the built in function.
        /// </summary>
        public LinqBuiltInFunctionKind BuiltInFunctionKind { get; private set; }
    }
}
