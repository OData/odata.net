using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.ODataClient.Tests.Netcore.Handlers
{
    public class VerificationCounter
    {
        public int ODataInvokeCount { get; internal set; }

        public int HttpInvokeCount { get; internal set; }
    }
}
