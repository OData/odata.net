//---------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Test.OData.DependencyInjection
{
#if !NETCOREAPP1_0
    [CLSCompliant(false)]
#endif
    public static class ServiceProviderExtensions
    {
        public static ServiceScopeWrapper CreateServiceScope(this IServiceProvider container)
        {
            var innerScope = container.GetRequiredService<IServiceScopeFactory>().CreateScope();
            return new ServiceScopeWrapper(innerScope);
        }
    }
}
