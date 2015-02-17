//---------------------------------------------------------------------
// <copyright file="PayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;

    /// <summary>
    /// PayloadGenerator to generate payloads covering different shapes and sizes
    /// </summary>
    [ImplementationName(typeof(IPayloadGenerator), "Default")]
    public class PayloadGenerator : IPayloadGenerator
    {
        /// <summary>
        /// URL string to represent the NextLink in a payload
        /// </summary>
        private const string NextLink = "http://odata.org/nextlink";

        /// <summary>
        /// Used to ensure unique property names
        /// </summary>
        private int propertyCounter = 0;

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the services to use for data generation
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelConceptualDataServices ConceptualDataServices { get; set; }

        /// <summary>
        /// Gets or sets the odata payload builder
        /// </summary>
        [InjectDependency]
        public IODataPayloadBuilder ODataPayloadBuilder { get; set; }

        /// <summary>
        /// A non dependency injected normalizer for making sure the payloads are ODataLib complaint
        /// </summary>
        private ODataWriterPayloadNormalizer normalizer = new ODataWriterPayloadNormalizer();

        /// <summary>
        /// Returns Atom payloads based on default model
        /// </summary>
        /// <returns>An IEnumerable of Payloads </returns>
        public IEnumerable<EntityInstance> GenerateAtomPayloads()
        {
            var model = TestModels.BuildDefaultAstoriaTestModel();
            var sets = model.EntityContainersAcrossModels().First().Elements.OfType<IEdmEntitySet>();

            // Create entity for each set in model.
            foreach (var set in sets)
            {
                if (!set.EntityType().IsAbstract)
                {
                    //create entity based on ElementType
                    EntityInstance payload = this.AddIDAndLink(this.CreateEntityInstance(set.EntityType()));
                    payload.WithDefaultAtomEntryAnnotations();
                    payload.WithTypeAnnotation(set.EntityType());
                    payload = (EntityInstance)this.normalizer.Visit(payload);
                    this.Assert.IsTrue(payload.Annotations.Count >= 2, "There should be a minimum of two annotations");
                    yield return payload;

                }
            }
        }

        /// <summary>
        /// Returns Json payloads based on default model
        /// </summary>
        /// <returns>An IEnumerable of Payloads </returns>
        public IEnumerable<EntityInstance> GenerateJsonPayloads()
        {
            List<EntityInstance> payloads = new List<EntityInstance>();
            var model = TestModels.BuildDefaultAstoriaTestModel();
            var sets = model.EntityContainersAcrossModels().First().Elements.OfType<IEdmEntitySet>();

            // Create entity for each set in model.
            foreach (var set in sets)
            {
                if (!set.EntityType().IsAbstract)
                {
                    EntityInstance payload = this.AddIDAndLink(this.CreateEntityInstance(set.EntityType())).WithTypeAnnotation(set.EntityType());
                    this.Assert.IsTrue(payload.Annotations.Count == 2, "There should only be two annotations");
                    payload = (EntityInstance)this.normalizer.Visit(payload);
                    payloads.Add(payload);
                }
            }

            return payloads;
        }

        /// <summary>
        /// Generates a set of interesting payloads for a given payload (i.e., puts the payload in all
        /// the interesting places in valid OData payloads where it can appear).
        /// </summary>
        /// <param name="payload">The payload to generate reader input payloads for.</param>
        /// <param name="validators">The validators to use for payload config skipping.</param>
        /// <returns>A set of payloads that use the <paramref name="payload"/> in interesting places.</returns>
        public IEnumerable<T> GeneratePayloads<T>(T payload) where T : PayloadTestDescriptor
        {
            return this.GeneratePayloads(payload, ODataPayloadElementConfigurationValidator.AllValidators);
        }

        /// <summary>
        /// Generates a set of interesting payloads for a given payload (i.e., puts the payload in all
        /// the interesting places in valid OData payloads where it can appear).
        /// </summary>
        /// <param name="payload">The payload to generate reader input payloads for.</param>
        /// <param name="validators">The validators to use for payload config skipping.</param>
        /// <returns>A set of payloads that use the <paramref name="payload"/> in interesting places.</returns>
        public IEnumerable<T> GeneratePayloads<T>(T payload, ODataPayloadElementConfigurationValidator.Validators validators) where T : PayloadTestDescriptor
        {
            Debug.Assert(payload != null, "payload != null");
            Debug.Assert(payload.PayloadElement != null, "payload.PayloadElement != null");

            IEnumerable<T> payloads;
            switch (payload.PayloadElement.ElementType)
            {
                case ODataPayloadElementType.EntitySetInstance:
                    payloads = this.GenerateFeedPayloads(payload);
                    break;

                case ODataPayloadElementType.EntityInstance:
                    payloads = this.GenerateEntryPayloads(payload);
                    break;

                case ODataPayloadElementType.NullPropertyInstance:
                case ODataPayloadElementType.PrimitiveProperty:
                case ODataPayloadElementType.ComplexProperty:
                case ODataPayloadElementType.PrimitiveMultiValueProperty:
                case ODataPayloadElementType.ComplexMultiValueProperty:
                    payloads = this.GeneratePropertyPayloads(payload);
                    break;

                case ODataPayloadElementType.NavigationPropertyInstance:
                    payloads = this.GenerateNavigationPropertyPayloads(payload);
                    break;

                case ODataPayloadElementType.NamedStreamInstance:
                    payloads = this.GenerateNamedStreamPayloads(payload);
                    break;

                case ODataPayloadElementType.ComplexInstanceCollection:
                case ODataPayloadElementType.PrimitiveCollection:
                    payloads = this.GenerateCollectionPayloads(payload);
                    break;

                case ODataPayloadElementType.BatchRequestPayload:
                case ODataPayloadElementType.BatchResponsePayload:
                case ODataPayloadElementType.ComplexInstance:
                case ODataPayloadElementType.DeferredLink:
                case ODataPayloadElementType.EmptyCollectionProperty:
                case ODataPayloadElementType.EmptyPayload:
                case ODataPayloadElementType.EmptyUntypedCollection:
                case ODataPayloadElementType.ExpandedLink:
                case ODataPayloadElementType.HtmlErrorPayload:
                case ODataPayloadElementType.LinkCollection:
                case ODataPayloadElementType.MetadataPayloadElement:
                case ODataPayloadElementType.ODataErrorPayload:
                case ODataPayloadElementType.PrimitiveValue:
                case ODataPayloadElementType.Unknown:
                default:
                    throw new NotSupportedException();
            }

            if (validators != null)
            {
                payloads = payloads.Select(p =>
                {
                    T result = (T)p.Clone();
                    result.SkipTestConfiguration = tc => (p.SkipTestConfiguration == null ? false : p.SkipTestConfiguration(tc)) || ODataPayloadElementConfigurationValidator.GetSkipTestConfiguration(p.PayloadElement, validators)(tc);
                    return result;
                });
            }

            return payloads;
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="feedPayload"/>.
        /// </summary>
        /// <param name="feedPayload">The feed payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="feedPayload"/>.</returns>
        private IEnumerable<T> GenerateFeedPayloads<T>(T feedPayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsTrue(feedPayload.PayloadElement is EntitySetInstance, "Expected an EntitySetInstance as payload.");

            // return the feed as top-level feed
            yield return feedPayload.FilterTopLevelFeed();

            // return the feed as content of an expanded link of a top-level entry
            yield return feedPayload.InEntryWithExpandedLink();

            // return the feed as content of an expanded link of the only entry in a top-level
            yield return feedPayload.InEntryWithExpandedLink().InFeed().FilterTopLevelFeed();

            // return the feed as content of an expanded link of the second entry in a top-level feed with three entries
            yield return feedPayload.InEntryWithExpandedLink().InFeed(3, PayloadGenerator.NextLink, 1, 1).FilterTopLevelFeed();

            // return the feed as content of an expanded link of the second entry in a top-level feed with three entries and a negative inline count
            yield return feedPayload.InEntryWithExpandedLink().InFeed(-3, PayloadGenerator.NextLink, 1, 1).FilterTopLevelFeed();

            // return the feed as content of an expanded link of an entry that itself is the content of an expanded link of a top-level entry
            yield return feedPayload.InEntryWithExpandedLink().InEntryWithExpandedLink(isSingletonRelationship: true);

            // return the feed as content of an expanded link within a 7 level hierachy of feeds within expanded links of a top level entry
            yield return feedPayload.InEntryWithExpandedLink().InEntryWithNestedExpandedFeeds(nestingLevel: 7);
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="propertyPayload"/>.
        /// </summary>
        /// <param name="propertyPayload">The property payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="propertyPayload"/>.</returns>
        private IEnumerable<T> GeneratePropertyPayloads<T>(T propertyPayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsTrue(propertyPayload.PayloadElement is PropertyInstance, "Expected a PropertyInstance as payload.");

            // return the property as a top-level property
            yield return propertyPayload;

            // return the property as the only property of a top-level complex property
            yield return propertyPayload.InComplexValue().InProperty();

            // return the property as part of a bigger top-level complex property
            yield return propertyPayload.InComplexValue(5, 5).InProperty();

            // return the property as the only property of a top-level entity
            yield return propertyPayload.InEntity();

            // return the property as part of a bigger top-level entity
            yield return propertyPayload.InEntity(15, 15);

            // return property in Complex Value which is in a MultiValue
            yield return propertyPayload.InComplexValue().InCollection().InProperty();

            // return property in Complex Value which is in a MultiValue with other complex values in the collection
            yield return propertyPayload.InComplexValue().InCollection(3, 3).InProperty();

            // return property in a complex value in a Collection which is a property of a complex value in a Collection
            yield return propertyPayload.InComplexValue().InCollection(1, 1).InProperty("InnerProperty").InComplexValue().InCollection(3, 3).InProperty();
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="entryPayload"/>.
        /// </summary>
        /// <param name="entryPayload">The entry payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="entryPayload"/>.</returns>
        private IEnumerable<T> GenerateEntryPayloads<T>(T entryPayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsTrue(entryPayload.PayloadElement is EntityInstance, "Expected an EntityInstance as payload.");

            // return as top level entry
            yield return entryPayload;

            // return the entry in another entry as ExpandedLink
            yield return entryPayload.InEntryWithExpandedLink(isSingletonRelationship: true);

            // return the entry in a feed with various combinations of nextLink, inlineCount and position of the entry w.r.t other entries
            yield return entryPayload.InFeed().FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(nextLink: PayloadGenerator.NextLink).FilterTopLevelFeed();
            yield return entryPayload.InFeed(elementsBefore: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(elementsBefore: 1, elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: -2, elementsBefore: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: 2, elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: -3, elementsBefore: 1, elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: 3, elementsBefore: 1, elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: -2, nextLink: PayloadGenerator.NextLink, elementsBefore: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: 2, nextLink: PayloadGenerator.NextLink, elementsAfter: 1).FilterTopLevelFeed();
            yield return entryPayload.InFeed(inlineCount: 2, nextLink: PayloadGenerator.NextLink, elementsBefore: 1, elementsAfter: 1).FilterTopLevelFeed();

            // return the entry in a feed that is inside an expanded link of a top-level entry
            yield return entryPayload.InFeed().InEntryWithExpandedLink();

            // return the entry in a feed with other entries that is inside an expanded link of a top-level entry
            yield return entryPayload.InFeed(null, null, 2, 1).InEntryWithExpandedLink();

            // return the entry in an expanded link of another entry that is inside a feed which in turn is inside an expanded link of a top-level entry
            yield return entryPayload.InEntryWithExpandedLink(isSingletonRelationship: true).InFeed().InEntryWithExpandedLink();

            // return the entry in a feed that is within a hierachy of 7 expanded feeds which are nested within a top-level entry
            yield return entryPayload.InEntryWithNestedExpandedFeeds(nestingLevel: 7);
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="navPropertyPayload"/>.
        /// </summary>
        /// <param name="navPropertyPayload">The navigation property payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="navPropertyPayload"/>.</returns>
        /// <remarks>
        /// This method assumes that the <paramref name="navPropertyPayload"/> represents a deferred navigation property. It will
        /// return the navigation property as deferred link first and will then expand it to singleton and collection
        /// expanded navigation properties.
        /// </remarks>
        private IEnumerable<T> GenerateNavigationPropertyPayloads<T>(T navPropertyPayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsNotNull(navPropertyPayload, "navPropertyPayload != null");
            this.Assert.IsNotNull(navPropertyPayload.PayloadElement, "navPropertyPayload.PayloadElement != null");

            NavigationPropertyInstance navPropertyInstance = navPropertyPayload.PayloadElement as NavigationPropertyInstance;
            this.Assert.IsNotNull(navPropertyInstance, "navPropertyInstance != null");
            this.Assert.IsTrue(navPropertyInstance.Value is DeferredLink, "Expected a deferred link navigation property.");

            // return the navigation property as as part of an entry with deferred link
            yield return navPropertyPayload.InEntity();

            // return the navigation property as as part of an entry with deferred link
            yield return navPropertyPayload.InEntity(2, 2);

            // return the navigation property as part of an entry with an expanded (singleton) link
            yield return navPropertyPayload.ExpandNavigationProperty(true).InEntity();

            // return the navigation property as part of an entry with an expanded (singleton) link
            yield return navPropertyPayload.ExpandNavigationProperty(true).InEntity(2, 2);

            // return the navigation property as part of an entry (incl. nav props) with an expanded (singleton) link
            yield return navPropertyPayload.ExpandNavigationProperty(true).InEntity(15, 15);

            // return the navigation property as part of an entry with an expanded (multi) link (with a single entry)
            yield return navPropertyPayload.ExpandNavigationProperty(false, 1).InEntity();

            // return the navigation property as part of an entry with an expanded (multi) link (with multiple entries)
            yield return navPropertyPayload.ExpandNavigationProperty(false, 3).InEntity();

            // return the navigation property as part of an entry with an expanded (multi) link
            yield return navPropertyPayload.ExpandNavigationProperty(false, 3).InEntity(2, 2);

            // return the navigation property as part of an entry (incl. nav props) with an expanded (multi) link
            yield return navPropertyPayload.ExpandNavigationProperty(false, 3).InEntity(15, 15);

            // return the navigation property as part of an entry with an expanded (multi) link with next link
            yield return navPropertyPayload.ExpandNavigationProperty(false, 3, "http://odata.org/nextlink").InEntity(2, 2);
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="streamReferencePayload"/>.
        /// </summary>
        /// <param name="streamReferencePayload">The stream reference value payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="streamReferencePayload"/>.</returns>
        /// <remarks>
        /// This method assumes that the <paramref name="streamReferencePayload"/> represents a single stream reference value (named stream). 
        /// It will return the stream reference value as part of different entity payloads.
        /// </remarks>
        private IEnumerable<T> GenerateNamedStreamPayloads<T>(T streamReferencePayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsNotNull(streamReferencePayload, "streamReferencePayload != null");
            this.Assert.IsNotNull(streamReferencePayload.PayloadElement, "streamReferencePayload.PayloadElement != null");

            NamedStreamInstance namedStreamInstance = streamReferencePayload.PayloadElement as NamedStreamInstance;
            this.Assert.IsNotNull(namedStreamInstance, "namedStreamInstance != null");

            // return the named stream as the only property in an entity (not counting the required key property)
            yield return streamReferencePayload.InEntity();

            // return the named stream with some properties before and after it
            yield return streamReferencePayload.InEntity(2, 2);

            // return the named stream with many properties before and after it
            yield return streamReferencePayload.InEntity(15, 15);
        }

        /// <summary>
        /// Generates all interesting reader payloads for the given <paramref name="collectionPayload"/>.
        /// </summary>
        /// <param name="collectionPayload">The collection payload to use in the generated payloads.</param>
        /// <returns>All interesting reader payloads for the given <paramref name="collectionPayload"/>.</returns>
        /// <remarks>
        /// This method assumes that the <paramref name="collectionPayload"/> represents a collection of primitive or complex instances. 
        /// </remarks>
        private IEnumerable<T> GenerateCollectionPayloads<T>(T collectionPayload) where T : PayloadTestDescriptor
        {
            this.Assert.IsNotNull(collectionPayload, "collectionPayload != null");

            // NOTE: collections are currently only supported at the top-level
            yield return collectionPayload;
        }

        private EntityInstance CreateEntityInstance(IEdmEntityType edmEntityType)
        {
            EntityInstance entity = new EntityInstance(edmEntityType.FullName(), false);
            foreach (var property in edmEntityType.Properties())
            {
                if (!(property is IEdmNavigationProperty))
                {
                    entity.Property(this.BuildProperty(property));
                }
            }

            return entity;
        }

        private PropertyInstance BuildProperty(IEdmProperty property)
        {
            if (property.Type.IsCollection())
            {
                var type = (IEdmCollectionType)property.Type.Definition;
                if (type.ElementType.IsComplex())
                {
                    var complex = new ComplexInstance(property.Type.FullName(), false).WithTypeAnnotation(type.ElementType.Definition);
                    foreach (var prop in ((IEdmComplexType)type.ElementType.Definition).StructuralProperties())
                    {
                        complex.Add(this.BuildProperty(prop));
                    }

                    return new ComplexMultiValueProperty(property.Name, 
                        new ComplexMultiValue(property.Type.FullName(), false, complex)).WithTypeAnnotation(type);
                }

                return new PrimitiveMultiValueProperty(property.Name, 
                    new PrimitiveMultiValue(property.Type.FullName(), false, 
                        this.GetValue((IEdmPrimitiveTypeReference) type.ElementType))).WithTypeAnnotation(property.Type);

            }

            if (property.IsComplex())
            {
                var complex = new ComplexInstance(property.Type.FullName(), false);
                foreach (var prop in ((IEdmComplexType)property.Type.Definition).StructuralProperties())
                {
                    complex.Add(this.BuildProperty(prop));
                }

                return new ComplexProperty(property.Name, complex).WithTypeAnnotation(property.Type);
            }

            return new PrimitiveProperty((property.Name), this.GetValue((IEdmPrimitiveTypeReference)property.Type)).WithTypeAnnotation(property.Type);
        }
    
        private PrimitiveValue GetValue(IEdmPrimitiveTypeReference edmTypeReference)
        {
            switch (edmTypeReference.PrimitiveDefinition().PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return new PrimitiveValue("Edm.Binary", 45).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Boolean:
                    return new PrimitiveValue("Edm.Boolean", false).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Byte:
                    return new PrimitiveValue("Edm.Byte", (byte)1).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return new PrimitiveValue("Edm.DateTimeOffset", new DateTimeOffset(new DateTime(2013, 10, 17), new TimeSpan(0, 0, 3, 0))).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Decimal:
                    return new PrimitiveValue("Edm.Decimal", (decimal)4.3).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Double:
                    return new PrimitiveValue("Edm.Double", (double)54.3333333333333).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Duration:
                    return new PrimitiveValue("Edm.Duration", new TimeSpan(1, 3, 0, 0)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Geography:
                    return new PrimitiveValue("Edm.Geography", GeographyFactory.Point(5.3, 3.5).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyPoint:
                    return new PrimitiveValue("Edm.GeographyPoint", GeographyFactory.Point(3.5, 3.55555555).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return new PrimitiveValue("Edm.GeographyPolygon", GeographyFactory.Polygon().Ring(3, 4).Ring(4, 5).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyCollection:
                    return new PrimitiveValue("Edm.GeographyCollection", GeographyFactory.Collection().Point(2, 3).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyLineString:
                    return new PrimitiveValue("Edm.GeographyLineString", GeographyFactory.LineString(5.3, 3.3)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return new PrimitiveValue("Edm.GeographyMultiLineString", GeographyFactory.MultiLineString().LineString(3, 4).LineString(5, 4).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return new PrimitiveValue("Edm.GeographyMultiPoint", GeographyFactory.MultiPoint().Point(3, 2).Point(6, 4).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return new PrimitiveValue("Edm.GeographyMultiPolygon", GeographyFactory.MultiPolygon().Polygon().Ring(65, 23).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Geometry:
                    return new PrimitiveValue("Edm.Geometry", GeometryFactory.Point(-3, 4, 5, 6)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return new PrimitiveValue("Edm.GeometryCollection", GeometryFactory.Collection().Point(3, 2, 3, null).Point(3, 2, 10, null)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryLineString:
                    return new PrimitiveValue("Edm.GeometryLineString", GeometryFactory.LineString(4.2, 12.3)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return new PrimitiveValue("Edm.GeometryMultiLineString", GeometryFactory.MultiLineString().LineString(3, 2).LineTo(3, 5).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new PrimitiveValue("Edm.GeometryMultiPoint", GeometryFactory.MultiPoint().Point(3, 2).Point(4.3, 3.9).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return new PrimitiveValue("Edm.GeometryMultiPolygon", GeometryFactory.MultiPolygon().Polygon().Ring(65, 23).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryPoint:
                    return new PrimitiveValue("Edm.GeometryPoint", GeometryFactory.Point(-2.3, 3.9)).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return new PrimitiveValue("Edm.GeometryPolygon", GeometryFactory.Polygon().Ring(3, 4).Ring(4, 5).Build()).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Guid:
                    return new PrimitiveValue("Edm.Guid", new Guid("00005259-2341-5431-5432-234234234234")).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Int16:
                    return new PrimitiveValue("Edm.Int16", (Int16)6).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Int32:
                    return new PrimitiveValue("Edm.Int32", 12).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Int64:
                    return new PrimitiveValue("Edm.Int64", (Int64)18).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.SByte:
                    return new PrimitiveValue("Edm.SByte", (sbyte)-3).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Single:
                    return new PrimitiveValue("Edm.Single", (Single)5.4).WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.Stream:
                    return new PrimitiveValue("Edm.Stream", "232312").WithTypeAnnotation(edmTypeReference);
                case EdmPrimitiveTypeKind.String:
                    return new PrimitiveValue("Edm.String", "Hello").WithTypeAnnotation(edmTypeReference);
                default:
                    throw new NotSupportedException("Primitive type kind not supported, please add new type kind.");
            }
        }

        /// <summary>
        /// Adds ID and self link to the payload if missing.
        /// </summary>
        /// <param name="payload"> payload under consideration</param>
        /// <returns>payload with id and self link added</returns>
        private EntityInstance AddIDAndLink(EntityInstance payload)
        {
            string id = "http://" + TestAtomConstants.AtomGuid + ".com/";
            if (payload.GetSelfLink() == null && payload.GetEditLink() == null)
            {
                payload.Annotations.Add(new SelfLinkAnnotation(id));
            }

            if (payload.Id == null)
            {
                payload.Id = id ;
            }

            return payload;
        }
    }
}
