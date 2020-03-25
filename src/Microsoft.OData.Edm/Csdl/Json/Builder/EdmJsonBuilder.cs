//---------------------------------------------------------------------
// <copyright file="EdmModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json.Ast;
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

            //IDictionary<string, EdmStructuredType> structuredTypes = typeBuilder.StructuredTypes;
            //IDictionary<string, EdmEnumType> enumTypes = typeBuilder.EnumTypes;
            //IDictionary<string, EdmTypeDefinition> typeDefinitions = typeBuilder.TypeDefinitions;



            return _modelMapping[mainCsdlModel];
        }

        public IEdmModel TryBuildEdmModel(CsdlJsonModel csdlJsonModel)
        {
            //List<SchemaJsonItem> allSchemaJsonItems = new List<SchemaJsonItem>();
            IDictionary<CsdlJsonSchemaItem, CsdlJsonModel> allSchemaJsonItems = new Dictionary<CsdlJsonSchemaItem, CsdlJsonModel>();

            foreach ( var schema in csdlJsonModel.Schemas)
            {
                foreach (CsdlJsonSchemaItem item in schema.SchemaItems)
                {
                    allSchemaJsonItems.Add(item, csdlJsonModel);
                }

                //foreach (CsdlJsonSchemaItem item in schema.EnumTypes)
                //{
                //    allSchemaJsonItems.Add(item, csdlJsonModel);
                //}
            }

            if (csdlJsonModel.ReferencedModels != null)
            {
                foreach (var referenced in csdlJsonModel.ReferencedModels)
                {
                    foreach (var schema in referenced.Schemas)
                    {
                        foreach (CsdlJsonSchemaItem item in schema.SchemaItems)
                        {
                            allSchemaJsonItems.Add(item, referenced);
                        }

                        //foreach (CsdlJsonSchemaItem item in schema.EnumTypes)
                        //{
                        //    allSchemaJsonItems.Add(item, referenced);
                        //}
                    }
                }
            }

            // Build All Structural types
         //   IEnumerable<StructuredTypeJsonItem> structuralTypeJsonItems = allSchemaJsonItems.OfType<StructuredTypeJsonItem>();

            EdmTypeJsonBuilder typeBuilder = new EdmTypeJsonBuilder(allSchemaJsonItems, _options);

            EdmModel mainModel = new EdmModel(false);

            typeBuilder.BuildSchemaItems();

            var allTypes = typeBuilder.BuiltTypes;

            if (csdlJsonModel.ReferencedModels != null)
            {
                foreach (var referenced in csdlJsonModel.ReferencedModels)
                {
                    IEdmModel subModel = BuildEdmModel(referenced, allTypes);

                    mainModel.AddReferencedModel(subModel);
                }
            }

            return BuildEdmModel(csdlJsonModel, allTypes);

            // Build All Enum Types, TypeDefintions

            // Build All Terms, Actions, Function

            // Build EntityContainer

            // Now, build all bodies

        }

        private static IEdmModel BuildEdmModel(CsdlJsonModel csdlModel, IDictionary<string, IEdmSchemaElement> allSchemaElements)
        {
            EdmModel edmModel = new EdmModel(false);
            foreach (var schema in csdlModel.Schemas)
            {
                foreach (CsdlJsonSchemaItem item in schema.SchemaItems)
                {
                    IEdmSchemaElement schemaElement = allSchemaElements[item.FullName];
                    edmModel.AddElement(schemaElement);

                    foreach (var annotation in item.Annotations)
                    {
                        annotation.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
                        edmModel.SetVocabularyAnnotation(annotation);
                    }
                }
            }

            edmModel.SetEdmVersion(csdlModel.Version);

            edmModel.SetEdmReferences(csdlModel.CurrentModelReferences);

            if (csdlModel.NamespaceAlias != null)
            {
                foreach (var item in csdlModel.NamespaceAlias)
                {
                    edmModel.SetNamespaceAlias(item.Value, item.Key);
                }
            }

            return edmModel;
        }
    }
}
