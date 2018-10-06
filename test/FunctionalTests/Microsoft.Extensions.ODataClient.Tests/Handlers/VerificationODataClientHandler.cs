using Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

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
            Console.WriteLine($"MaxProtocolVersion = {args.ODataClient.MaxProtocolVersion}");
            this.counter.ODataInvokeCount ++;
        }
    }
}
