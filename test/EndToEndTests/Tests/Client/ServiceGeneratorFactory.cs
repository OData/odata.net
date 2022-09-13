//---------------------------------------------------------------------
// <copyright file="ServiceGeneratorFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using Microsoft.Test.OData.Services.TestServices;

    public static class ServiceGeneratorFactory
    {
        public static IServiceUriGenerator CreateServiceUriGenerator()
        {
            return new DesktopUriGenerator();
        }
    }
}
