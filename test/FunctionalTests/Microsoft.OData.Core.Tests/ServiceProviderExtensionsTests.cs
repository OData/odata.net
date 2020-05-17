//---------------------------------------------------------------------
// <copyright file="ServiceProviderExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ServiceProviderExtensionsTests
    {
        [Fact]
        public void GetNonExistingServiceGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.Null(container.GetService<IFoo>());
        }

        [Fact]
        public void GetServiceGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<Foo>());
        }

        [Fact]
        public void GetServiceNonGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService(typeof(Foo)));
        }

        [Fact]
        public void GetNonExistingRequiredServiceThrows()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.Throws<ODataException>(() => container.GetRequiredService<IFoo>());
        }

        [Fact]
        public void GetRequiredServiceGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetRequiredService<Foo>());
        }

        [Fact]
        public void GetRequiredServiceNonGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetRequiredService(typeof(Foo)));
        }

        [Fact]
        public void GetServicesNonGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService<IFoo, Foo>(ServiceLifetime.Transient);
            builder.AddService<IFoo, Bar>(ServiceLifetime.Transient);
            IServiceProvider container = builder.BuildContainer();
            Assert.Equal(2, container.GetServices(typeof(IFoo)).Count());
        }

        [Fact]
        public void GetServicesGeneric()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService<IFoo, Foo>(ServiceLifetime.Transient);
            builder.AddService<IFoo, Bar>(ServiceLifetime.Transient);
            IServiceProvider container = builder.BuildContainer();
            Assert.Equal(2, container.GetServices<IFoo>().Count());
        }

        private interface IFoo { }

        private class Foo : IFoo { }

        private class Bar : IFoo { }
    }
}
