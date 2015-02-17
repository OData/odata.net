//---------------------------------------------------------------------
// <copyright file="TestURIWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;
using System.Net;
using System.IO;
using System.Reflection;

namespace AstoriaUnitTests.Tests
{
    internal abstract class TestURIWriter : IDisposable
    {
        protected string _uri;
        protected Dictionary<string, List<string>> _parameters;

        public void Dispose() { }

        public static TestURIWriter<T> CreateURIWriter<T>(string uri, Dictionary<string, List<string>> parameters)
        {
            return new TestURIWriter<T>(uri, parameters);
        }

        public static TestURIWriter<T> CreateURIWriter<T>(string baseUri, string resourceSetName)
        {
            return new TestURIWriter<T>(baseUri + "/" + resourceSetName, null);
        }

        internal string GetRawUri()
        {
            return _uri;
        }

        internal string GetUri()
        {
            StringBuilder queryoptions = new StringBuilder(_uri.TrimEnd('/'));
            if (_parameters.Count > 0)
            {
                queryoptions.Append("?");

                string[] keys = _parameters.Keys.ToArray();

                for (int ii = 0; ; )
                {
                    IEnumerator<string> v = _parameters[keys[ii]].GetEnumerator();
                    v.MoveNext();
                    while (true)
                    {
                        queryoptions.Append(keys[ii] +
                        "=" + v.Current);
                        if (v.MoveNext())
                            queryoptions.Append("&");
                        else
                            break;
                    }
                    if (++ii == keys.Length)
                        break;
                    queryoptions.Append("&");
                }
            }
            return queryoptions.ToString(); 
        }

        internal abstract string GetResponse();
    }

    internal class TestURIWriter<T> : TestURIWriter
    {
        internal TestURIWriter(string uri, Dictionary<string, List<string>> parameters) 
        {
            _uri = uri + "/";
            _parameters = parameters == null ? new Dictionary<string, List<string>>() : parameters;
        }

        internal TestURIWriter<S> NavigateToCollection<S>(Expression<Func<T, ICollection<S>>> selector, bool isLeaf)
        {
            return Navigate<S>(selector as LambdaExpression, isLeaf);
        }

        internal TestURIWriter<S> Navigate<S>(Expression<Func<T, S>> selector)
        {
            return Navigate<S>(selector as LambdaExpression, false);
        }

        private TestURIWriter<S> Navigate<S>(LambdaExpression le, bool isCollectionLeaf)
        {
            MemberExpression me = le.Body as MemberExpression;

            string newUri = _uri + me.Member.Name;
            return new TestURIWriter<S>(newUri, this.CloneParameters());
        }

        internal Dictionary<string, List<string>> CloneParameters()
        {
            return new Dictionary<string, List<string>>(_parameters);
        }

        internal TestURIWriter<T> OrderBy<TKey>(Expression<Func<T, TKey>> selector, bool descending)
        {
            var parameters = this.CloneParameters();
            string orderby = "";

            LambdaExpression le = selector as LambdaExpression;
            MemberExpression me = le.Body as MemberExpression;
            orderby = orderby + me.Member.Name;
            if (descending)
            {
                orderby = orderby + " desc";
            }

            AddParameter(parameters, "$orderby", orderby);
            return new TestURIWriter<T>(_uri, parameters);
        }

        internal TestURIWriter<S> Project<S>(Expression<Func<T, S>> selector)
        {
            var parameters = this.CloneParameters();

            //TODO: add $ back in when server projections working.
            AddParameter(parameters, "select", "");
            return new TestURIWriter<S>(_uri, parameters);
        }

        internal void AddParameter(Dictionary<string, List<string>> p, string name, string value)
        {
            if (p.ContainsKey(name))
            {
                p[name].Add(value);
            }
            else
            {
                p.Add(name, new List<string>(new string[] {value}));
            }
        }

        internal TestURIWriter<T> Take(int numberElements)
        {
            var parameters = this.CloneParameters();
            AddParameter(parameters, "$top", numberElements.ToString());
            return new TestURIWriter<T>(_uri, parameters);
        }

        internal TestURIWriter<T> Skip(int numberElements)
        {
            var parameters = this.CloneParameters();
            AddParameter(parameters, "$skip", numberElements.ToString());
            return new TestURIWriter<T>(_uri, parameters);
        }

        internal TestURIWriter<T> AddParam<K>(string name, K value)
        {
            var parameters = this.CloneParameters();
            AddParameter(parameters, name,  value.ToString());
            return new TestURIWriter<T>(_uri, parameters);
        }

        internal override String GetResponse()
        {
            var w = WebRequest.Create(this.GetUri());
            WebResponse r = w.GetResponse();
            return new StreamReader(r.GetResponseStream()).ReadToEnd();
        }  
    }

    internal static class URIWriterExtensions
    {
        internal static TestURIWriter<T> FindByKey<T>(this TestURIWriter<T> source, string keyName, object key)
        {
            string keyValue = key == null ? "null" : key.ToString(); 
            string newUri = source.GetRawUri().TrimEnd('/') + "(" + keyValue + ")"; 
            Type t = typeof(TestURIWriter);

            MethodInfo mi = t.GetMethod("CreateURIWriter",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null,
                    new Type[] { typeof(string), typeof(Dictionary<string, List<string>>) }, null);

            mi = mi.MakeGenericMethod(new Type[] { typeof(T) });

            TestURIWriter<T> w = (TestURIWriter<T>)mi.Invoke(null, new object[] { newUri, source.CloneParameters() });
            return w;
        }
    }
}
