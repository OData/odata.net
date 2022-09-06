//---------------------------------------------------------------------
// <copyright file="LtmTestProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Test.Taupo.Common;
using Microsoft.Test.Taupo.Contracts;
using Microsoft.Test.Taupo.Utilities;
using System.Security;

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    public class LtmTestProperties
    {
        private bool initialized = false;
        private Dictionary<string, string> props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<KeyValuePair<string, string>> GetParameterValues()
        {
            if (!this.initialized)
            {
                InitializeFromLtmContext();
                this.initialized = true;
            }

            return this.props;
        }

        [SecuritySafeCritical]
        private void InitializeFromLtmContext()
        {
            ITestProperties testProperties = (new LtmContext() as ITestProperties);
            var inits = testProperties.Get("Alias/InitString");
            if (inits != null)
            {
                foreach (var kvp in InitStringUtilities.ParseInitString((string)(inits.Value)))
                {
                    this.props[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}
