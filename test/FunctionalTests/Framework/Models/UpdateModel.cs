//---------------------------------------------------------------------
// <copyright file="UpdateModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using Microsoft.Test.KoKoMo;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.ModuleCore;
using Microsoft.OData.Client;
using System.Net;
using System.Reflection;

namespace System.Data.Test.Astoria
{
    using Microsoft.OData.Edm;

    ////////////////////////////////////////////////////////
    //  Update Model
    //
    ////////////////////////////////////////////////////////   

    public static class ModelExtensions
    {
        private static string GetRandomString(int maxLength)
        {
            Random randomizer = TestUtil.Random;
            System.Text.StringBuilder result = new System.Text.StringBuilder(maxLength);
            for (int index = 0; index < maxLength; index++)
            {
                result.Append((char)randomizer.Next(66, 91));
            }
            return result.ToString();
        }

        public static object CreateInstance(this ResourceType resourceType, bool setKeyValue)
        {
            if (resourceType.ClientClrType == null)
            {
                return null;
            }

            Random randomizer = TestUtil.Random;
            object resource = Activator.CreateInstance(resourceType.ClientClrType);
            foreach (NodeProperty property in resourceType.Properties)
            {

                if (!((ResourceProperty)property).IsNavigation
                    && !((ResourceProperty)property).IsComplexType
                    )
                {
                    if (property.PrimaryKey == null ||
                        property.PrimaryKey.Name != property.Name)
                    {
                        PropertyInfo reflectedProperty = resource.GetType().GetProperty(property.Name);

                        string propertyName = property.Name;
                        int? maxSizeFacet = property.Facets.MaxSize;
                        Type propertyType = property.GetSampleValue().GetType();
#if !ClientSKUFramework

                        if (propertyType != typeof(System.Data.Linq.Binary))
                        {
                            object propertyValue = propertyType.GetTypedValue(propertyName, maxSizeFacet);
                            reflectedProperty.SetValue(resource, propertyValue, null);
                        }
#endif

                    }
                }
                if (property.PrimaryKey != null)
                {
                    PropertyInfo reflectedProperty = resource.GetType().GetProperty(property.Name);
                    if (reflectedProperty.PropertyType == typeof(Int32))
                    {
                        reflectedProperty.SetValue(resource, randomizer.Next(10, 4000), null);
                    }

                }
            }
            return resource;
        }

        public static object GetTypedValue(this Type propertyType, string propertyName, int? maxSizeFacet)
        {
            AstoriaUnitTests.Data.TypeData dataForType = AstoriaUnitTests.Data.TypeData.FindForType(propertyType);

            object propertyValue = null;
            if (dataForType.ClrType == typeof(Decimal))
            {
                propertyValue = (decimal)2;
            }
            else if (dataForType.ClrType == typeof(DateTime) || (dataForType.ClrType == typeof(Nullable<DateTime>)))
            {
                if (propertyName == "BirthDate")
                {
                    propertyValue = new DateTime(1945, 1, 1);
                }
                else
                {
                    propertyValue = DateTime.Now.AddDays(1);
                }
            }
            else if (dataForType.ClrType == typeof(string))
            {
                string strValue = GetRandomString(maxSizeFacet != null ? maxSizeFacet.Value : 5);
#if !ClientSKUFramework

                strValue = System.Web.HttpUtility.HtmlEncode(strValue);
                if (maxSizeFacet != null)
                {
                    if (strValue.Length >= maxSizeFacet.Value)
                    {
                        strValue = strValue.Substring(0, maxSizeFacet.Value - 1);

                    }
                }
                propertyValue = strValue;
#endif
            }
            else
            {
                propertyValue = dataForType.SampleValues[1];

            }
            return propertyValue;
        }


        public static void EnsureInsert(DataServiceContext context, object entity, string entitySetName, Workspace workspace, string skipEntitySet)
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

                    }
                }
            }
#endif
        }

    }
    public static class NodeRelationExtension
    {
        public static ResourceType Source(this NodeRelation nodeRelation)
        {
            return ((ResourceAssociation)nodeRelation).Ends.FirstOrDefault().ResourceType;
        }
        public static ResourceType Target(this NodeRelation nodeRelation)
        {
            return ((ResourceAssociation)nodeRelation).Ends.LastOrDefault().ResourceType;
        }
        public static string GetAssociation(this ResourceType resourceType, ResourceType relatedType)//, Multiplicity associationMultiplicity)
        {
            string relation = "";
            foreach (NodeProperty property in resourceType.Properties)
            {

                if (((ResourceProperty)property).IsNavigation)
                {
                    if (property.Type is ResourceCollection)
                    {
                        if (((ResourceCollection)property.Type).SubType == relatedType)
                        {
                            relation = property.Name;
                            break;
                        }
                    }
                }
                if (property.Type == relatedType)
                {
                    relation = property.Name;
                    break;
                }

            }
            return relation;
        }
    }


    public class ClientModel : ClientModelBase
    {


        #region Model Variables
        protected WebDataCtxWrapper _context = null;
        protected object _target = null;
        protected Workspace _workspace = null;
        protected MergeOption _mergeOption = MergeOption.AppendOnly;

        protected Workspace workSpace
        {
            get
            {
                return this._workspace;
            }
            set
            {
                _workspace = value;
            }
        }

        [ModelVariable]
        protected override WebDataCtxWrapper Context
        {
            get
            {
                return this._context;
            }
            set
            {
                this._context = value;
            }
        }
        [ModelVariable]
        public long Timeout
        {
            get
            {
                return this.Engine.Options.Timeout / 7;
            }
        }
        #endregion

        public void DumpCode()
        {
            string traceDirectory = Path.Combine(Environment.CurrentDirectory, "ModelTraces");
            if (!Directory.Exists(traceDirectory))
            {
                Directory.CreateDirectory(traceDirectory);
            }
            string traceFile = Path.Combine(traceDirectory, DateTime.Now.Ticks.ToString() + ".cs");
            FileStream fs = new FileStream(traceFile, FileMode.CreateNew);
            fs.Write(System.Text.Encoding.UTF8.GetBytes(this.Context.CodeTrace)
                , 0
                ,
                System.Text.Encoding.UTF8.GetByteCount(this.Context.CodeTrace)
                );
            // this.Context.CodeTrace  
            fs.Close();
        }

        public ClientModel(Workspace workspace)
            : base(workspace)
        {
            this.workSpace = workspace;
        }
        public override ExpressionModel CreateModel()
        {
            return new ClientModel(this.workSpace);
        }

        [ModelAction(CallFirst = true)]
        public void CreateContext()
        {
            Context = new WebDataCtxWrapper(this.workSpace.ServiceRoot);
            Context.Credentials = CredentialCache.DefaultNetworkCredentials;
            AstoriaTestLog.WriteLineIgnore(this.workSpace.ServiceRoot.ToString());
        }

        [ModelAction(Weight = 10)]
        public void ModifyEntity()
        {
            string entitySetName = ""; object entity = null;
            ResourceContainer resContainer = null;
            while (resContainer == null)
            {
                entity = GetResource(out entitySetName, out resContainer);
            }
            UpdateModel updModel = new UpdateModel(workSpace, Context, entity, resContainer, null);
            updModel.RunningUnderLinkModel = true;
            ModelEngine engine = new ModelEngine();
            engine.Models.Add(updModel);
            engine.Options.Timeout = Timeout;
            engine.Run();
        }

        [ModelAction(Weight = 10)]
        public void Modify11Link()
        {
            AstoriaTestLog.WriteLine("Modify11Link");
            QueryResourceTypesByAssociationsForUpdate(Multiplicity.One, Multiplicity.One, false,
                       true, true,
                          delegate(ResourceContainer sourceContainer, ResourceType sourceResourceType,
                                                  ResourceContainer destinationContainer, ResourceType destinationType)
                          {
                              AstoriaTestLog.WriteLine("Source : {0} , Target :{1}", sourceContainer.Name, destinationContainer.Name);
                              ModelEngine engine = new ModelEngine();

                              LinkModel model = new LinkModel(sourceContainer,
                                  sourceResourceType,
                                  destinationContainer,
                                  destinationType,
                                  sourceResourceType.GetAssociation(destinationType), sourceContainer.Workspace, null
                                  );
                              model.RunningUnderScenarioModel = true;
                              model.Context = this.Context;
                              model.TargetMultiplicity = Multiplicity.Many;
                              engine.Models.Add(model);
                              engine.Options.Timeout = Timeout;
                              engine.Run();
                              return false;
                          });
        }


        [ModelAction(Weight = 10)]
        public void QueryEntities()
        {

            string entitySetName = ""; object entity = null;
            ResourceContainer resContainer = null;
            while (resContainer == null)
            {
                entity = GetResource(out entitySetName, out resContainer);
            }

            QueryModel queryModel = new QueryModel(workSpace, SerializationFormatKind.Atom, null);
            queryModel.RunningUnderScenarioModel = true;
            queryModel.ResContainer = resContainer;
            ModelEngine engine = new ModelEngine(queryModel);
            engine.Options.WeightScheme = WeightScheme.Custom;
            engine.Options.Timeout = Timeout;
            engine.Run();
            UriQueryBuilder ub = new UriQueryBuilder(workSpace, "");
            string ruri = ub.Build(queryModel.QueryResult);

            string uriRel = ruri.Substring(ruri.LastIndexOf(".svc/") + 5);

            if ((Context.BaseUri + "/" + uriRel).Length > 260) { return; }
            AstoriaTestLog.WriteLineIgnore(Context.BaseUri + "/" + uriRel);

            ResolveClientType(workSpace, Context, queryModel.ResultType);

            object enumerable = ExecuteQuery(Context, queryModel.ResultType, uriRel, "sync");

        }

        [ModelAction(Weight = 10)]
        public void Modify1NLink()
        {
            AstoriaTestLog.WriteLine("Modify1NLink");
            QueryResourceTypesByAssociationsForUpdate(Multiplicity.One, Multiplicity.Many, false,
                       true, true,
                          delegate(ResourceContainer sourceContainer, ResourceType sourceResourceType,
                                                  ResourceContainer destinationContainer, ResourceType destinationType)
                          {
                              //
                              ResourceType source = sourceResourceType.AssociationsOneToMany.First.Source.ResourceType;
                              if (!sourceContainer.ResourceTypes.Contains(source))
                              {
                                  //Do this if the source and target come out reversed
                                  ResourceContainer temp = sourceContainer;
                                  sourceContainer = destinationContainer;
                                  destinationContainer = temp;
                                  ResourceType resTemp = sourceResourceType;
                                  sourceResourceType = destinationType;
                                  destinationType = resTemp;
                              }
                              AstoriaTestLog.WriteLine("Source : {0} , Target :{1}", sourceContainer.Name, destinationContainer.Name);
                              ModelEngine engine = new ModelEngine();

                              LinkModel model = new LinkModel(sourceContainer,
                                  sourceResourceType,
                                  destinationContainer,
                                  destinationType,
                                  sourceResourceType.GetAssociation(destinationType), sourceContainer.Workspace, null
                                  );
                              model.Context = this.Context;
                              model.RunningUnderScenarioModel = true;
                              model.TargetMultiplicity = Multiplicity.One;
                              engine.Models.Add(model);
                              engine.Options.Timeout = Timeout;
                              engine.Run();
                              return false;
                          });
        }

        [ModelAction(Weight = 8)]
        [ModelParameter(Values = new object[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None, SaveChangesOptions.ContinueOnError, SaveChangesOptions.ReplaceOnUpdate }, Position = 0)]
        public void SaveChangesStep(SaveChangesOptions so)
        {
            base.CommitChanges(so);
        }

    }
    public class LinkModel : ExpressionModel
    {


        protected Workspace _workspace = null;
        protected object _source = null;
        protected WebDataCtxWrapper _context = null;
        protected object _target = null;
        protected MergeOption _mergeOption = MergeOption.AppendOnly;
        protected ResourceContainer _resContainer = null;
        protected ResourceType _resType = null;
        protected WebDataCtxWrapper _ctx = null;


        #region Model Variables
        [ModelVariable]
        public Workspace workSpace { get; set; }
        [ModelVariable]
        public WebDataCtxWrapper Context
        {
            get
            {
                if (_context == null && !RunningUnderScenarioModel)
                {

                    CreateContext();
                }
                return _context;
            }
            set
            {
                _context = value;
            }
        }
        [ModelVariable]
        public object Source
        {
            get
            {
                if (_source == null)
                {
                    _source = SourceResourceType.CreateInstance(false);
                }
                return _source;
            }
        }
        [ModelVariable]
        public EntityStates SourceState
        {
            get
            {
                EntityStates state = EntityStates.Detached;
                if (Context.Resources.Any(edw => edw.Entity == Source))
                {
                    state = Context.Resources.First(edw => edw.Entity == Source).State;
                }
                return state;
            }
        }
        [ModelVariable]
        public EntityStates TargetState
        {
            get
            {
                EntityStates state = EntityStates.Detached;
                if (Context.Resources.Any(edw => edw.Entity == Target))
                {
                    state = Context.Resources.First(edw => edw.Entity == Target).State;
                }
                return state;
            }
        }
        [ModelVariable]
        public ResourceContainer SourceContainer { get; set; }
        [ModelVariable]
        public ResourceContainer TargetContainer { get; set; }

        [ModelVariable]
        public bool RunningUnderScenarioModel { get; set; }

        [ModelVariable]
        public ResourceType SourceResourceType { get; set; }
        [ModelVariable]
        public ResourceType TargetResourceType { get; set; }
        [ModelVariable]
        public string AssociationName { get; set; }
        [ModelVariable]
        public Multiplicity TargetMultiplicity { get; set; }
        [ModelVariable]
        public object Target
        {
            get
            {
                if (_target == null)
                {
                    _target = TargetResourceType.CreateInstance(false);
                }
                return _target;
            }
        }
        [ModelVariable]
        public long UpdateModelDuration
        {
            get
            {
                return (this.Engine.Options.Timeout / 7);
            }
        }
        #endregion Model Variables

        public LinkModel(ResourceContainer sourceContainer,
            ResourceType sourceType,
            ResourceContainer targetContainer,
            ResourceType targetType,
            string associationName,
            Workspace workspace,
            LinkModel parent)
            : base(parent)
        {
            this.SourceContainer = sourceContainer;
            this.SourceResourceType = sourceType;
            this.TargetContainer = targetContainer;
            this.TargetResourceType = targetType;
            this.workSpace = workspace;
            this.AssociationName = associationName;
        }

        public override ExpressionModel CreateModel()
        {
            return new LinkModel(this.SourceContainer, this.SourceResourceType, this.TargetContainer,
            this.TargetResourceType, this.AssociationName, this.workSpace, this);
        }
        [ModelAction(CallFirst = true)]
        [ModelRequirement(Variable = "RunningUnderScenarioModel", Equal = false)]
        public void CreateContext()
        {

            _context = new WebDataCtxWrapper(this.workSpace.ServiceRoot);
            _context.Credentials = CredentialCache.DefaultNetworkCredentials;
            AstoriaTestLog.WriteLineIgnore(this.workSpace.ServiceRoot.ToString());
            //Source = SourceResourceType.CreateInstance( false );
            //Target = TargetResourceType.CreateInstance( false );

        }

        [ModelAction(Weight = 5)]
        public void ModifySource()
        {
            AstoriaTestLog.WriteLine("Modifying Source");
            ModifyEntity(true);
        }
        [ModelAction(Weight = 5)]
        public void ModifyTarget()
        {
            AstoriaTestLog.WriteLine("Modifying Target");
            ModifyEntity(false);
        }

        private void ModifyEntity(bool IsSource)
        {
            AstoriaTestLog.WriteLine("--Running Update Model");
            ModelEngine engine = new ModelEngine();
            engine.Options.Timeout = UpdateModelDuration;

            UpdateModel targetUpdate = null;
            if (IsSource)
            {
                targetUpdate = new UpdateModel(workSpace, Context, Source, SourceContainer, null);
            }
            else
            {
                targetUpdate = new UpdateModel(workSpace, Context, Target, TargetContainer, null);
            }
            engine.Models.Add(targetUpdate);
            engine.Run();
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "TargetMultiplicity", Equal = Multiplicity.One)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Detached)]
        public void SetLink()
        {
            AstoriaTestLog.WriteLine("context.SetLink {0} --> {1} --> {2}", SourceContainer.Name, AssociationName, TargetContainer.Name);
            Context.SetLink(Source, AssociationName, Target);
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "TargetMultiplicity", Equal = Multiplicity.One)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Detached)]
        public void SetLinkToNull()
        {
            AstoriaTestLog.WriteLine("context.SetLink {0} --> {1} --> NULL", SourceContainer.Name, AssociationName);
            Context.SetLink(Source, AssociationName, null);
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "TargetMultiplicity", Equal = Multiplicity.Many)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Added)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Added)]
        public void DeleteLink()
        {
            AstoriaTestLog.WriteLine("context.DeleteLink");
            Context.DeleteLink(Source, AssociationName, Target);
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "TargetMultiplicity", Equal = Multiplicity.Many)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Deleted)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Deleted)]
        public void AddLink()
        {
            try
            {
                AstoriaTestLog.WriteLine("context.AddLink {0} --> {1} --> {2}", SourceContainer.Name, AssociationName, TargetContainer.Name);
                Context.AddLink(Source, AssociationName, Target);
            }
            catch (InvalidOperationException ioException)
            {
                //this can be thrown because 
                //1. The relation is already being tracked by the context
                if (!this.Context.Links.Any(ldw => ldw.Source == Source && ldw.SourceProperty == AssociationName
                    && ldw.Target == Target)
                    )
                {
                    throw (ioException);
                }
            }
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "TargetMultiplicity", Equal = Multiplicity.Many)]
        [ModelRequirement(Variable = "SourceState", Not = EntityStates.Detached)]
        [ModelRequirement(Variable = "TargetState", Not = EntityStates.Detached)]
        public void DetachLink()
        {
            AstoriaTestLog.WriteLine("context.DetachLink");
            Context.DetachLink(Source, AssociationName, Target);

        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "SourceState", Equal = EntityState.Unchanged)]
        [ModelRequirement(Variable = "TargetState", Equal = EntityState.Unchanged)]

        public void AttachLink()
        {
            AstoriaTestLog.WriteLine("context.AttachLink");
            Context.AttachLink(Source, AssociationName, Target);
        }

        static int variationID = 3;
        [ModelAction(CallOnce = true)]
        [ModelRequirement(Variable = "RunningUnderScenarioModel", Equal = false)]
        [ModelParameter(Values = new object[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None, SaveChangesOptions.ContinueOnError, SaveChangesOptions.ReplaceOnUpdate }, Position = 0)]
        [ModelParameter(Values = new object[] { "sync", "async" }, Position = 1)]
        public virtual void SaveChanges(SaveChangesOptions so, string sync)
        {
            AstoriaTestLog.WriteLineIgnore("Save changes");
            DataServiceResponseWrapper response = null;

            try
            {
                switch (sync)
                {
                    case "sync":
                        response = this.Context.SaveChanges(so);
                        break;
                    case "async":
                        IAsyncResult async = this.Context.BeginSaveChanges(null, null);
                        response = this.Context.EndSaveChanges(async);
                        break;
                }
                //Save info on entities
                foreach (EntityDescriptorWrapper ed in Context.Resources)
                {
                    Uri _uri = null;
                    bool bURI = Context.TryGetUri(ed.Entity, out _uri);
                    //EntitiesAfter.Add( ed.Entity, new string[] { ed.State.ToString(), _uri == null ? "" : _uri.OriginalString } );
                }
                AstoriaTestLog.WriteVariationResult(true);
                //Verification
                //VerifySaveChanges( response );
                //VerifyData( this.Workspace, this.Entities, this.EntitiesAfter );
            }
            catch (Exception e)
            {
                if (IsConstraintException(e))
                {
                    if (!ValidConstraintException(e.InnerException.Message))
                    {
                        AstoriaTestLog.WriteLine(e);
                        AstoriaTestLog.WriteVariationResult(false);
                    }
                }
            }
            finally
            {
                AstoriaTestLog.WriteVariationEnd();
                AstoriaTestLog.WriteVariationStart("Link Model Variation  : " + variationID, variationID++);
            }
            //this.CreateContext();
        }
        private bool ValidConstraintException(string errorPayload)
        {
            bool failedValiantly = false;
            XDocument xdocError = XDocument.Parse(errorPayload);
            XName xnMessage = XName.Get("message", "xmlns");
            XName xnInnerError = XName.Get("innererror", "xmlns");
            string errorMessage = xdocError.Descendants(xnInnerError).First().Descendants(xnMessage).First().Value;

            string missingEntityReference = errorMessage.Split('.')[errorMessage.Split('.').Length - 2];
            string entitySetName = missingEntityReference.Split('\'')[1];

            bool sourceTargetExist = this.Context.Resources.Any(edw => edw.Entity.GetType() == TargetResourceType.ClientClrType)
                && this.Context.Resources.Any(edw => edw.Entity.GetType() == SourceResourceType.ClientClrType);
            failedValiantly = !sourceTargetExist;
            failedValiantly = !this.Context.Links.Any(ldw => ldw.Source == Source && ldw.Target == Target
                && ldw.SourceProperty == AssociationName);
            //AstoriaTestLog.WriteLine( entitySetName );
            return failedValiantly;
        }

        private bool IsConstraintException(Exception e)
        {
            string constaintViolationStackFrame = "at System.Data.Mapping.Update.Internal.UpdateTranslator.RelationshipConstraintValidator.ValidateConstraints";

            bool ConstraintFailed = false;
            if (e.InnerException != null)
            {
                ConstraintFailed = e.InnerException.Message.Contains(constaintViolationStackFrame);
            }
            else
            {
                ConstraintFailed = e.StackTrace.Contains(constaintViolationStackFrame);
            }
            return ConstraintFailed;
        }
    }
    public class UpdateModel : ClientModelBase
    {
        protected Workspace _workspace = null;
        protected MergeOption _mergeOption = MergeOption.AppendOnly;
        protected ResourceContainer _resContainer = null;
        protected ResourceType _resType = null;
        protected WebDataCtxWrapper _ctx = null;
        protected object _resource = null;
        protected object _keyedResource = null;
        protected string _resourceSetName = "";
        protected bool _canInsert = false;
        protected bool _canDelete = false;

        protected bool _linkMode = false;

        //keep track of all the entities modified and their values
        //key is the object
        //value is the State +  TryGetUri
        Hashtable Entities = new Hashtable();

        //track entities after calling SaveChages()
        Hashtable EntitiesAfter = new Hashtable();

        public enum LastAction
        {
            Init,
            AddObject,
            AttachObject,
            UpdateObject,
            DeleteObject,
            Detach,
            SaveChanges
        }

        public bool RunningUnderLinkModel { get; set; }
        //Constructor
        public UpdateModel(Workspace workspace, UpdateModel parent)
            : base(workspace)
        {
            _workspace = workspace;
        }

        public UpdateModel(Workspace workspace)
            : base(workspace)
        {
            _workspace = workspace;
        }

        public UpdateModel(Workspace workspace, WebDataCtxWrapper context, object entity, ResourceContainer resContainer,
            UpdateModel parent)
            : base(workspace)
        {
            _workspace = workspace;
            this.ResContainer = resContainer;
            this.CurrentResource = entity;
            CurrentResourceSet = resContainer.Name;

            _ctx = context;
            RunningUnderLinkModel = true;
        }

        //Overrides
        public override ExpressionModel CreateModel()
        {
            return new UpdateModel(this.Workspace, this);
        }

        public Workspace Workspace
        {
            get { return _workspace; }
        }

        public enum EntityState
        {
            Added,
            Modified,
            Deleted,
            Unchanged
        }

        public enum LinkState
        {
            Added,
            Modified,
            Deleted,
            Unchanged
        }


        [ModelVariable]
        public object KeyedCurrentResource
        {
            get
            {
                if (RunningUnderLinkModel)
                {
                    return _keyedResource;
                }
                else
                {
                    return GetResource(true, out _resourceSetName);
                }
            }
        }
        [ModelVariable]
        public object CurrentResource
        {

            get
            {
                if (RunningUnderLinkModel)
                {
                    return _resource;
                }
                else
                {
                    object resource = null;

                    while (resource == null)
                    {
                        resource = GetResource(out _resourceSetName);

                    }

                    return resource;
                }
            }
            set
            {
                if (!RunningUnderLinkModel)
                {
                    _resource = value;
                }
            }
        }

        [ModelVariable]
        public Type CurrentResourceType
        {
            get
            {
                return CurrentResource.GetType();
            }
        }

        [ModelVariable]
        public object RandomTrackedResource
        {
            get
            {
                int randomEntityIndex = randomizer.Next(0, Context.Resources.Count);
                return Context.Resources[randomEntityIndex].Entity;
            }
        }
        [ModelVariable]
        public string CurrentResourceSet
        {
            get
            {

                return _resourceSetName;
            }
            set
            {
                if (!RunningUnderLinkModel)
                {
                    _resourceSetName = value;
                }
            }
        }

        [ModelVariable]
        public MergeOption MergeOpt
        {
            get { return _mergeOption; }
            set { _mergeOption = value; }
        }

        [ModelVariable]
        public int EntitiesTracked
        {
            get { return (null != Context ? Context.Resources.Count : 0); }
        }

        [ModelVariable]
        public ResourceContainer ResContainer
        {
            get { return _resContainer; }
            set { _resContainer = value; }
        }

        [ModelVariable]
        public ResourceType ResType
        {
            get { return _resType; }
            set { _resType = value; }
        }

        [ModelVariable]
        protected override WebDataCtxWrapper Context
        {
            get { return _ctx; }
            set
            {
                if (!RunningUnderLinkModel) { _ctx = value; }
            }
        }

        [ModelVariable]
        public bool CanInsert
        {
            get { return _canInsert; }
            set { _canInsert = value; }
        }

        [ModelVariable]
        public bool CanDelete
        {
            get { return _canDelete; }
            set { _canDelete = value; }
        }

        // disable compiler warning 'The field is assigned but its value is never used'
        // since the fields are used by the model via reflection.
#pragma warning disable 414
        [ModelVariable]
        LastAction _action = LastAction.Init;

        [ModelVariable]
        EntityState _state = EntityState.Unchanged;

        //Actions
        [ModelAction(CallFirst = true)]
        public virtual void CreateContext()
        {
            this.Context = new WebDataCtxWrapper(this.Workspace.ServiceRoot);
            this.Context.Credentials = CredentialCache.DefaultNetworkCredentials;
            AstoriaTestLog.WriteLineIgnore(this.Workspace.ServiceRoot.ToString());
        }

        [ModelAction(Weight = 10)]
        public virtual void AddObject()
        {

            object newResource = CurrentResource;

            try
            {
                Context.AddObject(CurrentResourceSet, newResource);
                ModelExtensions.EnsureInsert(Context.UnderlyingContext, newResource, CurrentResourceSet, Workspace, CurrentResourceSet);
            }
            catch (InvalidOperationException e)
            {
                return;
            }

            AstoriaTestLog.WriteLine("Adding entity " + CurrentResourceSet);

            //Resolve the client type
            Type resType = CurrentResourceType;
            ResolveClientType(this.Workspace, this.Context, resType);
            _action = LastAction.AddObject;
            _state = EntityState.Added;
            //this.CanInsert = model.CanInsert;
            //this.CanDelete = model.CanDelete;
            //this.ResContainer = model.ResultContainer;
            //this.ResType = model.Result;
            //this.CurrentResource = newResource;
        }

        [ModelAction(Weight = 5)]
        public virtual void AttachObject()
        {
            //if (RunningUnderLinkModel)
            //    {
            //    object detachMe = RandomTrackedResource;
            //    Context.Detach( RandomTrackedResource );
            //    Context.AttachTo( CurrentResourceSet, detachMe );
            //    }
            //else
            {
                ScanModel model = new ScanModel(_workspace);
                ModelEngine engine = new ModelEngine();
                engine.Models.Add(model);
                engine.Run();

                if (!model.CanInsert)
                    return;

                object newResource = Activator.CreateInstance(model.Result.ClientClrType);
                KeyExpression key = Workspace.GetRandomExistingKey(model.ResultContainer, model.Result);

                if (key != null)
                    newResource.GetType().GetProperty(key.Properties[0].Name).SetValue(newResource, key.Values[0].ClrValue, new object[] { });
                else
                    return;

                try
                {
                    Context.AttachTo(model.ResultContainer.Name, newResource);
                }
                catch (InvalidOperationException e)
                {
                    return;
                }

                AstoriaTestLog.WriteLine("Attaching entity " + model.ResultContainer.BaseType.Name);

                _action = LastAction.AttachObject;
                _state = EntityState.Unchanged;
                this.CurrentResource = newResource;
                this.CanInsert = model.CanInsert;
                this.CanDelete = model.CanDelete;
                this.ResContainer = model.ResultContainer;
                this.ResType = model.Result;
            }
        }

        System.Random randomizer = new Random();
        [ModelAction(Weight = 5)]
        [ModelRequirement(Variable = "EntitiesTracked", GreaterThan = 1)]
        [ModelRequirement(Variable = "_state", Not = EntityState.Deleted)]
        [ModelRequirement(Variable = "CurrentResource", Not = null)]
        [ModelRequirement(Variable = "CanInsert", Not = false)]
        public virtual void UpdateObject()
        {
            object resourceToBeUpdated = RandomTrackedResource;
            AstoriaTestLog.WriteLine("Updating entity " + resourceToBeUpdated.GetType().Name);
            Context.UpdateObject(resourceToBeUpdated);

            _action = LastAction.UpdateObject;
            _state = EntityState.Modified;
        }

        [ModelAction(Weight = 10)]
        [ModelRequirement(Variable = "EntitiesTracked", GreaterThanOrEqual = 1)]
        [ModelRequirement(Variable = "_state", Not = EntityState.Deleted)]
        [ModelRequirement(Variable = "CanDelete", Not = false)]
        [ModelRequirement(Variable = "CanInsert", Not = false)]
        public virtual void DeleteObject()
        {

            object resourceToBeDeleted = RandomTrackedResource;
            AstoriaTestLog.WriteLine("Deleting entity " + resourceToBeDeleted.GetType().Name);
            Context.DeleteObject(resourceToBeDeleted);

            _action = LastAction.DeleteObject;
            _state = EntityState.Deleted;
        }

        [ModelAction(Weight = 5)]
        [ModelRequirement(Variable = "EntitiesTracked", GreaterThanOrEqual = 1)]
        public virtual void Detach()
        {
            AstoriaTestLog.WriteLine("Detaching entity " + CurrentResource.GetType().Name);
            object resourceToBeDetached = RandomTrackedResource;
            Context.Detach(resourceToBeDetached);

            _action = LastAction.Detach;
            CurrentResource = null;
            _state = EntityState.Unchanged;
        }

        #region Commented Code
        //[ModelAction]
        //[ModelRequirement( Variable = "EntitiesTracked", GreaterThanOrEqual = 1 )]
        //[ModelRequirement( Variable = "_state", Not = EntityState.Unchanged )]
        //public virtual void AddLink()
        //    {
        //    //based on the curent entity tracked, find a related entity and add a link to it

        //    ResourceAssociations associations = this.ResType.FindAssociationsOfMultiplicity( Multiplicity.One, Multiplicity.Many );
        //    if (associations.Count == 0)
        //        return;

        //    foreach (ResourceAssociation association in associations)
        //        {
        //        ResourceAssociationEnd Source = null;
        //        ResourceAssociationEnd Target = null;
        //        try
        //            {

        //            Source = association.Ends.Where<ResourceAssociationEnd>( rea => rea.Multiplicity == Multiplicity.One )
        //                .FirstOrDefault<ResourceAssociationEnd>();

        //            Target = association.Ends
        //                .Where<ResourceAssociationEnd>( rea => rea.Multiplicity == Multiplicity.Many && rea != Source )
        //                .FirstOrDefault<ResourceAssociationEnd>();

        //            ResourceType destinationType = Target.ResourceType;
        //            ResourceType sourceResourceType = Source.ResourceType;

        //            //If the Right Hand Side has a referential constraint , then changing 
        //            //the relation throws an exception
        //            if (sourceResourceType.Name == "Order_Details")
        //                continue;

        //            ResourceContainer destinationContainer = Workspace.ServiceContainer.ResourceContainers.Where<ResourceContainer>(
        //                 rc => rc.ResourceTypes.Any<ResourceType>( rt => rt == destinationType )
        //                     ).First<ResourceContainer>();

        //            ResourceContainer sourceContainer = Workspace.ServiceContainer.ResourceContainers.Where<ResourceContainer>(
        //                 rc => rc.ResourceTypes.Any<ResourceType>( rt => rt == Source.ResourceType )
        //                     ).First<ResourceContainer>();


        //            object parent = Activator.CreateInstance( sourceResourceType.ClientClrType );
        //            object child = Activator.CreateInstance( destinationType.ClientClrType );

        //            Context.AddObject( destinationContainer.Name, child );
        //            Context.AddLink( parent, Target.Name, child );
        //            }
        //        catch (Exception exc)
        //            {
        //            AstoriaTestLog.FailAndContinue( exc );
        //            }

        //        }

        //    }
        #endregion Commented Code

        [ModelAction(CallOnce = true)]
        [ModelParameter(Values = new object[] { MergeOption.AppendOnly, MergeOption.NoTracking, MergeOption.OverwriteChanges, MergeOption.PreserveChanges }, Position = 0)]
        public virtual void SetMergeOptions(MergeOption mo)
        {
            AstoriaTestLog.WriteLineIgnore("Setting MergeOptions :" + mo.ToString());
            this.Context.MergeOption = mo;
        }

        [ModelAction]
        [ModelRequirement(Variable = "RunningUnderLinkModel", Not = true)]
        [ModelRequirement(Variable = "_action", Not = LastAction.SaveChanges)]
        [ModelRequirement(Variable = "EntitiesTracked", GreaterThanOrEqual = 1)]
        [ModelParameter(Values = new object[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.None, SaveChangesOptions.ContinueOnError, SaveChangesOptions.ReplaceOnUpdate }, Position = 0)]
        [ModelParameter(Values = new object[] { "sync", "async" }, Position = 1)]
        public virtual void SaveChanges(SaveChangesOptions so, string sync)
        {
            AstoriaTestLog.WriteLineIgnore("Entities tracked:" + Context.Resources.Count);
            this.Entities.Clear();
            foreach (EntityDescriptorWrapper ed in Context.Resources)
            {
                Uri _uri = null;
                bool bURI = Context.TryGetUri(ed.Entity, out _uri);
                Entities.Add(ed.Entity, new string[] { ed.State.ToString(), _uri == null ? "" : _uri.OriginalString });
            }

            AstoriaTestLog.WriteLineIgnore("Save changes");

            if (Context.Resources.Count > 1)
                so = SaveChangesOptions.BatchWithSingleChangeset;

            DataServiceResponseWrapper response = null;

            try
            {
                switch (sync)
                {
                    case "sync":
                        response = this.Context.SaveChanges(so);
                        break;
                    case "async":
                        IAsyncResult async = this.Context.BeginSaveChanges(null, null);
                        response = this.Context.EndSaveChanges(async);
                        break;
                }
                //Save info on entities
                EntitiesAfter.Clear();
                foreach (EntityDescriptorWrapper ed in Context.Resources)
                {
                    Uri _uri = null;
                    bool bURI = Context.TryGetUri(ed.Entity, out _uri);
                    EntitiesAfter.Add(ed.Entity, new string[] { ed.State.ToString(), _uri == null ? "" : _uri.OriginalString });
                }

                //Verification
                VerifySaveChanges(response);
                VerifyData(this.Workspace, this.Entities, this.EntitiesAfter);
                //RaiseCompleted(true);
                //  AstoriaTestLog.WriteVariationResult(true);
            }
            catch (Exception e)
            {
                AstoriaTestLog.WriteLine(e);
                // AstoriaTestLog.WriteVariationResult(false);
                //RaiseCompleted(false);
            }
            finally
            {
                // AstoriaTestLog.WriteVariationEnd();
                // AstoriaTestLog.WriteVariationStart("Model Variation: " + ClientModelBase.variationID, ClientModelBase.variationID++);
                _action = LastAction.SaveChanges;
                _state = EntityState.Unchanged;
                CurrentResource = null;
                this.CreateContext();

            }


        }

        #region Helper

        public static Exception FindChangesetFailures(DataServiceResponseWrapper response)
        {
            Exception failure = null;
            foreach (ChangesetResponseWrapper changeset in response)
            {
                if (changeset.Error != null)
                    failure = changeset.Error;

                if (changeset.Descriptor.GetType() == typeof(EntityDescriptorWrapper))
                {
                    EntityDescriptorWrapper entity = (EntityDescriptorWrapper)changeset.Descriptor;
                    if (failure == null)
                        AstoriaTestLog.IsTrue(entity.State == EntityStates.Unchanged || entity.State == EntityStates.Detached);
                }
                else
                {
                    LinkDescriptorWrapper link = (LinkDescriptorWrapper)changeset.Descriptor;
                    if (failure == null)
                        AstoriaTestLog.IsTrue(link.State == EntityStates.Unchanged || link.State == EntityStates.Detached);
                }
            }

            return failure;
        }

        public static void VerifySaveChanges(DataServiceResponseWrapper response)
        {
            Exception failure = FindChangesetFailures(response);
            if (failure != null)
                throw failure;
        }

        public static void VerifyData(Workspace w, Hashtable entities, Hashtable entitiesAfter)
        {
            IDictionaryEnumerator _enum = entities.GetEnumerator();
            object _entity = null;
            EntityStates _state = EntityStates.Unchanged;
            string _uri = null;

            while (_enum.MoveNext())
            {
                _entity = _enum.Key;
                _state = (EntityStates)Enum.Parse(typeof(EntityStates), (string)((string[])_enum.Value)[0], true);

                switch (_state)
                {
                    case EntityStates.Added:
                        if (entitiesAfter != null)
                        {
                            _uri = (string)((string[])entitiesAfter[_entity])[1];
                            Verify(_entity, _uri, _state, w);
                            AstoriaTestLog.WriteLineIgnore("Verified entity " + _entity.GetType().Name + " was inserted!");
                        }
                        break;
                    case EntityStates.Unchanged:
                    case EntityStates.Modified:
                        _uri = (string)((string[])_enum.Value)[1];
                        Verify(_entity, _uri, _state, w);
                        AstoriaTestLog.WriteLineIgnore("Verified entity " + _entity.GetType().Name + " was " + _state.ToString().ToLower());
                        break;
                    case EntityStates.Deleted:
                        try
                        {
                            _uri = (string)((string[])_enum.Value)[1];
                            Verify(_entity, _uri, _state, w);
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException is DataServiceQueryException)
                                AstoriaTestLog.WriteLineIgnore("Verified entity " + _entity.GetType().Name + " was " + _state.ToString().ToLower());
                            else
                                AstoriaTestLog.FailAndContinue(e);
                        }
                        break;
                }

            }





        }

        public static void Verify(object entity, string uri, EntityStates state, Workspace w)
        {
            WebDataCtxWrapper ctx = new WebDataCtxWrapper(w.ServiceRoot);
            ctx.Credentials = CredentialCache.DefaultNetworkCredentials;

            MethodInfo mi = ctx.GetType().GetMethod("Execute", new Type[] { typeof(System.Uri) });
            MethodInfo execMethod = mi.MakeGenericMethod(entity.GetType());
            object results = execMethod.Invoke(ctx, new object[] { new Uri(uri, UriKind.RelativeOrAbsolute) });

            switch (state)
            {
                case EntityStates.Unchanged:
                case EntityStates.Modified:
                case EntityStates.Added:
                    //entity was updated

                    IEnumerator r = ((IEnumerable)results).GetEnumerator();

                    while (r.MoveNext())
                    {
                        VerifyResult(r.Current, entity);
                        break;
                    }
                    break;
                case EntityStates.Deleted:
                    AstoriaTestLog.WriteLineIgnore("deleted!");
                    break;
            }
        }


        public static void VerifyResult(object result, object baseline)
        {
            Object expectedResult = null;
            Object actualResult = null;
            PropertyInfo[] _props = baseline.GetType().GetProperties();

            foreach (PropertyInfo pi in _props)
            {
                expectedResult = pi.GetValue(baseline, null);

                PropertyInfo newpi = result.GetType().GetProperty(pi.Name);
                actualResult = newpi.GetValue(result, null);

                if (expectedResult != null)
                {
                    if (actualResult != null)
                    {
                        if (expectedResult.ToString().Contains(actualResult.ToString()))
                        {
                            //AstoriaTestLog.WriteLine("Found new value inserted for resource " + result.GetType().Name);
                        }
                        else
                        {
                            AstoriaTestLog.FailAndContinue(new TestFailedException("Insert/Update failed for resource " + result.GetType().Name + " Expected:" + expectedResult.ToString() + ".Actual:" + actualResult.ToString()));
                        }
                    }
                    else
                    {
                        AstoriaTestLog.FailAndContinue(new TestFailedException("Insert/Update failed for resource " + result.GetType().Name + " Expected:" + expectedResult.ToString() + ".Actual:" + actualResult.ToString()));
                    }
                }
            }

        }



        #endregion
    }

    public class ClientModelBase : ExpressionModel
    {

        Workspace _workspace;

        public ClientModelBase(Workspace workspace)
            : base()
        {
            _workspace = workspace;
        }
        protected static int variationID = 3;
        public ClientModelBase(Workspace workspace, UpdateModel parent)
            : base(parent)
        {
            _workspace = workspace;
        }
        public ClientModelBase(ClientModelBase parent)
            : base(parent)
        {
        }
        public override ExpressionModel CreateModel()
        {
            return new ClientModelBase(this._workspace, null);
        }

        protected object GetResource(out string entitySetName)
        {
            return GetResource(false, out entitySetName);
        }

        protected object GetResource(bool existing, out string entitySetName)
        {
            ResourceContainer resCon;


            return GetResource(existing, out entitySetName, out resCon);
        }
        protected object GetResource(out string entitySetName, out ResourceContainer resContainer)
        {
            bool canInsert;
            bool canDelete;
            return GetResource(false, out entitySetName, out resContainer);
        }
        protected virtual WebDataCtxWrapper Context { get; set; }

        protected void RaiseCompleted(bool testPassed)
        {
            if (TestVariationComplete != null)
            {
                TestVariationComplete(testPassed);
            }
        }
        public virtual void CommitChanges(SaveChangesOptions so)
        {
            try
            {
                Context.SaveChanges(so);
                AstoriaTestLog.WriteVariationResult(true);
                //RaiseCompleted(true);
            }
            catch (Exception e)
            {
                if (IsConstraintException(e))
                {
                    if (!ValidConstraintException(e.InnerException.Message))
                    {
                        AstoriaTestLog.WriteLine(e.ToString());
                        //RaiseCompleted(false);
                        //AstoriaTestLog.FailAndContinue(e);
                        AstoriaTestLog.WriteLine("Iteration Failed");
                        AstoriaTestLog.WriteVariationResult(false);
                    }
                    else
                    {
                        //RaiseCompleted(true);
                        AstoriaTestLog.WriteVariationResult(true);
                    }
                }
            }
            finally
            {
                AstoriaTestLog.WriteVariationEnd();
                AstoriaTestLog.WriteVariationStart("Save Changes  : " + variationID, variationID++);
            }
        }

        private bool ValidConstraintException(string errorPayload)
        {
            bool failedValiantly = false;
            XDocument xdocError = XDocument.Parse(errorPayload);
            XName xnMessage = XName.Get("message", "xmlns");
            XName xnInnerError = XName.Get("innererror", "xmlns");
            string errorMessage = xdocError.Descendants(xnInnerError).First().Descendants(xnMessage).First().Value;

            string missingEntityReference = errorMessage.Split('.')[errorMessage.Split('.').Length - 2];
            string entitySetName = missingEntityReference.Split('\'')[1];

            //bool sourceTargetExist = this.Context.Resources.Any( edw => edw.Entity.GetType() == TargetResourceType.ClientClrType )
            //    && this.Context.Resources.Any( edw => edw.Entity.GetType() == SourceResourceType.ClientClrType );
            //failedValiantly = !sourceTargetExist;
            //failedValiantly = !this.Context.Links.Any( ldw => ldw.Source == Source && ldw.Target == Target
            //    && ldw.SourceProperty == AssociationName );
            //AstoriaTestLog.WriteLine( entitySetName );
            return failedValiantly;
        }

        public delegate void TestVariationCompleteHandler(bool testResult);

        public event TestVariationCompleteHandler TestVariationComplete;

        private bool IsConstraintException(Exception e)
        {
            string constaintViolationStackFrame = "at System.Data.Mapping.Update.Internal.UpdateTranslator.RelationshipConstraintValidator.ValidateConstraints";

            bool ConstraintFailed = false;
            if (e.InnerException != null)
            {
                ConstraintFailed = e.InnerException.Message.Contains(constaintViolationStackFrame);
            }
            else
            {
                ConstraintFailed = e.StackTrace.Contains(constaintViolationStackFrame);
            }
            return ConstraintFailed;
        }
        protected object GetResource(bool existing, out string entitySetName, out ResourceContainer resContainer)
        {
            ScanModel model = new ScanModel(_workspace);
            ModelEngine engine = new ModelEngine();
            engine.Models.Add(model);
            engine.Run();
            entitySetName = "";
            resContainer = null;

            if (!model.CanInsert)
                return null;

            object newResource = Activator.CreateInstance(model.Result.ClientClrType);

            string propName = null;
            object propValue = null;

            entitySetName = model.ResultContainer.Name;
            resContainer = model.ResultContainer;

            foreach (ResourceProperty property in model.Result.Properties.Where(p => p.PrimaryKey != null || p.Facets.Nullable == false))
            {
                if (!property.IsComplexType && !property.IsNavigation) //&& !property.Facets.ServerGenerated)
                {
                    propValue = Resource.CreateValue(property).ClrValue;
                    propName = property.Name;
                    newResource.GetType().GetProperty(propName).SetValue(newResource, propValue, new object[] { });
                }
            }
            if (existing)
            {
                KeyExpression key = _workspace.GetRandomExistingKey(model.ResultContainer, model.Result);

                if (key != null)
                    newResource.GetType().GetProperty(key.Properties[0].Name).SetValue(newResource, key.Values[0].ClrValue, new object[] { });
                else
                    return null;
            }

            return newResource;
        }

        public static IEnumerable ExecuteQuery(WebDataCtxWrapper ctx, Type pType, string sUri, string execType)
        {
            MethodInfo mi = null;
            MethodInfo miConstructed = null;
            object enumerable = null;
            object[] args = { new Uri(sUri, UriKind.Relative) };

            Type cType = typeof(WebDataCtxWrapper);

            switch (execType)
            {
                case "sync":
                    mi = cType.GetMethod("Execute", new Type[] { typeof(System.Uri) });
                    miConstructed = mi.MakeGenericMethod(pType);
                    enumerable = miConstructed.Invoke(ctx, args);
                    break;
                case "async":
                    mi = cType.GetMethod("BeginExecute", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(System.Uri), typeof(System.AsyncCallback), typeof(object) }, null);
                    miConstructed = mi.MakeGenericMethod(pType);

                    object asyncResult = miConstructed.Invoke(ctx, new object[] { new Uri(sUri, UriKind.Relative), null, null });

                    MethodInfo endexecute = cType.GetMethod("EndExecute", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IAsyncResult) }, null);
                    MethodInfo miConstructed2 = endexecute.MakeGenericMethod(pType);
                    enumerable = miConstructed2.Invoke(ctx, new object[] { asyncResult });
                    break;
            }
            return (IEnumerable)enumerable;
        }

        public static void ResolveClientType(Workspace workspace, WebDataCtxWrapper ctx, Type resType)
        {
            if (workspace.Name == "Aruba")
            {
                switch (AstoriaTestProperties.DataLayerProviderKinds[0])
                {
                    case DataLayerProviderKind.Edm:
                        ctx.ResolveType = delegate(string typeName)
                        {
                            return resType.Assembly.GetType("ArubaClient." + typeName.Substring("Aruba.".Length), true, false);
                        };

                        ctx.ResolveName = delegate(Type type)
                        {
                            return "Aruba." + type.Name;

                        };
                        break;
                    case DataLayerProviderKind.LinqToSql:
                        ctx.ResolveType = delegate(string typeName)
                        {
                            return resType.Assembly.GetType("ArubaClientLTS." + typeName.Substring("Aruba.".Length), true, false);
                        };

                        ctx.ResolveName = delegate(Type type)
                        {
                            return "Aruba." + type.Name;

                        };
                        break;
                }
            }
        }

        #region Helper Methods
        protected bool ShouldAllowTestInheritanceFilter(ResourceType resourceType, bool allowInheritedTypes)
        {
            if (allowInheritedTypes)
            {
                if (resourceType.BaseType == null)
                    return false;
                return true;
            }
            else
            {
                //Skip any resourceTypes that are involved with inheritance
                if (resourceType.BaseType == null)
                    return true;
                return false;
            }
        }
        protected bool ShouldAllowTestOneOrLessRequiredAssociation(ResourceType resourceType, ResourceAssociation association)
        {
            ResourceAssociations associations = new ResourceAssociations();
            associations.Add(association);
            return ShouldAllowTestOneOrLessRequiredAssociation(resourceType, associations);
        }
        protected bool ShouldAllowTestOneOrLessRequiredAssociation(ResourceType resourceType, ResourceAssociations associations)
        {
            ResourceAssociations requiredAssociations = resourceType.AssociationsRequired;
            if (requiredAssociations.Count == 1)
            {
                if (associations.Contains(requiredAssociations[0]))
                    return true;
            }
            else if (requiredAssociations.Count == 0)
            {
                return true;
            }
            return false;
        }

        protected void QueryResourceTypesByAssociationsForUpdate(Multiplicity resourceTypeMul, Multiplicity otherEndMul, bool allowSelfAssociations, bool allowInheritedTypes, bool allowAssociationEntites,
              Func<ResourceContainer, ResourceType, ResourceContainer, ResourceType, bool> testfunction)
        {
            QueryResourceTypesForUpdate(delegate(ResourceContainer sourceContainer, ResourceType sourceResourceType, Workspace currentWorkspace)
            {
                //Skip type if its not insertable
                if (!sourceResourceType.IsInsertable)
                    return false;
                //Skip type if its abstract
                if (sourceResourceType.Facets.AbstractType)
                    return false;
                //Skip types based on inheritance flag
                if (!ShouldAllowTestInheritanceFilter(sourceResourceType, allowInheritedTypes))
                    return false;
                ResourceAssociations currentResourceAssociations = sourceResourceType.FindAssociationsOfMultiplicity(resourceTypeMul, otherEndMul);
                ResourceAssociations requiredAssociations = sourceResourceType.AssociationsRequired;

                ////Skipping these always example is Order_Details
                if (!allowAssociationEntites && sourceResourceType.IsAssociationEntity)
                    return false;
                //Skip types if no Associations of specified Multiplicity
                if (currentResourceAssociations.Count() == 0)
                {
                    return false;
                }
                if (!ShouldAllowTestOneOrLessRequiredAssociation(sourceResourceType, currentResourceAssociations))
                {
                    return false;
                }

                bool resourceTypeTestRun = false;

                foreach (ResourceAssociation association in currentResourceAssociations)
                {
                    ResourceAssociationEnd Source = null;
                    ResourceAssociationEnd Target = null;
                    try
                    {

                        Source = association.Ends.Where<ResourceAssociationEnd>(rea => rea.Multiplicity == resourceTypeMul)
                            .FirstOrDefault<ResourceAssociationEnd>();

                        Target = association.Ends
                            .Where<ResourceAssociationEnd>(rea => rea.Multiplicity == otherEndMul && rea != Source)
                            .FirstOrDefault<ResourceAssociationEnd>();


                        ResourceType destinationType = Target.ResourceType;
                        sourceResourceType = Source.ResourceType;

                        //If the Right Hand Side has a referential constraint , then changing 
                        //the relation throws an exception
                        if (sourceResourceType.Name == "Order_Details")
                            continue;

                        ResourceContainer destinationContainer = currentWorkspace.ServiceContainer.ResourceContainers.Where<ResourceContainer>(
                             rc => rc.ResourceTypes.Any<ResourceType>(rt => rt == destinationType)
                                 ).First<ResourceContainer>();

                        sourceContainer = currentWorkspace.ServiceContainer.ResourceContainers.Where<ResourceContainer>(
                             rc => rc.ResourceTypes.Any<ResourceType>(rt => rt == Source.ResourceType)
                                 ).First<ResourceContainer>();




                        //ResourceAssociation associationN = sourceResourceType.Relations.Skip<NodeRelation>( 1 ).First( nodeRel => (sourceResourceType == nodeRel.Source() && destinationType == nodeRel.Target()) ) as ResourceAssociation;

                        //(((ResourceAssociation)  sourceResourceType.Relations[1])).Source
                        //string associationName = sourceResourceType.Relations.First(nodeREl => nodeREl.   
                        resourceTypeTestRun = testfunction(sourceContainer, sourceResourceType,
                            destinationContainer, destinationType);
                    }
                    catch (Exception except)
                    {
                        //if (!(except is TestSkippedException))
                        //    {
                        AstoriaTestLog.FailAndContinue(except);
                        //}
                        resourceTypeTestRun = true;
                    }
                }
                return resourceTypeTestRun;
            });
        }

        protected void QueryResourceTypesForUpdate(Func<ResourceContainer, ResourceType, Workspace, bool> testfunction)
        {
            bool testRun = false;


            if (!_workspace.Settings.SupportsUpdate)
                return;
            bool resourceContainerRun = false;
            foreach (ResourceContainer container in _workspace.ServiceContainer.ResourceContainers)
            {
                bool resourceTypeTestRun = false;
                foreach (ResourceType resourceType in
                    container.ResourceTypes.Where(rt => rt.IsInsertable == true && rt.Facets.AbstractType == false)
                    )
                {

                    //TODO: Need to fix bugs in test Framework
                    //Type won't work because it has associations to 
                    //the same type but in two different sets
                    //if (resourceType.Name.Equals("Computers"))
                    //    continue;
                    if (resourceType.Name.Equals("DataKey_Bit"))
                        continue;

                    if (resourceType.Name.Equals("Task"))
                        continue;

                    //DateTime Key has a slash character in URI
                    if (resourceType.Name.Contains("DateTime"))
                        continue;

                    //Thirteen Navigations , the Client tests dont generate n number of target entities 
                    //required for the update to be succesful
                    if (resourceType.Name.Equals("ThirteenNavigations"))
                        continue;

                    try
                    {
                        if (testfunction(container, resourceType, _workspace))
                            resourceTypeTestRun = true;
                    }
                    catch (Exception exc)
                    {
                        resourceTypeTestRun = true;
                        AstoriaTestLog.FailAndContinue(exc);
                    }
                }
                if (resourceTypeTestRun)
                    resourceContainerRun = true;
            }
            if (resourceContainerRun)
                testRun = true;

            if (testRun == false)
            {
                AstoriaTestLog.WriteLine("Didn't execute a test, no resourceType found to match criteria");
            }
        }

        #endregion Helper Methods
    }
}
