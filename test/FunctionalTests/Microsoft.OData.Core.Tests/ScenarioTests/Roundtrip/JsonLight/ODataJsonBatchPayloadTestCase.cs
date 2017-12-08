//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchPayloadTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Xunit;

    /// <summary>
    /// Test case class use for verification of Json batch request and response payload processing.
    /// </summary>
    internal class ODataJsonBatchPayloadTestCase
    {
        internal string Description { get; set; }
        internal string RequestPayload { get; set; }
        internal Type ExceptionType { get; set; }
        internal string TokenInExceptionMessage { get; set; }

        internal IList<IList<string>> ListOfDependsOnIds;


        /// <summary>
        /// Verifier for dependsOn Ids available from the request operation message.
        /// </summary>
        internal Action<ODataBatchOperationRequestMessage, IList<string>> RequestMessageDependsOnIdVerifier =
            (message, dependsOnIds) =>
            {
                Assert.NotSame(message.DependsOnIds, dependsOnIds);
                Assert.Equal(message.DependsOnIds, dependsOnIds);
            };

        internal delegate void ValidateContentType(ODataBatchOperationRequestMessage requestMessage, int offset);
        internal ValidateContentType ContentTypeVerifier;

        /// <summary>
        /// Populate payload by replacing the designated token with encoded content.
        /// </summary>
        /// <param name="token">The token to be replaced.</param>
        /// <param name="encodedContent">The encoded content to be populated.</param>
        internal void PopulateEncodedContent(string token, string encodedContent)
        {
            RequestPayload = Regex.Replace(RequestPayload,
                        "\"body\": \"" + token +"\"",
                        "\"body\": \"" + encodedContent + "\"",
                        RegexOptions.Multiline);
        }
    }
}