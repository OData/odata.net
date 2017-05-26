//---------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData
{
    /// <summary>
    /// Extension methods for <see cref="IServiceProvider" />.
    /// </summary>
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets a service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public static T GetService<T>(this IServiceProvider container)
        {
            Debug.Assert(container != null, "container != null");

            return (T)container.GetService(typeof(T));
        }

        /// <summary>
        /// Gets a service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType"/>.</returns>
        /// <exception cref="System.InvalidOperationException">There is no service of type <paramref name="serviceType"/>.</exception>
        public static object GetRequiredService(this IServiceProvider container, Type serviceType)
        {
            Debug.Assert(container != null, "container != null");
            Debug.Assert(serviceType != null, "serviceType != null");

            var service = container.GetService(serviceType);
            if (service == null)
            {
                throw new ODataException(Strings.ServiceProviderExtensions_NoServiceRegistered(serviceType));
            }

            return service;
        }

        /// <summary>
        /// Gets a service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/>.</exception>
        public static T GetRequiredService<T>(this IServiceProvider container)
        {
            Debug.Assert(container != null, "container != null");

            return (T)container.GetRequiredService(typeof(T));
        }

        /// <summary>
        /// Gets an enumeration of services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
        /// <returns>An enumeration of services of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> GetServices<T>(this IServiceProvider container)
        {
            Debug.Assert(container != null, "container != null");

            return container.GetRequiredService<IEnumerable<T>>();
        }

        /// <summary>
        /// Gets an enumeration of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>An enumeration of services of type <paramref name="serviceType"/>.</returns>
        public static IEnumerable<object> GetServices(this IServiceProvider container, Type serviceType)
        {
            Debug.Assert(container != null, "container != null");
            Debug.Assert(serviceType != null, "serviceType != null");

            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);
            return (IEnumerable<object>)container.GetRequiredService(genericEnumerable);
        }

        /// <summary>
        /// Gets the service prototype of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service prototype to get.</typeparam>
        /// <param name="container">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
        /// <returns>The service prototype of type <typeparamref name="TService"/>.</returns>
        public static TService GetServicePrototype<TService>(this IServiceProvider container)
        {
            Debug.Assert(container != null, "container != null");

            return container.GetRequiredService<ServicePrototype<TService>>().Instance;
        }
    }
}
