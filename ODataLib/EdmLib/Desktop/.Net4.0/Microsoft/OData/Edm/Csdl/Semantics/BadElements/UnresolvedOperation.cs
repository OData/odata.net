//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Represents information about an EDM operation that failed to resolve.
    /// </summary>
    internal class UnresolvedOperation : BadElement, IEdmOperation, IUnresolvedElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly IEdmTypeReference returnType;

        public UnresolvedOperation(string qualifiedName, string errorMessage, EdmLocation location) 
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedOperation, errorMessage) })
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
            this.returnType = new BadTypeReference(new BadType(this.Errors), true);
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnType; }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return Enumerable.Empty<IEdmOperationParameter>(); }
        }

        public bool IsBound
        {
            get { return false; }
        }

        public Expressions.IEdmPathExpression EntitySetPath
        {
            get { return null; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.None; }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            return null;
        }
    }
}
