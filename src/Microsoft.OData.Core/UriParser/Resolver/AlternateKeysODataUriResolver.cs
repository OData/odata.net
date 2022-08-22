//---------------------------------------------------------------------
// <copyright file="AlternateKeysODataUriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Vocabularies.Community.V1;
    using Microsoft.OData.Edm.Vocabularies.V1;

    /// <summary>
    /// Implementation for resolving the alternate keys.
    /// </summary>
    public sealed class AlternateKeysODataUriResolver : ODataUriResolver
    {
        /// <summary>
        /// Model to be used for resolving the alternate keys.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// The <see cref="IEdmTerm"/>s that are used within <see cref="model"/> to represent alternate key annotations
        /// </summary>
        private readonly IEnumerable<IEdmTerm> alternateKeyTerms;

        /// <summary>
        /// Constructs a AlternateKeysODataUriResolver with the given edmModel to be used for resolving alternate keys
        /// </summary>
        /// <param name="model">The model to be used.</param>
        public AlternateKeysODataUriResolver(IEdmModel model)
            : this(model, new[] { AlternateKeysVocabularyModel.AlternateKeysTerm, CoreVocabularyModel.AlternateKeysTerm })
        {
        }

        /// <summary>
        /// Constructs a AlternateKeysODataUriResolver with the given edmModel to be used for resolving alternate keys
        /// </summary>
        /// <param name="model">The model to be used.</param>
        /// <param name="alternateKeyTerms">The <see cref="IEdmTerm"/>s that are used within <paramref name="model"/> to represent alternate key annotations</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="alternateKeyTerms"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="alternateKeyTerms"/> contains any <see langword="null"/> values</exception>
        public AlternateKeysODataUriResolver(IEdmModel model, IEnumerable<IEdmTerm> alternateKeyTerms)
        {
            if (alternateKeyTerms == null)
            {
                throw new ArgumentNullException(nameof(alternateKeyTerms));
            }

            this.alternateKeyTerms = alternateKeyTerms.ToList();

            if (this.alternateKeyTerms.Where(term => term == null).Any())
            {
                throw new ArgumentException(Strings.UriParser_NullAlternateKeyTerm(nameof(alternateKeyTerms)));
            }

            this.model = model;
        }

        /// <summary>
        /// Resolve keys for certain entity set, this function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="namedValues">The dictionary of name value pairs.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <returns>The resolved key list.</returns>
        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            if (base.TryResolveKeys(type, namedValues, convertFunc, out IEnumerable<KeyValuePair<string, object>> convertedPairs))
            {
                return convertedPairs;
            }

            if (!TryResolveAlternateKeys(type, namedValues, convertFunc, out IEnumerable<KeyValuePair<string, object>> alternateConvertedPairs))
            {
                throw ExceptionUtil.CreateBadRequestError(Strings.BadRequest_KeyOrAlternateKeyMismatch(type.FullName()));
            }

            return alternateConvertedPairs;
        }

        /// <summary>
        /// Try to resolve alternate keys for certain entity type, this function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="namedValues">The dictionary of name value pairs.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <param name="convertedPairs">The resolved key list.</param>
        /// <returns>True if resolve succeeded.</returns>
        private bool TryResolveAlternateKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc, out IEnumerable<KeyValuePair<string, object>> convertedPairs)
        {
            IEnumerable<IDictionary<string, IEdmProperty>> alternateKeys = model.GetAlternateKeysAnnotation(type, this.alternateKeyTerms);
            foreach (IDictionary<string, IEdmProperty> keys in alternateKeys)
            {
                if (TryResolveKeys(type, namedValues, keys, convertFunc, out convertedPairs))
                {
                    return true;
                }
            }

            convertedPairs = null;
            return false;
        }

        /// <summary>
        /// Try to resolve keys for certain entity type, this function would be called when key is specified as name value pairs. E.g. EntitySet(ID='key')
        /// </summary>
        /// <param name="type">Type for current entityset.</param>
        /// <param name="namedValues">The dictionary of name value pairs.</param>
        /// <param name="keyProperties">Dictionary of alias to key properties.</param>
        /// <param name="convertFunc">The convert function to be used for value converting.</param>
        /// <param name="convertedPairs">The resolved key list.</param>
        /// <returns>True if resolve succeeded.</returns>
        private bool TryResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, IDictionary<string, IEdmProperty> keyProperties, Func<IEdmTypeReference, string, object> convertFunc, out IEnumerable<KeyValuePair<string, object>> convertedPairs)
        {
            if (namedValues.Count != keyProperties.Count)
            {
                // Count of name value pair does not match the alias count in this set of
                // alternative keys ==> Unresolvable for this set.
                convertedPairs = null;
                return false;
            }

            Dictionary<string, object> pairs = new Dictionary<string, object>(StringComparer.Ordinal);

            foreach (KeyValuePair<string, IEdmProperty> kvp in keyProperties)
            {
                string valueText;

                if (!namedValues.TryGetValue(kvp.Key, out valueText) && !EnableCaseInsensitive)
                {
                    convertedPairs = null;
                    return false;
                }

                if (valueText == null)
                {
                    var list = namedValues.Keys.Where(key => string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (list.Count > 1)
                    {
                        throw new ODataException(Strings.UriParserMetadata_MultipleMatchingKeysFound(kvp.Key));
                    }
                    else if (list.Count == 0)
                    {
                        convertedPairs = null;
                        return false;
                    }

                    valueText = namedValues[list.Single()];
                }

                object convertedValue = convertFunc(kvp.Value.Type, valueText);
                if (convertedValue == null)
                {
                    convertedPairs = null;
                    return false;
                }

                pairs[kvp.Key] = convertedValue;
            }

            convertedPairs = pairs;
            return true;
        }
    }
}
