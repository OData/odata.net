//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSchemaWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        internal abstract Task WriteReferenceElementHeaderAsync(IEdmReference reference);
        internal abstract void WriteReferenceElementEnd(IEdmReference reference);
        internal abstract Task WriteReferenceElementEndAsync(IEdmReference reference);

        internal abstract void WritIncludeElementHeader(IEdmInclude include);
        internal abstract Task WritIncludeElementHeaderAsync(IEdmInclude include);
        internal abstract void WriteIncludeElementEnd(IEdmInclude include);
        internal abstract Task WriteIncludeElementEndAsync(IEdmInclude include);

        internal abstract void WriteTermElementHeader(IEdmTerm term, bool inlineType);
        internal abstract Task WriteTermElementHeaderAsync(IEdmTerm term, bool inlineType);

        internal abstract void WriteComplexTypeElementHeader(IEdmComplexType complexType);
        internal abstract Task WriteComplexTypeElementHeaderAsync(IEdmComplexType complexType);

        internal abstract void WriteEntityTypeElementHeader(IEdmEntityType entityType);
        internal abstract Task WriteEntityTypeElementHeaderAsync(IEdmEntityType entityType);

        internal abstract void WriteEnumTypeElementHeader(IEdmEnumType enumType);
        internal abstract Task WriteEnumTypeElementHeaderAsync(IEdmEnumType enumType);

        internal abstract void WriteEnumTypeElementEnd(IEdmEnumType enumType);
        internal abstract Task WriteEnumTypeElementEndAsync(IEdmEnumType enumType);

        internal abstract void WriteEntityContainerElementHeader(IEdmEntityContainer container);
        internal abstract Task WriteEntityContainerElementHeaderAsync(IEdmEntityContainer container);

        internal abstract void WriteEntitySetElementHeader(IEdmEntitySet entitySet);
        internal abstract Task WriteEntitySetElementHeaderAsync(IEdmEntitySet entitySet);

        internal abstract void WriteSingletonElementHeader(IEdmSingleton singleton);
        internal abstract Task WriteSingletonElementHeaderAsync(IEdmSingleton singleton);

        internal abstract void WriteDeclaredKeyPropertiesElementHeader();
        internal abstract Task WriteDeclaredKeyPropertiesElementHeaderAsync();

        internal abstract void WritePropertyRefElement(IEdmStructuralProperty property);
        internal abstract Task WritePropertyRefElementAsync(IEdmStructuralProperty property);

        internal abstract void WriteNavigationPropertyElementHeader(IEdmNavigationProperty property);
        internal abstract Task WriteNavigationPropertyElementHeaderAsync(IEdmNavigationProperty property);

        internal abstract void WriteNavigationOnDeleteActionElement(EdmOnDeleteAction operationAction);
        internal abstract Task WriteNavigationOnDeleteActionElementAsync(EdmOnDeleteAction operationAction);

        internal abstract void WriteSchemaElementHeader(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings);
        internal abstract Task WriteSchemaElementHeaderAsync(EdmSchema schema, string alias, IEnumerable<KeyValuePair<string, string>> mappings);

        internal abstract void WriteAnnotationsElementHeader(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget);
        internal abstract Task WriteAnnotationsElementHeaderAsync(KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget);

        internal virtual void WriteOutOfLineAnnotationsBegin(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            // nothing here
        }

        internal virtual Task WriteOutOfLineAnnotationsBeginAsync(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteOutOfLineAnnotationsEnd(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            // nothing here
        }

        internal virtual Task WriteOutOfLineAnnotationsEndAsync(IEnumerable<KeyValuePair<string, List<IEdmVocabularyAnnotation>>> outOfLineAnnotations)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteStructuralPropertyElementHeader(IEdmStructuralProperty property, bool inlineType);
        internal abstract Task WriteStructuralPropertyElementHeaderAsync(IEdmStructuralProperty property, bool inlineType);

        internal abstract void WriteEnumMemberElementHeader(IEdmEnumMember member);
        internal abstract Task WriteEnumMemberElementHeaderAsync(IEdmEnumMember member);

        internal virtual void WriteEnumMemberElementEnd(IEdmEnumMember member)
        {
            // Nothing here
        }

        internal virtual Task WriteEnumMemberElementEndAsync(IEdmEnumMember member)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteNavigationPropertyBinding(IEdmNavigationPropertyBinding binding);
        internal abstract Task WriteNavigationPropertyBindingAsync(IEdmNavigationPropertyBinding binding);

        internal virtual void WriteNavigationPropertyBindingsBegin(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            // Nothing here
        }

        internal virtual Task WriteNavigationPropertyBindingsBeginAsync(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteNavigationPropertyBindingsEnd(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            // Nothing here
        }

        internal virtual Task WriteNavigationPropertyBindingsEndAsync(IEnumerable<IEdmNavigationPropertyBinding> bindings)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteNullableAttribute(IEdmTypeReference reference);
        internal abstract Task WriteNullableAttributeAsync(IEdmTypeReference reference);

        internal abstract void WriteTypeDefinitionAttributes(IEdmTypeDefinitionReference reference);
        internal abstract Task WriteTypeDefinitionAttributesAsync(IEdmTypeDefinitionReference reference);

        internal abstract void WriteBinaryTypeAttributes(IEdmBinaryTypeReference reference);
        internal abstract Task WriteBinaryTypeAttributesAsync(IEdmBinaryTypeReference reference);

        internal abstract void WriteDecimalTypeAttributes(IEdmDecimalTypeReference reference);
        internal abstract Task WriteDecimalTypeAttributesAsync(IEdmDecimalTypeReference reference);

        internal abstract void WriteSpatialTypeAttributes(IEdmSpatialTypeReference reference);
        internal abstract Task WriteSpatialTypeAttributesAsync(IEdmSpatialTypeReference reference);

        internal abstract void WriteStringTypeAttributes(IEdmStringTypeReference reference);
        internal abstract Task WriteStringTypeAttributesAsync(IEdmStringTypeReference reference);

        internal abstract void WriteTemporalTypeAttributes(IEdmTemporalTypeReference reference);
        internal abstract Task WriteTemporalTypeAttributesAsync(IEdmTemporalTypeReference reference);

        internal virtual void WriteReferentialConstraintBegin(IEdmReferentialConstraint referentialConstraint)
        {
            // nothing here
        }

        internal virtual Task WriteReferentialConstraintBeginAsync(IEdmReferentialConstraint referentialConstraint)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteReferentialConstraintEnd(IEdmReferentialConstraint referentialConstraint)
        {
            // nothing here
        }

        internal virtual Task WriteReferentialConstraintEndAsync(IEdmReferentialConstraint referentialConstraint)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteReferentialConstraintPair(EdmReferentialConstraintPropertyPair pair);
        internal abstract Task WriteReferentialConstraintPairAsync(EdmReferentialConstraintPropertyPair pair);

        internal abstract void WriteAnnotationStringAttribute(IEdmDirectValueAnnotation annotation);
        internal abstract Task WriteAnnotationStringAttributeAsync(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteAnnotationStringElement(IEdmDirectValueAnnotation annotation);
        internal abstract Task WriteAnnotationStringElementAsync(IEdmDirectValueAnnotation annotation);

        internal abstract void WriteActionElementHeader(IEdmAction action);
        internal abstract Task WriteActionElementHeaderAsync(IEdmAction action);

        internal abstract void WriteFunctionElementHeader(IEdmFunction function);
        internal abstract Task WriteFunctionElementHeaderAsync(IEdmFunction function);

        internal abstract void WriteReturnTypeElementHeader(IEdmOperationReturn operationReturn);
        internal abstract Task WriteReturnTypeElementHeaderAsync(IEdmOperationReturn operationReturn);

        internal abstract void WriteTypeAttribute(IEdmTypeReference typeReference);
        internal abstract Task WriteTypeAttributeAsync(IEdmTypeReference typeReference);

        internal abstract void WriteActionImportElementHeader(IEdmActionImport actionImport);
        internal abstract Task WriteActionImportElementHeaderAsync(IEdmActionImport actionImport);

        internal abstract void WriteFunctionImportElementHeader(IEdmFunctionImport functionImport);
        internal abstract Task WriteFunctionImportElementHeaderAsync(IEdmFunctionImport functionImport);

        internal abstract void WriteOperationParameterElementHeader(IEdmOperationParameter parameter, bool inlineType);
        internal abstract Task WriteOperationParameterElementHeaderAsync(IEdmOperationParameter parameter, bool inlineType);

        internal abstract void WriteOperationParameterEndElement(IEdmOperationParameter parameter);
        internal abstract Task WriteOperationParameterEndElementAsync(IEdmOperationParameter parameter);

        internal abstract void WriteCollectionTypeElementHeader(IEdmCollectionType collectionType, bool inlineType);
        internal abstract Task WriteCollectionTypeElementHeaderAsync(IEdmCollectionType collectionType, bool inlineType);

        internal abstract void WriteInlineExpression(IEdmExpression expression);
        internal abstract Task WriteInlineExpressionAsync(IEdmExpression expression);

        internal abstract void WriteVocabularyAnnotationElementHeader(IEdmVocabularyAnnotation annotation, bool isInline);
        internal abstract Task WriteVocabularyAnnotationElementHeaderAsync(IEdmVocabularyAnnotation annotation, bool isInline);

        internal abstract void WriteVocabularyAnnotationElementEnd(IEdmVocabularyAnnotation annotation, bool isInline);
        internal abstract Task WriteVocabularyAnnotationElementEndAsync(IEdmVocabularyAnnotation annotation, bool isInline);

        internal abstract void WritePropertyValueElementHeader(IEdmPropertyConstructor value, bool isInline);
        internal abstract Task WritePropertyValueElementHeaderAsync(IEdmPropertyConstructor value, bool isInline);

        internal abstract void WriteRecordExpressionElementHeader(IEdmRecordExpression expression);
        internal abstract Task WriteRecordExpressionElementHeaderAsync(IEdmRecordExpression expression);

        internal abstract void WritePropertyConstructorElementHeader(IEdmPropertyConstructor constructor, bool isInline);
        internal abstract Task WritePropertyConstructorElementHeaderAsync(IEdmPropertyConstructor constructor, bool isInline);

        internal abstract void WritePropertyConstructorElementEnd(IEdmPropertyConstructor constructor);
        internal abstract Task WritePropertyConstructorElementEndAsync(IEdmPropertyConstructor constructor);

        internal abstract void WriteStringConstantExpressionElement(IEdmStringConstantExpression expression);
        internal abstract Task WriteStringConstantExpressionElementAsync(IEdmStringConstantExpression expression);

        internal abstract void WriteBinaryConstantExpressionElement(IEdmBinaryConstantExpression expression);
        internal abstract Task WriteBinaryConstantExpressionElementAsync(IEdmBinaryConstantExpression expression);

        internal abstract void WriteBooleanConstantExpressionElement(IEdmBooleanConstantExpression expression);
        internal abstract Task WriteBooleanConstantExpressionElementAsync(IEdmBooleanConstantExpression expression);

        internal abstract void WriteNullConstantExpressionElement(IEdmNullExpression expression);
        internal abstract Task WriteNullConstantExpressionElementAsync(IEdmNullExpression expression);

        internal abstract void WriteDateConstantExpressionElement(IEdmDateConstantExpression expression);
        internal abstract Task WriteDateConstantExpressionElementAsync(IEdmDateConstantExpression expression);

        internal virtual void WriteSchemaOperationsHeader<T>(KeyValuePair<string, IList<T>> operations)
        {
            // nothing here
        }

        internal virtual Task WriteSchemaOperationsHeaderAsync<T>(KeyValuePair<string, IList<T>> operations)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteSchemaOperationsEnd<T>(KeyValuePair<string, IList<T>> operation)
        {
            // nothing here
        }

        internal virtual Task WriteSchemaOperationsEndAsync<T>(KeyValuePair<string, IList<T>> operation)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteOperationParametersBegin(IEnumerable<IEdmOperationParameter> parameters)
        {
            // nothing here
        }

        internal virtual Task WriteOperationParametersBeginAsync(IEnumerable<IEdmOperationParameter> parameters)
        {
            return Task.CompletedTask;
        }

        internal virtual void WriteOperationParametersEnd(IEnumerable<IEdmOperationParameter> parameters)
        {
            // nothing here
        }

        internal virtual Task WriteOperationParametersEndAsync(IEnumerable<IEdmOperationParameter> parameters)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteDateTimeOffsetConstantExpressionElement(IEdmDateTimeOffsetConstantExpression expression);
        internal abstract Task WriteDateTimeOffsetConstantExpressionElementAsync(IEdmDateTimeOffsetConstantExpression expression);

        internal abstract void WriteDurationConstantExpressionElement(IEdmDurationConstantExpression expression);
        internal abstract Task WriteDurationConstantExpressionElementAsync(IEdmDurationConstantExpression expression);

        internal abstract void WriteDecimalConstantExpressionElement(IEdmDecimalConstantExpression expression);
        internal abstract Task WriteDecimalConstantExpressionElementAsync(IEdmDecimalConstantExpression expression);

        internal abstract void WriteFloatingConstantExpressionElement(IEdmFloatingConstantExpression expression);
        internal abstract Task WriteFloatingConstantExpressionElementAsync(IEdmFloatingConstantExpression expression);

        internal abstract void WriteFunctionApplicationElementHeader(IEdmApplyExpression expression);
        internal abstract Task WriteFunctionApplicationElementHeaderAsync(IEdmApplyExpression expression);

        internal abstract void WriteFunctionApplicationElementEnd(IEdmApplyExpression expression);
        internal abstract Task WriteFunctionApplicationElementEndAsync(IEdmApplyExpression expression);

        internal abstract void WriteGuidConstantExpressionElement(IEdmGuidConstantExpression expression);
        internal abstract Task WriteGuidConstantExpressionElementAsync(IEdmGuidConstantExpression expression);

        internal abstract void WriteIntegerConstantExpressionElement(IEdmIntegerConstantExpression expression);
        internal abstract Task WriteIntegerConstantExpressionElementAsync(IEdmIntegerConstantExpression expression);

        internal abstract void WritePathExpressionElement(IEdmPathExpression expression);
        internal abstract Task WritePathExpressionElementAsync(IEdmPathExpression expression);

        internal abstract void WriteAnnotationPathExpressionElement(IEdmPathExpression expression);
        internal abstract Task WriteAnnotationPathExpressionElementAsync(IEdmPathExpression expression);

        internal abstract void WritePropertyPathExpressionElement(IEdmPathExpression expression);
        internal abstract Task WritePropertyPathExpressionElementAsync(IEdmPathExpression expression);

        internal abstract void WriteNavigationPropertyPathExpressionElement(IEdmPathExpression expression);
        internal abstract Task WriteNavigationPropertyPathExpressionElementAsync(IEdmPathExpression expression);

        internal abstract void WriteIfExpressionElementHeader(IEdmIfExpression expression);
        internal abstract Task WriteIfExpressionElementHeaderAsync(IEdmIfExpression expression);

        internal abstract void WriteIfExpressionElementEnd(IEdmIfExpression expression);
        internal abstract Task WriteIfExpressionElementEndAsync(IEdmIfExpression expression);

        internal abstract void WriteCollectionExpressionElementHeader(IEdmCollectionExpression expression);
        internal abstract Task WriteCollectionExpressionElementHeaderAsync(IEdmCollectionExpression expression);

        internal abstract void WriteCollectionExpressionElementEnd(IEdmCollectionExpression expression);
        internal abstract Task WriteCollectionExpressionElementEndAsync(IEdmCollectionExpression expression);

        internal abstract void WriteLabeledElementHeader(IEdmLabeledExpression labeledElement);
        internal abstract Task WriteLabeledElementHeaderAsync(IEdmLabeledExpression labeledElement);

        internal abstract void WriteLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression labeledExpressionReference);
        internal abstract Task WriteLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression labeledExpressionReference);

        internal abstract void WriteTimeOfDayConstantExpressionElement(IEdmTimeOfDayConstantExpression expression);
        internal abstract Task WriteTimeOfDayConstantExpressionElementAsync(IEdmTimeOfDayConstantExpression expression);

        internal abstract void WriteIsOfExpressionElementHeader(IEdmIsOfExpression expression, bool inlineType);
        internal abstract Task WriteIsOfExpressionElementHeaderAsync(IEdmIsOfExpression expression, bool inlineType);

        internal virtual void WriteIsOfExpressionType(IEdmIsOfExpression expression, bool inlineType)
        {
            // nothing here
        }

        internal virtual Task WriteIsOfExpressionTypeAsync(IEdmIsOfExpression expression, bool inlineType)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteCastExpressionElementHeader(IEdmCastExpression expression, bool inlineType);
        internal abstract Task WriteCastExpressionElementHeaderAsync(IEdmCastExpression expression, bool inlineType);

        internal abstract void WriteCastExpressionElementEnd(IEdmCastExpression expression, bool inlineType);
        internal abstract Task WriteCastExpressionElementEndAsync(IEdmCastExpression expression, bool inlineType);

        internal virtual void WriteCastExpressionType(IEdmCastExpression expression, bool inlineType)
        {
            // nothing here
        }

        internal virtual Task WriteCastExpressionTypeAsync(IEdmCastExpression expression, bool inlineType)
        {
            return Task.CompletedTask;
        }

        internal abstract void WriteEnumMemberExpressionElement(IEdmEnumMemberExpression expression);
        internal abstract Task WriteEnumMemberExpressionElementAsync(IEdmEnumMemberExpression expression);

        internal abstract void WriteTypeDefinitionElementHeader(IEdmTypeDefinition typeDefinition);
        internal abstract Task WriteTypeDefinitionElementHeaderAsync(IEdmTypeDefinition typeDefinition);

        internal abstract void WriteEndElement();
        internal abstract Task WriteEndElementAsync();

        internal abstract void WriteArrayEndElement();
        internal abstract Task WriteArrayEndElementAsync();

        internal abstract void WriteOperationElementAttributes(IEdmOperation operation);
        internal abstract Task WriteOperationElementAttributesAsync(IEdmOperation operation);

        internal abstract void WriteOperationImportAttributes(IEdmOperationImport operationImport, string operationAttributeName);
        internal abstract Task WriteOperationImportAttributesAsync(IEdmOperationImport operationImport, string operationAttributeName);

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
