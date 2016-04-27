//---------------------------------------------------------------------
// <copyright file="BatchPayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;

    /// <summary>
    /// Payload Generator for Batch Payloads.
    /// </summary>
    [ImplementationSelector("IPayloadGenerator", DefaultImplementation = "Default", HelpText = "Return OData Payloads to enumerate over")]
    public class BatchPayloadGenerator : IPayloadGenerator
    {
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        [InjectTestParameter("TestRunKind", HelpText = "The test run kind to use for running the tests.")]
        public TestRunKind RunKind { get; set; }

        /// <summary>
        /// Not implemented for batch (as batch can have both content types in one payload)
        /// </summary>
        /// <returns>Throws exception</returns>
        public IEnumerable<EntityInstance> GenerateAtomPayloads()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented for batch (as batch can have both content types in one payload)
        /// </summary>
        /// <returns>Throws exception</returns>
        public IEnumerable<EntityInstance> GenerateJsonPayloads()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates deterministic payloads for checkin run otherwise generates arbitrary payloads.
        /// </summary>
        /// <typeparam name="T">Must be a PayloadTestDescriptor.</typeparam>
        /// <param name="payload">Payload to generate payloads from. If checkin suite only the model will be used.</param>
        /// <returns>IEnumerable of generated payloads.</returns>
        public IEnumerable<T> GeneratePayloads<T>(T payload) where T : PayloadTestDescriptor
        {
            if (this.RunKind == TestRunKind.CheckinSuite)
            {
                return (IEnumerable<T>)TestBatches.CreateBatchRequestTestDescriptors(this.RequestManager, (EdmModel) payload.PayloadEdmModel, true)
                    .Concat(TestBatches.CreateBatchResponseTestDescriptors(this.RequestManager, (EdmModel) payload.PayloadEdmModel, true));
            }
            else
            {
                return GenerateArbitraryPayloads(payload);
            }
        }

        private IEnumerable<T> GenerateArbitraryPayloads<T>(T payload) where T: PayloadTestDescriptor
        {
            // REQUEST PAYLOADS

            // Only operation or only 1 changeset
            var newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 0, 0);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;
            
            newPayload = payload.InBatchRequest(HttpVerb.Put, this.Random, this.RequestManager, 0, 0);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;
            
            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 0, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            // Last operation or changeset
            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 2, 0);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;
            
            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 2, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Delete, this.Random, this.RequestManager, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 3, 0);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            // First operation or changeset
            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Put, this.Random, this.RequestManager, 0, 2);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 0, 2);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 0, 3);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Delete, this.Random, this.RequestManager, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            // operation or changeset in middle of batch
            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 3, 1);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Put, this.Random, this.RequestManager, 1, 2);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Patch, this.Random, this.RequestManager, 2, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 1, 2);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Post, this.Random, this.RequestManager, 1, 3);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Delete, this.Random, this.RequestManager, 2, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 2, 2);
            newPayload.SkipTestConfiguration = ((config) => !config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchRequest(HttpVerb.Get, this.Random, this.RequestManager, 3, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && !config.IsRequest);
            yield return newPayload;

            // RESPONSE PAYLOADS
            // Only operation or only 1 changeset
            newPayload = payload.InBatchResponse(200, this.Random, this.RequestManager, true, 0, 0);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(404, this.Random, this.RequestManager, false, 0, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            //last operation or changeset
            newPayload = payload.InBatchResponse(405, this.Random, this.RequestManager, true, 2, 0);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(400, this.Random, this.RequestManager, false, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(950, this.Random, this.RequestManager, false, 3, 0);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(201, this.Random, this.RequestManager, true, 1, 0);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            //first operation or changeset
            newPayload = payload.InBatchResponse(204, this.Random, this.RequestManager, false, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(3, this.Random, this.RequestManager, true, 0, 2);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(207, this.Random, this.RequestManager, true, 0, 3);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(200, this.Random, this.RequestManager, false, 0, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(550, this.Random, this.RequestManager, true, 0, 3);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            //operation or changeset in middle of batch
            newPayload = payload.InBatchResponse(100, this.Random, this.RequestManager, true, 3, 1);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(200, this.Random, this.RequestManager, false, 1, 2);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(202, this.Random, this.RequestManager, true, 2, 1);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(404, this.Random, this.RequestManager, true, 2, 2);
            newPayload.SkipTestConfiguration = ((config) => config.IsRequest);
            yield return newPayload;

            newPayload = payload.InBatchResponse(5, this.Random, this.RequestManager, false, 1, 3);
            newPayload.SkipTestConfiguration = ((config) => config.Version < ODataVersion.V4 && config.IsRequest);
            yield return newPayload;
        }
    }
}
