//---------------------------------------------------------------------
// <copyright file="IdentityColumnAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Annotation representing IDENTITY column (with automatically generated values).
    /// </summary>
    public class IdentityColumnAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the IdentityColumnAnnotation class with default identity parameters.
        /// </summary>
        public IdentityColumnAnnotation()
            : this(1, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IdentityColumnAnnotation class with specific identity parameters.
        /// </summary>
        /// <param name="startValue">The start value.</param>
        /// <param name="incrementBy">The increment by value.</param>
        public IdentityColumnAnnotation(int startValue, int incrementBy)
        {
            this.StartValue = startValue;
            this.IncrementBy = incrementBy;
        }

        /// <summary>
        /// Gets or sets the start value.
        /// </summary>
        /// <value>The start value.</value>
        public int StartValue { get; set; }

        /// <summary>
        /// Gets or sets the increment by value.
        /// </summary>
        /// <value>The increment by.</value>
        public int IncrementBy { get; set; }
    }
}