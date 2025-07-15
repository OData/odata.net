//---------------------------------------------------------------------
// <copyright file="CustomODataResourceSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Serialization;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

internal class CustomODataResourceSerializer : ODataResourceSerializer
{
    public CustomODataResourceSerializer(IODataSerializerProvider serializerProvider) : base(serializerProvider)
    {
    }

    public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
    {
        var resource = base.CreateResource(selectExpandNode, resourceContext);
        resource.InstanceAnnotations.Add(new ODataInstanceAnnotation("MyNamespace.CustomAnnotation1", new ODataPrimitiveValue("Some annotation")));

        return resource;
    }
}
