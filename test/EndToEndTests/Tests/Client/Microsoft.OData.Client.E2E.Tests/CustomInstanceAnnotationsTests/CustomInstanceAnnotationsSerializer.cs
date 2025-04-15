//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.E2E.Tests.CustomInstanceAnnotationsTests;

// Entity Serializer (for individual entries)
public class CustomODataResourceSerializer : ODataResourceSerializer
{
    public CustomODataResourceSerializer(IODataSerializerProvider serializerProvider) : base(serializerProvider)
    {
    }

    public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
    {
        var resource = base.CreateResource(selectExpandNode, resourceContext);
        for(var i = 0; i < 200; i++)
        {
            resource.InstanceAnnotations.Add(new ODataInstanceAnnotation($"AnnotationOnEntry.AddedBeforeWriteStart.Index.{i}", new ODataPrimitiveValue($"EntryCustomValue{i}")));
        }

        return resource;
    }
}

// Feed Serializer (for collections)
public class CustomODataResourceSetSerializer : ODataResourceSetSerializer
{
    public CustomODataResourceSetSerializer(IODataSerializerProvider serializerProvider) : base(serializerProvider)
    {
    }

    public override ODataResourceSet CreateResourceSet(IEnumerable resourceSetInstance, IEdmCollectionTypeReference resourceSetType, ODataSerializerContext writeContext)
    {
        var resourceSet = base.CreateResourceSet(resourceSetInstance, resourceSetType, writeContext);
        for (var i = 0; i < 100; i++)
        {
            resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation($"AnnotationOnFeed.AddedBeforeWriteStart.Index.{i}", new ODataPrimitiveValue($"FeedCustomValue{i}")));
        }

        for (var i = 0; i < 100; i++)
        {
            resourceSet.InstanceAnnotations.Add(new ODataInstanceAnnotation($"AnnotationOnFeed.AddedAfterWriteStart.Index.{i}", new ODataPrimitiveValue($"FeedCustomValue{i}")));
        }

        return resourceSet;
    }
}

public class CustomODataSerializerProvider : ODataSerializerProvider
{
    public CustomODataSerializerProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override IODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
        if(edmType.IsEntity() || edmType.IsComplex())
        {
            return new CustomODataResourceSerializer(this);
        }
        
        if(edmType.IsCollection() && edmType.AsCollection().ElementType().IsEntity())
        {
            return new CustomODataResourceSetSerializer(this);
        }

        return base.GetEdmTypeSerializer(edmType);
    }
}
