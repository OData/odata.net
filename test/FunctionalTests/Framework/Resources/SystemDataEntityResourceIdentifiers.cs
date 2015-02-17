//---------------------------------------------------------------------
// <copyright file="SystemDataEntityResourceIdentifiers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Data;

    public class SystemDataEntityResourceIdentifiers
    {
        public static ResourceIdentifier Create(string id, Type expectedException, ComparisonFlag flag)
        {
            return new ResourceIdentifier(typeof(System.Data.EntityState).Assembly, id, flag, expectedException);
        }
        public static ResourceIdentifier Create(string id)
        {
            return Create(id, null, ComparisonFlag.Full);
        }
        public static ResourceIdentifier Create(string id,ComparisonFlag comparisionFlag)
        {
            return Create(id, null,comparisionFlag);
        }

        public static ResourceIdentifier Update_GeneralExecutionException = Create("Update_GeneralExecutionException", ComparisonFlag.Contains);
    }
}
