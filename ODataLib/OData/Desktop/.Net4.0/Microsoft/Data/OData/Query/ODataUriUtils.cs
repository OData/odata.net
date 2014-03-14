//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Spatial;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion

    /// <summary>
    /// URI Utility methods.
    /// </summary>
    public static class ODataUriUtils
    {
        /// <summary>
        /// Converts the given <paramref name="value"/> to a corresponding CLR type. Expects the 
        /// <paramref name="value"/> to have already been properly unescaped from an actual Uri.
        /// </summary>
        /// <param name="value">Value from a Uri to be converted.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <returns>A CLR object that the <paramref name="value"/> represents.</returns>
        public static object ConvertFromUriLiteral(string value, ODataVersion version)
        {
            return ODataUriUtils.ConvertFromUriLiteral(value, version, null, null);
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> to a corresponding CLR type. Expects the 
        /// <paramref name="value"/> to have already been properly unescaped from an actual Uri.
        /// </summary>
        /// <param name="value">Value from a Uri to be converted.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <param name="model">Optional model to perform verification against.</param>
        /// <param name="typeReference">Optional IEdmTypeReference to perform verification against. 
        ///  Callers must provide a <paramref name="model"/> containing this type if it is specified.</param>
        /// <returns>A CLR object that the <paramref name="value"/> represents.</returns>
        public static object ConvertFromUriLiteral(string value, ODataVersion version, IEdmModel model, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");
            if (typeReference != null && model == null)
            {
                throw new ODataException(ODataErrorStrings.ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel);
            }

            if (model == null)
            {
                model = Microsoft.Data.Edm.Library.EdmCoreModel.Instance;
            }

            // Let ExpressionLexer try to get a primitive
            ExpressionLexer lexer = new ExpressionLexer(value, false /*moveToFirstToken*/, false /*useSemicolonDelimeter*/);
            Exception error;
            ExpressionToken token;

            lexer.TryPeekNextToken(out token, out error);

            if (token.Kind == ExpressionTokenKind.BracketedExpression)
            {
                // Should be a complex or collection value
                return ODataUriConversionUtils.ConvertFromComplexOrCollectionValue(value, version, model, typeReference);
            }

            object result = lexer.ReadLiteralToken();

            // If we have a typeReference then perform verification and convert if necessary
            if (typeReference != null)
            {
                result = ODataUriConversionUtils.VerifyAndCoerceUriPrimitiveLiteral(result, model, typeReference, version);
            }

            if (result is ISpatial)
            {
                ODataVersionChecker.CheckSpatialValue(version);
            }

            return result;  
        }

        /// <summary>
        /// Converts the given object to a string for use in a Uri. Does not perform any of the escaping that <see cref="System.Uri"/> provides.
        /// No type verification is used.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <returns>A string representation of <paramref name="value"/> for use in a Url.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "designed to aid the creation on a URI, not create a full one")]
        public static string ConvertToUriLiteral(object value, ODataVersion version)
        {
            return ODataUriUtils.ConvertToUriLiteral(value, version, null);
        }

        /// <summary>
        /// Converts the given object to a string for use in a Uri. Does not perform any of the escaping that <see cref="System.Uri"/> provides.
        /// Will perform type verification based on the given model if possible.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <param name="model">Optional model to perform verification against.</param>
        /// <returns>A string representation of <paramref name="value"/> for use in a Url.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "designed to aid the creation on a URI, not create a full one")]
        public static string ConvertToUriLiteral(object value, ODataVersion version, IEdmModel model)
        {
            return ConvertToUriLiteral(value, version, model, ODataFormat.VerboseJson);
        }

        /// <summary>
        /// Converts the given object to a string in the specified format for use in a Uri. Does not perform any of the escaping that <see cref="System.Uri"/> provides.
        /// Will perform type verification based on the given model if possible.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="version">Version to be compliant with.</param>
        /// <param name="model">Optional model to perform verification against.</param>
        /// <param name="format">ODataFormat to use for structured values such as complex types and collections.</param>
        /// <returns>A string representation of <paramref name="value"/> for use in a Url.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "designed to aid the creation on a URI, not create a full one")]
        public static string ConvertToUriLiteral(object value, ODataVersion version, IEdmModel model, ODataFormat format)
        {
            if (value == null)
            {
                value = new ODataUriNullValue();
            }

            if (model == null)
            {
                model = Microsoft.Data.Edm.Library.EdmCoreModel.Instance;
            }

            ODataUriNullValue nullValue = value as ODataUriNullValue;
            if (nullValue != null)
            {
                return ODataUriConversionUtils.ConvertToUriNullValue(nullValue);
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return ODataUriConversionUtils.ConvertToUriCollectionLiteral(collectionValue, model, version, format);
            }

            ODataComplexValue complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                return ODataUriConversionUtils.ConvertToUriComplexLiteral(complexValue, model, version, format);
            }

            return ODataUriConversionUtils.ConvertToUriPrimitiveLiteral(value, version);
        }
    }
}
