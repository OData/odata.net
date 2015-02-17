//---------------------------------------------------------------------
// <copyright file="EntityContainerReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Entity Container Reference
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EntityContainerReference : EntityContainer
    {
        /// <summary>
        /// Initializes a new instance of the EntityContainerReference class with given name.
        /// </summary>
        /// <param name="name">Entity container reference name</param>
        public EntityContainerReference(string name)
            : base(name)
        {
        }
    }
}
