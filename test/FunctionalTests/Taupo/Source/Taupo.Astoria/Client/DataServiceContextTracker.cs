//---------------------------------------------------------------------
// <copyright file="DataServiceContextTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Tracks DataServiceContext operations.
    /// </summary>
    public static class DataServiceContextTracker
    {
        /// <summary>
        /// Tracks the AddObject method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="entity">The entity.</param>
        public static void TrackAddObject(this DataServiceContextData data, string entitySetName, object entity)
        {
            CheckEntitySetName(ref entitySetName);
            CheckEntityIsNotTracked(data, entity);

            var entityDescriptorData = data.CreateEntityDescriptorData(EntityStates.Added, data.GetNextChangeOrder(), entity)
                .SetEntitySetName(entitySetName);

            if (data.ResolveEntitySet != null)
            {
                entityDescriptorData.InsertLink = data.ResolveEntitySet(entitySetName);
            }
            else
            {
                entityDescriptorData.InsertLink = new Uri(UriHelpers.ConcatenateUriSegments(data.BaseUri.OriginalString, entitySetName));
            }
        }

        /// <summary>
        /// Tracks the AddRelatedObject method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        public static void TrackAddRelatedObject(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            CheckEntityIsNotTracked(data, target);

            EntityDescriptorData sourceDescriptorData = GetTrackedEntityDescriptorData(data, source, "Cannot add link:", "source");
            CheckStateIsNot(EntityStates.Deleted, sourceDescriptorData, "Cannot add related object:", "source");

            EntityDescriptorData targetDescriptorData = data.CreateEntityDescriptorData(EntityStates.Added, data.GetNextChangeOrder(), target);

            data.CreateLinkDescriptorData(EntityStates.Added, uint.MaxValue, sourceDescriptorData, sourcePropertyName, targetDescriptorData);

            targetDescriptorData
                .SetParentForInsert(source)
                .SetParentPropertyForInsert(sourcePropertyName);
        }

        /// <summary>
        /// Tracks the AttachTo method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="identity">The identity.</param>
        public static void TrackAttachTo(this DataServiceContextData data, string entitySetName, object entity, Uri identity)
        {
            TrackAttachTo(data, entitySetName, entity, identity, null);
        }

        /// <summary>
        /// Tracks the AttachTo method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="entityETag">The etag for the entity.</param>
        public static void TrackAttachTo(this DataServiceContextData data, string entitySetName, object entity, Uri identity, string entityETag)
        {
            TrackAttachTo(data, entitySetName, entity, identity, entityETag, null);
        }

        /// <summary>
        /// Tracks the AttachTo method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="entityETag">The etag for the entity.</param>
        /// <param name="editLink">The edit link for the entity</param>
        public static void TrackAttachTo(this DataServiceContextData data, string entitySetName, object entity, Uri identity, string entityETag, Uri editLink)
        {
            CheckEntitySetName(ref entitySetName);
            CheckEntityIsNotTracked(data, entity);
            CheckIdentity(data, identity);

            data.CreateEntityDescriptorData(EntityStates.Unchanged, data.GetNextChangeOrder(), entity)
                .SetEntitySetName(entitySetName)
                .SetETag(entityETag)
                .SetIdentity(identity)
                .SetEditLink(editLink);
        }

        /// <summary>
        /// Tracks the Detach method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the expected result of the Detach method call.</returns>
        public static bool TrackDetach(this DataServiceContextData data, object entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            EntityDescriptorData entityDescriptorData;

            if (data.TryGetEntityDescriptorData(entity, out entityDescriptorData))
            {
                if (data.EntityDescriptorsData.Any(ed => ed.ParentForInsert == entity && !object.ReferenceEquals(ed, entityDescriptorData)))
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Cannot detach entity as it's used as a parent for insert. Entity descriptor data: {0}.", entityDescriptorData));
                }

                foreach (LinkDescriptorData link in data.LinkDescriptorsData
                    .Where(l => l.SourceDescriptor == entityDescriptorData || l.TargetDescriptor == entityDescriptorData).ToList())
                {
                    data.RemoveDescriptorData(link);
                }

                return data.RemoveDescriptorData(entityDescriptorData);
            }

            return false;
        }

        /// <summary>
        /// Tracks the DeleteObject method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entity">The entity.</param>
        public static void TrackDeleteObject(this DataServiceContextData data, object entity)
        {
            var descriptorData = GetTrackedEntityDescriptorData(data, entity, "Failed to track DeleteObject:", "entity");
            if (descriptorData.State == EntityStates.Added)
            {
                data.RemoveDescriptorData(descriptorData);
            }
            else if (descriptorData.State != EntityStates.Deleted)
            {
                data.ChangeStateAndChangeOrder(descriptorData, EntityStates.Deleted, data.GetNextChangeOrder());
            }
        }

        /// <summary>
        /// Tracks the UpdateObject method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="entity">The entity.</param>
        public static void TrackUpdateObject(this DataServiceContextData data, object entity)
        {
            var descriptorData = GetTrackedEntityDescriptorData(data, entity, "Failed to track UpdateObject:", "entity");
            if (descriptorData.State == EntityStates.Unchanged)
            {
                data.ChangeStateAndChangeOrder(descriptorData, EntityStates.Modified, data.GetNextChangeOrder());
            }
        }

        /// <summary>
        /// Tracks the AddLink method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        public static void TrackAddLink(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            LinkDescriptorData linkDescriptorData;
            if (data.TryGetLinkDescriptorData(source, sourcePropertyName, target, out linkDescriptorData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "The link already exists: {0}.", linkDescriptorData.ToString()));
            }

            EntityDescriptorData sourceDescriptorData = GetTrackedEntityDescriptorData(data, source, "Cannot add link:", "source");
            CheckStateIsNot(EntityStates.Deleted, sourceDescriptorData, "Cannot add link:", "source");

            EntityDescriptorData targetDescriptorData = GetTrackedEntityDescriptorData(data, target, "Cannot add link:", "target");
            CheckStateIsNot(EntityStates.Deleted, targetDescriptorData, "Cannot add link:", "target");

            data.CreateLinkDescriptorData(EntityStates.Added, data.GetNextChangeOrder(), sourceDescriptorData, sourcePropertyName, targetDescriptorData);
        }

        /// <summary>
        /// Tracks the SetLink method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        public static void TrackSetLink(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(sourcePropertyName, "sourcePropertyName");

            EntityDescriptorData sourceDescriptorData = GetTrackedEntityDescriptorData(data, source, "Cannot set link:", "source");
            CheckStateIsNot(EntityStates.Deleted, sourceDescriptorData, "Cannot set link:", "source");

            EntityDescriptorData targetDescriptorData = null;
            if (target != null)
            {
                targetDescriptorData = GetTrackedEntityDescriptorData(data, target, "Cannot set link:", "target");
                CheckStateIsNot(EntityStates.Deleted, sourceDescriptorData, "Cannot set link:", "target");
            }

            var relatedToSource = data.LinkDescriptorsData.Where(e => e.SourceDescriptor.Entity == source && e.SourcePropertyName == sourcePropertyName).ToList();

            if (relatedToSource.Count > 1)
            {
                throw new TaupoInvalidOperationException("Cannot set link: source contains multiple links for the property: " + sourcePropertyName);
            }

            LinkDescriptorData existingLinkDescriptorData = relatedToSource.FirstOrDefault();

            if (existingLinkDescriptorData == null)
            {
                data.CreateLinkDescriptorData(EntityStates.Modified, data.GetNextChangeOrder(), sourceDescriptorData, sourcePropertyName, targetDescriptorData);
            }
            else
            {
                if (existingLinkDescriptorData.State != EntityStates.Modified || existingLinkDescriptorData.TargetDescriptor != targetDescriptorData)
                {
                    data.ChangeStateAndChangeOrder(existingLinkDescriptorData, EntityStates.Modified, data.GetNextChangeOrder());
                }

                existingLinkDescriptorData.TargetDescriptor = targetDescriptorData;
            }
        }

        /// <summary>
        /// Tracks the DeleteLink method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        public static void TrackDeleteLink(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            EntityDescriptorData sourceDescriptorData = GetTrackedEntityDescriptorData(data, source, "Cannot delete link:", "source");

            EntityDescriptorData targetDescriptorData = GetTrackedEntityDescriptorData(data, target, "Cannot delete link:", "target");

            LinkDescriptorData descriptorData;
            if (data.TryGetLinkDescriptorData(sourceDescriptorData, sourcePropertyName, targetDescriptorData, out descriptorData)
                && descriptorData.State == EntityStates.Added)
            {
                data.RemoveDescriptorData(descriptorData);
            }
            else
            {
                CheckStateIsNot(EntityStates.Added, sourceDescriptorData, "Cannot delete link:", "source");
                CheckStateIsNot(EntityStates.Added, targetDescriptorData, "Cannot delete link:", "target");

                if (descriptorData == null)
                {
                    data.CreateLinkDescriptorData(EntityStates.Deleted, data.GetNextChangeOrder(), sourceDescriptorData, sourcePropertyName, targetDescriptorData);
                }
                else if (descriptorData.State != EntityStates.Deleted)
                {
                    data.ChangeStateAndChangeOrder(descriptorData, EntityStates.Deleted, data.GetNextChangeOrder());
                }
            }
        }

        /// <summary>
        /// Tracks the AttachLink method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        public static void TrackAttachLink(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            LinkDescriptorData linkDescriptorData;
            if (data.TryGetLinkDescriptorData(source, sourcePropertyName, target, out linkDescriptorData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "The link already exists: {0}.", linkDescriptorData.ToString()));
            }

            EntityDescriptorData sourceDescriptorData = GetTrackedEntityDescriptorData(data, source, "Cannot attach link:", "source");
            CheckStateIsNot(EntityStates.Deleted, sourceDescriptorData, "Cannot attach link:", "source");
            CheckStateIsNot(EntityStates.Added, sourceDescriptorData, "Cannot attach link:", "source");

            EntityDescriptorData targetDescriptorData = null;
            if (target != null)
            {
                targetDescriptorData = GetTrackedEntityDescriptorData(data, target, "Cannot attach link:", "target");
                CheckStateIsNot(EntityStates.Deleted, targetDescriptorData, "Cannot attach link:", "target");
                CheckStateIsNot(EntityStates.Added, targetDescriptorData, "Cannot attach link:", "target");
            }

            data.CreateLinkDescriptorData(EntityStates.Unchanged, data.GetNextChangeOrder(), sourceDescriptorData, sourcePropertyName, targetDescriptorData);
        }

        /// <summary>
        /// Tracks the DetachLink method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns the expected result of the DetachLink method call. </returns>
        public static bool TrackDetachLink(this DataServiceContextData data, object source, string sourcePropertyName, object target)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            return data.RemoveLinkDescriptorData(source, sourcePropertyName, target);
        }

        /// <summary>
        /// Tracks the SaveChanges method.
        /// </summary>
        /// <param name="data">The data service context data on which to apply state transition.</param>
        /// <param name="options">The options.</param>
        /// <param name="response">The response.</param>
        /// <param name="cachedOperationsFromResponse">The individual operation respones, pre-enumerated and cached.</param>
        /// <param name="tracker">The entity data change tracker to use</param>
        public static void TrackSaveChanges(this DataServiceContextData data, SaveChangesOptions options, DSClient.DataServiceResponse response, IEnumerable<DSClient.OperationResponse> cachedOperationsFromResponse, IEntityDescriptorDataChangeTracker tracker)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckArgumentNotNull(cachedOperationsFromResponse, "cachedOperationsFromResponse");
            ExceptionUtilities.CheckArgumentNotNull(tracker, "tracker");

            // Check options and response consistency
            if ((options & SaveChangesOptions.ContinueOnError) == 0)
            {
                ExceptionUtilities.Assert(response.Count(r => r.Error != null) == 0, "Check save changes options and response consistency: no errors in the response when ContinueOnError is off.");
            }

            // because some links will not have separate requests, we need to keep track of all link changes
            var allPendingLinkChanges = data.GetOrderedChanges().OfType<LinkDescriptorData>().ToList();

            // go through the pending changes and update the states based on whether the request succeeded
            foreach (DSClient.ChangeOperationResponse changeResponse in cachedOperationsFromResponse)
            {
                DescriptorData descriptorData;
                LinkDescriptorData linkDescriptorData = null;
                StreamDescriptorData streamDescriptorData = null;
                var entityDescriptor = changeResponse.Descriptor as DSClient.EntityDescriptor;
                if (entityDescriptor != null)
                {
                    descriptorData = data.GetEntityDescriptorData(entityDescriptor.Entity);
                }
                else
                {
                    var linkDescriptor = changeResponse.Descriptor as DSClient.LinkDescriptor;
                    if (linkDescriptor != null)
                    {
                        linkDescriptorData = data.GetLinkDescriptorData(linkDescriptor.Source, linkDescriptor.SourceProperty, linkDescriptor.Target);
                        descriptorData = linkDescriptorData;
                        allPendingLinkChanges.Remove(linkDescriptorData);
                    }
                    else
                    {
                        // for stream descriptors, we need to find the parent descriptor, then get the stream descriptor data from it                        
                        var streamDescriptor = (DSClient.StreamDescriptor)changeResponse.Descriptor;
                        
                        entityDescriptor = streamDescriptor.EntityDescriptor;
                        streamDescriptorData = data.GetStreamDescriptorData(entityDescriptor.Entity, streamDescriptor.StreamLink.Name);
                        descriptorData = streamDescriptorData;
                    }
                }

                // don't update states for responses that indicate failure
                if (changeResponse.Error != null)
                {
                    continue;
                }

                // because the request succeeded, make the corresponding updates to the states
                if (descriptorData.State == EntityStates.Deleted ||
                        (linkDescriptorData != null &&
                        linkDescriptorData.State == EntityStates.Modified &&
                        linkDescriptorData.SourceDescriptor.State == EntityStates.Deleted))
                {
                    data.RemoveDescriptorData(descriptorData);
                }
                else
                {
                    // for non-deleted descriptors, we need to update states based on the headers
                    var entityDescriptorData = descriptorData as EntityDescriptorData;
                    if (entityDescriptorData != null)
                    {
                        if (entityDescriptorData.IsMediaLinkEntry && entityDescriptorData.DefaultStreamState == EntityStates.Modified)
                        {
                            entityDescriptorData.DefaultStreamDescriptor.UpdateFromHeaders(changeResponse.Headers);
                            entityDescriptorData.DefaultStreamState = EntityStates.Unchanged;
                        }
                        else
                        {
                            if (entityDescriptorData.IsMediaLinkEntry && entityDescriptorData.DefaultStreamState == EntityStates.Added)
                            {
                                entityDescriptorData.DefaultStreamState = EntityStates.Unchanged;
                            }

                            // because there might have been a reading-entity event for this entity, we need to apply the headers through the tracker
                            tracker.TrackUpdateFromHeaders(entityDescriptorData, changeResponse.Headers);

                            // ensure that all updates are applied before moving to the next response
                            tracker.ApplyPendingUpdates(entityDescriptorData);

                            entityDescriptorData.ParentForInsert = null;
                            entityDescriptorData.ParentPropertyForInsert = null;
                            entityDescriptorData.InsertLink = null;
                        }
                    }
                    else if (streamDescriptorData != null)
                    {
                        streamDescriptorData.UpdateFromHeaders(changeResponse.Headers);
                    }

                    descriptorData.State = EntityStates.Unchanged;
                }
            }

            // go through each link change that did not have an assocatiated response and update its state
            foreach (var linkDescriptorData in allPendingLinkChanges.OfType<LinkDescriptorData>())
            {
                if (linkDescriptorData.State == EntityStates.Added || linkDescriptorData.State == EntityStates.Modified)
                {
                    linkDescriptorData.State = EntityStates.Unchanged;
                }
            }
        }

        /// <summary>
        /// Tracks a call to SetSaveStream
        /// </summary>
        /// <param name="data">The context data.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="name">The name of the stream or null to indicate the default stream.</param>
        /// <param name="saveStreamLogger">A logger for the save stream</param>
        /// <param name="closeStream">A value indicating whether or not to close the stream</param>
        /// <param name="headers">The headers for the request</param>
        public static void TrackSetSaveStream(this DataServiceContextData data, object entity, string name, IStreamLogger saveStreamLogger, bool closeStream, IEnumerable<KeyValuePair<string, string>> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckArgumentNotNull(saveStreamLogger, "saveStreamLogger");
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            EntityDescriptorData descriptorData;
            ExceptionUtilities.Assert(data.TryGetEntityDescriptorData(entity, out descriptorData), "Cannot set save stream on an entity that is not being tracked");

            StreamDescriptorData streamDescriptor;
            if (name == null)
            {
                if (descriptorData.State == EntityStates.Added)
                {
                    descriptorData.DefaultStreamState = EntityStates.Added;
                }
                else
                {
                    descriptorData.DefaultStreamState = EntityStates.Modified;
                }

                streamDescriptor = descriptorData.DefaultStreamDescriptor;
            }
            else
            {
                streamDescriptor = descriptorData.StreamDescriptors.SingleOrDefault(n => n.Name == name);
                if (streamDescriptor == null)
                {
                    streamDescriptor = descriptorData.CreateStreamDescriptorData(EntityStates.Modified, data.GetNextChangeOrder(), name);
                }
                else
                {
                    data.ChangeStateAndChangeOrder(streamDescriptor, EntityStates.Modified, data.GetNextChangeOrder());
                }
            }

            // we need to trap what the product reads from the stream to compare it against what is sent on the wire
            streamDescriptor.SaveStream = new SaveStreamData(saveStreamLogger, closeStream, headers);
        }

        /// <summary>
        /// Tracks a call to GetReadStream
        /// </summary>
        /// <param name="data">The context data</param>
        /// <param name="entity">The entity</param>
        /// <param name="name">The name of the stream or null to indicate the default stream</param>
        /// <param name="contentType">The content type from the returned stream response</param>
        /// <param name="headers">The headers from the returned stream response</param>
        public static void TrackGetReadStream(this DataServiceContextData data, object entity, string name, string contentType, IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");

            EntityDescriptorData descriptorData;
            ExceptionUtilities.Assert(data.TryGetEntityDescriptorData(entity, out descriptorData), "Cannot set save stream on an entity that is not being tracked");

            if (name == null)
            {
                string etag;
                if (headers.TryGetValue(HttpHeaders.ETag, out etag))
                {
                    descriptorData.StreamETag = etag;
                }
            }
            else
            {
                var streamDescriptor = descriptorData.StreamDescriptors.SingleOrDefault(n => n.Name == name);
                ExceptionUtilities.CheckObjectNotNull(streamDescriptor, "Could not find stream descriptor with name '{0}' on entity descriptor: {1}", name, descriptorData);

                streamDescriptor.ContentType = contentType;
                streamDescriptor.UpdateFromHeaders(headers);
            }
        }

        private static EntityDescriptorData GetTrackedEntityDescriptorData(DataServiceContextData data, object entity, string errorMessage, string argumentName)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            EntityDescriptorData descriptorData;
            if (!data.TryGetEntityDescriptorData(entity, out descriptorData))
            {
                throw new TaupoInvalidOperationException(errorMessage + " " + argumentName + " is not tracked by the data service context data.");
            }

            return descriptorData;
        }

        private static void CheckEntityIsNotTracked(DataServiceContextData data, object entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            EntityDescriptorData descriptorData;
            if (data.TryGetEntityDescriptorData(entity, out descriptorData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Specified entity is already tracked by the descriptor data: {0}.", descriptorData.ToString()));
            }
        }

        private static void CheckStateIsNot(EntityStates state, DescriptorData descriptorData, string errorMessage, string argumentName)
        {
            if (descriptorData.State == state)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "{0} {1} is in {2} state.", errorMessage, argumentName, state));
            }
        }

        private static void CheckIdentity(DataServiceContextData data, Uri identity)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            if (identity == null)
            {
                throw new TaupoInvalidOperationException("Entity identity cannot be null or empty.");
            }

            EntityDescriptorData descriptorData;
            if (data.TryGetEntityDescriptorData(identity, out descriptorData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "There is already an entity descriptor data with the specified identity: {0}.", descriptorData.ToString()));
            }
        }

        private static void CheckEntitySetName(ref string entitySetName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");

            entitySetName = entitySetName.Trim('/');

            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");
        }
    }
}
