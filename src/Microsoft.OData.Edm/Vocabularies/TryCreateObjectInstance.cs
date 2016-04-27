//---------------------------------------------------------------------
// <copyright file="TryCreateObjectInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents a delegate for creating an instance of CLR type based on <see cref="IEdmValue"/> and <see cref="System.Type"/>.
    /// The delegate can be used to create CLR instances of polymorphic types.
    /// </summary>
    /// <param name="edmValue">The <see cref="IEdmStructuredValue"/> for which the <paramref name="objectInstance"/> needs to be created.</param>
    /// <param name="clrType">The expected CLR type of the object instance. In case of polymorphic properties and collections this may be a base type.</param>
    /// <param name="converter">The converter instance calling this delegate.</param>
    /// <param name="objectInstance">The output parameter returning a CLR object instance created for the <paramref name="edmValue"/>.</param>
    /// <param name="objectInstanceInitialized">The output parameter returning true if all properties of the created <paramref name="objectInstance"/> are initialized.
    /// False if properties of the created instance should be initialized using the default <see cref="EdmToClrConverter"/> logic.</param>
    /// <returns>True if the delegate produced a desired <paramref name="objectInstance"/>.
    /// If delegate returns false, the default <see cref="EdmToClrConverter"/> logic will be applied to create and populate a CLR object instance.</returns>
    public delegate bool TryCreateObjectInstance(
        IEdmStructuredValue edmValue,
        Type clrType,
        EdmToClrConverter converter,
        out object objectInstance,
        out bool objectInstanceInitialized);

    /// <summary>
    /// Represents a delegate to get property info of a CLR type based on the property name and <see cref="System.Type"/>.
    /// </summary>
    /// <param name="clrType">The CLR type which contains the property info.</param>
    /// <param name="edmName">The property name which might be server side name or client side name.</param>
    /// <param name="propertyInfo">The output parameter returning a PropertyInfo.</param>
    /// <returns>True if the delegate find the property, else false.</returns>
    public delegate bool TryGetClrPropertyInfo(
        Type clrType,
        string edmName,
        out PropertyInfo propertyInfo);

    /// <summary>
    /// Represents a delegate to get CLR type name based on the edm type name and <see cref="IEdmModel"/>.
    /// </summary>
    /// <param name="edmModel">The <see cref="IEdmModel"/> used to find the type name.</param>
    /// <param name="edmTypeName">The edm type name.</param>
    /// <param name="clrTypeName">The output parameter returning a CLR type name.</param>
    /// <returns>True if the delegate find the type name, else false.</returns>
    public delegate bool TryGetClrTypeName(
        IEdmModel edmModel,
        string edmTypeName,
        out string clrTypeName);
}
