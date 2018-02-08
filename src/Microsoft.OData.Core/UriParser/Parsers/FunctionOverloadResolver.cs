//---------------------------------------------------------------------
// <copyright file="FunctionOverloadResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

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
            IList<IEdmOperationImport> candidateMatchingOperationImports = null;
            IList<IEdmActionImport> foundActionImportsWhenLookingForFunctions = new List<IEdmActionImport>();

            try
            {
                if (parameterNames.Count > 0)
                {
                    // In this case we have to return a function so filter out actions because the number of parameters > 0.
                    candidateMatchingOperationImports = resolver.ResolveOperationImports(model, identifier).RemoveActionImports(out foundActionImportsWhenLookingForFunctions).FilterFunctionsByParameterNames(parameterNames, resolver.EnableCaseInsensitive).Cast<IEdmOperationImport>().ToList();
                }
                else
                {
                    candidateMatchingOperationImports = resolver.ResolveOperationImports(model, identifier).ToList();
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
                if (candidateMatchingOperationImports.Count > 1)
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

            // If parameter count is zero and there is one function import whoese parameter count is zero, return this function import.
            if (candidateMatchingOperationImports.Count > 1 && parameterNames.Count == 0)
            {
                candidateMatchingOperationImports = candidateMatchingOperationImports.Where(operationImport => operationImport.Operation.Parameters.Count() == 0).ToList();
            }

            if (candidateMatchingOperationImports.Count == 0)
            {
                matchingOperationImport = null;
                return false;
            }

            // If more than one overload matches, try to select based on optional parameters
            if (candidateMatchingOperationImports.Count > 1)
            {
                candidateMatchingOperationImports = candidateMatchingOperationImports.FindBestOverloadBasedOnParameters(parameterNames).ToList();
            }

            if (candidateMatchingOperationImports.Count > 1)
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
        internal static bool ResolveOperationFromList(string identifier, IEnumerable<string> parameterNames, IEdmType bindingType, IEdmModel model, out IEdmOperation matchingOperation, ODataUriResolver resolver)
        {
            // TODO: update code that is duplicate between operation and operation import, add more tests.
            // If the previous segment is an open type, the service action name is required to be fully qualified or else we always treat it as an open property name.
            if (bindingType != null)
            {
                // TODO: look up actual container names here?
                // When using extension, there may be function call with unqualified name. So loose the restriction here.
                if (bindingType.IsOpen() && !identifier.Contains(".") && resolver.GetType() == typeof(ODataUriResolver))
                {
                    matchingOperation = null;
                    return false;
                }
            }

            IList<IEdmOperation> candidateMatchingOperations = null;

            // The extension method FindBoundOperations & FindOperations call IEdmModel.FindDeclaredBoundOperations which can be implemented by anyone and it could throw any type of exception
            // so catching all of them and simply putting it in the inner exception.
            try
            {
                if (bindingType != null)
                {
                    candidateMatchingOperations = resolver.ResolveBoundOperations(model, identifier, bindingType).ToList();
                }
                else
                {
                    candidateMatchingOperations = resolver.ResolveUnboundOperations(model, identifier).ToList();
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

            IList<IEdmAction> foundActionsWhenLookingForFunctions = new List<IEdmAction>();
            int parameterNamesCount = parameterNames.Count();

            if (bindingType != null)
            {
                candidateMatchingOperations = candidateMatchingOperations.EnsureOperationsBoundWithBindingParameter().ToList();
            }

            // If the number of parameters > 0 then this has to be a function as actions can't have parameters on the uri only in the payload. Filter further by parameters in this case, otherwise don't.
            if (parameterNamesCount > 0)
            {
                // can only be a function as only functions have parameters on the uri.
                candidateMatchingOperations = candidateMatchingOperations.RemoveActions(out foundActionsWhenLookingForFunctions).FilterFunctionsByParameterNames(parameterNames, resolver.EnableCaseInsensitive).Cast<IEdmOperation>().ToList();
            }
            else if (bindingType != null)
            {
                // Filter out functions with more than one parameter. Actions should not be filtered as the parameters are in the payload not the uri
                candidateMatchingOperations = candidateMatchingOperations.Where(o => (o.IsFunction() && o.Parameters.Count() == 1) || o.IsAction()).ToList();
            }
            else
            {
                // Filter out functions with any parameters
                candidateMatchingOperations = candidateMatchingOperations.Where(o => (o.IsFunction() && !o.Parameters.Any()) || o.IsAction()).ToList();
            }

            // Only filter if there is more than one and its needed.
            if (candidateMatchingOperations.Count > 1)
            {
                candidateMatchingOperations = candidateMatchingOperations.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(bindingType).ToList();
            }

            // If any of the things returned are an action, it better be the only thing returned, and there can't be parameters in the URL
            if (candidateMatchingOperations.Any(f => f.IsAction()))
            {
                if (candidateMatchingOperations.Count > 1)
                {
                    if (candidateMatchingOperations.Any(o => o.IsFunction()))
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleOperationOverloads(identifier));
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleActionOverloads(identifier));
                    }
                }

                if (parameterNames.Count() != 0)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                }

                matchingOperation = candidateMatchingOperations.Single();
                return true;
            }

            if (foundActionsWhenLookingForFunctions.Count > 0)
            {
                throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
            }

            // If more than one overload matches, try to select based on optional parameters
            if (candidateMatchingOperations.Count > 1)
            {
                candidateMatchingOperations = candidateMatchingOperations.FindBestOverloadBasedOnParameters(parameterNames).ToList();
            }

            if (candidateMatchingOperations.Count > 1)
            {
                throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound(identifier, string.Join(",", parameterNames.ToArray())));
            }

            matchingOperation = candidateMatchingOperations.SingleOrDefault();
            return matchingOperation != null;
        }
    }
}