//---------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using FluentAssertions;
using Microsoft.CSharp;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.OData.Client.Design.T4.UnitTests
{
    [TestClass]
    public class ODataT4CodeGeneratorTests
    {
        private const bool CompileGeneratedCode = true;
        private static string AssemblyPath;
        private static string EdmxTestInputFile;
        private static string EdmxTestOutputFile;
        private static string MetadataUri;
        private static string T4TransformToolPath;
        private static string T4TemplatePath;
        private static string T4IncludeTemplatePath;

        [TestInitialize]
        public void Init()
        {
            AssemblyPath = Directory.GetCurrentDirectory();
            EdmxTestInputFile = AssemblyPath + "\\test_edmx" + ".edmx";
            EdmxTestOutputFile = AssemblyPath + "\\test_out" + ".output";
            MetadataUri = "File:\\" + EdmxTestInputFile;
            MetadataUri = Uri.EscapeDataString(MetadataUri);

            string commonProgramFiles = Environment.GetEnvironmentVariable("CommonProgramFiles");
            string T4TransformToolPathVer11 = commonProgramFiles + "\\Microsoft Shared\\TextTemplating\\11.0\\TextTransform.exe";
            string T4TransformToolPathVer12 = commonProgramFiles + "\\Microsoft Shared\\TextTemplating\\12.0\\TextTransform.exe";
            string T4TransformToolPathVer14 = commonProgramFiles + "\\Microsoft Shared\\TextTemplating\\14.0\\TextTransform.exe";

            if (File.Exists(T4TransformToolPathVer12))
            {
                T4TransformToolPath = T4TransformToolPathVer12;
            }
            else if (File.Exists(T4TransformToolPathVer14))
            {
                T4TransformToolPath = T4TransformToolPathVer14;
            }
            else
            {
                T4TransformToolPath = T4TransformToolPathVer11;
            }

            string T4TemplateName = "ODataT4CodeGenerator.tt";
            string T4IncludeTemplateName = "ODataT4CodeGenerator.ttinclude";
            T4TemplatePath = AssemblyPath + "\\ODataT4CodeGenerator.tt";
            T4IncludeTemplatePath = AssemblyPath + "\\ODataT4CodeGenerator.ttinclude";
            string ttSourceCode = string.Empty;
            string ttinlucdeSourceCode = string.Empty;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(T4TemplateName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ttSourceCode = reader.ReadToEnd();
                }
            }

            using (StreamWriter writer = new StreamWriter(T4TemplatePath))
            {
                writer.Write(ttSourceCode);
            }

            using (Stream stream = assembly.GetManifestResourceStream(T4IncludeTemplateName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ttinlucdeSourceCode = reader.ReadToEnd();
                }
            }

            using (StreamWriter writer = new StreamWriter(T4IncludeTemplatePath))
            {
                writer.Write(ttinlucdeSourceCode);
            }
        }

        [TestMethod]
        public void CodeGenSimpleEdmx()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, true, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, false, false);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, false/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, false, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenEntityHierarchyWithIDAndId()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.EntityHierarchyWithIDAndId.Metadata, null, true, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.EntityHierarchyWithIDAndId.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            // TODO: enable VB tests
            // Unlike C#, variable in VB is case insensitive. So Id and ID represent the same variable.
            // code = CodeGenWithT4Template(EntityClassCodeGeneratorTestDescriptors.EntityHierarchyWithIDAndId.Metadata, null, false, true);
            // EntityClassCodeGeneratorTestDescriptors.EntityHierarchyWithIDAndId.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGen_DSVGreaterThanMDSV()
        {
            var invalidEdmxDsvGreaterThanMdsv = @"<?xml version=""1.0"" standalone=""yes"" ?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" 
            xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""TestType"">
        <Key>
          <PropertyRef Name=""KeyProp"" />
        </Key>
        <Property Name=""KeyProp"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ValueProp"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
";
            try
            {
                CodeGenWithT4Template(invalidEdmxDsvGreaterThanMdsv, null, true, false);
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(ex.Message, "The value of the 'MaxDataServiceVersion' attribute must always be greater than or equal to the value of the 'OData-Version' attribute in the metadata document.");
            }
        }
        
        [TestMethod]
        public void CodeGenSetNamespacePrefixWithSingleNamespace()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Metadata, "NamespacePrefixWithSingleNamespace", true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Metadata, "NamespacePrefixWithSingleNamespace", false, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenSetNamespacePrefixWithDoubleNamespaces()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithDoubleNamespaces.Metadata, "Foo", true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithDoubleNamespaces.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithDoubleNamespaces.Metadata, "Foo", false, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithDoubleNamespaces.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenSetNamespacePrefixWithInheritence()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithInheritence.Metadata, "Foo", true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithInheritence.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithInheritence.Metadata, "Foo", false, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithInheritence.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void MergedFunctionalTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Metadata, null, true, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Metadata, null, false, false);
            ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Verify(code, false/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Metadata, null, false, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.MergedFunctionalTest.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithKeywordsAsNames()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.KeywordsAsNames.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.KeywordsAsNames.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.KeywordsAsNames.Metadata, null, false, false);
            ODataT4CodeGeneratorTestDescriptors.KeywordsAsNames.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithNamespaceInKeywords()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywords.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywords.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywords.Metadata, null, false, false);
            ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywords.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithNamespaceInKeywordsWithRefModel()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Metadata, null, true, false, true, getReferencedModelReaderFunc: ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.GetReferencedModelReaderFunc);
            ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Metadata, null, false, false, true, getReferencedModelReaderFunc: ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.GetReferencedModelReaderFunc);
            ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Verify(code, false/*isCSharp*/, false/*useDSC*/);

            //code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Metadata, null, true, true, false, false, ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.GetReferencedModelReaderFunc, true);
            //ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Verify(code, true /*isCSharp*/, true /*useDSC*/);

            //code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Metadata, null, false, true, false, false, ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.GetReferencedModelReaderFunc, true);
            //ODataT4CodeGeneratorTestDescriptors.NamespaceInKeywordsWithRefModel.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithMultiReferenceModel()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Metadata, null, true, false, getReferencedModelReaderFunc: ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.GetReferencedModelReaderFunc);
            ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Metadata, null, false, false, getReferencedModelReaderFunc: ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.GetReferencedModelReaderFunc);
            ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Verify(code, false/*isCSharp*/, false/*useDSC*/);

            //code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Metadata, null, true, true, false, false, ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.GetReferencedModelReaderFunc, true);
            //ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Verify(code, true /*isCSharp*/, true /*useDSC*/);

            //code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Metadata, null, false, true, false, false, ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.GetReferencedModelReaderFunc, true);
            //ODataT4CodeGeneratorTestDescriptors.MultiReferenceModel.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithUpperCamelCaseWithNamespacePrefix()
        {
            const string namespacePrefix = "namespacePrefix";
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithNamespacePrefix.Metadata, namespacePrefix, true, false, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithNamespacePrefix.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithNamespacePrefix.Metadata, namespacePrefix, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithNamespacePrefix.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithUpperCamelCaseWithoutNamespacePrefix()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Metadata, null, true, false, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Verify(code, true /*isCSharp*/, false /*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Metadata, null, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Verify(code, false/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Metadata, null, true, true, true, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Metadata, null, false, true, true, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.UpperCamelCaseWithoutNamespacePrefix.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenCommandlineSimpleEdmx()
        {
            string code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.Simple.Metadata, null, false, false);
            ODataT4CodeGeneratorTestDescriptors.Simple.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenCommandlineSetNamespacePrefixWithSingleNamespace()
        {
            string code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Metadata, "NamespacePrefixWithSingleNamespace", true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Metadata, "NamespacePrefixWithSingleNamespace", false, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixWithSingleNamespace.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenCommandlineSetNamespacePrefixRepeatWithSchemaNameSpace()
        {
            string code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Metadata, "NamespacePrefixRepeatWithSchemaNameSpace.Foo1.Foo11.Foo12", true, false);
            ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            //code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Metadata, "NamespacePrefixRepeatWithSchemaNameSpace.Foo1.Foo11.Foo12", true, true);
            //ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            //TODO: VB doesn't support duped segement in namespace.

            //code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Metadata, "Foo", false, true);
            //ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Verify(code, false/*isCSharp*/, true/*useDSC*/);

            //code = CodeGenWithT4TemplateFromCommandline(ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Metadata, "Foo", false, false);
            //ODataT4CodeGeneratorTestDescriptors.NamespacePrefixRepeatWithSchemaNameSpace.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        //TODO: Need To Confirm the behavior about Empty Schema
        [TestMethod]
        public void EmptySchema()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.EmptySchema.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.EmptySchema.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.EmptySchema.Metadata, null, false, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.EmptySchema.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenWithIgnoreUnexpectedElementsAndAttributes()
        {
            Action action = () => CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.IgnoreUnexpectedElementsAndAttributes.Metadata, null, true, false);
            action.ShouldThrow<InvalidOperationException>().WithMessage("The attribute 'FixLength' was not expected in the given context.");

            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.IgnoreUnexpectedElementsAndAttributes.Metadata, null, true, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.IgnoreUnexpectedElementsAndAttributes.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.IgnoreUnexpectedElementsAndAttributes.Metadata, null, false, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.IgnoreUnexpectedElementsAndAttributes.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenPrefixConflictTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.PrefixConflict.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.PrefixConflict.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.PrefixConflict.Metadata, null, false, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.PrefixConflict.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenDupNamesTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.DupNames.Metadata, null, true, false);
            ODataT4CodeGeneratorTestDescriptors.DupNames.Verify(code, true/*isCSharp*/, false/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.DupNames.Metadata, null, false, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.DupNames.Verify(code, false/*isCSharp*/, true/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenDupNamesWithCamelCaseTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.DupNamesWithCamelCase.Metadata, null, true, true, true, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.DupNamesWithCamelCase.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.DupNamesWithCamelCase.Metadata, null, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.DupNamesWithCamelCase.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenOverrideOperationsTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.OverrideOperations.Metadata, null, true, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.OverrideOperations.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.OverrideOperations.Metadata, null, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.OverrideOperations.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        [TestMethod]
        public void CodeGenAbstractEntityTypeWithoutKeyTest()
        {
            string code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.AbstractEntityTypeWithoutKey.Metadata, null, true, true, false, false, null, true);
            ODataT4CodeGeneratorTestDescriptors.AbstractEntityTypeWithoutKey.Verify(code, true/*isCSharp*/, true/*useDSC*/);

            code = CodeGenWithT4Template(ODataT4CodeGeneratorTestDescriptors.AbstractEntityTypeWithoutKey.Metadata, null, false, false, true);
            ODataT4CodeGeneratorTestDescriptors.AbstractEntityTypeWithoutKey.Verify(code, false/*isCSharp*/, false/*useDSC*/);
        }

        private static string CodeGenWithT4Template(string edmx, string namespacePrefix, bool isCSharp, bool useDataServiceCollection, bool enableNamingAlias = false, bool ignoreUnexpectedElementsAndAttributes = false, Func<Uri, XmlReader> getReferencedModelReaderFunc = null, bool appendDSCSuffix = false)
        {
            if (useDataServiceCollection
                && appendDSCSuffix) // hack now
            {
                var pattern = "<Schema (.*)Namespace=\"(.*?)\"";

                MatchCollection matches = Regex.Matches(edmx, pattern);
                foreach (Match match in matches)
                {
                    edmx = edmx.Replace(match.Groups[2].Value + ".", match.Groups[2].Value + ".DSC.");
                }

                edmx = Regex.Replace(edmx, pattern, "<Schema $1Namespace=\"$2.DSC\"", RegexOptions.Multiline);
            }

            ODataT4CodeGenerator t4CodeGenerator = new ODataT4CodeGenerator
            {
                Edmx = edmx,
                GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
                NamespacePrefix = namespacePrefix,
                TargetLanguage = isCSharp ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB,
                EnableNamingAlias = enableNamingAlias,
                IgnoreUnexpectedElementsAndAttributes = ignoreUnexpectedElementsAndAttributes
            };

            if (useDataServiceCollection)
            {
                t4CodeGenerator.UseDataServiceCollection = true;
            }

            string code = t4CodeGenerator.TransformText();

            if (CompileGeneratedCode)
            {
                // Comment next line to not to verify that the generated code can be compiled successfully
                GeneratedCodeShouldCompile(code, isCSharp);
            }

            return code;
        }

        private static string CodeGenWithT4TemplateFromCommandline(string edmx, string namespacePrefix, bool isCSharp, bool useDataServiceCollection)
        {
            File.WriteAllText(EdmxTestInputFile, edmx);

            ODataT4CodeGenerator.LanguageOption option;
            if (isCSharp)
            {
                option = ODataT4CodeGenerator.LanguageOption.CSharp;
            }
            else
            {
                option = ODataT4CodeGenerator.LanguageOption.VB;
            }

            string arguments = "-out \"" + EdmxTestOutputFile
                + "\" -a !!MetadataDocumentUri!" + MetadataUri
                + " -a !!UseDataServiceCollection!" + useDataServiceCollection.ToString()
                + " -a !!TargetLanguage!" + option.ToString()
                + (namespacePrefix == null ? string.Empty : (" -a !!NamespacePrefix!" + namespacePrefix))
                + " -p \"" + AssemblyPath + "\" \"" + T4TemplatePath + "\"";

            var returnValue = Execute(T4TransformToolPath, arguments, null);

            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue += arguments;
            }

            returnValue.Should().BeEmpty();

            File.Exists(EdmxTestOutputFile).Should().BeTrue();
            string code = File.ReadAllText(EdmxTestOutputFile);

            if (CompileGeneratedCode)
            {
                GeneratedCodeShouldCompile(code, isCSharp);
            }

            return code;
        }

        public static string Execute(string filename, string arguments, int? expectedExitCode)
        {
            Assert.IsNotNull(filename, "null filename");
            Assert.IsTrue(File.Exists(filename) && !Directory.Exists(filename), "missing file: {0}", filename);

            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (expectedExitCode.HasValue)
            {
                Assert.AreEqual(expectedExitCode.Value, process.ExitCode, "ExitCode for {0}", filename);
            }

            return output + error;
        }

        private static void GeneratedCodeShouldCompile(string source, bool isCSharp)
        {
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
                    typeof(ODataVersion).Assembly.Location,
                    DataFxAssemblyRef.File.System,
                    DataFxAssemblyRef.File.SystemCore,
                    DataFxAssemblyRef.File.SystemIO,
                    DataFxAssemblyRef.File.SystemRuntime,
                    DataFxAssemblyRef.File.SystemXml,
                    DataFxAssemblyRef.File.SystemXmlReaderWriter
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
            results.Errors.Should().BeEmpty();
        }
    }
}
