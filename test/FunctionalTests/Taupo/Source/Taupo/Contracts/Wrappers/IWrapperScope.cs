//---------------------------------------------------------------------
// <copyright file="IWrapperScope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Common services for wrappers to report calls they make agains the product.
    /// </summary>
    public interface IWrapperScope : IDisposable
    {
        /// <summary>
        /// Wraps the specified product instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result wrapper.</typeparam>
        /// <param name="product">The product instance.</param>
        /// <returns>The wrapper for the product instance.</returns>
        TResult Wrap<TResult>(object product) where TResult : IWrappedObject;

        /// <summary>
        /// Begins tracing of the call.
        /// </summary>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns>Call handle which is used to correlate parameters with result values.</returns>
        /// <remarks>
        /// If the call handle is 0, the wrapper will not call TraceResult() or TraceException()
        /// </remarks>
        int BeginTraceCall(MethodBase methodInfo, object instance, object[] parameterValues);

        /// <summary>
        /// Traces the result of a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="returnValue">The return value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", 
            Justification = "Don't want to make it a real return value because most of the time we won't want to change it")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate",
            Justification = "Don't want generics because there's a lot of reflection involved")]
        void TraceResult(int callId, MethodBase methodInfo, object instance, object[] parameterValues, ref object returnValue);

        /// <summary>
        /// Traces the exception which occured during a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="exception">The exception which occured.</param>
        void TraceException(int callId, MethodBase methodInfo, object instance, Exception exception);
    }
}