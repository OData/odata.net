//---------------------------------------------------------------------
// <copyright file="ODataPublicApiTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.PublicApi
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Suites.Data.Test;

    /// <summary>
    /// Class for check public api change
    /// </summary>
    [TestClass, TestCase]
    public class ODataPublicApiTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the public api change.")]
        public void PublicApiTest()
        {
            string testDirectory = TestContext.TestRunDirectory;
            Assert.IsTrue(Directory.Exists(testDirectory), "Test directory does not exists");
            string[] assemlyLists =
            {
                "Microsoft.Spatial.dll",
                "Microsoft.OData.Edm.dll",
                "Microsoft.OData.Core.dll",
                "Microsoft.OData.Client.dll"
            };

            string assemlyPath = Assembly.GetExecutingAssembly().Location;
            FileInfo assemly = new FileInfo(assemlyPath);
            string assemlyDirectory = assemly.Directory.FullName;
            string outputFile = testDirectory + Path.DirectorySeparatorChar + "PublicApi.out";

            using (FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Console.SetOut(sw);
                    foreach (var targetAssemly in assemlyLists)
                    {
                        string assemblyPath = assemlyDirectory + Path.DirectorySeparatorChar + targetAssemly;
                        Assert.IsTrue(File.Exists(assemblyPath), string.Format("{0} does not exist in current directory", assemblyPath));
                        string[] fakeAgrs = { assemblyPath };
                        new ObjectModelTest(fakeAgrs).RunDataTestWithoutVerifyMsiInstaller();
                    }
                }
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "PublicApi.bsl";
            string baselineString = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    baselineString = reader.ReadToEnd();
                }
            }
            string outputString = File.ReadAllText(outputFile);
            Assert.IsTrue(
              string.Compare(baselineString, outputString, StringComparison.Ordinal) == 0,
              "Base line file {0} and output file {1} do not match, please check." + Environment.NewLine +
              "To update the baseline, please run:" + Environment.NewLine + Environment.NewLine +
              "copy /y \"{1}\" \"{0}\"",
              @"test\FunctionalTests\Tests\DataOData\Tests\OData.Common.Tests\PublicApi\PublicApi.bsl",
              outputFile);
        }
    }
}
