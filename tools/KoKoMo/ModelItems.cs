//---------------------------------------------------------------------
// <copyright file="ModelItems.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;				
using System.Reflection;		    //Assembly
using System.Collections;		    //IComparer
using System.Collections.Generic;	//List<T>


namespace Microsoft.Test.KoKoMo
{
    ////////////////////////////////////////////////////////////////
	// ModelItems
	//
	////////////////////////////////////////////////////////////////
	/// <summary>
	/// This class represents a collection of ModelItem objects.
	/// This class has methods to manage the collection such as indexers, finders, add/remove, etc. 
    ///
    /// Implementation Note: All internal enumeration should be performed on this instead of _list
    /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
    /// over the list items.
	/// </summary>
	public abstract class ModelItems<T> : IEnumerable<T> where T : ModelItem
	{
		//Data
		protected IComparer<T>                  _weightcomparer = null; 
		protected List<T>                       _list           = new List<T>();	//List doesn't allow overriding Add, so we have to contain this
        protected List<ModelEventHandler<T>>    _onadded        = null;
        protected List<ModelEventHandler<T>>    _onremoved      = null;

		//Constructor
		/// <summary>
		/// Default constructor does nothing.
		/// </summary>
		public ModelItems()
		{
		}

		/// <summary>
		/// Constructor that adds the items passed in.
		/// </summary>
		/// <param name="items">Comma seperated list of items to add</param>
		public ModelItems(params T[] items)
		{
			this.Add(items);
		}

		/// <summary>
		/// Count of items in the collection.
		/// </summary>
		public virtual int				        Count
		{
			get { return _list.Count;					}
		}

		/// <summary>
		/// Position Indexer to return the object at position "index"
		/// </summary>
		public virtual T	        	        this[int index]
		{
			get 
			{
                //Find by index
                //Fail if not found, although make it easier to debug
				if(index < 0 || index > _list.Count)
					throw new IndexOutOfRangeException(this.Name + '[' + index + "] is not found");
				return _list[index];		
			}
		}

		/// <summary>
		/// Named indexer.
		/// </summary>
		public virtual T		                this[string name]
		{
			//Find first, fail if not found, although make it easier to debug
			get
			{
				ModelItems<T> found = this.Find(name);
				if(found == null || found.Count <= 0)
					throw new IndexOutOfRangeException(this.Name + "['" + name + "'] is not found");
				return found.First;
			}
		}

        public virtual T                        First
        {
            get 
            { 
                if(_list.Count > 0)
                    return _list[0]; 
                return null;
            }
        }

        public virtual bool                     Throws
		{
			get
			{
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
				{
					//If any of the items are invalid, break and return true
					if(item.Throws)
						return true;				
				}
				return false;	//Otherwise
			}
		}

        //Events
        public event ModelEventHandler<T>       Added
        {
            //Note: We need access to this list, so we can't use the default implementation
            add
            {
                if(_onadded == null)
                    _onadded = new List<ModelEventHandler<T>>();
                _onadded.Add(value);
            }
            remove
            {
                if (_onadded != null)
                    _onadded.Remove(value);
            }
        }

        public event ModelEventHandler<T>       Removed
        {
            //Note: We need access to this list, so we can't use the default implementation
            add
            {
                if(_onremoved == null)
                    _onremoved = new List<ModelEventHandler<T>>();
                _onremoved.Add(value);
            }
            remove
            {
                if(_onremoved != null)
                    _onremoved.Remove(value);
            }
        }
        
        public virtual void                     OnAdded(T item)
        {
            if (_onadded != null)
            {
                foreach (ModelEventHandler<T> handler in _onadded)
                    handler(this, item);
            }
        }

        public virtual void                     OnRemoved(T item)
        {
            if (_onremoved != null)
            {
                foreach (ModelEventHandler<T> handler in _onremoved)
                    handler(this, item);
            }
        }

		/// <summary>
		/// Adds an item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual T		                Add(T item)
		{
			_list.Add(item);

			//Events
			OnAdded(item);
			return item;	
		}

		/// <summary>
		/// Adds another ModelItems collection to this collection.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public virtual ModelItems<T>		    Add(ModelItems<T> items)
		{
			foreach(T item in items)
				this.Add(item);
			return items;
		}

		/// <summary>
		/// Adds an array of ModelItems.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public virtual T[]		            Add(params T[] items)
		{
			foreach(T item in items)
				this.Add(item);
			return items;
		}

		/// <summary>
		/// Removes a model item from this collection.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual T		            Remove(T item)
		{
			_list.Remove(item);

            //Events
            OnRemoved(item);
			return item;
		}

		/// <summary>
		/// Removes a collection of model items from this collection.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public virtual ModelItems<T>		Remove(ModelItems<T> items)
		{
			foreach(T item in items)
				this.Remove(item);
			return items;
		}

        public virtual T[]                  ToArray()
        {
            return _list.ToArray();
        }
        
        //Accessors
		/// <summary>
		/// Returns the Type.ToString for this item type.
		/// </summary>
		public virtual string			    Name
		{
			get { return this.GetType().Name;	}
		}

		/// <summary>
		/// Returns the weight of this collection which is the sum of all weights of all items.
		/// Setter sets the specified weight for all the items in the collection like a bulk update.
		/// </summary>
		public virtual int				    Weight
		{
			get
			{
				//Return the weight of all items
				int weight = 0;
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
					weight += item.Weight;
				return weight;
			}

			set
			{
				//Update the weight of all items
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
					item.Weight = value;
			}
		}

		/// <summary>
		/// Returns true if any of items is enabled.
		/// Sets diabled = false for all items in the collection.
		/// </summary>
		public virtual bool				    Enabled
		{
			get
			{
				//Return if any the items are enabled
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
				{
					if(item.Enabled)
						return true;
				}
				return false;
			}

			set
			{
				//Update the weight of all items
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
					item.Enabled = value;
			}
		}

        /// <summary>
		/// Returns the total accessed count for all items in the collection.
		/// </summary>
        public virtual int                  Accessed
		{
			get
			{
				//Return if any the items are accessed
				int accessed = 0;
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
					accessed += item.Accessed;
				return accessed;
			}
		}

        /// <summary>
        /// Returns true if for each that is tracked the accessed count is more than 0.
        /// </summary>
        public virtual bool                 AllCovered
        {
            get
            {
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
                {
                    if (item.Enabled && item.Tracked && item.Accessed == 0)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
		/// Sorts the internal array by weights.
		/// </summary>
		public virtual void				    SortByWeightDesc()
		{
			if(_weightcomparer == null)
				_weightcomparer = new ModelItemWeightComparer<T>(true/*desc*/);
			_list.Sort(_weightcomparer);
		}

		/// <summary>
		/// Clears the internal array.
		/// </summary>
		public virtual void				    Clear()
		{
			_list.Clear();
		}

		/// <summary>
		/// Returns the index of a particular item in the collection.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual int				    IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// Returns an enumerator over the internal array.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator		    GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator over the internal array.
		/// </summary>
		/// <returns></returns>
		IEnumerator<T>	                    IEnumerable<T>.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Overridden Clone method to copy this item.
		/// </summary>
		/// <returns></returns>
		public virtual object			    Clone()
		{
			ModelItems<T> clone = (ModelItems<T>)this.MemberwiseClone();
            clone._list = new List<T>();
            clone.Add(this);
            return clone;
		}

        /// <summary>
        /// Returns a collection of items found for the given names.
        /// </summary>
        /// <param name="names">Names of items. (usually type.ToString)</param>
        /// <returns>ModelItems collection</returns>
        public virtual ModelItems<T>        Find(params string[] names)
        {
            //Construct the typed collection (ie: (Models)Find).
            //The simplest, without dealing with constructors, is to clone and clear
            ModelItems<T> items = (ModelItems<T>)this.Clone();
            items.Clear();

            //Note: To perserve the order, we need to loop over the names in the outer loop
            //instead of the inner, which makes the exclusion (ie: include=false) more difficult
            foreach (string name in names)
            {
                /// Implementation Note: All internal enumeration should be performed on this instead of _list
                /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                /// over the list items.
                foreach (T item in this)
                {
                    //Compare
                    bool matched = String.Compare(item.Name, name, true/*ignore case*/) == 0;
                    if (matched)
                        items.Add(item);
                }
            }
            return items;
        }

        /// <summary>
        /// Returns a collection of items found except those specified.
        /// </summary>
        /// <param name="names">Names to ignore while returning.</param>
        /// <returns>ModelItems collection</returns>
        public virtual ModelItems<T>        FindExcept(params string[] names)
        {
            //Construct the typed collection (ie: (Models)Find).
            //The simplest, without dealing with constructors, is to clone and clear
            ModelItems<T> items = (ModelItems<T>)this.Clone();
            items.Clear();

            //Find all items that don't match ANY of the names
            //Note: This doesn't need to perserve the order, so matching this a seperate function (simplier)
            /// Implementation Note: All internal enumeration should be performed on this instead of _list
            /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
            /// over the list items.
            foreach (T item in this)
            {
                bool matched = false;
                foreach (string name in names)
                {
                    //Delegate
                    if (String.Compare(item.Name, name, true/*ignore case*/) == 0)
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// Returns all the items in the collection which match the flag specified.
        /// </summary>
        /// <param name="flag">Flag that is present on the item such as disabled, tracked, throws, etc.</param>
        /// <returns>Returns a ModelItems collection</returns>
        public virtual ModelItems<T>            FindFlag(int flag)
        {
            //Delegate
            return this.FindFlag(flag, true);
        }

        /// <summary>
        /// Returns all the items in the collection which match the flag specified.
        /// </summary>
        /// <param name="include">boolean to indicate if names are to be ignored or not, false for ignore.</param>
        /// <param name="flag">Flag that is present on the item such as disabled, tracked, throws, etc.</param>
        /// <returns>Returns a ModelItems collection</returns>
        /// <returns></returns>
        public virtual ModelItems<T>            FindFlag(int flag, bool include)
        {
            //Construct the typed collection (ie: (Models)Find).
            //The simplest, without dealing with constructors, is to clone and clear
            ModelItems<T> items = (ModelItems<T>)this.Clone();
            items.Clear();

            //Note: there could be more than one (even by the same name)
            /// Implementation Note: All internal enumeration should be performed on this instead of _list
            /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
            /// over the list items.
            foreach (T item in this)
            {
                if (include)
                {
                    if (item.IsFlag(flag))
                        items.Add(item);
                }
                else
                {
                    if (item.IsFlagExcept(flag))
                        items.Add(item);
                }
            }
            return items;
        }

        /// <summary>
        /// Returns a ModelItem in the collection for the given ModelItem instance
        /// </summary>
        /// <param name="instance">ModelItem instance</param>
        /// <returns>Returns a ModelItem instance or null if not found</returns>
        public virtual T                        FindInstance(T instance)
        {
            /// Implementation Note: All internal enumeration should be performed on this instead of _list
            /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
            /// over the list items.
            foreach (T item in this)
            {
                if ((object)item == (object)instance)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Returns a collection of ModelItems for the label
        /// </summary>
        /// <param name="label">Label to Find by</param>
        /// <returns>Returns a ModelItem collection</returns>
        public virtual ModelItems<T>            FindByLabel(object label)
        {
            //Construct itself without bothering about constructors.
            ModelItems<T> items = (ModelItems<T>)this.Clone();
            items.Clear();

            /// Implementation Note: All internal enumeration should be performed on this instead of _list
            /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
            /// over the list items.
            foreach (T item in this)
                if (item.CompareLabel(label) == 0)
                    items.Add(item);

            return items;
        }

        /// <summary>
        /// Returns a collection of ModelItems that do not define the given label
        /// </summary>
        /// <param name="label">Label to find by</param>
        /// <returns>Returns a ModelItem collection</returns>
        public virtual ModelItems<T>            FindByNoLabel(object label)
        {
            //Construct itself without bothering about constructors.
            ModelItems<T> items = (ModelItems<T>)this.Clone();
            items.Clear();

            /// Implementation Note: All internal enumeration should be performed on this instead of _list
            /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
            /// over the list items.
            foreach (T item in this)
                if (item.CompareLabel(label) != 0)
                    items.Add(item);

            return items;
        }

        public virtual T                        Choose(ModelEngine engine)
        {
            return this.Choose(engine.Options.Random, engine.Options.WeightScheme);
        }

        public virtual T                        Choose(Random r, WeightScheme weightscheme)
        {
            if (this.Count <= 0)
                return null;

            switch (weightscheme)
            {
                case WeightScheme.Equal:
                    return this[r.Next(this.Count)];

                case WeightScheme.Geometric:
                    {
                        int weight = (int)Math.Pow(2, this.Count) - 1;
                        int index = r.Next(1, weight + 1);
                        int level = (int)Math.Round(Math.Log(index + 1) / Math.Log(2));
                        level = this.Count - level;
                        return this[level];
                    }

                case WeightScheme.AdaptiveEqual:
                case WeightScheme.Custom:
                    {
                        int weight = this.Weight;
                        int index = r.Next(weight);
                        /// Implementation Note: All internal enumeration should be performed on this instead of _list
                        /// to ensure that any overriden GetEnumerator() can dictate how enumeration is performed
                        /// over the list items.
                        foreach (T item in this)
                        {
                            if (index < item.Weight)
                            {
                                //Note: Adaptive never turns off the action complete, just reduces frequency
                                if (weightscheme == WeightScheme.AdaptiveEqual && item.Weight > 1)
                                    item.Weight--;
                                return item;
                            }

                            index -= item.Weight;
                        }
                        return null;
                    }

                default:
                    throw new ModelException(null, "Unhandled WeightScheme: " + weightscheme);
            };
        }
    }

	///////////////////////////////////////////////////////////////////////////
	// ModelItemWeightComparer
	//
	///////////////////////////////////////////////////////////////////////////
	///<summary>This is a base class used to compare any two model items and used 
	///in sorting by weights, since weight is stored on the ModelItem class</summary>
	public class ModelItemWeightComparer<T> : IComparer<T> where T : ModelItem
	{
		//Data
		bool _desc = true;

		//Constructor
		///<summary>Constructor</summary>
		public ModelItemWeightComparer(bool desc)
		{
			_desc = desc;
		}

		//Methods
		///<summary>IComparer Compare method to compare two model items</summary>
		public virtual int                  Compare(T x, T y)
		{
			//Return Values:
			//		-1 => x < y. 
			//		 0 => x = y. 
			//		+1 => x > y. 

			//Note: If we want DESC sort, we'll reverse these definitions.
			if(_desc)
			{
			    T temp = x;
				x = y;
				y = temp;
			}

			//Reference comparison (or both null)
			if((object)x == (object)y)
				return 0;
			
			//Handle null
			if(x == null || y == null)
				return y == null ? 1 : -1;
			
			//Weight
			return ((IComparable)x.Weight).CompareTo(y.Weight);
		}
	}
}
