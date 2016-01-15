//---------------------------------------------------------------------
// <copyright file="UriCustomTypeParsers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers.TypeParsers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion

    /// <summary>
    /// This class is the CustomTypeParser manager and parser.
    /// Add a UriCustomTypeParser thourgh this class.
    /// This class is also used as an UriTypeParser.
    /// </summary>
    public class UriCustomTypeParsers : IUriTypeParser
    {
        #region Fields

        /// <summary>
        /// Used for General UriTypeParsers. These parsers will be called for every text that has to parsed.
        /// The parses could parse multiple EdmTypes.
        /// </summary>
        private static List<IUriTypeParser> CustomUriTypeParsers;

        private static object Locker = new object();

        /// <summary>
        /// "Registered" UriTypeParser to an EdmType. These parses will be called when the text has to be parsed to the
        /// specific EdmType they had registered to. Each of these parsers could parse only one EdmType. Better performace.
        /// </summary>
        private static List<UriTypeParserPerEdmType> UriCustomTypeParserPerEdmType;
        //// Consider use Dictionary<EmdTypeReference,IUriTypeParser> which is a better solution.
        //// the problem with dictionary - Generate an HashCode for an EdmTypeReference.

        #endregion

        #region Singleton

        // Internal Singleton so only interal assemblies could parse by the custom parsers.
        private static UriCustomTypeParsers SingleInstance;

        static UriCustomTypeParsers()
        {
            CustomUriTypeParsers = new List<IUriTypeParser>();
            UriCustomTypeParserPerEdmType = new List<UriTypeParserPerEdmType>();
        }

        private UriCustomTypeParsers() 
        {
        }

        internal static UriCustomTypeParsers Instance
        {
            get
            {
                if (SingleInstance == null)
                {
                    SingleInstance = new UriCustomTypeParsers();
                }

                return SingleInstance;
            }
        }

        #endregion

        #region IUriTypeParser Implementation - Internal

        /// <summary>
        /// Parse the given uri text.
        /// Try to parse with a specific UriTypeParser regeistered for the target EdmType.
        /// If no parser is registered, try to parse with the general parsers.
        /// This method is public becuase of the Interface, but the Singleton instance in internal so it could not be accessed by clients. 
        /// </summary>
        /// <param name="text">Part of the Uri which has to be parsed to a value of EdmType 'targetType'</param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the 'targetType' but failed during the parsing process</param>
        /// <returns>If parsing proceess has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
        {
            object targetValue;

            lock (Locker)
            {
                // Search for UriTypeParser which is registered for the given EdmType
                IUriTypeParser uriTypeParserForEdmType = GetUriTypeParserByEdmType(targetType);
                if (uriTypeParserForEdmType != null)
                {
                    return uriTypeParserForEdmType.ParseUriStringToType(text, targetType, out parsingException);
                }

                // Parse with all the general parsers
                // Stop when a parser succeeded parsing the text.
                foreach (IUriTypeParser uriCustomTypeParser in CustomUriTypeParsers)
                {
                    // Try to parse
                    targetValue = uriCustomTypeParser.ParseUriStringToType(text, targetType, out parsingException);

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
        /// Add a custom 'IUriTypeParser' which will be called to parse uri values during the uri parsing proccess.
        /// </summary>
        /// <param name="uriCustomTypeParser">The custom uri parser</param>
        /// <exception cref="ArgumentNullException">uriCustomTypeParser is null</exception>
        /// <exception cref="ODataException">The given IUriTypeParser instance already exists</exception>
        public static void AddCustomUriTypeParser(IUriTypeParser uriCustomTypeParser)
        {
            ExceptionUtils.CheckArgumentNotNull(uriCustomTypeParser, "uriCustomTypeParser");

            lock (Locker)
            {
                if (CustomUriTypeParsers.Contains(uriCustomTypeParser))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists);
                }

                CustomUriTypeParsers.Add(uriCustomTypeParser);
            }
        }

        /// <summary>
        /// Add a custom 'IUriTypeParser' which will be called to parse a value of the given EdmType during the UriParsing proccess.
        /// </summary>
        /// <param name="edmTypeReference">The EdmType the UriTypeParser can parse.</param>
        /// <param name="uriCustomTypeParser">The custom uri type parser to add.</param>
        /// <exception cref="ArgumentNullException">UriCustomTypeParser is null.</exception>
        /// <exception cref="ArgumentNullException">EdmTypeReference is null.</exception>
        /// <exception cref="ODataException">Another UriTypeParser is already registered for the given EdmType</exception>
        public static void AddCustomUriTypeParser(IEdmTypeReference edmTypeReference, IUriTypeParser uriCustomTypeParser)
        {
            ExceptionUtils.CheckArgumentNotNull(uriCustomTypeParser, "uriCustomTypeParser");

            ExceptionUtils.CheckArgumentNotNull(edmTypeReference, "edmTypeReference");

            lock (Locker)
            {
                if (IsEdmTypeAlreadyRegistered(edmTypeReference))
                {
                    throw new ODataException(ODataErrorStrings.UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(edmTypeReference.FullName()));
                }

                UriCustomTypeParserPerEdmType.Add(
                    new UriTypeParserPerEdmType
                    {
                        EdmTypeOfUriParser = edmTypeReference,
                        UriTypeParser = uriCustomTypeParser
                    });
            }
        }

        /// <summary>
        /// Remove the given custom 'IUriTypeParser' form cache.
        /// It will be removed from both regular parsers and parsers registered with EdmType.
        /// </summary>
        /// <param name="uriCustomTypeParser">The custom uri type parser to remove</param>
        /// <returns>'False' if the given parser to remove doesn't exist. 'True' if the parser has successfully removed</returns>
        /// <exception cref="ArgumentNullException">UriTypeParser is null</exception>
        public static bool RemoveCustomUriTypeParser(IUriTypeParser uriCustomTypeParser)
        {
            ExceptionUtils.CheckArgumentNotNull(uriCustomTypeParser, "uriCustomTypeParser");

            lock (Locker)
            {
                // Remove parser from the UriCustmTypeParsers per EdmType. Same instance can be registered to multiple EdmTypes.
                int numberOfParsersRemoved = UriCustomTypeParserPerEdmType.RemoveAll((parser) => parser.UriTypeParser.Equals(uriCustomTypeParser));

                // Remove parser from the general UriCustmTypeParsers. Same instacne can be add only once.
                bool isGeneralParserRemoved = CustomUriTypeParsers.Remove(uriCustomTypeParser);

                // Returns 'True' if at least one parser has been removed from the general parser of those registered to EdmType
                return Convert.ToBoolean(numberOfParsersRemoved) || isGeneralParserRemoved;
            }
        }

        #endregion

        #region Private Methods
        
        private static bool IsEdmTypeAlreadyRegistered(IEdmTypeReference edmTypeReference)
        {
            return UriCustomTypeParserPerEdmType.Any(uriParserOfEdmType => 
                uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference));
        }

        private static IUriTypeParser GetUriTypeParserByEdmType(IEdmTypeReference edmTypeReference)
        {
            UriTypeParserPerEdmType requestedUriTypeParser =
                UriCustomTypeParserPerEdmType.FirstOrDefault(uriParserOfEdmType =>
                uriParserOfEdmType.EdmTypeOfUriParser.IsEquivalentTo(edmTypeReference));

            if (requestedUriTypeParser == null)
            {
                return null;
            }

            return requestedUriTypeParser.UriTypeParser;
        }


        #endregion

        private class UriTypeParserPerEdmType
        {
            internal IEdmTypeReference EdmTypeOfUriParser { get; set; }

            internal IUriTypeParser UriTypeParser { get; set; }
        }
    }
}