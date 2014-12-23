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
