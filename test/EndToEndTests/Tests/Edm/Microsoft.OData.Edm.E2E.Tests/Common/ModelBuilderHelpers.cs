//---------------------------------------------------------------------
// <copyright file="ModelBuilderHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;

namespace Microsoft.OData.Edm.E2E.Tests.Common;

public static class ModelBuilderHelpers
{
    public static void SetNullableAttributes(IEnumerable<XElement> csdlElements, bool isNullable)
    {
        foreach (var csdlElement in csdlElements)
        {
            foreach (var element in csdlElement.Descendants())
            {
                if (null != element.Attribute("Nullable"))
                {
                    element.Attribute("Nullable").Value = isNullable ? "true" : "false";
                }
            }
        }
    }

    public static IEnumerable<XElement> ReplaceCsdlNamespacesForEdmVersion(XElement[] csdls, EdmVersion edmVersion)
    {
        var edmNamespace = GetCsdlFullNamespace(edmVersion);
        for (int i = 0; i < csdls.Count(); ++i)
        {
            if (edmNamespace != csdls[i].Name.Namespace)
            {
                csdls[i] = XElement.Parse(csdls[i].ToString().Replace(csdls[i].Name.Namespace.NamespaceName, edmNamespace.NamespaceName));
            }
        }
        return csdls;
    }

    public static string ReplaceCsdlNamespaceForEdmVersion(string csdl, EdmVersion edmVersion)
    {
        var edmNamespace = GetCsdlFullNamespace(edmVersion);
        var xmlReader = XmlReader.Create(new StringReader(csdl));
        while (xmlReader.Read())
        {
            if (xmlReader.Name == "Schema")
            {
                break;
            }
        }
        if (xmlReader.EOF)
        {
            throw new ArgumentException("{0} is not a well formed CSDL.");
        }
        return csdl.Replace(xmlReader.NamespaceURI, edmNamespace.NamespaceName);
    }

    public static IEnumerable<IEdmPrimitiveTypeReference> AllPrimitiveEdmTypes(EdmVersion edmVersion, bool isNullable)
    {
        IEnumerable<IEdmPrimitiveTypeReference> primitiveTypes = AllNonSpatialPrimitiveEdmTypes(isNullable);
        if (edmVersion >= EdmVersion.V40)
        {
            primitiveTypes = primitiveTypes.Concat(AllSpatialEdmTypes(isNullable));
        }

        return primitiveTypes;
    }

    public static IEdmPrimitiveTypeReference[] AllNonSpatialPrimitiveEdmTypes(bool nullable = false)
    {
        return new[]
        {
            EdmCoreModel.Instance.GetBinary(nullable),
            EdmCoreModel.Instance.GetBoolean(nullable),
            EdmCoreModel.Instance.GetByte(nullable),
            new EdmTemporalTypeReference(EdmCoreModel.Instance.GetDateTimeOffset(nullable).PrimitiveDefinition(), nullable, null),
            new EdmDecimalTypeReference(EdmCoreModel.Instance.GetDecimal(nullable).PrimitiveDefinition(), nullable),
            EdmCoreModel.Instance.GetDouble(nullable),
            EdmCoreModel.Instance.GetGuid(nullable),
            EdmCoreModel.Instance.GetInt16(nullable),
            EdmCoreModel.Instance.GetInt32(nullable),
            EdmCoreModel.Instance.GetInt64(nullable),
            EdmCoreModel.Instance.GetSByte(nullable),
            EdmCoreModel.Instance.GetSingle(nullable),
            EdmCoreModel.Instance.GetStream(nullable),
            EdmCoreModel.Instance.GetString(nullable),
            EdmCoreModel.Instance.GetDuration(nullable),
            EdmCoreModel.Instance.GetDate(nullable),
            EdmCoreModel.Instance.GetTimeOfDay(nullable),
        };
    }

    public static IEdmSpatialTypeReference[] AllSpatialEdmTypes(bool nullable = false)
    {
        return new[]
        {
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, nullable),
            EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, nullable),
        };
    }

    public static XNamespace GetCsdlFullNamespace(EdmVersion csdlVersion)
    {
        if (csdlVersion == EdmVersion.V40 || csdlVersion == EdmVersion.V401)
        {
            return EdmStringConstants.EdmOasisNamespace;
        }

        Assert.Fail("CSDL Schema Version is not supported: " + csdlVersion.ToString());
        return null;
    }
}
