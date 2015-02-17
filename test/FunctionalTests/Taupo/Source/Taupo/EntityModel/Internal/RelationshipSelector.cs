//---------------------------------------------------------------------
// <copyright file="RelationshipSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Relstionship selector which is used by data population driver.
    /// </summary>
    [DebuggerDisplay("{ToTraceString()}")]
    internal class RelationshipSelector
    {
        private const int TreshholdForCandidatesOnTarget = 5;
        
        private List<RelationshipGroupEnd> ends;

        private List<RelationshipGroupEnd> endsWhichFormTheKey;

        private List<RelationshipCandidate[]> usedVectors;

        private IRandomNumberGenerator random;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipSelector"/> class.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        public RelationshipSelector(IRandomNumberGenerator random)
        {
            this.random = random;
            this.usedVectors = new List<RelationshipCandidate[]>();
            this.ends = new List<RelationshipGroupEnd>();
            this.endsWhichFormTheKey = new List<RelationshipGroupEnd>();
        }

        /// <summary>
        /// Adds the specified relationship group end.
        /// </summary>
        /// <param name="end">The relationship group end.</param>
        /// <param name="isKey">Indicates if this end is part of the key.</param>
        public void Add(RelationshipGroupEnd end, bool isKey)
        {
            this.ends.Add(end);
            if (isKey)
            {
                this.endsWhichFormTheKey.Add(end);
            }
        }

        /// <summary>
        /// Sets the capacity selector if specified association set and role belong to this relationship selector.
        /// </summary>
        /// <param name="associationSetName">The association set name.</param>
        /// <param name="roleName">The role name.</param>
        /// <param name="selector">The capacitye range selector.</param>
        /// <returns>True if the capacity range selector was set, false otherwise.</returns>
        public bool SetCapacitySelectorIfApplicable(string associationSetName, string roleName, Func<CapacityRange> selector)
        {
            RelationshipGroupEnd end = this.ends.Where(e => e.AssociationSetEnds.Any(kv => kv.Key.Name == associationSetName && kv.Value.AssociationEnd.RoleName == roleName)).SingleOrDefault();

            if (end != null)
            {
                end.CapacitySelector = selector;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Considers the candidate for this relationship.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="key">The entity data key of the candidate.</param>
        public void ConsiderCandidate(EntitySet entitySet, EntityType entityType, EntityDataKey key)
        {
            foreach (RelationshipGroupEnd end in this.ends.Where(e => e.EntitySet == entitySet && entityType.IsKindOf(e.EntityType)))
            {
                CapacityRange capacityRange = end.CapacitySelector();

                if (capacityRange != CapacityRange.Zero)
                {
                    this.RemoveCandidatesOnTargetBasedOnTreshhold(end);
                    
                    RelationshipCandidate candidate = new RelationshipCandidate(end, key, capacityRange);
                    end.Candidates.Add(candidate);
                }
            }
        }

        /// <summary>
        /// Resets the selector: removes all candidates for the relationship.
        /// </summary>
        public void Reset()
        {
            this.usedVectors.Clear();

            foreach (RelationshipGroupEnd end in this.ends)
            {
                end.Candidates.Clear();
            }
        }

        /// <summary>
        /// Gets the relationships to create.
        /// </summary>
        /// <param name="relationshipsToCreate">The relationships to create.</param>
        /// <param name="associationSetEndsToCreateEntitiesFor">The association set ends to create entities for.</param>
        public void GetRelationshipsToCreate(
            out List<RelationshipDescription> relationshipsToCreate, 
            out List<AssociationSetEnd> associationSetEndsToCreateEntitiesFor)
        {
            relationshipsToCreate = null;
            associationSetEndsToCreateEntitiesFor = null;

            List<RelationshipCandidate[]> candidates = this.GetAllPossibleVectors();

            List<RelationshipCandidate[]> selectedVectors = this.ExtractVectorsThatAreNotOnTarget(candidates);
            
            if (this.random.Next(100) % 3 == 0 && candidates.Count > 0)
            {
                RelationshipCandidate[] vector = this.random.ChooseFrom(candidates);
                selectedVectors.Add(vector);
                this.RegisterAsUsedVectorIfNeeded(vector);
                foreach (RelationshipCandidate candiate in vector)
                {
                    this.IncrementRelatedCount(candiate);
                }
            }

            if (selectedVectors.Count > 0)
            {
                relationshipsToCreate = new List<RelationshipDescription>();
                foreach (RelationshipCandidate[] vector in selectedVectors)
                {
                    relationshipsToCreate.AddRange(this.GetRelationshipDescriptions(vector));
                }
            }
            else
            {
                associationSetEndsToCreateEntitiesFor = this.GetAssociationSetEndsToCreateEntitiesFor();
            }
        }

        internal string ToTraceString()
        {
            List<string> associationSetNames = new List<string>();
            foreach (var end in this.ends)
            {
                foreach (var kv in end.AssociationSetEnds)
                {
                    if (!associationSetNames.Contains(kv.Key.Name))
                    {
                        associationSetNames.Add(kv.Key.Name);
                    }
                }
            }

            return string.Join(",", associationSetNames.ToArray());
        }

        private List<RelationshipCandidate[]> GetAllPossibleVectors()
        {
            List<RelationshipCandidate[]> candidates = new List<RelationshipCandidate[]>();

            int totalCombinationsCount = 1;
            foreach (var end in this.ends)
            {
                totalCombinationsCount *= end.Candidates.Count;
            }

            for (int i = 0; i < totalCombinationsCount; i++)
            {
                RelationshipCandidate[] relationshipGroup = new RelationshipCandidate[this.ends.Count];
                int factor = 1;
                for (int j = 0; j < this.ends.Count; j++)
                {
                    int index = (i / factor) % this.ends[j].Candidates.Count;
                    factor *= this.ends[j].Candidates.Count;
                    relationshipGroup[j] = this.ends[j].Candidates[index];
                }

                if (!this.usedVectors.Any(c => this.IsSameKey(c, relationshipGroup)))
                {
                    candidates.Add(relationshipGroup);
                }
            }

            return candidates;
        }

        private List<RelationshipCandidate[]> ExtractVectorsThatAreNotOnTarget(List<RelationshipCandidate[]> candidates)
        {
            List<RelationshipCandidate[]> result = new List<RelationshipCandidate[]>();

            int maxCountNotOnTarget;
            List<RelationshipCandidate[]> farthestFromTarget = this.FindFarthestFromTarget(candidates, out maxCountNotOnTarget);
            while (maxCountNotOnTarget > 0)
            {
                RelationshipCandidate[] vector = this.random.ChooseFrom(farthestFromTarget);
                this.RegisterAsUsedVectorIfNeeded(vector);
                candidates.Remove(vector);

                result.Add(vector);

                foreach (RelationshipCandidate candiate in vector)
                {
                    this.IncrementRelatedCount(candiate);
                    if (candiate.IsCapacityReached)
                    {
                        var toRemove = candidates.Where(v => v.Contains(candiate)).ToList();
                        foreach (RelationshipCandidate[] v in toRemove)
                        {
                            candidates.Remove(v);
                        }
                    }
                }

                farthestFromTarget = this.FindFarthestFromTarget(candidates, out maxCountNotOnTarget);
            }

            return result;
        }

        private List<RelationshipCandidate[]> FindFarthestFromTarget(List<RelationshipCandidate[]> vectors, out int maxCountNotOnTarget)
        {
            List<RelationshipCandidate[]> result = new List<RelationshipCandidate[]>();

            maxCountNotOnTarget = 0;

            foreach (RelationshipCandidate[] vector in vectors)
            {
                int currentCountNotOnTarget = vector.Where(c => !c.IsMinimumRequirementMet).Count();
                if (currentCountNotOnTarget == maxCountNotOnTarget)
                {
                    result.Add(vector);
                }
                else if (currentCountNotOnTarget > maxCountNotOnTarget)
                {
                    maxCountNotOnTarget = currentCountNotOnTarget;
                    result.Clear();
                    result.Add(vector);
                }
            }

            return result;
        }

        private IList<RelationshipDescription> GetRelationshipDescriptions(RelationshipCandidate[] group)
        {
            List<RelationshipDescription> relationships = new List<RelationshipDescription>();

            List<KeyValuePair<AssociationSet, AssociationSetEnd>> processedSetEndPairs = new List<KeyValuePair<AssociationSet, AssociationSetEnd>>();
            
            foreach (RelationshipCandidate toSide in group)
            {
                foreach (var setEndPair in toSide.Parent.AssociationSetEnds.Where(p => !processedSetEndPairs.Contains(p)))
                {
                    // Find the "from" side of the relationship
                    foreach (var from in group.Where(c => c != toSide).Select(c => new { FromSide = c, OtherSetEndPair = c.Parent.AssociationSetEnds.Where(p => p.Key == setEndPair.Key).SingleOrDefault() })
                        .Where(a => a.OtherSetEndPair.Key != null))
                    {
                        relationships.Add(new RelationshipDescription(
                            setEndPair.Key,
                            from.FromSide.EntityDataKey,
                            setEndPair.Value.AssociationEnd.RoleName,
                            toSide.EntityDataKey));

                        processedSetEndPairs.Add(setEndPair);
                        processedSetEndPairs.Add(from.OtherSetEndPair);
                    }
                }
            }

            return relationships;
        }

        private List<AssociationSetEnd> GetAssociationSetEndsToCreateEntitiesFor()
        {
            List<AssociationSetEnd> associationSetEndsToCreateEntitiesFor = new List<AssociationSetEnd>();

            foreach (RelationshipGroupEnd end in this.ends.Where(e => e.Candidates.Any(c => !c.IsMinimumRequirementMet)))
            {
                int currentMinimum = int.MaxValue;
                AssociationSetEnd associationSetEndToCreateEntitiesFor = null;
                bool hasGroupEndWithZeroCandidates = false;
                foreach (var setEndPair in end.AssociationSetEnds)
                {
                    foreach (RelationshipGroupEnd otherEnd in this.ends.Where(e => e != end && e.AssociationSetEnds.Any(kv => kv.Key == setEndPair.Key)))
                    {
                        AssociationSetEnd otherAssociationSetEnd = setEndPair.Key.GetOtherEnd(setEndPair.Value);
                        if (otherEnd.Candidates.Count == 0)
                        {
                            associationSetEndsToCreateEntitiesFor.Add(otherAssociationSetEnd);
                            hasGroupEndWithZeroCandidates = true;
                            associationSetEndToCreateEntitiesFor = null;
                        }
                        else if (!hasGroupEndWithZeroCandidates)
                        {
                            if (otherEnd.Candidates.Count < currentMinimum)
                            {
                                currentMinimum = otherEnd.Candidates.Count;
                                associationSetEndToCreateEntitiesFor = otherAssociationSetEnd;
                            }
                        }
                    }
                }

                if (associationSetEndToCreateEntitiesFor != null)
                {
                    associationSetEndsToCreateEntitiesFor.Add(associationSetEndToCreateEntitiesFor);
                }
            }

            return associationSetEndsToCreateEntitiesFor;
        }

        private void IncrementRelatedCount(RelationshipCandidate candidate)
        {
            bool isMinimumRequirementAlreadyMet = candidate.IsMinimumRequirementMet;

            candidate.IncrementRelatedCount();

            if (candidate.IsCapacityReached)
            {
                this.RemoveCandidate(candidate);
            }
            else if (!isMinimumRequirementAlreadyMet && candidate.IsMinimumRequirementMet)
            {
                this.RemoveCandidatesOnTargetBasedOnTreshhold(candidate.Parent);
            }
        }

        private void RemoveCandidatesOnTargetBasedOnTreshhold(RelationshipGroupEnd end)
        {
            var candidatesOnTargetWithUnlimitedCapacity = end.Candidates.Where(
                c => c.IsMinimumRequirementMet && c.IsUnlimitedCapacity).ToList();

            for (int i = 0; i < candidatesOnTargetWithUnlimitedCapacity.Count - TreshholdForCandidatesOnTarget; i++)
            {
                this.RemoveCandidate(candidatesOnTargetWithUnlimitedCapacity[i]);
            }
        }

        private void RemoveCandidate(RelationshipCandidate toRemove)
        {
            toRemove.Parent.Candidates.Remove(toRemove);

            if (this.endsWhichFormTheKey.Contains(toRemove.Parent))
            {
                var rowsToRemove = this.usedVectors.Where(v => v.Contains(toRemove)).ToList();
                foreach (var row in rowsToRemove)
                {
                    this.usedVectors.Remove(row);
                }
            }
        }

        private void RegisterAsUsedVectorIfNeeded(RelationshipCandidate[] vector)
        {
            if (this.endsWhichFormTheKey.Count > 0)
            {
                this.usedVectors.Add(vector);
            }
        }

        private bool IsSameKey(RelationshipCandidate[] vector1, RelationshipCandidate[] vector2)
        {
            foreach (RelationshipGroupEnd end in this.endsWhichFormTheKey)
            {
                EntityDataKey key1 = vector1.Where(r => r.Parent == end).Select(r => r.EntityDataKey).Single();
                EntityDataKey key2 = vector2.Where(r => r.Parent == end).Select(r => r.EntityDataKey).Single();

                if (key1 != key2)
                {
                    return false;
                }
            }

            return this.endsWhichFormTheKey.Count > 0;
        }
    }
}
