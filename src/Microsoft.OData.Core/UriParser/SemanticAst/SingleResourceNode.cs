//---------------------------------------------------------------------
// <copyright file="SingleResourceNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a resource, including entity and complex.
    /// </summary>
    public abstract class SingleResourceNode : SingleValueNode
    {
        /// <summary>
        /// Gets the navigation source containing this single entity/complex.
        /// </summary>
        public abstract IEdmNavigationSource NavigationSource { get; }

        /// <summary>
        /// Gets the type of this single entity/complex.
        /// </summary>
        public abstract IEdmStructuredTypeReference StructuredTypeReference { get; }
    }

    //public sealed class SingleResourceValueNode : SingleResourceNode
    //{
    //    private IList<KeyValuePair<string, QueryNode>> _properties = new List<KeyValuePair<string, QueryNode>>();
    //    private IEdmNavigationSource _navigationSource = null;
    //    private IEdmStructuredTypeReference _structuredTypeReference = null;

    //    public override IEdmNavigationSource NavigationSource => _navigationSource;

    //    public override IEdmStructuredTypeReference StructuredTypeReference => _structuredTypeReference;

    //    public IReadOnlyCollection<KeyValuePair<string, QueryNode>> Properties => _properties.AsReadOnly();

    //    public void SetMetadata(IEdmNavigationSource navigationSource, IEdmStructuredTypeReference structuredTypeReference)
    //    {
    //        _navigationSource = navigationSource;
    //        _structuredTypeReference = structuredTypeReference;
    //    }

    //    public void Add(string propertyName, QueryNode value)
    //    {
    //        _properties.Add(new KeyValuePair<string, QueryNode>(propertyName, value));
    //    }

    //    public override QueryNodeKind Kind
    //    {
    //        get { return QueryNodeKind.ResourceConstant; }
    //    }

    //    public override IEdmTypeReference TypeReference => _structuredTypeReference;
    //}
}
