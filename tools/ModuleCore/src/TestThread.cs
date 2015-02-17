//---------------------------------------------------------------------
// <copyright file="TestThread.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;		//ArrayList
using System.Threading;         //Thread

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// Delegate
	//
	////////////////////////////////////////////////////////////////
	public delegate	void	ThreadFunc(object obj);
	

	////////////////////////////////////////////////////////////////
	// TestThreads
	//
	////////////////////////////////////////////////////////////////
	public class TestThreads
	{ 
		//Data
		protected ArrayList	_threads;

		//Constructor
		public	TestThreads()
		{
			_threads = new ArrayList();
		}

		//Static
		public	static int				    MaxThreads		=	20;

		//Accessors
		public	virtual	ArrayList			Internal
		{
			get { return _threads;					    }
		}

		public	virtual	int					Count
		{
			get { return Internal.Count;				}
		}

		public	virtual	TestThread			this[int index]
		{
			get { return (TestThread)(Internal[index]);	}
		}

		//Helpers
		public		virtual TestThread		Add(TestThread thread)
		{
			Internal.Add(thread);
			return thread;
		}

		public		virtual void		    Add(ThreadFunc func, object param)
		{
			Add(1, func, param);
		}

		public		virtual void		    Add(int threads, ThreadFunc func, object param)
		{
			//Default to one iteration
			Add(threads, 1, func, param);
		}

		public		virtual void		    Add(int threads, int iterations, ThreadFunc func, object param)
		{
			for(int i=0; i<threads; i++)
				Add(new TestThread(iterations, func, param));
		}

		public		virtual void		    Clear()
		{
			Internal.Clear();
		}

		public		virtual void		    Start()
		{
			for(int i=0; i<Count; i++)
				this[i].Start();
		}

		public		virtual void		    Abort()
		{
			for(int i=0; i<Count; i++)
				this[i].Abort();
		}

		public		virtual void		    Wait()
		{
			//Delegate
			this.Wait(Timeout.Infinite);
		}

		public		virtual bool		    Wait(int milliseconds)
		{
			Exception eReturn = null;
			bool terminated   = true;

			//Wait for all the threads to complete...
			for(int i=0; i<Count; i++)
			{
				//Even if a thread ends up throwing, we still having to wait for all the
				//other threads to complete first.  Then throw the first exception received.
				try
				{
					if(!this[i].Wait(milliseconds))
						terminated = false;
				}
				catch(Exception e)
				{
					//Only need to save off the first exception
					if(eReturn == null)
						eReturn = e;
				}
			}

			//If any of the threads failed, throw an exception...
			if(eReturn != null)
				throw eReturn;
				
			return terminated;
		}

		public		virtual void		    StartSingleThreaded()
		{
			//This is mainly a debugging tool to ensure your threading scenario works
			//if it was run actually sequentially.  (ie: non-multlthreaded).
			for(int i=0; i<Count; i++)
			{
				this[i].Start();
				this[i].Wait();
			}
		}
	}


	////////////////////////////////////////////////////////////////
	// TestThread
	//
	////////////////////////////////////////////////////////////////
	public class TestThread
	{ 
		//Data
		protected		ThreadFunc		_func;
		protected		int				_iterations;
		protected		object			_param;
		protected		Exception		_exception;
		protected		Thread			_thread;

		//Constructor
		public	TestThread(ThreadFunc func, object param)
			: this(1, func, param)
		{
			//Default to 1 iteration
		}

		public	TestThread(int iterations, ThreadFunc func, object param)
		{
			//Note: notice there are no "setters" on this class, so you can't reuse this class
			//for other thread functions, you need one class per thread function.  This is exaclty 
			//how the System.Thread class works, you can't reuse it, once the thread is complete, its done
			//so all the state set is for that thread...
		
			_thread		    = new Thread(new ThreadStart(InternalThreadStart));
			_func	        = func;
			_iterations	    = iterations;
			_param		    = param;
		}

		//Static
		public	static int			    MaxIterations		=	30;

		//Accessors
		public	virtual	Thread			Internal
		{
			get { return _thread;					}
		}

		public	virtual	int				Iterations
		{
			get { return _iterations;				}
		}

		public	virtual	ThreadFunc		Func
		{
			get { return _func;				        }
		}

		public	virtual	object			Param
		{
			get { return _param;					}
		}
		
		protected		void			InternalThreadStart()
		{
			//Note: We have a "wrapper" thread function thats always called.
			//This allows us much greater control than the normal System.Thread class.
			//	1.  It allows us to call the function repeatedly (iterations) 
			//  2.  It allows parameters to be passed into the thread function
			//	3.  It allows a return code from the thread function
			//	4.  etc...
			
			//Iterate the specified number of times
			for(int i=0; i<Iterations; i++)
			{
				//call the user thread function
				try
				{
					Func(Param);
				}
				catch(Exception e)
				{
					//Note: If we don't handle this exception it doesn't get handled by the 
					//main thread try-catch since its on a sperate thread.  Instead of crashing the 
					//URT - or requiring every thread function to catch any exception (there not expecting)
					//we will catch it and store the exception for later throw from the calling function
					TestLog.HandleException(e);
					_exception  = e;
				
					//We should break out of this iteration
					break;
				}
			}
		}

		public		virtual void		Start()
		{
			Internal.Start();
		}

		public		virtual void		Abort()
		{
			Internal.Abort();
		}

		public		virtual void		Wait()
		{
			//Delegate
			this.Wait(Timeout.Infinite);
		}

		public		virtual bool		Wait(int milliseconds)
		{
			//Wait for this thread to complete...
			bool terminated = Internal.Join(milliseconds);
		
			//Now throw any exceptions that occured from within the thread to the caller
			if(_exception != null)
				throw _exception;
				
			return terminated;
		}
	}
}

