//---------------------------------------------------------------------
// <copyright file="MultiKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.DataClasses
{
    #region Namespaces
    #endregion Namespaces

    internal sealed class MultiKey
    {
        public double KeyB { get; set; }
        public string KeyA { get; set; }
        public int Keya { get; set; }
        public string NonKey { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MultiKey other = obj as MultiKey;
            if (other == null) return false;

            return this.KeyB == other.KeyB &&
                this.Keya == other.Keya &&
                this.KeyA == other.KeyA &&
                this.NonKey == other.NonKey;
        }
    }
}
