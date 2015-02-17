//---------------------------------------------------------------------
// <copyright file="EntitySetReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Entity set reference
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class EntitySetReference : EntitySet
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetReference class with given name.
        /// </summary>
        /// <param name="name">Entity set name.</param>
        public EntitySetReference(string name)
            : base(name)
        {
        }
    }
}
