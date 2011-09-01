//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
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
        private readonly static Func<CsdlSemanticsEntityContainer, IEnumerable<IEdmEntityContainerElement>> s_computeElements = (me) => me.ComputeElements();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> entitySetDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>>();
        private readonly static Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmEntitySet>> s_computeEntitySetDictionary = (me) => me.ComputeEntitySetDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmAssociationSet>> associationSetDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, IEdmAssociationSet>>();
        private readonly static Func<CsdlSemanticsEntityContainer, Dictionary<string, IEdmAssociationSet>> s_computeAssociationSetDictionary = (me) => me.ComputeAssociationSetDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>> functionImportsDictionaryCache = new Cache<CsdlSemanticsEntityContainer, Dictionary<string, object>>();
        private readonly static Func<CsdlSemanticsEntityContainer, Dictionary<string, object>> s_computeFunctionImportsDictionary = (me) => me.ComputeFunctionImportsDictionary();

        private readonly Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer> extendsCache = new Cache<CsdlSemanticsEntityContainer, IEdmEntityContainer>();
        private readonly static Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> s_computeExtends = (me) => me.ComputeExtends();
        private readonly static Func<CsdlSemanticsEntityContainer, IEdmEntityContainer> s_onCycleExtends = (me) => new CyclicEntityContainer(me.entityContainer.Extends, me.Location);

        public CsdlSemanticsEntityContainer(CsdlSemanticsSchema context, CsdlEntityContainer entityContainer)
        {
            this.context = context;
            this.entityContainer = entityContainer;
            this.SetEntityContainerSchemaNamespace(context.Namespace);
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        internal CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public IEnumerable<IEdmEntityContainerElement> Elements
        {
            get { return this.elementsCache.GetValue(this, s_computeElements, null); }
        }

        private IEdmEntityContainer Extends
        {
            get { return this.extendsCache.GetValue(this, s_computeExtends, s_onCycleExtends); }
        }

        public string Name
        {
            get { return this.entityContainer.Name; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.Extends != null && this.Extends.IsBad())
                {
                    return ((IEdmCheckable)this.Extends).Errors;
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        public override CsdlElement Element
        {
            get { return this.entityContainer; }
        }

        private Dictionary<string, IEdmEntitySet> EntitySetDictionary
        {
            get { return this.entitySetDictionaryCache.GetValue(this, s_computeEntitySetDictionary, null); }
        }

        private Dictionary<string, IEdmAssociationSet> AssociationSetDictionary
        {
            get { return this.associationSetDictionaryCache.GetValue(this, s_computeAssociationSetDictionary, null); }
        }

        private Dictionary<string, object> FunctionImportsDictionary
        {
            get { return this.functionImportsDictionaryCache.GetValue(this, s_computeFunctionImportsDictionary, null); }
        }

        public IEdmEntitySet FindEntitySet(string name)
        {
            IEdmEntitySet element;
            return this.EntitySetDictionary.TryGetValue(name, out element) ? element : null;
        }

        public IEdmAssociationSet FindAssociationSet(string name)
        {
            IEdmAssociationSet element;
            return this.AssociationSetDictionary.TryGetValue(name, out element) ? element : null;
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

        private IEnumerable<IEdmEntityContainerElement> ComputeElements()
        {
            List<IEdmEntityContainerElement> elements = new List<IEdmEntityContainerElement>();
            if (this.entityContainer.Extends != null)
            {
                elements.AddRange(this.Extends.Elements);
            }

            foreach (CsdlEntitySet entitySet in this.entityContainer.EntitySets)
            {
                CsdlSemanticsEntitySet semanticsSet = new CsdlSemanticsEntitySet(this, entitySet);
                this.Model.RegisterContainerElement(this, semanticsSet);
                elements.Add(semanticsSet);
            }

            foreach (CsdlAssociationSet associationSet in this.entityContainer.AssociationSets)
            {
                CsdlSemanticsAssociationSet semanticsSet = new CsdlSemanticsAssociationSet(this, associationSet);
                this.Model.RegisterContainerElement(this, semanticsSet);
                elements.Add(semanticsSet);
            }

            foreach (CsdlFunctionImport functionImport in this.entityContainer.FunctionImports)
            {
                CsdlSemanticsFunctionImport semanticsFunction = new CsdlSemanticsFunctionImport(this, functionImport);
                this.Model.RegisterContainerElement(this, semanticsFunction);
                elements.Add(semanticsFunction);
            }

            return elements;
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

        private Dictionary<string, IEdmAssociationSet> ComputeAssociationSetDictionary()
        {
            Dictionary<string, IEdmAssociationSet> sets = new Dictionary<string, IEdmAssociationSet>();
            foreach (IEdmAssociationSet associationSet in this.Elements.OfType<IEdmAssociationSet>())
            {
                RegistrationHelper.AddElement(associationSet, associationSet.Name, sets, RegistrationHelper.CreateAmbiguousAssociationSetBinding);
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
                IEdmEntityContainer extendsContainer = this.Model.FindEntityContainer(this.entityContainer.Extends) as IEdmEntityContainer;
                CsdlSemanticsEntityContainer csdlContainer = extendsContainer as CsdlSemanticsEntityContainer;
                if (csdlContainer != null)
                {
                    IEdmEntityContainer junk = csdlContainer.Extends; // Evaluate the inductive step to detect cycles.
                }

                return extendsContainer ?? new UnresolvedEntityContainer(this.entityContainer.Extends, this.Location);
            }

            return null;
        }
    }
}
