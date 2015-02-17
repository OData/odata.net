//---------------------------------------------------------------------
// <copyright file="SaveStreamData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Data structure for the arguments to SetSaveStream
    /// </summary>
    public sealed class SaveStreamData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStreamData"/> class.
        /// </summary>
        /// <param name="streamLogger">A logger for the save stream</param>
        /// <param name="closeStream">A value indicating whether or not the stream should be closed</param>
        /// <param name="headers">The headers for the save stream request</param>
        internal SaveStreamData(IStreamLogger streamLogger, bool closeStream, IEnumerable<KeyValuePair<string, string>> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamLogger, "streamLogger");
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");
            this.StreamLogger = streamLogger;
            this.CloseStream = closeStream;
            this.Headers = headers;
        }

        /// <summary>
        /// Gets a logger for the stream to be saved
        /// </summary>
        public IStreamLogger StreamLogger { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the stream should be closed after saving it
        /// </summary>
        public bool CloseStream { get; private set; }

        /// <summary>
        /// Gets the headers for the save stream request
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ SaveStreamData {{ Closed = {0}, CloseStream = {1}, Headers = {{ {2} }}}}}}",
                this.StreamLogger.IsClosed,
                this.CloseStream,
                string.Join(", ", this.Headers.Select(h => h.Key).ToArray()));
        }

        /// <summary>
        /// Clones the current save stream data
        /// </summary>
        /// <returns>A clone of the current save stream data</returns>
        public SaveStreamData Clone()
        {
            return new SaveStreamData(this.StreamLogger, this.CloseStream, this.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value)));
        }
    }
}