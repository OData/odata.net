//---------------------------------------------------------------------
// <copyright file="IServiceMethodResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Interface for an object that will resolve runtime dependencies for Service Methods
    /// </summary>
    [ImplementationSelector("ServiceMethodResolver", DefaultImplementation = "Default")]
    public interface IServiceMethodResolver
    {
        /// <summary>
        /// Resolves service method body.
        /// </summary>
        /// <param name="serviceMethod">the service method</param>
        void ResolveServiceMethodBody(Function serviceMethod);

        /// <summary>
        /// Generates service method code.
        /// </summary>
        /// <param name="serviceMethod">the service method</param>
        void GenerateServiceMethodCode(Function serviceMethod);
    }
}
