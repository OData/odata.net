//---------------------------------------------------------------------
// <copyright file="MicrosoftDataWebClientResourceIdentifiers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    public class SystemDataServicesClientResourceIdentifiers
    {
        public static List<ResourceIdentifier> ResourceIdentifiers;

        public static ResourceIdentifier Create(string id, Type expectedException)
        {
            if (ResourceIdentifiers == null)
                ResourceIdentifiers = new List<ResourceIdentifier>();

            ResourceIdentifier resourceIdentifier = new ResourceIdentifier(typeof(Microsoft.OData.Client.DataServiceContext).Assembly, id, ComparisonFlag.Full, expectedException);
            ResourceIdentifiers.Add(resourceIdentifier);

            return resourceIdentifier;
        }

        public static ResourceIdentifier Create(string id)
        {
            return Create(id, null);
        }
        public static ResourceIdentifier ALinq_QueryOptionsOnlyAllowedOnSingletons = Create("ALinq_QueryOptionsOnlyAllowedOnSingletons");
        public static ResourceIdentifier Deserialize_ServerException = Create("Deserialize_ServerException");
        public static ResourceIdentifier ALinq_MethodNotSupported = Create("ALinq_MethodNotSupported", typeof(NotSupportedException));
        public static ResourceIdentifier DataServiceException_GeneralError = Create("DataServiceException_GeneralError");
    }
}
