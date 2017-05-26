//---------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorTestDescriptors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FluentAssertions;

namespace Microsoft.OData.Client.Design.T4.UnitTests
{
    public partial class ODataT4CodeGeneratorTestDescriptors
    {
        private const string ExpectedCSharpUseDSC = "ExpectedCSharpUseDSC";
        private const string ExpectedCSharp = "ExpectedCSharp";
        private const string ExpectedVBUseDSC = "ExpectedVBUseDSC";
        private const string ExpectedVB = "ExpectedVB";
        private const string T4Version = "2.4.0";

        public class ODataT4CodeGeneratorTestsDescriptor
        {
            /// <summary>
            /// Edmx Metadata to generate code from.
            /// </summary>
            public string Metadata { get; set; }

            /// <summary>
            /// Gets or Sets the func for getting the referenced model's stream.
            /// </summary>
            public Func<Uri, XmlReader> GetReferencedModelReaderFunc { get; set; }

            /// <summary>
            /// Dictionary of expected CSharp/VB code generation results.
            /// </summary>
            public Dictionary<string, string> ExpectedResults { get; set; }

            /// <summary>
            /// A custom verification action to perform. Takes in the generated code and runs asserts that the code was generated properly. A verification function provided here should be valid for both CodeGen using the Design DLL and T4.
            /// </summary>
            public Action<string, bool, bool> Verify { get; set; }
        }

        private static void VerifyGeneratedCode(string actualCode, Dictionary<string, string> expectedCode, bool isCSharp, bool useDSC, string key = null)
        {
            string expected;
            if (isCSharp && useDSC)
            {
                expected = expectedCode[ExpectedCSharpUseDSC];
            }
            else if (isCSharp && !useDSC)
            {
                expected = expectedCode[ExpectedCSharp];
            }
            else if (!isCSharp && useDSC)
            {
                expected = expectedCode[ExpectedVBUseDSC];
            }
            else
            {
                expected = expectedCode[ExpectedVB];
            }

            string actualBak = actualCode;
            string normalizedExpectedCode = Regex.Replace(expected, "// Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "'Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "//     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "'     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode,
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
                RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode,
                "Global.System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
                "Global.System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
                RegexOptions.Multiline);
            actualCode = Regex.Replace(actualCode, "// Generation date:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "'Generation date:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "//     Runtime Version:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "'     Runtime Version:.*", string.Empty);

            if (key == null)
            {
                actualCode.Should().Be(normalizedExpectedCode);
            }
            else
            {
                bool equal = actualCode == normalizedExpectedCode;

                if (!equal)
                {
                    string filename = key + (useDSC ? "DSC" : "") + (isCSharp ? ".cs" : ".vb");
                    string currentFolder = Directory.GetCurrentDirectory();
                    string path = Path.Combine(currentFolder, filename);
                    File.WriteAllText(path, actualBak);
                    string basePath = string.Format(currentFolder + @"\..\CodeGen\{0}", filename);
                    equal.Should().Be(true, "Baseline not equal.\n " +
                        "To diff run: \n" +
                        "odd \"{0}\" \"{1}\"\n" +
                        "To update run: \n" +
                        "copy /y \"{1}\" \"{0}\"\n" +
                        "\n", basePath, path);
                }
            }
        }

        private const string BaseName = "Microsoft.OData.Client.Design.T4.UnitTests.CodeGen.";
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        private static string LoadContentFromBaseline(string key)
        {
            Stream stream = null;
            try
            {
                stream = Assembly.GetManifestResourceStream(BaseName + key);
                if (stream == null)
                {
                    throw new ApplicationException("Baseline [" + key + "] not found.");
                }

                using (var sr = new StreamReader(stream))
                {
                    stream = null;
                    return sr.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        #region EntityHierarchyWithIDAndId

        public static string EdmxEntityHierarchyWithIDAndId = LoadContentFromBaseline("EntityHierarchyWithIDAndId.xml");
        public static string EntityHierarchyWithIDAndIdCSharpUseDSC = LoadContentFromBaseline("EntityHierarchyWithIDAndIdDSC.cs");
        public static string EntityHierarchyWithIDAndIdVBUseDSC = LoadContentFromBaseline("EntityHierarchyWithIDAndIdDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor EntityHierarchyWithIDAndId = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntityHierarchyWithIDAndId,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharpUseDSC, EntityHierarchyWithIDAndIdCSharpUseDSC }, { ExpectedVBUseDSC, EntityHierarchyWithIDAndIdVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntityHierarchyWithIDAndId.ExpectedResults, isCSharp, useDSC, "EntityHierarchyWithIDAndId"),
        };

        #endregion

        #region Simple

        public static string EdmxSimple = LoadContentFromBaseline("Simple.xml");
        public static string SimpleCSharp = LoadContentFromBaseline("Simple.cs");
        public static string SimpleCSharpUseDSC = LoadContentFromBaseline("SimpleDSC.cs");
        public static string SimpleVB = LoadContentFromBaseline("Simple.vb");
        public static string SimpleVBUseDSC = LoadContentFromBaseline("SimpleDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor Simple = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSimple,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, SimpleCSharp }, { ExpectedCSharpUseDSC, SimpleCSharpUseDSC }, { ExpectedVB, SimpleVB }, { ExpectedVBUseDSC, SimpleVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, Simple.ExpectedResults, isCSharp, useDSC, "Simple"),
        };
        #endregion

        #region NamespacePrefix

        public static string EdmxNamespacePrefixWithSingleNamespace = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.xml");
        public static string NamespacePrefixWithSingleNamespaceCSharp = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.cs");
        public static string NamespacePrefixWithSingleNamespaceVB = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.vb");

        public static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithSingleNamespace = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithSingleNamespace,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, NamespacePrefixWithSingleNamespaceCSharp }, 
                { ExpectedVB, NamespacePrefixWithSingleNamespaceVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithSingleNamespace.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithSingleNamespace"),
        };

        public static string EdmxNamespacePrefixWithDoubleNamespaces = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.xml");
        public static string NamespacePrefixWithDoubleNamespacesCSharp = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.cs");
        public static string NamespacePrefixWithDoubleNamespacesVB = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.vb");

        public static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithDoubleNamespaces = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithDoubleNamespaces,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, NamespacePrefixWithDoubleNamespacesCSharp }, 
                { ExpectedVB, NamespacePrefixWithDoubleNamespacesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithDoubleNamespaces.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithDoubleNamespaces"),
        };

        public static string EdmxNamespacePrefixWithInheritence = LoadContentFromBaseline("NamespacePrefixWithInheritence.xml");
        public static string NamespacePrefixWithInheritenceCSharp = LoadContentFromBaseline("NamespacePrefixWithInheritence.cs");
        public static string NamespacePrefixWithInheritenceVB = LoadContentFromBaseline("NamespacePrefixWithInheritence.vb");

        public static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithInheritence = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithInheritence,
            ExpectedResults = new Dictionary<string, string>() 
        {
                { ExpectedCSharp, NamespacePrefixWithInheritenceCSharp }, 
                { ExpectedVB, NamespacePrefixWithInheritenceVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithInheritence.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithInheritence"),
        };

        #endregion

        #region NamespacePrefixRepeatWithSchemaNameSpace

        public static string EdmxNamespacePrefixRepeatWithSchemaNameSpace = LoadContentFromBaseline("NamespacePrefixRepeatWithSchemaNameSpace.xml");
        public static string NamespacePrefixRepeatWithSchemaNameSpaceCSharp = LoadContentFromBaseline("NamespacePrefixRepeatWithSchemaNameSpace.cs");

        public static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixRepeatWithSchemaNameSpace = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixRepeatWithSchemaNameSpace, 
            ExpectedResults = new Dictionary<string, string>()
            {
                {ExpectedCSharp, NamespacePrefixRepeatWithSchemaNameSpaceCSharp}, 
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixRepeatWithSchemaNameSpace.ExpectedResults, isCSharp, useDSC, "NamespacePrefixRepeatWithSchemaNameSpace"),
        };

        #endregion

        #region KeywordsAsNames

        public static string EdmxKeywordsAsNames = LoadContentFromBaseline("KeywordsAsNames.xml");
        public static string KeywordsAsNamesCSharp = LoadContentFromBaseline("KeywordsAsNames.cs");
        public static string KeywordsAsNamesVB = LoadContentFromBaseline("KeywordsAsNames.vb");

        public static ODataT4CodeGeneratorTestsDescriptor KeywordsAsNames = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxKeywordsAsNames,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, KeywordsAsNamesCSharp }, 
                { ExpectedVB, KeywordsAsNamesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, KeywordsAsNames.ExpectedResults, isCSharp, useDSC, "KeywordsAsNames"),
        };

        #endregion

        #region MergedFunctionalTest

        public static string EdmxMergedFunctionalTest = LoadContentFromBaseline("MergedFunctionalTest.xml");
        public static string MergedFunctionalTestCSharp = LoadContentFromBaseline("MergedFunctionalTest.cs");
        public static string MergedFunctionalTestVB = LoadContentFromBaseline("MergedFunctionalTest.vb");
        public static string MergedFunctionalTestCSharpUseDSC = LoadContentFromBaseline("MergedFunctionalTestDSC.cs");
        public static string MergedFunctionalTestVBUseDSC = LoadContentFromBaseline("MergedFunctionalTestDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor MergedFunctionalTest = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxMergedFunctionalTest, 
            ExpectedResults = new Dictionary<string, string>()
            {
                {ExpectedCSharp, MergedFunctionalTestCSharp}, 
                {ExpectedCSharpUseDSC, MergedFunctionalTestCSharpUseDSC}, 
                {ExpectedVB, MergedFunctionalTestVB}, 
                {ExpectedVBUseDSC, MergedFunctionalTestVBUseDSC}
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MergedFunctionalTest.ExpectedResults, isCSharp, useDSC, "MergedFunctionalTest"),
        };

        #endregion

        #region Multiplicity

        public static string EdmxMultiplicity = LoadContentFromBaseline("Multiplicity.xml");
        public static string MultiplicityCSharp = LoadContentFromBaseline("Multiplicity.cs");
        public static string MultiplicityVB = LoadContentFromBaseline("Multiplicity.vb");
        public static string MultiplicityCSharpUseDSC = LoadContentFromBaseline("MultiplicityDSC.cs");
        public static string MultiplicityVBUseDSC = LoadContentFromBaseline("MultiplicityDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor Multiplicity = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxMultiplicity,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, MultiplicityCSharp },
                { ExpectedCSharpUseDSC, MultiplicityCSharpUseDSC },
                { ExpectedVB, MultiplicityVB },
                { ExpectedVBUseDSC, MultiplicityVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, Multiplicity.ExpectedResults, isCSharp, useDSC, "MultiplicityDSC"),
        };

        #endregion

        #region EmptySchema

        public static string EdmxEmptySchema = LoadContentFromBaseline("EmptySchema.xml");
        public static string EmptySchemaCSharp = LoadContentFromBaseline("EmptySchema.cs");
        public static string EmptySchemaVBUseDSC = LoadContentFromBaseline("EmptySchemaDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor EmptySchema = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEmptySchema,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EmptySchemaCSharp },
                { ExpectedVBUseDSC, EmptySchemaVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EmptySchema.ExpectedResults, isCSharp, useDSC, "EmptySchema"),
        };

        #endregion

        #region NamespaceInKeywords

        public static string EdmxNamespaceInKeywords = LoadContentFromBaseline("NamespaceInKeywords.xml");
        public static string NamespaceInKeywordsCSharp = LoadContentFromBaseline("NamespaceInKeywords.cs");
        public static string NamespaceInKeywordsVB = LoadContentFromBaseline("NamespaceInKeywords.vb");

        public static ODataT4CodeGeneratorTestsDescriptor NamespaceInKeywords = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespaceInKeywords,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, NamespaceInKeywordsCSharp }, 
                { ExpectedVB, NamespaceInKeywordsVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespaceInKeywords.ExpectedResults, isCSharp, useDSC, "NamespaceInKeywords"),
        };

        #endregion

        #region NamespaceInKeywordsWithRefModel

        public static string EdmxNamespaceInKeywordsWithRefModel = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.xml");
        public static string EdmxNamespaceInKeywordsWithRefModelReferencedEdmx = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelReferenced.xml");
        public static string NamespaceInKeywordsWithRefModelCSharp = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.cs");
        public static string NamespaceInKeywordsWithRefModelVB = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.vb");
        public static string NamespaceInKeywordsWithRefModelCSharpUseDSC = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelDSC.cs");
        public static string NamespaceInKeywordsWithRefModelVBUseDSC = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor NamespaceInKeywordsWithRefModel = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespaceInKeywordsWithRefModel,
            GetReferencedModelReaderFunc = url => XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(EdmxNamespaceInKeywordsWithRefModelReferencedEdmx))),
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, NamespaceInKeywordsWithRefModelCSharp }, 
                { ExpectedVB, NamespaceInKeywordsWithRefModelVB },
                { ExpectedCSharpUseDSC, NamespaceInKeywordsWithRefModelCSharpUseDSC }, 
                { ExpectedVBUseDSC, NamespaceInKeywordsWithRefModelVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespaceInKeywordsWithRefModel.ExpectedResults, isCSharp, useDSC, "NamespaceInKeywordsWithRefModel"),
        };

        #endregion

        #region MultiReferenceModel

        public static string EdmxWithMultiReferenceModel = LoadContentFromBaseline("MultiReferenceModel.xml");
        public static string MultiReferenceModelCoreTermsEdmx = LoadContentFromBaseline("MultiReferenceModelCoreTerms.xml");
        public static string MultiReferenceModelDeviceModelTermsEdmx = LoadContentFromBaseline("MultiReferenceModelDeviceModelTerms.xml");
        public static string MultiReferenceModelGPSEdmx = LoadContentFromBaseline("MultiReferenceModelGPS.xml");
        public static string MultiReferenceModelLocationEdmx = LoadContentFromBaseline("MultiReferenceModelLocation.xml");
        public static string MultiReferenceModelMapEdmx = LoadContentFromBaseline("MultiReferenceModelMap.xml");

        public static string MultiReferenceModelCSharp = LoadContentFromBaseline("MultiReferenceModel.cs");
        public static string MultiReferenceModelVB = LoadContentFromBaseline("MultiReferenceModel.vb");
        public static string MultiReferenceModelCSharpUseDSC = LoadContentFromBaseline("MultiReferenceModelDSC.cs");
        public static string MultiReferenceModelVBUseDSC = LoadContentFromBaseline("MultiReferenceModelDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor MultiReferenceModel = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxWithMultiReferenceModel,
            GetReferencedModelReaderFunc = url =>
            {
                string text;
                string urlStr = url.OriginalString;
                if (urlStr.EndsWith("CoreTerms.csdl"))
                {
                    text = MultiReferenceModelCoreTermsEdmx;
                }
                else if (urlStr.EndsWith("DeviceModelTerms.csdl"))
                {
                    text = MultiReferenceModelDeviceModelTermsEdmx;
                }
                else if (urlStr.EndsWith("GPS.csdl"))
                {
                    text = MultiReferenceModelGPSEdmx;
                }
                else if (urlStr.EndsWith("Location.csdl"))
                {
                    text = MultiReferenceModelLocationEdmx;
                }
                else // (urlStr.EndsWith("Map.csdl"))
                {
                    text = MultiReferenceModelMapEdmx;
                }

                XmlReaderSettings setting = new XmlReaderSettings()
                {
                    IgnoreWhitespace = true
                };

                return XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(text)), setting);
            },
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, MultiReferenceModelCSharp }, 
                { ExpectedVB, MultiReferenceModelVB },
                { ExpectedCSharpUseDSC, MultiReferenceModelCSharpUseDSC }, 
                { ExpectedVBUseDSC, MultiReferenceModelVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MultiReferenceModel.ExpectedResults, isCSharp, useDSC, "MultiReferenceModel"),
        };

        #endregion

        #region UpperCamelCaseWithNamespacePrefix

        public static string EdmxUpperCamelCaseWithNamespacePrefix = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.xml");
        public static string UpperCamelCaseWithNamespacePrefixCSharp = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.cs");
        public static string UpperCamelCaseWithNamespacePrefixVB = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.vb");
        
        public static ODataT4CodeGeneratorTestsDescriptor UpperCamelCaseWithNamespacePrefix = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUpperCamelCaseWithNamespacePrefix,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, UpperCamelCaseWithNamespacePrefixCSharp }, 
                { ExpectedVB, UpperCamelCaseWithNamespacePrefixVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, UpperCamelCaseWithNamespacePrefix.ExpectedResults, isCSharp, useDSC, "UpperCamelCaseWithNamespacePrefix"),
        };

        #endregion

        #region UpperCamelCaseWithoutNamespacePrefix

        public static string EdmxUpperCamelCaseWithoutNamespacePrefix = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.xml");
        public static string UpperCamelCaseWithoutNamespacePrefixCSharp = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.cs");
        public static string UpperCamelCaseWithoutNamespacePrefixVB = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.vb");
        public static string UpperCamelCaseWithoutNamespacePrefixCSharpUseDSC = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefixDSC.cs");
        public static string UpperCamelCaseWithoutNamespacePrefixVBUseDSC = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefixDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor UpperCamelCaseWithoutNamespacePrefix = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUpperCamelCaseWithoutNamespacePrefix,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, UpperCamelCaseWithoutNamespacePrefixCSharp }, 
                { ExpectedVB, UpperCamelCaseWithoutNamespacePrefixVB },
                { ExpectedCSharpUseDSC, UpperCamelCaseWithoutNamespacePrefixCSharpUseDSC },
                { ExpectedVBUseDSC, UpperCamelCaseWithoutNamespacePrefixVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, UpperCamelCaseWithoutNamespacePrefix.ExpectedResults, isCSharp, useDSC, "UpperCamelCaseWithoutNamespacePrefix"),
        };

        #endregion

        #region IgnoreUnexpectedElementsAndAttributes

        public static string EdmxUnexpectedElementsAndAttributes = LoadContentFromBaseline("UnexpectedElementsAndAttributes.xml");
        public static string UnexpectedElementsAndAttributesCSharp = LoadContentFromBaseline("UnexpectedElementsAndAttributes.cs");
        public static string UnexpectedElementsAndAttributesVB = LoadContentFromBaseline("UnexpectedElementsAndAttributes.vb");
        public static ODataT4CodeGeneratorTestsDescriptor IgnoreUnexpectedElementsAndAttributes = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUnexpectedElementsAndAttributes,
            ExpectedResults = new Dictionary<string, string>() 
            { 
                { ExpectedCSharp, UnexpectedElementsAndAttributesCSharp }, 
                { ExpectedVB, UnexpectedElementsAndAttributesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, IgnoreUnexpectedElementsAndAttributes.ExpectedResults, isCSharp, useDSC, "UnexpectedElementsAndAttributes"),
        };

        #endregion

        #region PrefixConflict
        public static string EdmxPrefixConflict = LoadContentFromBaseline("PrefixConflict.xml");
        public static string PrefixConflictCSharp = LoadContentFromBaseline("PrefixConflict.cs");
        public static string PrefixConflictVBUseDSC = LoadContentFromBaseline("PrefixConflictDSC.vb");

        public static ODataT4CodeGeneratorTestsDescriptor PrefixConflict = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxPrefixConflict,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, PrefixConflictCSharp },
                { ExpectedVBUseDSC, PrefixConflictVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, PrefixConflict.ExpectedResults, isCSharp, useDSC, "PrefixConflict"),
        };
        #endregion

        #region DupNames

        public static string EdmxDupNames = LoadContentFromBaseline("DupNames.xml");
        public static string DupNamesCSharp = LoadContentFromBaseline("DupNames.cs");
        public static string DupNamesVBUseDSC = LoadContentFromBaseline("DupNamesDSC.vb");
        public static string DupNamesWithCamelCaseCSharpUseDSC = LoadContentFromBaseline("DupNamesWithCamelCaseDSC.cs");
        public static string DupNamesWithCamelCaseVB = LoadContentFromBaseline("DupNamesWithCamelCase.vb");

        public static ODataT4CodeGeneratorTestsDescriptor DupNames = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxDupNames,
            ExpectedResults = new Dictionary<string, string>()
            { 
                { ExpectedCSharp, DupNamesCSharp },
                { ExpectedVBUseDSC, DupNamesVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, DupNames.ExpectedResults, isCSharp, useDSC, "DupNames"),
        };

        public static ODataT4CodeGeneratorTestsDescriptor DupNamesWithCamelCase = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxDupNames,
            ExpectedResults = new Dictionary<string, string>()
            { 
                { ExpectedCSharpUseDSC, DupNamesWithCamelCaseCSharpUseDSC },
                { ExpectedVB, DupNamesWithCamelCaseVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, DupNamesWithCamelCase.ExpectedResults, isCSharp, useDSC, "DupNamesWithCamelCase"),
        };
        #endregion

        #region OverrideOperations

        public static string EdmxOverrideOperations = LoadContentFromBaseline("OverrideOperations.xml");
        public static string OverrideOperationsCSharpUseDSC = LoadContentFromBaseline("OverrideOperationsDSC.cs");
        public static string OverrideOperationsVB = LoadContentFromBaseline("OverrideOperations.vb");

        public static ODataT4CodeGeneratorTestsDescriptor OverrideOperations = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxOverrideOperations,
            ExpectedResults = new Dictionary<string, string>()
            { 
                { ExpectedCSharpUseDSC, OverrideOperationsCSharpUseDSC },
                { ExpectedVB, OverrideOperationsVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, OverrideOperations.ExpectedResults, isCSharp, useDSC, "OverrideOperations"),
        };
        #endregion

        #region AbstractEntityTypeWithoutKey
        public static string EdmxAbstractEntityTypeWithoutKey = LoadContentFromBaseline("AbstractEntityTypeWithoutKey.xml");
        public static string AbstractEntityTypeWithoutKeyCSharpUseDSC = LoadContentFromBaseline("AbstractEntityTypeWithoutKeyDSC.cs");
        public static string AbstractEntityTypeWithoutKeyVB = LoadContentFromBaseline("AbstractEntityTypeWithoutKey.vb");

        public static ODataT4CodeGeneratorTestsDescriptor AbstractEntityTypeWithoutKey = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxAbstractEntityTypeWithoutKey,
            ExpectedResults = new Dictionary<string, string>()
            { 
                { ExpectedCSharpUseDSC, AbstractEntityTypeWithoutKeyCSharpUseDSC },
                { ExpectedVB, AbstractEntityTypeWithoutKeyVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, AbstractEntityTypeWithoutKey.ExpectedResults, isCSharp, useDSC, "AbstractEntityTypeWithoutKey"),
        };
        #endregion
    }
}