//---------------------------------------------------------------------
// <copyright file="EdmModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.Builder
{
    internal class EdmModelBuilder
    {
        private CsdlSerializerOptions _options;

        private IDictionary<CsdlModel, EdmModel> _modelMapping = new Dictionary<CsdlModel, EdmModel>();

        public EdmModelBuilder(CsdlSerializerOptions options)
        {
            _options = options;
        }

        public EdmModel TryBuildEdmModel(CsdlModel mainCsdlModel, IList<CsdlModel> referencedModels)
        {
            // Build Alias => Namespace mapping
            List<CsdlModel> allModels = new List<CsdlModel>();
            allModels.Add(mainCsdlModel);
            allModels.AddRange(referencedModels);

            EdmModel mainModel = BuildModelHeader(mainCsdlModel, null);

            foreach (var csdlModel in referencedModels)
            {
                BuildModelHeader(csdlModel, mainModel);
            }

            EdmSchemaBuilder typeBuilder = new EdmSchemaBuilder(_modelMapping);
            typeBuilder.BuildSchemaItems();

            return _modelMapping[mainCsdlModel];
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

            foreach (var aliasNsItem in csdlModel.AliasNsMapping)
            {
                edmModel.SetNamespaceAlias(aliasNsItem.Value, aliasNsItem.Key);
            }

            if (mainModel != null)
            {
                // We don't need the CoreMore everywhere?
                edmModel.RemoveReferencedMode(EdmCoreModel.Instance);
                mainModel.AddReferencedModel(edmModel);
            }

            _modelMapping[csdlModel] = edmModel;
            return edmModel;
        }
    }
}
