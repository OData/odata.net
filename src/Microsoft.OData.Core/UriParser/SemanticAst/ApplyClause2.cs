//---------------------------------------------------------------------
// <copyright file="ApplyClause2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Edm;
    using System.Collections.Generic;
    using System.Linq;
    using TreeNodeKinds;
    using System;

    /// <summary>
    /// Represents the set of transformations to perform as part of $apply.
    /// </summary>
    public class ApplyClause2 : QueryNode
    {
        public ApplyClause2(IEnumerable<QueryNode> transformations, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(transformations, "transformations");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            this._transformations = transformations;
            this._typeReference = typeReference;
        }

        private readonly IEnumerable<QueryNode> _transformations;

        /// <summary>
        /// The collection of transformations to perform.
        /// </summary>
        public IEnumerable<QueryNode> Transformations
        {
            get
            {
                return this._transformations;
            }
        }

        private readonly IEdmTypeReference _typeReference;

        public IEdmTypeReference TypeReference
        {
            get
            {

                return this._typeReference;
            }
        }

        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.Apply;
            }
        }
    }
}
