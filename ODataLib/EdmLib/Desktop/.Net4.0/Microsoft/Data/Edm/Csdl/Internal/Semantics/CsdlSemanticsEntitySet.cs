//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
