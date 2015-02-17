//---------------------------------------------------------------------
// <copyright file="RandomDataShuffler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="DataShuffler"/> which randomizes data.
    /// </summary>
    public class RandomDataShuffler : DataShuffler
    {
        /// <summary>
        /// Initializes a new instance of the RandomDataShuffler class.
        /// </summary>
        /// <param name="randomNumberGenerator">The random number generator.</param>
        public RandomDataShuffler(IRandomNumberGenerator randomNumberGenerator)
        {
            this.Random = randomNumberGenerator;
        }

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        protected IRandomNumberGenerator Random { get; private set; }

        /// <summary>
        /// Shuffles the specified list in place.
        /// </summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        public override void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int swapWith = i + this.Random.Next(list.Count - i);
                if (swapWith != i)
                {
                    T tmp = list[i];
                    list[i] = list[swapWith];
                    list[swapWith] = tmp;
                }
            }
        }
    }
}
