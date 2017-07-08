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
            if (EnableCaseInsensitive)
            {
                IEdmEntityContainer container = model.EntityContainer;
                if (container == null)
                {
                    return null;
                }

                var result = container.Elements.OfType<IEdmNavigationSource>()
                    .Where(source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase)).ToList();

                if (result.Count == 1)
                {
                    return result.Single();
                }
                else if (result.Count > 1)
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingNavigationSourcesFound(identifier));
                }
            }

            return model.FindDeclaredNavigationSource(identifier);
        }

        /// <summary>
        /// Resolve property from property name
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The resolved <see cref="IEdmProperty"/></returns>
        public virtual IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
        {
            if (EnableCaseInsensitive)
            {
                var result = type.Properties()
                .Where(_ => string.Equals(propertyName, _.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();

                if (result.Count == 1)
                {
                    return result.Single();
                }
                else if (result.Count > 1)
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingPropertiesFound(propertyName, type.FullTypeName()));
                }
            }

            return type.FindProperty(propertyName);
        }

        /// <summary>
        /// Resolve type name from model.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <returns>Resolved type.</returns>
        public virtual IEdmSchemaType ResolveType(IEdmModel model, string typeName)
        {
            if (EnableCaseInsensitive)
            {
                var result = model.SchemaElements.OfType<IEdmSchemaType>()
               .Where(_ => string.Equals(typeName, _.FullName(), StringComparison.OrdinalIgnoreCase))
               .ToList();

                if (result.Count == 1)
                {
                    return result.Single();
                }
                else if (result.Count > 1)
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingTypesFound(typeName));
                }
            }

            return model.FindType(typeName);
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
            if (EnableCaseInsensitive)
            {
                return model.SchemaElements.OfType<IEdmOperation>()
                    .Where(operation => string.Equals(
                            identifier,
                            operation.FullName(),
                            StringComparison.OrdinalIgnoreCase)
                    && operation.IsBound && operation.Parameters.Any()
                    && operation.HasEquivalentBindingType(bindingType));
            }

            return model.FindBoundOperations(identifier, bindingType);
        }

        /// <summary>
        /// Resolve unbound operations based on name.
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="identifier">The operation name.</param>
        /// <returns>Resolved operation list.</returns>
        public virtual IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier)
        {
            if (EnableCaseInsensitive)
            {
                return model.SchemaElements.OfType<IEdmOperation>()
                    .Where(operation => string.Equals(
                            identifier,
                            operation.FullName(),
                            StringComparison.OrdinalIgnoreCase)
                    && !operation.IsBound);
            }

            return model.FindOperations(identifier);
        }

        /// <summary>
        /// Resolve operation imports with certain name.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="identifier">The qualified name of the operation import which may or may not include the container name.</param>
        /// <returns>All operation imports that can be found by the specified name, returns an empty enumerable if no operation import exists.</returns>
        public virtual IEnumerable<IEdmOperationImport> ResolveOperationImports(IEdmModel model, string identifier)
        {
            if (EnableCaseInsensitive)
            {
                IEdmEntityContainer container = model.EntityContainer;
                if (container == null)
                {
                    return null;
                }

                return container.Elements.OfType<IEdmOperationImport>()
                    .Where(source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase));
            }

            return model.FindDeclaredOperationImports(identifier);
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
                    functionParameter = ResolveOpearationParameterNameCaseInsensitive(operation, item.Key);
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
                else if (!namedValues.TryGetValue(property.Name, out valueText))
                {
                    throw ExceptionUtil.CreateSyntaxError();
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
        internal static IEdmOperationParameter ResolveOpearationParameterNameCaseInsensitive(IEdmOperation operation, string identifier)
        {
            var list = operation.Parameters.Where(parameter => string.Equals(identifier, parameter.Name, StringComparison.OrdinalIgnoreCase)).ToList();

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
    }
}
