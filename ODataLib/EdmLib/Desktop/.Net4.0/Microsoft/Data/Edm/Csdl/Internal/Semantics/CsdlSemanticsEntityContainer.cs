//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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

        private readonly Cache<CsdlSemanticsEntityContainer, IEnumerable<CsdlSemanticsAssociationSet>> associationSetsCache = new Cache<CsdlSemanticsEntityContainer, IEnumerable<CsdlSemanticsAssociationSet>>();
        private static readonly Func<CsdlSemanticsEntityContainer, IEnumerable<CsdlSemanticsAssociationSet>> ComputeAssociationSetsFunc = (me) => me.ComputeAssociationSets();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>>> associationSetMappingsCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>>> ComputeAssociationSetMappingsFunc = (me) => me.ComputeAssociationSetMappings();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> entitySetDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> ComputeEntitySetDictionaryFunc = (me) => me.ComputeEntitySetDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>> functionImportsDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>>();
        private static readonly Func<CsdlSemanticsEntityContainer, Dictionary<string, object>> ComputeFunctionImportsDictionaryFunc = (me) => me.ComputeFunctionImportsDictionary();

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

        public IEnumerable<CsdlSemanticsAssociationSet> AssociationSets
        {
            get { return this.associationSetsCache.GetValue(this, ComputeAssociationSetsFunc, null); }
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

        private IEdmEntityContainer Extends
        {
            get { return this.extendsCache.GetValue(this, ComputeExtendsFunc, OnCycleExtendsFunc); }
        }

        private Dictionary<string, IEdmEntitySet> EntitySetDictionary
        {
            get { return this.entitySetDictionaryCache.GetValue(this, ComputeEntitySetDictionaryFunc, null); }
        }

        private Dictionary<string, object> FunctionImportsDictionary
        {
            get { return this.functionImportsDictionaryCache.GetValue(this, ComputeFunctionImportsDictionaryFunc, null); }
        }

        private Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>> AssociationSetMappings
        {
            get { return this.associationSetMappingsCache.GetValue(this, ComputeAssociationSetMappingsFunc, null); }
        }

        public IEdmEntitySet FindEntitySet(string name)
        {
            IEdmEntitySet element;
            return this.EntitySetDictionary.TryGetValue(name, out element) ? element : null;
        }

        public IEnumerable<CsdlSemanticsAssociationSet> FindAssociationSets(IEdmAssociation association)
        {
            IEnumerable<CsdlSemanticsAssociationSet> result;
            return this.AssociationSetMappings.TryGetValue(association, out result) ? result : null;
        }

        public IEnumerable<IEdmFunctionImport> FindFunctionImports(string name)
        {
            object element;
            if (this.FunctionImportsDictionary.TryGetValue(name, out element))
            {
                List<IEdmFunctionImport> listElement = element as List<IEdmFunctionImport>;
                if (listElement != null)
                {
                    return listElement;
                }

                return new IEdmFunctionImport[] { (IEdmFunctionImport)element };
            }

            return Enumerable.Empty<IEdmFunctionImport>();
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEnumerable<IEdmEntityContainerElement> ComputeElements()
        {
            List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();

            // Extends is either a CsdlSemanticsEntityContainer or BadEntityContainer.
            CsdlSemanticsEntityContainer csdlEntityContainer = this.Extends as CsdlSemanticsEntityContainer;
            if (csdlEntityContainer != null)
            {
                foreach (CsdlEntitySet entitySet in csdlEntityContainer.entityContainer.EntitySets)
                {
                    CsdlSemanticsEntitySet semanticsSet = new CsdlSemanticsEntitySet(this, entitySet);
                    elements.Add(semanticsSet);
                }

                foreach (CsdlFunctionImport functionImport in csdlEntityContainer.entityContainer.FunctionImports)
                {
                    CsdlSemanticsFunctionImport semanticsFunction = new CsdlSemanticsFunctionImport(this, functionImport);
                    elements.Add(semanticsFunction);
                }
            }

            foreach (CsdlEntitySet entitySet in this.entityContainer.EntitySets)
            {
                CsdlSemanticsEntitySet semanticsSet = new CsdlSemanticsEntitySet(this, entitySet);
                elements.Add(semanticsSet);
            }

            foreach (CsdlFunctionImport functionImport in this.entityContainer.FunctionImports)
            {
                CsdlSemanticsFunctionImport semanticsFunction = new CsdlSemanticsFunctionImport(this, functionImport);
                elements.Add(semanticsFunction);
            }

            return elements;
        }

        private IEnumerable<CsdlSemanticsAssociationSet> ComputeAssociationSets()
        {
            List<CsdlSemanticsAssociationSet> associationSets = new List<CsdlSemanticsAssociationSet>();
            if (this.entityContainer.Extends != null)
            {
                // Extends is either a CsdlSemanticsEntityContainer or BadEntityContainer.
                CsdlSemanticsEntityContainer csdlExtends = this.Extends as CsdlSemanticsEntityContainer;
                if (csdlExtends != null)
                {
                    foreach (CsdlAssociationSet associationSet in csdlExtends.entityContainer.AssociationSets)
                    {
                        CsdlSemanticsAssociationSet semanticsSet = new CsdlSemanticsAssociationSet(this, associationSet);
                        associationSets.Add(semanticsSet);
                    }
                }
            }

            foreach (CsdlAssociationSet associationSet in this.entityContainer.AssociationSets)
            {
                CsdlSemanticsAssociationSet semanticsSet = new CsdlSemanticsAssociationSet(this, associationSet);
                associationSets.Add(semanticsSet);
            }

            return associationSets;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            List<EdmError> errors = new List<EdmError>();
            if (this.Extends != null && this.Extends.IsBad())
            {
                errors.AddRange(((IEdmCheckable)this.Extends).Errors);
            }

            foreach (CsdlSemanticsAssociationSet associationSet in this.AssociationSets)
            {
                int count = errors.Count;
                errors.AddRange(associationSet.Errors());
                if (errors.Count == count)
                {
                    errors.AddRange(associationSet.End1.Errors());
                    errors.AddRange(associationSet.End2.Errors());
                }
            }
                
            return errors;
        }

        private Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>> ComputeAssociationSetMappings()
        {
            Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>> associationSetMappings = new Dictionary<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>>();
            if (this.entityContainer.Extends != null)
            {
                // Extends is either a CsdlSemanticsEntityContainer or BadEntityContainer.
                CsdlSemanticsEntityContainer csdlExtends = this.Extends as CsdlSemanticsEntityContainer;
                if (csdlExtends != null)
                {
                    foreach (KeyValuePair<IEdmAssociation, IEnumerable<CsdlSemanticsAssociationSet>> mapping in csdlExtends.AssociationSetMappings)
                    {
                        associationSetMappings[mapping.Key] = new List<CsdlSemanticsAssociationSet>(mapping.Value);
                    }
                }
            }

            foreach (CsdlSemanticsAssociationSet associationSet in this.AssociationSets)
            {
                CsdlSemanticsAssociation association = associationSet.Association as CsdlSemanticsAssociation;
                if (association != null)
                {
                    IEnumerable<CsdlSemanticsAssociationSet> associationSets;
                    if (!associationSetMappings.TryGetValue(association, out associationSets))
                    {
                        associationSets = new List<CsdlSemanticsAssociationSet>();
                        associationSetMappings[association] = associationSets;
                    }

                    ((List<CsdlSemanticsAssociationSet>)associationSets).Add(associationSet);
                }
            }

            return associationSetMappings;
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

        private Dictionary<string, object> ComputeFunctionImportsDictionary()
        {
            Dictionary<string, object> functionImports = new Dictionary<string, object>();
            foreach (IEdmFunctionImport functionImport in this.Elements.OfType<IEdmFunctionImport>())
            {
                RegistrationHelper.AddFunction(functionImport, functionImport.Name, functionImports);
            }

            return functionImports;
        }

        private IEdmEntityContainer ComputeExtends()
        {
            if (this.entityContainer.Extends != null)
            {
                CsdlSemanticsEntityContainer csdlContainer = this.Model.FindDeclaredEntityContainer(this.entityContainer.Extends) as CsdlSemanticsEntityContainer;
                if (csdlContainer != null)
                {
                    IEdmEntityContainer junk = csdlContainer.Extends; // Evaluate the inductive step to detect cycles.
                    return csdlContainer;
                }

                return new UnresolvedEntityContainer(this.entityContainer.Extends, this.Location);
            }

            return null;
        }
    }
}
