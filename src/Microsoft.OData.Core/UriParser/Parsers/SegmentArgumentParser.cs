//---------------------------------------------------------------------
// <copyright file="SegmentArgumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;

    /// <summary>Provides a class used to represent a key for a resource.</summary>
    /// <remarks>
    /// Internally, every key instance has a collection of values. These values
    /// can be named or positional, depending on how they were specified
    /// if parsed from a URI.
    /// </remarks>
    internal sealed class SegmentArgumentParser
    {
        /// <summary>Empty key singleton.</summary>
        private static readonly SegmentArgumentParser Empty = new SegmentArgumentParser();

        /// <summary>Whether or not the key was formatted as a segment.</summary>
        private readonly bool keysAsSegment;

        /// <summary>Whether Uri template parsing is enabled.</summary>
        private readonly bool enableUriTemplateParsing;

        /// <summary>Named values.</summary>
        private Dictionary<string, string> namedValues;

        /// <summary>Positional values.</summary>
        private List<string> positionalValues;

        /// <summary>Initializes a new empty <see cref="SegmentArgumentParser"/> instance.</summary>
        private SegmentArgumentParser()
        {
        }

        /// <summary>Initializes a new <see cref="SegmentArgumentParser"/> instance.</summary>
        /// <param name='namedValues'>Named values.</param>
        /// <param name='positionalValues'>Positional values for this instance.</param>
        /// <param name="keysAsSegment">Whether or not the key was formatted as a segment.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <remarks>
        /// One of namedValues or positionalValues should be non-null, but not both.
        /// </remarks>
        private SegmentArgumentParser(Dictionary<string, string> namedValues, List<string> positionalValues, bool keysAsSegment, bool enableUriTemplateParsing)
        {
            Debug.Assert(
                (namedValues == null) != (positionalValues == null),
                "namedValues == null != positionalValues == null -- one or the other should be assigned, but not both");
            this.namedValues = namedValues;
            this.positionalValues = positionalValues;
            this.keysAsSegment = keysAsSegment;
            this.enableUriTemplateParsing = enableUriTemplateParsing;
        }

        /// <summary>Whether the values have a name.</summary>
        public bool AreValuesNamed
        {
            get
            {
                return this.namedValues != null;
            }
        }

        /// <summary>Checks whether this key has any values.</summary>
        public bool IsEmpty
        {
            get
            {
                return this == Empty;
            }
        }

        /// <summary>Returns a dictionary of named values when they AreValuesNamed is true.</summary>
        public IDictionary<string, string> NamedValues
        {
            get
            {
                return this.namedValues;
            }
        }

        /// <summary>Returns a list of values when they AreValuesNamed is false.</summary>
        public IList<string> PositionalValues
        {
            get
            {
                return this.positionalValues;
            }
        }

        /// <summary>Whether or not the key was formatted as a segment.</summary>
        public bool KeyAsSegment
        {
            get { return this.keysAsSegment; }
        }

        /// <summary>Number of values in the key.</summary>
        public int ValueCount
        {
            get
            {
                if (this == Empty)
                {
                    return 0;
                }

                if (this.namedValues != null)
                {
                    return this.namedValues.Count;
                }

                Debug.Assert(this.positionalValues != null, "this.positionalValues != null");
                return this.positionalValues.Count;
            }
        }

        /// <summary>
        /// Add a new value to the named values list.
        /// </summary>
        /// <param name="key">the new key</param>
        /// <param name="value">the new value</param>
        public void AddNamedValue(string key, string value)
        {
            CreateIfNull(ref this.namedValues);
            if (!namedValues.ContainsKey(key))
            {
                this.namedValues[key] = value;
            }
        }

        /// <summary>Attempts to parse key values from the specified text.</summary>
        /// <param name='text'>Text to parse (not null).</param>
        /// <param name='instance'>After invocation, the parsed key instance.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>
        /// true if the key instance was parsed; false if there was a 
        /// syntactic error.
        /// </returns>
        /// <remarks>
        /// The returned instance contains only string values. To get typed values, a call to
        /// TryConvertValues is necessary.
        /// </remarks>
        public static bool TryParseKeysFromUri(string text, out SegmentArgumentParser instance, bool enableUriTemplateParsing)
        {
            return TryParseFromUri(text, true /*allowNamedValues*/, false /*allowNull*/, out instance, enableUriTemplateParsing);
        }

        /// <summary>
        /// Creates a key instance from the given raw segment text with a single positional value.
        /// </summary>
        /// <param name="segmentText">The segment text.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>A key instance with the given segment text as its only value.</returns>
        public static SegmentArgumentParser FromSegment(string segmentText, bool enableUriTemplateParsing)
        {
            return new SegmentArgumentParser(null, new List<string> { segmentText }, true, enableUriTemplateParsing);
        }

        /// <summary>Attempts to parse nullable values (only positional values, no name-value pairs) from the specified text.</summary>
        /// <param name='text'>Text to parse (not null).</param>
        /// <param name='instance'>After invocation, the parsed key instance.</param>
        /// <returns>
        /// true if the given values were parsed; false if there was a 
        /// syntactic error.
        /// </returns>
        /// <remarks>
        /// The returned instance contains only string values. To get typed values, a call to
        /// TryConvertValues is necessary.
        /// </remarks>
        public static bool TryParseNullableTokens(string text, out SegmentArgumentParser instance)
        {
            return TryParseFromUri(text, false /*allowNamedValues*/, true /*allowNull*/, out instance, false);
        }

        /// <summary>Tries to convert values to the keys of the specified type.</summary>
        /// <param name="targetEntityType">The specified type.</param>
        /// <param name="keyPairs">The converted key-value pairs.</param>
        /// <param name="resolver">The resolver to use.</param>
        /// <returns>true if all values were converted; false otherwise.</returns>
        public bool TryConvertValues(IEdmEntityType targetEntityType, out IEnumerable<KeyValuePair<string, object>> keyPairs, ODataUriResolver resolver)
        {
            Debug.Assert(!this.IsEmpty, "!this.IsEmpty -- caller should check");
            IList<IEdmStructuralProperty> keyProperties = targetEntityType.Key().ToList();
            Debug.Assert(keyProperties.Count == this.ValueCount || resolver.GetType() != typeof(ODataUriResolver), "type.KeyProperties.Count == this.ValueCount -- will change with containment");

            if (this.NamedValues != null)
            {
                keyPairs = resolver.ResolveKeys(targetEntityType, this.NamedValues, this.ConvertValueWrapper);
            }
            else
            {
                Debug.Assert(this.positionalValues != null, "positionalValues != null -- otherwise this is Empty");
                Debug.Assert(this.PositionalValues.Count == keyProperties.Count || resolver.GetType() != typeof(ODataUriResolver), "Count of positional values does not match.");
                keyPairs = resolver.ResolveKeys(targetEntityType, this.PositionalValues, this.ConvertValueWrapper);
            }

            return true;
        }

        /// <summary>
        /// Wrapper for TryConvertValue
        /// </summary>
        /// <param name="typeReference">the type to convert to (primitive or enum type)</param>
        /// <param name="valueText">the value to convert</param>
        /// <returns>Converted value, null if fails.</returns>
        private object ConvertValueWrapper(IEdmTypeReference typeReference, string valueText)
        {
            object value;
            if (!this.TryConvertValue(typeReference, valueText, out value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        /// Try to convert a value into an EDM primitive type, if template parsing enabled, the <paramref name="valueText"/> matching
        /// template would be converted into corresponding UriTemplateExpression.
        /// </summary>
        /// <param name="typeReference">the type to convert to (primitive or enum type)</param>
        /// <param name="valueText">the value to convert</param>
        /// <param name="convertedValue">The converted value, if conversion succeeded.</param>
        /// <returns>true if the conversion was successful.</returns>
        private bool TryConvertValue(IEdmTypeReference typeReference, string valueText, out object convertedValue)
        {
            UriTemplateExpression expression;
            if (this.enableUriTemplateParsing && UriTemplateParser.TryParseLiteral(valueText, typeReference, out expression))
            {
                convertedValue = expression;
                return true;
            }

            if (typeReference.IsEnum())
            {
                QueryNode enumNode = null;
                if (EnumBinder.TryBindIdentifier(valueText, typeReference.AsEnum(), null, out enumNode))
                {
                    convertedValue = enumNode;
                    return true;
                }

                convertedValue = null;
                return false;
            }

            IEdmPrimitiveTypeReference primitiveType = typeReference.AsPrimitive();
            Type primitiveClrType = EdmLibraryExtensions.GetPrimitiveClrType((IEdmPrimitiveType)primitiveType.Definition, primitiveType.IsNullable);
            LiteralParser literalParser = LiteralParser.ForKeys(this.keysAsSegment);
            return literalParser.TryParseLiteral(primitiveClrType, valueText, out convertedValue);
        }

        /// <summary>Attempts to parse key values from the specified text.</summary>
        /// <param name='text'>Text to parse (not null).</param>
        /// <param name="allowNamedValues">Set to true if the parser should accept named values
        ///     so syntax like Name='value'. If this is false, the parsing will fail on such constructs.</param>
        /// <param name="allowNull">Set to true if the parser should accept null values.
        ///     If set to false, the parser will fail on null values.</param>
        /// <param name='instance'>After invocation, the parsed key instance.</param>
        /// <param name="enableUriTemplateParsing">Whether Uri template parsing is enabled.</param>
        /// <returns>
        /// true if the key instance was parsed; false if there was a 
        /// syntactic error.
        /// </returns>
        /// <remarks>
        /// The returned instance contains only string values. To get typed values, a call to
        /// TryConvertValues is necessary.
        /// </remarks>
        private static bool TryParseFromUri(string text, bool allowNamedValues, bool allowNull, out SegmentArgumentParser instance, bool enableUriTemplateParsing)
        {
            Debug.Assert(text != null, "text != null");

            Dictionary<string, string> namedValues = null;
            List<string> positionalValues = null;

            // parse keys just like function parameters
            ExpressionLexer lexer = new ExpressionLexer("(" + text + ")", true, false);
            UriQueryExpressionParser exprParser = new UriQueryExpressionParser(ODataUriParserSettings.DefaultFilterLimit /* default limit for parsing key value */, lexer);
            var tmp = (new FunctionCallParser(lexer, exprParser)).ParseArgumentListOrEntityKeyList();
            if (lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                instance = null;
                return false;
            }

            if (tmp.Length == 0)
            {
                instance = Empty;
                return true;
            }

            string valueText = null;
            foreach (FunctionParameterToken t in tmp)
            {
                valueText = null;
                LiteralToken literalToken = t.ValueToken as LiteralToken;
                if (literalToken != null)
                {
                    valueText = literalToken.OriginalText;

                    // disallow "{...}" if enableUriTemplateParsing is false (which could have been seen as valid function parameter, e.g. array notation)
                    if (!enableUriTemplateParsing && UriTemplateParser.IsValidTemplateLiteral(valueText))
                    {
                        instance = null;
                        return false;
                    }
                }
                else
                {
                    DottedIdentifierToken dottedIdentifierToken = t.ValueToken as DottedIdentifierToken; // for enum
                    if (dottedIdentifierToken != null)
                    {
                        valueText = dottedIdentifierToken.Identifier;
                    }
                }

                if (valueText != null)
                {
                    if (t.ParameterName == null)
                    {
                        if (namedValues != null)
                        {
                            instance = null; // We cannot mix named and non-named values.
                            return false;
                        }

                        CreateIfNull(ref positionalValues);
                        positionalValues.Add(valueText);
                    }
                    else
                    {
                        if (positionalValues != null)
                        {
                            instance = null; // We cannot mix named and non-named values.
                            return false;
                        }

                        CreateIfNull(ref namedValues);
                        namedValues.Add(t.ParameterName, valueText);
                    }
                }
                else
                {
                    instance = null;
                    return false;
                }
            }

            instance = new SegmentArgumentParser(namedValues, positionalValues, false, enableUriTemplateParsing);
            return true;
        }

        /// <summary>Creates a new instance if the specified value is null.</summary>
        /// <typeparam name="T">Type of variable.</typeparam>
        /// <param name="value">Current value.</param>
        private static void CreateIfNull<T>(ref T value) where T : new()
        {
            if (value == null)
            {
                value = new T();
            }
        }
    }
}
