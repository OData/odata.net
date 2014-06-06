//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
