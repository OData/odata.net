//---------------------------------------------------------------------
// <copyright file="ModelRequirement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				


namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelRequirement (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
	public class ModelRequirementAttribute : ModelRangeAttribute
	{
		//Constructor
		public ModelRequirementAttribute()
		{
		}
		
		public ModelRequirementAttribute(params object[] values)
			: base(values)
		{
		}
	}

    ////////////////////////////////////////////////////////////////
	// ModelRequirement
	//
	////////////////////////////////////////////////////////////////
	public class ModelRequirement : ModelRange
	{
		ModelAction			_action;
		bool				_global = true;	//global requirement (ie: any)		

		//Constructor
        public ModelRequirement(ModelVariable variable, Object value)
			: this(variable, new ModelValue(value))
		{
			//Delegate
		}

        public ModelRequirement(ModelVariable variable, ModelValue value)
			: this(null, null, variable, value)
		{
			//Delegate
		}

		public ModelRequirement(ModelAction action, ModelVariable variable, ModelValue value)
			: this(action, null, variable, value)
		{
			//Delegate
		}

		public ModelRequirement(ModelAction action, ModelRequirementAttribute attr, ModelVariable variable, ModelValue value)
			: base(action != null ? action.Model : null, attr)
		{
			//Action
			_action = action;

			//Variable
			if (variable != null)
			{
				_variable = variable;
				if (variable.BitMask)
					this.BitMask = true;
			}
			//if(_variable == null)
			//    throw new ModelException(this, "An empty variable is not a valid requirement", null);

			//Only override the attr, if values are really specified
			if(value != null)
				_values = new ModelValues(value);

			//BitMask
			//TODO: Better place to expand this, (incase values added later).
			if(this.BitMask)
				this.AddBitCombinations();

			//Infer dynamic variable(s)
			this.InferDynamicVariables();

			//Requirements assoicated with actions, are not global.  They are tied to that particular
			//instance of the model, and it's instance of state variables.  However if not assoicated
			//with actions, the default behavior is that their global, they apply to all models
			//that contain that state variable, unless explicitly indicated otherwise.
			_global = (action == null);
		}

		public override	string					Name
		{
			get 
			{ 
			    if(_name == null)
			    {
			        String name = null;
			        if(_action != null)
			            name += _action.Name + ".";
			        if(_variable != null)
			            name += _variable.Name;
			        _name = name;
			    }
			    return _name;	
            }
		}

		public virtual ModelAction				Action
		{
			get { return _action;				}
		}

		public override Type					Type
		{
			get 
			{ 
				if(_variable != null)
					return _variable.Type;	
				return null;
			}
		}

		public virtual bool						Global
		{
			get { return _global;				}
			set { _global = value;				}
		}
	}

    ////////////////////////////////////////////////////////////////
    // ModelRequirements
    //
    ////////////////////////////////////////////////////////////////
    ///<summary>This class represents a collection of requirements, this is used by the ModelAction class.</summary>
    public class ModelRequirements : ModelRanges<ModelRequirement>
    {
        //Constructors
        ///<summary>Default constructor</summary>
        public ModelRequirements()
        {
        }

        ///<summary>Overload that takes an array of requirements</summary>
        public ModelRequirements(params ModelRequirement[] requirements)
            : base(requirements)
        {
        }

        ///<summary>Add a requirement to this collection from the variable and the value specified</summary>
        public virtual ModelRequirement     Add(ModelVariable variable, object value)
        {
            return this.Add(new ModelRequirement(variable, new ModelValue(value)));
        }

        ///<summary>Find a requirement from the variable specified</summary>
        public virtual ModelRequirement     Find(ModelVariable variable)
        {
            //Find a matching variable, of the specified type
            foreach (ModelRequirement requirement in this)
            {
                if (requirement.Variable == variable)
                    return requirement;
            }

            //Otherwise
            return null;
        }
    }
}
