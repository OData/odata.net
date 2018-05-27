//---------------------------------------------------------------------
// <copyright file="BatchWriterTestExpectedResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    #endregion Namespaces

    /// <summary>
    /// Expected results for writing batch payloads.
    /// </summary>
    internal sealed class BatchWriterTestExpectedResults : WriterTestExpectedResults
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public BatchWriterTestExpectedResults(Settings settings)
            : base(settings)
        {
        }

        internal BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] InvocationsAndOperationDescriptors { get; set; }
        internal Dictionary<string, string> ExpectedHeaders { get; set; }

        /// <summary>
        /// Finds the n-th change set (where n is specified in <paramref name="queryOperationIndex"/>) in the list of invocations and operations and returns the change set parts.
        /// </summary>
        /// <param name="queryOperationIndex">The index of the desired change set.</param>
        /// <returns>The set of change set operations for the requested change set.</returns>
        internal BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor[] GetChangeSet(int changeSetIndex)
        {
            // search for n-th change set start; return null if not found
            int currentChangeSetIx = -1;
            int changeSetStartIndex = -1;
            for (int i = 0; i < this.InvocationsAndOperationDescriptors.Length; ++i)
            {
                if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteStartChangeSet)
                {
                    currentChangeSetIx++;
                    if (currentChangeSetIx == changeSetIndex)
                    {
                        changeSetStartIndex = i;
                        break;
                    }
                }
            }

            if (changeSetStartIndex < 0)
            {
                return null;
            }

            // now find all the change set operations in the changeset
            var changeSetOperations = new List<BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor>();
            for (int i = changeSetStartIndex + 1; i < this.InvocationsAndOperationDescriptors.Length; ++i)
            {
                if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteEndChangeSet)
                {
                    return changeSetOperations.ToArray();
                }
                else if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteChangeSetOperation)
                {
                    changeSetOperations.Add(this.InvocationsAndOperationDescriptors[i].OperationDescriptor);
                }
                else
                {
                    throw new InvalidOperationException("Detected non-changeset operation within a changeset.");
                }
            }

            throw new InvalidOperationException("Detected changeset start without change set end.");
        }

        /// <summary>
        /// Finds the n-th query operation (where n is specified in <paramref name="queryOperationIndex"/>) in the list of invocations and operations; return it.
        /// </summary>
        /// <param name="queryOperationIndex">The index of the desired query operation.</param>
        /// <returns>The requested query operation.</returns>
        internal BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor GetQueryOperation(int queryOperationIndex)
        {
            // search for n-th query operation; return null if not found
            int currentQueryOperationIx = -1;
            BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor queryOperation = null;
            for (int i = 0; i < this.InvocationsAndOperationDescriptors.Length; ++i)
            {
                if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation)
                {
                    currentQueryOperationIx++;
                    if (currentQueryOperationIx == queryOperationIndex)
                    {
                        queryOperation = this.InvocationsAndOperationDescriptors[i].OperationDescriptor;
                        break;
                    }
                }
            }

            return queryOperation;
        }

        internal static Func<string, string> GetComparer(BatchWriterTestDescriptor.BatchWriterOperationTestDescriptor operationDescriptor, WriterTestConfiguration testConfiguration)
        {
            string expectedContent = null;

            var variables = TestWriterUtils.GetPayloadVariablesForTestConfiguration(testConfiguration);

            Func<string, string> normalizer = null;
            Func<string, string> comparer = null;

            var queryOperation = operationDescriptor as BatchWriterTestDescriptor.BatchWriterQueryOperationTestDescriptor;
            if (queryOperation != null)
            {
                // query operations don't specify content
                expectedContent = null;
            }
            else
            {
                var changeSetOperation = operationDescriptor as BatchWriterTestDescriptor.BatchWriterChangeSetOperationTestDescriptor;
                if (changeSetOperation != null)
                {
                    if (changeSetOperation.Payload != null)
                    {
                        Debug.Assert(changeSetOperation.ODataPayload == null, "changeSetOperation.ODataPayload == null");
                        expectedContent = changeSetOperation.Payload;
                        expectedContent = StringUtils.ResolveVariables(expectedContent, variables);
                    }
                    else if (changeSetOperation.ODataPayload != null)
                    {
                        Debug.Assert(changeSetOperation.Payload == null, "changeSetOperation.Payload == null");
                        if (changeSetOperation.ODataPayload.WriterTestExpectedResults != null)
                        {
                            comparer = GetComparerForWriterTestExpectedResults(changeSetOperation.ODataPayload);
                        }
                        else
                        {
                            expectedContent = changeSetOperation.ODataPayload.ExpectedResult;
                            if (changeSetOperation.ODataPayload.TestConfiguration.Format == ODataFormat.Json)
                            {
                                expectedContent = JsonUtils.GetComparableJsonString(expectedContent, variables);
                            }
                            else
                            {
                                throw new NotSupportedException("Only ATOM and JSON formats are supported for batch payloads.");
                            }
                        }
                    }
                }
                else
                {
                    var responseOperation = operationDescriptor as BatchWriterTestDescriptor.BatchWriterResponseOperationTestDescriptor;
                    if (responseOperation != null)
                    {
                        if (responseOperation.Payload != null)
                        {
                            Debug.Assert(responseOperation.ODataPayload == null, "responsePart.ODataPayload == null");
                            expectedContent = responseOperation.Payload;
                            expectedContent = StringUtils.ResolveVariables(expectedContent, variables);
                        }
                        else if (responseOperation.ODataPayload != null)
                        {
                            Debug.Assert(responseOperation.Payload == null, "responsePart.Payload == null");
                            if (responseOperation.ODataPayload.WriterTestExpectedResults != null)
                            {
                                comparer = GetComparerForWriterTestExpectedResults(responseOperation.ODataPayload);
                            }
                            else
                            {
                                expectedContent = responseOperation.ODataPayload.ExpectedResult;
                                if (responseOperation.ODataPayload.TestConfiguration.Format == ODataFormat.Json)
                                {
                                    expectedContent = JsonUtils.GetComparableJsonString(expectedContent, variables);
                                }
                                else
                                {
                                    throw new NotSupportedException("Only ATOM and JSON formats are supported for batch payloads.");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Found unsupported operation descriptor of type " + operationDescriptor.GetType().FullName + ".");
                    }
                }
            }

            return (observedContent) =>
                {
                    if (comparer != null)
                    {
                        return comparer(observedContent);
                    }

                    if (normalizer != null)
                    {
                        observedContent = normalizer(observedContent);
                    }

                    if (string.CompareOrdinal(expectedContent, observedContent) != 0)
                    {
                        return string.Format(
                            "Different operation content.{0}Expected:{0}-->{1}<--{0}Actual:{0}-->{2}<--{0}",
                            Environment.NewLine,
                            expectedContent,
                            observedContent);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Computes the number of top-level change sets and query operations in the batch.
        /// </summary>
        /// <returns>The number of top-level change sets and query operations in the batch.</returns>
        internal int GetOperationCount()
        {
            int count = 0;

            bool inChangeSet = false;
            for (int i = 0; i < this.InvocationsAndOperationDescriptors.Length; ++i)
            {
                if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteStartChangeSet)
                {
                    inChangeSet = true;
                    count++;
                }
                else if (this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteEndChangeSet)
                {
                    inChangeSet = false;
                }
                else if (!inChangeSet && this.InvocationsAndOperationDescriptors[i].Invocation == BatchWriterTestDescriptor.WriterInvocations.WriteQueryOperation)
                {
                    count++;
                }
            }

            return count;
        }

        private static Func<string, string> GetComparerForWriterTestExpectedResults(BatchWriterUtils.ODataPayload odataPayload)
        {
            if (odataPayload.TestConfiguration.Format == ODataFormat.Json)
            {
                return (observed) =>
                {
                    string error;
                    if (!TestWriterUtils.CompareJsonResults(
                        ((JsonWriterTestExpectedResults)odataPayload.WriterTestExpectedResults),
                        observed,
                        odataPayload.TestConfiguration,
                        out error))
                    {
                        return error;
                    }

                    return null;
                };
            }

            throw new NotSupportedException("Only ATOM and JSON formats are supported for batch payloads.");
        }
    }
}
