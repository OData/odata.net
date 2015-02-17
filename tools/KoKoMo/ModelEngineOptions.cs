//---------------------------------------------------------------------
// <copyright file="ModelEngineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Security.Permissions;
using System.Security;			

namespace Microsoft.Test.KoKoMo
{
	////////////////////////////////////////////////////////////////
	// WeightScheme
	//
	////////////////////////////////////////////////////////////////
	/// <summary>
	/// Global weighing scheme for the engine. Depending on this scheme
	/// the engine decides the probabilities.
	/// </summary>
	public enum WeightScheme
	{
		Custom,  		///Default weighting (specified in the model)
		Equal,			///Equal weighing
		AdaptiveEqual,	///Adjusted to weight uncalled item higher
		Geometric 		///Each item has twice the chance of the next one.
	};
	
	////////////////////////////////////////////////////////////////
	// ModelEngine
	//
	////////////////////////////////////////////////////////////////
	/// <summary>
	/// This class stores all the options that are set on the Engine.
	/// </summary>
	public class ModelEngineOptions : ICloneable
	{
		//Data
		int				    _seed			    = unchecked((int)DateTime.Now.Ticks);
		Random			    _rand			    = null;
		WeightScheme	    _weightscheme	    = WeightScheme.Custom;	//Default - as defined in the model
		long                _timeout			= 30;				    //Default, seconds
		long                _maxactions			= long.MaxValue;	    //Default, no limit
        bool                _addreturnedmodels  = true;
        bool                _loadAssemblyModels = true;
        bool                _tracing            = true;
        static ModelEngineOptions  _default     = null;

		//Constructor
		/// <summary>Default constructor</summary>
		public ModelEngineOptions()
		{
            _rand = new Random(_seed);
		}

		//Accessors
		/// <summary>
		/// Weight Scheme to be used by the Engine. See WeightScheme Enum for various settings.
		/// </summary>
		public virtual	WeightScheme		WeightScheme
		{
			//Expose our random generator, in case tests need it, (ie: one seed for complete repro)
			get	{ return _weightscheme;				}
			set	{ _weightscheme = value;			}
		}

        /// <summary>
        /// Sets/Gets a boolean that indicates whether or not assembly models should be loaded
        /// </summary>
        public virtual bool                 LoadAssemblyModels
        {
            get { return _loadAssemblyModels; }
            set { _loadAssemblyModels = value; }
        }

        /// <summary>
		/// Sets/Gets the unit value of the duration. Default value = 30 seconds
		/// </summary>
		public long							Timeout
		{
			get{ return _timeout;							}
			set{ _timeout = value;							}
		}

        /// <summary>
		/// Enable/Disable action tracing. Default value = true
		/// </summary>
		public bool						    Tracing
		{
			get{ return _tracing;					        }
			set{ _tracing = value;							}
		}

		/// <summary>
		/// Sets/Gets the maximum actions that must be executed by the state machine.
		/// After the number of actions set is hit, the machine stops and returns to user.
		/// Along with Seed, this is a powerful way to reproduce errors and add regressions.
		/// </summary>
		public long							MaxActions
		{
			get{ return _maxactions;					}
			set{ _maxactions = value;					}
		}

        /// <summary>
        /// This property allows you to automatically add returned models from actions
        /// </summary>
        public bool                         AddReturnedModels
        {
            get { return _addreturnedmodels;            }
            set { _addreturnedmodels = value;           }
        }
        
		public virtual	Random				Random
		{
			//Expose our random generator, in case tests need it, (ie: one seed for complete repro)
			get	{ return _rand;                         }
			set { _rand = value;                        }
		}

		/// <summary>
		/// The seed that is used to set/get on the random sequence generator.
		/// </summary>
		public virtual	int					Seed
		{
			get	{ return _seed;						    }
			set	
			{
				_seed = value;
				_rand = new Random(value);
			}
		}

		/// <summary>
		/// Default option settings, loaded once from the commandline
		/// </summary>
		public static ModelEngineOptions    Default
		{
			//Expose our random generator, in case tests need it, (ie: one seed for complete repro)
			get	
			{ 
			    if(_default == null)
			    {
			        _default = new ModelEngineOptions();
			        _default.Load();
			    }
			    return _default;
			}
		}

		/// <summary>
		/// Default option settings, loaded once from the commandline
		/// </summary>
		public virtual Object               Clone()
		{
            return this.MemberwiseClone();
		}

        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
		public void							Load()
		{
#if (!(SmartPhone || WindowsCE || PocketPC))    //GetCommandLineArgs is not implemented in .Net CF
			string[] args = Environment.GetCommandLineArgs();
			int count = 0;
			while (count < args.Length)
			{
				switch (args[count].ToUpper(System.Globalization.CultureInfo.InvariantCulture))
				{
                    case "/ADDRETURNEDMODELS+":
                        this.AddReturnedModels = true;
                        break;
                    case "/ADDRETURNEDMODELS-":
                        this.AddReturnedModels = false;
                        break;
					case "/SEED":
						if (count + 1 < args.Length)
						{
							this.Seed = Int32.Parse(args[count + 1]);
							count++;
						}
						else
							throw new ModelException(null, "Command line argument Seed needs a parameter");
						break;
					case "/MAXACTIONS":
						if (count + 1 < args.Length)
						{
							this.MaxActions = Int32.Parse(args[count + 1]);
							count++;
						}
						else
							throw new ModelException(null, "Command line argument MaxAction needs a parameter");
						break;
                    case "/LOADASSEMBLYMODELS":
                        if (count + 1 < args.Length)
                        {
                            this.LoadAssemblyModels = bool.Parse(args[count + 1]);
                            count++;
                        }
                        else
                            throw new ModelException(null, "Command line argument LoadAssemblyModels needs a parameter");
                        break;
                    case "/TIMEOUT":
						if (count + 1 < args.Length)
						{
							this.Timeout = Int32.Parse(args[count + 1]);
							count++;
						}
						else
							throw new ModelException(null, "Command line argument Timeout needs a parameter");
						break;
					case "/WEIGHTING":
						if (count + 1 < args.Length)
						{
							this.WeightScheme = (WeightScheme)Enum.Parse(typeof(WeightScheme), args[count + 1], true /* Ignore case */ );
							count++;
						}
						else
							throw new ModelException(null, "Command line argument Weighting needs a parameter");
						break;
					default:
						break;
				}
				count++;
			}
#endif //(!(SmartPhone || WindowsCE || PocketPC))
		}
	}
}
