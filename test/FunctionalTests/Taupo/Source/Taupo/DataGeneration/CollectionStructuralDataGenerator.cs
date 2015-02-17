//---------------------------------------------------------------------
// <copyright file="CollectionStructuralDataGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    
    /// <summary>
    /// Structural data generator for a collection.
    /// </summary>
    public class CollectionStructuralDataGenerator : DataGenerator<object>, IMemberDataGenerators
    {
        private IDataGenerator itemDataGenerator;
        private IRandomNumberGenerator random;
        private int minCount;
        private int maxCount;

        /// <summary>
        /// Initializes a new instance of the CollectionStructuralDataGenerator class.
        /// </summary>
        /// <param name="itemDataGenerator">The data generator for an item of the collection.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="minCount">Collection minimum count.</param>
        /// <param name="maxCount">Colleciton maximum count.</param>
        public CollectionStructuralDataGenerator(
            IDataGenerator itemDataGenerator,
            IRandomNumberGenerator random,
            int minCount,
            int maxCount)
        {
            ExceptionUtilities.CheckArgumentNotNull(itemDataGenerator, "itemDataGenerator");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            if (minCount < 0)
            {
                throw new TaupoArgumentException("Minimum count cannot be less than 0.");
            }
            
            ExceptionUtilities.CheckValidRange(minCount, "minCount", maxCount, "maxCount");

            this.itemDataGenerator = itemDataGenerator;
            this.minCount = minCount;
            this.maxCount = maxCount;
            this.random = random;
        }

        /// <summary>
        /// Generates collection data.
        /// </summary>
        /// <returns>Generated collection data in structural form (a list of named values).</returns>
        public override object GenerateData()
        {
            int count = this.minCount + this.random.Next(this.maxCount - this.minCount + 1);

            if (count == 0)
            {
                return EmptyData.Value;
            }

            List<NamedValue> result = new List<NamedValue>();
            for (int i = 0; i < count; i++)
            {
                var itemData = this.itemDataGenerator.GenerateData();
                result.AddMemberData(i.ToString(CultureInfo.InvariantCulture), itemData);
            }

            return result;
        }

        /// <summary>
        /// Gets a data generator for the specified member.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        /// <returns>A data generator for the specified member.</returns>
        public IDataGenerator GetMemberDataGenerator(string memberName)
        {
            string nextMemberName;
            string currenMemberName = MemberValueGenerators.GetCurrentMemberName(memberName, out nextMemberName);
            int index;
            ExceptionUtilities.Assert(int.TryParse(currenMemberName, out index), "Invalid member name for collection data generator. It should start with index. Member name: '{0}'.", memberName);

            IDataGenerator memberDataGenerator = this.itemDataGenerator;
            if (nextMemberName != null)
            {
                var memberDataGenerators = memberDataGenerator as IMemberDataGenerators;
                ExceptionUtilities.CheckObjectNotNull(memberDataGenerators, "Cannot find data generator for member '{0}'.", memberName);

                memberDataGenerator = memberDataGenerators.GetMemberDataGenerator(nextMemberName);
            }

            return memberDataGenerator;
        }
    }
}
