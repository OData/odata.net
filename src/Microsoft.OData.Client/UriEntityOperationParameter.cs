//---------------------------------------------------------------------
// <copyright file="UriEntityOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// Represents a function parameter that is either entity or collection of entities.
    /// </summary>
    public sealed class UriEntityOperationParameter : UriOperationParameter
    {
        /// <summary>
        /// If true, use entity reference instead of entity as the operation parameter.
        /// </summary>
        private readonly bool useEntityReference;

        /// <summary> Instantiates a new UriEntityOperationParameter </summary>
        /// <param name="name">The name of the uri operation parameter.</param>
        /// <param name="value">The value of the uri operation parameter.</param>
        public UriEntityOperationParameter(string name, Object value)
            : base(name, value)
        {
        }

        /// <summary> Instantiates a new UriOperationParameter </summary>
        /// <param name="name">The name of the uri operation parameter.</param>
        /// <param name="value">The value of the uri operation parameter.</param>
        /// <param name="useEntityReference">If true, use entity reference, instead of entity to serialize the parameter.</param>
        public UriEntityOperationParameter(string name, Object value, bool useEntityReference)
            : this(name, value)
        {
            this.useEntityReference = useEntityReference;
        }


        /// <summary>Use entity reference link.</summary>
        /// <returns>True if it uses entity reference link.</returns>
        internal bool UseEntityReference
        {
            get { return this.useEntityReference; }
        }
    }
}
