//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
