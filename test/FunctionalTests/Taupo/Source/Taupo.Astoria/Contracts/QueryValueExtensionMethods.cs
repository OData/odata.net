//---------------------------------------------------------------------
// <copyright file="QueryValueExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Extension methods that make writing astoria tests easier, aimed at Metadata helpers
    /// </summary>
    public static class QueryValueExtensionMethods
    {
        /// <summary>
        /// Synchronizes the entity represented by the given query structural value
        /// </summary>
        /// <param name="synchronizer">The synchronizer to extend</param>
        /// <param name="continuation">The continuation to use</param>
        /// <param name="existingEntity">The representation of the entity to synchronize</param>
        public static void SynchronizeEntity(this IAsyncDataSynchronizer synchronizer, IAsyncContinuation continuation, QueryStructuralValue existingEntity)
        {
            ExceptionUtilities.CheckArgumentNotNull(synchronizer, "synchronizer");
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(existingEntity, "existingEntity");

            var entityType = existingEntity.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Given structural value was not an entity type");
            string entitySetName = entityType.EntitySet.Name;
            var keyValues = entityType.Properties.Where(p => p.IsPrimaryKey).Select(p => new NamedValue(p.Name, existingEntity.GetScalarValue(p.Name).Value));
            synchronizer.SynchronizeEntityInstanceGraph(continuation, entitySetName, keyValues);
        }

        /// <summary>
        /// Returns the key property values for the given entity instance
        /// </summary>
        /// <param name="instance">The instance to get key property values from</param>
        /// <returns>The key values of the given instance</returns>
        public static IEnumerable<NamedValue> GetKeyPropertyValues(this QueryStructuralValue instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            return instance.Type.Properties.Where(p => p.IsPrimaryKey).Select(p => new NamedValue(p.Name, instance.GetScalarValue(p.Name).Value));
        }

        /// <summary>
        /// Applies the Take operation on the given collection.
        /// </summary>
        /// <param name="collection">The collection to take from</param>
        /// <param name="takeCount">How many elements to take.</param>
        /// <returns>Collection with the applied Take operation.</returns>
        public static QueryCollectionValue Take(this QueryCollectionValue collection, int takeCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            return collection.Take(collection.Type.EvaluationStrategy.IntegerType.CreateValue(takeCount));
        }

        /// <summary>
        /// Returns the stream property values for the given entity instance
        /// </summary>
        /// <param name="instance">The instance to get stream property values from</param>
        /// <param name="namedStream">The named stream.</param>
        /// <returns>The stream values of the given instance</returns>
        public static AstoriaQueryStreamValue GetStreamValue(this QueryStructuralValue instance, string namedStream)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(namedStream, "namedStream");
            return (AstoriaQueryStreamValue)instance.GetValue(namedStream);
        }

        /// <summary>
        /// Sets the value of a stream member.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="namedStream">The named stream.</param>
        /// <param name="value">The value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Base is abstract")]
        public static void SetStreamValue(this QueryStructuralValue instance, string namedStream, AstoriaQueryStreamValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(namedStream, "namedStream");

            instance.AssertPropertyType<AstoriaQueryStreamType>(namedStream);

            instance.SetValue(namedStream, value);
        }

        /// <summary>
        /// Returns the default stream property values for the given entity instance
        /// </summary>
        /// <param name="instance">The instance to get stream property values from</param>
        /// <returns>The stream values of the given instance</returns>
        public static AstoriaQueryStreamValue GetDefaultStreamValue(this QueryStructuralValue instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            return instance.GetStreamValue(AstoriaQueryStreamType.DefaultStreamPropertyName);
        }

        /// <summary>
        /// Sets the value of a default stream member.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Need to enforce that only the derived type(QueryStreamValue) is passed to the method instead of base type 'QueryValue'")]
        public static void SetDefaultStreamValue(this QueryStructuralValue instance, AstoriaQueryStreamValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            instance.SetStreamValue(AstoriaQueryStreamType.DefaultStreamPropertyName, value);
        }

        /// <summary>
        /// Generates the conventional id for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="generator">The generator to extend</param>
        /// <param name="instance">The entity instance</param>
        /// <returns>The convention-based id</returns>
        public static string GenerateEntityId(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance)
        {
            return generator.EnsureIsEntityAndCall(instance, generator.GenerateEntityId);
        }

        /// <summary>
        /// Generates the conventional edit link for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="generator">The generator to extend</param>
        /// <param name="instance">The entity instance</param>
        /// <returns>The convention-based edit link</returns>
        public static string GenerateEntityEditLink(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance)
        {
            return generator.EnsureIsEntityAndCall(instance, generator.GenerateEntityEditLink);
        }

        /// <summary>
        /// Generates the conventional stream edit link for the given entity: http://serviceRoot/SetName(keyValues)/$value
        /// </summary>
        /// <param name="generator">The generator to extend</param>
        /// <param name="instance">The entity instance</param>
        /// <returns>The convention-based stream edit link</returns>
        public static string GenerateDefaultStreamEditLink(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance)
        {
            return generator.EnsureIsEntityAndCall(instance, generator.GenerateDefaultStreamEditLink);
        }

        /// <summary>
        /// Generates the conventional edit link for the given named stream: http://serviceRoot/SetName(keyValues)/StreamName
        /// </summary>
        /// <param name="generator">The generator to extend</param>
        /// <param name="instance">The entity instance</param>
        /// <param name="streamName">The stream's name</param>
        /// <returns>The convention-based stream edit link</returns>
        public static string GenerateNamedStreamEditLink(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance, string streamName)
        {
            return generator.EnsureIsEntityAndCall(instance, (entitySet, entityType, keys) => generator.GenerateNamedStreamEditLink(entitySet, entityType, keys, streamName));
        }

        /// <summary>
        /// Generates either the default stream link or a named stream link based on whether the given stream name is null
        /// </summary>
        /// <param name="generator">The generator to extend</param>
        /// <param name="instance">The entity instance</param>
        /// <param name="streamName">The stream's name or null to indicate the default stream</param>
        /// <returns>The convention-based stream edit link</returns>
        public static string GenerateStreamEditLink(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance, string streamName)
        {
            if (streamName == null)
            {
                return generator.GenerateDefaultStreamEditLink(instance);
            }
            else
            {
                return generator.GenerateNamedStreamEditLink(instance, streamName);
            }
        }

        /// <summary>
        /// Determines whether the current value is from a dynamic property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>whether the current value is from a dynamic property</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific annotation is only relevant for values")]
        public static bool IsDynamicPropertyValue(this QueryValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            return value.Annotations.OfType<IsDynamicPropertyValueAnnotation>().Any();
        }

        /// <summary>
        /// Marks the value as being from a dynamic property and returns it.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The given value.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific annotation is only relevant for values")]
        public static TValue AsDynamicPropertyValue<TValue>(this TValue value) where TValue : QueryValue
        {
            value.AddAnnotationIfNotExist<QueryAnnotation>(IsDynamicPropertyValueAnnotation.Instance);
            return value;
        }

        /// <summary>
        /// Marks the values that do not have defined properties as being dynamic values.
        /// </summary>
        /// <param name="value">The structural value.</param>
        public static void MarkDynamicPropertyValues(this QueryStructuralValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            var entityType = value.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Instance was not an entity type");
            if (entityType.EntityType.IsOpen)
            {
                foreach (var memberName in value.MemberNames)
                {
                    if (!entityType.EntityType.AllProperties.Any(p => p.Name == memberName && p.IsMetadataDeclaredProperty()))
                    {
                        MarkPropertyAsDynamic(value, memberName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the expected etag for the stream value, taking into account that empty etags become null
        /// </summary>
        /// <param name="streamValue">The stream value to get the expected etag for</param>
        /// <returns>The expected etag</returns>
        internal static string GetExpectedETag(this AstoriaQueryStreamValue streamValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamValue, "streamValue");
            
            // empty stream etags are not written out
            var streamETag = streamValue.ETag;
            if (string.IsNullOrEmpty(streamETag))
            {
                streamETag = null;
            }

            return streamETag;
        }

        /// <summary>
        /// Sets the values of a stream property
        /// </summary>
        /// <param name="instance">The entity which contains the stream property</param>
        /// <param name="name">The name of the stream property or null to indicate the default stream</param>
        /// <param name="contentType">The stream's content type</param>
        /// <param name="etag">The stream's etag</param>
        /// <param name="editLink">The stream's edit-link</param>
        /// <param name="selfLink">The stream's self-link</param>
        /// <param name="content">The stream's content</param>
        internal static void SetStreamValue(this QueryStructuralValue instance, string name, string contentType, string etag, string editLink, string selfLink, byte[] content)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            if (name == null)
            {
                name = AstoriaQueryStreamType.DefaultStreamPropertyName;
            }

            var value = instance.GetStreamValue(name);

            // if its null, it may not have been initialized at all, so reassign it to be safe
            if (value.IsNull)
            {
                instance.SetStreamValue(name, value);
            }

            value.Value = content;
            value.ContentType = contentType;
            value.ETag = etag;

            value.EditLink = CreateRelativeOrAbsoluteUriOrReturnNull(editLink);
            value.SelfLink = CreateRelativeOrAbsoluteUriOrReturnNull(selfLink);
        }

        private static Uri CreateRelativeOrAbsoluteUriOrReturnNull(string linkValue)
        {
            if (linkValue == null)
            {
                return null;
            }

            return new Uri(linkValue, UriKind.RelativeOrAbsolute);
        }

        private static string EnsureIsEntityAndCall(this IODataConventionBasedLinkGenerator generator, QueryStructuralValue instance, Func<EntitySet, EntityType, IEnumerable<NamedValue>, string> callback)
        {
            ExceptionUtilities.CheckArgumentNotNull(generator, "generator");
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(callback, "callback");

            var queryEntityType = instance.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Entity's type was not a query entity type. Type was: {0}", instance.Type);

            return callback(queryEntityType.EntitySet, queryEntityType.EntityType, instance.GetKeyPropertyValues());
        }

        private static void MarkDynamicSubPropertyValues(QueryStructuralValue value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            var complexType = value.Type as QueryComplexType;
            ExceptionUtilities.CheckObjectNotNull(complexType, "Instance was not a complex type");

            foreach (var memberName in value.MemberNames)
            {
                MarkPropertyAsDynamic(value, memberName);
            }
        }

        private static void MarkPropertyAsDynamic(QueryStructuralValue value, string memberName)
        {
            var propertyValue = value.GetValue(memberName);
            if (propertyValue.Type is QueryComplexType)
            {
                MarkDynamicSubPropertyValues((QueryStructuralValue)propertyValue);
            }

            propertyValue.AsDynamicPropertyValue();
        }
    }
}
