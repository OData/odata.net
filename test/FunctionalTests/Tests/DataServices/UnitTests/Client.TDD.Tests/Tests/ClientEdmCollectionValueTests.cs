//---------------------------------------------------------------------
// <copyright file="ClientEdmCollectionValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Client;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientEdmCollectionValueTests
    {
        private IEdmPrimitiveTypeReference _intType;
        private ClientEdmCollectionValue _collection;
        private EdmIntegerConstant _value2;
        private EdmIntegerConstant _value1;

        [TestInitialize]
        public void Init()
        {
            this._intType = EdmCoreModel.Instance.GetInt32(false);
            this._value1 = new EdmIntegerConstant(1);
            this._value2 = new EdmIntegerConstant(2);
            this._collection = new ClientEdmCollectionValue(this._intType, new IEdmValue[] { this._value1, this._value2 });
        }

        [TestMethod]
        public void TypeShouldMatch()
        {
            this._collection.Type.Should().BeSameAs(this._intType);
        }

        [TestMethod]
        public void KindShouldMatch()
        {
            this._collection.ValueKind.Should().Be(EdmValueKind.Collection);
        }

        [TestMethod]
        public void ValuesShouldMatch()
        {
            this._collection.Elements.Select(v => v.Value).Should().ContainInOrder(new[] { this._value1, this._value2 });
        }
    }
}
