//---------------------------------------------------------------------
// <copyright file="Exp.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // Exp
    //
    ////////////////////////////////////////////////////////   
    public static class Exp
    {
        //Static
        public static ConstantExpression Constant(NodeValue value)
        {
            return new ConstantExpression(value);
        }

        public static ConstantExpression Constant(Object value, NodeType type)
        {
            return Exp.Constant(type.Value(value));
        }

        public static PropertyExpression Property(this NodeProperty property)
        {
            return new PropertyExpression(property);
        }

        public static PropertyExpression[] Properties(NodeProperty[] properties)
        {
            return properties.Select(p => Exp.Property(p)).ToArray();
        }

        public static VariableExpression Variable(Node variable)
        {
            return new VariableExpression(variable);
        }

        public static VariableExpression Variable(ResourceContainer container)
        {
            return new VariableExpression(container, container.BaseType);
        }

        public static KeyExpression Key(ResourceContainer container, ResourceType type, params NodeValue[] values)
        {
            return Key(container, type, type.Key.Properties.ToArray(), values);
        }

        public static KeyExpression Key(ResourceContainer container, NodeProperty[] properties, NodeValue[] values)
        {
            ConstantExpression[] constants = values.Select(v => Constant(v)).ToArray();
            PropertyExpression[] propExps = properties.Select(p => Property(p)).ToArray();
            return new KeyExpression(container, propExps, constants);
        }
        public static KeyExpression Key(ResourceContainer container, ResourceType type, NodeProperty[] properties, NodeValue[] values)
        {
            ConstantExpression[] constants = values.Select(v => Constant(v)).ToArray();
            PropertyExpression[] propExps = properties.Select(p => Property(p)).ToArray();
            return new KeyExpression(container, type, propExps, constants);
        }
        public static KeyExpression Key(ResourceContainer container, ResourceType type, int value)
        {
            NodeValue[] values = { new NodeValue(value, Clr.Types.Int32) };
            return Exp.Key(container, type, values);
        }

        public static KeyExpression Key(ResourceContainer container, ResourceType type, string value)
        {
            NodeValue[] values = { new NodeValue(value, Clr.Types.String) };
            return Exp.Key(container, type, values);
        }

        public static KeyExpression Key(ResourceContainer container, ResourceType type, IDictionary<string, object> properties)
        {
            return new KeyExpression(container, type, properties);
        }

        public static NewExpression New(this ExpNode type, params ExpNode[] arguments)
        {
            return new NewExpression(type, arguments);
        }

        //Logical
        public static LogicalExpression And(ExpNode left, ExpNode right)
        {
            return new LogicalExpression(left, right, LogicalOperator.And);
        }

        public static LogicalExpression Or(ExpNode left, ExpNode right)
        {
            return new LogicalExpression(left, right, LogicalOperator.Or);
        }

        public static LogicalExpression Not(ExpNode node)
        {
            return new LogicalExpression(node, null, LogicalOperator.Not);
        }

        //Comparison
        public static ComparisonExpression IsNull(this ExpNode node)
        {
            return Exp.Equal(node, Exp.Constant(node.Type.Null));
        }

        public static ComparisonExpression IsNotNull(this ExpNode node)
        {
            return Exp.NotEqual(node, Exp.Constant(node.Type.Null));
        }

        public static ComparisonExpression Equal(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.Equal);
        }

        public static ComparisonExpression NotEqual(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.NotEqual);
        }

        public static ComparisonExpression GreaterThan(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.GreaterThan);
        }

        public static ComparisonExpression GreaterThanOrEqual(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.GreaterThanOrEqual);
        }

        public static ComparisonExpression LessThan(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.LessThan);
        }

        public static ComparisonExpression LessThanOrEqual(this ExpNode left, ExpNode right)
        {
            return new ComparisonExpression(left, right, ComparisonOperator.LessThanOrEqual);
        }

        // Arithmetic
        public static ArithmeticExpression Add(this ExpNode left, ExpNode right)
        {
            return new ArithmeticExpression(left, right, ArithmeticOperator.Add);
        }

        public static ArithmeticExpression Div(this ExpNode left, ExpNode right)
        {
            return new ArithmeticExpression(left, right, ArithmeticOperator.Div);
        }

        public static ArithmeticExpression Mod(this ExpNode left, ExpNode right)
        {
            return new ArithmeticExpression(left, right, ArithmeticOperator.Mod);
        }

        public static ArithmeticExpression Mult(this ExpNode left, ExpNode right)
        {
            return new ArithmeticExpression(left, right, ArithmeticOperator.Mult);
        }

        public static ArithmeticExpression Sub(this ExpNode left, ExpNode right)
        {
            return new ArithmeticExpression(left, right, ArithmeticOperator.Sub);
        }

        //Methods (for filter)
        public static MethodExpression Contains(this ExpNode searchText, ExpNode container)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Contains");
            return new MethodExpression(container, mi, new ExpNode[] { searchText });
        }

        public static MethodExpression EndsWith(this ExpNode container, ExpNode searchText)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
            return new MethodExpression(container, mi, new ExpNode[] { searchText });
        }

        public static MethodExpression StartsWith(this ExpNode container, ExpNode searchText)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            return new MethodExpression(container, mi, new ExpNode[] { searchText });
        }

        public static MemberExpression Length(this ExpNode container)
        {
            System.Reflection.PropertyInfo mi = typeof(string).GetProperty("Length");
            return new MemberExpression(container, mi, null);
        }

        public static MethodExpression IndexOf(this ExpNode container, ExpNode searchText)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) });
            return new MethodExpression(container, mi, new ExpNode[] { searchText });
        }

        public static MethodExpression Insert(this ExpNode container, ExpNode index, ExpNode insertText)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Insert");
            return new MethodExpression(container, mi, new ExpNode[] { index, insertText });
        }

        public static MethodExpression Remove(this ExpNode container, ExpNode startIndex)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Remove", new Type[] { typeof(Int32) });
            return new MethodExpression(container, mi, new ExpNode[] { startIndex });
        }

        public static MethodExpression Remove(this ExpNode container, ExpNode startIndex, ExpNode count)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Remove", new Type[] { typeof(Int32), typeof(Int32) });
            return new MethodExpression(container, mi, new ExpNode[] { startIndex, count });
        }

        public static MethodExpression Replace(this ExpNode container, ExpNode find, ExpNode replace)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Replace", new Type[] { typeof(string), typeof(string) });
            return new MethodExpression(container, mi, new ExpNode[] { find, replace });
        }

        public static MethodExpression Substring(this ExpNode container, ExpNode position)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Substring", new Type[] { typeof(Int32) });
            return new MethodExpression(container, mi, new ExpNode[] { position });
        }

        public static MethodExpression Substring(this ExpNode container, ExpNode position, ExpNode length)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Substring", new Type[] { typeof(Int32), typeof(Int32) });
            return new MethodExpression(container, mi, new ExpNode[] { position, length });
        }

        public static MethodExpression ToLower(this ExpNode container)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("ToLower", new Type[] { });
            return new MethodExpression(container, mi, null);
        }

        public static MethodExpression ToUpper(this ExpNode container)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("ToUpper", new Type[] { });
            return new MethodExpression(container, mi, null);
        }
        public static MethodExpression Trim(this ExpNode container)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Trim", new Type[] { });
            return new MethodExpression(container, mi, null);
        }

        public static MethodExpression Concat(this ExpNode string1, ExpNode string2)
        {
            System.Reflection.MethodInfo mi = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            return new MethodExpression(null, mi, new ExpNode[] { string1, string2 });
        }

        //DateTime Properties
        public static MemberExpression Day(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Day");
            return new MemberExpression(container, pi, null);
        }

        public static MemberExpression Hour(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Hour");
            return new MemberExpression(container, pi, null);
        }

        public static MemberExpression Minute(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Minute");
            return new MemberExpression(container, pi, null);
        }

        public static MemberExpression Month(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Month");
            return new MemberExpression(container, pi, null);
        }

        public static MemberExpression Second(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Second");
            return new MemberExpression(container, pi, null);
        }

        public static MemberExpression Year(this ExpNode container)
        {
            System.Reflection.PropertyInfo pi = typeof(DateTime).GetProperty("Year");
            return new MemberExpression(container, pi, null);
        }

        // Math methods
        public static MethodExpression Round(this ExpNode arg)
        {
            System.Reflection.MethodInfo methodInfo;

            if (arg.Type is ClrDecimal)
                methodInfo = typeof(Math).GetMethod("Round", new Type[] { typeof(System.Decimal) });
            else
                methodInfo = typeof(Math).GetMethod("Round", new Type[] { typeof(System.Double) });

            return new MethodExpression(null, methodInfo, new ExpNode[] { arg });
        }

        public static MethodExpression Floor(this ExpNode arg)
        {
            System.Reflection.MethodInfo methodInfo;

            if (arg.Type is ClrDecimal)
                methodInfo = typeof(Math).GetMethod("Floor", new Type[] { typeof(System.Decimal) });
            else
                methodInfo = typeof(Math).GetMethod("Floor", new Type[] { typeof(System.Double) });

            return new MethodExpression(null, methodInfo, new ExpNode[] { arg });
        }

        public static MethodExpression Ceiling(this ExpNode arg)
        {
            System.Reflection.MethodInfo methodInfo;

            if (arg.Type is ClrDecimal)
                methodInfo = typeof(Math).GetMethod("Ceiling", new Type[] { typeof(System.Decimal) });
            else
                methodInfo = typeof(Math).GetMethod("Ceiling", new Type[] { typeof(System.Double) });

            return new MethodExpression(null, methodInfo, new ExpNode[] { arg });
        }

        public static NegateExpression Negate(this ExpNode arg)
        {
            return new NegateExpression(arg);
        }

        public static IsOfExpression IsOf(this ExpNode arg, NodeType type)
        {
            return new IsOfExpression(arg, type);
        }

        public static CastExpression Cast(this ExpNode arg, NodeType type)
        {
            return new CastExpression(arg, type);
        }

    }
}
