//---------------------------------------------------------------------
// <copyright file="WebServerLocatorCompleteEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Holds information on a web server found by the lookup service
    /// </summary>
    public class WebServerLocatorCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the WebServerLocatorCompleteEventArgs class
        /// </summary>
        /// <param name="machineName">MachineName Found, should be null if none found</param>
        /// <param name="traces">traces that occurred when looking for a web server</param>
        /// <param name="errors">errors that occurred when looking for a web server</param>
        internal WebServerLocatorCompleteEventArgs(string machineName, string[] traces, string[] errors)
        {
            this.Traces = new ReadOnlyCollection<string>(traces);
            this.Errors = new ReadOnlyCollection<string>(errors);
            this.MachineName = machineName;
        }

        /// <summary>
        /// Gets the name of the machine.
        /// </summary>
        /// <value>The name of the machine.</value>
        public string MachineName { get; private set; }
        
        /// <summary>
        /// Gets a list of errors that occurred when looking for a webserver
        /// </summary>
        public ReadOnlyCollection<string> Errors { get; private set; }

        /// <summary>
        /// Gets a list of trace messages that occurred when looking for a webserver
        /// </summary>
        public ReadOnlyCollection<string> Traces { get; private set; }
    }
}
