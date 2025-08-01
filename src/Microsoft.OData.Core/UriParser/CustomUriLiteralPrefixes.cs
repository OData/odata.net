//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Extends the uri parsing system of Literal Prefix.
    /// With this class, you can add a custom literal prefix for any EdmType.
    /// </summary>
    public static class CustomUriLiteralPrefixes
    {
        #region Public Static Methods

        /// <summary>
        /// Registers a custom literal prefix for a specific Edm type in the given <see cref="IEdmModel"/>.
        /// This allows the OData URI parser to recognize and correctly parse literals with the specified prefix
        /// as instances of the provided EDM type during query parsing.
        /// </summary>
        /// <example>
        /// For example, in the filter expression: <c>MyProperty eq MyCustomLiteral'VALUE'</c>,
        /// "MyCustomLiteral" is the literal prefix, and <paramref name="literalEdmTypeReference"/> specifies the type of "VALUE".
        /// </example>
        /// <param name="model">The Edm model to which the custom literal prefix will be added.</param>
        /// <param name="literalPrefix">The custom literal prefix to register (e.g., "MyCustomLiteral").</param>
        /// <param name="literalEdmTypeReference">The Edm type reference that the prefix maps to.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="model"/>, <paramref name="literalPrefix"/>, or <paramref name="literalEdmTypeReference"/> is null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="literalPrefix"/> is not a valid literal prefix.
        /// </exception>
        /// <exception cref="ODataException">
        /// Thrown if a custom literal prefix with the same name already exists in the model.
        /// </exception>
        public static void AddCustomLiteralPrefix(this IEdmModel model, string literalPrefix, IEdmTypeReference literalEdmTypeReference)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(literalEdmTypeReference, "literalEdmTypeReference");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, "literalPrefix");

            UriParserHelper.ValidatePrefixLiteral(literalPrefix);

            CustomUriLiteralPrefixesAnnotation customUriLiteralPrefixesAnnotation =
               model.GetOrSetCustomUriLiteralPrefixesAnnotation();

            if (customUriLiteralPrefixesAnnotation.CustomUriLiteralPrefixes.TryGetValue(literalPrefix, out _))
            {
                throw new ODataException(Error.Format(SRResources.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists, literalPrefix));
            }

            customUriLiteralPrefixesAnnotation.CustomUriLiteralPrefixes.TryAdd(literalPrefix, literalEdmTypeReference);
        }


        /// <summary>
        /// Removes a custom literal prefix from the given <see cref="IEdmModel"/>.
        /// </summary>
        /// <param name="model"> The Edm model from which the custom literal prefix will be removed.</param>
        /// <param name="literalPrefix">The name of the literal prefix to remove.</param>
        /// <returns>
        /// <c>true</c> if the literal prefix was found and successfully removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="model"/> is <c>null</c> or <paramref name="literalPrefix"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="literalPrefix"/> is not a valid literal prefix.
        /// </exception>
        public static bool RemoveCustomLiteralPrefix(this IEdmModel model, string literalPrefix)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, "literalPrefix");

            UriParserHelper.ValidatePrefixLiteral(literalPrefix);

            CustomUriLiteralPrefixesAnnotation customUriLiteralPrefixesAnnotation =
               model.GetOrSetCustomUriLiteralPrefixesAnnotation();

            return customUriLiteralPrefixesAnnotation.CustomUriLiteralPrefixes.TryRemove(literalPrefix, out _);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Retrieves the <see cref="IEdmTypeReference"/> associated with a custom literal prefix from the given <see cref="IEdmModel"/>.
        /// </summary>
        /// <param name="model">The Edm model to search for the custom literal prefix.</param>
        /// <param name="literalPrefix">The custom literal prefix whose associated Edm type is to be retrieved.</param>
        /// <returns>
        /// The <see cref="IEdmTypeReference"/> associated with the specified literal prefix, or <c>null</c> if no type is registered for the prefix.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="model"/> is <c>null</c> or <paramref name="literalPrefix"/> is <c>null</c> or empty.
        /// </exception>
        internal static IEdmTypeReference GetEdmTypeByCustomLiteralPrefix(this IEdmModel model, string literalPrefix)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, "literalPrefix");

            CustomUriLiteralPrefixesAnnotation customUriLiteralPrefixesAnnotation =
               model.GetOrSetCustomUriLiteralPrefixesAnnotation();

            if (customUriLiteralPrefixesAnnotation.CustomUriLiteralPrefixes.TryGetValue(literalPrefix, out IEdmTypeReference literalEdmTypeReference))
            {
                return literalEdmTypeReference;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the <see cref="CustomUriLiteralPrefixesAnnotation"/> from the Edm model,
        /// or creates and attaches a new one if it does not already exist.
        /// </summary>
        /// <param name="model">The Edm model to retrieve or update with the annotation.</param>
        /// <returns>
        /// The existing or newly created <see cref="CustomUriLiteralPrefixesAnnotation"/> instance associated with the model.
        /// </returns
        internal static CustomUriLiteralPrefixesAnnotation GetOrSetCustomUriLiteralPrefixesAnnotation(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            CustomUriLiteralPrefixesAnnotation annotation = model.GetAnnotationValue<CustomUriLiteralPrefixesAnnotation>(model);
            if (annotation == null)
            {
                annotation = new CustomUriLiteralPrefixesAnnotation();
                model.SetAnnotationValue(model, annotation);
            }

            return annotation;
        }

        #endregion
    }
}
