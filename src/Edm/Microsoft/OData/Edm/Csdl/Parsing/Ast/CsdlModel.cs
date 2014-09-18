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
    /// <summary>
    /// Represents a CSDL model.
    /// </summary>
    internal class CsdlModel
    {
        private readonly List<CsdlSchema> schemata = new List<CsdlSchema>();
        private readonly List<IEdmReference> currentModelReferences = new List<IEdmReference>();
        private readonly List<IEdmReference> parentModelReferences = new List<IEdmReference>();

        /// <summary>
        /// Represents current model's $lt;edmx:Reference /&gt;
        /// </summary>
        public IEnumerable<IEdmReference> CurrentModelReferences
        {
            get { return currentModelReferences; }
        }

        /// <summary>
        /// Represents parent model's $lt;edmx:Reference ... /&gt;
        /// </summary>
        public IEnumerable<IEdmReference> ParentModelReferences
        {
            get { return parentModelReferences; }
        }

        public IEnumerable<CsdlSchema> Schemata
        {
            get { return this.schemata; }
        }

        public void AddSchema(CsdlSchema schema)
        {
            this.schemata.Add(schema);
        }

        /// <summary>
        /// Adds from current model.
        /// </summary>
        /// <param name="referencesToAdd">The items to add.</param>
        public void AddCurrentModelReferences(IEnumerable<IEdmReference> referencesToAdd)
        {
            this.currentModelReferences.AddRange(referencesToAdd);
        }

        /// <summary>
        /// Adds from main model.
        /// </summary>
        /// <param name="referenceToAdd">The IEdmReference to add.</param>
        public void AddParentModelReferences(IEdmReference referenceToAdd)
        {
            this.parentModelReferences.Add(referenceToAdd);
        }
    }
}
