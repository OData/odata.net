//---------------------------------------------------------------------
// <copyright file="EmptyErrorResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;

    [ImplementationName(typeof(IClientExpectedErrorComparer), "Empty")]
    public class EmptyErrorResultComparer : IClientExpectedErrorComparer
    {
        public void Compare(ExpectedClientErrorBaseline expectedClientException, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
