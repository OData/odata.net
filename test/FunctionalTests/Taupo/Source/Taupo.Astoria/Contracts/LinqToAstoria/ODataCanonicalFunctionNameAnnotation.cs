//---------------------------------------------------------------------
// <copyright file="ODataCanonicalFunctionNameAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;

    /// <summary>
    /// Annotation to mark a built-in function with the name to use in OData uri's
    /// </summary>
    public class ODataCanonicalFunctionNameAnnotation : QueryAnnotation
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone
        /// </returns>
        public override QueryAnnotation Clone()
        {
            return new ODataCanonicalFunctionNameAnnotation() { Name = this.Name };
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
            var afterCast = other as ODataCanonicalFunctionNameAnnotation;
            if (afterCast == null)
            {
                return false;
            }

            return afterCast.Name == this.Name;
        }
    }
}