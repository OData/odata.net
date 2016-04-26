//---------------------------------------------------------------------
// <copyright file="OperationSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Delegate for abstracting away a call to IDataServiceActionProvider.AdvertiseServiceAction.
    /// </summary>
    /// <param name="serviceAction">Service action to be advertised.</param>
    /// <param name="resourceInstance">Instance of the resource to which the service action is bound.</param>
    /// <param name="resourceInstanceInFeed">true if the resource instance to be serialized is inside a feed; false otherwise. The value true
    /// suggests that this method might be called many times during serialization since it will get called once for every resource instance inside
    /// the feed. If it is an expensive operation to determine whether to advertise the service action for the <paramref name="resourceInstance"/>,
    /// the provider may choose to always advertise in order to optimize for performance.</param>
    /// <param name="actionToSerialize">The <see cref="Microsoft.OData.ODataAction"/> to be serialized. The server constructs 
    /// the version passed into this call, which may be replaced by an implementation of this interface.
    /// This should never be set to null unless returning false.</param>
    /// <returns>true if the service action should be advertised; false otherwise.</returns>
    internal delegate bool AdvertiseServiceActionCallback(OperationWrapper serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize);

    /// <summary>
    /// Converts action/function metadata into ODataLib object-model instances using an action provider.
    /// </summary>
    internal class OperationSerializer
    {
        /// <summary>
        /// Storage for the payload metadata property manager to use.
        /// </summary>
        private readonly PayloadMetadataPropertyManager metadataPropertyManager;

        /// <summary>
        /// Storage for the metadata query option interpreter to use.
        /// </summary>
        private readonly PayloadMetadataParameterInterpreter payloadMetadataParameterInterpreter;

        /// <summary>
        /// The link builder to use when serializing operations.
        /// </summary>
        private readonly OperationLinkBuilder operationLinkBuilder;

        /// <summary>
        /// Callback for determining whether to advertise a given action.
        /// </summary>
        private readonly AdvertiseServiceActionCallback advertiseServiceAction;

        /// <summary>
        /// The current format being serialized.
        /// </summary>
        private readonly ODataFormat format;

        /// <summary>
        /// The metadata uri of the service.
        /// </summary>
        private readonly Uri metadataUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationSerializer"/> class.
        /// </summary>
        /// <param name="payloadMetadataParameterInterpreter">The metadata query option interpreter </param>
        /// <param name="metadataPropertyManager">The metadata property manager.</param>
        /// <param name="advertiseServiceAction">The callback to use for determining whether an action should be advertised. Should wrap a call to IDataServiceActionProvider.AdvertiseServiceAction.</param>
        /// <param name="namespaceName">Namespace of the operation.</param>
        /// <param name="format">The current format being serialized into.</param>
        /// <param name="metadataUri">The metadata uri of the service.</param>
        internal OperationSerializer(
            PayloadMetadataParameterInterpreter payloadMetadataParameterInterpreter,
            PayloadMetadataPropertyManager metadataPropertyManager, 
            AdvertiseServiceActionCallback advertiseServiceAction, 
            string namespaceName, 
            ODataFormat format, 
            Uri metadataUri)
        {
            Debug.Assert(metadataPropertyManager != null, "metadataPropertyManager != null");
            Debug.Assert(payloadMetadataParameterInterpreter != null, "payloadMetadataParameterInterpreter != null");
            Debug.Assert(advertiseServiceAction != null, "advertiseServiceAction != null");

            this.payloadMetadataParameterInterpreter = payloadMetadataParameterInterpreter;
            this.metadataPropertyManager = metadataPropertyManager;

            this.operationLinkBuilder = new OperationLinkBuilder(namespaceName, metadataUri);
            this.format = format;
            this.metadataUri = metadataUri;
            
            this.advertiseServiceAction = advertiseServiceAction;
        }

        /// <summary>
        /// Serializes the given operations and returns the resulting instances of <see cref="ODataAction"/>.
        /// </summary>
        /// <param name="entityToSerialize">The entity to serialize.</param>
        /// <param name="resourceInstanceInFeed">Whether or not the entity is being serialized in a feed.</param>
        /// <param name="operationWrappers">The wrapped operations to serialize.</param>
        /// <returns>The serialized actions.</returns>
        internal IEnumerable<ODataAction> SerializeOperations(EntityToSerialize entityToSerialize, bool resourceInstanceInFeed, ICollection<OperationWrapper> operationWrappers)
        {
            Debug.Assert(operationWrappers != null, "serviceOperationWrapperList != null");

            var odataActions = new List<ODataAction>(operationWrappers.Count);
            
            // Create a hashset containing the names of all actions in the current scope that are duplicates.
            HashSet<string> collidingActionNames = GetCollidingActionNames(operationWrappers);
            foreach (OperationWrapper wrapper in operationWrappers)
            {
                bool actionNameHasCollision = collidingActionNames.Contains(wrapper.Name);

                ODataAction odataAction;
                if (this.TrySerializeOperation(entityToSerialize, resourceInstanceInFeed, actionNameHasCollision, wrapper, out odataAction))
                {
                    odataActions.Add(odataAction);
                }
            }

            return odataActions;
        }

        /// <summary>
        /// Creates a hash-set containing only the names of actions that have colliding names.
        /// </summary>
        /// <param name="operations">The operations to check for collisions.</param>
        /// <returns>The hash-set with the colliding names.</returns>
        private static HashSet<string> GetCollidingActionNames(IEnumerable<OperationWrapper> operations)
        {
            return operations
                .Select(wrapper => wrapper.Name)
                .Duplicates(StringComparer.Ordinal)
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <summary>
        /// Tries to serialize the operation.
        /// </summary>
        /// <param name="entityToSerialize">The entity to serialize.</param>
        /// <param name="resourceInstanceInFeed">Whether or not the entity is being serialized in a feed.</param>
        /// <param name="entityHasMultipleActionsWithSameName">Whether or not there are multiple operations in the current scope with the same name as the current operation.</param>
        /// <param name="serviceOperationWrapper">The service operation wrapper.</param>
        /// <param name="odataAction">The ODL object-model representation of the action.</param>
        /// <returns>Whether or not to serialize the operation.</returns>
        private bool TrySerializeOperation(EntityToSerialize entityToSerialize, bool resourceInstanceInFeed, bool entityHasMultipleActionsWithSameName, OperationWrapper serviceOperationWrapper, out ODataAction odataAction)
        {
            Debug.Assert(serviceOperationWrapper != null, "serviceOperationWrapper != null");

            // We only advertise actions. This is a debug assert because GetServiceOperationsByResourceType only returns actions.
            Debug.Assert(serviceOperationWrapper.Kind == OperationKind.Action, "Only actions can be advertised");

            Uri metadata = this.operationLinkBuilder.BuildMetadataLink(serviceOperationWrapper, entityHasMultipleActionsWithSameName);

            // If the action has OperationParameterBindingKind set to "Always" then we advertise the action without calling "AdvertiseServiceAction".
            bool isAlwaysAvailable = serviceOperationWrapper.OperationParameterBindingKind == OperationParameterBindingKind.Always;

            odataAction = new ODataAction { Metadata = metadata };

            // There is some subtlety to the interaction between action advertisement and whether or not to include title/target on the wire.
            // 
            // 1) If an action is always available:
            //    The provider author does not get a chance to customize the title/target values...
            //    so the values will be based on conventions...
            //    so by default do not write them on the wire
            // 2) If it is only sometimes available:
            //    The values need to be computed to provide them on the instance given to the provider...
            //    but they might not be changed by the provider author...
            //    so compare them to the computed values, and do not write them if they match.
            
            // TODO: Action provider should be able to customize title/target even if the action is 'always' advertised
            // If this gets fixed, then all the behavior should collapse to emulate case #2 above

            // Create a lazy Uri for the target, because we may need it more than once (see case #2 above).
            SimpleLazy<Uri> lazyActionTargetUri = new SimpleLazy<Uri>(() => this.operationLinkBuilder.BuildTargetLink(entityToSerialize, serviceOperationWrapper, entityHasMultipleActionsWithSameName));

            this.metadataPropertyManager.SetTitle(odataAction, isAlwaysAvailable, serviceOperationWrapper.Name);
            this.metadataPropertyManager.SetTarget(odataAction, isAlwaysAvailable, () => lazyActionTargetUri.Value);

            // If the operation is always available,
            // 1. Return true for MetadataQueryOption.All.
            // 2. Return false for MetadataQueryOption.None.
            // 3. Return false for MetadataQueryOption.Default.
            if (isAlwaysAvailable)
            {
                return this.payloadMetadataParameterInterpreter.ShouldIncludeAlwaysAvailableOperation();
            }

            return this.AskProviderIfActionShouldBeAdvertised(entityToSerialize, resourceInstanceInFeed, serviceOperationWrapper, lazyActionTargetUri, entityHasMultipleActionsWithSameName, ref odataAction);
        }

        /// <summary>
        /// Asks the provider if the action should be advertised in payloads.
        /// </summary>
        /// <param name="entityToSerialize">The entity to serialize.</param>
        /// <param name="resourceInstanceInFeed">Whether or not the entity is being serialized in a feed.</param>
        /// <param name="serviceOperationWrapper">The service operation wrapper.</param>
        /// <param name="lazyActionTargetUri">Target uri of the action, which will only be generated if needed.</param>
        /// <param name="entityHasMultipleActionsWithSameName">Whether or not there are multiple operations in the current scope with the same name as the current operation.</param>
        /// <param name="odataAction">The ODL object-model representation of the action.</param>
        /// <returns>Whether or not the action should be advertised.</returns>
        private bool AskProviderIfActionShouldBeAdvertised(EntityToSerialize entityToSerialize, bool resourceInstanceInFeed, OperationWrapper serviceOperationWrapper, SimpleLazy<Uri> lazyActionTargetUri, bool entityHasMultipleActionsWithSameName, ref ODataAction odataAction)
        {
            if (this.advertiseServiceAction(serviceOperationWrapper, entityToSerialize.Entity, resourceInstanceInFeed, ref odataAction))
            {
                if (odataAction == null)
                {
                    throw new DataServiceException(500, Microsoft.OData.Service.Strings.DataServiceActionProviderWrapper_AdvertiseServiceActionCannotReturnNullActionToSerialize);
                }

                // Always set target and title if there are overloaded actions.
                if (!entityHasMultipleActionsWithSameName)
                {
                    this.metadataPropertyManager.CheckForUnmodifiedTitle(odataAction, serviceOperationWrapper.Name);
                    this.metadataPropertyManager.CheckForUnmodifiedTarget(odataAction, () => lazyActionTargetUri.Value);
                }

                // make the target link relative
                this.MakeOperationTargetRelativeFromMetadataUriIfJsonLight(odataAction);

                return true;
            }

            odataAction = null;
            return false;
        }

        /// <summary>
        /// Method modifies the Operation Target Uri to be relative to the metadata uri if the
        /// Operation Target has the same host as the metadata uri and the format is JSONLight
        /// </summary>
        /// <param name="operation">Operation to update</param>
        [SuppressMessage("DataWeb.Usage", "AC0019:ShouldNotDireclyAccessPayloadMetadataProperties", Justification = "This component is allowed to set these properties as any user specified updates have already occurred.")]
        private void MakeOperationTargetRelativeFromMetadataUriIfJsonLight(ODataOperation operation)
        {
            Debug.Assert(operation != null, "operation != null");

            Uri targetLink = operation.Target;
            if (this.format == ODataFormat.Json && targetLink != null && targetLink.IsAbsoluteUri && this.metadataUri.IsBaseOf(targetLink))
            {
                operation.Target = this.metadataUri.MakeRelativeUri(targetLink);
            }
        }
    }
}