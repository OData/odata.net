//---------------------------------------------------------------------
// <copyright file="EntityContainerDataExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods for EntityContainerData
    /// </summary>
    public static class EntityContainerDataExtensions
    {
        /// <summary>
        /// Gets the related keys for the given roleName in an association(denoted by associationSetName), given this end's key.
        /// </summary>
        /// <param name="data">The EntityContainerData which contains AssociationSetData.</param>
        /// <param name="associationSetName">The name of the association set.</param>
        /// <param name="roleName">The roleName of the known end.</param>
        /// <param name="roleKey">The key for the role of the known end.</param>
        /// <returns>The Related keys of the other end.</returns>
        public static IEnumerable<EntityDataKey> GetRelatedKeys(this EntityContainerData data, string associationSetName, string roleName, EntityDataKey roleKey)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(associationSetName, "associationSetName");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(roleName, "roleName");
            ExceptionUtilities.CheckArgumentNotNull(roleKey, "roleKey");

            var associationSetData = data.GetAssociationSetData(associationSetName);
            string otherRoleName = associationSetData.AssociationSet.AssociationType.Ends.Where(e => e.RoleName != roleName).Single().RoleName;

            return associationSetData.Rows.Where(r => roleKey.Equals(r.GetRoleKey(roleName))).Select(r => r.GetRoleKey(otherRoleName));
        }
    }
}
