//---------------------------------------------------------------------
// <copyright file="IODataPayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for building odata payloads
    /// </summary>
    [ImplementationSelector("ODataPayloadBuilder", DefaultImplementation = "Default")]
    public interface IODataPayloadBuilder
    {
        /// <summary>
        /// Builds a complex type instance of the given type out of the given anonymous object
        /// </summary>
        /// <param name="complexType">The metadata type information for the complex type</param>
        /// <param name="anonymous">The data as an anonymous type</param>
        /// <returns>An entity instance with the given values</returns>
        ComplexInstance ComplexInstance(ComplexType complexType, object anonymous);

        /// <summary>
        /// Builds an entity instance of the given type out of the given anonymous object
        /// </summary>
        /// <param name="entityType">The metadata type information for the entity</param>
        /// <param name="anonymous">The data as an anonymous type</param>
        /// <returns>An entity instance with the given values</returns>
        EntityInstance EntityInstance(EntityType entityType, object anonymous);

        /// <summary>
        /// Constructs an entity with the given property values
        /// </summary>
        /// <param name="entityType">The metadata for the entity type</param>
        /// <param name="namedValues">The property values. Keys are expected to be '.' delimited property paths.</param>
        /// <returns>An entity instance with the given values</returns>
        EntityInstance EntityInstance(EntityType entityType, IEnumerable<NamedValue> namedValues);

        /// <summary>
        /// Builds a primitive property instance for the given metadata property with the given value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="value">The property value</param>
        /// <returns>A primitive property instance</returns>
        PrimitiveProperty PrimitiveProperty(MemberProperty memberProperty, object value);

        /// <summary>
        /// Builds a complex property instance for the given metadata property with the given anonymous type value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="anonymous">An anonymous type describing the property values of the complex instance</param>
        /// <returns>A complex property instance</returns>
        ComplexProperty ComplexProperty(MemberProperty memberProperty, object anonymous);

        /// <summary>
        /// Builds a complex property instance for the given metadata property with the given flattened values
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="namedValues">The flattened values</param>
        /// <returns>A complex property instance</returns>
        ComplexProperty ComplexProperty(MemberProperty memberProperty, IEnumerable<NamedValue> namedValues);

        /// <summary>
        /// Builds a collection property instance for the given metadata property with the given anonymous type value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="anonymous">An anonymous type contianing the property values of the collection</param>
        /// <returns>A complex property instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        PropertyInstance MultiValueProperty(MemberProperty memberProperty, object anonymous);

        /// <summary>
        /// Builds a property instance for the given value without a metadata representation
        /// Cannot infer type names on complex properties
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="valueType">The clr type of the property's value</param>
        /// <param name="value">The value of the property</param>
        /// <returns>A property instance</returns>
        PropertyInstance DynamicProperty(string name, Type valueType, object value);

        /// <summary>
        /// Constructs a complex instance with the given property values
        /// </summary>
        /// <param name="complexType">The metadata for the complex type</param>
        /// <param name="namedValues">The property values. Keys are expected to be '.' delimited property paths.</param>
        /// <returns>A complex instance with the given values</returns>
        ComplexInstance ComplexInstance(ComplexType complexType, IEnumerable<NamedValue> namedValues);

        /// <summary>
        /// Builds an empty or null MultiValue property for the ODataPayloadElement
        /// </summary>
        /// <param name="memberProperty">Property to build a null or empty value for</param>
        /// <param name="value">Value MUST be null or EmptyData.Value</param>
        /// <returns>A PropertyInstance ODataPayloadElement</returns>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        PropertyInstance MultiValuePropertyEmptyOrNull(MemberProperty memberProperty, object value);
    }
}