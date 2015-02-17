//---------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.DataClasses
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    internal sealed class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public Address Address { get; set; }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Customer other = obj as Customer;
            if (other == null) return false;

            if (this.ID != other.ID || this.Name != other.Name) return false;

            if (this.Address == null && other.Address != null ||
                this.Address != null && other.Address == null)
                return false;

            if (this.Address != null && !this.Address.Equals(other.Address))
                return false;

            if (this.Emails == null && other.Emails != null ||
                this.Emails != null && other.Emails == null)
                return false;

            if (this.Emails != null)
            {
                using (var myEnumerator = this.Emails.GetEnumerator())
                using (var otherEnumerator = other.Emails.GetEnumerator())
                {
                    while (myEnumerator.MoveNext())
                    {
                        if (!otherEnumerator.MoveNext()) return false;
                        if (myEnumerator.Current != otherEnumerator.Current) return false;
                    }

                    if (otherEnumerator.MoveNext()) return false;
                }
            }

            return true;
        }
    }
}
