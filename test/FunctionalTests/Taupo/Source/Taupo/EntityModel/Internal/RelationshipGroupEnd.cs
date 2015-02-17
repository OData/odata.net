//---------------------------------------------------------------------
// <copyright file="RelationshipGroupEnd.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// End of the relationship group.
    /// </summary>
    [DebuggerDisplay("{ToTraceString()}")]
    internal class RelationshipGroupEnd    
    {
        private List<KeyValuePair<AssociationSet, AssociationSetEnd>> associationSetEnds;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipGroupEnd"/> class.
        /// </summary>
        /// <param name="associationSet">The association set.</param>
        /// <param name="associationSetEnd">The association set end.</param>
        public RelationshipGroupEnd(AssociationSet associationSet, AssociationSetEnd associationSetEnd)
        {
            this.EntitySet = associationSetEnd.EntitySet;
            this.EntityType = associationSetEnd.AssociationEnd.EntityType;
            this.Multiplicity = associationSet.GetOtherEnd(associationSetEnd).AssociationEnd.Multiplicity;

            this.associationSetEnds = new List<KeyValuePair<AssociationSet, AssociationSetEnd>>();
            this.associationSetEnds.Add(new KeyValuePair<AssociationSet, AssociationSetEnd>(associationSet, associationSetEnd));
            this.Candidates = new List<RelationshipCandidate>();

            this.CapacitySelector = CapacityRangeSelector.GetDefaultCapacityRangeSelector(this.Multiplicity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipGroupEnd"/> class.
        /// </summary>
        /// <param name="associationSetAndEndPairs">The association set and end pairs.</param>
        public RelationshipGroupEnd(IEnumerable<KeyValuePair<AssociationSet, AssociationSetEnd>> associationSetAndEndPairs)
        {
            var entitySets = associationSetAndEndPairs.Select(p => p.Value.EntitySet).Distinct();
            ExceptionUtilities.Assert(entitySets.Count() == 1, "All AssociationSetEnds should have the same EntitySet");

            var entityTypes = associationSetAndEndPairs.Select(p => p.Value.AssociationEnd.EntityType).Distinct();
            ExceptionUtilities.Assert(entityTypes.Count() == 1, "All AssociationSetEnds should have the same EntityType");

            this.EntitySet = entitySets.Single();
            this.EntityType = entityTypes.Single();

            this.Multiplicity = EndMultiplicity.One;

            this.associationSetEnds = new List<KeyValuePair<AssociationSet, AssociationSetEnd>>();
            this.associationSetEnds.AddRange(associationSetAndEndPairs);
            this.Candidates = new List<RelationshipCandidate>();

            this.CapacitySelector = CapacityRangeSelector.GetDefaultCapacityRangeSelector(this.Multiplicity);
        }

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <value>The entity set.</value>
        public EntitySet EntitySet { get; private set; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Gets the list of pairs of AssociationSet and AssociationSetEnd this relationship group end represents.
        /// </summary>
        /// <value>The association set ends.</value>
        public IEnumerable<KeyValuePair<AssociationSet, AssociationSetEnd>> AssociationSetEnds
        {
            get { return this.associationSetEnds; }
        }

        /// <summary>
        /// Gets the candidates for this relationship group end.
        /// </summary>
        /// <value>The candidates.</value>
        public List<RelationshipCandidate> Candidates { get; private set; }

        /// <summary>
        /// Gets the multiplicity of this relationship group end.
        /// </summary>
        /// <value>The multiplicity.</value>
        public EndMultiplicity Multiplicity { get; private set; }

        /// <summary>
        /// Gets or sets the capacity range selector.
        /// </summary>
        /// <value>The capacity selector.</value>
        public Func<CapacityRange> CapacitySelector { get; set; }

        internal string ToTraceString()
        {
            return string.Join(",", this.AssociationSetEnds.Select(kv => kv.Key.Name + "." + kv.Value.AssociationEnd.RoleName).ToArray());
        }
    }
}
