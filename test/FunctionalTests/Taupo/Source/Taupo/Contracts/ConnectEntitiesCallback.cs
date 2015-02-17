//---------------------------------------------------------------------
// <copyright file="ConnectEntitiesCallback.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Represents a callback function whose implementation connects the <paramref name="source"/>
    /// and <paramref name="target"/>, using the <paramref name="associationName"/> and <paramref name="sourceRoleName"/>
    /// as guides. Examples of "connection" include setting navigation properties, synchronizing foreign keys, and so on.
    /// </summary>
    /// <param name="source">The first <see cref="IEntitySetData"/> to connect with the <paramref name="target"/>.</param>
    /// <param name="target">The second <see cref="IEntitySetData"/> to connect with the <paramref name="source"/>.</param>
    /// <param name="associationName">The name of the association which indicates how to connect these two objects.</param>
    /// <param name="sourceRoleName">The name of the role played by <paramref name="source"/> in the association.</param>
    /// <remarks>
    /// <para>
    /// Given a relationship between Product and Category, if Product is the source and Category is the target, this callback
    /// is invoked as follows:
    /// </para>
    /// <para>
    /// invoke(product, category, "Product_Category", "ProductRole")
    /// </para>
    /// </remarks>
    public delegate void ConnectEntitiesCallback(IEntitySetData source, IEntitySetData target, string associationName, string sourceRoleName);
}
