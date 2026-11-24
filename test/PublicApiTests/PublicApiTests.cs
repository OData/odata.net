//---------------------------------------------------------------------
// <copyright file="PublicApiTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;

namespace Microsoft.OData.PublicApi.Tests
{
    public class PublicApiTest
    {
        [Theory]
        [InlineData("net452")]
        [InlineData("netstandard2.0")]
        public void TestODataPublicApiOnMultipleFramework(string framework)
        {
            string[] packages =
            {
                "Microsoft.Spatial",
                "Microsoft.OData.Edm",
                "Microsoft.OData.Core",
                "Microsoft.OData.Client"
            };

            TestPublicApi(packages, framework);
        }

        [Fact]
        public void TestODataPublicApiOnNetStandard11()
        {
            // OData client doesn't have .NETStandard1.1 output
            string[] packages =
            {
                "Microsoft.Spatial",
                "Microsoft.OData.Edm",
                "Microsoft.OData.Core"
            };

            TestPublicApi(packages, "netstandard1.1");
        }

        [Theory]
        [InlineData("netstandard2.0", "netstandard1.1")]
        [InlineData("netstandard1.1", "netstandard2.0")]
        public void TestODataPublicApiOnDifferentFrameworksUsesRequestedAssembly(string firstFramework, string secondFramework)
        {
            foreach (string framework in new[] { firstFramework, secondFramework })
            {
                string outputPath = Environment.CurrentDirectory;
                string buildConfig = Directory.GetParent(outputPath).Name;
                string template = @"test\PublicApiTests\bin\" + buildConfig + @"\netcoreapp3.1";
                string assemblyPath = outputPath.Replace(template, @"src\Microsoft.OData.Edm\bin\" + buildConfig + @"\" + framework);
                string assemblyFullPath = Path.Combine(assemblyPath, "Microsoft.OData.Edm.dll");

                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
                    {
                        PublicApiHelper.DumpPublicApi(writer, assemblyFullPath);
                    }

                    string api = Encoding.UTF8.GetString(stream.ToArray());
                    Assert.Contains("Microsoft.OData.Edm.Csdl.CsdlReader", api);
                    if (framework == "netstandard2.0")
                    {
                        Assert.Contains("Microsoft.OData.Edm.Csdl.CsdlJsonReaderSettings", api);
                    }
                    else
                    {
                        Assert.DoesNotContain("Microsoft.OData.Edm.Csdl.CsdlJsonReaderSettings", api);
                    }
                }
            }
        }

        private static void TestPublicApi(IList<string> packageNames, string framework)
        {
            // Arrange
            string outputFileName = "Microsoft.OData.PublicApi." + framework + ".out";

            // ~ \odata.net\test\PublicApiTests\bin\Debug\netcoreapp3.1
            string outputPath = Environment.CurrentDirectory;
            string outputFile = outputPath + Path.DirectorySeparatorChar + outputFileName;

            // simply get "Debug/Release"
            var directoryInfo = Directory.GetParent(outputPath);
            string buildConfig = directoryInfo.Name;
            string template = @"test\PublicApiTests\bin\" + buildConfig + @"\netcoreapp3.1";

            // Act
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var packageName in packageNames)
                    {
                        string tempPath = @"src\" + packageName + @"\bin\" + buildConfig + @"\" + framework;
                        string assemblyPath = outputPath.Replace(template, tempPath);
                        string assemblyFullPath = assemblyPath + Path.DirectorySeparatorChar + packageName + ".dll";
                        Assert.True(File.Exists(assemblyFullPath), string.Format("{0} does not exist in current directory", assemblyFullPath));

                        sw.WriteLine(">>>" + packageName);
                        PublicApiHelper.DumpPublicApi(sw, assemblyFullPath);
                    }
                }
            }

            string outputString = File.ReadAllText(outputFile);

            string baseLineFileName = "Microsoft.OData.PublicApi." + framework + ".bsl";
            string baselineString = GetBaseLineString(baseLineFileName);

            // Assert
            Assert.True(String.Compare(baselineString, outputString, StringComparison.Ordinal) == 0,
                String.Format("The output file '{0}' and\n Base line file '{1}' do not match, please check.",
                outputFile,
                @"test\PublicApiTests\BaseLine\" + baseLineFileName));
        }

        private static string GetBaseLineString(string baseFileName)
        {
            const string projectDefaultNamespace = "Microsoft.OData.PublicApi.Tests";
            const string resourcesFolderName = "BaseLine";
            const string pathSeparator = ".";
            string path = projectDefaultNamespace + pathSeparator + resourcesFolderName + pathSeparator + baseFileName;

            using (Stream stream = typeof(PublicApiTest).Assembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "The embedded resource '{0}' was not found.", path);
                    throw new FileNotFoundException(message, path);
                }

                using (TextReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
