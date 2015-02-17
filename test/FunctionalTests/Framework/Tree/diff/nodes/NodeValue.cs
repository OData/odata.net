//---------------------------------------------------------------------
// <copyright file="NodeValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;               //IEnumerable
using System.Collections.Generic;       //List<T>

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // NodeValue
    //
    ////////////////////////////////////////////////////////   
    public class NodeValue : Node
    {
        //Data
        protected NodeType _type;
        protected Object _value;

        //Constructor
        public NodeValue(string name, Object value, NodeType type)
            : base(name, Node.EmptyNodes)
        {
            _type = type;
            _value = value;
            _desc = "Value";
        }

        public NodeValue(Object value, NodeType type)
            : this(null, value, type)
        {
        }

        //Accessors
        public virtual NodeType Type
        {
            get { return _type; }
        }

        public virtual Object ClrValue
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual object NullableClrValue
        {
            get
            {

                Type[] typeargs = new Type[] { _value.GetType() };
                System.Reflection.ConstructorInfo constructor = typeof(Nullable<>).MakeGenericType(_value.GetType()).GetConstructor(typeargs);
                object nullableValue = constructor.Invoke(new object[] { _value });

                return nullableValue;
            }
        }

        public virtual Type NullableClrType
        {
            get
            {
                Type[] typeargs = new Type[] { _value.GetType() };
                return typeof(Nullable<>).MakeGenericType(_value.GetType());
            }
        }

        //Abstract
        public override int CompareTo(Node y)
        {
            //Delegate to the type for specifics
            return _type.Compare(this.ClrValue, y != null ? ((NodeValue)y).ClrValue : null);
        }

        //Overrides
        public override String ToString()
        {
            return _type.ToString(this.ClrValue);
        }

        public override Object Clone()
        {
            NodeValue clone = (NodeValue)base.Clone();
            //Note: We purposely don't clone types, so we can always do identity checking
            return clone;
        }
    }
}
