//---------------------------------------------------------------------
// <copyright file="JsonReaderAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Json;

namespace Microsoft.OData.Tests.JsonLight
{
    public class JsonReaderAssertions
    {
        private IJsonReader jsonReader;

        internal JsonReaderAssertions(IJsonReader jsonReader)
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