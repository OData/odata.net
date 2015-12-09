//---------------------------------------------------------------------
// <copyright file="ODataEntryExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    /// <summary>
    /// Extensions methods for building OData Entries
    /// </summary>
    public static class ODataEntryBuilderExtensions
    {
        public static void Property(this ODataEntry entry, string propertyName, object value)
        {
            List<ODataProperty> properties = entry.Properties as List<ODataProperty>;
            if (properties == null)
            {
                properties = new List<ODataProperty>();
                if (entry.Properties != null)
                {
                    properties.AddRange(entry.Properties);
                }
            }

            properties.Add(new ODataProperty() {Name = propertyName, Value = value});
            entry.Properties = properties;
        }
    }
}
