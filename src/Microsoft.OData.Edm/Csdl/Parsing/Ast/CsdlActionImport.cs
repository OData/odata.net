//---------------------------------------------------------------------
// <copyright file="CsdlActionImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL action import.
    /// </summary>
    internal class CsdlActionImport : CsdlOperationImport
    {
        public CsdlActionImport(
            string name,
            string schemaOperationQualifiedTypeName,
            string entitySet,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(name, schemaOperationQualifiedTypeName, entitySet, new CsdlOperationParameter[] { }, null /*returnType*/, documentation, location)
        {
        }
    }
}
