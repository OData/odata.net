//---------------------------------------------------------------------
// <copyright file="ContainerBuilderExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ContainerBuilderExtensionsTests
    {
        [Fact]
        public void AddServiceWithServiceType()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, typeof(Foo));
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<Foo>());
        }

        [Fact]
        public void AddServiceWithTServiceAndTImplementation()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService<IFoo, Foo>(ServiceLifetime.Transient);
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<IFoo>());
        }

        [Fact]
        public void AddServiceWithTServiceOnly()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService<Foo>(ServiceLifetime.Transient);
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<Foo>());
        }

        [Fact]
        public void AddServiceWithTServiceFactory()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService(ServiceLifetime.Transient, sp => new Foo());
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<Foo>());
        }

        [Fact]
        public void AddServiceWithTServiceAndTImplementationFactory()
        {
            IContainerBuilder builder = new TestContainerBuilder();
            builder.AddService<IFoo>(ServiceLifetime.Transient, sp => new Foo());
            IServiceProvider container = builder.BuildContainer();
            Assert.NotNull(container.GetService<IFoo>());
        }

        private interface IFoo { }

        private class Foo : IFoo { }
    }
}
