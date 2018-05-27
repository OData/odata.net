﻿//---------------------------------------------------------------------
// <copyright file="PrimitiveValueMaterializationPolicyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client.Materialization
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DSClient = Microsoft.OData.Client;

    [TestClass]
    public class PrimitiveValueMaterializationPolicyTests
    {
        [TestMethod]
        public void StringValueShouldMaterializeCorrectly()
        {
            CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValue(typeof(string), "Edm.String", "foo").Should().Be("foo");
        }

        [TestMethod]
        public void NullStringValueShouldNotReturnOnMaterializing()
        {
            CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValueCollectionElement(typeof(string), "Edm.String", null).Should().Be(null);
        }

        [TestMethod]
        public void NullValueShouldReturnErrorNonNullableTypePrimitiveCollectionElement()
        {
            Action test = () => CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValueCollectionElement(typeof(int), "Edm.Int32", null);
            test.ShouldThrow<InvalidOperationException>().WithMessage(DSClient.Strings.Collection_NullCollectionItemsNotSupported);
        }

        [TestMethod]
        public void NullValueShouldReturnErrorNonNullableTypePrimitiveValue()
        {
            Action test = () => CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValue(typeof(int), "Edm.Int32", null);
            test.ShouldThrow<InvalidOperationException>().WithMessage("TODO: Is this reachable?");
        }

        [TestMethod]
        public void UnknownTypeShouldNotErrorOnMaterializingPrimitiveValue()
        {
            CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValueCollectionElement(typeof(UnknownPoint), "Edm.Int16", null).Should().Be(null);
        }

        [TestMethod]
        public void DateValueShouldMaterializeCorrectly()
        {
            CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValue(typeof(Date), "Edm.Date", "2014-09-28").Should().Be(new Date(2014, 9, 28));
        }

        [TestMethod]
        public void TimeOfDayValueShouldMaterializeCorrectly()
        {
            CreatePrimitiveValueMaterializationPolicy().MaterializePrimitiveDataValue(typeof(TimeOfDay), "Edm.TimeOfDay", "19:30:05.1230000").Should().Be(new TimeOfDay(19, 30, 5, 123));
        }

        internal PrimitiveValueMaterializationPolicy CreatePrimitiveValueMaterializationPolicy()
        {
            return new PrimitiveValueMaterializationPolicy(new TestMaterializerContext(), new SimpleLazy<PrimitivePropertyConverter>(() => new PrimitivePropertyConverter()));
        }

        public class UnknownPoint
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}