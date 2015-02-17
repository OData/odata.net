//---------------------------------------------------------------------
// <copyright file="IServiceUriGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices
{
    using System;

    public interface IServiceUriGenerator
    {
        Uri GenerateServiceUri(string path);
    }
}
