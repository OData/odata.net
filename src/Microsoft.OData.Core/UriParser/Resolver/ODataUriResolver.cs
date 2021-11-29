//---------------------------------------------------------------------
// <copyright file="ODataUriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
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
            if (navSource != null || !EnableCaseInsensitive)
            {
                return navSource;
            }

            IEdmEntityContainer container = model.EntityContainer;
            if (container == null)
            {
                return null;
            }

            var result = container.Elements.OfType<IEdmNavigationSource>()
                .Where(source => string.Equals(identifier, source.Name, StringComparison.OrdinalIgnoreCase));

            IEdmNavigationSource resolvedNavigationSource = null;

            foreach (IEdmNavigationSource candidate in result)
            {
                if (resolvedNavigationSource == null)
                {
                    resolvedNavigationSource = candidate;
                }
                else
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingNavigationSourcesFound(identifier));
                }
            }

            return resolvedNavigationSource;
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
            if (property != null || !EnableCaseInsensitive)
            {
                return property;
            }

            var result = type.Properties()
            .Where(_ => string.Equals(propertyName, _.Name, StringComparison.OrdinalIgnoreCase));

            IEdmProperty resolvedProperty = null;

            foreach (IEdmProperty candidate in result)
            {
                if (resolvedProperty == null)
                {
                    resolvedProperty = candidate;
                }
                else
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingPropertiesFound(propertyName, type.FullTypeName()));
                }
            }

            return resolvedProperty;
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
            if (term != null || !EnableCaseInsensitive)
            {
                return term;
            }

            IList<IEdmTerm> results = FindAcrossModels<IEdmTerm>(model, termName, /*caseInsensitive*/ true);

            if (results == null || results.Count == 0)
            {
                return null;
            }

            if (results.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingTypesFound(termName));
            }

            return results[0];
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
            if (type != null || !EnableCaseInsensitive)
            {
                return type;
            }

            IList<IEdmSchemaType> results = FindAcrossModels<IEdmSchemaType>(model, typeName, /*caseInsensitive*/ true);

            if (results == null || results.Count == 0)
            {
                return null;
            }

            if (results.Count > 1)
            {
                throw new ODataException(Strings.UriParserMetadata_MultipleMatchingTypesFound(typeName));
            }

            return results[0];
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

            IList<IEdmOperation> operations = FindAcrossModels<IEdmOperation>(model, identifier, /*caseInsensitive*/ true);
            if (operations != null && operations.Count > 0)
            {
                IList<IEdmOperation> matchedOperation = new List<IEdmOperation>();
                for (int i = 0; i < operations.Count; i++)
                {
                    if (operations[i].HasEquivalentBindingType(bindingType))
                    {
                        matchedOperation.Add(operations[i]);
                    }
                }

                return matchedOperation;
            }

            return Enumerable.Empty<IEdmOperation>();
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

            IList<IEdmOperation> operations = FindAcrossModels<IEdmOperation>(model, identifier, /*caseInsensitive*/ true);
            if (operations != null && operations.Count > 0)
            {
                IList<IEdmOperation> matchedOperation = new List<IEdmOperation>();
                for (int i = 0; i < operations.Count; i++)
                {
                    if (!operations[i].IsBound)
                    {
                        matchedOperation.Add(operations[i]);
                    }
                }

                return matchedOperation;
            }

            return Enumerable.Empty<IEdmOperation>();
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
                return Enumerable.Empty<IEdmOperationImport>();
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

                // ensure parameter name exists
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
            // Throw an error if key size from url doesn't match that from model.
            // Other derived ODataUriResolver intended for alternative key resolution, such as the built in AlternateKeysODataUriResolver,
            // should override this ResolveKeys method.
            IEnumerable<IEdmStructuralProperty> keys = type.Key();
            if (keys.Count() != positionalValues.Count)
            {
                throw ExceptionUtil.CreateBadRequestError(Strings.BadRequest_KeyCountMismatch(type.FullName()));
            }

            var keyPairList = new List<KeyValuePair<string, object>>(positionalValues.Count);

            int i = 0;
            foreach (IEdmProperty keyProperty in keys)
            {
                string valueText = positionalValues[i++];
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
            if (!TryResolveKeys(type, namedValues, convertFunc, out IEnumerable<KeyValuePair<string, object>> resolvedKeys))
            {
                throw ExceptionUtil.CreateBadRequestError(Strings.BadRequest_KeyMismatch(type.FullName()));
            }

            return resolvedKeys;
        }

        /// <summary>
        /// Attempts to resolve keys for a certain entity set.
        /// This function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// </summary>
        /// <param name="type">The type for the current EntitySet</param>
        /// <param name="namedValues">The dictionary of name-value pairs.</param>
        /// <param name="convertFunc">The converter function to be used for value conversion.</param>
        /// <param name="resolvedKeys">If the resolution was successful, this will contain the list of resolved keys.</param>
        /// <returns>True if the key resolution was successful, otherwise false.</returns>
        internal bool TryResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc, out IEnumerable<KeyValuePair<string, object>> resolvedKeys)
        {
            resolvedKeys = null;
            var convertedPairs = new Dictionary<string, object>(StringComparer.Ordinal);

            IEnumerable<IEdmStructuralProperty> keys = type.Key();
            int keyCount = 0;

            foreach (IEdmStructuralProperty property in keys)
            {
                keyCount++;
                string valueText;

                if (!namedValues.TryGetValue(property.Name, out valueText))
                {
                    if (EnableCaseInsensitive)
                    {
                        var list = namedValues.Keys.Where(key => string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase));

                        string caseInsensitiveKey = string.Empty;
                        bool keyFound = false;
                        foreach (string key in list)
                        {
                            if (keyFound)
                            {
                                return false;
                            }

                            caseInsensitiveKey = key;
                            keyFound = true;
                        }

                        if (!keyFound)
                        {
                            return false;
                        }

                        valueText = namedValues[caseInsensitiveKey];
                    }
                    else
                    {
                        return false;
                    }
                }

                object convertedValue = convertFunc(property.Type, valueText);
                if (convertedValue == null)
                {
                    return false;
                }

                convertedPairs[property.Name] = convertedValue;
            }

            // Fail if key size from url doesn't match that from model.
            // Classes that extend ODataUriResolver to provide alternative key-resolution logic
            // should override the ResolveKeys method (e.g. the built-in AlternateKeysODataUriResolver)
            if (keyCount != namedValues.Count)
            {
                return false;
            }

            resolvedKeys = convertedPairs;
            return true;
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
            var list = operation.Parameters.Where(parameter => string.Equals(identifier, parameter.Name, StringComparison.Ordinal));
            if (!list.Any())
            {
                // if no case sensitive, try case-insensitive
                list = operation.Parameters.Where(parameter => string.Equals(identifier, parameter.Name, StringComparison.OrdinalIgnoreCase));
            }

            IEdmOperationParameter resolvedOperationParameter = null;

            foreach (var parameter in list)
            {
                if (resolvedOperationParameter == null)
                {
                    resolvedOperationParameter = parameter;
                }
                else
                {
                    throw new ODataException(Strings.UriParserMetadata_MultipleMatchingParametersFound(identifier));
                }
            }

            return resolvedOperationParameter;
        }

        internal static ODataUriResolver GetUriResolver(IServiceProvider container)
        {
            if (container == null)
            {
                return Default;
            }

            return container.GetRequiredService<ODataUriResolver>();
        }

        private static IList<T> FindAcrossModels<T>(IEdmModel model, String qualifiedName, bool caseInsensitive) where T : IEdmSchemaElement
        {
            IList<T> results = new List<T>();
            FindSchemaElements<T>(model, qualifiedName, caseInsensitive, ref results);

            foreach (IEdmModel reference in model.ReferencedModels)
            {
                FindSchemaElements<T>(reference, qualifiedName, caseInsensitive, ref results);
            }

            return results;
        }

        private static void FindSchemaElements<T>(IEdmModel model, string qualifiedName, bool caseInsensitive, ref IList<T> results) where T : IEdmSchemaElement
        {
            foreach (IEdmSchemaElement schema in model.SchemaElements)
            {
                if (string.Equals(qualifiedName, schema.FullName(), caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    if (schema is T)
                    {
                        results.Add((T)schema);
                    }
                }
            }
        }
    }
}
