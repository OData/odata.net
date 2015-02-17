//---------------------------------------------------------------------
// <copyright file="NodeVisitor.cs" company="Microsoft">
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
	// NodeHandler
	//
	////////////////////////////////////////////////////////   
    public delegate bool NodeHandler<T>(T caller, T node) where T : Node;

    ////////////////////////////////////////////////////////
	// NodeVisitor<T,O>
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeVisitor<T,O> where T : Node
    {
        //Data
        protected NodeHandler<T> _handler;

        //Constructor
        protected NodeVisitor()
        {
        }

        //Methods
        public    virtual  O                    Process(T tree, NodeHandler<T> handler)
        {
            NodeHandler<T> old = _handler;
            try
            {
                _handler = handler;
                return this.Visit(default(T), tree);
            }
            finally
            {            
                _handler = old;
            }
        }

        protected virtual bool                  OnNode(T caller, T node)
        {
            if(_handler != null)
                return _handler(caller, node);
            return true;
        }

        //Overrides
        protected virtual O[]                   Visit(T caller, T[] nodes)
        {
            //Children
            return nodes.Select(node => this.Visit(caller, node)).ToArray();
        }

        protected abstract O                    Visit(T caller, T node);
    }

    ////////////////////////////////////////////////////////
	// NodeVisitor<T>
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeVisitor<T> : NodeVisitor<T, Object> where T : Node 
    {
        //Data

        //Constructor
        protected NodeVisitor()
        {
        }

    }

    ////////////////////////////////////////////////////////
	// NodeVisitor<T>
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeVisitor : NodeVisitor<Node>
    {
        //Data

        //Constructor
        protected NodeVisitor()
        {
        }

        //Accessors
        protected override Object               Visit(Node caller, Node node)
        {
            if(node == null)
            {
                return null;
            }
            else if(node is ComplexType)
            {
                ComplexType e       = (ComplexType)(Node)node;
                
                //Entity
                if(!this.OnNode(caller, e))
                    return null;

                //Properties
                this.Visit(e, e.Properties.ToArray());
            
                //Relations (including basetype)
                return this.Visit(e, e.Relations.ToArray());
            }
            else if(node is NodeProperty)
            {
                NodeProperty e      = (NodeProperty)node;
                
                //Property
                if(!this.OnNode(caller, e))
                    return null;

                //Type
                this.Visit(e, e.Type);

                //Facets
                return this.Visit(e, e.Facets.ToArray());
            }
            else if(node is NodeType)
            {
                NodeType e         = (NodeType)node;
                
                //Type
                if(!this.OnNode(caller, e))
                    return null;

                //Facets
                //TODO
                return null;
            }
            else if(node is NodeRelation)
            {
                NodeRelation e         = (NodeRelation)node;

                //Relation
                if(!this.OnNode(caller, e))
                    return null;

                //Properties
                //TODO:
                return null;
            }
            else if(node is NodeFacet)
            {
                NodeFacet e         = (NodeFacet)node;

                //Facet
                if(!this.OnNode(caller, e))
                    return null;

                //Value
                return this.Visit(e, e.Value);
            }
            else if(node is NodeValue)
            {
                NodeValue e         = (NodeValue)node;

                //Value
                if(!this.OnNode(caller, e))
                    return null;
                
                return null;
            }
            else if(node is NodeSet)
            {
                NodeSet e      = (NodeSet)node;

                //Set
                if(!this.OnNode(caller, e))
                    return null;

                //Children
                return this.Visit(e, e.ToArray());
            }
            else
            {
                //Delegate
                throw new Exception(this.GetType().Name + " Unhandled Node: " + node.GetType().Name);
            }
        }
    }
}
