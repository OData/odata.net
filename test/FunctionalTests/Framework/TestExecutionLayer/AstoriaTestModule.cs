//---------------------------------------------------------------------
// <copyright file="AstoriaTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Test.ModuleCore;    //TestModule
using Microsoft.Test.KoKoMo;
using System.Threading;
using System.Globalization;        //Modeling
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Net;
using System.Security.Policy;
using System.IO;
using System.Data.SqlClient;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// TestModule
	//
	////////////////////////////////////////////////////////   
    public abstract class AstoriaTestModule : TestModule
    {
        private Workspaces _workspaces = null;

        //Constructor
        public AstoriaTestModule()
        {
        }

        //Overrides
        public override void Init()
        {
            _workspaces = new Workspaces();

            //Redirect modeling output to our (debug) logger
            ModelTrace.Enabled = false;
            ModelTrace.Out = new TestLogWriter(TestLogFlags.Trace | TestLogFlags.Ignore);

            //Delegate
            base.Init();

            HashSet<string> interestingPropertyNames = new HashSet<string>()
            {
                "AstoriaBuild",
                "Host",
                "Client",
                "DataLayerProviderKinds",
                "HostAuthenicationMethod",
                "ServerVersion",
                "ClientVersion",
                "DesignVersion",
                "UseOpenTypes"
            };

            IEnumerable<PropertyInfo> properties = typeof(AstoriaTestProperties).GetProperties(BindingFlags.Public | BindingFlags.Static).OrderBy(p => p.Name);
            IEnumerable<PropertyInfo>[] subsets = new IEnumerable<PropertyInfo>[] 
            {
                properties.Where(p =>  interestingPropertyNames.Contains(p.Name)),
                properties.Where(p => !interestingPropertyNames.Contains(p.Name))
            };
            foreach (IEnumerable<PropertyInfo> subset in subsets)
            {
                foreach (PropertyInfo property in subset)
                {
                    Type type = property.PropertyType;
                    object value = property.GetValue(null, null);
                    if (type.IsValueType || type.IsPrimitive || type == typeof(string))
                        AstoriaTestLog.WriteLineIgnore(property.Name + ": " + value);
                    else if (type.IsArray)
                        AstoriaTestLog.WriteLineIgnore(property.Name + ": {" + string.Join(", ", ((Array)value).OfType<object>().Select(o => o.ToString()).ToArray()) + "}");
                }
                AstoriaTestLog.WriteLineIgnore("");
            }

            Workspace.CreateNewWorkspaceEvent += this.HandleWorkspaceCreationEvent;
        }

        public override void Terminate()
        {
            base.Terminate();

            // Cleanup workspaces
            if (AstoriaTestProperties.IsLabRun && _workspaces != null)
            {
                _workspaces.Dispose();
            }
        }

        protected override void OnEnter(TestMethod method)
        {
            if(method == TestMethod.Init)
                AstoriaTestProperties.Init();
        }

        protected override void OnLeave(TestMethod method)
        {
            if(method == TestMethod.Terminate)
                AstoriaTestProperties.Dispose();
        }

        protected override TestResult HandleException(Exception e)
        {
            return AstoriaTestLog.HandleException(e);
        }

        private static IEnumerable<TestItem> Descendents(TestItem item)
        {
            return item.Children.Cast<TestItem>().Union(item.Children.Cast<TestItem>().SelectMany(c => Descendents(c)));
        }

        /// <summary>
        /// Get nested test cases
        /// </summary>
        /// <param name="type">Root type of a test module</param>
        /// <returns>all the test items in an array list</returns>
        private ArrayList GetNestedTypes(Type type)
        {
            ArrayList types = new ArrayList();

            //Recurse
            if (type.BaseType != null)
                types = GetNestedTypes(type.BaseType);

            //Nested Types
            foreach (Type t in type.GetNestedTypes())
                types.Add(t);

            return types;
        }

        /// <summary>
        /// Add specific permissions to the permission set based on what test is running. Currently concurrency client test and databinding client test require additional permissions 
        /// </summary>
        /// <param name="ps"></param>
        private void AddPermissionByTest(PermissionSet ps)
        {
            // concurrency client test requires permissions to write files in IIS wwwroot directory.
            if (this.Name == "Astoria.Test.Client.Concurrency")
            {
                string iisPath = TrustedMethods.GetIISRootPath();
                FileIOPermission perm = new FileIOPermission(FileIOPermissionAccess.AllAccess, iisPath);
                ps.AddPermission(perm);
            }
            // databinding test requires reflection permission with member access flag
            else if (this.Name == "Astoria.Test.Client.ServerDrivenPaging")
            {
                string iisPath = TrustedMethods.GetIISRootPath();
                FileIOPermission perm1 = new FileIOPermission(FileIOPermissionAccess.AllAccess, iisPath);
                ps.AddPermission(perm1);
            }
            else if (this.Name == "Astoria.Test.FriendlyFeedsClient")
            {
                string iisPath = TrustedMethods.GetIISRootPath();
                FileIOPermission perm1 = new FileIOPermission(FileIOPermissionAccess.AllAccess, iisPath);
                ps.AddPermission(perm1);
            }

        }

        /// <summary>
        /// return the appdomain with permission set in medium trust level
        /// </summary>
        /// <returns> appdomain</returns>
        private AppDomain GetCustomizedAppDomain()
        {
            AppDomainSetup setup = new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory };
            setup.AppDomainInitializer = SetupAppDomain;
            var ps = new PermissionSet(PermissionState.None);
            StreamReader fs = new StreamReader("AstoriaClientTest_MediumTrustPermissionSet.xml");
            string strPerm = fs.ReadToEnd();
            strPerm = strPerm.Replace("$AppDir$", Environment.CurrentDirectory);
            ps.FromXml(SecurityElement.FromString(strPerm));
            AddPermissionByTest(ps);

            StrongName framework = Assembly.GetExecutingAssembly().Evidence.GetHostEvidence<StrongName>();
            StrongName moduleCore = typeof(TestItem).Assembly.Evidence.GetHostEvidence<StrongName>();
            StrongName kokomo = typeof(ModelEngineOptions).Assembly.Evidence.GetHostEvidence<StrongName>();
            StrongName[] sn = { framework, moduleCore, kokomo };

            AppDomain myDomain = AppDomain.CreateDomain("myDomain", null, setup, ps, sn);
            return myDomain;
        }

        private static void SetupAppDomain(string[] args)
        {
            TestLog.Internal = new LtmContext() as ITestLog;
            TestInput.Properties = new TestProps(new LtmContext() as ITestProperties);
        }

        protected override void DetermineFilters()
        {
            if (AstoriaTestProperties.ServiceTrustLevel != TrustLevel.Medium)
                base.DetermineFilters();
        }

        protected override void DetermineChildren()
        {
            // Use previous settings if  client partial trust level is not medium
            // We only perform client partial-trust test here
            if (AstoriaTestProperties.ServiceTrustLevel != TrustLevel.Medium || (!this.Name.Contains("Client")))
            {
                base.DetermineChildren();
            }

            else
            {
                AppDomain myDomain = this.GetCustomizedAppDomain();
                ArrayList types = this.GetNestedTypes(this.GetType());
                foreach (Type type in types)
                {
                    //Were only interested in objects that inherit from TestCase.
                    //NOTE: We need to filter other objects, since some internal VB classes throw trying 
                    //to get Attributes
                    if (!type.IsSubclassOf(typeof(TestCase)))
                        continue;

                    //Every class that has a TestCase attribute, IS a TestCase
                    foreach (TestCaseAttribute attr in type.GetCustomAttributes(typeof(TestCaseAttribute), false/*inhert*/))
                    {
                        //Create this class (call the constructor with no arguments)
                        // Create testcase instance in another app domain with desired permission set.
                        TestCase testcase = (TestCase)myDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

                        if (attr.Name == null)
                            attr.Name = type.Name;
                        testcase.Attribute = attr;
                        this.AddChild(testcase);
                    }
                }

                //Sort
                //Default sort is based upon IComparable of each item
                Children.Sort();
            }


            if (AstoriaTestProperties.RuntimeEnvironment != TestRuntimeEnvironment.CheckinSuites)
            {
                // ensure that we don't get 0/0 = 0% results due to empty test cases
                foreach (TestItem child in this.Children.Cast<TestItem>().Where(c => !Descendents(c).OfType<TestVariation>().Any()))
                {
                    child.Children.Clear();

                    TestFunc func = new TestFunc(delegate() { throw new TestSkippedException("Placeholder"); });
                    TestVariation variation = new TestVariation("Placeholder for module with no tests", func);
                    variation.Attribute = new VariationAttribute("Placeholder for module with no tests")
                    {
                        Id = 1,
                        Implemented = true,
                        Priority = AstoriaTestProperties.MinPriority
                    };

                    variation.Parent = child;
                    child.AddChild(variation);
                }
            }
        }

        protected void HandleWorkspaceCreationEvent(object sender, NewWorkspaceEventArgs e)
        {
            _workspaces.Add(e.Workspace);
        }
    }
}
