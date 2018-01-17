//---------------------------------------------------------------------
// <copyright file="CsdlConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Constants for CSDL XML.
    /// </summary>
    public static class CsdlConstants
    {
        #region Public Constants

        #region Old EDMX namespaces

        /// <summary>
        /// Version 4.0 of EDMX. Corresponds to EDMX namespace "http://docs.oasis-open.org/odata/ns/edmx".
        /// </summary>
        public static readonly Version EdmxVersion4 = EdmConstants.EdmVersion4;

        #endregion

        /// <summary>
        /// The current latest version of EDMX.
        /// </summary>
        public static readonly Version EdmxVersionLatest = EdmxVersion4;

        #endregion

        #region CSDL
        internal const string CsdlFileExtension = ".csdl";

        internal const string EdmOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";

        internal const string SchemaNamespaceAnnotation = "SchemaNamespace";
        internal const string AnnotationSerializationLocationAnnotation = "AnnotationSerializationLocation";
        internal const string NamespacePrefixAnnotation = "NamespacePrefix";
        internal const string IsEnumMemberValueExplicitAnnotation = "IsEnumMemberValueExplicit";
        internal const string IsSerializedAsElementAnnotation = "IsSerializedAsElement";
        internal const string NamespaceAliasAnnotation = "NamespaceAlias";
        internal const string UsedNamespacesAnnotation = "UsedNamespaces";
        internal const string ReferencesAnnotation = "References";
        internal const string PrimitiveValueConverterMapAnnotation = "PrimitiveValueConverterMap";

        internal const string Attribute_Abstract = "Abstract";
        internal const string Attribute_Action = "Action";
        internal const string Attribute_Alias = "Alias";
        internal const string Attribute_AnnotationPath = "AnnotationPath";
        internal const string Attribute_AppliesTo = "AppliesTo";
        internal const string Attribute_BaseType = "BaseType";
        internal const string Attribute_Binary = "Binary";
        internal const string Attribute_Bool = "Bool";
        internal const string Attribute_ContainsTarget = "ContainsTarget";
        internal const string Attribute_Date = "Date";
        internal const string Attribute_DateTimeOffset = "DateTimeOffset";
        internal const string Attribute_Decimal = "Decimal";
        internal const string Attribute_DefaultValue = "DefaultValue";
        internal const string Attribute_ElementType = "ElementType";
        internal const string Attribute_Extends = "Extends";
        internal const string Attribute_EntityType = "EntityType";
        internal const string Attribute_EntitySet = "EntitySet";
        internal const string Attribute_EntitySetPath = "EntitySetPath";
        internal const string Attribute_EnumMember = "EnumMember";
        internal const string Attribute_Float = "Float";
        internal const string Attribute_Function = "Function";
        internal const string Attribute_Guid = "Guid";
        internal const string Attribute_HasStream = "HasStream";
        internal const string Attribute_Int = "Int";
        internal const string Attribute_IncludeInServiceDocument = "IncludeInServiceDocument";
        internal const string Attribute_IsBound = "IsBound";
        internal const string Attribute_IsComposable = "IsComposable";
        internal const string Attribute_IsFlags = "IsFlags";
        internal const string Attribute_MaxLength = EdmConstants.FacetName_MaxLength;
        internal const string Attribute_Name = "Name";
        internal const string Attribute_Namespace = "Namespace";
        internal const string Attribute_NavigationPropertyPath = "NavigationPropertyPath";
        internal const string Attribute_Nullable = EdmConstants.FacetName_Nullable;
        internal const string Attribute_OpenType = "OpenType";
        internal const string Attribute_Partner = "Partner";
        internal const string Attribute_Path = "Path";
        internal const string Attribute_Precision = EdmConstants.FacetName_Precision;
        internal const string Attribute_Property = "Property";
        internal const string Attribute_PropertyPath = "PropertyPath";
        internal const string Attribute_ReferencedProperty = "ReferencedProperty";
        internal const string Attribute_Qualifier = "Qualifier";
        internal const string Attribute_Scale = EdmConstants.FacetName_Scale;
        internal const string Attribute_Srid = EdmConstants.FacetName_Srid;
        internal const string Attribute_String = "String";
        internal const string Attribute_Target = "Target";
        internal const string Attribute_Term = "Term";
        internal const string Attribute_Duration = "Duration";
        internal const string Attribute_TimeOfDay = "TimeOfDay";
        internal const string Attribute_Type = "Type";
        internal const string Attribute_UnderlyingType = "UnderlyingType";
        internal const string Attribute_Unicode = "Unicode";
        internal const string Attribute_Value = "Value";

        internal const string Element_Action = "Action";
        internal const string Element_ActionImport = "ActionImport";
        internal const string Element_Annotation = "Annotation";
        internal const string Element_Annotations = "Annotations";
        internal const string Element_Apply = "Apply";
        internal const string Element_Binary = "Binary";
        internal const string Element_Bool = "Bool";
        internal const string Element_Cast = "Cast";
        internal const string Element_Collection = "Collection";
        internal const string Element_CollectionType = "CollectionType";
        internal const string Element_ComplexType = "ComplexType";
        internal const string Element_Date = "Date";
        internal const string Element_DateTimeOffset = "DateTimeOffset";
        internal const string Element_Decimal = "Decimal";
        internal const string Element_Documentation = "Documentation";
        internal const string Element_EntityContainer = "EntityContainer";
        internal const string Element_EntitySet = "EntitySet";
        internal const string Element_EntitySetReference = "EntitySetReference";
        internal const string Element_EntityType = "EntityType";
        internal const string Element_EnumMember = "EnumMember";
        internal const string Element_EnumType = "EnumType";
        internal const string Element_Float = "Float";
        internal const string Element_Guid = "Guid";
        internal const string Element_Function = "Function";
        internal const string Element_FunctionImport = "FunctionImport";
        internal const string Element_FunctionReference = "FunctionReference";
        internal const string Element_If = "If";
        internal const string Element_IsType = "IsType";
        internal const string Element_Int = "Int";
        internal const string Element_Key = "Key";
        internal const string Element_LabeledElement = "LabeledElement";
        internal const string Element_LabeledElementReference = "LabeledElementReference";
        internal const string Element_LongDescription = "LongDescription";
        internal const string Element_Member = "Member";
        internal const string Element_NavigationProperty = "NavigationProperty";
        internal const string Element_NavigationPropertyBinding = "NavigationPropertyBinding";
        internal const string Element_NavigationPropertyPath = "NavigationPropertyPath";
        internal const string Element_Null = "Null";
        internal const string Element_OnDelete = "OnDelete";
        internal const string Element_Parameter = "Parameter";
        internal const string Element_ParameterReference = "ParameterReference";
        internal const string Element_Path = "Path";
        internal const string Element_Property = "Property";
        internal const string Element_PropertyPath = "PropertyPath";
        internal const string Element_PropertyRef = "PropertyRef";
        internal const string Element_PropertyReference = "PropertyReference";
        internal const string Element_PropertyValue = "PropertyValue";
        internal const string Element_Record = "Record";
        internal const string Element_ReferenceType = "ReferenceType";
        internal const string Element_ReferentialConstraint = "ReferentialConstraint";
        internal const string Element_ReturnType = "ReturnType";
        internal const string Element_Singleton = "Singleton";
        internal const string Element_Schema = "Schema";
        internal const string Element_String = "String";
        internal const string Element_Summary = "Summary";
        internal const string Element_Duration = "Duration";
        internal const string Element_Term = "Term";
        internal const string Element_TimeOfDay = "TimeOfDay";
        internal const string Element_TypeDefinition = "TypeDefinition";
        internal const string Element_TypeRef = "TypeRef";

        internal const string Value_Cascade = "Cascade";
        internal const string Value_Collection = "Collection";
        internal const string Value_EndMany = "*";
        internal const string Value_EndOptional = "0..1";
        internal const string Value_EndRequired = "1";
        internal const string Value_Max = EdmConstants.Value_Max;
        internal const string Value_None = "None";
        internal const string Value_Ref = "Ref";
        internal const string Value_SridVariable = EdmConstants.Value_SridVariable;
        internal const string Value_ScaleVariable = EdmConstants.Value_ScaleVariable;

        internal const string TypeName_Untyped = "Edm.Untyped";
        internal const string TypeName_Untyped_Short = "Untyped";

        internal const bool Default_Abstract = false;
        internal const bool Default_ContainsTarget = false;
        internal const bool Default_HasStream = false;
        internal const bool Default_IncludeInServiceDocument = false;
        internal const bool Default_IsAtomic = false;
        internal const bool Default_IsBound = false;
        internal const bool Default_IsComposable = false;
        internal const bool Default_IsFlags = false;
        internal const bool Default_OpenType = false;
        internal const bool Default_Nullable = true;
        internal const bool Default_IsUnicode = true;
        internal const int Default_TemporalPrecision = 0;
        internal const int Default_SpatialGeographySrid = 4326;
        internal const int Default_SpatialGeometrySrid = 0;
        internal const int Default_UnspecifiedSrid = Int32.MinValue;
        internal const int Default_Scale = 0;

        internal const int Max_NameLength = 480;
        internal const int Max_NamespaceLength = 512;

        #endregion

        #region EDMX

        internal const string EdmxFileExtension = ".edmx";

        internal const string EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

        internal const string ODataMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>
        /// The local name of the annotation that stores EDMX version of a model.
        /// </summary>
        internal const string EdmxVersionAnnotation = "EdmxVersion";

        internal const string Prefix_Edmx = "edmx";
        internal const string Prefix_ODataMetadata = "m";

        internal const string Attribute_TargetNamespace = "TargetNamespace";
        internal const string Attribute_TermNamespace = "TermNamespace";
        internal const string Attribute_Version = "Version";
        internal const string Attribute_Uri = "Uri";

        internal const string Element_ConceptualModels = "ConceptualModels";
        internal const string Element_Edmx = "Edmx";
        internal const string Element_Runtime = "Runtime";
        internal const string Element_Reference = "Reference";  // e.g. <edmx:Reference Uri="http://vocabs.odata.org/capabilities/v1">
        internal const string Element_Include = "Include";
        internal const string Element_IncludeAnnotations = "IncludeAnnotations";
        internal const string Element_DataServices = "DataServices";

        #endregion

        internal static Dictionary<Version, string[]> SupportedVersions = new Dictionary<Version, string[]>()
        {
            { EdmConstants.EdmVersion4, new string[] { EdmOasisNamespace } },
        };

        internal static Dictionary<Version, string> SupportedEdmxVersions = new Dictionary<Version, string>
        {
            { EdmxVersion4, EdmxOasisNamespace }
        };

        internal static Dictionary<string, Version> SupportedEdmxNamespaces = SupportedEdmxVersions.ToDictionary(v => v.Value, v => v.Key);

        internal static Dictionary<Version, Version> EdmToEdmxVersions = new Dictionary<Version, Version>
        {
            { EdmConstants.EdmVersion4, EdmxVersion4 }
        };
    }
}
