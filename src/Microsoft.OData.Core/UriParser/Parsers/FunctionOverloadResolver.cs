//---------------------------------------------------------------------
// <copyright file="FunctionOverloadResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    using System.Diagnostics;

    /// <summary>
    /// Helper class to help bind function overloads.
    /// This is shared between path and filter/orderby function resolution.
    /// </summary>
    internal static class FunctionOverloadResolver
    {
        /// <summary>
        /// Try to resolve a function from the given inputs.
        /// </summary>
        /// <param name="identifier">The identifier of the function that we're trying to find</param>
        /// <param name="parameterNames">the names of the parameters to search for.</param>
        /// <param name="model">the model to use to look up the operation import</param>
        /// <param name="matchingOperationImport">The single matching function found.</param>
        /// <param name="resolver">Resolver to be used.</param>
        /// <returns>True if a function was matched, false otherwise. Will throw if the model has illegal operation imports.</returns>
        internal static bool ResolveOperationImportFromList(string identifier, IList<string> parameterNames, IEdmModel model, out IEdmOperationImport matchingOperationImport, ODataUriResolver resolver)
        {
            IEnumerable<IEdmOperationImport> candidateMatchingOperationImports = null;
            IList<IEdmOperationImport> foundActionImportsWhenLookingForFunctions = new List<IEdmOperationImport>();

            try
            {
                if (parameterNames.Count > 0)
                {
                    // In this case we have to return a function so filter out actions because the number of parameters > 0.
                    candidateMatchingOperationImports = resolver.ResolveOperationImports(model, identifier)
                        .RemoveActionImports(out foundActionImportsWhenLookingForFunctions)
                        .FilterOperationsByParameterNames(parameterNames, resolver.EnableCaseInsensitive);
                }
                else
                {
                    candidateMatchingOperationImports = resolver.ResolveOperationImports(model, identifier);
                }
            }
            catch (Exception exc)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(exc))
                {
                    throw;
                }

                throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_FoundInvalidOperationImport(identifier), exc);
            }

            if (foundActionImportsWhenLookingForFunctions.Count > 0)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
            }

            // If any of the things returned are an action, it better be the only thing returned, and there can't be parameters in the URL
            if (candidateMatchingOperationImports.Any(f => f.IsActionImport()))
            {
                if (candidateMatchingOperationImports.Count() > 1)
                {
                    if (candidateMatchingOperationImports.Any(o => o.IsFunctionImport()))
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads(identifier));
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleActionImportOverloads(identifier));
                    }
                }

                if (parameterNames.Count() != 0)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                }

                matchingOperationImport = candidateMatchingOperationImports.Single();
                return true;
            }

            // If parameter count is zero and there is one function import whose parameter count is zero, return this function import.
            if (candidateMatchingOperationImports.Count() > 1 && parameterNames.Count == 0)
            {
                candidateMatchingOperationImports = candidateMatchingOperationImports.Where(operationImport => operationImport.Operation.Parameters.Count() == 0);
            }

            if (!candidateMatchingOperationImports.HasAny())
            {
                matchingOperationImport = null;
                return false;
            }

            // If more than one overload matches, try to select based on optional parameters
            if (candidateMatchingOperationImports.Count() > 1)
            {
                candidateMatchingOperationImports = candidateMatchingOperationImports.FindBestOverloadBasedOnParameters(parameterNames);
            }

            if (candidateMatchingOperationImports.Count() > 1)
            {
                throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationImportOverloads(identifier));
            }

            matchingOperationImport = candidateMatchingOperationImports.Single();

            return matchingOperationImport != null;
        }

        /// <summary>
        /// Try to resolve a function from the given inputs.
        /// </summary>
        /// <param name="identifier">The identifier of the function that we're trying to find</param>
        /// <param name="parameterNames">the names of the parameters to search for.</param>
        /// <param name="bindingType">the type of the previous segment</param>
        /// <param name="model">the model to use to look up the operation import</param>
        /// <param name="matchingOperation">The single matching function found.</param>
        /// <param name="resolver">Resolver to be used.</param>
        /// <returns>True if a function was matched, false otherwise. Will throw if the model has illegal operation imports.</returns>
        internal static bool ResolveOperationFromList(string identifier, IList<string> parameterNames, IEdmType bindingType, IEdmModel model, out IEdmOperation matchingOperation, ODataUriResolver resolver)
        {
            // TODO: update code that is duplicate between operation and operation import, add more tests.
            // If the previous segment is an open type, the service action name is required to be fully qualified or else we always treat it as an open property name.
            matchingOperation = null;
            if (bindingType != null)
            {
                // TODO: look up actual container names here?
                // When using extension, there may be function call with unqualified name. So loose the restriction here.
                if (bindingType.IsOpen() && !identifier.Contains(".") && resolver.GetType() == typeof(ODataUriResolver))
                {
                    return false;
                }
            }

            IEnumerable<IEdmOperation> operationsFromModel;

            // The extension method FindBoundOperations & FindOperations call IEdmModel.FindDeclaredBoundOperations which can be implemented by anyone and it could throw any type of exception
            // so catching all of them and simply putting it in the inner exception.
            try
            {
                if (bindingType != null)
                {
                    operationsFromModel = resolver.ResolveBoundOperations(model, identifier, bindingType);
                }
                else
                {
                    operationsFromModel = resolver.ResolveUnboundOperations(model, identifier);
                }
            }
            catch (Exception exc)
            {
                if (ExceptionUtils.IsCatchableExceptionType(exc))
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_FoundInvalidOperation(identifier), exc);
                }

                throw;
            }

            bool foundActionsWhenLookingForFunctions;
            // Filters candidates based on the parameter names specified in the uri, removes actions if there were parameters specified in the uri but set the out bool to indicate that.
            // If no parameters specified, then matches based on binding type or matches with operations with no parameters.
            IList<IEdmOperation> candidatesMatchingOperations = operationsFromModel.FilterOperationCandidatesBasedOnParameterList(bindingType, parameterNames, resolver.EnableCaseInsensitive, out foundActionsWhenLookingForFunctions);

            // Only filter if there is more than one and its needed.
            if (candidatesMatchingOperations.Count > 1 && bindingType != null)
            {
                candidatesMatchingOperations = candidatesMatchingOperations.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(bindingType) as IList<IEdmOperation>;
                
                // This will be only null when no candidates are left. In that case, we can return false here. 
                if (candidatesMatchingOperations == null)
                {
                    if (foundActionsWhenLookingForFunctions)
                    {
                        throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                    }

                    return false; 
                }
            }

            // If any of the candidates are an action, it better be the only thing returned, and there can't be parameters in the URL
            if (ResolveActionFromCandidates(candidatesMatchingOperations, identifier, parameterNames.Count > 0,  out matchingOperation))
            {
                return true;
            }

            if (foundActionsWhenLookingForFunctions)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
            }

            // If more than one overload matches, try to select based on optional parameters
            if (candidatesMatchingOperations.Count > 1)
            {
                candidatesMatchingOperations = candidatesMatchingOperations.FilterOverloadsBasedOnParameterCount(parameterNames.Count);
            }

            if (candidatesMatchingOperations.Count > 1)
            {
                throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound(identifier, string.Join(",", parameterNames.ToArray())));
            }

            matchingOperation = candidatesMatchingOperations.Count > 0 ? candidatesMatchingOperations[0] : null;
            return matchingOperation != null;
        }

        private static bool ResolveActionFromCandidates(IList<IEdmOperation> candidatesMatchingOperations, string identifier, bool hasParameters, out IEdmOperation matchingOperation)
        {
            bool actionExists = false;
            bool functionExists = false;
            matchingOperation = null;
            for (int i = 0; i < candidatesMatchingOperations.Count; i++)
            {
                if (candidatesMatchingOperations[i].IsFunction())
                {
                    if (actionExists)
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationOverloads(identifier));
                    }

                    functionExists = true;
                }

                if (candidatesMatchingOperations[i].IsAction())
                {
                    if (functionExists)
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationOverloads(identifier));
                    }

                    actionExists = true;
                }
            }

            if (actionExists)
            {
                if (candidatesMatchingOperations.Count > 1)
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleActionOverloads(identifier));
                }

                Debug.Assert(!hasParameters);
                matchingOperation = candidatesMatchingOperations.Count > 0 ? candidatesMatchingOperations[0] : null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if there are elements in the enumerable. Performance is better for pure enumerables.
        /// Use the other HasAny extension method if the underlying type is likely to be a list. 
        /// </summary>
        internal static bool HasAny<T>(this IEnumerable<T> enumerable) where T : class
        {
            if (enumerable != null)
            {
                return enumerable.GetEnumerator().MoveNext();
            }

            return false;
        }
    }
}
