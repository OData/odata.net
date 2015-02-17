//---------------------------------------------------------------------
// <copyright file="NodeProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using AstoriaUnitTests.Data;

    ////////////////////////////////////////////////////////
	// NodeProperty
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeProperty : Node
    {
        //Data
        protected NodeType              _type;
        
        //Constructor
        public NodeProperty(String name, NodeType type, params Node[] facets)
            : base(name, facets)
        {
            _type   = type;
            _desc   = "Property";
        }
        
        //Accessors
        public virtual NodeType                     Type
        {
            get { return _type;                                                             }
            set { _type = value;                                                            }
        }

        public virtual PrimaryKey                   PrimaryKey
        {
            get { return this.Relations.OfType<PrimaryKey>().SingleOrDefault();             }
        }

        public virtual IEnumerable<ForeignKey>      ForeignKeys
        {
            get { return this.Relations.OfType<ForeignKey>();                               }
        }

        public virtual IEnumerable<NodeRelation>    Relations
        {
            get 
            { 
                //Relations are stored on the parent (complex type), instead of duplicating them in both places.
                if(this.Parent is ComplexType)
                    return ((ComplexType)this.Parent).Relations.Where(r => r.Nodes[this.Name] != null);
                return null;
            }
        }

        public override Object                      Clone()
        {
            NodeProperty clone  = (NodeProperty)base.Clone();
            //Note: We purposely don't clone types, so we can always do identity checking
            return clone;
        }

        /// <summary>Returns a sample value for this type of node.</summary>
        /// <returns>A sample value for this type of node.</returns>
        public virtual object GetSampleValue()
        {
            // TODO: additional Facets should be examined to ensure the sample
            // value does not violate constraints.
            return this._type.CreateValue().ClrValue;
        }
    }
}
