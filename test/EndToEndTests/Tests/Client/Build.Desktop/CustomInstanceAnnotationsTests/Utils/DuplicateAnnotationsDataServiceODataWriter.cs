//---------------------------------------------------------------------
// <copyright file="DuplicateAnnotationsDataServiceODataWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using Microsoft.OData.Service;
    using Microsoft.OData;

    public class DuplicateAnnotationsDataServiceODataWriter : DataServiceODataWriter
    {
        public DuplicateAnnotationsDataServiceODataWriter(ODataWriter odataWriter)
            : base(odataWriter)
        {
        }

        public override void WriteStart(DataServiceODataWriterFeedArgs args)
        {
            base.WriteStart(args);
            foreach (var annotation in CustomInstanceAnnotationsGenerator.GetDuplicateAnnotations())
            {
                args.Feed.InstanceAnnotations.Add(annotation);
            }
        }

        public override void WriteStart(DataServiceODataWriterEntryArgs args)
        {
            base.WriteStart(args);
            foreach (var annotation in CustomInstanceAnnotationsGenerator.GetDuplicateAnnotations())
            {
                args.Entry.InstanceAnnotations.Add(annotation);
            }
        }
    }
}
