//---------------------------------------------------------------------
// <copyright file="ODataClientTemplateUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.OData.Client.Design.T4.UnitTests
{

    [TestClass]
    public class ODataClientTemplateUnitTest
    {
        internal class ODataClientTemplateImp : ODataT4CodeGenerator.ODataClientTemplate
        {
            public List<string> CalledActions { get; private set; }

            public ODataClientTemplateImp(ODataT4CodeGenerator.CodeGenerationContext context)
                : base(context)
            {
                this.CalledActions = new List<string>();
            }

            internal override string GlobalPrefix
            {
                get { return "global::"; }
            }

            internal override string SystemTypeTypeName
            {
                get { return "SystemType"; }
            }

            internal override string AbstractModifier
            {
                get { return "AbstractModifier"; }
            }

            internal override string DataServiceActionQueryTypeName
            {
                get { return "DataServiceActionQueryTypeName"; }
            }

            internal override string DataServiceActionQuerySingleOfTStructureTemplate
            {
                get { return "DataServiceActionQuerySingleOfTStructureTemplate"; }
            }

            internal override string DataServiceActionQueryOfTStructureTemplate
            {
                get { return "DataServiceActionQueryOfTStructureTemplate"; }
            }

            internal override string NotifyPropertyChangedModifier
            {
                get { return "NotifyPropertyChanged"; }
            }

            internal override string ClassInheritMarker
            {
                get { return "ClassInherit"; }
            }

            internal override string ParameterSeparator
            {
                get { return "ParameterSeparator"; }
            }

            internal override string KeyParameterSeparator
            {
                get { return "KeyParameterSeparator"; }
            }

            internal override string KeyDictionaryItemSeparator
            {
                get { return "KeyDictionaryItemSeparator"; }
            }

            internal override string SystemNullableStructureTemplate
            {
                get { return "SystemNullable"; }
            }

            internal override string ICollectionOfTStructureTemplate
            {
                get { return "ICollection`1"; }
            }

            internal override string DataServiceCollectionStructureTemplate
            {
                get { return "DataServiceCollection"; }
            }

            internal override string DataServiceQueryStructureTemplate
            {
                get { return "DataServiceQuery"; }
            }

            internal override string DataServiceQuerySingleStructureTemplate
            {
                get { return "DataServiceQuerySingle"; }
            }

            internal override string ObservableCollectionStructureTemplate
            {
                get { return "ObservableCollection"; }
            }

            internal override string ObjectModelCollectionStructureTemplate
            {
                get { return "ObjectModelCollection"; }
            }

            internal override string DataServiceCollectionConstructorParameters
            {
                get { return "PropertyConstructorParameters"; }
            }

            internal override string NewModifier
            {
                get { return "SystemNew"; }
            }

            internal override string GeoTypeInitializePattern
            {
                get { return "GeoTypeInitializePattern"; }
            }

            internal override string Int32TypeName
            {
                get { return "Int32"; }
            }

            internal override string StringTypeName
            {
                get { return "String"; }
            }

            internal override string BinaryTypeName
            {
                get { return "Binary"; }
            }

            internal override string DecimalTypeName
            {
                get { return "Decimal"; }
            }

            internal override string Int16TypeName
            {
                get { return "Int16"; }
            }

            internal override string SingleTypeName
            {
                get { return "Single"; }
            }

            internal override string BooleanTypeName
            {
                get { return "Boolean"; }
            }

            internal override string DoubleTypeName
            {
                get { return "Double"; }
            }

            internal override string GuidTypeName
            {
                get { return "Guid"; }
            }

            internal override string ByteTypeName
            {
                get { return "Byte"; }
            }

            internal override string Int64TypeName
            {
                get { return "Int64"; }
            }

            internal override string SByteTypeName
            {
                get { return "SByte"; }
            }

            internal override string DataServiceStreamLinkTypeName
            {
                get { return "Stream"; }
            }

            internal override string GeographyTypeName
            {
                get { return "Geography"; }
            }

            internal override string GeographyPointTypeName
            {
                get { return "GeographyPoint"; }
            }

            internal override string GeographyLineStringTypeName
            {
                get { return "GeographyLineString"; }
            }

            internal override string GeographyPolygonTypeName
            {
                get { return "GeographyPolygon"; }
            }

            internal override string GeographyCollectionTypeName
            {
                get { return "GeographyCollection"; }
            }

            internal override string GeographyMultiPolygonTypeName
            {
                get { return "GeographyMultiPolygon"; }
            }

            internal override string GeographyMultiLineStringTypeName
            {
                get { return "GeographyMultiLineString"; }
            }

            internal override string GeographyMultiPointTypeName
            {
                get { return "GeographyMultiPoint"; }
            }

            internal override string GeometryTypeName
            {
                get { return "Geometry"; }
            }

            internal override string GeometryPointTypeName
            {
                get { return "GeometryPoint"; }
            }

            internal override string GeometryLineStringTypeName
            {
                get { return "GeometryLineString"; }
            }

            internal override string GeometryPolygonTypeName
            {
                get { return "GeometryPolygon"; }
            }

            internal override string GeometryCollectionTypeName
            {
                get { return "GeometryCollection"; }
            }

            internal override string GeometryMultiPolygonTypeName
            {
                get { return "GeometryMultiPolygon"; }
            }

            internal override string GeometryMultiLineStringTypeName
            {
                get { return "GeometryMultiLineString"; }
            }

            internal override string GeometryMultiPointTypeName
            {
                get { return "GeometryMultiPoint"; }
            }

            internal override string DateTypeName
            {
                get { return "Date"; }
            }

            internal override string TimeOfDayTypeName
            {
                get { return "TimeOfDay"; }
            }

            internal override string DateTimeOffsetTypeName
            {
                get { return "DateTimeOffset"; }
            }

            internal override string DurationTypeName
            {
                get { return "Duration"; }
            }

            internal override string XmlConvertClassName
            {
                get { return "XmlConvertClassName"; }
            }

            internal override string EnumTypeName
            {
                get { return "EnumTypeName"; }
            }

            internal override HashSet<string> LanguageKeywords
            {
                get
                {
                    return new HashSet<string>(StringComparer.Ordinal)
                    {
                        "abstract", "as", "base", "byte", "bool", "break", "case", "catch", "char", "checked", "class", "const", "continue",
				        "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "for",
				        "foreach", "finally", "fixed", "float", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
			            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
				        "readonly", "ref", "return", "sbyte", "sealed", "string", "short", "sizeof", "stackalloc", "static", "struct", "switch",
				        "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "volatile",
				        "void", "while"
                    };
                }
            }

            internal override string FixPattern
            {
                get { return "@{0}"; }
            }

            internal override string EnumUnderlyingTypeMarker
            {
                get { return " : "; }
            }

            internal override string ConstantExpressionConstructorWithType
            {
                get { return "Expression.Constant({0}, typeof({1}))"; }
            }

            internal override string TypeofFormatter
            {
                get { return "typeof({0})"; }
            }

            internal override string UriOperationParameterConstructor
            {
                get { return "new global::Microsoft.OData.Client.UriOperationParameter(\"{0}\", {1})"; }
            }

            internal override string UriEntityOperationParameterConstructor
            {
                get { return "new global::Microsoft.OData.Client.UriEntityOperationParameter(\"{0}\", {1}, {2})";
                }
            }

            internal override string BodyOperationParameterConstructor
            {
                get { return "new global::Microsoft.OData.Client.BodyOperationParameter(\"{0}\", {1})"; }
            }

            internal override string BaseEntityType
            {
                get { return "global::Microsoft.OData.Client.BaseEntityType"; }
            }

            internal override string OverloadsModifier
            {
                get { return "new "; }
            }

            internal override string ODataVersion
            {
                get { return "global::Microsoft.OData.Core.ODataVersion.V4"; }
            }

            internal override string ParameterDeclarationTemplate
            {
                get { return "ParameterDeclarationTemplate"; }
            }

            internal override string DictionaryItemConstructor
            {
                get { return "DictionaryItemConstructor"; }
            }

            internal override void WriteFileHeader()
            {
                this.CalledActions.Add("WriteFileHeader()");
            }

            internal override void WriteNamespaceStart(string fullNamespace)
            {
                this.CalledActions.Add("WriteNamespaceStart(" + fullNamespace + ")");
            }

            internal override void WriteClassStartForEntityContainer(string originalContainerName, string containerName, string fixedContainerName)
            {

                this.CalledActions.Add("WriteClassStartForEntityContainer(" + originalContainerName + ", " + containerName + ", " + fixedContainerName + ")");
            }

            internal override void WriteMethodStartForEntityContainerConstructor(string containerName, string fixedContainerName)
            {
                this.CalledActions.Add("WriteMethodStartForEntityContainerConstructor(" + containerName + ", " + fixedContainerName + ")");
            }

            internal override void WriteKeyAsSegmentUrlConvention()
            {
                this.CalledActions.Add("WriteKeyAsSegmentUrlConvention()");
            }

            internal override void WriteInitializeResolveName()
            {

                this.CalledActions.Add("WriteInitializeResolveName()");
            }

            internal override void WriteInitializeResolveType()
            {
                this.CalledActions.Add("WriteInitializeResolveType()");
            }

            internal override void WriteClassEndForEntityContainerConstructor()
            {
                this.CalledActions.Add("WriteClassEndForEntityContainerConstructor()");
            }

            internal override void WriteMethodStartForResolveTypeFromName()
            {
                this.CalledActions.Add("WriteMethodStartForResolveTypeFromName()");
            }

            internal override void WriteResolveNamespace(string typeName, string fullNamespace, string languageDependentNamespace)
            {

                this.CalledActions.Add("WriteResolveNamespace(" + typeName + ", " + fullNamespace + ", " + languageDependentNamespace + ")");
            }

            internal override void WriteMethodEndForResolveTypeFromName()
            {
                this.CalledActions.Add("WriteMethodEndForResolveTypeFromName()");
            }

            internal override void WriteMethodStartForResolveNameFromType(string containerName, string fullNamespace)
            {

                this.CalledActions.Add("WriteMethodStartForResolveNameFromType(" + containerName + ", " + fullNamespace + ")");
            }

            internal override void WriteResolveType(string fullNamespace, string languageDependentNamespace)
            {

                this.CalledActions.Add("WriteResolveType(" + fullNamespace + ", " + languageDependentNamespace + ")");
            }

            internal override void WriteMethodEndForResolveNameFromType(bool modelHasInheritance)
            {

                this.CalledActions.Add("WriteMethodEndForResolveNameFromType(" + modelHasInheritance + ")");
            }

            internal override void WriteContextEntitySetProperty(string entitySetName, string entitySetFixedName, string originalEntitySetName, string entitySetElementTypeName, bool inContext = true)
            {
                this.CalledActions.Add("WriteContextEntitySetProperty(" + entitySetName + ", " + entitySetFixedName + ", " + originalEntitySetName + ", " + entitySetElementTypeName + ", " + inContext + ")");
            }

            internal override void WriteContextSingletonProperty(string singletonName, string singletonFixedName, string originalSingletonName, string singletonElementTypeName, bool inContext = true)
            {
                this.CalledActions.Add("WriteContextsSingletonProperty(" + singletonName + ", " + singletonFixedName + ", " + originalSingletonName + ", " + singletonElementTypeName + ", " + inContext + ")");
            }

            internal override void WriteContextAddToEntitySetMethod(string entitySetName, string originalEntitySetName, string typeName, string parameterName)
            {
                this.CalledActions.Add("WriteContextAddToEntitySetMethod(" + entitySetName + ", " + originalEntitySetName + ", " + typeName + ", " + parameterName + ")");
            }

            internal override void WriteGeneratedEdmModel(string escapedEdmxString)
            {
                this.CalledActions.Add("WriteGeneratedEdmModel(" + escapedEdmxString + ")");
            }

            internal override void WriteClassEndForEntityContainer()
            {
                this.CalledActions.Add("WriteClassEndForEntityContainer()");
            }

            internal override void WriteSummaryCommentForStructuredType(string typeName)
            {
                this.CalledActions.Add("WriteSummaryCommentForStructuredType(" + typeName + ")");
            }

            internal override void WriteKeyPropertiesCommentAndAttribute(IEnumerable<string> keyProperties, string keyString)
            {
                this.CalledActions.Add("WriteKeyPropertiesCommentAndAttribute(" + string.Join(", ", keyProperties) + ")");
            }

            internal override void WriteEntityTypeAttribute()
            {
                this.CalledActions.Add("WriteEntityTypeAttribute()");
            }

            internal override void WriteEntitySetAttribute(string entitySetName)
            {
                this.CalledActions.Add("WriteEntitySetAttribute(" + entitySetName + ")");
            }

            internal override void WriteEntityHasStreamAttribute()
            {
                this.CalledActions.Add("WriteEntityHasStreamAttribute()");
            }

            internal override void WriteClassStartForStructuredType(string abstractModifier, string typeName, string originalTypeName, string baseTypeName)
            {
                this.CalledActions.Add("WriteClassStartForStructuredType(" + abstractModifier + ", " + typeName + ", " + originalTypeName + ", " + baseTypeName + ")");
            }

            internal override void WriteSummaryCommentForStaticCreateMethod(string typeName)
            {
                this.CalledActions.Add("WriteSummaryCommentForStaticCreateMethod(" + typeName + ")");
            }

            internal override void WriteParameterCommentForStaticCreateMethod(string parameterName, string propertyName)
            {
                this.CalledActions.Add("WriteParameterCommentForStaticCreateMethod(" + parameterName + ", " + propertyName + ")");
            }

            internal override void WriteDeclarationStartForStaticCreateMethod(string typeName, string fixedTypeName)
            {
                this.CalledActions.Add("WriteDeclarationStartForStaticCreateMethod(" + typeName + ", " + fixedTypeName + ")");
            }

            internal override void WriteParameterForStaticCreateMethod(string parameterTypeName, string parameterName, string parameterSeparater)
            {
                this.CalledActions.Add("WriteParameterForStaticCreateMethod(" + parameterTypeName + ", " + parameterName + ", " + parameterSeparater + ")");
            }

            internal override void WriteDeclarationEndForStaticCreateMethod(string typeName, string instanceName)
            {
                this.CalledActions.Add("WriteDeclarationEndForStaticCreateMethod(" + typeName + ", " + instanceName + ")");
            }

            internal override void WriteParameterNullCheckForStaticCreateMethod(string parameterName)
            {
                this.CalledActions.Add("WriteParameterNullCheckForStaticCreateMethod(" + parameterName + ")");
            }

            internal override void WritePropertyValueAssignmentForStaticCreateMethod(string instanceName, string propertyName, string parameterName)
            {
                this.CalledActions.Add("WritePropertyValueAssignmentForStaticCreateMethod(" + instanceName + ", " + propertyName + ", " + parameterName + ")");
            }

            internal override void WriteMethodEndForStaticCreateMethod(string instanceName)
            {
                this.CalledActions.Add("WriteMethodEndForStaticCreateMethod(" + instanceName + ")");
            }

            internal override void WritePropertyForStructuredType(string propertyType, string originalPropertyName, string propertyName, string fixedPropertyName, string privatePropertyName, string propertyInitializationValue, bool writeOnPropertyChanged)
            {
                this.CalledActions.Add("WritePropertyForStructuredType(" + propertyType + ", " + originalPropertyName + ", " + propertyName + ", " + fixedPropertyName + ", " + privatePropertyName + ", " + propertyInitializationValue + ", " + writeOnPropertyChanged + ")");
            }

            internal override void WriteINotifyPropertyChangedImplementation()
            {
                this.CalledActions.Add("WriteINotifyPropertyChangedImplementation()");
            }

            internal override void WriteClassEndForStructuredType()
            {
                this.CalledActions.Add("WriteClassEndForStructuredType()");
            }

            internal override void WriteNamespaceEnd()
            {
                this.CalledActions.Add("WriteNamespaceEnd()");
            }

            internal override void WriteEnumFlags()
            {
                this.CalledActions.Add("WriteEnumFlags()");
            }

            internal override void WriteSummaryCommentForEnumType(string enumName)
            {
                this.CalledActions.Add("WriteSummaryCommentForEnumType(" + enumName + ")");
            }

            internal override void WriteEnumDeclaration(string enumName, string originalEnumName, string underlyingType)
            {
                this.CalledActions.Add("WriteEnumDeclaration(" + enumName + ", " + originalEnumName + ", " + underlyingType + ")");
            }

            internal override void WriteMemberForEnumType(string member, string originalMemberName, bool last)
            {
                this.CalledActions.Add("WriteMemberForEnumType(" + member + ", " + originalMemberName + ", " + last + ")");
            }

            internal override void WriteEnumEnd()
            {
                this.CalledActions.Add("WriteEnumEnd()");
            }

            internal override void WritePropertyRootNamespace(string containerName, string fullNamespace)
            {
                this.CalledActions.Add("WritePropertyRootNamespace(" + containerName + ", " + fullNamespace + ")");
            }

            internal override void WriteFunctionImportReturnCollectionResult(string functionName, string originalFunctionName, string returnTypeName, string parameters, string parameterValues, bool isComposable, bool useEntityReference)
            {
                this.CalledActions.Add("WriteFunctionImportReturnCollectionResult(" + functionName + ", " + originalFunctionName + ", " + returnTypeName + ", " + parameters + ", " + parameterValues + ", " + isComposable + ", " + useEntityReference + ")");
            }

            internal override void WriteFunctionImportReturnSingleResult(string functionName, string originalFunctionName, string returnTypeName, string returnTypeNameWithSingleSuffix, string parameters, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference)
            {
                this.CalledActions.Add("WriteFunctionImportReturnSingleResult(" + functionName + ", " + originalFunctionName + ", " + returnTypeName + ", " + returnTypeNameWithSingleSuffix + ", " + parameters + ", " + parameterValues + ", " + isComposable + ", " + useEntityReference + ")");
            }

            internal override void WriteBoundFunctionInEntityTypeReturnCollectionResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool useEntityReference)
            {
                this.CalledActions.Add("WriteBoundFunctionInEntityTypeReturnCollectionResult(" + hideBaseMethod + ", " + functionName + ", " + originalFunctionName + ", " + returnTypeName + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ", " + isComposable + ", " + useEntityReference + ")");
            }

            internal override void WriteBoundFunctionInEntityTypeReturnSingleResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string returnTypeNameWithSingleSuffix, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference)
            {
                this.CalledActions.Add("WriteBoundFunctionInEntityTypeReturnSingleResult(" + hideBaseMethod + ", " + functionName + ", " + originalFunctionName + ", " + returnTypeName + ", " + returnTypeNameWithSingleSuffix + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ", " + isComposable + ", " + isReturnEntity + ", " + useEntityReference + ")");
            }

            internal override void WriteActionImport(string actionName, string originalActionName, string returnTypeName, string parameters, string parameterValues)
            {
                this.CalledActions.Add("WriteActionImport(" + actionName + ", " + originalActionName + ", " + returnTypeName + ", " + parameters + ", " + parameterValues + ")");
            }

            internal override void WriteBoundActionInEntityType(bool hideBaseMethod, string actionName, string originalActionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues)
            {
                this.CalledActions.Add("WriteBoundActionInEntityType(" + hideBaseMethod + ", " + actionName + ", " + originalActionName + ", " + returnTypeName + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ")");
            }

            internal override void WriteConstructorForSingleType(string singleTypeName, string baseTypeName)
            {
                this.CalledActions.Add("WriteConstructorForSingleType(" + singleTypeName + ", " + baseTypeName + ")");
            }

            internal override void WriteExtensionMethodsStart()
            {
                this.CalledActions.Add("WriteExtensionMethodsStart()");
            }

            internal override void WriteExtensionMethodsEnd()
            {
                this.CalledActions.Add("WriteExtensionMethodsEnd()");
            }

            internal override void WriteByKeyMethods(string entityTypeName, string returnTypeName, IEnumerable<string> keys, string keyParameters, string keyDictionaryItems)
            {
                this.CalledActions.Add("WriteByKeyMethods(" + entityTypeName + ", " + returnTypeName + ", " + string.Join(", ", keys) + ", " + keyParameters + ", " + keyDictionaryItems + ")");
            }

            internal override void WriteCastToMethods(string baseTypeName, string derivedTypeName, string derivedTypeFullName, string returnTypeName)
            {
                this.CalledActions.Add("WriteCastToMethods(" + baseTypeName + ", " + derivedTypeName + ", " + derivedTypeFullName + ", " + returnTypeName + ")");
            }

            internal override void WriteBoundFunctionReturnSingleResultAsExtension(string functionName, string originalFunctionName,
                string boundTypeName, string returnTypeName, string returnTypeNameWithSingleSuffix, string parameters, string fullNamespace, string parameterValues,
                bool isComposable, bool isReturnEntity, bool useEntityReference)
            {
                this.CalledActions.Add("WriteBoundFunctionInSingleTypeReturnCollectionResult(" + functionName + ", " + originalFunctionName + ", " + boundTypeName + ", " + returnTypeName + ", " + returnTypeNameWithSingleSuffix + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ", " + isComposable + ", " + isReturnEntity + ", " + useEntityReference + ")");
            }

            internal override void WriteBoundFunctionReturnCollectionResultAsExtension(string functionName, string originalFunctionName,
                string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues,
                bool isComposable, bool useEntityReference)
            {
                this.CalledActions.Add("WriteBoundFunctionInSingleTypeReturnCollectionResult(" + functionName + ", " + originalFunctionName + ", " + boundTypeName + ", " + returnTypeName + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ", " + isComposable + ", " + useEntityReference + ")");
            }

            internal override void WriteBoundActionAsExtension(string actionName, string originalActionName, string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues)
            {
                this.CalledActions.Add("WriteBoundActionAsExtension(" + actionName + ", " + originalActionName + ", " + boundTypeName + ", " + returnTypeName + ", " + parameters + ", " + fullNamespace + ", " + parameterValues + ")");
            }
        }

        private static ODataT4CodeGenerator.CodeGenerationContext Context;

        private Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationMap = new Dictionary<IEdmStructuredType, List<IEdmOperation>>();

        private const string OneNamespaceAndEmptyComplexTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EmptyEntityTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"" Abstract=""true"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EmptyEnumTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""EnumType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string FullEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""EnumValue"" Type=""Namespace1.ContentType"" Nullable=""false"" />
      </ComplexType>
      <EnumType Name=""ContentType"" UnderlyingType=""Edm.Int32"" IsFlags=""true"">
          <Member Name=""Liquid""/>
          <Member Name=""Perishable""/>
          <Member Name=""Edible""/>
      </EnumType>
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""Complex"" Type=""Namespace1.ComplexType"" Nullable=""true"" />
        <Property Name=""Contents"" Type=""Collection(Namespace1.ContentType)"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EmptyEntityContainerEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string TwoNamespacesEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
    <Schema Namespace=""Namespace2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string BasicEntityContainerEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType""/>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string ModelHasInheritanceEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""BaseEntityType""/>
      <EntityType Name=""EntityType"" BaseType=""Namespace1.BaseEntityType""/>
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string AbstractComplexTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"" Abstract=""true""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string ComplexTypeWithPropertyEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string ComplexTypeWithPropertiesEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        #region Tests for WriteNamespaces

        [TestMethod]
        public void OnlyOneNamespaceShouldCallWriteNamespaceStartJustOnce()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespaces();
            // Verify WriteNamespaceStart to representing WriteNamepsace
            template.CalledActions.FindAll(act => act == "WriteNamespaceStart(Namespace1)").Count.Should().Be(1);
        }

        [TestMethod]
        public void TwoNamespacesShouldCallWriteNamespaceStartTwice()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(TwoNamespacesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespaces();
            // Verify WriteNamespaceStart representing WriteNamepsace
            template.CalledActions.FindAll(act => act.StartsWith("WriteNamespaceStart")).Count.Should().Be(2);
        }

        #endregion

        #region Tests for WriteNamespace

        private const string SimpleEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType""/>
      <EnumType Name=""EnumType""/>
      <EntityType Name=""EntityType"" Abstract=""true""/>
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void OnlyEntityContainerShouldCallWriteEntityContainer()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespace("Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteNamespaceStart(Namespace1)",
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteClassEndForEntityContainerConstructor()",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityContainer Name=\"\"EntityContainer\"\" />\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()",
                "WriteNamespaceEnd()",
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void OnlyComplexTypeShouldCallWriteComplexType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespace("Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteNamespaceStart(Namespace1)",
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, )",
                "WriteClassEndForStructuredType()",
                "WriteNamespaceEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void OnlyEnumTypeShouldCallWriteEnumType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEnumTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespace("Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteNamespaceStart(Namespace1)",
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumDeclaration(EnumType, EnumType, )",
                "WriteEnumEnd()",
                "WriteNamespaceEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void OnlyEntityTypeShouldCallWriteEntityType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespace("Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteNamespaceStart(Namespace1)",
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)", 
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteClassStartForStructuredType(AbstractModifier, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType)",
                "WriteClassEndForStructuredType()",
                "WriteExtensionMethodsStart()",
                "WriteExtensionMethodsEnd()",
                "WriteNamespaceEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void SimpleEdmxShouldCallMethodsInOrder()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(SimpleEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteNamespace("Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteNamespaceStart(Namespace1)",
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteClassEndForEntityContainerConstructor()",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <ComplexType Name=\"\"ComplexType\"\" />\r\n      <EnumType Name=\"\"EnumType\"\" />\r\n      <EntityType Name=\"\"EntityType\"\" Abstract=\"\"true\"\" />\r\n      <EntityContainer Name=\"\"EntityContainer\"\" />\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()",
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, )",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)", 
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteClassStartForStructuredType(AbstractModifier, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumDeclaration(EnumType, EnumType, )",
                "WriteEnumEnd()",
                "WriteExtensionMethodsStart()",
                "WriteExtensionMethodsEnd()",
                "WriteNamespaceEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteEntityContainer

        private const string EntityContainerUsingDifferentNamespacesEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType""/>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
        <EntitySet Name=""Set2"" EntityType=""Namespace2.EntityType"" />
      </EntityContainer>
    </Schema>
    <Schema Namespace=""Namespace2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""EntityType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteEntityContainerWithOneEntitySet()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(BasicEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainer(container, "Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteClassEndForEntityContainerConstructor()",
                "WriteContextEntitySetProperty(Set1, Set1, Set1, EntityType, True)",
                "WriteContextAddToEntitySetMethod(Set1, Set1, EntityType, entityType)",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n      <EntityContainer Name=\"\"EntityContainer\"\">\r\n        <EntitySet Name=\"\"Set1\"\" EntityType=\"\"Namespace1.EntityType\"\" />\r\n      </EntityContainer>\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerWithTwoEntitySetUsingDifferentNamespaces()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EntityContainerUsingDifferentNamespacesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainer(container, "Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteClassEndForEntityContainerConstructor()",
                "WriteContextEntitySetProperty(Set1, Set1, Set1, EntityType, True)",
                "WriteContextEntitySetProperty(Set2, Set2, Set2, global::Namespace2.EntityType, True)",
                "WriteContextAddToEntitySetMethod(Set1, Set1, EntityType, entityType)",
                "WriteContextAddToEntitySetMethod(Set2, Set2, global::Namespace2.EntityType, entityType)",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n      <EntityContainer Name=\"\"EntityContainer\"\">\r\n        <EntitySet Name=\"\"Set1\"\" EntityType=\"\"Namespace1.EntityType\"\" />\r\n        <EntitySet Name=\"\"Set2\"\" EntityType=\"\"Namespace2.EntityType\"\" />\r\n      </EntityContainer>\r\n    </Schema>\r\n    <Schema Namespace=\"\"Namespace2\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerWithNamesapcePrefixAndOneNamespace()
        {
            string namespacePrefix = "Foo";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(BasicEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainer(container, "Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteInitializeResolveName()",
                "WriteInitializeResolveType()",
                "WriteClassEndForEntityContainerConstructor()",
                "WritePropertyRootNamespace(EntityContainer, Foo)",
                "WriteMethodStartForResolveTypeFromName()",
                "WriteResolveNamespace(SystemType , Namespace1, Foo)",
                "WriteMethodEndForResolveTypeFromName()",
                "WriteMethodStartForResolveNameFromType(EntityContainer, Namespace1)",
                "WriteResolveType(Namespace1, Foo)",
                "WriteMethodEndForResolveNameFromType(False)",
                "WriteContextEntitySetProperty(Set1, Set1, Set1, EntityType, True)",
                "WriteContextAddToEntitySetMethod(Set1, Set1, EntityType, entityType)",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n      <EntityContainer Name=\"\"EntityContainer\"\">\r\n        <EntitySet Name=\"\"Set1\"\" EntityType=\"\"Namespace1.EntityType\"\" />\r\n      </EntityContainer>\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerWithNamesapcePrefixAndTwoNamespaces()
        {
            string namespacePrefix = "Foo";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EntityContainerUsingDifferentNamespacesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainer(container, "Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteClassStartForEntityContainer(EntityContainer, EntityContainer, EntityContainer)",
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteInitializeResolveName()",
                "WriteInitializeResolveType()",
                "WriteClassEndForEntityContainerConstructor()",
                "WritePropertyRootNamespace(EntityContainer, Foo.Namespace1)",
                "WriteMethodStartForResolveTypeFromName()",
                "WriteResolveNamespace(SystemType , Namespace1, Foo.Namespace1)",
                "WriteResolveNamespace(, Namespace2, Foo.Namespace2)",
                "WriteMethodEndForResolveTypeFromName()",
                "WriteMethodStartForResolveNameFromType(EntityContainer, Namespace1)",
                "WriteResolveType(Namespace1, Foo.Namespace1)",
                "WriteResolveType(Namespace2, Foo.Namespace2)",
                "WriteMethodEndForResolveNameFromType(False)",
                "WriteContextEntitySetProperty(Set1, Set1, Set1, EntityType, True)",
                "WriteContextEntitySetProperty(Set2, Set2, Set2, global::Foo.Namespace2.EntityType, True)",
                "WriteContextAddToEntitySetMethod(Set1, Set1, EntityType, entityType)",
                "WriteContextAddToEntitySetMethod(Set2, Set2, global::Foo.Namespace2.EntityType, entityType)",
                "WriteGeneratedEdmModel(<edmx:Edmx Version=\"\"4.0\"\" xmlns:edmx=\"\"http://docs.oasis-open.org/odata/ns/edmx\"\">\r\n  <edmx:DataServices>\r\n    <Schema Namespace=\"\"Namespace1\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n      <EntityContainer Name=\"\"EntityContainer\"\">\r\n        <EntitySet Name=\"\"Set1\"\" EntityType=\"\"Namespace1.EntityType\"\" />\r\n        <EntitySet Name=\"\"Set2\"\" EntityType=\"\"Namespace2.EntityType\"\" />\r\n      </EntityContainer>\r\n    </Schema>\r\n    <Schema Namespace=\"\"Namespace2\"\" xmlns=\"\"http://docs.oasis-open.org/odata/ns/edm\"\">\r\n      <EntityType Name=\"\"EntityType\"\" />\r\n    </Schema>\r\n  </edmx:DataServices>\r\n</edmx:Edmx>)",
                "WriteClassEndForEntityContainer()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteEntityContainerConstructor

        private const string KeyAsSegmentAnnotationEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""EntityContainer""/>
      <Annotations Target=""Namespace1.EntityContainer"">
        <Annotation Term=""Com.Microsoft.OData.Service.Conventions.V1.UrlConventions"" String=""KeyAsSegment"" />
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string KeyAsSegmentAnnotationDefiningDifferentNamespaceEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Annotations Target=""Namespace2.EntityContainer"">
        <Annotation Term=""Com.Microsoft.OData.Service.Conventions.V1.UrlConventions"" String=""KeyAsSegment"" />
      </Annotations>
    </Schema>
    <Schema Namespace=""Namespace2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""EntityContainer""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteEntityContainerConstructorWithoutResolveNameAndType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainerConstructor(container);

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteClassEndForEntityContainerConstructor()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerConstructorWithResolveNameWithoutResolveType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ModelHasInheritanceEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainerConstructor(container);

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteInitializeResolveName()",
                "WriteClassEndForEntityContainerConstructor()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerConstructorWithResolveNameAndResolveType()
        {
            const string namespacePrefix = "NamespacePrefix";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ModelHasInheritanceEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainerConstructor(container);

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteInitializeResolveName()",
                "WriteInitializeResolveType()",
                "WriteClassEndForEntityContainerConstructor()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerConstructorWithKeyAsSegment()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(KeyAsSegmentAnnotationEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainerConstructor(container);

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteKeyAsSegmentUrlConvention()",
                "WriteClassEndForEntityContainerConstructor()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityContainerConstructorWithKeyAsSegmentDefiningDifferentNamespace()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(KeyAsSegmentAnnotationDefiningDifferentNamespaceEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityContainer container = Context.GetSchemaElements("Namespace2").OfType<IEdmEntityContainer>().First();
            template.WriteEntityContainerConstructor(container);

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForEntityContainerConstructor(EntityContainer, EntityContainer)",
                "WriteKeyAsSegmentUrlConvention()",
                "WriteClassEndForEntityContainerConstructor()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteResolveTypeFromName

        [TestMethod]
        public void NoNeedResolveTypeFromNameShouldReturn()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteResolveTypeFromName();
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void NeedResolveTypeFromNameShouldRunInOrder()
        {
            const string namespacePrefix = "NamespacePrefix";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteResolveTypeFromName();

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForResolveTypeFromName()",
                "WriteResolveNamespace(SystemType , Namespace1, NamespacePrefix)",
                "WriteMethodEndForResolveTypeFromName()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void NeedResolveTypeFromNameForTwiceShouldRunInOrder()
        {
            const string namespacePrefix = "NamespacePrefix";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(TwoNamespacesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteResolveTypeFromName();

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForResolveTypeFromName()",
                "WriteResolveNamespace(SystemType , Namespace1, NamespacePrefix.Namespace1)",
                "WriteResolveNamespace(, Namespace2, NamespacePrefix.Namespace2)",
                "WriteMethodEndForResolveTypeFromName()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for NeedResolveNameFromType

        [TestMethod]
        public void NoNeedResolveNameFromTypeShouldReturn()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteResolveNameFromType("EntityContainer", "Namespace1");
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void NeedResolveNameFromTypeShouldRunInOrder()
        {
            const string namespacePrefix = "NamespacePrefix";
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityContainerEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            template.WriteResolveNameFromType("EntityContainer", "Namespace1");

            List<string> expectedActions = new List<string>
            {
                "WriteMethodStartForResolveNameFromType(EntityContainer, Namespace1)",
                "WriteResolveType(Namespace1, NamespacePrefix)",
                "WriteMethodEndForResolveNameFromType(False)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteEntityType

        private const string EntityTypeHasStreamAttributeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"" HasStream=""true""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EntityTypeWithPropertyEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string AbstractEntityTypeWithoutKeyEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""abstractEntityType"" Abstract=""true"">
        <Property Name=""propValue"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string MultipleEntitySetsWithTheSameEntityType = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""CustomerSet1"" EntityType=""Namespace1.Customer"" />
        <EntitySet Name=""CustomerSet2"" EntityType=""Namespace1.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string OneSingleEntitySet = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""CustomerSet1"" EntityType=""Namespace1.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";


        [TestMethod]
        public void WriteAbstractEntityTypeWithoutKey()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(AbstractEntityTypeWithoutKeyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            template.WriteEntityType(entityType, boundOperationMap);

            template.CalledActions.Should().NotContain(act => act.StartsWith("WriteKeyPropertiesCommentAndAttribute"));
        }

        [TestMethod]
        public void WriteEmptyEntityType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)", 
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteClassStartForStructuredType(AbstractModifier, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyEntityTypeWithUseDataServiceCollection()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEntityTypeEdmx, namespacePrefix) { UseDataServiceCollection = true };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)", 
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()", 
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteClassStartForStructuredType(AbstractModifier, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType, NotifyPropertyChanged)",
                "WriteINotifyPropertyChangedImplementation()",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyEntityTypeWithUseDataServiceCollectionAndEntitySet()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(BasicEntityContainerEdmx, namespacePrefix) { UseDataServiceCollection = true };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEnumerable<IEdmEntitySet> entitySets = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First().EntitySets();
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            foreach (var edmEntitySet in entitySets)
            {
                List<IEdmNavigationSource> navigationSourceList = null;
                if (!Context.ElementTypeToNavigationSourceMap.TryGetValue(edmEntitySet.EntityType(), out navigationSourceList))
                {
                    navigationSourceList = new List<IEdmNavigationSource>();
                    Context.ElementTypeToNavigationSourceMap.Add(edmEntitySet.EntityType(), navigationSourceList);
                }

                navigationSourceList.Add(edmEntitySet);
            }

            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)",
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()", 
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteEntitySetAttribute(Set1)",
                "WriteClassStartForStructuredType(, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType, NotifyPropertyChanged)",
                "WriteINotifyPropertyChangedImplementation()",
                "WriteClassEndForStructuredType()"
            };

            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyEntityTypeWithHasStreamAttribute()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EntityTypeHasStreamAttributeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)",
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteEntityHasStreamAttribute()",
                "WriteClassStartForStructuredType(, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyEntityTypeWithBaseType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ModelHasInheritanceEdmx, namespacePrefix) { UseDataServiceCollection = true };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().Last();
            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)",
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteEntityTypeAttribute()",
                "WriteClassStartForStructuredType(, EntityType, EntityType, ClassInheritBaseEntityType)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityTypeWithProperty()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EntityTypeWithPropertyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(EntityTypeSingle)",
                "WriteClassStartForStructuredType(, EntityTypeSingle, EntityTypeSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(EntityTypeSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(EntityType)",
                "WriteKeyPropertiesCommentAndAttribute(Id)",
                "WriteClassStartForStructuredType(, EntityType, EntityType, global::Microsoft.OData.Client.BaseEntityType)",
                "WriteSummaryCommentForStaticCreateMethod(EntityType)",
                "WriteParameterCommentForStaticCreateMethod(ID, Id)",
                "WriteDeclarationStartForStaticCreateMethod(EntityType, EntityType)",
                "WriteParameterForStaticCreateMethod(Guid, ID, )",
                "WriteDeclarationEndForStaticCreateMethod(EntityType, entityType)",
                "WritePropertyValueAssignmentForStaticCreateMethod(entityType, Id, ID)",
                "WriteMethodEndForStaticCreateMethod(entityType)",
                "WritePropertyForStructuredType(Guid, Id, Id, Id, _Id, , False)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityTypeForMoreThanTwoEntitySetsWithTheSameEntityTypeInEntityContainer()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(MultipleEntitySetsWithTheSameEntityType, namespacePrefix) { UseDataServiceCollection = true };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEnumerable<IEdmEntitySet> entitySets = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First().EntitySets();
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            foreach (var edmEntitySet in entitySets)
            {
                List<IEdmNavigationSource> navigationSourceList = null;
                if (!Context.ElementTypeToNavigationSourceMap.TryGetValue(edmEntitySet.EntityType(), out navigationSourceList))
                {
                    navigationSourceList = new List<IEdmNavigationSource>();
                    Context.ElementTypeToNavigationSourceMap.Add(edmEntitySet.EntityType(), navigationSourceList);
                }

                navigationSourceList.Add(edmEntitySet);
            }

            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(CustomerSingle)", 
                "WriteClassStartForStructuredType(, CustomerSingle, CustomerSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(CustomerSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(Customer)",
                "WriteKeyPropertiesCommentAndAttribute(PersonId)",
                "WriteClassStartForStructuredType(, Customer, Customer, global::Microsoft.OData.Client.BaseEntityType, NotifyPropertyChanged)",
                "WriteSummaryCommentForStaticCreateMethod(Customer)",
                "WriteParameterCommentForStaticCreateMethod(personId, PersonId)",
                "WriteDeclarationStartForStaticCreateMethod(Customer, Customer)",
                "WriteParameterForStaticCreateMethod(Int32, personId, )",
                "WriteDeclarationEndForStaticCreateMethod(Customer, customer)",
                "WritePropertyValueAssignmentForStaticCreateMethod(customer, PersonId, personId)",
                "WriteMethodEndForStaticCreateMethod(customer)",
                "WritePropertyForStructuredType(Int32, PersonId, PersonId, PersonId, _PersonId, , True)",
                "WriteINotifyPropertyChangedImplementation()",
                "WriteClassEndForStructuredType()"
            };

            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEntityTypeForOneEntitySetInEntityContainer()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneSingleEntitySet, namespacePrefix) { UseDataServiceCollection = true };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEnumerable<IEdmEntitySet> entitySets = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityContainer>().First().EntitySets();
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().First();
            foreach (var edmEntitySet in entitySets)
            {
                List<IEdmNavigationSource> navigationSourceList = null;
                if (!Context.ElementTypeToNavigationSourceMap.TryGetValue(edmEntitySet.EntityType(), out navigationSourceList))
                {
                    navigationSourceList = new List<IEdmNavigationSource>();
                    Context.ElementTypeToNavigationSourceMap.Add(edmEntitySet.EntityType(), navigationSourceList);
                }

                navigationSourceList.Add(edmEntitySet);
            }

            template.WriteEntityType(entityType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(CustomerSingle)",
                "WriteClassStartForStructuredType(, CustomerSingle, CustomerSingle, ClassInheritDataServiceQuerySingle)",
                "WriteConstructorForSingleType(CustomerSingle, DataServiceQuerySingle)",
                "WriteClassEndForStructuredType()",
                "WriteSummaryCommentForStructuredType(Customer)",
                "WriteKeyPropertiesCommentAndAttribute(PersonId)",
                "WriteEntitySetAttribute(CustomerSet1)",
                "WriteClassStartForStructuredType(, Customer, Customer, global::Microsoft.OData.Client.BaseEntityType, NotifyPropertyChanged)",
                "WriteSummaryCommentForStaticCreateMethod(Customer)",
                "WriteParameterCommentForStaticCreateMethod(personId, PersonId)",
                "WriteDeclarationStartForStaticCreateMethod(Customer, Customer)",
                "WriteParameterForStaticCreateMethod(Int32, personId, )",
                "WriteDeclarationEndForStaticCreateMethod(Customer, customer)",
                "WritePropertyValueAssignmentForStaticCreateMethod(customer, PersonId, personId)",
                "WriteMethodEndForStaticCreateMethod(customer)",
                "WritePropertyForStructuredType(Int32, PersonId, PersonId, PersonId, _PersonId, , True)",
                "WriteINotifyPropertyChangedImplementation()",
                "WriteClassEndForStructuredType()"
            };

            template.CalledActions.Should().Equal(expectedActions);
        }
        #endregion

        #region Tests for WriteComplexType

        private const string ComplexTypeWithBaseType = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""BaseComplexType""/>
      <ComplexType Name=""ComplexType"" BaseType=""Namespace1.BaseComplexType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteEmptyComplexType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteComplexType(complexType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, )",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyComplexTypeWithUseDataServiceCollection()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix)
            {
                UseDataServiceCollection = true
            };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteComplexType(complexType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, ClassInheritNotifyPropertyChanged)",
                "WriteINotifyPropertyChangedImplementation()",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteComplexTypeWithBaseType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithBaseType, namespacePrefix)
            {
                UseDataServiceCollection = true
            };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().Last();
            template.WriteComplexType(complexType, boundOperationMap);


            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, ClassInheritBaseComplexType)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteComplexTypeWithProperty()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteComplexType(complexType, boundOperationMap);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStructuredType(ComplexType)",
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, )",
                "WriteSummaryCommentForStaticCreateMethod(ComplexType)",
                "WriteParameterCommentForStaticCreateMethod(value, Value)",
                "WriteDeclarationStartForStaticCreateMethod(ComplexType, ComplexType)",
                "WriteParameterForStaticCreateMethod(String, value, )",
                "WriteDeclarationEndForStaticCreateMethod(ComplexType, complexType)",
                "WritePropertyValueAssignmentForStaticCreateMethod(complexType, Value, value)",
                "WriteMethodEndForStaticCreateMethod(complexType)",
                "WritePropertyForStructuredType(String, Value, Value, Value, _Value, , False)",
                "WriteClassEndForStructuredType()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteEnumType

        private const string EnumTypeWithFlagsEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""EnumType"" IsFlags=""true""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EnumTypeWithUnderlyingTypeInt32Edmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""EnumType"" UnderlyingType=""Edm.Int32""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EnumTypeWithUnderlyingTypeNotInt32Edmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""EnumType"" UnderlyingType=""Edm.String""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EnumTypeWithMemberEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""ContentType"" UnderlyingType=""Edm.Int32"" IsFlags=""true"">
          <Member Name=""Liquid""/>
      </EnumType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteEmptyEnumType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEnumTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteEnumType(enumType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumDeclaration(EnumType, EnumType, )",
                "WriteEnumEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEmptyEnumTypeWithFlags()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EnumTypeWithFlagsEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteEnumType(enumType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumFlags()",
                "WriteEnumDeclaration(EnumType, EnumType, )",
                "WriteEnumEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEnumTypeWithUnderlyingTypeInt32()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EnumTypeWithUnderlyingTypeInt32Edmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteEnumType(enumType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumDeclaration(EnumType, EnumType, )",
                "WriteEnumEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEnumTypeWithUnderlyingTypeNotInt32()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EnumTypeWithUnderlyingTypeNotInt32Edmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteEnumType(enumType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForEnumType(EnumType)",
                "WriteEnumDeclaration(EnumType, EnumType,  : String)",
                "WriteEnumEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteEnumTypeWithMembers()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EnumTypeWithMemberEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteEnumType(enumType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForEnumType(ContentType)",
                "WriteEnumFlags()",
                "WriteEnumDeclaration(ContentType, ContentType, )",
                "WriteMemberForEnumType(Liquid = 0, Liquid, True)",
                "WriteEnumEnd()"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteStructuredTypeDeclaration

        private const string ComplexTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"" Abstract=""true""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string InheritedComplexTypeEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"" BaseType=""Namespace2.BaseComplexType""/>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteEmptyStructuredType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteStructurdTypeDeclaration(complexType, string.Empty);

            List<string> expectedActions = new List<string>
            {                
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, )"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteAbstractModifier()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(AbstractComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteStructurdTypeDeclaration(complexType, string.Empty);

            List<string> expectedActions = new List<string>
            {                
                "WriteClassStartForStructuredType(AbstractModifier, ComplexType, ComplexType, )"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteNoBaseTypeStructuredTypeWithUseDataServiceCollection()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix)
            {
                UseDataServiceCollection = true
            };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteStructurdTypeDeclaration(complexType, string.Empty);

            List<string> expectedActions = new List<string>
            {                
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, ClassInheritNotifyPropertyChanged)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteStructuredTypeWithSameNamespaceBaseType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ModelHasInheritanceEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEntityType entityType = Context.GetSchemaElements("Namespace1").OfType<IEdmEntityType>().Last();
            template.WriteStructurdTypeDeclaration(entityType, string.Empty);

            List<string> expectedActions = new List<string>
            {                
                "WriteClassStartForStructuredType(, EntityType, EntityType, ClassInheritBaseEntityType)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteStructuredTypeWithDifferentNamespaceBaseType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(InheritedComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteStructurdTypeDeclaration(complexType, string.Empty);

            List<string> expectedActions = new List<string>
            {                
                "WriteClassStartForStructuredType(, ComplexType, ComplexType, ClassInheritglobal::Namespace2.BaseComplexType)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteTypeStaticCreateMethod

        private const string ComplexTypeWithNullablePropertyEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""true"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string ComplexTypeWithCollectionPropertyEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Collection(Edm.String)"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteStaticCreateMethodForAbstractStructuredType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(AbstractComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteStaticCreateMethodForEmptyStructuredType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteStaticCreateMethodForStructuredTypeWithProperty()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStaticCreateMethod(ComplexType)",
                "WriteParameterCommentForStaticCreateMethod(value, Value)",
                "WriteDeclarationStartForStaticCreateMethod(ComplexType, ComplexType)",
                "WriteParameterForStaticCreateMethod(String, value, )",
                "WriteDeclarationEndForStaticCreateMethod(ComplexType, complexType)",
                "WritePropertyValueAssignmentForStaticCreateMethod(complexType, Value, value)",
                "WriteMethodEndForStaticCreateMethod(complexType)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteStaticCreateMethodForStructuredTypeWithProperties()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertiesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);

            List<string> expectedActions = new List<string>
            {
                "WriteSummaryCommentForStaticCreateMethod(ComplexType)",
                "WriteParameterCommentForStaticCreateMethod(name, Name)",
                "WriteParameterCommentForStaticCreateMethod(value, Value)",
                "WriteDeclarationStartForStaticCreateMethod(ComplexType, ComplexType)",
                "WriteParameterForStaticCreateMethod(String, name, , )",
                "WriteParameterForStaticCreateMethod(String, value, )",
                "WriteDeclarationEndForStaticCreateMethod(ComplexType, complexType)",
                "WritePropertyValueAssignmentForStaticCreateMethod(complexType, Name, name)",
                "WritePropertyValueAssignmentForStaticCreateMethod(complexType, Value, value)",
                "WriteMethodEndForStaticCreateMethod(complexType)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteStaticCreateMethodForStructuredTypeWithNullableProperty()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithNullablePropertyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteStaticCreateMethodForStructuredTypeWithCollectionProperty()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithCollectionPropertyEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WriteTypeStaticCreateMethod("ComplexType", complexType);
            template.CalledActions.Should().BeEmpty();
        }

        #endregion

        #region Tests for WriteStaticCreateMethodParameters

        private const string ComplexTypeWithMorethan5PropertiesEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FirstName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""MiddleName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""LastName"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Age"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Gender"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteStaticCreateMethodParametersForEmptyStructuredType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            KeyValuePair<IEdmProperty, string>[] propertyToParameterNamePairs = complexType.Properties().Select(p => new KeyValuePair<IEdmProperty, string>(p, p.Name)).ToArray();
            template.WriteStaticCreateMethodParameters(propertyToParameterNamePairs);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteStaticCreateMethodParametersForStructuredTypeWithProperties()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertiesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            KeyValuePair<IEdmProperty, string>[] propertyToParameterNamePairs = complexType.Properties().Select(p => new KeyValuePair<IEdmProperty, string>(p, p.Name)).ToArray();
            template.WriteStaticCreateMethodParameters(propertyToParameterNamePairs);

            List<string> expectedActions = new List<string>
            {
                "WriteParameterForStaticCreateMethod(String, Name, , )",
                "WriteParameterForStaticCreateMethod(String, Value, )"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WriteStaticCreateMethodParametersForStructuredTypeWithMoreThan5Properties()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithMorethan5PropertiesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            KeyValuePair<IEdmProperty, string>[] propertyToParameterNamePairs = complexType.Properties().Select(p => new KeyValuePair<IEdmProperty, string>(p, p.Name)).ToArray();
            template.WriteStaticCreateMethodParameters(propertyToParameterNamePairs);

            List<string> expectedActions = new List<string>
            {
                "WriteParameterForStaticCreateMethod(Int32, Id, ParameterSeparator)",
                "WriteParameterForStaticCreateMethod(String, FirstName, ParameterSeparator)",
                "WriteParameterForStaticCreateMethod(String, MiddleName, ParameterSeparator)",
                "WriteParameterForStaticCreateMethod(String, LastName, ParameterSeparator)",
                "WriteParameterForStaticCreateMethod(Int32, Age, ParameterSeparator)",
                "WriteParameterForStaticCreateMethod(String, Gender, )"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WritePropertiesForStructuredType

        [TestMethod]
        public void WritePropertiesForEmptyStructuredType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(OneNamespaceAndEmptyComplexTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WritePropertiesForStructuredTypeWithProperties()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertiesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            List<string> expectedActions = new List<string>
            {
                "WritePropertyForStructuredType(String, Name, Name, Name, _Name, , False)",
                "WritePropertyForStructuredType(String, Value, Value, Value, _Value, , False)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        [TestMethod]
        public void WritePropertiesForStructuredTypeWithPropertyAndUseDataServiceCollection()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(ComplexTypeWithPropertyEdmx, namespacePrefix)
            {
                UseDataServiceCollection = true
            };
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            List<string> expectedActions = new List<string>
            {
                "WritePropertyForStructuredType(String, Value, Value, Value, _Value, , True)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for WriteMembersForEnumType

        private const string EnumTypeWithMembersEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType Name=""ContentType"" UnderlyingType=""Edm.Int32"" IsFlags=""true"">
          <Member Name=""Liquid""/>
          <Member Name=""Perishable""/>
      </EnumType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteMembersForEmptyEnumType()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EmptyEnumTypeEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteMembersForEnumType(enumType.Members);
            template.CalledActions.Should().BeEmpty();
        }

        [TestMethod]
        public void WriteMembersForEnumTypeWithMembers()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(EnumTypeWithMembersEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmEnumType enumType = Context.GetSchemaElements("Namespace1").OfType<IEdmEnumType>().First();
            template.WriteMembersForEnumType(enumType.Members);

            List<string> expectedActions = new List<string>
            {
                "WriteMemberForEnumType(Liquid = 0, Liquid, False)",
                "WriteMemberForEnumType(Perishable = 1, Perishable, True)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }

        #endregion

        #region Tests for prefix conflict

        private const string PrefixConflictEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
       <ComplexType Name=""ComplexType"">
         <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
         <Property Name=""_Name"" Type=""Edm.String"" Nullable=""false"" />
         <Property Name=""__Name"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WritePrefixConfict()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(PrefixConflictEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            List<string> expectedActions = new List<string>
            {
                "WritePropertyForStructuredType(String, Name, Name, Name, _Name1, , False)",
                "WritePropertyForStructuredType(String, _Name, _Name, _Name, __Name1, , False)",
                "WritePropertyForStructuredType(String, __Name, __Name, __Name, ___Name, , False)"
            };
            template.CalledActions.Should().Equal(expectedActions);
        }
        #endregion

        #region Tests for Dup Names

        private const string DupNamesEdmx = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
       <ComplexType Name=""Name"">
         <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
         <Property Name=""name"" Type=""Edm.String"" Nullable=""false"" />
         <Property Name=""Name1"" Type=""Edm.String"" Nullable=""false"" />
         <Property Name=""_Name2"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void WriteDupNames()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(DupNamesEdmx, namespacePrefix);
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.SetPropertyIdentifierMappingsIfNameConflicts(complexType.Name, complexType);
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            List<string> expectedActions = new List<string>
            {
                "WritePropertyForStructuredType(String, Name, Name2, Name2, _Name21, , False)",
                "WritePropertyForStructuredType(String, name, name, name, _name, , False)",
                "WritePropertyForStructuredType(String, Name1, Name1, Name1, _Name1, , False)",
                "WritePropertyForStructuredType(String, _Name2, _Name2, _Name2, __Name2, , False)",
            };
            template.CalledActions.Should().Contain(expectedActions);
        }

        [TestMethod]
        public void WriteDupNamesWithCamelCase()
        {
            string namespacePrefix = string.Empty;
            Context = new ODataT4CodeGenerator.CodeGenerationContext(DupNamesEdmx, namespacePrefix);
            Context.EnableNamingAlias = true;
            ODataClientTemplateImp template = new ODataClientTemplateImp(Context);
            IEdmComplexType complexType = Context.GetSchemaElements("Namespace1").OfType<IEdmComplexType>().First();
            template.SetPropertyIdentifierMappingsIfNameConflicts(complexType.Name, complexType);
            template.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            List<string> expectedActions = new List<string>
            {
                "WritePropertyForStructuredType(String, Name, Name2, Name2, _Name21, , False)",
                "WritePropertyForStructuredType(String, name, Name3, Name3, _Name3, , False)",
                "WritePropertyForStructuredType(String, Name1, Name1, Name1, _Name1, , False)",
                "WritePropertyForStructuredType(String, _Name2, _Name2, _Name2, __Name2, , False)",
            };
            template.CalledActions.Should().Contain(expectedActions);
        }

        #endregion

        [TestMethod]
        public void GetFixedNameShouldReadNonKeywords()
        {
            ODataClientTemplateImp template = new ODataClientTemplateImp(new ODataT4CodeGenerator.CodeGenerationContext(FullEdmx, null));
            template.GetFixedName("Name").Should().Be("Name");
        }

        [TestMethod]
        public void FixParameterNameShouldReadKeywords()
        {
            ODataClientTemplateImp template = new ODataClientTemplateImp(new ODataT4CodeGenerator.CodeGenerationContext(FullEdmx, null));
            template.GetFixedName("bool").Should().Be("@bool");
        }
    }
}
