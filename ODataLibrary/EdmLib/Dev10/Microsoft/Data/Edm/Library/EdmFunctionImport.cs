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

using System;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function import.
    /// </summary>
    public class EdmFunctionImport : EdmFunctionBase, IEdmFunctionImport
    {
        private bool sideEffecting = true;
        private IEdmEntitySet entitySet;

        /// <summary>
        /// Initializes a new instance of the EdmFunctionImport class.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        public EdmFunctionImport(string name, IEdmTypeReference returnType)
            : base(name, returnType)
        {
            this.Bindable = false;
            this.Composable = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this function import has side-effects.
        /// <see cref="SideEffecting"/> cannot be set to true if <see cref="Composable"/> is set to true.
        /// </summary>
        public bool SideEffecting
        {
            get { return this.sideEffecting; }
            set { this.sideEffecting = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this functon import can be composed inside expressions.
        /// <see cref="Composable"/> cannot be set to true if <see cref="SideEffecting"/> is set to true.
        /// </summary>
        public bool Composable
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this function import can be used as an extension method for the type of the first parameter of this function import.
        /// </summary>
        public bool Bindable
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the entity set path of the function import
        /// </summary>
        public string EntitySetPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity set of this function.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
            set { this.SetField(ref this.entitySet, value); }
        }

        /// <summary>
        /// Gets the kind of this function, which is always FunctionImport.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }
    }
}
