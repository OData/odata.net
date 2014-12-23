//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core;

    /// <summary>
    /// Forwards calls to an OData Writer
    /// </summary>
    internal class ODataWriterWrapper
    {
        /// <summary> The odataWriter. </summary>
        private readonly ODataWriter odataWriter;

        /// <summary> The payload writing events. </summary>
        private readonly DataServiceClientRequestPipelineConfiguration requestPipeline;

        /// <summary>
        /// Prevents a default instance of the <see cref="ODataWriterWrapper"/> class from being created.
        /// </summary>
        /// <param name="odataWriter">The odata writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        private ODataWriterWrapper(ODataWriter odataWriter, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            Debug.Assert(odataWriter != null, "odataWriter != null");
            Debug.Assert(requestPipeline != null, "DataServiceClientRequestPipelineConfiguration != null");

            this.odataWriter = odataWriter;
            this.requestPipeline = requestPipeline;
        }

        /// <summary>
        /// Creates the odata entry writer.
        /// </summary>
        /// <remarks>We never create a feed writer as the client doesn't support deep insert, if we did this would need to change</remarks>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        /// <returns>The odata Writer Wrapper</returns>
        internal static ODataWriterWrapper CreateForEntry(ODataMessageWriter messageWriter, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            return new ODataWriterWrapper(messageWriter.CreateODataEntryWriter(), requestPipeline);
        }

        /// <summary>
        /// Creates the odata entry writer for testing purposes only
        /// </summary>
        /// <param name="writer">The odata writer.</param>
        /// <param name="requestPipeline">The request pipeline configuration.</param>
        /// <returns>The odata Writer Wrapper</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:'ODataReaderWrapper.CreateForTest(ODataReader)' appears to have no upstream public or protected callers.", Justification = "Method is only used for testing purposes.")]
        internal static ODataWriterWrapper CreateForEntryTest(ODataWriter writer, DataServiceClientRequestPipelineConfiguration requestPipeline)
        {
            return new ODataWriterWrapper(writer, requestPipeline);
        }

        /// <summary>
        /// Writes the start.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void WriteStart(ODataEntry entry, object entity)
        {
            this.requestPipeline.ExecuteOnEntryStartActions(entry, entity);
            this.odataWriter.WriteStart(entry);
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="entity">The entity.</param>
        internal void WriteEnd(ODataEntry entry, object entity)
        {
            this.requestPipeline.ExecuteOnEntryEndActions(entry, entity);
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the end.
        /// </summary>
        /// <param name="navlink">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteEnd(ODataNavigationLink navlink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNavigationLinkEndActions(navlink, source, target);
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the end Actions.
        /// </summary>
        /// <param name="navlink">The link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteNavigationLinkEnd(ODataNavigationLink navlink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNavigationLinkEndActions(navlink, source, target);
        }

        /// <summary>
        /// Writes the navigation link end.
        /// </summary>
        internal void WriteNavigationLinksEnd()
        {
            this.odataWriter.WriteEnd();
        }

        /// <summary>
        /// Writes the start.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteStart(ODataNavigationLink navigationLink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNavigationLinkStartActions(navigationLink, source, target);
            this.odataWriter.WriteStart(navigationLink);
        }

        /// <summary>
        /// Writes the start for Navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteNavigationLinkStart(ODataNavigationLink navigationLink, object source, object target)
        {
            this.requestPipeline.ExecuteOnNavigationLinkStartActions(navigationLink, source, target);
        }

        /// <summary>
        /// Writes the start for a collection of navigation links.
        /// </summary>
        /// <param name="navigationLink">The navigation link.</param>
        internal void WriteNavigationLinksStart(ODataNavigationLink navigationLink)
        {
            this.odataWriter.WriteStart(navigationLink);
        }

        /// <summary>
        /// Writes the entity reference link.
        /// </summary>
        /// <param name="referenceLink">The reference link.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal void WriteEntityReferenceLink(ODataEntityReferenceLink referenceLink, object source, object target)
        {
            this.requestPipeline.ExecuteEntityReferenceLinkActions(referenceLink, source, target);
            this.odataWriter.WriteEntityReferenceLink(referenceLink);
        }
    }
}
