//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Data.Services.Common;
    #endregion Namespaces

    /// <summary>
    /// Represents an enumerable of <see cref="EntityPropertyMappingAttribute"/> that new items can be added to.
    /// </summary>
    public sealed class ODataEntityPropertyMappingCollection : IEnumerable<EntityPropertyMappingAttribute>
    {
        /// <summary>List of the mappings represented by this enumerable.</summary>
        private readonly List<EntityPropertyMappingAttribute> mappings;

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.Metadata.ODataEntityPropertyMappingCollection" /> class.</summary>
        public ODataEntityPropertyMappingCollection()
        {
            this.mappings = new List<EntityPropertyMappingAttribute>();
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Data.OData.Metadata.ODataEntityPropertyMappingCollection" /> class.</summary>
        /// <param name="other">An enumerable of <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> used to initialize the instance. This argument must not be null.</param>
        public ODataEntityPropertyMappingCollection(IEnumerable<EntityPropertyMappingAttribute> other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.mappings = new List<EntityPropertyMappingAttribute>(other);
        }

        /// <summary>
        /// The count of mappings stored in this collection.
        /// </summary>
        internal int Count
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.mappings.Count;
            }
        }

        /// <summary>Adds the mapping to the list of all mappings represented by this class.</summary>
        /// <param name="mapping">The <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> to add to the enumerable represented by this class.</param>
        public void Add(EntityPropertyMappingAttribute mapping)
        {
            ExceptionUtils.CheckArgumentNotNull(mapping, "mapping");
            this.mappings.Add(mapping);
        }

        /// <summary>Returns an enumerator for the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</summary>
        /// <returns>An enumerator for the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</returns>
        public IEnumerator<EntityPropertyMappingAttribute> GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }

        /// <summary>Returns a non-generic enumerator for the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</summary>
        /// <returns>A non-generic enumerator for the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }
    }
}
