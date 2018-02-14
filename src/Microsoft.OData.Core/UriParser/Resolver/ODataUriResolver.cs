//---------------------------------------------------------------------
// <copyright file="ODataUriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class for resolving different kinds of Uri parsing context.
    /// </summary>
    public class ODataUriResolver
    {
        /// <summary>
        /// Instance for <see cref="ODataUriResolver"/>.
        /// </summary>
        private static readonly ODataUriResolver Default = new ODataUriResolver();

        /// <summary>
        /// Promotion rules for type facets.
        /// </summary>
        private TypeFacetsPromotionRules typeFacetsPromotionRules = new TypeFacetsPromotionRules();

        /// <summary>
        /// Whether to enable case insensitive for the resolver.
        /// </summary>
        /// <remarks>
        /// All extensions should look at this property and keep case sensitive behavior consistent.
        /// </remarks>
        public virtual bool EnableCaseInsensitive { get; set; }

        /// <summary>
        /// Gets and sets the optional-$-sign-prefix for OData system query option.
        /// </summary>
        /// <remarks>
        /// All extensions should look at this property and keep case sensitive behavior consistent.
        /// </remarks>
        public virtual bool EnableNoDollarQueryOptions { get; set; }

        /// <summary>
        /// Gets and sets promotion rules for type facets.
        /// </summary>
        public TypeFacetsPromotionRules TypeFacetsPromotionRules
        {
            get
            {
                return typeFacetsPromotionRules;
            }

            set
            {
                typeFacetsPromotionRules = value;
            }
        }

        /// <summary>
        /// Promote the left and right operand types
        /// </summary>
        /// <param name="binaryOperatorKind">the operator kind</param>
        /// <param name="leftNode">the left operand</param>
        /// <param name="rightNode">the right operand</param>
        /// <param name="typeReference">type reference for the result BinaryOperatorNode.</param>
        public virtual void PromoteBinaryOperandTypes(
               BinaryOperatorKind binaryOperatorKind,
               ref SingleValueNode leftNode,
               ref SingleValueNode rightNode,
               out IEdmTypeReference typeReference)
        {
            typeReference = null;
            BinaryOperatorBinder.PromoteOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, typeFacetsPromotionRules);
        }

        /// <summary>
        /// Resolve navigation source from model.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The identifier to be resolved.</param>
        /// <returns>The resolved navigation source.</returns>
        public virtual IEdmNavigationSource ResolveNavigationSource(IEdmModel model, string identifier)
        {
            IEdmNavigationSource navSource = model.FindDeclaredNavigationSource(identifier);
            if (navSource != null | !EnableCaseInsensitive)
            {
                return navSource;
            }

            IEdmEntityContainer container = model.EntityContainer;
            if (container == null)
            {
                return null;
            }

            var result = container.Elements.OfType<IEdmNavigationSource>()
                .Where(source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (result.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingNavigationSourcesFound(identifier));
            }

            return result.SingleOrDefault();
        }

        /// <summary>
        /// Resolve property from property name
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The resolved <see cref="IEdmProperty"/></returns>
        public virtual IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
        {
            IEdmProperty property = type.FindProperty(propertyName);
            if (property != null | !EnableCaseInsensitive)
            {
                return property;
            }

            var result = type.Properties()
            .Where(_ => string.Equals(propertyName, _.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

            if (result.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingPropertiesFound(propertyName, type.FullTypeName()));
            }

            return result.SingleOrDefault();
        }

        /// <summary>
        /// Resolve term name from model.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="termName">The term name to be resolved.</param>
        /// <returns>Resolved term.</returns>
        public virtual IEdmTerm ResolveTerm(IEdmModel model, string termName)
        {
            IEdmTerm term = model.FindTerm(termName);
            if (term != null | !EnableCaseInsensitive)
            {
                return term;
            }

            IList<IEdmTerm> results = FindAcrossModels<IEdmTerm>(model, termName, /*caseInsensitive*/ true);

            if (results.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingTypesFound(termName));
            }

            return results.SingleOrDefault();
        }

        /// <summary>
        /// Resolve type name from model.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <returns>Resolved type.</returns>
        public virtual IEdmSchemaType ResolveType(IEdmModel model, string typeName)
        {
            IEdmSchemaType type = model.FindType(typeName);
            if (type != null | !EnableCaseInsensitive)
            {
                return type;
            }

            IList<IEdmSchemaType> results = FindAcrossModels<IEdmSchemaType>(model, typeName, /*caseInsensitive*/ true);
            if (results.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingTypesFound(typeName));
            }

            return results.SingleOrDefault();
        }

        /// <summary>
        /// Resolve bound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <param name="bindingType">The type operation was binding to.</param>
        /// <returns>Resolved operation list.</returns>
        public virtual IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier, IEdmType bindingType)
        {
            IEnumerable<IEdmOperation> results = model.FindBoundOperations(identifier, bindingType);
            if (results.Any() || !EnableCaseInsensitive)
            {
                return results;
            }

            return FindAcrossModels<IEdmOperation>(model, identifier, /*caseInsensitive*/ true)
                .Where(operation =>
                    operation.IsBound
                    && operation.Parameters.Any()
                    && operation.HasEquivalentBindingType(bindingType));
        }

        /// <summary>
        /// Resolve unbound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <returns>Resolved operation list.</returns>
        public virtual IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
        {
            IEnumerable<IEdmOperation> results = model.FindOperations(identifier);
            if (results.Any() || !EnableCaseInsensitive)
            {
                return results;
            }

            return FindAcrossModels<IEdmOperation>(model, identifier, /*caseInsensitive*/ true)
                .Where(operation => !operation.IsBound);
        }

        /// <summary>
        /// Resolve operation imports with certain name.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="identifier">The qualified name of the operation import which may or may not include the container name.</param>
        /// <returns>All operation imports that can be found by the specified name, returns an empty enumerable if no operation import exists.</returns>
        public virtual IEnumerable<IEdmOperationImport> ResolveOperationImports(IEdmModel model, string identifier)
        {
            IEnumerable<IEdmOperationImport> results = model.FindDeclaredOperationImports(identifier);
            if (results.Any() || !EnableCaseInsensitive)
            {
                return results;
            }

            IEdmEntityContainer container = model.EntityContainer;
            if (container == null)
            {
                return null;
            }

            return container.Elements.OfType<IEdmOperationImport>()
                .Where(source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Resolve operation's parameters.
        /// </summary>
        /// <param name="operation">Current operation for parameters.</param>
        /// <param name="input">A dictionary the parameter list.</param>
        /// <returns>A dictionary containing resolved parameters.</returns>
        public virtual IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            Dictionary<IEdmOperationParameter, SingleValueNode> result = new Dictionary<IEdmOperationParameter, SingleValueNode>(EqualityComparer<IEdmOperationParameter>.Default);
            foreach (var item in input)
            {
                IEdmOperationParameter functionParameter = null;
                if (EnableCaseInsensitive)
                {
                    functionParameter = ResolveOperationParameterNameCaseInsensitive(operation, item.Key);
                }
                else
                {
                    functionParameter = operation.FindParameter(item.Key);
                }

                // ensure parameter name existis
                if (functionParameter == null)
                {
                    throw new ODataException(Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation(item.Key, operation.Name));
                }

                result.Add(functionParameter, item.Value);
            }

            return result;
        }

        /// <summary>
        /// Resolve keys for certain entity set, this function would be called when key is specified as positional values. E.g. EntitySet('key')
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="positionalValues">The list of positional values.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <returns>The resolved key list.</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            var keyProperties = type.Key().ToList();
            var keyPairList = new List<KeyValuePair<string, object>>(positionalValues.Count);

            for (int i = 0; i < keyProperties.Count; i++)
            {
                string valueText = positionalValues[i];
                IEdmProperty keyProperty = keyProperties[i];
                object convertedValue = convertFunc(keyProperty.Type, valueText);
                if (convertedValue == null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                keyPairList.Add(new KeyValuePair<string, object>(keyProperty.Name, convertedValue));
            }

            return keyPairList;
        }

        /// <summary>
        /// Resolve keys for certain entity set, this function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="namedValues">The dictionary of name value pairs.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <returns>The resolved key list.</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            var convertedPairs = new Dictionary<string, object>(StringComparer.Ordinal);
            var keyProperties = type.Key().ToList();

            foreach (IEdmStructuralProperty property in keyProperties)
            {
                string valueText;

                if (!namedValues.TryGetValue(property.Name, out valueText))
                {
                    if (EnableCaseInsensitive)
                    {
                        var list = namedValues.Keys.Where(key => string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (list.Count > 1)
                        {
                            throw new ODataException(Strings.UriParserMetadata_MultipleMatchingKeysFound(property.Name));
                        }
                        else if (list.Count == 0)
                        {
                            throw ExceptionUtil.CreateSyntaxError();
                        }

                        valueText = namedValues[list.Single()];
                    }
                    else
                    {
                        throw ExceptionUtil.CreateSyntaxError();
                    }
                }

                object convertedValue = convertFunc(property.Type, valueText);
                if (convertedValue == null)
                {
                    throw ExceptionUtil.CreateSyntaxError();
                }

                convertedPairs[property.Name] = convertedValue;
            }

            return convertedPairs;
        }

        /// <summary>
        /// Resolve an operation parameter's name with case insensitive enabled
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="identifier">Name for the parameter.</param>
        /// <returns>The resolved operation parameter.</returns>
        internal static IEdmOperationParameter ResolveOperationParameterNameCaseInsensitive(IEdmOperation operation, string identifier)
        {
            // first look for a case-sensitive match
            var list = operation.Parameters.Where(parameter => string.Equals(identifier, parameter.Name, StringComparison.Ordinal)).ToList();
            if (list.Count == 0)
            {
                // if no case sensitive, try case-insensitive
                list = operation.Parameters.Where(parameter => string.Equals(identifier, parameter.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (list.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingParametersFound(identifier));
            }

            if (list.Count == 1)
            {
                return list.Single();
            }

            return null;
        }

        internal static ODataUriResolver GetUriResolver(IServiceProvider container)
        {
            if (container == null)
            {
                return Default;
            }

            return container.GetRequiredService<ODataUriResolver>();
        }

        private static List<T> FindAcrossModels<T>(IEdmModel model, String qualifiedName, bool caseInsensitive) where T : IEdmSchemaElement
        {
            List<T> results = FindSchemaElements<T>(model, qualifiedName, caseInsensitive).ToList();

            foreach (IEdmModel reference in model.ReferencedModels)
            {
                results.AddRange(FindSchemaElements<T>(reference, qualifiedName, caseInsensitive));
            }

            return results;
        }

        private static IEnumerable<T> FindSchemaElements<T>(IEdmModel model, string qualifiedName, bool caseInsensitive) where T : IEdmSchemaElement
        {
            return model.SchemaElements.OfType<T>()
            .Where(e => string.Equals(qualifiedName, e.FullName(), caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }
    }
}
