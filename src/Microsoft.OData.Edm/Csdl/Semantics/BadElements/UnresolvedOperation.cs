//---------------------------------------------------------------------
// <copyright file="UnresolvedOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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

        public IEdmPathExpression EntitySetPath
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
