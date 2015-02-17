//---------------------------------------------------------------------
// <copyright file="ModelParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Reflection;		//Reflection


namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelParameter (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple=true)]
	public class ModelParameterAttribute : ModelRangeAttribute
	{
		//Data
		int			_position	= -1;		//Not specified

		//Constructor
		public ModelParameterAttribute()
		{
		}

		public ModelParameterAttribute(params object[] values)
			: base(values)
		{
		}

		//Accessors
		public int			Position
		{
			get { return _position;		}
			set { _position = value;	}
		}
	}

	////////////////////////////////////////////////////////////////
	// ModelParameter
	//
	////////////////////////////////////////////////////////////////
	public class ModelParameter : ModelRange
	{
		//Data
		ModelAction			_action;
		ParameterInfo		_paraminfo;
		
		//Constructor
		public ModelParameter(ModelAction action, ModelParameterAttribute attr, ParameterInfo paraminfo)
			: base(action != null ? action.Model : null, attr)
		{
			_action		= action;
			_paraminfo	= paraminfo;

			//Infer values from the type, if not specified
			if(attr != null && attr.Type != null)
				this.AddValuesFromType(attr, null);

			//BitMask
			//TODO: Better place to expand this, (incase values added later).
			if(this.BitMask)
				this.AddBitCombinations();

			//Infer dynamic variable(s)
			this.InferDynamicVariables();
		}

		public virtual ModelAction			Action
		{
			get { return _action;				}
		}

		public virtual	int					Position
		{
			get { return _paraminfo.Position;						}
		}

		public override Type				Type
		{
			get { return _paraminfo.ParameterType;					}
		}

		public override	string				Name
		{
			get { return _paraminfo.Name;							}
		}

		public virtual bool                 IsOptional
		{
            get { return Attribute.IsDefined(_paraminfo, typeof(ParamArrayAttribute)); }
		}

		public override string				FullName
		{
			get 
			{ 
				if(_fullname == null && _action != null)
				{
					_fullname = _action.FullName + "." + this.Name;
					return _fullname;
				}

				//Otherwise
				return this.Name;
			}
		}
    
        public virtual bool                 IsNullable
        {
            get
            {
                return this.Type.GetGenericTypeDefinition() == typeof( System.Nullable<> );
            }
        }

        public virtual object			    Clone()
        {
            ModelParameter clone = (ModelParameter)this.MemberwiseClone();

            //Clone the collections, so add/remove is independent
            clone.Values = (ModelValues)this.Values.Clone();
            return clone;
        }
    }

    ////////////////////////////////////////////////////////////////
    // ModelParameters
    //
    ////////////////////////////////////////////////////////////////
    ///<summary>This class represents a collection of model parameters, this is used by the action class to store its parameters</summary>
    public class ModelParameters : ModelRanges<ModelParameter>
    {
        //Constructors
        ///<summary>Default constructor</summary>
        public ModelParameters()
        {
        }

        public new virtual ModelParameters  Find(params string[] names)
        {
            return (ModelParameters)base.Find(names);
        }

        public new virtual ModelParameters  FindExcept(params string[] names)
        {
            return (ModelParameters)base.FindExcept(names);
        }

        ///<summary>Return a collection of all the values of all the parameters in this collection</summary>
        public virtual ModelValues          Values
        {
            get
            {
                ModelValues values = new ModelValues();
                foreach (ModelRange range in this)
                    values.Add(range.Values);

                return values;
            }
        }

        ///<summary>A string concatenated version of the parameters to output as method signature</summary>
        public override string              ToString()
        {
            string output = null;
            foreach (ModelParameter parameter in this)
            {
                if (output != null)
                    output += ", ";
                output += parameter.Value;
            }

            return output;
        }
    }
}
