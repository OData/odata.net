//---------------------------------------------------------------------
// <copyright file="NamespaceAliasManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Manages namespaces and aliases for csdl / ssdl
    /// </summary>
    public class NamespaceAliasManager
    {
        // namespace -> alias
        private Dictionary<string, string> namespaceToAlias = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the namespace name of the speical "Self" alias
        /// If set to null, won't use the "Self" alias
        /// </summary>
        public string NamespaceForSelfAlias { get; set; }

        /// <summary>
        /// Adds an alias for a namespace.
        /// </summary>
        /// <param name="namespaceName">The namespace.</param>
        /// <param name="alias">The alias for the namespace.</param>
        public void AddNamespaceAlias(string namespaceName, string alias)
        {
            ExceptionUtilities.CheckStringNotNullOrEmpty(namespaceName, "Namespace name cannot be null or empty.");
            ExceptionUtilities.CheckStringNotNullOrEmpty(alias, "alias cannot be null or empty.");
            ExceptionUtilities.Assert(!this.namespaceToAlias.ContainsKey(namespaceName), "Alias for Namespace " + namespaceName + " already exists!");

            this.namespaceToAlias.Add(namespaceName, alias);
        }

        /// <summary>
        /// Gets all namespace -> alias mappings
        /// </summary>
        /// <returns>A set of pairs: namespace -> alias.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetNamespaceToAliasMappings()
        {
            return this.namespaceToAlias.Select(kvp => 
            {
                if (kvp.Key == this.NamespaceForSelfAlias)
                {
                    return new KeyValuePair<string, string>(kvp.Key, "Self");
                }

                return kvp;
            });
        }

        /// <summary>
        /// Gets the fully qualified name (Namespace.Name or Alias.Name)
        /// </summary>
        /// <param name="namedItem">The item that has a Namespace and a Name.</param>
        /// <returns>The fully qualified name.</returns>
        public string GetFullyQualifiedName(INamedItem namedItem)
        {
            string prefix = string.Empty;
            if (namedItem.NamespaceName != null)
            {
                string alias;
                if (this.TryGetAlias(namedItem.NamespaceName, out alias))
                {
                    prefix = alias + ".";
                }
                else
                {
                    prefix = namedItem.NamespaceName + ".";
                }
            }

            return prefix + namedItem.Name;
        }

        /// <summary>
        /// Try get the alias for a Namespace.
        /// </summary>
        /// <param name="namespaceName">The namespace.</param>
        /// <param name="alias">The alias, if there is one.</param>
        /// <returns>True if there is an alias, false otherwise.</returns>
        public bool TryGetAlias(string namespaceName, out string alias)
        {
            if (namespaceName == this.NamespaceForSelfAlias)
            {
                alias = "Self";
                return true;
            }

            if (this.namespaceToAlias.ContainsKey(namespaceName))
            {
                alias = this.namespaceToAlias[namespaceName];
                return true;
            }

            alias = null;
            return false;
        }
    }
}
