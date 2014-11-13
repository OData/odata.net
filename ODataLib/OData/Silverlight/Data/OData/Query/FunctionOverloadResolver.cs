//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Helper class to help bind function overloads. 
    /// This is shared between path and filter/orderby function resolution.
    /// </summary>
    internal static class FunctionOverloadResolver
    {
        /// <summary>
        /// Given a list of possible functions and a list of parameter names, choose a single function that exactly matches
        /// the parameter names. If more than one function matches, throw.
        /// </summary>
        /// <remarks>
        /// Binding parameters will be ignored in this method. Only non-binding parameters are matched.
        /// </remarks>
        /// <param name="functionImports">The list of function imports to search.</param>
        /// <param name="parameters">The list of non-binding parameter names to match.</param>
        /// <param name="functionName">Name of the function. Only used for error strings.</param>
        /// <returns>A single function import that matches the parameter names exactly.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri parsing does not go through the same resolvers/settings that payload reading/writing does.")]
        internal static IEdmFunctionImport ResolveOverloadsByParameterNames(ICollection<IEdmFunctionImport> functionImports, ICollection<string> parameters, string functionName)
        {
            DebugUtils.CheckNoExternalCallers();
            IEdmFunctionImport candidateFunction = null;
            foreach (IEdmFunctionImport functionImport in functionImports)
            {
                IEnumerable<IEdmFunctionParameter> parametersToMatch = functionImport.Parameters;

                // bindable functions don't require the first parameter be specified, since its already implied in the path.
                if (functionImport.IsBindable)
                {
                    parametersToMatch = parametersToMatch.Skip(1);
                }

                // if any parameter count is different, don't consider it a match.
                List<IEdmFunctionParameter> functionImportParameters = parametersToMatch.ToList();
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
                    candidateFunction = functionImport;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound(functionName, string.Join(", ", parameters.ToArray())));
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
        /// <param name="model">the model to use to look up the function import</param>
        /// <param name="matchingFunctionImport">The single matching function found.</param>
        /// <returns>True if a function was matched, false otherwise. Will throw if the model has illegal function imports.</returns>
        internal static bool ResolveFunctionsFromList(string identifier, IList<string> parameterNames, IEdmType bindingType, IEdmModel model, out IEdmFunctionImport matchingFunctionImport)
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
            IList<IEdmFunctionImport> functionImports = model.FindFunctionImportsBySpecificBindingParameterType(bindingType, identifier).ToList();
            if (functionImports.Count == 0)
            {
                matchingFunctionImport = null;
                return false;
            }

            Debug.Assert(functionImports != null && functionImports.Count > 0, "Should have returned at least one function import to get here.");
            Debug.Assert(functionImports.All(f => f.Name == identifier.Split('.').Last()), "list of possible functions must have same name as identifier");

            if (!functionImports.AllHaveEqualReturnTypeAndAttributes())
            {
                throw new ODataException(ODataErrorStrings.RequestUriProcessor_FoundInvalidFunctionImport(identifier));
            }

            // If any of the things returned are an action, it better be the only thing returned, and there can't be parameters in the URL
            if (functionImports.Any(f => f.IsSideEffecting))
            {
                if (functionImports.Count > 1)
                {
                    throw new ODataException(ODataErrorStrings.FunctionOverloadResolver_MultipleActionOverloads(identifier));
                }

                if (parameterNames.Count() != 0)
                {
                    throw ExceptionUtil.CreateBadRequestError(ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(identifier));
                }

                matchingFunctionImport = functionImports.Single();
                return true;
            }

            // Functions - resolve overloads as needed
            matchingFunctionImport = ResolveOverloadsByParameterNames(functionImports, parameterNames, identifier);
            return matchingFunctionImport != null;
        }
    }
}
