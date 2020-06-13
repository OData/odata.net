//---------------------------------------------------------------------
// <copyright file="KeyAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.KeyAttribute" /> class. </summary>
        /// <param name="keyName">The string that contains name of the key attribute.</param>
        public KeyAttribute(string keyName)
        {
            Util.CheckArgumentNull(keyName, "keyName");
            Util.CheckArgumentNullAndEmpty(keyName, "KeyName");
            this.keyNames = new ReadOnlyCollection<string>(new string[1] { keyName });
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.KeyAttribute" /> class. </summary>
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
