//---------------------------------------------------------------------
// <copyright file="IEntityModelFixupWithValidate.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// interface for all entity model fixups that also performs validation
    /// </summary>
    public interface IEntityModelFixupWithValidate : IEntityModelFixup, IEntityModelValidator
    {
    }
}
