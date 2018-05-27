//---------------------------------------------------------------------
// <copyright file="UnknownEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    internal class UnknownEntitySet : IEdmUnknownEntitySet
    {
        private readonly IEdmNavigationProperty navigationProperty;
        private readonly IEdmNavigationSource parentNavigationSource;
        private IEdmPathExpression path;

        public UnknownEntitySet(IEdmNavigationSource parentNavigationSource, IEdmNavigationProperty navigationProperty)
        {
            this.parentNavigationSource = parentNavigationSource;
            this.navigationProperty = navigationProperty;
        }

        public string Name
        {
            get { return this.navigationProperty.Name; }
        }

        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get { return null; }
        }

        public IEdmPathExpression Path
        {
            get { return this.path ?? (this.path = ComputePath()); }
        }

        public IEdmType Type
        {
            get
            {
                return this.navigationProperty.Type.Definition;
            }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            return null;
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            return null;
        }

        public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            return null;
        }

        private IEdmPathExpression ComputePath()
        {
            List<string> newPath = new List<string>(this.parentNavigationSource.Path.PathSegments);
            newPath.Add(this.navigationProperty.Name);
            return new EdmPathExpression(newPath.ToArray());
        }
    }
}
