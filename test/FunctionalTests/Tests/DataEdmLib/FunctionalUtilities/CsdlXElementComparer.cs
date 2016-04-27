//---------------------------------------------------------------------
// <copyright file="CsdlXElementComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EdmConstants = Microsoft.Test.OData.Utils.Metadata.EdmConstants;

    /// <summary>
    /// Compares two Csdl (each as an XElement), considering equivalent ordering
    /// </summary>
    public class CsdlXElementComparer
    {
        private CsdlXElementSorter csdlSorter;

        public CsdlXElementComparer()
        {
            this.csdlSorter = new CsdlXElementSorter();
        }

        public void Compare(XElement expected, XElement actual)
        {
            this.UpdateSchemasForUseStrongSpatialTypes(expected);
            this.UpdateSchemasForUseStrongSpatialTypes(actual);

            this.UpdateSchemaAliasToSchemaNamepsace(expected);
            this.UpdateSchemaAliasToSchemaNamepsace(actual);

            this.UpdatePrimitiveTypeNameForEdmFullName(expected);
            this.UpdatePrimitiveTypeNameForEdmFullName(actual);

            this.CompensateDefaultFacets(expected);
            this.CompensateDefaultFacets(actual);

            this.CompensateSpatialTypeSrid(expected);
            this.CompensateSpatialTypeSrid(actual);

            XElement sortedExpected = this.csdlSorter.SortCsdl(expected);
            XElement sortedActual = this.csdlSorter.SortCsdl(actual);

            Console.WriteLine("Sorted Expected: " + expected.ToString());
            Console.WriteLine("Sorted Actual: " + sortedActual.ToString());

            Assert.AreEqual(sortedExpected.ToString(), sortedActual.ToString(), "Csdl not equal!");
        }

        private void UpdateSchemasForUseStrongSpatialTypes(XElement xElement)
        {
            // Entity Framework always sets the UseStrongSpatialTypes attribute to false while the EdmLib serializer does not use. To compensate this diffrence, we remove this attribute. 
            var strongSpatialTypeAttribute = xElement.Attribute(XName.Get("UseStrongSpatialTypes", EdmConstants.AnnotationNamespace));
            if (null != strongSpatialTypeAttribute)
            {
                strongSpatialTypeAttribute.Remove();
            }
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

        private void UpdateSchemaAliasToSchemaNamepsace(XElement csdlElement)
        {
            var alias = csdlElement.Attribute(XName.Get("Alias"));
            if (null != alias)
            {
                var aliasValue = alias.Value;
                var namespaceValue = csdlElement.Attribute(XName.Get("Namespace")).Value;

                foreach (var element in csdlElement.Elements())
                {
                    UpdateSchemaAliasToSchemaNamepsace(element, aliasValue, namespaceValue);
                }

                // Remove the alias attribute from the schema, since this specific attribute is not expected 
                // to be maintained when the model gets serialized
                csdlElement.Attribute(XName.Get("Alias")).Remove();
            }
        }

        private void UpdateSchemaAliasToSchemaNamepsace(XElement element, string aliasValue, string namespaceValue)
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
                UpdateSchemaAliasToSchemaNamepsace(subElement, aliasValue, namespaceValue);
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
            foreach (var primitiveType in ModelBuilder.AllPrimitiveEdmTypes(EdmVersion.Latest, false))
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
            var attributeDefaultValues = new Dictionary<string, Boolean>() { { "Nullable", true } };
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
}
