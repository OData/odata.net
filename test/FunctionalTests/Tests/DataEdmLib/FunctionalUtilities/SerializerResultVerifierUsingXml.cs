//---------------------------------------------------------------------
// <copyright file="SerializerResultVerifierUsingXml.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class SerializerResultVerifierUsingXml
    {
        public void Verify(IEnumerable<XElement> expectedXElements, IEnumerable<XElement> actualXElements)
        {
            Assert.AreEqual(expectedXElements.Count(), actualXElements.Count(), "Unexpected number of Csdl files!");

            var comparer = new CsdlXElementComparer();

            // extract EntityContainers into one place
            XElement expectedContainers = CsdlElementExtractor.ExtractElementByName(expectedXElements, "EntityContainer");
            XElement actualContainers = CsdlElementExtractor.ExtractElementByName(actualXElements, "EntityContainer");

            // compare just the EntityContainers
            Console.WriteLine("Expected: " + expectedContainers.ToString());
            Console.WriteLine("Actual: " + actualContainers.ToString());
            comparer.Compare(expectedContainers, actualContainers);

            // compare non-EntityContainers
            foreach (XElement expectedXElement in expectedXElements)
            {
                string schemaNamespace = expectedXElement.Attribute("Namespace").Value;
                XElement actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == e.Attribute("Namespace").Value);

                Assert.IsNotNull(actualXElement, "Cannot find schema for '{0}' in result Csdls!", schemaNamespace);

                Console.WriteLine("Expected: " + expectedXElement.ToString());
                Console.WriteLine("Actual: " + actualXElement.ToString());
                comparer.Compare(expectedXElement, actualXElement);
            }
        }
    }
}
