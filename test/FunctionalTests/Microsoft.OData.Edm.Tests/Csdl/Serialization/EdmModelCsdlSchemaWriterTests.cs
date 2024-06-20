//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriterTests.cs" company="Microsoft">
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
    /// <summary>
    ///Tests EdmModelCsdlSchemaWriter functionalities
    ///</summary>
    public class EdmModelCsdlSchemaWriterTests
    {
        private static readonly EdmEntityContainer defaultContainer = new EdmEntityContainer("Default.NameSpace", "Container");
        private static readonly EdmAction defaultCheckoutAction = new EdmAction("Default.NameSpace2", "CheckOut", null);
        private static readonly EdmFunction defaultGetStuffFunction = new EdmFunction("Default.NameSpace2", "GetStuff", EdmCoreModel.Instance.GetString(true));

        #region Action/Function element attribute writer tests.
        [Fact]
        public void BoundOperationShouldWriteIsBoundEqualTrueAttribute()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            this.TestWriteActionElementHeaderMethod(action, @"<Action Name=""Checkout"" IsBound=""true""");
        }

        [Fact]
        public async Task BoundOperationShouldWriteIsBoundEqualTrueAttribute_Async()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteActionElementHeaderMethodWithAsync(action, @"<Action Name=""Checkout"" IsBound=""true""");
        }

        [Fact]
        public void NonBoundOperationShouldNotWriteIsBoundAttribute()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            this.TestWriteFunctionElementHeaderMethod(function, @"<Function Name=""Checkout""");
        }

        [Fact]
        public async Task NonBoundOperationShouldNotWriteIsBoundAttribute_Async()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteFunctionElementHeaderMethodWithAsync(function, @"<Function Name=""Checkout""");
        }

        [Fact]
        public void BoundOperationWithEntitySetPathShouldWriteEntitySetPathAttributeWithCorrectValue()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, new EdmPathExpression("Customer", "Orders") /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            this.TestWriteActionElementHeaderMethod(action, @"<Action Name=""Checkout"" IsBound=""true"" EntitySetPath=""Customer/Orders""");
        }

        [Fact]
        public async Task BoundOperationWithEntitySetPathShouldWriteEntitySetPathAttributeWithCorrectValue_Async()
        {
            EdmAction action = new EdmAction("Default.Namespace", "Checkout", null /*returnType*/, true /*isBound*/, new EdmPathExpression("Customer", "Orders") /*entitySetPath*/);
            action.AddParameter("param", EdmCoreModel.Instance.GetString(true));
            await this.TestWriteActionElementHeaderMethodWithAsync(action, @"<Action Name=""Checkout"" IsBound=""true"" EntitySetPath=""Customer/Orders""");
        }

        [Fact]
        public void ComposableFunctionShouldWriteIsComposableEqualTrue()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, true /*isComposable*/);
            this.TestWriteFunctionElementHeaderMethod(function, @"<Function Name=""Checkout"" IsComposable=""true""");
        }

        [Fact]
        public async Task ComposableFunctionShouldWriteIsComposableEqualTrue_Async()
        {
            EdmFunction function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(true) /*returnType*/, false /*isBound*/, null /*entitySetPath*/, true /*isComposable*/);
            await this.TestWriteFunctionElementHeaderMethodWithAsync(function, @"<Function Name=""Checkout"" IsComposable=""true""");
        }

        private void TestWriteActionElementHeaderMethod(EdmAction action, string expected)
        {
            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteActionElementHeader(action), expected);
        }

        private async Task TestWriteActionElementHeaderMethodWithAsync(EdmAction action, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteActionElementHeaderAsync(action), expected);
        }

        private void TestWriteFunctionElementHeaderMethod(EdmFunction function, string expected)
        {
            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteFunctionElementHeader(function), expected);
        }

        private async Task TestWriteFunctionElementHeaderMethodWithAsync(EdmFunction function, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteFunctionElementHeaderAsync(function), expected);
        }
        #endregion

        #region OperationImport tests.
        [Fact]
        public void ValidateEntitySetAtttributeCorrectlyWritesOutEntitySet()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Customers"));

            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteActionImportElementHeader(actionImport),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Customers""");
        }

        [Fact]
        public async Task ValidateEntitySetAtttributeCorrectlyWritesOutEntitySet_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Customers"));

            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteActionImportElementHeaderAsync(actionImport),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Customers""");
        }

        [Fact]
        public void ValidateEntitySetAtttributeCorrectlyWritesEntitySetPath()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteFunctionImportElementHeader(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""");
        }

        [Fact]
        public async Task ValidateEntitySetAtttributeCorrectlyWritesEntitySetPath_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""");
        }

        [Fact]
        public void ValidateIncorrectEdmExpressionThrows()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(true), 1));

            Action errorTest = () => CreateEdmModelCsdlSchemaWriterForErrorTest().WriteActionImportElementHeader(actionImport);

            var exception = Assert.Throws<InvalidOperationException>(errorTest);
            Assert.Equal(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(actionImport.Name), exception.Message);
        }

        [Fact]
        public async Task ValidateIncorrectEdmExpressionThrows_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(true), 1));
            
            var csdlSchemaWriter = CreateEdmModelCsdlSchemaWriterForErrorTest();
            async Task errorTest() => await csdlSchemaWriter.WriteActionImportElementHeaderAsync(actionImport);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(errorTest);
            Assert.Equal(Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(actionImport.Name), exception.Message);
        }

        #endregion

        #region ActionImport tests.
        [Fact]
        public void ValidateCorrectActionImportNameAndActionAttributeValueWrittenCorrectly()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteActionImportElementHeader(actionImport),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut""");
        }

        [Fact]
        public async Task ValidateCorrectActionImportNameAndActionAttributeValueWrittenCorrectly_Async()
        {
            EdmActionImport actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteActionImportElementHeaderAsync(actionImport),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut""");
        }

        #endregion

        #region FunctionImport tests.
        [Fact]
        public void ValidateCorrectFunctionNameAndFunctionAttributeValueWrittenCorrectly()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteFunctionImportElementHeader(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""");
        }

        [Fact]
        public async Task ValidateCorrectFunctionNameAndFunctionAttributeValueWrittenCorrectly_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), false);

            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders""");
        }

        [Fact]
        public void ValidateFunctionIncludeInServiceDocumentWrittenAsTrue()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), true);

            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteFunctionImportElementHeader(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders"" IncludeInServiceDocument=""true""");
        }

        [Fact]
        public async Task ValidateFunctionIncludeInServiceDocumentWrittenAsTrue_Async()
        {
            EdmFunctionImport functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Customers", "Orders"), true);

            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteFunctionImportElementHeaderAsync(functionImport),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Customers/Orders"" IncludeInServiceDocument=""true""");
        }
        #endregion

        #region Open ComplexType tests.
        [Fact]
        public void ShouldWriteOpenTypeAttributeForOpenComplexType()
        {
            IEdmComplexType complexType = new EdmComplexType("Default.NameSpace2", "OpenComplex", null, false, true);
            TestWriteComplexTypeElementHeaderMethod(complexType, @"<ComplexType Name=""OpenComplex"" OpenType=""true""");
        }

        [Fact]
        public async Task ShouldWriteOpenTypeAttributeForOpenComplexType_Async()
        {
            IEdmComplexType complexType = new EdmComplexType("Default.NameSpace2", "OpenComplex", null, false, true);
            await TestWriteComplexTypeElementHeaderMethodWithAsync(complexType, @"<ComplexType Name=""OpenComplex"" OpenType=""true""");
        }

        private void TestWriteComplexTypeElementHeaderMethod(IEdmComplexType complexType, string expected)
        {
            this.EdmModelCsdlSchemaWriterTest(writer => writer.WriteComplexTypeElementHeader(complexType), expected);
        }

        private async Task TestWriteComplexTypeElementHeaderMethodWithAsync(IEdmComplexType complexType, string expected)
        {
            await this.EdmModelCsdlSchemaWriterTestAsync(async (writer) => await writer.WriteComplexTypeElementHeaderAsync(complexType), expected);
        }
        #endregion

        internal void EdmModelCsdlSchemaWriterTest(Action<EdmModelCsdlSchemaWriter> testAction, string expectedPayload)
        {
            XmlWriter xmlWriter;
            MemoryStream memoryStream;
            EdmModelCsdlSchemaWriter schemaWriter = CreateEdmModelCsdlSchemaWriter(out xmlWriter, out memoryStream);

            testAction(schemaWriter);

            xmlWriter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memoryStream);

            // Removing xml header to make the baseline's more compact and focused on the test at hand.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expectedPayload, result);
        }

        internal async Task EdmModelCsdlSchemaWriterTestAsync(Func<EdmModelCsdlSchemaWriter, Task> testAction, string expectedPayload)
        {
            XmlWriter xmlWriter;
            MemoryStream memoryStream;
            EdmModelCsdlSchemaWriter schemaWriter = CreateEdmModelCsdlSchemaWriter(out xmlWriter, out memoryStream);

            await testAction(schemaWriter);

            await xmlWriter.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memoryStream);

            // Removing xml header to make the baseline's more compact and focused on the test at hand.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expectedPayload, result);
        }

        private static EdmModelCsdlSchemaWriter CreateEdmModelCsdlSchemaWriterForErrorTest()
        {
            XmlWriter writer = null;
            MemoryStream memoryStream = null;

            return CreateEdmModelCsdlSchemaWriter(out writer, out memoryStream);
        }

        private static EdmModelCsdlSchemaWriter CreateEdmModelCsdlSchemaWriter(out XmlWriter xmlWriter, out MemoryStream memoryStream)
        {
            memoryStream = new MemoryStream();
            IEdmModel model = new EdmModel();
            model.SetEdmxVersion(new Version(4, 0));
            var namespaceAliasMappings = model.GetNamespaceAliases();
            Version edmxVersion = model.GetEdmxVersion();
            xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings() { Async = true });
            return new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion, new CsdlXmlWriterSettings());
        }
    }
}
