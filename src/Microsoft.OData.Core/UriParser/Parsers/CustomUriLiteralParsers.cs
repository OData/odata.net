//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

            CustomUriLiteralParsersAnnotation customUriLiteralParsersAnnotation =
                model.GetOrSetCustomUriLiteralParsersAnnotation();

            if (customUriLiteralParsersAnnotation.CustomUriLiteralParsers.TryGetValue(customUriLiteralParser, out _))
            {
                throw new ODataException(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
            }
            else
            {
                // Add the custom parser to the model's annotation
                customUriLiteralParsersAnnotation.CustomUriLiteralParsers.TryAdd(customUriLiteralParser, 0);
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

            CustomUriLiteralParsersAnnotation customUriLiteralParsersAnnotation =
                model.GetOrSetCustomUriLiteralParsersAnnotation();

            if (customUriLiteralParsersAnnotation.CustomUriLiteralParsersByEdmType.TryGetValue(edmTypeReference, out _))
            {
                // If the parser already exists for this EdmType, throw an exception
                throw new ODataException(Error.Format(SRResources.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists, edmTypeReference.FullName()));
            }
            else
            {
                // Add the custom parser to the model's annotation
                customUriLiteralParsersAnnotation.CustomUriLiteralParsersByEdmType.TryAdd(edmTypeReference, customUriLiteralParser);
            }
        }

        /// <summary>
        /// Removes the given custom URI literal parser from the cache.
        /// It will be removed from both general parsers and parsers associated with specific Edm type.
        /// </summary>
        /// <param name="model">Edm model from which the custom URI literal parser will be removed.</param>
        /// <param name="customUriLiteralParser">The custom URI literal parser to remove.</param>
        /// <returns><c>true</c> if the custom URI literal parser is successfully removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="model"/>  is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/>  is null.</exception>
        public static bool RemoveCustomUriLiteralParser(this IEdmModel model, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            List<IEdmTypeReference> edmTypeMappedToUriLiteralParser = new List<IEdmTypeReference>();
            CustomUriLiteralParsersAnnotation customUriLiteralParsersAnnotation =
                model.GetOrSetCustomUriLiteralParsersAnnotation();

            bool removed = false;

            foreach (KeyValuePair<IEdmTypeReference, IUriLiteralParser> kvPair in customUriLiteralParsersAnnotation.CustomUriLiteralParsersByEdmType)
            {
                if (kvPair.Value.Equals(customUriLiteralParser))
                {
                    edmTypeMappedToUriLiteralParser.Add(kvPair.Key);
                }
            }


            foreach (IEdmTypeReference edmTypeReference in edmTypeMappedToUriLiteralParser)
            {
                // Remove the parser from the model's annotation
                removed |= customUriLiteralParsersAnnotation.CustomUriLiteralParsersByEdmType.TryRemove(edmTypeReference, out _);
            }

            removed |= customUriLiteralParsersAnnotation.CustomUriLiteralParsers.TryRemove(customUriLiteralParser, out _);

            return removed;
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Retrieves the <see cref="CustomUriLiteralParsersAnnotation"/> from the Edm model,
        /// or creates and attaches a new one if it does not already exist.
        /// </summary>
        /// <param name="model">The Edm model to retrieve or update with the annotation.</param>
        /// <returns>
        /// The existing or newly created <see cref="CustomUriLiteralParsersAnnotation"/> instance associated with the model.
        /// </returns
        internal static CustomUriLiteralParsersAnnotation GetOrSetCustomUriLiteralParsersAnnotation(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            CustomUriLiteralParsersAnnotation annotation = model.GetAnnotationValue<CustomUriLiteralParsersAnnotation>(model);
            if (annotation == null)
            {
                annotation = new CustomUriLiteralParsersAnnotation();
                model.SetAnnotationValue(model, annotation);
            }

            return annotation;
        }

        #endregion Internal Static Methods
    }
}