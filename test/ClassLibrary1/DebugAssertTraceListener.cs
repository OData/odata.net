#if !NETCOREAPP1_1
namespace Microsoft.OData.TestCommon
{
    using System;
    using System.Diagnostics;

    public sealed class DebugAssertTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        public override void Fail(string message)
        {
            throw new Exception(message);
        }

        public override void Fail(string message, string detailMessage)
        {
            Fail($"Message: {message}. Details: {detailMessage}.");
        }
    }
}
#endif