//---------------------------------------------------------------------
// <copyright file="CsdlFunctionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL function import.
    /// </summary>
    internal class CsdlFunctionImport : CsdlOperationImport
    {
        public CsdlFunctionImport(
            string name,
            string schemaOperationQualifiedTypeName,
            string entitySet,
            bool includeInServiceDocument,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(name, schemaOperationQualifiedTypeName, entitySet, new CsdlOperationParameter[] { }, null /*returnType*/, documentation, location)
        {
            this.IncludeInServiceDocument = includeInServiceDocument;
        }

        public bool IncludeInServiceDocument { get; private set; }
    }
}
