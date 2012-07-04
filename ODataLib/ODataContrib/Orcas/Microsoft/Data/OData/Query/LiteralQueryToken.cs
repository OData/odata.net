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
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a literal value.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class LiteralQueryToken : QueryToken
#else
    public sealed class LiteralQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        private readonly string originalText;

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Create a new LiteralQueryToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        public LiteralQueryToken(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Create a new LiteralQueryToken given value and originalText
        /// </summary>
        /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
        /// <param name="originalText">The original text value of the literal.</param>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal LiteralQueryToken(object value, string originalText)
            : this(value)
        {
            DebugUtils.CheckNoExternalCallers();
            this.originalText = originalText;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Literal; }
        }

        /// <summary>
        /// The value of the literal. This is a parsed primitive value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// The original text value of the literal.
        /// </summary>
        /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
        /// We should only use this during type promotion when applying metadata.</remarks>
        internal string OriginalText
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers(); 
                return this.originalText;
            }
        }
    }
}
