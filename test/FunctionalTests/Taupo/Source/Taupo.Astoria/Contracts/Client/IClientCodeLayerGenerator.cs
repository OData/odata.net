//---------------------------------------------------------------------
// <copyright file="IClientCodeLayerGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for generating client code layers.
    /// </summary>
    [ImplementationSelector("ClientCodeLayerGenerator", DefaultImplementation = "Remote")]
    public interface IClientCodeLayerGenerator
    {
        /// <summary>
        /// Generates the client-side proxy classes then calls the given callback
        /// </summary>
        /// <param name="continuation">The async continuation to report completion on</param>
        /// <param name="serviceRoot">The root uri of the service</param>
        /// <param name="model">The model for the service</param>
        /// <param name="language">The language to generate code in</param>
        /// <param name="onCompletion">The action to invoke with the generated code</param>
        void GenerateClientCode(IAsyncContinuation continuation, Uri serviceRoot, EntityModelSchema model, IProgrammingLanguageStrategy language, Action<string> onCompletion);
    }
}
