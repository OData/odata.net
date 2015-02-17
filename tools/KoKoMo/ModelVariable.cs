//---------------------------------------------------------------------
// <copyright file="ModelVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Reflection;		    //MemberInfo
using System.Security.Permissions;
using System.Security;		

namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelVariable (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ModelVariableAttribute : ModelRangeAttribute
	{
    }

    ////////////////////////////////////////////////////////////////
	// ModelVariable
	//
	////////////////////////////////////////////////////////////////
	public class ModelVariable : ModelRange
	{
		//Note: We allow model variables to be simple fields, or properties.  Fields for simplicity
		//in value comparison, and properties for custom complex expressions (ie: any code).
		MemberInfo				    _info;
        Object                      _instance;

        //Note: Many variables (ie: methods/properties) are expensive, some have side-effects,
        //and many are used as requirements on numerous actions.  Instead of calling them for each
        //and every variable on each an every action/requirement/parameter, we only call them once.
        //We cache the value internally, and the engine resets the cache
        bool                        _cached         = false;
        Object                      _cachedvalue    = null;

        //Constructor
		public ModelVariable(Model model, MemberInfo info, ModelVariableAttribute attr)
			: this(model, model, info, info.Name, attr)
		{
        }

		public ModelVariable(Model model, Object instance, MemberInfo info, String name, ModelVariableAttribute attr)
			: base(model, attr)
		{
			_info			= info;
            _instance       = instance;
            _name           = name;

            if(attr != null)
            {
                //Infer values from the type, if not specified
			    if(_values.Count == 0 || attr.Type != null)
				    this.AddValuesFromType(attr, this.Type);
            }
			
			//BitMask
			//TODO: Better place to expand this, (incase values added later).
			if(this.BitMask)
				this.AddBitCombinations();
        }

		public override	string			Name
		{
			get { return _name;				    }
		}

		public override	Type			Type
		{
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            [SecuritySafeCritical]
			get 
			{	
				if(_info is FieldInfo)
					return ((FieldInfo)_info).FieldType;			//Field
                if(_info is MethodInfo)
                    return ((MethodInfo)_info).ReturnType;			//Method
                return ((PropertyInfo)_info).PropertyType;			//Property
			}
		}

        public virtual	object			CurrentValue
		{
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            [SecuritySafeCritical]
            get 
            {
                //Invoke (field, method, property)
                try
                {
	                if(_info is FieldInfo)
		                return ((FieldInfo)_info).GetValue(_instance);			//Field
                    else if(_info is MethodInfo)
                        return ((MethodInfo)_info).Invoke(_instance, null);		//Method (no parameters)
                    else
                        return ((PropertyInfo)_info).GetValue(_instance, null);	//Property
                }
                catch(Exception e)
                {
	                //Make this easier to debug
	                throw new ModelException(this, null, e);
                }
            }
        }

        public virtual	object			CachedValue
		{
			get 
			{ 
                if(_cached)
                    return _cachedvalue;


	            _cached = true;
	            _cachedvalue = this.CurrentValue;
	            return _cachedvalue;
			}
            set
            {
                _cached = false;
                _cachedvalue = value;
            }
		}

		public override void			Reload()
		{
		}
	}

    ////////////////////////////////////////////////////////////////
    // ModelVariables
    //
    ////////////////////////////////////////////////////////////////
    ///<summary>This class represents a collection of model variables</summary>
    public class ModelVariables : ModelRanges<ModelVariable>
    {
        //Constructors
        ///<summary>Default constructor</summary>
        public ModelVariables()
        {
        }

        public new virtual ModelVariables   Find(params string[] names)
        {
            return (ModelVariables)base.Find(names);
        }

        public new virtual ModelVariables   FindExcept(params string[] names)
        {
            return (ModelVariables)base.FindExcept(names);
        }

        public virtual ModelVariable        this[Type type]
        {
            //Find first, fail if not found, although make it easier to debug
            get
            {
                ModelVariables found = this.FindType(type);
                if (found == null || found.Count <= 0)
                    throw new IndexOutOfRangeException(this.Name + "['" + type + "'] is not found");
                return found.First;
            }
        }

        ///<summary>Finds a model variable in this collection of the specified type</summary>
        public virtual ModelVariables       FindType(Type type)
        {
            //Find a matching variable, of the specified type
            ModelVariables found = new ModelVariables();
            foreach (ModelVariable variable in this)
            {
                if (variable.Type == type)
                    found.Add(variable);
            }

            return found;
        }
    }
}
