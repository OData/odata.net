//---------------------------------------------------------------------
// <copyright file="JsonReaderAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.Json
{
    using FluentAssertions;
    using Microsoft.OData.Core.Json;

    public class JsonReaderAssertions
    {
        private JsonReader jsonReader;

        internal JsonReaderAssertions(JsonReader jsonReader)
        {
            this.jsonReader = jsonReader;
        }

        internal AndConstraint<JsonReaderAssertions> BeOn(JsonNodeType nodeType, object value)
        {
            return this.BeOn(nodeType, value, null);
        }

        internal AndConstraint<JsonReaderAssertions> BeOn(JsonNodeType nodeType, object value, string reason, params object[] reasonArgs)
        {
            this.jsonReader.NodeType.Should().Be(nodeType, reason, reasonArgs);
            this.jsonReader.Value.Should().Be(value, reason, reasonArgs);
            return new AndConstraint<JsonReaderAssertions>(this);
        }
    }
}