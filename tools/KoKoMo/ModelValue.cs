//---------------------------------------------------------------------
// <copyright file="ModelValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Collections;		//Comparer


namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// ModelValueOperator
	//
	////////////////////////////////////////////////////////////////
	public enum ModelValueOperator
	{
		Equal,					// x == y
		NotEqual,				// x != y
		GreaterThan,			// x >  y
		GreaterThanOrEqual,		// x >= y
		LessThan,				// x <  y
		LessThanOrEqual,		// x <= y
	}

	////////////////////////////////////////////////////////////////
	// ModelValue
	//
	////////////////////////////////////////////////////////////////
	public class ModelValue : ModelItem
	{
		//Data

		//Note: We want to allow dynamic (instance) changing of the data, without affecting
		//the assoicated attribute.  So simply store that state as well
		Type					_type;
		object					_value			= null;
		ModelVariable			_variable		= null;	//Dynamic value
		ModelValueOperator		_operator		= ModelValueOperator.Equal;	

		//Constructor
		public ModelValue(object value)
			: this(value, ModelValueOperator.Equal)
		{
			//Delegate
		}

		public ModelValue(object value, ModelValueOperator op)
			: this(value != null ? value.GetType() : null, value, op)
		{
			//Delegate
		}

        public ModelValue(Type type, object value)
            : this(type, value, ModelValueOperator.Equal)
        {
            //Delegate
        }
        
        public ModelValue(Type type, object value, ModelValueOperator op)
			: base(null, null)
		{
			_type		= type;
			_operator	= op;
			_value      = value;
		}

		public virtual Type				Type
		{
			get { return _type;							}
			set { _type = value;						}
		}

		public virtual bool				IsEnum
		{
			get { return this.Type != null && this.Type.IsEnum;		}
		}

		public object					Value
		{
			get 
			{ 
				if(_variable != null)
					return _variable.CachedValue;	//Dynamic value
				return _value;
			}
		}

		public ModelVariable			Variable
		{
			get { return _variable;		}
			set { _variable = value;	}
		}

		public override void			Reload()
		{
		}

		public override string			ToString()
		{
			if(this.Value != null)
				return this.Value.ToString();
			return null;
		}

		public ModelValueOperator		Operator
		{
			get { return _operator;				}
			set { _operator = value;			}
		}

		public virtual bool				Evaluate(object expected)
		{
			//Delegate
			return this.Evaluate(expected, _operator);
		}

		public virtual bool				Evaluate(object expected, ModelValueOperator op)
		{
			bool matched = false;
			int icompare = 0;
			object actual= this.Value;

			//Delegate the actual comparison
			try
			{
				if(op == ModelValueOperator.Equal || op == ModelValueOperator.NotEqual)
				{
					//Note: For == or !=, we don't require IComparer
					icompare = object.Equals(expected, actual) ? 0 : 1;
				}
				else 
				{
					//Note: To use <, >, the value needs to support IComparer
					icompare = Comparer.DefaultInvariant.Compare(expected, actual);
				}
			}
			catch(Exception e)
			{
				//Make this easier to debug
				throw new ModelException(this, "Unable to compare: '" + actual + "' to: '" + expected + "'", e);
			}

			//Determine if matched (equal, notequal, etc)
			switch(op)
			{
				case ModelValueOperator.Equal:
					matched = (icompare == 0);
					break;

				case ModelValueOperator.NotEqual:
					matched = (icompare != 0);
					break;

				case ModelValueOperator.GreaterThan:
					matched = (icompare >  0);
					break;

				case ModelValueOperator.GreaterThanOrEqual:
					matched = (icompare >=  0);
					break;

				case ModelValueOperator.LessThan:
					matched = (icompare <  0);
					break;

				case ModelValueOperator.LessThanOrEqual:
					matched = (icompare <=  0);
					break;

				default:
					throw new ModelException(this, "Unhandled Operator: " + op, null);
			};
			
			return matched;
		}
	}

    ////////////////////////////////////////////////////////////////
    // ModelValues
    //
    ////////////////////////////////////////////////////////////////
    ///<summary>This class represents a collection of model values. Many of our custom attributes represent collection entities. 
    ///We use this class to store all the possible values of the entity. </summary>
    public class ModelValues : ModelItems<ModelValue>
    {
        ///<summary>Default constructor</summary>
        public ModelValues()
        {
        }

        ///<summary>Constructor with multiple model values to build the collection</summary>
        public ModelValues(params ModelValue[] values)
            : base(values)
        {
        }

        ///<summary>Add an array of objects to this collection</summary>
        public virtual void             Add(object[] values)
        {
            if (values == null)
                values = new Object[] { null };

            foreach (object value in values)
                this.Add(new ModelValue(value));
        }

        ///<summary>Returns an object array of the values stored in the collection</summary>
        public new virtual object[]     ToArray()
        {
            object[] values = null;
            if (this.Count > 0)
            {
                values = new object[this.Count];
                for (int i = 0; i < this.Count; i++)
                    values[i] = this[i].Value;
            }
            return values;
        }

        ///<summary>Finds a value that matches the instance</summary>
        public virtual ModelValue       FindValue(object instance)
        {
            foreach (ModelValue val in _list)
            {
                if (val.Evaluate(instance, ModelValueOperator.Equal))
                    return val;
            }
            return null;
        }

        ///<summary>Finds all the model values that match the operator specified</summary>
        public virtual ModelValues      FindOperator(ModelValueOperator op)
        {
            return this.FindOperator(op, true);
        }

        ///<summary>Finds all the model values except those that match the operator specified</summary>
        public virtual ModelValues      FindOperator(ModelValueOperator op, bool include)
        {
            ModelValues found = new ModelValues();
            foreach (ModelValue item in _list)
            {
                if ((item.Operator == op) == include)
                    found.Add(item);
            }
            return found;
        }
    }
}
