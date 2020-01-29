//---------------------------------------------------------------------
// <copyright file="IDependencyTrigger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Interface describing anything that can be depended upon in tracking semantic changes in an EDM model.
    /// </summary>
    internal interface IDependencyTrigger
    {
        HashSetInternal<IDependent> Dependents { get; }
    }
}
