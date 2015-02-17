//---------------------------------------------------------------------
// <copyright file="ModelRange.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Reflection;		//MemberInfo
using System.Collections;		//Hashtable


namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelValueConjunction
	//
	////////////////////////////////////////////////////////////////
	public enum ModelValueConjunction
	{
		And,
		Or,				
	}

	////////////////////////////////////////////////////////////////
	// ModelValueFlags
	//
	////////////////////////////////////////////////////////////////
	public enum ModelValueFlags
	{
		Bitmask		= 0x00000001,
	}

	////////////////////////////////////////////////////////////////
	// ModelRange (attribute)
	//
	////////////////////////////////////////////////////////////////
	public abstract class ModelRangeAttribute : ModelItemAttribute
	{
		//Data
		Type					_type			= null;
		internal ModelValues	_values			= new ModelValues();
		ModelValueConjunction	_conjunction	= ModelValueConjunction.And;
		string					_variable		= null;

		protected ModelRangeAttribute()
		{
		}

		protected ModelRangeAttribute(params object[] values)
		{
			this.Values = values;
		}

		//Accessors
		public ModelValueConjunction	Conjunction
		{
			get { return _conjunction;			}
			set { _conjunction = value;			}
		}

		public new ModelValueFlags		Flags
		{
			get { return (ModelValueFlags)base.Flags;	}
			set { base.Flags = (int)value;				}
		}

		public Type						Type
		{
			get { return _type;							}
			set { _type = value;						}
		}

		public object[]					Values
		{
			get { return _values.ToArray();				}
			set { _values.Add(value);					}		
	    }

		public object					Value
		{
			get 
			{ 
				if(_values.Count > 0)
					return _values.First.Value;	
				return null;					
			}
			set { _values.Add(new ModelValue(value, ModelValueOperator.Equal));		    }
		}

		public object					Equal
		{
			get { return this.Value;	}
			set { this.Value = value;   }
		}

		public object					Not
		{
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.NotEqual));		}
		}

		public object					GreaterThan
		{
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.GreaterThan));	}
		}

		public object					GreaterThanOrEqual
		{
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.GreaterThanOrEqual));	}
		}

		public object					LessThan
		{
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.LessThan));				}
		}

		public object					LessThanOrEqual
		{
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.LessThanOrEqual));				}
		}

		public object					Min
		{
			//Delegate (this is an alias over GreaterThanOrEqual)
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.GreaterThanOrEqual));	}
		}

		public object					Max
		{
			//Delegate (this is an alias over LessThanOrEqual)
			get { return this.Value;	}
			set { _values.Add(new ModelValue(value, ModelValueOperator.LessThanOrEqual));				}
		}

		public object[]					Any
		{
			get { return this.Values;	}
			set 
			{ 
				_values.Clear();
				_values.Add(value);	
				_conjunction= ModelValueConjunction.Or;
			}
		}

		public bool						BitMask
		{
			get { return IsFlag((int)ModelValueFlags.Bitmask);	}
			set { SetFlag((int)ModelValueFlags.Bitmask, value);	}
		}

		public string					Variable
		{
			get { return _variable;		}
			set { _variable = value;	}
		}
	}


	////////////////////////////////////////////////////////////////
	// ModelRange
	//
	////////////////////////////////////////////////////////////////
	public abstract class ModelRange : ModelItem
	{
		//Data

		//Note: We want to allow dynamic (instance) changing of the data, without affecting
		//the assoicated attribute.  So simply store that state as well
		internal ModelValues	_values			= new ModelValues();
		ModelValueConjunction	_conjunction	= ModelValueConjunction.And;
		protected ModelVariable	_variable		= null;

		//Constructor
		public ModelRange(Model model, ModelRangeAttribute attr)
			: base(model, attr)
		{
			SetAttributeValues(attr);
		}

		//Accessors
		public virtual	void			SetAttributeValues(ModelRangeAttribute attr)
		{
			base.SetAttributeValues(attr);
			if(attr != null)
			{
				_conjunction	= attr.Conjunction;
				
				//Specified variable
				if (attr.Variable != null && this.Model != null)
                    _variable = DetermineVariable(attr.Variable);

				//Copy the value array (so modifications don't muck with the original static model)
				_values.Clear();
				_values.Add(attr._values);
			}
		}

		public virtual void             InferDynamicVariables()
		{
			//Infer dynamic variable(s)
			foreach (ModelValue v in _values)
			{
				//Example: Max="GetFieldCount"
				//If the value is specified as a string, and the parameter/requirement isn't a string,
				//then we try to assume the input is actually a 'variable' to call, so try and find it.
				string variable = v.Value as string;
				if (variable != null && this.Type != typeof(String) && this.Model != null)
			        v.Variable = DetermineVariable(variable);
			}
		}

		public virtual ModelVariable    DetermineVariable(String name)
		{
            ModelVariable v = null;

            //[ModelVariable]
            if(this.Model.Variables.Find(name).Count > 0)
                v = this.Model.Variables[name];
            
            //If not found, then is could be a dynamic variable (Columns.Count)
            Object instance     = this.Model;
			if(v == null && name != null)
            {
                //Loop over the parts (ie: Command.Connection.IsOpen)
                string[] parts = name.TrimEnd('(', ')').Split('.');
                for(int i = 0; i < parts.Length; i++)
			    {
                    String part = parts[i];
                    if(String.IsNullOrEmpty(part))
                        continue;

				    //Reflection
				    MemberInfo[] members = instance.GetType().GetMember(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if(members == null || members.Length == 0)
                        throw new ModelException(this, "Unable to find variable: '" + part + "'");
                    
                    MemberInfo member = members[0];//TODO: Abmigious, more than one?
				    
				    //Variable
				    v = new ModelVariable(this.Model, instance, members[0], name, null);
				    if(i+1<parts.Length)
				        instance = v.CurrentValue;
				        
				    //Stop iterating once we hit a null
				    if(instance == null)
				        break;
			    }
			    
			    if(v != null)
    			    this.Model.Variables.Add(v);    
            }
			
			if(v == null)
			    throw new ModelException(this, "Unable to find variable: '" + name + "'");
			return v;
        }

		public virtual void				AddValuesFromType(ModelRangeAttribute attr, Type type)
		{
			//Use the attribute, if specified
			if(attr != null && attr.Type != null)
				type = attr.Type;
			if(type != null)
			{
				//Infer values from the type						
				if(type.IsEnum)
				{
#if (!(SmartPhone || WindowsCE || PocketPC))    //Enum.GetValues not supported on .Net CF
					//Add all the values
					foreach(object e in Enum.GetValues(type))
						_values.Add(new ModelValue(type, e));
#endif //(!(SmartPhone || WindowsCE || PocketPC))
				}
				else if(type == typeof(bool))
				{
					_values.Add(new ModelValue(type, true));
					_values.Add(new ModelValue(type, false));
				}
			}
		}

		public virtual	void			AddBitCombinations()
		{
			//AutoExpand enum into bit combinations
			//Note: We do this to make choosing the parameters easier and faster (ie: one time hit)
			//In addition we do this inorder to keep track of each combinations coverage

			//Must be an Enumerable type
			if(this.Type == null || !this.Type.IsEnum)
				throw new ModelException(this, "BitMask can only be specified on a Enumerable type", null);

			//Note: A simple (ripple) counter is the easiest solution
			//	For example, Given 3 bits { A, B, C } the combinations are simply:
			//						A, B, C		  Value
			//						-------------------
			//						0, 0, 0,	= 0 (invalid, no bits on)
			//						0, 0, 1,	= 1
			//						0, 1, 0,	= 2
			//						0, 1, 1,	= 3
			//						1, 0, 0,	= 4
			//						1, 0, 1,	= 5
			//						1, 1, 0,	= 6
			//						1, 1, 1,	= 7

			ModelValues values	= this.Values;
			Hashtable hash	= new Hashtable();			//Automatic Dup collapsing

			int bits	= values.Count;					//bits
			int total	= (int)Math.Pow(2, bits);		//2^n
			for(int i=1;i<total; i++)
			{
				int value = 0;

				//Calculate which bits are on
				for(int bit=0; bit<bits; bit++)
				{
					int mask = 0x00000001 << bit;
					if((i & mask) != 0)
						value |= (int)values[bit].Value;
				}

				//Note: We have to covert it back to the enum, inorder to call the method
				hash[value] = Enum.ToObject(this.Type, value);	//Since 0 is skipped
			}

			//Update our list of values
			values.Clear();
			foreach(object value in hash.Values)
				values.Add(new ModelValue(value));
		}

		public ModelValueConjunction	Conjunction
		{
			get { return _conjunction;			}
			set { _conjunction = value;			}
		}

		public new ModelValueFlags		Flags
		{
			get { return (ModelValueFlags)base.Flags;	}
			set { base.Flags = (int)value;				}
		}

		public abstract Type			Type
		{
			get;
		}

		public virtual bool				IsEnum
		{
			get { return this.Type != null && this.Type.IsEnum;		}
		}

		public virtual	ModelValues	    Values
		{
			get 
			{ 
				//If not specified, try to infer them from the type
				if(_values.Count==0)
				{
				}
				return _values;
			}
			set { _values = value;	}
		}

		public ModelValue				Value
		{
			get 
			{ 
				return _values.First;					
			}
			set { _values = new ModelValues(value);	}
		}

		public ModelVariable			Variable
		{
			get { return _variable;									}
		}

		public bool						BitMask
		{
			get { return IsFlag((int)ModelValueFlags.Bitmask);		}
			set { SetFlag((int)ModelValueFlags.Bitmask, value);		}
		}

		public virtual  bool			Evaluate(object value)
		{
			//Loop through the values
			int matched = 0;
			foreach(ModelValue v in this.Values)
			{
				object expected = value;

				//Bitwise comparison
				if(this.BitMask)
				{
					//Enums must be in the same type (to compare correctly)
					expected = Enum.ToObject(v.Type, (int)expected & (int)v.Value);
				}

				//Simple comparison
				if(v.Evaluate(expected))
					matched++;
			}

			if(this.Conjunction == ModelValueConjunction.Or)
				return matched > 0;
			return matched == this.Values.Count;	//AND
		}

		public override void			Reload()
		{
		}
	}

    ////////////////////////////////////////////////////////////////
    // ModelRanges
    //
    ////////////////////////////////////////////////////////////////
    ///<summary>This class represents a range of items. This forms a base class for ModelVariables, ModelParameters and ModelRequirements.</summary>
    public abstract class ModelRanges<T> : ModelItems<T> where T : ModelItem
    {
        //Constructor
        ///<summary>Default constructor</summary>
        public ModelRanges()
        {
        }

        ///<summary>Constructor with set of values</summary>
        public ModelRanges(params T[] values)
            : base(values)
        {
        }

        ///<summary>Returns a collection of all items that match the flag specified.</summary>
        public virtual ModelRanges<T>   FindFlag(ModelValueFlags flag)
        {
            //Delegate
            return this.FindFlag(flag, true);
        }

        ///<summary>Returns a collection of all items except those that match the flag specified.</summary>
        public virtual ModelRanges<T>   FindFlag(ModelValueFlags flag, bool include)
        {
            //Delegate
            return (ModelRanges<T>)base.FindFlag((int)flag, include);
        }
    }
}
