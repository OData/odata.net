//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL entity container.
    /// </summary>
    internal class CsdlEntityContainer : CsdlNamedElement
    {
        private readonly string extends;
        private readonly List<CsdlEntitySet> entitySets;
        private readonly List<CsdlAssociationSet> associationSets;
        private readonly List<CsdlFunctionImport> functionImports;

        public CsdlEntityContainer(string name, string extends, IEnumerable<CsdlEntitySet> entitySets, IEnumerable<CsdlAssociationSet> associationSets, IEnumerable<CsdlFunctionImport> functionImports, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.extends = extends;
            this.entitySets = new List<CsdlEntitySet>(entitySets);
            this.associationSets = new List<CsdlAssociationSet>(associationSets);
            this.functionImports = new List<CsdlFunctionImport>(functionImports);
        }

        public string Extends
        {
            get { return this.extends; }
        }

        public IEnumerable<CsdlEntitySet> EntitySets
        {
            get { return this.entitySets; }
        }

        public IEnumerable<CsdlAssociationSet> AssociationSets
        {
            get { return this.associationSets; }
        }

        public IEnumerable<CsdlFunctionImport> FunctionImports
        {
            get { return this.functionImports; }
        }
    }
}
