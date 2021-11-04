//---------------------------------------------------------------------
// <copyright file="LtmTestModuleItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    [SecurityCritical]
    internal class LtmTestModuleItem : LtmTestItem
    {
        public LtmTestModuleItem(TestModule item)
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
                return TestType.TestModule;
            }
        }
    }
}
