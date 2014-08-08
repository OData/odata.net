//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#if ASTORIA_CLIENT
namespace System.Data.Services.Client
#endif
#if ASTORIA_SERVER
namespace System.Data.Services
#endif
#if !ASTORIA_CLIENT && !ASTORIA_SERVER
namespace Microsoft.Data.OData
#endif
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A simple implementation of LazyOfT since the framework version is not available in all platforms we compile this code in...
    /// </summary>
    /// <typeparam name="T">Type to lazy create.</typeparam>
    internal sealed class SimpleLazy<T>
    {
        /// <summary>
        /// For thread safty in creating the value.
        /// </summary>
        private readonly object mutex;

        /// <summary>
        /// The factory method to create the lazy instance.
        /// </summary>
        private readonly Func<T> factory;

        /// <summary>
        /// Holds the lazy instance to create.
        /// </summary>
        private T value;

        /// <summary>
        /// true if the factory method has been called, false otherwise.
        /// </summary>
        private bool valueCreated;

        /// <summary>
        /// Creates an instance of ODataLazyOfT.
        /// </summary>
        /// <param name="factory">The factory method to create the lazy instance.</param>
        internal SimpleLazy(Func<T> factory)
            : this(factory, false)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
        }

        /// <summary>
        /// Creates an instance of ODataLazyOfT.
        /// </summary>
        /// <param name="factory">The factory method to create the lazy instance.</param>
        /// <param name="isThreadSafe">true if the value will be created in a thread safety, false assume single thread access to Value.</param>
        internal SimpleLazy(Func<T> factory, bool isThreadSafe)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(factory != null, "factory != null");
            this.factory = factory;
            this.valueCreated = false;
            if (isThreadSafe)
            {
                this.mutex = new object();
            }
        }

        /// <summary>
        /// Creates the value if it hasn't already been created and returns the created value.
        /// </summary>
        internal T Value
        {
            get
            {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
                if (!this.valueCreated)
                {
                    if (this.mutex != null)
                    {
                        lock (this.mutex)
                        {
                            if (!this.valueCreated)
                            {
                                this.CreateValue();
                            }
                        }
                    }
                    else
                    {
                        this.CreateValue();
                    }
                }

                return this.value;
            }
        }

        /// <summary>
        /// Creates the value.
        /// </summary>
        private void CreateValue()
        {
            this.value = this.factory();
            this.valueCreated = true;
        }
    }
}
