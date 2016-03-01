//---------------------------------------------------------------------
// <copyright file="ServiceGeneratorFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Test.OData.Services.TestServices;

namespace Microsoft.OData.Profile111.Tests
{
    public static class ServiceGeneratorFactory
    {
        public static IServiceUriGenerator CreateServiceUriGenerator()
        {
            return new DesktopExternalUriGenerator();
        }
    }
}
