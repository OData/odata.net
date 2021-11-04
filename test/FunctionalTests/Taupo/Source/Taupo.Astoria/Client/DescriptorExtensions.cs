//---------------------------------------------------------------------
// <copyright file="DescriptorExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods for entity and link decriptors.
    /// </summary>
    public static class DescriptorExtensions
    {
        /// <summary>
        /// Gets string representation of the specified entity descriptor.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor.</param>
        /// <returns>The string representation of the specified entity descriptor.</returns>
        public static string ToTraceString(this EntityDescriptor entityDescriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityDescriptor, "entityDescriptor");

            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ State = {0}, Identity = {1}, Type = {2} }}",
                entityDescriptor.State,
                entityDescriptor.Identity,
                entityDescriptor.Entity.GetType().Name);
        }

        /// <summary>
        /// Gets string representation of the specified link descriptor.
        /// </summary>
        /// <param name="linkDescriptor">The link descriptor.</param>
        /// <returns>The string representation of the specified link descriptor.</returns>
        public static string ToTraceString(this LinkDescriptor linkDescriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(linkDescriptor, "linkDescriptor");

            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ State = {0}, Source = {{ {1} }}, SourceProperty = '{2}', Target = {{ {3} }} }}",
                linkDescriptor.State,
                linkDescriptor.Source,
                linkDescriptor.SourceProperty,
                linkDescriptor.Target == null ? "null" : linkDescriptor.Target);
        }

        /// <summary>
        /// Gets string representation of the specified stream descriptor.
        /// </summary>
        /// <param name="streamDescriptor">The stream descriptor.</param>
        /// <returns>The string representation of the specified stream descriptor.</returns>
        public static string ToTraceString(this StreamDescriptor streamDescriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(streamDescriptor, "streamDescriptor");

            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ State = {0}, Name = '{1}', Edit = '{2}', Self = '{3}', ContentType = '{4}', ETag = '{5}' }}",
                streamDescriptor.State,
                streamDescriptor.StreamLink.Name,
                streamDescriptor.StreamLink.EditLink,
                streamDescriptor.StreamLink.SelfLink,
                streamDescriptor.StreamLink.ContentType,
                streamDescriptor.StreamLink.ETag);
        }

        /// <summary>
        /// Gets the change order for the descriptor via reflection
        /// </summary>
        /// <param name="descriptor">The descriptor to get the change order for</param>
        /// <returns>The change order</returns>
        [SecuritySafeCritical]
        [AssertJustification("Need get internal property for sanity check and for cases where it cannot be predicted")]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static long GetChangeOrder(this Descriptor descriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(descriptor, "descriptor");

            var changeOrderProperty = typeof(Descriptor).GetProperty("ChangeOrder", false, false);
            ExceptionUtilities.CheckObjectNotNull(changeOrderProperty, "Could not find property 'ChangeOrder' on type 'Descriptor'");

            uint actualChangeOrder = (uint)changeOrderProperty.GetValue(descriptor, null);
            return Convert.ToInt64(actualChangeOrder);
        }
    }
}
