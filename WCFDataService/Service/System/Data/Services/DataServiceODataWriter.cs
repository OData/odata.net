//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.OData;

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
