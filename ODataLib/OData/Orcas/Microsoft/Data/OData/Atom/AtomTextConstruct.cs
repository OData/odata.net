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

namespace Microsoft.Data.OData.Atom
{
    /// <summary>
    /// Atom metadata description for a text construct (plain text, html or xhtml).
    /// </summary>
    public sealed class AtomTextConstruct : ODataAnnotatable
    {
        /// <summary>
        /// The kind of the text construct (plain text, html, xhtml).
        /// </summary>
        public AtomTextConstructKind Kind
        {
            get;
            set;
        }

        /// <summary>
        /// The text content.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a string to an <see cref="AtomTextConstruct"/> instance.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to convert to an <see cref="AtomTextConstruct"/>.</param>
        /// <returns>The <see cref="AtomTextConstruct"/> instance created for <paramref name="text"/>.</returns>
        public static AtomTextConstruct ToTextConstruct(string text)
        {
            return new AtomTextConstruct
            {
                Text = text
            };
        }

        /// <summary>
        /// Implicit conversion from string to <see cref="AtomTextConstruct"/>.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to convert to an <see cref="AtomTextConstruct"/>.</param>
        /// <returns>The <see cref="AtomTextConstruct"/> result.</returns>
        public static implicit operator AtomTextConstruct(string text)
        {
            return ToTextConstruct(text);
        }
    }
}
