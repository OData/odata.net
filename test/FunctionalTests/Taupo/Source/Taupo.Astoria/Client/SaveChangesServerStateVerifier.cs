//---------------------------------------------------------------------
// <copyright file="SaveChangesServerStateVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Default implementation of the save-changes server state verifier
    /// </summary>
    [ImplementationName(typeof(ISaveChangesServerStateVerifier), "Default")]
    public class SaveChangesServerStateVerifier : ISaveChangesServerStateVerifier
    {
        private IDictionary<DataServiceContextData, IEnumerable<ChangeData>> expectedChangeCache;
        private DataServiceContextData currentContextData;
        
        /// <summary>
        /// Initializes a new instance of the SaveChangesServerStateVerifier class.
        /// </summary>
        public SaveChangesServerStateVerifier()
        {
            this.expectedChangeCache = new Dictionary<DataServiceContextData, IEnumerable<ChangeData>>(ReferenceEqualityComparer.Create<DataServiceContextData>());
            this.VerifyData = true;
            this.Asynchronous = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the verifier validates any changes to the data made by the client library
        /// </summary>
        [InjectTestParameter("VerifyData", DefaultValueDescription = "True", HelpText = "Validate data after insert/update operations")]
        public bool VerifyData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the verifier should re-query asynchronously
        /// </summary>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "False")]
        public bool Asynchronous { get; set; }

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceContextCreator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextCreator DataServiceContextCreator { get; set; }

        /// <summary>
        /// Gets or sets the object services.
        /// </summary>
        /// <value>The object services.</value>
        [InjectDependency(IsRequired = true)]
        public IEntityModelObjectServices ObjectServices { get; set; }

        /// <summary>
        /// Gets or sets the entity model schema.
        /// </summary>
        /// <value>The entity model schema.</value>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema ModelSchema { get; set; }

        /// <summary>
        /// Gets or sets the XML primitive converter.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlPrimitiveConverter XmlPrimitiveConverter { get; set; }

        /// <summary>
        /// Initializes the expected changes to verify after SaveChanges completes
        /// </summary>
        /// <param name="contextData">The context data</param>
        /// <param name="propertyValues">The property values of the tracked client objects</param>
        public void InitializeExpectedChanges(DataServiceContextData contextData, IDictionary<object, IEnumerable<NamedValue>> propertyValues)
        {
            this.expectedChangeCache[contextData] = contextData.GetOrderedChanges().Where(d => !(d is StreamDescriptorData)).Select(d => this.CreateChangeData(d, propertyValues)).ToList();
        }

        /// <summary>
        /// Verifies that the values on the server are correct
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        /// <param name="contextData">The context data</param>
        public void VerifyChangesOnServer(IAsyncContinuation continuation, DataServiceContextData contextData)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(contextData, "contextData");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.currentContextData = contextData;

            IEnumerable<ChangeData> expectedChanges;
            ExceptionUtilities.Assert(this.expectedChangeCache.TryGetValue(contextData, out expectedChanges), "Expected changes for given context have not been initialized");
            this.expectedChangeCache.Remove(contextData);

            AsyncHelpers.AsyncForEach(expectedChanges, continuation, this.VerifyChangeOnServer);
        }

        private ChangeData CreateChangeData(DescriptorData descriptorData, IDictionary<object, IEnumerable<NamedValue>> propertyValuesBeforeSave)
        {
            var entityDescriptorData = descriptorData as EntityDescriptorData;
            if (entityDescriptorData != null)
            {
                IEnumerable<NamedValue> propertyValues;
                ExceptionUtilities.Assert(propertyValuesBeforeSave.TryGetValue(entityDescriptorData.Entity, out propertyValues), "Could not find property values for descriptor: {0}", entityDescriptorData);

                return EntityChangeData.Create(entityDescriptorData, propertyValues);
            }

            var linkDescriptorData = descriptorData as LinkDescriptorData;
            ExceptionUtilities.CheckObjectNotNull(linkDescriptorData, "Descriptor was neither an entity nor a link");

            return LinkChangeData.Create(linkDescriptorData);
        }

        private void VerifyChangeOnServer(ChangeData change, IAsyncContinuation continuation)
        {
            //// For now use different data service context to re-query and verify entities and links.
            //// In the future consider using an 'Oracle' component to re-query the changes if/when it comes online.
            using (IWrapperScope scope = new NullWrapperScope())
            {
                WrappedDataServiceContext otherContext = this.DataServiceContextCreator.CreateContext(scope, this.currentContextData.ContextType, this.currentContextData.BaseUri);
                otherContext.ResolveName = this.currentContextData.ResolveName;
                otherContext.ResolveType = this.currentContextData.ResolveType;
                otherContext.IgnoreResourceNotFoundException = true;

                var uri = change.GetUriForRequery();
                otherContext.Execute<WrappedObject>(
                    continuation, 
                    this.Asynchronous,
                    change.ClrTypeForRequery,
                    uri,
                    results =>
                    {
                        var result = results.ToList();
                        VerifyChangeOnServer(change, otherContext, result);
                        continuation.Continue();
                    });
            }
        }

        private void VerifyChangeOnServer(ChangeData change, WrappedDataServiceContext wrappedContext, List<WrappedObject> result)
        {
            var linkChange = change as LinkChangeData;
            int expectedCount = change.State == EntityStates.Deleted ? 0 : 1;
            if (linkChange != null && linkChange.DescriptorData.TargetDescriptor == null)
            {
                expectedCount = 0;
            }

            this.Assert.AreEqual(expectedCount, result.Count, "Verify only one entity is returned for the uri representing link or descriptor.");

            if (expectedCount == 1)
            {
                var entity = result.Single();

                this.Assert.AreEqual((object)change.ClrTypeForRequery, entity.Product.GetType(), "Verify the CLR type for the query");
                if (linkChange != null)
                {
                    var targetEntity = linkChange.DescriptorData.TargetDescriptor.Entity;
                    ExceptionUtilities.CheckObjectNotNull(targetEntity, "Target was unexpectedly null for link change: {0}", linkChange.ToString());
                    
                    var targetType = this.GetEntityType(targetEntity.GetType());
                    var possibleSets = this.ModelSchema.EntityContainers.SelectMany(c => c.EntitySets).Where(s => targetType.IsKindOf(s.EntityType)).ToList();
                    if (!possibleSets.Any(s => s.Annotations.OfType<EntryIdReplacementAnnotation>().Any(a => a.AppendRequestIdToName)))
                    {
                        var expectedIdentity = linkChange.DescriptorData.TargetDescriptor.Identity;
                        var actualIdentity = wrappedContext.GetEntityDescriptor(entity).Identity;

                        this.Assert.AreEqual(expectedIdentity, actualIdentity, ValueComparer.Instance, "Entity ID did not match for link change: {0}", linkChange.ToString());
                    }
                }
                else if (this.VerifyData)
                {
                    this.VerifyPropertiesValues((EntityChangeData)change, entity.Product);
                }
            }
        }

        private void VerifyPropertiesValues(EntityChangeData entityChange, object entity)
        {
            var entityType = this.GetEntityType(entity.GetType());
            var serverNamedValues = this.ObjectServices.GetPropertiesValues(entity, entityType);

            foreach (NamedValue serverNamedValue in serverNamedValues)
            {
                object cachedClientValue = entityChange.CachedPropertiesValues[serverNamedValue.Name];

                // check if it's store-generated property
                MemberProperty property = this.GetPropertyWithPath(serverNamedValue.Name, entityType);
                StoreGeneratedPatternAnnotation storeGenAnnotation = property.Annotations.OfType<StoreGeneratedPatternAnnotation>().SingleOrDefault();

                // The client does not perform fix-up on dependent properties, so we skip those properties here. The verification in the protocol tests
                // is sufficient for ensuring the server behavior is correct
                if (entityType.GetDependentProperties().Contains(property))
                {
                    continue;
                }

                bool isMediaLinkEntryInsert = entityType.HasStream() && entityChange.State == EntityStates.Added;
                if (storeGenAnnotation != null)
                {
                    bool clientShouldBeUpdated = !isMediaLinkEntryInsert;
                    if (isMediaLinkEntryInsert || (entityChange.State == EntityStates.Modified && storeGenAnnotation.ServerGeneratedOnUpdate))
                    {
                        // if the product ever supports materializing the property values generated by the server
                        // during an MLE insert, then this can be changed to expect the server values on DSRP.None as well
                        clientShouldBeUpdated = this.currentContextData.AddAndUpdateResponsePreference == DataServiceResponsePreference.IncludeContent;
                    }
                    else if (!isMediaLinkEntryInsert && entityChange.State == EntityStates.Added && storeGenAnnotation.ServerGeneratedOnInsert)
                    {
                        clientShouldBeUpdated = this.currentContextData.AddAndUpdateResponsePreference != DataServiceResponsePreference.NoContent;
                    }

                    var clientValue = this.ObjectServices.GetObjectAdapter(entityType.FullName).GetMemberValue<object>(entityChange.DescriptorData.Entity, serverNamedValue.Name);
                    if (clientShouldBeUpdated)
                    {
                        this.Assert.AreEqual(
                            serverNamedValue.Value,
                            clientValue,
                            ValueComparer.Instance,
                            "Server-generated value should have been propagated back to the client. {0}.{1}",
                            entityChange.DescriptorData.Identity,
                            serverNamedValue.Name);
                    }
                    else
                    {
                           this.Assert.AreEqual(
                            cachedClientValue,
                            clientValue,
                            ValueComparer.Instance,
                            "Server-generated value should NOT have been propagated back to the client. {0}.{1}",
                            entityChange.DescriptorData.Identity,
                            serverNamedValue.Name);
                    }
                }
                else
                {
                    // The value provided on the client should have been saved to the store.
                    this.VerifyPropertyValue(
                        cachedClientValue,
                        serverNamedValue.Value,
                        serverNamedValue.Name,
                        entityChange.DescriptorData.Identity.OriginalString);
                }
            }
        }

        private void VerifyPropertyValue(object expected, object actual, string propertyName, string entityIdentity)
        {
            var expectedAsString = expected as string;
            var expectedAsByteArray = expected as byte[];

            if (expectedAsString != null)
            {
                this.VerifyStringValue(expectedAsString, (string)actual, propertyName, entityIdentity);
            }
            else if (expectedAsByteArray != null)
            {
                this.VerifyBinaryValue(expectedAsByteArray, (byte[])actual, propertyName, entityIdentity);
            }
            else
            {
                this.Assert.AreEqual(
                    expected,
                    actual,
                    ValueComparer.Instance,
                    "Verification failed for the property: '{0}', entity: '{1}'",
                    propertyName,
                    entityIdentity);
            }
        }

        private void VerifyBinaryValue(byte[] expected, byte[] actual, string propertyName, string entityIdentity)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            this.Assert.IsNotNull(actual, string.Format(CultureInfo.InvariantCulture, "Byte array value unexpectedly null for property: '{0}', entity: '{1}'", propertyName, entityIdentity));

            this.Assert.IsTrue(
                expected.Length <= actual.Length,
                string.Format(CultureInfo.InvariantCulture, "Verify length of byte array value for property: '{0}', entity: '{1}'\r\nExpected length {2} should be less than actual lenght {3}.", propertyName, entityIdentity, expected.Length, actual.Length));

            int i = 0;
            for (; i < expected.Length; i++)
            {
                this.Assert.AreEqual(expected[i], actual[i], string.Format(CultureInfo.InvariantCulture, "Verify {0} element in byte array value for property: '{1}', entity: '{2}'.", i, propertyName, entityIdentity));
            }

            // The actual value comes from the store and can be padded.
            while (i < actual.Length)
            {
                this.Assert.AreEqual((byte)0, actual[i], string.Format(CultureInfo.InvariantCulture, "Verify {0} element in byte array value for property: '{1}', entity: '{2}'.", i, propertyName, entityIdentity));
                i++;
            }
        }

        private void VerifyStringValue(string expected, string actual, string propertyName, string entityIdentity)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            this.Assert.IsNotNull(actual, string.Format(CultureInfo.InvariantCulture, "String value unexpectedly null for property: '{0}', entity: '{1}'", propertyName, entityIdentity));

            // The actual value comes from the store and can be padded.
            this.Assert.IsTrue(expected.Length <= actual.Length, string.Format(CultureInfo.InvariantCulture, "Verify length of stirng value for property: '{0}', entity: '{1}'", propertyName, entityIdentity));

            this.Assert.AreEqual(expected, actual.Substring(0, expected.Length), string.Format(CultureInfo.InvariantCulture, "Verify string value for property: '{0}', entity: '{1}'", propertyName, entityIdentity));

            if (actual.Length > expected.Length)
            {
                string trimmed = actual.Substring(expected.Length).TrimEnd(' ');
                this.Assert.AreEqual(string.Empty, trimmed, string.Format(CultureInfo.InvariantCulture, "Verify value from store is padded with spaces for property: '{0}', entity: '{1}'", propertyName, entityIdentity));
            }
        }

        private EntityType GetEntityType(Type clrType)
        {
            var found = this.ModelSchema.EntityTypes.Where(t => t.FullName == clrType.FullName).ToList();

            this.Assert.AreEqual(1, found.Count, "Unexpected number of matches found for the entity type: " + clrType.FullName);

            return found.First();
        }

        private MemberProperty GetPropertyWithPath(string propertyPath, EntityType entityType)
        {
            string[] propertyNames = propertyPath.Split('.');
            var currentProperties = entityType.AllProperties;
            MemberProperty result = null;
            bool propertyIsBag = false;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                long bagIndex;
                if (propertyIsBag && long.TryParse(propertyNames[i], out bagIndex))
                {
                    propertyIsBag = false;
                    continue;
                }

                result = currentProperties.Where(p => p.Name == propertyNames[i]).Single();
                var complexDataType = result.PropertyType as ComplexDataType;
                if (complexDataType != null)
                {
                    currentProperties = complexDataType.Definition.Properties;
                }

                var collectionDataType = result.PropertyType as CollectionDataType;
                propertyIsBag = collectionDataType != null;

                if (propertyIsBag)
                {
                    complexDataType = collectionDataType.ElementDataType as ComplexDataType;
                    if (complexDataType != null)
                    {
                        currentProperties = complexDataType.Definition.Properties;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Abstract class that keeps the state as it was before SaveChanges for the descriptor data that represents change.
        /// </summary>
        internal abstract class ChangeData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChangeData"/> class.
            /// </summary>
            /// <param name="descriptorData">The descriptor data.</param>
            protected ChangeData(DescriptorData descriptorData)
            {
                this.State = descriptorData.State;
                this.DescriptorData = descriptorData;
            }

            /// <summary>
            /// Gets the state.
            /// </summary>
            /// <value>The state.</value>
            public EntityStates State { get; private set; }

            /// <summary>
            /// Gets the descriptor data.
            /// </summary>
            /// <value>The descriptor data.</value>
            public DescriptorData DescriptorData { get; private set; }

            /// <summary>
            /// Gets or sets the CLR type for requery.
            /// </summary>
            /// <value>The CLR type for requery.</value>
            public Type ClrTypeForRequery { get; protected set; }

            /// <summary>
            /// Gets the Uri for requery.
            /// </summary>
            /// <returns>Uri for requery.</returns>
            public abstract Uri GetUriForRequery();

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "Change: {0}; DescriptorData: {1}", this.State, this.DescriptorData);
            }
        }

        /// <summary>
        /// Keeps the entity properties values and the state as they were right before SaveChanges for the entity.
        /// </summary>
        internal class EntityChangeData : ChangeData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EntityChangeData"/> class.
            /// </summary>
            /// <param name="entityDescriptorData">The entity descriptor data.</param>
            protected EntityChangeData(EntityDescriptorData entityDescriptorData)
                : base(entityDescriptorData)
            {
                this.CachedPropertiesValues = new Dictionary<string, object>();
            }

            /// <summary>
            /// Gets the cached properties values.
            /// </summary>
            /// <value>The cached properties values.</value>
            public Dictionary<string, object> CachedPropertiesValues { get; private set; }

            /// <summary>
            /// Gets the entity descriptor data.
            /// </summary>
            /// <value>The descriptor data.</value>
            public new EntityDescriptorData DescriptorData
            {
                get { return (EntityDescriptorData)base.DescriptorData; }
            }

            /// <summary>
            /// Creates EntityChangeData which captures the state of the specified entity descriptor data.
            /// </summary>
            /// <param name="entityDescriptorData">The entity descriptor data.</param>
            /// <param name="propertiesValues">The properties values before SaveChanges.</param>
            /// <returns>Entity change data.</returns>
            public static ChangeData Create(EntityDescriptorData entityDescriptorData, IEnumerable<NamedValue> propertiesValues)
            {
                EntityChangeData changeData = new EntityChangeData(entityDescriptorData);

                foreach (NamedValue nv in propertiesValues)
                {
                    changeData.CachedPropertiesValues.Add(nv.Name, nv.Value);
                }

                changeData.ClrTypeForRequery = entityDescriptorData.EntityClrType;

                return changeData;
            }

            /// <summary>
            /// Gets the Uri for requery.
            /// </summary>
            /// <returns>Uri for requery.</returns>
            public override Uri GetUriForRequery()
            {
                var uri = this.DescriptorData.SelfLink;
                if (uri == null)
                {
                    uri = this.DescriptorData.EditLink;
                }

                ExceptionUtilities.CheckObjectNotNull(uri, "The Uri for requery is null");
                return uri;
            }
        }

        /// <summary>
        /// Keep the state as it was right before SaveChanges for the link.
        /// </summary>
        internal class LinkChangeData : ChangeData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LinkChangeData"/> class.
            /// </summary>
            /// <param name="linkDescriptorData">The link descriptor data.</param>
            protected LinkChangeData(LinkDescriptorData linkDescriptorData)
                : base(linkDescriptorData)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this change is for reference navigation property.
            /// </summary>
            /// <value>
            ///    <c>true</c> if this change is for reference navigation property; otherwise, <c>false</c>.
            /// </value>
            public bool IsReference { get; private set; }

            /// <summary>
            /// Gets the entity descriptor data.
            /// </summary>
            /// <value>The descriptor data.</value>
            public new LinkDescriptorData DescriptorData
            {
                get { return (LinkDescriptorData)base.DescriptorData; }
            }

            /// <summary>
            /// Creates LinkChangeData which captures the state of the specified link descriptor data.
            /// </summary>
            /// <param name="linkData">The link descriptor data.</param>
            /// <returns>Link change data.</returns>
            public static LinkChangeData Create(LinkDescriptorData linkData)
            {
                ExceptionUtilities.CheckArgumentNotNull(linkData, "linkData");

                LinkChangeData changeData = new LinkChangeData(linkData);

                var isEnumerable = linkData.SourceDescriptor.EntityClrType.GetProperty(linkData.SourcePropertyName).PropertyType.GetInterfaces().Any(t => t.Name == "IEnumerable");
                changeData.IsReference = !isEnumerable;

                if (linkData.TargetDescriptor != null)
                {
                    changeData.ClrTypeForRequery = linkData.TargetDescriptor.EntityClrType;
                }
                else
                {
                    ExceptionUtilities.Assert(changeData.IsReference, "TargetDescriptor can not be null for the link descriptor representing collection.");
                    changeData.ClrTypeForRequery = linkData.SourceDescriptor.EntityClrType.GetProperty(linkData.SourcePropertyName).PropertyType;
                }

                return changeData;
            }

            /// <summary>
            /// Gets the URI for requery.
            /// </summary>
            /// <returns>Uri for requery.</returns>
            public override Uri GetUriForRequery()
            {
                LinkDescriptorData linkData = this.DescriptorData;
                
                Uri uri = null;
                var linkInfo = linkData.SourceDescriptor.LinkInfos.SingleOrDefault(l => l.Name == linkData.SourcePropertyName);
                if (linkInfo != null)
                {
                    uri = linkInfo.NavigationLink;
                }

                if (uri == null)
                {
                    uri = new Uri(linkData.SourceDescriptor.EditLink + "/" + linkData.SourcePropertyName);
                }

                if (!this.IsReference)
                {
                    ExceptionUtilities.Assert(linkData.TargetDescriptor != null, "TargetDescriptor can not be null for the link descriptor representing collection.");
                    ExceptionUtilities.CheckObjectNotNull(linkData.TargetDescriptor.EditLink, "Target edit-link can not be null for the link descriptor representing collection.");

                    Regex keyRegex = new Regex(@".+(?<key>\(.+\))");
                    Match match = keyRegex.Match(linkData.TargetDescriptor.EditLink.OriginalString);
                    ExceptionUtilities.Assert(match.Success, "Unhandled entity edit-link format. Edit link: " + linkData.TargetDescriptor.EditLink);

                    string targetKey = match.Groups["key"].Value;
                    uri = new Uri(uri + targetKey);
                }

                return uri;
            }
        }
    }
}
