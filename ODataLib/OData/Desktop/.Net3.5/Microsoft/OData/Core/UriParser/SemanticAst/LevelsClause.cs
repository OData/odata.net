//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
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
        /// <param name="level">The level value for the LevelsClause.</param>
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
