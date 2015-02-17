//---------------------------------------------------------------------
// <copyright file="NodePrinter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //Stack<T>
using System.Text;                      //StringBuilder

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodePrinter
	//
	////////////////////////////////////////////////////////   
    public class NodePrinter<T> where T : Node
    {
        //Data
        protected NodeVisitor<T>            _visitor;
        protected StringBuilder             _buffer;    
        protected Stack<T>                  _stack;
        protected int                       _indent;

        //Constructor
        public NodePrinter(NodeVisitor<T> visitor)
            : base()
        {
            _buffer     = new StringBuilder();
            _visitor    = visitor;
            _stack      = new Stack<T>();
        }

        //Helpers
        public virtual String               Format(T tree)
        {
            _buffer.Length = 0; //Clear
            
            //Visit the tree
            _visitor.Process(tree, new NodeHandler<T>(OnNode));

            //Display                
           return _buffer.ToString();
        }

        //Overrides
        protected virtual bool             OnNode(T caller, T node)
        {
            _buffer.AppendFormat("{0}{1}{2}\n",
                this.Prefix(caller, node),
                this.Format(caller, node),
                this.Suffix(caller, node)
                );

            return true;    //Continue
        }   

        //Internal
        protected virtual String            Format(T caller, T node)
        {
            return node.Name;
        }

        protected virtual String            Prefix(T caller, T node)
        {
            if(_stack.Count == 0)
            {
                _stack.Push(caller);
                _indent++;
            }
            else if(_stack.Contains(caller))
            {
                while(_stack.Peek() != caller)
                {
                    _stack.Pop();
                    _indent--;
                }
            }           
            else
            {
                _stack.Push(caller);
                _indent++;
            }
            
            return new String(Enumerable.Repeat('\t', _indent).ToArray());
         }

        protected virtual String            Suffix(T caller, T node)
        {
            return null;              
        }
    }
}
