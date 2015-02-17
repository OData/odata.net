//---------------------------------------------------------------------
// <copyright file="NodeVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;      //StringBuilder

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodeVerifier
	//
	////////////////////////////////////////////////////////   
    public class NodeVerifier<T> where T : Node
    {
        //Data
        protected NodeComparer<T>    _comparer;
        protected StringBuilder      _buffer;

        //Constructor
        public NodeVerifier(NodeComparer<T> comparer)
            : base()
        {
            _comparer   = comparer;
        }

        //Helpers
        public virtual void                 Verify(T actual, T expected)
        {
            _buffer = new StringBuilder();
            
            //Diff the trees
            if(!_comparer.Compare(actual, expected, new NodeDiffHandler<T>(OnVerify)))
            {
                //Failed - output all differences
                throw new Exception("Verification Failed: \n" + 
                                _buffer.ToString()
                                );    
            }

            //Passed
            System.Data.Test.Astoria.AstoriaTestLog.TraceLine("Verification Passed, no differences...");
        }

        //Overrides
        protected virtual void              OnVerify(T caller, NodeDiff<T> diff)
        {
            String verb = diff.IsDeleted ? "Missing" : diff.IsAdded ? "Unexpected" : "Different";
                    
            _buffer.AppendFormat(
                "{0}: {1}({2}, {3})", verb, diff.Node.Desc, (caller != null ? caller.Name : null), diff.Node.ToString()
                );
        }   
    }
}
