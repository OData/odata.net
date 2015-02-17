//---------------------------------------------------------------------
// <copyright file="EntitySetObjectData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Encapsulates an instance of an object and information about its corresponding <see cref="EntitySet"/>.
    /// </summary>
    public class EntitySetObjectData : IEntitySetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySetObjectData"/> class.
        /// </summary>
        /// <param name="data">The instance of an CLR type whose corresponding <see cref="EntityType"/> belongs to the <see cref="EntitySet"/>
        /// with the specified <paramref name="entitySetName"/>.</param>
        /// <param name="entitySetName">The name of the <see cref="EntitySet"/> to which the <paramref name="data"/> belongs.</param>
        public EntitySetObjectData(object data, string entitySetName)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");

            this.Data = data;
            this.EntitySetName = entitySetName;
        }

        /// <summary>
        /// Gets the container-qualified name of the <see cref="EntitySet"/> which <see cref="Data"/> belongs to.
        /// </summary>
        public string EntitySetName { get; private set; }

        /// <summary>
        /// Gets the instance of the CLR type which maps to this instance's <see cref="EntityType"/>.
        /// </summary>
        public object Data { get; private set; }
    }
}
