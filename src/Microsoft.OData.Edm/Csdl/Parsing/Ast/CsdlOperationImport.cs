//---------------------------------------------------------------------
// <copyright file="CsdlOperationImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL operation import.
    /// </summary>
    internal abstract class CsdlOperationImport : CsdlFunctionBase
    {
        private readonly string entitySet;

        protected CsdlOperationImport(
            string name,
            string schemaOperationQualifiedTypeName,
            string entitySet,
            IEnumerable<CsdlOperationParameter> parameters,
            CsdlTypeReference returnType,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(name, parameters, returnType, documentation, location)
        {
            this.entitySet = entitySet;
            this.SchemaOperationQualifiedTypeName = schemaOperationQualifiedTypeName;
        }

        public string EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the name of the schema operation qualified type.
        /// </summary>
        public string SchemaOperationQualifiedTypeName { get; private set; }
    }
}
