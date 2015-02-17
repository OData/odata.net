//---------------------------------------------------------------------
// <copyright file="NodeFinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;                   //IEnumerable
using System.Collections.Generic;           //List<T>
using System.Collections.ObjectModel;       //ReadOnlyCollection

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodeFinder
	//
	////////////////////////////////////////////////////////   
    public class NodeFinder<T> where T : Node
    {
        //Data
        protected T              _findnode;
        protected T              _findcaller;
        protected T              _found;
        protected NodeVisitor<T> _visitor;

        //Constructor
        public NodeFinder(NodeVisitor<T> visitor)
        {
            _visitor = visitor;
        }

        //Helpers
        public virtual T            Find(T caller, T node, T tree)
        {
            //State
            _findcaller = caller;
            _findnode   = node;
            _found      = null;  

            //Visit
            _visitor.Process(tree, new NodeHandler<T>(OnNode));
            
            //Return
            return _found;
        }

        //Overrides
        protected virtual bool      OnNode(T caller, T node)
        {
            if(this.SameIdentity(caller, _findcaller) &&
                this.SameIdentity(node, _findnode))
            {
                _found = node;
                return false;       //Done
            }
            return true;            //Continue
        }

        //Internal
        protected virtual bool      SameIdentity(T a, T b)
        {
            //Determine if same identity (ie: a = Customer, b = Customer) although they might be different
            //instances or contain different values, properties, facets.  We basically need a way to determine
            //identity (without requiring guids). 
            if(a == null || b == null)
                return a == b;
             
            //Types
            if(a.GetType() != b.GetType())
                return false;
                
            //Name (currently our primary identity)
            if(a.Name != b.Name)
                return false;
            
            //Ensure the parents are the same
            if(!this.SameIdentity((T)a.Parent, (T)b.Parent))
                return false;
                
            return true;
        }
    }
}
