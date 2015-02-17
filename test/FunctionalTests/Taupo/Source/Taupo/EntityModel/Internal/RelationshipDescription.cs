//---------------------------------------------------------------------
// <copyright file="RelationshipDescription.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Describes relationship.
    /// </summary>
    internal class RelationshipDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipDescription"/> class.
        /// </summary>
        /// <param name="associationSet">The association set.</param>
        /// <param name="from">EntityDataKey for the 'from' end.</param>
        /// <param name="toRoleName">Role name for the 'to' end</param>
        /// <param name="to">EntityDataKey for the 'to' end.</param>
        public RelationshipDescription(AssociationSet associationSet, EntityDataKey from, string toRoleName, EntityDataKey to)
        {
            this.AssociationSet = associationSet;
            this.From = from;
            this.ToRoleName = toRoleName;
            this.To = to;
            this.FromRoleName = this.AssociationSet.Ends.Where(e => e.AssociationEnd.RoleName != toRoleName).Single().AssociationEnd.RoleName;
        }

        /// <summary>
        /// Gets the associtation set.
        /// </summary>
        /// <value>The associtation set.</value>
        public AssociationSet AssociationSet { get; private set; }

        /// <summary>
        /// Gets EntityDataKey for the 'from' end.
        /// </summary>
        /// <value>EntityDataKey for the 'from' end.</value>
        public EntityDataKey From { get; private set; }

        /// <summary>
        /// Gets EntityDataKey for the 'to' end.
        /// </summary>
        /// <value>EntityDataKey for the 'to' end.</value>
        public EntityDataKey To { get; private set; }

        /// <summary>
        /// Gets the role name of the 'to' end.
        /// </summary>
        /// <value>The role name of the 'to' end.</value>
        public string ToRoleName { get; private set; }

        /// <summary>
        ///  Gets the role name of the 'from' end.
        /// </summary>
        /// <value>The role name of the 'from' end.</value>
        public string FromRoleName { get; private set; }
    }
}
