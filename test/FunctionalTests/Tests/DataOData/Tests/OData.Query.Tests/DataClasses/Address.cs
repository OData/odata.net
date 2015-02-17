//---------------------------------------------------------------------
// <copyright file="Address.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.DataClasses
{
    #region Namespaces
    #endregion Namespaces

    internal sealed class Address
    {
        public string City { get; set; }
        public int Zip { get; set; }

        public override int GetHashCode()
        {
            return this.City.GetHashCode() ^ this.Zip.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Address other = obj as Address;
            if (other == null) return false;

            return this.City == other.City && this.Zip == other.Zip;
        }
    }
}
