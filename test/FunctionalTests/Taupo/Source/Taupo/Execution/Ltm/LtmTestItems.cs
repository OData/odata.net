//---------------------------------------------------------------------
// <copyright file="LtmTestItems.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System.Collections.Generic;
    using System.Security;

    [SecurityCritical]
    internal class LtmTestItems : List<ITestItem>, ITestItems
    {
        public LtmTestItems()
        {
        }

        public LtmTestItems(IEnumerable<ITestItem> items)
            : base(items)
        {
        }

        int ITestItems.Count
        {
            [SecurityCritical]
            get { return this.Count; }
        }

        [SecurityCritical]
        public ITestItem GetItem(int index)
        {
            return this[index];
        }
    }
}
