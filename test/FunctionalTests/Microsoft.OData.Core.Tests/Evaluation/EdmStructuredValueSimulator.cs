//---------------------------------------------------------------------
// <copyright file="EdmStructuredValueSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class EdmStructuredValueSimulator : IEdmStructuredValue
    {
        public EdmStructuredValueSimulator(IEdmTypeReference typeReference, IEnumerable<KeyValuePair<string, IEdmValue>> values)
        {
            this.Type = typeReference;
            this.PropertyValues =
                values.Select(p => new EdmPropertyValue(p.Key, p.Value))
                    .ToList();
        }

        public EdmStructuredValueSimulator(IEdmEntityType entityType, IEnumerable<KeyValuePair<string, IEdmValue>> values)
            : this(entityType == null ? null : new EdmEntityTypeReference(entityType, false), values)
        {
        }

        public EdmStructuredValueSimulator()
        {
            this.Type = null;
            this.PropertyValues = Enumerable.Empty<IEdmPropertyValue>();
        }

        public IEdmTypeReference Type { get; private set; }

        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.Structured; }
        }

        public IEnumerable<IEdmPropertyValue> PropertyValues { get; private set; }

        public IEdmPropertyValue FindPropertyValue(string propertyName)
        {
            return this.PropertyValues.FirstOrDefault(p => p.Name == propertyName);
        }
    }
}