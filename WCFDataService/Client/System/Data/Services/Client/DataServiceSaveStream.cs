//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Client
{
    #region Namespaces

    using System.Diagnostics;
    using System.IO;

#if WINDOWS_PHONE
    using System.Runtime.Serialization;
#endif
    #endregion Namespaces

    /// <summary>Stream wrapper for MR POST/PUT which also holds the information if the stream should be closed or not.</summary>
#if WINDOWS_PHONE
    [DataContract]
#endif
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

#if WINDOWS_PHONE
        /// <summary>
        /// The content of the stream we need to save during tombstoning
        /// </summary>
        [DataMember]
        internal byte[] StreamContent
        {
            get
            {
                if (this.stream.CanSeek && this.stream.CanRead)
                {
                    if (this.stream.Position == 0)
                    {
                        byte[] data = new byte[this.stream.Length];
                        this.stream.Read(data, 0, data.Length);
                        this.stream.Seek(0, SeekOrigin.Begin);
                        return data;
                    }
                    else
                    {
                        // DEVNOTE(pqian):
                        // Normally we would not restrict the user to set a partially read stream
                        // However, for serialization, we cannot know if we have initiated the read or
                        // the stream was read before passing in to us. Therefore it's dangerous to serialize
                        // a partially read stream. (We could end up recoverying successfully and sending
                        // only part of the data to the server)
                        throw new InvalidOperationException(Strings.Context_SaveStreamHasBeenRead);
                    }
                }
                else
                {
                    throw new InvalidOperationException(Strings.Context_SaveStreamNonSeekable);
                }
            }

            set
            {
#if DEBUG 
                Debug.Assert(this.deserializing, "Property can only be set when this instance is deserializing");   
#endif
                this.stream = new MemoryStream(value);
            }
        }

        /// <summary>
        /// Settable property to save the close property on this instance.
        /// </summary>
        [DataMember]
        internal bool ShouldClose
        {
            get
            {
                return this.close;
            }

            set
            {
#if DEBUG 
                Debug.Assert(this.deserializing, "Property can only be set when this instance is deserializing");   
#endif
                this.close = value;
            }
        }
#endif
        /// <summary>
        /// Arguments to be used for creation of the HTTP request when POST/PUT for the MR is issued.
        /// </summary>
#if WINDOWS_PHONE
        [DataMember]
#endif
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
#if PORTABLELIB
                this.stream.Dispose();
#else
                this.stream.Close();
#endif
            }
        }

#if DEBUG && WINDOWS_PHONE
        /// <summary>
        /// Called during deserialization of this instance by DataContractSerialization
        /// </summary>
        /// <param name="context">Streaming context for this deserialization session</param>
        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
        }

        /// <summary>
        /// Called after this instance has been deserialized by DataContractSerialization
        /// </summary>
        /// <param name="context">Streaming context for this deserialization session</param>
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
        }
#endif
    }
}
