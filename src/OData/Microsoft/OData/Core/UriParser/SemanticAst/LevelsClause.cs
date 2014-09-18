//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
