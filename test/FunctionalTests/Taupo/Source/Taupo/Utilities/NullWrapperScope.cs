//---------------------------------------------------------------------
// <copyright file="NullWrapperScope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Stub implementation if IWrapperScope which does not do any tracing, but is only capable of creating wrappers.
    /// </summary>
    public sealed class NullWrapperScope : IWrapperScope
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wraps the specified product instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result wrapper.</typeparam>
        /// <param name="product">The product instance.</param>
        /// <returns>The wrapper for the product instance.</returns>
        public TResult Wrap<TResult>(object product)
            where TResult : IWrappedObject
        {
            if (product == null)
            {
                return default(TResult);
            }

            ExceptionUtilities.Assert(!(product is IWrappedObject), "Cannot wrap a wrapper.");

            ConstructorInfo ctor = typeof(TResult).GetInstanceConstructors(true).Single();
            return (TResult)ctor.Invoke(new object[] { this, product });
        }

        /// <summary>
        /// Begins tracing of the call.
        /// </summary>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns>
        /// Call handle which is used to correlate parameters with result values.
        /// </returns>
        /// <remarks>
        /// If the call handle is 0, the wrapper will not call TraceResult() or TraceException()
        /// </remarks>
        public int BeginTraceCall(MethodBase methodInfo, object instance, object[] parameterValues)
        {
            return 0;
        }

        /// <summary>
        /// Traces the result of a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="returnValue">The return value.</param>
        public void TraceResult(int callId, MethodBase methodInfo, object instance, object[] parameterValues, ref object returnValue)
        {
        }

        /// <summary>
        /// Traces the exception which occured durion a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="exception">The exception which occured.</param>
        public void TraceException(int callId, MethodBase methodInfo, object instance, Exception exception)
        {
        }
    }
}
