//---------------------------------------------------------------------
// <copyright file="AstoriaClientTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.ModuleCore;
    using System.Data.Test.Astoria;
    using System.Linq.Expressions;
    using Microsoft.OData.Client;
    using System.Collections;
    using LinqExtensions;
    using EDMMetadataExtensions;
    using System.Xml.Linq;
    using System.Net;

    public class AstoriaClientTestCase : AstoriaFeatureTestCase<ClientConcurrencyWorkspaces>
    {
        public WebDataCtxWrapper CurrentContext;
        #region Helper Methods

        protected void QueryResource(bool workspaceShouldUpdate)
        {
            ForEachResourceType(
                (resourceType, resourceContainer, workspace) =>
                {
                    try
                    {
                        CreateContext(resourceType, workspace);
                        object entity = ((DataServiceQuery)
                                    CurrentContext.CreateQueryOfT(resourceContainer.Name, resourceType.ClientClrType)).First(true);
                        if (ChainedFunction != null)
                        {
                            ChainedFunction();
                        }
                    }
                    catch (System.Reflection.TargetInvocationException tiException)
                    {
                        if (!tiException.ToString().Contains("Sequence contains"))
                        {
                            throw tiException;
                        }
                    }
                }, workspaceShouldUpdate
            );
        }

#if !ClientSKUFramework


        protected void QueryLoadProperty(Func<IEdmEntityType, List<string>> getNavPropsLambda)
        {
            QueryLoadProperty(getNavPropsLambda, false);
        }
        protected void QueryLoadProperty(Func<IEdmEntityType, List<string>> getNavPropsLambda, bool workspaceShouldUpdate)
        {

            foreach (EntityDescriptor entityDec in CurrentContext.UnderlyingContext.Entities)
            {

                IEdmEntityType entityType = DataServiceMetadata.EntityTypes.FirstOrDefault(eType => eType.Name == entityDec.Entity.GetType().Name);
                foreach (string collNavProperty in getNavPropsLambda(entityType))
                {
                    AstoriaTestLog.WriteLine("Loading Properties {0}", collNavProperty);
                    CurrentContext.LoadProperty(entityDec.Entity, collNavProperty);
                    VerifyLoadProperty(entityDec.Entity, collNavProperty, CurrentContext);
                }

            }

        }

#endif

        private void VerifyLoadProperty(object entity, string propertyName, WebDataCtxWrapper Context)
        {
            Uri entityUri = null;
            if (Context.TryGetUri(entity, out entityUri))
            {
                Uri navigationPropertyURI = new Uri(String.Format("{0}/{1}", entityUri.OriginalString, propertyName)); ;
                Type navPropType = entity.GetType().GetProperty(propertyName).PropertyType;
                if (navPropType.IsGenericType)
                {
                    navPropType = navPropType.GetGenericArguments()[0];
                }
                WebDataCtxWrapper baselineCOntext = new WebDataCtxWrapper(Context.BaseUri);
                baselineCOntext.Credentials = CredentialCache.DefaultCredentials;
                QueryOperationResponse qoResponse = baselineCOntext.ExecuteOfT(navPropType, navigationPropertyURI) as QueryOperationResponse;
                IEnumerator enumerator = qoResponse.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    object baselineEntity = enumerator.Current;
                    Uri navPropEntityURI = null;
                    if (baselineCOntext.TryGetUri(baselineEntity, out  navPropEntityURI))
                    {
                        AstoriaTestLog.IsNotNull(Context.TryGetEntityOfT(navPropType, navPropEntityURI), "Could not find entity in Navigation property ");
                    }
                }
            }
        }
        protected ResourceType GetResourceType(string propertyName, ResourceType parentType)
        {
            NodeType nodeType = parentType.Properties[propertyName].Type;
            ResourceType resourceTYpe = null;
            if (nodeType is ResourceCollection)
            {
                resourceTYpe = ((ResourceCollection)nodeType).SubType as ResourceType;
            }
            else
            {
                resourceTYpe = nodeType as ResourceType;
            }
            return resourceTYpe;
        }

#if !ClientSKUFramework


        protected void QueryNavProperty(Func<IEdmEntityType, List<string>> getNavPropsLambda)
        {
            ForEachResourceType(
                (resourceType, resourceContainer, workspace) =>
                {
                    CreateContext(resourceType, workspace);
                    AstoriaTestLog.WriteLine("Querying entityset {0}", resourceContainer.Name);
                    IEdmEntityType entityType = DataServiceMetadata.EntityTypes.FirstOrDefault(eType => eType.Name == resourceType.Name);
                    KeyExpression keyExp = workspace.GetRandomExistingKey(resourceContainer);
                    if (keyExp != null && (!(resourceContainer is ServiceOperation)))
                    {
                        List<KVP> keyExpValues = WebDataCtxWrapper.ConvertKeyExpression(keyExp);
                        foreach (string collNavProperty in getNavPropsLambda(entityType))
                        {
                            AstoriaTestLog.WriteLine("Querying Properties {0}", collNavProperty);
                            try
                            {

                                DataServiceQuery queryWithExpand = ((DataServiceQuery)CurrentContext.CreateQueryOfT(resourceContainer.Name, resourceType.ClientClrType)).Where(keyExpValues);//.SelectNavigationProperty(collNavProperty) as DataServiceQuery;
                                IEnumerator enumerateQueryResults = ((IQueryable)queryWithExpand).GetEnumerator();
                                object entity = null;
                                if (enumerateQueryResults.MoveNext())
                                    entity = enumerateQueryResults.Current;
                                while (enumerateQueryResults.MoveNext()) ;
                                Uri entityUri = null;
                                CurrentContext.UnderlyingContext.TryGetUri(entity, out entityUri);
                                CurrentContext.Detach(entity);
                                entityUri = new Uri(entityUri.OriginalString + "/" + collNavProperty);

                                var qoREsponse = CurrentContext.ExecuteOfT(GetResourceType(collNavProperty, resourceType).ClientClrType, entityUri);
                                IEnumerator enumerator = qoREsponse.GetEnumerator();
                                while (enumerator.MoveNext()) ;
                                if (ChainedFunction != null)
                                {
                                    ChainedFunction();
                                }
                            }
                            catch (System.Reflection.TargetInvocationException tiException)
                            {
                                if (!tiException.ToString().Contains("Sequence Contains"))
                                {
                                    throw tiException;
                                }
                            }
                        }
                    }
                }, false);
        }
        protected void QueryExpandProperty(Func<IEdmEntityType, List<string>> getNavPropsLambda)
        {
            ForEachResourceType(
                (resourceType, resourceContainer, workspace) =>
                {
                    CreateContext(resourceType, workspace);
                    AstoriaTestLog.WriteLine("Querying entityset {0}", resourceContainer.Name);
                    IEdmEntityType entityType = DataServiceMetadata.EntityTypes.FirstOrDefault(eType => eType.Name == resourceType.Name);
                    KeyExpression keyExp = workspace.GetRandomExistingKey(resourceContainer);
                    if (keyExp != null)
                    {
                        List<KVP> keyExpValues = WebDataCtxWrapper.ConvertKeyExpression(keyExp);
                        foreach (string collNavProperty in getNavPropsLambda(entityType))
                        {
                            AstoriaTestLog.WriteLine("Expanding Properties {0}", collNavProperty);
                            try
                            {
                                DataServiceQuery queryWithExpand = ((DataServiceQuery)CurrentContext.CreateQueryOfT(resourceContainer.Name, resourceType.ClientClrType)).Where(keyExpValues).Expand(collNavProperty);
                                IEnumerator enumerateQueryResults = ((IQueryable)queryWithExpand).GetEnumerator();
                                try
                                {
                                    while (enumerateQueryResults.MoveNext()) ;
                                }
                                catch (OptimisticConcurrencyException oException)
                                {
                                    AstoriaTestLog.WriteLineIgnore("Failed as per Expand causes etags not to be included." + oException.Message);
                                }
                                if (ChainedFunction != null)
                                {
                                    ChainedFunction();
                                }
                            }
                            catch (System.Reflection.TargetInvocationException tiException)
                            {
                                if (!tiException.ToString().Contains("Sequence Contains"))
                                {
                                    throw tiException;
                                }
                            }
                        }
                    }
                }, false);
        }
#endif

        protected void QueryNavPropCollection()
        {
#if !ClientSKUFramework

            Func<IEdmEntityType, List<string>> getNavPropsLambda = edmEntityType =>
            {
                return edmEntityType.CollectionNavigationProperties();
            };
            QueryNavProperty(getNavPropsLambda);
#endif
        }
        protected void QueryNavPropReference()
        {
#if !ClientSKUFramework

            Func<IEdmEntityType, List<string>> getNavPropsLambda = edmEntityType =>
            {
                return edmEntityType.ReferenceNavigationProperties();
            };
            QueryNavProperty(getNavPropsLambda);
#endif
        }

        ResourceContainer currentContainer;
        ResourceType currentResourceType;
        Workspace currentWorkspace;
        public SaveChangesOptions SaveChangeOption { get; set; }

        protected void UpdateAction(ResourceType resourceType, ResourceContainer resourceContainer, object entity)
        {
            AstoriaTestLog.WriteLine("Updating entity set :{0} , entity type :{1}", resourceContainer.Name, resourceType.Name);
            //Change any of the properties that make up the Etag
#if !ClientSKUFramework

            IEdmEntityType entityType = DataServiceMetadata.GetEntityType(entity);
            foreach (string eTagProperty in entityType.EtagProperties())
            {
                Type propertyType = entity.GetType().GetProperty(eTagProperty).PropertyType;
                object newValue = propertyType.GetTypedValue(eTagProperty, resourceType.Properties[eTagProperty].Facets.MaxSize);
                //entity.SetPropertyValue(eTagProperty, newValue);
            }
#endif

            CurrentContext.UpdateObject(entity);
            ExecuteAndValidate(() => { CurrentContext.SaveChanges(SaveChangeOption); });
        }

        protected void ExecuteAndValidate(Action saveChangesAction)
        {
            try
            {
                saveChangesAction();
                ContinueTest = true;
            }
            catch (Exception exception)
            {
                ValidateUpdateException(exception);
                ContinueTest = false;
            }
        }

        protected void ValidateUpdateException(Exception failedException)
        {
            string[] strExpectedErrorMessages = { "System.Data.Mapping.Update.Internal.UpdateTranslator.RelationshipConstraintValidator.ValidateConstraints()",
                                                       "System.Data.EntityUtil.ThrowPropertyIsNotNullable",
                                                        "Violation of PRIMARY KEY constraint"};
            if (!strExpectedErrorMessages.Any(str => failedException.ToString().Contains(str)))
            {
                throw failedException;
            }
        }

        protected void Update()
        {
            Update(null);
        }
        protected void Update(Action nextAction)
        {
            foreach (EntityDescriptor entityDesc in CurrentContext.UnderlyingContext.Entities)
            {
                if (entityDesc.State != EntityStates.Deleted && !((ClientConcurrencyWorkspaces)TestWorkspaces).SkipCLRType(entityDesc.Entity.GetType()))
                {
                    CurrentContext.UpdateObject(entityDesc.Entity);
                }
            }
            if (nextAction != null)
            {
                nextAction();
            }
        }

        public MergeOption mergeOption { get; set; }
        protected WebDataCtxWrapper CreateContext(ResourceType resourceType, Workspace workspace)
        {
            CurrentContext = new WebDataCtxWrapper(new Uri(workspace.ServiceUri));
            CurrentContext.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            CurrentContext.MergeOption = mergeOption;
            //CurrentContext.UndeclaredPropertyBehaviorType = UndeclaredPropertyBehavior.Support;
            CurrentContext.UnderlyingContext.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(CurrentContext_SendingRequest);
            return CurrentContext;
        }

        protected WebDataCtxWrapper CreateContext(Workspace workspace)
        {

            CurrentContext = new WebDataCtxWrapper(new Uri(workspace.ServiceUri));
            CurrentContext.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            CurrentContext.ResolveName = (clrType) =>
            {
                return clrType.FullName.Replace("SDPV2", "").Replace("V2", "");
            };
            CurrentContext.MergeOption = mergeOption;
            //CurrentContext.UndeclaredPropertyBehaviorType = UndeclaredPropertyBehavior.Support;
            CurrentContext.UnderlyingContext.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(CurrentContext_SendingRequest);
            return CurrentContext;
        }

        protected WebDataCtxWrapper CreateContext(Workspace workspace, bool conflictOverride)
        {

            var CurrentContext = new WebDataCtxWrapper(new Uri(workspace.ServiceUri));
            CurrentContext.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            CurrentContext.MergeOption = mergeOption;
            //CurrentContext.UndeclaredPropertyBehaviorType = UndeclaredPropertyBehavior.Support;
            CurrentContext.UnderlyingContext.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(CurrentContext_SendingRequest);
            return CurrentContext;
        }

        void CurrentContext_SendingRequest(object sender, SendingRequest2EventArgs e)
        {

        }

        protected object InsertAction(ResourceType resourceType, ResourceContainer resourceContainer, Workspace workspace)
        {
            AstoriaTestLog.WriteLine("Inserting into entity set :{0} , entity type :{1}", resourceContainer.Name, resourceType.Name);
            object entity = null;
            if (resourceType.Name == "AllTypesComplexEntity")
            {
                IQueryable results = CurrentContext.ExecuteOfT(resourceType.ClientClrType, new Uri(String.Format("{0}?$top=1", resourceContainer.Name), UriKind.RelativeOrAbsolute)).AsQueryable();
                IEnumerator enumerat = results.GetEnumerator();
                enumerat.MoveNext();
                entity = enumerat.Current;
                ResourceProperty keyProperty = resourceType.Properties.Cast<ResourceProperty>().First(prop => prop.PrimaryKey != null);
                entity.GetType().GetProperty(keyProperty.Name).SetValue(entity, null, null);
            }
            else
            {
                entity = resourceType.CreateInstance(false);
            }
            CurrentContext.AddObject(resourceContainer.Name, entity);
            CurrentContext.EnsureInsert(entity, resourceType);
            EnsureInsert(CurrentContext.UnderlyingContext, entity, resourceContainer.Name, workspace, "");
            ExecuteAndValidate(() => { CurrentContext.SaveChanges(SaveChangeOption); });
            return entity;
        }
        protected Action ChainedFunction { get; set; }
        protected object ExecuteURI(ResourceType resourceType, string requestURI)
        {
            return ExecuteURI(resourceType, requestURI, CurrentContext);
        }
        protected object ExecuteURI(ResourceType resourceType, string requestURI, WebDataCtxWrapper context)
        {
            IQueryable result = context.ExecuteOfT(resourceType.ClientClrType, new Uri(requestURI, UriKind.RelativeOrAbsolute)).AsQueryable();

            object entity = null;
            IEnumerator requestEnumerator = result.GetEnumerator();
            if (requestEnumerator.MoveNext())
                entity = requestEnumerator.Current;
            return entity;
        }

        protected void InsertStep()
        {
            ForEachResourceType(
             (resourceType, resourceContainer, workspace) =>
             {
                 CreateContext(resourceType, workspace);
                 object entity = InsertAction(resourceType, resourceContainer, workspace);
                 if (ContinueTest)
                 {
                     if (ChainedFunction != null)
                         ChainedFunction();

                 }
             }, true);
        }

        protected void InsertReferenceNavProperty()
        {
            ForEachResourceType(
             (resourceType, resourceContainer, workspace) =>
             {
                 CreateContext(resourceType, workspace);
#if !ClientSKUFramework
                 object entity = ExecuteURI(resourceType, String.Format("/{0}", resourceContainer.Name));
                 Uri entityUri = null;


                 IEdmEntityType entityType = DataServiceMetadata.GetEntityType(entity);
                 if (entityType == null) return;
                 CurrentContext.TryGetUri(entity, out entityUri);
                 foreach (string refNavProperty in entityType.ReferenceNavigationProperties())
                 {
                     ResourceType navResType = workspace.ServiceContainer.ResourceTypes.FirstOrDefault(rt => rt.Name == refNavProperty);
                     if (navResType != null)
                     {
                         object navEntity = navResType.CreateInstance(false);
                         string navPropertyURI = String.Format("{0}/{1}", entityUri.Segments[entityUri.Segments.Length - 1], refNavProperty);
                         CurrentContext.AddObject(navPropertyURI, navEntity);
                         CurrentContext.EnsureInsert(navEntity, navResType);
                     }
                 }
                 ExecuteAndValidate(() => CurrentContext.SaveChanges(SaveChangeOption));
                 if (ContinueTest)
                 {
                     if (ChainedFunction != null)
                         ChainedFunction();

                 }
#endif
             }, true);
        }

        protected bool ContinueTest = true;
        protected ClientConcurrencyWorkspaces TestWorkspaces { get; set; }
        protected virtual void ForEachResourceType(Action<ResourceType, ResourceContainer, Workspace> someAction, bool workspaceShouldUpdate)
        {

            TestWorkspaces.ForEachResourceType(
                (resourceType, container, workspace) =>
                {
                    currentContainer = container;
                    currentResourceType = resourceType;
                    currentWorkspace = workspace;
                    someAction(resourceType, container, workspace);
                }, workspaceShouldUpdate
                );
        }


        protected virtual Func<WebDataCtxWrapper, ResourceType, Uri, IQueryable> DoUriQuery { get; set; }
        protected virtual Func<WebDataCtxWrapper, ResourceType, Expression, IQueryable> DoLinqQuery { get; set; }

        protected void EnsureInsert(DataServiceContext context, object entity, string entitySetName, Workspace workspace, string skipEntitySet)
        {
#if !ClientSKUFramework

            IEdmEntityType entityType = null;
            if (DataServiceMetadata.ServiceMetadata == null)
            {
                DataServiceMetadata.LoadServiceMetadata(workspace.ServiceUri);
            }
            if (DataServiceMetadata.EntityTypes.Any(eType => eType.Name == entitySetName))
            {
                entityType = DataServiceMetadata.EntityTypes.First(eType => eType.Name == entitySetName);
            }
            if (entityType == null && DataServiceMetadata.EntityTypes.Any(eType => eType.Name == entity.GetType().Name))
            {
                entityType = DataServiceMetadata.EntityTypes.First(eType => eType.Name == entity.GetType().Name);
                entitySetName = entity.GetType().Name;
            }
            if (entityType == null) return;
            foreach (IEdmNavigationProperty navProperty in entityType.NavigationProperties())
            {
                if (context.Links.All(ld => (ld.SourceProperty != navProperty.Name)))
                {
                    if (navProperty.TargetMultiplicity() == EdmMultiplicity.One && navProperty.Name != skipEntitySet)
                    {
                        IEdmEntityType navProperyEntityType = DataServiceMetadata.GetEntityType(navProperty);
                        ResourceType resourceType = workspace.ServiceContainer.ResourceTypes.First(rt => rt.Name == navProperyEntityType.Name);
                        object instance = resourceType.CreateInstance(false);
                        context.AddObject(navProperyEntityType.Name == "College" ? "Colleges" : navProperyEntityType.Name, instance);
                        context.SetLink(entity, navProperty.Name, instance);
                        context.SaveChangesDefaultOptions = SaveChangesOptions.BatchWithSingleChangeset;
                    }
                }
            }
#endif
        }

        protected void Delete()
        {
            foreach (EntityDescriptor entityDesc in CurrentContext.UnderlyingContext.Entities)
            {
                ResourceType entityType = currentWorkspace.ServiceContainer.ResourceTypes.FirstOrDefault(rt => rt.Name == entityDesc.Entity.GetType().Name);
                if (entityType != null)
                {
                    CurrentContext.EnsureDelete(entityDesc.Entity, entityType);
                }

            }
        }
        #endregion
    }
}
