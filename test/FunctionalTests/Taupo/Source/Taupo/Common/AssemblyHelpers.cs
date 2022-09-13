//---------------------------------------------------------------------
// <copyright file="AssemblyHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Helper class for dealing with assembly loading.
    /// </summary>
    public static class AssemblyHelpers
    {
        /// <summary>
        /// Loads and resolves the assembly using non-standard reference paths.
        /// </summary>
        /// <param name="assemblyFullPath">The assembly full path.</param>
        /// <param name="referencePaths">The reference paths.</param>
        /// <returns>Loaded assembly.</returns>
        public static Assembly LoadAssembly(string assemblyFullPath, params string[] referencePaths)
        {
            return new Resolver(assemblyFullPath, referencePaths.Select(c => IOHelpers.GetFullPath(c)).ToArray()).LoadAssembly();
        }

        /// <summary>
        /// Helper class to load assemblies from a subdirectory.
        /// </summary>
        private class Resolver
        {
            private string fullPath;
            private string assemblyName;
            private string[] referencePaths;
            private Queue<Assembly> assembliesToResolve = new Queue<Assembly>();

            internal Resolver(string assemblyFullPath, string[] referencePaths)
            {
                this.fullPath = assemblyFullPath;
                this.assemblyName = Path.GetFileNameWithoutExtension(assemblyFullPath);
                this.referencePaths = referencePaths;
            }

            [SecuritySafeCritical]
            internal Assembly LoadAssembly()
            {
                AppDomain.CurrentDomain.AssemblyResolve += this.AssemblyResolve;
                try
                {
                    var a = Assembly.Load(this.assemblyName);

                    // resolve references in all assemblies that we have loaded so far
                    // new assemblies may be added to the queue as part of the resolution process
                    while (this.assembliesToResolve.Count > 0)
                    {
                        Assembly asm = this.assembliesToResolve.Dequeue();

                        foreach (AssemblyName name in asm.GetReferencedAssemblies())
                        {
                            Assembly.Load(name);
                        }
                    }

                    return a;
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= this.AssemblyResolve;
                }
            }

            /// <summary>
            /// Raised when the CLR can't resolve an assembly through normal fusion rules.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Arguments containing the event data.</param>
            /// <returns>The <see cref="Assembly"/> requested by the CLR assembly loader
            /// based on the event data.</returns>
            [SecuritySafeCritical]
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            [SuppressMessage("Microsoft.Reliability", "CA2001", Justification = "Assembly.Load is needed here.")]
            [AssertJustification("Loading assemblies from various locations demands FileIOPermission (Read | PathDiscovery) to those paths.")]
            private Assembly AssemblyResolve(object sender, ResolveEventArgs e)
            {
                Assembly result = null; 

                // we load our initial assembly directly from the specified file
                if (e.Name == this.assemblyName)
                {
                    result = Assembly.LoadFrom(this.fullPath);
                }
                else
                {
                    // otherwise search reference paths
                    var asmName = new AssemblyName(e.Name);
                    foreach (string path in this.referencePaths)
                    {
                        string fullName = Path.Combine(path, asmName.Name + ".dll");
                        if (File.Exists(fullName))
                        {
                            result = Assembly.LoadFrom(fullName);
                        }
                    }
                }

                if (result != null)
                {
                    // if we have loaded an assembly, enqueue it for further resolution
                    this.assembliesToResolve.Enqueue(result);
                }

                return result;
            }
        }
    }
}
