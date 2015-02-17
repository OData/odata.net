//---------------------------------------------------------------------
// <copyright file="EntityDataKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for an entity key.
    /// </summary>
    [DebuggerDisplay("{ToString()}")] 
    public class EntityDataKey : IEquatable<EntityDataKey>
    {
        private static readonly Type[] primitiveTypes = new[] 
            {
                typeof(bool),
                typeof(string),
                typeof(byte[]),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(byte), 
                typeof(sbyte),
                typeof(short), 
                typeof(ushort), 
                typeof(int), 
                typeof(uint), 
                typeof(long), 
                typeof(ulong), 
                typeof(float), 
                typeof(double), 
                typeof(decimal)
            };
        
        private readonly object[] keyValues;
        
        internal EntityDataKey(IEnumerable<string> keyNames)
        {
            this.KeyNames = keyNames.ToList().AsReadOnly();
            this.keyValues = new object[this.KeyNames.Count];

            for (int i = 0; i < this.keyValues.Length; i++)
            {
                this.keyValues[i] = UninitializedData.Value;
            }
        }

        /// <summary>
        /// Gets the names of key members.
        /// </summary>
        /// <value>The key names.</value>
        public IList<string> KeyNames { get; private set; }

        /// <summary>
        /// Gets or sets the value of the key member.
        /// </summary>
        /// <param name="name">Name of the key member.</param>
        /// <value>Value of the key member.</value>
        public object this[string name]
        {
            get { return this.GetValue(name); }
            set { this.SetValue(name, value); }
        }

        /// <summary>
        /// Creates EntityDataKey from the value which is typically an anonymous object.
        /// This allows readable, in-line definition of EntityDataKey.
        /// </summary>
        /// <param name="value">The value to create EntityDataKey from.</param>
        /// <returns>Entity data key</returns>
        /// <example>EntityDataKey.CreateFrom( new {Id1 = 1, Id2 = 2} )</example>
        public static EntityDataKey CreateFrom(object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");

            var keyMembers = value.GetType().GetProperties();
            if (!keyMembers.Any())
            {
                throw new TaupoArgumentException("Cannot create entity data key with 0 key members.");
            }

            var key = new EntityDataKey(keyMembers.Select(p => p.Name));

            foreach (PropertyInfo propertyInfo in keyMembers)
            {
                key.SetValue(propertyInfo.Name, propertyInfo.GetValue(value, null));
            }

            return key;
        }

        /// <summary>
        /// Creates EntityDataKey from a collection of NamedValue instances
        /// </summary>
        /// <param name="values">The collection of values to create EntityDataKey from.</param>
        /// <returns>Entity data key</returns>
        public static EntityDataKey CreateFromValues(IEnumerable<NamedValue> values)
        {
            ExceptionUtilities.CheckArgumentNotNull(values, "values");
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");

            var key = new EntityDataKey(values.Select(v => v.Name));
            foreach (var keyValue in values)
            {
                key[keyValue.Name] = keyValue.Value;
            }

            return key;
        }

        /// <summary>
        /// Gets the value of the key member.
        /// </summary>
        /// <param name="name">The name of the key member.</param>
        /// <returns>Value of the key member.</returns>
        public object GetValue(string name)
        {
            return this.keyValues[this.GetOrdinalAndCheck(name)];
        }

        /// <summary>
        /// Sets the value of the key member.
        /// </summary>
        /// <param name="name">The name of the key member.</param>
        /// <param name="value">Value of the key member.</param>
        public void SetValue(string name, object value)
        {
            int ordinal = this.GetOrdinalAndCheck(name);
            this.keyValues[ordinal] = value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int result = 0;
            foreach (object value in this.keyValues.Where(v => v != null))
            {
                result ^= ValueComparer.Instance.GetHashCode(value);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="EntityDataKey"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="EntityDataKey"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="EntityDataKey"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EntityDataKey other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.KeyNames.Count != other.KeyNames.Count)
            {
                return false;
            }

            foreach (string name in this.KeyNames)
            {
                int otherOrdinal = other.GetOrdinal(name);
                if (otherOrdinal < 0)
                {
                    return false;
                }

                int ordinal = this.GetOrdinal(name);

                if (!ValueComparer.Instance.Equals(this.keyValues[ordinal], other.keyValues[otherOrdinal]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as EntityDataKey);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="EntityDataKey"/>, for use in debugging and logging.
        /// </summary>
        /// <returns>String representation of this <see cref="EntityDataKey"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.Append("{ ");

            string separator = string.Empty;
            foreach (string name in this.KeyNames)
            {
                sb.Append(separator);
                sb.Append(name).Append("=").Append(this.GetValue(name));
                separator = "; ";
            }

            sb.Append(" }");

            return sb.ToString();
        }

        internal void ImportFrom(object value)
        {
            if (value == null)
            {
                throw new TaupoInvalidOperationException("Cannot import " + typeof(EntityDataKey).Name + " from null object.");
            }

            if (primitiveTypes.Contains(value.GetType()))
            {
                // singleton key
                if (this.KeyNames.Count == 1)
                {
                    this.SetValue(this.KeyNames[0], value);
                }
                else
                {
                    throw new TaupoInvalidOperationException("Cannot import composite " + typeof(EntityDataKey).Name + " from singleton primitive value.");
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
                {
                    this.SetValue(propertyInfo.Name, propertyInfo.GetValue(value, null));
                }
            }
        }

        private int GetOrdinalAndCheck(string name)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            int ordinal = this.GetOrdinal(name);

            if (ordinal < 0)
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "This key does not contain member '{0}'.", name));
            }

            return ordinal;
        }

        private int GetOrdinal(string name)
        {
            return this.KeyNames.IndexOf(name);
        }
    }
}
