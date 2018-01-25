//---------------------------------------------------------------------
// <copyright file="ClientActionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Globalization;
    using System.Xml;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Provider = Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using t = System.Data.Test.Astoria;
    #endregion

    /// <summary>
    /// End-to-end tests for DataServiceContext's Execute method on ServiceActions. 
    /// </summary>
    /// <remarks>
    /// 9/1/2011 iawillia: No parameters.  Tests for the root, entity,
    /// entity collection, and queryable binding. Return types: void, primitive, complex,
    /// entity, collection of entity, entity queryable. Collection of primitive and collection
    /// of complex are included but require a slightly different pattern for use.
    /// </remarks>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class ClientActionTests
    {
        #region Test Preperation

        private static List<EntityType> entityInstances;
        private static DSPServiceDefinition service;

        static ClientActionTests()
        {
            entityInstances = new List<EntityType>();
            service = ClientActionTests.ModelWithActions();
        }

        /// <summary>
        /// Resets Updated to false for the EntityType instances in entityInstances,
        /// and does the same the entities in service's "set".
        /// </summary>
        /// <param name="service"></param>
        private static void ResetUpdateFlags(DSPServiceDefinition service)
        {
            foreach (EntityType e in entityInstances)
            {
                e.Updated = false;
            }
            foreach (DSPResource resource in service.CurrentDataSource.GetResourceSetEntities("Set"))
            {
                resource.SetValue("Updated", false);
            }
        }

        /// <summary>Calls BeginExecute or Execute method on the DataServiceContext.</summary>
        private static OperationResponse MyExecute<TResult>(DataServiceContext ctx, Uri uri, bool singleResult, bool isAsync, params OperationParameter[] parameters)
        {
            if (!isAsync)
            {
                return (QueryOperationResponse<TResult>)ctx.Execute<TResult>(uri, "POST", singleResult, parameters);
            }
            else
            {
                IAsyncResult result = ctx.BeginExecute<TResult>(uri, null, null, "POST", singleResult, parameters);
                return (QueryOperationResponse<TResult>)ctx.EndExecute<TResult>(result);
            }
        }

        /// <summary>Calls BeginExecute or Execute method overloads expecting void result on the DataServiceContext.</summary>
        private static OperationResponse MyExecute(DataServiceContext ctx, Uri uri, bool isAsync, params OperationParameter[] parameters)
        {
            if (isAsync)
            {
                IAsyncResult result = ctx.BeginExecute(uri, null, null, "POST", parameters);
                return ctx.EndExecute(result);
            }
            else
            {
                return ctx.Execute(uri, "POST", parameters);
            }
        }

        private bool[] AsyncOptions = { true, false };

        private class PositiveTestCase
        {
            public string RequestUri
            {
                get;
                set;
            }

            public IEnumerable ExpectedResults
            {
                get;
                set;
            }

            public int StatusCode
            {
                get;
                set;
            }

            public Type ExpectedReturnType
            {
                get;
                set;
            }

            public virtual Func<DataServiceContext, Uri, bool, OperationResponse> ExecuteMethod
            {
                get;
                set;
            }

            public virtual Func<DataServiceContext, Uri, bool, OperationParameter[], OperationResponse> ExecuteMethodWithParams
            {
                get;
                set;
            }

            public OperationParameter[] OperationParameters
            {
                get;
                set;
            }
        }

        private class ServerErrorTestCase
        {
            public string RequestUri
            {
                get;
                set;
            }

            public string ErrorMsg
            {
                get;
                set;
            }

            public int StatusCode
            {
                get;
                set;
            }

            public virtual Func<DataServiceContext, Uri, bool, OperationResponse> ExecuteMethod
            {
                get;
                set;
            }
        }

        private class ClientErrorTestCase
        {
            public string RequestUri
            {
                get;
                set;
            }

            public string AtomErrorMsg
            {
                get;
                set;
            }

            public string JsonLightErrorMsg
            {
                get;
                set;
            }

            public int StatusCode
            {
                get;
                set;
            }

            public virtual Func<DataServiceContext, Uri, bool, OperationResponse> ExecuteMethod
            {
                get;
                set;
            }
        }

        private class ComplexType
        {
            public string PrimitiveProperty
            {
                get;
                set;
            }
            public ComplexType ComplexProperty
            {
                get;
                set;
            }
            public override int GetHashCode()
            {
                int p = 1;
                if (PrimitiveProperty != null)
                {
                    p = PrimitiveProperty.GetHashCode();
                }

                int c = 1;
                if (ComplexProperty != null)
                {
                    c = ComplexProperty.GetHashCode();
                }

                return 7 * p * c;
            }
            public override bool Equals(object obj)
            {
                bool baseResult = base.Equals(obj);

                if (baseResult)
                {
                    return true;
                }
                else
                {
                    ComplexType other = obj as ComplexType;
                    if (other == null)
                    {
                        return false;
                    }

                    if (this.PrimitiveProperty == null)
                    {
                        if (other.PrimitiveProperty != null)
                        {
                            return false;
                        }
                    }
                    else if (!this.PrimitiveProperty.Equals(other.PrimitiveProperty))
                    {
                        return false;
                    }

                    if (this.ComplexProperty == null)
                    {
                        if (other.ComplexProperty != null)
                        {
                            return false;
                        }
                    }
                    else if (!this.ComplexProperty.Equals(other.ComplexProperty))
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        private class EntityType
        {
            public int ID
            {
                get;
                set;
            }
            public bool Updated
            {
                get;
                set;
            }
            public string PrimitiveProperty
            {
                get;
                set;
            }
            public ComplexType ComplexProperty
            {
                get;
                set;
            }
            public ICollection<string> PrimitiveCollectionProperty
            {
                get;
                set;
            }
            public ICollection<ComplexType> ComplexCollectionProperty
            {
                get;
                set;
            }
            public EntityType ResourceReferenceProperty
            {
                get;
                set;
            }
            public ICollection<EntityType> ResourceSetReferenceProperty
            {
                get;
                set;
            }
            public EntityType2 ResourceReferenceProperty2
            {
                get;
                set;
            }
            public ICollection<EntityType2> ResourceSetReferenceProperty2
            {
                get;
                set;
            }
            public override int GetHashCode()
            {
                return 47 * this.ID.GetHashCode() * this.Updated.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                EntityType other = obj as EntityType;
                if (other == null)
                {
                    return false;
                }

                if (this.ID != other.ID)
                {
                    return false;
                }

                if (this.Updated != other.Updated)
                {
                    return false;
                }

                return true;
            }

        }

        private class EntityType2
        {
            public int ID
            {
                get;
                set;
            }
            public override bool Equals(object obj)
            {
                EntityType2 other = obj as EntityType2;
                if (other == null)
                {
                    return false;
                }

                return this.ID == other.ID;
            }
            public override int GetHashCode()
            {
                return 17 * this.ID.GetHashCode();
            }
        }

        private static DSPServiceDefinition ModelWithActions()
        {
            entityInstances = new List<EntityType>();
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var complexType = metadata.AddComplexType("ComplexType", null, null, false);
            metadata.AddPrimitiveProperty(complexType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexType, "ComplexProperty", complexType);

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType, "Updated", typeof(bool));
            metadata.AddPrimitiveProperty(entityType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);
            metadata.AddCollectionProperty(entityType, "PrimitiveCollectionProperty", Provider.ResourceType.GetPrimitiveResourceType(typeof(string)));
            metadata.AddCollectionProperty(entityType, "ComplexCollectionProperty", complexType);
            var resourceReferenceProperty = metadata.AddResourceReferenceProperty(entityType, "ResourceReferenceProperty", entityType);
            var resourceSetReferenceProperty = metadata.AddResourceSetReferenceProperty(entityType, "ResourceSetReferenceProperty", entityType);

            var entityType2 = metadata.AddEntityType("EntityType2", null, null, false);
            metadata.AddKeyProperty(entityType2, "ID", typeof(int));

            var resourceReferenceProperty2 = metadata.AddResourceReferenceProperty(entityType, "ResourceReferenceProperty2", entityType2);
            var resourceSetReferenceProperty2 = metadata.AddResourceSetReferenceProperty(entityType, "ResourceSetReferenceProperty2", entityType2);

            var set = metadata.AddResourceSet("Set", entityType);
            var set2 = metadata.AddResourceSet("Set2", entityType2);

            metadata.AddResourceAssociationSet(new Provider.ResourceAssociationSet(
                "ResourceReference",
                new Provider.ResourceAssociationSetEnd(set, entityType, resourceReferenceProperty),
                new Provider.ResourceAssociationSetEnd(set, entityType, null)));

            metadata.AddResourceAssociationSet(new Provider.ResourceAssociationSet(
                "ResourceSetReference",
                new Provider.ResourceAssociationSetEnd(set, entityType, resourceSetReferenceProperty),
                new Provider.ResourceAssociationSetEnd(set, entityType, null)));

            metadata.AddResourceAssociationSet(new Provider.ResourceAssociationSet(
                "ResourceReference2",
                new Provider.ResourceAssociationSetEnd(set, entityType, resourceReferenceProperty2),
                new Provider.ResourceAssociationSetEnd(set2, entityType2, null)));

            metadata.AddResourceAssociationSet(new Provider.ResourceAssociationSet(
                "ResourceSetReference2",
                new Provider.ResourceAssociationSetEnd(set, entityType, resourceSetReferenceProperty2),
                new Provider.ResourceAssociationSetEnd(set2, entityType2, null)));

            DSPContext context = new DSPContext();
            metadata.SetReadOnly();

            DSPActionProvider actionProvider = new DSPActionProvider();
            ClientActionTests.SetupActions(actionProvider, new ActionContext(context));

            DSPResource complex1 = new DSPResource(complexType);
            complex1.SetValue("PrimitiveProperty", "complex1");
            ComplexType clientComplex1 = new ComplexType() { PrimitiveProperty = "complex1" };
            DSPResource complex2 = new DSPResource(complexType);
            complex2.SetValue("PrimitiveProperty", "complex2");
            complex2.SetValue("ComplexProperty", complex1);
            ComplexType clientComplex2 = new ComplexType() { PrimitiveProperty = "complex2", ComplexProperty = clientComplex1 };

            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("Updated", false);
            entity1.SetValue("PrimitiveProperty", "entity1");
            entity1.SetValue("ComplexProperty", complex2);
            entity1.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity1.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });
            EntityType clientEntity1 = new EntityType()
            {
                ID = 1,
                Updated = false,
                PrimitiveProperty = "entity1",
                ComplexProperty = clientComplex2,
                PrimitiveCollectionProperty = new string[] { "value1", "value2" },
                ComplexCollectionProperty = new[] { clientComplex1, clientComplex2 }
            };

            DSPResource entity2 = new DSPResource(entityType);
            entity2.SetValue("ID", 2);
            entity2.SetValue("Updated", false);
            entity2.SetValue("PrimitiveProperty", "entity2");
            entity2.SetValue("ComplexProperty", complex2);
            entity2.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity2.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });
            EntityType clientEntity2 = new EntityType()
            {
                ID = 2,
                Updated = false,
                PrimitiveProperty = "entity2",
                ComplexProperty = clientComplex2,
                PrimitiveCollectionProperty = new string[] { "value1", "value2" },
                ComplexCollectionProperty = new[] { clientComplex1, clientComplex2 }
            };

            DSPResource entity3 = new DSPResource(entityType);
            entity3.SetValue("ID", 3);
            entity3.SetValue("Updated", false);
            entity3.SetValue("PrimitiveProperty", "entity3");
            entity3.SetValue("ComplexProperty", complex2);
            entity3.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity3.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });
            EntityType clientEntity3 = new EntityType()
            {
                ID = 3,
                Updated = false,
                PrimitiveProperty = "entity3",
                ComplexProperty = clientComplex2,
                PrimitiveCollectionProperty = new string[] { "value1", "value2" },
                ComplexCollectionProperty = new[] { clientComplex1, clientComplex2 }
            };

            entity1.SetValue("ResourceReferenceProperty", entity2);
            clientEntity1.ResourceReferenceProperty = clientEntity2;
            entity2.SetValue("ResourceReferenceProperty", entity3);
            clientEntity2.ResourceReferenceProperty = clientEntity3;
            entity3.SetValue("ResourceReferenceProperty", entity1);
            clientEntity3.ResourceReferenceProperty = clientEntity1;

            entity1.SetValue("ResourceSetReferenceProperty", new[] { entity2, entity3 });
            clientEntity1.ResourceSetReferenceProperty = new[] { clientEntity2, clientEntity3 };
            entity2.SetValue("ResourceSetReferenceProperty", new[] { entity1, entity3 });
            clientEntity2.ResourceSetReferenceProperty = new[] { clientEntity1, clientEntity3 };
            entity3.SetValue("ResourceSetReferenceProperty", new[] { entity1, entity2 });
            clientEntity3.ResourceSetReferenceProperty = new[] { clientEntity1, clientEntity2 };

            entity1.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            clientEntity1.ResourceSetReferenceProperty2 = new EntityType2[0];
            entity2.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            clientEntity2.ResourceSetReferenceProperty2 = new EntityType2[0];
            entity3.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            clientEntity3.ResourceSetReferenceProperty2 = new EntityType2[0];

            context.GetResourceSetEntities("Set").Add(entity1);
            entityInstances.Add(clientEntity1);
            context.GetResourceSetEntities("Set").Add(entity2);
            entityInstances.Add(clientEntity2);
            context.GetResourceSetEntities("Set").Add(entity3);
            entityInstances.Add(clientEntity3);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true, MaxProtocolVersion = ODataProtocolVersion.V4 },
            };

            return service;
        }

        private static MethodInfo[] GetActionsFromContextType()
        {
            return typeof(ActionContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Where(m => m.GetCustomAttributes(typeof(DSPActionAttribute), true).FirstOrDefault() != null).ToArray();
        }

        private static void SetupActions(DSPActionProvider actionProvider, ActionContext context)
        {
            foreach (MethodInfo method in ClientActionTests.GetActionsFromContextType())
            {
                actionProvider.AddAction(method, method.IsStatic ? null : context);
            }
        }

        private class ActionContext
        {
            private DSPContext context;

            public ActionContext(DSPContext context)
            {
                this.context = context;
            }

            #region Top Level Actions

            /* 
             * Actions on the root work are the same as an action on the first entity except for
             * TopLevelAction_EntityCollection and TopLevelAction_EntityQueryable which get the entire 
             * entity set.
             */

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never)]
            public void TopLevelAction_Void()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never)]
            public string TopLevelAction_Primitive()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (string)resource.GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public DSPResource TopLevelAction_Complex()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ComplexProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never)]
            public IEnumerable<string> TopLevelAction_PrimitiveCollection()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (IEnumerable<string>)resource.GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public IEnumerable<DSPResource> TopLevelAction_ComplexCollection()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (IEnumerable<DSPResource>)resource.GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public DSPResource TopLevelAction_Entity()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return resource;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IEnumerable<DSPResource> TopLevelAction_EntityCollection()
            {
                List<object> resources = this.context.GetResourceSetEntities("Set");
                List<DSPResource> enumerableResources = new List<DSPResource>();
                foreach (object obj in resources)
                {
                    DSPResource resource = (DSPResource)obj;
                    resource.SetValue("Updated", true);
                    enumerableResources.Add(resource);
                }
                return enumerableResources;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IQueryable<DSPResource> TopLevelAction_EntityQueryable()
            {
                return this.TopLevelAction_EntityCollection().AsQueryable();
            }

            #endregion Top Level Actions

            #region Action on single entity

            /*
             *  Actions on a single entity operation. Gets the corresponding property from the binded entity.
             */

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnSingleEntity_Void(DSPResource resource)
            {
                resource.SetValue("Updated", true);
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnSingleEntity_Primitive(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (string)resource.GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnSingleEntity_Complex(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ComplexProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnSingleEntity_PrimitiveCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (IEnumerable<string>)resource.GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnSingleEntity_ComplexCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (IEnumerable<DSPResource>)resource.GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnSingleEntity_Entity(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ResourceReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnSingleEntity_EntityCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (IEnumerable<DSPResource>)resource.GetValue("ResourceSetReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnSingleEntity_EntityQueryable(DSPResource resource)
            {
                return ActionOnSingleEntity_EntityCollection(resource).AsQueryable();
            }

            #endregion Action on single entity

            #region Action on entity collection

            /*
             * Does the same thing as the equivalent action on a single entity for the First on the set given,
             * but also set's Updated to true for all resources.
             */

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnEntityCollection_Void(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnEntityCollection_Primitive(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (string)resources.First().GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityCollection_Complex(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ComplexProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnEntityCollection_PrimitiveCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<string>)((DSPResource)resources.First()).GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollection_ComplexCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)((DSPResource)resources.First()).GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityCollection_Entity(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ResourceReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollection_EntityCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)resources.First().GetValue("ResourceSetReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnEntityCollection_EntityQueryable(IEnumerable<DSPResource> resources)
            {
                return ActionOnEntityCollection_EntityCollection(resources).AsQueryable();
            }

            #endregion Action on entity collection

            #region Action on entity queryable

            /*
             * Actions on queryable sets behavior identically  to actions on entity collections.
             */

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnEntityQueryable_Void(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnEntityQueryable_Primitive(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (string)resources.First().GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityQueryable_Complex(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ComplexProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnEntityQueryable_PrimitiveCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<string>)((DSPResource)resources.First()).GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityQueryable_ComplexCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)((DSPResource)resources.First()).GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityQueryable_Entity(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ResourceReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityQueryable_EntityCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)resources.First().GetValue("ResourceSetReferenceProperty");
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnEntityQueryable_EntityQueryable(IQueryable<DSPResource> resources)
            {
                return ActionOnEntityQueryable_EntityCollection(resources).AsQueryable();
            }

            #endregion Action on entity queryable

            #region Bindable Action with single parameter

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.Byte" })]
            public static int ActionOnEntityCollectionWithParam_PrimitiveByteArray(IQueryable<DSPResource> resource, byte[] value)
            {
                int res = 0;
                for (int i = 0; i < value.Length; ++i)
                {
                    res += (int)value[i];
                }
                return res;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "System.Int32" })]
            public static int TopLevelAction_WithParam_Primitive(int value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "System.Nullable`1[System.Int32]" })]
            public static void TopLevelAction_WithParam_NullablePrimitive(int? value)
            {
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static string ActionOnEntityCollectionWithParam_Primitive(IQueryable<DSPResource> resource, string value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static IEnumerable<string> ActionOnEntityCollectionWithParam_PrimitiveCollection(IQueryable<DSPResource> resource, IEnumerable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static IQueryable<string> ActionOnEntityCollectionWithParam_PrimitiveQueryable(IQueryable<DSPResource> resource, IQueryable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static DSPResource ActionOnEntityCollectionWithParam_Complex(IQueryable<DSPResource> resource, DSPResource value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollectionWithParam_ComplexCollection(IQueryable<DSPResource> resource, IEnumerable<DSPResource> value)
            {
                return value;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static IQueryable<DSPResource> ActionOnEntityCollectionWithParam_ComplexQueryable(IQueryable<DSPResource> resource, IQueryable<DSPResource> value)
            {
                return value;
            }

            #endregion

            #region Bindable Action with Two Parameters
            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String", "System.Int32" })]
            public static string ActionOnEntityCollectionWithParam_Primitive_Primitive(IQueryable<DSPResource> resource, string value1, int value2)
            {
                return value1 + value2;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String", "System.Int32" })]
            public static IEnumerable<string> ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive(IQueryable<DSPResource> resource, IEnumerable<string> value1, int value2)
            {
                foreach (var v in value1)
                {
                    yield return v + value2;
                }
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.Int32", "System.String" })]
            public static IQueryable<string> ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable(IQueryable<DSPResource> resource, int value1, IQueryable<string> value2)
            {
                if (value1 == 0)
                {
                    throw new DataServiceException(400, "value1 must not be 0");
                }

                return value2;
            }

            [DSPActionAttribute(Provider.OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static DSPResource ActionOnEntityCollectionWithParam_Complex_Complex(IQueryable<DSPResource> resource, DSPResource value1, DSPResource value2)
            {
                if (value1 == null)
                {
                    throw new DataServiceException(400, "value1 cannot be null.");
                }

                if (value2 == null)
                {
                    throw new DataServiceException(400, "value2 cannot be null.");
                }

                return value1;
            }

            #endregion Bindable Action with Two Parameters
        }

        #endregion

        #region Success Test Cases

        private PositiveTestCase[] positiveTestCases = new PositiveTestCase[]
            {
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_Void",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute(ctx, uri, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_Primitive",
                    ExpectedResults = new object[] { entityInstances.First().PrimitiveProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_Complex",
                    ExpectedResults = new object[] { entityInstances.First().ComplexProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_PrimitiveCollection",
                    ExpectedResults = entityInstances.First().PrimitiveCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_ComplexCollection",
                    ExpectedResults = entityInstances.First().ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_Entity",
                    ExpectedResults = new object[] { entityInstances.First() },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_Entity",
                    ExpectedResults = new object[] { entityInstances.First() },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<EntityType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_EntityCollection",
                    ExpectedResults = entityInstances,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach(EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_EntityCollection",
                    ExpectedResults = entityInstances,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach(EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_EntityQueryable",
                    ExpectedResults = entityInstances.AsQueryable(),
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach(EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/TopLevelAction_EntityQueryable",
                    ExpectedResults = entityInstances.AsQueryable(),
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach(EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute(ctx, uri, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive",
                    ExpectedResults = new object[] { entityInstances[1].PrimitiveProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<string>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex",
                    ExpectedResults = new object[] { entityInstances[1].ComplexProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection",
                    ExpectedResults = entityInstances[1].PrimitiveCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<string>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection",
                    ExpectedResults = entityInstances[1].ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(void),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity",
                    ExpectedResults = new object[] { entityInstances[1].ResourceReferenceProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection",
                    ExpectedResults = entityInstances[1].ResourceSetReferenceProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable",
                    ExpectedResults = entityInstances[1].ResourceSetReferenceProperty.AsQueryable(),
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        entityInstances[1].Updated = true;
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute(ctx, uri, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Primitive",
                    ExpectedResults = new object[] { entityInstances.First().PrimitiveProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach(EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<string>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Complex",
                    ExpectedResults = new object[] { entityInstances.First().ComplexProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<ComplexType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_PrimitiveCollection",
                    ExpectedResults = entityInstances.First().PrimitiveCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<string>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_ComplexCollection",
                    ExpectedResults = entityInstances.First().ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<ComplexType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Entity",
                    ExpectedResults = new object[] { entityInstances.First().ResourceReferenceProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityCollection",
                    ExpectedResults = entityInstances.First().ResourceSetReferenceProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityQueryable",
                    ExpectedResults = entityInstances.First().ResourceSetReferenceProperty.AsQueryable(),
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute(ctx, uri, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive",
                    ExpectedResults = new object[] { entityInstances.First().PrimitiveProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<string>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Complex",
                    ExpectedResults = new object[] { entityInstances.First().ComplexProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<ComplexType>(ctx, uri, true, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_PrimitiveCollection",
                    ExpectedResults = entityInstances.First().PrimitiveCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<string>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_ComplexCollection",
                    ExpectedResults = entityInstances.First().ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<ComplexType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity",
                    ExpectedResults = new object[] { entityInstances.First().ResourceReferenceProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityCollection",
                    ExpectedResults = entityInstances.First().ResourceSetReferenceProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
                new PositiveTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable",
                    ExpectedResults = entityInstances.First().ResourceSetReferenceProperty.AsQueryable(),
                    StatusCode = 200,
                    ExpectedReturnType = typeof(EntityType),
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        foreach (EntityType e in entityInstances)
                        {
                            e.Updated = true;
                        }
                        return MyExecute<EntityType>(ctx, uri, false, isAsync);
                    },
                },
            };

        #endregion

        #region Server Error Test Cases

        private ServerErrorTestCase[] serverErrorTestCases = new ServerErrorTestCase[]
            {
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_WithParam_Primitive",
                    ErrorMsg = "A null value was found for the property named 'value', which has the expected type 'Edm.Int32[Nullable=False]'. The expected type 'Edm.Int32[Nullable=False]' does not allow null values.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) =>
                    {
                        return MyExecute(ctx, uri, isAsync,
                        new OperationParameter[]
                        {
                            new BodyOperationParameter("value", null)
                        });
                    },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void(1)",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void' cannot include key predicates, however it may end with empty parenthesis.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Complex(1)",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Complex' cannot include key predicates, however it may end with empty parenthesis.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<ComplexType>(ctx, uri, true, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_PrimitiveCollection(1)",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_PrimitiveCollection' cannot include key predicates, however it may end with empty parenthesis.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<string>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityCollection(1)",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityCollection' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable(1)",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity/$ref",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 404,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ResourceReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity/$ref",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 404,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable/$count",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive/$value",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<string>(ctx, uri, true, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity/ResourceSetReferenceProperty/$ref",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<EntityType>(ctx, uri, false, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ResourceReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    ErrorMsg = "The binding parameter for 'ActionOnEntityCollection_Void' is not assignable from the result of the uri segment 'ResourceReferenceProperty'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ResourceSetReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ResourceSetReferenceProperty'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/PrimitiveProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMsg = "The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void' in the request URI is not valid. The segment 'PrimitiveProperty' refers to a primitive property, function, or service operation, so the only supported value from the next segment is '$value'.",
                    StatusCode = 404,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ComplexProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ComplexProperty'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/PrimitiveCollectionProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    ErrorMsg = "Found an operation bound to a non-entity type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ComplexCollectionProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    ErrorMsg = "Found an operation bound to a non-entity type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ResourceReferenceProperty2/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ResourceReferenceProperty2'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/ResourceSetReferenceProperty2/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    ErrorMsg = "The binding parameter for 'ActionOnEntityCollection_Void' is not assignable from the result of the uri segment 'ResourceSetReferenceProperty2'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/TopLevelAction_Void()",
                    ErrorMsg = "The request URI is not valid. Since the segment 'Set' refers to a collection, this must be the last segment in the request URI or it must be followed by an function or action that can be bound to it otherwise all intermediate segments must refer to a single resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/TopLevelAction_Void()",
                    ErrorMsg = "Resource not found for the segment 'TopLevelAction_Void'.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()/Foo",
                    ErrorMsg = "The request URI is not valid. The segment 'TopLevelAction_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void()/Foo",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void()/Foo",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void()/Foo",
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$expand=ResourceReferenceProperty",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$filter=PrimitiveProperty eq \"foo\"",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$orderby=PrimitiveProperty",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$skip=1",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$top=1",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$count=true",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$select=ResourceReferenceProperty",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
                new ServerErrorTestCase() {
                    RequestUri = "/TopLevelAction_Void()?$skiptoken=foo",
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute(ctx, uri, isAsync); },
                },
            };

        #endregion

        #region Client Error Test Cases

        // These error messages aren't ideal, but Execute() with the bool will only be around until ODataLib is integrated into the client. 
        private ClientErrorTestCase[] clientErrorTestCases = new ClientErrorTestCase[]
            {
                // singleResult param is false for primitive
                new ClientErrorTestCase() {
                    RequestUri = "/TopLevelAction_Primitive",
                    AtomErrorMsg = "",
                    JsonLightErrorMsg = string.Format(CultureInfo.InvariantCulture, ODataLibResourceUtil.GetString("ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType"), "<serviceRoot>/$metadata#Edm.String", "Edm.String", "Collection(Edm.String)"),
                    StatusCode = 200,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<string>(ctx, uri, false, isAsync); },
                },
                // singleResult param is false for complex
                new ClientErrorTestCase() {
                    RequestUri = "/TopLevelAction_Complex",
                    AtomErrorMsg = "",
                    JsonLightErrorMsg = string.Format(CultureInfo.InvariantCulture, ODataLibResourceUtil.GetString("ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType"), "<serviceRoot>/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType", "AstoriaUnitTests.Tests.Actions.ComplexType", "Collection(AstoriaUnitTests.Tests.Actions.ComplexType)"),
                    StatusCode = 200,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<ComplexType>(ctx, uri, false, isAsync); },
                },
                // singleResult param is true for primitive collection
                new ClientErrorTestCase() {
                    RequestUri = "/TopLevelAction_PrimitiveCollection",
                    AtomErrorMsg = "An XML node of type 'Element' was found in a string value.",
                    JsonLightErrorMsg = string.Format(CultureInfo.InvariantCulture, ODataLibResourceUtil.GetString("ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType"), "<serviceRoot>/$metadata#Collection(Edm.String)", "Collection(Edm.String)", "Edm.String"),
                    StatusCode = 200,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<string>(ctx, uri, true, isAsync); },
                },
                // singleResult param is true for complex collection
                new ClientErrorTestCase() {
                    RequestUri = "/TopLevelAction_ComplexCollection",
                    AtomErrorMsg = "The property 'element' does not exist on type 'AstoriaUnitTests.ClientActionTests_ComplexType'.",
                    JsonLightErrorMsg = string.Format(CultureInfo.InvariantCulture, ODataLibResourceUtil.GetString("ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType"), "<serviceRoot>/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)", "Collection(AstoriaUnitTests.Tests.Actions.ComplexType)", "AstoriaUnitTests.Tests.Actions.ComplexType"),
                    StatusCode = 200,
                    ExecuteMethod = (ctx, uri, isAsync) => { return MyExecute<ComplexType>(ctx, uri, true, isAsync); },
                },
            };

        #endregion

        #region Test Methods

        [TestMethod]
        public void ExecuteActionSuccessTestsWithParameters()
        {
            // Use Execute to invoke service actions with parameters from the client. Success cases.
            #region Testcases
            PositiveTestCase[] positiveTestCases = new PositiveTestCase[]
            {
                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveByteArray",
                    ExpectedResults = new object[] { 3 },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(int),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", new byte[] { 0x01, 0x02 } )},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<int>(ctx, uri, true, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive",
                    ExpectedResults = new object[] { "first" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", "first")},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, true, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection",
                    ExpectedResults = new object[] { "first", "second" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", new List<string>() { "first", "second" })},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, false, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveQueryable",
                    ExpectedResults = new object[] { "first", "second" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", new List<string>() {"first", "second"} )},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, false, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Complex",
                    ExpectedResults = new object[] {new ComplexType() { PrimitiveProperty = "complex1" }},
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", new ComplexType() { PrimitiveProperty = "complex1" })},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, true, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection",
                    ExpectedResults = entityInstances.First().ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", entityInstances.First().ComplexCollectionProperty )},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, false /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexQueryable",
                    ExpectedResults = entityInstances.First().ComplexCollectionProperty,
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value", entityInstances.First().ComplexCollectionProperty )},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, false /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_Primitive",
                    ExpectedResults = new object[] { "first5" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value1", "first"), new BodyOperationParameter("value2", 5)},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, true /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    ExpectedResults = new object[] { "first5", "second5" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value1", new List<string>() { "first", "second" }), new BodyOperationParameter("value2", 5)},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, false /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable",
                    ExpectedResults = new object[] { "first", "second" },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(string),
                    OperationParameters = new OperationParameter[] { new BodyOperationParameter("value1", 5), new BodyOperationParameter("value2", new List<string>() { "first", "second" })},
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<string>(ctx, uri, false /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Complex_Complex",
                    ExpectedResults = new object[] { entityInstances.First().ComplexProperty },
                    StatusCode = 200,
                    ExpectedReturnType = typeof(ComplexType),
                    OperationParameters = new OperationParameter[]
                    {
                        new BodyOperationParameter("value1", entityInstances.First().ComplexProperty),
                        new BodyOperationParameter("value2", entityInstances.First().ComplexProperty)
                    },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        entityInstances.First().Updated = true;
                        return MyExecute<ComplexType>(ctx, uri, true /*singleResult*/, isAsync, operationParameters);
                    },
                },

                new PositiveTestCase()
                {
                    RequestUri = "/TopLevelAction_WithParam_NullablePrimitive",
                    ExpectedResults = new object[] {  },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    OperationParameters = new OperationParameter[]
                    {
                        new BodyOperationParameter("value", null)
                    },
                    ExecuteMethodWithParams = (ctx, uri, isAsync, operationParameters) =>
                    {
                        return MyExecute(ctx, uri, isAsync, operationParameters);
                    },
                },
            };
            #endregion

            List<string> failureUris = new List<string>();
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                IEdmModel serverModel = ResolveModelFromMetadataUri(new Uri(request.ServiceRoot + "/$metadata"));

                foreach (bool isAsync in this.AsyncOptions)
                {
                    t.TestUtil.RunCombinations(positiveTestCases, new[] { true }, (testCase, useJsonLight) =>
                    {
                        try
                        {
                            ResetUpdateFlags(service);
                            DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                            if (useJsonLight)
                            {
                                ctx.Format.UseJson(serverModel);
                                ctx.ResolveType = this.ResolveClientTypeFromServerName;
                                ctx.ResolveName = this.ResolveServerNameFromClientType;
                            }

                            Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);

                            OperationResponse operationResponse = testCase.ExecuteMethodWithParams.Invoke(ctx, uri, isAsync, testCase.OperationParameters);

                            Assert.IsNotNull(operationResponse);
                            QueryOperationResponse queryOperationResponse = operationResponse as QueryOperationResponse;

                            Assert.IsNotNull(queryOperationResponse);
                            Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                            Assert.IsNull(operationResponse.Error);

                            IEnumerator actualEnumerator = queryOperationResponse.GetEnumerator();
                            IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                            while (expectedEnumerator.MoveNext())
                            {
                                Assert.IsTrue(actualEnumerator.MoveNext());
                                Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                            }
                        }
                        catch (Exception)
                        {
                            failureUris.Add(testCase.RequestUri);
                            throw;
                        }
                    });
                }
            }
        }

        private static IEdmModel ResolveModelFromMetadataUri(Uri metadataDocumentUri)
        {
            IEdmModel serviceModel;
            IEnumerable<EdmError> errors;
            if (!CsdlReader.TryParse(XmlReader.Create(metadataDocumentUri.OriginalString), out serviceModel, out errors))
            {
                Assert.Fail(string.Join(Environment.NewLine, errors.Select(e => e.ToString())));
            }

            return serviceModel;
        }

        private Type ResolveClientTypeFromServerName(string typeName)
        {
            switch (typeName)
            {
                case "AstoriaUnitTests.Tests.Actions.ComplexType":
                    return typeof(AstoriaUnitTests.ClientActionTests.ComplexType);

                case "AstoriaUnitTests.Tests.Actions.EntityType":
                    return typeof(AstoriaUnitTests.ClientActionTests.EntityType);

                case "AstoriaUnitTests.Tests.Actions.EntityType2":
                    return typeof(AstoriaUnitTests.ClientActionTests.EntityType2);

                default:
                    throw new InvalidOperationException("Unrecognized type name:" + typeName);
            }
        }

        private string ResolveServerNameFromClientType(Type type)
        {
            if (type == typeof(AstoriaUnitTests.ClientActionTests.ComplexType))
            {
                return "AstoriaUnitTests.Tests.Actions.ComplexType";
            }

            if (type == typeof(AstoriaUnitTests.ClientActionTests.EntityType))
            {
                return "AstoriaUnitTests.Tests.Actions.EntityType";
            }

            if (type == typeof(AstoriaUnitTests.ClientActionTests.EntityType2))
            {
                return "AstoriaUnitTests.Tests.Actions.EntityType2";
            }

            throw new InvalidOperationException("Unrecognized type:" + type.FullName);
        }

        [TestMethod]
        public void ExecuteActionSuccessTests()
        {
            // Use Execute to invoke service actions from the client. Success cases. Uses QueryOperationResponse without the type parameter.
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                IEdmModel serverModel = ResolveModelFromMetadataUri(new Uri(request.ServiceRoot + "/$metadata"));

                foreach (bool isAsync in this.AsyncOptions)
                {
                    t.TestUtil.RunCombinations(positiveTestCases, new[] { true }, (testCase, useJsonLight) =>
                    {
                        ResetUpdateFlags(service);
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        if (useJsonLight)
                        {
                            ctx.Format.UseJson(serverModel);
                            ctx.ResolveType = this.ResolveClientTypeFromServerName;
                            ctx.ResolveName = this.ResolveServerNameFromClientType;
                        }

                        Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);

                        OperationResponse operationResponse = testCase.ExecuteMethod.Invoke(ctx, uri, isAsync);

                        Assert.IsNotNull(operationResponse);
                        QueryOperationResponse queryOperationResponse = operationResponse as QueryOperationResponse;
                        Assert.IsNotNull(queryOperationResponse);
                        Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                        Assert.IsNull(operationResponse.Error);

                        IEnumerator actualEnumerator = queryOperationResponse.GetEnumerator();
                        IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                        while (expectedEnumerator.MoveNext())
                        {
                            Assert.IsTrue(actualEnumerator.MoveNext());
                            Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                        }
                    });
                }
            }
        }

        [TestMethod]
        public void ExecuteActionSuccessTests2()
        {
            // Use Execute to invoke service actions from the client. Success cases. Ensures behavior when QueryOperationResponse<T> (with the type parameter) is correct.
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                IEdmModel serverModel = ResolveModelFromMetadataUri(new Uri(request.ServiceRoot + "/$metadata"));

                foreach (bool isAsync in this.AsyncOptions)
                {
                    t.TestUtil.RunCombinations(positiveTestCases, new[] { true }, (testCase, useJsonLight) =>
                    {
                        ResetUpdateFlags(service);
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        if (useJsonLight)
                        {
                            ctx.Format.UseJson(serverModel);
                            ctx.ResolveType = this.ResolveClientTypeFromServerName;
                            ctx.ResolveName = this.ResolveServerNameFromClientType;
                        }

                        Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);

                        OperationResponse operationResponse = testCase.ExecuteMethod.Invoke(ctx, uri, isAsync);
                        Assert.IsNotNull(operationResponse);

                        if (testCase.ExpectedReturnType != typeof(void))
                        {
                            if (testCase.ExpectedReturnType == typeof(string))
                            {
                                QueryOperationResponse<string> queryOperationResponse = operationResponse as QueryOperationResponse<string>;

                                Assert.IsNotNull(queryOperationResponse);
                                Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                                Assert.IsNull(operationResponse.Error);

                                IEnumerator<string> actualEnumerator = queryOperationResponse.GetEnumerator();
                                IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                                while (expectedEnumerator.MoveNext())
                                {
                                    Assert.IsTrue(actualEnumerator.MoveNext());
                                    Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                                }
                            }
                            else if (testCase.ExpectedReturnType == typeof(ComplexType))
                            {
                                QueryOperationResponse<ComplexType> queryOperationResponse = operationResponse as QueryOperationResponse<ComplexType>;

                                Assert.IsNotNull(queryOperationResponse);
                                Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                                Assert.IsNull(operationResponse.Error);

                                IEnumerator<ComplexType> actualEnumerator = queryOperationResponse.GetEnumerator();
                                IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                                while (expectedEnumerator.MoveNext())
                                {
                                    Assert.IsTrue(actualEnumerator.MoveNext());
                                    Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                                }
                            }
                            else if (testCase.ExpectedReturnType == typeof(EntityType))
                            {
                                QueryOperationResponse<EntityType> queryOperationResponse = operationResponse as QueryOperationResponse<EntityType>;

                                Assert.IsNotNull(queryOperationResponse);
                                Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                                Assert.IsNull(operationResponse.Error);

                                IEnumerator<EntityType> actualEnumerator = queryOperationResponse.GetEnumerator();
                                IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                                while (expectedEnumerator.MoveNext())
                                {
                                    Assert.IsTrue(actualEnumerator.MoveNext());
                                    Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                                }
                            }
                        }
                        else
                        {
                            QueryOperationResponse queryOperationResponse = operationResponse as QueryOperationResponse;
                            Assert.IsNotNull(queryOperationResponse);
                            Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                            Assert.IsNull(operationResponse.Error);

                            IEnumerator actualEnumerator = queryOperationResponse.GetEnumerator();
                            IEnumerator expectedEnumerator = testCase.ExpectedResults.GetEnumerator();
                            while (expectedEnumerator.MoveNext())
                            {
                                Assert.IsTrue(actualEnumerator.MoveNext());
                                Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
                            }
                        }
                    });
                }
            }
        }

        [TestMethod]
        public void ExecuteActionServerErrorTests()
        {
            List<string> failingTestCaseUris = new List<string>();
            // Use Execute to invoke service actions from the client. Server error cases.
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                IEdmModel serverModel = ResolveModelFromMetadataUri(new Uri(request.ServiceRoot + "/$metadata"));

                foreach (bool isAsync in this.AsyncOptions)
                {
                    t.TestUtil.RunCombinations(serverErrorTestCases, new[] { true }, (testCase, useJsonLight) =>
                      {
                          try
                          {
                              ResetUpdateFlags(service);
                              DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                              if (useJsonLight)
                              {
                                  ctx.Format.UseJson(serverModel);
                                  ctx.ResolveType = this.ResolveClientTypeFromServerName;
                                  ctx.ResolveName = this.ResolveServerNameFromClientType;
                              }

                              Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);

                              Exception e = t.TestUtil.RunCatching(() => testCase.ExecuteMethod.Invoke(ctx, uri, isAsync));
                              Assert.IsNotNull(e);
                              t.TestUtil.AssertContains(e.InnerException.Message, testCase.ErrorMsg);
                              foreach (object o in service.CurrentDataSource.GetResourceSetEntities("Set"))
                              {
                                  Assert.IsFalse((Boolean)((DSPResource)o).GetValue("Updated"));
                              }
                          }
                          catch (Exception)
                          {
                              failingTestCaseUris.Add(testCase.RequestUri);
                              throw;
                          }
                      });
                }
            }
        }

        [TestMethod]
        public void ExecuteActionClientErrorTests()
        {
            // Use Execute to invoke service actions from the client. Cases for errors that occur on the client during materialization.
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                IEdmModel serverModel = ResolveModelFromMetadataUri(new Uri(request.ServiceRoot + "/$metadata"));

                foreach (bool isAsync in this.AsyncOptions)
                {
                    t.TestUtil.RunCombinations(clientErrorTestCases, new[] { true }, (testCase, useJsonLight) =>
                    {
                        ResetUpdateFlags(service);
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

                        string testCaseErrorMsg = testCase.JsonLightErrorMsg.Replace("<serviceRoot>", request.BaseUri);
                        ctx.Format.UseJson(serverModel);
                        ctx.ResolveType = this.ResolveClientTypeFromServerName;
                        ctx.ResolveName = this.ResolveServerNameFromClientType;


                        Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);

                        OperationResponse operationResponse = testCase.ExecuteMethod.Invoke(ctx, uri, isAsync);
                        Assert.IsNotNull(operationResponse);
                        QueryOperationResponse queryOperationResponse = operationResponse as QueryOperationResponse;
                        Assert.IsNotNull(queryOperationResponse);
                        Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                        Assert.IsNull(operationResponse.Error);

                        // All the error this is meant to test should occur during materialization
                        try
                        {
                            IEnumerator actualEnumerator = queryOperationResponse.GetEnumerator();
                            while (actualEnumerator.MoveNext())
                            {
                                object o = actualEnumerator.Current;
                            }
                            // Assert.Fail();
                        }
                        catch (Exception e)
                        {
                            //Assert.IsFalse(e is AssertFailedException);
                            t.TestUtil.AssertContains(e.Message, testCaseErrorMsg);
                        }
                    });
                }
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void TestClientEventsWithAction()
        {
            // Use Execute to invoke an actions from the client, and verifies that the client actions are fired appropriately.
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                ResetUpdateFlags(service);
                int buildingRequestCalls = 0;
                int sendingRequest2Calls = 0;
                int receivingResponseCalls = 0;

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                ctx.BuildingRequest += (sender, args) =>
                {
                    args.Descriptor.Should().BeNull("we shouldn't expose a descriptor on Execute action calls");
                    args.Headers.Add("CustomHeader", "Custom value");
                    buildingRequestCalls++;
                };
                ctx.SendingRequest2 += (sender, args) =>
                {
                    args.Descriptor.Should().BeNull("we shouldn't expose a descriptor on Execute action calls");
                    args.RequestMessage.Headers.Should().Contain(new KeyValuePair<string, string>("CustomHeader", "Custom value"));
                    sendingRequest2Calls++;
                };
                ctx.ReceivingResponse += (sender, args) =>
                {
                    args.ResponseMessage.Should().NotBeNull();
                    args.ResponseMessage.StatusCode.Should().Be(200);
                    args.IsBatchPart.Should().BeFalse();
                    receivingResponseCalls++;
                };

                Uri uri = new Uri(request.ServiceRoot + "/TopLevelAction_Entity");

                var operationResponse = (QueryOperationResponse<EntityType>)ctx.Execute<EntityType>(uri, "POST", true);
                operationResponse.Single();

                buildingRequestCalls.Should().Be(1);
                sendingRequest2Calls.Should().Be(1);
                receivingResponseCalls.Should().Be(1);
            }
        }

        #endregion
    }
}
