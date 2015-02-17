//---------------------------------------------------------------------
// <copyright file="DataServiceMemberGeneratorAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Decorates an EntitySet with an annotation that indicates that Members will be added to the DataService Class
    /// </summary>
    public abstract class DataServiceMemberGeneratorAnnotation : CompositeAnnotation
    {
    }
}