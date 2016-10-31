//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.EdmToClrConversion
{
    /// <summary>
    /// Represents a delegate for creating an instance of CLR type based on <see cref="IEdmValue"/> and <see cref="Type"/>.
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
}
