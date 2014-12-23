//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Denotes the key property or properties of an entity. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Accessors are available for processed input.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class KeyAttribute : System.Attribute
    {
        /// <summary>Name of the properties that form the key.</summary>
        private readonly ReadOnlyCollection<string> keyNames;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.KeyAttribute" /> class. </summary>
        /// <param name="keyName">The string that contains name of the key attribute.</param>
        public KeyAttribute(string keyName)
        {
            Util.CheckArgumentNull(keyName, "keyName");
            Util.CheckArgumentNullAndEmpty(keyName, "KeyName");
            this.keyNames = new ReadOnlyCollection<string>(new string[1] { keyName });
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.KeyAttribute" /> class. </summary>
        /// <param name="keyNames">An array of string values that contain key attribute names.</param>
        public KeyAttribute(params string[] keyNames)
        {
            Util.CheckArgumentNull(keyNames, "keyNames");
            if (keyNames.Length == 0 || keyNames.Any(f => f == null || f.Length == 0))
            {
                throw Error.Argument(Strings.DSKAttribute_MustSpecifyAtleastOnePropertyName, "keyNames");
            }

            this.keyNames = new ReadOnlyCollection<string>(keyNames);
        }

        /// <summary>Gets the names of key attributes.</summary>
        /// <returns>String value that contains names of key attributes. </returns>
        public ReadOnlyCollection<string> KeyNames
        {
            get
            {
                return this.keyNames;
            }
        }
    }
}
