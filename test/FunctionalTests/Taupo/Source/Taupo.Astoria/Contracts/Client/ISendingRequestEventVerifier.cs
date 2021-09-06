//---------------------------------------------------------------------
// <copyright file="ISendingRequestEventVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for verifying the SendingRequest2 event raised by the data service context
    /// </summary>
    [ImplementationSelector("SendingRequestEventVerifier", DefaultImplementation = "Default", HelpText = "The verifier for DataServiceContext.SendingRequest2.")] 
    public interface ISendingRequestEventVerifier : IDataServiceContextEventVerifier
    {
        /// <summary>
        /// Gets or sets the callback that tests can use to customize the behavior of the event and fix-up the expected headers
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Not following the event-handler pattern")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Not following the event-handler pattern")]
        event Action<SendingRequest2EventArgs, IDictionary<string, string>> AlterRequestAndExpectedHeaders;
    }
}
