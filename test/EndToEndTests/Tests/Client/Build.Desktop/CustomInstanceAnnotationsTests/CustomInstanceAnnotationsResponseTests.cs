//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsResponseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.DataDriven;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils;
    using Xunit;
    using Xunit.Abstractions;

    public class CustomInstanceAnnotationsResponseTests : EndToEndTestBase
    {
        private IEdmModel model;

        public CustomInstanceAnnotationsResponseTests(ITestOutputHelper helper)
            : base(ODataWriterServiceUtil.CreateODataWriterServiceDescriptor<CustomInstanceAnnotationsWriter>(), helper)
        {
        }

        public override void CustomTestInitialize()
        {
            this.model = CustomInstanceAnnotationsReader.GetServiceModel(new Uri(this.ServiceUri + "$metadata"));
        }

        private string[] testMimeTypes = 
        { 
            MimeTypes.ApplicationJsonODataLightNonStreaming, 
            MimeTypes.ApplicationJsonODataLightStreaming,
        };

        private string[] feedQueries = 
        { 
            "Customer", 
            "Customer?$select=Name", 
            "Customer?$filter=true eq false", 
            "Customer?$expand=Orders", 
            "Login?$expand=SentMessages,ReceivedMessages", 
            "Login?$select=Username&$expand=SentMessages,ReceivedMessages", 
            "Login?$expand=SentMessages,ReceivedMessages&$select=Username" 
        };

        string[] entryQueries = 
        {
            "Customer(-10)", 
            "Customer(-10)?$select=Name", 
            "Customer(-10)?$expand=Orders", 
            "Login('1')?$expand=SentMessages,ReceivedMessages"
        };

        Func<string, bool>[] instanceAnnotationFilters =
        {
            null,
            ODataUtils.CreateAnnotationFilter("*"),
            name => true,
            name => false
        };

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void ReadFeed()
        {
            this.Invoke(this.ReadFeedTest, CreateData(this.feedQueries), CreateData(this.testMimeTypes), CreateData(false), CreateData(instanceAnnotationFilters), new Constraint[0]);
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void ReadEntry()
        {
            this.Invoke(this.ReadEntryTest, CreateData(this.entryQueries), CreateData(this.testMimeTypes), CreateData(false), CreateData(instanceAnnotationFilters), new Constraint[0]);
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void ReadFeedVerifyStateOfReader()
        {
            this.Invoke(this.ReadFeedTest, CreateData(this.feedQueries), CreateData(this.testMimeTypes), CreateData(true), CreateData(instanceAnnotationFilters), new Constraint[0]);
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void ReadEntryVerifyStateOfReader()
        {
            this.Invoke(this.ReadEntryTest, CreateData(this.entryQueries), CreateData(this.testMimeTypes), CreateData(true), CreateData(instanceAnnotationFilters), new Constraint[0]);
        }

        internal void ReadFeedTest(string uri, string contentType, bool verifyAnnotationsOnStart, Func<string, bool> shouldIncludeAnnotation)
        {
            var odataMessageReaderSettings = CreateODataMessageReaderSettings(shouldIncludeAnnotation);
            var actualAnnotatedItems = CustomInstanceAnnotationsReader.ReadFeed(new Uri(this.ServiceUri + uri), contentType, odataMessageReaderSettings, this.model);
            actualAnnotatedItems.VerifyAnnotatedItems(contentType, HasExpandedNavigationProperties(uri), verifyAnnotationsOnStart, shouldIncludeAnnotation);
        }

        internal void ReadEntryTest(string uri, string contentType, bool verifyAnnotationsOnStart, Func<string, bool> shouldIncludeAnnotation)
        {
            var odataMessageReaderSettings = CreateODataMessageReaderSettings(shouldIncludeAnnotation);
            var actualAnnotatedItems = CustomInstanceAnnotationsReader.ReadEntry(new Uri(this.ServiceUri + uri), contentType, odataMessageReaderSettings, this.model);
            actualAnnotatedItems.VerifyAnnotatedItems(contentType, HasExpandedNavigationProperties(uri), verifyAnnotationsOnStart, shouldIncludeAnnotation);
        }

        public static bool HasExpandedNavigationProperties(string uri)
        {
            if (uri.Contains("$expand"))
            {
                return true;
            }

            if (uri.Contains("Customer"))
            {
                var parts = uri.Split('?', '&');
                var selects = parts.Where(s => s.Contains("$select"));
                if(selects.Count() == 0 || selects.Any(s=>s.Contains("PrimaryContactInfo") || s.Contains("BackupContactInfo")))
                {
                    return true;
                }
            }
            return false;
        }

        private static ODataMessageReaderSettings CreateODataMessageReaderSettings(Func<string,bool> shouldIncludeAnnotation)
        {
            return new ODataMessageReaderSettings { MessageQuotas = new ODataMessageQuotas { MaxReceivedMessageSize = long.MaxValue }, ShouldIncludeAnnotation = shouldIncludeAnnotation };
        }
    }
}