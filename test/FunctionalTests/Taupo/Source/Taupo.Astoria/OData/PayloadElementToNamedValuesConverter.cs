//---------------------------------------------------------------------
// <copyright file="PayloadElementToNamedValuesConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Default implementation of the converter between payload elements and named-value pairs
    /// </summary>
    [ImplementationName(typeof(IPayloadElementToNamedValuesConverter), "Default")]
    public class PayloadElementToNamedValuesConverter : ODataPayloadElementVisitorBase, IPayloadElementToNamedValuesConverter
    {
        private readonly Stack<string> currentPath = new Stack<string>();
        private readonly IList<string> orderedNames = new List<string>();
        private readonly IDictionary<string, NamedValue> valuesByName = new Dictionary<string, NamedValue>();

        /// <summary>
        /// Converts the given payload into a series of named value pairs
        /// </summary>
        /// <param name="payload">The payload to convert</param>
        /// <returns>The named-value pairs represented by the given payload</returns>
        public IEnumerable<NamedValue> ConvertToNamedValues(ODataPayloadElement payload)
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.Assert(this.currentPath.Count == 0, "Stack was not empty");
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            
            this.orderedNames.Clear();
            this.valuesByName.Clear();
            
            this.Recurse(payload);

            ExceptionUtilities.Assert(this.orderedNames.Count == this.valuesByName.Count, "Number of names does not match number of values");
            return this.orderedNames.Select(n => this.valuesByName[n]);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            var epmTrees = payloadElement.Annotations.OfType<XmlTreeAnnotation>().ToList();

            if (epmTrees.Count > 0)
            {
                var entityType = payloadElement.Annotations
                    .OfType<DataTypeAnnotation>()
                    .Select(d => d.DataType)
                    .OfType<EntityDataType>()
                    .Select(d => d.Definition)
                    .SingleOrDefault();
                ExceptionUtilities.CheckObjectNotNull(entityType, "Could not get entity type from payload annotations");
            }

            // do EPM first so that in-content values win
            base.Visit(payloadElement);

            if (epmTrees.Count > 0)
            {
                // When deep EPM is involved, we can generate named values for mapped properties of a complex
                // property that has null value. So, trim any child paths of null values.
                var nullValueList = this.valuesByName.Where(v => v.Value.Value == null).ToList();
                foreach (var nullValue in nullValueList)
                {
                    var nullChildPaths = this.orderedNames.Where(n => n.StartsWith(nullValue.Key + ".", StringComparison.OrdinalIgnoreCase)).ToList();
                    foreach (var nullChildPath in nullChildPaths)
                    {
                        this.RemoveValue(nullChildPath);
                    }
                }
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            var value = payloadElement.ClrValue;
            this.AddValue(value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            if (payloadElement.IsNull)
            {
                // TODO: null or uninitializeddata?
                this.AddValue((object)null);
            }
            else
            {
                base.Visit(payloadElement);
            }
        }

        /// <summary>
        /// Helper method for visiting properties
        /// </summary>
        /// <param name="payloadElement">The property to visit</param>
        /// <param name="value">The value of the property</param>
        protected override void VisitProperty(PropertyInstance payloadElement, ODataPayloadElement value)
        {
            using (new DelegateBasedDisposable(() => this.currentPath.Pop()))
            {
                this.currentPath.Push(payloadElement.Name);
                base.VisitProperty(payloadElement, value);
            }
        }

        /// <summary>
        /// Helper method for visiting collections
        /// </summary>
        /// <param name="payloadElement">The collection to visit</param>
        protected override void VisitCollection(ODataPayloadElementCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var enumerable = payloadElement as IEnumerable;
            if (enumerable != null)
            {
                var elements = enumerable.Cast<ODataPayloadElement>().ToList();
                if (elements.Count == 0)
                {
                    this.AddValue(EmptyData.Value);
                }
                else
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        using (new DelegateBasedDisposable(() => this.currentPath.Pop()))
                        {
                            this.currentPath.Push(i.ToString(CultureInfo.InvariantCulture));
                            this.Recurse(elements[i]);
                        }
                    }
                }
            }
        }

        private void AddValue(object value)
        {
            var namedValue = new NamedValue(string.Join(".", this.currentPath.Reverse().ToArray()), value);
            this.AddValue(namedValue);
        }

        private void AddValue(NamedValue value)
        {
            if (this.valuesByName.ContainsKey(value.Name))
            {
                this.orderedNames.Remove(value.Name);
            }

            this.orderedNames.Add(value.Name);
            this.valuesByName[value.Name] = value;
        }

        private void RemoveValue(string name)
        {
            this.orderedNames.Remove(name);
            this.valuesByName.Remove(name);
        }
    }
}