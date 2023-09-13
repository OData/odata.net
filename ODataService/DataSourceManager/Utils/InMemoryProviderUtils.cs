// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace DataSourceManager.Utils
{
    public static class InMemoryProviderUtils
    {
        const string sessionKey = "dataKey";
        static public string GetSessionId(HttpContext context)
        {
            var session = context.Session;
            string id;
            if (session != null)
            {
                id = session.GetString(sessionKey);
                if (id == null)
                {
                    id = System.Guid.NewGuid().ToString().Replace("-","");
                    session.SetString(sessionKey, id);
                }

                return id;
            }

            return null;
        }
    }
}
