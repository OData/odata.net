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
using Microsoft.OData.Edm.Csdl.Json;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// Provides CSDL-JSON parsing services for EDM models.
    /// </summary>
    internal class EdmTypeJsonBuilder
    {
        private IDictionary<SchemaJsonItem, CsdlJsonModel> _schemaItemsToModelMapping;

        private IDictionary<string, SchemaJsonItem> _schemaItems = new Dictionary<string, SchemaJsonItem>();

        private readonly IDictionary<string, IEdmSchemaElement> _schemaElements = new Dictionary<string, IEdmSchemaElement>();

        private CsdlSerializerOptions _options;
        internal EdmTypeJsonBuilder(IDictionary<SchemaJsonItem, CsdlJsonModel> sschemaItemsToModelMapping, CsdlSerializerOptions options)
        {
            _options = options;
            _schemaItemsToModelMapping = sschemaItemsToModelMapping;

            sschemaItemsToModelMapping.ForEach(k => _schemaItems[k.Key.FullName] = k.Key);
        }

        internal string ReplaceAlias(SchemaJsonItem jsonItem, string name)
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
            foreach (var termJsonItem in _schemaElements.OfType<TermJsonItem>())
            {
                BuildTermType(termJsonItem);
            }

            foreach (var item in _schemaItems)
            {
                CreateEdmTypeBody(item.Value);
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

        private SchemaJsonItem FindBaseItem(string fullQualifiedName)
        {
            SchemaJsonItem baseItem;
            _schemaItems.TryGetValue(fullQualifiedName, out baseItem);

            //
            return baseItem;
        }

        private void BuildSchemaElementHeader(SchemaJsonItem schemaItem)
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
                    ComplexTypeJsonItem complex = (ComplexTypeJsonItem)schemaItem;
                    IEdmComplexType baseComplexType = null;
                    if (complex.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(complex, complex.BaseType);

                        SchemaJsonItem baseItem = FindBaseItem(replacedBaseTypeName);

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
                    EntityTypeJsonItem entity = (EntityTypeJsonItem)schemaItem;

                    IEdmEntityType baseEntityType = null;
                    if (entity.BaseType != null)
                    {
                        string replacedBaseTypeName = ReplaceAlias(entity, entity.BaseType);

                        SchemaJsonItem baseItem = FindBaseItem(replacedBaseTypeName);

                        BuildSchemaElementHeader(baseItem);
                        baseEntityType = GetSchemaElement(replacedBaseTypeName) as IEdmEntityType;

                        Contract.Assert(baseEntityType != null);
                    }

                    EdmEntityType entityType = new EdmEntityType(schemaItem.Namespace, schemaItem.Name, baseEntityType,
                        entity.IsAbstract, entity.IsOpen, entity.HasStream);
                    _schemaElements.Add(schemaItem.FullName, entityType);

                    break;

                case SchemaMemberKind.Enum:
                    EnumTypeJsonItem enumItem = (EnumTypeJsonItem)schemaItem;

                    EdmEnumType enumType = new EdmEnumType(enumItem.Namespace, enumItem.Name,
                            GetUnderlyingType(enumItem.UnderlyingTypeName), enumItem.IsFlags);

                    _schemaElements.Add(enumItem.FullName, enumType);

                    break;

                case SchemaMemberKind.TypeDefinition:
                    TypeDefinitionJsonItem typeDefinitionItem = (TypeDefinitionJsonItem)schemaItem;

                    EdmTypeDefinition typeDefinition = new EdmTypeDefinition(typeDefinitionItem.Namespace, typeDefinitionItem.Name,
                        GetUnderlyingType(typeDefinitionItem.UnderlyingTypeName));

                    _schemaElements.Add(typeDefinitionItem.FullName, typeDefinition);

                    break;

                case SchemaMemberKind.EntityContainer:
                    EntityContainerJsonItem entityContainerItem = (EntityContainerJsonItem)schemaItem;

                    EdmEntityContainer edmEntityContainer = new EdmEntityContainer(entityContainerItem.Namespace, entityContainerItem.Name);
                    _schemaElements.Add(entityContainerItem.FullName, edmEntityContainer);

                    break;

                case SchemaMemberKind.Action:
                case SchemaMemberKind.Function:
                case SchemaMemberKind.Term:
                    // Don't build action, function, term until all types are built
                    break;
            }
        }

        private void BuildTermType(TermJsonItem termItem)
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

            EdmTerm edmTerm = new EdmTerm(termItem.Namespace, termItem.Name, termType, termItem.AppliesTo, termItem.DefaultValue);

            _schemaElements.Add(termItem.FullName, edmTerm);
        }

        

        private static IEdmPrimitiveType GetUnderlyingType(string underlyingType)
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

        private void CreateEdmTypeBody(SchemaJsonItem item)
        {
            IEdmSchemaElement schemaElement = GetSchemaElement(item.FullName);

            switch (item.Kind)
            {
                case SchemaMemberKind.Complex:
                    CreateComplexTypeBody((EdmComplexType)schemaElement, (ComplexTypeJsonItem)item);
                    break;

                case SchemaMemberKind.Entity:
                    CreateEntityTypeBody((EdmEntityType)schemaElement, (EntityTypeJsonItem)item);
                    break;

                case SchemaMemberKind.Enum:
                    CreateEnumTypeBody((EdmEnumType)schemaElement, (EnumTypeJsonItem)item);
                    break;

                case SchemaMemberKind.TypeDefinition:
                    break;

                case SchemaMemberKind.Action:
                    break;

                case SchemaMemberKind.Function:
                    break;

                case SchemaMemberKind.Term:
                    // The body of term may contain the annotation.
                    break;

                case SchemaMemberKind.EntityContainer:
                    BuildEntityContainerBody((EdmEntityContainer)schemaElement, (EntityContainerJsonItem)item);
                    break;
            }
        }

        private void BuildEntityContainerBody(EdmEntityContainer edmEntityContainer, EntityContainerJsonItem entityContainerJsonItem)
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

                    return;
                }

                if (TryBuildOperationImport(edmEntityContainer, name, member.Value))
                {
                    return;
                }

                if (TryBuildNavigationSource(edmEntityContainer, name, member.Value))
                {
                    return;
                }
            }
        }

        public static bool TryBuildOperationImport(EdmEntityContainer edmEntityContainer, string name, IJsonValue jsonValue)
        {
            return false;
        }

        public bool TryBuildNavigationSource(EdmEntityContainer edmEntityContainer, string name, IJsonValue jsonValue)
        {
            if (jsonValue.ValueKind != JsonValueKind.JObject)
            {
                return false;
            }

            JsonObjectValue objValue = (JsonObjectValue)jsonValue;

         //   IList<CsdlNavigationPropertyBinding> navigationPropertyBindings = new List<CsdlNavigationPropertyBinding>();
         //   IList<CsdlAnnotation> annotations = new List<CsdlAnnotation>();
            string type = null;
            bool? isCollection = null;
            bool? includeInServiceDocument = null;
            bool? nullable = null;
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
                        nullable = propertyValue.ParseAsBooleanPrimitive();
                        break;

                    case "$NavigationPropertyBinding":
                        // The value of $NavigationPropertyBinding is an object.
               //         navigationPropertyBindings = BuildCsdlNavigationPropertyBinding(propertyValue);
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

            if (type == null)
            {
                return false;
            }

            IEdmEntityType entityType = GetSchemaElement(type) as IEdmEntityType;
         //   IEdmNavigationSource navigationSource = null;
            if (isCollection != null)
            {
                // entitySet
                if (!isCollection.Value)
                {
                    // If presented, $IsCollection should be true
                    throw new Exception();
                }

                EdmEntitySet entitySet = new EdmEntitySet(edmEntityContainer, name, entityType, includeInServiceDocument.Value);
            //    entitySet.
                edmEntityContainer.AddElement(entitySet);
            }
            else
            {
                // singleton
                EdmSingleton singleton = new EdmSingleton(edmEntityContainer, name, entityType);
                edmEntityContainer.AddElement(singleton);
            }

            return true;
        }

        private void CreateComplexTypeBody(EdmComplexType type, ComplexTypeJsonItem complexJsonItem)
        {
            Contract.Assert(type != null);
            Contract.Assert(complexJsonItem != null);

            BuildStructuredTypeBody(type, complexJsonItem);
        }

        private void CreateEntityTypeBody(EdmEntityType type, EntityTypeJsonItem entityJsonItem)
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

        private void CreateEnumTypeBody(EdmEnumType enumType, EnumTypeJsonItem enumJsonItem)
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
            _schemaElements.TryGetValue(qualifiedName, out schemaElement);

            return schemaElement as IEdmTerm;
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
        private void BuildStructuredTypeBody(EdmStructuredType structuredType, StructuredTypeJsonItem structuredJsonItem)
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
