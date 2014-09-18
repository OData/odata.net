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

using System;
using System.Collections.Generic;

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
