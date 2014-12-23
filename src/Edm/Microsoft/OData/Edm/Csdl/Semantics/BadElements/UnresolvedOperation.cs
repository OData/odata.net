//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
