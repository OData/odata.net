//---------------------------------------------------------------------
// <copyright file="EdmLibCsdlContentGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.OData.Utils.Metadata;

    /// <summary>
    /// Contains extension methods to provide additional features to CsdlContentGenerator
    /// </summary>
    public static class EdmLibCsdlContentGenerator
    {
        public static XNamespace GetCsdlFullNamespace(EdmVersion csdlVersion)
        {
            if (csdlVersion == EdmVersion.V40)
            {
                return EdmConstants.EdmOasisNamespace;
            }
            else
            {
                throw new ArgumentOutOfRangeException("CSDL Schema Version is not supported: " + csdlVersion.ToString());
            }
        }

        public static EdmVersion GetEdmVersion(XNamespace edmNamespace)
        {
            var edmVersions = new EdmVersion[] { EdmVersion.V40 };
            foreach (var edmVersion in edmVersions)
            {
                if (GetCsdlFullNamespace(edmVersion) == edmNamespace)
                {
                    return edmVersion;
                }
            }
            throw new ArgumentOutOfRangeException("The namespace is not a EDM namespace " + edmNamespace.NamespaceName);
        }

        public static IEnumerable<string> GetBaseTypes(XElement csdl, string entityFullTypeName)
        {
            var entityTypeName = entityFullTypeName.Split('.').Last();
            var entityTypeNamespace = entityFullTypeName.Substring(0, entityFullTypeName.Length - ('.' + entityTypeName).Length);

            var baseTypes = new List<string>();
            if (csdl.Attribute("Namespace").Value != entityTypeNamespace)
            {
                return baseTypes;
            }

            var types = csdl.Elements(XName.Get("EntityType", csdl.Name.NamespaceName)).Where(n => n.Attribute("Name").Value == entityTypeName);
            foreach (var type in types)
            {
                if (type.Attribute("BaseType") != null)
                {
                    baseTypes.AddRange(GetBaseTypes(csdl, type.Attribute("BaseType").Value));
                    baseTypes.Add(type.Attribute("BaseType").Value);
                }
            }
            return baseTypes;
        }

        public static IEnumerable<string> GetDirectlyDerivedTypes(XElement csdl, string structuralTypeElementName, string fullTypeName)
        {
            var entityTypeName = fullTypeName.Split('.').Last();
            var entityTypeNamespace = fullTypeName.Substring(0, fullTypeName.Length - ('.' + entityTypeName).Length);

            var derivedTypes = new List<string>();
            var types = csdl.Elements(XName.Get(structuralTypeElementName, csdl.Name.NamespaceName)).Where(n => null != n.Attribute("BaseType") && fullTypeName == n.Attribute("BaseType").Value);
            foreach (var type in types)
            {
                if (null != type.Attribute("Name"))
                {
                    var typeFullName = csdl.Attribute("Namespace").Value + "." + type.Attribute("Name").Value;
                    derivedTypes.Add(typeFullName);
                }
            }
            return derivedTypes;
        }

        public static IEnumerable<string> GetDirectlyDerivedTypes(IEnumerable<XElement> csdlElements, string structuralTypeElementName, string fullTypeName)
        {
            var derivedTypes = new List<string>();
            foreach (var csdlElement in csdlElements)
            {
                derivedTypes.AddRange(GetDirectlyDerivedTypes(csdlElement, structuralTypeElementName, fullTypeName));
            }
            return derivedTypes;
        }

        public static IEnumerable<string> GetDerivedTypes(IEnumerable<XElement> csdlElements, string structuralTypeElementName, string fullTypeName)
        {
            var results = new List<string>();
            var derivedTypes = GetDirectlyDerivedTypes(csdlElements, structuralTypeElementName, fullTypeName);
            results.AddRange(derivedTypes);
            foreach (var derivedType in derivedTypes)
            {
                results.AddRange(GetDerivedTypes(csdlElements, structuralTypeElementName, derivedType));
            }
            return results;
        }
    }
}
