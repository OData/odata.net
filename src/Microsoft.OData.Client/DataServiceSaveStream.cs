//---------------------------------------------------------------------
// <copyright file="DataServiceSaveStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;

    #endregion Namespaces

    /// <summary>Stream wrapper for MR POST/PUT which also holds the information if the stream should be closed or not.</summary>
    internal class DataServiceSaveStream
    {
        /// <summary>Arguments for the request when POST/PUT of the stream is issued.</summary>
        private DataServiceRequestArgs args;

        /// <summary>The stream we are wrapping.
        /// Can be null in which case we didn't open it yet.</summary>
        private Stream stream;

        /// <summary>Set to true if the stream should be closed once we're done with it.</summary>
        private bool close;

#if DEBUG
        /// <summary> True,if this instance is being deserialized via DataContractSerialization, false otherwise </summary>
        private bool deserializing;
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        /// <param name="close">Should the stream be closed before SaveChanges returns.</param>
        /// <param name="args">Additional arguments to apply to the request before sending it.</param>
        internal DataServiceSaveStream(Stream stream, bool close, DataServiceRequestArgs args)
        {
            Debug.Assert(stream != null, "stream must not be null.");

            this.stream = stream;
            this.close = close;
            this.args = args;
#if DEBUG
            this.deserializing = false;
#endif
        }

        /// <summary>The stream to use.</summary>
        internal Stream Stream
        {
            get
            {
                return this.stream;
            }
        }

        /// <summary>
        /// Arguments to be used for creation of the HTTP request when POST/PUT for the MR is issued.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "The setter is called during de-serialization")]
        internal DataServiceRequestArgs Args
        {
            get
            {
                return this.args;
            }

            set
            {
#if DEBUG
                Debug.Assert(this.deserializing, "Property can only be set when this instance is deserializing");
#endif
                this.args = value;
            }
        }

        /// <summary>
        /// Close the stream if required.
        /// This is so that callers can simply call this method and don't have to care about the settings.
        /// </summary>
        internal void Close()
        {
            if (this.stream != null && this.close)
            {
                this.stream.Close();
            }
        }
    }
}
