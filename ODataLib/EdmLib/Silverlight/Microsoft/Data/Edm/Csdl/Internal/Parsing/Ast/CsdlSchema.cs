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

using System;
using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL schema.
    /// </summary>
    internal class CsdlSchema : CsdlElementWithDocumentation
    {
        private readonly List<CsdlUsing> usings;
        private readonly List<CsdlAssociation> associations;
        private readonly List<CsdlStructuredType> structuredTypes;
        private readonly List<CsdlEnumType> enumTypes;
        private readonly List<CsdlFunction> functions;
        private readonly List<CsdlValueTerm> valueTerms;
        private readonly List<CsdlEntityContainer> entityContainers;
        private readonly List<CsdlAnnotations> outOfLineAnnotations;

        private readonly string alias;
        private readonly string namespaceName;
        private readonly Version version;

        public CsdlSchema(
            string namespaceName,
            string alias,
            Version version,
            IEnumerable<CsdlUsing> usings,
            IEnumerable<CsdlAssociation> associations,
            IEnumerable<CsdlStructuredType> structuredTypes,
            IEnumerable<CsdlEnumType> enumTypes,
            IEnumerable<CsdlFunction> functions,
            IEnumerable<CsdlValueTerm> valueTerms,
            IEnumerable<CsdlEntityContainer> entityContainers,
            IEnumerable<CsdlAnnotations> outOfLineAnnotations,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(documentation, location)
        {
            this.alias = alias;
            this.namespaceName = namespaceName;
            this.version = version;
            this.usings = new List<CsdlUsing>(usings);
            this.associations = new List<CsdlAssociation>(associations);
            this.structuredTypes = new List<CsdlStructuredType>(structuredTypes);
            this.enumTypes = new List<CsdlEnumType>(enumTypes);
            this.functions = new List<CsdlFunction>(functions);
            this.valueTerms = new List<CsdlValueTerm>(valueTerms);
            this.entityContainers = new List<CsdlEntityContainer>(entityContainers);
            this.outOfLineAnnotations = new List<CsdlAnnotations>(outOfLineAnnotations);
        }

        public IEnumerable<CsdlUsing> Usings
        {
            get { return this.usings; }
        }

        public IEnumerable<CsdlAssociation> Associations
        {
            get { return this.associations; }
        }

        public IEnumerable<CsdlStructuredType> StructuredTypes
        {
            get { return this.structuredTypes; }
        }

        public IEnumerable<CsdlEnumType> EnumTypes
        {
            get { return this.enumTypes; }
        }

        public IEnumerable<CsdlFunction> Functions
        {
            get { return this.functions; }
        }

        public IEnumerable<CsdlValueTerm> ValueTerms
        {
            get { return this.valueTerms; }
        }

        public IEnumerable<CsdlEntityContainer> EntityContainers
        {
            get { return this.entityContainers; }
        }

        public IEnumerable<CsdlAnnotations> OutOfLineAnnotations
        {
            get { return this.outOfLineAnnotations; }
        }

        public string Alias
        {
            get { return this.alias; }
        }

        public string Namespace
        {
            get { return this.namespaceName; }
        }

        public Version Version
        {
            get { return this.version; }
        }
    }
}
