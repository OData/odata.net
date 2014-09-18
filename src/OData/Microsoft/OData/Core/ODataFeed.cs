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

namespace Microsoft.OData.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes a collection of entities.
    /// </summary>
    public sealed class ODataFeed : ODataFeedBase
    {
        /// <summary>The feed actions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataAction> actions = new List<ODataAction>();

        /// <summary>The feed functions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataFunction> functions = new List<ODataFunction>();

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataFeed"/>.
        /// </summary>
        private ODataFeedAndEntrySerializationInfo serializationInfo;

        /// <summary>Gets the feed actions.</summary>
        /// <returns>The feed actions.</returns>
        public IEnumerable<ODataAction> Actions
        {
            get { return this.actions; }
        }

        /// <summary>Gets the feed functions.</summary>
        /// <returns>The feed functions.</returns>
        public IEnumerable<ODataFunction> Functions
        {
            get { return this.functions; }
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataFeed"/>.
        /// </summary>
        internal ODataFeedAndEntrySerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataFeedAndEntrySerializationInfo.Validate(value);
            }
        }

        /// <summary>
        /// Add action to feed.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(ODataAction action)
        {
            ExceptionUtils.CheckArgumentNotNull(action, "action");
            if (!this.actions.Contains(action))
            {
                this.actions.Add(action);
            }
        }

        /// <summary>
        /// Add function to feed.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(ODataFunction function)
        {
            ExceptionUtils.CheckArgumentNotNull(function, "function");
            if (!this.functions.Contains(function))
            {
                this.functions.Add(function);
            }
        }
    }
}
