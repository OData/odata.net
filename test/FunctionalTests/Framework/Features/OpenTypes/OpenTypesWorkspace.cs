//---------------------------------------------------------------------
// <copyright file="OpenTypesWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class OpenTypesWorkspace : NonClrWorkspace
    {
        public OpenTypesWorkspace()
            : base("OpenTypes", "OpenTypes", "OpenTypesContainer")
        {
            this.Language = WorkspaceLanguage.CSharp;

            this.Settings.SkipDataPopulation = true;
            this.Settings.UseLazyPropertyLoading = false;

            this.SkipContentVerification = true;
            this.GenerateCallOrderInterceptors = true;

            this.GenerateNestedComplexTypes = true;

            //this.ServiceAdditionalCode += string.Join(Environment.NewLine, new string[] 
            //{
            //    "[WebGet]",
            //    "public void Toggle_IsOpenType(string resourceTypeName)",
            //    "{",
            //    "   Microsoft.OData.Service.Providers.ResourceType type;",
            //    "   if(this.CurrentDataSource.TryResolveResourceType(resourceTypeName, out type)",
            //    "   {",
            //    "       type.IsOpenType = !type.IsOpenType;",
            //    "   }",
            //    "}"
            //});
        }

        public bool GenerateNestedComplexTypes { get; set; }

        public override void PrepareRequest(AstoriaRequest request)
        {
            // do nothing, test cases will handle verification
        }

        protected internal override void PopulateClientTypes()
        {
            // do nothing
        }

        //public override void DefineClrProperties()
        //{
            
        //}

        public override void VerifyMetadata(string actualMetadata)
        {
            base.VerifyMetadata(actualMetadata);
        }

        public void OverrideIsOpenType(ResourceType type, bool value)
        {
            type.Facets.IsOpenType = value;
            RequestUtil.GetAndVerifyStatusCode(this, 
                this.ServiceUri + "/SetIsOpenType?resourceTypeName='" + this.ContextNamespace + "." + type.Name + "'&value=" + value.ToString().ToLowerInvariant(), 
                System.Net.HttpStatusCode.NoContent);
        }

        #region service container
        public override ServiceContainer ServiceContainer
        {
            set
            {
                _serviceContainer = value;
            }

            get
            {
                if (_serviceContainer == null)
                {
                    _serviceContainer = Resource.ServiceContainer(this, "OpenTypes");

                    // generate some complex types
                    ComplexType simpleComplexType = Resource.ComplexType("SimpleComplexType", this.ContextNamespace,
                        Resource.Property("Int", Clr.Types.Int32),
                        Resource.Property("String", Clr.Types.String, NodeFacet.MaxSize(15)),
                        Resource.Property("DateTime", Clr.Types.DateTime),
                        Resource.Property("Binary", Clr.Types.Binary, NodeFacet.MaxSize(15)),
                        Resource.Property("Decimal", Clr.Types.Decimal),
                        Resource.Property("Float", Clr.Types.Float));
                    _serviceContainer.AddNode(simpleComplexType);

                    if (this.GenerateNestedComplexTypes)
                    {
                        ComplexType nestedComplexType = Resource.ComplexType("NestedComplexType", this.ContextNamespace,
                            Resource.Property("Value", Clr.Types.Guid),
                            Resource.Property("Nested", Resource.ComplexType("MiddleNestedComplexType", this.ContextNamespace,
                                Resource.Property("Value", Clr.Types.Boolean),
                                Resource.Property("Nested", Resource.ComplexType("DeepNestedComplexType", this.ContextNamespace,
                                    Resource.Property("Value", simpleComplexType))))));
                        _serviceContainer.AddNode(nestedComplexType);
                    }

                    //ComplexType abstractComplexType = Resource.ComplexType("AbstractComplexType", this.ContextNamespace, NodeFacet.AbstractType(),
                    //    Resource.Property("Value", Clr.Types.Double));
                    //_serviceContainer.AddNode(abstractComplexType);

                    //ComplexType derivedComplexType = new ComplexType("DerivedComplexType", this.ContextNamespace, abstractComplexType,
                    //    Resource.Property("Value2", Clr.Types.Int64));

                    ResourceType closedAbstractBase = Resource.ResourceType("Mixed_ClosedAbstractBase", this.ContextNamespace,
                        NodeFacet.IsOpenType(false), NodeFacet.AbstractType(),
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()));

                    ResourceType closedBase = Resource.ResourceType("Mixed_ClosedBase", this.ContextNamespace, closedAbstractBase, NodeFacet.IsOpenType(false));
                    ResourceType openBase = Resource.ResourceType("Mixed_OpenBase", this.ContextNamespace, closedAbstractBase, NodeFacet.IsOpenType(true));
                    ResourceType openDerived = Resource.ResourceType("Mixed_OpenDerived", this.ContextNamespace, closedBase, NodeFacet.IsOpenType(true));

                    ResourceContainer mixedInheritanceSet = Resource.ResourceContainer("MixedInheritanceSet", closedBase, openBase, openDerived);
                    _serviceContainer.AddNode(mixedInheritanceSet);

                    ResourceType openAbstractBase = Resource.ResourceType("Open_AbstractBase", this.ContextNamespace,
                        NodeFacet.IsOpenType(true), NodeFacet.AbstractType(),
                        Resource.Property("Id", Clr.Types.Int32, Resource.Key()));

                    openBase = Resource.ResourceType("Open_Base", this.ContextNamespace, openAbstractBase, NodeFacet.IsOpenType(true));
                    openDerived = Resource.ResourceType("Open_Derived", this.ContextNamespace, openBase, NodeFacet.IsOpenType(true));

                    ResourceContainer openInheritanceSet = Resource.ResourceContainer("OpenInheritanceSet", openAbstractBase, openBase, openDerived);
                    _serviceContainer.AddNode(openInheritanceSet);

                    foreach (NodeType type in Clr.Types)
                    {
                        if (type == Clr.Types.Binary || type == Clr.Types.Boolean)
                            continue;

                        string typeName = type.Name;

                        ResourceType closed = Resource.ResourceType("SingleKey_Closed_" + typeName, this.ContextNamespace, NodeFacet.IsOpenType(false),
                            Resource.Property("Id", type, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                            Resource.Property("Simple", Clr.Types.Choose(), NodeFacet.MaxSize(15)),
                            Resource.Property("Complex", _serviceContainer.ComplexTypes.Choose()));

                        ResourceType derived = Resource.ResourceType("SingleKey_OpenDerived_" + typeName, this.ContextNamespace, closed, NodeFacet.IsOpenType(true));

                        ResourceContainer mixedSet = Resource.ResourceContainer("SingleKey_Mixed_" + typeName, closed, derived);
                        _serviceContainer.AddNode(mixedSet);

                        ResourceType open = Resource.ResourceType("SingleKey_Open_" + typeName, this.ContextNamespace, NodeFacet.IsOpenType(true),
                        Resource.Property("Id", type, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                        Resource.Property("Simple", Clr.Types.Choose(), NodeFacet.MaxSize(15)),
                        Resource.Property("Complex", _serviceContainer.ComplexTypes.Choose()));

                        ResourceContainer openSet = Resource.ResourceContainer("SingleKey_Open_" + typeName, open);
                        _serviceContainer.AddNode(openSet);
                    }

                    foreach (NodeType type in Clr.Types)
                    {
                        if (type == Clr.Types.Binary)
                            continue;

                        NodeType otherType = null;
                        while (otherType == null || otherType == Clr.Types.Binary || otherType == type)
                            otherType = Clr.Types.Choose();

                        string typeName = type.Name;
                        string otherTypeName = otherType.Name;

                        ResourceType closed = Resource.ResourceType("MultiKey_Closed_" + typeName + "_" + otherTypeName, this.ContextNamespace, NodeFacet.IsOpenType(false),
                            Resource.Property("Id1", type, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                            Resource.Property("Id2", otherType, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                            Resource.Property("Simple", Clr.Types.Choose(), NodeFacet.MaxSize(15)),
                            Resource.Property("Complex", _serviceContainer.ComplexTypes.Choose()));

                        ResourceType derived = Resource.ResourceType("MultiKey_OpenDerived_" + typeName + "_" + otherTypeName, this.ContextNamespace, closed, NodeFacet.IsOpenType(true));
                        ResourceContainer mixedSet = Resource.ResourceContainer("MultiKey_Mixed_" + typeName + "_" + otherTypeName, closed, derived);
                        _serviceContainer.AddNode(mixedSet);

                        ResourceType open = Resource.ResourceType("MultiKey_Open_" + typeName + "_" + otherTypeName, this.ContextNamespace, NodeFacet.IsOpenType(true),
                            Resource.Property("Id1", type, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                            Resource.Property("Id2", otherType, Resource.Key(), NodeFacet.MaxSize(15), NodeFacet.Precision(28), NodeFacet.Scale(4)),
                            Resource.Property("Simple", Clr.Types.Choose(), NodeFacet.MaxSize(15)),
                            Resource.Property("Complex", _serviceContainer.ComplexTypes.Choose()));

                        ResourceContainer openSet = Resource.ResourceContainer("MultiKey_Open_" + typeName + "_" + otherTypeName, open);
                        _serviceContainer.AddNode(openSet);
                    }

                    // add some nav props
                    foreach (ResourceContainer container in _serviceContainer.ResourceContainers)
                    {
                        ResourceContainer setForCollection = _serviceContainer.ResourceContainers.Where(rc => rc != container).Choose();
                        ResourceContainer setForReference = _serviceContainer.ResourceContainers.Where(rc => rc != container).Choose();

                        ResourceType type = container.BaseType;
                        ResourceType typeForCollection = setForCollection.BaseType;
                        ResourceType typeForReference = setForReference.BaseType;

                        ResourceAssociationEnd thisEnd = Resource.End(type.Name, type, Multiplicity.One);
                        ResourceAssociationEnd collectionEnd = Resource.End(typeForCollection.Name, typeForCollection, Multiplicity.Many);
                        ResourceAssociationEnd referenceEnd = Resource.End(typeForReference.Name, typeForReference, Multiplicity.One);

                        ResourceAssociation collection = Resource.Association(type.Name + "_" + typeForCollection.Name, thisEnd, collectionEnd);
                        ResourceAssociation reference = Resource.Association(type.Name + "_" + typeForReference.Name, thisEnd, referenceEnd);

                        //TODO: add back-references?
                        type.Properties.Add(Resource.Property("Collection_" + setForCollection.Name, Resource.Collection(typeForCollection), collection, thisEnd, collectionEnd));
                        type.Properties.Add(Resource.Property("Reference_" + setForReference.Name, typeForReference, reference, thisEnd, referenceEnd));

                        foreach (var derivedType in type.DerivedTypes)
                        {
                            type.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation).ToList().ForEach(p => derivedType.Properties.Add(p));
                        }
                    }

                    foreach (ResourceContainer rc in _serviceContainer.ResourceContainers)
                    {
                        foreach (ResourceType t in rc.ResourceTypes)
                        {
                            t.InferAssociations();
                        }
                    }
                }
                return _serviceContainer;
            }
        }
        #endregion
    }
}
