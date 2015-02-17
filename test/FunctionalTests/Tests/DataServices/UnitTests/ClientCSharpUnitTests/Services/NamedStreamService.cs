//---------------------------------------------------------------------
// <copyright file="NamedStreamService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using AstoriaUnitTests.Stubs.DataServiceProvider;

    /// <summary>
    /// Test service with named streams.
    /// </summary>
    internal static class NamedStreamService
    {
        internal static DSPServiceDefinition SetUpNamedStreamService()
        {
            DSPMetadata metadata = new DSPMetadata("NamedStreamIDSPContainer", "NamedStreamTest");

            // entity with streams
            ResourceType entityWithNamedStreams = metadata.AddEntityType("EntityWithNamedStreams", null, null, false);
            metadata.AddKeyProperty(entityWithNamedStreams, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityWithNamedStreams, "Name", typeof(string));
            ResourceProperty streamInfo1 = new ResourceProperty("Stream1", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(Stream)));
            entityWithNamedStreams.AddProperty(streamInfo1);
            ResourceProperty streamInfo2 = new ResourceProperty("Stream2", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(Stream)));
            entityWithNamedStreams.AddProperty(streamInfo2);

            // entity1 with streams
            ResourceType entityWithNamedStreams1 = metadata.AddEntityType("EntityWithNamedStreams1", null, null, false);
            metadata.AddKeyProperty(entityWithNamedStreams1, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityWithNamedStreams1, "Name", typeof(string));
            ResourceProperty refStreamInfo1 = new ResourceProperty("RefStream1", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(Stream)));
            entityWithNamedStreams1.AddProperty(refStreamInfo1);

            // entity2 with streams
            ResourceType entityWithNamedStreams2 = metadata.AddEntityType("EntityWithNamedStreams2", null, null, false);
            metadata.AddKeyProperty(entityWithNamedStreams2, "ID", typeof(string));
            metadata.AddPrimitiveProperty(entityWithNamedStreams2, "Name", typeof(string));
            ResourceProperty collectionStreamInfo = new ResourceProperty("ColStream", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(Stream)));
            entityWithNamedStreams2.AddProperty(collectionStreamInfo);

            ResourceSet set1 = metadata.AddResourceSet("MySet1", entityWithNamedStreams);
            ResourceSet set2 = metadata.AddResourceSet("MySet2", entityWithNamedStreams1);
            ResourceSet set3 = metadata.AddResourceSet("MySet3", entityWithNamedStreams2);

            // add navigation property to entityWithNamedStreams
            metadata.AddResourceReferenceProperty(entityWithNamedStreams, "Ref", set2, entityWithNamedStreams1);
            metadata.AddResourceSetReferenceProperty(entityWithNamedStreams, "Collection", set3, entityWithNamedStreams2);
            metadata.AddResourceSetReferenceProperty(entityWithNamedStreams2, "Collection1", set2, entityWithNamedStreams1);

            DSPServiceDefinition service = new DSPServiceDefinition();
            service.Metadata = metadata;
            service.MediaResourceStorage = new DSPMediaResourceStorage();
            service.SupportMediaResource = true;
            service.SupportNamedStream = true;
            service.ForceVerboseErrors = true;
            service.PageSizeCustomizer = (config, type) =>
                                         {
                                             config.SetEntitySetPageSize("MySet3", 1);
                                         };

            // populate data
            DSPContext context = new DSPContext();

            DSPResource entity1 = new DSPResource(entityWithNamedStreams);
            {
                context.GetResourceSetEntities("MySet1").Add(entity1);

                entity1.SetValue("ID", 1);
                entity1.SetValue("Name", "Foo");
                DSPMediaResource namedStream1 = service.MediaResourceStorage.CreateMediaResource(entity1, streamInfo1);
                namedStream1.ContentType = "image/jpeg";
                byte[] data1 = new byte[] { 0, 1, 2, 3, 4 };
                namedStream1.GetWriteStream().Write(data1, 0, data1.Length);

                DSPMediaResource namedStream2 = service.MediaResourceStorage.CreateMediaResource(entity1, streamInfo2);
                namedStream2.ContentType = "image/jpeg";
                byte[] data2 = new byte[] { 0, 1, 2, 3, 4 };
                namedStream2.GetWriteStream().Write(data2, 0, data2.Length);
            }

            DSPResource entity2 = new DSPResource(entityWithNamedStreams1);
            {
                context.GetResourceSetEntities("MySet2").Add(entity2);

                entity2.SetValue("ID", 3);
                entity2.SetValue("Name", "Bar");
                DSPMediaResource refNamedStream1 = service.MediaResourceStorage.CreateMediaResource(entity2, refStreamInfo1);
                refNamedStream1.ContentType = "image/jpeg";
                byte[] data1 = new byte[] { 0, 1, 2, 3, 4 };
                refNamedStream1.GetWriteStream().Write(data1, 0, data1.Length);

                // set the navigation property
                entity1.SetValue("Ref", entity2);
            }

            {
                DSPResource entity3 = new DSPResource(entityWithNamedStreams2);
                context.GetResourceSetEntities("MySet3").Add(entity3);

                entity3.SetValue("ID", "ABCDE");
                entity3.SetValue("Name", "Bar");
                DSPMediaResource stream = service.MediaResourceStorage.CreateMediaResource(entity3, collectionStreamInfo);
                stream.ContentType = "image/jpeg";
                byte[] data1 = new byte[] { 0, 1, 2, 3, 4 };
                stream.GetWriteStream().Write(data1, 0, data1.Length);
                entity3.SetValue("Collection1", new List<DSPResource>() { entity2 });

                DSPResource entity4 = new DSPResource(entityWithNamedStreams2);
                context.GetResourceSetEntities("MySet3").Add(entity4);

                entity4.SetValue("ID", "XYZ");
                entity4.SetValue("Name", "Foo");
                DSPMediaResource stream1 = service.MediaResourceStorage.CreateMediaResource(entity3, collectionStreamInfo);
                stream1.ContentType = "image/jpeg";
                stream1.GetWriteStream().Write(data1, 0, data1.Length);
                entity4.SetValue("Collection1", new List<DSPResource>() { entity2 });

                entity1.SetValue("Collection", new List<DSPResource>() { entity3, entity4 });
            }

            service.DataSource = context;
            return service;
        }
    }

    /// <summary>
    /// Test client class for consuming the service.
    /// </summary>
    public class EntityWithNamedStreams2
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DataServiceStreamLink Stream1 { get; set; }
        public DataServiceStreamLink Stream2 { get; set; }
        public DataServiceStreamLink ColStream { get; set; }
        public List<EntityWithNamedStreams1> Collection1 { get; set; }
    }

    /// <summary>
    /// Test client class for consuming the service.
    /// </summary>
    public class EntityWithNamedStreams1
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DataServiceStreamLink Stream1 { get; set; }
        public DataServiceStreamLink Stream2 { get; set; }
        public EntityWithNamedStreams1 Ref { get; set; }
        public DataServiceStreamLink RefStream1 { get; set; }
        public List<EntityWithNamedStreams2> Collection { get; set; }
    }
}