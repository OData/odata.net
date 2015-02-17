//---------------------------------------------------------------------
// <copyright file="SystemNetUtility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.IO;

    /// <summary>Provides utility methods for System.Net types.</summary>
    public static class SystemNetUtility
    {
        /// <summary>Gets the System.Net.Logging type.</summary>
        public static Type SystemNetLogging
        {
            [DebuggerStepThrough]
            get
            {
                Assembly assembly = typeof(System.Net.WebRequest).Assembly;
                return assembly.GetType("System.Net.Logging", true);
            }
        }

        /// <summary>Gets the 'Sockets' <see cref='TraceSource' />.</summary>
        public static TraceSource SystemNetSocketsSource
        {
            get
            {
                PropertyInfo property = ReflectionUtility.GetProperty(SystemNetLogging, "Sockets");
                return (TraceSource)property.GetValue(null, TestUtil.EmptyObjectArray);
            }
        }

        /// <summary>Gets the 'Web' <see cref='TraceSource' />.</summary>
        public static TraceSource SystemNetWebSource
        {
            get
            {
                PropertyInfo property = ReflectionUtility.GetProperty(SystemNetLogging, "Web");
                return (TraceSource)property.GetValue(null, TestUtil.EmptyObjectArray);
            }
        }

        public static SystemNetLoggingScope CreateLoggingScope()
        {
            return new SystemNetLoggingScope();
        }

        public class SystemNetLoggingScope : IDisposable
        {
            private readonly StringWriter writer;

            public SystemNetLoggingScope()
            {
                this.writer = new StringWriter();

                ReflectionUtility.InvokeMethod(SystemNetLogging, "InitializeLogging");
                ReflectionUtility.SetField(SystemNetLogging, "s_LoggingEnabled", true);
                System.Diagnostics.TraceSource s = SystemNetWebSource;
                s.Switch.Level = SourceLevels.All;
                s.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(writer));
            }

            public void Clear()
            {
                this.writer.GetStringBuilder().Length = 0;
            }

            public void FlushToTrace()
            {
                Trace.WriteLine(this.ToString());
                this.Clear();
            }

            public void Dispose()
            {
                ReflectionUtility.InvokeMethod(SystemNetLogging, "Close");
                ReflectionUtility.SetField(SystemNetLogging, "s_LoggingEnabled", false);
            }

            public override string ToString()
            {
                return this.writer.ToString();
            }
        }
    }
}
