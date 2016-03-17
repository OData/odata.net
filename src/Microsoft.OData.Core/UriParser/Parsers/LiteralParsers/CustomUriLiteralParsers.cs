//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Parsers.Common;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        private static List<IUriLiteralParser> customUriLiteralParsers = new List<IUriLiteralParser>();

        /// <summary>
        /// "Registered" uri literal parser to an EdmType. These parsers will be called when the text has to be parsed to the
        /// specific EdmType they had registered to. Each of these parsers could parse only one EdmType. Better performace.
        /// </summary>
        private static List<UriLiteralParserPerEdmType> customUriLiteralParserPerEdmType = new List<UriLiteralParserPerEdmType>();

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
                if (singleInstance == null)
                {
                    singleInstance = new CustomUriLiteralParsers();
                }

                return singleInstance;
            }
        }

        #endregion

        #region IUriLiteralParser Implementation - Internal

        /// <summary>
        /// Parse the given uri text.
        /// Try to parse with a specific Uri literal parser regeistered for the target EdmType.
        /// If no parser is registered, try to parse with the general parsers.
        /// This method is public becuase of the Interface, but the Singleton instance in internal so it could not be accessed by clients. 
        /// </summary>
        /// <param name="text">Part of the Uri which has to be parsed to a value of EdmType <paramref name="targetType"/></param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the <paramref name="targetType"/> but failed during the parsing process</param>
        /// <returns>If parsing proceess has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
        {
            object targetValue;

            lock (Locker)
            {
                // Search for Uri literal parser which is registered for the given EdmType
                IUriLiteralParser uriLiteralParserForEdmType = GetUriLiteralParserByEdmType(targetType);
                if (uriLiteralParserForEdmType != null)
                {
                    return uriLiteralParserForEdmType.ParseUriStringToType(text, targetType, out parsingException);
                }

                // Parse with all the general parsers
                // Stop when a parser succeeded parsing the text.
                foreach (IUriLiteralParser customUriLiteralParser in customUriLiteralParsers)
                {
                    // Try to parse
                    targetValue = customUriLiteralParser.ParseUriStringToType(text, targetType, out parsingException);

                    // The uriCustomParser could parse the given targetType but failed during the parsing proccess
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
            }

            // No uriCustomParser could parse the requested uri text.
            parsingException = null;
            return null;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Add a custom 'IUriLiteralParser' which will be called to parse uri values during the uri parsing proccess.
        /// </summary>
        /// <param name="customUriLiteralParser">The custom uri parser</param>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null</exception>
        /// <exception cref="ODataException">The given IUriLiteralParser instance already exists</exception>
        public static void AddCustomUriLiteralParser(IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");

            lock (Locker)
            {
                if (customUriLiteralParsers.Contains(customUriLiteralParser))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
                }

                customUriLiteralParsers.Add(customUriLiteralParser);
            }
        }

        /// <summary>
        /// Add a custom 'IUriLiteralParser' which will be called to parse a value of the given EdmType during the UriParsing proccess.
        /// </summary>
        /// <param name="edmTypeReference">The EdmType the Uri literal parser can parse.</param>
        /// <param name="customUriLiteralParser">The custom uri type parser to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="customUriLiteralParser"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="edmTypeReference"/> is null.</exception>
        /// <exception cref="ODataException">Another Uri literal parser is already registered for the given EdmType</exception>
        public static void AddCustomUriLiteralParser(IEdmTypeReference edmTypeReference, IUriLiteralParser customUriLiteralParser)
        {
            ExceptionUtils.CheckArgumentNotNull(customUriLiteralParser, "customUriLiteralParser");
            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, "edmTypeReference");

            lock (Locker)
            {
                if (IsEdmTypeAlreadyRegistered(edmTypeReference))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(edmTypeReference.FullName()));
                }

                customUriLiteralParserPerEdmType.Add(
                    new UriLiteralParserPerEdmType
                    {
                        EdmTypeOfUriParser = edmTypeReference,
                        UriLiteralParser = customUriLiteralParser
                    });
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

            lock (Locker)
            {
                // Remove parser from the customUriLiteralParserPerEdmType. Same instance can be registered to multiple EdmTypes.
                int numberOfParsersRemoved = customUriLiteralParserPerEdmType.RemoveAll((parser) => parser.UriLiteralParser.Equals(customUriLiteralParser));

                // Remove parser from the general custom uri literal parsers. Same instance can be add only once.
                bool isGeneralParserRemoved = customUriLiteralParsers.Remove(customUriLiteralParser);

                // Returns 'True' if at least one parser has been removed from the general parser of those registered to EdmType
                return (numberOfParsersRemoved > 0) || isGeneralParserRemoved;
            }
        }

        #endregion

        #region Private Methods

        private static bool IsEdmTypeAlreadyRegistered(IEdmTypeReference edmTypeReference)
        {
            return customUriLiteralParserPerEdmType.Any(uriParserOfEdmType =>
                uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference));
        }

        private static IUriLiteralParser GetUriLiteralParserByEdmType(IEdmTypeReference edmTypeReference)
        {
            UriLiteralParserPerEdmType requestedUriLiteralParser =
                customUriLiteralParserPerEdmType.FirstOrDefault(uriParserOfEdmType =>
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