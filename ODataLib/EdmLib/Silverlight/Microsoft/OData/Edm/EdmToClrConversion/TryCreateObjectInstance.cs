//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.EdmToClrConversion
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
