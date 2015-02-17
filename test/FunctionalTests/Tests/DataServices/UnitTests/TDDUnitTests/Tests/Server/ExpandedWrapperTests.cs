//---------------------------------------------------------------------
// <copyright file="ExpandedWrapperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Internal;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpandedWrapperTests
    {
        private readonly ExpandedWrapper<EntityType, int, EntityType, IList<EntityType>> expandedWrapper = new ExpandedWrapper<EntityType, int, EntityType, IList<EntityType>>();
        private readonly EntityType reference = new EntityType();

        [TestInitialize]
        public void Init()
        {
            this.expandedWrapper.ProjectedProperty1 = this.reference;
            this.expandedWrapper.Description = "Id,ReferenceProperty";
        }

        [TestMethod]
        public void ExpandedWrapperShouldAllowNullReferenceDescription()
        {
            // should not throw
            this.expandedWrapper.ReferenceDescription = null;
            this.expandedWrapper.ReferenceDescription.Should().BeNull();
        }

        [TestMethod]
        public void ExpandedWrapperShouldWrapEnumerableReferencePropertyWithoutDescription()
        {
            var expandedPropertyValue = this.expandedWrapper.GetExpandedPropertyValue("ReferenceProperty");
            expandedPropertyValue.Should().NotBeNull();
            expandedPropertyValue.GetType().FullName.Should().Be("Microsoft.OData.Service.Internal.ProjectedWrapper+EnumerableWrapper");
        }

        [TestMethod]
        public void ExpandedWrapperShouldNotWrapEnumerableReferencePropertyWithDescription()
        {
            this.expandedWrapper.ReferenceDescription = "ReferenceProperty";
            this.expandedWrapper.GetExpandedPropertyValue("ReferenceProperty").Should().Be(this.reference);
        }

        private class EntityType : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
