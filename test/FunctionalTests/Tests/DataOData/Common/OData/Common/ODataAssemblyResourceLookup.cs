//---------------------------------------------------------------------
// <copyright file="ODataAssemblyResourceLookup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Utilities;
    using System.Reflection;

    /// <summary>
    /// Implementation of IAssemblyResourceLookup that provides an alternative, hardcoded lookup
    /// for strings that have not been localized yet.
    /// </summary>
    public class ODataAssemblyResourceLookup : AssemblyResourceLookup
    {
        private static Dictionary<string, string> AlternativeResourceStrings = new Dictionary<string, string>
        {
        };

        /// <summary>
        /// Constructor for ODataAssemblyResourceLookup.
        /// </summary>
        /// <param name="assembly">The assembly to lookup resource string in.</param>
        public ODataAssemblyResourceLookup(Assembly assembly)
            : base(assembly)
        {
        }

        /// <summary>
        /// Override of LookupString that checks whether the resource id exists in the alternative lookup
        /// on failure when reading from assembly.
        /// </summary>
        /// <param name="resourceKey">The resource key to lookup.</param>
        /// <returns>The resource string.</returns>
        public override string LookupString(string resourceKey)
        {
            try
            {
                return base.LookupString(resourceKey);
            }
            catch (TaupoArgumentException ex)
            {
                string alternativeString = string.Empty;
                if (AlternativeResourceStrings.TryGetValue(resourceKey, out alternativeString))
                {
                    return alternativeString;
                }

                throw ex;
            }
        }
    }
}
