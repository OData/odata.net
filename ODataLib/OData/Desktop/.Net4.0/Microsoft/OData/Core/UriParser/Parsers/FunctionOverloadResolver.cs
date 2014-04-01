//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Helper class to help bind function overloads. 
    /// This is shared between path and filter/orderby function resolution.
    /// </summary>
    internal static class FunctionOverloadResolver
    {
        /// <summary>
        /// Given a list of possible operations and a list of parameter names, choose a single operation that exactly matches
        /// the parameter names. If more than one operation matches, throw.
        /// </summary>
        /// <remarks>
        /// Binding parameters will be ignored in this method. Only non-binding parameters are matched.
        /// </remarks>
        /// <param name="operationImports">The list of operation imports to search.</param>
        /// <param name="parameters">The list of non-binding parameter names to match.</param>
        /// <param name="operationName">Name of the operation. Only used for error strings.</param>
        /// <returns>A single operation import that matches the parameter names exactly.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        internal static IEdmOperationImport ResolveOverloadsByParameterNames(ICollection<IEdmOperationImport> operationImports, ICollection<string> parameters, string operationName)
        {
            DebugUtils.CheckNoExternalCallers();
            IEdmOperationImport candidateFunction = null;
            foreach (IEdmOperationImport operationImport in operationImports)
            {
                IEnumerable<IEdmOperationParameter> parametersToMatch = operationImport.Parameters;

                // bindable functions don't require the first parameter be specified, since its already implied in the path.
                if (operationImport.IsBindable)
                {
                    parametersToMatch = parametersToMatch.Skip(1);
                }

                // if any parameter count is different, don't consider it a match.
                List<IEdmOperationParameter> functionImportParameters = parametersToMatch.ToList();
                if (functionImportParameters.Count != parameters.Count)
                {
                    continue;
                }

                // if any parameter was missing, don't consider it a match.
                if (functionImportParameters.Any(p => parameters.All(k => k != p.Name)))
                {
                    continue;
                }

                if (candidateFunction == null)
                {
                    candidateFunction = operationImport;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound(operationName, string.Join(", ", parameters.ToArray())));
                }
            }

            return candidateFunction;
        }

        /// <summary>
        /// Try to resolve a function from the given inputs.
        /// </summary>
        /// <param name="identifier">The identifier of the function that we're trying to find</param>
        /// <param name="parameterNames">the names of the parameters to search for.</param>
        /// <param name="bindingType">the type of the previous segment</param>
        /// <param name="model">the model to use to look up the operation import</param>
        /// <param name="matchingFunctionImport">The single matching function found.</param>
        /// <returns>True if a function was matched, false otherwise. Will throw if the model has illegal operation imports.</returns>
        internal static bool ResolveFunctionsFromList(string identifier, IList<string> parameterNames, IEdmType bindingType, IEdmModel model, out IEdmOperationImport matchingFunctionImport)
        {
            DebugUtils.CheckNoExternalCallers();

            // If the previous segment is an open type, the service action name is required to be fully qualified or else we always treat it as an open property name.
            if (bindingType != null)
            {
                // TODO: look up actual container names here?
                if (bindingType.IsOpenType() && !identifier.Contains("."))
                {
                    matchingFunctionImport = null;
                    return false;
                }
            }

            // if the model implements the extensions interface, then leave resolution entirely up to the user.
            var extendedModel = model as IODataUriParserModelExtensions;
            if (extendedModel != null)
            {
                matchingFunctionImport = extendedModel.FindFunctionImportByBindingParameterType(bindingType, identifier, parameterNames);
                if (matchingFunctionImport == null)
                {
                    return false;
                }

                return true;
            }

            // Get all the possible functions by name and binding type
            // For this method to end up NOT throwing, this call must return a single action or 1 or more functions
            IList<IEdmOperationImport> operationImports = model.FindFunctionImportsBySpecificBindingParameterType(bindingType, identifier).ToList();
            if (operationImports.Count == 0)
            {
                matchingFunctionImport = null;
                return false;
            }

            Debug.Assert(operationImports != null && operationImports.Count > 0, "Should have returned at least one operation import to get here.");
            Debug.Assert(operationImports.All(f => f.Name == identifier.Split('.').Last()), "list of possible functions must have same name as identifier");

            if (!operationImports.AllHaveEqualReturnTypeAndAttributes())
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_FoundInvalidFunctionImport(identifier));
            }

            // If any of the things returned are an action, it better be the only thing returned, and there can't be parameters in the URL
            if (operationImports.Any(f => f.IsSideEffecting))
            {
                if (operationImports.Count > 1)
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleActionOverloads(identifier));
                }

                if (parameterNames.Count() != 0)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                }

                matchingFunctionImport = operationImports.Single();
                return true;
            }

            // Functions - resolve overloads as needed
            matchingFunctionImport = ResolveOverloadsByParameterNames(operationImports, parameterNames, identifier);
            return matchingFunctionImport != null;
        }
    }
}
