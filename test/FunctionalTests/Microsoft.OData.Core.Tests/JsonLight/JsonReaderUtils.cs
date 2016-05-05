//---------------------------------------------------------------------
// <copyright file="JsonReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.JsonLight
{
    public static class JsonReaderUtils
    {
        internal static JsonReaderAssertions Should(this IJsonReader jsonReader)
        {
            return new JsonReaderAssertions(jsonReader);
        }
    }
}
