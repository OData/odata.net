//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    using System.Linq.Expressions;

    /// <summary>
    /// Interface to be implemented by providers who want to support actions and functions.
    /// </summary>
    internal interface IDataServiceExecutionProvider
    {
        /// <summary>
        /// Invokes an expression that represents the full request.
        /// </summary>
        /// <param name="requestExpression"> An expression that includes calls to
        /// one or more MethodInfo or one or more calls to 
        /// IDataServiceUpdateProvider2.InvokeAction(..) or 
        /// IDataServiceQueryProvider2.InvokeFunction(..)</param>
        /// <param name="context"> Current context.</param>
        /// <returns> The result of the invoked expression.</returns>
        object Execute(
            Expression requestExpression, 
            DataServiceOperationContext context);
    }
}
