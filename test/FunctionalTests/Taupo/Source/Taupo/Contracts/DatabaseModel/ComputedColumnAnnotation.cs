//---------------------------------------------------------------------
// <copyright file="ComputedColumnAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Annotation representing COMPUTED column (with computed values).
    /// </summary>
    public class ComputedColumnAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the ComputedColumnAnnotation class with default computed text.
        /// </summary>
        public ComputedColumnAnnotation()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComputedColumnAnnotation class with specific computed text.
        /// </summary>
        /// <param name="computedText">The text to compute the column value.</param>
        public ComputedColumnAnnotation(string computedText)
        {
            this.ComputedText = computedText;
        }

        /// <summary>
        /// Gets or sets the ComputedText value.
        /// </summary>
        /// <value>The start value.</value>
        public string ComputedText { get; set; }       
    }
}