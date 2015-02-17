//---------------------------------------------------------------------
// <copyright file="IResponseVerificationContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for encapsulating common logic and state used while verifiying a request.
    /// Primarily used so that verifiers do not accidentally change local state in a way that breaks other verifiers.
    /// All local state changes (such as synchronization) should be done through this contract only.
    /// </summary>
    [ImplementationSelector("ResponseVerificationContext", DefaultImplementation = "Default")]
    public interface IResponseVerificationContext
    {
        /// <summary>
        /// Informs the context that verification for the given request has begun. Can be called multiple times.
        /// Disposing the returned value indicates that verification has finished.
        /// </summary>
        /// <param name="request">The request being verified</param>
        /// <returns>A disposable token which, when disposed, indicates that verification is finished.</returns>
        IDisposable Begin(ODataRequest request);

        /// <summary>
        /// Gets the entity inserted by the given request.
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="response">The response</param>
        /// <returns>The inserted entity</returns>
        QueryStructuralValue GetInsertedEntity(ODataRequest request, ODataResponse response);

        /// <summary>
        /// Gets the pre and post update representations of the entity updated by the given request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="beforeUpdate">The entity before the update</param>
        /// <param name="afterUpdate">The entity after the update</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Need to return both values")]
        void GetUpdatedEntity(ODataRequest request, out QueryStructuralValue beforeUpdate, out QueryStructuralValue afterUpdate);
    }
}
