//---------------------------------------------------------------------
// <copyright file="FileLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// The file logger class
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        /// <summary>
        /// The logger id
        /// </summary>
        private readonly Guid loggerID = Guid.NewGuid();

        /// <summary>
        /// The writer lock
        /// </summary>
        private readonly object writerLocker = new object();

        /// <summary>
        /// The streamwriter to file
        /// </summary>
        private readonly StreamWriter sw;

        /// <summary>
        /// Log folder path
        /// </summary>
        private static readonly string LogFolder = Path.Combine("TestLogs", DateTime.Now.ToString("yyyy-MM-dd"));

        private readonly string filename;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the FileLogger class
        /// </summary>
        /// <param name="filename">The file name</param>
        public FileLogger(string filename)
        {
            filename = Path.Combine(LogFolder, string.Format("{0}_{1}.csv", filename, DateTime.Now.ToString("HH-mm-ss")));
            this.TransferOnly = false;
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }

            Reliability.Log.Info("Logging into : {0}", filename);

            this.sw = new StreamWriter(filename, true);
            this.filename = filename;
            this.sw.Flush();
        }

        /// <summary>
        /// Gets logger id
        /// </summary>
        public virtual Guid ID
        {
            get { return this.loggerID; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether transfer only
        /// </summary>
        public bool TransferOnly { get; set; }

        /// <summary>
        /// Interface IDisposable 
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (null != this.sw)
                {
                    this.sw.Close();
                    Reliability.Log.AddFile(this.filename);
                }

                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Add file
        /// </summary>
        /// <param name="callid">The call id</param>
        /// <param name="filePath">The file path</param>
        public void AddFile(string callid, string filePath)
        {
            // do nothing
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="callid">The call id</param>
        /// <param name="msg">Message txt</param>
        public void Log(TraceEventType type, string callid, string msg)
        {
            lock (this.writerLocker)
            {
                if (type == TraceEventType.Transfer)
                {
                    if (msg.StartsWith("["))
                    {
                        int index = msg.IndexOf("] ");
                        if (index != -1)
                        {
                            msg = msg.Substring(index + 2);
                        }
                    }

                    this.sw.WriteLine("{0},{1},{2}", type, callid, msg);
                    this.sw.Flush();
                }
                else if (!this.TransferOnly)
                {
                    this.sw.WriteLine(
                        "{0},{1},{2}", type, callid, msg.Replace("\r", "\\r").Replace("\n", "\\n").Replace(",", "$(comma)"));
                    this.sw.Flush();
                }
            }
        }
    }
}
