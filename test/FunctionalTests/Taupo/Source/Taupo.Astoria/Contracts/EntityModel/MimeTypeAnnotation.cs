//---------------------------------------------------------------------
// <copyright file="MimeTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// MimeTypeAnnotation holds all the information required to build a mime type attribute
    /// </summary>
    public class MimeTypeAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the MimeTypeAnnotation class
        /// </summary>
        /// <param name="mimeTypeValue">Mime type value</param>
        public MimeTypeAnnotation(string mimeTypeValue)
        {
            this.MimeTypeValue = mimeTypeValue;
        }

        /// <summary>
        /// Initializes a new instance of the MimeTypeAnnotation class
        /// </summary>
        internal MimeTypeAnnotation()
        {
        }

        /// <summary>
        /// Gets mime type value
        /// </summary>
        public string MimeTypeValue { get; private set; }
    }
}
