//---------------------------------------------------------------------
// <copyright file="IDataServiceExecutionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
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
