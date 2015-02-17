//---------------------------------------------------------------------
// <copyright file="NodeDiff.cs" company="Microsoft">
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
	// NodeDiffHandler
	//
	////////////////////////////////////////////////////////   
    public delegate void NodeDiffHandler<T>(T caller, NodeDiff<T> diff) where T : Node;

    ////////////////////////////////////////////////////////
	// NodeDiff
	//
	////////////////////////////////////////////////////////   
    public class NodeDiff<T> where T : Node
    {
        //Data
        protected T  _left;          //Original
        protected T  _right;         //Modified
        
        //Constructor
        public NodeDiff(T left, T right)
        {
            _left   = left;
            _right  = right;
        }

        //Accesssors
        public virtual T            Node
        {
            get { return _right ?? _left;       }
        }

        public virtual T            Left
        {
            get { return _left;                 }
        }

        public virtual T            Right
        {
            get { return _right;                }
        }

        //
        //  left   right  state (0 = null, 1 = non-null)
        // -------+------+-----------
        //  0       0       no change
        //  0       1       added
        //  1       0       deleted
        //  1       1       modified
        public virtual bool         IsAdded
        {
            get { return _left == null && _right != null;  }
        }

        public virtual bool         IsDeleted
        {
            get { return _left != null && _right == null;  }
        }

        public virtual bool         IsModified
        {
            get { return _left != null && _right != null;  }
        }

        //Overrides
        public override String      ToString()
        {
            return this.Node.Name;
        }
    }
}
