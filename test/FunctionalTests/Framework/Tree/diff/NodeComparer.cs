//---------------------------------------------------------------------
// <copyright file="NodeComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;                   //IEnumerable
using System.Collections.Generic;           //List<T>

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodeComparer
	//
	////////////////////////////////////////////////////////   
    public class NodeComparer<T> where T : Node
    {
        //Data
        protected NodeVisitor<T>         _visitor;
        protected NodeDiffHandler<T>     _handler;
        protected T                      _a;
        protected T                      _b;        
        protected bool                   _same;

        //Constructor
        public NodeComparer(NodeVisitor<T> visitor)
        {
            _visitor = visitor;
        }

        //Overrides
        public virtual bool                 Compare(T a, T b, NodeDiffHandler<T> handler)
        {
            //State
            _handler= handler;
            _a      = a;
            _b      = b;
            _same   = true;
            
            //Simple Algortyhm: 
            //  We have a simple rountine that compares(a -> b).
            //  It does this through a brain-dead find routine ie: find(a in b).
            //
            //  That would give you edits + removals, but unfortunatly doesn't give you additions in b.
            //  So we simply do it twice (ie: find(a, b), then find(b, a), which equates to:
            //      Compare(a, b) = covers: 
            //                       #1 b's removals (ie: deletions) 
            //                       #2 b's edits (ie: changes)
            //      Compare(b, a) = coers:
            //                       #3 a's removals (ie: insertions)
            //                       #4 a's edits from b (ie: changes we don't care about)
                            
            //First pass: Compare(a -> b)
            _visitor.Process(a, new NodeHandler<T>(OnLeft));
            
            //Second pass: Compare(b -> a)
            _visitor.Process(b, new NodeHandler<T>(OnRight));
            
            return _same;
        }

        //Events
        protected virtual bool              OnLeft(T caller, T a)
        {
            NodeFinder<T> b = new NodeFinder<T>(_visitor);
            T found = b.Find(caller, a, _b);
            
            if(found == null)
            {
                _handler(caller, new NodeDiff<T>(a, found));    //Delete
                _same = false;
                return false;   //Stop processing deletions
            }
            else if(found.CompareTo(a) != 0)
            {
                _handler(caller, new NodeDiff<T>(a, found));    //Edit
                _same = false;
            }
            return true;
        }

        protected virtual bool              OnRight(T caller, T b)
        {
            NodeFinder<T> a = new NodeFinder<T>(_visitor);
            T found = a.Find(caller, b, _a);
            
            if(found == null)
            {
                _handler(caller, new NodeDiff<T>(found, b));    //Insertion
                _same = false;
            }
            
            //Note: We ignore edits in this case.
            return true;
        }
    }
}
