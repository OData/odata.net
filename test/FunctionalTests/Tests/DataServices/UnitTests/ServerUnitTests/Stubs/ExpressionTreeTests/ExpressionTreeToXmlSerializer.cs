﻿//---------------------------------------------------------------------
// <copyright file="ExpressionTreeToXmlSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;
    using System.Xml;
    using System.Text;

    /// <summary>
    /// ExpressionTreeToXmlSerializer
    /// </summary>
    internal class ExpressionTreeToXmlSerializer : ExpressionVisitor
    {
        /// <summary>
        /// Set this to true if the XML should contain fully qualified type names
        /// </summary>
        public static bool UseFullyQualifiedTypeNames = false;

        #region Xml Constants

        private const string XmlExpression = "Expression";
        private const string XmlExpressionClrType = "type";
        private const string XmlExpressionNull = "null";
        private const string XmlExpressionConversion = "Conversion";
        private const string XmlConditionalTest = "Test";
        private const string XmlConditionalIfTrue = "True";
        private const string XmlConditionalIfFalse = "False";
        private const string XmlMethodCallInfo = "Method";
        private const string XmlMethodCallType = "type";
        private const string XmlMethodCallObject = "Object";
        private const string XmlMethodCallArguments = "Arguments";
        private const string XmlMemberAccessExpression = "MemberExpression";
        private const string XmlMemberAccessInfo = "Member";
        private const string XmlLambdaBody = "Body";
        private const string XmlUnaryOperand = "Operand";
        private const string XmlBinaryLeft = "Left";
        private const string XmlBinaryRight = "Right";
        private const string XmlMemberAssignment = "MemberAssignment";
        private const string XmlMemberName = "member";
        private const string XmlIsTypeOperand = "TypeOperand";

        #endregion

        private static StringBuilder outputText;
        private static XmlWriter writer;

        /// <summary>
        /// Serialize the <paramref name="expression"/> into a string in XML format
        /// </summary>
        /// <param name="expression">The Expression Tree</param>
        /// <returns>The serialized Xml in String Format</returns>
        public static String SerializeToString(Expression expression)
        {
            outputText = new StringBuilder();
            writer = XmlWriter.Create(outputText);

            new ExpressionTreeToXmlSerializer().Visit(expression);

            writer.Flush();
            return outputText.ToString();
        }

        /// <summary>
        /// Serialize the <paramref name="expression"/> into an XML document
        /// </summary>
        /// <param name="expression">The Expression Tree</param>
        /// <returns>An XML document representing the expression tree</returns>
        public static XmlDocument SerializeToXml(Expression expression)
        {
            XmlDocument xdoc = new XmlDocument();
            string xml = SerializeToString(expression);
            xdoc.LoadXml(xml);
            return xdoc;
        }

        /// <summary>
        /// Visit an expression and serialize to xml
        /// </summary>
        /// <param name="exp">The expression to visit</param>
        /// <returns>The visited expression </returns>
        internal override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                WriteNullTag();
            }
            else
            {
                writer.WriteStartElement(exp.NodeType.ToString());
                writer.WriteAttributeString(XmlExpressionClrType, GetTypeDescription(exp.Type));
                base.Visit(exp);
                writer.WriteEndElement();
            }

            return exp;
        }

        /// <summary>
        /// ParameterExpression visit method
        /// </summary>
        /// <param name="p">The ParameterExpression expression to visit</param>
        /// <returns>The visited ParameterExpression expression </returns>
        internal override Expression VisitParameter(ParameterExpression p)
        {
            writer.WriteString(p.Name);
            return p;
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="u">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    this.Visit(u.Operand);
                    break;
                default:
                    // <Method>...</Method>
                    WriteMethodInfo(u.Method);

                    // <Operand>...</Operand>
                    writer.WriteStartElement(XmlUnaryOperand);
                    this.Visit(u.Operand);
                    writer.WriteEndElement();
                    break;
            }

            return u;
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal override Expression VisitBinary(BinaryExpression b)
        {
            if (b.Method != null)
            {
                // <Method>...</Method>
                WriteMethodInfo(b.Method);
            }

            if (b.Conversion != null)
            {
                // <Conversion>...</Conversion>
                writer.WriteStartElement(XmlExpressionConversion);
                this.Visit(b.Conversion);
                writer.WriteEndElement();
            }

            // <Left>...</Left>
            writer.WriteStartElement(XmlBinaryLeft);
            this.Visit(b.Left);
            writer.WriteEndElement();

            // <Right>...</Right>
            writer.WriteStartElement(XmlBinaryRight);
            this.Visit(b.Right);
            writer.WriteEndElement();

            return b;
        }

        /// <summary>
        /// ConditionalExpression visit method
        /// </summary>
        /// <param name="c">The ConditionalExpression expression to visit</param>
        /// <returns>The visited ConditionalExpression expression </returns>
        internal override Expression VisitConditional(ConditionalExpression c)
        {
            // <Test>...</Test>
            writer.WriteStartElement(XmlConditionalTest);
            this.Visit(c.Test);
            writer.WriteEndElement();

            // <True>...</True>
            writer.WriteStartElement(XmlConditionalIfTrue);
            this.Visit(c.IfTrue);
            writer.WriteEndElement();

            // <False>...</False>
            writer.WriteStartElement(XmlConditionalIfFalse);
            this.Visit(c.IfFalse);
            writer.WriteEndElement();

            return c;
        }

        /// <summary>
        /// ConstantExpression visit method
        /// </summary>
        /// <param name="c">The ConstantExpression expression to visit</param>
        /// <returns>The visited ConstantExpression expression </returns>
        internal override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                writer.WriteString("null");
            }
            else
            {
                if (c.Type == typeof(Type))
                {
                    writer.WriteString(GetTypeDescription(c.Value as Type));
                }
                else if (c.Type == typeof(ResourceProperty))
                {
                    writer.WriteString((c.Value as ResourceProperty).Name);
                }
                else if (c.Type == typeof(ResourceType))
                {
                    writer.WriteString((c.Value as ResourceType).FullName);
                }
                else
                {
                    writer.WriteString(c.Value.ToString());
                }
            }

            return c;
        }

        /// <summary>
        /// MethodCallExpression visit method
        /// </summary>
        /// <param name="m">The MethodCallExpression expression to visit</param>
        /// <returns>The visited MethodCallExpression expression </returns>
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            // <Method>...</Method>
            WriteMethodInfo(m.Method);

            // <Object>...</Object>
            writer.WriteStartElement(XmlMethodCallObject);
            this.Visit(m.Object);
            writer.WriteEndElement();

            // <Arguments>...</Arguments>
            writer.WriteStartElement(XmlMethodCallArguments);
            this.VisitExpressionList(m.Arguments);
            writer.WriteEndElement();

            return m;
        }

        /// <summary>
        /// MemberExpression visit method
        /// </summary>
        /// <param name="m">The MemberExpression expression to visit</param>
        /// <returns>The visited MemberExpression expression </returns>
        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            // <Member>...</Member>
            WriteMemberInfo(m.Member);

            // <MemberExpression>...</MemberExpression>
            writer.WriteStartElement(XmlMemberAccessExpression);
            this.Visit(m.Expression);
            writer.WriteEndElement();

            return m;
        }

        /// <summary>
        /// LambdaExpression visit method
        /// </summary>
        /// <param name="lambda">The LambdaExpression to visit</param>
        /// <returns>The visited LambdaExpression</returns>
        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            // <Parameter name="..." />
            // [...]
            foreach (ParameterExpression p in lambda.Parameters)
            {
                this.Visit(p);
            }

            // <Body>...</Body>
            writer.WriteStartElement(XmlLambdaBody);
            this.Visit(lambda.Body);
            writer.WriteEndElement();

            return lambda;
        }

        /// <summary>
        /// Binding List visit method
        /// </summary>
        /// <param name="original">The Binding list to visit</param>
        /// <returns>The visited Binding list</returns>
        internal override IEnumerable<MemberBinding> VisitBindingList(System.Collections.ObjectModel.ReadOnlyCollection<MemberBinding> original)
        {
            foreach (var binding in original)
            {
                this.VisitBinding(binding);
            }
            return original;
        }

        /// <summary>
        /// MemberAssignment visit method
        /// </summary>
        /// <param name="assignment">The MemberAssignment to visit</param>
        /// <returns>The visited MemberAssignmentt</returns>
        internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            // <MemberBinding member="...">...</MemberBinding>
            writer.WriteStartElement(XmlMemberAssignment);
            writer.WriteAttributeString(XmlMemberName, assignment.Member.Name);
            this.Visit(assignment.Expression);
            writer.WriteEndElement();
            return assignment;
        }

        /// <summary>
        /// NewExpression visit method
        /// </summary>
        /// <param name="nex">The NewExpression to visit</param>
        /// <returns>The visited NewExpression</returns>
        internal override NewExpression VisitNew(NewExpression nex)
        {
            if (nex.Arguments.Count > 0)
            {
                // <Arguments>...</Arguments>
                writer.WriteStartElement(XmlMethodCallArguments);
                this.VisitExpressionList(nex.Arguments);
                writer.WriteEndElement();
            }
            return nex;
        }

        /// <summary>
        /// MemberInitExpression visit method
        /// </summary>
        /// <param name="init">The MemberInitExpression to visit</param>
        /// <returns>The visited MemberInitExpression</returns>
        internal override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.VisitNew(init.NewExpression);
            this.VisitBindingList(init.Bindings);
            return init;
        }

        /// <summary>
        /// TypeBinaryExpression visit method
        /// </summary>
        /// <param name="b">The TypeBinaryExpression expression to visit</param>
        /// <returns>The visited TypeBinaryExpression expression </returns>
        internal override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            writer.WriteStartElement(XmlIsTypeOperand);
            writer.WriteString(GetTypeDescription(b.TypeOperand));
            writer.WriteEndElement();
            writer.WriteStartElement(XmlExpression);
            this.Visit(b.Expression);
            writer.WriteEndElement();
            return b;
        }

        #region Not Implemented Expressions

        /// <summary>
        /// ElementInit visit method
        /// </summary>
        /// <param name="initializer">The ElementInit expression to visit</param>
        /// <returns>The visited ElementInit expression </returns>
        internal override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ElementInit expression list visit method
        /// </summary>
        /// <param name="original">The ElementInit expression list  to visit</param>
        /// <returns>The visited ElementInit expression list </returns>
        internal override IEnumerable<ElementInit> VisitElementInitializerList(System.Collections.ObjectModel.ReadOnlyCollection<ElementInit> original)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// InvocationExpression visit method
        /// </summary>
        /// <param name="iv">The InvocationExpression to visit</param>
        /// <returns>The visited InvocationExpression</returns>
        internal override Expression VisitInvocation(InvocationExpression iv)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ListInitExpression visit method
        /// </summary>
        /// <param name="init">The ListInitExpression to visit</param>
        /// <returns>The visited ListInitExpression</returns>
        internal override Expression VisitListInit(ListInitExpression init)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// MemberListBinding visit method
        /// </summary>
        /// <param name="binding">The MemberListBinding to visit</param>
        /// <returns>The visited MemberListBinding</returns>
        internal override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// MemberMemberBinding visit method
        /// </summary>
        /// <param name="binding">The MemberMemberBinding to visit</param>
        /// <returns>The visited MemberMemberBinding</returns>
        internal override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NewArrayExpression visit method
        /// </summary>
        /// <param name="na">The NewArrayExpression to visit</param>
        /// <returns>The visited NewArrayExpression</returns>
        internal override Expression VisitNewArray(NewArrayExpression na)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Serialize a MethodInfo
        /// </summary>
        /// <param name="mi">The method to serialize</param>
        private static void WriteMethodInfo(MethodInfo mi)
        {
            // <Method name="..." type="..." />
            writer.WriteStartElement(XmlMethodCallInfo);
            if (mi != null)
            {
                writer.WriteAttributeString(XmlMethodCallType, GetTypeDescription(mi.ReflectedType));
                writer.WriteString(mi.Name);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize a MemberInfo
        /// </summary>
        /// <param name="mi">The member to serialize</param>
        private static void WriteMemberInfo(MemberInfo mi)
        {
            if (mi != null)
            {
                // <Member name="..." type="..." />
                writer.WriteStartElement(XmlMemberAccessInfo);
                writer.WriteAttributeString(XmlMethodCallType, GetTypeDescription(mi.ReflectedType));
                writer.WriteString(mi.Name);
                writer.WriteEndElement();
            }
            else
            {
                WriteNullTag();
            }
        }

        /// <summary>
        /// Write a tag representing null
        /// </summary>
        private static void WriteNullTag()
        {
            writer.WriteStartElement(XmlExpressionNull);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Returns a textual description of a CLR type.
        /// </summary>
        /// <param name="type">The type to describe</param>
        /// <returns>The description of the type</returns>
        public static string GetTypeDescription(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                string name;
                if (UseFullyQualifiedTypeNames)
                {
                    name = type.GetGenericTypeDefinition().FullName;
                }
                else
                {
                    name = type.GetGenericTypeDefinition().Name;
                }
                int i = name.IndexOf('`');
                if (i > 0)
                {
                    name = name.Substring(0, i);
                }
                sb.Append(name);
                sb.Append("[");
                bool first = true;
                foreach (Type genericArgument in type.GetGenericArguments())
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    first = false;
                    sb.Append(GetTypeDescription(genericArgument));
                }
                sb.Append("]");
                return sb.ToString();
            }
            else
            {
                if (UseFullyQualifiedTypeNames)
                {
                    return type.FullName;
                }
                else
                {
                    return type.Name;
                }
            }
        }
    }
}
