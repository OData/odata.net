//---------------------------------------------------------------------
// <copyright file="RuntimeEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;

namespace AstoriaUnitTests
{
    /// <summary>A runtime evaluator in which expressions are bound to</summary>
    public interface IRuntimeEvaluator
    {
        /// <summary>Binary add method.</summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>The result from the binary operation</returns>
        object Add(object left, object right);

        /// <summary>Binary Substract method.</summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>The result from the binary operation</returns>
        object Subtract(object left, object right);

        /// <summary>Binary Multiply method.</summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>The result from the binary operation</returns>
        object Multiply(object left, object right);

        /// <summary>Binary Divide method.</summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>The result from the binary operation</returns>
        object Divide(object left, object right);

        /// <summary>Binary Mod method.</summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>The result from the binary operation</returns>
        object Modulo(object left, object right);

        /// <summary>Unary Round method.</summary>
        /// <param name="arg">Operand.</param>
        /// <returns>The result of rounding.</returns>
        object Round(object arg);

        /// <summary>Unary Not operator.</summary>
        /// <param name="arg">Operand.</param>
        /// <returns>The result of not.</returns>
        object Not(object arg);

        /// <summary>Unary negate operator.</summary>
        /// <param name="arg">Operand.</param>
        /// <returns>The result of negate.</returns>
        object Negate(object arg);

        /// <summary>Unary Floor method.</summary>
        /// <param name="arg">Operand.</param>
        /// <returns>The result of floor.</returns>
        object Floor(object arg);

        /// <summary>Unary Ceiling method.</summary>
        /// <param name="arg">Operand.</param>
        /// <returns>The result of Ceiling.</returns>
        object Ceiling(object arg);

        /// <summary>Performs comparison of two values.</summary>
        /// <param name="left">Left object</param>
        /// <param name="right">Right object</param>
        /// <returns>integer value of comparison result</returns>
        int Compare(object left, object right);

        /// <summary>Gives back a sequence of values of type T.</summary>
        /// <typeparam name="T">Type of objects in returned sequence.</typeparam>
        /// <param name="source">Source object which is a sequence.</param>
        /// <returns>Sequence of values of type T.</returns>
        IEnumerable<T> SequenceGetValueGeneric<T>(object source);

        /// <summary>Gives back a sequence of objects.</summary>
        /// <param name="source">Source object which is a sequence.</param>
        /// <returns>Sequence of objects.</returns>
        IEnumerable<object> SequenceGetValue(object source);
    }

    internal class RuntimeEvaluator : IRuntimeEvaluator
    {
        #region IRuntimeEvaluator Members

        public object Add(object left, object right)
        {
            if (left is double || right is double)
            {
                return (double)left + (double)right;
            }
            else if (left is float || right is float)
            {
                return (float)left + (float)right;
            }
            else if (left is decimal || right is decimal)
            {
                return (decimal)left + (decimal)right;
            }
            else if (left is long || right is long)
            {
                return (long)left + (long)right;
            }
            else if (left is int || right is int)
            {
                return (int)left + (int)right;
            }
            else if (left is short || right is short)
            {
                return (short)left + (short)right;
            }
            else if (left is byte || right is byte)
            {
                return (byte)left + (byte)right;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot add type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public object Subtract(object left, object right)
        {
            if (left is double || right is double)
            {
                return (double)left - (double)right;
            }
            else if (left is float || right is float)
            {
                return (float)left - (float)right;
            }
            else if (left is decimal || right is decimal)
            {
                return (decimal)left - (decimal)right;
            }
            else if (left is long || right is long)
            {
                return (long)left - (long)right;
            }
            else if (left is int || right is int)
            {
                return (int)left - (int)right;
            }
            else if (left is short || right is short)
            {
                return (short)left - (short)right;
            }
            else if (left is byte || right is byte)
            {
                return (byte)left - (byte)right;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot subtract type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public object Multiply(object left, object right)
        {
            if (left is double || right is double)
            {
                return (double)left * (double)right;
            }
            else if (left is float || right is float)
            {
                return (float)left * (float)right;
            }
            else if (left is decimal || right is decimal)
            {
                return (decimal)left * (decimal)right;
            }
            else if (left is long || right is long)
            {
                return (long)left * (long)right;
            }
            else if (left is int || right is int)
            {
                return (int)left * (int)right;
            }
            else if (left is short || right is short)
            {
                return (short)left * (short)right;
            }
            else if (left is byte || right is byte)
            {
                return (byte)left * (byte)right;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot multiply type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public object Divide(object left, object right)
        {
            if (left is double || right is double)
            {
                return (double)left / (double)right;
            }
            else if (left is float || right is float)
            {
                return (float)left / (float)right;
            }
            else if (left is decimal || right is decimal)
            {
                return (decimal)left / (decimal)right;
            }
            else if (left is long || right is long)
            {
                return (long)left / (long)right;
            }
            else if (left is int || right is int)
            {
                return (int)left / (int)right;
            }
            else if (left is short || right is short)
            {
                return (short)left / (short)right;
            }
            else if (left is byte || right is byte)
            {
                return (byte)left / (byte)right;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot divide type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public object Modulo(object left, object right)
        {
            if (left is double || right is double)
            {
                return (double)left % (double)right;
            }
            else if (left is float || right is float)
            {
                return (float)left % (float)right;
            }
            else if (left is decimal || right is decimal)
            {
                return (decimal)left % (decimal)right;
            }
            else if (left is long || right is long)
            {
                return (long)left % (long)right;
            }
            else if (left is int || right is int)
            {
                return (int)left % (int)right;
            }
            else if (left is short || right is short)
            {
                return (short)left % (short)right;
            }
            else if (left is byte || right is byte)
            {
                return (byte)left % (byte)right;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Cannot mopdulo type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public object Not(object arg)
        {
            if (arg is bool)
            {
                return !(bool)arg;
            }
            else
            {
                return arg != null;
            }
        }

        public object Negate(object arg)
        {
            switch (Type.GetTypeCode(arg.GetType()))
            {
                case TypeCode.Int16:
                    return -(short)arg;
                case TypeCode.SByte:
                    return -(byte)arg;
                case TypeCode.Int32:
                    return -(int)arg;
                case TypeCode.Int64:
                    return -(long)arg;
            }

            return null;
        }

        public object Ceiling(object arg)
        {
            if (arg is decimal)
            {
                return Math.Ceiling((decimal)arg);
            }
            else if (arg is Single)
            {
                return (Single)Math.Ceiling((Single)arg);
            }
            else
            {
                return Math.Ceiling((double)arg);
            }
        }

        public object Floor(object arg)
        {
            if (arg is decimal)
            {
                return Math.Floor((decimal)arg);
            }
            else if (arg is Single)
            {
                return (Single)Math.Floor((Single)arg);
            }
            else
            {
                return Math.Floor((double)arg);
            }
        }


        public object Round(object arg)
        {
            if (arg is decimal)
            {
                return Math.Round((decimal)arg);
            }
            else if (arg is Single)
            {
                return (Single)Math.Round((Single)arg);
            }
            else
            {
                return Math.Round((double)arg);
            }
        }

        public int Compare(object left, object right)
        {
            if (left != null && right != null && 
                left.GetType() != right.GetType())
            {
                if (!DemoteOperands(ref left, ref right))
                {
                    return 1;
                }
            }

            return Comparer.Default.Compare(left, right);
        }

        public IEnumerable<T> SequenceGetValueGeneric<T>(object source)
        {
            if (!(source is IEnumerable<T>))
            {
                throw new ArgumentException("source is not a sequence.");
            }

            foreach (T element in (IEnumerable<T>)source)
            {
                yield return element;
            }
        }

        public IEnumerable<object> SequenceGetValue(object source)
        {
            if (!(source is IEnumerable<object>))
            {
                throw new ArgumentException("source is not a sequence.");
            }

            foreach (object element in (IEnumerable<object>)source)
            {
                yield return element;
            }
        }

        #endregion

        private static readonly Dictionary<KeyValuePair<Type, Type>, Type> demotions = new Dictionary<KeyValuePair<Type, Type>, Type>();

        static RuntimeEvaluator()
        {
            demotions.Add(new KeyValuePair<Type, Type>(typeof(bool), typeof(int)), typeof(bool));
            demotions.Add(new KeyValuePair<Type, Type>(typeof(int), typeof(bool)), typeof(bool));
            demotions.Add(new KeyValuePair<Type, Type>(typeof(string), typeof(bool)), typeof(bool));
            demotions.Add(new KeyValuePair<Type, Type>(typeof(bool), typeof(string)), typeof(bool));
        }

        private static bool DemoteOperands(ref object left, ref object right)
        {
            Type demotedType;
            if (demotions.TryGetValue(new KeyValuePair<Type, Type>(left.GetType(), right.GetType()), out demotedType))
            {
                left = DemoteValue(left, demotedType);
                right = DemoteValue(right, demotedType);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static object DemoteValue(object input, Type demotedType)
        {
            if (input.GetType() != demotedType)
            {
                if (input.GetType() == typeof(int))
                {
                    return (int)input != 0;
                }
                else
                {
                    return ((string)input).Length != 0;
                }
            }

            return input;
        }
    }
}
