//---------------------------------------------------------------------
// <copyright file="NodeExecutor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;      //StringBuilder

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodeExecutor
	//
	////////////////////////////////////////////////////////   
    public class NodeExecutor<T> where T : Node
    {
        //Data
        protected T                  _currenttree;
        protected NodeComparer<T>    _comparer;
        protected NodeVerifier<T>    _verifier;
        protected NodeDiffHandler<T> _handler;

        //Constructor
        public NodeExecutor(NodeVisitor<T> visitor, NodeDiffHandler<T> handler)
            : base()
        {
            _comparer   = new NodeComparer<T>(visitor);    //Default comparer - override for specifics
            _verifier   = new NodeVerifier<T>(_comparer);  //Default verifier - override for specifics
            _handler    = handler;
        }

        //Accessors

        //Helpers
        public virtual void                 Clear()
        {
            //TODO:
            _currenttree = null;
        }

        public virtual void                 Execute(T[] expected)
        {
            //Compare the trees
            T root = this.Root(expected);
            _comparer.Compare(_currenttree, root, _handler);
            
            //On success, update our answer
            _currenttree = (T)root.Clone();
        }

        public virtual void                 Verify(T actual, T[] expected)
        {
            T root = this.Root(expected);
            if(root == null)
                root = _currenttree;
            
            //Comparer the trees
            //TODO: Physical later actual is currently missing,
            if(actual == null)
                actual = root;
            _verifier.Verify(actual, root);

            //On success, update our answer
            _currenttree = (T)root.Clone();
        }

        //Internal
        protected virtual T                 Root(params T[] nodes)
        {
            //Note: We place all nodes into a top level node.  This way
            //you can Execute with a set of 'disjoint' nodes without having to always place them in 
            //an entity set to get correct diffs.
            return (T)(Node)new NodeSet<T>("Root", nodes);
        }
    }
}
