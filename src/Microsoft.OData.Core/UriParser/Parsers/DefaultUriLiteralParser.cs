//---------------------------------------------------------------------
// <copyright file="DefaultUriLiteralParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    internal sealed class DefaultUriLiteralParser : IUriLiteralParser
    {
        #region Fields

        // All Uri Literal Parsers
        private List<IUriLiteralParser> uriTypeParsers;

        #endregion

        // Use a ConditionalWeakTable to associate a single DefaultUriLiteralParser instance with each IEdmModel.
        // This ensures that each model gets its own parser instance, and allows the parser to be garbage collected
        // automatically when the model is no longer referenced, preventing memory leaks.
        private static readonly ConditionalWeakTable<IEdmModel, DefaultUriLiteralParser> _cwTable
                = new ConditionalWeakTable<IEdmModel, DefaultUriLiteralParser>();

        public static DefaultUriLiteralParser GetOrCreate(IEdmModel model)
        {
            if (model == null)
            {
                throw new System.ArgumentNullException(nameof(model));
            }

            // The idea is to have a single DefaultUriLiteralParser instance for each model
            return _cwTable.GetValue(model, m => new DefaultUriLiteralParser(m));
        }

        private DefaultUriLiteralParser(IEdmModel model)
        {
            // It is important that custom URI literal parsers will be added first, so it will be called before the others built-in parsers
            this.uriTypeParsers = new List<IUriLiteralParser>
            {
                { new CustomUriLiteralParser(model) },
                { UriPrimitiveTypeParser.Instance }
            };
        }

        #region IUriLiteralParser implementation

        /// <summary>
        /// Try to parse the given text by each parser.
        /// </summary>
        /// <param name="text">Part of the Uri which has to be parsed to a value of EdmType <paramref name="targetType"/></param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the <paramref name="targetType"/> but failed during the parsing process</param>
        /// <returns>If the parsing process has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
        {
            parsingException = null;
            object targetValue;

            // Try to parse the uri text with each parser
            foreach (IUriLiteralParser uriTypeParser in this.uriTypeParsers)
            {
                targetValue = uriTypeParser.ParseUriStringToType(text, targetType, out parsingException);

                // Stop in case the parser has returned an exception
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

            return null;
        }

        #endregion IUriLiteralParser implementation

        /// <summary>
        /// Provides management and dispatching of custom URI literal parsers registered with an <see cref="IEdmModel"/>.
        /// Acts as an <see cref="IUriLiteralParser"/> implementation that delegates parsing requests to type-specific
        /// or general custom parsers associated with the model.
        /// </summary>
        private sealed class CustomUriLiteralParser : IUriLiteralParser
        {
            #region Singleton

            private IEdmModel model;

            internal CustomUriLiteralParser(IEdmModel model)
            {
                this.model = model;
            }

            #endregion

            #region IUriLiteralParser implementation - CustomUriLiteralParser

            /// <summary>
            /// Parse the given text of Edm type <paramref name="targetType"/> to it's object instance.
            /// </summary>
            /// <param name="text">Part of the URI which has to be parsed to a value of Edm type <paramref name="targetType"/>.</param>
            /// <param name="targetType">The Edm type which the URI text has to be parsed to.</param>
            /// <param name="parsingException">
            /// When the parser recognizes the <paramref name="targetType"/> and attempts to parse <paramref name="text"/>, 
            /// but an error occurs during the parsing process (for example, invalid format or value out of range), 
            /// assign a <see cref="UriLiteralParsingException"/> describing the failure to this parameter.
            /// Set to <c>null</c> if parsing is successful or if the parser does not support the specified <paramref name="targetType"/>.
            /// </param>
            /// <returns>The parsed object if parsing process succeeds; otherwise <c>null</c>.</returns>
            /// <remarks>
            /// The <paramref name="parsingException"/> parameter should be assigned a non-null value only
            /// if the parser supports the specified <paramref name="targetType"/> and an error occurs
            /// during the parsing process (for example, due to invalid format or value out of range).
            /// If the parser does not support the <paramref name="targetType"/>, or if parsing is successful,
            /// <paramref name="parsingException"/> must be set to <c>null</c>.
            /// This method is public because of the interface, but the singleton instance is internal so it cannot be accessed externally.
            /// </remarks>
            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                CustomUriLiteralParsersAnnotation customUriLiteralParsersAnnotation =
                    this.model.GetOrSetCustomUriLiteralParsersAnnotation();

                // Search for custom URI literal parser which is registered for the given Edm type
                if (customUriLiteralParsersAnnotation.CustomUriLiteralParsersByEdmType.TryGetValue(targetType, out IUriLiteralParser uriLiteralParserForEdmType))
                {
                    return uriLiteralParserForEdmType.ParseUriStringToType(text, targetType, out parsingException);
                }

                // Parse with the general URI literal parsers
                // Stop when a custom URI literal parser succeeded parsing the text.
                foreach (KeyValuePair<IUriLiteralParser, byte> kvPair in customUriLiteralParsersAnnotation.CustomUriLiteralParsers)
                {
                    IUriLiteralParser customUriLiteralParser = kvPair.Key;

                    // Try to parse
                    object targetValue = customUriLiteralParser.ParseUriStringToType(text, targetType, out parsingException);

                    // The custom URI literal parser could parse the given targetType but failed during the parsing process
                    if (parsingException != null)
                    {
                        return null;
                    }

                    // TODO: This has been the existing behavior, but what if null is a valid return value for the particular scenario?
                    // In case of no exception and no value - The parse cannot parse the given text
                    if (targetValue != null)
                    {
                        return targetValue;
                    }
                }

                // No custom URI literal parser could parse the requested URI text.
                parsingException = null;

                return null;
            }

            #endregion IUriLiteralParser implementation - CustomUriLiteralParser
        }
    }
}