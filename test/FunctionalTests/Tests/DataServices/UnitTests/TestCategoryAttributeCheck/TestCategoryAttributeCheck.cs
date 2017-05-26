//---------------------------------------------------------------------
// <copyright file="TestCategoryAttributeCheck.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// These unit tests check for the Category attribute on the testmethods which have been split up into partitions.
    /// This to make sure no tests have been added which are not being run
    /// </summary>
    [TestClass]
    public class TestCategoryAttributeCheck
    {
        [TestMethod]
        public void VerifyODataLibReaderTestCategories()
        {
            var validCategoryNames = new[]
            {
                "Reader.Atom",
                "Reader.Batch",
                "Reader.Epm",
                "Reader.Json",
                "Reader.Links",
                "Reader.Collections",
                "Reader.ComplexValues",
                "Reader.DataServiceVersion",
                "Reader.MetadataValidation",
                "Reader.Entries",
                "Reader.Errors",
                "Reader.Feeds",
                "Reader.MessageReader",
                "Reader.MetadataDocument",
                "Reader.Operations",
                "Reader.PrimitiveValues",
                "Reader.Properties",
                "Reader.ContentType",
                "Reader.ServiceDocument",
                "Reader.Streams",
                "Reader.UriHandling",
            };

            VerifyTestCategoryAttributes("Microsoft.Test.Taupo.OData.Reader.Tests.dll", validCategoryNames);
        }

        [TestMethod]
        public void VerifyServerUnitTests1Categories()
        {
            VerifyTestCategoryAttributes("Microsoft.Data.ServerUnitTests1.UnitTests.dll", new[] { "Partition1", "Partition2", "Partition3" });
        }

        [TestMethod]
        public void VerifyServerUnitTests2Categories()
        {
            VerifyTestCategoryAttributes("Microsoft.Data.ServerUnitTests2.UnitTests.dll", new[] { "Partition1", "Partition2" });
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void VerifyWebClientUnitTestsCategories()
        {
            VerifyTestCategoryAttributes("AstoriaClientUnitTests.dll", new[] { "Partition1", "Partition2", "Partition3" });
        }

        private static void VerifyTestCategoryAttributes(string assemblyName, IEnumerable<string> validCategoryNames)
        {
            string assemblyLocation = Path.Combine(TestUtil.SuiteBinDirectory, assemblyName);
            Assembly currentAssembly = Assembly.LoadFrom(assemblyLocation);

            foreach (Type type in currentAssembly.GetTypes())
            {
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    if (methodInfo.IsDefined(typeof(TestMethodAttribute), true))
                    {
                        Assert.IsTrue(methodInfo.IsDefined(typeof(TestCategoryAttribute), true), "Test Method " + methodInfo.Name + " has no TestCategory attribute and therefore will never run as part of the Check-in Tests");

                        object[] customAttributes = methodInfo.GetCustomAttributes(typeof(TestCategoryAttribute), true);
                        Assert.AreEqual(1, customAttributes.Count(), "Unexpected number of TestCategory attributes found on method " + methodInfo.Name);
                        Assert.IsTrue(ValidValueForTestCategory((TestCategoryAttribute)customAttributes.Single(), validCategoryNames), "TestCategory attribute on method " + methodInfo.Name + " has invalid category name");
                    }
                }
            }
        }

        private static bool ValidValueForTestCategory(TestCategoryAttribute attribute, IEnumerable<string> validCategoryNames)
        {
            return attribute.TestCategories.Count(t => validCategoryNames.Contains(t)) == 1;
        }
    }
}
