//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriterTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Serialization
{
    public partial class EdmModelCsdlSchemaWriterTests
    {
        #region Action/Function element attribute writer tests.
        [Fact]
        public async Task BoundOperationShouldWriteIsBoundEqualTrueAttribute_Async()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteActionElementHeaderMethodWithAsync(action, @"<Action Name=""Checkout"" IsBound=""true""").ConfigureAwait(false);
        }

        [Fact]
        public async Task NonBoundOperationShouldNotWriteIsBoundAttribute_Async()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteFunctionElementHeaderMethodWithAsync(function, @"<Function Name=""Checkout""").ConfigureAwait(false);
        }

        [Fact]
        public async Task BoundOperationWithEntitySetPathShouldWriteEntitySetPathAttributeWithCorrectValue_Async()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, new EdmPathExpression("Customer", "Orders") /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteActionElementHeaderMethodWithAsync(action, @"<Action Name=""Checkout"" IsBound=""true"" EntitySetPath=""Customer/Orders""").ConfigureAwait(false);
        }

        [Fact]
        public async Task ComposableFunctionShouldWriteIsComposableEqualTrue_Async()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, true /*isComposable*/);
            await this.TestWriteFunctionElementHeaderMethodWithAsync(function, @"<Function Name=""Checkout"" IsComposable=""true""").ConfigureAwait(false);
        }

        private async Task TestWriteActionElementHeaderMethodWithAsync(EdmAction action, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteActionElementHeaderAsync(action).ConfigureAwait(false), expected).ConfigureAwait(false);
        }

        private async Task TestWriteFunctionElementHeaderMethodWithAsync(EdmFunction function, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteFunctionElementHeaderAsync(function).ConfigureAwait(false), expected).ConfigureAwait(false);
        }
        #endregion

        #region OperationImport tests.
        [Fact]
        public async Task ValidateEntitySetAtttributeCorrectlyWritesOutEntitySet_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Customers"));

            await this.EdmModelCsdlSchemaWriterTestAsync(
                async (writer) => await writer.WriteActionImportElementHeaderAsync(actionImport).ConfigureAwait(false),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Customers""").ConfigureAwait(false);
        }

        [Fact]
        public async Task ValidateEntitySetAtttributeCorrectlyWritesEntitySetPath_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            await this.EdmModelCsdlSchemaWriterTestAsync(
                async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""").ConfigureAwait(false);
        }

        [Fact]
        public async Task ValidateIncorrectEdmExpressionThrows_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(true), 1));

            var csdlSchemaWriter = CreateEdmModelCsdlSchemaWriterForErrorTestForAsync();
            async Task errorTest() => await csdlSchemaWriter.WriteActionImportElementHeaderAsync(actionImport).ConfigureAwait(false);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(errorTest).ConfigureAwait(false);
            Assert.Equal(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(actionImport.Name), exception.Message);
        }
        #endregion

        #region ActionImport tests.
        [Fact]
        public async Task ValidateCorrectActionImportNameAndActionAttributeValueWrittenCorrectly_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            await this.EdmModelCsdlSchemaWriterTestAsync(
                async (writer) => await writer.WriteActionImportElementHeaderAsync(actionImport).ConfigureAwait(false),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut""").ConfigureAwait(false);
        }
        #endregion

        #region FunctionImport tests.
        [Fact]
        public async Task ValidateCorrectFunctionNameAndFunctionAttributeValueWrittenCorrectly_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            await this.EdmModelCsdlSchemaWriterTestAsync(
                async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""").ConfigureAwait(false);
        }

        [Fact]
        public async Task ValidateFunctionIncludeInServiceDocumentWrittenAsTrue_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), true);

            await this.EdmModelCsdlSchemaWriterTestAsync(
                async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders"" IncludeInServiceDocument=""true""").ConfigureAwait(false);
        }
        #endregion

        #region Open ComplexType tests.
        [Fact]
        public async Task ShouldWriteOpenTypeAttributeForOpenComplexType_Async()
        {
            IEdmComplexType complexType = new EdmComplexType("Default.NameSpace2", "OpenComplex", null, false, true);
            await TestWriteComplexTypeElementHeaderMethodWithAsync(complexType, @"<ComplexType Name=""OpenComplex"" OpenType=""true""").ConfigureAwait(false);
        }

        private async Task TestWriteComplexTypeElementHeaderMethodWithAsync(IEdmComplexType complexType, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteComplexTypeElementHeaderAsync(complexType).ConfigureAwait(false), expected).ConfigureAwait(false);
        }

        internal async Task EdmModelCsdlSchemaWriterTestAsync(Func<EdmModelCsdlSchemaWriter, Task> testAction, string expectedPayload)
        {
            XmlWriter xmlWriter;
            MemoryStream memoryStream;
            EdmModelCsdlSchemaWriter schemaWriter = CreateEdmModelCsdlSchemaWriterForAsync(out xmlWriter, out memoryStream);

            await testAction(schemaWriter).ConfigureAwait(false);

            await xmlWriter.FlushAsync().ConfigureAwait(false);

            memoryStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memoryStream);

            // Removing xml header to make the baseline's more compact and focused on the test at hand.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expectedPayload, result);
        }

        private static EdmModelCsdlSchemaWriter CreateEdmModelCsdlSchemaWriterForErrorTestForAsync()
        {
            XmlWriter writer = null;
            MemoryStream memoryStream = null;

            return CreateEdmModelCsdlSchemaWriterForAsync(out writer, out memoryStream);
        }

        private static EdmModelCsdlSchemaWriter CreateEdmModelCsdlSchemaWriterForAsync(out XmlWriter xmlWriter, out MemoryStream memoryStream)
        {
            memoryStream = new MemoryStream();
            IEdmModel model = new EdmModel();
            model.SetEdmxVersion(new Version(4, 0));
            var namespaceAliasMappings = model.GetNamespaceAliases();
            Version edmxVersion = model.GetEdmxVersion();
            xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings() { Async = true });
            return new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion, new CsdlXmlWriterSettings());
        }
        #endregion
    }
}
