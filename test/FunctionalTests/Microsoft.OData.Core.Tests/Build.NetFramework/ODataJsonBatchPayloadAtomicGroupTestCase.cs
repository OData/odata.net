//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchPayloadAtomicGroupTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Test case class use for verification of Json batch request and response payload processing.
    /// </summary>
    internal class ODataJsonBatchPayloadAtomicGroupTestCase
    {
        internal string Desciption { get; set; }
        internal string RequestPayload { get; set; }
        internal Type ExceptionType { get; set; }
        internal string TokenInExceptionMessage { get; set; }

        /// <summary>
        /// Verifier for dependsOn Ids available from the request operation message.
        /// </summary>
        internal Action<ODataBatchOperationRequestMessage, IList<string>> RequestMessageDependsOnIdVerifier =
            (message, dependsOnIds) =>
            {
                Assert.Equal(message.DependsOnRequestIds, dependsOnIds);
            };

        internal IList<IList<string>> ListOfDependsOnIds;
    }
}