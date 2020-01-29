//---------------------------------------------------------------------
// <copyright file="IDependent.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Interface describing anything that can be dependent on a dependency trigger in tracking semantic changes in an EDM model.
    /// </summary>
    internal interface IDependent : IFlushCaches
    {
        HashSetInternal<IDependencyTrigger> DependsOn { get; }
    }
}
