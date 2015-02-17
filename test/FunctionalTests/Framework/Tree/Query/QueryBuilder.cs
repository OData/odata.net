//---------------------------------------------------------------------
// <copyright file="QueryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;

    /// <summary>Base class for node visitors that can produce a string.</summary>
    public abstract class QueryBuilder : NodeVisitor<ExpNode, String>
    {
        //Data
        protected readonly Workspace _workspace;
        
        //Constructor
        protected QueryBuilder(Workspace workspace)
        {
            _workspace = workspace;
        }

        //Helpers
        public virtual String           Build(ExpNode node)
        {
            return this.Visit(null, node);
        }

        public abstract void            Execute(params ExpNode[] tree);
    }
}
