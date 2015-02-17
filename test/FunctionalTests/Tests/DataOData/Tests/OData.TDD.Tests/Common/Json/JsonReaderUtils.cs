//---------------------------------------------------------------------
// <copyright file="JsonReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.Json
{
    using Microsoft.OData.Core.Json;

    public static class JsonReaderUtils
    {
        internal static JsonReaderAssertions Should(this JsonReader jsonReader)
        {
            return new JsonReaderAssertions(jsonReader);
        }
    }
}
