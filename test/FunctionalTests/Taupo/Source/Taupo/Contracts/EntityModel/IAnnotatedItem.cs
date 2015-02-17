//---------------------------------------------------------------------
// <copyright file="IAnnotatedItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Interface for a Metadata item that can have annotations
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Used to name the generic annotatable more appropriately.")]
    public interface IAnnotatedItem : IAnnotatable<Annotation>
    {
    }
}
