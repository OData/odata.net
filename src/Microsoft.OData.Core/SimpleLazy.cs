//---------------------------------------------------------------------
// <copyright file="SimpleLazy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#endif
#if ODATA_SERVICE
namespace Microsoft.OData.Service
#endif
#if !ODATA_CLIENT && !ODATA_SERVICE
namespace Microsoft.OData
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
        }

        /// <summary>
        /// Creates an instance of ODataLazyOfT.
        /// </summary>
        /// <param name="factory">The factory method to create the lazy instance.</param>
        /// <param name="isThreadSafe">true if the value will be created in a thread safety, false assume single thread access to Value.</param>
        internal SimpleLazy(Func<T> factory, bool isThreadSafe)
        {
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