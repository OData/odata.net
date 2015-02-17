//---------------------------------------------------------------------
// <copyright file="Model.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Reflection;		    //BindingFlags
using System.Collections.Generic;	//List<T>

namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// Model (attribute)
	//
	////////////////////////////////////////////////////////////////
	[AttributeUsage(AttributeTargets.Class)]
	public class ModelAttribute : ModelItemAttribute
	{
		//Data
		protected int  _maxinstances    = Int32.MaxValue;	//Default (unlimited)
        protected bool _inherit         = false;
		//Constructor
		public ModelAttribute()
		{
		}
				
		//Properties
		public virtual int	                MaxInstances
		{
			get { return _maxinstances;		        }
			set { _maxinstances = value;		    }
		}

        public virtual bool                 Inherit
        {
            get { return _inherit;                  }
            set { _inherit = value;                 }
        }
	}

    ////////////////////////////////////////////////////////////////
    // Delegates
    //
    ////////////////////////////////////////////////////////////////
    public delegate void ModelEventHandler<T>(Object source, T item) where T : ModelItem;
    public delegate bool CallBeforeHandler(ModelAction action, ModelParameters parameters);
	public delegate void CallAfterHandler(ModelAction action, ModelParameters parameters, object result);


	////////////////////////////////////////////////////////////////
	// Model
	//
	////////////////////////////////////////////////////////////////
	public abstract class Model : ModelItem
	{
		//Data
		ModelActions		    _actions;
		ModelVariables		    _variables;
		ModelEngine			    _engine;
        bool                    _isExecuting    = true;
        List<CallBeforeHandler> _callbefore;
        Model                   _parentmodel    = null;
        int                     _maxinstances   = Int32.MaxValue;	//Default (unlimited)
        bool                    _inherit        = false;
        private static Dictionary<Type, object[]> CustomAttributes = new Dictionary<Type, object[]>();

        //Constructor
		public Model()
			: base(null, null)
		{
			//Obtain the attribute.
			//Note: We don't want to require a constructor that takes the attribute, since this
			//is an inherited class by the user, we'll simply obtain the attribute for default info.
            foreach (ModelAttribute attr in this.GetCustomAttributes(typeof(ModelAttribute)) )
				this.SetAttributeValues(attr);

			//By default, constructing the object, it becomes enabled.
			//This is so, you can automatically construct (disabled) models, and add them to the system
			//However if called in the context of the [attr] caller, it will get reset to the attribute
			this.Disabled = false;
		}

		public virtual	void				Init()
		{
			//Override, if you have code that should be executed first (before any methods are called)
		}

		public virtual	void				Terminate()
		{
			//TODO: Call from the engine
			//Override, if you have code that should be executed before the model is released
		}

        public override void                SetAttributeValues(ModelItemAttribute attr)
        {
            base.SetAttributeValues(attr);
            if (attr is ModelAttribute)
            {
                ModelAttribute mattr = attr as ModelAttribute;
                _maxinstances = mattr.MaxInstances;
                _inherit = mattr.Inherit;
            }
        }

		public virtual	ModelEngine			Engine
		{
			get { return _engine;			}
			set { _engine = value;			}
		}

		//Accessors

        /// <summary>
        /// Indicates if the model is executing IUT code or just exploring the state space.
        /// </summary>
        public virtual bool                 IsExecuting
        {
            get { return _isExecuting;          }
            set { _isExecuting = value;         }
        }

        public virtual int                  MaxInstances
        {
            get { return _maxinstances;         }
            set { _maxinstances = value;        }
        }

        public virtual bool                 Inherit
        {
            get { return _inherit;              }
            set { _inherit = value;             }
        }

        public virtual	ModelActions		Actions
		{
			get
			{
				//Dynamically find all actions (of this model)
				//Note: Override if you had a different way to determine actions
				if(_actions == null)
				{
					_actions = new ModelActions();

					//Looking for *ALL* methods and properties
					BindingFlags bindingflags = BindingFlags.Public | BindingFlags.NonPublic | 
												BindingFlags.Instance | BindingFlags.Static | 
												BindingFlags.GetProperty | BindingFlags.SetProperty | 
												BindingFlags.InvokeMethod;

					//Loop over them
					foreach(MethodInfo info in this.GetType().GetMethods(bindingflags))
					{
						foreach(ModelActionAttribute attr in info.GetCustomAttributes(typeof(ModelActionAttribute), _inherit))
						{
							_actions.Add(new ModelAction(this, info, attr));
						}
					}

					//Sort
					_actions.SortByWeightDesc();
				}			
				return _actions;
			}
		}

		public virtual	ModelVariables		Variables
		{
			get
			{
				//Dynamically find all model variables
				//Note: Override if you had a different way to determine variables
				if(_variables == null)
				{
					//Find all [ModelAction] attributes
					_variables = new ModelVariables();

					//Looking for *ALL* fields and properties
					BindingFlags bindingflags = BindingFlags.Public | BindingFlags.NonPublic | 
												BindingFlags.Instance | BindingFlags.Static | 
												BindingFlags.GetField | BindingFlags.GetProperty;

					//Loop over them
					foreach(MemberInfo info in this.GetType().GetMembers(bindingflags))
					{
                        foreach (ModelVariableAttribute attr in info.GetCustomAttributes(typeof(ModelVariableAttribute), _inherit))
						{
							_variables.Add(new ModelVariable(this, info, attr));
						}
					}

					//Sort
					_variables.SortByWeightDesc();
				}			
				return _variables;
			}
		}

		public event CallBeforeHandler      CallBefore
		{
            //Note: We want the handler to be able to return a result (ie: continue) so we override this
            add 
            { 
                if(_callbefore == null) 
                    _callbefore = new List<CallBeforeHandler>();
                _callbefore.Add(value); 
            }
            remove 
            { 
                if(_callbefore != null)
                    _callbefore.Remove(value); 
            }
		}

		public virtual bool				    OnCallBefore(ModelAction action, ModelParameters parameters)
		{
			//Note: Override if you want to do something, or call other methods BEFORE execution of the action
			bool result = true; //true, indicates continue to call the action
            if(_callbefore != null)
            {
                foreach(CallBeforeHandler handler in _callbefore)
                    result &= handler(action, parameters);
            }

			return result;	
		}

		public event CallAfterHandler       CallAfter;
		public virtual void				    OnCallAfter(ModelAction action, ModelParameters parameters, object result)
		{
            //Note: Override if you want to do after
            if(CallAfter != null)
                CallAfter(action, parameters, result);
		}

        public virtual void                 OnException(ModelAction action, ModelParameters parameters, Exception e)
        {
			//Since were using reflection, if an error occurs within the method
			//make this easier to debug (for the method writer) so they see their exception
			//instead of the reflection based one.
			while(e.InnerException != null && e is TargetInvocationException)
				e = e.InnerException;

			//Find what should have thrown the exception, action or parameters
			Type exception		= action.Exception;
            string exceptionid = action.ExceptionId;
			if(exception == null && exceptionid == null)
			{
				//Otherwise maybe one of the parameters was supposed to throw
				//Find the parameter that expects an error
				ModelParameters found = (ModelParameters)parameters.FindFlag((int)ModelItemFlags.Throws);
				if(found.Count > 0)
				{
					//Note: We find the 'highest' weighted parameter (ie: order of errors processed)
					found.SortByWeightDesc();					
					exception	= found.First.Exception;
					exceptionid = found.First.ExceptionId;
				}
			}

			//Exception not expected
			if(exception == null && exceptionid == null)
				throw new ModelException(action, "Threw an exception: " + e.Message,  e);

			//Expected: Simple verification, type based
			if(exception != null && (exception != e.GetType()))
				throw new ModelException(action, "Threw the wrong exception: " + e.Message,  e);

			//Expected: Advanced verification, user implemented function
			if(exceptionid != null)
				this.OnException(action, parameters, e, exceptionid);	//Throws if not verified
        }

        public virtual void                 OnException(ModelAction action, ModelParameters parameters, Exception e, string id)
        {
			//Override this method, and verify the ExceptionId specified in the model
			throw new ModelException(this, "ExceptionId was specified and not verified.  Override Model.VerifyException, and verify the ExceptionId as was specified in the model", e);
		}

		public override void			    Reload()
		{
			//Clear, so they will be dynamically setup by reflection again
			_actions	= null;
			_variables	= null;
		}

        public virtual Model                ParentModel
        {
            get { return _parentmodel;              }
            set { _parentmodel = value;             }
        }

        public virtual Models               Children
        {
            get
            {
                //We have to recompute this everytime since 
                //new children could have been added.
                //Can change this later.
                Models found = new Models(this.Engine);
                if(this.Engine != null)
                {
                    foreach(Model model in this.Engine.Models)
                    {
                        if(model.ParentModel == this)
                            found.Add(model);                        
                    }
            }
                return found;
            }
        }
    
        protected virtual Object[]          GetCustomAttributes(Type attributetype)
        {
            Type key        = this.GetType();
            object[] value  = null;

            if (!CustomAttributes.TryGetValue(key, out value))
            {
                value = this.GetType().GetCustomAttributes(attributetype, _inherit);
                CustomAttributes[key] = value; //add or update the value, last update wins
            }
            return value;
        }
    }


    ////////////////////////////////////////////////////////////////
    // Models
    //
    ////////////////////////////////////////////////////////////////
    public class Models : ModelItems<Model>
    {
        //Data
        protected ModelEngine _engine = null;

        //Constructors

        ///<summary>Constructor</summary>
        ///<param name="engine">The engine context for this collection.</param>
        public Models(ModelEngine engine)
        {
            _engine = engine;
        }

        ///<summary>Type Indexer</summary>
        public virtual Model                    this[Type type]
        {
            get
            {
                //Find first, fail if not found, although make it easier to debug
                Models found = this.FindType(type);
                if (found == null || found.Count <= 0)
                    throw new IndexOutOfRangeException(this.Name + "['" + type + "'] is not found");
                return found.First;
            }
        }

        ///<summary>Find models of type "type"</summary>
        public virtual Models                   FindType(Type type)
        {
            //Find a matching variable, of the specified type
            Models found = new Models(_engine);
            foreach (Model model in this)
            {
                if (model.GetType() == type || model.GetType().IsSubclassOf(type))
                    found.Add(model);
            }

            //Otherwise
            return found;
        }

        ///<summary>Add a Model to collection</summary>
        public override Model                   Add(Model model)
        {
            //Delegate
            base.Add(model);
            
            //Hook up the engine
            model.Engine = _engine;
            return model;
        }
        
        //Events
        public override void                    OnAdded(Model model)
        {
            //Delegate
            base.OnAdded(model);

            //Notify the parents
            if(_engine != null && _engine.Parent != null)
                _engine.Parent.Models.OnAdded(model);
        }

        public override void                    OnRemoved(Model model)
        {
            //Delegate
            base.OnRemoved(model);

            //Notify the parents
            if (_engine != null && _engine.Parent != null)
                _engine.Parent.Models.OnRemoved(model);
        }

        ///<summary>Given the assembly or the calling object, find models in the assembly and add them to the collection</summary>
        public virtual void                     AddFromAssembly(object caller)
        {
            //Delegate
            //Note: Assembly.GetAssembly is not implemented in .Net CF
            this.AddFromAssembly(caller.GetType().Assembly);
        }

        ///<summary>Add the models found in the specified assembly</summary>
        public virtual void                     AddFromAssembly(Assembly assembly)
        {
            //Find all [Model] attributes
            foreach (Type type in assembly.GetTypes())
            {
                //Models can be inherited from common bases
                if (type.IsAbstract)
                    continue;

                //Loop over all attributes
                foreach (ModelAttribute attr in type.GetCustomAttributes(typeof(ModelAttribute), false))
                {
                    //Ensure the class if of type Model
                    //Instead of throwing a hard to debug, constructor error
                    if (!type.IsSubclassOf(typeof(Model)))
                        throw new ModelException(_engine, "[Model] class: '" + type.Name + "' must inherit from Model");

                    //Ensure the class has a default constructor (required)
                    ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                null, new Type[0], null);   //Note: Types.EmptyTypes is not supported on .Net CF
                    if (ctor == null)
                        throw new ModelException(_engine, "[Model] class: '" + type.Name + "' must implement a default constructor");

                    //Construct the model
                    Model model = (Model)ctor.Invoke(null);
                    model.SetAttributeValues(attr);
                    this.Add(model);
                }
            }
        }

        /// <summary>
        /// Initialize all models in the collection.
        /// </summary>
        public virtual void                     Init()
        {
            foreach (Model model in this)
                model.Init();
        }

        /// <summary>
        /// Get or Set the 'IsExecuting' property for all models in the collection. This flag allows the 
        /// model developer to indicate which portions of the model should be run only during execution.
        /// </summary>
        public virtual bool                     IsExecuting
        {
            get
            {
                if (this.Count == 0)
                    throw new InvalidOperationException("Unable to read aggregate property 'IsExecuting' because the model collection is empty.");

                bool val = this.First.IsExecuting;

                for (int i = 1; i < this.Count; i++)
                {
                    if (val != this[i].IsExecuting)
                        throw new ModelException(_engine, "Unable to read aggregate property 'IsExecuting' because not all models have the same value.");

                    val = this[i].IsExecuting;
                }

                return val;
            }

            //note: less error handling is needed here because "set" of a non-nullable type is better defined
            //than "get" over an empty or varied set of objects.
            set
            {
                foreach (Model model in this)
                    model.IsExecuting = value;
            }
        }

        ///<summary>Return a collection of ModelAction for all the actions that are defined on all models in this collection</summary>
        public virtual ModelActions             Actions
        {
            get
            {
                //Helper to return the flat list of all actions
                ModelActions actions = new ModelActions();
                foreach (Model model in this)
                    actions.Add(model.Actions);
                return actions;
            }
        }

        ///<summary>Return a collection of all variables defined in all the models in this collection</summary>
        public virtual ModelVariables           Variables
        {
            get
            {
                //Helper to return the flat list of all variables
                ModelVariables variables = new ModelVariables();
                foreach (Model model in this)
                    variables.Add(model.Variables);
                return variables;
            }
        }

        public virtual int                      MaxInstances
        {
            get 
            {
                //Return the smaller of the values
                int maxinstances = Int32.MaxValue;
                foreach(Model model in this)
                    maxinstances = Math.Min(maxinstances, model.MaxInstances);
                return maxinstances;
            }
            set 
            { 
                foreach(Model model in this)
                    model.MaxInstances = value; 
            }
        }


        /// <summary>
        /// Set the CallBefore event on all models in the engine.
        /// </summary>
        public virtual event CallBeforeHandler  CallBefore
        {
            add
            {
                foreach (Model model in this)
                    model.CallBefore += value;
            }
            remove
            {
                foreach (Model model in this)
                    model.CallBefore -= value;
            }
        }

        /// <summary>
        /// Set the CallAfter event on all models in the engine.
        /// </summary>
        public virtual event CallAfterHandler   CallAfter
        {
            add
            {
                foreach (Model model in this)
                    model.CallAfter += value;
            }
            remove
            {
                foreach (Model model in this)
                    model.CallAfter -= value;
            }
        }
    }
}
