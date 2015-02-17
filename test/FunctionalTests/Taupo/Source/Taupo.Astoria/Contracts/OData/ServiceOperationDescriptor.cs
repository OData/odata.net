//---------------------------------------------------------------------
// <copyright file="ServiceOperationDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// Represents descriptor information for function or action
    /// </summary>
    public class ServiceOperationDescriptor : ODataPayloadElement
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Descriptor is an function or nots 
        /// </summary>
        public bool IsFunction 
        {
            get 
            {
                return !this.IsAction;
            }

            set
            {
                this.IsAction = !value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Descriptor is an action or nots 
        /// </summary>
        public bool IsAction { get; set; }

        /// <summary>
        /// Gets or sets the value of the Metadata
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Target
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets the String representation of the Service Operation Descriptor
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "IsFunction={0}, IsAction={1}, Metadata={2}, Title={3}, Target={4}", this.IsFunction, this.IsAction, this.Metadata, this.Title, this.Target);
            }
        }

        /// <summary>
        /// Visits an ServiceOperationDescriptor and returns the result
        /// </summary>
        /// <typeparam name="TResult">Result Type</typeparam>
        /// <param name="visitor">visitor to use</param>
        /// <returns>Result from visiting Action Descriptor</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Visits an ServiceOperationDescriptor
        /// </summary>
        /// <param name="visitor">visitor to use</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
