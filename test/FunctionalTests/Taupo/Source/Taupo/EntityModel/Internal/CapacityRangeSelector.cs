//---------------------------------------------------------------------
// <copyright file="CapacityRangeSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Capacity range selector used by RelationshipSelector.
    /// </summary>
    internal class CapacityRangeSelector
    {
        private static List<CapacityRange> interestingCapacitiesForOptionalReference = new List<CapacityRange>
            { 
              CapacityRange.AtMostOne, 
              CapacityRange.ExactlyOne,
              CapacityRange.Zero
            };

        private static List<CapacityRange> interestingCapacitiesForCollection = new List<CapacityRange>
            { 
              CapacityRange.Any, 
              CapacityRange.ExactlyOne,
              CapacityRange.Zero,
              CapacityRange.Within(2, 10)
            };
        
        private int count;

        private IList<CapacityRange> interestingCapacities;

        private CapacityRangeSelector(IList<CapacityRange> interestingCapacityRanges)
        {
            this.interestingCapacities = interestingCapacityRanges;
        }

        /// <summary>
        /// Gets the default capacity range selector for the given multiplicity.
        /// </summary>
        /// <param name="multiplicity">The multiplicity.</param>
        /// <returns>Capacity range selector for the given multiplicity.</returns>
        public static Func<CapacityRange> GetDefaultCapacityRangeSelector(EndMultiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case EndMultiplicity.One:
                    return () => CapacityRange.ExactlyOne;

                case EndMultiplicity.ZeroOne:
                    {
                        CapacityRangeSelector selector = new CapacityRangeSelector(interestingCapacitiesForOptionalReference);
                        return () => selector.GetNextCapacity();
                    }

                case EndMultiplicity.Many:
                    {
                        CapacityRangeSelector selector = new CapacityRangeSelector(interestingCapacitiesForCollection);
                        return () => selector.GetNextCapacity();
                    }

                default:
                    throw new TaupoInvalidOperationException("Unhandled multiplicity : " + multiplicity);
            }
        }

        private CapacityRange GetNextCapacity()
        {
            this.count++;

            if (this.count <= this.interestingCapacities.Count)
            {
                return this.interestingCapacities[this.count - 1];
            }

            return this.interestingCapacities[0];
        }
    }
}
