//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Expressions;

    /// <summary>
    /// Provides semantics for CsdlAbstractNavigationSource.
    /// </summary>
    internal abstract class CsdlSemanticsNavigationSource : CsdlSemanticsElement, IEdmNavigationSource
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
        protected readonly CsdlAbstractNavigationSource navigationSource;
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
        protected readonly CsdlSemanticsEntityContainer container;
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
        protected readonly IEdmPathExpression path;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter", Justification = "protected field in internal class.")]
        protected readonly Cache<CsdlSemanticsNavigationSource, IEdmEntityType> typeCache = new Cache<CsdlSemanticsNavigationSource, IEdmEntityType>();
        protected static readonly Func<CsdlSemanticsNavigationSource, IEdmEntityType> ComputeElementTypeFunc = (me) => me.ComputeElementType();

        private readonly Cache<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> navigationTargetsCache = new Cache<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>>();
        private static readonly Func<CsdlSemanticsNavigationSource, IEnumerable<IEdmNavigationPropertyBinding>> ComputeNavigationTargetsFunc = (me) => me.ComputeNavigationTargets();

        private readonly Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet> containedNavigationPropertyCache =
            new Dictionary<IEdmNavigationProperty, IEdmContainedEntitySet>();

        private readonly Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet> unknownNavigationPropertyCache =
            new Dictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>();

        public CsdlSemanticsNavigationSource(CsdlSemanticsEntityContainer container, CsdlAbstractNavigationSource navigationSource)
            : base(navigationSource)
        {
            this.container = container;
            this.navigationSource = navigationSource;
            this.path = new EdmPathExpression(this.navigationSource.Name);
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
            get { return this.navigationSource; }
        }

        public string Name
        {
            get { return this.navigationSource.Name; }
        }

        public Expressions.IEdmPathExpression Path
        {
            get { return this.path; }
        }

        public abstract IEdmType Type { get; }

        public abstract EdmContainerElementKind ContainerElementKind { get; }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return this.navigationTargetsCache.GetValue(this, ComputeNavigationTargetsFunc, null); }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");

            if (!property.ContainsTarget)
            {
                foreach (IEdmNavigationPropertyBinding targetMapping in this.NavigationPropertyBindings)
                {
                    if (targetMapping.NavigationProperty == property)
                    {
                        return targetMapping.Target;
                    }
                }
            }
            else
            {
                IEdmContainedEntitySet result;
                if (this.containedNavigationPropertyCache.TryGetValue(property, out result))
                {
                    return result;
                }
                else
                {
                    result = new EdmContainedEntitySet(this, property);
                    this.containedNavigationPropertyCache.Add(property, result);
                }

                return result;
            }

            IEdmUnknownEntitySet unknownEntitySet;
            if (!this.unknownNavigationPropertyCache.TryGetValue(property, out unknownEntitySet))
            {
                unknownEntitySet = new EdmUnknownEntitySet(this, property);
                this.unknownNavigationPropertyCache.Add(property, unknownEntitySet);
            }

            return unknownEntitySet;
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.container.Context);
        }

        protected abstract IEdmEntityType ComputeElementType();

        private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
        {
            return this.navigationSource.NavigationPropertyBindings.Select(this.CreateSemanticMappingForBinding).ToList();
        }

        private IEdmNavigationPropertyBinding CreateSemanticMappingForBinding(CsdlNavigationPropertyBinding binding)
        {
            IEdmNavigationProperty navigationProperty = this.ResolveNavigationPropertyPathForBinding(binding);
            
            IEdmNavigationSource targetNavigationSource = this.Container.FindEntitySetExtended(binding.Target);
            if (targetNavigationSource == null)
            {
                targetNavigationSource = this.Container.FindSingletonExtended(binding.Target);
                if (targetNavigationSource == null)
                {
                    targetNavigationSource = new UnresolvedEntitySet(binding.Target, this.Container, binding.Location);
                }
            }

            return new EdmNavigationPropertyBinding(navigationProperty, targetNavigationSource);
        }

        private IEdmNavigationProperty ResolveNavigationPropertyPathForBinding(CsdlNavigationPropertyBinding binding)
        {
            Debug.Assert(binding != null, "binding != null");

            string bindingPath = binding.Path;
            Debug.Assert(bindingPath != null, "bindingPath != null");

            IEdmEntityType definingType = this.typeCache.GetValue(this, ComputeElementTypeFunc, null);

            const char Slash = '/';
            if (bindingPath.Length == 0 || bindingPath[bindingPath.Length - 1] == Slash)
            {
                // if the path did not actually contain a navigation property, then treat it as an unresolved path.
                // TODO: improve the error given in this case? (Task #1459901)
                return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
            }

            string[] pathSegements = bindingPath.Split(Slash);
            for (int index = 0; index < pathSegements.Length - 1; index++)
            {
                string pathSegement = pathSegements[index];
                if (pathSegement.Length == 0)
                {
                    // TODO: improve the error given in this case? (Task #1459901)
                    return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
                }

                IEdmEntityType derivedType = this.container.Context.FindType(pathSegement) as IEdmEntityType;
                if (derivedType == null)
                {
                    IEdmProperty property = definingType.FindProperty(pathSegement);
                    if (property == null || !(property is IEdmNavigationProperty))
                    {
                        return new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
                    }

                    definingType = (property as IEdmNavigationProperty).ToEntityType();
                }
                else
                {
                    definingType = derivedType;
                }
            }

            IEdmNavigationProperty navigationProperty = definingType.FindProperty(pathSegements.Last()) as IEdmNavigationProperty;
            if (navigationProperty == null)
            {
                // TODO: improve the error given in this case? (Task #1459901)
                navigationProperty = new UnresolvedNavigationPropertyPath(definingType, bindingPath, binding.Location);
            }

            return navigationProperty;
        }
    }
}
