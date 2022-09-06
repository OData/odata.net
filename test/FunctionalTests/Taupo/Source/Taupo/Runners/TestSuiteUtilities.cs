//---------------------------------------------------------------------
// <copyright file="TestSuiteUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Utilities for working with test suites
    /// </summary>
    public static class TestSuiteUtilities
    {
        /// <summary>
        /// Loads the test suite from the specified XML reader.
        /// Determines the assemblies in the suite file by looing in the root or in at the platform level
        /// You cannot have assembly elements defined in the platorm and root level or an error will be thrown
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The loaded test suite</returns>
        public static TestSuite LoadFrom(XmlReader reader)
        {
            string platform = "desktop";

            ExceptionUtilities.CheckArgumentNotNull(reader, "reader");

            reader.MoveToContent();
            if (reader.LocalName != "suite")
            {
                throw new TaupoInvalidOperationException("Invalid suite file. Must start with a top-level <suite /> element.");
            }

            var suite = new TestSuite();
            XDocument document = XDocument.Load(reader);

            string[] valid = new string[] { "desktop", "Silverlight", "assembly", "include", "exclude", "parameter" };

            var unknown = document.Element(XName.Get("suite")).Descendants().Where(d => !valid.Contains(d.Name.LocalName)).Select(d => d);
            foreach (var u in unknown)
            {
                throw new TaupoInvalidOperationException("Unrecognized element <" + u.Name.ToString() + " />");
            }

            // add the test case names
            var includes = document.Descendants(XName.Get("include")).Select(i => GetAttributeValue(i, "name"));
            foreach (var i in includes)
            {
                suite.Items.Add(new TestSuiteItem(i, true));
            }

            var excludes = document.Descendants(XName.Get("exclude")).Select(e => GetAttributeValue(e, "name"));
            foreach (var i in excludes)
            {
                suite.Items.Add(new TestSuiteItem(i, false));
            }

            // add the assemblies names
            var rootAssemblies = document.Elements("suite").Elements("assembly").Select(a => GetAttributeValue(a, "name"));
            var platformLevelAssemblies = document.Elements("suite").Elements(platform).Descendants(XName.Get("assembly")).Select(a => GetAttributeValue(a, "name"));
            
            if (platformLevelAssemblies.Any() && rootAssemblies.Any())
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid suite file. Cannot have <assembly> elements both at the root and inside <{0}>.Choose only one location.", platform));
            }
            
            // Assemblies are only retrieved from the root level or platform level not both
            List<string> assemblies = null;
            if (rootAssemblies.Any())
            {
                assemblies = new List<string>(rootAssemblies);
            }
            else
            {
                assemblies = new List<string>(platformLevelAssemblies);
            }

            foreach (var a in assemblies)
            {
                suite.Assemblies.Add(a);
            }

            // add the parameters
            var parameters = document.Descendants(XName.Get("parameter"));
            foreach (var p in parameters)
            {
                suite.Parameters.Add(GetAttributeValue(p, "name"), GetAttributeValue(p, "value"));
            }

            return suite;
        }

        /// <summary>
        /// Saves the test suite to the specified XML writer.
        /// </summary>
        /// <param name="suite">the suite to save</param>
        /// <param name="writer">The XML writer.</param>
        public static void SaveTo(TestSuite suite, XmlWriter writer)
        {
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");

            writer.WriteStartElement("suite");
            foreach (string asm in suite.Assemblies)
            {
                writer.WriteStartElement("assembly");
                writer.WriteAttributeString("name", asm);
                writer.WriteEndElement();
            }

            foreach (var kvp in suite.Parameters.OrderBy(c => c.Key))
            {
                writer.WriteStartElement("parameter");
                writer.WriteAttributeString("name", kvp.Key);
                writer.WriteAttributeString("value", kvp.Value);
                writer.WriteEndElement();
            }

            foreach (var item in suite.Items)
            {
                writer.WriteStartElement(item.IsIncluded ? "include" : "exclude");
                writer.WriteAttributeString("name", item.Name);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static string GetAttributeValue(XElement element, string name)
        {
            var attribute = element.Attribute(XName.Get(name));

            if (attribute == null)
            {
                throw new TaupoInvalidOperationException("Missing required '" + name + "' attribute on <" + element.Name + " />");
            }

            return attribute.Value;
        }
    }
}
