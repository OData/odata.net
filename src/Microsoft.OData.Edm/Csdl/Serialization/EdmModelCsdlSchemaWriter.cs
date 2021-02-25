//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    /// <summary>
    /// Base class for CSDL & Schema writer.
    /// </summary>
    internal abstract class EdmModelCsdlSchemaWriter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmModelCsdlSchemaWriter"/> class.
        /// </summary>
        /// <param name="model">The Edm model.</param>
        /// <param name="edmVersion">The Edm version.</param>
        internal EdmModelCsdlSchemaWriter(IEdmModel model, Version edmVersion)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(edmVersion, "edmVersion");

            EdmVersion = edmVersion;
            Model = model;
            NamespaceAliasMappings = model.GetNamespaceAliases();
        }

        /// <summary>
        /// Gets the Edm model.
        /// </summary>
        public IEdmModel Model { get; }

        /// <summary>
        /// Gets the Edm version.
        /// </summary>
        public Version EdmVersion { get; }

        /// <summary>
        /// Gets the namespace alias mapping.
        /// </summary>
        public VersioningDictionary<string, string> NamespaceAliasMappings { get; }

        internal abstract void WriteReferenceElementHeader(IEdmReference reference);
        internal abstract void WriteReferenceElementEnd(IEdmReference reference);

        internal abstract void WritIncludeElementHeader(IEdmInclude include);
        internal abstract void WriteIncludeElementEnd(IEdmInclude include);

        internal abstract void WriteTermElementHeader(IEdmTerm term, bool inlineType);

        internal abstract void WriteComplexTypeElementHeader(IEdmComplexType complexType);

        internal abstract void WriteEntityTypeElementHeader(IEdmEntityType entityType);

        internal abstract void WriteEnumTypeElementHeader(IEdmEnumType enumType);

        internal abstract void WriteEnumTypeElementEnd(IEdmEnumType enumType);

        internal abstract void WriteEntityContainerElementHeader(IEdmEntityContainer container);

        internal abstract void WriteEntitySetElementHeader(IEdmEntitySet entitySet);

        internal abstract void WriteSingletonElementHeader(IEdmSingleton singleton);

        internal abstract void WriteDeclaredKeyPropertiesElementHeader();

        internal abstract void WritePropertyRefElement(IEdmStructuralProperty property);

        internal abstract void WriteNavigationPropertyElementHeader(IEdmNavigationProperty property);

        internal abstract void WriteNavigationOnDeleteActionElement(EdmOnDeleteAction operationAction);

        internal abstract void WriteSchemaElementHeader(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings);

        internal abstract void WriteAnnotationsElementHeader(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget);

        internal virtual void WriteOutOfLineAnnotationsBegin(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            // nothing here
        }

        internal virtual void WriteOutOfLineAnnotationsEnd(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            // nothing here
        }

        internal abstract void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType);

        internal abstract void WriteEnumMemberElementHeader(IEdmEnumMember member);

        internal virtual void WriteEnumMemberElementEnd(IEdmEnumMember member)
        {
            // Nothing here
        }

        internal abstract void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding);

        internal virtual void WriteNavigationPropertyBindingsBegin(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            // Nothing here
        }

        internal virtual void WriteNavigationPropertyBindingsEnd(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            // Nothing here
        }

        internal abstract void WriteNullableAttribute(IEdmTypeReference reference);

        internal abstract void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference);

        internal abstract void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference);

        internal abstract void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference);

        internal abstract void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference);

        internal abstract void WriteStringTypeAttributes(IEdmStringTypeReference reference);

        internal abstract void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference);

        internal virtual void WriteReferentialConstraintBegin(IEdmReferentialConstraint referentialConstraint)
        {
            // nothing here
        }

        internal virtual void WriteReferentialConstraintEnd(IEdmReferentialConstraint referentialConstraint)
        {
            // nothing here
        }

        internal abstract void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair);

        internal abstract void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteActionElementHeader(IEdmAction action);

        internal abstract void WriteFunctionElementHeader(IEdmFunction function);

        internal abstract void WriteReturnTypeElementHeader(IEdmOperationReturn operationReturn);

        internal abstract void WriteTypeAttribute(IEdmTypeReference typeReference);

        internal abstract void WriteActionImportElementHeader(IEdmActionImport actionImport);

        internal abstract void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport);

        internal abstract void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType);

        internal abstract void WriteOperationParameterEndElement(IEdmOperationParameter parameter);

        internal abstract void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType);

        internal abstract void WriteInlineExpression(IEdmExpression expression);

        internal abstract void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline);

        internal abstract void WriteVocabularyAnnotationElementEnd(IEdmVocabularyAnnotation annotation, bool isInline);

        internal abstract void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline);

        internal abstract void WriteRecordExpressionElementHeader(IEdmRecordExpression expression);

        internal abstract void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline);

        internal abstract void WritePropertyConstructorElementEnd(IEdmPropertyConstructor constructor);

        internal abstract void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression);

        internal abstract void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression);

        internal abstract void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression);

        internal abstract void WriteNullConstantExpressionElement(IEdmNullExpression expression);

        internal abstract void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression);

        internal virtual void WriteSchemaOperationsHeader<T>(KeyValuePair<string, IList<T>> operations)
        {
            // nothing here
        }

        internal virtual void WriteSchemaOperationsEnd<T>(KeyValuePair<string, IList<T>> operation)
        {
            // nothing here
        }

        internal virtual void WriteOperationParametersBegin(IEnumerable<IEdmOperationParameter> parameters)
        {
            // nothing here
        }

        internal virtual void WriteOperationParametersEnd(IEnumerable<IEdmOperationParameter> parameters)
        {
            // nothing here
        }

        internal abstract void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression);

        internal abstract void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression);

        internal abstract void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression);

        internal abstract void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression);

        internal abstract void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression);

        internal abstract void WriteFunctionApplicationElementEnd(IEdmApplyExpression expression);

        internal abstract void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression);

        internal abstract void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression);

        internal abstract void WritePathExpressionElement(IEdmPathExpression expression);

        internal abstract void WriteAnnotationPathExpressionElement(IEdmPathExpression expression);

        internal abstract void WritePropertyPathExpressionElement(IEdmPathExpression expression);

        internal abstract void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression);

        internal abstract void WriteIfExpressionElementHeader(IEdmIfExpression expression);

        internal abstract void WriteIfExpressionElementEnd(IEdmIfExpression expression);

        internal abstract void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression);

        internal abstract void WriteCollectionExpressionElementEnd(IEdmCollectionExpression expression);

        internal abstract void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement);

        internal abstract void WriteLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression labeledExpressionReference);

        internal abstract void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression);

        internal abstract void WriteIsTypeExpressionElementHeader(IEdmIsTypeExpression expression, bool inlineType);

        internal virtual void WriteIsOfExpressionType(IEdmIsTypeExpression expression, bool inlineType)
        {
            // nothing here
        }

        internal abstract void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType);

        internal abstract void WriteCastExpressionElementEnd(IEdmCastExpression expression, bool inlineType);

        internal virtual void WriteCastExpressionType(IEdmCastExpression expression, bool inlineType)
        {
            // nothing here
        }

        internal abstract void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression);

        internal abstract void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition);

        internal abstract void WriteEndElement();

        internal abstract void WriteArrayEndElement();

        internal abstract void WriteOperationElementAttributes(IEdmOperation operation);

        internal abstract void WriteOperationImportAttributes(IEdmOperationImport operationImport, string operationAttributeName);

        internal static string PathAsXml(IEnumerable<string> path)
        {
            return EdmUtil.JoinInternal("/", path);
        }

        protected static string EnumMemberAsXmlOrJson(IEnumerable<IEdmEnumMember> members)
        {
            string enumTypeName = members.First().DeclaringType.FullName();
            List<string> memberList = new List<string>();
            foreach (var member in members)
            {
                memberList.Add(enumTypeName + "/" + member.Name);
            }

            return string.Join(" ", memberList.ToArray());
        }
    }
}
