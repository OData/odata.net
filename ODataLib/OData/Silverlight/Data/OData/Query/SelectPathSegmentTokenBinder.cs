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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Builds segments from tokens within $select.
    /// </summary>
    internal static class SelectPathSegmentTokenBinder
    {
        /// <summary>
        /// Build a segment from a token.
        /// </summary>
        /// <param name="tokenIn">the token to bind</param>
        /// <param name="model">The model.</param>
        /// <param name="entityType">the entity type of the current scope based on type segments.</param>
        /// <returns>The segment created from the token.</returns>
        public static ODataPathSegment ConvertNonTypeTokenToSegment(PathSegmentToken tokenIn, IEdmModel model, IEdmEntityType entityType)
        {
            ODataPathSegment nextSegment;
            if (TryBindAsDeclaredProperty(tokenIn, entityType, out nextSegment))
            {
                return nextSegment;
            }

            // for open types, operations must be container-qualified, and because the token type indicates it was not a .-seperated identifier, we should not try to look up operations.
            if (!entityType.IsOpen || tokenIn.IsNamespaceOrContainerQualified())
            {
                if (TryBindAsOperation(tokenIn, model, entityType, out nextSegment))
                {
                    return nextSegment;
                }
            }

            if (entityType.IsOpen)
            {
                return new OpenPropertySegment(tokenIn.Identifier);
            }

            throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(entityType.FullName(), tokenIn.Identifier));
        }

        /// <summary>
        /// Build a wildcard selection item
        /// </summary>
        /// <param name="tokenIn">the token to bind to a wildcard</param>
        /// <param name="model">the model to search for this wildcard</param>
        /// <param name="item">the new wildcard selection item, if we found one</param>
        /// <returns>true if we successfully bound to a wildcard, false otherwise</returns>
        public static bool TryBindAsWildcard(PathSegmentToken tokenIn, IEdmModel model, out SelectItem item)
        {
            bool isTypeToken = tokenIn.IsNamespaceOrContainerQualified();
            bool wildcard = tokenIn.Identifier.EndsWith("*", StringComparison.Ordinal);

            IEdmEntityContainer container;
            if (isTypeToken && wildcard && UriEdmHelpers.TryGetEntityContainer(tokenIn.Identifier.Substring(0, tokenIn.Identifier.LastIndexOf('.')), model, out container))
            {
                item = new ContainerQualifiedWildcardSelectItem(container);
                return true;
            }

            if (tokenIn.Identifier == "*")
            {
                item = new WildcardSelectItem();
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Tries to bind a given token as an Operation.
        /// </summary>
        /// <param name="pathToken">Token to bind.</param>
        /// <param name="model">The model.</param>
        /// <param name="entityType">the current entity type to use as the binding type when looking for operations.</param>
        /// <param name="segment">Bound segment if the token was bound to an operation successfully, or null.</param>
        /// <returns>True if the token was bound successfully, or false otherwise.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        private static bool TryBindAsOperation(PathSegmentToken pathToken, IEdmModel model, IEdmEntityType entityType, out ODataPathSegment segment)
        {
            Debug.Assert(pathToken != null, "pathToken != null");
            Debug.Assert(entityType != null, "bindingType != null");

            IEnumerable<IEdmFunctionImport> functionImports;
            var resolver = model as IODataUriParserModelExtensions;
            if (resolver != null)
            {
                functionImports = resolver.FindFunctionImportsByBindingParameterTypeHierarchy(entityType, pathToken.Identifier);
            }
            else
            {
                functionImports = model.FindFunctionImportsByBindingParameterTypeHierarchy(entityType, pathToken.Identifier);
            }

            List<IEdmFunctionImport> possibleFunctions = functionImports.ToList();

            if (possibleFunctions.Count <= 0)
            {
                segment = null;
                return false;
            }

            segment = new OperationSegment(possibleFunctions, null /*entitySet*/);
            return true;
        }

        /// <summary>
        /// Tries to bind a given token as an a declared structural or navigation property.
        /// </summary>
        /// <param name="tokenIn">Token to bind.</param>
        /// <param name="entityType">the entity type to search for this property</param>
        /// <param name="segment">Bound segment if the token was bound to a declared property successfully, or null.</param>
        /// <returns>True if the token was bound successfully, or false otherwise.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        private static bool TryBindAsDeclaredProperty(PathSegmentToken tokenIn, IEdmEntityType entityType, out ODataPathSegment segment)
        {
            IEdmProperty prop = entityType.FindProperty(tokenIn.Identifier);
            if (prop == null)
            {
                segment = null;
                return false;
            }

            if (prop.PropertyKind == EdmPropertyKind.Structural)
            {
                segment = new PropertySegment((IEdmStructuralProperty)prop);
                return true;
            }

            if (prop.PropertyKind == EdmPropertyKind.Navigation)
            {
                segment = new NavigationPropertySegment((IEdmNavigationProperty)prop, null /*TODO set*/);
                return true;
            }

            throw new ODataException(ODataErrorStrings.SelectExpandBinder_UnknownPropertyType(prop.Name));
        }
    }
}
