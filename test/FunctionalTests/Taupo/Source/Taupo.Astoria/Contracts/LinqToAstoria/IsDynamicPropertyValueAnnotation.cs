//---------------------------------------------------------------------
// <copyright file="IsDynamicPropertyValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Annotation which indicates a query value came from a dynamic property
    /// </summary>
    public class IsDynamicPropertyValueAnnotation : QueryAnnotation
    {
        private static IsDynamicPropertyValueAnnotation instance = new IsDynamicPropertyValueAnnotation();

        /// <summary>
        /// Prevents a default instance of the <see cref="IsDynamicPropertyValueAnnotation"/> class from being created.
        /// </summary>
        private IsDynamicPropertyValueAnnotation()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the annotation.
        /// </summary>
        public static IsDynamicPropertyValueAnnotation Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone
        /// </returns>
        public override QueryAnnotation Clone()
        {
            return instance;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public override bool Equals(QueryAnnotation other)
        {
            return object.ReferenceEquals(this, other);
        }
    }
}
