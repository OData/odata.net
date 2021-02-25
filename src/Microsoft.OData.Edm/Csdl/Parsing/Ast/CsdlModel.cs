//---------------------------------------------------------------------
// <copyright file="CsdlModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL model.
    /// </summary>
    internal class CsdlModel
    {
        private readonly List<CsdlSchema> schemata = new List<CsdlSchema>();
        private readonly List<CsdlReference> currentModelReferences = new List<CsdlReference>();
        private readonly List<CsdlReference> parentModelReferences = new List<CsdlReference>();

        /// <summary>
        /// Represents current model's $lt;edmx:Reference /&gt;
        /// </summary>
        public IEnumerable<CsdlReference> CurrentModelReferences
        {
            get { return currentModelReferences; }
        }

        /// <summary>
        /// Represents parent model's $lt;edmx:Reference ... /&gt;
        /// </summary>
        public IEnumerable<CsdlReference> ParentModelReferences
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

        public Version CsdlVersion { get; set; }

        /// <summary>
        /// Adds from current model.
        /// </summary>
        /// <param name="referencesToAdd">The items to add.</param>
        public void AddCurrentModelReferences(IEnumerable<CsdlReference> referencesToAdd)
        {
            this.currentModelReferences.AddRange(referencesToAdd);
        }

        /// <summary>
        /// Adds from main model.
        /// </summary>
        /// <param name="referenceToAdd">The IEdmReference to add.</param>
        public void AddParentModelReferences(CsdlReference referenceToAdd)
        {
            this.parentModelReferences.Add(referenceToAdd);
        }
    }
}
