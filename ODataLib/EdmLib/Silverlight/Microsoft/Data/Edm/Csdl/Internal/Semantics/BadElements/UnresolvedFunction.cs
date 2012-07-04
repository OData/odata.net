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

using System.Linq;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents information about an EDM function that failed to resolve.
    /// </summary>
    internal class UnresolvedFunction : BadElement, IEdmFunction, IUnresolvedElement
    {
        private readonly string namespaceName;
        private readonly string name;
        private readonly IEdmTypeReference returnType;

        public UnresolvedFunction(string qualifiedName, string errorMessage,  EdmLocation location)
            : base(new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedFunction, errorMessage) })
        {
            qualifiedName = qualifiedName ?? string.Empty;
            EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name);
            this.returnType = new BadTypeReference(new BadType(this.Errors), true);
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string DefiningExpression
        {
            get { return null; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnType; }
        }

        public System.Collections.Generic.IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return Enumerable.Empty<IEdmFunctionParameter>(); }
        }

        public IEdmFunctionParameter FindParameter(string name)
        {
            return null;
        }
    }
}
