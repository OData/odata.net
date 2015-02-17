//---------------------------------------------------------------------
// <copyright file="OpenTypeMethodsImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using DSP = Microsoft.OData.Service.Providers;
using Microsoft.OData.Service;
using System.Collections;
using System.Data.Test.Astoria.CallOrder;

namespace System.Data.Test.Astoria.LateBound
{
    public enum OpenTypeMethodsImplementations
    {
        Default,
        Realistic,
        Tolerant
    }

    #region base class
    public abstract class OpenTypeMethodsImplementation : IComparer<object>
    {
        public static OpenTypeMethodsImplementation GetImplementation(OpenTypeMethodsImplementations type)
        {
            return GetImplementation(type, false);
        }

        public static OpenTypeMethodsImplementation GetImplementation(OpenTypeMethodsImplementations type, bool lazyEvaluation)
        {
            OpenTypeMethodsImplementation impl = null;
            switch(type)
            {
                case OpenTypeMethodsImplementations.Default:
                    impl = new DefaultOpenTypeMethodsImplementation();
                    break;

                case OpenTypeMethodsImplementations.Realistic:
                    impl = new RealisticOpenTypeMethodsImplementation();
                    break;

                case OpenTypeMethodsImplementations.Tolerant:
                    impl = new TolerantOpenTypeMethodsImplementation();
                    break;

                default:
                    return null;
            }

            impl.UseLazyBooleanEvaluation = lazyEvaluation;
            return impl;
        }

        public static MethodInfo GetImplementation(MethodInfo lateBoundMethod)
        {
            if (lateBoundMethod.ReflectedType != typeof(OpenTypeMethods))
                throw new ArgumentException("Given method is not an OpenTypeMethod: " + lateBoundMethod.ToString());

            return typeof(OpenTypeMethodsImplementation).GetMethod(lateBoundMethod.Name, lateBoundMethod.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        protected static T Convert<T>(object thing)
        {
            return (T)System.Convert.ChangeType(thing, typeof(T));
        }

        public IDataServiceQueryProvider QueryProvider
        {
            get;
            set;
        }

        public bool UseLazyBooleanEvaluation
        {
            get;
            set;
        }

        public Expression ConvertMethodCall(MethodInfo lateBoundMethod, params Expression[] parameters)
        {
            return ConvertMethodCall(lateBoundMethod, parameters.AsEnumerable());
        }

        public Expression ConvertMethodCall(MethodInfo lateBoundMethod, IEnumerable<Expression> parameters)
        {
            MethodInfo implementation = GetImplementation(lateBoundMethod);
            if (implementation == null)
                throw new ArgumentNullException("Could not find corresponding method for: " + lateBoundMethod.ToString());

            Expression constant = Expression.Constant(this, typeof(OpenTypeMethodsImplementation));
            return Expression.Call(constant, implementation, parameters);
        }

        public abstract object GetValue(object value, string property);

        public abstract object Add(object left, object right);
        
        public abstract object AndAlso(object left, object right);
        
        public abstract object Ceiling(object value);
        
        public abstract object Concat(object first, object second);
        
        public abstract object Day(object dateTime);
        
        public abstract object Divide(object left, object right);
        
        public abstract object EndsWith(object targetString, object substring);
        
        public abstract object Equal(object left, object right);
        
        public abstract object Floor(object value);
        
        public abstract object GreaterThan(object left, object right);
        
        public abstract object GreaterThanOrEqual(object left, object right);
        
        public abstract object Hour(object dateTime);
        
        public abstract object IndexOf(object targetString, object substring);
        
        public abstract object Length(object value);
        
        public abstract object LessThan(object left, object right);
        
        public abstract object LessThanOrEqual(object left, object right);
        
        public abstract object Minute(object dateTime);
        
        public abstract object Modulo(object left, object right);
        
        public abstract object Month(object dateTime);
        
        public abstract object Multiply(object left, object right);
        
        public abstract object Negate(object value);
        
        public abstract object Not(object value);
        
        public abstract object NotEqual(object left, object right);
        
        public abstract object OrElse(object left, object right);
        
        public abstract object Replace(object targetString, object substring, object newString);
        
        public abstract object Round(object value);
        
        public abstract object Second(object dateTime);
        
        public abstract object StartsWith(object targetString, object substring);
        
        public abstract object Substring(object targetString, object startIndex);
        
        public abstract object Substring(object targetString, object startIndex, object length);
        
        public abstract object Contains(object targetString,object substring);
        
        public abstract object Subtract(object left, object right);
        
        public abstract object ToLower(object targetString);
        
        public abstract object ToUpper(object targetString);
        
        public abstract object Trim(object targetString);

        public abstract object Year(object dateTime);

        public abstract object Convert(object value, DSP.ResourceType type);

        public abstract object TypeIs(object value, DSP.ResourceType type);

        public virtual object Distance(object operand1, object operand2)
        {
            throw new NotImplementedException("Not yet implemented");
        }

        public virtual object Intersects(object operand1, object operand2)
        {
            throw new NotImplementedException("Not yet implemented");
        }

        // the rest are not late-bound methods specific, but help us in re-writing the query tree
        public virtual int Compare(object left, object right)
        {
            if (left == right)
                return 0;

            // null is 'less than' a non-null value
            if (left == null)
            {
                if (right == null)
                    return 0;
                else
                    return -1;
            }

            Type comparerType = typeof(Comparer<>).MakeGenericType(left.GetType());
            PropertyInfo defaultProperty = comparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
            IComparer comparer = (IComparer)defaultProperty.GetValue(null, null);
            return comparer.Compare(left, right);
        }
    }
    #endregion

    // implementation to use by default
    #region default behavior (throws for everything)
    public class DefaultOpenTypeMethodsImplementation : OpenTypeMethodsImplementation
    {
        public override object GetValue(object value, string property)
        {
            throw new NotImplementedException();
        }

        public override object Add(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object AndAlso(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Ceiling(object value)
        {
            throw new NotImplementedException();
        }

        public override object Concat(object first, object second)
        {
            throw new NotImplementedException();
        }

        public override object Day(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object Divide(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object EndsWith(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        public override object Equal(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Floor(object value)
        {
            throw new NotImplementedException();
        }

        public override object GreaterThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object GreaterThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Hour(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object IndexOf(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        public override object Length(object value)
        {
            throw new NotImplementedException();
        }

        public override object LessThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object LessThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Minute(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object Modulo(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Month(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object Multiply(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Negate(object value)
        {
            throw new NotImplementedException();
        }

        public override object Not(object value)
        {
            throw new NotImplementedException();
        }

        public override object NotEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object OrElse(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object Replace(object targetString, object substring, object newString)
        {
            throw new NotImplementedException();
        }

        public override object Round(object value)
        {
            throw new NotImplementedException();
        }

        public override object Second(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object StartsWith(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        public override object Substring(object targetString, object startIndex)
        {
            throw new NotImplementedException();
        }

        public override object Substring(object targetString, object startIndex, object length)
        {
            throw new NotImplementedException();
        }

        public override object Contains(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        public override object Subtract(object left, object right)
        {
            throw new NotImplementedException();
        }

        public override object ToLower(object targetString)
        {
            throw new NotImplementedException();
        }

        public override object ToUpper(object targetString)
        {
            throw new NotImplementedException();
        }

        public override object Trim(object targetString)
        {
            throw new NotImplementedException();
        }

        public override object Year(object dateTime)
        {
            throw new NotImplementedException();
        }

        public override object Convert(object value, DSP.ResourceType type)
        {
            throw new NotImplementedException();
        }

        public override object TypeIs(object value, DSP.ResourceType type)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    // implementation to use when /UseOpenTypes is true
    #region realistic behavior (performs 'reasonable' set of operations, throws the rest of the time)
    public class RealisticOpenTypeMethodsImplementation : OpenTypeMethodsImplementation
    {
        public override object GetValue(object value, string property)
        {
            APICallLog.Current.Push();
            try
            {
                // for sub-values of a complex type inside an open property, GetValue will still be used
                // so we should check before calling GetOpenPropertyValue (which will throw on non-open types)
                DSP.ResourceType type = QueryProvider.GetResourceType(value);
                if(type == null || type.IsOpenType)
                    return QueryProvider.GetOpenPropertyValue(value, property);

                if (type.ResourceTypeKind != ResourceTypeKind.ComplexType)
                    throw new DataServiceException(500, "OpenTypeMethods.GetValue used on non-open, non-complex type");
                
                DSP.ResourceProperty rp = type.Properties.SingleOrDefault(p => p.Name == property);
                if (rp == null)
                    throw new DataServiceException(500, "OpenTypeMethods.GetValue used on non-open type with non-existent property");

                return QueryProvider.GetPropertyValue(value, rp);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #region comparison

        //private int Compare(object left, object right)
        //{
        //    if (left == right)
        //        return 0;

        //    // null always comes first
        //    if (left == null && right != null)
        //        return 1;

        //    if (right == null && left != null)
        //        return -1;

        //    if (left is string || right is string)
        //    {
        //        return left.ToString().CompareTo(right.ToString());
        //    }

        //    if (left.GetType() == right.GetType())
        //    {
        //        if (left is IComparable)
        //            return ((IComparable)left).CompareTo(right);
        //    }

        //    try
        //    {
        //        return ((double)left).CompareTo((double)right);
        //    }
        //    catch
        //    {
        //        throw new DataServiceException(400, String.Format("Cannot compare types {0} and {1}", left.GetType(), right.GetType()));
        //    }
        //}

        public override object GreaterThan(object left, object right)
        {
            try
            {
                return Compare(left, right) > 0;
            }
            catch(ArgumentException)
            {
                throw new DataServiceException(400, String.Format("GreaterThan incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        public override object GreaterThanOrEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) >= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, String.Format("GreaterThanOrEqual incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        public override object LessThan(object left, object right)
        {
            try
            {
                return Compare(left, right) < 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, String.Format("LessThan incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        public override object LessThanOrEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) <= 0;
            }
            catch (ArgumentException)
            {
                throw new DataServiceException(400, String.Format("LessThanOrEqual incompatible with operand types '{0}' and '{1}'.", left.GetType().Name, right.GetType().Name));
            }
        }

        public override object Equal(object left, object right)
        {
            try
            {
                return Compare(left, right) == 0;
            }
            catch
            {
                return false;
            }
        }

        public override object NotEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) != 0;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        public override object Add(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) + Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) + Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) + Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) + Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) + Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) + Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) + Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, String.Format("Cannot add type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public override object AndAlso(object left, object right)
        {
            if (left is bool && right is bool)
                return (bool)left && (bool)right;
            throw new DataServiceException(400, String.Format("AndAlso not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        public override object Ceiling(object value)
        {
            if (value is decimal)
                return Math.Ceiling((decimal)value);
            else if (value is double)
                return Math.Ceiling((double)value);
            else if (value is float)
                return Math.Ceiling((float)value);

            throw new DataServiceException(400, "Cannot use Ceiling with type " + value.GetType());
        }

        public override object Concat(object first, object second)
        {
            if(first is string && second is string)
                return (string)first + (string)second;

            throw new DataServiceException(400, String.Format("Concat not valid between types {0} and {1}", first.GetType(), second.GetType()));
        }

        public override object Day(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Day;
            throw new DataServiceException(400, "Cannot use Day with type " + dateTime.GetType());
        }

        public override object Divide(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) / Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) / Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) / Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) / Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) / Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) / Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) / Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, String.Format("Cannot divide type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public override object EndsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
                return ((string)targetString).EndsWith((string)substring);

            throw new DataServiceException(400, String.Format("EndsWith not valid between types {0} and {1}", targetString.GetType(), substring.GetType()));
            
        }

        public override object Floor(object value)
        {
            if (value is decimal)
                return Math.Floor((decimal)value);
            else if (value is double)
                return Math.Floor((double)value);
            else if (value is float)
                return Math.Floor((float)value);

            throw new DataServiceException(400, "Floor not valid for type " + value.GetType());
        }

        public override object Hour(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Hour;
            throw new DataServiceException(400, "Cannot use Hour with type " + dateTime.GetType());
        }

        public override object IndexOf(object targetString, object substring)
        {
            if(targetString is string && substring is string)
                return ((string)targetString).IndexOf((string)substring);
            throw new DataServiceException(400, String.Format("Cannot use IndexOf with types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        public override object Length(object value)
        {
            if (value is string)
                return ((string)value).Length;
            if (value is Array)
                return ((Array)value).Length;
            throw new DataServiceException(400, "Length not valid on type " + value.GetType());
        }

        public override object Minute(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Minute;
            throw new DataServiceException(400, "Cannot use Minute with type " + dateTime.GetType());
        }

        public override object Modulo(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) % Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) % Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) % Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) % Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) % Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) % Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) % Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, String.Format("Cannot mopdulo type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public override object Month(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Month;
            throw new DataServiceException(400, "Cannot use Month with type {0}" + dateTime.GetType());
        }

        public override object Multiply(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) * Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) * Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) * Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) * Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) * Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) * Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) * Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, String.Format("Cannot multiply type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public override object Negate(object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Int16:
                    return -(short)value;
                case TypeCode.SByte:
                    return -(byte)value;
                case TypeCode.Int32:
                    return -(int)value;
                case TypeCode.Int64:
                    return -(long)value;
            }

            return null;
        }

        public override object Not(object value)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return value != null;
            }
        }

        public override object OrElse(object left, object right)
        {
            if (left is bool && right is bool)
                return (bool)left || (bool)right;
            throw new DataServiceException(400, String.Format("OrElse not valid between types {0} and {1}", left.GetType(), right.GetType()));
        }

        public override object Replace(object targetString, object substring, object newString)
        {
            if(targetString is string && substring is string && newString is string)
                return ((string)targetString).Replace((string)substring, (string)newString);

            throw new DataServiceException(400, String.Format("Cannot use Replace with types {0}, {1}, and {2}", targetString.GetType(), substring.GetType(), newString.GetType()));
        }

        public override object Round(object value)
        {
            if (value is decimal)
                return Math.Round((decimal)value);
            else if(value is double)
                return Math.Round((double)value);
            else if (value is float)
                return Math.Round((float)value);

            throw new DataServiceException("Cannot use Round with type " + value.GetType());
        }

        public override object Second(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Second;
            throw new DataServiceException(400, "Cannot use Second with type {0}" + dateTime.GetType());
        }

        public override object StartsWith(object targetString, object substring)
        {
            if(targetString is string && substring is string)
                return ((string)targetString).StartsWith((string)substring);
            throw new DataServiceException(400, String.Format("Cannot use StartsWith with types {0} and {1}", substring.GetType(), targetString.GetType()));
        }

        public override object Substring(object targetString, object startIndex)
        {
            if (targetString is string && startIndex is int)
            return ((string)targetString).Substring((int)startIndex);
            throw new DataServiceException(400, String.Format("Cannot use Substring with types {0} and {1}", targetString.GetType(), startIndex.GetType()));
        }

        public override object Substring(object targetString, object startIndex, object length)
        {
            if (targetString is string && startIndex is int && length is int)
                return ((string)targetString).Substring((int)startIndex, (int)length);
            throw new DataServiceException(400, String.Format("Cannot use Substring with types {0}, {1} and {2}", targetString.GetType(), startIndex.GetType(), length.GetType()));
        }

        public override object Contains(object targetString, object substring)
        {
            if(substring is string && targetString is string)
                return ((string)targetString).Contains((string)substring);
            throw new DataServiceException(400, String.Format("Cannot use Contains with types {0} and {1}", targetString.GetType(), substring.GetType()));
        }

        public override object Subtract(object left, object right)
        {
            if (left is double || right is double)
            {
                return Convert<double>(left) - Convert<double>(right);
            }
            else if (left is float || right is float)
            {
                return Convert<float>(left) - Convert<float>(right);
            }
            else if (left is decimal || right is decimal)
            {
                return Convert<decimal>(left) - Convert<decimal>(right);
            }
            else if (left is long || right is long)
            {
                return Convert<long>(left) - Convert<long>(right);
            }
            else if (left is int || right is int)
            {
                return Convert<int>(left) - Convert<int>(right);
            }
            else if (left is short || right is short)
            {
                return Convert<short>(left) - Convert<short>(right);
            }
            else if (left is byte || right is byte)
            {
                return Convert<byte>(left) - Convert<byte>(right);
            }
            else
            {
                throw new DataServiceException(400, String.Format("Cannot subtract type {0} to {1}", left.GetType(), right.GetType()));
            }
        }

        public override object ToLower(object targetString)
        {
            if (targetString is string)
                return ((string)targetString).ToLower();
            throw new DataServiceException(400, "Cannot use ToLower with type " + targetString.GetType());
        }

        public override object ToUpper(object targetString)
        {
            if (targetString is string)
                return ((string)targetString).ToLower();
            throw new DataServiceException(400, "Cannot use ToUpper with type " + targetString.GetType());
        }

        public override object Trim(object targetString)
        {
            if(targetString is string)
                return ((string)targetString).Trim();
            throw new DataServiceException(400, "Cannot use Trim with type " + targetString.GetType());
        }

        public override object Year(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Year;
            throw new DataServiceException(400, "Cannot use Year with type " + dateTime.GetType());
        }

        public override object Convert(object value, DSP.ResourceType type)
        {
            if(value == null)
                return null;

            if (type.InstanceType.IsAssignableFrom(value.GetType()))
                return value;

            throw new InvalidCastException("Instance of '" + value.GetType() + "' cannot be converted to resource type '" + type.FullName + "'");
        }

        public override object TypeIs(object value, DSP.ResourceType type)
        {
            APICallLog.Current.Push();
            try
            {
                if (value == null)
                    return false;

                DSP.ResourceType instanceType = QueryProvider.GetResourceType(value);
                if (instanceType != null)
                {
                    IDataServiceMetadataProvider metadataProvider = QueryProvider as IDataServiceMetadataProvider;
                    if (metadataProvider != null && metadataProvider.HasDerivedTypes(type))
                    {
                        if (metadataProvider.GetDerivedTypes(type).Contains(instanceType))
                            return true;
                    }
                    return type == instanceType;
                }

                return type.InstanceType.IsAssignableFrom(value.GetType());
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }
    #endregion

    // implementation to use when testing late bound methods
    #region tolerant behavior (allows everything)
    public class TolerantOpenTypeMethodsImplementation : RealisticOpenTypeMethodsImplementation
    {
        #region comparison

        public override int Compare(object left, object right)
        {
            try
            {
                return base.Compare(left, right);
            }
            catch
            {
                return left.GetType().ToString().CompareTo(right.GetType().ToString());
            }
        }

        public override object GreaterThan(object left, object right)
        {
            return Compare(left, right) > 0;
        }

        public override object GreaterThanOrEqual(object left, object right)
        {
            return Compare(left, right) >= 0;
        }

        public override object LessThan(object left, object right)
        {
            return Compare(left, right) < 0;
        }

        public override object LessThanOrEqual(object left, object right)
        {
            return Compare(left, right) <= 0;
        }

        public override object Equal(object left, object right)
        {
            try
            {
                return Compare(left, right) == 0;
            }
            catch
            {
                return false;
            }
        }

        public override object NotEqual(object left, object right)
        {
            try
            {
                return Compare(left, right) != 0;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        public override object Add(object left, object right)
        {
            try
            {
                return (double)left + (double)right;
            }
            catch
            {
                return left;
            }
        }

        public override object AndAlso(object left, object right)
        {
            if (left is bool && right is bool)
                return (bool)left && (bool)right;
            return false;
        }

        public override object Ceiling(object value)
        {
            if (value is decimal)
                return Math.Ceiling((decimal)value);
            else if (value is double)
                return Math.Ceiling((double)value);
            else if (value is float)
                return Math.Ceiling((float)value);
            return value;
        }

        public override object Concat(object first, object second)
        {
            if (first is string && second is string)
                return (string)first + (string)second;

            return first;
        }

        public override object Day(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Day;
            return dateTime;
        }

        public override object Divide(object left, object right)
        {
            try
            {
                return (double)left / (double)right;
            }
            catch
            {
                return left;
            }
        }

        public override object EndsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
                return ((string)targetString).EndsWith((string)substring);

            return false;
        }

        public override object Floor(object value)
        {
            if (value is decimal)
                return Math.Floor((decimal)value);
            else if (value is double)
                return Math.Floor((double)value);
            else if(value is float)
                return Math.Floor((float)value);

            return value;
        }

        public override object Hour(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Hour;
            return dateTime;
        }

        public override object IndexOf(object targetString, object substring)
        {
            if (targetString is string && substring is string)
                return ((string)targetString).IndexOf((string)substring);
            return -1;
        }

        public override object Length(object value)
        {
            if (value is string)
                return ((string)value).Length;
            if (value is Array)
                return ((Array)value).Length;
            return value;
        }

        public override object Minute(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Minute;
            return dateTime;
        }

        public override object Modulo(object left, object right)
        {
            try
            {
                return (double)left % (double)right;
            }
            catch
            {
                return left;
            }
        }

        public override object Month(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Month;
            return dateTime;
        }

        public override object Multiply(object left, object right)
        {
            try
            {
                return (double)left * (double)right;
            }
            catch
            {
                return left;
            }
        }

        public override object Negate(object value)
        {
            if (value is byte)
                return -(byte)value;
            if(value is short)
                return -(short)value;
            if(value is int)
                return -(int)value;
            if(value is long)
                return -(long)value;
            if (value is decimal)
                return -(decimal)value;
            if (value is float)
                return -(float)value;
            if (value is double)
                return -(double)value;

            return value;
        }

        public override object Not(object value)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            else
            {
                return value != null;
            }
        }

        public override object OrElse(object left, object right)
        {
            if (left is bool && right is bool)
                return (bool)left || (bool)right;

            return false;
        }

        public override object Replace(object targetString, object substring, object newString)
        {
            if (targetString is string && substring is string && newString is string)
                return ((string)targetString).Replace((string)substring, (string)newString);

            return targetString;
        }

        public override object Round(object value)
        {
            if (value is decimal)
                return Math.Round((decimal)value);
            else if (value is double)
                return Math.Round((double)value);
            else if(value is float)
                return Math.Round((float)value);

            return value;
        }

        public override object Second(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Second;
            return dateTime;
        }

        public override object StartsWith(object targetString, object substring)
        {
            if (targetString is string && substring is string)
                return ((string)targetString).StartsWith((string)substring);
            return false;
        }

        public override object Substring(object targetString, object startIndex)
        {
            if (targetString is string && startIndex is int)
            {
                string target = (string)targetString;
                int index = (int)startIndex;
                if (index < 0 || index >= target.Length)
                    return null;
                return target.Substring(index);
            }
            return targetString;
        }

        public override object Substring(object targetString, object startIndex, object length)
        {
            if (targetString is string && startIndex is int && length is int)
            {
                string target = (string)targetString;
                int index = (int)startIndex;
                int l = (int)length;
                if (index < 0 || index >= target.Length)
                    return null;
                if (index + l >= target.Length)
                    return target.Substring(index);
                else
                    return target.Substring(index, l);
            }
            return targetString;
        }

        public override object Contains(object targetString, object substring)
        {
            if (substring is string && targetString is string)
            {
                string target = (string)targetString;
                string sub = (string)substring;
                if (sub.Length > target.Length)
                    return false;
                return target.Contains(sub);
            }
            return false;
        }

        public override object Subtract(object left, object right)
        {
            try
            {
                return (double)left - (double)right;
            }
            catch
            {
                return left;
            }
        }

        public override object ToLower(object targetString)
        {
            if (targetString is string)
                return ((string)targetString).ToLower();
            return targetString;
        }

        public override object ToUpper(object targetString)
        {
            if (targetString is string)
                return ((string)targetString).ToLower();
            return targetString;
        }

        public override object Trim(object targetString)
        {
            if (targetString is string)
                return ((string)targetString).Trim();
            return targetString;
        }

        public override object Year(object dateTime)
        {
            if (dateTime is DateTime)
                return ((DateTime)dateTime).Year;
            return dateTime;
        }

        public override object Convert(object value, DSP.ResourceType type)
        {
            if(value == null)
                return null;

            if (type.InstanceType.IsAssignableFrom(value.GetType()))
                return value;

            // not sure how to make this tolerant without it blowing up elsewhere
            throw new InvalidCastException("Instance of '" + value.GetType() + "' cannot be converted to resource type '" + type.FullName + "'");
        }

        public override object TypeIs(object value, DSP.ResourceType type)
        {
            // not clear how we can make this tolerant without it blowing up
            APICallLog.Current.Push();
            try
            {
                if (value == null)
                    return false;

                DSP.ResourceType instanceType = QueryProvider.GetResourceType(value);
                if (instanceType != null)
                {
                    IDataServiceMetadataProvider metadataProvider = QueryProvider as IDataServiceMetadataProvider;
                    if (metadataProvider != null && metadataProvider.HasDerivedTypes(type))
                    {
                        if (metadataProvider.GetDerivedTypes(type).Contains(instanceType))
                            return true;
                    }
                    return type == instanceType;
                }

                return type.InstanceType.IsAssignableFrom(value.GetType());
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }
    #endregion
}
