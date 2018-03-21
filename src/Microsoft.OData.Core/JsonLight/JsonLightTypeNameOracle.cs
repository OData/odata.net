﻿//---------------------------------------------------------------------
// <copyright file="JsonLightTypeNameOracle.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in JSON Light.
    /// </summary>
    internal abstract class JsonLightTypeNameOracle : TypeNameOracle
    {
        /// <summary>
        /// Determines the resource set type name to write to the payload.
        /// </summary>
        /// <param name="expectedResourceTypeName">The expected resource type name of the items in the resource set.</param>
        /// <param name="resourceSet">The ODataResourceSet whose type is to be written.</param>
        /// <param name="isUndeclared">true if the resource set is for some undeclared property.</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        internal abstract string GetResourceSetTypeNameForWriting(string expectedResourceTypeName, ODataResourceSet resourceSet, bool isUndeclared);

        /// <summary>
        /// Determines the entity type name to write to the payload.
        /// </summary>
        /// <param name="expectedTypeName">The expected type name, e.g. the base type of the set or the nav prop.</param>
        /// <param name="resource">The ODataResource whose type is to be written.</param>
        /// <param name="isUndeclared">true if the resource is for some undeclared property.</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        internal abstract string GetResourceTypeNameForWriting(string expectedTypeName, ODataResourceBase resource, bool isUndeclared);

        /// <summary>
        /// Determines the type name to write to the payload.  Json Light type names are only written into the payload for open properties
        /// or if the payload type name is more derived than the model type name.
        /// </summary>
        /// <param name="value">The ODataValue whose type name is to be written.</param>
        /// <param name="typeReferenceFromMetadata">The type as expected by the model.</param>
        /// <param name="typeReferenceFromValue">The type resolved from the value.</param>
        /// <param name="isOpenProperty">true if the type name belongs to an open property, false otherwise.</param>
        /// <returns>Type name to write to the payload, or null if no type should be written.</returns>
        internal abstract string GetValueTypeNameForWriting(
            ODataValue value,
            IEdmTypeReference typeReferenceFromMetadata,
            IEdmTypeReference typeReferenceFromValue,
            bool isOpenProperty);

        /// <summary>
        /// Determines the type name to write to the payload.  Json Light type names are only written into the payload for open properties
        /// or if the payload type name is more derived than the model type name.
        /// </summary>
        /// <param name="value">The ODataValue whose type name is to be written.</param>
        /// <param name="propertyInfo">The serialization info of current property</param>
        /// <param name="isOpenProperty">true if the type name belongs to an open property, false otherwise.</param>
        /// <returns>Type name to write to the payload, or null if no type should be written.</returns>
        internal abstract string GetValueTypeNameForWriting(
            ODataValue value,
            PropertySerializationInfo propertyInfo,
            bool isOpenProperty);
    }
}
