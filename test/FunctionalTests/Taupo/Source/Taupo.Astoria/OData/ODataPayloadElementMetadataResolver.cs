//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementMetadataResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Default implementation of the payload element medatadata resolver contract
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementMetadataResolver), "Default")]
    public class ODataPayloadElementMetadataResolver : ODataPayloadElementVisitorBase, IODataPayloadElementMetadataResolver
    {
        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementMetadataResolver class
        /// </summary>
        public ODataPayloadElementMetadataResolver()
        {
            this.MetadataStack = new Stack<object>();
        }

        /// <summary>
        /// Gets the metadata stack. Should not be modified directly.
        /// </summary>
        internal Stack<object> MetadataStack { get; private set; }

        /// <summary>
        /// Gets or sets the initial stack size of the entity to help determine if the current element is at the payload root. Should not be modified directly.
        /// </summary>
        internal int InitialStackSize { get; set; }

        private EntitySet CurrentEntitySet
        {
            get
            {
                return this.MetadataStack.OfType<EntitySet>().FirstOrDefault(); // first will return the 'highest' item on the stack
            }
        }

        private bool IsRootElement
        {
            get
            {
                return this.MetadataStack.Count == this.InitialStackSize;
            }
        }
                
        /// <summary>
        /// Annotates the given payload based on the metadata in the given uri
        /// </summary>
        /// <param name="rootElement">The payload to annotate with metadata information</param>
        /// <param name="uri">The uri that corresponds to the given payload</param>
        public void ResolveMetadata(ODataPayloadElement rootElement, ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");

            this.InitializeMetadataStack(uri);
            this.InitialStackSize = this.MetadataStack.Count;

            rootElement.Add(new ExpectedPayloadElementTypeAnnotation() { ExpectedType = uri.GetExpectedPayloadType() });

            // if the uri did not contain any metadata, do nothing
            if (this.InitialStackSize > 0)
            {
                // if this is the result of service operation or action, the root element needs to have the function itself and its return type
                var serviceOperation = this.MetadataStack.OfType<Function>().FirstOrDefault();
                if (serviceOperation != null)
                {
                    rootElement.AddAnnotationIfNotExist(new FunctionAnnotation() { Function = serviceOperation });
                    rootElement.AddAnnotationIfNotExist(new DataTypeAnnotation() { DataType = serviceOperation.ReturnType });
                }

                this.Recurse(rootElement);
            }
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitComplexOrEntityInstance(payloadElement);
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(DeferredLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var navigation = this.MetadataStack.OfType<NavigationProperty>().FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(navigation, "No navigation property found on metadata stack");
            payloadElement.AddAnnotationIfNotExist(new NavigationPropertyAnnotation() { Property = navigation });
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EmptyUntypedCollection payloadElement)
        {
            // an empty entity set at the top level can appear as an untyped collection
            if (this.IsRootElement)
            {
                var entitySet = this.MetadataStack.Peek() as EntitySet;
                if (entitySet != null)
                {
                    payloadElement.AddAnnotationIfNotExist(new EntitySetAnnotation() { EntitySet = entitySet });
                    return;
                }
            }

            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitComplexOrEntityInstance(payloadElement);
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var entitySet = this.MetadataStack.Peek() as EntitySet;
            ExceptionUtilities.CheckObjectNotNull(entitySet, "Expected entity set, got '{0}'", this.MetadataStack.Peek());

            payloadElement.AddAnnotationIfNotExist(new EntitySetAnnotation() { EntitySet = entitySet });
            foreach (var element in payloadElement)
            {
                this.Recurse(element);
            }
        }
        
        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var navigation = this.MetadataStack.Peek() as NavigationProperty;
            ExceptionUtilities.CheckObjectNotNull(navigation, "Expected navigation, got '{0}'", this.MetadataStack.Peek());
            payloadElement.AddAnnotationIfNotExist(new NavigationPropertyAnnotation() { Property = navigation });

            if (payloadElement.ExpandedElement != null)
            {
                try
                {
                    this.MetadataStack.Push(this.CurrentEntitySet.GetRelatedEntitySet(navigation));
                    this.Recurse(payloadElement.ExpandedElement);
                }
                finally
                {
                    this.MetadataStack.Pop();
                }
            }
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(LinkCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var navigation = this.MetadataStack.OfType<NavigationProperty>().FirstOrDefault();
            ExceptionUtilities.CheckObjectNotNull(navigation, "No navigation property found on metadata stack");
            payloadElement.AddAnnotationIfNotExist(new NavigationPropertyAnnotation() { Property = navigation });
            
            foreach (var link in payloadElement)
            {
                this.Recurse(link);
            }
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var navigation = this.MetadataStack.Peek() as NavigationProperty;
            ExceptionUtilities.CheckObjectNotNull(navigation, "Expected navigation, got '{0}'", this.MetadataStack.Peek());
            ExceptionUtilities.Assert(payloadElement.Name == navigation.Name, "Property name mismatch");
            payloadElement.AddAnnotationIfNotExist(new NavigationPropertyAnnotation() { Property = navigation });
            this.Recurse(payloadElement.Value);
        }
        
        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            
            // It will not always be a Primitive DataType, it may be an empty value of some collection type from a serviceoperation or action
            var dataType = this.MetadataStack.Peek() as DataType;
            ExceptionUtilities.CheckObjectNotNull(dataType, "Expected primitive data type, got '{0}'", this.MetadataStack.Peek());
            payloadElement.AddAnnotationIfNotExist(new DataTypeAnnotation() { DataType = dataType });
        }
        
        /// <summary>
        /// Helper method for visiting properties
        /// </summary>
        /// <param name="payloadElement">The property to visit</param>
        /// <param name="value">The value of the property</param>
        protected override void VisitProperty(PropertyInstance payloadElement, ODataPayloadElement value)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var property = this.MetadataStack.Peek() as MemberProperty;
            if (property != null)
            {
                ExceptionUtilities.Assert(payloadElement.Name == property.Name, "Property name mismatch");
                payloadElement.AddAnnotationIfNotExist(new MemberPropertyAnnotation() { Property = property });

                try
                {
                    this.MetadataStack.Push(property.PropertyType);
                    base.VisitProperty(payloadElement, value);
                }
                finally
                {
                    this.MetadataStack.Pop();
                }
            }
            else
            {
                var navprop = this.MetadataStack.Peek() as NavigationProperty;
                if (navprop != null)
                {
                    ExceptionUtilities.Assert(payloadElement.Name == navprop.Name, "Property name mismatch");
                    try
                    {
                        this.MetadataStack.Push(this.CurrentEntitySet.GetRelatedEntitySet(navprop));
                        base.VisitProperty(payloadElement, value);
                    }
                    finally
                    {
                        this.MetadataStack.Pop();
                    }
                }
                else
                {
                    var serviceOperation = this.MetadataStack.OfType<Function>().SingleOrDefault(f => f.Name.Equals(payloadElement.Name, StringComparison.Ordinal));
                    if (payloadElement.Name.Contains('.'))
                    {
                        // if the action has a name collision with a property in the entity, it will have the <container name>.<action name> in the payload
                        // split the payloadElement.Name and get the last token which will match the function name
                        string lastToken = payloadElement.Name.Split('.').Last();
                        serviceOperation = this.MetadataStack.OfType<Function>().SingleOrDefault(f => f.Name.Equals(lastToken, StringComparison.Ordinal));
                    }

                    ExceptionUtilities.CheckObjectNotNull(serviceOperation, "Expected property, got " + this.MetadataStack.Peek().ToString());

                    // Non-entity values returned from a service operation call appear identical to member properties,
                    // with the operation name as the property name
                    base.VisitProperty(payloadElement, value);
                }
            }
        }

        /// <summary>
        /// Helper method for visiting collections
        /// </summary>
        /// <param name="payloadElement">The collection to visit</param>
        protected override void VisitCollection(ODataPayloadElementCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var dataType = this.MetadataStack.Peek() as CollectionDataType;
            if (dataType != null)
            {
                ExceptionUtilities.CheckObjectNotNull(dataType, "Expected collection data type, got '{0}'", this.MetadataStack.Peek());
                payloadElement.AddAnnotationIfNotExist(new DataTypeAnnotation() { DataType = dataType });

                try
                {
                    this.MetadataStack.Push(dataType.ElementDataType);
                    base.VisitCollection(payloadElement);
                }
                finally
                {
                    this.MetadataStack.Pop();
                }
            }
            else
            {
                var entitySet = this.MetadataStack.Peek() as EntitySet;
                ExceptionUtilities.CheckObjectNotNull(entitySet, "Expected entity set, got '{0}'", this.MetadataStack.Peek());
                payloadElement.Add(new EntitySetAnnotation() { EntitySet = entitySet });
                base.VisitCollection(payloadElement);
            }
        }

        /// <summary>
        /// Helper method for visiting null or empty properties
        /// </summary>
        /// <param name="payloadElement">The property to visit</param>
        protected override void VisitEmptyOrNullProperty(PropertyInstance payloadElement)
        {
            base.VisitEmptyOrNullProperty(payloadElement);

            var property = this.MetadataStack.Peek() as MemberProperty;
            var navigation = this.MetadataStack.Peek() as NavigationProperty;

            if (property != null)
            {
                ExceptionUtilities.Assert(payloadElement.Name == property.Name, "Property name mismatch");
                payloadElement.AddAnnotationIfNotExist(new MemberPropertyAnnotation() { Property = property });
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(navigation, "Expected property or navigation, got '{0}'", this.MetadataStack.Peek());
                ExceptionUtilities.Assert(payloadElement.Name == navigation.Name, "Property name mismatch");
                payloadElement.AddAnnotationIfNotExist(new NavigationPropertyAnnotation() { Property = navigation });

                var emptyCollection = payloadElement as EmptyCollectionProperty;
                if (emptyCollection != null && emptyCollection.Value != null)
                {
                    emptyCollection.Value.AddAnnotationIfNotExist(new EntitySetAnnotation() { EntitySet = this.CurrentEntitySet.GetRelatedEntitySet(navigation) });
                }
            }
        }

        private void VisitComplexOrEntityInstance(ComplexInstance payloadElement)
        {
            var entitySet = this.MetadataStack.Peek() as EntitySet;
            if (entitySet != null)
            {
                payloadElement.AddAnnotationIfNotExist(new EntitySetAnnotation() { EntitySet = entitySet });

                // start with the entity type from the set
                var entityType = entitySet.EntityType;

                // if the payload has a type name in it, and its different from the entity type on the set...
                if (payloadElement.FullTypeName != null && payloadElement.FullTypeName != entityType.FullName)
                {
                    // ... then go find the more specific type in the model and use that instead (if it exists)
                    var specificEntityType = entitySet.Container.Model.EntityTypes.SingleOrDefault(t => t.IsKindOf(entityType) && t.FullName == payloadElement.FullTypeName);
                    if (specificEntityType != null)
                    {
                        entityType = specificEntityType;
                    }
                }

                if (entityType != null)
                {
                    payloadElement.AddAnnotationIfNotExist(new DataTypeAnnotation() { DataType = DataTypes.EntityType.WithDefinition(entityType) });
                    this.VisitMemberProperties(payloadElement, entityType.AllProperties);
                    this.VisitNavigationProperties(payloadElement.Properties, entityType.AllNavigationProperties);
                }
            }
            else
            {
                var dataType = this.MetadataStack.Peek() as ComplexDataType;
                ExceptionUtilities.CheckObjectNotNull(dataType, "Expected complex data type, got '{0}'", this.MetadataStack.Peek());
                payloadElement.AddAnnotationIfNotExist(new DataTypeAnnotation() { DataType = dataType });
                this.VisitMemberProperties(payloadElement, dataType.Definition.Properties);
            }
        }

        private void VisitMemberProperties(ComplexInstance payloadElement, IEnumerable<MemberProperty> properties)
        {
            foreach (var propertyInstance in payloadElement.Properties)
            {
                var nextProperty = properties.SingleOrDefault(p => p.Name == propertyInstance.Name);
                if (nextProperty != null)
                {
                    try
                    {
                        this.MetadataStack.Push(nextProperty);
                        this.Recurse(propertyInstance);
                    }
                    finally
                    {
                        this.MetadataStack.Pop();
                    }
                }
            }
        }

        private void VisitNavigationProperties(IEnumerable<PropertyInstance> payloadProperties, IEnumerable<NavigationProperty> properties)
        {
            foreach (var navigation in payloadProperties)
            {
                var nextNavigation = properties.SingleOrDefault(n => n.Name == navigation.Name);
                if (nextNavigation != null)
                {
                    try
                    {
                        this.MetadataStack.Push(nextNavigation);
                        this.Recurse(navigation);
                    }
                    finally
                    {
                        this.MetadataStack.Pop();
                    }
                }
            }
        }

        private void InitializeMetadataStack(ODataUri uri)
        {
            this.MetadataStack.Clear();
            EntitySet currentSet = null;
            foreach (var segment in uri.Segments)
            {
                var setSegment = segment as EntitySetSegment;
                if (setSegment != null)
                {
                    this.MetadataStack.Push(setSegment.EntitySet);
                    currentSet = setSegment.EntitySet;
                    continue;
                }

                var navigationSegment = segment as NavigationSegment;
                if (navigationSegment != null)
                {
                    this.MetadataStack.Push(navigationSegment.NavigationProperty);
                    this.MetadataStack.Push(this.CurrentEntitySet.GetRelatedEntitySet(navigationSegment.NavigationProperty));
                    currentSet = currentSet.GetRelatedEntitySet(navigationSegment.NavigationProperty);
                    continue;
                }

                var propertySegment = segment as PropertySegment;
                if (propertySegment != null)
                {
                    this.MetadataStack.Push(propertySegment.Property);
                    continue;
                }

                var functionSegment = segment as FunctionSegment;
                if (functionSegment != null)
                {
                    EntitySet returningEntitySet;
                    if (this.InitializeFunctionMetadata(functionSegment.Function, currentSet, out returningEntitySet))
                    {
                        currentSet = returningEntitySet;
                    }

                    continue;
                }

                // special segment types
                if (segment.SegmentType == ODataUriSegmentType.Count)
                {
                    this.MetadataStack.Push(DataTypes.Integer);
                }
                else if (segment.SegmentType == ODataUriSegmentType.Value)
                {
                    var property = this.MetadataStack.Peek() as MemberProperty;
                    if (property != null)
                    {
                        this.MetadataStack.Push(property.PropertyType);    
                    }
                }
            }
        }

        private bool InitializeFunctionMetadata(Function function, EntitySet previousEntitySet, out EntitySet returningEntitySet)
        {
            // Push the function as the source of the response, and then
            // push further metadata describing the expected response, if appropriate
            this.MetadataStack.Push(function);

            EntitySet entitySet = null;
            bool foundExpectedEntitySet;
            ServiceOperationAnnotation serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (serviceOperationAnnotation == null)
            {
                foundExpectedEntitySet = function.TryGetExpectedServiceOperationEntitySet(out entitySet);
            }
            else
            {
                foundExpectedEntitySet = function.TryGetExpectedActionEntitySet(previousEntitySet, out entitySet);
            }

            if (foundExpectedEntitySet)
            {
                this.MetadataStack.Push(entitySet);
                returningEntitySet = entitySet;
                return true;
            }

            if (function.ReturnType != null)
            {
                this.MetadataStack.Push(function.ReturnType);
            }

            returningEntitySet = null;
            return false;
        }
    }
}
