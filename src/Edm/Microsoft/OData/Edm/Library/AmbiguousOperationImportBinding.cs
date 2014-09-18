//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Class that represents an unresolved operation import binding to two or more operation imports.
    /// </summary>
    internal class AmbiguousOperationImportBinding : AmbiguousBinding<IEdmOperationImport>, IEdmOperationImport
    {
        private readonly IEdmOperationImport first;

        public AmbiguousOperationImportBinding(IEdmOperationImport first, IEdmOperationImport second)
            : base(first, second)
        {
            this.first = first;
        }

        public IEdmOperation Operation
        {
            get { return this.first.Operation; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return null; }
        }

        public IEdmEntityContainer Container
        {
            get { return first.Container; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return first.ContainerElementKind; }
        }

        public IEdmExpression EntitySet
        {
            get { return null; }
        }
    }
}
