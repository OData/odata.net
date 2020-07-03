//---------------------------------------------------------------------
// <copyright file="ConventionalEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.OData.Tests.Evaluation;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using EdmValueUtils = Microsoft.OData.Client.EdmValueUtils;
    using Xunit;

    public class ConventionalEntityMetadataBuilderTests
    {
        private ConventionalODataEntityMetadataBuilder metadataBuilder;
        private EdmEntitySet entitySet;
        private IEdmStructuredValue structure;

        public ConventionalEntityMetadataBuilderTests()
        {
            var entityContainer = new EdmEntityContainer("MyNamespace", "MyThing");

            var entityType = new EdmEntityType("MyNamespace", "EntityType");
            entityType.AddKeys(new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false)));

            this.entitySet = this.entitySet = new EdmEntitySet(entityContainer, "EntitySet", entityType);
            this.structure = new EdmStructuredValueSimulator(entityType, new Dictionary<string, IEdmValue> { { "Id", EdmValueUtils.ConvertPrimitiveValue(1, null).Value } });

            this.metadataBuilder = new ConventionalODataEntityMetadataBuilder(new Uri("http://test/"), ((IEdmEntitySet)this.entitySet).Name, this.structure, DataServiceUrlKeyDelimiter.Parentheses);
        }

        [Fact]
        public void EditLinkShouldMatchConventions()
        {
            var editLink = metadataBuilder.GetEditLink();
            editLink.Should().NotBeNull();
            editLink.OriginalString.Should().Be("http://test/EntitySet(1)");
        }

        [Fact]
        public void IdShouldMatchConventions()
        {
            this.metadataBuilder.GetId()
                .Should()
                .Be("http://test/EntitySet(1)");
        }

        [Fact]
        public void ReadLinkShouldBeNull()
        {
            this.metadataBuilder.GetReadLink().Should().BeNull();
        }

        [Fact]
        public void ETagShouldBeNull()
        {
            this.metadataBuilder.GetETag().Should().BeNull();
        }
    }
}