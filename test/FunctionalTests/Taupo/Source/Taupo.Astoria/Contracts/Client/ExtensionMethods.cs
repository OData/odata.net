//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Extension methods for the wrapped client objects.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Executes SaveChanges on the specified context and verifies the results.
        /// </summary>
        /// <param name="verifier">The verifier to use for verification.</param>
        /// <param name="context">The context to verify SaveChanges on.</param>
        /// <returns>The response from SaveChanges</returns>
        public static DSClient.DataServiceResponse VerifySaveChanges(this ISaveChangesVerifier verifier, WrappedDataServiceContext context)
        {
            DataServiceContextData data = CheckParametersAndGetDataServiceContextData(verifier, context);
            return verifier.VerifySaveChanges(data, (DSClient.DataServiceContext)context.Product);
        }

        /// <summary>
        /// Executes SaveChanges on the specified context and with specified options and verifies the results.
        /// </summary>
        /// <param name="verifier">The verifier to use for verification.</param>
        /// <param name="context">The context to verify SaveChanges on.</param>
        /// <param name="options">The options for saving changes.</param>
        /// <returns>The response from SaveChanges</returns>
        public static DSClient.DataServiceResponse VerifySaveChanges(this ISaveChangesVerifier verifier, WrappedDataServiceContext context, SaveChangesOptions options)
        {
            DataServiceContextData data = CheckParametersAndGetDataServiceContextData(verifier, context);
            return verifier.VerifySaveChanges(data, (DSClient.DataServiceContext)context.Product, options);
        }

        /// <summary>
        /// Executes SaveChanges on the specified context and with the default options and verifies the results.
        /// </summary>
        /// <param name="verifier">The verifier to use for verification.</param>
        /// <param name="contextData">The data for the context.</param>
        /// <param name="context">The context to verify SaveChanges on.</param>
        /// <returns>The response from SaveChanges</returns>
        public static DSClient.DataServiceResponse VerifySaveChanges(this ISaveChangesVerifier verifier, DataServiceContextData contextData, DSClient.DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(verifier, "verifier");
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            return verifier.VerifySaveChanges(contextData, context, null);
        }

        /// <summary>
        /// Executes SaveChanges on the specified context and with specified options and verifies the results.
        /// </summary>
        /// <param name="verifier">The verifier to use for verification.</param>
        /// <param name="contextData">The data for the context.</param>
        /// <param name="context">The context to verify SaveChanges on.</param>
        /// <param name="options">The options for saving changes.</param>
        /// <returns>The response from SaveChanges</returns>
        public static DSClient.DataServiceResponse VerifySaveChanges(this ISaveChangesVerifier verifier, DataServiceContextData contextData, DSClient.DataServiceContext context, SaveChangesOptions? options)
        {
            ExceptionUtilities.CheckArgumentNotNull(verifier, "verifier");
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            DSClient.DataServiceResponse response = null;
            SyncHelpers.ExecuteActionAndWait(c1 => verifier.VerifySaveChanges(c1, contextData, context, options, (c2, r) => { response = r; c2.Continue(); }));
            return response;
        }

        /// <summary>
        /// Executes SaveChanges on the specified context and verifies the results.
        /// </summary>
        /// <param name="verifier">The verifier to use for verification.</param>
        /// <param name="continuation">The asynchronous continuation</param>
        /// <param name="context">The context to verify SaveChanges on.</param>
        /// <param name="options">The options to use, or null for the default</param>
        /// <param name="onCompletion">The callback to call on completion</param>
        public static void VerifySaveChanges(this ISaveChangesVerifier verifier, IAsyncContinuation continuation, WrappedDataServiceContext context, SaveChangesOptions? options, Action<IAsyncContinuation, DSClient.DataServiceResponse> onCompletion)
        {
            DataServiceContextData data = CheckParametersAndGetDataServiceContextData(verifier, context);
            verifier.VerifySaveChanges(continuation, data, (DSClient.DataServiceContext)context.Product, options, onCompletion);
        }

        /// <summary>
        /// Gets the header value for the given response preference
        /// </summary>
        /// <param name="preference">The response preference</param>
        /// <returns>The header value for the given response preference</returns>
        public static string ToHeaderValue(this DataServiceResponsePreference preference)
        {
            if (preference == DataServiceResponsePreference.None)
            {
                return null;
            }
            else if (preference == DataServiceResponsePreference.IncludeContent)
            {
                return HttpHeaders.ReturnContent;
            }
            else
            {
                ExceptionUtilities.Assert(preference == DataServiceResponsePreference.NoContent, "Unexpected preference value");
                return HttpHeaders.ReturnNoContent;
            }
        }

        /// <summary>
        /// Converts the MergeOption product enum value to the equivalent test enum value
        /// </summary>
        /// <param name="option">The value to convert</param>
        /// <returns>The converted value</returns>
        public static MergeOption ToTestEnum(this DSClient.MergeOption option)
        {
            return Contracts.ExtensionMethods.ConvertEnum<DSClient.MergeOption, MergeOption>(option);
        }

        /// <summary>
        /// Converts the MergeOption test enum value to the equivalent product enum value
        /// </summary>
        /// <param name="option">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DSClient.MergeOption ToProductEnum(this MergeOption option)
        {
            return Contracts.ExtensionMethods.ConvertEnum<MergeOption, DSClient.MergeOption>(option);
        }

        /// <summary>
        /// Converts the EntityStates product enum value to the equivalent test enum value
        /// </summary>
        /// <param name="state">The value to convert</param>
        /// <returns>The converted value</returns>
        public static EntityStates ToTestEnum(this DSClient.EntityStates state)
        {
            return Contracts.ExtensionMethods.ConvertEnum<DSClient.EntityStates, EntityStates>(state);
        }

        /// <summary>
        /// Converts the EntityStates test enum value to the equivalent product enum value
        /// </summary>
        /// <param name="state">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DSClient.EntityStates ToProductEnum(this EntityStates state)
        {
            return Contracts.ExtensionMethods.ConvertEnum<EntityStates, DSClient.EntityStates>(state);
        }

        /// <summary>
        /// Converts the SaveChangesOptions product enum value to the equivalent test enum value
        /// </summary>
        /// <param name="option">The value to convert</param>
        /// <returns>The converted value</returns>
        public static SaveChangesOptions ToTestEnum(this DSClient.SaveChangesOptions option)
        {
            return Contracts.ExtensionMethods.ConvertEnum<DSClient.SaveChangesOptions, SaveChangesOptions>(option);
        }

        /// <summary>
        /// Converts the SaveChangesOptions test enum value to the equivalent product enum value
        /// </summary>
        /// <param name="option">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DSClient.SaveChangesOptions ToProductEnum(this SaveChangesOptions option)
        {
            ExceptionUtilities.Assert(option != SaveChangesOptions.Unspecified, "Cannot convert unspecified option value");
            return Contracts.ExtensionMethods.ConvertEnum<SaveChangesOptions, DSClient.SaveChangesOptions>(option);
        }

        /// <summary>
        /// Converts the DataServiceResponsePreference product enum value to the equivalent test enum value
        /// </summary>
        /// <param name="preference">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DataServiceResponsePreference ToTestEnum(this DSClient.DataServiceResponsePreference preference)
        {
            return Contracts.ExtensionMethods.ConvertEnum<DSClient.DataServiceResponsePreference, DataServiceResponsePreference>(preference);
        }

        /// <summary>
        /// Converts the DataServiceResponsePreference test enum value to the equivalent product enum value
        /// </summary>
        /// <param name="preference">The value to convert</param>
        /// <returns>The converted value</returns>
        public static DSClient.DataServiceResponsePreference ToProductEnum(this DataServiceResponsePreference preference)
        {
            ExceptionUtilities.Assert(preference != DataServiceResponsePreference.Unspecified, "Cannot convert unspecified preference value");
            return Contracts.ExtensionMethods.ConvertEnum<DataServiceResponsePreference, DSClient.DataServiceResponsePreference>(preference);
        }

        private static DataServiceContextData CheckParametersAndGetDataServiceContextData(ISaveChangesVerifier verifier, WrappedDataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(verifier, "verifier");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            IDataServiceContextTrackingScope trackingScope = context.Scope as IDataServiceContextTrackingScope;
            ExceptionUtilities.CheckObjectNotNull(trackingScope, "Wrapped data service sontext does not belong to a tracking scope. Cannot obtain DataServiceContextData for verification.");
            return trackingScope.GetDataServiceContextData(context);
        }
    }
}
