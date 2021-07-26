//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsNavigationSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl.Parsing.Ast;
    using Microsoft.OData.Edm.Vocabularies;

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

        private readonly ConcurrentDictionary<IEdmNavigationProperty, IEdmContainedEntitySet> containedNavigationPropertyCache =
            new ConcurrentDictionary<IEdmNavigationProperty, IEdmContainedEntitySet>();

        private readonly ConcurrentDictionary<IEdmNavigationProperty, IEdmUnknownEntitySet> unknownNavigationPropertyCache =
            new ConcurrentDictionary<IEdmNavigationProperty, IEdmUnknownEntitySet>();

        private readonly ConcurrentDictionary<IEdmNavigationProperty, IEnumerable<IEdmNavigationPropertyBinding>> navigationPropertyBindingCache = 
            new ConcurrentDictionary<IEdmNavigationProperty, IEnumerable<IEdmNavigationPropertyBinding>>();

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

        public IEdmPathExpression Path
        {
            get { return this.path; }
        }

        public abstract IEdmType Type { get; }

        public abstract EdmContainerElementKind ContainerElementKind { get; }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return this.navigationTargetsCache.GetValue(this, ComputeNavigationTargetsFunc, null); }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property, IEdmPathExpression bindingPath)
        {
            EdmUtil.CheckArgumentNull(property, "property");

            if (!property.ContainsTarget && bindingPath != null)
            {
                foreach (IEdmNavigationPropertyBinding targetMapping in this.NavigationPropertyBindings)
                {
                    if (targetMapping.NavigationProperty == property && targetMapping.Path.Path == bindingPath.Path)
                    {
                        return targetMapping.Target;
                    }
                }
            }
            else if (property.ContainsTarget)
            {
                return EdmUtil.DictionaryGetOrUpdate(
                    this.containedNavigationPropertyCache,
                    property,
                    navProperty => new EdmContainedEntitySet(this, navProperty));
            }

            return EdmUtil.DictionaryGetOrUpdate(
                    this.unknownNavigationPropertyCache,
                    property,
                    navProperty => new EdmUnknownEntitySet(this, navProperty));
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            bool isDerived = !this.Type.AsElementType().IsOrInheritsFrom(navigationProperty.DeclaringType);

            IEdmPathExpression bindingPath = isDerived
                ? new EdmPathExpression(navigationProperty.DeclaringType.FullTypeName(), navigationProperty.Name)
                : new EdmPathExpression(navigationProperty.Name);

            return FindNavigationTarget(navigationProperty, bindingPath);
        }

        public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            if (!navigationProperty.ContainsTarget)
            {
                return EdmUtil.DictionaryGetOrUpdate(
                    this.navigationPropertyBindingCache,
                    navigationProperty,
                    property => this.NavigationPropertyBindings.Where(targetMapping => targetMapping.NavigationProperty == property).ToList());
            }

            return null;
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.container.Context);
        }

        protected abstract IEdmEntityType ComputeElementType();

        private IEnumerable<IEdmNavigationPropertyBinding> ComputeNavigationTargets()
        {
            return this.navigationSource.NavigationPropertyBindings.Select(this.CreateSemanticMappingForBinding);
        }

        private IEdmNavigationPropertyBinding CreateSemanticMappingForBinding(CsdlNavigationPropertyBinding binding)
        {
            IEdmNavigationProperty navigationProperty = this.ResolveNavigationPropertyPathForBinding(binding);

            IEdmNavigationSource targetNavigationSource = this.Container.FindNavigationSourceExtended(binding.Target);
            if (targetNavigationSource == null)
            {
                targetNavigationSource = this.Container.FindSingletonExtended(binding.Target);
                if (targetNavigationSource == null)
                {
                    targetNavigationSource = new UnresolvedEntitySet(binding.Target, this.Container, binding.Location);
                }
            }

            return new EdmNavigationPropertyBinding(navigationProperty, targetNavigationSource, new EdmPathExpression(binding.Path));
        }

        private IEdmNavigationProperty ResolveNavigationPropertyPathForBinding(CsdlNavigationPropertyBinding binding)
        {
            Debug.Assert(binding != null);
            Debug.Assert(binding.Path != null);
            var pathSegments = binding.Path.Split('/');
            IEdmStructuredType definingType = this.typeCache.GetValue(this, ComputeElementTypeFunc, null);
            for (int index = 0; index < pathSegments.Length - 1; index++)
            {
                string segment = pathSegments[index];
                if (segment.IndexOf('.') < 0)
                {
                    var property = definingType.FindProperty(segment);
                    if (property == null)
                    {
                        return new UnresolvedNavigationPropertyPath(definingType, binding.Path, binding.Location);
                    }

                    var navProperty = property as IEdmNavigationProperty;
                    if (navProperty != null && !navProperty.ContainsTarget)
                    {
                        // TODO: Improve error message #644.
                        return new UnresolvedNavigationPropertyPath(definingType, binding.Path, binding.Location);
                    }

                    definingType = property.Type.Definition.AsElementType() as IEdmStructuredType;
                    if (definingType == null)
                    {
                        // TODO: Improve error message #644.
                        return new UnresolvedNavigationPropertyPath(definingType, binding.Path, binding.Location);
                    }
                }
                else
                {
                    var derivedType = container.Context.FindType(segment) as IEdmStructuredType;
                    if (derivedType == null || !derivedType.IsOrInheritsFrom(definingType))
                    {
                        // TODO: Improve error message #644.
                        return new UnresolvedNavigationPropertyPath(definingType, binding.Path, binding.Location);
                    }

                    definingType = derivedType;
                }
            }

            return definingType.FindProperty(pathSegments.Last()) as IEdmNavigationProperty
                   ?? new UnresolvedNavigationPropertyPath(definingType, binding.Path, binding.Location);
        }
    }
}