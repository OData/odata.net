//---------------------------------------------------------------------
// <copyright file="UriQueryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Xml;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using AstoriaUnitTests.Data;
    using System.Collections.Generic;

    //TODO: Needs to implement a Query Builder and be filled out
    public class UriQueryBuilder : QueryBuilder
    {
        private string _rootHttpAddress;

        //Constructor
        public UriQueryBuilder(Workspace workspace, string rootHttpAddress)
            : base(workspace)
        {
            _rootHttpAddress = rootHttpAddress;
            this.UseBinaryFormatForDates = true;
            this.CleanUpSpecialCharacters = true;
            this.EscapeUriValues = false;
            this.UseSmallCasing = null;
        }

        public Workspace Workspace
        {
            [DebuggerStepThrough]
            get { return base._workspace; }
        }

        public override void Execute(params ExpNode[] expressions)
        {
            foreach (ExpNode t in expressions)
            {
                //Build query
                String query = this.Build(t);
            }
        }

        public override string Build(ExpNode node)
        {
            string serviceName;
            string queryUri = base.Build(node);
            return this.Workspace.ServiceUri + "/" + queryUri;
        }

        public bool UseBinaryFormatForDates
        {
            get;
            set;
        }

        public bool CleanUpSpecialCharacters
        {
            get;
            set;
        }

        public bool EscapeUriValues
        {
            get;
            set;
        }

        public bool? UseSmallCasing
        {
            get;
            set;
        }

        protected override String Visit(ExpNode caller, ExpNode node)
        {

            if (node is ProjectExpression)
            {
                ProjectExpression e = (ProjectExpression)node;
                if (e.Projections.Count == 0)
                {
                    return String.Format("{0}",
                            this.Visit(e, e.Input)
                            );
                }
                else if (e.Projections.Count == 1)
                {
                    if (e.Projections[0] is PropertyExpression)
                    {
                        return String.Format("{0}/{1}",
                            this.Visit(e, e.Input),
                            this.Visit(e, e.Projections[0])
                            );
                    }
                }
                else
                    throw new Exception("More than 1 projection, invalid");

            }
            else if (node is ServiceOperationExpression)
            {
                AstoriaTestLog.WriteLine("Calling Service Operation");
                ServiceOperationExpression e = (ServiceOperationExpression)node;

                string serviceOp = this.Visit(e, e.Input);
                AstoriaTestLog.WriteLine(serviceOp);
                StringBuilder sbParametersString = new StringBuilder();
                if (!serviceOp.Contains("?"))
                {
                    sbParametersString.Append("?");
                }
                for (int index = 0; index < e.Arguments.Length; index++)
                {
                    ServiceOperationParameterExpression parameter = e.Arguments[index];
                    if (index > 0)
                    {
                        sbParametersString.Append("&");
                    }
                    string strLiteralString = CreateKeyStringValue(parameter.ParameterValue);
                    sbParametersString.AppendFormat("{0}={1}", parameter.ParameterName, strLiteralString);
                }
                return String.Format("{0}{1}", serviceOp, sbParametersString.ToString());
            }
            else if (node is ScanExpression)
            {
                ScanExpression e = (ScanExpression)node;

                if (e.Input is VariableExpression && ((VariableExpression)e.Input).Variable is ResourceContainer)
                    return String.Format("{0}",
                            this.Visit(e, e.Input)
                            );
                throw new Exception("Unsupported on in scan expression");

            }
            else if (node is PredicateExpression)
            {
                PredicateExpression e = (PredicateExpression)node;
                KeyExpression key = e.Predicate as KeyExpression;
                if (key != null)
                {
                    return String.Format("{0}({1})",
                        this.Visit(e, e.Input),
                        this.Visit(e, e.Predicate)
                        );
                }
                else
                    return String.Format("{0}?$filter={1}",
                        this.Visit(e, e.Input),
                        this.Visit(e, e.Predicate)
                        );
            }
            else if (node is CountExpression)
            {
                CountExpression e = (CountExpression)node;
                string sCount = "";
                string visit = "";

                if (!e.IsInline)
                {
                    visit = this.Visit(e, e.Input);
                    sCount = string.Format("{0}/$count", visit);
                }
                else
                {
                    visit = this.Visit(e, e.Input);

                    if (visit.IndexOf("?$") == -1)
                        sCount = string.Format("{0}?$inlinecount={1}", visit, e.CountKind);
                    else
                        sCount = string.Format("{0}&$inlinecount={1}", visit, e.CountKind);
                }
                return sCount;
            }
            else if (node is NewExpression)
            {
                NewExpression e = (NewExpression)node;
                string uri = this.Visit(e, e.Input);
                ExpNode[] pe = new ExpNode[] { };

                List<ExpNode> nodes = new List<ExpNode>();
                foreach (ExpNode parameter in e.Arguments)
                {
                    nodes.Add(parameter as ExpNode);
                }
                pe = nodes.ToArray();
                string _select = String.Format("{0}", this.Visit(e, pe[0]));

                if (nodes.Count > 1)
                {
                    for (int j = 1; j < nodes.Count; j++)
                    {
                        _select = String.Format("{0},{1}", _select, this.Visit(e, pe[j]));
                    }
                }

                if (uri.IndexOf("?$") == -1)
                    return string.Format("{0}?$select={1}", uri, _select);
                else
                    return string.Format("{0}&$select={1}", uri, _select);

            }
            else if (node is MemberBindExpression)
            {
                return this.Visit(node, ((MemberBindExpression)node).SourceProperty);
            }
            else if (node is TopExpression)
            {
                TopExpression e = (TopExpression)node;
                string visit = this.Visit(e, e.Input);

                if (visit.IndexOf("?$") == -1)
                    return string.Format("{0}?$top={1}", visit, e.Predicate);
                else
                    return string.Format("{0}&$top={1}", visit, e.Predicate);
            }
            else if (node is SkipExpression)
            {
                SkipExpression e = (SkipExpression)node;
                string visit = this.Visit(e, e.Input);

                if (visit.IndexOf("?$") == -1)
                    return string.Format("{0}?$skip={1}", visit, e.Predicate);
                else
                    return string.Format("{0}&$skip={1}", visit, e.Predicate);
            }
            else if (node is OrderByExpression)
            {
                OrderByExpression e = (OrderByExpression)node;
                string ordervalue = String.Empty;
                int i = 0;

                string visit = this.Visit(e, e.Input);
                if (e.ExcludeFromUri) { return visit; }

                string propVisit = this.Visit(e, e.PropertiesExp[0]);

                switch (e.AscDesc)
                {
                    case true:
                        if (visit.IndexOf("?$") == -1)
                            ordervalue = String.Format("{0}?$orderby={1}", visit, propVisit);
                        else
                            ordervalue = String.Format("{0}&$orderby={1}", visit, propVisit);
                        break;
                    case false:
                        if (visit.IndexOf("?$") == -1)
                            ordervalue = String.Format("{0}?$orderby={1} desc", visit, propVisit);
                        else
                            ordervalue = String.Format("{0}&$orderby={1} desc", visit, propVisit);
                        break;
                };

                if (e.PropertiesExp.Length > 0)
                {
                    for (i++; i < e.PropertiesExp.Length; i++)
                    {
                        String nextValue = this.Visit(e, e.PropertiesExp[i]);
                        nextValue = String.Format("{0}", nextValue);

                        switch (e.AscDesc)
                        {
                            case true:
                                ordervalue = String.Format("{0},{1}", ordervalue, nextValue);
                                break;
                            case false:
                                ordervalue = String.Format("{0},{1} desc", ordervalue, nextValue);
                                break;
                        }
                    }
                }
                return ordervalue;
            }
            else if (node is ThenByExpression)
            {
                ThenByExpression e = (ThenByExpression)node;
                string ordervalue = String.Empty;
                int i = 0;

                string visit = this.Visit(e, e.Input);
                if (e.ExcludeFromUri) { return visit; }

                switch (e.AscDesc)
                {
                    case true:
                        ordervalue = String.Format("{0},{1}", visit, e.Properties[0].Name);
                        break;
                    case false:
                        ordervalue = String.Format("{0},{1} desc", visit, e.Properties[0].Name);
                        break;
                }

                if (e.Properties.Length > 0)
                {
                    for (i++; i < e.Properties.Length; i++)
                    {
                        String nextValue = e.Properties[i].Name;
                        nextValue = String.Format("{0}", nextValue);

                        switch (e.AscDesc)
                        {
                            case true:
                                ordervalue = String.Format("{0},{1}", ordervalue, nextValue);
                                break;
                            case false:
                                ordervalue = String.Format("{0},{1} desc", ordervalue, nextValue);
                                break;
                        }
                    }
                }
                return ordervalue;
            }
            else if (node is ExpandExpression)
            {
                ExpandExpression e = (ExpandExpression)node;
                string uri = this.Visit(e, e.Input);

                string expand = String.Format("{0}", this.Visit(e, e.PropertiesExp[0]));

                if (e.Properties.Length > 1)
                    for (int i = 1; i < e.Properties.Length; i++)
                        expand = String.Format("{0},{1}", expand, this.Visit(e, e.PropertiesExp[i]));

                if (uri.IndexOf("?$") == -1)
                    return string.Format("{0}?$expand={1}", uri, expand);
                else
                    return string.Format("{0}&$expand={1}", uri, expand);

            }
            else if (node is NavigationExpression)
            {
                NavigationExpression e = (NavigationExpression)node;
                if (!e.IsLink)
                    return String.Format("{0}/{1}", this.Visit(e, e.Input), this.Visit(e, e.PropertyExp));
                else
                    return String.Format("{0}/{1}/$ref", this.Visit(e, e.Input), this.Visit(e, e.PropertyExp));
            }
            else if (node is KeyExpression)
            {
                KeyExpression key = node as KeyExpression;
                return CreateKeyString(key);
            }
            else if (node is NestedPropertyExpression)
            {
                NestedPropertyExpression e = (NestedPropertyExpression)node;
                string enitySetname = e.Name;
                string nestedProperty = "";

                foreach (PropertyExpression p in e.PropertyExpressions)
                {
                    string interim = this.Visit(e, p);
                    //AstoriaTestLog.WriteLine(interim); 
                    if (p.ValueOnly)
                        nestedProperty = String.Format("{0}/{1}/{2},", nestedProperty, enitySetname, interim + "/$value").TrimStart('/');
                    else
                        nestedProperty = String.Format("{0}/{1}/{2},", nestedProperty, enitySetname, interim).TrimStart('/');

                }

                return nestedProperty.TrimEnd(',').Replace(",/", ",");
            }
            else if (node is PropertyExpression)
            {
                PropertyExpression e = (PropertyExpression)node;
                if (e.ValueOnly)
                    return e.Property.Name + "/$value";
                else
                    return e.Property.Name;
            }
            else if (node is VariableExpression)
            {
                VariableExpression e = (VariableExpression)node;
                return e.Variable.Name;
            }
            if (node is LogicalExpression)
            {
                LogicalExpression e = (LogicalExpression)node;
                string left = this.Visit(e, e.Left);
                string right = null;

                if (e.Operator != LogicalOperator.Not)
                {
                    right = this.Visit(e, e.Right);
                }

                string logical;
                switch (e.Operator)
                {
                    case LogicalOperator.And:
                        logical = string.Format("{0} and {1}", left, right);
                        break;
                    case LogicalOperator.Or:
                        logical = string.Format("{0} or {1}", left, right);
                        break;
                    case LogicalOperator.Not:
                        logical = string.Format("not {0}", left);
                        break;

                    default:
                        throw new Exception("Unhandled Comparison Type: " + e.Operator);
                }
                return logical;
            }
            else if (node is ComparisonExpression)
            {
                ComparisonExpression e = (ComparisonExpression)node;
                String left = this.Visit(e, e.Left);
                String right = this.Visit(e, e.Right);

                switch (e.Operator)
                {
                    case ComparisonOperator.Equal:
                        return String.Format("{0} eq {1}", left, right);

                    case ComparisonOperator.NotEqual:
                        return String.Format("{0} ne {1}", left, right);

                    case ComparisonOperator.GreaterThan:
                        return String.Format("{0} gt {1}", left, right);

                    case ComparisonOperator.GreaterThanOrEqual:
                        return String.Format("{0} ge {1}", left, right);

                    case ComparisonOperator.LessThan:
                        return String.Format("{0} lt {1}", left, right);

                    case ComparisonOperator.LessThanOrEqual:
                        return String.Format("{0} le {1}", left, right);

                    default:
                        throw new Exception("Unhandled Comparison Type: " + e.Operator);
                };
            }
            else if (node is IsOfExpression || node is CastExpression)
            {
                ExpNode target;
                NodeType targetType;
                string operation;
                if (node is IsOfExpression)
                {
                    IsOfExpression e = (IsOfExpression)node;
                    operation = "isof";
                    target = e.Target;
                    targetType = e.TargetType;
                }
                else
                {
                    CastExpression e = (CastExpression)node;
                    operation = "cast";
                    target = e.Target;
                    targetType = e.TargetType;
                }

                string targetTypeName = targetType.FullName;
                if (targetType is PrimitiveType)
                {
                    targetTypeName = TypeData.FindForType(targetType.ClrType).GetEdmTypeName();
                }

                if (target == null)
                {
                    return String.Format("{0}('{1}')", operation, targetTypeName);
                }
                else
                {
                    return String.Format("{0}({1}, '{2}')", operation, this.Visit(node, target), targetTypeName);
                }
            }
            else if (node is ArithmeticExpression)
            {
                ArithmeticExpression e = (ArithmeticExpression)node;
                String left = this.Visit(e, e.Left);
                String right = this.Visit(e, e.Right);

                switch (e.Operator)
                {
                    case ArithmeticOperator.Add:
                        return String.Format("{0} add {1}", left, right);

                    case ArithmeticOperator.Div:
                        return String.Format("{0} div {1}", left, right);

                    case ArithmeticOperator.Mod:
                        return String.Format("{0} mod {1}", left, right);

                    case ArithmeticOperator.Mult:
                        return String.Format("{0} mul {1}", left, right);

                    case ArithmeticOperator.Sub:
                        return String.Format("{0} sub {1}", left, right);

                    default:
                        throw new Exception("Unhandled Arithmetic Type: " + e.Operator);
                };
            }
            else if (node is MethodExpression)
            {
                MethodExpression e = (MethodExpression)node;
                return BuildMemberOrMethodExpression(node, e.Caller, e.Arguments, e.Name);
            }
            else if (node is MemberExpression)
            {
                MemberExpression e = (MemberExpression)node;
                return BuildMemberOrMethodExpression(node, e.Caller, e.Arguments, e.Name);
            }
            else if (node is ConstantExpression)
            {
                ConstantExpression e = (ConstantExpression)node;
                object value = e.Value.ClrValue;

                string val = TypeData.FormatForKey(value, this.UseSmallCasing, false);
                if (this.EscapeUriValues)
                {
                    // FormatForKey already does this for doubles that don't have the D
                    if (!(value is double) || Versioning.Server.SupportsV2Features)
                        val = Uri.EscapeDataString(val);
                }

                if (value == null)
                    val = "null";
                else if (!(value is String))
                {
                    val = val.Replace("+", "").Replace("%2B", "");
                }

                return val;
            }
            else if (node is NegateExpression)
            {
                NegateExpression e = (NegateExpression)node;
                return "-(" + this.Visit(e, e.Argument) + ")";
            }
            else if (node is FirstExpression)
            {
                FirstExpression first = node as FirstExpression;
                return this.Visit(first, first.Input);
            }
            else if (node is FirstOrDefaultExpression)
            {
                FirstOrDefaultExpression first = node as FirstOrDefaultExpression;
                return this.Visit(first, first.Input);
            }
            else if (node is SingleExpression)
            {
                SingleExpression first = node as SingleExpression;
                return this.Visit(first, first.Input);
            }
            else if (node is SingleOrDefaultExpression)
            {
                SingleOrDefaultExpression first = node as SingleOrDefaultExpression;
                return this.Visit(first, first.Input);
            }
            else
            {
                throw new Exception(this.GetType().Name + " Unhandled Node: " + node.GetType().Name);
            }
        }

        public static string CreateKeyString(KeyExpression keyExp, bool binaryFormat)
        {
            Workspace w = keyExp.ResourceContainer.Workspace;
            UriQueryBuilder builder = new UriQueryBuilder(w, w.ServiceUri);
            builder.UseBinaryFormatForDates = binaryFormat;
            builder.CleanUpSpecialCharacters = true;

            return builder.CreateKeyString(keyExp);
        }

        public string CreateKeyString(KeyExpression keyExp)
        {
            System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<object> values = new System.Collections.Generic.List<object>();
            foreach (KeyValuePair<PropertyExpression, ConstantExpression> pair in keyExp.EnumerateIncludedPairs())
            {
                names.Add(pair.Key.Name);
                values.Add(pair.Value.Value.ClrValue);
            }
            return CreateKeyString(names.ToArray(), values.ToArray());
        }

        public string CreateKeyStringValue(object propertyValue)
        {
            if (propertyValue is DateTime)
            {
                if (this.UseBinaryFormatForDates)
                {
                    string prefix;
                    if (this.UseSmallCasing.HasValue && this.UseSmallCasing.Value)
                        prefix = "DaTeTiMe'";
                    else
                        prefix = "datetime'";
                    string dateTime = prefix + XmlConvert.ToString((DateTime)propertyValue, XmlDateTimeSerializationMode.RoundtripKind) + "'";
                    byte[] bytes = (new UTF8Encoding()).GetBytes(dateTime);
                    return TypeData.FormatByteArrayForKey(bytes, this.UseSmallCasing);
                }
                else
                {
                    string formatted = TypeData.FormatForKey(propertyValue, this.UseSmallCasing, Versioning.Server.SupportsV2Features);
                    formatted = Uri.EscapeDataString(formatted);
                    return formatted;
                }
            }
            else
            {
                string formatted = TypeData.FormatForKey(propertyValue, this.UseSmallCasing, Versioning.Server.SupportsV2Features);

                if (this.EscapeUriValues)
                {
                    // for some silly reason, TypeData.FormatForKey does this for already for doubles UNLESS IT PUTS THE 'D'
                    if (!(propertyValue is double) || Versioning.Server.SupportsV2Features)
                        formatted = Uri.EscapeDataString(formatted);
                }

                if (!(propertyValue is String) && this.CleanUpSpecialCharacters)
                    return formatted.Replace("+", "").Replace("%2B", "");
                else
                    return formatted;
            }
        }

        public string CreateKeyStringPair(string propertyName, object propertyValue)
        {
            return propertyName + "=" + CreateKeyStringValue(propertyValue);
        }

        public string CreateKeyString(string[] propertyNames, object[] propertyValues)
        {
            object[] values = propertyValues;
            string keyValue = String.Empty;
            StringBuilder builder = null;

            for (int i = 0; i < values.Length; ++i)
            {
                if (null == builder) { builder = new StringBuilder(); }

                if (0 < builder.Length) { builder.Append(','); }

                if (values.Length > 1)
                    builder.Append(CreateKeyStringPair(propertyNames[i], values[i]));
                else
                    builder.Append(CreateKeyStringValue(values[i]));
            }
            if (null != builder)
            {
                keyValue = builder.ToString();
            }
            return keyValue;
        }

        protected virtual string BuildMemberOrMethodExpression(ExpNode node, ExpNode caller, ExpNode[] arguments, string name)
        {
            string instance = null;
            string args = "";
            string actualName = null;

            if (caller != null)
                instance = this.Visit(node, caller);

            if (name.ToLowerInvariant() == "contains")
            {
                actualName = "contains";
                args = this.Visit(node, arguments[0]);
                return string.Format("{0}({1},{2})", actualName, instance, args);
            }
            else
            {
                actualName = name.ToLowerInvariant();

                if (arguments != null && arguments.Length > 0)
                {
                    for (int i = 0; i < arguments.Length; i++)
                        args += this.Visit(node, arguments[i]) + ", ";

                    args = args.Substring(0, args.Length - 2);

                    if (instance == null)
                        return string.Format("{0}({1})", actualName, args);
                    else
                        return string.Format("{0}({1},{2})", actualName, instance, args);
                }
                else
                    return string.Format("{0}({1})", actualName, instance);
            }
        }

        protected virtual String Deliminated(String[] strings)
        {
            return this.Deliminated(", ", strings);
        }

        protected virtual String Quote(string s)
        {
            return string.Format("'{0}'", s);
        }

        protected virtual String Deliminated(String deliminator, String[] strings)
        {
            return strings
                    .Aggregate("", (a, s) => (a == "" ? a : a + deliminator) + s);
        }

        protected virtual String Escaped(Node node)
        {
            return '[' + node.Name + ']';
        }
        public static string UrlEncodeString(string original)
        {
            string result = null;
            foreach (char c in original)
            {
                //if (c == ' ')
                //result += "%20";
                //else 
                if (c == '<')
                    result += "%3C";
                else if (c == '>')
                    result += "%3E";
                else if (c == '#')
                    result += "%23";
                else if (c == '%')
                    result += "%25";
                else if (c == '{')
                    result += "%7B";
                else if (c == '}')
                    result += "%7D";
                else if (c == '|')
                    result += "%7C";
                else if (c == '\\')
                    result += "%5C";
                else if (c == '^')
                    result += "%5E";
                else if (c == '~')
                    result += "%7E";
                else if (c == '[')
                    result += "%5B:";
                else if (c == ']')
                    result += "%5D";
                else if (c == '\'')
                    result += "''";
                else if (c == ';')
                    result += "%3B";
                else if (c == '/')
                    result += "%2F";
                else if (c == '?')
                    result += "%3F";
                else if (c == ':')
                    result += "%3A";
                else if (c == '@')
                    result += "%40";
                else if (c == '=')
                    result += "%3D";
                else if (c == '&')
                    result += "%26";
                else if (c == '$')
                    result += "%24";
                else if (c == '+')
                    result += "%2B";
                else
                    result += c;
            }
            if (original != null && original.Length == 0)
                result = "";
            return result;
        }
    }
}
