//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentClientIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.TDD.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Xunit;

    public class KeyAsSegmentClientIntegrationTests
    {
        private DataServiceContext contextWithKeyAsSegment;

        public KeyAsSegmentClientIntegrationTests()
        {
            this.contextWithKeyAsSegment = new DataServiceContext(new Uri("http://myservice/", UriKind.Absolute), ODataProtocolVersion.V4)
            {
                UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash
            };
        }

        [Fact]
        public void LinqShouldUseKeyAsSegmentConvention()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Where(t => t.ID == 1);
            query.ToString().Should().Be("http://myservice/Things/1");
        }

        [Fact]
        public void TypeSegmentAfterSetShouldBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").OfType<DerivedType>();
            query.ToString().Should().Be("http://myservice/Things/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType");
        }

        [Fact]
        public void TypeSegmentAfterEntityShouldBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Where(t => t.ID == 1).OfType<DerivedType>();
            query.ToString().Should().Be("http://myservice/Things/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/1");
        }

        [Fact]
        public void TypeSegmenBeforeEntityShouldBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").OfType<DerivedType>().Where(t => t.ID == 1);
            query.ToString().Should().Be("http://myservice/Things/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/1");
        }

        [Fact]
        public void TypeSegmentAfterReferenceShouldBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Where(t => t.ID == 1).Select(t => ((t as DerivedType).Reference as DerivedType).Prop);
            query.ToString().Should().Be("http://myservice/Things/1/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/Reference/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/Prop");
        }

        [Fact]
        public void TypeSegmentAfterCollectionShouldBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Where(t => t.ID == 1).SelectMany(t => ((t as DerivedType).Collection)).Where(t2 => t2.ID == 2).OfType<DerivedType>();
            query.ToString().Should().Be("http://myservice/Things/1/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/Collection/$/AstoriaUnitTests.Tests.TDD.Client.DerivedType/2");
        }

        [Fact]
        public void TypeSegmentInFilterShouldNotBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Where(t => (t as DerivedType).Prop == 1);
            query.ToString().Should().Be("http://myservice/Things?$filter=AstoriaUnitTests.Tests.TDD.Client.DerivedType/Prop eq 1");
        }

        [Fact]
        public void TypeSegmentInOrderByShouldNotBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").OrderBy(t => (t as DerivedType).Prop);
            query.ToString().Should().Be("http://myservice/Things?$orderby=AstoriaUnitTests.Tests.TDD.Client.DerivedType/Prop");
        }

        [Fact]
        public void TypeSegmentInSelectAndExpandShouldNotBeEscapedWhenUrlConventionIsKeyAsSegment()
        {
            var query = this.contextWithKeyAsSegment.CreateQuery<BaseType>("Things").Select(t => new { ((t as DerivedType).Reference as DerivedType).Prop });
            query.ToString().Should().Be("http://myservice/Things?$expand=AstoriaUnitTests.Tests.TDD.Client.DerivedType/Reference($select=AstoriaUnitTests.Tests.TDD.Client.DerivedType/Prop)");
        }

        [Fact]
        public void AttachToShouldUseKeyAsSegmentConventionForIdentityAndEditLink()
        {
            this.contextWithKeyAsSegment.AttachTo("Things", new BaseType());
            var descriptor = this.contextWithKeyAsSegment.Entities.Single();
            descriptor.Identity.Should().Be("http://myservice/Things/0");
            descriptor.EditLink.Should().Be("http://myservice/Things/0");
        }
    }

    internal class BaseType
    {
        public int ID { get; set; }
    }

    internal class DerivedType : BaseType
    {
        public int Prop { get; set; }
        public BaseType Reference { get; set; }
        public ICollection<BaseType> Collection { get; set; }
    }
}
