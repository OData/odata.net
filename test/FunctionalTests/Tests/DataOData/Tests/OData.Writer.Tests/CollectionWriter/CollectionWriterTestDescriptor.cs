//---------------------------------------------------------------------
// <copyright file="CollectionWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Test payload and result descriptor for collection writing consisting of a collection name, a payload item
    /// and a set of invocation instructions to be issued against the collection writer.
    /// </summary>
    public sealed class CollectionWriterTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings : WriterTestDescriptor.Settings
        {
        }

        private readonly string collectionName;
        private readonly string collectionTypeName;
        private readonly object[] payloadItems;
        private readonly WriterInvocations[] invocations;
        private readonly PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback;

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, string collectionTypeName, object[] payloadItems, PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback, IEdmModel model)
            : this(settings, collectionName, payloadItems, expectedResultCallback, model)
        {
            this.collectionTypeName = collectionTypeName;
        }

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, ItemDescription[] itemDescriptions, IEdmModel model)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            this.payloadItems = itemDescriptions.Select(i => i.Item).ToArray();
            bool errorOnly;
            this.invocations = CreateInvocations(payloadItems, out errorOnly);
            this.expectedResultCallback = CollectionWriterUtils.CreateExpectedResultCallback(collectionName, itemDescriptions, errorOnly, this.TestDescriptorSettings.ExpectedResultSettings);
            this.Model = model;
        }

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, WriterInvocations[] invocations, Func<WriterTestConfiguration, ExpectedException> expectedExceptionFunc, ItemDescription collectionItem)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            ItemDescription[] itemDescriptions;
            bool errorOnly;
            this.payloadItems = CreatePayloadItems(invocations, collectionItem, null, out itemDescriptions, out errorOnly);
            this.invocations = invocations;
            if (expectedExceptionFunc == null )
            {
                // skip validation of cases that do not expect an error
                this.expectedResultCallback = testConfig => null;
            }
            else
            {
                this.expectedResultCallback =
                    testConfig => new WriterTestExpectedResults(this.TestDescriptorSettings.ExpectedResultSettings) 
                    { 
                        ExpectedException2 = expectedExceptionFunc(testConfig) 
                    };
            }
        }

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, WriterInvocations[] invocations, Func<WriterTestConfiguration, ExpectedException> expectedExceptionFunc, ItemDescription collectionItem, ItemDescription errorItem, IEdmModel model)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            ItemDescription[] itemDescriptions;
            bool errorOnly;
            this.payloadItems = CreatePayloadItems(invocations, collectionItem, errorItem, out itemDescriptions, out errorOnly);
            this.invocations = invocations;
            this.Model = model;

            if (expectedExceptionFunc == null)
            {
                this.expectedResultCallback = CollectionWriterUtils.CreateExpectedResultCallback(collectionName, itemDescriptions, errorOnly, this.TestDescriptorSettings.ExpectedResultSettings);
            }
            else
            {
                this.expectedResultCallback = CollectionWriterUtils.CreateExpectedErrorResultCallback(collectionName, expectedExceptionFunc, this.TestDescriptorSettings.ExpectedResultSettings);
            }
        }

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, object[] payloadItems, ExpectedException expectedException, IEdmModel model)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            this.payloadItems = payloadItems;
            bool errorOnly;
            this.invocations = CreateInvocations(payloadItems, out errorOnly);
            this.expectedResultCallback = CollectionWriterUtils.CreateExpectedErrorResultCallback(collectionName, tc => expectedException, this.TestDescriptorSettings.ExpectedResultSettings);
            this.Model = model;
        }

        public CollectionWriterTestDescriptor(Settings settings, string collectionName, ItemDescription[] itemDescriptions, ExpectedException expectedException, IEdmModel model)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            this.payloadItems = itemDescriptions.Select(i => i.Item).ToArray();
            bool errorOnly;
            this.invocations = CreateInvocations(payloadItems, out errorOnly);
            this.expectedResultCallback = CollectionWriterUtils.CreateExpectedErrorResultCallback(collectionName, tc => expectedException, this.TestDescriptorSettings.ExpectedResultSettings);
            this.Model = model;
        }
        
        public CollectionWriterTestDescriptor(Settings settings, string collectionName, object[] payloadItems, PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback, IEdmModel model)
        {
            this.TestDescriptorSettings = settings;
            this.collectionName = collectionName;
            this.payloadItems = payloadItems;
            bool errorOnly;
            this.invocations = CreateInvocations(payloadItems, out errorOnly);
            this.expectedResultCallback = expectedResultCallback;
            this.Model = model;
        }

        /// <summary>
        /// Gets or sets the test descripto settings.
        /// </summary>
        public Settings TestDescriptorSettings { get; private set; }

        public IEdmTypeReference ItemTypeParameter { get; set; }

        internal string CollectionName
        {
            get { return this.collectionName; }
        }

        internal string CollectionTypeName
        {
            get { return this.collectionTypeName; }
        }

        internal object[] PayloadItems
        {
            get { return this.payloadItems; }
        }

        internal WriterInvocations[] Invocations
        {
            get { return this.invocations; }
        }

        internal PayloadWriterTestDescriptor.WriterTestExpectedResultCallback ExpectedResultCallback
        {
            get { return this.expectedResultCallback; }
        }

        public IEdmModel Model { get; set; }

        private static CollectionWriterTestDescriptor.WriterInvocations[] CreateInvocations(object[] payloadItems, out bool errorOnly)
        {
            errorOnly = false;

            if (payloadItems == null)
            {
                return new CollectionWriterTestDescriptor.WriterInvocations[0];
            }

            var invocations = new List<CollectionWriterTestDescriptor.WriterInvocations>();
            invocations.Add(CollectionWriterTestDescriptor.WriterInvocations.StartCollection);
            bool addEndInvocation = true;

            int payloadItemCount = payloadItems.Length;
            if (payloadItemCount > 0)
            {
                errorOnly = true;
            }

            for (int i = 0; i < payloadItemCount; ++i)
            {
                object payloadItem = payloadItems[i];
                ODataError error = payloadItem as ODataError;
                if (error != null)
                {
                    invocations.Add(CollectionWriterTestDescriptor.WriterInvocations.Error);
                    addEndInvocation = false;
                }
                else
                {
                    errorOnly = false;
                    invocations.Add(CollectionWriterTestDescriptor.WriterInvocations.Item);
                }
            }

            if (addEndInvocation)
            {
                invocations.Add(CollectionWriterTestDescriptor.WriterInvocations.EndCollection);
            }

            return invocations.ToArray();
        }

        private static object[] CreatePayloadItems(
            CollectionWriterTestDescriptor.WriterInvocations[] invocations,
            ItemDescription payloadItem,
            ItemDescription errorItem,
            out ItemDescription[] itemDescriptions,
            out bool errorOnly)
        {
            if (invocations == null || invocations.Length == 0)
            {
                itemDescriptions = null;
                errorOnly = false;
                return null;
            }

            List<object> payloadItems = new List<object>();
            List<ItemDescription> descriptions = new List<ItemDescription>();

            bool? onlyErrorsWritten = null;
            for (int i = 0; i < invocations.Length; ++i)
            {
                switch (invocations[i])
                {
                    case CollectionWriterTestDescriptor.WriterInvocations.StartCollection:
                        break;
                    case CollectionWriterTestDescriptor.WriterInvocations.Item:
                        payloadItems.Add(payloadItem.Item);
                        descriptions.Add(payloadItem);
                        onlyErrorsWritten = false;
                        break;
                    case CollectionWriterTestDescriptor.WriterInvocations.Error:
                        payloadItems.Add(errorItem.Item);
                        descriptions.Add(errorItem);
                        if (!onlyErrorsWritten.HasValue)
                        {
                            onlyErrorsWritten = true;
                        }
                        break;
                    case CollectionWriterTestDescriptor.WriterInvocations.EndCollection:
                        break;

                    case CollectionWriterTestDescriptor.WriterInvocations.UserException:
                        break;

                    default:
                        throw new NotSupportedException("Unsupported invocation kind.");
                }
            }

            itemDescriptions = descriptions.ToArray();
            errorOnly = onlyErrorsWritten.HasValue && onlyErrorsWritten.Value;
            return payloadItems.ToArray();
        }

        public sealed class ItemDescription
        {
            internal object Item { get; set; }
            internal string ExpectedXml { get; set; }
            internal string[] ExpectedJsonLightLines { get; set; }
        }

        public enum WriterInvocations
        {
            StartCollection,
            Item,
            Error,
            EndCollection,
            UserException,
        }
    }
}
