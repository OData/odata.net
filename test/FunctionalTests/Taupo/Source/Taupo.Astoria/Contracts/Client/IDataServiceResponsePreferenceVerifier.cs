//---------------------------------------------------------------------
// <copyright file="IDataServiceResponsePreferenceVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for DataServiceResponsePreferenceVerifier
    /// </summary>
    [ImplementationSelector("DataServiceResponsePreferenceVerifier", DefaultImplementation = "Default", HelpText = "The verifier for the DataServiceResponsePreference")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need a different interface to be able to dependency inject this, but doesn't need any additional API.")]
    public interface IDataServiceResponsePreferenceVerifier : IDataServiceContextEventVerifier
    {
    }
}
