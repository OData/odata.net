//---------------------------------------------------------------------
// <copyright file="LinqQueryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
#if !ClientSKUFramework
using System.Data.Test.Astoria.NonClr;
#endif
using System.Globalization;
using System.Collections;
using Microsoft.OData.Client;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    //TODO: Need to Create a linq query based on the query tree
    //Might be able to steal from old CTF stuff
    public class LinqQueryBuilder : QueryBuilder
    {
        // this knob is used for cases with service operations returning complex types
        // currently, this only occurs in row count tests
        public static bool EnableTypeCoercion = true;

        private IQueryable _queryable;
        private IQueryable _queryableClient;
        private object _queryableSingle;


        //Constructor
        public LinqQueryBuilder(Workspace workspace)
            : base(workspace)
        {
        }
        public bool CountingMode { get; set; }

        public LinqQueryBuilder(Workspace workspace, IQueryable queryableClient)
            : base(workspace)
        {
            _queryableClient = queryableClient;
        }

        public Workspace EdmWorkspace
        {
            get { return base._workspace; }
        }

        public virtual IQueryable QueryResult
        {
            get { return _queryable; }
        }

        public virtual object QueryResultSingle
        {
            get { return _queryableSingle; }
        }

        public List<string> ProjectedProperties
        {
            get;
            set;
        }

        public PropertyMappings PropertyMapping
        {
            get;
            set;
        }

        public Type NewExpressionType { get; set; }

        private StringBuilder _sbExpression = new StringBuilder();
        public string QueryExpression
        {
            get
            {
                return _sbExpression.ToString();
            }
        }

        private PropertyExpression[] _expand;
        public PropertyExpression[] ExpandExpression
        {
            get
            {
                return _expand;
            }
            set
            {
                _expand = value;
            }
        }

        private void WriteTrace(string trace)
        {
            _sbExpression.Append(trace);
        }
        public override void Execute(params ExpNode[] expressions)
        {
            foreach (ExpNode t in expressions)
            {
                //Build query
                String query = this.Build(t);
            }
        }

        protected LambdaExpression CreateLambda(Expression body, ParameterExpression[] parameters)
        {
            return LambdaExpression.Lambda(body, parameters);
        }

        protected LambdaExpression CreateLambda(Type deletateType, Expression body, ParameterExpression[] parameters)
        {
            return LambdaExpression.Lambda(deletateType, body, parameters);
        }

        protected MethodInfo GetMethod(string methodName)
        {
            return ReflectionHelper.GetStaticMethod(typeof(System.Linq.Queryable), methodName);
        }

        protected void Where(Type sourceType, LambdaExpression predicate)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Where);
            Type[] genericArguments = new Type[] { sourceType };
            Object obj = this.Invoke(m, new object[] { _queryable, predicate }, genericArguments);
            _queryable = (IQueryable)obj;
        }

        public static Type GetEquivalentAnonymousType(params PropertyExpression[] properties)
        {
            List<Type> propertyTypes = properties.Select(prop => prop.Property.Type.ClrType).ToList();
            int commas = propertyTypes.Count;
            return Type.GetType(String.Format("AnonymousType`{0}", commas)).MakeGenericType(propertyTypes.ToArray());
        }
        public static Type GetEquivalentAnonymousType(Func<NodeType, Type> mutateTypes, params PropertyExpression[] properties)
        {
            List<Type> propertyTypes = properties.Select(prop => mutateTypes(prop.Property.Type)).ToList();
            int commas = propertyTypes.Count;
            return Type.GetType(String.Format("AnonymousType`{0}", commas)).MakeGenericType(propertyTypes.ToArray());
        }
        public static Type GetEquivalentAnonymousType(params Type[] propertyTypes)
        {
            //List<Type> propertyTypes = properties.Select(prop => prop.Property.Type.ClrType).ToList();
            int commas = propertyTypes.Length;
            return Type.GetType(String.Format("AnonymousType`{0}", commas)).MakeGenericType(propertyTypes.ToArray());
        }

        protected void SelectMany(Type sourceType, Type destType, LambdaExpression navigation)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.SelectMany);
            Type[] genericArguments = new Type[] { sourceType, destType };
            if (sourceType != _queryable.ElementType)
            {
                this.OfType(sourceType);
            }
            Object obj = this.Invoke(m, new object[] { _queryable, navigation }, genericArguments);
            _queryable = (IQueryable)obj;
        }

        protected void Select(Type sourceType, Type destType, LambdaExpression resultType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Select);
            Type[] genericArguments = new Type[] { sourceType, destType };
            if (sourceType != _queryable.ElementType)
            {
                this.OfType(sourceType);
            }
            Object obj = this.Invoke(m, new object[] { _queryable, resultType }, genericArguments);
            _queryable = (IQueryable)obj;
        }
        protected void OfType(Type ofType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.OfType);
            Type[] genericArguments = new Type[] { ofType };
            Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            _queryable = (IQueryable)obj;
        }

        protected void First(Type firstType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.First);
            Type[] genericArguments = new Type[] { firstType };
            Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            _queryableSingle = obj;
        }

        protected void FirstOrDefault(Type firstType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.FirstOrDefault);
            Type[] genericArguments = new Type[] { firstType };
            Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            _queryableSingle = obj;
        }

        protected void Single(Type firstType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Single);
            Type[] genericArguments = new Type[] { firstType };
            Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            _queryableSingle = obj;
        }

        protected void SingleOrDefault(Type firstType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.SingleOrDefault);
            Type[] genericArguments = new Type[] { firstType };
            Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            _queryableSingle = obj;
        }

        protected void Top(Type sourceType, int i)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Top);
            Type[] genericArguments = new Type[] { sourceType };
            Object obj = this.Invoke(m, new object[] { _queryable, i }, genericArguments);
            if (!CountingMode)
                _queryable = (IQueryable)obj;
        }

        protected void Skip(Type sourceType, int i)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Skip);
            Type[] genericArguments = new Type[] { sourceType };
            Object obj = this.Invoke(m, new object[] { _queryable, i }, genericArguments);
            if (!CountingMode)
                _queryable = (IQueryable)obj;
        }

        protected void Count(Type sourceType)
        {
            MethodInfo m = GetMethod(ReflectionHelper.Methods.Count);
            Type[] genericArguments = new Type[] { _queryable.ElementType };
            if (_queryable.ElementType != sourceType && EnableTypeCoercion)
            {
                this.OfType(sourceType);
                genericArguments = new Type[] { sourceType };
            }
            _queryableSingle = Expression.Call(typeof(Queryable), "Count", genericArguments, _queryable.Expression);

            _queryableSingle = TrustedMethods.IQueryableProviderExecuteMethodCall(_queryable, _queryableSingle);
            //_queryableSingle = countFunc.Invoke(_queryable, null);
            //Object obj = this.Invoke(m, new object[] { _queryable }, genericArguments);
            //_queryableSingle = obj;  // Int32
        }

        protected void Sort(Type sourceType, Type keyType, LambdaExpression ordervalues, bool bAscDesc)
        {

            MethodInfo m;
            Type[] genericArguments;
            Object obj;

            switch (bAscDesc)
            {
                case true:
                    m = GetMethod(ReflectionHelper.Methods.Sort);
                    genericArguments = new Type[] { sourceType, keyType };
                    obj = this.Invoke(m, new object[] { _queryable, ordervalues }, genericArguments);
                    _queryable = (IQueryable)obj;
                    break;
                case false:
                    m = GetMethod(ReflectionHelper.Methods.SortDesc);
                    genericArguments = new Type[] { sourceType, keyType };
                    obj = this.Invoke(m, new object[] { _queryable, ordervalues }, genericArguments);
                    _queryable = (IQueryable)obj;
                    break;
            }
        }

        protected void ThenBy(Type sourceType, Type keyType, LambdaExpression ordervalues, bool bAscDesc)
        {
            MethodInfo m;
            Type[] genericArguments;
            Object obj;

            switch (bAscDesc)
            {
                case true:
                    m = GetMethod(ReflectionHelper.Methods.ThenBy);
                    genericArguments = new Type[] { sourceType, keyType };
                    obj = this.Invoke(m, new object[] { _queryable, ordervalues }, genericArguments);
                    _queryable = (IQueryable)obj;
                    break;
                case false:
                    m = GetMethod(ReflectionHelper.Methods.ThenByDesc);
                    genericArguments = new Type[] { sourceType, keyType };
                    obj = this.Invoke(m, new object[] { _queryable, ordervalues }, genericArguments);
                    _queryable = (IQueryable)obj;
                    break;
            }

        }

        protected object Invoke(MethodInfo method, Object[] args, Type[] genericArgs)
        {
            MethodInfo genMethod = method.MakeGenericMethod(genericArgs);
            return genMethod.Invoke(null, args);
        }

        protected ResourceType GetContainedType(ResourceContainer container)
        {
            int count = container.Count();
            if (count == 1)
            {
                return container.ResourceTypes.First();
            }
            else
            {
                throw new Exception("Currently, unexpected number of ResoureTypes.");
            }
        }

        public override String Build(ExpNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node", "node cannot be null");
            string queryUri = base.Build(node);
            return "";
        }



        protected override String Visit(ExpNode caller, ExpNode node)
        {
            if (node is ProjectExpression)
            {
                ProjectExpression e = (ProjectExpression)node;
                if (e.Projections.Count == 0)
                {
                    this.WriteTrace("Select ");
                    return this.Visit(e, e.Input);
                }
                else if (e.Projections.Count == 1)
                {
                    string result = this.Visit(e, e.Input);

                    if (e.Projections[0] is PropertyExpression)
                    {
                        PropertyExpression prop = (PropertyExpression)e.Projections[0];
                        ResourceType parentType = (ResourceType)prop.Property.Parent;
                        ParameterExpression p;
                        if (_queryableClient != null)
                        {
                            p = Expression.Parameter(parentType.ClientClrType, "p");
                        }
                        else
                        {
                            p = Expression.Parameter(parentType.ClrType, "p");
                        }
                        ParameterExpression[] parameters = new ParameterExpression[] { p };
                        Expression body = Visit(e, prop, parameters);

                        Type enumerableType;
                        Type funcType;
                        LambdaExpression lambda;

                        if (prop.Type is CollectionType)
                        {
                            //Get Generic IEnumerable of destination Type
                            NodeType destType = ((CollectionType)prop.Type).SubType;

                            if (_queryableClient != null)
                            {
                                enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClientClrType);
                                funcType = typeof(Func<,>).MakeGenericType(parentType.ClientClrType, enumerableType);
                                lambda = CreateLambda(funcType, body, parameters);
                                this.SelectMany(parentType.ClientClrType, destType.ClientClrType, lambda);
                                this.WriteTrace("SelectMany " + lambda.ToString());
                            }
                            else
                            {
                                enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClrType);
                                funcType = typeof(Func<,>).MakeGenericType(parentType.ClrType, enumerableType);
                                lambda = CreateLambda(funcType, body, parameters);
                                this.SelectMany(parentType.ClrType, destType.ClrType, lambda);
                            }
                        }
                        else
                        {
                            PropertyExpression actualProp;
                            if (prop is NestedPropertyExpression)
                                actualProp = ((NestedPropertyExpression)prop).PropertyExpressions.LastOrDefault();
                            else
                                actualProp = prop;

                            Type destType;
                            if (_queryableClient != null)
                            {
                                //Get Generic IEnumerable of destination Type
                                if (prop.Type.ClientClrType != null)
                                    destType = actualProp.Property.Facets.Nullable ? actualProp.Type.NullableClientClrType : actualProp.Type.ClientClrType;
                                else
                                    destType = actualProp.Property.Facets.Nullable ? actualProp.Type.NullableClrType : actualProp.Type.ClrType;

                                funcType = typeof(Func<,>).MakeGenericType(parentType.ClientClrType, destType);
                                lambda = CreateLambda(funcType, body, parameters);
                                this.Select(parentType.ClientClrType, destType, lambda);
                                this.WriteTrace("Select " + lambda.ToString());
                            }
                            else
                            {
                                //Get Generic IEnumerable of destination Type
                                if (!actualProp.Property.Facets.Nullable || this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                                    destType = actualProp.Type.ClrType;
                                else
                                    destType = actualProp.Type.NullableClrType;

                                funcType = typeof(Func<,>).MakeGenericType(parentType.ClrType, destType);
                                lambda = CreateLambda(funcType, body, parameters);
                                this.Select(parentType.ClrType, destType, lambda);
                                this.WriteTrace("Select " + lambda.ToString());
                            }
                        }
                    }
                    else if (e.Projections[0] is ConstantExpression)
                    {
                        ConstantExpression prop = (ConstantExpression)e.Projections[0];
                        Type destType = prop.Type.ClrType;
                        Type pType = _queryable.ElementType.UnderlyingSystemType;

                        ParameterExpression p = Expression.Parameter(pType, "p");
                        ParameterExpression[] parameters = new ParameterExpression[] { p };
                        Expression body = Visit(e, prop, parameters);
                        Type funcType = typeof(Func<,>).MakeGenericType(pType, destType);

                        LambdaExpression lambda = CreateLambda(funcType, body, parameters);
                        this.Select(_queryable.ElementType.UnderlyingSystemType, destType, lambda);
                        this.WriteTrace(lambda.ToString());

                    }
                    else if (e.Projections[0] is NewExpression)
                    {
                        VisitNewExpression((NewExpression)e.Projections[0]);
                    }

                    return result;
                }
                else
                    throw new Exception("More than 1 projection, invalid");

            }
            else if (node is ScanExpression)
            {
                ScanExpression e = (ScanExpression)node;
                VariableExpression variableExp = e.Input as VariableExpression;

                if (_queryableClient != null)
                {
                    _queryable = _queryableClient;
                    return "ScanExpression";
                }


                if (variableExp != null && variableExp.Variable is ServiceOperation)
                {
                    _queryable = this.EdmWorkspace.ServiceOperationToQueryable((ServiceOperation)(variableExp.Variable));
                    this.WriteTrace("From" + variableExp.Variable.Name);
                    return "ScanExpression";
                }
                else
                    if (variableExp != null && variableExp.Variable is ResourceContainer)
                    {
                        _queryable = this.EdmWorkspace.ResourceContainerToQueryable((ResourceContainer)(variableExp.Variable));
                        if (_queryable == null)
                        {
                            throw new ArgumentException("_queryable  is null");
                        }
                        this.WriteTrace("From" + variableExp.Variable.Name);
                        return "ScanExpression";
                    }
                    else
                    {
                        throw new Exception("Only VariableExpression containing ResourceContainer is supported");
                    }

            }
            else if (node is ServiceOperationExpression)
            {
                ServiceOperationExpression serviceOpExpr = node as ServiceOperationExpression;
                VariableExpression variableExp = serviceOpExpr.Input as VariableExpression;
                ServiceOperation serviceOp = ((ServiceOperation)variableExp.Variable);
                if (variableExp != null && variableExp.Variable is ResourceContainer)
                {
                    _queryable = this.EdmWorkspace.ResourceContainerToQueryable(serviceOp.Container);
                    Top(_queryable.ElementType, serviceOp.ResultLimit);
                    if (_queryable == null)
                    {
                        throw new ArgumentException("_queryable  is null");
                    }
                    this.WriteTrace("From" + variableExp.Variable.Name);
                    return "ScanExpression";
                }
                else
                {
                    throw new Exception("Only VariableExpression containing ResourceContainer is supported");
                }
            }
            else if (node is PredicateExpression)
            {
                PredicateExpression e = (PredicateExpression)node;
                this.Visit(e, e.Input);

                NodeType paramType;
                if (e.Input.Type is CollectionType)
                {
                    paramType = ((CollectionType)(e.Input.Type)).SubType;
                }
                //else if (e.Input.Type is ResourceType)
                //{

                //}
                else
                {
                    paramType = e.Input.Type;
                }

                ParameterExpression p = null;
                if (_queryableClient != null)
                    p = Expression.Parameter(paramType.ClientClrType, "p");
                else
                    p = Expression.Parameter(paramType.ClrType, "p");

                ParameterExpression[] parameters = new ParameterExpression[] { p };
                Expression body = Visit(e, e.Predicate, parameters);

                LambdaExpression lambda = Expression.Lambda(body, parameters);

                if (_queryableClient != null)
                    this.Where(paramType.ClientClrType, lambda);
                else
                    this.Where(paramType.ClrType, lambda);

                // This throws on Arabic when datetime (out of range for Arabic Calendar) is serialized via ToString()
                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != "ar")
                    this.WriteTrace("Where " + lambda.Body.ToString());
                return "PredicateExpression";

            }
            else if (node is CountExpression)
            {
                CountExpression e = (CountExpression)node;
                this.Visit(e, e.Input);

                if (!e.IsInline || !(_queryable is DataServiceQuery))
                {
                    // Determine generic <Type> for .Count().
                    NodeType paramType;
                    NodeType destType = null;
                    paramType = e.ScanNode.Type;

                    if (paramType is CollectionType)
                    {
                        CollectionType collectionType = (CollectionType)paramType;
                        destType = collectionType.SubType;
                        if (_queryableClient == null)
                            this.Count(destType.ClrType);
                    }
                    else
                    {
                        if (_queryable != null && _queryable is IList)
                        {
                            IList listCount = _queryable as IList;
                            if (listCount != null)
                            {
                                _queryableSingle = listCount.Count;
                            }
                        }
                        if (_queryableClient == null)
                        {
                            this.Count(paramType.ClrType);
                        }
                        else
                        {
                            this.Count(paramType.ClientClrType);
                        }
                    }
                    this.WriteTrace("$count=" + e.CountKind);
                }
                else
                {
                    MethodInfo m = _queryable.GetType().GetMethods()
                        .First(method => method.Name == "IncludeCount" && method.GetParameters().Length == 0);
                    object obj = m.Invoke(_queryable, new object[] { });
                    _queryable = (IQueryable)obj;
                    this.WriteTrace("$inlinecount=allpages");
                }

                return "CountExpression";
            }
            else if (node is TopExpression)
            {
                TopExpression e = (TopExpression)node;
                this.Visit(e, e.Input);

                NodeType paramType;
                NodeType destType;
                paramType = e.ScanNode.Type;
                if (paramType is CollectionType)
                {
                    CollectionType collectionType = (CollectionType)paramType;
                    destType = collectionType.SubType;
                    if (_queryableClient != null)
                        this.Top(destType.ClientClrType, e.Predicate);
                    else
                        this.Top(destType.ClrType, e.Predicate);

                }
                else
                {
                    if (_queryableClient != null)
                        this.Top(paramType.ClientClrType, e.Predicate);
                    else
                        this.Top(paramType.ClrType, e.Predicate);
                }

                this.WriteTrace("Top " + e.Predicate.ToString());
                return "TopExpression";
                //return this.Visit(e, e.Input);

            }
            else if (node is SkipExpression)
            {
                SkipExpression e = (SkipExpression)node;
                this.Visit(e, e.Input);

                NodeType paramType;
                NodeType destType;
                paramType = e.ScanNode.Type;

                if (paramType is CollectionType)
                {
                    CollectionType collectionType = (CollectionType)paramType;
                    destType = collectionType.SubType;
                    if (_queryableClient != null)
                        this.Skip(destType.ClientClrType, e.Predicate);
                    else
                        this.Skip(destType.ClrType, e.Predicate);
                }
                else
                {
                    if (_queryableClient != null)
                        this.Skip(paramType.ClientClrType, e.Predicate);
                    else
                        this.Skip(paramType.ClrType, e.Predicate);
                }

                //return this.Visit(e, e.Input);
                this.WriteTrace("Skip " + e.Predicate.ToString());
                return "SkipExpression";

            }
            else if (node is OrderByExpression)
            {
                ParameterExpression p;
                Type funcType;
                NodeType destType = null; ;
                LambdaExpression ordervalues;
                ParameterExpression[] parameters;
                Expression body;

                OrderByExpression e = (OrderByExpression)node;

                this.Visit(e, e.Input);

                NodeType paramType;
                paramType = e.ScanNode.Type;

                if (paramType is CollectionType)
                {
                    CollectionType collectionType = (CollectionType)paramType;
                    destType = collectionType.SubType;

                    if (_queryableClient != null)
                        p = Expression.Parameter(destType.ClientClrType, "p");
                    else
                        p = Expression.Parameter(destType.ClrType, "p");

                    parameters = new ParameterExpression[] { p };
                    body = Visit(e, e, parameters);
                    Type enumerableType = body.Type;

                    if (_queryableClient != null)
                    {
                        funcType = typeof(Func<,>).MakeGenericType(destType.ClientClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.Sort(destType.ClientClrType, enumerableType, ordervalues, e.AscDesc);
                    }
                    else
                    {
                        funcType = typeof(Func<,>).MakeGenericType(destType.ClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.Sort(destType.ClrType, enumerableType, ordervalues, e.AscDesc);
                    }
                }
                else
                {
                    if (_queryableClient != null)
                        p = Expression.Parameter(paramType.ClientClrType, "p");
                    else
                        p = Expression.Parameter(paramType.ClrType, "p");

                    parameters = new ParameterExpression[] { p };

                    //body = Expression.Property(parameters[0], e.PropertiesExp[0].Name);
                    body = Visit(e, e, parameters);

                    Type enumerableType = body.Type;

                    if (_queryableClient != null)
                    {
                        funcType = typeof(Func<,>).MakeGenericType(paramType.ClientClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.Sort(paramType.ClientClrType, enumerableType, ordervalues, e.AscDesc);

                        //more properties
                        if (e.PropertiesExp.Count() > 1)
                        {
                            Type t = this._queryable.ElementType;
                            //body = Expression.Property(parameters[0], e.PropertiesExp[1].Name);

                            if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                            {
                                MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);
                                body = Expression.Convert(
                                            Expression.Call(null, mi, parameters[0], Expression.Constant(e.PropertiesExp[1].Name, typeof(String))), e.Type.ClrType);
                            }
                            else
                                body = Expression.Property(parameters[0], e.PropertiesExp[1].Name);

                            funcType = typeof(Func<,>).MakeGenericType(t, body.Type);
                            ordervalues = CreateLambda(funcType, body, parameters);
                            this.ThenBy(t, body.Type, ordervalues, e.AscDesc);
                            return "";
                        }
                    }
                    else
                    {
                        funcType = typeof(Func<,>).MakeGenericType(paramType.ClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.Sort(paramType.ClrType, enumerableType, ordervalues, e.AscDesc);

                        //more properties
                        if (e.PropertiesExp.Count() > 1)
                        {
                            Type t = this._queryable.ElementType;

                            if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                            {
                                MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);

                                for (int i = 1; i < e.PropertiesExp.Length; i++)
                                {
                                    body = Expression.Convert(
                                                Expression.Call(null, mi, parameters[0], Expression.Constant(e.PropertiesExp[i].Name, typeof(String))), e.PropertiesExp[i].Type.ClrType);

                                    funcType = typeof(Func<,>).MakeGenericType(t, body.Type);
                                    ordervalues = CreateLambda(funcType, body, parameters);
                                    this.ThenBy(t, body.Type, ordervalues, e.AscDesc);
                                }
                            }
                            else
                            {
                                for (int i = 1; i < e.PropertiesExp.Length; i++)
                                {
                                    body = Expression.Property(parameters[0], e.PropertiesExp[i].Name);

                                    funcType = typeof(Func<,>).MakeGenericType(t, body.Type);
                                    ordervalues = CreateLambda(funcType, body, parameters);
                                    this.ThenBy(t, body.Type, ordervalues, e.AscDesc);
                                }
                            }

                            return "";
                        }
                    }

                    this.WriteTrace("Sort " + ordervalues.Body.ToString());
                }

                return "OrderByExpression";
            }
            else if (node is ThenByExpression)
            {
                ParameterExpression p;
                Type funcType;
                NodeType destType = null; ;
                LambdaExpression ordervalues;
                ParameterExpression[] parameters;
                Expression body;

                ThenByExpression e = (ThenByExpression)node;

                this.Visit(e, e.Input);

                NodeType paramType;
                paramType = e.ScanNode.Type;

                if (paramType is CollectionType)
                {
                    CollectionType collectionType = (CollectionType)paramType;
                    destType = collectionType.SubType;

                    if (_queryableClient != null)
                        p = Expression.Parameter(destType.ClientClrType, "p");
                    else
                        p = Expression.Parameter(destType.ClrType, "p");

                    parameters = new ParameterExpression[] { p };
                    body = Visit(e, e, parameters);
                    Type enumerableType = body.Type;

                    if (_queryableClient != null)
                    {
                        funcType = typeof(Func<,>).MakeGenericType(destType.ClientClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.ThenBy(destType.ClientClrType, enumerableType, ordervalues, e.AscDesc);
                    }
                    else
                    {
                        funcType = typeof(Func<,>).MakeGenericType(destType.ClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.ThenBy(destType.ClrType, enumerableType, ordervalues, e.AscDesc);
                    }
                }
                else
                {
                    if (_queryableClient != null)
                        p = Expression.Parameter(paramType.ClientClrType, "p");
                    else
                        p = Expression.Parameter(paramType.ClrType, "p");

                    parameters = new ParameterExpression[] { p };

                    body = Visit(e, e, parameters);
                    Type enumerableType = body.Type;

                    if (_queryableClient != null)
                    {
                        funcType = typeof(Func<,>).MakeGenericType(paramType.ClientClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.ThenBy(paramType.ClientClrType, enumerableType, ordervalues, e.AscDesc);

                        //more properties

                        //if (e.PropertiesExp.Count() > 1)
                        //{
                        //    for(int i=1;i<e.PropertiesExp.Length;i++)
                        //    {
                        //        Type t = this._queryable.ElementType;
                        //        body = Expression.Property(parameters[0], e.PropertiesExp[i].Name);

                        //        funcType = typeof(Func<,>).MakeGenericType(t, body.Type);
                        //        ordervalues = CreateLambda(funcType, body, parameters);
                        //        this.ThenBy(t, body.Type, ordervalues, e.AscDesc);
                        //    }

                        //}
                    }
                    else
                    {
                        funcType = typeof(Func<,>).MakeGenericType(paramType.ClrType, enumerableType);
                        ordervalues = CreateLambda(funcType, body, parameters);
                        this.ThenBy(paramType.ClrType, enumerableType, ordervalues, e.AscDesc);
                    }
                }

                //return this.Visit(e, e.Input);
                this.WriteTrace("ThenBy");
                return "ThenByExpression";
            }
            else if (node is OfTypeExpression)
            {
                OfTypeExpression e = (OfTypeExpression)node;
                this.Visit(e, e.Input);

                this.OfType(e.ResourceType.ClrType);
                return "OfTypeExpression";
            }
            else if (node is FirstExpression)
            {
                FirstExpression e = (FirstExpression)node;
                this.Visit(e, e.Input);

                if (_queryableClient != null)
                {
                    if (this.NewExpressionType != null)
                        this.First(this.NewExpressionType);
                    else
                        this.First(e.ResourceType.ClientClrType);
                }
                else
                    this.First(e.ResourceType.ClrType);
                return "FirstExpression";
            }
            else if (node is FirstOrDefaultExpression)
            {
                FirstOrDefaultExpression e = (FirstOrDefaultExpression)node;
                this.Visit(e, e.Input);

                if (_queryableClient != null)
                {
                    if (this.NewExpressionType != null)
                        this.FirstOrDefault(this.NewExpressionType);
                    else
                        this.FirstOrDefault(e.ResourceType.ClientClrType);
                }
                else
                    this.FirstOrDefault(e.ResourceType.ClrType);

                return "FirstOrDefaultExpression";
            }
            else if (node is SingleExpression)
            {
                SingleExpression e = (SingleExpression)node;
                this.Visit(e, e.Input);

                if (_queryableClient != null)
                {
                    if (this.NewExpressionType != null)
                        this.Single(this.NewExpressionType);
                    else
                        this.Single(e.ResourceType.ClientClrType);
                }
                else
                    this.Single(e.ResourceType.ClrType);

                return "SingleExpression";
            }
            else if (node is SingleOrDefaultExpression)
            {
                SingleOrDefaultExpression e = (SingleOrDefaultExpression)node;
                this.Visit(e, e.Input);

                if (_queryableClient != null)
                {
                    if (this.NewExpressionType != null)
                        this.SingleOrDefault(this.NewExpressionType);
                    else
                        this.SingleOrDefault(e.ResourceType.ClientClrType);
                }
                else
                    this.SingleOrDefault(e.ResourceType.ClrType);
                return "SingleOrDefaultExpression";
            }
            else if (node is NavigationExpression)
            {
                NavigationExpression e = (NavigationExpression)node;
                this.Visit(e, e.Input);
                ResourceType parentType = (ResourceType)e.Property.Parent;
                ParameterExpression p = null;

                if (_queryableClient != null)
                    p = Expression.Parameter(parentType.ClientClrType, "p");
                else
                    p = Expression.Parameter(parentType.ClrType, "p");

                ParameterExpression[] parameters = new ParameterExpression[] { p };
                Expression body = Visit(e, e.PropertyExp, parameters);

                NodeType propertyType = e.Type;
                if (e.Type is CollectionType)
                {
                    //We are navigating from parentType to destType
                    NodeType destType;
                    CollectionType collectionType = (CollectionType)e.Type;
                    destType = collectionType.SubType;

                    Type funcType;
                    LambdaExpression lambda;

                    //Get Generic IEnumerable of destination Type
                    Type enumerableType = null;
                    if (_queryableClient != null)
                    {
                        enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClientClrType);
                        funcType = typeof(Func<,>).MakeGenericType(parentType.ClientClrType, enumerableType);
                        lambda = CreateLambda(funcType, body, parameters);
                        this.SelectMany(parentType.ClientClrType, destType.ClientClrType, lambda);
                        this.WriteTrace("SelectMany " + lambda.Body.ToString());
                    }
                    else
                    {
                        enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClrType);
                        funcType = typeof(Func<,>).MakeGenericType(parentType.ClrType, enumerableType);
                        lambda = CreateLambda(funcType, body, parameters);
                        this.SelectMany(parentType.ClrType, destType.ClrType, lambda);
                    }

                }
                else if (e.Type is ResourceType)
                {
                    //We are navigating from parentType to destType
                    NodeType destType = e.Type;

                    Type funcType;
                    LambdaExpression lambda;
                    Type enumerableType;
                    //Get Generic IEnumerable of destination Type
                    if (_queryableClient != null)
                    {

                        enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClientClrType);
                        funcType = typeof(Func<,>).MakeGenericType(parentType.ClientClrType, destType.ClientClrType);
                        lambda = CreateLambda(funcType, body, parameters);
                        this.Select(parentType.ClientClrType, destType.ClientClrType, lambda);
                        this.WriteTrace("SelectMany " + lambda.ToString());
                    }
                    else
                    {
                        enumerableType = typeof(IEnumerable<>).MakeGenericType(destType.ClrType);
                        funcType = typeof(Func<,>).MakeGenericType(parentType.ClrType, destType.ClrType);
                        lambda = CreateLambda(funcType, body, parameters);
                        this.Select(parentType.ClrType, destType.ClrType, lambda);
                    }

                }
                else
                {
                    throw new Exception("Error, property contained in Navigation is a navigation property");
                }

                return "NavigationExpression";
            }
            else if (node is ExpandExpression)
            {
                ExpandExpression e = (ExpandExpression)node;

                this.Visit(e, e.Input);

                this.ExpandExpression = e.PropertiesExp;

                if (e.PropertiesExp != null && e.PropertiesExp.Length > 0)
                {
                    MethodInfo m = _queryable.GetType().GetMethod("Expand", new Type[] { typeof(string) });

                    if (m != null)
                    {
                        string expand = string.Join<PropertyExpression>(",", e.PropertiesExp);

                        //foreach (PropertyExpression propertyExp in e.PropertiesExp)
                        //{
                            object obj = m.Invoke(_queryable, new object[] { expand });
                            _queryable = (IQueryable)obj;
                        //}
                    }
                }

                return "ExpandExpression";
            }
            else if (node is NewExpression)
            {
                VisitNewExpression((NewExpression)node);
                return "NewExpression";
            }
            else
            {
                //if (destType is ResourceType)
                // {
                //funcType = typeof(Func<,>).MakeGenericType(parentType.ClrType, destType.ClrType);
                //lambda = CreateLambda(funcType, body, parameters);
                //this.Select(parentType.ClrType, destType.ClrType, lambda);
                // }
                //else
                // {
                // }



                throw new Exception(this.GetType().Name + " Unhandled Node: " + node.GetType().Name);



            }
        }

        /// <summary>
        /// For a given set of constructor arguments , return the type that has the respective LHS properties for the RHS expressions
        /// </summary>
        /// <param name="newExpression">The newExpression that specifies the arguments to be created</param>
        /// <param name="param"></param>
        /// <returns>Anonymous Type which has the same properties as the RHS</returns>
        private Type BuildAnonymousType(NewExpression newExpression, ParameterExpression param)
        {
            List<Type> PropertyTYpes = new List<Type>();
            List<MemberBindExpression> memberBindExpressions = new List<MemberBindExpression>();

            foreach (ExpNode argument in newExpression.Arguments)
            {
                //Visit the RHS and find the return type of the expression
                Expression bindExpression = Visit(argument, argument, new ParameterExpression[] { param });
                //EntityType.Property = InputType.Property
                PropertyTYpes.Add(
                        bindExpression.Type
                    );
            }
            int fieldIndex = 1;
            string fieldName = "Field";

            Type entityType = GetEquivalentAnonymousType(PropertyTYpes.ToArray());

            //Now that we have the appropriate LHS for the RHS , we remove the previous RHS 
            //and replace them with MemberBindExpressions binding the RHS to the LHS
            List<ExpNode> addTheseNodes = new List<ExpNode>();
            foreach (ExpNode argument in newExpression.Arguments)
            {
                if (argument is MemberBindExpression)
                {
                    throw new InvalidOperationException("This linq query cannot have memberBindExpressions");
                }
                addTheseNodes.Add(new MemberBindExpression(argument, new TypedMemberExpression(entityType, fieldName + fieldIndex++)));
            }
            //Remove all the old RHS expressions
            newExpression.Arguments.Clear();
            //add the new MemberBindExpressions which bind RHS to LHS
            newExpression.Arguments.Add(addTheseNodes.ToArray());
            return entityType;
        }
        private MemberBinding VisitMemberBindingExpression(ParameterExpression parameter, ExpNode node)
        {

            if (node is NestedPropertyExpression)
            {
                NestedPropertyExpression npExpression = node as NestedPropertyExpression;
                System.Linq.Expressions.MemberExpression topLevelProperty = Expression.Property(parameter, npExpression.Property.Name);
                ParameterExpression paramToSecondLevel = Expression.Parameter(topLevelProperty.Type.IsGenericType ? topLevelProperty.Type.GetGenericArguments()[0] : topLevelProperty.Type, "topLevelProperty");
                System.Linq.Expressions.MemberExpression secondLevelProperty = Expression.Property(paramToSecondLevel, npExpression.PropertyExpressions[0].Name);
                MemberBinding bindProperty = null;
                bindProperty = Expression.Bind(secondLevelProperty.Member, paramToSecondLevel);
                return bindProperty;
            }
            //if this is a propertyExpression , 
            //then we are projecting into the same property of the same entity type
            else if (node is PropertyExpression)
            {
                PropertyExpression property = node as PropertyExpression;
                ResourceProperty rProperty = property.Property as ResourceProperty;
                MemberBinding bindProperty = null;
                Type typeContainingProperty = parameter.Type;
                string sourceProperty = property.Name;
                string targetProperty = property.Name;
                //input.Property = input.Property
                bindProperty = GetMemberInitExpressionForProperty(typeContainingProperty, sourceProperty, targetProperty, parameter);
                return bindProperty;
            }
            else if (node is MemberBindExpression)
            {
                MemberBindExpression propertyBinding = node as MemberBindExpression;
                MemberBinding bindProperty = null;
                string rhsProperty = null;
                //we assume that the assignment is between properties of the same type 
                //InputType=>
                var rhsParameter = parameter;
                int propertyNameIndex = 1;
                string propertyName = "Field" + 1;
                //When the RHS of the assigment is an expression
                //ex: MyFullName = FirstName+"( Astoria )"+LastName
                if (propertyBinding.TargetProperty != null)
                {
                    rhsProperty = propertyBinding.TargetProperty.PropertyName;
                    //EntityType=>
                    rhsParameter = Expression.Parameter(propertyBinding.TargetProperty.EntityType, "rhsParam");
                }
                else
                {
                    rhsProperty = propertyName + (propertyNameIndex++);
                    rhsProperty = propertyBinding.TargetPropertyName;
                }
                //FirstName+"( Astoria )"+LastName
                Expression LHS = Visit(node, propertyBinding.SourceProperty, new ParameterExpression[] { parameter });

                //store the LHS and RHS for comparision in VerifyLinq
                if (LHS is System.Linq.Expressions.MemberExpression || LHS is UnaryExpression)
                {
                    if (PropertyMapping == null)
                    {
                        PropertyMapping = new PropertyMappings();
                    }
                    if (ProjectedProperties == null)
                    {
                        ProjectedProperties = new List<string>();
                    }
                    string lhsPropertyName = String.Empty;
                    if (LHS is System.Linq.Expressions.MemberExpression)
                    {
                        lhsPropertyName = ((System.Linq.Expressions.MemberExpression)LHS).Member.Name;
                    }
                    else if (LHS is UnaryExpression && this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                    {
                        UnaryExpression unaryLHS = LHS as UnaryExpression;
                        if (unaryLHS.Operand is MethodCallExpression)
                        {
                            MethodCallExpression mcExpression = unaryLHS.Operand as MethodCallExpression;
                            if (mcExpression.Method.Name == "GetPropertyFrom")
                            {
                                lhsPropertyName = mcExpression.Arguments[1].ToString().Replace("\"", "");
                            }
                        }
                    }
                    PropertyMapping.Add(new KeyValuePair<string, string>(lhsPropertyName, rhsProperty));
                    ProjectedProperties.Add(lhsPropertyName);
                }

                Type EntityType = rhsParameter.Type;
                //rhs.Property
                System.Linq.Expressions.MemberExpression member = Expression.Property(rhsParameter, rhsProperty);
                //get_Property
                MemberInfo propertyMemberInfo = EntityType.GetProperty(rhsProperty);
                //rhs.Property = LHS
                bindProperty = Expression.Bind(propertyMemberInfo, LHS);
                return bindProperty;
            }
            throw new NotSupportedException(String.Format("this node type is not supported :{0}", node.Name));
        }

        private void VisitNewExpression(NewExpression e)
        {
            NewExpression newExpression = e;// (e.Projections[0]) as NewExpression;
            //passing in the input builds the _queryable instance that contains the results for the Where Selector that this New expression
            //is supposed to create instances from.
            this.Visit(e, newExpression.Input);
            //this is the entity type for the Queryable that will be generating the new entities
            Type inputType = _queryable.ElementType;
            ParameterExpression inputParameter = Expression.Parameter(inputType, "entity");
            //We need to create an anonymous type that has the same type declaration as the rhs parameters
            if (newExpression.EntityType == null)
            {
                newExpression.EntityType = BuildAnonymousType(newExpression, inputParameter);

            }
            this.NewExpressionType = newExpression.EntityType;

            //If the new entity to be created is of a specific entity type
            if (newExpression.EntityType != null)
            {
                //input=>
                List<MemberBinding> bindings = new List<MemberBinding>();
                foreach (ExpNode argument in newExpression.Arguments)
                {
                    //EntityType.Property = InputType.Property
                    MemberBinding bindPropety = VisitMemberBindingExpression(inputParameter, argument);
                    bindings.Add(bindPropety);
                }
                //for now, we assume a default empty constructor , 
                //TODO: tests require a constructor with more than one parameter
                //=> new EntityType()
                System.Linq.Expressions.NewExpression constructor = Expression.New(newExpression.EntityType);
                // => new EntityType() { EntityType.Property= InputType.Property }
                Expression objectWithPropertiesSet = Expression.MemberInit(constructor, bindings);
                //=> Select<InputType,EntityType>(
                var funcType = LambdaExpression.GetFuncType(new Type[] { inputType, newExpression.EntityType });
                var createInstance = Expression.Lambda(funcType, objectWithPropertiesSet, inputParameter);
                //Select<InputType,EntityType>( input => new EntityType() { EntityType.Property= InputType.Property }
                Expression selectExpression = Expression.Call(typeof(Queryable), "Select", new Type[] { inputType, newExpression.EntityType }, _queryable.Expression, createInstance);
                _queryable = _queryable.Provider.CreateQuery(selectExpression);
                AstoriaTestLog.WriteLineIgnore(selectExpression.ToString());
            }
        }



        private MemberBinding GetMemberInitExpressionForProperty(Type type, string sourceProperty, string targetProperty, ParameterExpression param)
        {
            Type EntityType = param.Type;


            System.Linq.Expressions.MemberExpression member = Expression.Property(param, sourceProperty);
            //get_Property
            MemberInfo propertyMemberInfo = EntityType.GetProperty(targetProperty);
            //get_Property(entityInstance)
            MemberBinding bindFirstName = Expression.Bind(propertyMemberInfo, member);
            return bindFirstName;
        }

        private static object GetPropertyFrom(object source, string propertyName)
        {
#if !ClientSKUFramework

            return ((RowComplexType)source).Properties[propertyName];
#endif
#if ClientSKUFramework
	    return null;
#endif
        }

        protected virtual Expression Visit(ExpNode caller, ExpNode node, ParameterExpression[] parameters)
        {
            if (node is LogicalExpression)
            {
                LogicalExpression e = (LogicalExpression)node;
                Expression left = this.Visit(e, e.Left, parameters);
                Expression right = null;

                if (e.Operator != LogicalOperator.Not)
                {
                    right = this.Visit(e, e.Right, parameters);
                }

                Expression logical;
                switch (e.Operator)
                {
                    case LogicalOperator.And:
                        logical = Expression.And(left, right);
                        break;
                    case LogicalOperator.Or:
                        logical = Expression.Or(left, right);
                        break;
                    case LogicalOperator.Not:
                        logical = Expression.Not(left);
                        break;
                    default:
                        throw new Exception("Unhandled Comparison Type: " + e.Operator);
                }

                return logical;
            }
            else if (node is TypedMemberExpression)
            {
                TypedMemberExpression tme = node as TypedMemberExpression;
                ParameterExpression param = Expression.Parameter(tme.EntityType, "entity");
                return Expression.Property(param, tme.PropertyName);
            }
            else if (node is IsOfExpression)
            {
                IsOfExpression e = (IsOfExpression)node;

                if (e.Target == null)
                {
                    return null;
                }
                else
                {
                    return Expression.TypeIs(this.Visit(e, e.Target, parameters), e.TargetType.ClrType);
                }
            }
            else if (node is CastExpression)
            {
                return null;
            }
            else if (node is ComparisonExpression)
            {
                ComparisonExpression e = (ComparisonExpression)node;
                Expression left = this.Visit(e, e.Left, parameters);
                Expression right = this.Visit(e, e.Right, parameters);

                Expression comparison;

                // TODO: For strings, change expressions to Methodexpressions (Compare) for gt, lt, le, ge
                switch (e.Operator)
                {
                    case ComparisonOperator.Equal:
                        comparison = Expression.Equal(left, right);
                        break;

                    case ComparisonOperator.NotEqual:
                        comparison = Expression.NotEqual(left, right);
                        break;

                    case ComparisonOperator.GreaterThan:
                        if (e.Left.Type is ClrString && e.Right.Type is ClrString && _workspace.Database != null)
                        {
                            MethodInfo methodInfo = typeof(LinqQueryBuilder).GetMethod("StringGreaterThan", BindingFlags.NonPublic | BindingFlags.Static);
                            comparison = Expression.GreaterThan(left, right, false, methodInfo);
                        }
                        else
                            comparison = Expression.GreaterThan(left, right);

                        break;

                    case ComparisonOperator.GreaterThanOrEqual:
                        if (e.Left.Type is ClrString && e.Right.Type is ClrString && _workspace.Database != null)
                        {
                            MethodInfo methodInfo = typeof(LinqQueryBuilder).GetMethod("StringGreaterThanOrEqual", BindingFlags.NonPublic | BindingFlags.Static);
                            comparison = Expression.GreaterThanOrEqual(left, right, false, methodInfo);
                        }
                        else
                            comparison = Expression.GreaterThanOrEqual(left, right);
                        break;

                    case ComparisonOperator.LessThan:
                        if (e.Left.Type is ClrString && e.Right.Type is ClrString && _workspace.Database != null)
                        {
                            MethodInfo methodInfo = typeof(LinqQueryBuilder).GetMethod("StringLessThan", BindingFlags.NonPublic | BindingFlags.Static);
                            comparison = Expression.LessThan(left, right, false, methodInfo);
                        }
                        else
                            comparison = Expression.LessThan(left, right);
                        break;

                    case ComparisonOperator.LessThanOrEqual:
                        if (e.Left.Type is ClrString && e.Right.Type is ClrString && _workspace.Database != null)
                        {
                            MethodInfo methodInfo = typeof(LinqQueryBuilder).GetMethod("StringLessThanOrEqual", BindingFlags.NonPublic | BindingFlags.Static);
                            comparison = Expression.LessThanOrEqual(left, right, false, methodInfo);
                        }
                        else
                            comparison = Expression.LessThanOrEqual(left, right);
                        break;

                    default:
                        throw new Exception("Unhandled Comparison Type: " + e.Operator);
                };

                return comparison;
            }
            else if (node is ArithmeticExpression)
            {
                ArithmeticExpression e = (ArithmeticExpression)node;
                Expression left = this.Visit(e, e.Left, parameters);
                Expression right = this.Visit(e, e.Right, parameters);

                Expression arithmetic;

                switch (e.Operator)
                {
                    case ArithmeticOperator.Add:
                        arithmetic = Expression.Add(left, right);
                        break;

                    case ArithmeticOperator.Div:
                        arithmetic = Expression.Divide(left, right);
                        break;

                    case ArithmeticOperator.Mod:
                        arithmetic = Expression.Modulo(left, right);
                        break;

                    case ArithmeticOperator.Mult:
                        arithmetic = Expression.Multiply(left, right);
                        break;

                    case ArithmeticOperator.Sub:
                        arithmetic = Expression.Subtract(left, right);
                        break;

                    default:
                        throw new Exception("Unhandled Arithmetic Type: " + e.Operator);
                };

                return arithmetic;
            }
            else if (node is MethodExpression)
            {
                MethodExpression e = (MethodExpression)node;
                Expression instance = null;

                if (e.Caller != null)
                    instance = this.Visit(e, e.Caller, parameters);

                if (e.Arguments != null && e.Arguments.Length > 0)
                {
                    Expression[] args = new Expression[e.Arguments.Length];

                    for (int i = 0; i < e.Arguments.Length; i++)
                        args[i] = this.Visit(e, e.Arguments[i], parameters);

                    return MethodCallExpression.Call(instance, e.MethodInfo, args);
                }
                else
                    return MethodCallExpression.Call(instance, e.MethodInfo, null);
            }
            else if (node is MemberExpression)
            {
                MemberExpression e = (MemberExpression)node;
                Expression instance = null;

                if (e.Caller != null)
                    instance = this.Visit(e, e.Caller, parameters);

                if (e.Arguments != null && e.Arguments.Length > 0)
                {
                    Expression[] args = new Expression[e.Arguments.Length];

                    for (int i = 0; i < e.Arguments.Length; i++)
                        args[i] = this.Visit(e, e.Arguments[i], parameters);

                    return System.Linq.Expressions.MemberExpression.Property(instance, e.Property); // arguments ignored
                }
                else
                    return System.Linq.Expressions.MemberExpression.Property(instance, e.Property);
            }
            else if (node is NestedPropertyExpression)
            {
                NestedPropertyExpression e = (NestedPropertyExpression)node;

                if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                {
                    MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);

                    Expression e1 = Expression.Convert(
                            Expression.Call(null, mi, parameters[0], Expression.Constant(e.PropertyExpressions[0].Property.Name, typeof(String))), e.PropertyExpressions[0].Type.ClrType);
                    return
                        Expression.Convert(
                            Expression.Call(null, mi, e1, Expression.Constant(e.PropertyExpressions[1].Property.Name, typeof(String))), e.PropertyExpressions[1].Type.ClrType);
                }
                else
                {
                    Expression root = null;
                    Expression secondLevel = null;

                    if (e.PropertyExpressions.Count() == 1)
                    {
                        root = Expression.Property(parameters[0], e.Name);
                        secondLevel = Expression.Property(root, e.PropertyExpressions[0].Property.Name);
                    }
                    else
                    {
                        if (e.PropertyExpressions[0].Property.Facets.Nullable && !e.PropertyExpressions[1].Property.Type.ClrType.IsClass)
                        {
                            root = Expression.Property(parameters[0], e.Property.Name);
                        }
                        else
                        {
                            if (e.PropertyExpressions[0].Property.Facets.Nullable)
                            {
                                root = Expression.Property(Expression.Property(parameters[0], e.Property.Name), "Value");
                            }
                            else
                            {
                                root = Expression.Property(parameters[0], e.Property.Name);
                            }
                        }


                        if (e.PropertyExpressions[1].Property.Facets.Nullable && !e.PropertyExpressions[1].Property.Type.ClrType.IsClass)
                        {
                            secondLevel = Expression.Property(
                                Expression.Property(root, e.PropertyExpressions[1].Property.Name)
                                , "Value");
                        }
                        else
                        {
                            secondLevel = Expression.Property(root, e.PropertyExpressions[1].Property.Name);
                        }
                    }
                    //secondLevel = Expression.Property( 
                    return secondLevel;

                }
            }
            else if (node is NullablePropertyExpression)
            {
                NullablePropertyExpression e = (NullablePropertyExpression)node;
                if (e.Property.Facets.Nullable)
                {
                    if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                    {
                        MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);
                        return Expression.Convert(
                                    Expression.Call(null, mi, parameters[0], Expression.Constant(e.Property.Name, typeof(String))), e.Type.ClrType);
                    }
                    else
                        return Expression.Property(Expression.Property(parameters[0], e.Property.Name), "Value");
                }
                else
                {
                    if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                    {
                        MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);
                        return Expression.Convert(
                                    Expression.Call(null, mi, parameters[0], Expression.Constant(e.Property.Name, typeof(String))), e.Type.ClrType);
                    }
                    else
                        return Expression.Property(parameters[0], e.Property.Name);
                }
            }
            else if (node is PropertyExpression)
            {
                PropertyExpression e = (PropertyExpression)node;

                if (this._workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
                {
                    MethodInfo mi = this.GetType().GetMethod("GetPropertyFrom", BindingFlags.NonPublic | BindingFlags.Static);
                    return Expression.Convert(
                                Expression.Call(null, mi, parameters[0], Expression.Constant(e.Property.Name, typeof(String))), e.Type.ClrType);
                }
                else
                    return Expression.Property(parameters[0], e.Property.Name);
            }
            else if (node is ConstantExpression)
            {
                ConstantExpression e = (ConstantExpression)node;
                if (e.Facets.Nullable)
                {
                    return Expression.Constant(e.Value.NullableClrValue, e.Value.NullableClrType);
                }
                else
                {
                    return Expression.Constant(e.Value.ClrValue);
                }

            }
            else if (node is KeyExpression)
            {
                KeyExpression e = (KeyExpression)node;
                return Visit(e, e.Predicate, parameters);
            }
            else if (node is OrderByExpression)
            {
                OrderByExpression e = (OrderByExpression)node;
                return this.Visit(e, e.PropertiesExp[0], parameters);
            }
            else if (node is ThenByExpression)
            {
                ThenByExpression e = (ThenByExpression)node;
                return Expression.Property(parameters[0], e.PropertiesExp[0].Name);
            }
            else
            {
                throw new Exception(this.GetType().Name + " Unhandled Node: " + node.GetType().Name);
            }

        }

        private static byte ByteAdd(byte left, byte right)
        {
            return (byte)(left + right);
        }

        private static bool StringGreaterThan(string left, string right)
        {
            return string.CompareOrdinal(left, right) > 0;
        }

        private static bool StringGreaterThanOrEqual(string left, string right)
        {
            return string.CompareOrdinal(left, right) >= 0;
        }

        private static bool StringLessThan(string left, string right)
        {
            return string.CompareOrdinal(left, right) < 0;
        }

        private static bool StringLessThanOrEqual(string left, string right)
        {
            return string.CompareOrdinal(left, right) <= 0;
        }

        protected virtual String Deliminated(String[] strings)
        {
            return this.Deliminated(", ", strings);
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

        protected virtual String Quote(string s)
        {
            return string.Format("\"{0}\"", s);
        }
    }

    public class PropertyMappings : List<KeyValuePair<string, string>>
    {
        public string this[string propertyName]
        {
            get
            {
                return this.FirstOrDefault(kvp => kvp.Key == propertyName).Value;
            }
        }
    }

    public static class ReflectionHelper
    {
        public class Methods
        {
            public const string Where = @"System.Linq.IQueryable`1[TSource] Where[TSource](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,System.Boolean]])";
            public const string Select = @"System.Linq.IQueryable`1[TResult] Select[TSource,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TResult]])";
            public const string SelectMany = @"System.Linq.IQueryable`1[TResult] SelectMany[TSource,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,System.Collections.Generic.IEnumerable`1[TResult]]])";
            public const string Count = @"Int32 Count[TSource](System.Linq.IQueryable`1[TSource])";
            public const string Top = @"System.Linq.IQueryable`1[TSource] Take[TSource](System.Linq.IQueryable`1[TSource], Int32)";
            public const string Skip = @"System.Linq.IQueryable`1[TSource] Skip[TSource](System.Linq.IQueryable`1[TSource], Int32)";
            public const string Sort = @"System.Linq.IOrderedQueryable`1[TSource] OrderBy[TSource,TKey](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string SortDesc = @"System.Linq.IOrderedQueryable`1[TSource] OrderByDescending[TSource,TKey](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string ThenBy = @"System.Linq.IOrderedQueryable`1[TSource] ThenBy[TSource,TKey](System.Linq.IOrderedQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string ThenByDesc = @"System.Linq.IOrderedQueryable`1[TSource] ThenByDescending[TSource,TKey](System.Linq.IOrderedQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string OfType = @"System.Linq.IQueryable`1[TResult] OfType[TResult](System.Linq.IQueryable)";
            public const string First = @"TSource First[TSource](System.Linq.IQueryable`1[TSource])";
            public const string FirstOrDefault = @"TSource FirstOrDefault[TSource](System.Linq.IQueryable`1[TSource])";
            public const string Single = @"TSource Single[TSource](System.Linq.IQueryable`1[TSource])";
            public const string SingleOrDefault = @"TSource SingleOrDefault[TSource](System.Linq.IQueryable`1[TSource])";
        }

        private static Dictionary<string, MethodInfo> methodTable = new Dictionary<string, MethodInfo>();

        public static MethodInfo GetStaticMethod(Type type, string name)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Default;
            MethodInfo methodInfo = null;
            if (!methodTable.TryGetValue(name, out methodInfo))
            {
                foreach (MethodInfo m in type.GetMembers(flags))
                {
                    if (m.ToString() == name)
                    {
                        methodInfo = m;
                        methodTable.Add(name, m);
                    }
                }

            }

            return methodInfo;
        }
    }



}
#region Projected Types


public class AnonymousType<T1>
{
    public T1 Field1 { get; set; }
}

public class AnonymousType<T1, T2>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
}

public class AnonymousType<T1, T2, T3>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
{
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
}

public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44, T45 field45)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
        Field45 = field45;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
    public T45 Field45 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45, T46>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44, T45 field45, T46 field46)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
        Field45 = field45;
        Field46 = field46;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
    public T45 Field45 { get; set; }
    public T46 Field46 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45, T46, T47>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44, T45 field45, T46 field46, T47 field47)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
        Field45 = field45;
        Field46 = field46;
        Field47 = field47;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
    public T45 Field45 { get; set; }
    public T46 Field46 { get; set; }
    public T47 Field47 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45, T46, T47, T48>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44, T45 field45, T46 field46, T47 field47, T48 field48)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
        Field45 = field45;
        Field46 = field46;
        Field47 = field47;
        Field48 = field48;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
    public T45 Field45 { get; set; }
    public T46 Field46 { get; set; }
    public T47 Field47 { get; set; }
    public T48 Field48 { get; set; }
}
public class AnonymousType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35, T36, T37, T38, T39, T40, T41, T42, T43, T44, T45, T46, T47, T48, T49>
{

    public AnonymousType()
    {
    }
    public AnonymousType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13, T14 field14, T15 field15, T16 field16, T17 field17, T18 field18, T19 field19, T20 field20, T21 field21, T22 field22, T23 field23, T24 field24, T25 field25, T26 field26, T27 field27, T28 field28, T29 field29, T30 field30, T31 field31, T32 field32, T33 field33, T34 field34, T35 field35, T36 field36, T37 field37, T38 field38, T39 field39, T40 field40, T41 field41, T42 field42, T43 field43, T44 field44, T45 field45, T46 field46, T47 field47, T48 field48, T49 field49)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
        Field5 = field5;
        Field6 = field6;
        Field7 = field7;
        Field8 = field8;
        Field9 = field9;
        Field10 = field10;
        Field11 = field11;
        Field12 = field12;
        Field13 = field13;
        Field14 = field14;
        Field15 = field15;
        Field16 = field16;
        Field17 = field17;
        Field18 = field18;
        Field19 = field19;
        Field20 = field20;
        Field21 = field21;
        Field22 = field22;
        Field23 = field23;
        Field24 = field24;
        Field25 = field25;
        Field26 = field26;
        Field27 = field27;
        Field28 = field28;
        Field29 = field29;
        Field30 = field30;
        Field31 = field31;
        Field32 = field32;
        Field33 = field33;
        Field34 = field34;
        Field35 = field35;
        Field36 = field36;
        Field37 = field37;
        Field38 = field38;
        Field39 = field39;
        Field40 = field40;
        Field41 = field41;
        Field42 = field42;
        Field43 = field43;
        Field44 = field44;
        Field45 = field45;
        Field46 = field46;
        Field47 = field47;
        Field48 = field48;
        Field49 = field49;
    }
    public T1 Field1 { get; set; }
    public T2 Field2 { get; set; }
    public T3 Field3 { get; set; }
    public T4 Field4 { get; set; }
    public T5 Field5 { get; set; }
    public T6 Field6 { get; set; }
    public T7 Field7 { get; set; }
    public T8 Field8 { get; set; }
    public T9 Field9 { get; set; }
    public T10 Field10 { get; set; }
    public T11 Field11 { get; set; }
    public T12 Field12 { get; set; }
    public T13 Field13 { get; set; }
    public T14 Field14 { get; set; }
    public T15 Field15 { get; set; }
    public T16 Field16 { get; set; }
    public T17 Field17 { get; set; }
    public T18 Field18 { get; set; }
    public T19 Field19 { get; set; }
    public T20 Field20 { get; set; }
    public T21 Field21 { get; set; }
    public T22 Field22 { get; set; }
    public T23 Field23 { get; set; }
    public T24 Field24 { get; set; }
    public T25 Field25 { get; set; }
    public T26 Field26 { get; set; }
    public T27 Field27 { get; set; }
    public T28 Field28 { get; set; }
    public T29 Field29 { get; set; }
    public T30 Field30 { get; set; }
    public T31 Field31 { get; set; }
    public T32 Field32 { get; set; }
    public T33 Field33 { get; set; }
    public T34 Field34 { get; set; }
    public T35 Field35 { get; set; }
    public T36 Field36 { get; set; }
    public T37 Field37 { get; set; }
    public T38 Field38 { get; set; }
    public T39 Field39 { get; set; }
    public T40 Field40 { get; set; }
    public T41 Field41 { get; set; }
    public T42 Field42 { get; set; }
    public T43 Field43 { get; set; }
    public T44 Field44 { get; set; }
    public T45 Field45 { get; set; }
    public T46 Field46 { get; set; }
    public T47 Field47 { get; set; }
    public T48 Field48 { get; set; }
    public T49 Field49 { get; set; }
}

#endregion

