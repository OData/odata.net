//---------------------------------------------------------------------
// <copyright file="DuplicateAnnotationsErrorTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests
{
    using System;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils;
    using Xunit;
    using Xunit.Abstractions;

    public class DuplicateAnnotationsErrorTest : EndToEndTestBase
    {
        public DuplicateAnnotationsErrorTest(ITestOutputHelper helper)
            : base(ODataWriterServiceUtil.CreateODataWriterServiceDescriptor<DuplicateAnnotationsDataServiceODataWriter>(), helper)
        {
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void WriteDuplicateAnnotationOnFeedError()
        {
            this.Invoke(
                this.AssertDuplicateAnnotationErrorIsThrown,
                CreateData("Customer"),
                CreateData(MimeTypes.ApplicationJsonODataLightNonStreaming, MimeTypes.ApplicationJsonODataLightStreaming),
                new DataDriven.Constraint[0]);
        }

        [Fact(Skip="VSUpgrade19 - DataDriven Test")]
        public void WriteDuplicateAnnotationOnEntryError()
        {
            this.Invoke(
                this.AssertDuplicateAnnotationErrorIsThrown,
                CreateData("Customer(-10)"),
                CreateData(MimeTypes.ApplicationJsonODataLightNonStreaming, MimeTypes.ApplicationJsonODataLightStreaming),
                new DataDriven.Constraint[0]);
        }

        internal void AssertDuplicateAnnotationErrorIsThrown(string uri, string contentType)
        {
            this.AssertInStreamErrorThrown(uri, contentType, "JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection", CustomInstanceAnnotationsGenerator.DuplicateAnnotationName);
        }

        private void AssertInStreamErrorThrown(string uri, string contentType, string resourceIdentifier, params object[] arguments)
        {
            var response = CustomInstanceAnnotationsReader.GetResponseString(new Uri(this.ServiceUri + uri), contentType);
            
            // In-stream errors cannot be parsed using ODL reader, so we have to check for a match in the response stream
            StringResourceUtil.VerifyODataLibString(response, resourceIdentifier, false /* isExactMatch */, arguments);
        }
   }
}