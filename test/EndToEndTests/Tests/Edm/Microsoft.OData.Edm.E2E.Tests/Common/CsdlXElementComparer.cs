//---------------------------------------------------------------------
// <copyright file="CsdlXElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml.Linq;

namespace Microsoft.OData.Edm.E2E.Tests.Common;

/// <summary>
/// Compares two Csdl (each as an XElement), considering equivalent ordering
/// </summary>
public class CsdlXElementComparer
{
    private CsdlXElementSorter csdlSorter;

    public CsdlXElementComparer()
    {
        csdlSorter = new CsdlXElementSorter();
    }

    public void Compare(XElement expected, XElement actual)
    {
        UpdateSchemasForUseStrongSpatialTypes(expected);
        UpdateSchemasForUseStrongSpatialTypes(actual);

        UpdateSchemaAliasToSchemaNamespace(expected);
        UpdateSchemaAliasToSchemaNamespace(actual);

        UpdatePrimitiveTypeNameForEdmFullName(expected);
        UpdatePrimitiveTypeNameForEdmFullName(actual);

        CompensateDefaultFacets(expected);
        CompensateDefaultFacets(actual);

        CompensateSpatialTypeSrid(expected);
        CompensateSpatialTypeSrid(actual);

        XElement sortedExpected = csdlSorter.SortCsdl(expected);
        XElement sortedActual = csdlSorter.SortCsdl(actual);

        Assert.Equal(sortedExpected.ToString(), sortedActual.ToString());
    }

    private void UpdateSchemasForUseStrongSpatialTypes(XElement xElement)
    {
        // Entity Framework always sets the UseStrongSpatialTypes attribute to false while the EdmLib serializer does not use. To compensate this difference, we remove this attribute. 
        var strongSpatialTypeAttribute = xElement.Attribute(XName.Get("UseStrongSpatialTypes", EdmStringConstants.AnnotationNamespace));
        strongSpatialTypeAttribute?.Remove();
    }

    private void CompensateSpatialTypeSrid(XElement csdlElement)
    {
        var typeAttributeNames = new string[] { "ReturnType", "Type" };
        var spatialTypeSrids = new Dictionary<string, string>()
            {
                {"Geography","4326"},
                {"GeographyPoint","4326"},
                {"GeographyLineString","4326"},
                {"GeographyPolygon","4326"},
                {"GeographyCollection","4326"},
                {"GeographyMultiPolygon","4326"},
                {"GeographyMultiLineString","4326"},
                {"GeographyMultiPoint","4326"},
                {"Geometry","0"},
                {"GeometryPoint","0"},
                {"GeometryLineString","0"},
                {"GeometryPolygon","0"},
                {"GeometryCollection","0"},
                {"GeometryMultiPolygon","0"},
                {"GeometryMultiLineString","0"},
                {"GeometryMultiPoint","0"},
            };

        var SRID = XName.Get("SRID");
        foreach (var element in csdlElement.Descendants())
        {
            var typeAttribute = element.Attributes().SingleOrDefault(n =>
                                                        typeAttributeNames.Any(a => a == n.Name.LocalName)
                                                        && !element.Attributes(SRID).Any()
                                                        && spatialTypeSrids.ContainsKey(n.Value.Replace("Edm.", string.Empty))
                                                    );
            if (typeAttribute != null)
            {
                element.SetAttributeValue(SRID, spatialTypeSrids[typeAttribute.Value.Replace("Edm.", string.Empty)]);
            }
        }
    }

    private void UpdateSchemaAliasToSchemaNamespace(XElement csdlElement)
    {
        var alias = csdlElement.Attribute(XName.Get("Alias"));
        if (null != alias)
        {
            var aliasValue = alias.Value;
            var namespaceValue = csdlElement.Attribute(XName.Get("Namespace")).Value;

            foreach (var element in csdlElement.Elements())
            {
                UpdateSchemaAliasToSchemaNamespace(element, aliasValue, namespaceValue);
            }

            // Remove the alias attribute from the schema, since this specific attribute is not expected 
            // to be maintained when the model gets serialized
            csdlElement.Attribute(XName.Get("Alias")).Remove();
        }
    }

    private void UpdateSchemaAliasToSchemaNamespace(XElement element, string aliasValue, string namespaceValue)
    {
        var attributesToUpdate = new List<string> { "Target", "Type", "Term", "BaseType" };

        foreach (var attribute in attributesToUpdate)
        {
            var elementAttribute = element.Attribute(XName.Get(attribute));

            if (null != elementAttribute && elementAttribute.Value.StartsWith(aliasValue + "."))
            {
                element.Attribute(XName.Get(attribute)).Value = namespaceValue + elementAttribute.Value.Substring(aliasValue.Length);
            }
        }

        foreach (var subElement in element.Elements())
        {
            UpdateSchemaAliasToSchemaNamespace(subElement, aliasValue, namespaceValue);
        }
    }

    private void UpdatePrimitiveTypeNameForEdmFullName(XElement csdlElement)
    {
        UpdatePrimitiveTypeNameByAttribute(csdlElement, "Type");
        UpdatePrimitiveTypeNameByAttribute(csdlElement, "ReturnType");
    }

    private void UpdatePrimitiveTypeNameByAttribute(XElement csdlElement, string attributeName)
    {
        var primitiveTypeNames = new Dictionary<string, string>();
        foreach (var primitiveType in ModelBuilderHelpers.AllPrimitiveEdmTypes(EdmVersion.V401, false))
        {
            primitiveTypeNames.Add((primitiveType.Definition as IEdmSchemaElement).Name, primitiveType.TestFullName());
        }

        var elementToUpdate = csdlElement.Descendants().Where(n => n.Attribute(attributeName) != null);
        foreach (var element in elementToUpdate)
        {
            var originalElementTypeName = GetElementType(element.Attribute(attributeName).Value);
            if (primitiveTypeNames.ContainsKey(originalElementTypeName))
            {
                element.SetAttributeValue(attributeName, element.Attribute(attributeName).Value.Replace(originalElementTypeName, primitiveTypeNames[originalElementTypeName]));
            }
        }
    }

    private void CompensateDefaultFacets(XElement elementToUpdate)
    {
        var elementNamesToUpdate = new string[] { "Property", "TypeRef", "ReturnType", "Parameter", "Function" };
        var attributeDefaultValues = new Dictionary<string, bool>() { { "Nullable", true } };
        var edmStringTypeName = EdmCoreModel.Instance.GetString(false).TestFullName();
        var edmBinaryTypeName = EdmCoreModel.Instance.GetBinary(false).TestFullName();

        foreach (var element in elementToUpdate.Descendants())
        {
            if (elementNamesToUpdate.Any(n => n == element.Name.LocalName) && element.Parent.Name.LocalName != "FunctionImport")
            {
                SetDefaultBooleanAttribute(element, "Nullable", attributeDefaultValues["Nullable"]);
            }
        }
    }

    private string GetElementType(string type)
    {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("(Collection)\\((?<ElementType>.*)\\)");
        var match = regex.Match(type);
        if (match.Success)
        {
            var elementType = match.Groups["ElementType"].Value;
            return GetElementType(elementType);
        }

        return type;
    }

    private void SetDefaultBooleanAttribute(XElement element, string attributeName, bool defaultValue)
    {
        if (element.Attribute(attributeName) == null)
        {
            element.SetAttributeValue(attributeName, defaultValue);
        }
    }
}
