//---------------------------------------------------------------------
// <copyright file="TestProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;			//IEnumerator
using System.Collections.Generic;   //List<T>
using System.Security.Permissions;
using System.Security;   

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestInput
	//
	////////////////////////////////////////////////////////////////
	public class TestInput
	{
		//Data
		static TestProps	    _properties;
		static String			_initstring;
		static String			_commandline;

		//Constructor
		static public	TestProps	        Properties
		{
			get 
			{
                //Delegate for now...
                if(_properties == null)
                    TestInput.Properties = new TestProps(new TestProperties());
                return _properties;
			}
            [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
            [SecuritySafeCritical]
			set 
            { 
                //TODO: This code should eventually be in ltm and then we simply only use it's collection
				//_properties = value;
				
				//For now, we do our own logic
                //Clear between each run
                _properties = new TestProps(new TestProperties());
                
                //InitString keywords
                String initstring = value["Alias/InitString"];
                if(initstring != null)
                {
                    _properties["Alias/InitString"] = initstring;
                    //Keywords
                    Hashtable keywords = KeywordParser.ParseKeywords(initstring);
                    foreach(String key in keywords.Keys)
                        _properties["Alias/InitString/" + key] = keywords[key] as String;
                }
                
                //CommandLine
                String commandline = value["CommandLine"] ?? Environment.CommandLine;
                if(commandline != null)
                {
                    _properties["CommandLine"] = commandline;
                    //Options
                    KeywordParser.Tokens tokens = new KeywordParser.Tokens();
                    tokens.Equal = " ";
                    tokens.Seperator = "/";
                    Hashtable options = KeywordParser.ParseKeywords(commandline, tokens);
                    foreach (String key in options.Keys)
                        _properties["CommandLine/" + key] = options[key] as String;
                }
            }
		}

		static internal	void			    Dispose()
		{
			//Reset the info.  
			//Since this is a static class, (to make it simplier to access from anywhere in your code)
			//we need to reset this info everytime a test is run - so if you don't select an alias
			//the next time it doesn't use the previous alias setting - ie: ProviderInfo doesn't 
			//get called when no alias is selected...
			_properties		    = null;
			_initstring		    = null;
			_commandline	    = null;
		}

        static public	string			    InitString
		{
			get 
			{ 
				//Useful typed getter
				if(_initstring == null)
					_initstring = TestInput.Properties["Alias/InitString"];
				return _initstring;	
			}
		}

        static public	string			    CommandLine
		{
            get
            {
			    //Useful typed getter
                if (_commandline == null)
                    _commandline = TestInput.Properties["CommandLine"];
                return _commandline;
            }
		}

        static public	String			    Filter
		{
			get 
            {
                if(TestInput.Properties != null)
                    return TestInput.Properties["CommandLine/Filter"];		
                return null;
            }
		}
		
		static public	String			    MaxPriority
		{
			get 
            { 
                if(TestInput.Properties != null)
                    return TestInput.Properties["CommandLine/MaxPriority"];	
                return null;
            }
		}
    }

    ////////////////////////////////////////////////////////////////
	// TestProps
	//
	////////////////////////////////////////////////////////////////
	public class TestProps : ITestProperties, IEnumerable, IEnumerator
	{
		//Data
		protected ITestProperties	_internal;
		protected int               _enum = -1;

		//Constructor
	    public TestProps(ITestProperties properties)
	    {
            _internal = properties;
        }

        //Accessors
        public virtual ITestProperties  Internal
        {
            get { return _internal;     }
        }
        
        public virtual int              Count
        {
            get 
            { 
                if(_internal != null)
                    return _internal.Count;                           
                return 0;
            }
        }

        public virtual TestProp         this[int index]
        {
            get 
            { 
                ITestProperty property = _internal.GetItem(index);
                if(property != null)
                    return new TestProp(property);    
                return null;
            }
        }

        public virtual String           this[String name]
        {
            get 
            { 
                ITestProperty property = _internal.Get(name);
                if(property != null)
                    return StringEx.ToString(property.Value);
                return null;
            }
            set 
            { 
                this.Add(name).Value = value;
            }
        }

        public virtual TestProp         Get(String name)
        {
            ITestProperty property = _internal.Get(name);
            if(property != null)
                return new TestProp(property);
            return null;
        }

        public virtual TestProp         Add(String name)
        {
            return new TestProp(_internal.Add(name));
        }

        public virtual void             Remove(String name)
        {
            _internal.Remove(name);
        }

        public virtual IEnumerator      GetEnumerator()
        {
            return this;
        }

        public virtual bool             MoveNext()
        {
            if(_enum+1 >= this.Count)
                return false;
            _enum++;
            return true;
        }

        public virtual Object           Current
        {
            get { return this[_enum];      }
        }

        public virtual void             Reset()
        {
            _enum = -1;
        }

        public virtual void             Clear()
        {
            if(_internal != null)
                _internal.Clear();
        }

        ITestProperty                   ITestProperties.Add(String name)
        {
            return _internal.Add(name);
        }

        ITestProperty                   ITestProperties.Get(String name)
        {
            return _internal.Get(name);
        }

        ITestProperty                   ITestProperties.GetItem(int index)
        {
            return _internal.GetItem(index);      
        }
    }

    ////////////////////////////////////////////////////////////////
	// TestProp
	//
	////////////////////////////////////////////////////////////////
	public class TestProp : ITestProperty, IEnumerable
	{
		//Data
		protected ITestProperty	    _internal;

		//Constructor
	    public TestProp(ITestProperty property)
	    {
            _internal = property;
        }

        //Accessors
        public virtual ITestProperty    Internal
        {
            get { return _internal;             }
        }

        public virtual String           Name
        {
            get { return _internal.Name;        }
        }

        public virtual String           Desc
        {
            get { return _internal.Desc;        }
        }

        public virtual TestPropertyFlags    Flags
        {
            get { return _internal.Flags;       }
            set { _internal.Flags = value;      }
        }

        public virtual Object               Value
        {
            get { return _internal.Value;           }
            set { _internal.set_Value(ref value);   }
        }

        public virtual TestProps            Children
        {
            get { return new TestProps(_internal.Children); }
        }

        public virtual IEnumerator          GetEnumerator()
        {
            return this.Children;
        }

        void                                ITestProperty.set_Value(ref object value)
        {
            _internal.set_Value(ref value);
        }

        ITestProperties                     ITestProperty.Children
        {
            get { return _internal.Children;                            }
        }

        ITestProperties                     ITestProperty.Metadata
        {
            get { return _internal.Metadata;                            }
        }

    }

    ////////////////////////////////////////////////////////////////
	// TestProperty
	//
	////////////////////////////////////////////////////////////////
    [Serializable]
	public class TestProperty : ITestProperty
	{
        //Data
        protected   String              _name       = null;
        protected   String              _desc       = null;
        protected   Object              _value      = null;
        protected   TestPropertyFlags   _flags      = 0;
        protected   TestProperties      _metadata   = null;
        protected   TestProperties      _children   = null;

        //Constructor
        public TestProperty(String name, Object value)
        {
            _name   = name;
            _value  = value;
        }

        //Accessors
		public string				Name		
        { 
            get { return _name;     }
            set { _name = value;    }
        }
		
        public string				Desc
        { 
            get { return _desc;     }
            set { _desc = value;    }
        }

        public TestPropertyFlags   	Flags		
        { 
            get { return _flags;    }
            set { _flags = value;   }
        }

		public object				Value		
        { 
            get { return _value;    }
            set { _value = value;   }
        }

		public void 				set_Value(ref object value)
        { 
            _value = value;   
        }

        public ITestProperties		Metadata
        { 
            get { return _metadata; }
        }

		public ITestProperties		Children
        { 
            get { return _children; }
        }
    }

	////////////////////////////////////////////////////////////////
	// TestProperties
	//
	////////////////////////////////////////////////////////////////
    [Serializable]
    public class TestProperties : ITestProperties, IEnumerable
	{
		//Data
        protected List<TestProperty>   _list = null;

		//Constructor
		public	TestProperties()
		{
            _list = new List<TestProperty>();
		}

        //Methods
        public virtual int              Count
        {
            get { return _list.Count;                       }
        }

        public virtual TestProperty 	this[int index]
        {
            get { return _list[index];                      }
        }

        public virtual Object      		this[String name]
        {
            get
            {
                ITestProperty property = this.Get(name);
                if(property != null)
                    return property.Value;
                return null;
            }
            set { this.Add(name).Value = value;             }
        }

        public virtual int              IndexOf(string name)
        {
            int count = _list.Count;
            for(int i=0; i<count; i++)
            {
                if(String.Compare(_list[i].Name, name, true/*ignorecase*/)==0)
                    return i;
            }
            return -1;
        }

        public virtual IEnumerator		GetEnumerator()
		{ 
			return _list.GetEnumerator();
		}
        
        ITestProperty                   ITestProperties.GetItem(int index)
        {
            return this[index];          
        }

        public virtual ITestProperty	Get(String name)
        {
            int index = this.IndexOf(name);
            if(index >= 0)
                return _list[index];
            return null;
        }

		ITestProperty	                ITestProperties.Add(String name)
        {
            return (TestProperty)Add(name);
        }
        
        public virtual TestProperty	    Add(String name)
        {
            //Exists
            int index = this.IndexOf(name);
            if(index >= 0)
                return _list[index];

            //Otherwise add
            TestProperty property = new TestProperty(name, null);
            _list.Add(property);
            return property;
        }

        public virtual void             Remove(String name)
        {
            int index = this.IndexOf(name);
            if(index >= 0)
                _list.RemoveAt(index);
        }

        public virtual void        	    Clear()
        {
            _list.Clear();
        }
    }
}
