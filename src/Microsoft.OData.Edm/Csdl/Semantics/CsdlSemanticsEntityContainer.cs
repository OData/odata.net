//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntityContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntityContainer.
    /// </summary>
    internal class CsdlSemanticsEntityContainer : CsdlSemanticsElement, IEdmEntityContainer, IEdmCheckable
    {
        private readonly CsdlEntityContainer entityContainer;
        private readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>> elementsCache = new Cache<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>>();
        private static readonly Func<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>> ComputeElementsFunc = (me) => me.ComputeElements();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> entitySetDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> ComputeEntitySetDictionaryFunc = (me) => me.ComputeEntitySetDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>> singletonDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmSingleton>> ComputeSingletonDictionaryFunc = (me) => me.ComputeSingletonDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>> operationImportsDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, object>> ComputeOperationImportsDictionaryFunc = (me) => me.ComputeOperationImportsDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsEntityContainer, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsEntityContainer, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        private readonly Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer> extendsCache = new Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer>();
        private static readonly Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> ComputeExtendsFunc = (me) => me.ComputeExtends();
        private static readonly Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> OnCycleExtendsFunc = (me) => new CyclicEntityContainer(me.entityContainer.Extends, me.Location);

        public CsdlSemanticsEntityContainer(CsdlSemanticsSchema context, CsdlEntityContainer entityContainer)
            : base(entityContainer)
        {
            this.context = context;
            this.entityContainer = entityContainer;
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.EntityContainer; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return this.elementsCache.GetValue(this, ComputeElementsFunc, null); }
        }

        public string Namespace
        {
            get { return this.context.Namespace; }
        }

        public string Name
        {
            get { return this.entityContainer.Name; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.entityContainer; }
        }

        internal CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        internal IEdmEntityContainer Extends
        {
            get { return this.extendsCache.GetValue(this, ComputeExtendsFunc, OnCycleExtendsFunc); }
        }

        private Dictionary<string, IEdmEntitySet> EntitySetDictionary
        {
            get { return this.entitySetDictionaryCache.GetValue(this, ComputeEntitySetDictionaryFunc, null); }
        }

        private Dictionary<string, IEdmSingleton> SingletonDictionary
        {
            get { return this.singletonDictionaryCache.GetValue(this, ComputeSingletonDictionaryFunc, null); }
        }

        private Dictionary<string, object> OperationImportsDictionary
        {
            get { return this.operationImportsDictionaryCache.GetValue(this, ComputeOperationImportsDictionaryFunc, null); }
        }

        public IEdmEntitySet FindEntitySet(string name)
        {
            IEdmEntitySet element;
            return this.EntitySetDictionary.TryGetValue(name, out element) ? element : null;
        }

        public IEdmSingleton FindSingleton(string name)
        {
            IEdmSingleton element;
            return this.SingletonDictionary.TryGetValue(name, out element) ? element : null;
        }

        public IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
        {
            object element;
            if (this.OperationImportsDictionary.TryGetValue(operationName, out element))
            {
                List<IEdmOperationImport> listElement = element as List<IEdmOperationImport>;
                if (listElement != null)
                {
                    return listElement;
                }

                return new IEdmOperationImport[] { (IEdmOperationImport)element };
            }

            return Enumerable.Empty<IEdmOperationImport>();
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEnumerable<IEdmEntityContainerElement> ComputeElements()
        {
            List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();

            // don't import this.Extends' elements.
            // (all IEdmxxx like IEdmEntityContainer should let extension methods handle cross model searches).
            foreach (CsdlEntitySet entitySet in this.entityContainer.EntitySets)
            {
                CsdlSemanticsEntitySet semanticsSet = new CsdlSemanticsEntitySet(this, entitySet);
                elements.Add(semanticsSet);
            }

            foreach (CsdlSingleton singleton in entityContainer.Singletons)
            {
                CsdlSemanticsSingleton semanticsSingleton = new CsdlSemanticsSingleton(this, singleton);
                elements.Add(semanticsSingleton);
            }

            foreach (CsdlOperationImport operationImport in this.entityContainer.OperationImports)
            {
                this.AddOperationImport(operationImport, elements);
            }

            return elements;
        }

        private void AddOperationImport(CsdlOperationImport operationImport, List<IEdmEntityContainerElement> elements)
        {
            var functionImport = operationImport as CsdlFunctionImport;
            var actionImport = operationImport as CsdlActionImport;
            CsdlSemanticsOperationImport semanticsOperation = null;
            EdmSchemaElementKind filterKind = EdmSchemaElementKind.Action;
            if (functionImport != null)
            {
                filterKind = EdmSchemaElementKind.Function;
            }

            // OperationImports only work with non-bound operations hence this extra logic in the where clause
            var operations = this.context.FindOperations(operationImport.SchemaOperationQualifiedTypeName).Where(o => o.SchemaElementKind == filterKind && !o.IsBound);

            int operationsCount = 0;
            foreach (IEdmOperation operation in operations)
            {
                if (functionImport != null)
                {
                    semanticsOperation = new CsdlSemanticsFunctionImport(this, functionImport, (IEdmFunction)operation);
                }
                else
                {
                    Debug.Assert(actionImport != null, "actionImport should not be null");
                    semanticsOperation = new CsdlSemanticsActionImport(this, actionImport, (IEdmAction)operation);
                }

                operationsCount++;
                elements.Add(semanticsOperation);
            }

            // If none have been created then its an unresolved operation.
            if (operationsCount == 0)
            {
                if (filterKind == EdmSchemaElementKind.Action)
                {
                    var action = new UnresolvedAction(operationImport.SchemaOperationQualifiedTypeName, Edm.Strings.Bad_UnresolvedOperation(operationImport.SchemaOperationQualifiedTypeName), operationImport.Location);
                    semanticsOperation = new CsdlSemanticsActionImport(this, actionImport, action);
                }
                else
                {
                    Debug.Assert(filterKind == EdmSchemaElementKind.Function, "Should be a function");
                    var function = new UnresolvedFunction(operationImport.SchemaOperationQualifiedTypeName, Edm.Strings.Bad_UnresolvedOperation(operationImport.SchemaOperationQualifiedTypeName), operationImport.Location);
                    semanticsOperation = new CsdlSemanticsFunctionImport(this, functionImport, function);
                }

                elements.Add(semanticsOperation);
            }
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = new List<EdmError>();
            if (this.Extends != null && this.Extends.IsBad())
            {
                errors.AddRange(((IEdmCheckable)this.Extends).Errors);
            }

            return errors;
        }

        private Dictionary<string, IEdmEntitySet> ComputeEntitySetDictionary()
        {
            Dictionary<string, IEdmEntitySet> sets = new Dictionary<string, IEdmEntitySet>();
            foreach (IEdmEntitySet entitySet in this.Elements.OfType<IEdmEntitySet>())
            {
                RegistrationHelper.AddElement(entitySet, entitySet.Name, sets, RegistrationHelper.CreateAmbiguousEntitySetBinding);
            }

            return sets;
        }

        private Dictionary<string, IEdmSingleton> ComputeSingletonDictionary()
        {
            Dictionary<string, IEdmSingleton> sets = new Dictionary<string, IEdmSingleton>();
            foreach (IEdmSingleton singleton in this.Elements.OfType<IEdmSingleton>())
            {
                RegistrationHelper.AddElement(singleton, singleton.Name, sets, RegistrationHelper.CreateAmbiguousSingletonBinding);
            }

            return sets;
        }

        private Dictionary<string, object> ComputeOperationImportsDictionary()
        {
            Dictionary<string, object> operationImports = new Dictionary<string, object>();
            foreach (IEdmOperationImport operationImport in this.Elements.OfType<IEdmOperationImport>())
            {
                RegistrationHelper.AddOperationImport(operationImport, operationImport.Name, operationImports);
            }

            return operationImports;
        }

        private IEdmEntityContainer ComputeExtends()
        {
            string containerFullNameExtended = this.entityContainer.Extends;
            if (containerFullNameExtended != null)
            {
                IEdmEntityContainer ret = this.Context.FindEntityContainer(containerFullNameExtended);
                return ret ?? new UnresolvedEntityContainer(this.entityContainer.Extends, this.Location);
            }

            return null;
        }
    }
}
