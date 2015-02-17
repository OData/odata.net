//---------------------------------------------------------------------
// <copyright file="INamedValuesGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System.Collections.Generic;

    /// <summary>
    /// Members' values generator. Generates data in form of key-value pairs where Key is a member name and Value is a member value.
    /// </summary>
    public interface INamedValuesGenerator : IDataGenerator<IList<NamedValue>>, IMemberDataGenerators
    {
    }
}
