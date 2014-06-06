//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.Providers
#else
namespace Microsoft.OData.Service.Providers
#endif
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmEntityType"/> implementation that supports delay-loading of properties.
    /// </summary>
    internal class EdmEntityTypeWithDelayLoadedProperties : EdmEntityType
    {
        /// <summary>The lock object for the delayed property loading.</summary>
        private readonly object lockObject = new object();

        /// <summary>An action that is used to create the properties for this type.</summary>
        private Action<EdmEntityTypeWithDelayLoadedProperties> propertyLoadAction;

        /// <summary>
        /// Initializes a new instance of the EdmEntityTypeWithDelayLoadedProperties class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="name">Name of the entity.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="hasStream">Denotes if the type is a media entity.</param>
        /// <param name="propertyLoadAction">An action that is used to create the properties for this type.</param>
        internal EdmEntityTypeWithDelayLoadedProperties(
            string namespaceName, 
            string name,
            IEdmEntityType baseType, 
            bool isAbstract, 
            bool isOpen,
            bool hasStream,
            Action<EdmEntityTypeWithDelayLoadedProperties> propertyLoadAction)
            : base(namespaceName, name, baseType, isAbstract, isOpen, hasStream)
        {
            Debug.Assert(propertyLoadAction != null, "propertyLoadAction != null");
            this.propertyLoadAction = propertyLoadAction;
        }

        /// <summary>
        /// Gets or sets the structural properties of the entity type that make up the entity key.
        /// </summary>
        public override IEnumerable<IEdmStructuralProperty> DeclaredKey
        {
            get
            {
                this.EnsurePropertyLoaded();
                return base.DeclaredKey;
            }
        }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        public override IEnumerable<IEdmProperty> DeclaredProperties
        {
            get
            {
                this.EnsurePropertyLoaded();
                return base.DeclaredProperties;
            }
        }

        /// <summary>
        /// Ensures that the properties have been loaded and can be used.
        /// </summary>
        private void EnsurePropertyLoaded()
        {
            lock (this.lockObject)
            {
                if (this.propertyLoadAction != null)
                {
                    this.propertyLoadAction(this);
                    this.propertyLoadAction = null;
                }
            }
        }
    }
}
