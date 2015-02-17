//---------------------------------------------------------------------
// <copyright file="TestAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.Test.ModuleCore
{
 	////////////////////////////////////////////////////////////////
	// SecurityFlags
	//
	////////////////////////////////////////////////////////////////
	public enum SecurityFlags
	{
		None		= 0,
		FullTrust	= 1,
		ThreatModel = 2,
	};
	
	////////////////////////////////////////////////////////////////
	// TestAttribute (attribute)
	//
	////////////////////////////////////////////////////////////////
    [Serializable()]
	public abstract class TestAttribute : Attribute
	{
		//Data
		protected	string				_name;
		protected	string				_desc;
		protected	object[]			_params;
		protected	int					_id;
		protected	string				_guid;
		protected   bool				_inheritance	= true;
		protected   TestAttribute		_parent			= null;
		protected	string				_filter;
		protected	string				_version;
		
		//Allows Inhertiance (ie: object to determine if ever been set)
		protected	int?				_priority;		//Allows Inhertiance
		protected	string			    _purpose;		//Allows Inhertiance
		protected	bool?				_implemented;	//Allows Inhertiance
		protected	string[]			_owners;		//Allows Inhertiance
		protected	string[]			_areas;		    //Allows Inhertiance
		protected	bool?				_skipped;		//Allows Inhertiance
		protected	bool?				_error;			//Allows Inhertiance
		protected	bool?				_manual;		//Allows Inhertiance
		protected	SecurityFlags?		_security;		//Allows Inhertiance
		protected	string				_filtercriteria;//Allows Inhertiance
		protected	string[]			_languages;		//Allows Inhertiance
		protected	string				_xml;			//Allows Inhertiance
		protected	int?				_timeout;		//Allows Inhertiance
		protected	int?				_threads;		//Allows Inhertiance
		protected	int?				_repeat;		//Allows Inhertiance
		protected	bool?				_stress;		//Allows Inhertiance

		//Constructors
		public			TestAttribute()
		{
		}

		public			TestAttribute(string desc)
		{
			Desc = desc;
		}

        public          TestAttribute(string desc, params Object[] parameters)
        {
            Desc    = desc;
            Params  = parameters;
        }
        
        //Accessors
		public	virtual string				Name
		{
			get { return _name;			}
			set { _name = value;		}
		}

		public	virtual string				Desc
		{
			get { return _desc;			}
			set { _desc = value;		}
		}

		public  virtual	int					Id
		{
			get { return _id;			}
			set { _id = value;			}
		}

		public  virtual	string				Guid
		{
			get { return _guid;			}
			set { _guid = value;		}
		}

		public  virtual	int					Priority
		{
			get 
			{ 
				if(_priority == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Priority;
					
					//Default
					return 2;
				}
				return (int)_priority;
			}
			set { _priority = value;												}
		}

        [TestProperty(Visible = true)]
        public virtual string               Owner
		{
			get 
			{ 
				if(_owners != null)
					return _owners[0];
				return null;
			}
			set 
			{ 
				if(_owners == null)
					_owners = new string[1];
				_owners[0] = value;	
			}
		}

        [TestProperty(Visible = true, MultipleValues=true)]
        public virtual string[]             Owners
		{
			get { return _owners;		}
			set { _owners = value;		}
		}

        [TestProperty(Visible = true)]
        public virtual string               Area
		{
			get 
			{ 
				if(_areas != null)
					return _areas[0];
				return null;
			}
			set 
			{ 
				if(_areas == null)
					_areas = new string[1];
				_areas[0] = value;	
			}
		}

        [TestProperty(Visible = true, MultipleValues=true)]
        public virtual string[]             Areas
		{
			get { return _areas;		}
			set { _areas = value;		}
		}

		public  virtual	string				Version
		{
			get { return _version;		}
			set { _version = value;		}
		}

		[TestProperty(Visible=false)]
		public  virtual	object				Param
		{
			get 
			{ 
				if(_params != null)
					return _params[0];
				return null;
			}
			set 
			{ 
				if(_params == null)
					_params = new object[1];
				_params[0] = value;	
			}
		}

		[TestProperty(Visible=false, MultipleValues=true)]
		public  virtual	object[]			Params
		{
			get { return _params;		}
			set { _params = value;		}
		}

		[TestProperty(Visible=false, DefaultValue=true)]
		public  virtual	bool				Inheritance
		{
			get { return _inheritance;	}
			set { _inheritance = value;	}
		}

		public  virtual	TestAttribute			Parent
		{
			get { return _parent;		}
			set { _parent = value;		}
		}

		public  virtual	string				Filter
		{
			get { return _filter;		}
			set { _filter = value;		}
		}

		[TestProperty(Visible=true)]
		public  virtual	string				Purpose
		{
			get 
			{ 
				if(_purpose == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Purpose;
					
					//Default
					return null;
				}
				return (string)_purpose;
			}
			set { _purpose = value;												    }
		}
		
		[TestProperty(Visible=true, DefaultValue=true)]
		public  virtual	bool				Implemented
		{
			get 
			{ 
				if(_implemented == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Implemented;
					
					//Default
					return true;	
				}
				return (bool)_implemented;
			}
			set { _implemented = value;												}
		}

		[TestProperty(Visible=true, DefaultValue=false)]
		public  virtual	bool				Skipped
		{
			get
			{
				if(_skipped == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Skipped;
					
					//Default
					return false;	
				}
				return (bool)_skipped;
			}
			set { _skipped = value;		}
		}

		[TestProperty(Visible=true, DefaultValue=false)]
		public  virtual	bool				Error
		{
			get
			{
				if(_error == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Error;
	
					//Default
					return false;	
				}
				return (bool)_error;
			}
			set { _error = value;		}
		}

		[TestProperty(Visible=true, DefaultValue=false)]
		public  virtual	bool				Manual
		{
			get
			{
				if(_manual == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Manual;
	
					//Default
					return false;	
				}
				return (bool)_manual;
			}
			set { _manual = value;		}
		}

		[TestProperty(Visible=true, DefaultValue=SecurityFlags.None)]
		public  virtual	SecurityFlags		Security
		{
			get
			{
				if(_security == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Security;
	
					//Default
					return SecurityFlags.None;	
				}
				return (SecurityFlags)_security;
			}
			set { _security = value;	}
		}

		[TestProperty(Visible=false, MultipleValues=true)]
		public  virtual	string				FilterCriteria
		{
			get
			{
				if(_filtercriteria == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.FilterCriteria;
					
					//Default
					return null;	
				}
				return (string)_filtercriteria;
			}
			set { _filtercriteria = value;		}
		}

		[TestProperty(Visible=false)]
		public  virtual	string				Language
		{
			get 
			{ 
				if(Languages != null)
					return Languages[0];
				return null;
			}
			set 
			{ 
				if(Languages == null)
					Languages = new string[1];
				Languages[0] = value;	
			}
		}

		[TestProperty(Visible=false, MultipleValues=true)]
		public  virtual	string[]			Languages
		{
			get
			{
				if(_languages == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Languages;
					
					//Default
					return null;	
				}
				return (string[])_languages;
			}
			set { _languages = value;	}
		}

		[TestProperty(Visible=false, MultipleValues=true)]
		public  virtual	string				Xml
		{
			get
			{
				if(_xml == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Xml;
					
					//Default
					return null;	
				}
				return (string)_xml;
			}
			set { _xml = value;			}
		}

		[TestProperty(Visible=true, DefaultValue=false)]
		public  virtual	bool				Stress
		{
			get
			{
				if(_stress == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Stress;
	
					//Default
					return false;	
				}
				return (bool)_stress;
			}
			set { _stress = value;	}
		}

		[TestProperty(Visible=true, DefaultValue=0)]
		public  virtual	int					Timeout
		{
			get
			{
				if(_timeout == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Timeout;
	
					//Default (infinite)
					return 0;
				}
				return (int)_timeout;
			}
			set { _timeout = value;	}
		}

		[TestProperty(Visible=true, DefaultValue=1)]
		public  virtual	int					Threads
		{
			get
			{
				if(_threads == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Threads;
	
					//Default (one thread)
					return 1;
				}
				return (int)_threads;
			}
			set { _threads = value;	}
		}

		[TestProperty(Visible=true, DefaultValue=0)]
		public  virtual	int					Repeat
		{
			get
			{
				if(_repeat == null)
				{
					//Inheritance
					if(Inheritance && _parent != null)
						return _parent.Repeat;
	
					//Default (no repeat)
					return 0;
				}
				return (int)_repeat;
			}
			set { _repeat = value;	}
		}
	}


 	////////////////////////////////////////////////////////////////
	// TestModule (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TestModuleAttribute : TestAttribute
	{
		//Data
		protected string		_created;
		protected string		_modified;

		//Constructors
		public			TestModuleAttribute()
			: base()
		{
		}

		public			TestModuleAttribute(string desc)
			: base(desc)
		{
		}

        public          TestModuleAttribute(string desc, params Object[] parameters)
            : base(desc, parameters)
        {
        }

        [TestProperty(Visible = true)]
		public  virtual	string		Created
		{
			get { return _created;		}
			set { _created = value;		}
		}

		[TestProperty(Visible=true)]
		public  virtual	string		Modified
		{
			get { return _modified;		}
			set { _modified = value;	}
		}
	}

 	////////////////////////////////////////////////////////////////
	// TestCase (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [Serializable()]
	public class TestCaseAttribute : TestAttribute
	{
		//Constructors
		public			TestCaseAttribute()
			: base()
		{
		}

		public			TestCaseAttribute(string desc)
			: base(desc)
		{
		}
    
        public          TestCaseAttribute(string desc, params Object[] parameters)
            : base(desc, parameters)
        {
        }
    }


 	////////////////////////////////////////////////////////////////
	// Variation (attribute)
	//
	////////////////////////////////////////////////////////////////
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
	public class VariationAttribute : TestAttribute
	{
		//Data

		//Constructors
		public			VariationAttribute() 
			: base()
		{
		}

		public			VariationAttribute(string desc)
			: base(desc)
		{
		}
    
        public          VariationAttribute(string desc, params Object[] parameters)
            : base(desc, parameters)
        {
        }
    }

    ////////////////////////////////////////////////////////////////
	// TestPropertyAttribute (attribute)
	//
	////////////////////////////////////////////////////////////////
    [AttributeUsage(AttributeTargets.Property)]
	public class TestPropertyAttribute : Attribute
	{
		//Data
		protected string					_name;
		protected string					_desc;
		protected Guid					    _guid;
		protected int						_id;
		protected object					_defaultvalue;
		protected TestPropertyFlags 	    _flags = TestPropertyFlags.Read;

		//Constructors
		public	TestPropertyAttribute()
		{
		}

		public  virtual	string	            Name
		{
			get { return _name;			}
			set { _name = value;		}
		}

		public  virtual	string	            Desc
		{
			get { return _desc;			}
			set { _desc = value;		}
		}

		public  virtual	Guid	            Guid
		{
			get { return _guid;			}
			set { _guid = value;		}
		}

		public  virtual	int		            Id
		{
			get { return _id;			}
			set { _id = value;			}
		}

		public  virtual	object	            DefaultValue
		{
			get { return _defaultvalue;	}
			set { _defaultvalue = value;}
		}

		public  virtual	bool	            Settable
		{
			get { return this.IsFlag(TestPropertyFlags.Write);			        }
			set { this.SetFlag(TestPropertyFlags.Write, value);		            }
		}

		public  virtual	bool	            Required
		{
			get { return this.IsFlag(TestPropertyFlags.Required);		        }
			set { this.SetFlag(TestPropertyFlags.Required, value);		        }
		}

		public  virtual	bool	            Inherit
		{
			get { return this.IsFlag(TestPropertyFlags.Inheritance);	        }
			set { this.SetFlag(TestPropertyFlags.Inheritance, value);	        }
		}

		public  virtual	bool	            Visible
		{
			get { return this.IsFlag(TestPropertyFlags.Visible);		        }
			set { this.SetFlag(TestPropertyFlags.Visible, value);		        }
		}

		public  virtual	bool	            MultipleValues
		{
			get { return this.IsFlag(TestPropertyFlags.MultipleValues);	        }
			set { this.SetFlag(TestPropertyFlags.MultipleValues, value);	    }
		}
/*
		public  virtual	bool	            Default
		{
			get { return this.IsFlag(TestPropertyFlags.DefaultValue);	        }
			set { this.SetFlag(TestPropertyFlags.DefaultValue, value);	        }
		}
*/
		public  virtual	TestPropertyFlags   Flags
		{
			get { return _flags;												}
			set { _flags = value;												}
		}

		//Helpers
		protected bool	                    IsFlag(TestPropertyFlags flags)
		{
			return (_flags & flags) == flags;
		}

		protected void	                    SetFlag(TestPropertyFlags flags, bool value)
		{
			if(value)
				_flags |= flags;		
			else
				_flags &= ~flags;
		}
	}

	////////////////////////////////////////////////////////////////
	// TestInclude (attribute)
	//
	////////////////////////////////////////////////////////////////
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TestIncludeAttribute : Attribute
	{
		//Data
		protected string    _name;
		protected string	_file;
		protected string	_files;
		protected string	_filter;

		//Constructors
		public	TestIncludeAttribute()
		{
		}

		public  virtual	string	Name
		{
			//Prefix for testcase names
			get { return _name;			}
			set { _name = value;		}
		}

		public  virtual	string	File
		{
			get { return _file;			}
			set { _file = value;		}
		}

		public  virtual	string	Files
		{
			//Search Pattern (ie: *.*)
			get { return _files;		}
			set { _files = value;		}
		}

		public  virtual	string	Filter
		{
			get { return _filter;		}
			set { _filter = value;		}
		}
	}
}
