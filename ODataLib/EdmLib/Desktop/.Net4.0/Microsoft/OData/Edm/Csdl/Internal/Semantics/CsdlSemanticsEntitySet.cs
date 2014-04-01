//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
    using Microsoft.OData.Edm.Internal;
    using Microsoft.OData.Edm.Library;

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
            if (this.entitySet.NavigationPropertyBindings.Any())
            {
                return this.entitySet.NavigationPropertyBindings.Select(this.CreateSemanticMappingForBinding).ToList();
            }

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

        private IEdmNavigationTargetMapping CreateSemanticMappingForBinding(CsdlNavigationPropertyBinding binding)
        {
            IEdmNavigationProperty navigationProperty = this.ResolveNavigationPropertyPathForBinding(binding);
            
            IEdmEntitySet targetEntitySet = this.Container.FindEntitySet(binding.Target);
            if (targetEntitySet == null)
            {
                targetEntitySet = new UnresolvedEntitySet(binding.Target, this.Container, binding.Location);
            }

            return new EdmNavigationTargetMapping(navigationProperty, targetEntitySet);
        }

        private IEdmNavigationProperty ResolveNavigationPropertyPathForBinding(CsdlNavigationPropertyBinding binding)
        {
            Debug.Assert(binding != null, "binding != null");

            string bindingPath = binding.Path;
            Debug.Assert(bindingPath != null, "bindingPath != null");

            IEdmEntityType definingType = this.ElementType;

            const char Slash = '/';
            if (bindingPath.Length == 0 || bindingPath[bindingPath.Length - 1] == Slash)
            {
                // if the path did not actually contain a navigation property, then treat it as an unresolved path.
                // TODO: improve the error given in this case? (Task #1459901)
                return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
            }

            // for now, we don't support full path expressions, but we need to support type-qualified property names
            // in order to reach parity with V4. For now, if the path contains a slash, assume it is a derived type name
            // and try to parse it as such. If for any reason it does not fit this case, return a bad entity type and
            // a bad navigation property.
            int lastIndexOfSlash = bindingPath.LastIndexOf(Slash);
            if (lastIndexOfSlash >= 0)
            {
                string derivedTypeName = bindingPath.Substring(0, lastIndexOfSlash);
                if (derivedTypeName.Length == 0 || derivedTypeName.IndexOf(Slash) >= 0)
                {
                    // Either the slash is the first character, or there are multiple slashes,
                    // so this is not a valid type-qualified property.
                    // TODO: improve the error given in this case? (Task #1459901)
                    return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
                }

                IEdmEntityType derivedType = this.container.Context.FindType(derivedTypeName) as IEdmEntityType;
                if (derivedType == null || !derivedType.IsOrInheritsFrom(definingType))
                {
                    // either the type name could not be resolved, or it resolved to a type that does not derive from the current type.
                    // TODO: improve the error given in this case? (Task #1459901)
                    return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
                }

                bindingPath = bindingPath.Substring(lastIndexOfSlash + 1);
                definingType = derivedType;
            }

            // finally, just look up the navigation property. If it isn't found or is not a navigation, treat
            // this as an unresolved path.
            IEdmNavigationProperty navigationProperty = definingType.FindProperty(bindingPath) as IEdmNavigationProperty;
            if (navigationProperty == null)
            {
                navigationProperty = new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
            }

            return navigationProperty;
        }
    }
}

