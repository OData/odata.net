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
using System.Linq;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a semantically invalid EDM entity container.
    /// </summary>
    internal class BadEntityContainer : BadElement, IEdmEntityContainer
    {
        private readonly string name;

        public BadEntityContainer(string name, IEnumerable<EdmError> errors)
            : base(errors)
        {
            this.name = name ?? string.Empty;
        }

        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return Enumerable.Empty<IEdmEntityContainerElement>(); }
        }

        public IEdmEntitySet FindEntitySet(string setName)
        {
            return null;
        }

        public IEdmAssociationSet FindAssociationSet(string setName)
        {
            return null;
        }

        public IEnumerable<IEdmFunctionImport> FindFunctionImports(string functionName)
        {
            return null;
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
