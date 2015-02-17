//---------------------------------------------------------------------
// <copyright file="ActionOperationRightsAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation for marking the rights of an action
    /// </summary>
    public class ActionOperationRightsAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the Action Rights
        /// </summary>
        public ActionOperationRights Value { get; set; }
    }
}