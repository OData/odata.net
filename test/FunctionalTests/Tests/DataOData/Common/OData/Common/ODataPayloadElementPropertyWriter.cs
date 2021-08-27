//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementPropertyWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    [ImplementationName(typeof(IODataPayloadElementPropertyWriter), "ODataPayloadElementPropertyWriter")]
    public sealed class ODataPayloadElementPropertyWriter : ODataPayloadElementVisitorBase, IODataPayloadElementPropertyWriter, IODataPayloadElementVisitor
    {
        private ODataMessageWriter writer = null;
        private List<ODataProperty> odataProperties = new List<ODataProperty>();

        private static ODataProperty CreateProperty(string name, object value)
        {
            ODataProperty prop = new ODataProperty()
            {
                Name = name,
                Value = value,
            };
            return prop;
        }

        /// <summary>
        /// The base for recursing a property to write a property using ODataMessageWriter
        /// </summary>
        /// <param name="writer">The message writer to use for writing the property</param>
        /// <param name="payload">The payload to be written as a property</param>
        public void WriteProperty(ODataMessageWriter writer, ODataPayloadElement payload)
        {
            this.writer = writer;
            payload.Accept(this);
            ExceptionUtilities.CheckObjectNotNull(this.odataProperties, "ODataProperty cannot be null");
            ExceptionUtilities.Assert(this.odataProperties.Count == 1, "There can be only one property left when writing the result");
            this.writer.WritePropertyAsync(this.odataProperties.First()).Wait();
            this.odataProperties.Clear();
        }

        /// <summary>
        /// Visits a primitive property and adds a property to the list of values to be written
        /// </summary>
        /// <param name="payloadElement">The property to be added to the list</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            this.odataProperties.Add(CreateProperty(payloadElement.Name, payloadElement.Value.ClrValue));
        }

        /// <summary>
        /// Visits a primitive multivalue property and adds a property to the list of values to be written
        /// </summary>
        /// <param name="payloadElement">The property to be added to the list</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            var items = payloadElement.Value.Select(item => item.ClrValue).ToList();
            this.odataProperties.Add(new ODataProperty()
            {
                Name = payloadElement.Name, 
                Value = new ODataCollectionValue()
                {
                    Items = items,
                    TypeName = payloadElement.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType.TestFullName()
                }
            });
        }

        /// <summary>
        /// Visits a complex property.  A copy of the current properties to be written is taken so that
        /// the complex property can use the global list to track its properties as its children are visited.
        /// Adds the complex property to the list of properties to be written.
        /// </summary>
        /// <param name="payloadElement">The complex property to visit and add</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            //Copy global property list to local variable and clear it so it can be used
            //for the complex properties children
            var arr = this.odataProperties;
            this.odataProperties = new List<ODataProperty>();

            base.Visit(payloadElement);
            ExceptionUtilities.CheckObjectNotNull(this.odataProperties, "ODataProperties cannot be null");

            //create a new complex property to add to the list from the properties children
            var complexValue = new ODataComplexValue() { TypeName = payloadElement.Value.FullTypeName };
            complexValue.Properties = this.odataProperties.ToList();//.AsEnumerable();

            //Return the global property list to its initial state with the new complex property added
            this.odataProperties.Clear();
            this.odataProperties.AddRange(arr);
            this.odataProperties.Add(CreateProperty(payloadElement.Name, complexValue));
        }

        /// <summary>
        /// Visits a complex property.  A copy of the current properties to be written is taken so that
        /// the complex property can use the global list to track its properties as its children are visited.
        /// Adds the complex property to the list of properties to be written.
        /// </summary>
        /// <param name="payloadElement">The complex property to visit and add</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            // Save off properties of parent
            var arr = this.odataProperties;
            var items = new List<ODataComplexValue>();
            foreach (var item in payloadElement.Value)
            {
                this.odataProperties = new List<ODataProperty>();
                
                this.Visit(item);
                ExceptionUtilities.CheckObjectNotNull(this.odataProperties, "ODataProperties cannot be null");
                var typename = item.FullTypeName.Substring(11, item.FullTypeName.Length - 12);
                var complexValue = new ODataComplexValue() { TypeName = typename };
                complexValue.Properties = this.odataProperties.ToList();//.AsEnumerable();
                items.Add(complexValue);
            }

            //Return the global property list to its initial state with the new complex property added
            this.odataProperties.Clear();
            this.odataProperties.AddRange(arr);

            //create a new complex collection property to add to the list from the properties children
            this.odataProperties.Add(new ODataProperty()
            {
                Name = payloadElement.Name, 
                Value = new ODataCollectionValue()
                {
                    Items = items, TypeName = payloadElement.Value.FullTypeName
                }
            });
        }

        /// <summary>
        /// Visits a null property instance and adds a property with a name and null value
        /// </summary>
        /// <param name="payloadElement">Null property to add to writter list</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            this.odataProperties.Add(CreateProperty(payloadElement.Name, null));
        }
    }
}
