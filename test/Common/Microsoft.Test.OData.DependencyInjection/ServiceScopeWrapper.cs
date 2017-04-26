//---------------------------------------------------------------------
// <copyright file="ServiceScopeWrapper.cs" company="Microsoft">
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
    public class ServiceScopeWrapper
    {
        private readonly IServiceScope scope;

        public ServiceScopeWrapper(IServiceScope scope)
        {
            this.scope = scope;
        }

        public IServiceProvider ServiceProvider
        {
            get { return this.scope.ServiceProvider; }
        }

        public void Dispose()
        {
            this.scope.Dispose();
        }
    }
}
