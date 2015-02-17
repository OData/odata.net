//---------------------------------------------------------------------
// <copyright file="ModelBuilderHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.OData.Utils.Metadata;

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
            var edmNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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
            var edmNamespace = EdmLibCsdlContentGenerator.GetCsdlFullNamespace(edmVersion);
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
    }
}
