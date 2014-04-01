//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL model.
    /// </summary>
    internal class CsdlModel
    {
        private readonly List<CsdlSchema> schemata = new List<CsdlSchema>();
        private readonly List<IEdmInclude> includes = new List<IEdmInclude>();
        private readonly List<IEdmIncludeAnnotations> includeAnnotations = new List<IEdmIncludeAnnotations>();

        /// <summary>
        /// if count ==0, includes all types. 
        /// It represents $lt;edmx:Include Namespace="Org.OData.Capabilities.V1" /&gt;
        /// TODO challenh REF p2 rename it to 'ToBeExported'
        /// </summary>
        public IEnumerable<IEdmInclude> Includes
        {
            get { return includes; }
        }

        /// <summary>
        /// if count ==0, includes all annotations.
        /// It represents $lt;edmx:IncludeAnnotations ... /&gt;
        /// </summary>
        public IEnumerable<IEdmIncludeAnnotations> IncludeAnnotations
        {
            get { return includeAnnotations; }
        }

        public IEnumerable<CsdlSchema> Schemata
        {
            get { return this.schemata; }
        }

        public void AddSchema(CsdlSchema schema)
        {
            this.schemata.Add(schema);
        }

        public void AddIncludes(IEnumerable<IEdmInclude> includesToAdd)
        {
            this.includes.AddRange(includesToAdd);
        }

        public void AddIncludeAnnotations(IEnumerable<IEdmIncludeAnnotations> includeAnnotationsToAdd)
        {
            this.includeAnnotations.AddRange(includeAnnotationsToAdd);
        }
    }
}
