//---------------------------------------------------------------------
// <copyright file="MaterializerEntryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ClientStrings = Microsoft.OData.Client.Strings;

    /// <summary>
    /// TODO: test the rest of the functionality in <see cref="MaterializerEntry"/>.
    /// </summary>
    [TestClass]
    public class MaterializerEntryTests
    {
        private const string ExpectedTypeName = "Fake.NS.Type";
        private readonly ClientEdmModel clientModel = new ClientEdmModel(ODataProtocolVersion.V4);

        [TestMethod]
        public void ServerTypeNameShouldFromEntryInJsonIfNoAnnotationIsPresent()
        {
            var testSubject = this.CreateMaterializerEntry(ODataFormat.Json, e => e.TypeName = ExpectedTypeName);
            testSubject.EntityDescriptor.ServerTypeName.Should().Be(ExpectedTypeName);
        }

        [TestMethod]
        public void ServerTypeNameShouldFromEntryInJsonIfAnnotationValueIsNull()
        {
            var testSubject = this.CreateMaterializerEntry(
               ODataFormat.Json,
               e =>
               {
                   e.TypeName = ExpectedTypeName;
                   e.TypeAnnotation = new ODataTypeAnnotation();
               });
            testSubject.EntityDescriptor.ServerTypeName.Should().Be(ExpectedTypeName);
        }

        [TestMethod]
        public void ServerTypeNameShouldFromAnnotationInJsonIfAnnotationValueIsNotNull()
        {
            var testSubject = this.CreateMaterializerEntry(
               ODataFormat.Json,
               e =>
               {
                   e.TypeName = "foo";
                   e.TypeAnnotation = new ODataTypeAnnotation(ExpectedTypeName);
               });
            testSubject.EntityDescriptor.ServerTypeName.Should().Be(ExpectedTypeName);
        }

        private MaterializerEntry CreateMaterializerEntry(ODataFormat format, Action<ODataResource> modifyEntry = null)
        {
            var entry = new ODataResource();
            if (modifyEntry != null)
            {
                modifyEntry(entry);
            }

            return MaterializerEntry.CreateEntry(entry, format, true, this.clientModel);
        }
    }
}