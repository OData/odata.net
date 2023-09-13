// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.AspNetCore;
using Microsoft.Restier.Core;

namespace DynamicService
{
    public class DynamicApiController : RestierController
    {
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            IServiceProvider serviceProvider = Request.GetRequestContainer();
            //ApiFactory factory = Request.GetRequestContainer().GetRequiredService<ApiFactory>();
            var apiProperty = typeof(ApiBase).GetProperty("base", BindingFlags.NonPublic | BindingFlags.Instance);
            apiProperty.SetValue(this, typeof(NWind.NWindDataSource));

            return await base.Get(cancellationToken);
        }
    }
}