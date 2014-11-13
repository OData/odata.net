//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata description for a text construct (plain text, html or xhtml).
    /// </summary>
    public sealed class AtomTextConstruct : ODataAnnotatable
    {
        /// <summary>Gets or sets the kind of the text construct (plain text, html, xhtml).</summary>
        /// <returns>The kind of the text construct.</returns>
        public AtomTextConstructKind Kind
        {
            get;
            set;
        }

        /// <summary>Gets or sets the text content.</summary>
        /// <returns>The text content.</returns>
        public string Text
        {
            get;
            set;
        }

        /// <summary> Converts a string to an <see cref="T:Microsoft.OData.Core.Atom.AtomTextConstruct" /> instance. </summary>
        /// <returns>The <see cref="T:Microsoft.OData.Core.Atom.AtomTextConstruct" /> instance created for text.</returns>
        /// <param name="text">The <see cref="T:System.String" /> to convert to an <see cref="T:Microsoft.OData.Core.Atom.AtomTextConstruct" />.</param>
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
