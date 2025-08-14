//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Provides extension methods for managing custom URI literal parsers in an <see cref="IEdmModel"/>.
    /// </summary>
    /// <remarks>These methods allow adding, removing, and associating custom URI literal parsers with
    /// specific Edm types or for general use during the URI parsing process. Custom parsers can be used to handle
    /// specialized literal parsing scenarios in OData services.</remarks>
    public static class CustomUriLiteralParsers
    {
        #region Public Static Methods

        /// <summary>
        /// Adds a custom URI literal parser which will be called to parse URI literals during the URI parsing process.
        /// </summary>
        /// <param name="model">Edm model to which the custom URI literal parser will be added.</param>
        /// <param name="customUriLiteralParser">The custom URI literal parser.</param>
        /// <exception cref="ArgumentNullException"><paramref name="model"/>  is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null.</exception>
        /// <exception cref="ODataException">The specified custom URI literal parser already exists.</exception>
        public static void AddCustomUriLiteralParser(this IEdmModel model, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            CustomUriLiteralParsersStore store = model.GetOrCreateStore();

            if (store.Contains(customUriLiteralParser))
            {
                throw new ODataException(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
            
            // If a race occurs, throw the same exception for consistent behaviour
            if (!store.Add(customUriLiteralParser))
            {
                throw new ODataException(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
        }

        /// <summary>
        /// Adds a custom URI literal parser which will be called to parse a literal of the given Edm type during the URI parsing process.
        /// </summary>
        /// <param name="model">Edm model to which the custom URI literal parser will be added.</param>
        /// <param name="edmTypeReference">The Edm type that the custom URI literal parser can parse.</param>
        /// <param name="customUriLiteralParser">The custom URI literal parser to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="model"/>  is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="edmTypeReference"/> is null.</exception>
        /// <exception cref="ODataException">Another custom URI literal parser is already registered for the given Edm type.</exception>
        public static void AddCustomUriLiteralParser(this IEdmModel model, IEdmTypeReference edmTypeReference, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, "edmTypeReference");
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            CustomUriLiteralParsersStore store = model.GetOrCreateStore();

            if (store.TryGet(edmTypeReference, out _))
            {
                // If the parser already exists for this EdmType, throw an exception
                throw new ODataException(Error.Format(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists, edmTypeReference.FullName()));
            }
            
            if (!store.Add(edmTypeReference, customUriLiteralParser))
            {
                // Another thread registered for this type first — throw same exception
                throw new ODataException(Error.Format(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists, edmTypeReference.FullName()));
            }
        }

        /// <summary>
        /// Removes the given custom URI literal parser from the cache.
        /// It will be removed from both general parsers and parsers associated with specific Edm type.
        /// </summary>
        /// <param name="model">Edm model from which the custom URI literal parser will be removed.</param>
        /// <param name="customUriLiteralParser">The custom URI literal parser to remove.</param>
        /// <returns><c>true</c> if the custom URI literal parser is successfully removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="model"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null.</exception>
        public static bool RemoveCustomUriLiteralParser(this IEdmModel model, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            CustomUriLiteralParsersStore store = model.GetOrCreateStore();

            return store.Remove(customUriLiteralParser);
        }

        #endregion

        #region Private Methods

        internal static CustomUriLiteralParsersStore GetOrCreateStore(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");
            return CustomUriLiteralParsersStore.GetOrCreate(model);
        }


        #endregion Private Methods
    }
}