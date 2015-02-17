//---------------------------------------------------------------------
// <copyright file="PayloadTransformationFluentExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extensions the payload transformation APIs to provide a fluent way to add/remove transforms.
    /// </summary>
    public static class PayloadTransformationFluentExtensions
    {
        /// <summary>
        ///  Adds the transform to the scope.
        /// </summary>
        /// <typeparam name="TPayload">The payload type for the transform.</typeparam>
        /// <param name="scope">Current scope.</param>
        /// <param name="transforms">The transforms to add.</param>
        /// <returns>Returns the modified payload transformation scope.</returns>
        public static IPayloadTransformationScope With<TPayload>(this IPayloadTransformationScope scope, params IPayloadTransform<TPayload>[] transforms)
        {
            if (scope != null)
            {
                transforms.ForEach(t => scope.Add(t));
            }

            return scope;
        }

        /// <summary>
        /// Removes matching transforms from the scope.
        /// </summary>
        /// <typeparam name="TPayload">The payload type for the transforms.</typeparam>
        /// <param name="scope">Current scope.</param>
        /// <param name="predicate">The predicate to use to find matching transforms.</param>
        /// <returns>Returns the modified payload transformation scope.</returns>
        public static IPayloadTransformationScope Without<TPayload>(this IPayloadTransformationScope scope, Predicate<IPayloadTransform<TPayload>> predicate)
        {
            if (scope != null)
            {
                scope.RemoveAll(predicate);
            }

            return scope;
        }

        /// <summary>
        /// Removes matching transforms from the scope.
        /// </summary>
        /// <typeparam name="TPayload">The payload type for the transforms.</typeparam>
        /// <typeparam name="TTransform">The transform type.</typeparam>
        /// <param name="scope">Current scope.</param>
        /// <returns>Returns the modified payload transformation scope.</returns>
        public static IPayloadTransformationScope Without<TPayload, TTransform>(this IPayloadTransformationScope scope) where TTransform : IPayloadTransform<TPayload>
        {
            return scope.Without<TPayload>(t => t is TTransform);
        }

        /// <summary>
        /// Calls the action method with the default scope when the scope is null or with the passed scope.
        /// </summary>
        /// <param name="scope">Payload transformation scope to be used.</param>
        /// <param name="toRun">The action method.</param>
        public static void Run(this IPayloadTransformationScope scope, Action toRun)
        {
            ExceptionUtilities.CheckArgumentNotNull(toRun, "toRun");

            if (scope == null)
            {
                toRun();
            }
            else
            {
                using (scope.Apply())
                {
                    toRun();
                }
            }
        }

        /// <summary>
        /// Returns the default scope for a payload transform factory.
        /// </summary>
        /// <param name="factory">Payload transform factory.</param>
        /// <returns>Payload transformation scope.</returns>
        public static IPayloadTransformationScope DefaultScope(this IPayloadTransformFactory factory)
        {
            ExceptionUtilities.CheckArgumentNotNull(factory, "factory");
            return factory.GetScope(false);
        }

        /// <summary>
        /// Returns an empty scope for a payload transform factory.
        /// </summary>
        /// <param name="factory">Payload transform factory.</param>
        /// <returns>Empty payload transformation scope.</returns>
        public static IPayloadTransformationScope EmptyScope(this IPayloadTransformFactory factory)
        {
            ExceptionUtilities.CheckArgumentNotNull(factory, "factory");
            return factory.GetScope(true);
        }
    }
}
