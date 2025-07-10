//---------------------------------------------------------------------
// <copyright file="ClientInstanceAnnotationsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.CustomInstanceAnnotations;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Xunit;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Person;

namespace Microsoft.OData.Client.E2E.Tests.CustomInstanceAnnotationsTests;

public class ClientInstanceAnnotationsTests : EndToEndTestBase<ClientInstanceAnnotationsTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(CustomInstanceAnnotationTestsController), typeof(MetadataController));
            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents(
                    "odata",
                    DefaultEdmModel.GetEdmModel(),
                    configureServices: s => s.Replace(ServiceDescriptor.Singleton<IODataSerializerProvider, CustomODataSerializerProvider>())));
        }
    }

    public ClientInstanceAnnotationsTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    [Fact]
    public void TestRequestWithAllAnnotationsIncluded()
    {
        // Arrange
        var entityMaterialized = false;
        var feedMaterialized = false;

        _context.Format.UseJson();

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=\"*\"");
        };

        _context.Configurations.ResponsePipeline
            .OnEntityMaterialized((args) =>
            {
                entityMaterialized = true;
                Assert.Equal(200, args.Entry.InstanceAnnotations.Count);
            })
            .OnFeedEnded((args) =>
            {
                var count = args.Feed.InstanceAnnotations.Count;
                if(args.Feed.InstanceAnnotations.Count > 0)
                {
                    feedMaterialized = true;
                    Assert.Equal(200, args.Feed.InstanceAnnotations.Count);
                }
            });

        // Act
        _context.CreateQuery<Person>("People").Execute().ToList();

        // Assert
        Assert.True(entityMaterialized, "Entity should have been materialized.");
        Assert.True(feedMaterialized, "Feed should have been materialized.");
    }

    [Fact]
    public void TestRequestWithSingleSpecificAnnotation()
    {
        // Arrange
        _context.Format.UseJson();

        var entityMaterialized = false;
        var feedMaterialized = false;

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=AnnotationOnFeed.AddedBeforeWriteStart.Index.0");
        };

        _context.Configurations.ResponsePipeline
            .OnEntityMaterialized((args) =>
            {
                entityMaterialized = true;
                Assert.True(args.Entry.InstanceAnnotations.Count <= 1, "should find the derived type name in @odata.type or none.");
            })
            .OnFeedEnded((args) =>
            {
                if(args.Feed.InstanceAnnotations.Count > 0)
                {
                    feedMaterialized = true;
                    Assert.True(args.Feed.InstanceAnnotations.Count == 1, "Count was " + args.Feed.InstanceAnnotations.Count);
                    Assert.True(args.Feed.InstanceAnnotations.Single().Name == "AnnotationOnFeed.AddedBeforeWriteStart.Index.0", "Name was " + args.Feed.InstanceAnnotations.Single().Name);
                }
            });

        // Act
        _context.CreateQuery<Person>("People").Execute().ToList();

        // Assert
        Assert.True(entityMaterialized, "Entity should have been materialized.");
        Assert.True(feedMaterialized, "Feed should have been materialized.");
    }

    [Fact]
    public void TestRequestWithMultipleCompatiblePreferHeaders()
    {
        // Arrange
        _context.Format.UseJson();
        var entityMaterialized = false;
        var feedMaterialized = false;

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).PreferHeader().AnnotationFilter = "AnnotationOnFeed.AddedBeforeWriteStart.Index.0";
            ((HttpClientRequestMessage)eventArgs.RequestMessage).PreferHeader().ReturnContent = true;
        };

        _context.Configurations.ResponsePipeline
            .OnEntityMaterialized((args) =>
            {
                entityMaterialized = true;
                Assert.True(args.Entry.InstanceAnnotations.Count <= 1, "should find the derived type name in @odata.type or none.");
            })
            .OnFeedEnded((args) =>
            {
                if (args.Feed.InstanceAnnotations.Count > 0)
                {
                    feedMaterialized = true;
                    Assert.Single(args.Feed.InstanceAnnotations);
                    Assert.Equal("AnnotationOnFeed.AddedBeforeWriteStart.Index.0", args.Feed.InstanceAnnotations.Single().Name);
                }
            });

        // Act
        _context.CreateQuery<Person>("People").Execute().ToList();

        // Assert
        Assert.True(entityMaterialized, "Entity should have been materialized.");
        Assert.True(feedMaterialized, "Feed should have been materialized.");
    }

    [Fact]
    public void TestRequestWithIncompatiblePreferHeaders()
    {
        // Arrange
        var entityMaterialized = false;
        _context.Format.UseJson();

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).PreferHeader().AnnotationFilter = "AnnotationOnFeed.AddedBeforeWriteStart.Index.0";
            ((HttpClientRequestMessage)eventArgs.RequestMessage).PreferHeader().ReturnContent = false;
        };

        _context.Configurations.ResponsePipeline.OnEntityMaterialized((args) =>
        {
            entityMaterialized = true;
            Assert.Fail("Should not return the entity when return content false");
        });

        // Act
        var newPerson = new Person() { FirstName = "Bob", LastName = "Wanjohi", PersonID = 5444 };
        _context.AddObject("People", newPerson);
        _context.SaveChanges();

        // Assert
        Assert.False(entityMaterialized, "Entity should not have been materialized.");
    }

    [Fact]
    public void TestRequestWithMultipleIncludeAnnotationsHeaders()
    {
        // Arrange
        _context.Format.UseJson();

        var entityMaterialized = false;
        var feedMaterialized = false;

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=-AnnotationOnFeed.AddedBeforeWriteStart.*; odata.include-annotations=*");
        };

        _context.Configurations.ResponsePipeline
            .OnEntityMaterialized((args) =>
            {
                entityMaterialized = true;
                Assert.True(args.Entry.InstanceAnnotations.Count <= 1, "should find the derived type name in @odata.type or none.");
            })
            .OnFeedEnded((args) =>
            {
                feedMaterialized = true;
                Assert.True(args.Feed.InstanceAnnotations.Count == 0, "negative filter should result in no annotations");
            });

        // Act
        _context.CreateQuery<Person>("People").Execute().ToList();

        // Assert
        Assert.True(entityMaterialized, "Entity should have been materialized.");
        Assert.True(feedMaterialized, "Feed should have been materialized.");
    }

    [Fact]
    public void TestRequestWithInvertedIncludeAnnotationsHeaders()
    {
        // Arrange
        _context.Format.UseJson();
        var entityMaterialized = false;
        var feedMaterialized = false;

        _context.SendingRequest2 += delegate (Object? sender, SendingRequest2EventArgs eventArgs)
        {
            ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=\"*,-AnnotationOnFeed.AddedBeforeWriteStart.*\"");
        };

        _context.Configurations.ResponsePipeline
            .OnEntityMaterialized((args) =>
            {
                entityMaterialized = true;
                Assert.Equal(200, args.Entry.InstanceAnnotations.Count);
            })
            .OnFeedEnded((args) =>
            {
                if (args.Feed.InstanceAnnotations.Count > 0)
                {
                    feedMaterialized = true;
                    Assert.Equal(100, args.Feed.InstanceAnnotations.Count);
                }
            });

        // Act
        _context.CreateQuery<Person>("People").Execute().ToList();

        // Assert
        Assert.True(entityMaterialized, "Entity should have been materialized.");
        Assert.True(feedMaterialized, "Feed should have been materialized.");
    }

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "custominstanceannotationstests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
