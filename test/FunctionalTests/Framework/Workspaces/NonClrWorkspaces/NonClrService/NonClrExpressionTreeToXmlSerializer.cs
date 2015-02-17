//---------------------------------------------------------------------
// <copyright file="NonClrExpressionTreeToXmlSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.NonClr
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
    using System.Data.Test.Astoria.LateBound;

    /// <summary>
    /// ExpressionTreeToXmlSerializer
    /// </summary>
    internal class ExpressionTreeToXmlSerializer : System.Data.Test.Astoria.LateBound.ExpressionVisitor
    {
        #region Xml Constants

        private const string XmlExpressionElement = "Expression";
        private const string XmlExpressionTypeElement = "Type";
        private const string XmlExpressionClrTypeElement = "ClrType";
        private const string XmlExpressionNull = "<null/>";
        private const string XmlExpressionConversion = "Conversion";
        private const string XmlExpressionValue = "Value";
        private const string XmlConditionalTest = "Test";
        private const string XmlConditionalIfTrue = "True";
        private const string XmlConditionalIfFalse = "False";
        private const string XmlMethodCallInfo = "Method";
        private const string XmlMethodCallName = "Name";
        private const string XmlMethodCallType = "ReflectedType";
        private const string XmlMethodCallObject = "Object";
        private const string XmlMethodCallArguments = "Arguments";
        private const string XmlMemberAccessExpression = "MemberExpression";
        private const string XmlMemberInitExpression = "MemberInitExpression";
        private const string XmlNewExpression = "New";
        private const string XmlConstructorInfo = "Constructor";
        private const string XmlBindingsElement = "Bindings";
        private const string XmlBindingElement = "Binding";
        private const string XmlBindingType = "BindingType";
        private const string XmlMemberAccessInfo = "Member";
        private const string XmlLambdaBody = "Body";
        private const string XmlLambdaParameter = "Parameter";
        private const string XmlLambdaParameterName = "name";
        private const string XmlUnaryOperand = "Operand";
        private const string XmlBinaryLeft = "Left";
        private const string XmlBinaryRight = "Right";
        private const string XmlTypeIs = "TypeIs";
        private const string XmlTypeName = "TypeName";

        #endregion

        private static StringBuilder outputText;
        private static XmlWriter writer;

        /// <summary>
        /// Serialize the expression into Xml format
        /// </summary>
        /// <param name="exp">The Expression</param>
        /// <returns>The serialized Xml in String Format</returns>
        public static String Serialize(Expression exp)
        {
            outputText = new StringBuilder();
            writer = XmlTextWriter.Create(outputText);
            new ExpressionTreeToXmlSerializer().Visit(exp);
            writer.Flush();
            return outputText.ToString();
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
                writer.WriteStartElement(XmlExpressionElement);
                writer.WriteElementString(XmlExpressionTypeElement, exp.NodeType.ToString());
                writer.WriteElementString(XmlExpressionClrTypeElement, exp.Type.AssemblyQualifiedName);
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
            writer.WriteStartElement(XmlLambdaParameter);
            writer.WriteAttributeString(XmlLambdaParameterName, p.Name);
            writer.WriteEndElement();
            return p;
        }

        /// <summary>
        /// UnaryExpression visit method
        /// </summary>
        /// <param name="u">The UnaryExpression expression to visit</param>
        /// <returns>The visited UnaryExpression expression </returns>
        internal override Expression VisitUnary(UnaryExpression u)
        {
            // <Method>...</Method>
            WriteMethodInfo(u.Method);

            // <Operand>...</Operand>
            writer.WriteStartElement(XmlUnaryOperand);
            this.Visit(u.Operand);
            writer.WriteEndElement();

            return u;
        }

        /// <summary>
        /// BinaryExpression visit method
        /// </summary>
        /// <param name="b">The BinaryExpression expression to visit</param>
        /// <returns>The visited BinaryExpression expression </returns>
        internal override Expression VisitBinary(BinaryExpression b)
        {
            // <Method>...</Method>
            WriteMethodInfo(b.Method);

            // <Conversion>...</Conversion>
            writer.WriteStartElement(XmlExpressionConversion);
            this.Visit(b.Conversion);
            writer.WriteEndElement();

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
            // <Value>...</Value>
            if( c.Value == null )
                writer.WriteElementString(XmlExpressionValue, "null");
            else
                writer.WriteElementString(XmlExpressionValue, c.Value.ToString());

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
            // <Body>...</Body>
            writer.WriteStartElement(XmlLambdaBody);
            this.Visit(lambda.Body);
            writer.WriteEndElement();

            // <Parameter name="..." />
            // [...]
            foreach (ParameterExpression p in lambda.Parameters)
            {
                this.VisitParameter(p);
            }

            return lambda;
        }

        #region Not Implemented Expressions

        /// <summary>
        /// MemberBinding visit method
        /// </summary>
        /// <param name="binding">The MemberBinding expression to visit</param>
        /// <returns>The visited MemberBinding expression </returns>
        internal override MemberBinding VisitBinding(MemberBinding binding)
        {
            writer.WriteStartElement(XmlBindingElement);
            writer.WriteElementString(XmlBindingType, binding.BindingType.ToString());

            MemberBinding returnValue = null;

            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    returnValue = this.VisitMemberAssignment((MemberAssignment)binding);
                    break;
                case MemberBindingType.MemberBinding:
                    returnValue = this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                    break;
                case MemberBindingType.ListBinding:
                    returnValue = this.VisitMemberListBinding((MemberListBinding)binding);
                    break;
            }

            writer.WriteEndElement();
            return returnValue;
        }

        /// <summary>
        /// Binding List visit method
        /// </summary>
        /// <param name="original">The Binding list to visit</param>
        /// <returns>The visited Binding list</returns>
        internal override IEnumerable<MemberBinding> VisitBindingList(System.Collections.ObjectModel.ReadOnlyCollection<MemberBinding> original)
        {
            writer.WriteStartElement(XmlBindingsElement);

            foreach (MemberBinding mb in original)
            {
                this.VisitBinding(mb);
            }

            writer.WriteEndElement();
            return original;
        }

        /// <summary>
        /// ElementInit visit method
        /// </summary>
        /// <param name="initializer">The ElementInit expression to visit</param>
        /// <returns>The visited ElementInit expression </returns>
        internal override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            this.VisitExpressionList(initializer.Arguments);
            return initializer;
        }

        /// <summary>
        /// ElementInit expression list visit method
        /// </summary>
        /// <param name="original">The ElementInit expression list  to visit</param>
        /// <returns>The visited ElementInit expression list </returns>
        internal override IEnumerable<ElementInit> VisitElementInitializerList(System.Collections.ObjectModel.ReadOnlyCollection<ElementInit> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitElementInitializer(original[i]);
            }

            return original;
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
        /// MemberAssignment visit method
        /// </summary>
        /// <param name="assignment">The MemberAssignment to visit</param>
        /// <returns>The visited MemberAssignmentt</returns>
        internal override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            this.Visit(assignment.Expression);
            return assignment;
        }

        /// <summary>
        /// MemberInitExpression visit method
        /// </summary>
        /// <param name="init">The MemberInitExpression to visit</param>
        /// <returns>The visited MemberInitExpression</returns>
        internal override Expression VisitMemberInit(MemberInitExpression init)
        {
            if (init != null)
            {
                // <MemberInit>
                //  <New>...</New>
                //  <Bindings>...</Bindings>
                // </MemberInit>
                writer.WriteStartElement(XmlMemberInitExpression);
                this.VisitNew(init.NewExpression);
                this.VisitBindingList(init.Bindings);
                writer.WriteEndElement();
            }
            else
            {
                WriteNullTag();
            }

            return init;
        }

        /// <summary>
        /// MemberListBinding visit method
        /// </summary>
        /// <param name="binding">The MemberListBinding to visit</param>
        /// <returns>The visited MemberListBinding</returns>
        internal override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            this.VisitElementInitializerList(binding.Initializers);
            return binding; 
        }

        /// <summary>
        /// MemberMemberBinding visit method
        /// </summary>
        /// <param name="binding">The MemberMemberBinding to visit</param>
        /// <returns>The visited MemberMemberBinding</returns>
        internal override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            this.VisitBindingList(binding.Bindings);
            return binding;
        }

        /// <summary>
        /// NewExpression visit method
        /// </summary>
        /// <param name="nex">The NewExpression to visit</param>
        /// <returns>The visited NewExpression</returns>
        internal override NewExpression VisitNew(NewExpression nex)
        {
            writer.WriteStartElement(XmlNewExpression);
            writer.WriteElementString(XmlConstructorInfo, nex.Constructor.ToString());
            writer.WriteEndElement();

            return nex;
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

        /// <summary>
        /// TypeBinaryExpression visit method
        /// </summary>
        /// <param name="b">The TypeBinaryExpression expression to visit</param>
        /// <returns>The visited TypeBinaryExpression expression </returns>
        internal override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            writer.WriteStartElement(XmlTypeIs);
            writer.WriteElementString(XmlTypeName, b.TypeOperand.ToString());
            writer.WriteEndElement();
            return b;
        }

        #endregion

        /// <summary>
        /// Serialize a MethodInfo
        /// </summary>
        /// <param name="mi">The method to serialize</param>
        private static void WriteMethodInfo(MethodInfo mi)
        {
            if (mi != null)
            {
                // <Method>
                //  <Name>...</Name>
                //  <ReflectedType>...</ReflectedType>
                // </Method>
                writer.WriteStartElement(XmlMethodCallInfo);
                writer.WriteElementString(XmlMethodCallName, mi.Name);
                writer.WriteElementString(XmlMethodCallType, mi.ReflectedType.AssemblyQualifiedName);
                writer.WriteEndElement();
            }
            else
            {
                WriteNullTag();
            }
        }

        /// <summary>
        /// Serialize a MemberInfo
        /// </summary>
        /// <param name="mi">The member to serialize</param>
        private static void WriteMemberInfo(MemberInfo mi)
        {
            if (mi != null)
            {
                // <Member>
                //  <Name>...</Name>
                //  <ReflectedType>...</ReflectedType>
                // </Member>
                writer.WriteStartElement(XmlMemberAccessInfo);
                writer.WriteElementString(XmlMethodCallName, mi.Name);
                writer.WriteElementString(XmlMethodCallType, mi.ReflectedType.AssemblyQualifiedName);
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
            writer.WriteRaw(XmlExpressionNull);
        }
    }
}
