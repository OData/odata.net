//---------------------------------------------------------------------
// <copyright file="DataServiceContextData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Respresents data for the DataServiceContext (from Microsoft.OData.Client namespace).
    /// </summary>
    public sealed class DataServiceContextData
    {
        private readonly List<EntityDescriptorData> entityDatas = new List<EntityDescriptorData>();
        private readonly List<LinkDescriptorData> linkDatas = new List<LinkDescriptorData>();
        private long nextChangeOrder = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceContextData"/> class.
        /// </summary>
        public DataServiceContextData()
            : this(typeof(DSClient.DataServiceContext), DataServiceProtocolVersion.V4)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceContextData"/> class.
        /// </summary>
        /// <param name="contextType">The type of the context this context data is associated with</param>
        /// <param name="maxProtocolVersion">The max protocol version of the context</param>
        public DataServiceContextData(Type contextType, DataServiceProtocolVersion maxProtocolVersion)
        {
            this.ContextType = contextType;
            this.MaxProtocolVersion = maxProtocolVersion;
            this.HttpStack = "ClientStack";
        }

        /// <summary>
        /// Gets the entity descriptors data.
        /// </summary>
        /// <value>The entity descriptors data.</value>
        public IEnumerable<EntityDescriptorData> EntityDescriptorsData
        {
            get { return this.entityDatas.AsEnumerable(); }
        }

        /// <summary>
        /// Gets the link descriptors data.
        /// </summary>
        /// <value>The link descriptors data.</value>
        public IEnumerable<LinkDescriptorData> LinkDescriptorsData
        {
            get { return this.linkDatas.AsEnumerable(); }
        }

        /// <summary>
        /// Gets or sets the base uri of the context data.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the merge option
        /// </summary>
        public MergeOption MergeOption { get; set; }

        /// <summary>
        /// Gets or sets the response preference
        /// </summary>
        public DataServiceResponsePreference AddAndUpdateResponsePreference { get; set; }

        /// <summary>
        /// Gets or sets the entity set resolver to use
        /// </summary>
        public Func<string, Uri> ResolveEntitySet { get; set; }

        /// <summary>
        /// Gets or sets the type resolver to use
        /// </summary>
        public Func<string, Type> ResolveType { get; set; }

        /// <summary>
        /// Gets or sets the type name resolver to use
        /// </summary>
        public Func<Type, string> ResolveName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to tunnel verbs through POST
        /// </summary>
        public bool UsePostTunneling { get; set; }

        /// <summary>
        /// Gets the MaxProtocolVersion
        /// </summary>
        public DataServiceProtocolVersion MaxProtocolVersion { get; private set; }

        /// <summary>
        /// Gets or sets the HttpStack
        /// </summary>
        public string HttpStack { get; set; }

        /// <summary>
        /// Gets the type of the context this context-data is associated with
        /// </summary>
        internal Type ContextType { get; private set; }

        /// <summary>
        /// Creates the data service context data from data service context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="maxProtocolVersion">The max protocol version.</param>
        /// <returns>DataServiceContextData based on context</returns>
        public static DataServiceContextData CreateDataServiceContextDataFromDataServiceContext(DSClient.DataServiceContext context, DataServiceProtocolVersion maxProtocolVersion)
        {
            var contextData = new DataServiceContextData(context.GetType(), maxProtocolVersion);

            // set all the properties on the context data to the values from the context
            foreach (var productProperty in typeof(DSClient.DataServiceContext).GetProperties(true, false))
            {
                var testProperty = typeof(DataServiceContextData).GetProperty(productProperty.Name, true, false);
                if (testProperty != null && testProperty.PropertyType.IsAssignableFrom(productProperty.PropertyType))
                {
                    var value = productProperty.GetValue(context, null);
                    testProperty.SetValue(contextData, value, null);
                }
            }

            return contextData;
        }

        /// <summary>
        /// Useses the XML HTTP stack.
        /// </summary>
        /// <returns>True if its using the XmlHttp stack</returns>
        public bool UsesXmlHttpStack()
        {
            if (this.HttpStack == "XmlHttp")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the ordered changes.
        /// </summary>
        /// <returns>Returnes descriptor data representing changes ordered based on the chnage order.</returns>
        public IEnumerable<DescriptorData> GetOrderedChanges()
        {
            // Note: Union is not supported in Silverlight
            var list = this.entityDatas.Cast<DescriptorData>().ToList();
            list.AddRange(this.linkDatas.Cast<DescriptorData>());
            list.AddRange(this.entityDatas.SelectMany(e => e.StreamDescriptors).Cast<DescriptorData>());

            // remove unchanged descriptors
            list = list.Where(delegate(DescriptorData descriptor)
            {
                var entityDescriptor = descriptor as EntityDescriptorData;

                // for MLEs, even if the entity is unchanged, we include it if the MR has changed
                if (entityDescriptor != null && entityDescriptor.IsMediaLinkEntry && entityDescriptor.DefaultStreamState != EntityStates.Unchanged)
                {
                    return true;
                }

                return descriptor.State != EntityStates.Unchanged;
            }).ToList();

            // sort by change order
            return list.OrderBy(e => e.ChangeOrder);
        }

        /// <summary>
        /// Creates an entity descriptor data and puts it into the list of entity descriptors data.
        /// <see cref="ChangeStateAndChangeOrder"/> for the restrictions on the state and change order.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="changeOrder">The change order.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The created entity descriptor data.</returns>
        /// <exception cref="TaupoArgumentException">
        /// When entity is null.
        /// </exception>
        public EntityDescriptorData CreateEntityDescriptorData(EntityStates state, long changeOrder, object entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            var data = new EntityDescriptorData(this, entity);
            this.ChangeStateAndChangeOrder(data, state, changeOrder);

            this.entityDatas.Add(data);

            return data;
        }

        /// <summary>
        /// Creates an entity descriptor data and puts it into the list of entity descriptors data.
        /// <see cref="ChangeStateAndChangeOrder"/> for the restrictions on the state and change order.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="changeOrder">The change order.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="entityClrType">The client clr type of the entity.</param>
        /// <returns>The created entity descriptor data.</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When entity is null or identity is null.
        /// </exception>
        /// <exception cref="TaupoArgumentException">
        /// When identity is empty.
        /// </exception>
        /// <exception cref="TaupoArgumentException">
        /// When state is Added. Use overload that takes entity instance as an input
        /// as in this case entity instance should always be known.
        /// </exception>
        /// <remarks>
        /// This method is useful when expressing expectations for the LoadProperty method
        /// as in this case there may be no knowledge about entity instance.
        /// When entity instance is known use overload that takes entity instance as an input.
        /// </remarks>
        public EntityDescriptorData CreateEntityDescriptorData(EntityStates state, long changeOrder, Uri identity, Type entityClrType)
        {
            if (state == EntityStates.Added)
            {
                throw new TaupoArgumentException("State cannot be Added. Use overload that takes entity as an input argument.");
            }

            var data = new EntityDescriptorData(this, identity, entityClrType)
                .SetIdentity(identity);
            this.ChangeStateAndChangeOrder(data, state, changeOrder);

            this.entityDatas.Add(data);

            return data;
        }

        /// <summary>
        /// Creates the link descriptor data and puts in into the list of link descriptors data.
        /// <see cref="ChangeStateAndChangeOrder"/> for the restrictions on the state and change order.        
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="changeOrder">The change order.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <returns>The created link descriptor data.</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When source or sourcePropertyName is null.
        /// </exception>
        /// <exception cref="TaupoArgumentException">
        /// When sourcePropertyName is empty.
        /// </exception>
        /// <exception cref="TaupoInvalidOperationException">
        /// When entity descriptor data is not found for the source or target (if target is not null).
        /// </exception>
        public LinkDescriptorData CreateLinkDescriptorData(EntityStates state, long changeOrder, object source, string sourcePropertyName, object target)
        {
            CheckLinkArguments(source, sourcePropertyName);

            EntityDescriptorData sourceData = this.FindEntityData(source);
            if (sourceData == null)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not find entity descriptor data for the source: {0}.", source));
            }

            // Target can be null
            EntityDescriptorData targetData = null;
            if (target != null)
            {
                targetData = this.FindEntityData(target);

                if (targetData == null)
                {
                    throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not find entity descriptor data for the target: {0}.", target));
                }
            }

            return this.CreateLinkDescriptorData(state, changeOrder, sourceData, sourcePropertyName, targetData);
        }

        /// <summary>
        /// Creates the link descriptor data and puts in into the list of link descriptors data.
        /// <see cref="ChangeStateAndChangeOrder"/> for the restrictions on the state and change order.        
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="changeOrder">The change order.</param>
        /// <param name="sourceData">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="targetData">The target.</param>
        /// <returns>The created link descriptor data.</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When source or sourcePropertyName is null.
        /// </exception>
        /// <exception cref="TaupoArgumentException">
        /// When sourcePropertyName is empty.
        /// </exception>
        /// <exception cref="TaupoInvalidOperationException">
        /// When entity descriptor data is not found for the source or target (if target is not null).
        /// </exception>
        public LinkDescriptorData CreateLinkDescriptorData(EntityStates state, long changeOrder, EntityDescriptorData sourceData, string sourcePropertyName, EntityDescriptorData targetData)
        {
            ExceptionUtilities.CheckArgumentNotNull(sourceData, "sourceData");
            ExceptionUtilities.CheckArgumentNotNull(sourcePropertyName, "sourcePropertyName");

            var linkData = new LinkDescriptorData(sourceData, sourcePropertyName, targetData);
            this.ChangeStateAndChangeOrder(linkData, state, changeOrder);

            this.linkDatas.Add(linkData);

            return linkData;
        }

        /// <summary>
        /// Changes the state and change order.
        /// </summary>
        /// <param name="descriptorData">The descriptor data.</param>
        /// <param name="state">The state.</param>
        /// <param name="changeOrder">The change order.</param>
        /// <remarks>There is no check that change order is unique as it's expensive.
        /// Use <see cref="GetNextChangeOrder"/> to treat the change (if any) as latest.
        /// </remarks>
        /// <exception cref="TaupoArgumentNullException">
        /// When descriptorData is null.
        /// </exception>
        /// <exception cref="TaupoInvalidOperationException">
        /// When state is Detached, change order is less then 0 or change order is 0 and state is not Unchanged.
        /// </exception>
        public void ChangeStateAndChangeOrder(DescriptorData descriptorData, EntityStates state, long changeOrder)
        {
            ExceptionUtilities.CheckArgumentNotNull(descriptorData, "descriptorData");

            if (state == EntityStates.Detached)
            {
                throw new TaupoInvalidOperationException("Cannot create descriptor in a Detached state.");
            }

            if (changeOrder < 0)
            {
                throw new TaupoInvalidOperationException("Change order cannot be less then 0.");
            }

            if (changeOrder == 0 && state != EntityStates.Unchanged)
            {
                throw new TaupoInvalidOperationException("Change order cannot be 0 when state is not Unchanged.");
            }

            //// Note: there is no check that change order is unique as it's expensive.

            descriptorData.State = state;
            descriptorData.ChangeOrder = changeOrder;

            if (this.nextChangeOrder <= descriptorData.ChangeOrder && descriptorData.ChangeOrder != uint.MaxValue)
            {
                this.nextChangeOrder = descriptorData.ChangeOrder + 1;
            }
        }

        /// <summary>
        /// Gets the next change order which can be used to put descriptor data
        /// at the end of ordered changes list when creating entity or link descriptor data
        /// <seealso cref="CreateEntityDescriptorData(EntityStates, int, object)"/>
        /// <seealso cref="CreateEntityDescriptorData(EntityStates, int, string, Type)"/>
        /// <seealso cref="CreateLinkDescriptorData"/>
        /// </summary>
        /// <returns>The next change order</returns>
        public long GetNextChangeOrder()
        {
            if (this.nextChangeOrder >= uint.MaxValue)
            {
                throw new TaupoInvalidOperationException("Next change order cannot be obtained as it's reached the limit.");
            }

            return this.nextChangeOrder;
        }

        /// <summary>
        /// Gets the entity descriptor data for the specified entity instance.
        /// </summary>
        /// <param name="entity">The entity instance for which to return the entity descriptor data.</param>
        /// <returns>The entity descriptor data for the entity.</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When entity is null.
        /// </exception>
        /// <exception cref="TaupoInvalidOperationException">
        /// When entity descriptor data for the specified entity does not exist.
        /// </exception>
        public EntityDescriptorData GetEntityDescriptorData(object entity)
        {
            EntityDescriptorData descriptorData;

            if (!this.TryGetEntityDescriptorData(entity, out descriptorData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "No entity descriptor data is found for the specified entity: {0}.", entity));
            }

            return descriptorData;
        }

        /// <summary>
        /// Tries to get entity descriptor data for the specified entity instance.
        /// </summary>
        /// <param name="entity">The entity instance for which to return the entity descriptor data.</param>
        /// <param name="entityData">The entity descriptor data to be retrieved.</param>
        /// <returns>True if entity descriptor data is found, false otherwise.</returns>
        /// <exception cref="TaupoArgumentNullException">
        /// When entity is null.
        /// </exception>        
        public bool TryGetEntityDescriptorData(object entity, out EntityDescriptorData entityData)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");

            entityData = this.FindEntityData(entity);

            return entityData != null;
        }

        /// <summary>
        /// Gets the link descriptor data for the specified link.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <returns>The link descriptor data for the link.</returns>
        /// <exception cref="TaupoInvalidOperationException">
        /// When link descriptor data the specified link does not exist.
        /// </exception>
        public LinkDescriptorData GetLinkDescriptorData(object source, string sourcePropertyName, object target)
        {
            LinkDescriptorData linkData;

            if (!this.TryGetLinkDescriptorData(source, sourcePropertyName, target, out linkData))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Link descriptor data is not found for the specified link: source = {{{0}}}, source property name='{1}', target = {{{2}}}.", source, sourcePropertyName, target));
            }

            return linkData;
        }

        /// <summary>
        /// Tries to get link descriptor data for the specified link.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <param name="linkData">The link descriptor data to be retrieved.</param>
        /// <returns>True if link descriptor data is found, false otherwise.</returns>
        public bool TryGetLinkDescriptorData(object source, string sourcePropertyName, object target, out LinkDescriptorData linkData)
        {
            CheckLinkArguments(source, sourcePropertyName);

            // Consider using dictionary for faster look-up and to avoid duplicates.
            var found = this.LinkDescriptorsData.Where(e => IsMatch(e.SourceDescriptor, source) && 
                ((target == null && e.TargetDescriptor == null) || (e.TargetDescriptor != null && IsMatch(e.TargetDescriptor, target)))
                && e.SourcePropertyName == sourcePropertyName).ToList();

            if (found.Count > 1)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Multiple link descriptors data found for the specified link: source = {{{0}}}, source property name='{1}', target = {{{2}}}.", source, sourcePropertyName, target));
            }

            linkData = found.FirstOrDefault();

            return linkData != null;
        }

        /// <summary>
        /// Gets the stream descriptor data with the given name for the given entity
        /// </summary>
        /// <param name="entity">The entity to get the stream descriptor for</param>
        /// <param name="name">The name of the stream descriptor</param>
        /// <returns>The stream descriptor data</returns>
        public StreamDescriptorData GetStreamDescriptorData(object entity, string name)
        {
            StreamDescriptorData streamData;
            ExceptionUtilities.Assert(this.TryGetStreamDescriptorData(entity, name, out streamData), "Could not find stream descriptor with name '{0}'", name);
            return streamData;
        }

        /// <summary>
        /// Tries to get the stream descriptor data with the given name for the given entity
        /// </summary>
        /// <param name="entity">The entity to get the stream descriptor for</param>
        /// <param name="name">The name of the stream descriptor</param>
        /// <param name="streamData">The stream descriptor data</param>
        /// <returns>True if the stream descriptor data was found, otherwise false</returns>
        public bool TryGetStreamDescriptorData(object entity, string name, out StreamDescriptorData streamData)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(name, "name");

            streamData = null;
            EntityDescriptorData entityData;
            if (!this.TryGetEntityDescriptorData(entity, out entityData))
            {
                streamData = null;
                return false;
            }

            var found = entityData.StreamDescriptors.Where(n => n.Name == name).ToList();
            ExceptionUtilities.Assert(found.Count <= 1, "Multiple stream descriptors found with name '{0}'. Entity = {1}", name, entityData);
            if (found.Count < 1)
            {
                return false;
            }

            streamData = found[0];
            return true;
        }

        /// <summary>
        /// Removes the entity descriptor data for the specified entity.
        /// </summary>
        /// <param name="entity">The entity for which to return descriptor data.</param>
        /// <returns>False if descriptor data is not found for the specified entity, true otherwise.</returns>
        public bool RemoveEntityDescriptorData(object entity)
        {
            EntityDescriptorData descriptorData;
            if (!this.TryGetEntityDescriptorData(entity, out descriptorData))
            {
                return false;
            }

            this.RemoveEntityData(descriptorData);
            return true;
        }

        /// <summary>
        /// Removes the link descriptor data for the specified link.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <returns>False if descriptor data is not found for the specified link, true otherwise.</returns>
        public bool RemoveLinkDescriptorData(object source, string sourcePropertyName, object target)
        {
            LinkDescriptorData linkData;
            if (!this.TryGetLinkDescriptorData(source, sourcePropertyName, target, out linkData))
            {
                return false;
            }

            this.RemoveLinkData(linkData);
            return true;
        }

        /// <summary>
        /// Removes the descriptor data.
        /// </summary>
        /// <param name="descriptorData">The descriptor data.</param>
        /// <returns>False if the descriptor data is not found, true otherwise.</returns>
        public bool RemoveDescriptorData(DescriptorData descriptorData)
        {
            ExceptionUtilities.CheckArgumentNotNull(descriptorData, "descriptorData");

            var entityData = descriptorData as EntityDescriptorData;

            if (entityData != null)
            {
                return this.RemoveEntityData(entityData);
            }
            else
            {
                return this.RemoveLinkData((LinkDescriptorData)descriptorData);
            }
        }

        /// <summary>
        /// Generates a dump of the contents of the <see cref="DataServiceContextData"/> (suitable for tracing).
        /// </summary>
        /// <returns>Contents of the data service context data rendered as a string.</returns>
        public string ToTraceString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Entity descriptors: " + this.entityDatas.Count);
            builder.AppendLine("Link descriptors: " + this.linkDatas.Count);

            // Note: Union is not supported in Silverlight
            var list = this.entityDatas.Cast<DescriptorData>().ToList();
            list.AddRange(this.linkDatas.Cast<DescriptorData>());
            foreach (var data in list.OrderBy(e => e.ChangeOrder))
            {
                builder.Append("\t");
                builder.AppendLine(data.ToString());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Clones the current data service context data
        /// </summary>
        /// <returns>A clone of the current data service context data</returns>
        public DataServiceContextData Clone()
        {
            var clone = new DataServiceContextData(this.ContextType, this.MaxProtocolVersion);
            clone.nextChangeOrder = this.nextChangeOrder;

            if (this.BaseUri != null)
            {
                clone.BaseUri = new Uri(this.BaseUri.OriginalString, UriKind.RelativeOrAbsolute);
            }
            
            clone.AddAndUpdateResponsePreference = this.AddAndUpdateResponsePreference;
            clone.MergeOption = this.MergeOption;
            clone.ResolveEntitySet = this.ResolveEntitySet;
            clone.ResolveName = this.ResolveName;
            clone.ResolveType = this.ResolveType;
            clone.UsePostTunneling = this.UsePostTunneling;
            clone.HttpStack = this.HttpStack;

            var mapping = new Dictionary<EntityDescriptorData, EntityDescriptorData>(ReferenceEqualityComparer.Create<EntityDescriptorData>());
            foreach (var entityDescriptor in this.entityDatas)
            {
                var clonedDescriptor = entityDescriptor.Clone(clone);
                mapping[entityDescriptor] = clonedDescriptor;
                clone.entityDatas.Add(clonedDescriptor);
            }

            foreach (var linkDescriptor in this.linkDatas)
            {
                EntityDescriptorData clonedSource = null;
                EntityDescriptorData clonedTarget = null;
                mapping.TryGetValue(linkDescriptor.SourceDescriptor, out clonedSource);
                if (linkDescriptor.TargetDescriptor != null)
                {
                    mapping.TryGetValue(linkDescriptor.TargetDescriptor, out clonedTarget);
                }

                var clonedDescriptor = linkDescriptor.Clone(clonedSource, clonedTarget);
                clone.linkDatas.Add(clonedDescriptor);
            }

            return clone;
        }

        private static void CheckLinkArguments(object source, string sourcePropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            //// Note: target can be null

            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(sourcePropertyName, "sourcePropertyName");
        }

        private static bool IsMatch(EntityDescriptorData entityData, object entityIdentity)
        {
            //// entityIdentity could be
            ////      1. EntityDescriptorData => compare by reference
            ////      2. Entity instatnce => compare entity by reference
            ////      3. identity string => compare as strings

            ExceptionUtilities.Assert(entityData != null, "entityDescriptorData cannot be null!");
            return entityData == entityIdentity || entityData.Entity == entityIdentity || (entityData.Identity == (entityIdentity as Uri) && entityData.Identity != null);
        }

        private static void SetDetachedState(DescriptorData data)
        {
            data.State = EntityStates.Detached;
        }

        private EntityDescriptorData FindEntityData(object entityIdentity)
        {
            // Consider using dictionary for faster look-up and to avoid duplicates.
            var found = this.entityDatas.Where(e => IsMatch(e, entityIdentity)).ToList();
            if (found.Count > 1)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Multiple entity descriptors found matching entity: {0}.", entityIdentity));
            }

            return found.FirstOrDefault();
        }

        private bool RemoveEntityData(EntityDescriptorData entityData)
        {
            if (!this.entityDatas.Remove(entityData))
            {
                return false;
            }

            entityData.RemoveStreamDescriptorDatas();
            entityData.RemoveLinkInfoDatas();

            SetDetachedState(entityData);
            return true;
        }

        private bool RemoveLinkData(LinkDescriptorData linkData)
        {
            if (!this.linkDatas.Remove(linkData))
            {
                return false;
            }

            SetDetachedState(linkData);
            return true;
        }
    }
}
