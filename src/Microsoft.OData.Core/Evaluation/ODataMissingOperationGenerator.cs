//---------------------------------------------------------------------
// <copyright file="ODataMissingOperationGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.JsonLight;

    /// <summary>
    /// Generates operations which were omitted by the service because they fully match conventions/templates and are always available.
    /// </summary>
    internal sealed class ODataMissingOperationGenerator
    {
        /// <summary>The current resource metadata context.</summary>
        private readonly IODataMetadataContext metadataContext;

        /// <summary>The metadata context of the resource to generate the missing operations for.</summary>
        private readonly IODataResourceMetadataContext resourceMetadataContext;

        /// <summary>The list of computed actions.</summary>
        private List<ODataAction> computedActions;

        /// <summary>The list of computed functions.</summary>
        private List<ODataFunction> computedFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMissingOperationGenerator"/> class.
        /// </summary>
        /// <param name="resourceMetadataContext">The metadata context of the resource to generate the missing operations for.</param>
        /// <param name="metadataContext">The current resource metadata context.</param>
        internal ODataMissingOperationGenerator(IODataResourceMetadataContext resourceMetadataContext, IODataMetadataContext metadataContext)
        {
            Debug.Assert(resourceMetadataContext != null, "resourceMetadataCotext != null");
            Debug.Assert(metadataContext != null, "metadataContext != null");

            this.resourceMetadataContext = resourceMetadataContext;
            this.metadataContext = metadataContext;
        }

        /// <summary>
        /// Gets the computed missing Actions from the generator.
        /// </summary>
        /// <returns>The computed missing Actions.</returns>
        internal IEnumerable<ODataAction> GetComputedActions()
        {
            this.ComputeMissingOperationsToResource();
            return this.computedActions;
        }

        /// <summary>
        /// Gets the computed missing Functions from the generator.
        /// </summary>
        /// <returns>The computed missing Functions.</returns>
        internal IEnumerable<ODataFunction> GetComputedFunctions()
        {
            this.ComputeMissingOperationsToResource();
            return this.computedFunctions;
        }

        /// <summary>
        /// Returns a hash set of operation imports (actions and functions) in the given resource.
        /// </summary>
        /// <param name="resource">The resource in question.</param>
        /// <param name="model">The edm model to resolve operation imports.</param>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <returns>The hash set of operation imports (actions and functions) in the given resource.</returns>
        private static HashSet<IEdmOperation> GetOperationsInEntry(ODataResourceBase resource, IEdmModel model, Uri metadataDocumentUri)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(metadataDocumentUri != null && metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri != null && metadataDocumentUri.IsAbsoluteUri");

            HashSet<IEdmOperation> edmOperationImportsInEntry = new HashSet<IEdmOperation>(EqualityComparer<IEdmOperation>.Default);
            IEnumerable<ODataOperation> operations = ODataUtilsInternal.ConcatEnumerables((IEnumerable<ODataOperation>)resource.NonComputedActions, (IEnumerable<ODataOperation>)resource.NonComputedFunctions);
            if (operations != null)
            {
                foreach (ODataOperation operation in operations)
                {
                    Debug.Assert(operation.Metadata != null, "operation.Metadata != null");
                    string operationMetadataString = UriUtils.UriToString(operation.Metadata);
                    Debug.Assert(
                        ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString),
                        "ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString)");
                    Debug.Assert(
                        operationMetadataString[0] == ODataConstants.ContextUriFragmentIndicator || metadataDocumentUri.IsBaseOf(operation.Metadata),
                        "operationMetadataString[0] == JsonLightConstants.ContextUriFragmentIndicator || metadataDocumentUri.IsBaseOf(operation.Metadata)");

                    string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, operationMetadataString);
                    IEnumerable<IEdmOperation> edmOperations = model.ResolveOperations(fullyQualifiedOperationName);
                    if (edmOperations != null)
                    {
                        foreach (IEdmOperation edmOperation in edmOperations)
                        {
                            edmOperationImportsInEntry.Add(edmOperation);
                        }
                    }
                }
            }

            return edmOperationImportsInEntry;
        }

        /// <summary>
        /// Computes the operations that are missing from the payload but should be added by conventions onto the resource.
        /// </summary>
        private void ComputeMissingOperationsToResource()
        {
            Debug.Assert(this.resourceMetadataContext != null, "this.resourceMetadataContext != null");
            Debug.Assert(this.metadataContext != null, "this.metadataContext != null");

            if (this.computedActions == null)
            {
                Debug.Assert(this.computedFunctions == null, "this.computedFunctions == null");

                this.computedActions = new List<ODataAction>();
                this.computedFunctions = new List<ODataFunction>();
                HashSet<IEdmOperation> edmOperations = GetOperationsInEntry(this.resourceMetadataContext.Resource, this.metadataContext.Model, this.metadataContext.MetadataDocumentUri);
                foreach (IEdmOperation bindableOperation in this.resourceMetadataContext.SelectedBindableOperations)
                {
                    // if this operation appears in the payload, skip it.
                    if (edmOperations.Contains(bindableOperation))
                    {
                        continue;
                    }

                    string metadataReferencePropertyName = ODataConstants.ContextUriFragmentIndicator + ODataJsonLightUtils.GetMetadataReferenceName(this.metadataContext.Model, bindableOperation);

                    bool isAction;
                    ODataOperation operation = ODataJsonLightUtils.CreateODataOperation(this.metadataContext.MetadataDocumentUri, metadataReferencePropertyName, bindableOperation, out isAction);
                    if (bindableOperation.Parameters.Any() && this.resourceMetadataContext.ActualResourceTypeName != bindableOperation.Parameters.First().Type.FullName())
                    {
                        operation.BindingParameterTypeName = bindableOperation.Parameters.First().Type.FullName();
                    }

                    operation.SetMetadataBuilder(this.resourceMetadataContext.Resource.MetadataBuilder, this.metadataContext.MetadataDocumentUri);
                    if (isAction)
                    {
                        this.computedActions.Add((ODataAction)operation);
                    }
                    else
                    {
                        this.computedFunctions.Add((ODataFunction)operation);
                    }
                }
            }
        }
    }
}