//---------------------------------------------------------------------
// <copyright file="CustomResourceSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.AnnotationTests;

public class CustomResourceSerializer : ODataResourceSerializer
{
    private readonly string _odataNamespace = "Microsoft.OData.E2E.TestCommon.Common.Server.Default";

    public CustomResourceSerializer(IODataSerializerProvider serializerProvider) : base(serializerProvider)
    {
    }

    public override ODataResource CreateResource(SelectExpandNode selectExpandNode, ResourceContext resourceContext)
    {
        var resource = base.CreateResource(selectExpandNode, resourceContext);
        var pathValue = resourceContext.Request.Path.Value;

        if(pathValue != null)
        {
            // Add instance annotations based on the path value
            if (pathValue.EndsWith("odata/Boss"))
            {
                resource.InstanceAnnotations.Add(new ODataInstanceAnnotation(_odataNamespace + ".IsBoss", new ODataPrimitiveValue(true)));
            }

            // Add instance annotations for the HomeAddress property
            else if ((pathValue.Contains("odata/People") && pathValue.EndsWith("HomeAddress")) || 
                (resourceContext.StructuredType.TypeKind == EdmTypeKind.Complex && resourceContext.ResourceInstance is HomeAddress))
            {
                resource.InstanceAnnotations.Add(new ODataInstanceAnnotation(_odataNamespace + ".AddressType", new ODataPrimitiveValue("Home")));
            }
        }

        foreach (var property in resource.Properties)
        {
            switch(property.Name)
            {
                case "Emails":
                    property.InstanceAnnotations.Add(new ODataInstanceAnnotation(_odataNamespace + ".DisplayName", new ODataPrimitiveValue("EmailAddresses")));
                    break;
                case "City":
                    property.InstanceAnnotations.Add(new ODataInstanceAnnotation(_odataNamespace + ".CityInfo", new ODataPrimitiveValue("BestCity")));
                    break;
                case "Birthday":
                case "UpdatedTime":
                    {
                        // Convert the DateTimeOffset to UTC
                        if (((ODataProperty)property).Value is DateTimeOffset dto)
                        {
                            ((ODataProperty)property).Value = new DateTimeOffset(dto.DateTime, TimeSpan.Zero);
                        }
                    }
                    break;

            }
        }

        return resource;
    }
}
