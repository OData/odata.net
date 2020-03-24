//---------------------------------------------------------------------
// <copyright file="EdmTypeJsonBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Json.Ast;
using Microsoft.OData.Edm.Csdl.Json.Value;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Json.Builder
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// Complex, Entity, Enum, TypeDefinition -> IEdmSchemaType
    /// </summary>
    internal class EdmTypeJsonBuilder
    {
        private IDictionary<CsdlJsonSchemaItem, CsdlJsonModel> _schemaItemsToModelMapping;

        private IDictionary<string, CsdlJsonSchemaItem> _schemaItems = new Dictionary<string, CsdlJsonSchemaItem>();

        private readonly IDictionary<string, IEdmSchemaElement> _schemaElements = new Dictionary<string, IEdmSchemaElement>();



        private CsdlSerializerOptions _options;
        internal EdmTypeJsonBuilder(IDictionary<CsdlJsonSchemaItem, CsdlJsonModel> sschemaItemsToModelMapping, CsdlSerializerOptions options)
        {
            _options = options;
            _schemaItemsToModelMapping = sschemaItemsToModelMapping;

            sschemaItemsToModelMapping.ForEach(k => _schemaItems[k.Key.FullName] = k.Key);
        }

        internal string ReplaceAlias(CsdlJsonSchemaItem jsonItem, string name)
        {
            CsdlJsonModel declaredModel = _schemaItemsToModelMapping[jsonItem];
            return declaredModel.ReplaceAlias(name);
        }

        public IDictionary<string, IEdmSchemaElement> BuiltTypes
        {
            get
            {
                return _schemaElements;
            }
        }


        public void BuildSchemaItems()
        {
            if (_options == null)
            {
                return;
            }

            // Reset
            _schemaElements.Clear();

            // Create headers to allow CreateEdmTypeBody to blindly references other things.
            foreach (var item in _schemaItems)
            {
                BuildSchemaElementHeader(item.Value);
            }

            // Build the term after building the types
            foreach (var termJsonItem in _schemaItems.Values.OfType<CsdlJsonSchemaTermItem>())
            {
                BuildTermType(termJsonItem);
            }

            foreach (var item in _schemaItems)
            {
                CreateSchemaElementBody(item.Value);
            }

            //foreach (StructuralTypeConfiguration structrual in _configurations.OfType<StructuralTypeConfiguration>())
            //{
            //    CreateNavigationProperty(structrual);
            //}

           // _schemaElements.ForEach(e => _edmModel.AddElement(e.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullQualifiedName"></param>
        /// <returns></returns>
        private IEdmSchemaElement GetSchemaElement(string fullQualifiedName) // maybe alias name?
        {
            IEdmSchemaElement schemaElement;
            if (_schemaElements.TryGetValue(fullQualifiedName, out schemaElement))
            {
                return schemaElement;
            }

            return null;
        }

        private CsdlJsonSchemaItem FindBaseItem(string fullQualifiedName)
        {
            CsdlJsonSchemaItem baseItem;
            _schemaItems.TryGetValue(fullQualifiedName, out baseItem);

            //
            return baseItem;
        }

        private void BuildSchemaElementHeader(CsdlJsonSchemaItem schemaItem)
        {
            IEdmSchemaElement schemaElement = GetSchemaElement(schemaItem.FullName);
            if (schemaElement != null)
            {
                // created before
                return;
            }

            switch (schemaItem.Kind)
            {
                case SchemaMemberKind.Complex:
                    CsdlJsonSchemaComplexItem complex = (CsdlJsonSchemaComplexItem)schemaItem;
                    IEdmComplexType baseComplexType = null;
                    if (complex.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(complex, complex.BaseType);

                        CsdlJsonSchemaItem baseItem = FindBaseItem(replacedBaseTypeName);

                        BuildSchemaElementHeader(baseItem);

                        baseComplexType = GetSchemaElement(replacedBaseTypeName) as IEdmComplexType;

                        Contract.Assert(baseComplexType != null);
                    }

                    EdmComplexType complexType = new EdmComplexType(schemaItem.Namespace,
                        schemaItem.Name,
                        baseComplexType, complex.IsAbstract, complex.IsOpen);

                    _schemaElements.Add(schemaItem.FullName, complexType);

                    break;

                case SchemaMemberKind.Entity:
                    CsdlJsonSchemaEntityItem entity = (CsdlJsonSchemaEntityItem)schemaItem;

                    IEdmEntityType baseEntityType = null;
                    if (entity.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(entity, entity.BaseType);

                        CsdlJsonSchemaItem baseItem = FindBaseItem(replacedBaseTypeName);

                        BuildSchemaElementHeader(baseItem);
                        baseEntityType = GetSchemaElement(replacedBaseTypeName) as IEdmEntityType;

                        Contract.Assert(baseEntityType != null);
                    }

                    EdmEntityType entityType = new EdmEntityType(schemaItem.Namespace, schemaItem.Name, baseEntityType,
                        entity.IsAbstract, entity.IsOpen, entity.HasStream);
                    _schemaElements.Add(schemaItem.FullName, entityType);

                    break;

                case SchemaMemberKind.Enum:
                    CsdlJsonSchemaEnumItem enumItem = (CsdlJsonSchemaEnumItem)schemaItem;

                    EdmEnumType enumType = new EdmEnumType(enumItem.Namespace, enumItem.Name,
                            GetEnumUnderlyingType(enumItem.UnderlyingTypeName), enumItem.IsFlags);

                    _schemaElements.Add(enumItem.FullName, enumType);

                    break;

                case SchemaMemberKind.TypeDefinition:
                    CsdlJsonSchemaTypeDefinitionItem typeDefinitionItem = (CsdlJsonSchemaTypeDefinitionItem)schemaItem;

                    EdmTypeDefinition typeDefinition = new EdmTypeDefinition(typeDefinitionItem.Namespace, typeDefinitionItem.Name,
                        GetUnderlyingType(typeDefinitionItem));

                    _schemaElements.Add(typeDefinitionItem.FullName, typeDefinition);

                    break;

                case SchemaMemberKind.EntityContainer:
                    CsdlJsonSchemaEntityContainerItem entityContainerItem = (CsdlJsonSchemaEntityContainerItem)schemaItem;

                    EdmEntityContainer edmEntityContainer = new EdmEntityContainer(entityContainerItem.Namespace, entityContainerItem.Name);
                    _schemaElements.Add(entityContainerItem.FullName, edmEntityContainer);

                    break;

                case SchemaMemberKind.Action:
                   // EdmAction()
                case SchemaMemberKind.Function:
                case SchemaMemberKind.Term:
                    // Don't build action, function, term until all types are built
                    break;
            }
        }

        private void BuildTermType(CsdlJsonSchemaTermItem termItem)
        {
            IEdmTypeReference termType = BuildTypeReference(termItem.QualifiedTypeName,
                termItem.IsCollection,
                termItem.Nulable,
                false,
                termItem.MaxLength,
                termItem.Unicode,
                termItem.Precision,
                termItem.Scale,
                termItem.Srid);

            string appliesTo = null;
            if (termItem.AppliesTo != null)
            {
                appliesTo = string.Join(" ", termItem.AppliesTo);
            }

            EdmTerm edmTerm = new EdmTerm(termItem.Namespace, termItem.Name, termType, appliesTo, termItem.DefaultValue);

            _schemaElements.Add(termItem.FullName, edmTerm);
        }

        private IEdmPrimitiveType GetUnderlyingType(CsdlJsonSchemaTypeDefinitionItem typeDefinitionItem)
        {
            IEdmTypeReference underlyingTypeRef = BuildTypeReference(typeDefinitionItem.UnderlyingTypeName,
                false,
                true,
                false,
                typeDefinitionItem.MaxLength,
                typeDefinitionItem.Unicode,
                typeDefinitionItem.Precision,
                typeDefinitionItem.Scale,
                typeDefinitionItem.Srid);

            IEdmType underlyingType = underlyingTypeRef.Definition;
            if (underlyingType.TypeKind != EdmTypeKind.Primitive)
            {
                throw new Exception();
            }

            return (IEdmPrimitiveType)underlyingType;
        }


        private static IEdmPrimitiveType GetEnumUnderlyingType(string underlyingType)
        {
            if (underlyingType != null)
            {
                var underlyingTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(underlyingType);
                if ( underlyingTypeKind != EdmPrimitiveTypeKind.None)
                {
                    return EdmCoreModel.Instance.GetPrimitiveType(underlyingTypeKind);
                }

                throw new Exception();
            }

            return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
        }

        private void CreateSchemaElementBody(CsdlJsonSchemaItem item)
        {
            IEdmSchemaElement schemaElement = GetSchemaElement(item.FullName);

            switch (item.Kind)
            {
                case SchemaMemberKind.Complex:
                    CreateComplexTypeBody((EdmComplexType)schemaElement, (CsdlJsonSchemaComplexItem)item);
                    break;

                case SchemaMemberKind.Entity:
                    CreateEntityTypeBody((EdmEntityType)schemaElement, (CsdlJsonSchemaEntityItem)item);
                    break;

                case SchemaMemberKind.Enum:
                    CreateEnumTypeBody((EdmEnumType)schemaElement, (CsdlJsonSchemaEnumItem)item);
                    break;

                case SchemaMemberKind.TypeDefinition:
                    // TypeDefinition may have annotations
                    break;

                case SchemaMemberKind.Action:
                    break;

                case SchemaMemberKind.Function:
                    break;

                case SchemaMemberKind.Term:
                    // The body of term may contain the annotation.
                    break;

                case SchemaMemberKind.EntityContainer:
                    BuildEntityContainerBody((EdmEntityContainer)schemaElement, (CsdlJsonSchemaEntityContainerItem)item);
                    break;
            }
        }

        private void BuildEntityContainerBody(EdmEntityContainer edmEntityContainer, CsdlJsonSchemaEntityContainerItem entityContainerJsonItem)
        {
            Contract.Assert(edmEntityContainer != null);
            Contract.Assert(entityContainerJsonItem != null);

            foreach (var member in entityContainerJsonItem.Members)
            {
                string name = member.Key;

                // It maybe entity container's annotation
                if (name[0] == '@')
                {
              //      string termName = name.Substring(1);
              //      IEdmTerm edmTerm = FindTerm(termName);
            //        IEdmExpression expression = BuildExpression(member.Value, entityContainerJsonItem.JsonPath, edmTerm.Type);

                    // annotation for the enum type
         //           EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(edmEntityContainer,
              //          edmTerm, expression);

                  //  edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);

                 //   _edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);

                    continue;
                }

                if (TryBuildOperationImport(edmEntityContainer, name, member.Value, entityContainerJsonItem.JsonPath))
                {
                    return;
                }

                if (TryBuildNavigationSource(edmEntityContainer, entityContainerJsonItem, name, member.Value, entityContainerJsonItem.JsonPath))
                {
                    return;
                }
            }
        }

        public static bool TryBuildOperationImport(EdmEntityContainer edmEntityContainer, string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return false;
            }

            JsonObjectValue objValue = (JsonObjectValue)jsonValue;

            IJsonValue importValue;
            if (objValue.TryGetValue("$Action", out importValue))
            {
                // ActionImport
                return BuildCsdlActionImport(edmEntityContainer, name, objValue, jsonPath);
            }

            if (objValue.TryGetValue("$Function", out importValue))
            {
                // FunctionImport
                return BuildCsdlActionImport(edmEntityContainer, name, objValue, jsonPath);
            }

            return false;
        }

        public static bool BuildCsdlActionImport(EdmEntityContainer edmEntityContainer, string name, JsonObjectValue importObject, IJsonPath jsonPath)
        {
         //   string action = null;
            string entitySet = null;
            foreach (var property in importObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Action":
                        // The value of $Action is a string containing the qualified name of an unbound action.
               //         action = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    default:
                        //if (propertyName[0] == '@')
                        //{
                        //    string termName = propertyName.Substring(1);
                        //    annotations.Add(BuildCsdlAnnotation(termName, propertyValue));
                        //}

                        break;
                }
            }

            IEdmAction edmAction = null;

            EdmActionImport ationImport = new EdmActionImport(edmEntityContainer, name, edmAction, new EdmPathExpression(entitySet));
            edmEntityContainer.AddElement(ationImport);
            return true;
        }

        public static bool BuildCsdlFunctionImport(EdmEntityContainer edmEntityContainer, string name, JsonObjectValue importObject, IJsonPath jsonPath)
        {
         //   string function = null;
            string entitySet = null;
            bool? includeInServiceDocument = false; // Absence of the member means false.
            foreach (var property in importObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Function":
                        // The value of $Function is a string containing the qualified name of an unbound function.
                //        function = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$EntitySet":
                        // The value of $EntitySet is a string containing either the unqualified name of an entity set in the same entity container
                        // or a path to an entity set in a different entity container.
                        entitySet = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means false.
                        includeInServiceDocument = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        break;

                    default:
                        //if (propertyName[0] == '@')
                        //{
                        //    string termName = propertyName.Substring(1);
                        //    annotations.Add(BuildCsdlAnnotation(termName, propertyValue));
                        //}

                        break;
                }
            }

            IEdmFunction edmFunction = null;

            EdmFunctionImport functionImport = new EdmFunctionImport(edmEntityContainer, name, edmFunction, new EdmPathExpression(entitySet), includeInServiceDocument.Value);
            edmEntityContainer.AddElement(functionImport);

            return true;
        }

        public bool TryBuildNavigationSource(EdmEntityContainer edmEntityContainer, 
            CsdlJsonSchemaEntityContainerItem entityContainerJsonItem, string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return false;
            }

            JsonObjectValue objValue = (JsonObjectValue)jsonValue;
      //      bool? nullable;
            string type = null;
            bool? isCollection = null;
            bool? includeInServiceDocument = null;
            foreach (var property in objValue)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Type":
                        // The value of $Type is the qualified name of an entity type.
                        type = propertyValue.ParseAsStringPrimitive();
                        break;

                    case "$Collection":
                        // The value of $Collection is the Booelan value true.
                        isCollection = propertyValue.ParseAsBooleanPrimitive();
                        break;

                    case "$IncludeInServiceDocument":
                        // The value of $IncludeInServiceDocument is one of the Boolean literals true or false. Absence of the member means true.
                        includeInServiceDocument = propertyValue.ParseAsBooleanPrimitive();
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        // In OData 4.0 responses this member MUST NOT be specified.
 //                       nullable = propertyValue.ParseAsBooleanPrimitive();
                        break;

                    case "$NavigationPropertyBinding":
                        // The value of $NavigationPropertyBinding is an object.
               //         navigationPropertyBindings = BuildCsdlNavigationPropertyBinding(propertyValue);
                        break;

                    default:
                        if (propertyName[0] == '@')
                        {
                            string qualifier;
                            string termName = TryParseAnnotationName(propertyName, out qualifier);
                            Debug.Assert(termName != null);

                            termName = ReplaceAlias(entityContainerJsonItem, termName);
                            IEdmTerm edmTerm = FindTerm(termName);

                            IEdmExpression expression = null;
                            AnnotationJsonBuilder annotationBuilder = new AnnotationJsonBuilder(_options);
                            if (edmTerm == null)
                            {
                                expression = annotationBuilder.BuildExpression(propertyValue, jsonPath);
                                edmTerm = new UnresolvedVocabularyTerm(termName);
                            }
                            else
                            {
                                expression = annotationBuilder.BuildExpression(propertyValue, jsonPath, edmTerm.Type);
                            }

                            EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(edmEntityContainer,
                              edmTerm, expression);

                            entityContainerJsonItem.AddAnnotations(edmVocabularyAnnotation);
                             //edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);

                            //   _edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);

                        }
                        break;
                }
            }

            if (type == null)
            {
                return false;
            }

            string replacedTypeName = ReplaceAlias(entityContainerJsonItem, type);

            IEdmEntityType entityType = GetSchemaElement(replacedTypeName) as IEdmEntityType;
         //   IEdmNavigationSource navigationSource = null;
            if (isCollection != null)
            {
                // entitySet
                if (!isCollection.Value)
                {
                    // If presented, $IsCollection should be true
                    throw new Exception();
                }

                EdmEntitySet entitySet = new EdmEntitySet(edmEntityContainer, name, entityType, includeInServiceDocument == null? true : includeInServiceDocument.Value);
            //    entitySet.
                edmEntityContainer.AddElement(entitySet);
            }
            else
            {
                //if (Version == EdmConstants.EdmVersion4)
                //{
                //    if (nullable != null)
                //    {
                //        throw new Exception();
                //    }
                //}
                // singleton
                EdmSingleton singleton = new EdmSingleton(edmEntityContainer, name, entityType);
                edmEntityContainer.AddElement(singleton);
            }

            return true;
        }

        private void CreateComplexTypeBody(EdmComplexType type, CsdlJsonSchemaComplexItem complexJsonItem)
        {
            Contract.Assert(type != null);
            Contract.Assert(complexJsonItem != null);

            BuildStructuredTypeBody(type, complexJsonItem);
        }

        private void CreateEntityTypeBody(EdmEntityType type, CsdlJsonSchemaEntityItem entityJsonItem)
        {
            Contract.Assert(type != null);
            Contract.Assert(entityJsonItem != null);

            BuildStructuredTypeBody(type, entityJsonItem);

            //var keys = ((IEnumerable<PropertyConfiguration>)config.Keys)
            //                         .Concat(config.EnumKeys)
            //                         .OrderBy(p => p.Order)
            //                         .ThenBy(p => p.Name)
            //                         .Select(p => type.DeclaredProperties.OfType<IEdmStructuralProperty>().First(dp => dp.Name == p.Name));
            //type.AddKeys(keys);
        }

        public class AnnotationWrapper
        {
            public IEdmTerm Term { get; set; }

            public IEdmExpression Expression { get; set; }
        }

        private void CreateEnumTypeBody(EdmEnumType enumType, CsdlJsonSchemaEnumItem enumJsonItem)
        {
            Contract.Assert(enumType != null);
            Contract.Assert(enumJsonItem != null);

            IList<IEdmEnumMember> enumMembers = new List<IEdmEnumMember>();

            IDictionary<string, IList<AnnotationWrapper>> memberAnnotations = new Dictionary<string, IList<AnnotationWrapper>>();

            foreach (var member in enumJsonItem.Members)
            {
                string propertyName = member.Key;
                IJsonValue propertyValue = member.Value;

                string annotationName;
                propertyName = TryParsePropertyName(propertyName, out annotationName);
                if (annotationName == null)
                {
                    // Without '@' in the name, so it's normal enum member name
                    IEdmEnumMember enumMember = BuildEnumMember(propertyName, enumType, propertyValue, enumJsonItem.JsonPath);

                    enumMembers.Add(enumMember); // save it for later annotation applying
                    enumType.AddMember(enumMember);
                }
                else
                {
                    string qualifier;
                    string termName = TryParseAnnotationName(annotationName, out qualifier);
                    Debug.Assert(termName != null);
                    IEdmTerm edmTerm = FindTerm(termName);

                    IEdmExpression expression = BuildExpression(propertyValue, enumJsonItem.JsonPath, edmTerm.Type);

                    if (propertyName == null)
                    {
                        // annotation for the enum type
              //          EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(enumType,
                   //         edmTerm, expression);

                   //     edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);

                   //     _edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);
                    }
                    else
                    {
                        // annotation on annotation
                        if (propertyName[0] == '@')
                        {
                            // So far, ODL doesn't support annotation on annotation
                        }
                        else
                        {
                            // annotation for enum member
                            IList<AnnotationWrapper> values;
                            if (!memberAnnotations.TryGetValue(propertyName, out values))
                            {
                                values = new List<AnnotationWrapper>();
                                memberAnnotations[propertyName] = values;
                            }

                            values.Add(new AnnotationWrapper
                            {
                                Term = edmTerm,
                                Expression = expression
                            });
                        }
                    }
                }
            }

            foreach (var item in memberAnnotations)
            {
                // TODO: maybe refactor later for performance
                IEdmEnumMember member = enumMembers.FirstOrDefault(c => c.Name == item.Key);
                if (member == null)
                {
                    throw new Exception();
                }

                //foreach (var annotation in item.Value)
                //{
                //    EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(member, annotation.Term, annotation.Expression);
                //    edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
                //    _edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);
                //}
            }

        }

        public static IEdmExpression BuildExpression(IJsonValue jsonValue, IJsonPath jsonPath, IEdmTypeReference edmType)
        {
            EdmTypeKind termTypeKind = edmType.TypeKind();
            switch (termTypeKind)
            {
                case EdmTypeKind.Primitive:
                   // IEdmPrimitiveTypeReference primitiveType = (IEdmPrimitiveTypeReference)edmType;
                    return null;
                    //return BuildPrimitiveExpression(jsonValue, jsonPath, primitiveType);

                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                  //  IEdmStructuredTypeReference structuredType = (IEdmStructuredTypeReference)edmType;
                    return null;
                    // return BuildRecordExpression(jsonValue, jsonPath, structuredType);

                case EdmTypeKind.Enum:
                    //return BuildEnumMemberExpression(jsonValue, jsonPath, edmType.AsEnum());

                case EdmTypeKind.TypeDefinition:
                    break;

                case EdmTypeKind.Path:
                    break;

                case EdmTypeKind.Collection:
                    //return BuildCollectionExpression(jsonValue, jsonPath, edmType.AsCollection());

                case EdmTypeKind.Untyped:
                // So far, we don't support a Untyped term.
                default:
                    // A valid term should not be here.
                    Debug.Assert(false, "We should be here never for a valid term.");
                    break;
            }

            return null;
        }

        public static IEdmEnumMember BuildEnumMember(string name, IEdmEnumType enumType, IJsonValue enumMemberObject, IJsonPath jsonPath)
        {
            // Enumeration Member Object
            // Enumeration type members are represented as JSON object members, where the object member name is the enumeration member name and the object member value is the enumeration member value.
            // For members of flags enumeration types a combined enumeration member value is equivalent to the bitwise OR of the discrete values.
            // Annotations for enumeration members are prefixed with the enumeration member name.

       //     CsdlLocation location = new CsdlLocation(-1, -1);
            long? value = enumMemberObject.ParseAsIntegerPrimitive(jsonPath);

            return new EdmEnumMember(enumType, name,
                    new EdmEnumMemberValue(value.Value));
        }

        private IEdmTerm FindTerm(string qualifiedName)
        {
            IEdmSchemaElement schemaElement;
            if (_schemaElements.TryGetValue(qualifiedName, out schemaElement))
            {
                return schemaElement as IEdmTerm;
            }

            return new UnresolvedVocabularyTerm(qualifiedName);
        }

        /// <summary>
        /// Parse the input string to see whether it's a valid annotation name.
        /// If it's valid, seperate string into term name or optional qualifier name.
        /// It it's not valid, return false
        /// </summary>
        /// <param name="propertyName">The input property name.</param>
        /// <param name="annotationName"></param>
        /// <returns></returns>
        internal static string TryParsePropertyName(string propertyName, out string annotationName)
        {
            //JsonReader already checks the property name.
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName));
            annotationName = null;

            // Annotation name consists of an at (@) character,
            // BE caution:
            // An annotation can itself be annotated. Annotations on annotations are represented as a member
            // whose name consists of the annotation name (including the optional qualifier),
            // followed by an at (@) character, followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier.
            // for example: 
            // "@Measures.ISOCurrency": "USD",
            // "@Measures.ISOCurrency@Core.Description": "The parent company’s currency"
            //
            // So, Core.Description is annotation for "Measures.ISOCurrency annotation.
            // Therefore, we use "LastIndexOf".
            int index = propertyName.LastIndexOf('@');
            if (index == -1)
            {
                return propertyName;
            }

            // with "@"
            annotationName = propertyName.Substring(index);
            return propertyName.Substring(0, index);
        }

        /// <summary>
        /// Parse the input string to see whether it's a valid annotation name.
        /// If it's valid, seperate string into term name or optional qualifier name.
        /// It it's not valid, return false
        /// </summary>
        /// <param name="propertyName">The input property name.</param>
        /// <param name="term"></param>
        /// <param name="qualifier"></param>
        /// <returns></returns>
        internal static string TryParseAnnotationName(string annotation, out string qualifier)
        {
            qualifier = null;
            if (string.IsNullOrWhiteSpace(annotation))
            {
                return null;
            }

            // Annotation name consists of an at (@) character,
            if (annotation[0] != '@')
            {
                return null;
            }

            string term;
            // followed by the qualified name of a term, optionally followed by a hash (#) and a qualifier
            int index = annotation.IndexOf('#');
            if (index != -1)
            {
                term = annotation.Substring(1, index); // 1 means remove '@'
                qualifier = annotation.Substring(index + 1); // + 1 means remove '#'
            }
            else
            {
                term = annotation.Substring(1); // 1 means remove '@';
            }

            return term;
        }

        /// <summary>
        /// A complex type is represented as a member of the schema object whose name is the unqualified name of the complex type and whose value is an object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="complexObject"></param>
        /// <returns></returns>
        private void BuildStructuredTypeBody(EdmStructuredType structuredType, CsdlJsonSchemaStructuredItem structuredJsonItem)
        {
            // It MAY contain the members $BaseType, $Abstract, and $OpenType.
            // It also MAY contain members representing structural properties and navigation properties as well as annotations.

            IList<IEdmProperty> properties = new List<IEdmProperty>();

            IDictionary<string, IEdmStructuralProperty> declaredStructuralProperties = new Dictionary<string, IEdmStructuralProperty>();
            IJsonValue keyValue = null;
            IJsonPath jsonPath = structuredJsonItem.JsonPath;
            foreach (var member in structuredJsonItem.Members)
            {
                string propertyName = member.Key;
                IJsonValue propertyValue = member.Value;

                if (propertyName == "$Key")
                {
                    keyValue = propertyValue;
                    continue;
                }

                string annotationName;
                propertyName = TryParsePropertyName(propertyName, out annotationName);
                if (annotationName == null)
                {
                    // Without '@' in the name, so it's normal property name
                    // so it's complex property
                    // Property is an object
                    JsonObjectValue objValue = (JsonObjectValue)propertyValue;
                    IJsonValue kindValue;
                    if (objValue.TryGetValue("$Kind", out kindValue))
                    {
                        string kindString = kindValue.ParseAsStringPrimitive(jsonPath);
                        if (kindString == CsdlConstants.Element_NavigationProperty)
                        {
                            IEdmNavigationProperty navProperty = BuildNavigationProperty(propertyName, objValue);
                            properties.Add(navProperty);
                            structuredType.AddProperty(navProperty);
                            break;
                        }
                    }

                    // The property object MAY contain the member $Kind with a string value of Property.
                    // This member SHOULD be omitted to reduce document size.
                    IEdmStructuralProperty structuralProperty = BuildStructuralProperty(structuredType, propertyName, objValue, structuredJsonItem.JsonPath);
                    properties.Add(structuralProperty);
                    structuredType.AddProperty(structuralProperty);

                    declaredStructuralProperties[propertyName] = structuralProperty;
                }
                else
                {
                    //string qualifier;
                    //string termName = TryParseAnnotationName(annotationName, out qualifier);
                    //Debug.Assert(termName != null);
                    //IEdmTerm edmTerm = FindTerm(termName);

                   // IEdmExpression expression = BuildExpression(propertyValue, structuredJsonItem.JsonPath, edmTerm.Type);

                    if (propertyName == null)
                    {
                        // annotation for the structured type
                        //IEdmVocabularyAnnotatable target;
                        //if (structuredType.TypeKind == EdmTypeKind.Complex)
                        //{
                        //    target = (IEdmComplexType)structuredType;
                        //}
                        //else
                        //{
                        //    target = (IEdmEntityType)structuredType;
                        //}
                        //EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(target, edmTerm, expression);

                        //edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);

                        //_edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);
                    }
                    else
                    {
                        //// annotation on annotation
                        //if (propertyName[0] == '@')
                        //{
                        //    // So far, ODL doesn't support annotation on annotation
                        //}
                        //else
                        //{
                        //    // annotation for enum member
                        //    IList<AnnotationWrapper> values;
                        //    if (!propertyAnnotations.TryGetValue(propertyName, out values))
                        //    {
                        //        values = new List<AnnotationWrapper>();
                        //        propertyAnnotations[propertyName] = values;
                        //    }

                        //    values.Add(new AnnotationWrapper
                        //    {
                        //        Term = edmTerm,
                        //        Expression = expression
                        //    });
                        //}
                    }
                }
            }

            //foreach (var item in propertyAnnotations)
            //{
            //    // TODO: maybe refactor later for performance
            //    IEdmProperty edmProperty = properties.FirstOrDefault(c => c.Name == item.Key);
            //    if (edmProperty != null)
            //    {
            //        //foreach (var annotation in item.Value)
            //        //{
            //        //    //EdmVocabularyAnnotation edmVocabularyAnnotation = new EdmVocabularyAnnotation(edmProperty, annotation.Term, annotation.Expression);
            //        //    //edmVocabularyAnnotation.SetSerializationLocation(_edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            //        //    //_edmModel.SetVocabularyAnnotation(edmVocabularyAnnotation);
            //        //}

            //        continue;
            //    }

            //    throw new Exception();
            //}

            if (keyValue != null)
            {
                EdmEntityType entityType = structuredType as EdmEntityType;
                JsonArrayValue keyJsonValue = keyValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);
                IList<string> keys = keyJsonValue.ParseArray<string>(jsonPath, (v, p) => v.ParseAsStringPrimitive(p));
                foreach (var keyStr in keys)
                {
                    IEdmStructuralProperty keyProperty = declaredStructuralProperties[keyStr];
                    entityType.AddKeys(keyProperty);
                }
            }
        }

        private static IEdmNavigationProperty BuildNavigationProperty(string name, IJsonValue jsonValue)
        {
            return null;
        }

        private IEdmStructuralProperty BuildStructuralProperty(EdmStructuredType declaringType, string name, IJsonValue jsonValue, IJsonPath jsonPath)
        {
            // Structural properties are represented as members of the object representing a structured type.
            // The member name is the property name, the member value is an object.
            // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
            // It MAY contain the member $Type, $Collection, $Nullable, $MaxLength, $Unicode, $Precision, $Scale, $SRID, and $DefaultValue.
            string typeName = "Edm.String"; // Absence of the $Type member means the type is Edm.String.
            bool? isCollection = false;
            bool? nullable = false; //  Absence of the member means false.
            int? maxLength = null;
            int? precision = null;
            int? scale = null;
            bool? unicode = null; // Absence of the member means true.
            int? srid = null;
            string defaultValue = null;
            string kind;
            JsonObjectValue propertyObject = jsonValue.ValidateRequiredJsonValue<JsonObjectValue>(jsonPath);
            //IList<CsdlAnnotation> propertyAnnotations = new List<CsdlAnnotation>();
            foreach (var property in propertyObject)
            {
                string propertyName = property.Key;
                IJsonValue propertyValue = property.Value;

                switch (propertyName)
                {
                    case "$Kind":
                        // The property object MAY contain the member $Kind with a string value of Property. This member SHOULD be omitted to reduce document size.
                        kind = propertyValue.ParseAsStringPrimitive(jsonPath);
                        if (kind != "Property")
                        {
                            throw new Exception();
                        }
                        break;

                    case "$Type":
                        // Absence of the $Type member means the type is Edm.String. This member SHOULD be omitted for string properties to reduce document size.
                        typeName = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    case "$Collection":
                        // For collection-valued properties the value of $Type is the qualified name of the property’s item type,
                        // and the member $Collection MUST be present with the literal value true.
                        isCollection = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        // TODO: should verify this value must be true?
                        break;

                    case "$Nullable":
                        // The value of $Nullable is one of the Boolean literals true or false. Absence of the member means false.
                        nullable = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        break;

                    case "$MaxLength":
                        // The value of $MaxLength is a positive integer.
                        // CSDL xml defines a symbolic value max that is only allowed in OData 4.0 responses. This symbolic value is not allowed in CDSL JSON documents at all.
                        maxLength = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Precision":
                        // The value of $Precision is a number.
                        precision = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Scale":
                        // The value of $Scale is a number or a string with one of the symbolic values floating or variable.
                        // Absence of $Scale means variable. However, "floating" and "variable" is not supported.
                        scale = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$Unicode":
                        // The value of $Unicode is one of the Boolean literals true or false. Absence of the member means true.
                        unicode = propertyValue.ParseAsBooleanPrimitive(jsonPath);
                        break;

                    case "$SRID":
                        // The value of $SRID is a string containing a number or the symbolic value variable.
                        // So far, ODL doesn't support string of SRID.
                        srid = propertyValue.ParseAsIntegerPrimitive(jsonPath);
                        break;

                    case "$DefaultValue":
                        // The value of $DefaultValue is the type-specific JSON representation of the default value of the property.
                        // For properties of type Edm.Decimal and Edm.Int64 the representation depends on the media type parameter IEEE754Compatible.
                        // So far, ODL only suppports the string default value.
                        defaultValue = propertyValue.ParseAsStringPrimitive(jsonPath);
                        break;

                    default:
                        //string termName;
                        //propertyName = SeperateAnnotationName(propertyName, out termName);
                        //if (termName != null && propertyName == null)
                        //{
                        //    // annotation for the property
                        //    CsdlAnnotation annotation = BuildCsdlAnnotation(termName, propertyValue);
                        //    propertyAnnotations.Add(annotation);
                        //    break;
                        //}

                        // Without '@' in the name, it's not allowed in Navigation Property object
                        //throw new Exception();
                        break;
                }
            }

            IEdmTypeReference typeReference = BuildTypeReference(typeName, isCollection.Value, nullable.Value, false, maxLength, unicode, precision, scale, srid);

            EdmStructuralProperty edmStructuralProperty = declaringType.AddStructuralProperty(name, typeReference, defaultValue);

            //foreach (var item in propertyAnnotations)
            //{
            //    csdlProperty.AddAnnotation(item);
            //}

            return edmStructuralProperty;
        }

        public IEdmTypeReference BuildTypeReference(string typeString, // if it's collection, it's element type string
            bool isCollection,
            bool isNullable,
            bool isUnbounded,
            int? maxLength,
            bool? unicode,
            int? precision,
            int? scale,
            int? srid)
        {
            IEdmTypeReference type = ParseNamedTypeReference(typeString, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            if (isCollection)
            {
                type = new EdmCollectionTypeReference(new EdmCollectionType(type));
            }

            return type;
        }

        private IEdmTypeReference ParseNamedTypeReference(string typeName, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid)
        {
            IEdmPrimitiveTypeReference primitiveType;
            EdmPrimitiveTypeKind kind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return EdmCoreModel.Instance.GetPrimitive(kind, isNullable);

                case EdmPrimitiveTypeKind.Binary:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmBinaryTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmTemporalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision);

                case EdmPrimitiveTypeKind.Decimal:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmDecimalTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, precision, scale);

                case EdmPrimitiveTypeKind.String:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmStringTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, isUnbounded, maxLength, unicode);

                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    primitiveType = EdmCoreModel.Instance.GetPrimitive(kind, isNullable);
                    return new EdmSpatialTypeReference(primitiveType.PrimitiveDefinition(), primitiveType.IsNullable, srid);

                case EdmPrimitiveTypeKind.None:
                    break;
            }

            EdmPathTypeKind pathTypeKind = EdmCoreModel.Instance.GetPathTypeKind(typeName);
            if (pathTypeKind != EdmPathTypeKind.None)
            {
                return EdmCoreModel.Instance.GetPathType(pathTypeKind, isNullable);
            }

            IEdmSchemaElement schemaElement;
            if (_schemaElements.TryGetValue(typeName, out schemaElement))
            {
                return GetEdmTypeReference(schemaElement as IEdmType, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
            }

            // If we can't find the type, find it from referenced model.
            // IEdmType edmType = _edmModel.FindType(typeName);
            return null;
            //return GetEdmTypeReference(edmType, isNullable, isUnbounded, maxLength, unicode, precision, scale, srid);
        }

        private static IEdmTypeReference GetEdmTypeReference(IEdmType edmType, bool isNullable,
             bool isUnbounded,
             int? maxLength,
             bool? unicode,
             int? precision,
             int? scale,
             int? srid)
        {
            switch (edmType.TypeKind)
            {
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)edmType, isNullable);

                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)edmType, isNullable);

                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference((IEdmEnumType)edmType, isNullable);

                case EdmTypeKind.TypeDefinition:
                    return new EdmTypeDefinitionReference((IEdmTypeDefinition)edmType, isNullable, isUnbounded, maxLength, isUnbounded, precision, scale, srid);
            }

            throw new CsdlParseException();
        }
    }
}
