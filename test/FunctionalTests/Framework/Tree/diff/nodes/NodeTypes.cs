//---------------------------------------------------------------------
// <copyright file="NodeTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using AstoriaUnitTests.Data;

    ////////////////////////////////////////////////////////
	// NodeType
	//
	////////////////////////////////////////////////////////   
    public abstract class NodeType : Node, IComparer
    {
        //Constructor
        public NodeType(String name, Type clrtype, params NodeFacet[] facets)
            : base(name, clrtype !=null? clrtype.Namespace:name, facets)
        {
            ClrType = clrtype;
            Default = this.Value(null);
            Null = this.Value(null);
            _desc = "Type";
        }

        public NodeType(String name, String typeNamespace, params NodeFacet[] facets)
            : base(name,typeNamespace, facets)
        {
            Default = this.Value(null);
            Null = this.Value(null);
            _desc = "Type";
        }
        
        //Accessors
        public virtual Type ClrType
        {
            get;
            set;
        }

        public virtual Type ClientClrType
        {
            get;
            set;
        }

        public virtual Type NullableClientClrType
        {
            get
            {
                return (ClientClrType.IsValueType) ? typeof(Nullable<>).MakeGenericType(this.ClientClrType)
                    : ClientClrType;
            }
        }


        /// <summary>Whether this type represents some form of numeric tpye.</summary>
        public virtual bool IsNumeric
        {
            get
            {
                return TypeData.IsTypeNumeric(this.ClrType);
            }
        }
       
        public virtual Type NullableClrType
        {
            get
            {
                return (ClrType.IsValueType) ? typeof(Nullable<>).MakeGenericType(this.ClrType)
                    : ClrType;
            }
        }

        public virtual NodeValue Default
        {
            get;
            protected set;
        }

        public virtual NodeValue Null
        {
            get;
            private set;
        }

        //Abstract
        public abstract NodeValue CreateValue();
        public abstract int Compare(Object x, Object y);

        //Methods
        public virtual NodeValue Value(Object value)
        {
            return new NodeValue(value, this);
        }

        public virtual String ToString(Object value)
        {
            //Override for specifics
            if(value == null)
                return null;
            return value.ToString();
        }

        public override Object Clone()
        {
            //Nothing else to deep copy
            return base.Clone();
        }
    }

    ////////////////////////////////////////////////////////
	// PrimitiveType
	//
	////////////////////////////////////////////////////////   
    public abstract class PrimitiveType : NodeType
    {
        //Data
        protected   Decimal   _max;
        protected   Decimal   _min;
        protected   bool      _comparable   = true;
        protected   bool      _indexable    = true;
                
        //Constructor
        public PrimitiveType(String name, Type clrtype, params NodeFacet[] facets)
            : base(name, clrtype, facets)
        {
        }

        //Accessors
        public virtual Decimal              Max
        {
            get { return _max;                  }
        }

        public virtual Decimal              Min
        {
            get { return _min;                  }
        }

        public virtual bool                 Comparable
        {
            get { return _comparable;           }
        }

        public virtual bool                 Indexable
        {
            get { return _indexable;            }
        }


        public virtual NodeValue CreateRandomValueForFacets(NodeFacets propertyFacets)
        {
            return this.CreateRandomValue();
        }
        //Overrides
        public virtual NodeValue CreateRandomValue()
        {
            // TODO: consider adding some random version of this; determinism is now required because
            // this is used by check-in tests.
            TypeData typeData = TypeData.FindForType(this.ClrType);
            int index = AstoriaTestProperties.Random.Next(typeData.SampleValues.Length);
            
            return new NodeValue(typeData.SampleValues[index], this);
        }
        public virtual NodeValue CreateValueForFacets(NodeFacets propertyFacets)
        {
            return this.CreateValue();
        }
        //Overrides
        public override NodeValue CreateValue()
        {
            // TODO: consider adding some random version of this; determinism is now required because
            // this is used by check-in tests.
            TypeData typeData = TypeData.FindForType(this.ClrType);
            return new NodeValue(typeData.NonNullValue, this);
        }

        public override int                 Compare(Object x, Object y)
        {
            //Handle null
            if(x == null || y == null)
                return (x == y) ? 0 : (x == null ? -1 : 1);
                
            //Ensure the same type
            if (ClrType != x.GetType() || ClrType != y.GetType())
                throw new Exception("Unable to compare different types: " + x.GetType().Name + " to " + y.GetType().Name);
                
            //For primitives simply delegate to IComparable
            if(x is IComparable)
                return ((IComparable)x).CompareTo(y);
                
            //Otherwise, you need to override this method
            throw new Exception(this.Name + ":" + x.GetType().Name + " isn't comparable, override Compare for type specific comparison.");
        }

        //Overrides
        public override Object          Clone()
        {
            //Note: We purposely don't clone types, so we can always do identity checking
            throw new Exception("Clone of primitive types is purposely not supported");
        }
        public virtual bool IsApproxPrecision
        {
            get { return false; }
        }
        public virtual bool IsApproxValueComparable(NodeValue nodeValue)
        {
            return true;
        }
    }

    ////////////////////////////////////////////////////////
	// PrimitiveType<T>
	//
	////////////////////////////////////////////////////////   
    public abstract class PrimitiveType<T> : PrimitiveType
    {
        //Data
                
        //Constructor
        public PrimitiveType(params NodeFacet[] facets)
            : base(typeof(T).Name, typeof(T), facets)
        {
            Default = this.Value(default(T));
        }
    }
    
    ////////////////////////////////////////////////////////
	// ComplexType
	//
	////////////////////////////////////////////////////////   
    public class ComplexType : NodeType
    {
        //Data
        protected Nodes<NodeProperty>   _properties;
        protected NodeRelations         _relations;
        protected ExpNode               _condition; //Note: Condition can be many (ie: x AND y AND z)
                
        //Constructor
        public ComplexType(String name, Type clrtype, params Node[] properties)
            : base(name, clrtype, properties.OfType<NodeFacet>().ToArray())
        {
            _properties = new Nodes<NodeProperty>(this, properties.OfType<NodeProperty>());
            _relations  = new NodeRelations(this, properties.OfType<NodeRelation>().ToArray());
            DerivedTypes = new List<ComplexType>();
            
            //Pull-up additional relations, if they were specified at the property level
            this.Relations.Infer();
        }
        public ComplexType(String name, string typeNamespace, params Node[] properties)
            : base(name, typeNamespace, properties.OfType<NodeFacet>().ToArray())
        {
            _properties = new Nodes<NodeProperty>(this, properties.OfType<NodeProperty>());
            _relations = new NodeRelations(this, properties.OfType<NodeRelation>().ToArray());
            DerivedTypes = new List<ComplexType>();

            //Pull-up additional relations, if they were specified at the property level
            this.Relations.Infer();
        }

        public ComplexType(String name, string typeNamespace, ComplexType baseType, params Node[] properties)
            : base(name, typeNamespace, properties.OfType<NodeFacet>().ToArray())
        {
            _properties = new Nodes<NodeProperty>(this, baseType.Properties.Union(properties.OfType<NodeProperty>()));
            _relations = new NodeRelations(this, properties.OfType<NodeRelation>().ToArray());
            DerivedTypes = new List<ComplexType>();

            this.BaseType = baseType;
            this.BaseType.DerivedTypes.Add(this);
            foreach (ComplexType ancestor in this.BaseType.BaseTypes)
                ancestor.DerivedTypes.Add(this);
            this.Relations.Infer();
        }

        public bool HasDerivedTypes
        {
            get
            {
                return DerivedTypes.Any();
            }
        }

        public List<ComplexType> DerivedTypes 
        { 
            get; 
            private set; 
        }
        
        //Accessors
        public virtual Nodes<NodeProperty>      Properties
        {
            get { return _properties;               }
        }

        public virtual ComplexType              BaseType
        {
            //Simple helper, otherwise use the relations collection direclty
            get { return this.Relations.BaseType;   }
            set { this.Relations.BaseType = value;  }
        }

        public virtual IEnumerable<ComplexType> BaseTypes
        {
            get 
            {
                //Simple helper to obtain all base types, in the chain
                ComplexType b = this.BaseType;
                while(b != null)
                {
                    yield return b;
                    b = b.BaseType;
                }
            }
        }

        public virtual ExpNode                  Condition
        {
            get { return _condition;                }
            set { _condition = value;               }
        }

        public virtual NodeRelations            Relations
        {
            get { return _relations;                }
        }

        //Overrides
        public override NodeValue               CreateValue()
        {
            //TODO:
            throw new NotImplementedException("Type '" + this.GetType() + "' does not implement CreateValue().");
        }

        public override int                     Compare(Object x, Object y)
        {
            //TODO:
            throw new NotImplementedException("Type '" + this.GetType() + "' does not implement Compare(object, object).");
        }

        public override Object                  Clone()
        {
            ComplexType clone   = (ComplexType)base.Clone();
            clone._properties   = (Nodes<NodeProperty>)this.Properties.Clone();
            clone._relations    = (NodeRelations)this.Relations.Clone();
            return clone;
        }
    }

    ////////////////////////////////////////////////////////
	// CollectionType
	//
	////////////////////////////////////////////////////////   
    public abstract class CollectionType : NodeType
    {
        //Data
        protected NodeType  _subtype;
                
        //Constructor
        public CollectionType(NodeType subtype, params NodeFacet[] facets)
            : base(subtype.Name + "[]",(Type) null, facets)
        {
            _subtype = subtype;
        }
        
        //Accessors
        public virtual NodeType                 SubType
        {
            get { return _subtype;                  }
        }

        public override Object                  Clone()
        {
            CollectionType clone   = (CollectionType)base.Clone();
            //Note: We purposely don't clone types, so we can always do identity checking
            return clone;
        }
    }

    ////////////////////////////////////////////////////////
	// PrimitiveOrComplexCollectionType
	//
	////////////////////////////////////////////////////////  
    public class PrimitiveOrComplexCollectionType : NodeType
    {
        protected NodeType _subtype;

        //Constructor
        public PrimitiveOrComplexCollectionType(NodeType subtype, params NodeFacet[] facets)
            : base(subtype.Name + "[]", GetClrType(subtype), facets)
        {
            _subtype = subtype;
            this.ClientClrType = typeof(List<>).MakeGenericType(subtype.ClientClrType ?? subtype.ClrType);
        }
        
        //Accessors
        public virtual NodeType                 SubType
        {
            get { return _subtype;                  }
        }

        public override Object                  Clone()
        {
            CollectionType clone   = (CollectionType)base.Clone();
            return clone;
        }

        //Overrides
        public override NodeValue CreateValue()
        {
            throw new NotImplementedException("Type '" + this.GetType() + "' does not implement CreateValue().");
        }

        public override int Compare(Object x, Object y)
        {
            throw new NotImplementedException("Type '" + this.GetType() + "' does not implement Compare(object, object).");
        }

        private static Type GetClrType(NodeType subtype)
        {
            if (subtype.ClrType != null)
            {
                return typeof(IEnumerable<>).MakeGenericType(subtype.ClrType);
            }

            return null;
        }
    }

    ////////////////////////////////////////////////////////
	// NodeTypes
	//
	////////////////////////////////////////////////////////   
    public class NodeTypes<T> : Nodes<T> where T : NodeType
    {
        //Data
        
        //Constructor
        public NodeTypes(Node parent, params T[] array)
            : base(parent, array)
        {
        }

        public NodeTypes(Node parent, IEnumerable<T> array)
            : base(parent, array)
        {
        }

        //Accessors
        public virtual T                this[Type clrtype]
        {
            get { return this.Where(t => t.ClrType == clrtype).Single();                        }
        }

        //Derived classes
        public virtual NodeTypes<PrimitiveType>     Primitive
        {
            get { return new NodeTypes<PrimitiveType>(_parent, this.OfType<PrimitiveType>());   }
        }

        public virtual NodeTypes<ComplexType>       Complex
        {
            get { return new NodeTypes<ComplexType>(_parent, this.OfType<ComplexType>());       }
        }
        /*
        public virtual NodeTypes<ResourceContainerNode>       ResourceContainer
        {
            get { return new NodeTypes<ComplexType>(_parent, this.OfType<ComplexType>());       }
        }*/
        //Common Accessor
        public virtual T                Boolean
        {
            get { return this[typeof(bool)];        }
        }

        public virtual T                Int32
        {            
            get { return this[typeof(int)];         }
        }
        public virtual T Int64
        {
            get { return this[typeof(Int64)]; }
        }
        public virtual T Int16
        {
            get { return this[typeof(short)];       }
        }

        public virtual T Float
        {
            get { return this[typeof(float)];       }
        }

        public virtual T                String
        {   
            get { return this[typeof(string)];      }
        }

        public virtual T                Decimal
        {   
            get { return this[typeof(decimal)];     }
        }

        public virtual T                DateTime
        {   
            get { return this[typeof(DateTime)];    }
        }
        public virtual T                Byte
        {
            get { return this[typeof(Byte)]; }
        }
        public virtual T SByte
        {
            get { return this[typeof(SByte)]; }
        }
        public virtual T                Single
        {
            get { return this[typeof(Single)]; }
        }
        public virtual T                Double
        {
            get { return this[typeof(Double)]; }
        }
        public virtual T Binary
        {
            get { return this[typeof(byte[])]; }
        }
        public virtual T Guid
        {
            get { return this[typeof(Guid)]; }
        }
    }

    ////////////////////////////////////////////////////////
	// NodeTypes
	//
	////////////////////////////////////////////////////////   
    public class NodeTypes : NodeTypes<NodeType>
    {
        //Data
        
        //Constructor
        public NodeTypes(Node parent, params NodeType[] array)
            : base(parent, array)
        {
        }

        //Accessors
    }
}
