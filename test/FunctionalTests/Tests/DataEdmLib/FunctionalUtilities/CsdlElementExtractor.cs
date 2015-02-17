//---------------------------------------------------------------------
// <copyright file="CsdlElementExtractor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Extract specific type of elements out from input schemas into a separate schema
    /// Caution: modifies input XElements!
    /// </summary>
    public class CsdlElementExtractor
    {
        public static XElement ExtractElementByName(IEnumerable<XElement> inputSchemas, string elementNameToExtract)
        {
            if (inputSchemas == null || !inputSchemas.Any())
            {
                throw new InvalidOperationException("Needs at least one schema to extract element!");
            }

            XNamespace csdlXNamespace = inputSchemas.First().Name.Namespace;
            var containers = new XElement(csdlXNamespace + "Schema",
                                      new XAttribute("Namespace", "ExtractedElements"));

            foreach (var s in inputSchemas)
            {
                foreach (var c in s.Elements(csdlXNamespace + elementNameToExtract).ToArray())
                {
                    c.Remove();
                    containers.Add(c);
                }
            }

            return containers;
        }
    }
}
