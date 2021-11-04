//---------------------------------------------------------------------
// <copyright file="DataServiceContextVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Verifier for the DataServiceContext.
    /// </summary>
    [ImplementationName(typeof(IDataServiceContextVerifier), "Default")]
    public class DataServiceContextVerifier : IDataServiceContextVerifier
    {
        /// <summary>
        /// Verifies the data service context.
        /// </summary>
        /// <param name="contextData">The expected data for the context.</param>
        /// <param name="context">The context to verify.</param>
        public void VerifyDataServiceContext(DataServiceContextData contextData, DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckArgumentNotNull(context, "context");

            var verifier = new Verifier(contextData, context);

            verifier.Verify();
        }

        /// <summary>
        /// Verifier for the DataServiceContext.
        /// </summary>
        private class Verifier
        {
            private readonly DataServiceContextData contextData;
            private readonly DataServiceContext context;

            /// <summary>
            /// Initializes a new instance of the <see cref="Verifier"/> class.
            /// </summary>
            /// <param name="contextData">The context data.</param>
            /// <param name="context">The context.</param>
            public Verifier(DataServiceContextData contextData, DataServiceContext context)
            {
                this.contextData = contextData;
                this.context = context;
            }

            /// <summary>
            /// Verifies DataServiceContext against expected data.
            /// </summary>
            public void Verify()
            {
                VerifyExpectedVsActual(this.contextData.ResolveEntitySet, this.context.ResolveEntitySet, "Context entity set resolver did not match");
                VerifyExpectedVsActual(this.contextData.MaxProtocolVersion, this.context.MaxProtocolVersion.ToTestEnum(), "Context max protocol version did not match");
                VerifyExpectedVsActual(this.contextData.AddAndUpdateResponsePreference.ToProductEnum(), this.context.AddAndUpdateResponsePreference, "Context response preference did not match");
                VerifyExpectedVsActual(this.contextData.BaseUri, this.context.BaseUri, "Context base uri did not match");
                VerifyExpectedVsActual(this.contextData.ResolveType, this.context.ResolveType, "Context type resolver did not match");
                VerifyExpectedVsActual(this.contextData.ResolveName, this.context.ResolveName, "Context type name resolver did not match");
                VerifyExpectedVsActual(this.contextData.MergeOption.ToProductEnum(), this.context.MergeOption, "Context merge option did not match");
                VerifyExpectedVsActual(this.contextData.UsePostTunneling, this.context.UsePostTunneling, "Context post-tunneling setting did not match");

                var verifiedEntityDescriptors = new List<EntityDescriptor>();

                foreach (var expectedEntityEntry in this.contextData.EntityDescriptorsData)
                {
                    var descriptor = this.GetEntityDescriptor(expectedEntityEntry);

                    this.VerifyEntityDescriptor(expectedEntityEntry, descriptor);

                    verifiedEntityDescriptors.Add(descriptor);
                }

                if (verifiedEntityDescriptors.Count != this.context.Entities.Count)
                {
                    var extraEntities = this.context.Entities.Where(e => !verifiedEntityDescriptors.Contains(e)).Select(e => e.ToTraceString());

                    throw new AssertionFailedException("Unexpected entity descriptors:\r\n" + string.Join("\r\n", extraEntities.ToArray()));
                }

                var verifiedLinkDescriptors = new List<LinkDescriptor>();

                foreach (var expectedLinkEntry in this.contextData.LinkDescriptorsData)
                {
                    var linkDescriptor = this.GetLinkDescriptor(expectedLinkEntry);

                    VerifyState(expectedLinkEntry, linkDescriptor);

                    verifiedLinkDescriptors.Add(linkDescriptor);
                }

                if (verifiedLinkDescriptors.Count != this.context.Links.Count)
                {
                    var extraLinks = this.context.Links.Where(e => !verifiedLinkDescriptors.Contains(e)).Select(this.ToTraceString);

                    throw new AssertionFailedException("Unexpected link descriptors:\r\n" + string.Join("\r\n", extraLinks.ToArray()));
                }
            }

            private static void VerifyState(DescriptorData expectedEntry, Descriptor actualDescriptor)
            {
                VerifyExpectedVsActual(expectedEntry.State, actualDescriptor.State.ToTestEnum(), expectedEntry, "State");
                VerifyExpectedVsActual(expectedEntry.ChangeOrder, actualDescriptor.GetChangeOrder(), expectedEntry, "ChangeOrder");
            }

            private static void VerifyETag(string expectedETag, string actualETag, EntityDescriptorData entityEntry)
            {
                VerifyExpectedVsActual(expectedETag, actualETag, entityEntry, "ETag");
            }

            private static void VerifyExpectedVsActual<TValue>(TValue expected, TValue actual, string message)
            {
                if (!ValueComparer.Instance.Equals(expected, actual))
                {
                    throw new DataComparisonException(message) { ExpectedValue = expected, ActualValue = actual };
                }
            }

            private static void VerifyExpectedVsActual<TValue>(TValue expected, TValue actual, DescriptorData entry, string miscompareIdentity)
            {
                bool comparisonFailed = false;

                var expectedUri = expected as Uri;
                var actualUri = actual as Uri;
                if (expectedUri != null && actualUri != null && !expectedUri.IsAbsoluteUri)
                {
                    // TODO: if/when the product adds a way to get the xml:base from the <feed> element, we will have absolute URIs and can remove this
                    comparisonFailed = !actualUri.OriginalString.EndsWith(expectedUri.OriginalString, StringComparison.Ordinal);
                }
                else
                {
                    comparisonFailed = !ValueComparer.Instance.Equals(expected, actual);
                }

                if (comparisonFailed)
                {
                    throw new DataComparisonException(
                        string.Format(CultureInfo.InvariantCulture, "{0} verification failed for the descriptor data: {1}.", miscompareIdentity, entry))
                        {
                            ExpectedValue = GetValueForDataComparisonException(expected),
                            ActualValue = GetValueForDataComparisonException(actual)
                        };
                }
            }

            private static object GetValueForDataComparisonException(object value)
            {
                EntityDescriptor entityDescriptor = value as EntityDescriptor;
                if (entityDescriptor != null)
                {
                    return entityDescriptor.ToTraceString();
                }

                return value;
            }

            [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OperationDescriptors", Justification = "Spelling is correct.")]
            [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsAction", Justification = "Spelling is correct.")]
            private void VerifyEntityDescriptor(EntityDescriptorData expectedDescriptorData, EntityDescriptor actualDescriptor)
            {
                VerifyState(expectedDescriptorData, actualDescriptor);

                VerifyETag(expectedDescriptorData.ETag, actualDescriptor.ETag, expectedDescriptorData);

                VerifyExpectedVsActual(expectedDescriptorData.ServerTypeName, actualDescriptor.ServerTypeName, expectedDescriptorData, "ServerTypeName");

                VerifyExpectedVsActual(expectedDescriptorData.Identity, actualDescriptor.Identity, expectedDescriptorData, "Identity");

                VerifyExpectedVsActual(expectedDescriptorData.EditLink, actualDescriptor.EditLink, expectedDescriptorData, "EditLink");

                EntityDescriptor expectedParentForInsert = null;

                if (expectedDescriptorData.ParentForInsert != null)
                {
                    EntityDescriptorData parentForInsertDescriptorData;
                    if (!this.contextData.TryGetEntityDescriptorData(expectedDescriptorData.ParentForInsert, out parentForInsertDescriptorData))
                    {
                        throw new TaupoInvalidOperationException(
                            string.Format(
                            CultureInfo.InvariantCulture,
                            "Expected context data should have entity descriptor for the parent for insert {0}. Descriptor data = {1}.",
                            expectedDescriptorData.ParentForInsert,
                            expectedDescriptorData));
                    }

                    expectedParentForInsert = this.GetEntityDescriptor(parentForInsertDescriptorData);
                }

                VerifyExpectedVsActual(expectedParentForInsert, actualDescriptor.ParentForInsert, expectedDescriptorData, "ParentForInsert");

                VerifyExpectedVsActual(expectedDescriptorData.ParentPropertyForInsert, actualDescriptor.ParentPropertyForInsert, expectedDescriptorData, "ParentPropertyForInsert");

                if (actualDescriptor.ParentForInsert == null)
                {
                    ExceptionUtilities.Assert(actualDescriptor.ParentPropertyForInsert == null, "ParentPropertyForInsert must be null when ParentForInsert is null.");
                }
                else
                {
                    ExceptionUtilities.Assert(!string.IsNullOrEmpty(actualDescriptor.ParentPropertyForInsert), "ParentPropertyForInsert cannot be null or empty when ParentForInsert is not null.");
                }

                VerifyExpectedVsActual(expectedDescriptorData.EditLink, actualDescriptor.EditLink, expectedDescriptorData, "EditLink");
                VerifyExpectedVsActual(expectedDescriptorData.SelfLink, actualDescriptor.SelfLink, expectedDescriptorData, "SelfLink");

                VerifyExpectedVsActual(expectedDescriptorData.EditStreamUri, actualDescriptor.EditStreamUri, expectedDescriptorData, "EditStreamUri");
                VerifyExpectedVsActual(expectedDescriptorData.ReadStreamUri, actualDescriptor.ReadStreamUri, expectedDescriptorData, "ReadStreamUri");
                VerifyExpectedVsActual(expectedDescriptorData.StreamETag, actualDescriptor.StreamETag, expectedDescriptorData, "StreamETag");

                // Check that the internal StreamDescriptor for the default stream is in the correct state, if we can do the reflection safely.
                // Note that if the descriptor is not an MLE, we don't check whether the internal value is null, since that is very much an implementation detail
                if (expectedDescriptorData.IsMediaLinkEntry && AppDomain.CurrentDomain.IsFullyTrusted)
                 {
                    var defaultStreamDescriptorProperty = typeof(EntityDescriptor).GetProperty("DefaultStreamDescriptor", BindingFlags.Instance | BindingFlags.NonPublic);
                    ExceptionUtilities.CheckObjectNotNull(defaultStreamDescriptorProperty, "Could not find property 'DefaultStreamDescriptor' on EntityDescriptor");

                    var defaultStreamDescriptor = (StreamDescriptor)defaultStreamDescriptorProperty.GetValue(actualDescriptor, null);
                    ExceptionUtilities.CheckObjectNotNull(defaultStreamDescriptor, "Product's default stream descriptor was null for entity descriptor: {0}", expectedDescriptorData);

                    var defaultStreamState = defaultStreamDescriptor.State;
                    VerifyExpectedVsActual(expectedDescriptorData.DefaultStreamState, defaultStreamState.ToTestEnum(), expectedDescriptorData, "DefaultStreamDesciptor.State");
                }

                VerifyExpectedVsActual(expectedDescriptorData.LinkInfos.Count, actualDescriptor.LinkInfos.Count, expectedDescriptorData, "LinkInfos.Count");

                for (int i = 0; i < expectedDescriptorData.LinkInfos.Count; i++)
                {
                    var expectedLinkInfo = expectedDescriptorData.LinkInfos.ElementAt(i);
                    var actualLinkInfo = actualDescriptor.LinkInfos.ElementAt(i);

                    VerifyExpectedVsActual(expectedLinkInfo.Name, actualLinkInfo.Name, expectedDescriptorData, "LinkInfos[" + i + "].Name");
                    VerifyExpectedVsActual(expectedLinkInfo.NavigationLink, actualLinkInfo.NavigationLink, expectedDescriptorData, "LinkInfos[" + i + "].NavigationLink");
                    VerifyExpectedVsActual(expectedLinkInfo.RelationshipLink, actualLinkInfo.AssociationLink, expectedDescriptorData, "LinkInfos[" + i + "].AssociationLink");
                }

                VerifyExpectedVsActual(expectedDescriptorData.OperationDescriptors.Count, actualDescriptor.OperationDescriptors.Count, expectedDescriptorData, "OperationDescriptors.Count");
                for (int i = 0; i < expectedDescriptorData.OperationDescriptors.Count; i++)
                {
                    var expectedOperationDescriptor = expectedDescriptorData.OperationDescriptors.ElementAt(i);
                    var actualOperationDescriptor = actualDescriptor.OperationDescriptors.ElementAt(i);

                    VerifyExpectedVsActual(expectedOperationDescriptor.Metadata, actualOperationDescriptor.Metadata, expectedDescriptorData, "OperationDescriptors[" + i + "].Metadata");
                    VerifyExpectedVsActual(expectedOperationDescriptor.Target, actualOperationDescriptor.Target, expectedDescriptorData, "OperationDescriptors[" + i + "].Target");
                    VerifyExpectedVsActual(expectedOperationDescriptor.Title, actualOperationDescriptor.Title, expectedDescriptorData, "OperationDescriptors[" + i + "].Title");
                    VerifyExpectedVsActual(expectedOperationDescriptor.IsAction, actualOperationDescriptor is ActionDescriptor, "OperationDescriptors[" + i + "].IsAction");
                }

                VerifyExpectedVsActual(expectedDescriptorData.StreamDescriptors.Count, actualDescriptor.StreamDescriptors.Count, expectedDescriptorData, "StreamDescriptors.Count");
                for (int i = 0; i < expectedDescriptorData.StreamDescriptors.Count; i++)
                {
                    var expectedStreamDescriptor = expectedDescriptorData.StreamDescriptors.ElementAt(i);
                    var actualStreamDescriptor = actualDescriptor.StreamDescriptors.ElementAt(i);

                    VerifyExpectedVsActual(expectedStreamDescriptor.Name, actualStreamDescriptor.StreamLink.Name, expectedDescriptorData, "StreamDescriptors[" + i + "].Name");
                    VerifyExpectedVsActual(expectedStreamDescriptor.ContentType, actualStreamDescriptor.StreamLink.ContentType, expectedDescriptorData, "StreamDescriptors[" + i + "].ContentType");
                    VerifyExpectedVsActual(expectedStreamDescriptor.EditLink, actualStreamDescriptor.StreamLink.EditLink, expectedDescriptorData, "StreamDescriptors[" + i + "].EditLink");
                    VerifyExpectedVsActual(expectedStreamDescriptor.ETag, actualStreamDescriptor.StreamLink.ETag, expectedDescriptorData, "StreamDescriptors[" + i + "].ETag");
                    VerifyExpectedVsActual(expectedStreamDescriptor.SelfLink, actualStreamDescriptor.StreamLink.SelfLink, expectedDescriptorData, "StreamDescriptors[" + i + "].SelfLink");

                    ExceptionUtilities.Assert(object.ReferenceEquals(expectedDescriptorData, expectedStreamDescriptor.EntityDescriptor), "Stream descriptor data had unexpected entity descriptor data");
                    VerifyExpectedVsActual(actualDescriptor, actualStreamDescriptor.EntityDescriptor, expectedDescriptorData, "StreamDescriptors[" + i + "].EntityDescriptor");
                }
            }

            private EntityDescriptor GetEntityDescriptor(EntityDescriptorData entityDescriptorData)
            {
                EntityDescriptor descriptor = null;

                if (entityDescriptorData.Entity != null)
                {
                    descriptor = this.context.GetEntityDescriptor(entityDescriptorData.Entity);
                }
                else
                {
                    ExceptionUtilities.Assert(entityDescriptorData.Identity == null, "Entity descriptor's Identity cannnot be null when entity is null.");

                    descriptor = this.FindEntityDescriptorWithIdentity(entityDescriptorData.Identity);
                }

                if (descriptor == null)
                {
                    throw new AssertionFailedException(
                        string.Format(CultureInfo.InvariantCulture, "Entity descriptor is missing from the data service context. Expected entity descriptor: {{{0}}}.", entityDescriptorData));
                }

                return descriptor;
            }

            private EntityDescriptor FindEntityDescriptorWithIdentity(Uri identity)
            {
                return this.context.Entities.Where(e => e.Identity == identity).SingleOrDefault();
            }

            private LinkDescriptor GetLinkDescriptor(LinkDescriptorData linkDescriptorData)
            {
                object sourceEntity = this.GetEntityDescriptor(linkDescriptorData.SourceDescriptor).Entity;

                object targetEntity = linkDescriptorData.TargetDescriptor != null ?
                    this.GetEntityDescriptor(linkDescriptorData.TargetDescriptor).Entity : null;

                // Note: GetLinkDescriptor throws ArgumenNullException when target is null even so there is an existing link with null target.
                var descriptor = targetEntity != null ?
                        this.context.GetLinkDescriptor(sourceEntity, linkDescriptorData.SourcePropertyName, targetEntity)
                        : this.context.Links.Where(l => l.Source == sourceEntity && l.SourceProperty == linkDescriptorData.SourcePropertyName && l.Target == null).SingleOrDefault();

                if (descriptor == null)
                {
                    throw new AssertionFailedException(
                        string.Format(CultureInfo.InvariantCulture, "Link descriptor is missing from the data service context. Expected link descriptor data: {{{0}}}.", linkDescriptorData));
                }

                return descriptor;
            }

            private string ToTraceString(LinkDescriptor descriptor)
            {
                ExceptionUtilities.Assert(descriptor != null, "descriptor cannot be null.");

                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{{ State = {0}, Source = {{ Identity = {1} }}, SourceProperty = '{2}', Target = {{ {3} }} }}",
                    descriptor.State,
                    this.context.GetEntityDescriptor(descriptor.Source).Identity,
                    descriptor.SourceProperty,
                    descriptor.Target == null ? "null" : "Identity = " + this.context.GetEntityDescriptor(descriptor.Target).Identity);
            }
        }
    }
}
