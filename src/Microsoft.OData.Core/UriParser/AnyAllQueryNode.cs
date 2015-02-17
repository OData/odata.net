//---------------------------------------------------------------------
// <copyright file="AnyAllQueryNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Query
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    #endregion Namespaces

    /// <summary>
    /// Query node representing an Any/All query.
    /// </summary>
    public abstract class AnyAllQueryNode : SingleValueQueryNode
    {
        /// <summary>
        /// The associated boolean expression
        /// </summary>
        public QueryNode Body
        {
            get;
            set;
        }

        /// <summary>
        /// The parent entity set or navigation property
        /// </summary>
        public QueryNode Source
        {
            get;
            set;
        }
    }
}