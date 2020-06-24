//---------------------------------------------------------------------
// <copyright file="ClientInstanceAnnotationsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils;
    using System.IO;
    using System.Text;
    using Xunit.Abstractions;
    using Xunit;

    public class ClientInstanceAnnotationsTests : EndToEndTestBase
    {
        private IEdmModel model;

        public ClientInstanceAnnotationsTests(ITestOutputHelper helper)
            : base(ODataWriterServiceUtil.CreateODataWriterServiceDescriptor<CustomInstanceAnnotationsWriter>(), helper)
        {

        }

        public override void CustomTestInitialize()
        {
            this.model = CustomInstanceAnnotationsReader.GetServiceModel(new Uri(this.ServiceUri + "$metadata"));
            CustomInstanceAnnotationsGenerator.NextNameIndex = 0;
        }

        [Fact]
        public void DetectPayloadKind()
        {
            var responseMessage = new Microsoft.Test.OData.Tests.Client.Common.InMemoryMessage();
            responseMessage.Stream = new MemoryStream(Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xml:base=""http://localhost:12367/Test/Data.ashx/UnitedNations/Demographic/v1/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">DataSeries</title>
  <id>http://localhost:12367/Test/Data.ashx/UnitedNations/Demographic/v1/DataSeries</id>
  <updated>2013-06-11T02:31:42Z</updated>
  <link rel=""self"" title=""DataSeries"" href=""DataSeries"" />
  <entry>
    <id>http://localhost:12367/Test/Data.ashx/UnitedNations/Demographic/v1/DataSeries('1')</id>
    <title type=""text""></title>
    <updated>2013-06-11T02:31:42Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""DataSeries"" href=""DataSeries('1')"" />
    <category term=""UnitedNations.Demographic.DataSeries"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:Id>1</d:Id>
        <d:DataSetId>POP</d:DataSetId>
        <d:Name>Population by urban/rural residence</d:Name>
      </m:properties>
    </content>
  </entry>
</feed>
"));
            responseMessage.SetHeader("Content-Type", "application/atom+xml");

            using (var messageReader = new ODataMessageReader(responseMessage as IODataResponseMessage, new ODataMessageReaderSettings()))
            {
                var reader = messageReader.DetectPayloadKind();
            }

        }

        [Fact]
        public void RequestWithSinglePreferHeaderAll()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            var entityMaterialised = false;
            var feedMaterialised = false;
            context.Format.UseJson();
            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=\"*\"");
            };

            context.Configurations.ResponsePipeline
                .OnEntityMaterialized((args) =>
                {
                    entityMaterialised = true;
                    Assert.True(199 <= args.Entry.InstanceAnnotations.Count, "Unexpected count of entry annotations");
                    Assert.True(200 >= args.Entry.InstanceAnnotations.Count, "Unexpected count of entry annotations");
                })
                .OnFeedEnded((args) =>
                {
                    feedMaterialised = true;
                    Assert.Equal(199, args.Feed.InstanceAnnotations.Count);
                });

            context.CreateQuery<Person>("Person").Execute().ToList();
            if (!entityMaterialised || !feedMaterialised)
            {
                Assert.True(false, "Entities({0}) and Feed({1}) should have been materialized.");
            }
        }

        [Fact]
        public void RequestSingleAnnotation()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            var entityMaterialised = false;
            var feedMaterialised = false;
            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=AnnotationOnFeed.AddedBeforeWriteStart.index.0");
            };
            context.Configurations.ResponsePipeline
                .OnEntityMaterialized((args) =>
                {
                    entityMaterialised = true;
                    Assert.True(args.Entry.InstanceAnnotations.Count() <= 1, "should find the derived type name in @odata.type or none.");
                })
                .OnFeedEnded((args) =>
                {
                    feedMaterialised = true;
                    Assert.True(args.Feed.InstanceAnnotations.Count == 1, "Count was " + args.Feed.InstanceAnnotations.Count);
                    Assert.True(args.Feed.InstanceAnnotations.Single().Name == "AnnotationOnFeed.AddedBeforeWriteStart.index.0");
                });

            context.CreateQuery<Person>("Person").Execute().ToList();
            if (!entityMaterialised || !feedMaterialised)
            {
                Assert.True(false, "Entities({0}) and Feed({1}) should have been materialized.");
            }
        }

        [Fact]
        public void RequestWithMultipleCompatiblePreferHeaderParts()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            var entityMaterialised = false;
            var feedMaterialised = false;
            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).PreferHeader().AnnotationFilter = "AnnotationOnFeed.AddedBeforeWriteStart.index.0";
                ((HttpWebRequestMessage)eventArgs.RequestMessage).PreferHeader().ReturnContent = true;
            };
            context.Configurations.ResponsePipeline
                .OnEntityMaterialized((args) =>
                {
                    entityMaterialised = true;
                    Assert.True(args.Entry.InstanceAnnotations.Count() <= 1, "should find the derived type name in @odata.type or none.");
                })
                .OnFeedEnded((args) =>
                {
                    feedMaterialised = true;
                    Assert.True(args.Feed.InstanceAnnotations.Count() == 1);
                    Assert.True(args.Feed.InstanceAnnotations.Single().Name == "AnnotationOnFeed.AddedBeforeWriteStart.index.0");
                });

            context.CreateQuery<Person>("Person").Execute().ToList();
            if (!entityMaterialised || !feedMaterialised)
            {
                Assert.True(false, "Entities({0}) and Feed({1}) should have been materialized.");
            }
        }

        [Fact]
        public void RequestWithMultipleIncompatiblePreferHeaders()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();

            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).PreferHeader().AnnotationFilter = "AnnotationOnFeed.AddedBeforeWriteStart.index.0";
                ((HttpWebRequestMessage)eventArgs.RequestMessage).PreferHeader().ReturnContent = false;
            };
            context.Configurations.ResponsePipeline.OnEntityMaterialized((args) =>
            {
                Assert.True(false, "Should not return the entity when return content false");
            });

            Person newPerson = new Person() { Name = "Bob", PersonId = 5444 };
            context.AddObject("Person", newPerson);
            context.SaveChanges();
        }

        [Fact]
        public void RequestWithMultipleIncludeAnnotationsHeaders()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            var entityMaterialised = false;
            var feedMaterialised = false;
            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=-AnnotationOnFeed.AddedBeforeWriteStart.*; odata.include-annotations=*");

            };
            context.Configurations.ResponsePipeline
                .OnEntityMaterialized((args) =>
                {
                    entityMaterialised = true;
                    Assert.True(args.Entry.InstanceAnnotations.Count() <= 1, "should find the derived type name in @odata.type or none.");
                })
                .OnFeedEnded((args) =>
                {
                    feedMaterialised = true;
                    Assert.True(args.Feed.InstanceAnnotations.Count() == 0, "negative filter should result in no annotations");
                });

            context.CreateQuery<Person>("Person").Execute().ToList();
            if (!entityMaterialised || !feedMaterialised)
            {
                Assert.True(false, "Entities({0}) and Feed({1}) should have been materialized.");
            }
        }

        [Fact]
        public void RequestWithMultipleIncludeAnnotationsHeadersInvertHeaders()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            var entityMaterialised = false;
            var feedMaterialised = false;
            context.SendingRequest2 += delegate (Object sender, SendingRequest2EventArgs eventArgs)
            {
                ((HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=\"*,-AnnotationOnFeed.AddedBeforeWriteStart.*\"");

            };
            context.Configurations.ResponsePipeline.OnEntityMaterialized((args) =>
            {
                entityMaterialised = true;
                Assert.True(199 <= args.Entry.InstanceAnnotations.Count, "Unexpected count of entry annotations");
                Assert.True(200 >= args.Entry.InstanceAnnotations.Count, "Unexpected count of entry annotations");
            });
            context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                feedMaterialised = true;
                Assert.Equal(100, args.Feed.InstanceAnnotations.Count);
            });

            context.CreateQuery<Person>("Person").Execute().ToList();
            if (!entityMaterialised || !feedMaterialised)
            {
                Assert.True(false, "Entities({0}) and Feed({1}) should have been materialized.");
            }
        }
    }
}
