//---------------------------------------------------------------------
// <copyright file="DataServiceRequestMessageFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Selects the implementation to use depending on the requesttrasport mode used. 
    /// </summary>
    internal class DataServiceRequestMessageFactory : IDataServiceRequestMessageFactory
    {
        /// <summary>
        /// A Factory class to use in selecting the implementation to use depending on the 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="dataServiceContext"></param>
        /// <returns></returns>

        public DataServiceClientRequestMessage CreateRequestMessage(DataServiceClientRequestMessageArgs args, DataServiceContext dataServiceContext)
        {
            if (dataServiceContext.HttpRequestTransportMode == HttpRequestTransportMode.HttpWebRequest)
            {
                return new HttpWebRequestMessage(args);
            }
            else
            {
                return new HttpClientRequestMessage(args);
            }
        }
    }
}
