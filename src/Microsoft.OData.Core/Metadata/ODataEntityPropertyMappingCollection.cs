//---------------------------------------------------------------------
// <copyright file="ODataEntityPropertyMappingCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Service.Common;
    #endregion Namespaces

    /// <summary>
    /// Represents an enumerable of <see cref="EntityPropertyMappingAttribute"/> that new items can be added to.
    /// </summary>
    public sealed class ODataEntityPropertyMappingCollection : IEnumerable<EntityPropertyMappingAttribute>
    {
        /// <summary>List of the mappings represented by this enumerable.</summary>
        private readonly List<EntityPropertyMappingAttribute> mappings;

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.Metadata.ODataEntityPropertyMappingCollection" /> class.</summary>
        public ODataEntityPropertyMappingCollection()
        {
            this.mappings = new List<EntityPropertyMappingAttribute>();
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.OData.Core.Metadata.ODataEntityPropertyMappingCollection" /> class.</summary>
        /// <param name="other">An enumerable of <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> used to initialize the instance. This argument must not be null.</param>
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
        /// <param name="mapping">The <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> to add to the enumerable represented by this class.</param>
        public void Add(EntityPropertyMappingAttribute mapping)
        {
            ExceptionUtils.CheckArgumentNotNull(mapping, "mapping");
            this.mappings.Add(mapping);
        }

        /// <summary>Returns an enumerator for the <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</summary>
        /// <returns>An enumerator for the <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</returns>
        public IEnumerator<EntityPropertyMappingAttribute> GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }

        /// <summary>Returns a non-generic enumerator for the <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</summary>
        /// <returns>A non-generic enumerator for the <see cref="T:Microsoft.OData.Service.Common.EntityPropertyMappingAttribute" /> instances in this enumerable.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }
    }
}
