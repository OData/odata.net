//---------------------------------------------------------------------
// <copyright file="LinqIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using AstoriaUnitTests.TDD.Tests.Client;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Xunit;

    /// <summary>
    /// Test class that contains tests for LINQ generated queries
    /// </summary>
    /// <remarks>
    /// LightSwitch:  Cannot navigate relationships belonging to entity with multi-segment key
    /// </remarks>
    public class LinqIntegrationTests
    {
        [Fact]
        public void AddQueryOptionForFormatShouldThrow()
        {
            DataServiceContext ctx = new DataServiceContext(new Uri("http://myservice/", UriKind.Absolute));
            ctx.CreateQuery<Customer>("Customers").AddQueryOption("$format", "atom").ToString().Should()
                .Be("Error translating Linq expression to URI: The '$format' query option is not supported. Use the DataServiceContext.Format property to set the desired format.");
        }

        readonly string rootUrl = "http://www.odata.com/service.svc/";
        DataServiceContext context;
        string query;

        private DateTimeOffset sampleDateTimeOffset;
        private string sampleDate;

        public LinqIntegrationTests()
        {
            context = new DataServiceContext(new Uri(rootUrl));

            this.sampleDateTimeOffset = new DateTime(2012, 12, 17, 9, 23, 31, DateTimeKind.Utc);
            this.sampleDate = XmlConvert.ToString(this.sampleDateTimeOffset);
        }

        #region Unit tests for enity with 1 key

        [Key("ID")]
        public class EntityWithOneKey
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int AnotherProperty { get; set; }
        }

        [Key("ID1")]
        public class AnotherEntityWithOneKey
        {
            public int ID1 { get; set; }
            public string Name { get; set; }
            public int AnotherProperty { get; set; }
        }

        [Fact]
        public void WhereClauseWithEqualShouldReturnUrlForSingleton()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test").Where(p => p.ID == 1).ToString();

            Assert.Equal(rootUrl + "Test(1)", query);
        }

        [Fact]
        public void WhereClauseWithNotEqualShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .Where(p => p.ID != 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID ne 1", query);
        }

        [Fact]
        public void WhereClauseWithGreaterThanShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .Where(p => p.ID > 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID gt 1", query);
        }

        [Fact]
        public void WhereClauseWithGreaterThanOrEqualToShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .Where(p => p.ID >= 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID ge 1", query);
        }

        [Fact]
        public void WhereClauseWithLessThanShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .Where(p => p.ID < 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID lt 1", query);
        }

        [Fact]
        public void WhereClauseWithLessThanOrEqualToShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .Where(p => p.ID <= 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID le 1", query);
        }

        [Fact]
        public void NonFilterQueryOptionBeforeKeyPredicateShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithOneKey>("Test")
                .OrderBy(p => p.ID)
                .Where(p => p.ID == 1)
                .ToString();

            Assert.Equal(rootUrl + "Test?$orderby=ID&$filter=ID eq 1", query);
        }
        #endregion Unit tests for enity with 1 key

        #region Unit tests for entity with 2 keys

        [Key("ID1", "ID2")]
        internal class EntityWithTwoKeyProperties
        {
            public int ID1 { get; set; }
            public int ID2 { get; set; }
            public string Name { get; set; }
            public AnotherEntityWithTwoKeyProperties NavProperty { get; set; }
            public List<AnotherEntityWithTwoKeyProperties> NavPropertyCollection { get; set; }
        }

        [Key("Key1", "Key2")]
        internal class AnotherEntityWithTwoKeyProperties
        {
            public int Key1 { get; set; }
            public int Key2 { get; set; }
            public string NonKeyProperty { get; set; }
        }

        internal class DerivedEntityWithTwoKeyProperties : EntityWithTwoKeyProperties
        {
            public string AdditionalStringProperty { get; set; }
            public int AdditionalIntProperty { get; set; }
            public AnotherEntityWithOneKey AnotherEntityWithOneKey { get; set; }
        }

        [Fact]
        public void TwoWhereClausesForKeysShouldReturnUrlForSingleton()
        {
            // Simple case, only two key properties provided
            string query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                .Where(p => p.ID1 == 1)
                .Where(p => p.ID2 == 2)
                .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)", query);
        }

        [Fact]
        public void TwoWhereClausesForKeysInDifferentOrderShouldReturnUrlForSingltonInCorrectOrder()
        {
            // Key properties provided in reverse order
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                            .Where(p => p.ID2 == 1).Where(p => p.ID1 == 2)
                            .ToString();
            Assert.Equal(rootUrl + "Test(ID2=1,ID1=2)", query);
        }

        [Fact]
        public void OneWhereClauseForNonKeyShouldReturnUrlForFilter()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                           .Where(p => p.Name == "foo")
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=Name eq 'foo'", query);
        }

        [Fact]
        public void OneWhereClauseForKeysShouldReturnUrlForSingleton()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.ID2 == 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)", query);
        }

        [Fact]
        public void OneWhereClauseForKeysInDerviedClassShouldReturnUrlForSingleton()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<DerivedEntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.ID2 == 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)", query);
        }


        [Fact]
        public void SameKeyProvidedTwiceInOneWhereClauseShouldReturnError()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<DerivedEntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.ID1 == 2)
                           .ToString();
            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_CanOnlyApplyOneKeyPredicate), query);
        }

        [Fact]
        public void MultiWhereClauseForKeysInDerviedClassShouldReturnUrlForSingleton()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<DerivedEntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1)
                           .Where(p => p.ID2 == 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)", query);
        }

        [Fact]
        public void OneWhereClauseForParatialKeyAndNavigationPropertyKeyInDerviedClassShouldReturnUrlForFilter()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<DerivedEntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.NavProperty.Key1 == 3)
                           .ToString();
            Assert.Equal(rootUrl + @"Test?$filter=ID1 eq 1 and NavProperty/Key1 eq 3", query);
        }

        [Fact]
        public void OneWhereClauseForPartialKeyAndNavigationPropertySameKeyNameInDerviedClassShouldReturnUrlForFilter()
        {
            // Provide both key properites in single where clause
            query = context.CreateQuery<DerivedEntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID2 == 1 && p.AnotherEntityWithOneKey.ID1 == 3)
                           .ToString();
            Assert.Equal(rootUrl + @"Test?$filter=ID2 eq 1 and AnotherEntityWithOneKey/ID1 eq 3", query);
        }

        [Fact]
        public void OneWhereClausesForOneKeyShouldReturnUrlWithFilter()
        {
            // Provide only one key proprty
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID2 == 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID2 eq 2", query);
        }

        [Fact]
        public void OneWhereClausesForOrOperationShouldReturnUrlWithFilter()
        {
            // Provide only one key proprty
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 || p.ID2 == 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 or ID2 eq 2", query);
        }

        [Fact]
        public void OneWhereClausesForNotEqualsShouldReturnUrlWithFilter()
        {
            // Provide only one key proprty
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.ID2 != 2)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 ne 2", query);
        }

        [Fact]
        public void OneKeyAndOneNonKeyShouldReturnUrlWithFilter()
        {
            // Provide one key property and one non-key property
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                            .Where(p => p.ID1 == 1).Where(p => p.Name == "foo")
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and Name eq 'foo'", query);
        }

        [Fact]
        public void WhereClausesForKeysAndNonKeysShouldReturnUrlWithFilter()
        {
            // provide all key properties and non-key properties
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.Name == "foo")
                            .Where(p => p.ID2 == 2)
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and Name eq 'foo' and ID2 eq 2", query);
        }

        [Fact]
        public void WhereClausesForKeysAndNonKeysShouldReturnUrlWithFilterInCorrecTorder()
        {
            // provide all key properties and non-key properties
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.ID2 == 2)
                            .Where(p => p.Name == "foo")
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 eq 2 and Name eq 'foo'", query);
        }

        [Fact]
        public void TwoWhereClausesForKeysAndSelectForNavEntityShouldReturnUrlForNavProperty()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1).Where(p => p.ID2 == 2)
                                  .Select(p => p.NavProperty)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)/NavProperty", query);
        }

        [Fact]
        public void WhereClausesForKeysAndNonKeysAndSelectForNavPropertyShouldReturnError()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1).Where(p => p.Name == "foo").Where(p => p.ID2 == 2)
                                  .Select(p => p.NavProperty)
                                  .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionsOnlyAllowedOnLeafNodes), query);
        }

        [Fact]
        public void WhereClausesForKeysForEntityAndSelectForNavPropertyWithWhereClauseShouldReturnError()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1).Where(p => p.ID2 == 2)
                                  .Select(p => p.NavProperty)
                                  .Where(p => p.NonKeyProperty == "bar")
                                  .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionsOnlyAllowedOnSingletons), query);
        }

        [Fact]
        public void WhereClausesForKeysOfEntityAndNavPropertyShouldReturnUrlWithNavPropertySingleton()
        {
            // Provide all key properties for entity as well as navigational entity
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1).Where(p => p.ID2 == 2)
                                  .SelectMany(p => p.NavPropertyCollection)
                                  .Where(n => n.Key1 == 3).Where(n => n.Key2 == 4)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)/NavPropertyCollection(Key1=3,Key2=4)", query);
        }

        [Fact]
        public void WhereClausesForKeysOfEntityAndWhereClausesForPartialKeysOfNavPropertyShouldReturnUrlWithFilter()
        {
            // Provide all key properties for entity and partial key property for navigational entity
            query = context.CreateQuery<EntityWithTwoKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1).Where(p => p.ID2 == 2)
                                  .SelectMany(p => p.NavPropertyCollection)
                                  .Where(n => n.Key1 == 3)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2)/NavPropertyCollection?$filter=Key1 eq 3", query);
        }

        #endregion Unit tests for entity with 2 keys

        #region Unit tests for entity with 3 keys

        [Key("ID1", "ID2", "ID3")]
        internal class EntityWithThreeKeyProperties
        {
            public int ID1 { get; set; }
            public string ID2 { get; set; }
            public DateTimeOffset ID3 { get; set; }
            public string Name { get; set; }
            public AnotherEntityWithThreeKeyProperties NavProperty { get; set; }
            public List<AnotherEntityWithThreeKeyProperties> NavPropertyCollection { get; set; }
        }

        [Key("Key1", "Key2", "Key3")]
        internal class AnotherEntityWithThreeKeyProperties
        {
            public int Key1 { get; set; }
            public int Key2 { get; set; }
            public int Key3 { get; set; }
            public string NonKeyProperty { get; set; }
        }

        [Fact]
        public void ThreeWhereClausesForKeysShouldReturnUrlForSingleton()
        {
            // Simple case, only two key properties provided
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Where(p => p.ID1 == 1)
                .Where(p => p.ID2 == "foo")
                .Where(p => p.ID3 == this.sampleDateTimeOffset)
                .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")", query);
        }

        [Fact]
        public void ThreeWhereClausesForKeysInDifferentOrderShouldReturnUrlForSingltonInCorrectOrder()
        {
            // Key properties provided in reverse order
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID2 == "bar")
                            .Where(p => p.ID1 == 2)
                            .Where(p => p.ID3 == this.sampleDateTimeOffset)
                            .ToString();
            Assert.Equal(rootUrl + "Test(ID2='bar',ID1=2,ID3=" + this.sampleDate + ")", query);
        }

        [Fact]
        public void WhereClausesWithDuplicateKeysShouldReturnError()
        {
            // Add duplicate key property
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID2 == "bar")
                            .Where(p => p.ID1 == 2)
                            .Where(p => p.ID3 == this.sampleDateTimeOffset)
                            .Where(p => p.ID1 == 2)
                            .ToString();
            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_CanOnlyApplyOneKeyPredicate), query);
        }

        [Fact]
        public void OneWhereClauseForThreeKeysShouldReturnUrlForSingleton()
        {
            // Provide all key properites in single where clause
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && p.ID2 == "foo" && p.ID3 == this.sampleDateTimeOffset)
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")", query);
        }

        [Fact]
        public void OneWhereClauseForThreeKeysWithGroupingsShouldReturnUrlForSingleton()
        {
            // Provide all key properites in single where clause, but use parantheses for few of them
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 && (p.ID2 == "foo" && p.ID3 == this.sampleDateTimeOffset))
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")", query);
        }

        [Fact]
        public void OneWhereClauseForThreeKeysWithDifferntGroupingsShouldReturnUrlForSingleton()
        {
            // Provide all key properites in single where clause, but use parantheses for few of them
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => (p.ID1 == 1 && p.ID2 == "foo") && p.ID3 == this.sampleDateTimeOffset)
                           .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")", query);
        }

        [Fact]
        public void OneWhereClauseWithOrForThreeKeysShouldReturnUrlWithFilter()
        {
            // Provide all key properites in single where clause, with OR operator
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 || p.ID2 == "foo" || p.ID3 == this.sampleDateTimeOffset)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 or ID2 eq 'foo' or ID3 eq " + this.sampleDate, query);
        }

        [Fact]
        public void OneWhereClauseWithOneAndOneOrForThreeKeysShouldReturnUrlWithFilter()
        {
            // Provide all key properites in single where clause with one AND and one OR operators
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => p.ID1 == 1 || p.ID2 == "foo" && p.ID3 == this.sampleDateTimeOffset)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 or ID2 eq 'foo' and ID3 eq " + this.sampleDate, query);
        }

        [Fact]
        public void TwoWhereClausesForOneKeyShouldReturnUrlWithFilter()
        {
            // Provide only partial list of  key proprties
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                           .Where(p => p.ID2 == "foo")
                           .Where(p => p.ID3 == this.sampleDateTimeOffset)
                           .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID2 eq 'foo' and ID3 eq " + this.sampleDate, query);
        }

        [Fact]
        public void ThreeKeys_OneKeyAndOneNonKeyShouldReturnUrlWithFilter()
        {
            // Provide one key property and one non-key property
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.Name == "foo")
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and Name eq 'foo'", query);
        }

        [Fact]
        public void TwoKeysAndOneNonKeyShouldReturnUrlWithFilter()
        {
            // Provide one key property and one non-key property
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.ID3 == this.sampleDateTimeOffset)
                            .Where(p => p.Name == "foo")
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID3 eq " + this.sampleDate + " and Name eq 'foo'", query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysAndNonKeysShouldReturnUrlWithFilter()
        {
            // provide all key properties and non-key properties
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.Name == "foo")
                            .Where(p => p.ID2 == "foo")
                            .Where(p => p.ID3 == this.sampleDateTimeOffset)
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and Name eq 'foo' and ID2 eq 'foo' and ID3 eq " + this.sampleDate, query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysAndNonKeysShouldReturnUrlWithFilterInCorrectOrder()
        {
            // provide all key properties and non-key properties
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                            .Where(p => p.ID1 == 1)
                            .Where(p => p.ID2 == "foo")
                            .Where(p => p.ID3 == this.sampleDateTimeOffset)
                            .Where(p => p.Name == "foo")
                            .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 eq 'foo' and ID3 eq " + this.sampleDate + " and Name eq 'foo'", query);
        }

        [Fact]
        public void ThreeWhereClausesForKeysAndSelectForNavEntityShouldReturnUrlForNavProperty()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1)
                                  .Where(p => p.ID2 == "foo")
                                  .Where(p => p.ID3 == this.sampleDateTimeOffset)
                                  .Select(p => p.NavProperty)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")/NavProperty", query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysAndNonKeysAndSelectForNavPropertyShouldReturnError()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1)
                                  .Where(p => p.Name == "foo")
                                  .Where(p => p.ID2 == "foo")
                                  .Where(p => p.ID3 == this.sampleDateTimeOffset)
                                  .Select(p => p.NavProperty)
                                  .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionsOnlyAllowedOnLeafNodes), query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysForEntityAndSelectForNavPropertyWithWhereClauseShouldReturnError()
        {
            // Provide all key properties for entity and select a navigational entity
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1)
                                  .Where(p => p.ID2 == "foo")
                                  .Where(p => p.ID3 == this.sampleDateTimeOffset)
                                  .Select(p => p.NavProperty)
                                  .Where(p => p.NonKeyProperty == "bar")
                                  .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionsOnlyAllowedOnSingletons), query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysOfEntityAndNavPropertyShouldReturnUrlWithNavPropertySingleton()
        {
            // Provide all key properties for entity as well as navigational entity
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1)
                                  .Where(p => p.ID2 == "foo")
                                  .Where(p => p.ID3 == this.sampleDateTimeOffset)
                                  .SelectMany(p => p.NavPropertyCollection)
                                  .Where(n => n.Key1 == 4)
                                  .Where(n => n.Key2 == 5)
                                  .Where(n => n.Key3 == 6)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")/NavPropertyCollection(Key1=4,Key2=5,Key3=6)", query);
        }

        [Fact]
        public void ThreeKeys_WhereClausesForKeysOfEntityAndWhereClausesForPartialKeysOfNavPropertyShouldReturnUrlWithFilter()
        {
            // Provide all key properties for entity and partial key property for navigational entity
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                                  .Where(p => p.ID1 == 1)
                                  .Where(p => p.ID2 == "foo")
                                  .Where(p => p.ID3 == this.sampleDateTimeOffset)
                                  .SelectMany(p => p.NavPropertyCollection)
                                  .Where(n => n.Key1 == 3)
                                  .Where(n => n.Key2 == 4)
                                  .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2='foo',ID3=" + this.sampleDate + ")/NavPropertyCollection?$filter=Key1 eq 3 and Key2 eq 4", query);
        }
        [Fact]
        public void ThreeKeys_NonFilterQueryOptionBeforeKeyPredicateShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .OrderBy(p => p.ID1)
                .Where(p => p.ID1 == 1)
                .Where(p => p.ID2 == "foo")
                .Where(p => p.ID3 == this.sampleDateTimeOffset)
                .ToString();

            Assert.Equal(rootUrl + "Test?$orderby=ID1&$filter=ID1 eq 1 and ID2 eq 'foo' and ID3 eq " + this.sampleDate, query);
        }

        [Fact]
        public void ThreeKeys_NonFilterQueryOptionAfterKeyPredicateShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Where(p => p.ID1 == 1)
                .Where(p => p.ID2 == "foo")
                .Where(p => p.ID3 == this.sampleDateTimeOffset)
                .OrderBy(p => p.ID2)
                .ToString();

            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 eq 'foo' and ID3 eq " + this.sampleDate + "&$orderby=ID2", query);
        }

        [Fact]
        public void ThreeKeys_NonFilterQueryOptionBetweenKeyPredicateShouldReturnUrlWithFilter()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Where(p => p.ID1 == 1)
                .OrderBy(p => p.ID2)
                .Where(p => p.ID2 == "foo")
                .Where(p => p.ID3 == this.sampleDateTimeOffset)
                .ToString();

            Assert.Equal(rootUrl + "Test?$orderby=ID2&$filter=ID1 eq 1 and ID2 eq 'foo' and ID3 eq " + this.sampleDate, query);
        }

        [Key("ID1")]
        public class DummyEntityWithOneKey
        {
            public int ID1 { get; set; }
            public string Name { get; set; }
            public int AnotherProperty { get; set; }
        }

        [Fact]
        public void ProjectionQueryToCreateNewTypeWithWhereClauseShouldThrowError()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Select(p => new DummyEntityWithOneKey
                {
                    ID1 = p.ID1,
                    Name = p.Name
                })
                .Where(oneKey => oneKey.AnotherProperty > 1)
                .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionOutOfOrder("filter", "select")), query);
        }

        [Fact]
        public void ProjectionQueryToAnonymousTypeWithWhereClauseShouldThrowError()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Select(p => new
                {
                    ID1 = p.ID1,
                    Name = p.Name
                })
                .Where(anon => anon.ID1 > 1)
                .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionOutOfOrder("filter", "select")), query);
        }

        [Fact]
        public void ProjectionQueryToNewTypeWithOrderByClauseShouldThrowError()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Select(p => new DummyEntityWithOneKey
                {
                    ID1 = p.ID1,
                    Name = p.Name
                })
                .OrderBy(dummy => dummy.Name)
                .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionOutOfOrder("orderby", "select")), query);
        }

        [Fact]
        public void ProjectionQueryToAnonymousTypeWithOrderByClauseShouldThrowError()
        {
            query = context.CreateQuery<EntityWithThreeKeyProperties>("Test")
                .Select(p => new
                {
                    ID1 = p.ID1,
                    Name = p.Name
                })
                .OrderBy(dummy => dummy.Name)
                .ToString();

            Assert.Equal(Strings.ALinq_TranslationError(Strings.ALinq_QueryOptionOutOfOrder("orderby", "select")), query);
        }
        #endregion

        #region Unit tests for entity with 5 keys

        [Key("ID1", "ID2", "ID3", "ID4", "ID5")]
        internal class EntityWithFiveKeyProperties
        {
            public int ID1 { get; set; }
            public int ID2 { get; set; }
            public int ID3 { get; set; }
            public int ID4 { get; set; }
            public int ID5 { get; set; }
            public string Name { get; set; }
            public AnotherEntityWithThreeKeyProperties NavProperty { get; set; }
            public List<AnotherEntityWithThreeKeyProperties> NavPropertyCollection { get; set; }
        }

        [Fact]
        public void FiveKeys_FiveWhereClausesForKeysShouldReturnUrlForSingleton()
        {
            // Simple case, only two key properties provided
            string query = context.CreateQuery<EntityWithFiveKeyProperties>("Test")
                .Where(p => p.ID1 == 1)
                .Where(p => p.ID2 == 2)
                .Where(p => p.ID3 == 3)
                .Where(p => p.ID4 == 4)
                .Where(p => p.ID5 == 5)
                .ToString();
            Assert.Equal(rootUrl + "Test(ID1=1,ID2=2,ID3=3,ID4=4,ID5=5)", query);
        }

        [Fact]
        public void OneWhereClausesWithDifferentCombinationsOfOperatorsForKeysShouldReturnUrlWithFilter()
        {
            // Simple case, only two key properties provided
            string query = context.CreateQuery<EntityWithFiveKeyProperties>("Test")
                .Where(p => p.ID1 == 1 && p.ID2 == 2 || (p.ID3 == 3 && p.ID4 == 4) && p.ID5 != 5)
                .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 eq 2 or ID3 eq 3 and ID4 eq 4 and ID5 ne 5", query);
        }

        [Fact]
        public void OneWhereClausesWithDifferentCombinationsOfGroupedOperatorsForKeysShouldReturnUrlWithFilter()
        {
            // Simple case, only two key properties provided
            string query = context.CreateQuery<EntityWithFiveKeyProperties>("Test")
                .Where(p => p.ID1 == 1 && p.ID2 == 2 && (p.ID3 == 3 || p.ID4 == 4) && p.ID5 != 5)
                .ToString();
            Assert.Equal(rootUrl + "Test?$filter=ID1 eq 1 and ID2 eq 2 and (ID3 eq 3 or ID4 eq 4) and ID5 ne 5", query);
        }

        #endregion Unit tests for entity with 5 keys

        #region Unit tests for date/time related function

        [Key("ID")]
        public class EntityWithDateAndTime
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTimeOffset dateTimeOffsetProperty { get; set; }
            public Date dateProperty { get; set; }
            public TimeOfDay timeOfDayProperty { get; set; }
        }

        [Fact]
        public void WhereClauseWithYearOfDateTimeOffsetShouldReturnUrlWithYearFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Year == 2014).ToString();

            Assert.Equal(rootUrl + "Test?$filter=year(dateTimeOffsetProperty) eq 2014", query);
        }

        [Fact]
        public void WhereClauseWithYearOfDateShouldReturnUrlWithYearFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateProperty.Year == 2014).ToString();

            Assert.Equal(rootUrl + "Test?$filter=year(dateProperty) eq 2014", query);
        }

        [Fact]
        public void WhereClauseWithMonthOfDateTimeOffsetShouldReturnUrlWithMonthFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Month == 9).ToString();

            Assert.Equal(rootUrl + "Test?$filter=month(dateTimeOffsetProperty) eq 9", query);
        }

        [Fact]
        public void WhereClauseWithMonthOfDateShouldReturnUrlWithMonthFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateProperty.Month > 9).ToString();

            Assert.Equal(rootUrl + "Test?$filter=month(dateProperty) gt 9", query);
        }

        [Fact]
        public void WhereClauseWithDayOfDateTimeOffsetShouldReturnUrlWithDayFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Day == 25).ToString();

            Assert.Equal(rootUrl + "Test?$filter=day(dateTimeOffsetProperty) eq 25", query);
        }

        [Fact]
        public void WhereClauseWithDayOfDateShouldReturnUrlWithDayFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateProperty.Day <= 25).ToString();

            Assert.Equal(rootUrl + "Test?$filter=day(dateProperty) le 25", query);
        }

        [Fact]
        public void WhereClauseWithHourOfDateTimeOffsetShouldReturnUrlWithHourFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Hour == 12).ToString();

            Assert.Equal(rootUrl + "Test?$filter=hour(dateTimeOffsetProperty) eq 12", query);
        }

        [Fact]
        public void WhereClauseWithHoursOfDateTimeOffsetShouldReturnUrlWithHourFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.timeOfDayProperty.Hours < 12).ToString();

            Assert.Equal(rootUrl + "Test?$filter=hour(timeOfDayProperty) lt 12", query);
        }

        [Fact]
        public void WhereClauseWithMintueOfDateTimeOffsetShouldReturnUrlWithMinuteFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Minute == 30).ToString();

            Assert.Equal(rootUrl + "Test?$filter=minute(dateTimeOffsetProperty) eq 30", query);
        }

        [Fact]
        public void WhereClauseWithMinutesOfDateTimeOffsetShouldReturnUrlWithMinuteFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.timeOfDayProperty.Minutes >= 30).ToString();

            Assert.Equal(rootUrl + "Test?$filter=minute(timeOfDayProperty) ge 30", query);
        }

        [Fact]
        public void WhereClauseWithSecondOfDateTimeOffsetShouldReturnUrlWithSecondFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Second == 30).ToString();

            Assert.Equal(rootUrl + "Test?$filter=second(dateTimeOffsetProperty) eq 30", query);
        }

        [Fact]
        public void WhereClauseWithMSecondsOfDateTimeOffsetShouldReturnUrlWithSecondFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.timeOfDayProperty.Seconds != 59).ToString();

            Assert.Equal(rootUrl + "Test?$filter=second(timeOfDayProperty) ne 59", query);
        }
        [Fact]
        public void WhereClauseWithDateOfDateTimeOffsetShouldReturnUrlWithDateFunction()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date == (DateTimeOffset.MinValue)).ToString();

            Assert.Equal(rootUrl + "Test?$filter=cast(date(dateTimeOffsetProperty),'Edm.DateTimeOffset') eq 0001-01-01T00:00:00Z", query);
        }
        [Fact]
        public void WhereClauseWithDateAndHourOfDateTimeOffsetShouldReturnUrlWithDateAndHourFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Hour == 12).ToString();

            Assert.Equal(rootUrl + "Test?$filter=hour(date(dateTimeOffsetProperty)) eq 12", query);
        }
        [Fact]
        public void WhereClauseWithDateAndMinuteOfDateTimeOffsetShouldReturnUrlWithDateAndMinuteFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Minute == 30).ToString();

            Assert.Equal(rootUrl + "Test?$filter=minute(date(dateTimeOffsetProperty)) eq 30", query);
        }
        [Fact]
        public void WhereClauseWithDateAndSecondOfDateTimeOffsetShouldReturnUrlWithDateAndSecondFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Second == 30).ToString();

            Assert.Equal(rootUrl + "Test?$filter=second(date(dateTimeOffsetProperty)) eq 30", query);
        }
        [Fact]
        public void WhereClauseWithDateAndYearOfDateTimeOffsetShouldReturnUrlWithDateAndYearFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Year == 2014).ToString();

            Assert.Equal(rootUrl + "Test?$filter=year(date(dateTimeOffsetProperty)) eq 2014", query);
        }
        [Fact]
        public void WhereClauseWithDateAndMonthOfDateTimeOffsetShouldReturnUrlWithDateAndMonthFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Month == 9).ToString();

            Assert.Equal(rootUrl + "Test?$filter=month(date(dateTimeOffsetProperty)) eq 9", query);
        }
        [Fact]
        public void WhereClauseWithDateAndDayOfDateTimeOffsetShouldReturnUrlWithDateAndDayFunctions()
        {
            query = context.CreateQuery<EntityWithDateAndTime>("Test").Where(p => p.dateTimeOffsetProperty.Date.Day == 9).ToString();

            Assert.Equal(rootUrl + "Test?$filter=day(date(dateTimeOffsetProperty)) eq 9", query);
        }

        #endregion
    }
}
