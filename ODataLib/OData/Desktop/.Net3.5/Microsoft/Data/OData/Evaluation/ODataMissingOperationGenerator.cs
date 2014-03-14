//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.JsonLight;
    using Microsoft.Data.OData.Metadata;

    /// <summary>
    /// Generates operations which were omitted by the service because they fully match conventions/templates and are always available.
    /// </summary>
    internal sealed class ODataMissingOperationGenerator
    {
        /// <summary>The current entry metadata context.</summary>
        private readonly IODataMetadataContext metadataContext;

        /// <summary>The metadata context of the entry to generate the missing operations for.</summary>
        private readonly IODataEntryMetadataContext entryMetadataContext;

        /// <summary>The list of computed actions.</summary>
        private List<ODataAction> computedActions;

        /// <summary>The list of computed functions.</summary>
        private List<ODataFunction> computedFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMissingOperationGenerator"/> class.
        /// </summary>
        /// <param name="entryMetadataContext">The metadata context of the entry to generate the missing operations for.</param>
        /// <param name="metadataContext">The current entry metadata context.</param>
        internal ODataMissingOperationGenerator(IODataEntryMetadataContext entryMetadataContext, IODataMetadataContext metadataContext)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entryMetadataContext != null, "entryMetadataCotext != null");
            Debug.Assert(metadataContext != null, "metadataContext != null");

            this.entryMetadataContext = entryMetadataContext;
            this.metadataContext = metadataContext;
        }

        /// <summary>
        /// Gets the computed missing Actions from the generator.
        /// </summary>
        /// <returns>The computed missing Actions.</returns>
        internal IEnumerable<ODataAction> GetComputedActions()
        {
            DebugUtils.CheckNoExternalCallers();
            this.ComputeMissingOperationsToEntry();
            return this.computedActions;
        }

        /// <summary>
        /// Gets the computed missing Functions from the generator.
        /// </summary>
        /// <returns>The computed missing Functions.</returns>
        internal IEnumerable<ODataFunction> GetComputedFunctions()
        {
            DebugUtils.CheckNoExternalCallers();
            this.ComputeMissingOperationsToEntry();
            return this.computedFunctions;
        }

        /// <summary>
        /// Returns a hash set of function imports (actions and functions) in the given entry.
        /// </summary>
        /// <param name="entry">The entry in question.</param>
        /// <param name="model">The edm model to resolve function imports.</param>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <returns>The hash set of function imports (actions and functions) in the given entry.</returns>
        private static HashSet<IEdmFunctionImport> GetFunctionImportsInEntry(ODataEntry entry, IEdmModel model, Uri metadataDocumentUri)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(metadataDocumentUri != null && metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri != null && metadataDocumentUri.IsAbsoluteUri");

            HashSet<IEdmFunctionImport> functionImportsInEntry = new HashSet<IEdmFunctionImport>(EqualityComparer<IEdmFunctionImport>.Default);
            IEnumerable<ODataOperation> operations = ODataUtilsInternal.ConcatEnumerables((IEnumerable<ODataOperation>)entry.NonComputedActions, (IEnumerable<ODataOperation>)entry.NonComputedFunctions);
            if (operations != null)
            {
                foreach (ODataOperation operation in operations)
                {
                    Debug.Assert(operation.Metadata != null, "operation.Metadata != null");
                    string operationMetadataString = UriUtilsCommon.UriToString(operation.Metadata);
                    Debug.Assert(
                        ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString),
                        "ODataJsonLightUtils.IsMetadataReferenceProperty(operationMetadataString)");
                    Debug.Assert(
                        operationMetadataString[0] == JsonLightConstants.MetadataUriFragmentIndicator || metadataDocumentUri.IsBaseOf(operation.Metadata),
                        "operationMetadataString[0] == JsonLightConstants.MetadataUriFragmentIndicator || metadataDocumentUri.IsBaseOf(operation.Metadata)");

                    string fullyQualifiedOperationName = ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, operationMetadataString);
                    IEnumerable<IEdmFunctionImport> functionImports = model.ResolveFunctionImports(fullyQualifiedOperationName);
                    if (functionImports != null)
                    {
                        foreach (IEdmFunctionImport functionImport in functionImports)
                        {
                            functionImportsInEntry.Add(functionImport);
                        }
                    }
                }
            }

            return functionImportsInEntry;
        }

        /// <summary>
        /// Computes the operations that are missing from the payload but should be added by conventions onto the entry.
        /// </summary>
        private void ComputeMissingOperationsToEntry()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.entryMetadataContext != null, "this.entryMetadataContext != null");
            Debug.Assert(this.metadataContext != null, "this.metadataContext != null");

            if (this.computedActions == null)
            {
                Debug.Assert(this.computedFunctions == null, "this.computedFunctions == null");

                this.computedActions = new List<ODataAction>();
                this.computedFunctions = new List<ODataFunction>();
                HashSet<IEdmFunctionImport> operationsInEntry = GetFunctionImportsInEntry(this.entryMetadataContext.Entry, this.metadataContext.Model, this.metadataContext.MetadataDocumentUri);
                foreach (IEdmFunctionImport alwaysBindableOperation in this.entryMetadataContext.SelectedAlwaysBindableOperations)
                {
                    // if this operation appears in the payload, skip it.
                    if (operationsInEntry.Contains(alwaysBindableOperation))
                    {
                        continue;
                    }

                    string metadataReferencePropertyName = JsonLightConstants.MetadataUriFragmentIndicator + ODataJsonLightUtils.GetMetadataReferenceName(alwaysBindableOperation);
                    bool isAction;
                    ODataOperation operation = ODataJsonLightUtils.CreateODataOperation(this.metadataContext.MetadataDocumentUri, metadataReferencePropertyName, alwaysBindableOperation, out isAction);
                    operation.SetMetadataBuilder(this.entryMetadataContext.Entry.MetadataBuilder, this.metadataContext.MetadataDocumentUri);
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
