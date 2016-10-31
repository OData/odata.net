//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
