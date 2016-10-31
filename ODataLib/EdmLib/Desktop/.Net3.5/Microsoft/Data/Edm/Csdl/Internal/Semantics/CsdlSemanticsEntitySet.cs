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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlEntitySet.
    /// </summary>
    internal class CsdlSemanticsEntitySet : CsdlSemanticsElement, IEdmEntitySet
    {
        private readonly CsdlEntitySet entitySet;
        private readonly CsdlSemanticsEntityContainer container;

        private readonly Cache<CsdlSemanticsEntitySet, IEdmEntityType> elementTypeCache = new Cache<CsdlSemanticsEntitySet, IEdmEntityType>();
        private static readonly Func<CsdlSemanticsEntitySet, IEdmEntityType> ComputeElementTypeFunc = (me) => me.ComputeElementType();

        private readonly Cache<CsdlSemanticsEntitySet, IEnumerable<IEdmNavigationTargetMapping>> navigationTargetsCache = new Cache<CsdlSemanticsEntitySet, IEnumerable<IEdmNavigationTargetMapping>>();
        private static readonly Func<CsdlSemanticsEntitySet, IEnumerable<IEdmNavigationTargetMapping>> ComputeNavigationTargetsFunc = (me) => me.ComputeNavigationTargets();

        public CsdlSemanticsEntitySet(CsdlSemanticsEntityContainer container, CsdlEntitySet entitySet)
            : base(entitySet)
        {
            this.container = container;
            this.entitySet = entitySet;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.container.Model; }
        }

        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        public override CsdlElement Element
        {
            get { return this.entitySet; }
        }

        public string Name
        {
            get { return this.entitySet.Name; }
        }

        public IEdmEntityType ElementType
        {
            get
            {
               return this.elementTypeCache.GetValue(this, ComputeElementTypeFunc, null);
            }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        public IEnumerable<IEdmNavigationTargetMapping> NavigationTargets
        {
            get { return this.navigationTargetsCache.GetValue(this, ComputeNavigationTargetsFunc, null); }
        }

        public IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty property)
        {
            foreach (IEdmNavigationTargetMapping targetMapping in this.NavigationTargets)
            {
                if (targetMapping.NavigationProperty == property)
                {
                    return targetMapping.TargetEntitySet;
                }
            }

            return null;
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.container.Context);
        }

        private IEdmEntityType ComputeElementType()
        {
            return this.container.Context.FindType(this.entitySet.EntityType) as IEdmEntityType ?? new UnresolvedEntityType(this.entitySet.EntityType, this.Location);
        }

        private IEnumerable<IEdmNavigationTargetMapping> ComputeNavigationTargets()
        {
            List<IEdmNavigationTargetMapping> targets = new List<IEdmNavigationTargetMapping>();

            foreach (IEdmNavigationProperty navigationProperty in this.ElementType.NavigationProperties())
            {
                IEdmEntitySet target = this.FindNavigationTargetHelper(navigationProperty);
                if (target != null)
                {
                    targets.Add(new EdmNavigationTargetMapping(navigationProperty, target));
                }
            }

            foreach (IEdmEntityType derivedType in this.Model.FindAllDerivedTypes(this.ElementType))
            {
                foreach (IEdmNavigationProperty navigationProperty in derivedType.DeclaredNavigationProperties())
                {
                    IEdmEntitySet target = this.FindNavigationTargetHelper(navigationProperty);
                    if (target != null)
                    {
                        targets.Add(new EdmNavigationTargetMapping(navigationProperty, target));
                    }
                }
            }

            return targets;
        }

        private IEdmEntitySet FindNavigationTargetHelper(IEdmNavigationProperty property)
        {
            CsdlSemanticsNavigationProperty navigationProperty = property as CsdlSemanticsNavigationProperty;
            if (navigationProperty != null)
            {
                foreach (CsdlSemanticsEntityContainer container in this.Model.EntityContainers())
                {
                    IEnumerable<CsdlSemanticsAssociationSet> associationSets = container.FindAssociationSets(navigationProperty.Association);
                    if (associationSets != null)
                    {
                        foreach (CsdlSemanticsAssociationSet associationSet in associationSets)
                        {
                            CsdlSemanticsAssociationSetEnd end1 = associationSet.End1 as CsdlSemanticsAssociationSetEnd;
                            CsdlSemanticsAssociationSetEnd end2 = associationSet.End2 as CsdlSemanticsAssociationSetEnd;
                            if (associationSet.End1.EntitySet == this && navigationProperty.To == associationSet.End2.Role)
                            {
                                this.Model.SetAssociationSetName(associationSet.End1.EntitySet, property, associationSet.Name);
                                if (end1 != null && end2 != null)
                                {
                                    this.Model.SetAssociationSetAnnotations(end1.EntitySet, property, associationSet.DirectValueAnnotations, end1.DirectValueAnnotations, end2.DirectValueAnnotations);
                                }

                                return associationSet.End2.EntitySet;
                            }
                            else if (associationSet.End2.EntitySet == this && navigationProperty.To == associationSet.End1.Role)
                            {
                                this.Model.SetAssociationSetName(associationSet.End2.EntitySet, property, associationSet.Name);
                                if (end1 != null && end2 != null)
                                {
                                    this.Model.SetAssociationSetAnnotations(end2.EntitySet, property, associationSet.DirectValueAnnotations, end2.DirectValueAnnotations, end1.DirectValueAnnotations);
                                }

                                return associationSet.End1.EntitySet;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
