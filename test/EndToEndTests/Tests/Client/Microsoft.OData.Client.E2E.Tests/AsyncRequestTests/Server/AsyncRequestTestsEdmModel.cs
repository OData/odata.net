//-----------------------------------------------------------------------------
// <copyright file="AsyncRequestTestsDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.Client.E2E.Tests.Common;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Server
{
    public class AsyncRequestTestsEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Person>("People");

            return builder.GetEdmModel();
        }
    }
}
