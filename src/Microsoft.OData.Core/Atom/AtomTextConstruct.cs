//---------------------------------------------------------------------
// <copyright file="AtomTextConstruct.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
