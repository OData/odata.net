//---------------------------------------------------------------------
// <copyright file="ConstructiveApiCsdlXElementComparer.cs" company="Microsoft">
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

    /// <summary>
    /// Compares two Csdl (each as an XElement) for Constructive API models, considering 
    /// 1. Equivalent ordering
    /// 2. Association type inferrence
    /// </summary>
    public class ConstructiveApiCsdlXElementComparer
    {

        private CsdlXElementComparer csdlXElementComparer;

        public ConstructiveApiCsdlXElementComparer()
        {
            this.csdlXElementComparer = new CsdlXElementComparer();
        }

        public void Compare(List<XElement> expectXElements, List<XElement> actualXElements)
        {
            Assert.AreEqual(expectXElements.Count(), actualXElements.Count(), "Unexpected number of Csdl files!");

            // extract EntityContainers into one place
            XElement expectedContainers = CsdlElementExtractor.ExtractElementByName(expectXElements, "EntityContainer");
            XElement actualContainers = CsdlElementExtractor.ExtractElementByName(actualXElements, "EntityContainer");

            // compare just the EntityContainers
            Console.WriteLine("Expected: " + expectedContainers.ToString());
            Console.WriteLine("Actual: " + actualContainers.ToString());
            csdlXElementComparer.Compare(expectedContainers, actualContainers);

            foreach (var expectXElement in expectXElements)
            {
                string schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
                XElement actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

                Assert.IsNotNull(actualXElement, "Cannot find schema for '{0}' in result Csdls!", schemaNamespace);

                Console.WriteLine("Expected: " + expectXElement.ToString());
                Console.WriteLine("Actual: " + actualXElement.ToString());

                csdlXElementComparer.Compare(expectXElement, actualXElement);
                // TODO: Extend the TaupoModelComparer for the Constructible APIs instead of using CsdlXElementComparer. 
            }
        }
    }
}
