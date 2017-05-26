//---------------------------------------------------------------------
// <copyright file="EntityInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a single entity
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EntityInstance : ComplexInstance, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the EntityInstance class
        /// </summary>
        public EntityInstance()
            : this(null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityInstance class with the given values
        /// </summary>
        /// <param name="fullTypeName">The full type name of the value</param>
        /// <param name="isNull">Whether or not the value is null</param>
        public EntityInstance(string fullTypeName, bool isNull)
            : this(fullTypeName, isNull, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntityInstance class with the given values
        /// </summary>
        /// <param name="fullTypeName">The full type name of the value</param>
        /// <param name="isNull">Whether or not the value is null</param>
        /// <param name="idLink">The id link for the entity</param>
        /// <param name="entityETag">The ETag of the entity</param>
        public EntityInstance(string fullTypeName, bool isNull, string idLink, string entityETag)
            : base(fullTypeName, isNull)
        {
            this.Id = idLink;
            this.ETag = entityETag;
            this.ServiceOperationDescriptors = new List<ServiceOperationDescriptor>();
        }

        /// <summary>
        /// Gets or sets the IsComplex for the enti
        /// </summary>
        public bool IsComplex { get; set; }
        /// <summary>
        /// Gets or sets the ETag for the entity
        /// </summary>
        public string ETag { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the entity
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the edit link of the entity
        /// </summary>
        public string EditLink { get; set; }

        /// <summary>
        /// Gets or sets the stream ETag
        /// </summary>
        public string StreamETag { get; set; }

        /// <summary>
        /// Gets or sets the stream edit link
        /// </summary>
        public string StreamEditLink { get; set; }

        /// <summary>
        /// Gets or sets the stream source link
        /// </summary>
        public string StreamSourceLink { get; set; }

        /// <summary>
        /// Gets or sets the stream content type
        /// </summary>
        public string StreamContentType { get; set; }

        /// <summary>
        /// Gets the ServiceOperationDescriptors for the entity instance
        /// </summary>
        public IList<ServiceOperationDescriptor> ServiceOperationDescriptors { get; private set; }

        /// <summary>
        /// Adds a ServiceOperationDescriptor to the list of Action/Function Descriptors based on its type
        /// </summary>
        /// <param name="serviceOperationDescriptor">An ActionDescriptor or a FunctionDescriptor to add</param>
        public void Add(ServiceOperationDescriptor serviceOperationDescriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(serviceOperationDescriptor, "serviceOperationDescriptor");
            this.ServiceOperationDescriptors.Add(serviceOperationDescriptor);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}