//---------------------------------------------------------------------
// <copyright file="TestItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;		//BindingFlags
using System.Collections;		//IEnumerable
using System.Security.Permissions;
using System.Security;		

namespace Microsoft.Test.ModuleCore
{
    ////////////////////////////////////////////////////////////////
    // TestItem
    //
    ////////////////////////////////////////////////////////////////
    public abstract class TestItem : MarshalByRefObject, ITestItem, IComparable
    {
        //Data
        protected TestItem _parent;
        protected TestItems _children;
        protected TestAttribute _attribute;
        protected TestProperties _metadata;
        protected TestType _testtype;
        protected TestFlags _testflags;
        protected TestItem _currentchild;

        //Constructor
        public TestItem(string name, string desc, TestType testtype)
        {
            if (name != null)
                this.Name = name;
            if (this.Name == null)
                this.Name = this.GetType().Name;
            if (desc != null)
                this.Desc = desc;
            _testtype = testtype;
        }

        //Accessors
        public virtual TestAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                    _attribute = CreateAttribute();
                return _attribute;
            }
            set { _attribute = value; }
        }

        protected abstract TestAttribute CreateAttribute();

        //Note: These are just a mere convience to access the attribute values
        //for this particular object.  Also note that for non-attribute based
        //scenarios (dynamic), the attribute class will be created just to hold
        //the values
        public object Param
        {
            get { return Attribute.Param; }
            set { Attribute.Param = value; }
        }

        public object[] Params
        {
            get { return Attribute.Params; }
            set { Attribute.Params = value; }
        }

        public int Id
        {
            get { return Attribute.Id; }
            set { Attribute.Id = value; }
        }

        public int Order
        {
            get { return Attribute.Id; }
            set { Attribute.Id = value; }
        }

        public int Priority
        {
            get { return Attribute.Priority; }
            set { Attribute.Priority = value; }
        }

        public string Guid
        {
            get { return Attribute.Guid; }
            set { Attribute.Guid = value; }
        }

        public TestType Type
        {
            get { return _testtype; }
            set { _testtype = value; }
        }

        public TestFlags Flags
        {
            get { return _testflags; }
            set { _testflags = value; }
        }

        public string Name
        {
            get { return Attribute.Name; }
            set { Attribute.Name = value; }
        }

        public string Desc
        {
            get { return Attribute.Desc; }
            set { Attribute.Desc = value; }
        }

        public string Purpose
        {
            get { return Attribute.Purpose; }
            set { Attribute.Purpose = value; }
        }

        public bool Implemented
        {
            get { return Attribute.Implemented; }
            set { Attribute.Implemented = value; }
        }

        public string Owner
        {
            get { return Attribute.Owner; }
            set { Attribute.Owner = value; }
        }

        public string[] Owners
        {
            get { return Attribute.Owners; }
            set { Attribute.Owners = value; }
        }

        public string Version
        {
            get { return Attribute.Version; }
            set { Attribute.Version = value; }
        }

        public bool Skipped
        {
            get { return Attribute.Skipped; }
            set { Attribute.Skipped = value; }
        }

        public bool Error
        {
            get { return Attribute.Error; }
            set { Attribute.Error = value; }
        }

        public bool Manual
        {
            get { return Attribute.Manual; }
            set { Manual = value; }
        }

        public SecurityFlags Security
        {
            get { return Attribute.Security; }
            set { Attribute.Security = value; }
        }

        public bool Inheritance
        {
            get { return Attribute.Inheritance; }
            set { Attribute.Inheritance = value; }
        }

        public string FilterCriteria
        {
            get { return Attribute.FilterCriteria; }
            set { Attribute.FilterCriteria = value; }
        }

        public string Language
        {
            get { return Attribute.Language; }
            set { Attribute.Language = value; }
        }

        public string[] Languages
        {
            get { return Attribute.Languages; }
            set { Attribute.Languages = value; }
        }

        public string Xml
        {
            get { return Attribute.Xml; }
            set { Attribute.Xml = value; }
        }

        public bool Stress
        {
            get { return Attribute.Stress; }
            set { Attribute.Stress = value; }
        }

        public int Threads
        {
            get { return Attribute.Threads; }
            set { Attribute.Threads = value; }
        }

        public int Repeat
        {
            get { return Attribute.Repeat; }
            set { Attribute.Repeat = value; }
        }

        public int Timeout
        {
            get { return Attribute.Timeout; }
            set { Attribute.Timeout = value; }
        }

        public string Filter
        {
            get { return Attribute.Filter; }
            set { Attribute.Filter = value; }
        }

        public TestItem Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        ITestItems ITestItem.Children
        {
            get { return this.Children; }
        }

        public TestItems Children
        {
            get
            {
                //Deferred Creation of the children.
                if (_children == null)
                    _children = new TestItems();
                return _children;
            }
            set { _children = value; }
        }

        public TestItem CurrentChild
        {
            //Currently executing child
            //Note: We do this so that within global functions can know which 
            //testcase/variation were in, without having to pass this state from execute arround
            get { return _currentchild; }
            set { _currentchild = value; }
        }

        ITestProperties ITestItem.Metadata
        {
            get { return this.Metadata; }
        }

        public TestProperties Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new TestProperties();

                    //Note: The default implemention simply exposes the attribute assoicated with
                    //this test as properties (meta-data).  However you could expose any additional 
                    //properties, either attribute, or even alter or override this default altogether.
                    foreach (PropertyInfo propinfo in this.Attribute.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        //Obtain the name
                        string name = propinfo.Name;
                        string desc = name;
                        object defaultvalue = null;
                        int id = 1;
                        TestPropertyFlags flags = TestPropertyFlags.Read;
                        //						Guid guid

                        //Obtain meta-data about this property
                        //This is simply done using an attribute on the attribute property.  You could obtain the meta-data
                        //different and populate the property object yourself, however the default uses attributes for simplicitly
                        object[] attributes = propinfo.GetCustomAttributes(typeof(TestPropertyAttribute), true/*inhert*/);

                        //Note: if the property isn't attributed, it's probably not meant to be included 
                        //as a testproperty, and just a generic helper of some sort
                        if (attributes == null || attributes.Length < 1)
                            continue;

                        //Loop over attributes
                        foreach (TestPropertyAttribute attr in attributes)
                        {
                            //Name
                            if (attr.Name != null)
                                name = attr.Name;

                            //Desc
                            if (attr.Desc != null)
                                desc = attr.Desc;

                            //Id
                            if (attr.Id >= 0)
                                id = attr.Id;

                            //Guid
                            //TODO:
                            //							if(attr.Guid)
                            //								guid = attr.Guid;

                            //Flags
                            if (attr.Flags != 0)
                                flags = attr.Flags;

                            //Default
                            if (attr.DefaultValue != null)
                                defaultvalue = attr.DefaultValue;
                        }

                        //Obtain the value
                        object value = null;
                        try
                        {
                            //Note: there could be exceptions thrown accessing the property
                            value = propinfo.GetValue(this.Attribute, null);
                        }
                        catch (Exception e)
                        {
                            //Continue processing other properties, even if this one fails for some reason.
                            TestLog.WriteLineIgnore(e.ToString());
                        }

                        //If the value is an array, then we override the perfered serialization
                        bool array = (value != null && value.GetType().HasElementType && value is IEnumerable);
                        if (array)
                            flags |= TestPropertyFlags.MultipleValues;

                        //If the value is the same as the default value, update the flags
                        if (value != null && value.Equals(defaultvalue))
                            flags |= TestPropertyFlags.DefaultValue;

                        //Value (arrays as seperate properties)
                        TestProperty property = _metadata.Add(name);
                        property.Value = value;
                        property.Desc = desc;
                        property.Flags = flags;
                        //property.Order	= id;
                        //property.Guid	= guid;
                    }
                }

                return _metadata;
            }
        }

        protected virtual void UpdateAttributes()
        {
        }

        protected virtual void DetermineChildren()
        {
            //Dynamically figure out what the contained testcases...
            foreach (Type type in TypeEx.GetNestedTypes(this.GetType(), true/*inherit*/))
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
                    TestCase testcase = (TestCase)Activator.CreateInstance(type);
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
        public T GetParamFromParams<T>(int index)
        {
            if (this.Params == null)
                throw new InvalidOperationException("No Params exist to get from");
            if (index >= this.Params.Length)
                throw new ArgumentOutOfRangeException("There are " + this.Params.Length + " in the Params collection, the index:" + index.ToString() + " is out of the range");
            object o = this.Params[index];
            if (o.GetType().Equals(typeof(T)) || o.GetType().IsSubclassOf(typeof(T)))
                return (T)o;
            else
                throw new InvalidOperationException("Type:" + o.GetType().FullName + " is incompatible with type: " + typeof(T).FullName);
        }
        public void AddChild(TestItem child)
        {
            //Setup the parent
            child.Parent = this;

            //Inheritance of attributes
            child.Attribute.Parent = this.Attribute;

            //Adjust the Id (if not set)
            if (child.Id <= 0)
                child.Id = Children.Count + 1;

            //Only add implemented items
            //Note: we still increment id counts, to save the 'spot' once they are implemented
            if (child.Implemented)
            {
                //Determine any children of this node (before adding it)
                //Note: We don't call 'determinechildren' from within the (testcase) constructor
                //since none of the attributes/properties are setup until now.  So as soon as we
                //setup that information then we call determinechildren which when implemented
                //for dynamic tests can now look at those properties (otherwise they wouldn't be setup
                //until after the constructor returns).
                child.DetermineChildren();

                //Add it to our list...
                Children.Add(child);
            }
        }

        protected TestItem FindChild(Type type)
        {
            //Compare
            if (type == this.GetType())
                return this;

            //Otherwise recursive
            foreach (TestItem item in this.Children)
            {
                TestItem found = item.FindChild(type);
                if (found != null)
                    return found;
            }

            return null;
        }

        public virtual void Init()
        {
            //Note: This version is the only override-able Init, since the other one 
            //automatically handles the exception you might throw from this function
            //Note: If you override this function, (as with most overrides) make sure you call the base.
        }

        TestResult ITestItem.Init()
        {
            //Enter
            OnEnter(TestMethod.Init);

            TestResult result = TestResult.Passed;
            if (Parent != null)
                Parent.CurrentChild = this;

            //Obtain the error object (to prime it)
            try
            {
                if (_testtype == TestType.TestModule)
                {
                    //Clear any previous existing info (in the static class).
                    //Note: We actually have to clear these "statics" since they are not cleaned up
                    //until the process exits.  Which means that if you run again, in an apartment model
                    //thread it will try to release these when setting them which is not allowed to 
                    //call a method on an object created in another apartment
                    if (TestLog.Internal == null)
                    {
                        TestLog.Internal = new LtmContext() as ITestLog;
                        TestInput.Properties = new TestProps(new LtmContext() as ITestProperties);
                    }
                }
            }
            catch (Exception e)
            {
                result = HandleException(e);
            }

            //NOTE: Since exceptions are a way-of-life in COOL, and the fact that they are just
            //caught by the runtime before passed back to LTM, we lose the entire stack and just 
            //an unknown error code is returned.

            //To help solve this we will wrap the call in a try catch and actually
            //log the exception to the LTM output window... 
            try
            {
                //Skipped
                if (this.Skipped || !this.Implemented)
                {
                    result = TestResult.Skipped;
                }
                else
                {
                    this.Init();
                }
			}
            catch (Exception e)
            {
                result = HandleException(e);
            }

            //Leave
            OnLeave(TestMethod.Init);
            return result;
        }

        public virtual void Execute()
        {
            //Note: This version is the only override-able version, since the other one 
            //automatically handles the exception you might throw from this function
            //Note: If you override this function, (as with most overrides) make sure you call the base.
        }

        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        TestResult ITestItem.Execute()
        {
            //Enter
            OnEnter(TestMethod.Execute);
            TestResult result = TestResult.Passed;

            try
            {
                //Skipped
                if (this.Skipped || !this.Implemented)
                    result = TestResult.Skipped;
                else
                    this.Execute();
            }
            catch (Exception e)
            {
                result = HandleException(e);
            }

            //Leave
            OnLeave(TestMethod.Execute);
            return result;
        }

        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            System.Runtime.Remoting.Lifetime.ILease lease = (System.Runtime.Remoting.Lifetime.ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == System.Runtime.Remoting.Lifetime.LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.FromMinutes(30);
                lease.RenewOnCallTime = TimeSpan.FromMinutes(30);
            }
            return null;
        }

        public virtual void Terminate()
        {
            //Note: This version is the only override-able Terminate, since the other one 
            //automatically handles the exception you might throw from this function
            //Note: If you override this function, (as with most overrides) make sure you call the base.
        }

        TestResult ITestItem.Terminate()
        {
            //Enter
            OnEnter(TestMethod.Terminate);
            TestResult result = TestResult.Passed;

            //NOTE: Since exceptions are a way-of-life in C#, and the fact that they are just
            //caught by the runtime before passed back to LTM, we lose the entire stack and just 
            //an unknown error code is returned.

            //To help solve this we will wrap the call in a try catch and actually
            //log the exception to the LTM output window... 
            try
            {
                //Skipped
                if (this.Skipped || !this.Implemented)
                    result = TestResult.Skipped;
                else
                    this.Terminate();

                //Before exiting make sure we reset our CurChild to null, to prevent incorrect uses 
                if (Parent != null)
                    Parent.CurrentChild = null;
            }
            catch (Exception e)
            {
                HandleException(e);
            }

            try
            {
                //Clear any previous existing info (in the static class).
                if (_testtype == TestType.TestModule)
                {
                    //Note: We actually have to clear these "statics" since they are not cleaned up
                    //until the process exits.  Which means that if you run again, in an apartment model
                    //thread it will try to release these when setting them which is not allowed to 
                    //call a method on an object created in another apartment
                    TestInput.Dispose();
                    TestLog.Dispose();
                }
            }
            catch (Exception e)
            {
                result = HandleException(e);
            }

            //This is also a good point to hint to the GC to free up any unused memory
            //at the end of each TestCase and the end of each module.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Leave
            OnLeave(TestMethod.Terminate);
            return result;
        }

        protected virtual void OnEnter(TestMethod method)
        {
            //TODO: Events
        }

        protected virtual void OnLeave(TestMethod method)
        {
            //TODO: Events
        }

        protected virtual TestResult HandleException(Exception e)
        {
            //Note: override this if your product has specilized exceptions (ie: nesting or collections)
            //that you need to recurse over of print out differently
            return TestLog.HandleException(e);
        }

        public virtual int CompareTo(object o)
        {
            //Default comparison, name based.
            return this.Name.CompareTo(((TestItem)o).Name);
        }
    }

    ////////////////////////////////////////////////////////////////
    // TestItems
    //
    ////////////////////////////////////////////////////////////////
    [Serializable]
    public class TestItems : ArrayList, ITestItems
    {
        //Data

        //Constructor
        public TestItems()
        {
        }

        //ITestItems
        int ITestItems.Count
        {
            get { return this.Count; }
        }

        ITestItem ITestItems.GetItem(int index)
        {
            return (ITestItem)this[index];
        }
    }
}
