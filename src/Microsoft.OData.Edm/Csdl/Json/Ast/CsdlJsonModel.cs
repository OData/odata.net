//---------------------------------------------------------------------
// <copyright file="CsdlJsonModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonModel
    {
    //    private List<SchemaJsonItem> _schemaItems;
        private IList<CsdlJsonSchema> _schemas;
        private readonly List<IEdmReference> _currentModelReferences = new List<IEdmReference>();
        private readonly List<IEdmReference> _parentModelReferences = new List<IEdmReference>();

        // Aliases are document-global, so all schemas defined within or included into a document MUST have different aliases,
        // and aliases MUST differ from the namespaces of all schemas defined within or included into a document. 
        // Key is alias,
        // Value is the namespace
        private IDictionary<string, string> _namespaceAlias;

        public CsdlJsonModel(string uri, Version version)
        {
            Uri = uri;
            Version = version;
            ReferencedModels = null;
            _schemas = new List<CsdlJsonSchema>();
        }

        public string Uri { get; }

        public Version Version { get; }
        public EdmModel EdmModel { get; }

        public IEnumerable<IEdmReference> CurrentModelReferences
        {
            get
            {
                return _currentModelReferences;
            }
        }
        /// <summary>
        /// Adds from current model.
        /// </summary>
        /// <param name="referencesToAdd">The items to add.</param>
        public void AddCurrentModelReferences(IEnumerable<IEdmReference> referencesToAdd)
        {
            if (referencesToAdd != null)
            {
                _currentModelReferences.AddRange(referencesToAdd);
            }
        }

        /// <summary>
        /// Adds from main model.
        /// </summary>
        /// <param name="referenceToAdd">The IEdmReference to add.</param>
        public void AddParentModelReferences(IEdmReference referenceToAdd)
        {
            _parentModelReferences.Add(referenceToAdd);
        }

        public IList<CsdlJsonSchema> Schemas { get { return _schemas;  } }

        public void AddSchema(CsdlJsonSchema schema)
        {
            _schemas.Add(schema);
        }
        public IDictionary<string, string> NamespaceAlias { get { return _namespaceAlias; } }

        public IList<CsdlJsonModel> ReferencedModels { get; private set; }

        //public IList<SchemaJsonItem> SchemaItems { get { return _schemaItems; } }

        //public void AddSchemaJsonItems(IList<SchemaJsonItem> items)
        //{
        //    if (_schemaItems == null)
        //    {
        //        _schemaItems = new List<SchemaJsonItem>();
        //    }

        //    _schemaItems.AddRange(items);
        //}

        public void AddReferencedModel(CsdlJsonModel referencedModel)
        {
            if (ReferencedModels == null)
            {
                ReferencedModels = new List<CsdlJsonModel>();
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

        public void BuildNamespaceAlias()
        {
            if (_namespaceAlias != null)
            {
                return; // done before
            }

            _namespaceAlias = new Dictionary<string, string>();

            foreach (var includes in _currentModelReferences.SelectMany(s => s.Includes))
            {
                // The value of $Include is an array. Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                _namespaceAlias.Add(includes.Alias, includes.Namespace);
            }

            foreach (var schema in _schemas)
            {
                if (schema.Alias != null)
                {
                    _namespaceAlias.Add(schema.Alias, schema.Namespace);
                }
            }
        }

        internal string ReplaceAlias(string name)
        {
            if (_namespaceAlias == null)
            {
                return name;
            }

            int idx = name.IndexOf('.');
            if (idx > 0)
            {
                var typeAlias = name.Substring(0, idx);

                string namespaceFound;
                if (_namespaceAlias.TryGetValue(typeAlias, out namespaceFound))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}{1}", namespaceFound, name.Substring(idx));
                }
            }

            return name;
        }
    }
}
