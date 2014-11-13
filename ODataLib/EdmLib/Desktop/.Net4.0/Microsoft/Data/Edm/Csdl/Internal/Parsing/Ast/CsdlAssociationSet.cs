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

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL association set.
    /// </summary>
    internal class CsdlAssociationSet : CsdlNamedElement
    {
        private readonly string association;
        private readonly CsdlAssociationSetEnd end1;
        private readonly CsdlAssociationSetEnd end2;

        public CsdlAssociationSet(string name, string association, CsdlAssociationSetEnd end1, CsdlAssociationSetEnd end2, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.association = association;
            this.end1 = end1;
            this.end2 = end2;
        }

        public string Association
        {
            get { return this.association; }
        }

        public CsdlAssociationSetEnd End1
        {
            get { return this.end1; }
        }

        public CsdlAssociationSetEnd End2
        {
            get { return this.end2; }
        }
    }
}
