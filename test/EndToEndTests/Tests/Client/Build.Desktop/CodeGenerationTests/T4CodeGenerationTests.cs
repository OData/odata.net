//---------------------------------------------------------------------
// <copyright file="T4CodeGenerationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.ComponentModel;
using System.Xml;

namespace Microsoft.Test.OData.Tests.Client.CodeGenerationTests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.CSharp;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Design.T4;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualBasic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [DeploymentItem(@"CSDSCReferences\", "CSDSCReferences")]
    [DeploymentItem(@"CSReferences\", "CSReferences")]
    [DeploymentItem(@"VBDSCReferences\", "VBDSCReferences")]
    [DeploymentItem(@"VBReferences\", "VBReferences")]
#if !WIN8 && !WINDOWSPHONE
    [DeploymentItem(@"EntityFramework.dll")]
#endif
    [TestClass]
    public class T4CodeGenerationTests
    {
        private const string NameSpacePrefixReference = "Microsoft.Test.OData.Services.TestServices.";
        private const string NameSpacePrefixCodeGenerationTests = "CodeGenerationTests.";
        private const string T4Version = "2.4.0";
        private const string SystemIO = "System.IO.dll";
        private const string SystemRuntime = "System.Runtime.dll";
        private const string SystemXmlReaderWriter = "System.Xml.ReaderWriter.dll";

        private static readonly Dictionary<string, ServiceDescriptor> TestServiceDescriptors = new Dictionary<string, ServiceDescriptor>()
        {
            // CS + UseDataServiceCollection codegen results of the following services are referenced by client tests  
            {NameSpacePrefixReference + "AstoriaDefaultServiceReference", ServiceDescriptors.AstoriaDefaultService},
            {NameSpacePrefixReference + "ActionOverloadingServiceReference", ServiceDescriptors.ActionOverloadingService},
            {NameSpacePrefixReference + "KeyAsSegmentServiceReference", ServiceDescriptors.KeyAsSegmentService},
            {NameSpacePrefixReference + "ODataWriterDefaultServiceReference", ServiceDescriptors.ODataWriterService},
            {NameSpacePrefixReference + "PrimitiveKeysServiceReference", ServiceDescriptors.PrimitiveKeysService},

            //  tests running against the following services depend on modified References.cs, so we will compile the generated code and verify the assembly
            {NameSpacePrefixCodeGenerationTests + "OpenTypesServiceReference", ServiceDescriptors.OpenTypesService},
            {NameSpacePrefixCodeGenerationTests + "PublicProviderEFServiceReference", ServiceDescriptors.PublicProviderEFService},
            {NameSpacePrefixCodeGenerationTests + "PublicProviderReflectionServiceReference", ServiceDescriptors.PublicProviderReflectionService},
            {NameSpacePrefixCodeGenerationTests + "ModelReferenceServiceReference", ServiceDescriptors.ModelRefServiceDescriptor},

            //{NameSpacePrefixReference + "ODataWCFServiceReference", ServiceDescriptors.ODataWCFServiceDescriptor}
            //{NameSpacePrefixReference + "TrippinServiceReference", ServiceDescriptors.TripPinServiceDescriptor},
            //{NameSpacePrefixReference + "PluggableFormatServiceReference", ServiceDescriptors.PluggableFormatServiceDescriptor},
            //{NameSpacePrefixReference + "OperationServiceReference", ServiceDescriptors.OperationServiceDescriptor},
        };

        private static ODataT4CodeGenerator.LanguageOption[] languageOptions = { ODataT4CodeGenerator.LanguageOption.CSharp, ODataT4CodeGenerator.LanguageOption.VB };
        private static bool[] useDataServiceCollectionBools = { true, false };

        /// <summary>
        /// Verify that generated code is the same as the client code layer we checked in. 
        /// If this test fails, either there is regression in codegen or we need to update the client code layer in E2E framework.
        /// </summary>
        [TestMethod]
        public void VerifyGeneratedCodeString()
        {
            TestServiceUtil.ServiceUriGenerator = new ServiceUriGenerator();
            foreach (var descriptor in TestServiceDescriptors.Where(tsd => tsd.Key.StartsWith(NameSpacePrefixReference)))
            {
                var serviceWrapper = new DefaultServiceWrapper(descriptor.Value);
                try
                {
                    serviceWrapper.StartService();
                    var edmx = RetrieveServiceModelEdmx(serviceWrapper.ServiceUri);
                    Assert.IsNotNull(edmx);

                    foreach (var languageOption in languageOptions)
                    {
                        foreach (var useDataServiceCollection in useDataServiceCollectionBools)
                        {
                            var extension = languageOption == ODataT4CodeGenerator.LanguageOption.CSharp ? "cs" : "vb";
                            var path = languageOption == ODataT4CodeGenerator.LanguageOption.CSharp ? @".\CS" : @".\VB";
                            path += useDataServiceCollection ? "DSCReferences" : "References";
                            path = Path.Combine(path, descriptor.Key + "." + extension);
                            var expectedCode = File.ReadAllText(path);
                            var generatedCode = this.Generate(edmx, descriptor.Key, languageOption, useDataServiceCollection);
                            this.VerifyGeneratedCodeString(expectedCode, generatedCode, path);

                            // To generate the updated client layer files, uncomment this, and then copy the resulting files under folder 
                            // CSDSCReferences & CSReferences & VBDSCReferences & VBReferences to the reference projects
                            // this.GenerateClientCodeFile(edmx, path, descriptor.Key, languageOption, useDataServiceCollection);
                        }
                    }
                }
                finally
                {
                    serviceWrapper.StopService();
                }
            }
        }

        /// <summary>
        /// Compile generated cs/vb code and do basic verification against the IEdmModel.
        /// </summary>
        [TestMethod]
        public void CompileAndVerifyGeneratedCode()
        {
            TestServiceUtil.ServiceUriGenerator = new ServiceUriGenerator();
            foreach (var descriptor in TestServiceDescriptors)
            {
                var serviceWrapper = new DefaultServiceWrapper(descriptor.Value);
                try
                {
                    serviceWrapper.StartService();

                    var edmx = RetrieveServiceModelEdmx(serviceWrapper.ServiceUri);
                    Assert.IsNotNull(edmx);

                    // retrieve the IEdmModel corresponding with the edmx
                    byte[] byteArray = Encoding.UTF8.GetBytes(edmx);
                    var message = new StreamResponseMessage(new MemoryStream(byteArray));
                    message.SetHeader("Content-Type", MimeTypes.ApplicationXml);

                    IEdmModel model = null;
                    using (var messageReader = new ODataMessageReader(message))
                    {
                        model = messageReader.ReadMetadataDocument();
                    }

                    foreach (var languageOption in languageOptions)
                    {
                        foreach (var useDataServiceCollection in useDataServiceCollectionBools)
                        {
                            var generatedCode = this.Generate(edmx, descriptor.Key, languageOption, useDataServiceCollection);
                            this.CompileAndVerify(generatedCode, languageOption == ODataT4CodeGenerator.LanguageOption.CSharp, model);
                        }
                    }
                }
                finally
                {
                    serviceWrapper.StopService();
                }
            }
        }

        /// <summary>
        /// Query $medatadata and return the metadata string.
        /// </summary>
        /// <param name="uri">The service Uri</param>
        /// <returns>The metadata string</returns>
        private string RetrieveServiceModelEdmx(Uri serviceUri)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(serviceUri, "$metadata"));
            request.Accept = MimeTypes.ApplicationXml;
            var response = request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            return streamReader.ReadToEnd();
        }

        private string Generate(string edmx, string referenceName, ODataT4CodeGenerator.LanguageOption languageOption, bool useDataServiceCollection)
        {
            ODataT4CodeGenerator t4CodeGenerator = new ODataT4CodeGenerator
            {
                Edmx = edmx,
                NamespacePrefix = referenceName,
                TargetLanguage = languageOption,
                UseDataServiceCollection = useDataServiceCollection,
            };

            return t4CodeGenerator.TransformText();
        }

        private void CompileAndVerify(string source, bool isCSharp, IEdmModel model)
        {
            // verify that the given cs/vb code can be compiled successfully
            CompilerParameters compilerOptions = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = true,
                WarningLevel = 4,
                ReferencedAssemblies =
                {
                    typeof(DataServiceContext).Assembly.Location,
                    typeof(IEdmModel).Assembly.Location,
                    typeof(GeographyPoint).Assembly.Location,
                    typeof(Uri).Assembly.Location,
                    typeof(IQueryable).Assembly.Location,
                    typeof(INotifyPropertyChanged).Assembly.Location,
                    typeof(XmlReader).Assembly.Location,
                    SystemIO,
                    SystemRuntime,
                    SystemXmlReaderWriter
                }
            };

            CodeDomProvider codeProvider = null;
            if (isCSharp)
            {
                codeProvider = new CSharpCodeProvider();
            }
            else
            {
                codeProvider = new VBCodeProvider();
            }

            var results = codeProvider.CompileAssemblyFromSource(compilerOptions, source);
            Assert.AreEqual(0, results.Errors.Count);

            // verify that the generated assembly contains entity types, complex types, and the entity container specified in the given model
            foreach (var modelElement in model.SchemaElements.Where(se => se.SchemaElementKind != EdmSchemaElementKind.Function && se.SchemaElementKind != EdmSchemaElementKind.Action))
            {
                Type t = results.CompiledAssembly.DefinedTypes.FirstOrDefault(dt => dt.Name == modelElement.Name);
                Assert.IsNotNull(t, modelElement.Name);

                IEdmEntityType entityType = modelElement as IEdmEntityType;
                IEdmComplexType complexTypeType = modelElement as IEdmComplexType;
                IEdmEntityContainer container = modelElement as IEdmEntityContainer;
                if (entityType != null)
                {
                    foreach (var edmProperty in entityType.DeclaredProperties)
                    {
                        Assert.IsNotNull(t.GetProperties().Where(p => p.Name == edmProperty.Name), "Property not found: " + edmProperty.Name + " in " + entityType.Name);
                    }
                }
                else if (complexTypeType != null)
                {
                    foreach (var edmProperty in complexTypeType.DeclaredProperties)
                    {
                        Assert.IsNotNull(t.GetProperties().Where(p => p.Name == edmProperty.Name), "Property not found: " + edmProperty.Name + " in " + complexTypeType.Name);
                    }
                }
                else if (container != null)
                {
                    foreach (var entitySet in container.EntitySets())
                    {
                        Assert.IsNotNull(t.GetProperties().Where(p => p.Name == entitySet.Name), "EntitySet not found: " + entitySet.Name);
                    }
                }
                else
                {
                    Assert.Fail("Unhandled model element " + modelElement.GetType().Name);
                }
            }
        }

        private void VerifyGeneratedCodeString(string expected, string actual, string path)
        {
            // Since test model is delay-loaded when generating $metadata output, the order of entity types/properties could change in test runs,
            // we verify that the actual codegen result is the equivalent with the expected except that:
            // 1. do not verify comments for "Generation date" and "Runtime Version"
            // 2. do not verify the $metadata string (ModelParts) embedded in the generated code
            // 3. ignore the order of entity types and properties in the generated code

            var expectedLines = expected.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var actualLines = actual.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(expectedLines.Count(), actualLines.Count());

            int modelPartStartLine = 0; // the first line of the edmx string
            int modelPartEndLine = 0; // the last line of the edmx string
            for (int i = 0; i < actualLines.Count(); i++)
            {
                if (actualLines[i].Contains("<edmx:Edmx Version="))
                {
                    modelPartStartLine = i;
                }
                else if (actualLines[i].Contains("</edmx:Edmx>"))
                {
                    modelPartEndLine = i;
                }
            }

            Assert.IsTrue(modelPartStartLine > 0 && modelPartStartLine <= modelPartEndLine, "metadata output not found in generated code");

            for (int i = 0; i < actualLines.Count(); i++)
            {
                if (!expectedLines.Contains(actualLines[i]))
                {
                    Assert.IsTrue((i >= modelPartStartLine && i <= modelPartEndLine) ||
                        actualLines[i].StartsWith("// Generation date:") ||
                        actualLines[i].StartsWith("'Generation date:") ||
                        actualLines[i].StartsWith("//     Runtime Version:") ||
                        actualLines[i].StartsWith("'     Runtime Version:") ||
                        actualLines[i].EndsWith(string.Format(@"[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", ""{0}"")]", T4Version)) ||
                        actualLines[i].EndsWith(string.Format(@"<Global.System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", ""{0}"")>  _", T4Version)),
                        "Expected: " + expected + Environment.NewLine + "Actual: " + actual);
                }
            }
        }

        private void GenerateClientCodeFile(string edmx, string path, string nameSpacePrefix, ODataT4CodeGenerator.LanguageOption languageOption, bool useDataServiceCollection)
        {
            var result = this.Generate(edmx, nameSpacePrefix, languageOption, useDataServiceCollection);
            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.WriteLine(result);
            }
        }

        private class ServiceUriGenerator : IServiceUriGenerator
        {
            private const int ServicePort = 9090;

            /// <summary>
            /// Generates a Uri for a test service hosted on the local machine.
            /// </summary>
            /// <param name="path">A unique path segment to identify the service.</param>
            /// <returns>Uri for the locally hosted service.</returns>
            public Uri GenerateServiceUri(string path)
            {
                return new Uri("http://" + Environment.MachineName + ":" + ServicePort + "/" + path + DateTimeOffset.Now.Ticks.ToString(CultureInfo.InvariantCulture) + "/");
            }
        }
    }
}
