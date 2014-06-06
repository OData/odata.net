//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core;

    /// <summary>
    /// Public class which wraps the ODataWriter instance.
    /// </summary>
    public class DataServiceODataWriter
    {
        /// <summary>
        /// ODataWriter instance that this class wraps.
        /// </summary>
        private readonly ODataWriter innerWriter;

        /// <summary>
        /// Creates a new instance of DataServiceODataWriter.
        /// </summary>
        /// <param name="odataWriter">Instance of ODataWriter that this class wraps.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "odata", Justification = "odata is the correct spelling")]
        public DataServiceODataWriter(ODataWriter odataWriter)
        {
            WebUtil.CheckArgumentNull(odataWriter, "odataWriter");
            this.innerWriter = odataWriter;
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="args">DataServiceODataWriterFeedArgs which contains the ODataFeed and the collection instance to serialize.</param>
        public virtual void WriteStart(DataServiceODataWriterFeedArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.innerWriter.WriteStart(args.Feed);
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="args">DataServiceODataWriterEntryArgs which contains the ODataEntry and the entry instance to serialize.</param>
        public virtual void WriteStart(DataServiceODataWriterEntryArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.innerWriter.WriteStart(args.Entry);
        }

        /// <summary>
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="args">DataServiceODataWriterNavigationLinkArgs which contains the ODataNavigationLink to serialize.</param>
        public virtual void WriteStart(DataServiceODataWriterNavigationLinkArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.innerWriter.WriteStart(args.NavigationLink);
        }

        /// <summary>
        /// Finish writing a feed/entry/navigation link.
        /// </summary>
        public virtual void WriteEnd()
        {
            this.innerWriter.WriteEnd();
        }

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="args">DataServiceODataWriterFeedArgs which contains the ODataFeed and the collection instance that is being serialized.</param>
        /// <remarks>
        /// This method calls WriteEnd() and it's used to track when WriteEnd is called for feed.
        /// </remarks>
        public virtual void WriteEnd(DataServiceODataWriterFeedArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.WriteEnd();
        }

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="args">DataServiceODataWriterEntryArgs which contains the ODataEntry and the entry instance that is being serialized.</param>
        /// <remarks>
        /// This method calls WriteEnd() and it's used to track when WriteEnd is called for Entry.
        /// </remarks>
        public virtual void WriteEnd(DataServiceODataWriterEntryArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.WriteEnd();
        }

        /// <summary>
        /// Finish writing a navigation link.
        /// </summary>
        /// <param name="args">DataServiceODataWriterNavigationLinkArgs which contains the ODataNavigationLink that is being serialized.</param>
        /// <remarks>
        /// This method calls WriteEnd() and it's used to track when WriteEnd is called for Link.
        /// </remarks>
        public virtual void WriteEnd(DataServiceODataWriterNavigationLinkArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");
            this.WriteEnd();
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public virtual void Flush()
        {
            this.innerWriter.Flush();
        }
    }
}
