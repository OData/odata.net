//---------------------------------------------------------------------
// <copyright file="StoreFunctionMetadataAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation for additional metadata for function in storage model
    /// </summary>
    public class StoreFunctionMetadataAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoreFunctionMetadataAnnotation class.
        /// </summary>
        public StoreFunctionMetadataAnnotation()
        {
            // By default function is composable.
            this.IsComposable = true;
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the function returns an aggregate value.
        /// </summary>
        public bool IsAggregate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the function is built-in.
        /// </summary>
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the function is niladic.
        /// </summary>
        public bool IsNiladicFunction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the function is composable.
        /// </summary>
        public bool IsComposable { get; set; }

        /// <summary>
        /// Gets or sets the store function name.
        /// </summary>
        public string StoreFunctionName { get; set; }

        /// <summary>
        /// Gets or sets the store schema name.
        /// </summary>
        public string Schema { get; set; }
    }
}
