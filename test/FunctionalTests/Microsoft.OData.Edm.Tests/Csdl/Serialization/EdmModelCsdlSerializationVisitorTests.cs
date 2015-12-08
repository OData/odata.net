//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSerializationVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Serialization
{
    /// <summary>
    /// Unit tests of EdmModelCsdlSerializationVisitor. Aiming for whitebox coverage of these methods.
    /// </summary>
    public class EdmModelCsdlSerializationVisitorTests
    {
        private static readonly EdmEntityContainer defaultContainer = new EdmEntityContainer("Default.NameSpace", "Container");
        private static readonly EdmAction defaultCheckoutAction = new EdmAction("Default.NameSpace2", "CheckOut", null);
        private static readonly EdmFunction defaultGetStuffFunction = new EdmFunction("Default.NameSpace2", "GetStuff", EdmCoreModel.Instance.GetString(true));

        #region Test that action and function writes out Return types and parameters properly
        [Fact]
        public void VerifyPrimitiveCollectionReturnTypeDefinedInChildReturnTypeElement()
        {
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action), 
                @"<Action Name=""Checkout""><ReturnType Type=""Collection(Edm.String)"" /></Action>");
        }

        [Fact]
        public void VerifyNullableIsWrittenInChildReturnTypeElementForCollectionElementType()
        {
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: 10, isUnicode: false, isNullable: false)));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""Collection(Edm.String)"" Nullable=""false"" MaxLength=""10"" Unicode=""false"" /></Action>");
        }

        [Fact]
        public void VerifyPrimitiveReturnTypeDefinedInChildReturnTypeElement()
        {
            var action = new EdmAction("Default.Namespace", "Checkout", EdmCoreModel.Instance.GetString(false));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""Edm.String"" Nullable=""false"" /></Action>");
        }

        [Fact]
        public void VerifyEntityReturnTypeDefinedInChildReturnTypeElement()
        {
            var entityType = new EdmEntityType("NS.ds", "EntityType");
            var action = new EdmAction("Default.Namespace", "Checkout", new EdmEntityTypeReference(entityType, false));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><ReturnType Type=""NS.ds.EntityType"" Nullable=""false"" /></Action>");
        }

        [Fact]
        public void VerifyNoReturnTypeElementIsWrittenWhenNoReturnTypeIsProvided()
        {
            var action = new EdmAction("Default.Namespace", "Checkout", null);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action),
                @"<Action Name=""Checkout"" />");
        }

        [Fact]
        public void VerifyOperationParameterWritten()
        {
            var action = new EdmAction("Default.Namespace", "Checkout", null);
            action.AddParameter("firstParameter", EdmCoreModel.Instance.GetSingle(true));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(action),
                @"<Action Name=""Checkout""><Parameter Name=""firstParameter"" Type=""Edm.Single"" /></Action>");
        }

        // Note: Since the logic that writes out is identitical minus the call the WriteElement func which is tested elsewhere
        // only one test for Function is needed to ensure everything works.
        [Fact]
        public void FunctionShouldWriteOutCorrectly()
        {
            var function = new EdmFunction("Default.Namespace", "Checkout", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            VisitAndVerifyXml(
                (visitor) => visitor.VisitSchemaElement(function),
                @"<Function Name=""Checkout""><ReturnType Type=""Collection(Edm.String)"" /></Function>");
        }
        #endregion

        #region ActionImport tests.
        [Fact]
        public void VerifyActionImportWrittenCorrectly()
        {
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[]{ actionImport}),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithNoEntitySetPropertyOnlyWrittenOnce()
        {
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            VisitAndVerifyXml(
                (visitor) =>
                {
                    visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 });
                },
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithSameEntitySetOnlyWrittenOnce()
        {
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))));
            VisitAndVerifyXml(
                (visitor) =>
                {
                    visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 });
                },
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedActionImportsWithSameEdmPathOnlyWrittenOnce()
        {
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));
            VisitAndVerifyXml(
                (visitor) =>
                {
                    visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImport, actionImport2 });
                },
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />");
        }

        [Fact]
        public void VerifyIdenticalNamedActionImportsWithDifferentEntitySetPropertiesAreWritten()
        {
            var actionImportOnSet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))));
            var actionImportOnSet2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set2", new EdmEntityType("foo", "type"))));
            var actionImportWithNoEntitySet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImportWithUniqueEdmPath = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));
            VisitAndVerifyXml(
                (visitor) =>
                {
                    visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { actionImportOnSet, actionImportOnSet2, actionImportWithNoEntitySet, actionImportWithUniqueEdmPath });
                },
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" /><ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set2"" /><ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" /><ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />");
        }

        #endregion

        #region FunctionImport tests.
        [Fact]
        public void VerifyFunctionImportWrittenCorrectly()
        {
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithoutEntitySetValueOnlyWrittenOnce()
        {
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetValueOnlyWrittenOnce()
        {
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))), true);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" IncludeInServiceDocument=""true"" />");
        }

        [Fact]
        public void VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetPathValueOnlyWrittenOnce()
        {
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);
            VisitAndVerifyXml(
                (visitor) => visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImport, functionImport2 }),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" IncludeInServiceDocument=""true"" />");
        }

        [Fact]
        public void VerifyIdenticalNamedFunctionImportsWithDifferentEntitySetPropertiesAreWritten()
        {
            var functionImportOnSet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set", new EdmEntityType("foo", "type"))), false);
            var functionImportOnSet2 = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmEntitySetReferenceExpression(new EdmEntitySet(defaultContainer, "Set2", new EdmEntityType("foo", "type"))), false);
            var functionmportWithNoEntitySet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, null, false);
            var functionImportWithUniqueEdmPath = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), false);
            VisitAndVerifyXml(
                (visitor) =>
                {
                    visitor.VisitEntityContainerElements(new IEdmEntityContainerElement[] { functionImportOnSet, functionImportOnSet2, functionmportWithNoEntitySet, functionImportWithUniqueEdmPath });
                },
                @"<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" /><FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set2"" /><FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" /><FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" />");
        }

        #endregion
        internal void VisitAndVerifyXml(Action<EdmModelCsdlSerializationVisitor> testAction, string expected)
        {
            XmlWriter xmlWriter;
            MemoryStream memStream;
            IEdmModel model = new EdmModel();

            model.SetEdmxVersion(new Version(4, 0));
            Version edmxVersion = model.GetEdmxVersion();
            memStream = new MemoryStream();
            xmlWriter = XmlWriter.Create(memStream, new XmlWriterSettings(){ConformanceLevel = ConformanceLevel.Auto});
            var visitor = new EdmModelCsdlSerializationVisitor(model, xmlWriter, edmxVersion);

            testAction(visitor);
            xmlWriter.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memStream);
            
            // Remove extra xml header text as its not needed.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expected, result);
        }
    }
}
