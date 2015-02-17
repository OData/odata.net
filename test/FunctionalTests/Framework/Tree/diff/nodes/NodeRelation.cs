//---------------------------------------------------------------------
// <copyright file="NodeRelation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// NodeRelation
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeRelation : NodeSet<Node>
    {
        //Data
        
        //Constructor
        public NodeRelation(String name, params Node[] nodes)
            : base(name, nodes)
        {
            _desc = "Relation";
        }

        //Accessors

        //Overrides
    }

    ////////////////////////////////////////////////////////
	// BaseType
	//
	////////////////////////////////////////////////////////   
    public class BaseType : NodeRelation
    {
        //Constructor
        public BaseType(ComplexType basetype)
            : base(null, basetype)
        {
            _desc = "BaseType";
        }

        //Accessors
        public virtual ComplexType      Base
        {
            get { return (ComplexType)_nodes.Single();          }
        }
        
        //Overrides
    }

    ////////////////////////////////////////////////////////
	// PrimaryKey
	//
	////////////////////////////////////////////////////////   
    public class PrimaryKey : NodeRelation
    {
        //Constructor
        public PrimaryKey(String name, params NodeProperty[] properties)
            : base(name, properties)
        {
            _desc = "Key";
        }

        //Accessors
        public virtual IEnumerable<NodeProperty>    Properties
        {
            get { return _nodes.OfType<NodeProperty>();     }
        }

        /// <summary>Number of properties that make up the primary key.</summary>
        public int PropertyCount
        {
            get { return this.Properties.Count(); }
        }
    }

    ////////////////////////////////////////////////////////
	// ForeignKey
	//
	////////////////////////////////////////////////////////   
    public class ForeignKey : NodeRelation
    {
        //Data
        protected PrimaryKey _pk;

        //Constructor
        public ForeignKey(String name, PrimaryKey pk, params NodeProperty[] properties)
            : base(name, properties)
        {
            _desc = "ForeignKey";
            _pk = pk;
        }

        //Accessors
        public virtual PrimaryKey                   PrimaryKey
        {
            get { return _pk;                               }
        }

        public virtual IEnumerable<NodeProperty>    Properties
        {
            get { return _nodes.OfType<NodeProperty>();     }
        }
        
        //Overrides
        public override Object                      Clone()
        {
            ForeignKey clone    = (ForeignKey)base.Clone();
            clone._pk           = (PrimaryKey)this._pk.Clone();
            return clone;
        }
    }

    ////////////////////////////////////////////////////////
	// NodeRelations
	//
	////////////////////////////////////////////////////////   
    public class NodeRelations : Nodes<NodeRelation>
    {
        //Constructor
        public NodeRelations(Node parent, params NodeRelation[] relations)
            : base(parent, relations)
        {
        }
        
        //Accessors
        public virtual ComplexType              BaseType
        {
            get { return this.OfType<BaseType>().Select(b => b.Base).FirstOrDefault();          }
            set 
            { 
                this.Remove(this.OfType<BaseType>().FirstOrDefault());       //Clear previous
                if(value != null)
                    this.Add(new BaseType(value));                          //Add
            }
        }

        public virtual PrimaryKey               PrimaryKey
        {
            get { return this.OfType<PrimaryKey>().FirstOrDefault();        }
            set 
            { 
                this.Remove(this.PrimaryKey);                               //Clear previous
                if(value != null)
                    this.Add(value);                                        //Add
            }
        }

        public virtual IEnumerable<ForeignKey>  ForeignKeys
        {
            get { return this.OfType<ForeignKey>();                         }
            set 
            { 
                this.Remove(this.ForeignKeys.ToArray());                    //Clear previous
                if(value != null)
                    this.Add(value.ToArray());                              //Add
            }
        }

        //Internal
        public virtual void                   Infer()
        {
            ComplexType type = _parent as ComplexType;
            if(type == null)
                return;
                
            //Pull-up additional relations, if they were specified at the property level
            var relations = type.Properties.SelectMany(p => p._other.OfType<NodeRelation>());
            
            //PK/FK
            this.InferPrimaryKey(relations.OfType<PrimaryKey>().ToArray());
            this.InferForeignKey(relations.OfType<ForeignKey>().ToArray());
            
            //BaseType
            BaseType basetype = relations.OfType<BaseType>().SingleOrDefault();
            if(basetype != null)
                type.BaseType = basetype.Base; 

            //Conditions
            var conditions = type.Properties.SelectMany(p => p._other.OfType<ExpNode>());
            foreach(ExpNode condition in conditions)
            {
                ExpNode c = condition;
                if(type.Condition != null)
                    c = Exp.And(type.Condition, c);
                type.Condition = c;
            }
        }
        
        protected virtual void                  InferPrimaryKey(PrimaryKey[] keys)
        {
            //There can only be 1-PK, so all get collasped into a compound key
            if(keys.Length > 0)
            {
                PrimaryKey relation = 
                    (PrimaryKey)this.Add(
                        new PrimaryKey(
                            keys.Select(pk => pk.Name).First(),                    //Name
                            keys.Select(pk => (NodeProperty)pk.Parent).ToArray()   //Columns
                            )
                        );

                //Default name (PK_Customers)
                if(relation.Name == null)
                    relation.Name = "PK_" + relation.Parent;
            }
        }

        protected virtual void                  InferForeignKey(ForeignKey[] keys)
        {
            //There can be N-FKs, each get collasped based upon referenced PK
            var groupbypk = keys.GroupBy(fk => fk.PrimaryKey).ToArray();
            foreach(var group in groupbypk)
            {
                ForeignKey relation = 
                    (ForeignKey)this.Add(
                        new ForeignKey(
                            group.Select(g => g.Name).First(),                  //Name
                            group.Key,                                          //PK
                            group.Select(g => (NodeProperty)g.Parent).ToArray() //Columns
                            )
                        );

                //Default name (FK_CustomersOrders)
                if(relation.Name == null)
                    relation.Name = "FK_" + relation.PrimaryKey.Parent + relation.Parent;
            }
        }
    }
}
