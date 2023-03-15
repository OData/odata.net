//---------------------------------------------------------------------
// <copyright file="UriFunctionAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Indicates a method that should be mapped to a Uri Function.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UriFunctionAttribute : Attribute
    {
        /// <summary>Allow client side evaluation.</summary>
        private readonly bool allowClientSideEvaluation;

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.UriFunctionAttribute" /> class. </summary>
        /// <param name="allowClientSideEvaluation">Use client side evaluation when possible. Default is false.</param>
        public UriFunctionAttribute(bool allowClientSideEvaluation = false)
        {
            this.allowClientSideEvaluation = allowClientSideEvaluation;
        }

        /// <summary>Can client side evaluation be used.</summary>
        /// <returns>Boolean value indicating if client side evaluation can be used. </returns>
        public bool AllowClientSideEvaluation
        {
            get { return this.allowClientSideEvaluation; }
        }
    }
}
