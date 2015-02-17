//---------------------------------------------------------------------
// <copyright file="IJsonValueVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Visits a Json value using the double dispatch visitor pattern.
    /// </summary>
    public interface IJsonValueVisitor
    {
        /// <summary>
        /// Visits a json object.
        /// </summary>
        /// <param name="jsonObject">The json object being visited.</param>
        void Visit(JsonObject jsonObject);

        /// <summary>
        /// Visits a json array.
        /// </summary>
        /// <param name="jsonArray">The json array being visited.</param>
        void Visit(JsonArray jsonArray);

        /// <summary>
        /// Visits a json primitive value.
        /// </summary>
        /// <param name="primitive">The json primitive being visited.</param>
        void Visit(JsonPrimitiveValue primitive);

        /// <summary>
        /// Visits a json property.
        /// </summary>
        /// <param name="jsonProperty">The json property being visited.</param>
        void Visit(JsonProperty jsonProperty);
    }
}