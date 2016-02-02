﻿//---------------------------------------------------------------------
// <copyright file="DefaultUriTypeParser.cs" company="Microsoft">
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
    using Microsoft.OData.Core.UriParser.Parsers.TypeParsers.Common;
    using Microsoft.OData.Edm;

    #endregion

    internal sealed class DefaultUriTypeParser : IUriTypeParser
    {
        #region Fields

        // All Uri Type Parsers
        private List<IUriTypeParser> UriTypeParsers;

        #endregion

        #region Singleton

        private static DefaultUriTypeParser SingleInstance = new DefaultUriTypeParser();

        private DefaultUriTypeParser()
        {
            // It is important that UriCustomTypeParsers will be added first, so it will be called before the others built-in parsers
            UriTypeParsers = new List<IUriTypeParser> 
            {
                { UriCustomTypeParsers.Instance },
                { UriPrimitiveTypeParser.Instance } 
            };
        }

        internal static DefaultUriTypeParser Instance
        {
            get
            {
                return SingleInstance;
            }
        }

        #endregion

        #region IUriTypeParser Implementation

        /// <summary>
        /// Try to parse the given text by each parser.
        /// </summary>
        /// <param name="text">Part of the Uri which has to be parsed to a value of EdmType <paramref name="targetType"/></param>
        /// <param name="targetType">The type which the uri text has to be parsed to</param>
        /// <param name="parsingException">Assign the exception only in case the text could be parsed to the <paramref name="targetType"/> but failed during the parsing process</param>
        /// <returns>If the parsing proceess has succeeded, returns the parsed object, otherwise returns 'Null'</returns>
        public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriTypeParsingException parsingException)
        {
            parsingException = null;
            object targetValue;

            // Try to parse the uri text with each parser
            foreach (IUriTypeParser uriTypeParser in UriTypeParsers)
            {
                targetValue = uriTypeParser.ParseUriStringToType(text, targetType, out parsingException);

                // Stop in case the parser has returned an excpetion
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

        #endregion
    }
}