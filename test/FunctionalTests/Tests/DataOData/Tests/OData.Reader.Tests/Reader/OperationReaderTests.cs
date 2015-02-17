//---------------------------------------------------------------------
// <copyright file="OperationReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of m:action and m:function elements.
    /// </summary>
    [TestClass, TestCase]
    public class OperationReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Test the reading of m:action and m:function elements.")]
        public void ReadActionsAndFunctionsTest()
        {   
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<ServiceOperationDescriptor> operations = new ServiceOperationDescriptor[]
            {
                new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#operation", Target = "http://odata.org/Target" },
                new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#operation", Title = "Title", Target = "http://odata.org/Target2" },
            };

            operations = operations.Concat(operations.Select(o => (ServiceOperationDescriptor)new ServiceOperationDescriptor { IsFunction = true, Metadata = o.Metadata, Title = o.Title, Target = o.Target }));

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = operations.Select( o => 
                {
                    EntityInstance entityInstance = PayloadBuilder.Entity("TestModel.CityType");
                    entityInstance.Add(o);
                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = entityInstance,
                        PayloadEdmModel = model
                    };
                });

            // Add couple more handcrafted cases
            testDescriptors.Concat(new PayloadReaderTestDescriptor[]
            {
                // Duplicate action - different targets
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#action", Target = "http://odata.org/service/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#action", Target = "http://odata.org/service/action2" }),
                    PayloadEdmModel = model
                },
                // Duplicate action - same targets
                // same targets in a metadata - allowed
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#action", Target = "http://odata.org/service/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#action", Target = "http://odata.org/service/action" }),
                    PayloadEdmModel = model
                },
                // Duplicate function - different targets and titles
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/service/$metadata#function", Target = "http://odata.org/service/function", Title = "Function 1" })
                        .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/service/$metadata#function", Target = "http://odata.org/service/function2", Title = "Function 2" }),
                    PayloadEdmModel = model
                },
                // Duplicate function - same targets and titles
                // same targets in a metadata - allowed
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/service/$metadata#function", Target = "http://odata.org/service/function", Title = "Function 1" })
                        .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/service/$metadata#function", Target = "http://odata.org/service/function", Title = "Function 1" }),
                    PayloadEdmModel = model
                },
                // Function and Action with the same name (ODL doesn't validate this case)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/service/$metadata#operation", Target = "http://odata.org/service/action", Title = "Action" })
                        .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/service/$metadata#operation", Target = "http://odata.org/service/function", Title = "Function" }),
                    PayloadEdmModel = model
                },
            });

            // Generate interesting payloads for the EntityInstance with actions/functions
            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                ODataVersionUtils.AllSupportedVersions,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => tc.Format == ODataFormat.Atom),
                (testDescriptor, maxProtocolVersion, testConfiguration) =>
                {
                    if (maxProtocolVersion < testConfiguration.Version)
                    {
                        // This would fail since the DSV > MPV.
                        return;
                    }

                    // In requests or if MPV < V3 we don't expect the operations to be read.
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveOperationsNormalizer.Normalize);
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyMaxProtocolVersion(maxProtocolVersion));
                });
        }
    }
}