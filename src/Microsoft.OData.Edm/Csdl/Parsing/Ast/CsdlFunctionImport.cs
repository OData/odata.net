//---------------------------------------------------------------------
// <copyright file="CsdlFunctionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;

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
            CsdlLocation location)
            : base(name, schemaOperationQualifiedTypeName, entitySet, Enumerable.Empty<CsdlOperationParameter>(), null /*returnType*/, location)
        {
            this.IncludeInServiceDocument = includeInServiceDocument;
        }

        public bool IncludeInServiceDocument { get; private set; }
    }
}
