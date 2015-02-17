//---------------------------------------------------------------------
// <copyright file="DataServiceExecutionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System.Diagnostics;

    /// <summary>
    /// Default internal implementation of IDataServiceExecutionProvider.
    /// </summary>
    internal class DataServiceExecutionProvider : IDataServiceExecutionProvider
    {
        /// <summary>
        /// Invokes an expression that represents the full request.
        /// </summary>
        /// <param name="requestExpression"> An expression that includes calls to
        /// one or more MethodInfo or one or more calls to 
        /// IDataServiceUpdateProvider2.InvokeAction(..) or 
        /// IDataServiceQueryProvider2.InvokeFunction(..)</param>
        /// <param name="operationContext"> Current context. </param>
        /// <returns>The result of the invoked expression.</returns>
        public object Execute(System.Linq.Expressions.Expression requestExpression, DataServiceOperationContext operationContext)
        {
            Debug.Assert(requestExpression != null, "requestExpression != null");
            Debug.Assert(operationContext != null, "operationContext != null");

            return ExpressionEvaluator.Evaluate(requestExpression);
        }
    }
}
