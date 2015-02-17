//---------------------------------------------------------------------
// <copyright file="ODataBatchWriterTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Batch writer test case used to specify the payload generator for batch writer tests.
    /// </summary>
    public class ODataBatchWriterTestCase : ODataWriterTestCase
    {
        /// <summary>
        /// Does the dependency injection of the BatchPayloadGenerator
        /// </summary>
        /// <param name="container">The container to inject the dependencies into.</param>
        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.Register<IPayloadGenerator, BatchPayloadGenerator>();
        }
    }
}
