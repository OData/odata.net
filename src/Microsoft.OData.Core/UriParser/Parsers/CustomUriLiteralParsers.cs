//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion

    /// <summary>
    /// This class is the custom literal parser manager and parser.
    /// Add a Uri custom literal parser through this class.
    /// This class is also used as an UriLiteralParser.
    /// </summary>
    public sealed class CustomUriLiteralParsers : IUriLiteralParser
    {
        #region Fields

        private static readonly object Locker = new object();


        /// <summary>
        /// Used for General uri literal parsers. These parsers will be called for every text that has to parsed.
        /// The parses could parse multiple EdmTypes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>")]
        private static IUriLiteralParser[] customUriLiteralParsers = new IUriLiteralParser[0];

        /// <summary>
        /// "Registered" uri literal parser to an EdmType. These parsers will be called when the text has to be parsed to the
        /// specific EdmType they had registered to. Each of these parsers could parse only one EdmType. Better performance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations.", Justification = "<Pending>")]
        private static UriLiteralParserPerEdmType[] customUriLiteralParserPerEdmType = new UriLiteralParserPerEdmType[0];

        //// TODO: Consider use Dictionary<EmdTypeReference,IUriLiteralParser> which is a better solution.
        //// The problem with dictionary is to generate an HashCode for an EdmTypeReference.

        #endregion

        #region Singleton

        // Internal Singleton so only interal assemblies could parse by the custom parsers.
        private static CustomUriLiteralParsers singleInstance;

        private CustomUriLiteralParsers()
        {
        }

        internal static CustomUriLiteralParsers Instance
        {
            get
            {
                if (CustomUriLiteralParsers.singleInstance == null)
                {
                    CustomUriLiteralParsers.singleInstance = new CustomUriLiteralParsers();
                }

                return CustomUriLiteralParsers.singleInstance;
            }
        }

        #endregion

        #region IUriLiteralParser Implementation - Internal

        /// <summary>
        /// Parse the given uri text.
        /// Try to parse with a specific Uri literal parser registered for the target EdmType.
        /// If no parser is registered, try to parse with the general parsers.
        /// This method is public because of the Interface, but the Singleton instance in internal so it could not be accessed by clients.
        /// </summary>
        /// <param name="text">Part of the Uri which has to be parsed to a value of EdmType <paramref name="targetType"/></param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the <paramref name="targetType"/> but failed during the parsing process</param>
        /// <returns>If parsing process has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
        {
            IUriLiteralParser uriLiteralParserForEdmType = CustomUriLiteralParsers.GetUriLiteralParserByEdmType(targetType);

            // Search for Uri literal parser which is registered for the given EdmType
            if (uriLiteralParserForEdmType != null)
            {
                return uriLiteralParserForEdmType.ParseUriStringToType(text, targetType, out parsingException);
            }

            // Parse with all the general parsers
            // Stop when a parser succeeded parsing the text.
            IUriLiteralParser[] localCustomUriLiteralParsers = CustomUriLiteralParsers.customUriLiteralParsers;
            foreach (IUriLiteralParser customUriLiteralParser in localCustomUriLiteralParsers)
            {
                // Try to parse
                object targetValue = customUriLiteralParser.ParseUriStringToType(text, targetType, out parsingException);

                // The uriCustomParser could parse the given targetType but failed during the parsing process
                if (parsingException != null)
                {
                    return null;
                }

                // In case of no exception and no value - The parse cannot parse the given text
                if (targetValue != null)
                {
                    return targetValue;
                }
            }

            // No uriCustomParser could parse the requested uri text.
            parsingException = null;
            return null;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Add a custom 'IUriLiteralParser' which will be called to parse uri values during the uri parsing process.
        /// </summary>
        /// <param name="customUriLiteralParser">The custom uri parser</param>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null</exception>
        /// <exception cref="ODataException">The given IUriLiteralParser instance already exists</exception>
        public static void AddCustomUriLiteralParser(IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            lock (CustomUriLiteralParsers.Locker)
            {
                if (CustomUriLiteralParsers.customUriLiteralParsers.Contains(customUriLiteralParser))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
                }

                CustomUriLiteralParsers.customUriLiteralParsers = CustomUriLiteralParsers.customUriLiteralParsers.Concat(new IUriLiteralParser[] { customUriLiteralParser }).ToArray();
            }
        }

        /// <summary>
        /// Add a custom 'IUriLiteralParser' which will be called to parse a value of the given EdmType during the UriParsing process.
        /// </summary>
        /// <param name="edmTypeReference">The EdmType the Uri literal parser can parse.</param>
        /// <param name="customUriLiteralParser">The custom uri type parser to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="edmTypeReference"/> is null.</exception>
        /// <exception cref="ODataException">Another Uri literal parser is already registered for the given EdmType</exception>
        public static void AddCustomUriLiteralParser(IEdmTypeReference edmTypeReference, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, "edmTypeReference");
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            lock (CustomUriLiteralParsers.Locker)
            {
                if (CustomUriLiteralParsers.IsEdmTypeAlreadyRegistered(edmTypeReference))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(edmTypeReference.FullName()));
                }

                CustomUriLiteralParsers.customUriLiteralParserPerEdmType = CustomUriLiteralParsers.customUriLiteralParserPerEdmType.Concat(
                    new UriLiteralParserPerEdmType[]
                    {
                        new UriLiteralParserPerEdmType
                        {
                            EdmTypeOfUriParser = edmTypeReference,
                            UriLiteralParser = customUriLiteralParser
                        }
                    })
                    .ToArray();
            }
        }

        /// <summary>
        /// Remove the given custom 'IUriLiteralParser' form cache.
        /// It will be removed from both regular parsers and parsers registered with EdmType.
        /// </summary>
        /// <param name="customUriLiteralParser">The custom uri type parser to remove</param>
        /// <returns>'False' if the given parser to remove doesn't exist. 'True' if the parser has successfully removed</returns>
        /// <exception cref="ArgumentNullException">Uri literal parser is null</exception>
        public static bool RemoveCustomUriLiteralParser(IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            lock (CustomUriLiteralParsers.Locker)
            {
                // Remove parser from the customUriLiteralParserPerEdmType. Same instance can be registered to multiple EdmTypes.
                UriLiteralParserPerEdmType[] newCustomUriLiteralParserPerEdmType = CustomUriLiteralParsers.customUriLiteralParserPerEdmType
                    .Where((parser) => !parser.UriLiteralParser.Equals(customUriLiteralParser))
                    .ToArray();

                // Remove parser from the general custom uri literal parsers. Same instance can be add only once.
                IUriLiteralParser[] newCustomUriLiteralParsers = CustomUriLiteralParsers.customUriLiteralParsers
                    .Where((parser) => !parser.Equals(customUriLiteralParser))
                    .ToArray();

                // Returns 'True' if at least one parser has been removed from the general parser of those registered to EdmType
                bool removed = newCustomUriLiteralParserPerEdmType.Length < CustomUriLiteralParsers.customUriLiteralParserPerEdmType.Length ||
                    newCustomUriLiteralParsers.Length < CustomUriLiteralParsers.customUriLiteralParsers.Length;

                CustomUriLiteralParsers.customUriLiteralParserPerEdmType = newCustomUriLiteralParserPerEdmType;
                CustomUriLiteralParsers.customUriLiteralParsers = newCustomUriLiteralParsers;

                return removed;
            }
        }

        #endregion

        #region Private Methods

        private static bool IsEdmTypeAlreadyRegistered(IEdmTypeReference edmTypeReference)
        {
            return CustomUriLiteralParsers.customUriLiteralParserPerEdmType.Any(uriParserOfEdmType =>
                EdmElementComparer.IsEquivalentTo(uriParserOfEdmType.EdmTypeOfUriParser, edmTypeReference));
        }

        private static IUriLiteralParser GetUriLiteralParserByEdmType(IEdmTypeReference edmTypeReference)
        {
            UriLiteralParserPerEdmType requestedUriLiteralParser =
                CustomUriLiteralParsers.customUriLiteralParserPerEdmType.FirstOrDefault(uriParserOfEdmType =>
                uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference));

            if (requestedUriLiteralParser == null)
            {
                return null;
            }

            return requestedUriLiteralParser.UriLiteralParser;
        }


        #endregion

        private sealed class UriLiteralParserPerEdmType
        {
            internal IEdmTypeReference EdmTypeOfUriParser { get; set; }

            internal IUriLiteralParser UriLiteralParser { get; set; }
        }
    }
}