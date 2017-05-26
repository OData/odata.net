//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementConfigurationValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    #endregion Namespaces

    /// <summary>
    /// Given an ODataPayloadElement this class determines if it can be used for a given test configuration.
    /// </summary>
    public class ODataPayloadElementConfigurationValidator
    {
        #region TestConfigurationLimits
        /// <summary>
        /// Class which describes the current test configuration limits.
        /// </summary>
        public class TestConfigurationLimits
        {
            /// <summary>
            /// true if the configuration must not be for a request.
            /// </summary>
            private bool disallowRequest = false;

            /// <summary>
            /// true if the configuration must not be for a response.
            /// </summary>
            private bool disallowResponse = false;

            /// <summary>
            /// The minimum payload version required.
            /// </summary>
            private ODataVersion minPayloadVersion = ODataVersion.V4;

            /// <summary>
            /// Sets the configuration limit to disallow requests.
            /// </summary>
            public void DisallowRequest()
            {
                this.disallowRequest = true;
            }

            /// <summary>
            /// Sets the configuration limit to disallow responses.
            /// </summary>
            public void DisallowResponse()
            {
                this.disallowResponse = true;
            }

            /// <summary>
            /// Sets the configuration limit to require minimum payload version.
            /// </summary>
            /// <param name="version">The minimum payload version required.</param>
            public void RaiseMinPayloadVersion(ODataVersion version)
            {
                if (this.minPayloadVersion < version)
                {
                    this.minPayloadVersion = version;
                }
            }

            /// <summary>
            /// Given a test configuration this returns true if the configuration should be skipped as per these limits.
            /// </summary>
            /// <param name="testConfiguration">The test configuration to check.</param>
            /// <returns>true if the configuration is not allowed by these limits (and thus should be skipped).</returns>
            public bool SkipTestConfiguration(TestConfiguration testConfiguration)
            {
                if (this.disallowRequest && testConfiguration.IsRequest)
                {
                    return true;
                }

                if (this.disallowResponse && !testConfiguration.IsRequest)
                {
                    return true;
                }

                return this.SkipVersion(testConfiguration.Version);
            }

            /// <summary>
            /// Given a payload version returns true if the version should be skipped as per these limits.
            /// </summary>
            /// <param name="version">The payload version to check.</param>
            /// <returns>true if the version if not alllowed by these limits (and thus should be skipped).</returns>
            public bool SkipVersion(ODataVersion version)
            {
                if (version < this.minPayloadVersion)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        /// <summary>
        /// Each configuration validator needs to implement this delegate.
        /// </summary>
        /// <param name="payloadElement">The payload element to check.</param>
        /// <param name="testConfigLimits">The configuration limits to modify if necessary.</param>
        public delegate void ConfigurationValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits);

        #region Validators
        /// <summary>
        /// Class which represents collection of validators to use.
        /// </summary>
        public class Validators
        {
            /// <summary>
            /// Map of the payload elemen type to the list of validators which apply to that type.
            /// </summary>
            private Dictionary<ODataPayloadElementType, List<ConfigurationValidator>> map;

            /// <summary>
            /// Constructor.
            /// </summary>
            public Validators()
            {
                this.map = new Dictionary<ODataPayloadElementType, List<ConfigurationValidator>>();
            }

            /// <summary>
            /// Copy constructor.
            /// </summary>
            /// <param name="other">The validators collection to clone.</param>
            public Validators(Validators other)
                : this()
            {
                foreach (var m in other.map)
                {
                    this.map.Add(m.Key, new List<ConfigurationValidator>(m.Value));
                }
            }

            /// <summary>
            /// Adds a validator.
            /// </summary>
            /// <param name="validator">The validator delegate to add.</param>
            /// <param name="payloadElementTypes">The payload element types to add the validator for.</param>
            public void Add(ConfigurationValidator validator, params ODataPayloadElementType[] payloadElementTypes)
            {
                foreach (var payloadElementType in payloadElementTypes)
                {
                    List<ConfigurationValidator> validators;
                    if (!this.map.TryGetValue(payloadElementType, out validators))
                    {
                        validators = new List<ConfigurationValidator>();
                        this.map[payloadElementType] = validators;
                    }

                    validators.Add(validator);
                }
            }

            /// <summary>
            /// Applies validators to the specified element.
            /// </summary>
            /// <param name="element">The payload element to apply the validators to.</param>
            /// <param name="testConfigLimits">The test configuration limits to modify.</param>
            public void Apply(ODataPayloadElement element, TestConfigurationLimits testConfigLimits)
            {
                List<ConfigurationValidator> validators;
                if (this.map.TryGetValue(element.ElementType, out validators))
                {
                    foreach (var validator in validators)
                    {
                        validator(element, testConfigLimits);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// All available validators.
        /// </summary>
        public static Validators AllValidators;

        /// <summary>
        /// Initializes static members of the ODataPayloadElementConfigurationValidator type.
        /// </summary>
        static ODataPayloadElementConfigurationValidator()
        {
            AllValidators = new Validators();
            AllValidators.Add(NonEntityCollectionValidator, ODataPayloadElementType.PrimitiveMultiValue, ODataPayloadElementType.ComplexMultiValue);
            AllValidators.Add(StreamPropertyValidator, ODataPayloadElementType.NamedStreamInstance);
            AllValidators.Add(AssociationLinkValidator, ODataPayloadElementType.NavigationPropertyInstance);
            AllValidators.Add(SpatialValueValidator, ODataPayloadElementType.PrimitiveValue);
            AllValidators.Add(ActionAndFunctionValidator, ODataPayloadElementType.EntityInstance);
        }

        #region Validator functions.
        /// <summary>
        /// Validator for collections of primitive and complex types.
        /// </summary>
        /// <param name="payloadElement">The payload element to validate.</param>
        /// <param name="testConfigLimits">The test configuration limits to modify.</param>
        public static void NonEntityCollectionValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits)
        {
            ExceptionUtilities.Assert(payloadElement is PrimitiveMultiValue || payloadElement is ComplexMultiValue, "This validator only works on collection values.");

            // Collection values require V3.
            testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
        }

        /// <summary>
        /// Validator for stream properties in requests.
        /// </summary>
        /// <param name="payloadElement">The payload element to validate.</param>
        /// <param name="testConfigLimits">The test configuration limits to modify.</param>
        public static void StreamPropertyValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits)
        {
            ExceptionUtilities.Assert(payloadElement is NamedStreamInstance, "This validator only works on stream properties.");

            // Stream properties are not allowed in requests.
            testConfigLimits.DisallowRequest();

            // Stream properties are only allowed in >=V3 requests.
            testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
        }

        /// <summary>
        /// Validator for stream properties in requests.
        /// </summary>
        /// <param name="payloadElement">The payload element to validate.</param>
        /// <param name="testConfigLimits">The test configuration limits to modify.</param>
        public static void AssociationLinkValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits)
        {
            NavigationPropertyInstance navigationProperty = payloadElement as NavigationPropertyInstance;
            ExceptionUtilities.Assert(navigationProperty != null, "This validator only works on navigation properties.");

            if (navigationProperty.AssociationLink != null)
            {
                // Association links are not allowed in requests.
                testConfigLimits.DisallowRequest();

                // Association links can only be read if MPV >= V3, they are recognized even in <V3 payloads
                // but we don't have a way to specify this here, so just mark them as >=V3 as well.
                testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
            }
        }

        /// <summary>
        /// Validator for spatial values.
        /// </summary>
        /// <param name="payloadElement">The payload element to validate.</param>
        /// <param name="testConfigLimits">The test configuration limits to modify.</param>
        public static void SpatialValueValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits)
        {
            PrimitiveValue primitiveValue = payloadElement as PrimitiveValue;
            ExceptionUtilities.Assert(primitiveValue != null, "This validator only works on primitive values.");

            if (primitiveValue.ClrValue is ISpatial)
            {
                // Spatial values require V3.
                testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
            }
            else if (primitiveValue.ClrValue == null)
            {
                EntityModelTypeAnnotation typeAnnotation = primitiveValue.GetAnnotation<EntityModelTypeAnnotation>();
                if (typeAnnotation != null && typeAnnotation.EdmModelType.Definition.IsSpatial())
                {
                    testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
                }
            }
        }

        /// <summary>
        /// Validator for actions and functions.
        /// </summary>
        /// <param name="payloadElement">The payload element to validate.</param>
        /// <param name="testConfigLimits">The test configuration limits to modify.</param>
        public static void ActionAndFunctionValidator(ODataPayloadElement payloadElement, TestConfigurationLimits testConfigLimits)
        {
            EntityInstance entityInstance = payloadElement as EntityInstance;
            ExceptionUtilities.Assert(entityInstance != null, "This validator only works on entity instances.");

            if (entityInstance.ServiceOperationDescriptors.Any())
            {
                // Actions and Functions require V3 response payloads.
                testConfigLimits.DisallowRequest();
                testConfigLimits.RaiseMinPayloadVersion(ODataVersion.V4);
            }
        }
        #endregion

        /// <summary>
        /// For a given payload element determines its test configuration limits.
        /// </summary>
        /// <param name="payloadElement">The payload element to check.</param>
        /// <param name="validators">The collection of validators to use.</param>
        /// <returns>The test configuration limits computed.</returns>
        public static TestConfigurationLimits GetTestConfigurationLimits(ODataPayloadElement payloadElement, Validators validators)
        {
            TestConfigurationLimits testConfigLimits = new TestConfigurationLimits();
            new Visitor(validators, testConfigLimits).Visit(payloadElement);
            return testConfigLimits;
        }

        /// <summary>
        /// For a given payload element returns a func which skips test configuration which are not allowed.
        /// </summary>
        /// <param name="payloadElement">The payload element to check.</param>
        /// <param name="validators">The collection of validators to use.</param>
        /// <returns>The func to use for skipping test configuration which are not allowed by that payload element.</returns>
        public static Func<TestConfiguration, bool> GetSkipTestConfiguration(ODataPayloadElement payloadElement, Validators validators)
        {
            return GetTestConfigurationLimits(payloadElement, validators).SkipTestConfiguration;
        }

        #region Visitor
        /// <summary>
        /// Private visitor to walk the payload elements and apply validators.
        /// </summary>
        private class Visitor : ODataPayloadElementVisitorBase
        {
            /// <summary>
            /// The validators collection to use.
            /// </summary>
            private Validators validators;

            /// <summary>
            /// The test configuration limits to modify.
            /// </summary>
            private TestConfigurationLimits testConfigLimits;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="validators">The validators collection to use.</param>
            /// <param name="testConfigLimits">The test configuration limits to modify.</param>
            public Visitor(Validators validators, TestConfigurationLimits testConfigLimits)
            {
                this.validators = validators;
                this.testConfigLimits = testConfigLimits;
            }

            /// <summary>
            /// Visit the specified payload element and apply validators to it.
            /// </summary>
            /// <param name="payloadElement">The payload element to inspect.</param>
            public void Visit(ODataPayloadElement payloadElement)
            {
                this.Recurse(payloadElement);
            }

            /// <summary>
            /// Called for each payload element as the visitor walks the tree.
            /// </summary>
            /// <param name="element">The payload element to visit.</param>
            protected override void Recurse(ODataPayloadElement element)
            {
                base.Recurse(element);
                this.validators.Apply(element, this.testConfigLimits);
            }
        }
        #endregion
    }
}
