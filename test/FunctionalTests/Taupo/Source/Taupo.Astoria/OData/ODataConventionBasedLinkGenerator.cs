//---------------------------------------------------------------------
// <copyright file="ODataConventionBasedLinkGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Default implementation of the component which generates OData links based on conventions
    /// </summary>
    [ImplementationName(typeof(IODataConventionBasedLinkGenerator), "Default")]
    public class ODataConventionBasedLinkGenerator : IODataConventionBasedLinkGenerator
    {
        /// <summary>
        /// Gets or sets the uri to string converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriToStringConverter { get; set; }

        /// <summary>
        /// Gets or sets the service descriptor
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaServiceDescriptor Service { get; set; }

        /// <summary>
        /// Generates the conventional id for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The entity id</returns>
        public string GenerateEntityEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues)
        {
            // null checks performed by this helper
            var uri = this.GenerateEntityEditLinkUri(entitySet, entityType, keyValues);
            return this.UriToStringConverter.ConvertToString(uri);
        }

        /// <summary>
        /// Generates the conventional edit link for the given entity: http://serviceRoot/SetName(keyValues)
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The convention-based edit link</returns>
        public string GenerateEntityId(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues)
        {
            // null checks performed by this helper
            var uri = this.GenerateEntityIdUri(entitySet, entityType, keyValues);
            return this.UriToStringConverter.ConvertToString(uri);
        }

        /// <summary>
        /// Generates the conventional stream edit link for the given entity: http://serviceRoot/SetName(keyValues)/$value
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <returns>The convention-based stream edit link</returns>
        public string GenerateDefaultStreamEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues)
        {
            // null checks performed by this helper
            var uri = this.GenerateEntityEditLinkUri(entitySet, entityType, keyValues);

            uri.Segments.Add(SystemSegment.Value);

            return this.UriToStringConverter.ConvertToString(uri);
        }

        /// <summary>
        /// Generates the conventional edit link for the given named stream: http://serviceRoot/SetName(keyValues)/StreamName
        /// </summary>
        /// <param name="entitySet">The entity set of the entity</param>
        /// <param name="entityType">The type of the entity</param>
        /// <param name="keyValues">They values of the entity's key</param>
        /// <param name="streamName">The stream's name</param>
        /// <returns>The convention-based stream edit link</returns>
        public string GenerateNamedStreamEditLink(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues, string streamName)
        {
            // null checks performed by this helper
            var uri = this.GenerateEntityEditLinkUri(entitySet, entityType, keyValues);

            // null checks performed by ODataUriBuilder
            uri.Segments.Add(ODataUriBuilder.NamedStream(entityType, streamName));

            return this.UriToStringConverter.ConvertToString(uri);
        }

        /// <summary>
        /// Returns a value indicating whether edit links for entities in the given set should contain type segments
        /// </summary>
        /// <param name="entitySet">The entity set</param>
        /// <param name="entityType">The entity type</param>
        /// <returns>True if the type-segment should be in the edit link, otherwise false</returns>
        internal static bool EditLinkShouldContainTypeSegment(EntitySet entitySet, EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container, "Entity set '{0}' did not have its container set", entitySet.Name);
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container.Model, "Entity container '{0}' did not have its model set", entitySet.Container.Name);

            return entitySet.EntityType != entityType;
        }

        private ODataUri GenerateEntityIdUri(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // null checks performed by ODataUriBuilder
            var conventionalId = new ODataUri();
            conventionalId.Segments.Add(new ServiceRootSegment(this.Service.ServiceUri));
            conventionalId.Segments.Add(ODataUriBuilder.EntitySet(entitySet));
            conventionalId.Segments.Add(ODataUriBuilder.Key(entityType, keyValues));
            return conventionalId;
        }

        private ODataUri GenerateEntityEditLinkUri(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> keyValues)
        {
            // null checks performed by ODataUriBuilder
            var conventionalEditLink = this.GenerateEntityIdUri(entitySet, entityType, keyValues);
            if (EditLinkShouldContainTypeSegment(entitySet, entityType))
            {
                conventionalEditLink.Segments.Add(ODataUriBuilder.EntityType(entityType));
            }

            return conventionalEditLink;
        }
    }
}
