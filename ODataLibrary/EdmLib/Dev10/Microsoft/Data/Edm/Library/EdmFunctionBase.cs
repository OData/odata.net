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

using System.Collections.Generic;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function or function import.
    /// </summary>
    public abstract class EdmFunctionBase : EdmNamedElement, IEdmFunctionBase, IDependencyTrigger
    {
        private readonly List<IEdmFunctionParameter> parameters = new List<IEdmFunctionParameter>();
        private IEdmTypeReference returnType;
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();

        /// <summary>
        /// Initializes a new instance of the EdmFunctionBase class.
        /// </summary>
        protected EdmFunctionBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EdmFunctionBase class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="returnType">The return type of the function.</param>
        protected EdmFunctionBase(string name, IEdmTypeReference returnType)
            : base(name)
        {
            this.returnType = returnType;
        }

        /// <summary>
        /// Gets or sets the name of this function.
        /// </summary>
        new public string Name
        {
            get { return this.elementName; }
            set { this.SetField(ref this.elementName, value ?? string.Empty); }
        }

        /// <summary>
        /// Gets or sets the return type of this function.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get { return this.returnType; }
            set { this.SetField(ref this.returnType, value); }
        }

        /// <summary>
        /// Gets the parameters of this function.
        /// </summary>
        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parameters; }
        }

        HashSetInternal<IDependent> IDependencyTrigger.Dependents
        {
            get { return this.dependents; }
        }

        /// <summary>
        /// Searches for a parameter with the given name in this function and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter to be found.</param>
        /// <returns>The requested parameter, or null if no such parameter exists.</returns>
        public IEdmFunctionParameter FindParameter(string name)
        {
            foreach (IEdmFunctionParameter parameter in this.parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        ///  <param name="type">The type of the parameter being added.</param>
        public void AddParameter(string name, IEdmTypeReference type)
        {
            this.parameters.Add(new EdmFunctionParameter(name, type));
            this.FireDependency();
        }

        /// <summary>
        /// Adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="parameter">The parameter being added.</param>
        public void AddParameter(IEdmFunctionParameter parameter)
        {
            this.parameters.Add(parameter);
            this.FireDependency();
        }

        /// <summary>
        /// Removes a parameter from this function.
        /// </summary>
        /// <param name="parameter">The parameter being removed.</param>
        public void RemoveParameter(IEdmFunctionParameter parameter)
        {
            this.parameters.Remove(parameter);
            this.FireDependency();
        }
    }
}
