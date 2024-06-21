//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSerializationVisitor.cs" company="Microsoft">
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
    internal sealed class EdmModelCsdlSerializationVisitor : EdmModelVisitor
    {
        private readonly EdmModelCsdlSchemaWriter schemaWriter;
        private readonly List<IEdmNavigationProperty> navigationProperties = new List<IEdmNavigationProperty>();
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;
        private readonly bool isXml;

        internal EdmModelCsdlSerializationVisitor(IEdmModel model, EdmModelCsdlSchemaWriter edmWriter)
            : base(model)
        {
            this.schemaWriter = edmWriter;
            this.namespaceAliasMappings = this.schemaWriter.NamespaceAliasMappings;
            this.isXml = this.schemaWriter is EdmModelCsdlSchemaXmlWriter;
        }

        public override void VisitEntityContainerElements(IEnumerable<IEdmEntityContainerElement> elements)
        {
            HashSet<string> functionImportsWritten = new HashSet<string>();
            HashSet<string> actionImportsWritten = new HashSet<string>();

            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        this.ProcessEntitySet((IEdmEntitySet)element);
                        break;
                    case EdmContainerElementKind.Singleton:
                        this.ProcessSingleton((IEdmSingleton)element);
                        break;
                    case EdmContainerElementKind.ActionImport:
                        // Skip actionImports that have the same name for same overloads of a action.
                        IEdmActionImport actionImport = (IEdmActionImport)element;
                        string uniqueActionName = actionImport.Name + "_" + actionImport.Action.FullName() + GetEntitySetString(actionImport);
                        if (!actionImportsWritten.Contains(uniqueActionName))
                        {
                            actionImportsWritten.Add(uniqueActionName);
                            this.ProcessActionImport(actionImport);
                        }

                        break;
                    case EdmContainerElementKind.FunctionImport:
                        // Skip functionImports that have the same name for same overloads of a function.
                        IEdmFunctionImport functionImport = (IEdmFunctionImport)element;
                        string uniqueFunctionName = functionImport.Name + "_" + functionImport.Function.FullName() + GetEntitySetString(functionImport);
                        if (!functionImportsWritten.Contains(uniqueFunctionName))
                        {
                            functionImportsWritten.Add(uniqueFunctionName);
                            this.ProcessFunctionImport(functionImport);
                        }

                        break;
                    case EdmContainerElementKind.None:
                        this.ProcessEntityContainerElement(element);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                }
            }
        }

        /// <summary>
        /// Asynchronously visits the elements of the entity container.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task VisitEntityContainerElementsAsync(IEnumerable<IEdmEntityContainerElement> elements)
        {
            var functionImportsWritten = new HashSet<string>();
            var actionImportsWritten = new HashSet<string>();

            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        await this.ProcessEntitySetAsync((IEdmEntitySet)element).ConfigureAwait(false);
                        break;
                    case EdmContainerElementKind.Singleton:
                        await this.ProcessSingletonAsync((IEdmSingleton)element).ConfigureAwait(false);
                        break;
                    case EdmContainerElementKind.ActionImport:
                        // Skip actionImports that have the same name for same overloads of a action.
                        IEdmActionImport actionImport = (IEdmActionImport)element;

                        var uniqueActionName = string.Concat(actionImport.Name, "_", actionImport.Action.FullName(), GetEntitySetString(actionImport));
                        if (!actionImportsWritten.Contains(uniqueActionName))
                        {
                            actionImportsWritten.Add(uniqueActionName);
                            await this.ProcessActionImportAsync(actionImport).ConfigureAwait(false);
                        }

                        break;
                    case EdmContainerElementKind.FunctionImport:
                        // Skip functionImports that have the same name for same overloads of a function.
                        IEdmFunctionImport functionImport = (IEdmFunctionImport)element;

                        var uniqueFunctionName = string.Concat(functionImport.Name, "_", functionImport.Function.FullName(), GetEntitySetString(functionImport));
                        if (!functionImportsWritten.Contains(uniqueFunctionName))
                        {
                            functionImportsWritten.Add(uniqueFunctionName);
                            await this.ProcessFunctionImportAsync(functionImport).ConfigureAwait(false);
                        }

                        break;
                    case EdmContainerElementKind.None:
                        await this.ProcessEntityContainerElementAsync(element).ConfigureAwait(false);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                }
            }
        }

        internal void VisitEdmSchema(EdmSchema element, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string alias = null;
            if (this.namespaceAliasMappings != null)
            {
                this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias);
            }

            this.schemaWriter.WriteSchemaElementHeader(element, alias, mappings);

            VisitSchemaElements(element.SchemaElements);

            // process the functions/actions seperately
            foreach (var operation in element.SchemaOperations)
            {
                this.schemaWriter.WriteSchemaOperationsHeader(operation);
                VisitSchemaElements(operation.Value.AsEnumerable<IEdmSchemaElement>()); // Call AsEnumerable() to make .net 3.5 happy
                this.schemaWriter.WriteSchemaOperationsEnd(operation);
            }

            // EntityContainers are excluded from the EdmSchema.SchemaElements property so they can be forced to the end.
            VisitCollection(element.EntityContainers, this.ProcessEntityContainer);

            if (element.OutOfLineAnnotations.Any())
            {
                this.schemaWriter.WriteOutOfLineAnnotationsBegin(element.OutOfLineAnnotations);
                foreach (KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget in element.OutOfLineAnnotations)
                {
                    this.schemaWriter.WriteAnnotationsElementHeader(annotationsForTarget);
                    VisitVocabularyAnnotations(annotationsForTarget.Value);
                    this.schemaWriter.WriteEndElement();
                }

                this.schemaWriter.WriteOutOfLineAnnotationsEnd(element.OutOfLineAnnotations);
            }

            this.schemaWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously visits Edm Schema.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        internal async Task VisitEdmSchemaAsync(EdmSchema element, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string alias = null;
            if (this.namespaceAliasMappings != null)
            {
                this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias);
            }

            await this.schemaWriter.WriteSchemaElementHeaderAsync(element, alias, mappings).ConfigureAwait(false);

            VisitSchemaElements(element.SchemaElements);

            // process the functions/actions seperately
            foreach (var operation in element.SchemaOperations)
            {
                await this.schemaWriter.WriteSchemaOperationsHeaderAsync(operation).ConfigureAwait(false);
                VisitSchemaElements(operation.Value.AsEnumerable<IEdmSchemaElement>()); // Call AsEnumerable() to make .net 3.5 happy
                await this.schemaWriter.WriteSchemaOperationsEndAsync(operation).ConfigureAwait(false);
            }

            // EntityContainers are excluded from the EdmSchema.SchemaElements property so they can be forced to the end.
            VisitCollection(element.EntityContainers, async (e) => await this.ProcessEntityContainerAsync(e).ConfigureAwait(false));

            if (element.OutOfLineAnnotations.Any())
            {
                await this.schemaWriter.WriteOutOfLineAnnotationsBeginAsync(element.OutOfLineAnnotations).ConfigureAwait(false);
                foreach (KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget in element.OutOfLineAnnotations)
                {
                    await this.schemaWriter.WriteAnnotationsElementHeaderAsync(annotationsForTarget).ConfigureAwait(false);
                    VisitVocabularyAnnotations(annotationsForTarget.Value);
                    await this.schemaWriter.WriteEndElementAsync().ConfigureAwait(false);
                }

                await this.schemaWriter.WriteOutOfLineAnnotationsEndAsync(element.OutOfLineAnnotations).ConfigureAwait(false);
            }

            await this.schemaWriter.WriteEndElementAsync().ConfigureAwait(false);
        }

        protected override void ProcessEntityContainer(IEdmEntityContainer element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntityContainerElementHeader);
            base.ProcessEntityContainer(element);
            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the entity container.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessEntityContainerAsync(IEdmEntityContainer element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteEntityContainerElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await base.ProcessEntityContainerAsync(element).ConfigureAwait(false);
            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessEntitySet(IEdmEntitySet element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntitySetElementHeader);

            base.ProcessEntitySet(element);

            ProcessNavigationPropertyBindings(element);

            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the entity set.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessEntitySetAsync(IEdmEntitySet element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteEntitySetElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);

            await base.ProcessEntitySetAsync(element).ConfigureAwait(false);

            await ProcessNavigationPropertyBindingsAsync(element).ConfigureAwait(false);

            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessSingleton(IEdmSingleton element)
        {
            this.BeginElement(element, this.schemaWriter.WriteSingletonElementHeader);

            base.ProcessSingleton(element);

            ProcessNavigationPropertyBindings(element);

            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the singleton.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessSingletonAsync(IEdmSingleton element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteSingletonElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);

            await base.ProcessSingletonAsync(element).ConfigureAwait(false);

            await ProcessNavigationPropertyBindingsAsync(element).ConfigureAwait(false);

            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessEntityType(IEdmEntityType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntityTypeElementHeader);
            if (element.DeclaredKey != null && element.DeclaredKey.Any())
            {
                this.VisitEntityTypeDeclaredKey(element.DeclaredKey);
            }

            this.VisitProperties(element.DeclaredStructuralProperties().Cast<IEdmProperty>());
            this.VisitProperties(element.DeclaredNavigationProperties().Cast<IEdmProperty>());
            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the entity type.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessEntityTypeAsync(IEdmEntityType element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteEntityTypeElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            if (element.DeclaredKey != null && element.DeclaredKey.Any())
            {
                await this.VisitEntityTypeDeclaredKeyAsync(element.DeclaredKey).ConfigureAwait(false);
            }

            this.VisitProperties(element.DeclaredStructuralProperties().Cast<IEdmProperty>());
            this.VisitProperties(element.DeclaredNavigationProperties().Cast<IEdmProperty>());
            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessStructuralProperty(IEdmStructuralProperty element)
        {
            bool inlineType = IsInlineType(element.Type);
            this.BeginElement(element, (IEdmStructuralProperty t) => { this.schemaWriter.WriteStructuralPropertyElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the structural property.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessStructuralPropertyAsync(IEdmStructuralProperty element)
        {
            bool inlineType = IsInlineType(element.Type);
            await this.BeginElementAsync(
                    element, 
                    async (IEdmStructuralProperty t) => { await this.schemaWriter.WriteStructuralPropertyElementHeaderAsync(t, inlineType).ConfigureAwait(false); }, 
                    async e => { await this.ProcessFacetsAsync(e.Type, inlineType).ConfigureAwait(false); }
                ).ConfigureAwait(false);
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element)
        {
            this.schemaWriter.WriteTypeDefinitionAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the type definition reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessTypeDefinitionReferenceAsync(IEdmTypeDefinitionReference element)
        {
            return this.schemaWriter.WriteTypeDefinitionAttributesAsync(element);
        }

        protected override void ProcessBinaryTypeReference(IEdmBinaryTypeReference element)
        {
            this.schemaWriter.WriteBinaryTypeAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the binary type reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessBinaryTypeReferenceAsync(IEdmBinaryTypeReference element)
        {
            return this.schemaWriter.WriteBinaryTypeAttributesAsync(element);
        }

        protected override void ProcessDecimalTypeReference(IEdmDecimalTypeReference element)
        {
            this.schemaWriter.WriteDecimalTypeAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the decimal type reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessDecimalTypeReferenceAsync(IEdmDecimalTypeReference element)
        {
            return this.schemaWriter.WriteDecimalTypeAttributesAsync(element);
        }

        protected override void ProcessSpatialTypeReference(IEdmSpatialTypeReference element)
        {
            this.schemaWriter.WriteSpatialTypeAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the spatial type reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessSpatialTypeReferenceAsync(IEdmSpatialTypeReference element)
        {
            return this.schemaWriter.WriteSpatialTypeAttributesAsync(element);
        }

        protected override void ProcessStringTypeReference(IEdmStringTypeReference element)
        {
            this.schemaWriter.WriteStringTypeAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the string type reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessStringTypeReferenceAsync(IEdmStringTypeReference element)
        {
            return this.schemaWriter.WriteStringTypeAttributesAsync(element);
        }

        protected override void ProcessTemporalTypeReference(IEdmTemporalTypeReference element)
        {
            this.schemaWriter.WriteTemporalTypeAttributes(element);
        }

        /// <summary>
        /// Asynchronously processes the temporal type reference.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessTemporalTypeReferenceAsync(IEdmTemporalTypeReference element)
        {
            return this.schemaWriter.WriteTemporalTypeAttributesAsync(element);
        }

        protected override void ProcessNavigationProperty(IEdmNavigationProperty element)
        {
            this.BeginElement(element, this.schemaWriter.WriteNavigationPropertyElementHeader);

            // From 4.0.1 spec says:
            // "OnDelete" has "None, Cascade, SetNull, SetDefault" values defined in 4.01.
            // But, ODL now only process "Cascade" and "None".So we should fix it to support the others.
            if (element.OnDelete != EdmOnDeleteAction.None)
            {
                this.schemaWriter.WriteNavigationOnDeleteActionElement(element.OnDelete);
            }

            this.ProcessReferentialConstraint(element.ReferentialConstraint);

            this.EndElement(element);
            this.navigationProperties.Add(element);
        }

        /// <summary>
        /// Asynchronously processes the navigation property.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessNavigationPropertyAsync(IEdmNavigationProperty element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteNavigationPropertyElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);

            // From 4.0.1 spec says:
            // "OnDelete" has "None, Cascade, SetNull, SetDefault" values defined in 4.01.
            // But, ODL now only process "Cascade" and "None".So we should fix it to support the others.
            if (element.OnDelete != EdmOnDeleteAction.None)
            {
                await this.schemaWriter.WriteNavigationOnDeleteActionElementAsync(element.OnDelete).ConfigureAwait(false);
            }

            await this.ProcessReferentialConstraintAsync(element.ReferentialConstraint).ConfigureAwait(false);

            await this.EndElementAsync(element).ConfigureAwait(false);
            this.navigationProperties.Add(element);
        }

        protected override void ProcessComplexType(IEdmComplexType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteComplexTypeElementHeader);
            base.ProcessComplexType(element);
            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the complex type.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessComplexTypeAsync(IEdmComplexType element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteComplexTypeElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await base.ProcessComplexTypeAsync(element).ConfigureAwait(false);
            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessEnumType(IEdmEnumType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEnumTypeElementHeader);
            base.ProcessEnumType(element);
            this.EndElement(element, this.schemaWriter.WriteEnumTypeElementEnd);
        }

        /// <summary>
        /// Asynchronously processes the enum type.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessEnumTypeAsync(IEdmEnumType element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteEnumTypeElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await base.ProcessEnumTypeAsync(element).ConfigureAwait(false);
            await this.EndElementAsync(element, async (element) => await this.schemaWriter.WriteEnumTypeElementEndAsync(element).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessEnumMember(IEdmEnumMember element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEnumMemberElementHeader);
            this.EndElement(element, this.schemaWriter.WriteEnumMemberElementEnd);
        }

        /// <summary>
        /// Asynchronously processes the enum member.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessEnumMemberAsync(IEdmEnumMember element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteEnumMemberElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await this.EndElementAsync(element, async (e) => await this.schemaWriter.WriteEnumMemberElementEndAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
        {
            this.BeginElement(element, this.schemaWriter.WriteTypeDefinitionElementHeader);
            base.ProcessTypeDefinition(element);
            this.EndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the type definition.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessTypeDefinitionAsync(IEdmTypeDefinition element)
        {
            await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteTypeDefinitionElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await base.ProcessTypeDefinitionAsync(element).ConfigureAwait(false);
            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessTerm(IEdmTerm term)
        {
            bool inlineType = term.Type != null && IsInlineType(term.Type);
            this.BeginElement(term, (IEdmTerm t) => { this.schemaWriter.WriteTermElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                if (term.Type != null)
                {
                    VisitTypeReference(term.Type);
                }
            }

            this.EndElement(term);
        }

        /// <summary>
        /// Asynchronously processes the term.
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        protected override async Task ProcessTermAsync(IEdmTerm term)
        {
            bool inlineType = term.Type != null && IsInlineType(term.Type);
            await this.BeginElementAsync(
                    term, 
                    async (IEdmTerm t) => { await this.schemaWriter.WriteTermElementHeaderAsync(t, inlineType).ConfigureAwait(false); }, 
                    async (e) => { await this.ProcessFacetsAsync(e.Type, inlineType).ConfigureAwait(false); }
                ).ConfigureAwait(false);
            if (!inlineType)
            {
                if (term.Type != null)
                {
                    VisitTypeReference(term.Type);
                }
            }

            await this.EndElementAsync(term).ConfigureAwait(false);
        }

        protected override void ProcessAction(IEdmAction action)
        {
            this.ProcessOperation(action, this.schemaWriter.WriteActionElementHeader);
        }

        /// <summary>
        /// Asynchronously processes the action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected override Task ProcessActionAsync(IEdmAction action)
        {
            return this.ProcessOperationAsync(action, async (a) => await this.schemaWriter.WriteActionElementHeaderAsync(a).ConfigureAwait(false));
        }

        protected override void ProcessFunction(IEdmFunction function)
        {
            this.ProcessOperation(function, this.schemaWriter.WriteFunctionElementHeader);
        }

        /// <summary>
        /// Asynchronously processes the function.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        protected override Task ProcessFunctionAsync(IEdmFunction function)
        {
            return this.ProcessOperationAsync(function, async (f) => await this.schemaWriter.WriteFunctionElementHeaderAsync(f).ConfigureAwait(false));
        }

        protected override void ProcessOperationParameter(IEdmOperationParameter element)
        {
            bool inlineType = IsInlineType(element.Type);
            this.BeginElement(
                element,
                (IEdmOperationParameter t) => { this.schemaWriter.WriteOperationParameterElementHeader(t, inlineType); },
                e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model)));
            }

            this.schemaWriter.WriteOperationParameterEndElement(element);
        }

        /// <summary>
        /// Asynchronously processes the operation parameter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessOperationParameterAsync(IEdmOperationParameter element)
        {
            bool inlineType = IsInlineType(element.Type);
            await this.BeginElementAsync(
                    element,
                    async (IEdmOperationParameter t) => { await this.schemaWriter.WriteOperationParameterElementHeaderAsync(t, inlineType).ConfigureAwait(false); },
                    async e => { await this.ProcessFacetsAsync(e.Type, inlineType).ConfigureAwait(false); }
                ).ConfigureAwait(false);
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            await this.VisitPrimitiveElementAnnotationsAsync(this.Model.DirectValueAnnotations(element)).ConfigureAwait(false);
            if (element is IEdmVocabularyAnnotatable vocabularyAnnotatableElement)
            {
                await this.VisitElementVocabularyAnnotationsAsync(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model))).ConfigureAwait(false);
            }

            await this.schemaWriter.WriteOperationParameterEndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessOperationReturn(IEdmOperationReturn operationReturn)
        {
            if (operationReturn == null)
            {
                return;
            }

            bool inlineType = IsInlineType(operationReturn.Type);
            this.BeginElement(
                operationReturn.Type,
                type => this.schemaWriter.WriteReturnTypeElementHeader(operationReturn),
                type =>
                {
                    if (inlineType)
                    {
                        this.schemaWriter.WriteTypeAttribute(type);
                        this.ProcessFacets(type, true /*inlineType*/);
                    }
                    else
                    {
                        this.VisitTypeReference(type);
                    }
                });
            this.EndElement(operationReturn);
        }

        /// <summary>
        /// Asynchronously processes the operation return.
        /// </summary>
        /// <param name="operationReturn"></param>
        /// <returns></returns>
        protected override async Task ProcessOperationReturnAsync(IEdmOperationReturn operationReturn)
        {
            if (operationReturn == null)
            {
                return;
            }

            bool inlineType = IsInlineType(operationReturn.Type);
            await this.BeginElementAsync(
                operationReturn.Type,
                async type => await this.schemaWriter.WriteReturnTypeElementHeaderAsync(operationReturn).ConfigureAwait(false),
                async type =>
                {
                    if (inlineType)
                    {
                        await this.schemaWriter.WriteTypeAttributeAsync(type).ConfigureAwait(false);
                        await this.ProcessFacetsAsync(type, true /*inlineType*/).ConfigureAwait(false);
                    }
                    else
                    {
                        this.VisitTypeReference(type);
                    }
                }).ConfigureAwait(false);
            await this.EndElementAsync(operationReturn).ConfigureAwait(false);
        }

        protected override void ProcessCollectionType(IEdmCollectionType element)
        {
            bool inlineType = IsInlineType(element.ElementType);
            this.BeginElement(
                element,
                (IEdmCollectionType t) => this.schemaWriter.WriteCollectionTypeElementHeader(t, inlineType),
                e => this.ProcessFacets(e.ElementType, inlineType));
            if (!inlineType)
            {
                VisitTypeReference(element.ElementType);
            }

            this.EndElement(element);
        }

        protected override async Task ProcessCollectionTypeAsync(IEdmCollectionType element)
        {
            bool inlineType = IsInlineType(element.ElementType);
            await this.BeginElementAsync(
                    element,
                    async (IEdmCollectionType t) => await this.schemaWriter.WriteCollectionTypeElementHeaderAsync(t, inlineType).ConfigureAwait(false),
                    async e => await this.ProcessFacetsAsync(e.ElementType, inlineType).ConfigureAwait(false)
                ).ConfigureAwait(false);
            if (!inlineType)
            {
                VisitTypeReference(element.ElementType);
            }

            await this.EndElementAsync(element).ConfigureAwait(false);
        }

        protected override void ProcessActionImport(IEdmActionImport actionImport)
        {
            this.BeginElement(actionImport, this.schemaWriter.WriteActionImportElementHeader);
            this.EndElement(actionImport);
        }

        /// <summary>
        /// Asynchronously processes the action import.
        /// </summary>
        /// <param name="actionImport"></param>
        /// <returns></returns>
        protected override async Task ProcessActionImportAsync(IEdmActionImport actionImport)
        {
            await this.BeginElementAsync(actionImport, async (a) => await this.schemaWriter.WriteActionImportElementHeaderAsync(a).ConfigureAwait(false)).ConfigureAwait(false);
            await this.EndElementAsync(actionImport).ConfigureAwait(false);
        }

        protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.BeginElement(functionImport, this.schemaWriter.WriteFunctionImportElementHeader);
            this.EndElement(functionImport);
        }

        /// <summary>
        /// Asynchronously processes the function import.
        /// </summary>
        /// <param name="functionImport"></param>
        /// <returns></returns>
        protected override async Task ProcessFunctionImportAsync(IEdmFunctionImport functionImport)
        {
            await this.BeginElementAsync(functionImport, async (f) => await this.schemaWriter.WriteFunctionImportElementHeaderAsync(f).ConfigureAwait(false)).ConfigureAwait(false);
            await this.EndElementAsync(functionImport).ConfigureAwait(false);
        }

        #region Vocabulary Annotations

        protected override void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
        {
            bool isInline = IsInlineExpression(annotation.Value);
            this.BeginElement(annotation, t => this.schemaWriter.WriteVocabularyAnnotationElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessAnnotation(annotation);
            }

            this.EndElement(annotation, t => this.schemaWriter.WriteVocabularyAnnotationElementEnd(annotation, isInline));
        }

        /// <summary>
        /// Asynchronously processes the annotation.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        protected override async Task ProcessAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            bool isInline = IsInlineExpression(annotation.Value);
            await this.BeginElementAsync(annotation, async t => await this.schemaWriter.WriteVocabularyAnnotationElementHeaderAsync(t, isInline).ConfigureAwait(false)).ConfigureAwait(false);
            if (!isInline)
            {
                await base.ProcessAnnotationAsync(annotation).ConfigureAwait(false);
            }

            await this.EndElementAsync(annotation, async t => await this.schemaWriter.WriteVocabularyAnnotationElementEndAsync(annotation, isInline).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #region Expressions

        protected override void ProcessStringConstantExpression(IEdmStringConstantExpression expression)
        {
            this.schemaWriter.WriteStringConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the string constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessStringConstantExpressionAsync(IEdmStringConstantExpression expression)
        {
            return this.schemaWriter.WriteStringConstantExpressionElementAsync(expression);
        }

        protected override void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression)
        {
            this.schemaWriter.WriteBinaryConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the binary constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessBinaryConstantExpressionAsync(IEdmBinaryConstantExpression expression)
        {
            return this.schemaWriter.WriteBinaryConstantExpressionElementAsync(expression);
        }

        protected override void ProcessRecordExpression(IEdmRecordExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteRecordExpressionElementHeader);
            this.VisitPropertyConstructors(expression.Properties);
            this.EndElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the record expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessRecordExpressionAsync(IEdmRecordExpression expression)
        {
            await this.BeginElementAsync(expression, async (e) => await this.schemaWriter.WriteRecordExpressionElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            this.VisitPropertyConstructors(expression.Properties);
            await this.EndElementAsync(expression).ConfigureAwait(false);
        }

        protected override void ProcessLabeledExpression(IEdmLabeledExpression element)
        {
            if (element.Name == null)
            {
                base.ProcessLabeledExpression(element);
            }
            else
            {
                this.BeginElement(element, this.schemaWriter.WriteLabeledElementHeader);
                base.ProcessLabeledExpression(element);
                this.EndElement(element);
            }
        }

        /// <summary>
        /// Asynchronously processes the labeled expression.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override async Task ProcessLabeledExpressionAsync(IEdmLabeledExpression element)
        {
            if (element.Name == null)
            {
                await base.ProcessLabeledExpressionAsync(element).ConfigureAwait(false);
            }
            else
            {
                await this.BeginElementAsync(element, async (e) => await this.schemaWriter.WriteLabeledElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
                await base.ProcessLabeledExpressionAsync(element).ConfigureAwait(false);
                await this.EndElementAsync(element).ConfigureAwait(false);
            }
        }

        protected override void ProcessLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression element)
        {
            this.schemaWriter.WriteLabeledExpressionReferenceExpression(element);
        }

        /// <summary>
        /// Asynchronously processes the labeled expression reference expression.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Task ProcessLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression element)
        {
            return this.schemaWriter.WriteLabeledExpressionReferenceExpressionAsync(element);
        }

        protected override void ProcessPropertyConstructor(IEdmPropertyConstructor constructor)
        {
            bool isInline = IsInlineExpression(constructor.Value);
            this.BeginElement(constructor, t => this.schemaWriter.WritePropertyConstructorElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessPropertyConstructor(constructor);
            }

            this.EndElement(constructor, this.schemaWriter.WritePropertyConstructorElementEnd);
        }

        /// <summary>
        /// Asynchronously processes the property constructor.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        protected override async Task ProcessPropertyConstructorAsync(IEdmPropertyConstructor constructor)
        {
            bool isInline = IsInlineExpression(constructor.Value);
            await this.BeginElementAsync(constructor, async t => await this.schemaWriter.WritePropertyConstructorElementHeaderAsync(t, isInline).ConfigureAwait(false)).ConfigureAwait(false);
            if (!isInline)
            {
                await base.ProcessPropertyConstructorAsync(constructor).ConfigureAwait(false);
            }

            await this.EndElementAsync(constructor, async (c) => await this.schemaWriter.WritePropertyConstructorElementEndAsync(c).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WritePathExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the path expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.schemaWriter.WritePathExpressionElementAsync(expression);
        }

        protected override void ProcessPropertyPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WritePropertyPathExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the property path expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.schemaWriter.WritePropertyPathExpressionElementAsync(expression);
        }

        protected override void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WriteNavigationPropertyPathExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the navigation property path expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessNavigationPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.schemaWriter.WriteNavigationPropertyPathExpressionElementAsync(expression);
        }

        protected override void ProcessAnnotationPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WriteAnnotationPathExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the annotation path expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessAnnotationPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.schemaWriter.WriteAnnotationPathExpressionElementAsync(expression);
        }

        protected override void ProcessCollectionExpression(IEdmCollectionExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteCollectionExpressionElementHeader);
            this.VisitExpressions(expression.Elements);
            this.EndElement(expression, this.schemaWriter.WriteCollectionExpressionElementEnd);
        }

        /// <summary>
        /// Asynchronously processes the collection expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessCollectionExpressionAsync(IEdmCollectionExpression expression)
        {
            await this.BeginElementAsync(expression, async (e) => await this.schemaWriter.WriteCollectionExpressionElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            this.VisitExpressions(expression.Elements);
            await this.EndElementAsync(expression, async (e) => await this.schemaWriter.WriteCollectionExpressionElementEndAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessIsTypeExpression(IEdmIsTypeExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);

            if (this.isXml)
            {
                this.BeginElement(expression, (IEdmIsTypeExpression t) => { this.schemaWriter.WriteIsTypeExpressionElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
                if (!inlineType)
                {
                    VisitTypeReference(expression.Type);
                }

                this.VisitExpression(expression.Operand);
                this.EndElement(expression);
            }
            else
            {
                this.BeginElement(expression, (IEdmIsTypeExpression t) => { this.schemaWriter.WriteIsTypeExpressionElementHeader(t, inlineType); });
                this.VisitExpression(expression.Operand);
                this.schemaWriter.WriteIsOfExpressionType(expression, inlineType);
                this.ProcessFacets(expression.Type, inlineType);
                this.EndElement(expression);
            }
        }

        /// <summary>
        /// Asynchronously processes the is type expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessIsTypeExpressionAsync(IEdmIsTypeExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);

            if (this.isXml)
            {
                await this.BeginElementAsync(
                        expression, 
                        async (IEdmIsTypeExpression t) => { await this.schemaWriter.WriteIsTypeExpressionElementHeaderAsync(t, inlineType).ConfigureAwait(false); }, 
                        async (e) => { await this.ProcessFacetsAsync(e.Type, inlineType).ConfigureAwait(false); }
                    ).ConfigureAwait(false);

                if (!inlineType)
                {
                    VisitTypeReference(expression.Type);
                }

                this.VisitExpression(expression.Operand);
                await this.EndElementAsync(expression).ConfigureAwait(false);
            }
            else
            {
                await this.BeginElementAsync(
                        expression, 
                        async (IEdmIsTypeExpression t) => { await this.schemaWriter.WriteIsTypeExpressionElementHeaderAsync(t, inlineType).ConfigureAwait(false); }
                    ).ConfigureAwait(false);
                this.VisitExpression(expression.Operand);
                await this.schemaWriter.WriteIsOfExpressionTypeAsync(expression, inlineType).ConfigureAwait(false);
                await this.ProcessFacetsAsync(expression.Type, inlineType).ConfigureAwait(false);
                await this.EndElementAsync(expression).ConfigureAwait(false);
            }
        }

        protected override void ProcessIntegerConstantExpression(IEdmIntegerConstantExpression expression)
        {
            this.schemaWriter.WriteIntegerConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the integer constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessIntegerConstantExpressionAsync(IEdmIntegerConstantExpression expression)
        {
            return this.schemaWriter.WriteIntegerConstantExpressionElementAsync(expression);
        }

        protected override void ProcessIfExpression(IEdmIfExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteIfExpressionElementHeader);
            base.ProcessIfExpression(expression);
            this.EndElement(expression, this.schemaWriter.WriteIfExpressionElementEnd);
        }

        /// <summary>
        /// Asynchronously processes the if expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessIfExpressionAsync(IEdmIfExpression expression)
        {
            await this.BeginElementAsync(expression, async (e) => await this.schemaWriter.WriteIfExpressionElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            await base.ProcessIfExpressionAsync(expression).ConfigureAwait(false);
            await this.EndElementAsync(expression, async (e) => await this.schemaWriter.WriteIfExpressionElementEndAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
        {
            this.BeginElement(expression, e => this.schemaWriter.WriteFunctionApplicationElementHeader(e));
            this.VisitExpressions(expression.Arguments);
            this.EndElement(expression, e => this.schemaWriter.WriteFunctionApplicationElementEnd(e));
        }

        /// <summary>
        /// Asynchronously processes the function application expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessFunctionApplicationExpressionAsync(IEdmApplyExpression expression)
        {
            await this.BeginElementAsync(expression, async (e) => await this.schemaWriter.WriteFunctionApplicationElementHeaderAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
            this.VisitExpressions(expression.Arguments);
            await this.EndElementAsync(expression, async (e) => await this.schemaWriter.WriteFunctionApplicationElementEndAsync(e).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected override void ProcessFloatingConstantExpression(IEdmFloatingConstantExpression expression)
        {
            this.schemaWriter.WriteFloatingConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the floating constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessFloatingConstantExpressionAsync(IEdmFloatingConstantExpression expression)
        {
            return this.schemaWriter.WriteFloatingConstantExpressionElementAsync(expression);
        }

        protected override void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression)
        {
            this.schemaWriter.WriteGuidConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the guid constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessGuidConstantExpressionAsync(IEdmGuidConstantExpression expression)
        {
            return this.schemaWriter.WriteGuidConstantExpressionElementAsync(expression);
        }

        protected override void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression)
        {
            this.schemaWriter.WriteEnumMemberExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the enum member expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessEnumMemberExpressionAsync(IEdmEnumMemberExpression expression)
        {
            return this.schemaWriter.WriteEnumMemberExpressionElementAsync(expression);
        }

        protected override void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression)
        {
            this.schemaWriter.WriteDecimalConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the decimal constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessDecimalConstantExpressionAsync(IEdmDecimalConstantExpression expression)
        {
            return this.schemaWriter.WriteDecimalConstantExpressionElementAsync(expression);
        }

        protected override void ProcessDateConstantExpression(IEdmDateConstantExpression expression)
        {
            this.schemaWriter.WriteDateConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the date constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessDateConstantExpressionAsync(IEdmDateConstantExpression expression)
        {
            return this.schemaWriter.WriteDateConstantExpressionElementAsync(expression);
        }

        protected override void ProcessDateTimeOffsetConstantExpression(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.schemaWriter.WriteDateTimeOffsetConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the date time offset constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessDateTimeOffsetConstantExpressionAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            return this.schemaWriter.WriteDateTimeOffsetConstantExpressionElementAsync(expression);
        }

        protected override void ProcessDurationConstantExpression(IEdmDurationConstantExpression expression)
        {
            this.schemaWriter.WriteDurationConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the duration constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessDurationConstantExpressionAsync(IEdmDurationConstantExpression expression)
        {
            return this.schemaWriter.WriteDurationConstantExpressionElementAsync(expression);
        }

        protected override void ProcessTimeOfDayConstantExpression(IEdmTimeOfDayConstantExpression expression)
        {
            this.schemaWriter.WriteTimeOfDayConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the time of day constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessTimeOfDayConstantExpressionAsync(IEdmTimeOfDayConstantExpression expression)
        {
            return this.schemaWriter.WriteTimeOfDayConstantExpressionElementAsync(expression);
        }

        protected override void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression)
        {
            this.schemaWriter.WriteBooleanConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the boolean constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessBooleanConstantExpressionAsync(IEdmBooleanConstantExpression expression)
        {
            return this.schemaWriter.WriteBooleanConstantExpressionElementAsync(expression);
        }

        protected override void ProcessNullConstantExpression(IEdmNullExpression expression)
        {
            this.schemaWriter.WriteNullConstantExpressionElement(expression);
        }

        /// <summary>
        /// Asynchronously processes the null constant expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Task ProcessNullConstantExpressionAsync(IEdmNullExpression expression)
        {
            return this.schemaWriter.WriteNullConstantExpressionElementAsync(expression);
        }

        protected override void ProcessCastExpression(IEdmCastExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);

            if (this.isXml)
            {
                this.BeginElement(expression, (IEdmCastExpression t) => { this.schemaWriter.WriteCastExpressionElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
                if (!inlineType)
                {
                    VisitTypeReference(expression.Type);
                }

                this.VisitExpression(expression.Operand);
                this.EndElement(expression);
            }
            else
            {
                this.BeginElement(expression, (IEdmCastExpression t) => { this.schemaWriter.WriteCastExpressionElementHeader(t, inlineType); });

                this.VisitExpression(expression.Operand);

                this.schemaWriter.WriteCastExpressionType(expression, inlineType);
                this.ProcessFacets(expression.Type, inlineType);

                this.EndElement(expression, t => this.schemaWriter.WriteCastExpressionElementEnd(t, inlineType));
            }
        }

        /// <summary>
        /// Asynchronously processes the cast expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override async Task ProcessCastExpressionAsync(IEdmCastExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);

            if (this.isXml)
            {
                await this.BeginElementAsync(
                        expression, 
                        async (IEdmCastExpression t) => { await this.schemaWriter.WriteCastExpressionElementHeaderAsync(t, inlineType).ConfigureAwait(false); }, 
                        async (e) => { await this.ProcessFacetsAsync(e.Type, inlineType).ConfigureAwait(false); }
                    ).ConfigureAwait(false);

                if (!inlineType)
                {
                    VisitTypeReference(expression.Type);
                }

                this.VisitExpression(expression.Operand);
                await this.EndElementAsync(expression).ConfigureAwait(false);
            }
            else
            {
                await this.BeginElementAsync(
                        expression, 
                        async (IEdmCastExpression t) => { await this.schemaWriter.WriteCastExpressionElementHeaderAsync(t, inlineType).ConfigureAwait(false); }
                    ).ConfigureAwait(false);

                this.VisitExpression(expression.Operand);

                await this.schemaWriter.WriteCastExpressionTypeAsync(expression, inlineType).ConfigureAwait(false);
                await this.ProcessFacetsAsync(expression.Type, inlineType).ConfigureAwait(false);

                await this.EndElementAsync(
                        expression, 
                        async (t) => await this.schemaWriter.WriteCastExpressionElementEndAsync(t, inlineType).ConfigureAwait(false)
                    ).ConfigureAwait(false);
            }
        }

        #endregion

        private void ProcessNavigationPropertyBindings(IEdmNavigationSource navigationSource)
        {
            if (navigationSource != null && navigationSource.NavigationPropertyBindings.Any())
            {
                this.schemaWriter.WriteNavigationPropertyBindingsBegin(navigationSource.NavigationPropertyBindings);

                foreach (IEdmNavigationPropertyBinding binding in navigationSource.NavigationPropertyBindings)
                {
                    this.schemaWriter.WriteNavigationPropertyBinding(binding);
                }

                this.schemaWriter.WriteNavigationPropertyBindingsEnd(navigationSource.NavigationPropertyBindings);
            }
        }

        /// <summary>
        /// Asynchronously processes the navigation property bindings.
        /// </summary>
        /// <param name="navigationSource"></param>
        /// <returns></returns>
        private async Task ProcessNavigationPropertyBindingsAsync(IEdmNavigationSource navigationSource)
        {
            if (navigationSource != null && navigationSource.NavigationPropertyBindings.Any())
            {
                await this.schemaWriter.WriteNavigationPropertyBindingsBeginAsync(navigationSource.NavigationPropertyBindings).ConfigureAwait(false);

                foreach (IEdmNavigationPropertyBinding binding in navigationSource.NavigationPropertyBindings)
                {
                    await this.schemaWriter.WriteNavigationPropertyBindingAsync(binding).ConfigureAwait(false);
                }

                await this.schemaWriter.WriteNavigationPropertyBindingsEndAsync(navigationSource.NavigationPropertyBindings).ConfigureAwait(false);
            }
        }

        private static bool IsInlineType(IEdmTypeReference reference)
        {
            if (reference.Definition is IEdmSchemaElement || reference.IsEntityReference())
            {
                return true;
            }
            else if (reference.IsCollection())
            {
                return reference.AsCollection().CollectionDefinition().ElementType.Definition is IEdmSchemaElement;
            }

            return false;
        }

        private static bool IsInlineExpression(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                case EdmExpressionKind.BooleanConstant:
                case EdmExpressionKind.DateConstant:
                case EdmExpressionKind.DateTimeOffsetConstant:
                case EdmExpressionKind.DecimalConstant:
                case EdmExpressionKind.DurationConstant:
                case EdmExpressionKind.FloatingConstant:
                case EdmExpressionKind.GuidConstant:
                case EdmExpressionKind.IntegerConstant:
                case EdmExpressionKind.Path:
                case EdmExpressionKind.PropertyPath:
                case EdmExpressionKind.NavigationPropertyPath:
                case EdmExpressionKind.AnnotationPath:
                case EdmExpressionKind.StringConstant:
                case EdmExpressionKind.TimeOfDayConstant:
                    return true;
            }

            return false;
        }

        private static string GetEntitySetString(IEdmOperationImport operationImport)
        {
            if (operationImport.EntitySet != null)
            {
                var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                if (pathExpression != null)
                {
                    return EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments);
                }
            }

            return null;
        }

        private void ProcessOperation<TOperation>(TOperation operation, Action<TOperation> writeElementAction) where TOperation : IEdmOperation
        {
            this.BeginElement(operation, writeElementAction);

            this.schemaWriter.WriteOperationParametersBegin(operation.Parameters);
            this.VisitOperationParameters(operation.Parameters);
            this.schemaWriter.WriteOperationParametersEnd(operation.Parameters);

            IEdmOperationReturn operationReturn = operation.GetReturn();
            this.ProcessOperationReturn(operationReturn);

            this.EndElement(operation);
        }

        /// <summary>
        /// Asynchronously processes the operation.
        /// </summary>
        /// <typeparam name="TOperation"></typeparam>
        /// <param name="operation"></param>
        /// <param name="writeElementAction"></param>
        /// <returns></returns>
        private async Task ProcessOperationAsync<TOperation>(TOperation operation, Func<TOperation, Task> writeElementAction) where TOperation : IEdmOperation
        {
            await this.BeginElementAsync(operation, writeElementAction).ConfigureAwait(false);

            await this.schemaWriter.WriteOperationParametersBeginAsync(operation.Parameters).ConfigureAwait(false);
            this.VisitOperationParameters(operation.Parameters);
            await this.schemaWriter.WriteOperationParametersEndAsync(operation.Parameters).ConfigureAwait(false);

            IEdmOperationReturn operationReturn = operation.GetReturn();
            await this.ProcessOperationReturnAsync(operationReturn).ConfigureAwait(false);

            await this.EndElementAsync(operation).ConfigureAwait(false);
        }

        private void ProcessReferentialConstraint(IEdmReferentialConstraint element)
        {
            if (element != null)
            {
                this.schemaWriter.WriteReferentialConstraintBegin(element);
                foreach (var pair in element.PropertyPairs)
                {
                    this.schemaWriter.WriteReferentialConstraintPair(pair);
                }

                this.schemaWriter.WriteReferentialConstraintEnd(element);
            }
        }

        /// <summary>
        /// Asynchronously processes the referential constraint.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private async Task ProcessReferentialConstraintAsync(IEdmReferentialConstraint element)
        {
            if (element != null)
            {
                await this.schemaWriter.WriteReferentialConstraintBeginAsync(element).ConfigureAwait(false);
                foreach (var pair in element.PropertyPairs)
                {
                    await this.schemaWriter.WriteReferentialConstraintPairAsync(pair).ConfigureAwait(false);
                }

                await this.schemaWriter.WriteReferentialConstraintEndAsync(element).ConfigureAwait(false);
            }
        }

        private void ProcessFacets(IEdmTypeReference element, bool inlineType)
        {
            if (element != null)
            {
                if (element.IsEntityReference())
                {
                    // No facets get serialized for an entity reference.
                    return;
                }

                if (inlineType)
                {
                    if (element.TypeKind() == EdmTypeKind.Collection)
                    {
                        IEdmCollectionTypeReference collectionElement = element.AsCollection();
                        this.schemaWriter.WriteNullableAttribute(collectionElement.CollectionDefinition().ElementType);
                        VisitTypeReference(collectionElement.CollectionDefinition().ElementType);
                    }
                    else
                    {
                        this.schemaWriter.WriteNullableAttribute(element);
                        VisitTypeReference(element);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously processes the facets.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="inlineType"></param>
        /// <returns></returns>
        private async Task ProcessFacetsAsync(IEdmTypeReference element, bool inlineType)
        {
            if (element != null)
            {
                if (element.IsEntityReference())
                {
                    // No facets get serialized for an entity reference.
                    return;
                }

                if (inlineType)
                {
                    if (element.TypeKind() == EdmTypeKind.Collection)
                    {
                        IEdmCollectionTypeReference collectionElement = element.AsCollection();
                        await this.schemaWriter.WriteNullableAttributeAsync(collectionElement.CollectionDefinition().ElementType).ConfigureAwait(false);
                        VisitTypeReference(collectionElement.CollectionDefinition().ElementType);
                    }
                    else
                    {
                        await this.schemaWriter.WriteNullableAttributeAsync(element).ConfigureAwait(false);
                        VisitTypeReference(element);
                    }
                }
            }
        }

        private void VisitEntityTypeDeclaredKey(IEnumerable<IEdmStructuralProperty> keyProperties)
        {
            this.schemaWriter.WriteDeclaredKeyPropertiesElementHeader();
            this.VisitPropertyRefs(keyProperties);
            this.schemaWriter.WriteArrayEndElement();
        }

        /// <summary>
        /// Asynchronously visits the entity type declared key.
        /// </summary>
        /// <param name="keyProperties"></param>
        /// <returns></returns>
        private async Task VisitEntityTypeDeclaredKeyAsync(IEnumerable<IEdmStructuralProperty> keyProperties)
        {
            await this.schemaWriter.WriteDeclaredKeyPropertiesElementHeaderAsync().ConfigureAwait(false);
            await this.VisitPropertyRefsAsync(keyProperties).ConfigureAwait(false);
            await this.schemaWriter.WriteArrayEndElementAsync().ConfigureAwait(false);
        }

        private void VisitPropertyRefs(IEnumerable<IEdmStructuralProperty> properties)
        {
            foreach (IEdmStructuralProperty property in properties)
            {
                this.schemaWriter.WritePropertyRefElement(property);
            }
        }

        /// <summary>
        /// Asynchronously visits the property references.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private async Task VisitPropertyRefsAsync(IEnumerable<IEdmStructuralProperty> properties)
        {
            foreach (IEdmStructuralProperty property in properties)
            {
                await this.schemaWriter.WritePropertyRefElementAsync(property).ConfigureAwait(false);
            }
        }

        private void VisitAttributeAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri)
                {
                    var edmValue = annotation.Value as IEdmValue;
                    if (edmValue != null)
                    {
                        if (!edmValue.IsSerializedAsElement(this.Model))
                        {
                            if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                            {
                                this.ProcessAttributeAnnotation(annotation);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously visits the attribute annotations.
        /// </summary>
        /// <param name="annotations"></param>
        /// <returns></returns>
        private async Task VisitAttributeAnnotationsAsync(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri && annotation.Value is IEdmValue edmValue && !edmValue.IsSerializedAsElement(this.Model))
                {
                    if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                    {
                        await this.ProcessAttributeAnnotationAsync(annotation).ConfigureAwait(false);
                    }
                }
            }
        }

        private void VisitPrimitiveElementAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri)
                {
                    var edmValue = annotation.Value as IEdmValue;
                    if (edmValue != null)
                    {
                        if (edmValue.IsSerializedAsElement(this.Model))
                        {
                            if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                            {
                                this.ProcessElementAnnotation(annotation);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously visits the primitive element annotations.
        /// </summary>
        /// <param name="annotations"></param>
        /// <returns></returns>
        private async Task VisitPrimitiveElementAnnotationsAsync(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri && annotation.Value is IEdmValue edmValue && edmValue.IsSerializedAsElement(this.Model))
                {
                    if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                    {
                        await this.ProcessElementAnnotationAsync(annotation).ConfigureAwait(false);
                    }
                }
            }
        }

        private void ProcessAttributeAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.schemaWriter.WriteAnnotationStringAttribute(annotation);
        }

        /// <summary>
        /// Asynchronously processes the attribute annotation.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        private Task ProcessAttributeAnnotationAsync(IEdmDirectValueAnnotation annotation)
        {
            return this.schemaWriter.WriteAnnotationStringAttributeAsync(annotation);
        }

        private void ProcessElementAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.schemaWriter.WriteAnnotationStringElement(annotation);
        }

        /// <summary>
        /// Asynchronously processes the element annotation.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        private Task ProcessElementAnnotationAsync(IEdmDirectValueAnnotation annotation)
        {
            return this.schemaWriter.WriteAnnotationStringElementAsync(annotation);
        }

        private void VisitElementVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                this.ProcessAnnotation(annotation);
            }
        }

        /// <summary>
        /// Asynchronously visits the element vocabulary annotations.
        /// </summary>
        /// <param name="annotations"></param>
        /// <returns></returns>
        private async Task VisitElementVocabularyAnnotationsAsync(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                await this.ProcessAnnotationAsync(annotation).ConfigureAwait(false);
            }
        }

        private void BeginElement<TElement>(TElement element, Action<TElement> elementHeaderWriter, params Action<TElement>[] additionalAttributeWriters)
            where TElement : IEdmElement
        {
            elementHeaderWriter(element);
            if (additionalAttributeWriters != null)
            {
                foreach (Action<TElement> action in additionalAttributeWriters)
                {
                    action(element);
                }
            }

            this.VisitAttributeAnnotations(this.Model.DirectValueAnnotations(element));
        }

        /// <summary>
        /// Asynchronously begins the element.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="element"></param>
        /// <param name="elementHeaderWriter"></param>
        /// <param name="additionalAttributeWriters"></param>
        /// <returns></returns>
        private async Task BeginElementAsync<TElement>(TElement element, Func<TElement, Task> elementHeaderWriter, params Func<TElement, Task>[] additionalAttributeWriters)
            where TElement : IEdmElement
        {
            await elementHeaderWriter(element).ConfigureAwait(false);
            if (additionalAttributeWriters != null)
            {
                foreach (Func<TElement, Task> action in additionalAttributeWriters)
                {
                    await action(element).ConfigureAwait(false);
                }
            }

            await this.VisitAttributeAnnotationsAsync(this.Model.DirectValueAnnotations(element)).ConfigureAwait(false);
        }

        private void EndElement<TElement>(TElement element, Action<TElement> elementEndWriter) where TElement : IEdmElement
        {
            this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));

            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model)));
            }

            elementEndWriter(element);
        }

        /// <summary>
        /// Asynchronously ends the element.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="element"></param>
        /// <param name="elementEndWriter"></param>
        /// <returns></returns>
        private async Task EndElementAsync<TElement>(TElement element, Func<TElement, Task> elementEndWriter) where TElement : IEdmElement
        {
            await this.VisitPrimitiveElementAnnotationsAsync(this.Model.DirectValueAnnotations(element)).ConfigureAwait(false);

            if (element is IEdmVocabularyAnnotatable vocabularyAnnotatableElement)
            {
                await this.VisitElementVocabularyAnnotationsAsync(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model))).ConfigureAwait(false);
            }

            await elementEndWriter(element).ConfigureAwait(false);
        }

        private void EndElement(IEdmElement element)
        {
            this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model)));
            }

            this.schemaWriter.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously ends the element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private async Task EndElementAsync(IEdmElement element)
        {
            await this.VisitPrimitiveElementAnnotationsAsync(this.Model.DirectValueAnnotations(element)).ConfigureAwait(false);
            if (element is IEdmVocabularyAnnotatable vocabularyAnnotatableElement)
            {
                await this.VisitElementVocabularyAnnotationsAsync(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model))).ConfigureAwait(false);
            }

            await this.schemaWriter.WriteEndElementAsync().ConfigureAwait(false);
        }
    }
}
