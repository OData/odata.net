//---------------------------------------------------------------------
// <copyright file="AstoriaTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.ModuleCore;
    using Microsoft.Test.KoKoMo;
    using System.Linq;

    /// <summary>This class should be used as the base test class for Astoria test cases.</summary>
    public partial class AstoriaTestCase : TestCase, IComparer
    {
        //Data
        private static string _suiteName;
        private Workspaces _workspaces = null;

        /// <summary>Initializes a new <see cref="AstoriaTestCase"/> instance.</summary>
        public AstoriaTestCase()
        {
            KeepInContent = true;
        }

        int IComparer.Compare(Object x, Object y)
        {
            TestItem xItem = (TestItem)x;
            TestItem yItem = (TestItem)y;

            if (xItem.Id < yItem.Id)
                return -1;
            else if (xItem.Id > yItem.Id)
                return 1;

            return 0;
        }

        public static void SimpleTestOnSpecificContainerType(Workspaces workspaces, string containerName, string resourceTypeName, Action<ResourceContainer, ResourceType> action)
        {
            bool testRun = false;
            bool testSkipped = false;

            foreach (Workspace workspace in workspaces)
            {
                ResourceType resourceType = null;
                ResourceContainer container = null;
                IEnumerable<ResourceContainer> containers = workspace.ServiceContainer.ResourceContainers.Where(rc => rc.Name == containerName);
                if (containers.Count() > 0)
                    container = containers.First();
                if (container != null)
                {
                    if (!container.Workspace.Settings.SupportsUpdate)
                    {
                        testSkipped = true;
                        continue;
                    }
                    IEnumerable<ResourceType> resourceTypes = container.ResourceTypes.Where(rt => rt.Name == resourceTypeName).ToList();
                    if (resourceTypes.Count() > 0)
                        resourceType = resourceTypes.First();
                    action.Invoke(container, resourceType);
                    testRun = true;
                }
            }
            //Test likely to be more of a one off type of test on a specific resourceType
            if ((testRun == false) && (!testSkipped))
                throw new TestFailedException("Couldn't find a container:" + containerName + " with a type:" + resourceTypeName + " to run test with");
            else
            {
                if (testSkipped)
                    throw new TestSkippedException("Container: " + containerName + " in this workspace does not support update");
            }

        }
        protected void SimpleTestOnSpecificContainerType(string containerName, string resourceTypeName, Action<ResourceContainer, ResourceType> action)
        {
            AstoriaTestCase.SimpleTestOnSpecificContainerType(this.Workspaces,containerName, resourceTypeName, action);
        }

        protected override void DetermineChildren()
        {
            #region Friendlyfeeds test injection
            bool generateVariation = false;

            foreach (MethodInfo method in this.GetType().GetMethods())
            {
                foreach (VariationAttribute testVarAttrib in
                    method.GetCustomAttributes(typeof(VariationAttribute), false/*inhert*/))
                {
                    if (ShowFFAttributedTests)
                    {
                        //Loop through the Attributes for method
                        foreach (FriendlyFeedsTestVariationAttribute attr in
                            method.GetCustomAttributes(typeof(FriendlyFeedsTestVariationAttribute), false/*inhert*/))
                        {
                            if (testVarAttrib.Implemented)
                            {

                                //no support for inherited types FF tests in NonClr scenarios
                                generateVariation = (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr))
                                    && attr.InheritanceFilter == InerhitedTypes.Both || !AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr);


                                generateVariation = AstoriaTestProperties.MaxPriority == 0 ? testVarAttrib.Desc == "Query top level navigation collection sync" : true;
                                //If the DataLayer is NonCLR and we have types which are Open Types,
                                //Any mappings with KeepInContent=False cannot succeed in Queries.
                                if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr)
                                    && AstoriaTestProperties.UseOpenTypes
                                    && attr.GenerateClientTypes
                                    && !attr.KeepInContent)
                                {
                                    generateVariation = false;
                                }
                                //if (attr.InheritanceFilter == InerhitedTypes.Both)
                                //{
                                if (generateVariation)
                                {
                                    TestVariation testVariation = attr.GenerateVariation(this, method, testVarAttrib);
                                    if (testVariation != null)
                                        this.AddChild(testVariation);
                                }
                                //}
                            }
                        }
                    }
                    if (ShowRowCountTests)
                    {
                        //Loop through the Attributes for method
                        foreach (RowCountTestVariationAttribute attr in
                            method.GetCustomAttributes(typeof(RowCountTestVariationAttribute), false/*inhert*/))
                        {
                            if (attr.Implemented)
                            {

                                TestVariation testVariation = attr.GenerateVariation(this, method);
                                this.AddChild(testVariation);

                                //}
                            }
                        }
                    }

                }
            }
            #endregion
            if (ShowNonFFAttributedTests && !ShowFFAttributedTests)
            {
                //if (!ShowRowCountTests)
                //{
                //    List<RowCountTestVariation> rowCountVariations = this.Children.OfType<RowCountTestVariation>().ToList();
                //    rowCountVariations.ForEach(
                //        rcv =>
                //        {
                //            this.Children.Remove(rcv as VariationAttribute );
                //        }
                //        );
                //}
                base.DetermineChildren();
                //Now determine matrix children
                foreach (MethodInfo method in this.GetType().GetMethods())
                {
                    //Loop through the Attributes for method
                    foreach (MatrixVariationAttribute attr in method.GetCustomAttributes(typeof(MatrixVariationAttribute), false/*inhert*/))
                    {
                        List<TestVariation> variations = attr.EnumerateVariations(this, method);
                        foreach (TestVariation v in variations)
                        {
                            this.AddChild(v);
                        }
                    }
                }
            }

            int index = 0;
            while(index < this.Children.Count)
            {
                TestItem item = (TestItem)this.Children[index];
                //If it's a test case or a variation that is between min and max priority we want to keep it, else remove it from the list
                if (!(item is TestVariation) || item.Children.Count > 0 || (AstoriaTestProperties.MinPriority <= item.Priority && item.Priority <= AstoriaTestProperties.MaxPriority))
                    index++;
                else
                    this.Children.RemoveAt(index);
            }

            if (this.Children.Count <= 0)
                this.Parent.Children.Remove(this);
            else
                this.Children.Sort();
        }

        /// <summary>
        /// Hook added so that people can update workspaces with Service Operations
        /// 
        /// </summary>
        /// <param name="workspace"></param>
        public virtual void OnWorkspaceCreate(Workspace workspace)
        {
        }
        public Workspaces Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = System.Data.Test.Astoria.Workspaces.GetWorkspacesFilteredByTestProperties(this.OnWorkspaceCreate);
                }

                return _workspaces;
            }
        }

        //Overrides
        public override void Init()
        {
            //Delegate
            base.Init();

            CheckWindows7CompatMode();
        }

        public override void Terminate()
        {
            CheckWindows7CompatMode();

            //Delegate
            base.Terminate();
        }

        private static void CheckWindows7CompatMode()
        {
            switch (AstoriaTestProperties.WindowsCompatFlag)
            {
                case WindowsCompatFlag.None:
                    return;
                case WindowsCompatFlag.Vista:
                    throw new NotImplementedException("Vista Compatible Mode Check Not Implemented");
                case WindowsCompatFlag.Win7:
                    // Check that if we are running on Win7 then we are in Win7 compat mode
                    if (OsCompatibility.IsWindows7CompatibilitySupported)
                    {
                        try
                        {
                            OsCompatibility.EnsureWindows7DynamicContextIsActive();
                        }
                        catch (Exception e)
                        {
                            AstoriaTestLog.Warning(false, e.ToString());
                        }
                        AstoriaTestLog.WriteLineIgnore("Verified that we are running in Win7 compat mode");
                    }
                    else
                    {
                        AstoriaTestLog.WriteLineIgnore("Win7 compat mode not supported");
                    }
                    break;
                case WindowsCompatFlag.Win8:
                    throw new NotImplementedException("Windows 8 Compatible Mode Check Not Implemented");
                default:
                    throw new NotImplementedException("Unknown Compatible Mode Check Not Implemented");
            }
        }

        protected override TestVariation CreateVariation(TestFunc func)
        {
            return new AstoriaTestVariation(func);
        }

        protected override void OnEnter(TestMethod method)
        {
        }

        protected override void OnLeave(TestMethod method)
        {
        }

        protected override TestResult HandleException(Exception e)
        {
            return TestLog.HandleException(e);
        }
    }

    public partial class AstoriaTestCase
    {
        public bool ShowFFAttributedTests { get; set; }
        public bool ShowRowCountTests { get; set; }
        private bool _showNonFFtests = true;
        public bool ShowNonFFAttributedTests
        {
            get
            {
                return _showNonFFtests;
            }
            set
            {
                _showNonFFtests = value;
            }
        }
        public bool KeepInContent { get; set; }
        public bool KeepSameNamespace { get; set; }
        public bool IncludeComplexTypes { get; set; }
        public bool RemoveUnEligibleTypes { get; set; }
        public string[] PreDefinedPaths { get; set; }
        public bool GenerateClientTypes { get; set; }
        public bool MapPropertiesToAtomElements { get; set; }
        public Func<ResourceType, bool> filterResourcesLambda { get; set; }
        public bool MapPropertiesToNonAtomElements { get; set; }
    }

    public enum InerhitedTypes
    {
        BaseTypes,
        DerivedTypes,
        Both
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RowCountTestVariationAttribute : Attribute
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public bool Implemented { get; set; }
        public string Desc { get; set; }
        public object[] Params { get; set; }
        public object Param { get; set; }
        public TestVariation GenerateVariation(TestCase testcase, MethodInfo method)
        {
            TestVariation rcTestVariation;
            TestFunc testMethodAction = delegate()
            {
                method.Invoke(testcase, null);
            };
            rcTestVariation = new TestVariation(testMethodAction);

            rcTestVariation.Params = this.Params;
            rcTestVariation.Param = this.Param;
            rcTestVariation.Desc = this.Desc;
            rcTestVariation.Id = this.Id;
            rcTestVariation.Priority = this.Priority;

            return rcTestVariation;
        }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FriendlyFeedsTestVariationAttribute : Attribute
    {
        private static void SkipInvalidRuns()
        {
            if (AstoriaTestProperties.Host == Host.WebServiceHost)
                throw new TestSkippedException("Friendly feeds variations are not suppported on WebServiceHost");

            //if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.InMemoryLinq))
            //    throw new TestSkippedException("Friendly feeds variations are not suppported on InMemory workspaces");
        }

        public bool KeepInContent { get; set; }
        public bool KeepSameNamespace { get; set; }
        public bool IncludeComplexTypes { get; set; }
        public bool GenerateClientTypes { get; set; }
        public bool RemoveUnEligibleTypes { get; set; }
        public bool MapToAtomElements { get; set; }
        public bool MapToNonAtomElements { get; set; }
        public string[] PreDefinedPaths { get; set; }
        public InerhitedTypes InheritanceFilter { get; set; }
        public int Id { get; set; }
        public int Priority { get; set; }
        private bool _generateServerEpmMappints = true;
        public bool GenerateServerEPMMappings
        {
            get
            {
                return _generateServerEpmMappints;
            }
            set
            {
                _generateServerEpmMappints = value;
            }
        }

        public Func<ResourceType, bool> FilterResourcesLambda { get; set; }
        public FriendlyFeedsTestVariationAttribute()
            : base()
        {
        }

        private Func<ResourceType, bool> GenerateLambda()
        {
            Func<ResourceType, bool> filterResourcesLambda = null;
            switch (InheritanceFilter)
            {
                case InerhitedTypes.DerivedTypes:
                    filterResourcesLambda = delegate(ResourceType resourceType)
                    {
                        if (resourceType.BaseType != null)
                        {
                            resourceType.BaseType.Properties.ToList()
                                .ForEach(
                                baseProp =>
                                {
                                    resourceType.Properties.Add(baseProp);
                                }
                                );
                        }
                        return resourceType.BaseType != null;
                    };
                    break;
                case InerhitedTypes.BaseTypes:
                    filterResourcesLambda = delegate {
                        return false;
                    };
                    break;
                case InerhitedTypes.Both:
                    filterResourcesLambda = delegate(ResourceType resourceType)
                    {
                        return !resourceType.HasDerivedTypes
                            && resourceType.BaseType == null;
                    };
                    break;
            }
            return filterResourcesLambda;
        }



        public TestVariation GenerateVariation(TestCase testcase, MethodInfo method, VariationAttribute testVarAttrib)
        {
            //do not generate variations for tests which have KeepInContent set to false and the version is not V2
            if (!this.KeepInContent && !Versioning.Server.SupportsV2Features)
            {
                return null;
            }

            TestVariation ffTestVariation;
            MethodInfo generateEpmMappingsMethod = testcase.GetType().GetMethod("GenerateEpmMappings");
            FilterResourcesLambda = GenerateLambda();

            TestFunc testMethodAction = delegate()
            {
                bool thisTestFailed = true;

                SkipInvalidRuns();

                try
                {
                    generateEpmMappingsMethod.Invoke(testcase, new object[]{
                    KeepInContent,//bool pKeepInContent,
                    RemoveUnEligibleTypes,//bool pRemoveUnEligibleTypes,
                    GenerateClientTypes,//bool pGenerateClientTypes,
                    MapToAtomElements,//bool pMapPropertiesToAtomElements,
                    MapToNonAtomElements,//bool pMapPropertiesToNonAtomElements,
                    GenerateServerEPMMappings,//bool GenerateServerEPMMappings,
                    KeepSameNamespace,//bool KeepSameNamespace,
                    IncludeComplexTypes,//bool  IncludeComplexTypes
                    PreDefinedPaths  ,//string[] PreDefinedPaths
                    FilterResourcesLambda //Func<ResourceType, bool> pFilterResourcesLambda
                });
                    method.Invoke(testcase, null);


                    thisTestFailed = AstoriaTestLog.FailureFound;
                }
                catch (Microsoft.OData.Client.DataServiceClientException dsException)
                {
                    if (dsException.InnerException != null)
                        AstoriaTestLog.WriteLine(dsException.InnerException.Message);
                }
                catch (TestFailedException testException)
                {

                    AstoriaTestLog.WriteLine("Test failed  with :{0}", testException.Message);
                    AstoriaTestLog.WriteLine("Repro Details\r\nClient Code");
                }
                finally
                {
                    //Disabling this as OOM errors prevent file copying
                    thisTestFailed = false;
                    #region //Clean out all the facets after the test , so that the next test is clear
                    object workspacePointer = null;
                    PropertyInfo testWorkspacesProperty = testcase.Parent.GetType().GetProperty("Workspaces");
                    workspacePointer = testcase.Parent;
                    if (testWorkspacesProperty == null)
                    {
                        testWorkspacesProperty = testcase.GetType().GetProperty("Workspaces");
                        workspacePointer = testcase;
                    }
                    Workspaces testWorkspaces = (Workspaces)testWorkspacesProperty.GetValue(workspacePointer, null);
                    foreach (Workspace workspace in testWorkspaces)
                    {
                        if (thisTestFailed)
                        {
                            string testFailureReproPath = String.Format("FF_Failure_{0}", System.Guid.NewGuid().ToString());
                            testFailureReproPath = System.IO.Path.Combine(Environment.CurrentDirectory, testFailureReproPath);
                            IOUtil.CopyFolder(workspace.DataService.DestinationFolder, testFailureReproPath, true);
                            AstoriaTestLog.WriteLine("Test failed , Repro available at : {0} ", testFailureReproPath);
                        }
                 
                        if (!(workspace is EdmWorkspace))
                        {
                            workspace.GenerateClientTypes = true;
                            workspace.GenerateServerMappings = false;
                            workspace.ApplyFriendlyFeeds();
                        }
                    }
                    #endregion

                }
            };
            ffTestVariation = new TestVariation(testMethodAction);
            if (testVarAttrib != null)
            {
                ffTestVariation.Params = testVarAttrib.Params;
                ffTestVariation.Param = testVarAttrib.Param;
                ffTestVariation.Desc = String.Format(
                "{3} , {0},{1},{2},{4}{5}{6}{7}",
                GenerateClientTypes ? "Client Mapped" : "",
                GenerateServerEPMMappings ? "Server Mapped" : "",
                MapToAtomElements ? "Mapped to Atom" : "Mapped to Non-Atom Elements",
                testVarAttrib.Desc, InheritanceFilter.ToString(),
                KeepSameNamespace ? "Same Namespace" : "Different NameSpaces",
                PreDefinedPaths != null ? ",PredefinedPaths" : "", KeepInContent.ToString());
                if (AstoriaTestProperties.MaxPriority == 0)
                {
                    ffTestVariation.Id = 1;
                }
                else
                {
                    ffTestVariation.Id = idCounter++;
                }
                ffTestVariation.Priority = this.Priority;
            }
            return ffTestVariation;

        }

        private static bool IsSupportedHost()
        {
            return AstoriaTestProperties.Host != Host.WebServiceHost;
        }

        private static bool IsSupportedDataProvider()
        {
            return AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.Edm)
                   ||
                   AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.LinqToSql)
                   ||
                   AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr);
        }
        static int idCounter = 1;
    }
}
