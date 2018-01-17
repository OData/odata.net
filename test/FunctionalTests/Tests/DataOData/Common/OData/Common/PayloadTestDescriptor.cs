//---------------------------------------------------------------------
// <copyright file="PayloadTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Test descriptor for a payload.
    /// </summary>
    public class PayloadTestDescriptor: IAnnotatable<PayloadTestDescriptorAnnotation>
    {
        /// <summary>
        /// The payload kind which is being tested.
        /// If the func is specified it is executed before the test is ran for a given test configuration.
        /// If the func returns true, the test is not ran and "success" is reported right away.
        /// </summary>
        private ODataPayloadKind payloadKind = ODataPayloadKind.Unsupported;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PayloadTestDescriptor()
        {
            this.Annotations = new List<PayloadTestDescriptorAnnotation>();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other test descriptor to copy values from.</param>
        public PayloadTestDescriptor(PayloadTestDescriptor other)
        {
            this.PayloadModel = other.PayloadModel;
            this.PayloadElementModelContainer = other.PayloadElementModelContainer;
            this.PayloadElement = other.PayloadElement;
            this.SkipTestConfiguration = other.SkipTestConfiguration;
            this.Annotations = new List<PayloadTestDescriptorAnnotation>(other.Annotations);
            this.IsGeneratedPayload = other.IsGeneratedPayload;
            this.PayloadEdmElementContainer = other.PayloadEdmElementContainer;
            this.PayloadEdmModel = other.PayloadEdmModel;
        }

        /// <summary>
        /// true if this test descriptor was produced via a payload generator; otherwise false.
        /// </summary>
        public bool IsGeneratedPayload { get; set; }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// Can be null in which case the payload will execute without metadata.
        /// </summary>
        public EntityModelSchema PayloadModel { get; set; }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// Can be null in which case the payload will execute without metadata.
        /// </summary>
        public IEdmModel PayloadEdmModel { get; set; }

        /// <summary>
        /// The container in the model for the test payload. 
        /// For entity sets this is their entity container, for properties it is their owning type.
        /// </summary>        
        public AnnotatedItem PayloadElementModelContainer { get; set; }

        /// <summary>
        /// The data type of the value being written.
        /// </summary>
        public AnnotatedItem PayloadElementType { get; set; }

        /// <summary>
        /// The container in the model for the test payload. 
        /// For entity sets this is their entity container, for properties it is their owning type.
        /// Added for using EdmLib Elements
        /// </summary>        
        public IEdmElement PayloadEdmElementContainer { get; set; }

        /// <summary>
        /// The data type of the value being written.
        /// Added for using EdmLib Elements
        /// </summary>
        public IEdmElement PayloadEdmElementType { get; set; }

        /// <summary>
        /// The payload element to write as the input for the reader.
        /// And if the expected result is not specified differently, this will also be the expected result of the reader.
        /// </summary>
        public ODataPayloadElement PayloadElement { get; set; }

        /// <summary>
        /// If the func is specified it is executed before the test is ran for a given test configuration.
        /// If the func returns true, the test is not ran and "success" is reported right away.
        /// </summary>
        public Func<TestConfiguration, bool> SkipTestConfiguration { get; set; }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public virtual ODataPayloadKind PayloadKind
        {
            get
            {
                if (this.payloadKind == ODataPayloadKind.Unsupported)
                {
                    this.payloadKind = this.PayloadElement.GetPayloadKindFromPayloadElement();
                }

                return this.payloadKind;
            }

            set
            {
                this.payloadKind = value;
            }
        }

        /// <summary>
        /// A description of the payload, used for debugging.
        /// </summary>
        public string DebugDescription { get; set; }

        /// <summary>
        /// Returns text description of the test.
        /// </summary>
        /// <returns>A humanly readable description of the test, used for debugging.</returns>
        public override string ToString()
        {
            string result = string.Format("PayloadKind: {0}", this.PayloadKind);
            if (this.DebugDescription != null)
            {
                result = this.DebugDescription + Environment.NewLine + result;
            }

            return result;
        }
        
        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected virtual bool ShouldSkipForTestConfiguration<T>(T testConfiguration) where T: TestConfiguration
        {
            if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a copy of this PayloadTestDescriptor
        /// </summary>
        public virtual object Clone()
        {
            return new PayloadTestDescriptor(this);
        }

        /// <summary>
        /// The list of annotations associated with this PayloadTestDescriptor
        /// </summary>
        public IList<PayloadTestDescriptorAnnotation> Annotations { get; private set; }
    }
}
