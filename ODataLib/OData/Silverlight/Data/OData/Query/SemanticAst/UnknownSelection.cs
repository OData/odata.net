//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    /// <summary>
    /// Singleton sentinal instance of <see cref="Selection"/> that indicates that $select has not been processed yet. Should never be exposed to the user.
    /// </summary>
    internal sealed class UnknownSelection : Selection
    {
        /// <summary>
        /// Singleton instance of <see cref="UnknownSelection"/>.
        /// </summary>
        public static readonly UnknownSelection Instance = new UnknownSelection();

        /// <summary>
        /// Creates the singleton instance of this class.
        /// </summary>
        private UnknownSelection()
        {
        }
    }
}
