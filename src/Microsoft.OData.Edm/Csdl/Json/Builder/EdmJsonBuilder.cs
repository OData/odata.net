//---------------------------------------------------------------------
// <copyright file="EdmModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    internal class AliasNamespaceHelper
    {
        private IDictionary<CsdlModel, IDictionary<string, string>> _mappings;
        public AliasNamespaceHelper(IList<CsdlModel> allModels)
        {
            _mappings = new Dictionary<CsdlModel, IDictionary<string, string>>();
            foreach (var csdlModel in allModels)
            {
                IDictionary<string, string> aliasNamespace = new Dictionary<string, string>();

                foreach (var includes in csdlModel.CurrentModelReferences.SelectMany(s => s.Includes))
                {
                    // The value of $Include is an array. Array items are objects that MUST contain the member $Namespace and MAY contain the member $Alias.
                    aliasNamespace.Add(includes.Alias, includes.Namespace);
                }

                foreach (var schema in csdlModel.Schemata)
                {
                    if (schema.Alias != null)
                    {
                        aliasNamespace.Add(schema.Alias, schema.Namespace);
                    }
                }

                _mappings[csdlModel] = aliasNamespace;
            }
        }

        /// <summary>
        /// Alias => Namespace
        /// </summary>
        /// <param name="csdlModel"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetAliasNamespace(CsdlModel csdlModel)
        {
            IDictionary<string, string> aliasNs;
            _mappings.TryGetValue(csdlModel, out aliasNs);
            return aliasNs;
        }

        public string ReplaceAlias(CsdlModel csdlModel, string input)
        {
            IDictionary<string, string> aliasNs = GetAliasNamespace(csdlModel);
            if (aliasNs == null)
            {
                return input;
            }

            int idx = input.IndexOf('.');
            if (idx > 0)
            {
                var typeAlias = input.Substring(0, idx);

                string namespaceFound;
                if (aliasNs.TryGetValue(typeAlias, out namespaceFound))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}{1}", namespaceFound, input.Substring(idx));
                }
            }

            return input;
        }
    }

    internal class EdmModelBuilder
    {
        private CsdlSerializerOptions _options;

        private IDictionary<CsdlModel, EdmModel> _modelMapping = new Dictionary<CsdlModel, EdmModel>();
        private AliasNamespaceHelper _aliasNamespaceMapping;

        public EdmModelBuilder(CsdlSerializerOptions options)
        {
            _options = options;
        }

        private EdmModel BuildModelHeader(CsdlModel csdlModel, EdmModel mainModel)
        {
            if (_options == null)
            {
                return null;
            }

            EdmModel edmModel = new EdmModel(false);

            edmModel.SetEdmVersion(csdlModel.Version);

            edmModel.SetEdmReferences(csdlModel.CurrentModelReferences);

            foreach (var aliasNsItem in _aliasNamespaceMapping.GetAliasNamespace(csdlModel))
            {
                edmModel.SetNamespaceAlias(aliasNsItem.Value, aliasNsItem.Key);
            }

            if (mainModel != null)
            {
                edmModel.RemoveReferencedMode(EdmCoreModel.Instance);
                mainModel.AddReferencedModel(edmModel);
            }

            _modelMapping[csdlModel] = edmModel;
            return edmModel;
        }

        public EdmModel TryBuildEdmModel(CsdlModel mainCsdlModel, IList<CsdlModel> referencedModels)
        {
            // Build Alias => Namespace mapping
            List<CsdlModel> allModels = new List<CsdlModel>();
            allModels.Add(mainCsdlModel);
            allModels.AddRange(referencedModels);

            _aliasNamespaceMapping = new AliasNamespaceHelper(allModels);

            EdmModel mainModel = BuildModelHeader(mainCsdlModel, null);

            foreach (var csdlModel in referencedModels)
            {
                BuildModelHeader(csdlModel, mainModel);
            }

            SchemaTypeJsonBuilder typeBuilder = new SchemaTypeJsonBuilder(_modelMapping, _aliasNamespaceMapping);
            typeBuilder.BuildSchemaItems();

            return _modelMapping[mainCsdlModel];
        }
    }
}
