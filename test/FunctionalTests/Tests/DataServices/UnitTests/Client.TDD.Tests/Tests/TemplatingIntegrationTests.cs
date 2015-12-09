//---------------------------------------------------------------------
// <copyright file="TemplatingIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Core.Tests.Evaluation;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm.Values;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TemplatingIntegrationTests
    {
        [TestMethod]
        public void ContextShouldUseConventionsByDefault()
        {
            Assert.IsInstanceOfType(new TestClientContext().GetMetadataBuilder("Fake", new EdmStructuredValueSimulator()), typeof(ConventionalODataEntityMetadataBuilder));
        }

        [TestMethod]
        public void AttachShouldUseBuilder()
        {
            const string fakeEditLink = "http://thisIsTheEditLink.org/";
            const string fakeId = "http://thisIsTheId.org/";

            var simulator = new EntityMetadataBuilderSimulator 
            { 
                GetEditLinkFunc = () => new Uri(fakeEditLink),
                GetIdFunc = () => new Uri(fakeId),
            };

            var testContext = new TestClientContext(new Uri("http://temp.org/"))
            { 
                GetMetadataBuilderFunc = (set, e) => simulator 
            };

            testContext.AttachTo("FakeSet", new SingleKeyType { Property = "foo" });
            EntityDescriptor entityDescriptor = testContext.Entities[0];
            entityDescriptor.EditLink.Should().Be(fakeEditLink);
            entityDescriptor.Identity.Should().Be(fakeId);
        }
        
        [TestMethod]
        public void AttachShouldFailOnNullBuilder()
        {
            var testContext = new TestClientContext
            {
                GetMetadataBuilderFunc = (set, e) => null
            };

            Action action = () => testContext.AttachTo("FakeSet", new SingleKeyType { Property = "foo" });
            action.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_EntityMetadataBuilderIsRequired);
        }
        
        [TestMethod]
        public void AttachPinningTest()
        {
            const string expectedOutput = @"http://test.org/test/EntityS%C3%A9t1('foo%2B%2Fbar')
http://test.org/test/EntityS%C3%A9t2(Property1='fo%2Bo',Property2='b%2Far')
http://test.org/test/EntitySet1('foo')
http://test.org/test/EntitySet2(Property1='foo',Property2='bar')
http://test.org/test/EntitySet1('foo')
http://test.org/test/EntitySet2(Property1='foo',Property2='bar')
http://test.org/test/EntitySet1/Fake(1)('foo')
http://test.org/test/EntitySet1/Fake(1)('foo')
http://test.org/test/EntitySet1/Fake(1)('foo')
http://test.org/test/EntitySet1/Fake(1)('foo')
http://test.org/test/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://test.org/test/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://test.org/test/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://test.org/test/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
";
            var ctx = new DataServiceContext(new Uri("http://test.org/test"));
            RunAttachPinningTest(ctx, expectedOutput);
            
            ctx = new DataServiceContext(new Uri("http://test.org/test/"));
            RunAttachPinningTest(ctx, expectedOutput);
        }
 
        [TestMethod]
        public void AttachPinningTestWithEntitySetResolver()
        {
            const string expectedOutput = @"http://resolved.org/EntityS%C3%A9t1('foo%2B%2Fbar')
http://resolved.org/EntityS%C3%A9t2(Property1='fo%2Bo',Property2='b%2Far')
http://resolved.org/EntitySet1('foo')
http://resolved.org/EntitySet2(Property1='foo',Property2='bar')
http://resolved.org/EntitySet1('foo')
http://resolved.org/EntitySet2(Property1='foo',Property2='bar')
http://resolved.org/EntitySet1/Fake(1)('foo')
http://resolved.org/EntitySet1/Fake(1)('foo')
http://resolved.org/EntitySet1/Fake(1)('foo')
http://resolved.org/EntitySet1/Fake(1)('foo')
http://resolved.org/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://resolved.org/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://resolved.org/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
http://resolved.org/EntitySet2/Fake(1)/Navigation(Property1='foo',Property2='bar')
";
            var ctx = new TestClientContext { ResolveEntitySet = s => new Uri("http://resolved.org/" + s) };
            RunAttachPinningTest(ctx, expectedOutput);

            ctx = new TestClientContext { ResolveEntitySet = s => new Uri("http://resolved.org/" + s + '/') };
            RunAttachPinningTest(ctx, expectedOutput);
        }

        [TestMethod]
        public void AttachShouldFailOnNullKeys()
        {
            var ctx = new DataServiceContext(new Uri("http://test.org/test"));

            Action withNullKey = () => ctx.AttachTo("EntitySet1", new SingleKeyType { Property = null });
            withNullKey.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_NullKeysAreNotSupported("Property"));

            withNullKey = () => ctx.AttachTo("EntitySet1", new CompositeKeyType { Property1 = null, Property2 = "bar" });
            withNullKey.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_NullKeysAreNotSupported("Property1"));

            withNullKey = () => ctx.AttachTo("EntitySet1", new CompositeKeyType { Property1 = "foo", Property2 = null });
            withNullKey.ShouldThrow<InvalidOperationException>().WithMessage(Strings.Context_NullKeysAreNotSupported("Property2"));
        }

        private static void RunAttachPinningTest(DataServiceContext ctx, string expectedOutput)
        {
            var output = new StringBuilder();

            // with special characters
            AttachAndLog(output, ctx, "EntitySét1", new SingleKeyType {Property = "foo+/bar"});
            AttachAndLog(output, ctx, "EntitySét2", new CompositeKeyType {Property1 = "fo+o", Property2 = "b/ar"});

            // with preceeding slash
            AttachAndLog(output, ctx, "/EntitySet1", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "/EntitySet2", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});

            // with trailing slash
            AttachAndLog(output, ctx, "EntitySet1/", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "EntitySet2/", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});

            // with additional segments
            AttachAndLog(output, ctx, "EntitySet1/Fake(1)", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "EntitySet1/Fake(1)", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "EntitySet1/Fake(1)/", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "EntitySet1/Fake(1)/", new SingleKeyType {Property = "foo"});
            AttachAndLog(output, ctx, "EntitySet2/Fake(1)/Navigation", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});
            AttachAndLog(output, ctx, "EntitySet2/Fake(1)/Navigation", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});
            AttachAndLog(output, ctx, "EntitySet2/Fake(1)/Navigation/", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});
            AttachAndLog(output, ctx, "EntitySet2/Fake(1)/Navigation/", new CompositeKeyType {Property1 = "foo", Property2 = "bar"});

            var actualOutput = output.ToString();
            actualOutput.Should().Be(expectedOutput);
        }

        private static void AttachAndLog(StringBuilder output, DataServiceContext ctx, string entitySetName, object entity)
        {
            try
            {
                ctx.AttachTo(entitySetName, entity);

                EntityDescriptor entityDescriptor = ctx.Entities[0];
                entityDescriptor.Identity.Should().Be(entityDescriptor.EditLink.AbsoluteUri);
                output.AppendLine(entityDescriptor.EditLink.OriginalString);

                ctx.Detach(entity);
            }
            catch (Exception e)
            {
                var exception = e;
                while (exception != null)
                {
                    output.Append(exception.GetType().FullName);
                    output.Append(": ");
                    output.AppendLine(exception.Message);
                    exception = exception.InnerException;
                }
            }
        }

        private class TestClientContext : DataServiceContext
        {
            public TestClientContext(Uri serviceRoot, ODataProtocolVersion version, ClientEdmModel model)
                : base(serviceRoot, version, model)
            {
            }

            public TestClientContext(Uri serviceRoot) : base(serviceRoot)
            {
            }

            public TestClientContext()
            {
            }

            public Func<string, IEdmStructuredValue, Microsoft.OData.Client.ODataEntityMetadataBuilder> GetMetadataBuilderFunc { get; set; }

            public Microsoft.OData.Client.ODataEntityMetadataBuilder GetMetadataBuilder(string entitySetName, IEdmStructuredValue entityInstance)
            {
                return base.GetEntityMetadataBuilder(entitySetName, entityInstance);
            }

            internal override Microsoft.OData.Client.ODataEntityMetadataBuilder GetEntityMetadataBuilder(string entitySetName, IEdmStructuredValue entityInstance)
            {
                if (this.GetMetadataBuilderFunc == null)
                {
                    return base.GetEntityMetadataBuilder(entitySetName, entityInstance);
                }

                return this.GetMetadataBuilderFunc(entitySetName, entityInstance);
            }
        }

        [Key("Property2", "Property1")] //intentionally reversed
        private class CompositeKeyType
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [Key("Property")]
        private class SingleKeyType
        {
            public string Property { get; set; }
        }

        private class EntityMetadataBuilderSimulator : Microsoft.OData.Client.ODataEntityMetadataBuilder
        {
            public Func<Uri> GetEditLinkFunc { get; set; }
            public Func<Uri> GetIdFunc { get; set; }


            internal override Uri GetEditLink()
            {
                if (this.GetEditLinkFunc == null)
                {
                    return null;
                }

                return this.GetEditLinkFunc();
            }

            internal override Uri GetId()
            {
                if (this.GetIdFunc == null)
                {
                    return null;
                }

                return this.GetIdFunc();
            }

            internal override string GetETag()
            {
                return null;
            }

            internal override Uri GetReadLink()
            {
                return null;
            }

            internal override bool TryGetIdForSerialization(out Uri id)
            {
                id = null;
                return false;
            }
        }
    }
}
