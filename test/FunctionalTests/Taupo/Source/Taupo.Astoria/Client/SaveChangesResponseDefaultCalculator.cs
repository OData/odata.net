//---------------------------------------------------------------------
// <copyright file="SaveChangesResponseDefaultCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// The default calculator for the expected response from DataServiceContext.SaveChanges.
    /// Should be used for positive cases: i.e. assumes there are no errors in the response.
    /// </summary>
    [ImplementationName(typeof(ISaveChangesResponseCalculator), "Default", HelpText = "The default calculator for the expected response from DataServiceContext.SaveChanges. Should be used for positive cases: i.e. assumes there are no errors in the response.")]
    public class SaveChangesResponseDefaultCalculator : ISaveChangesResponseCalculator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveChangesResponseDefaultCalculator"/> class.
        /// </summary>
        public SaveChangesResponseDefaultCalculator()
        {
            this.ProtocolVersion = DataServiceProtocolVersion.V4;
        }

        /// <summary>
        /// Gets or sets the protocol version.
        /// </summary>
        /// <value>The data service protocol version.</value>
        [InjectTestParameter("MaxProtocolVersion", DefaultValueDescription = "V3")]
        public DataServiceProtocolVersion ProtocolVersion { get; set; }

        /// <summary>
        /// Calculates the expected response from DataServiceContext.SaveChanges based on the context data before saving changes.
        /// Assumes there are no errors in the response.
        /// </summary>
        /// <param name="dataBeforeSaveChanges">The data before saving changes.</param>
        /// <param name="options">The options for saving changes.</param>
        /// <param name="context">The DataServiceContext instance which is calling SaveChanges.</param>
        /// <returns><see cref="DataServiceResponseData"/> that expresses expectations for the response.</returns>
        public DataServiceResponseData CalculateSaveChangesResponseData(DataServiceContextData dataBeforeSaveChanges, SaveChangesOptions options, DSClient.DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataBeforeSaveChanges, "dataBeforeSaveChanges");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            DataServiceResponseData responseData = new DataServiceResponseData();

            responseData.IsBatchResponse = options == SaveChangesOptions.Batch;

            bool hasChanges = false;

            // Note: ordering is important as changes should be processed in the order specified by user.
            foreach (DescriptorData descriptorData in dataBeforeSaveChanges.GetOrderedChanges())
            {
                int statusCode = (int)HttpStatusCode.NoContent;

                var entityDescriptorData = descriptorData as EntityDescriptorData;
                if (entityDescriptorData != null)
                {
                    if (entityDescriptorData.IsMediaLinkEntry && entityDescriptorData.DefaultStreamState == EntityStates.Modified)
                    {
                        responseData.Add(new ChangeOperationResponseData(descriptorData) { StatusCode = statusCode });
                    }

                    if (descriptorData.State == EntityStates.Added)
                    {
                        statusCode = GetStatusCodeForInsert(context);

                        if (entityDescriptorData.IsMediaLinkEntry)
                        {
                            responseData.Add(new ChangeOperationResponseData(descriptorData) { StatusCode = statusCode });
                            statusCode = GetStatusCodeForUpdate(context);
                        }
                    }
                    else if (descriptorData.State == EntityStates.Modified)
                    {
                        statusCode = GetStatusCodeForUpdate(context);
                    }
                    else if (descriptorData.State != EntityStates.Deleted)
                    {
                        continue;
                    }
                }

                var linkDescriptorData = descriptorData as LinkDescriptorData;
                if (linkDescriptorData != null && (linkDescriptorData.State == EntityStates.Added || linkDescriptorData.State == EntityStates.Modified))
                {
                    if (!linkDescriptorData.WillTriggerSeparateRequest())
                    {
                        continue;
                    }
                }

                responseData.Add(new ChangeOperationResponseData(descriptorData) { StatusCode = statusCode });
                hasChanges = true;
            }

            if (responseData.IsBatchResponse)
            {
                responseData.BatchStatusCode = hasChanges ? (int)HttpStatusCode.Accepted : 0;
            }
            else
            {
                responseData.BatchStatusCode = -1;
            }

            return responseData;
        }

        private static int GetStatusCodeForInsert(DSClient.DataServiceContext context)
        {
            DataServiceResponsePreference preference = DataServiceResponsePreference.Unspecified;
            preference = context.AddAndUpdateResponsePreference.ToTestEnum();

            if (preference == DataServiceResponsePreference.NoContent)
            {
                return (int)HttpStatusCode.NoContent;
            }
            else
            {
                return (int)HttpStatusCode.Created;
            }
        }

        private static int GetStatusCodeForUpdate(DSClient.DataServiceContext context)
        {
            DataServiceResponsePreference preference = DataServiceResponsePreference.Unspecified;
            preference = context.AddAndUpdateResponsePreference.ToTestEnum();

            if (preference == DataServiceResponsePreference.IncludeContent)
            {
                return (int)HttpStatusCode.OK;
            }
            else
            {
                return (int)HttpStatusCode.NoContent;
            }
        }
    }
}
