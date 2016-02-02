//---------------------------------------------------------------------
// <copyright file="WriteReadFeedTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    public class WriteReadFeedTestBase
    {
        /// <summary>
        /// Writes an OData Feed with number <see cref="numberOfEntries"/> of entries <see cref="entry"/>
        /// </summary>
        /// <param name="writeStream"></param>
        /// <param name="edmModel"></param>
        /// <param name="numberOfEntries"></param>
        /// <param name="entry"></param>
        /// <param name="entitySet"></param>
        /// <returns>The payload size</returns>
        protected Int64 WriteFeed(Stream writeStream, IEdmModel edmModel, long numberOfEntries, ODataEntry entry, IEdmEntitySetBase entitySet)
        {
            using (var messageWriter = ODataMessageHelper.CreateMessageWriter(writeStream, edmModel))
            {
                ODataWriter writer = messageWriter.CreateODataFeedWriter(entitySet);
                writer.WriteStart(new ODataFeed { Id = new Uri("http://www.odata.org/Perf.svc") });
                for (long i = 0; i < numberOfEntries; ++i)
                {
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                }
                writer.WriteEnd();
                writer.Flush();
            }

            return writeStream.Length; // return payload size
        }

        /// <summary>
        /// Reads feed from stream
        /// </summary>
        /// <param name="readStream"></param>
        /// <param name="edmModel"></param>
        /// <param name="entitySet"></param>
        /// <param name="expectedBaseEntityType"></param>
        protected void ReadFeed(Stream readStream, IEdmModel edmModel, IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            readStream.Seek(0, SeekOrigin.Begin);
            using (var messageReader = ODataMessageHelper.CreateMessageReader(readStream, edmModel))
            {
                ODataReader feedReader = messageReader.CreateODataFeedReader(entitySet, expectedBaseEntityType);
                while (feedReader.Read()) { }
            }
        }
    }
}
