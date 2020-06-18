//---------------------------------------------------------------------
// <copyright file="EntitySetAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Indicates the entity set to which a client data service class belongs.</summary>
    /// <remarks>
    /// This attribute is generated only when there is one entity set associated with the type.
    /// When there are more than one entity set associated with the type, then the entity set
    /// name can be passed in through the EntitySetNameResolver event.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class EntitySetAttribute : System.Attribute
    {
        /// <summary>
        /// The entity set name.
        /// </summary>
        private readonly string entitySet;

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.Client.EntitySetAttribute" />.</summary>
        /// <param name="entitySet">The entity set to which the class belongs.</param>
        public EntitySetAttribute(string entitySet)
        {
            this.entitySet = entitySet;
        }

        /// <summary>Gets the entity set to which the class belongs.</summary>
        /// <returns>The entity set as string value. </returns>
        public string EntitySet
        {
            get
            {
                return this.entitySet;
            }
        }
    }
}
