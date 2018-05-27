//---------------------------------------------------------------------
// <copyright file="LevelsClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// The result of parsing $levels option
    /// </summary>
    public sealed class LevelsClause
    {
        /// <summary>
        /// Whether targeting at level max.
        /// </summary>
        private bool isMaxLevel;

        /// <summary>
        /// The level value.
        /// </summary>
        private long level;

        /// <summary>
        /// Constructs a <see cref="LevelsClause"/> from given parameters.
        /// </summary>
        /// <param name="isMaxLevel">Flag indicating max level is specified.</param>
        /// <param name="level">
        /// The level value for the LevelsClause.
        /// This value is only used when <paramref name="isMaxLevel"/> is set to false.
        /// </param>
        public LevelsClause(bool isMaxLevel, long level)
        {
            this.isMaxLevel = isMaxLevel;
            this.level = level;
        }

        /// <summary>
        /// Get a flag indicating whether max level is specified.
        /// </summary>
        public bool IsMaxLevel
        {
            get { return this.isMaxLevel; }
        }

        /// <summary>
        /// The level value for current expand option.
        /// </summary>
        /// <remarks>This value is trivial when IsMaxLevel is True.</remarks>
        public long Level
        {
            get { return level; }
        }
    }
}
