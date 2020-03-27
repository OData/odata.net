//---------------------------------------------------------------------
// <copyright file="CsdlModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL model.
    /// </summary>
    internal class CsdlModel
    {
        private IDictionary<string, string> aliasNsmapping;
        private readonly List<CsdlSchema> schemata = new List<CsdlSchema>();
        private readonly List<IEdmReference> currentModelReferences = new List<IEdmReference>();
        private readonly List<IEdmReference> parentModelReferences = new List<IEdmReference>();

        public CsdlModel() { }
        public CsdlModel(string uri, Version version)
        {
            Uri = uri;
            Version = version;
        }

        public string Uri { get; }

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

        public Version Version { get; set; }

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

        public IList<CsdlModel> ReferencedModels { get; private set; }

        public void AddReferencedModel(CsdlModel referencedModel)
        {
            if (ReferencedModels == null)
            {
                ReferencedModels = new List<CsdlModel>();
            }

            ReferencedModels.Add(referencedModel);
        }

        public bool IsReferencedModelAdded(string uri)
        {
            if (ReferencedModels == null)
            {
                return false;
            }

            return ReferencedModels.Any(c => c.Uri == uri);
        }

        public IDictionary<string, string> AliasNsMapping
        {
            get
            {
                BuildAliasNamespaceMapping();
                return this.aliasNsmapping;
            }
        }
        public string ReplaceAlias(string input)
        {
            if (this.aliasNsmapping == null)
            {
                BuildAliasNamespaceMapping();
            }

            if (this.aliasNsmapping.Count == 0)
            {
                return input;
            }

            int idx = input.IndexOf('.');
            if (idx > 0)
            {
                var typeAlias = input.Substring(0, idx);

                string namespaceFound;
                if (this.aliasNsmapping.TryGetValue(typeAlias, out namespaceFound))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}{1}", namespaceFound, input.Substring(idx));
                }
            }

            return input;
        }

        private void BuildAliasNamespaceMapping()
        {
            if (this.aliasNsmapping != null)
            {
                return;
            }

            this.aliasNsmapping = new Dictionary<string, string>();
            foreach (var includes in currentModelReferences.SelectMany(s => s.Includes))
            {
                // The value of $Include is an array. Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                this.aliasNsmapping.Add(includes.Alias, includes.Namespace);
            }

            foreach (var schema in this.schemata)
            {
                if (schema.Alias != null)
                {
                    this.aliasNsmapping.Add(schema.Alias, schema.Namespace);
                }
            }
        }
    }
}
