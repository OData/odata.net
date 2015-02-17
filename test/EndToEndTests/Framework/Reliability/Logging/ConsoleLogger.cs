//---------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Console Logger
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Log id, the console logger is singleton
        /// </summary>
        private static readonly Guid LoggerID = Guid.NewGuid();

        /// <summary>
        /// the console stream write locker
        /// </summary>
        private readonly object writeLocker = new object();

        /// <summary>
        /// Std ouput handle id
        /// </summary>
        private const int StdOutputHandle = -11;

        /// <summary>
        /// DOS US page code
        /// </summary>
        private const int CodePageDosUs = 437;

        /// <summary>
        /// Console Logger
        /// </summary>
        private static TextWriter consoleSw = Console.Out;

        /// <summary>
        /// We only want to detach the console once in the process lifetime
        /// </summary>
        private static bool detached;

        /// <summary>
        /// Gets log id
        /// </summary>
        public Guid ID
        {
            get { return LoggerID; }
        }

        /// <summary>
        /// Detach the current process from current console and alloc a new console
        /// </summary>
        public static void Detach()
        {
            if (!detached)
            {
                // Always try to free the console of MSTEST attached.
                FreeConsole();

                if (AllocConsole() != 0)
                {
                    IntPtr stdHandle = GetStdHandle(StdOutputHandle);
                    var safeFileHandle = new SafeFileHandle(stdHandle, true);
                    var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                    Encoding encoding = Encoding.GetEncoding(CodePageDosUs);
                    consoleSw = new StreamWriter(fileStream, encoding) { AutoFlush = true };
                }
                else
                {
                    consoleSw = Console.Out;
                }

                detached = true;
            }
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="callid">call id</param>
        /// <param name="msg">Message txt</param>
        public void Log(TraceEventType type, string callid, string msg)
        {
            if (type != TraceEventType.Transfer)
            {
                lock (this.writeLocker)
                {
                    var original = Console.ForegroundColor;
                    try
                    {
                        this.SetConsoleColor(type);
                        consoleSw.WriteLine("{0} {1} {2}", type.ToString().ToBracketString(), callid.ToBracketString(), msg);
                    }
                    catch (Exception)
                    {
                        // sometimes there is an error System.ObjectDisposedException: Cannot write to a closed TextWriter
                        Console.Out.WriteLine("{0} {1}", type.ToString().ToBracketString(), msg);
                    }
                    finally
                    {
                        Console.ForegroundColor = original;
                    }
                }
            }
        }

        /// <summary>
        /// Add additional log file
        /// </summary>
        /// <param name="callid">the call id</param>
        /// <param name="filePath">the file path</param>
        public void AddFile(string callid, string filePath)
        {
            this.Log(TraceEventType.Information, callid, string.Format("Additional log file : {0}.", filePath));
        }

        /// <summary>
        /// Get standard handle of current console
        /// </summary>
        /// <param name="stdHandle">starndard handle id</param>
        /// <returns>The standard handle</returns>
        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int stdHandle);

        /// <summary>
        /// Allocate a console to current process
        /// </summary>
        /// <returns>If function failed, return Zero. Otherwise, returns non-Zero</returns>
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        /// <summary>
        /// Free attached a console of current process
        /// </summary>
        /// <returns>If function failed, return Zero. Otherwise, returns non-Zero</returns>
        [DllImport("kernel32.dll",
            EntryPoint = "FreeConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int FreeConsole();

        /// <summary>
        /// Set Console Color for different message type
        /// </summary>
        /// <param name="type">message type</param>
        private void SetConsoleColor(TraceEventType type)
        {
            switch (type)
            {
                case TraceEventType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
        }
    }
}
