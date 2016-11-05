//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion

    /// <summary>
    /// Extends the uri parsing system of Literal Prefix.
    /// With this class, you can add a custom literal prefix for any EdmType.
    /// </summary>
    public static class CustomUriLiteralPrefixes
    {
        #region Fields

        private static readonly object Locker = new object();

        private static Dictionary<string, IEdmTypeReference> CustomLiteralPrefixesOfEdmTypes = new Dictionary<string, IEdmTypeReference>(StringComparer.Ordinal);

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Add a literal prefix for the given EdmType.
        /// </summary>
        /// <example>filter=MyProperty eq MyCustomLiteral'VALUE'.
        /// "MyCustomLiteral" is the literal prefix and the <paramref name="literalEdmTypeReference"/> is the type of the "VALUE".</example>
        /// <param name="literalPrefix">The custom name of the literal prefix</param>
        /// <param name="literalEdmTypeReference">The edm type of the custom literal</param>
        /// <exception cref="ArgumentNullException">Arguments are null or empty</exception>
        /// <exception cref="ArgumentException">The given literal prefix is not valid</exception>
        /// <exception cref="ODataException">The given literal prefix already exists</exception>
        public static void AddCustomLiteralPrefix(string literalPrefix, IEdmTypeReference literalEdmTypeReference)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentNotNull(literalEdmTypeReference, "literalEdmTypeReference");

            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, "literalPrefix");

            UriParserHelper.ValidatePrefixLiteral(literalPrefix);

            // Try to add the custom uri literal to cache
            lock (Locker)
            {
                // Check if literal does already exists
                if (CustomLiteralPrefixesOfEdmTypes.ContainsKey(literalPrefix))
                {
                    throw new ODataException(ODataErrorStrings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(literalPrefix));
                }

                CustomLiteralPrefixesOfEdmTypes.Add(literalPrefix, literalEdmTypeReference);
            }
        }

        /// <summary>
        /// Remove the given literal prefix
        /// </summary>
        /// <param name="literalPrefix">The custom name of the literal prefix</param>
        /// <returns>'true' if the literal prefix is successfully found and removed; otherwise, 'false'.</returns>
        /// <exception cref="ArgumentNullException">Argument is null or empty</exception>
        public static bool RemoveCustomLiteralPrefix(string literalPrefix)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(literalPrefix, "literalPrefix");

            UriParserHelper.ValidatePrefixLiteral(literalPrefix);

            // Try to remove the custom uri literal prefix from cache
            lock (Locker)
            {
                return CustomLiteralPrefixesOfEdmTypes.Remove(literalPrefix);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the EdmTypeReference of the given literal prefix
        /// </summary>
        /// <param name="literalPrefix">The literal prefix of the EdmType</param>
        /// <returns>Null if the custom literal prefix has no registered EdmType.</returns>
        internal static IEdmTypeReference GetEdmTypeByCustomLiteralPrefix(string literalPrefix)
        {
            lock (Locker)
            {
                IEdmTypeReference edmTypeReference;
                if (CustomLiteralPrefixesOfEdmTypes.TryGetValue(literalPrefix, out edmTypeReference))
                {
                    return edmTypeReference;
                }
            }

            return null;
        }

        #endregion
    }
}