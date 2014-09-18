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

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    using System;

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
