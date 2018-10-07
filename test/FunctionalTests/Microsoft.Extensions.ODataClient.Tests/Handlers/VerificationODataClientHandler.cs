//---------------------------------------------------------------------
// <copyright file="VerificationODataClientHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers;
using System;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore
{
    public class VerificationODataClientHandler : IODataClientHandler
    {
        private readonly VerificationCounter counter;

        public VerificationODataClientHandler(VerificationCounter counter)
        {
            this.counter = counter;
        }

        public void OnClientCreated(ClientCreatedArgs args)
        {
            var client = args.ODataClient;
            Console.WriteLine($"MaxProtocolVersion = {client.MaxProtocolVersion}");

            // set header and serialization type
            client.SendingRequest2 += (s, e) =>
            {
                client.Format.UseJson();
                e.RequestMessage.SetHeader("api-version", args.ODataClient.Configurations.Properties["api-version"] as string);
            };

            this.counter.ODataInvokeCount ++;
        }
    }
}
