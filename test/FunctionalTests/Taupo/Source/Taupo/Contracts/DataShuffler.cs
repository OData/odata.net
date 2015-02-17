//---------------------------------------------------------------------
// <copyright file="DataShuffler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Reorganizes contents of a list by shuffling them.
    /// </summary>
    public abstract class DataShuffler
    {
        /// <summary>
        /// Gets the null shuffler (which makes no changes ot the list).
        /// </summary>
        /// <value>The null shuffler.</value>
        public static DataShuffler Null
        {
            get { return NullShuffler.Instance; }
        }

        /// <summary>
        /// Shuffles the specified list in place.
        /// </summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="list">The list to be shuffled.</param>
        public abstract void Shuffle<T>(IList<T> list);

        /// <summary>
        /// Null implementation of <see cref="DataShuffler"/> which does not shuffle anything.
        /// </summary>
        private class NullShuffler : DataShuffler
        {
            internal static readonly NullShuffler Instance = new NullShuffler();

            /// <summary>
            /// Shuffles the specified list in place.
            /// </summary>
            /// <typeparam name="T">Type of the list element.</typeparam>
            /// <param name="list">The list to be shuffled.</param>
            public override void Shuffle<T>(IList<T> list)
            {
                // do nothing
            }
        }
    }
}
