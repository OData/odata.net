//---------------------------------------------------------------------
// <copyright file="CustomUriTypePrefixLiterals.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Parsers.UriParsers;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion

    /// <summary>
    /// Extends the uri parsing system of Literal Prefix.
    /// With this class, you can add a custom literal prefix for any EdmType.
    /// </summary>
    public class CustomUriTypePrefixLiterals
    {
        #region Fields

        private static Dictionary<string, IEdmTypeReference> PrefixLiteralsOfEdmTypes;

        private static object Locker;

        #endregion

        #region Ctor

        static CustomUriTypePrefixLiterals()
        {
            PrefixLiteralsOfEdmTypes = new Dictionary<string, IEdmTypeReference>();
            Locker = new object();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Add a type prefix literal for the given EdmType.
        /// </summary>
        /// <example>filter=MyProperty eq MyCustomLiteral'VALUE'.
        /// "MyCustomLiteral" is the literalPrefixName and the literalEdmTypeReference is the type of the "VALUE".</example>
        /// <param name="typePrefixLiteralName">The custom name of the new type prefix literal name</param>
        /// <param name="literalEdmTypeReference">The edm type of the custom literal</param>
        /// <exception cref="ArgumentNullException">Arguments are null or empty</exception>
        /// <exception cref="ArgumentException">The given literal prefix is not valid</exception>
        /// <exception cref="ODataException">The given literal prefix already exists</exception>
        public static void AddCustomUriTypePrefixLiteral(string typePrefixLiteralName, IEdmTypeReference literalEdmTypeReference)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentNotNull(literalEdmTypeReference, "literalEdmTypeReference");

            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(typePrefixLiteralName, "typePrefixLiteralName");

            UriParserHelper.ValidatePrefixLiteral(typePrefixLiteralName);

            // Try to add the custom uri literal to cache
            lock (Locker)
            {
                // Check if literal does already exists
                if (PrefixLiteralsOfEdmTypes.ContainsKey(typePrefixLiteralName))
                {
                    throw new ODataException(ODataErrorStrings.CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(typePrefixLiteralName));
                }

                PrefixLiteralsOfEdmTypes.Add(typePrefixLiteralName, literalEdmTypeReference);
            }
        }

        /// <summary>
        /// Remove the given type prefix literal.
        /// </summary>
        /// <param name="typePrefixLiteralName">The custom name of the new type prefix literal name</param>
        /// <returns>'true' if the prefix litral is successfully found and removed; otherwise, 'false'.</returns>
        /// <exception cref="ArgumentNullException">Argumnet is null or empty</exception>
        public static bool RemoveCustomUriTypePrefixLiteral(string typePrefixLiteralName)
        {
            // Arguments validation
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(typePrefixLiteralName, "typePrefixLiteralName");

            UriParserHelper.ValidatePrefixLiteral(typePrefixLiteralName);

            // Try to remove the custom uri literal to cache
            lock (Locker)
            {
                return PrefixLiteralsOfEdmTypes.Remove(typePrefixLiteralName);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the EdmTypeReference of the given literal prefix name
        /// </summary>
        /// <param name="typePrefixLiteralName">The prefix literal name of the EdmType</param>
        /// <returns>Null if the literal prefix has no registered custom EdmType.</returns>
        internal static IEdmTypeReference GetCustomEdmTypeByLiteralPrefix(string typePrefixLiteralName)
        {
            lock (Locker)
            {
                IEdmTypeReference customEdmTypeReference;
                if (PrefixLiteralsOfEdmTypes.TryGetValue(typePrefixLiteralName, out customEdmTypeReference))
                {
                    return customEdmTypeReference;
                }
            }

            return null;
        }

        #endregion
    }
}