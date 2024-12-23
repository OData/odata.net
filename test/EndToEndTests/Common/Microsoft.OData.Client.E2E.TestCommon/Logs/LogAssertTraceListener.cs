//-----------------------------------------------------------------------------
// <copyright file="LogAssertTraceListener.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData.Client.E2E.TestCommon.Logs;

public class LogAssertTraceListener : TraceListener
{
    public LogAssertTraceListener()
    {
        // Clear existing listeners and add this listener
        Trace.Listeners.Clear();
        Trace.Listeners.Add(this);
    }

    public override void Write(string? message) { }

    public override void WriteLine(string? message) { }

    public override void Fail(string? message, string? detailMessage)
    {
        // Log the assertion failure
        Console.WriteLine($"DEBUG ASSERTION FAILED: {message} {detailMessage}");
    }
}

