//---------------------------------------------------------------------
// <copyright file="DataServiceContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class DataServiceContextTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://service/"));

            // Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Configurations);
            Assert.NotNull(context.Format);
            Assert.NotNull(context.Entities);
            Assert.NotNull(context.Links);
            Assert.NotNull(context.Model);
            Assert.NotNull(context.BaseUri);
        }

        [Fact]
        public void BaseUri_Property_SetAndGet()
        {
            // Arrange
            var uri = new Uri("http://service/");

            // Act & Assert
            var context = new DataServiceContext(uri);

            Assert.Equal(uri, context.BaseUri);

            var newUri = new Uri("http://newservice/");
            context.BaseUri = newUri;
            Assert.Equal(newUri, context.BaseUri);
        }

        [Fact]
        public void MergeOption_Property_SetAndGet()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://service/"));
            context.MergeOption = MergeOption.OverwriteChanges;

            // Assert
            Assert.Equal(MergeOption.OverwriteChanges, context.MergeOption);
        }

        [Fact]
        public void UrlKeyDelimiter_Property_SetAndGet()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://service/"));
            context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Assert
            Assert.Equal(DataServiceUrlKeyDelimiter.Slash, context.UrlKeyDelimiter);
        }

        [Fact]
        public void AddObject_TracksEntity()
        {
            // Arrange
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            var entity = new TestEntity { Id = 1 };

            // Act
            context.AddObject("TestEntities", entity);

            // Assert
            Assert.Single(context.Entities);
            Assert.Equal(entity, context.Entities[0].Entity);
        }

        [Fact]
        public void DeleteObject_RemovesEntity()
        {
            // Arrange
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            var entity = new TestEntity { Id = 1 };

            // Act & Assert
            context.AddObject("TestEntities", entity);
            Assert.Single(context.Entities);

            context.DeleteObject(entity);
            Assert.Empty(context.Entities);
        }

        [Fact]
        public void AddLink_TracksLink()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            var parent = new TestEntity { Id = 1 };
            var child = new TestEntity { Id = 2 };
            context.AddObject("Parents", parent);
            context.AddObject("Children", child);

            context.AddLink(parent, "Children", child);

            // Assert
            Assert.Single(context.Links);
            Assert.Equal(parent, context.Links[0].Source);
            Assert.Equal(child, context.Links[0].Target);
        }

        [Fact]
        public async Task LoadPropertyAsync_ThrowsOnNullEntity()
        {
            // Arrange & Act & Assert
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            await Assert.ThrowsAsync<ArgumentNullException>(() => context.LoadPropertyAsync(null, "Property"));
        }

        [Fact]
        public void SetLink_ThrowsOnInvalidProperty()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            var parent = new TestEntity { Id = 1 };
            var child = new TestEntity { Id = 2 };
            context.AddObject("Parents", parent);
            context.AddObject("Children", child);

            // Assert
            Assert.Throws<InvalidOperationException>(() => context.SetLink(parent, "NonExistentProperty", child));
        }

        [Fact]
        public void Event_SendingRequest2_CanBeSubscribed()
        {
            // Arrange & Act
            var context = new DataServiceContext(new Uri("http://localhost/odata/"));
            bool eventFired = false;
            context.SendingRequest2 += (s, e) => eventFired = true;

            // Simulate event firing
            context.GetType().GetMethod("FireSendingRequest2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(context, new object[] { new SendingRequest2EventArgs(null, null, false) });

            // Assert
            Assert.True(eventFired);
        }

        private class TestEntity
        {
            public int Id { get; set; }
            public Collection<TestEntity> Children { get; set; }
        }
    }
}
