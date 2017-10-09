//---------------------------------------------------------------------
// <copyright file="ServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using Microsoft.Test.OData.Framework.Server;

    /// <summary>
    /// Abstract implementation of the IServiceWrapper. This class is utilized to communicate with the service via RPC.
    /// This class is utilized to help migrate tests to an out-proc model by providing shared functionality between
    /// service classes. It is up to the subclasses to instantiate services specific to their types.
    /// </summary>
    public abstract class ServiceWrapper : IServiceWrapper
    {
        /// <summary>
        /// Handle for the service process.
        /// </summary>
        protected readonly Process ServiceProcess = new Process();

        /// <summary>
        /// Location of the ServiceWrapperApp.exe. The subclasses of this service wrapper launch
        /// the app and receive information regarding the service from the app in a server/client model.
        /// </summary>
        private readonly string ServiceLocation = @"..\..\Desktop\ServiceWrapperApp.exe";

        /// <summary>
        /// Initializes a new instance of the ServiceWrapper class.
        /// </summary>
        public ServiceWrapper()
        {
            ServiceProcess.StartInfo.FileName = ServiceLocation;
            ServiceProcess.StartInfo.CreateNoWindow = true;

            // Send text to the service. Needed to inform the service to start/stop.
            ServiceProcess.StartInfo.RedirectStandardInput = true;

            // Receive text from the service. Needed to receive the service URI from ServiceWrapperApp.exe.
            // UseShellExecute must be false for this to be true.
            ServiceProcess.StartInfo.UseShellExecute = false;
            ServiceProcess.StartInfo.RedirectStandardOutput = true;
        }

        /// <summary>
        /// Finalizer for the ServiceWrapper class.
        /// </summary>
        ~ServiceWrapper()
        {
            StopService();
        }

        /// <summary>
        /// Gets the URI for the service.
        /// </summary>
        public Uri ServiceUri { get; private set; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void StartService()
        {
            // When the service starts via an IPC command, it will output the service URI, which
            // needs to be captured here. This must be handled asynchronously but at the same
            // time, the test cannot proceed without the URI. Therefore, pull the URI string variable
            // every so often to check when it has been populated with the response from the service.
            string uri = string.Empty;
            ServiceProcess.OutputDataReceived += new DataReceivedEventHandler((sender, eventArgs) =>
            {
                uri = eventArgs.Data;
            });
            ServiceProcess.Start();
            ServiceProcess.BeginOutputReadLine();
            ServiceProcess.StandardInput.WriteLine(string.Format("{0}", (int)IPCCommandMap.ServiceCommands.StartService));

            // To ensure that the test case does not last too long, make it a max of 2 seconds (8 cycles * 250ms sleeps)
            for (int i = 0; i < 8; ++i)
            {
                if (uri.Length > 0)
                {
                    break;
                }

                Thread.Sleep(250);
            }

            ServiceUri = new Uri(uri);
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            if (!ServiceProcess.HasExited)
            {
                ServiceProcess.StandardInput.WriteLine(string.Format("{0}", (int)IPCCommandMap.ServiceCommands.StopService));
            }
        }
    }
}