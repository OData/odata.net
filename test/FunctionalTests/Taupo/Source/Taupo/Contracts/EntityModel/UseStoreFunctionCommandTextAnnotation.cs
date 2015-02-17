//---------------------------------------------------------------------
// <copyright file="UseStoreFunctionCommandTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation for additional metadata for function in storage model
    /// </summary>
    public class UseStoreFunctionCommandTextAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the UseStoreFunctionCommandTextAnnotation class.
        /// </summary>
        /// <param name="useCommandText">Whether the command should be inline or in the store</param>
        public UseStoreFunctionCommandTextAnnotation(bool useCommandText)
        {
            this.UseCommandText = useCommandText;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the body should be used as CommandText or as a function body
        /// </summary>
        public bool UseCommandText { get; set; }
    }
}
