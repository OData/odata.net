//---------------------------------------------------------------------
// <copyright file="ClientTypeHasNoKeysAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// This annotation helps the client code-gen in generating the [DataServiceEntity] attribute on the client-side types
    /// which specifies that the client type doesn't have a key property
    /// </summary>
    public class ClientTypeHasNoKeysAnnotation : TagAnnotation
    {
    }
}
