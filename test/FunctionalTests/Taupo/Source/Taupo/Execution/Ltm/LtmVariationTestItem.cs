//---------------------------------------------------------------------
// <copyright file="LtmVariationTestItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    [SecurityCritical]
    internal class LtmVariationTestItem : LtmTestItem
    {
        public LtmVariationTestItem(VariationTestItem item)
            : base(item)
        {
        }

        [SecurityCritical]
        public override IEnumerable<LtmTestItem> GetChildren()
        {
            return TestItem.Children.Select(c => LtmTestLoader.Wrap(c));
        }

        public override TestType ItemType
        {
            [SecurityCritical]
            get
            {
                return TestType.TestVariation;
            }
        }
    }
}
