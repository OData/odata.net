//---------------------------------------------------------------------
// <copyright file="SystemResourceIdentifiers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;

    public class SystemResourceIdentifiers
    {
        public static ResourceIdentifier Create(string id, Type expectedException, ComparisonFlag flag)
        {
            return new ResourceIdentifier(typeof(System.Net.WebRequest).Assembly, id, flag, expectedException);
        }
        public static ResourceIdentifier Create(string id)
        {
            return Create(id, null, ComparisonFlag.Full);
        }
        public static ResourceIdentifier Create(string id,ComparisonFlag comparisionFlag)
        {
            return Create(id, null,comparisionFlag);
        }
        public static ResourceIdentifier Net_webstatus_NameResolutionFailure = Create("net_webstatus_NameResolutionFailure",ComparisonFlag.Contains);
        public static ResourceIdentifier Net_servererror = Create("net_servererror",ComparisonFlag.EndsWith);
        public static ResourceIdentifier Net_httpstatuscode_NotFound = Create("net_httpstatuscode_NotFound");
        public static ResourceIdentifier Net_webstatus_ConnectFailure = Create("net_webstatus_ConnectFailure");
    }
}
