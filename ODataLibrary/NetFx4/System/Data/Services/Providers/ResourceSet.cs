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

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Class to keep information about a resource set.
    /// </summary>
    /// <remarks>
    /// Custom providers can choose to use it as is or derive from it
    /// in order to flow provider-specific data.
    /// </remarks>
    [DebuggerDisplay("{Name}: {ResourceType}")]
#if INTERNAL_DROP
    internal class ResourceSet : ODataAnnotatable
#else
    public class ResourceSet : ODataAnnotatable
#endif
    {
        #region Fields
        /// <summary>
        /// Reference to resource type that this resource set is a collection of.
        /// </summary>
        private readonly ResourceType elementType;

        /// <summary>
        /// Name of the resource set.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Is true, if the resource set is fully initialized and validated. No more changes can be made once its set to readonly.
        /// </summary>
        private bool isReadOnly;

        /// <summary>Is true, if key properties should be ordered as per declared order when used for constructing OrderBy queries.
        /// Otherwise the default alphabetical order is used.</summary>
        private bool useMetadataKeyOrder;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Constructs a new ResourceSet instance using the specified name and ResourceType instance.
        /// </summary>
        /// <param name="name">name of the resource set.</param>
        /// <param name="elementType">Reference to clr type that this resource set is a collection of.</param>
        public ResourceSet(string name, ResourceType elementType)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");

            if (elementType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ArgumentException(Strings.ResourceSet_ResourceSetMustBeAssociatedWithEntityType);
            }

            this.name = name;
            this.elementType = elementType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Name of the resource set.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Reference to resource type that this resource set is a collection of.
        /// </summary>
        public ResourceType ResourceType
        {
            get { return this.elementType; }
        }

        /// <summary>
        /// PlaceHolder to hold custom state information about resource set.
        /// </summary>
        public object CustomState
        {
            get
            {
                return this.GetCustomState();
            }

            set
            {
                this.SetCustomState(value);
            }
        }

        /// <summary>
        /// Returns true, if this container has been set to read only. Otherwise returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        /// <summary>
        /// Is true, if key properties should be ordered as per declared order when used for constructing OrderBy queries.
        /// Otherwise the default alphabetical order is used.
        /// </summary>
        public bool UseMetadataKeyOrder
        {
            get
            {
                return this.useMetadataKeyOrder;
            }

            set
            {
                this.ThrowIfSealed();
                this.useMetadataKeyOrder = value;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Sets the resource set to readonly mode. resource sets cannot be updated once this property is set.
        /// </summary>
        public void SetReadOnly()
        {
            // If its already set to readonly, then its a no-op
            if (this.isReadOnly)
            {
                return;
            }

            this.elementType.SetReadOnly();
            this.isReadOnly = true;
        }

        /// <summary>
        /// Checks if the resource set is sealed. If not, it throws an InvalidOperationException.
        /// </summary>
        private void ThrowIfSealed()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(Strings.ResourceSet_Sealed(this.Name));
            }
        }
        #endregion Methods
    }
}
