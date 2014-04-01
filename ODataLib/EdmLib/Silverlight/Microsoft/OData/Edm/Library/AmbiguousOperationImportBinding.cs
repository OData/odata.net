//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
