//---------------------------------------------------------------------
// <copyright file="ExpressionsWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !ClientSKUFramework
using Microsoft.OData.Service.Providers;
#endif
namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Creates workspaces for capturing and serializing expressions.
    //---------------------------------------------------------------------
    public class ExpressionsWorkspaces : BlobsFriendlyFeedsConcurrencyWorkspaces
    {
        //---------------------------------------------------------------------
        protected override void WorkspaceCallback(Workspace w)
        {
            // Enable expression capture callback.
            InjectCode(w, "ExpressionHook", "ExpressionTreeTestBaseVisitor", "ExpressionTreeToXmlSerializer");

            // Enable various features.
            base.WorkspaceCallback(w);

            // Fix all NonClr property types.
            AstoriaTestProperties.Seed = 2009;
        }
    }
}
