//---------------------------------------------------------------------
// <copyright file="CompositeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Composite annotation which consists of scalar properties.
    /// Automatically serialized to CSDL as custom Taupo annotation.
    /// </summary>
    public abstract class CompositeAnnotation : Annotation
    {
    }
}