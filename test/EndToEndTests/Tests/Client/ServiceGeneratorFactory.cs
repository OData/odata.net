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
#if WINDOWSPHONE
            return new WindowsPhoneUriGenerator();
#else
#if SILVERLIGHT
            return new SilverlightUriGenerator();
#else
#if WIN8
            return new WindowsStoreUriGenerator();
#else
#if PORTABLELIB
            return new DesktopExternalUriGenerator();
#else
            return new DesktopUriGenerator();
#endif
#endif
#endif
#endif
        }
    }
}
