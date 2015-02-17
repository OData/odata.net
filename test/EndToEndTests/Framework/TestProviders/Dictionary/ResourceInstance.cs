//---------------------------------------------------------------------
// <copyright file="ResourceInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;

    /// <summary>
    /// A resource instance that stores property values in a dictionary
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Doesn't need to be serialized")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Current name is fine")]
    public class ResourceInstance : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the ResourceInstance class with the given type
        /// </summary>
        /// <param name="type">The type of the instance</param>
        public ResourceInstance(ResourceType type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceInstance class with the given type and set
        /// </summary>
        /// <param name="type">The type of the instance</param>
        /// <param name="set">The set the instance belongs to</param>
        public ResourceInstance(ResourceType type, ResourceSet set)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            if (type.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                ExceptionUtilities.CheckArgumentNotNull(set, "set");
                this.IsEntityType = true;
                this.ResourceSetName = set.Name;
            }
            else
            {
                ExceptionUtilities.Assert(type.ResourceTypeKind == ResourceTypeKind.ComplexType, "Only entity and complex types can be represented. Type was: '{0}'", type);
                ExceptionUtilities.Assert(set == null, "Complex types cannot have sets");
                this.IsEntityType = false;
            }

            this.ResourceTypeName = type.FullName;
        }

        /// <summary>
        /// Gets the type name for this instance
        /// </summary>
        public string ResourceTypeName { get; private set; }

        /// <summary>
        /// Gets the set name for this instance, will be null if this is a complex type.
        /// </summary>
        public string ResourceSetName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this is an entity type. False indicates that it is a complex type.
        /// </summary>
        public bool IsEntityType { get; private set; }

        /// <summary>
        /// Returns whether or not the two property values are equivalent
        /// </summary>
        /// <param name="value1">The first value</param>
        /// <param name="value2">The second value</param>
        /// <returns>True if they are equivalent, otherwise false</returns>
        public static bool ArePropertyValuesEqual(object value1, object value2)
        {
            var value1Binary = value1 as byte[];
            if (value1Binary != null)
            {
                var value2Binary = value2 as byte[];
                if (value2Binary == null)
                {
                    return false;
                }

                if (!value1Binary.SequenceEqual(value2Binary))
                {
                    return false;
                }
            }
            else if (value1 == null)
            {
                return value2 == null;
            }
            else
            {
                return value1.Equals(value2);
            }

            return true;
        }
        
        /// <summary>
        /// Gets a property value or null
        /// </summary>
        /// <param name="propertyName">Name of property to get</param>
        /// <returns>value of property or null if nothing is found</returns>
        public object GetPropertyOrNull(string propertyName)
        {
            if (this.ContainsKey(propertyName))
            {
                return this[propertyName];
            }

            return null;
        }

        /// <summary>
        /// Returns the default value for an object of the given type
        /// </summary>
        /// <typeparam name="TObject">The type to get the default for</typeparam>
        /// <returns>The default value of the type</returns>
        protected TObject GetDefaultValue<TObject>()
        {
            return default(TObject);
        }
    }
}