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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion

    /// <summary>Class describing a single node on the tree of projections
    /// and expansions. This is the base class used for any projected property.</summary>
    [DebuggerDisplay("ProjectionNode {PropertyName}")]
    internal class ProjectionNode
    {
        #region Private fields
        /// <summary>The name of the property to project.</summary>
        /// <remarks>If this node represents the root of the projection tree, this name is an empty string.</remarks>
        private readonly string propertyName;

        /// <summary>The <see cref="ResourceProperty"/> for the property to be projected.</summary>
        /// <remarks>If this node represents an open property or it's the root of the projection tree,
        /// this field is null.</remarks>
        private readonly ResourceProperty property;

        /// <summary>Target resource type on which the projection needs to happen.</summary>
        private ResourceType targetResourceType;

        #endregion

        #region Constructors
        /// <summary>Creates new instance of <see cref="ProjectionNode"/> which represents a simple projected property.</summary>
        /// <param name="propertyName">The name of the property to project.</param>
        /// <param name="property">The <see cref="ResourceProperty"/> for the property to project. If an open property
        /// is to be projected, specify null.</param>
        /// <param name="targetResourceType">The resource type for which the <see cref="property"/>needs to be projected.</param>
        internal ProjectionNode(string propertyName, ResourceProperty property, ResourceType targetResourceType)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(property == null || property.Name == propertyName, "If the property is specified its name must match.");
#if DEBUG
            // For rootProjectionNode, there is no targetResourceType. Hence checking for propertyName to be String.Empty to excluse RootProjectionNode from the assert.
            if (!String.IsNullOrEmpty(propertyName))
            {
                Debug.Assert(targetResourceType != null, "targetResourceType != null");
                if (property != null)
                {
                    Debug.Assert(object.ReferenceEquals(targetResourceType.TryResolvePropertyName(propertyName), property), "object.ReferenceEquals(targetResourceType.TryResolvePropertyName(propertyName), property)");
                }
                else
                {
                    Debug.Assert(targetResourceType.IsOpenType, "targetResourceType.IsOpenType");
                }
            }
#endif

            this.propertyName = propertyName;
            this.property = property;
            this.targetResourceType = targetResourceType;
        }
        #endregion

        #region Public properties
        /// <summary>The name of the property to project.</summary>
        /// <remarks>If this node represents the root of the projection tree, this name is an empty string.</remarks>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>The <see cref="ResourceProperty"/> for the property to be projected.</summary>
        /// <remarks>If this node represents an open property or it's the root of the projection tree,
        /// this property is null.</remarks>
        public ResourceProperty Property
        {
            get
            {
                return this.property;
            }
        }

        /// <summary>The resource type for which <see cref="Property"/>needs to be projected.</summary>
        public ResourceType TargetResourceType
        {
            get
            {
                return this.targetResourceType;
            }

            set
            {
                Debug.Assert(value != null, "resource type should never be null");
                Debug.Assert(value.IsAssignableFrom(this.targetResourceType), "value.IsAssignableFrom(this.targetResourceType)");
                this.targetResourceType = value;
            }
        }
        #endregion
    }
}
