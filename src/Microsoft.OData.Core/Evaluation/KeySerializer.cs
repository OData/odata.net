//---------------------------------------------------------------------
// <copyright file="KeySerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
#if ODATA_SERVICE
namespace Microsoft.OData.Service.Serializers
#else
namespace Microsoft.OData.Evaluation
#endif
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
#if !ODATA_CORE
    using Microsoft.OData;
#endif

    /// <summary>
    /// Component for serializing entity key values for building identities, edit links, etc.
    /// </summary>
    internal abstract class KeySerializer
    {
        /// <summary>Singleton instance of the default serializer.</summary>
        private static readonly DefaultKeySerializer DefaultInstance = new DefaultKeySerializer();

        /// <summary>Singleton instance of the segment-based serializer.</summary>
        private static readonly SegmentKeySerializer SegmentInstance = new SegmentKeySerializer();

        /// <summary>
        /// Creates a new key serializer.
        /// </summary>
        /// <param name="enableKeyAsSegment">Whether enable key as segment.</param>
        /// <returns>A new key serializer.</returns>
        internal static KeySerializer Create(bool enableKeyAsSegment)
        {
            if (enableKeyAsSegment)
            {
                return SegmentInstance;
            }

            return DefaultInstance;
        }

        /// <summary>
        /// Appends the key expression for an entity to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
        /// <param name="builder">The builder to append onto.</param>
        /// <param name="keyProperties">The key properties.</param>
        /// <param name="getPropertyName">The callback to get each property's name.</param>
        /// <param name="getPropertyValue">The callback to get each property's value.</param>
        internal abstract void AppendKeyExpression<TProperty>(StringBuilder builder, ICollection<TProperty> keyProperties, Func<TProperty, string> getPropertyName, Func<TProperty, object> getPropertyValue);

        /// <summary>
        /// Gets the value of the key property and serializes it to a string.
        /// </summary>
        /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
        /// <param name="getPropertyValue">The callback to get the value for a property.</param>
        /// <param name="property">The key property.</param>
        /// <param name="literalFormatter">The literal formatter to use.</param>
        /// <returns>The serialized key property value.</returns>
        private static string GetKeyValueAsString<TProperty>(Func<TProperty, object> getPropertyValue, TProperty property, LiteralFormatter literalFormatter)
        {
            Debug.Assert(getPropertyValue != null, "getPropertyValue != null");
            object keyValue = getPropertyValue(property);

            Debug.Assert(keyValue != null, "keyValue != null");
            string keyValueText = literalFormatter.Format(keyValue);

            Debug.Assert(keyValueText != null, "keyValueText != null");
            return keyValueText;
        }

        /// <summary>
        /// Appends the key using the parentheses-based syntax (e.g. Customers(1)) onto the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder to append onto.</param>
        /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
        /// <param name="keyProperties">The key properties.</param>
        /// <param name="getPropertyName">The callback to get each property's name.</param>
        /// <param name="getPropertyValue">The callback to get each property's value.</param>
        private static void AppendKeyWithParentheses<TProperty>(StringBuilder builder, ICollection<TProperty> keyProperties, Func<TProperty, string> getPropertyName, Func<TProperty, object> getPropertyValue)
        {
            Debug.Assert(builder != null, "builder != null");
            Debug.Assert(keyProperties != null, "keyProperties != null");
            Debug.Assert(keyProperties.Count != 0, "every resource type must have a key");
            Debug.Assert(getPropertyValue != null, "getPropertyValue != null");

            LiteralFormatter literalFormatter = LiteralFormatter.ForKeys(false);

            builder.Append('(');

            bool first = true;
            foreach (TProperty property in keyProperties)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(',');
                }

                if (keyProperties.Count != 1)
                {
                    builder.Append(getPropertyName(property));
                    builder.Append('=');
                }

                var keyValueText = GetKeyValueAsString(getPropertyValue, property, literalFormatter);
                builder.Append(keyValueText);
            }

            builder.Append(')');
        }

        /// <summary>
        /// Default implementation of the key serializer which uses parentheses (e.g. Customers(1)).
        /// </summary>
        private sealed class DefaultKeySerializer : KeySerializer
        {
            /// <summary>
            /// Appends the key expression for an entity to the given <see cref="StringBuilder"/>
            /// </summary>
            /// <param name="builder">The builder to append onto.</param>
            /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
            /// <param name="keyProperties">The key properties.</param>
            /// <param name="getPropertyName">The callback to get each property's name.</param>
            /// <param name="getPropertyValue">The callback to get each property's value.</param>
            internal override void AppendKeyExpression<TProperty>(StringBuilder builder, ICollection<TProperty> keyProperties, Func<TProperty, string> getPropertyName, Func<TProperty, object> getPropertyValue)
            {
                AppendKeyWithParentheses(builder, keyProperties, getPropertyName, getPropertyValue);
            }
        }

        /// <summary>
        /// Implementation of the key serializer which uses segments (e.g. Customers/1).
        /// </summary>
        private sealed class SegmentKeySerializer : KeySerializer
        {
            /// <summary>
            /// Creates a new instance of <see cref="SegmentKeySerializer"/>.
            /// </summary>
            internal SegmentKeySerializer()
            {
            }

            /// <summary>
            /// Appends the key expression for an entity to the given <see cref="StringBuilder"/>
            /// </summary>
            /// <param name="builder">The builder to append onto.</param>
            /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
            /// <param name="keyProperties">The key properties.</param>
            /// <param name="getPropertyName">The callback to get each property's name.</param>
            /// <param name="getPropertyValue">The callback to get each property's value.</param>
            internal override void AppendKeyExpression<TProperty>(StringBuilder builder, ICollection<TProperty> keyProperties, Func<TProperty, string> getPropertyName, Func<TProperty, object> getPropertyValue)
            {
                Debug.Assert(builder != null, "builder != null");
                Debug.Assert(keyProperties != null, "keyProperties != null");

                // Keys-as-segments mode is only supported for non-composite keys, so if there is more than 1 key property,
                // then fall back to the default behavior for the edit link and identity.
                if (keyProperties.Count > 1)
                {
                    AppendKeyWithParentheses(builder, keyProperties, getPropertyName, getPropertyValue);
                }
                else
                {
                    AppendKeyWithSegments(builder, keyProperties, getPropertyValue);
                }
            }

            /// <summary>
            /// Appends the key for the current resource using segment-based syntax (e.g. Customers/1) onto the given <see cref="StringBuilder"/>.
            /// </summary>
            /// <param name="builder">The builder to append onto.</param>
            /// <typeparam name="TProperty">The type used to represent properties.</typeparam>
            /// <param name="keyProperties">The key properties.</param>
            /// <param name="getPropertyValue">The callback to get each property's value.</param>
            private static void AppendKeyWithSegments<TProperty>(StringBuilder builder, ICollection<TProperty> keyProperties, Func<TProperty, object> getPropertyValue)
            {
                Debug.Assert(keyProperties != null, "keyProperties != null");
                Debug.Assert(keyProperties.Count == 1, "Only supported for non-composite keys.");

                builder.Append('/');
                builder.Append(GetKeyValueAsString(getPropertyValue, keyProperties.Single(), LiteralFormatter.ForKeys(true)));
            }
        }
    }
}
