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

using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public abstract class EdmProperty : EdmNamedElement, IEdmProperty, IDependencyTrigger
    {
        private readonly IEdmStructuredType declaringType;
        private IEdmTypeReference type;
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();

        /// <summary>
        /// Initializes a new instance of the EdmProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        protected EdmProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : base(name)
        {
            this.declaringType = declaringType;
            this.type = type;

            EdmStructuredType declaringModelType = declaringType as EdmStructuredType;
            if (declaringModelType != null)
            {
                declaringModelType.AddProperty(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the EdmProperty class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        protected EdmProperty(IEdmStructuredType declaringType)
        {
            this.declaringType = declaringType;
            this.type = null;

            EdmStructuredType declaringModelType = declaringType as EdmStructuredType;
            if (declaringModelType != null)
            {
                declaringModelType.AddProperty(this);
            }
        }

        /// <summary>
        /// Gets or sets the name of this property.
        /// </summary>
        new public string Name
        {
            get { return this.elementName; }
            set { this.SetField(this.declaringType as IDependent, ref this.elementName, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets or sets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
            set { this.SetField(this.declaringType as IDependent, ref this.type, value); }
        }
        
        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        HashSetInternal<IDependent> IDependencyTrigger.Dependents
        {
            get { return this.dependents; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public abstract EdmPropertyKind PropertyKind
        {
            get;
        }
    }
}
