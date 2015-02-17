//---------------------------------------------------------------------
// <copyright file="JsonTextAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;

    /// <summary>
    /// Base class for all the text annotations for JSON OM.
    /// </summary>
    public abstract class JsonTextAnnotation : JsonAnnotation
    {
        /// <summary>
        /// Gets or sets the text representation of the value in question.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>the clone of the annotation</returns>
        public override sealed JsonAnnotation Clone()
        {
            var annotation = (JsonTextAnnotation)Activator.CreateInstance(this.GetType());
            annotation.Text = this.Text;
            return annotation;
        }
    }
}
