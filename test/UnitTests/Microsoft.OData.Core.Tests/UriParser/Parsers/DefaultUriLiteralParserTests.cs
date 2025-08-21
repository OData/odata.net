//---------------------------------------------------------------------
// <copyright file="DefaultUriLiteralParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class DefaultUriLiteralParserTests
    {
        private static IEdmModel NewModel() => new EdmModel();
        private static IEdmTypeReference EdmTString(bool nullable = false) => EdmCoreModel.Instance.GetString(nullable);

        private sealed class MarkerStringParser : IUriLiteralParser
        {
            private readonly string marker;
            private readonly IEdmTypeReference key;

            public MarkerStringParser(string marker, IEdmTypeReference key)
            {
                this.marker = marker;
                this.key = key;
            }

            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                parsingException = null;

                // Only claim support for the specific type we were created with.
                if (!targetType.IsEquivalentTo(key))
                {
                    return null;
                }

                // Return a marker so we can verify the DefaultUriLiteralParser routed to us.
                return marker;
            }
        }

        [Fact]
        public void GetOrCreate_NullModel_Throws()
        {
            Assert.Throws<ArgumentNullException>("model", () => DefaultUriLiteralParser.GetOrCreate(null));
        }

        [Fact]
        public void GetOrCreate_SameModel_ReturnsSameInstance()
        {
            var model = NewModel();

            var parserInstance1 = DefaultUriLiteralParser.GetOrCreate(model);
            var parserInstance2 = DefaultUriLiteralParser.GetOrCreate(model);

            Assert.Same(parserInstance1, parserInstance2);
        }

        [Fact]
        public void GetOrCreate_DifferentModels_ReturnDifferentInstances()
        {
            var model1 = NewModel();
            var model2 = NewModel();

            var parserInstance1 = DefaultUriLiteralParser.GetOrCreate(model1);
            var parserInstance2 = DefaultUriLiteralParser.GetOrCreate(model2);

            Assert.NotSame(parserInstance1, parserInstance2);
        }

        [Fact]
        public async Task GetOrCreate_IsThreadSafe_SameModel_SingleInstance()
        {
            var model = NewModel();
            var bag = new ConcurrentBag<DefaultUriLiteralParser>();

            await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount * 4).Select(_ =>
                Task.Run(() => bag.Add(DefaultUriLiteralParser.GetOrCreate(model)))));

            Assert.Single(bag.Distinct());
        }

        [Fact]
        public async Task GetOrCreate_IsThreadSafe_ManyModels_IsolatedInstances()
        {
            // Create many distinct models and ensure each maps to its own parser instance.
            const int count = 64;
            var models = Enumerable.Range(0, count).Select(_ => NewModel()).ToArray();
            var map = new ConcurrentDictionary<IEdmModel, DefaultUriLiteralParser>(ReferenceEqualityComparer<IEdmModel>.Instance);

            await Task.WhenAll(models.Select(m => Task.Run(() =>
            {
                var parser = DefaultUriLiteralParser.GetOrCreate(m);
                map.TryAdd(m, parser);
            })));

            Assert.Equal(count, map.Count);
            Assert.Equal(count, map.Values.Distinct().Count());
        }

        [Fact]
        public void GetOrCreate_ReturnedInstance_CooperatesWithCustomParsers_AndPrefersCustomOverBuiltIn()
        {
            // Arrange a custom parser for Edm.String(true) and ensure the model-scoped parser dispatches to it.
            var model = NewModel();
            var stringType = EdmTString(true);
            var marker = "CUSTOM-PARSER-MARKER";
            var custom = new MarkerStringParser(marker, stringType);

            model.AddCustomUriLiteralParser(stringType, custom);

            // Act
            var sut = DefaultUriLiteralParser.GetOrCreate(model);
            var result = sut.ParseUriStringToType("'anything'", stringType, out var parsingException);

            // Assert
            Assert.Null(parsingException);
            Assert.Equal(marker, result);

            // Cleanup
            Assert.True(model.RemoveCustomUriLiteralParser(custom));
        }
    }
}
