//---------------------------------------------------------------------
// <copyright file="SelectedOperationsCache.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Cache for operations that were selected in the URI.
    /// </summary>
    internal class SelectedOperationsCache
    {
        /// <summary>
        /// Cache of selected actions based on the type that was specified in the URI, either the base type of the feed or a type segment that preceeded the action name.
        /// This is stored so that at serialization time, we can conditionally select actions based on the instance type being serialized.
        /// </summary>
        private readonly IDictionary<ResourceType, List<OperationWrapper>> selectedOperationsByUriType = new Dictionary<ResourceType, List<OperationWrapper>>(ReferenceEqualityComparer<ResourceType>.Instance);

        /// <summary>
        /// Cache of operations which should be serialized based on specific serialization-time instance types.
        /// </summary>
        private readonly IDictionary<ResourceType, List<OperationWrapper>> selectedOperationsByPayloadType = new Dictionary<ResourceType, List<OperationWrapper>>(ReferenceEqualityComparer<ResourceType>.Instance);

        /// <summary>
        /// Adds a set of selected operations from the URL.
        /// </summary>
        /// <param name="specificSelectedTypeInUri">The specific type for which the actions are selected. Should be either the base type of the feed or from a type segment.</param>
        /// <param name="selectedOperations">The selected operations returned by the provider.</param>
        /// <returns>Whether any operations were selected.</returns>
        internal bool AddSelectedOperations(ResourceType specificSelectedTypeInUri, IEnumerable<OperationWrapper> selectedOperations)
        {
            Debug.Assert(selectedOperations != null, "operations != null");

            List<OperationWrapper> operationsAlreadySelected = null;
            foreach (var selectedOperation in selectedOperations)
            {
                if (operationsAlreadySelected == null && !this.selectedOperationsByUriType.TryGetValue(specificSelectedTypeInUri, out operationsAlreadySelected))
                {
                    this.selectedOperationsByUriType[specificSelectedTypeInUri] = operationsAlreadySelected = new List<OperationWrapper>();
                }

                operationsAlreadySelected.Add(selectedOperation);
            }

            return operationsAlreadySelected != null;
        }

        /// <summary>
        /// Gets the set of operations that should be serialized for a specific instance type based on what was selected in the URI and what is bindable to the given type.
        /// </summary>
        /// <param name="instanceTypeBeingSerialized">The instance type being serialized.</param>
        /// <returns>The selected bindable operations.</returns>
        internal List<OperationWrapper> GetSelectedOperations(ResourceType instanceTypeBeingSerialized)
        {
            Debug.Assert(instanceTypeBeingSerialized != null, "instanceTypeBeingSerialized != null");

            List<OperationWrapper> selectedOperationsForPayloadType;
            if (!this.selectedOperationsByPayloadType.TryGetValue(instanceTypeBeingSerialized, out selectedOperationsForPayloadType))
            {
                // go gather all the selected operations for this type based on what was selected in the URL.
                selectedOperationsForPayloadType = this.GetSelectedAndBindableOperationsForAssignableTypes(instanceTypeBeingSerialized);

                // add them to the cache
                this.selectedOperationsByPayloadType[instanceTypeBeingSerialized] = selectedOperationsForPayloadType;
            }

            return selectedOperationsForPayloadType;
        }

        /// <summary>
        /// Gets the set of operations selected for types the instance type can be assigned to that are bindable to the instance type.
        /// </summary>
        /// <param name="instanceTypeBeingSerialized">The instance type being serialized.</param>
        /// <returns>The selected bindable operations.</returns>
        private List<OperationWrapper> GetSelectedAndBindableOperationsForAssignableTypes(ResourceType instanceTypeBeingSerialized)
        {
            IEnumerable<OperationWrapper> selectedOperations = Enumerable.Empty<OperationWrapper>();

            // Gather up all the selected operations that were selected for types that this instance type is assignable from.
            foreach (var baseType in instanceTypeBeingSerialized.BaseTypesAndSelf())
            {
                List<OperationWrapper> selectedOperationsForUriType;
                if (this.selectedOperationsByUriType.TryGetValue(baseType, out selectedOperationsForUriType))
                {
                    selectedOperations = selectedOperations.Concat(selectedOperationsForUriType);
                }
            }

            // Only return the operations that are actually bindable to the instance type.
            // this filtering is needed because the key for something like Person will include actions bound to Employee and Manager as well,
            // so that something like ?$select=ActionName will match a varying set of actions based on which instance type is being serialized.
            selectedOperations = selectedOperations.Where(o => o.BindingParameter.ParameterType.IsAssignableFrom(instanceTypeBeingSerialized));

            // Ensure the set is distinct before caching it.
            selectedOperations = selectedOperations.Distinct();

            return selectedOperations.ToList();
        }
    }
}