//---------------------------------------------------------------------
// <copyright file="RelationshipCandidate.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Represents candidate for a relationship.
    /// </summary>
    internal class RelationshipCandidate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipCandidate"/> class.
        /// </summary>
        /// <param name="parent">The parent relationship group end this candidate can be used for.</param>
        /// <param name="entityDataKey">The entity data key.</param>
        /// <param name="capacityRange">The capacity range.</param>
        public RelationshipCandidate(
            RelationshipGroupEnd parent,
            EntityDataKey entityDataKey,
            CapacityRange capacityRange)
        {
            this.EntityDataKey = entityDataKey;
            this.CapacityRange = capacityRange;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets parent relationship group end this candidate can be used for.
        /// </summary>
        /// <value>The parent relationship group end this candidate can be used for.</value>
        public RelationshipGroupEnd Parent { get; private set; }

        /// <summary>
        /// Gets the entity data key.
        /// </summary>
        /// <value>The entity data key.</value>
        public EntityDataKey EntityDataKey { get; private set; }

        /// <summary>
        /// Gets the related entities count.
        /// </summary>
        /// <value>The related count.</value>
        public int RelatedCount { get; private set; }

        /// <summary>
        /// Gets the capacity range.
        /// </summary>
        /// <value>The capacity range.</value>
        public CapacityRange CapacityRange { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the capacity reached.
        /// </summary>
        /// <value>
        ///     <c>true</c> if capacity reached; otherwise, <c>false</c>.
        /// </value>
        public bool IsCapacityReached
        {
            get { return this.CapacityRange.Max >= 0 && this.RelatedCount >= this.CapacityRange.Max; }
        }

        /// <summary>
        /// Gets a value indicating whether minimum requirement met.
        /// </summary>
        /// <value>
        ///     <c>true</c> if minimum requirement met; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinimumRequirementMet
        {
            get { return this.CapacityRange.Min == -1 || this.RelatedCount >= this.CapacityRange.Min; }
        }

        /// <summary>
        /// Gets a value indicating whether capacity is unlimited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if capacity is unlimited; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnlimitedCapacity
        {
            get { return this.CapacityRange.Max == -1; }
        }

        /// <summary>
        /// Increments the related count.
        /// </summary>
        public void IncrementRelatedCount()
        {
            ExceptionUtilities.Assert(!this.IsCapacityReached, "Capacity for the entity is reached. Cannot increment the related entities count.");
            
            this.RelatedCount = this.RelatedCount + 1;
        }
    }
}
