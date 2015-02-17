//---------------------------------------------------------------------
// <copyright file="fxBits.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;       //IComparer
using System.Text;              //StringBuilder

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    //fxBits
    //
    ////////////////////////////////////////////////////////
    public class fxBits<T>
    {
        //Data
        int _bits;

        public bool             this[T bit]
        {
            get { return Is(bit);       }
            set { Set(bit, value);      }
        }

        public bool             Is(T bit) 
        {
            int b = (int)(object)bit;   //cast away T
            return (_bits & b) == b;
        }

        public fxBits<T>         Set(T bit, bool value)
        {
            if(value)
                Set(bit);
            else
                Clear(bit);
            return this;
        }

        public fxBits<T>         Set(T bit)
        {
            int b = (int)(object)bit;   //cast away T
            _bits |= b;
            return this;
        }

        public fxBits<T>         Clear(T bit)
        {
            int b = (int)(object)bit;   //cast away T
            _bits &= ~b;
            return this;
        }

        public fxBits<T>         Clear()
        {
            _bits = 0;
            return this;
        }

        public T                Value
        {
            get { return (T)(object)_bits;      }
            set { _bits = (int)(object)value;   }
        }
    }

    ////////////////////////////////////////////////////////
    //fxLongBits
    //
    ////////////////////////////////////////////////////////
    public class fxLongBits<T>
    {
        //Data
        ulong _bits;

        public bool             this[T bit]
        {
            get { return Is(bit);       }
            set { Set(bit, value);      }
        }

        public bool             Is(T bit) 
        {
            ulong b = Convert.ToUInt64((object)bit);   //cast away T
            return (_bits & b) == b;
        }

        public fxLongBits<T>         Set(T bit, bool value)
        {
            if(value)
                Set(bit);
            else
                Clear(bit);
            return this;
        }

        public fxLongBits<T>         Set(T bit)
        {
            ulong b = Convert.ToUInt64((object)bit);   //cast away T
            _bits |= b;
            return this;
        }

        public fxLongBits<T>         Clear(T bit)
        {
            ulong b = Convert.ToUInt64((object)bit);   //cast away T
            _bits &= ~b;
            return this;
        }

        public fxLongBits<T>         Clear()
        {
            _bits = 0;
            return this;
        }

        public T                Value
        {
            get
            {
                return (T)Convert.ChangeType(_bits, Enum.GetUnderlyingType(typeof(T)));
            }
                
            set { _bits = Convert.ToUInt64((object)value); }
        }
    }
}
