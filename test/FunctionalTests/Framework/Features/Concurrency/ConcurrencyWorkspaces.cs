//---------------------------------------------------------------------
// <copyright file="ConcurrencyWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
namespace System.Data.Test.Astoria
{
    public class ConcurrencyWorkspaces : FeatureWorkspaces
    {
        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // TODO: need a way to make sure all containers exist

            if (!attribute.Name.Equals("Aruba", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (attribute.DataLayerProviderKind != DataLayerProviderKind.Edm
                && attribute.DataLayerProviderKind != DataLayerProviderKind.InMemoryLinq
                && attribute.DataLayerProviderKind != DataLayerProviderKind.NonClr)

                return false;

            return true;
        }

        protected override void WorkspaceCallback(Workspace workspace)
        {
            if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
            {
#if !ClientSKUFramework

                // do this NOW instead of later, so that we can refer to these settings
                (workspace as NonClrWorkspace).DefineClrProperties();
#endif
            }

            Dictionary<string, string[]> tokensMap = new Dictionary<string, string[]>();
            tokensMap.Add("DefectBug", new string[] { "Number" });
            tokensMap.Add("Owner", new string[] { "LastName" });
            tokensMap.Add("Run", new string[] { "Purpose" });
            tokensMap.Add("Task", new string[] { "Deleted", "Investigates" });
            tokensMap.Add("OwnerDetail", new string[] { "Level" });
            tokensMap.Add("ProjectBug", new string[] { "Number" });
            tokensMap.Add("NonDefaultMappings",
                new string[] { "c_int_AS_decimal", "c_smallint_AS_decimal", "c_smalldatetime_AS_datetime" });
            tokensMap.Add("AllTypes",
                new string[] { "c2_int", "c3_smallint", "c4_tinyint", "c5_bit", "c6_datetime", "c7_smalldatetime",
                            "c8_decimal_28_4_", "c9_numeric_28_4_", "c10_real", "c11_float", "c12_money",
                            "c13_smallmoney", /*"c14_varchar_512_", "c15_char_512_",*/ "c26_smallint",
                            "c27_tinyint", "c28_bit", "c46_uniqueidentifier", "c47_bigint"});
            tokensMap.Add("Vehicle", new string[] { "Make", "Model", "Year" });
            tokensMap.Add("Test1s", new string[] { "Name" }); // for deep expand where top type has no etag
            tokensMap.Add("Run2s", new string[] { "Name" });
            tokensMap.Add("Student", new string[] { "FirstName", "MiddleName", "LastName", "Major" });
            tokensMap.Add("College", new string[] { "Name", "City", "State" });
            tokensMap.Add("Config", new string[] { "Arch" });
            //tokensMap.Add("DeepTree_C", new string[] { "C_Int" });
            tokensMap.Add("LabOwners", new string[] { "Changed" });

            foreach (ResourceType type in workspace.ServiceContainer.ResourceTypes.Where(rt => rt.Name.StartsWith("DataKey_")))
                tokensMap.Add(type.Name, type.Properties
                    .OfType<ResourceProperty>()
                    .Where(p => p.PrimaryKey == null && !p.IsNavigation && !p.IsComplexType)
                    .Select(p => p.Name)
                    .ToArray());

            foreach (KeyValuePair<string, string[]> pair in tokensMap)
            {
                ResourceType type = workspace.ServiceContainer.ResourceTypes.SingleOrDefault(t => t.Name == pair.Key);
                if (type == null)
                {
                    // look through the basetypes
                    type = workspace.ServiceContainer.ResourceTypes.SingleOrDefault(t => t.BaseType != null && t.BaseType.Name == pair.Key);
                    if (type == null)
                    {
                        AstoriaTestLog.FailAndContinue(new TestWarningException("Could not find type '" + pair.Key + "'"));
                        continue;
                    }
                    type = type.BaseType as ResourceType;
                }

                // ensure that we put the property names in the order they are in the service container
                List<string> propertyNames = new List<string>();
                List<string> etagProperties = pair.Value.ToList();
                foreach (var property in type.Properties.OfType<ResourceProperty>())
                {
                    var propertyName = property.Name;
                    if (etagProperties.Contains(propertyName))
                    {
                        propertyNames.Add(propertyName);
                        etagProperties.Remove(propertyName);

                        if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr && !property.Facets.Nullable)
                        {
                            // due to an issue with the provider not enforcing non-nullability on weakly-backed properties,
                            // if the property is non-nullable make sure its strongly backed
                            property.Facets.IsClrProperty = true;
                            type.Facets.IsClrType = true;
                            foreach (var derived in type.DerivedTypes)
                            {
                                derived.Facets.IsClrType = true;
                            }
                        }
                    }
                }

                if (etagProperties.Any())
                    throw new TestWarningException(string.Format("Could not find properties on type '{0}': {1}", type.Name, string.Join(", ", etagProperties.ToArray())));

                type.Facets.Add(NodeFacet.Attribute(new ConcurrencyAttribute(type, propertyNames)));
            }

            List<ServiceOperation> serviceOperations = new List<ServiceOperation>();
            foreach (ResourceContainer container in workspace.ServiceContainer.ResourceContainers)
            {
                // no MEST yet for service ops!!
                if (workspace.ServiceContainer.ResourceContainers.Any(c => c != container && c.BaseType == container.BaseType))
                    continue;

                ServiceOperation serviceOp = Resource.ServiceOperation("Get" + container.Name, container, container.BaseType, container.ResourceTypes.ToArray());

                string fullTypeName = container.BaseType.Namespace + "." + container.BaseType.Name;
#if !ClientSKUFramework

                if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr && !container.BaseType.Facets.IsClrType)
                    fullTypeName = typeof(NonClr.RowEntityType).FullName;
#endif
                serviceOp.ServiceOpCode = string.Join(Environment.NewLine, new string[]
                {
                    "[WebGet]",
                    "public IQueryable<" + fullTypeName + "> " + serviceOp.Name + "()",
                    "{",
                    "     return GetEntitySet<" + fullTypeName + ">(\"" + container.Name + "\");",
                    "}"
                });
#if !ClientSKUFramework

                serviceOp.ServiceOperationResultKind = Microsoft.OData.Service.Providers.ServiceOperationResultKind.QueryWithMultipleResults;
#endif
                serviceOp.ExpectedTypeName = fullTypeName;
                serviceOperations.Add(serviceOp);
            }

            workspace.ServiceContainer.AddNodes(serviceOperations);
            workspace.AddServiceOperationCode();
        }

        protected override IEnumerable<Workspace> Construct(Type t)
        {
#if !ClientSKUFramework

            if (typeof(InMemoryWorkspace).IsAssignableFrom(t))
            {
                List<Workspace> constructed = new List<Workspace>();
                Workspace withoutCustom = (Workspace)Activator.CreateInstance(t);
                withoutCustom.Settings.UpdatableImplementation = UpdatableImplementation.IUpdatable;
                constructed.Add(withoutCustom);

                if (Versioning.Server.SupportsV2Features)
                {
                    Workspace withCustom = (Workspace)Activator.CreateInstance(t);
                    withoutCustom.Settings.UpdatableImplementation = UpdatableImplementation.DataServiceUpdateProvider;
                    withCustom.WebServiceName = withCustom.WebServiceName + "_UpdateProvider";
                    constructed.Add(withCustom);
                }

                return constructed;
            }
            else
                return base.Construct(t);
#endif
#if ClientSKUFramework
	return null;
#endif

        }
    }
    public class ClientConcurrencyWorkspaces : FeatureWorkspaces
    {
        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // TODO: need a way to make sure all containers exist
            return true;
        }
        public bool UseInheritedTypes { get; set; }

        protected bool FilterWorkspaces(WorkspaceAttribute attribute)
        {
            return !attribute.Name.Contains("PicturesTags");
        }
        protected override void Filter()
        {
            Filter(this.FilterWorkspaces);
        }
        protected override void FilterAndCreate()
        {
            base.FilterAndCreate(WorkspaceCallback, this.FilterWorkspaces);
        }
        public bool SkipType(ResourceType resourceType, bool workspaceShouldUpdate)
        {
            bool skipType = true;
            if ((UseInheritedTypes && resourceType.BaseType != null)
                               ||
                               (!UseInheritedTypes && resourceType.BaseType == null)
                               )
            {
                //Entities with DateTime Keys cause a Http 400 bad request in IIS 
                if (!resourceType.Name.Contains("DateTime") && resourceType.Facets.Attributes.OfType<ConcurrencyAttribute>().Any())
                {
                    //Skip any types that have server generated properties as non-key properties
                    if (!resourceType.Properties.Cast<ResourceProperty>().Any(rp => rp.PrimaryKey == null && rp.Facets.ServerGenerated))
                    {
                        if (!workspaceShouldUpdate ||
                                (resourceType.Name != "DataKey_Bit" && resourceType.Name != "DataKey_BigInt" && resourceType.Name != "Employees"))
                        {
                            if (!workspaceShouldUpdate || resourceType.IsInsertable)
                            {
                                skipType = false;
                            }
                        }
                    }
                }
            }
            return skipType;
        }

        public bool SkipCLRType(Type resourceType)
        {
            return new string[] { "DataKey_Bit", "DataKey_BigInt", "DataKey_DateTime", "DataKey_SmallDateTime", "DataKey_DateTime", "DefectBug", "ProjectBug" }.Contains(resourceType.Name);
        }
        public virtual void ForEachResourceType(Action<ResourceType, ResourceContainer, Workspace> someAction, bool workspaceShouldUpdate)
        {
            Func<ResourceType, bool> typeHasConcurrencyTokens = (resourceType) =>
            {
                bool hasConcurrencyTokens = false;
                foreach (ResourceProperty property in resourceType.Properties.OfType<ResourceProperty>())
                {
                    if (property.Facets.ConcurrencyMode == ConcurrencyMode.Fixed)
                    {
                        hasConcurrencyTokens = true;
                        break;
                    }
                }
                return hasConcurrencyTokens;
            };
            foreach (Workspace workspace in this)
            {
                bool doneProcessing = false;
                DataServiceMetadata.LoadServiceMetadata(workspace.ServiceUri);
                if (workspaceShouldUpdate && !workspace.Settings.SupportsUpdate)
                {
                    throw (new TestSkippedException("Workspace does not support updating"));
                }
                foreach (ResourceContainer container in workspace.ServiceContainer.ResourceContainers)
                {
                    foreach (ResourceType resourceType in
                        container.ResourceTypes.Where(rType => (!workspaceShouldUpdate || rType.IsInsertable) && typeHasConcurrencyTokens(rType)))
                    {
                        try
                        {
                            //Filter only types which have base types , if required
                            if (!SkipType(resourceType, workspaceShouldUpdate))
                            {
                                doneProcessing = true;
                                someAction(resourceType, container, workspace);

                            }
                        }
                        catch (OptimisticConcurrencyException oException)
                        {
                            throw oException;
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is OptimisticConcurrencyException))
                                AstoriaTestLog.FailAndContinue(exception);
                        }
                        if (doneProcessing)
                        {
                            break;
                        }

                    }
                    if (doneProcessing)
                    {
                        break;
                    }
                }
            }
        }

        List<string> typesTraversed;
        int expandDepth = 0;
        private string GetDottedPath(ResourceType resourceType)
        {
            if (typesTraversed == null)
                typesTraversed = new List<string>();
            if (typesTraversed.Contains(resourceType.Name))
            {
                return "";
            }
            string dottedpath = "";
            expandDepth++;
            typesTraversed.Add(resourceType.Name);
            if (resourceType.Properties.Cast<ResourceProperty>().Any(rp => rp.IsNavigation))
            {
                ResourceProperty navigationProperty = resourceType.Properties.Cast<ResourceProperty>().First(rp => rp.IsNavigation);
                NodeType navType = navigationProperty.Type;
                ResourceType navResType = null;
                if (navType is ResourceCollection)
                {
                    navResType = ((ResourceCollection)navType).SubType as ResourceType;
                }
                else
                {
                    navResType = navType as ResourceType;
                }
                if (navResType != null)
                {
                    dottedpath += "." + navigationProperty.Name + "." + GetDottedPath(navResType);
                }
                else
                {
                    dottedpath += "." + navigationProperty.Name;
                }

            }
            return dottedpath.TrimStart('.').TrimEnd('.');
        }
        List<ServiceOperation> serviceOperations;
        protected void SetupServiceOperations(Workspace workspace, ResourceContainer resourceContainer, ResourceType resourceType)
        {
            if (serviceOperations == null)
                serviceOperations = new List<ServiceOperation>();

            #region Single level Expand
            ServiceOperation serviceOperation = Resource.ServiceOperation(String.Format("Return{0}OfTWithExpandOneLevel", resourceType.Name), resourceContainer, resourceType);
            serviceOperation.ExpectedTypeName = resourceType.Name;
            StringBuilder includeString = new StringBuilder();
            foreach (ResourceProperty navigationProperty in resourceType.Properties.Cast<ResourceProperty>().Where(rp => rp.IsNavigation))
            {
                includeString.AppendFormat(".Include(\"{0}\")", navigationProperty.Name);
            }
            serviceOperation.ServiceOpCode = String.Format(Workspace.IQueryableOfTWithOneIncludeServiceOpTemplate, resourceType.Name, resourceContainer.Name, workspace.ContextNamespace, resourceType.Name, includeString.ToString());
            #endregion


            #region Multiple level Expand

            includeString = new StringBuilder();
            string dottedExpandPath = GetDottedPath(resourceType);
            ServiceOperation serviceOperation2LevelExpand = Resource.ServiceOperation(String.Format("Return{0}OfTWithExpand{1}Level", resourceType.Name, expandDepth), resourceContainer, resourceType);
            if (dottedExpandPath.Length > 0)
            {
                includeString.AppendFormat(".Include(\"{0}\")", dottedExpandPath);
                serviceOperation2LevelExpand.ServiceOpCode = String.Format(Workspace.IQueryableOfTWithNIncludeServiceOpTemplate, resourceType.Name, resourceContainer.Name, workspace.ContextNamespace, resourceType.Name, includeString.ToString(), expandDepth);
            #endregion
                if (!serviceOperations.Any(sop => sop.Name == serviceOperation2LevelExpand.Name)
                          && (!workspace.ServiceContainer.Any(sop => sop.Name == serviceOperation2LevelExpand.Name))
                          )
                {
                    serviceOperations.Add(serviceOperation2LevelExpand);
                }
            }

            if (!serviceOperations.Any(sop => sop.Name == serviceOperation.Name)
                          && (!workspace.ServiceContainer.Any(sop => sop.Name == serviceOperation.Name))
                          )
            {
                serviceOperations.Add(serviceOperation);
            }
            expandDepth = 0;
            typesTraversed = new List<string>();
            //
        }
        protected bool AnyBaseTypeHasConcurrencyFacets(ResourceType resourceType)
        {
            if (resourceType.BaseType == null)
            {
                return
                    resourceType.Facets.Attributes.Any(attr => attr is ConcurrencyAttribute) || resourceType.Name == "DeepTree_A" || resourceType.Name == "ConfigBase";

            }
            else
            {
                return AnyBaseTypeHasConcurrencyFacets(resourceType.BaseType as ResourceType);
            }

        }


        protected override void WorkspaceCallback(Workspace workspace)
        {
            if (PostWorkspaceCreation == null)
            {
                PostWorkspaceCreation = (featWorkspaces) =>
                {
                    foreach (Workspace featureWorkspace in this)
                    {
                        featureWorkspace.GenerateServerMappings = false;
                        featureWorkspace.GenerateClientTypes = false;
                        //featureWorkspace.SetupServiceOperations();
                        featureWorkspace.ApplyFriendlyFeeds();
                        this.SetEntitySetPageSizes(featureWorkspace);
                    }
                };
            }

            //serviceOperations = new List<ServiceOperation>();

            foreach (ResourceContainer container in workspace.ServiceContainer.ResourceContainers)
            {
                #region Setup Concurrency tokens on resourceTypes
                foreach (ResourceType resourceType in container.ResourceTypes)
                {
                    SetupServiceOperations(workspace, container, resourceType);
                    SetupConcurrencyTokens(resourceType);
                }
                #endregion


            }

            //workspace.ServiceContainer.AddServiceOperation(serviceOperations);
        }

        private void SetEntitySetPageSizes(Workspace workspace)
        {
            foreach (ResourceContainer container in workspace.ServiceContainer.ResourceContainers.OfType<ResourceContainer>())
            {
                if (!(container is ServiceOperation))
                {
                    workspace.DataService.ConfigSettings.SetEntitySetPageSize(container.Name, 5);
                }
            }
        }

        private void SetupConcurrencyTokens(ResourceType resourceType)
        {
            List<string> mapTheseProperties = new List<string>();
            if (UseInheritedTypes)
            {
                if (!AnyBaseTypeHasConcurrencyFacets(resourceType) && !resourceType.HasDerivedTypes && resourceType.BaseType == null)
                {
                    foreach (ResourceProperty property in resourceType.Properties.OfType<ResourceProperty>()
                          .Where(
                              rp => rp.PrimaryKey == null && !rp.IsNavigation && !rp.IsComplexType && rp.Type.ClrType != typeof(System.Byte[])
                                      && rp.Facets.MaxSize == null
                                      && !rp.Facets.IsStoreBlob)
                          )
                    {
                        mapTheseProperties.Add(property.Name);
                    }
                    resourceType.Facets.Add(NodeFacet.Attribute(new ConcurrencyAttribute(resourceType, mapTheseProperties)));
                }
            }
            else
            {
                foreach (ResourceProperty property in resourceType.Properties.OfType<ResourceProperty>()
                      .Where(
                          rp => rp.PrimaryKey == null && !rp.IsNavigation && !rp.IsComplexType && rp.Type.ClrType != typeof(System.Byte[])
                                  && rp.Facets.MaxSize == null
                                  && !rp.Facets.IsStoreBlob)
                      )
                {
                    mapTheseProperties.Add(property.Name);
                }
                if (resourceType.BaseType == null)
                    resourceType.Facets.Add(NodeFacet.Attribute(new ConcurrencyAttribute(resourceType, mapTheseProperties)));
            }
        }
    }
}
