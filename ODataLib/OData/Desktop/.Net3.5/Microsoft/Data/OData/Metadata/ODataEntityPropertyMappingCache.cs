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
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// Annotation stored on an entity type to hold entity property mapping information.
    /// </summary>
    internal sealed class ODataEntityPropertyMappingCache
    {
        /// <summary>
        /// A list of the EPM mappings this cache was constructed for. 
        /// Used to determine whether the cache is dirty or not.
        /// </summary>
        private readonly ODataEntityPropertyMappingCollection mappings;

        /// <summary>
        /// Inherited EntityPropertyMapping attributes.
        /// </summary>
        private readonly List<EntityPropertyMappingAttribute> mappingsForInheritedProperties;

        /// <summary>
        /// Own EntityPropertyMapping attributes.
        /// </summary>
        private readonly List<EntityPropertyMappingAttribute> mappingsForDeclaredProperties;

        /// <summary>
        /// EPM source tree for the type this annotation belongs to.
        /// </summary>
        private readonly EpmSourceTree epmSourceTree;

        /// <summary>
        /// EPM target tree for the type this annotation belongs to.
        /// </summary>
        private readonly EpmTargetTree epmTargetTree;

        /// <summary>
        /// EDM model.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>The total number of entity property mappings for the entity type that this cache is created for (on the type itself and all its base types).</summary>
        private readonly int totalMappingCount;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mappings">The EPM mappings to create the cache for.</param>
        /// <param name="model">The EDM model.</param>
        /// <param name="totalMappingCount">The total number of entity property mappings 
        /// for the entity type that this cache is created for (on the type itself and all its base types).</param>
        internal ODataEntityPropertyMappingCache(ODataEntityPropertyMappingCollection mappings, IEdmModel model, int totalMappingCount)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model.IsUserModel(), "model.IsUserModel()");

            this.mappings = mappings;
            this.model = model;
            this.totalMappingCount = totalMappingCount;

            // Note that we new up everything here eagerly because we will only create the EPM annotation for types
            // for which we know for sure that they have EPM and thus we will need all of these anyway.
            this.mappingsForInheritedProperties = new List<EntityPropertyMappingAttribute>();
            this.mappingsForDeclaredProperties = mappings == null
                ? new List<EntityPropertyMappingAttribute>()
                : new List<EntityPropertyMappingAttribute>(mappings);
            this.epmTargetTree = new EpmTargetTree();
            this.epmSourceTree = new EpmSourceTree(this.epmTargetTree);
        }

        /// <summary>
        /// Inherited EntityPropertyMapping attributes.
        /// </summary>
        internal List<EntityPropertyMappingAttribute> MappingsForInheritedProperties
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.mappingsForInheritedProperties;
            }
        }

        /// <summary>
        /// Own EntityPropertyMapping attributes.
        /// </summary>
        internal List<EntityPropertyMappingAttribute> MappingsForDeclaredProperties
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.mappingsForDeclaredProperties;
            }
        }

        /// <summary>
        /// EPM source tree for the type this annotation belongs to.
        /// </summary>
        internal EpmSourceTree EpmSourceTree
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.epmSourceTree;
            }
        }

        /// <summary>
        /// EPM target tree for the type this annotation belongs to.
        /// </summary>
        internal EpmTargetTree EpmTargetTree
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.epmTargetTree;
            }
        }

        /// <summary>
        /// All EntityPropertyMapping attributes.
        /// </summary>
        internal IEnumerable<EntityPropertyMappingAttribute> AllMappings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.MappingsForDeclaredProperties.Concat(this.MappingsForInheritedProperties);
            }
        }

        /// <summary>
        /// The total number of entity property mappings for the entity type that this cache is created for (on the type itself and all its base types).
        /// </summary>
        internal int TotalMappingCount
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.totalMappingCount;
            }
        }

        /// <summary>
        /// Initializes the EPM annotation with EPM information from the specified type.
        /// </summary>
        /// <param name="definingEntityType">Entity type to use the EPM infromation from.</param>
        /// <param name="affectedEntityType">Entity type for this the EPM information is being built.</param>
        internal void BuildEpmForType(IEdmEntityType definingEntityType, IEdmEntityType affectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(definingEntityType != null, "definingEntityType != null");
            Debug.Assert(affectedEntityType != null, "affectedEntityType != null");

            if (definingEntityType.BaseType != null)
            {
                this.BuildEpmForType(definingEntityType.BaseEntityType(), affectedEntityType);
            }

            ODataEntityPropertyMappingCollection mappingsForType = this.model.GetEntityPropertyMappings(definingEntityType);
            if (mappingsForType == null)
            {
                return;
            }

            foreach (EntityPropertyMappingAttribute mapping in mappingsForType)
            {
                this.epmSourceTree.Add(new EntityPropertyMappingInfo(mapping, definingEntityType, affectedEntityType));

                if (definingEntityType == affectedEntityType)
                {
                    if (!PropertyExistsOnType(affectedEntityType, mapping))
                    {
                        this.MappingsForInheritedProperties.Add(mapping);
                        this.MappingsForDeclaredProperties.Remove(mapping);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the current cache is dirty with respect to the <paramref name="propertyMappings"/>.
        /// </summary>
        /// <param name="propertyMappings">The EPM mappings to check this cache against.</param>
        /// <returns>true if the <paramref name="propertyMappings"/> are not the same as the ones the cache has been created for (or have changed).</returns>
        internal bool IsDirty(ODataEntityPropertyMappingCollection propertyMappings)
        {
            DebugUtils.CheckNoExternalCallers();
         
            // NOTE: we only allow adding more mappings to an existing collection; so if the 
            // references of the collections are the same and the counts are the same there has been no change.
            if (this.mappings == null && propertyMappings == null)
            {
                return false;
            }

            if (!object.ReferenceEquals(this.mappings, propertyMappings))
            {
                return true;
            }

            return this.mappings.Count != propertyMappings.Count;
        }

        /// <summary>
        /// Does given property in the attribute exist in the specified type.
        /// </summary>
        /// <param name="structuredType">The type to inspect.</param>
        /// <param name="epmAttribute">Attribute which has PropertyName.</param>
        /// <returns>true if property exists in the specified type, false otherwise.</returns>
        private static bool PropertyExistsOnType(IEdmStructuredType structuredType, EntityPropertyMappingAttribute epmAttribute)
        {
            Debug.Assert(structuredType != null, "structuredType != null");
            Debug.Assert(epmAttribute != null, "epmAttribute != null");

            int indexOfSeparator = epmAttribute.SourcePath.IndexOf('/');
            String propertyToLookFor = indexOfSeparator == -1 ? epmAttribute.SourcePath : epmAttribute.SourcePath.Substring(0, indexOfSeparator);
            return structuredType.DeclaredProperties.Any(p => p.Name == propertyToLookFor);
        }
    }
}
